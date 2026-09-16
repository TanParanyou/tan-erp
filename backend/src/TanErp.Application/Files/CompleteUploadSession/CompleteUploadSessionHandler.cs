using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Files;

namespace TanErp.Application.Files.CompleteUploadSession;

/// <summary>
/// Verifies magic numbers of each uploaded file, persists binary via IFileStorageProvider,
/// and records metadata in the file store.
/// Server-side security gate — all validation here is authoritative.
/// </summary>
public class CompleteUploadSessionHandler
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB post-compression limit

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IFileStorageProvider _storageProvider;
    private readonly IFileStore _fileStore;
    private readonly IClock _clock;

    public CompleteUploadSessionHandler(
        IRequestAccessResolver accessResolver,
        IFileStorageProvider storageProvider,
        IFileStore fileStore,
        IClock clock)
    {
        _accessResolver = accessResolver;
        _storageProvider = storageProvider;
        _fileStore = fileStore;
        _clock = clock;
    }

    public async Task<Result<CompleteUploadSessionProjection>> Handle(
        CompleteUploadSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<CompleteUploadSessionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (command.Files.Count == 0)
        {
            return Result<CompleteUploadSessionProjection>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID", "At least one file is required."));
        }

        var completed = new List<CompletedFileProjection>();
        var savedPaths = new List<string>();

        try
        {
            foreach (var fileInput in command.Files)
            {
                // 2. Read first bytes for magic number verification
                var headerBuffer = new byte[12];
                var bytesRead = await fileInput.Content.ReadAsync(headerBuffer, cancellationToken);

                if (bytesRead < 4)
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_UPLOAD_SESSION_INVALID", $"File '{fileInput.OriginalFilename}' is too small to be a valid image."),
                        cancellationToken);
                }

                // 3. Server-side magic number check
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

                // 4. Size check against declared size (sanity guard)
                if (fileInput.FileSizeBytes > MaxFileSizeBytes)
                {
                    return await RollbackAndFail(savedPaths,
                        new Error("FILE_UPLOAD_SESSION_INVALID",
                            $"File '{fileInput.OriginalFilename}' exceeds the maximum allowed size of 10 MB."),
                        cancellationToken);
                }

                // 5. Reconstruct the full stream (prepend header bytes we already read)
                var fullStream = new PrependedStream(headerBuffer[..bytesRead], fileInput.Content);

                // 6. Persist binary
                var storagePath = await _storageProvider.SaveAsync(
                    access.OrganizationId,
                    command.SessionId,
                    fileInput.OriginalFilename,
                    fullStream,
                    cancellationToken);

                savedPaths.Add(storagePath);

                // 7. Persist metadata
                var fileId = Guid.NewGuid();
                var uploadedFile = new UploadedFile(
                    fileId,
                    access.OrganizationId,
                    storagePath,
                    fileInput.OriginalFilename,
                    detectedType,
                    fileInput.FileSizeBytes,
                    command.SessionId,
                    access.ActorUserId,
                    _clock.UtcNow);

                await _fileStore.SaveAsync(uploadedFile, cancellationToken);

                completed.Add(new CompletedFileProjection(
                    fileId,
                    fileInput.OriginalFilename,
                    detectedType,
                    fileInput.FileSizeBytes,
                    _storageProvider.GetServingUrl(storagePath)));
            }
        }
        catch
        {
            // Roll back any already-saved files on unexpected error
            await RollbackFilesAsync(savedPaths, cancellationToken);
            throw;
        }

        return Result<CompleteUploadSessionProjection>.Success(
            new CompleteUploadSessionProjection(command.SessionId, completed));
    }

    private async Task<Result<CompleteUploadSessionProjection>> RollbackAndFail(
        IReadOnlyList<string> savedPaths,
        Error error,
        CancellationToken cancellationToken)
    {
        await RollbackFilesAsync(savedPaths, cancellationToken);
        return Result<CompleteUploadSessionProjection>.Failure(error);
    }

    private async Task RollbackFilesAsync(IEnumerable<string> paths, CancellationToken cancellationToken)
    {
        foreach (var path in paths)
        {
            try
            {
                await _storageProvider.DeleteAsync(path, cancellationToken);
            }
            catch
            {
                // Best-effort rollback — log elsewhere, do not rethrow
            }
        }
    }

    /// <summary>
    /// Detects media type from magic number bytes.
    /// JPEG: FF D8 FF
    /// PNG:  89 50 4E 47 0D 0A 1A 0A
    /// WebP: 52 49 46 46 ?? ?? ?? ?? 57 45 42 50
    /// </summary>
    private static string? DetectMediaType(byte[] header)
    {
        if (header.Length >= 3 &&
            header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return AllowedMediaTypes.Jpeg;
        }

        if (header.Length >= 8 &&
            header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
            header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
        {
            return AllowedMediaTypes.Png;
        }

        if (header.Length >= 12 &&
            header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
            header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
        {
            return AllowedMediaTypes.WebP;
        }

        return null;
    }
}

/// <summary>
/// Reconstructs a stream by prepending already-read header bytes before the remaining content.
/// Used after peeking magic number bytes from an upload stream.
/// </summary>
internal sealed class PrependedStream : Stream
{
    private readonly byte[] _prefix;
    private int _prefixOffset;
    private readonly Stream _inner;

    public PrependedStream(byte[] prefix, Stream inner)
    {
        _prefix = prefix;
        _inner = inner;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var prefixRemaining = _prefix.Length - _prefixOffset;
        if (prefixRemaining > 0)
        {
            var prefixRead = Math.Min(prefixRemaining, count);
            Array.Copy(_prefix, _prefixOffset, buffer, offset, prefixRead);
            _prefixOffset += prefixRead;
            return prefixRead;
        }
        return _inner.Read(buffer, offset, count);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var prefixRemaining = _prefix.Length - _prefixOffset;
        if (prefixRemaining > 0)
        {
            var prefixRead = Math.Min(prefixRemaining, count);
            Array.Copy(_prefix, _prefixOffset, buffer, offset, prefixRead);
            _prefixOffset += prefixRead;
            return prefixRead;
        }
        return await _inner.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);
    }

    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
