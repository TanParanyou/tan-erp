using System.Security.Cryptography;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Files;

namespace TanErp.Application.Files.CompleteUploadSession;

/// <summary>
/// Verifies slot matching, server-side magic numbers, persists binary via IFileStorageProvider,
/// and atomically transitions FileUploadSession to consumed while creating UploadedFile records.
/// </summary>
public class CompleteUploadSessionHandler
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IFileParentAccessResolver _parentAccessResolver;
    private readonly IFileStorageProvider _storageProvider;
    private readonly IFileStore _fileStore;
    private readonly IClock _clock;

    public CompleteUploadSessionHandler(
        IRequestAccessResolver accessResolver,
        IFileParentAccessResolver parentAccessResolver,
        IFileStorageProvider storageProvider,
        IFileStore fileStore,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _parentAccessResolver = parentAccessResolver;
        _storageProvider = storageProvider;
        _fileStore = fileStore;
        _clock = clock;
    }

    public async Task<Result<CompleteUploadSessionProjection>> Handle(
        CompleteUploadSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve user context
        var accessResult = await _accessResolver.ResolveAnyAsync(
            command.FirebaseUid,
            command.MembershipId,
            [
                "opportunities.update",
                "customers.update",
                "customers.create",
                "sites.update",
                "sites.create",
                "items.manage-images",
                "items.update"
            ],
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<CompleteUploadSessionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Fetch session and validate status/actor/expiry
        var sessionResult = await _fileStore.GetCompletableSessionAsync(
            access.OrganizationId,
            access.ActorUserId,
            command.SessionId,
            cancellationToken);

        if (sessionResult.IsFailure)
        {
            return Result<CompleteUploadSessionProjection>.Failure(sessionResult.Error);
        }

        var session = sessionResult.Value!;

        // 3. Re-verify parent access for the session
        var parentResult = await _parentAccessResolver.ResolveAsync(
            access,
            session.ParentType,
            session.ParentId,
            session.CreationIntentId,
            FileAccessOperation.Upload,
            cancellationToken);

        if (parentResult.IsFailure)
        {
            return Result<CompleteUploadSessionProjection>.Failure(parentResult.Error);
        }

        // 4. Validate slot counts and matching metadata
        if (command.Files.Count == 0 || command.Files.Count != session.Slots.Count)
        {
            return Result<CompleteUploadSessionProjection>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID",
                    $"File count ({command.Files.Count}) does not match session slots count ({session.Slots.Count})."));
        }

        var slotMap = session.Slots.ToDictionary(s => s.Id);
        foreach (var fileInput in command.Files)
        {
            if (!slotMap.TryGetValue(fileInput.SlotId, out var slot))
            {
                return Result<CompleteUploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"Slot ID '{fileInput.SlotId}' does not belong to this upload session."));
            }

            if (!string.Equals(fileInput.OriginalFilename.Trim(), slot.Filename.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return Result<CompleteUploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"File name '{fileInput.OriginalFilename}' does not match declared slot name '{slot.Filename}'."));
            }

            if (fileInput.FileSizeBytes != slot.FileSizeBytes)
            {
                return Result<CompleteUploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"File size ({fileInput.FileSizeBytes} bytes) does not match declared slot size ({slot.FileSizeBytes} bytes)."));
            }

            if (!string.Equals(fileInput.MediaType.Trim(), slot.MediaType.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return Result<CompleteUploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"Media type '{fileInput.MediaType}' does not match declared slot type '{slot.MediaType}'."));
            }
        }

        var completed = new List<CompletedFileProjection>();
        var uploadedFiles = new List<UploadedFile>();
        var savedPaths = new List<string>();

        try
        {
            foreach (var fileInput in command.Files)
            {
                // 5. Read first bytes for magic number verification
                var headerBuffer = new byte[12];
                var bytesRead = await fileInput.Content.ReadAsync(headerBuffer, cancellationToken);

                if (bytesRead < 4)
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_UPLOAD_SESSION_INVALID", $"File '{fileInput.OriginalFilename}' is too small to be a valid image."),
                        cancellationToken);
                }

                // 6. Server-side magic number check
                var detectedType = DetectMediaType(headerBuffer);
                if (detectedType is null)
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_UPLOAD_SESSION_INVALID",
                            $"File '{fileInput.OriginalFilename}' does not match an allowed image format (WebP, JPEG, PNG)."),
                        cancellationToken);
                }

                if (!AllowedMediaTypes.IsAllowed(detectedType))
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_UPLOAD_SESSION_INVALID",
                            $"Detected media type '{detectedType}' is not allowed."),
                        cancellationToken);
                }

                // 7. Bound, verify and hash the actual stream before storage
                await using var verified = new MemoryStream(capacity: checked((int)Math.Min(fileInput.FileSizeBytes, MaxFileSizeBytes)));
                await verified.WriteAsync(headerBuffer.AsMemory(0, bytesRead), cancellationToken);
                await CopyWithLimitAsync(fileInput.Content, verified, MaxFileSizeBytes + 1 - bytesRead, cancellationToken);

                if (verified.Length > MaxFileSizeBytes)
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_TOO_LARGE", $"File '{fileInput.OriginalFilename}' exceeds the maximum allowed size of 10 MB."),
                        cancellationToken);
                }

                if (verified.Length != fileInput.FileSizeBytes)
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_SIZE_MISMATCH", $"Uploaded byte count ({verified.Length}) does not match the declared size ({fileInput.FileSizeBytes})."),
                        cancellationToken);
                }

                var sha256 = Convert.ToHexString(SHA256.HashData(verified.ToArray())).ToLowerInvariant();
                verified.Position = 0;

                // 8. Persist verified binary
                var storagePath = await _storageProvider.SaveAsync(
                    access.OrganizationId,
                    command.SessionId.ToString(),
                    fileInput.OriginalFilename,
                    verified,
                    cancellationToken);

                savedPaths.Add(storagePath);

                // 9. Prepare metadata with SHA-256 and content_verified status
                var fileId = Guid.NewGuid();
                var uploadedFile = new UploadedFile(
                    fileId,
                    access.OrganizationId,
                    storagePath,
                    fileInput.OriginalFilename,
                    detectedType,
                    verified.Length,
                    command.SessionId.ToString(),
                    access.ActorUserId,
                    _clock.UtcNow,
                    contentSha256: sha256,
                    scanStatus: FileScanStatus.ContentVerified,
                    verifiedAtUtc: _clock.UtcNow);

                uploadedFiles.Add(uploadedFile);

                completed.Add(new CompletedFileProjection(
                    fileId,
                    fileInput.OriginalFilename,
                    detectedType,
                    verified.Length,
                    $"/api/v1/files/{fileId}/content"));
            }

            // 11. Atomically consume session and persist uploaded file records
            var completeResult = await _fileStore.CompleteSessionAsync(
                access.OrganizationId,
                access.ActorUserId,
                command.SessionId,
                uploadedFiles,
                cancellationToken);

            if (completeResult.IsFailure)
            {
                return await RollbackAndFail(savedPaths, completeResult.Error, cancellationToken);
            }

            return Result<CompleteUploadSessionProjection>.Success(
                new CompleteUploadSessionProjection(command.SessionId, completed));
        }
        catch (Exception)
        {
            foreach (var path in savedPaths)
            {
                await _storageProvider.DeleteAsync(path, CancellationToken.None);
            }
            throw;
        }
    }

    private async Task<Result<CompleteUploadSessionProjection>> RollbackAndFail(
        List<string> savedPaths,
        Error error,
        CancellationToken cancellationToken)
    {
        foreach (var path in savedPaths)
        {
            await _storageProvider.DeleteAsync(path, cancellationToken);
        }
        return Result<CompleteUploadSessionProjection>.Failure(error);
    }

    private static string? DetectMediaType(ReadOnlySpan<byte> header)
    {
        // JPEG: FF D8 FF
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return "image/jpeg";

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (header.Length >= 8 &&
            header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
            header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
            return "image/png";

        // WebP: RIFF ???? WEBP
        if (header.Length >= 12 &&
            header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
            header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
            return "image/webp";

        return null;
    }

    private static async Task CopyWithLimitAsync(Stream source, Stream destination, long maxAdditionalBytes, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long totalRead = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, maxAdditionalBytes - totalRead + 1), cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            totalRead += read;
            if (totalRead > maxAdditionalBytes)
            {
                break;
            }
        }
    }
}
