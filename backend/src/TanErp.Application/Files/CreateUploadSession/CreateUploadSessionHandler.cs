using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Files;

namespace TanErp.Application.Files.CreateUploadSession;

/// <summary>
/// Validates the upload session request, enforces parent binding and user permissions,
/// and persists a new FileUploadSession with its slots.
/// </summary>
public class CreateUploadSessionHandler
{
    private const int MaxFilesPerSession = 20;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    private const long MaxSessionSizeBytes = 100 * 1024 * 1024; // 100 MB

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IFileParentAccessResolver _parentAccessResolver;
    private readonly IFileStore _fileStore;

    public CreateUploadSessionHandler(
        IRequestAccessResolver accessResolver,
        IFileParentAccessResolver parentAccessResolver,
        IFileStore fileStore)
    {
        _accessResolver = accessResolver;
        _parentAccessResolver = parentAccessResolver;
        _fileStore = fileStore;
    }

    public async Task<Result<UploadSessionProjection>> Handle(
        CreateUploadSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate parentType
        if (!FileParentTypes.IsValid(command.ParentType))
        {
            return Result<UploadSessionProjection>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID",
                    $"Parent type '{command.ParentType}' is invalid. Supported types: opportunity, customer, site."));
        }

        // 2. Validate parentId vs creationIntentId
        if (command.ParentId.HasValue && command.CreationIntentId.HasValue)
        {
            return Result<UploadSessionProjection>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", "Cannot specify both parentId and creationIntentId."));
        }

        if (!command.ParentId.HasValue && !command.CreationIntentId.HasValue)
        {
            return Result<UploadSessionProjection>.Failure(
                new Error("FILE_PARENT_TYPE_INVALID", "Exactly one of parentId or creationIntentId must be provided."));
        }

        // 3. Resolve user membership and basic access
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
            return Result<UploadSessionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 4. Resolve parent scope and permission
        var parentResult = await _parentAccessResolver.ResolveAsync(
            access,
            command.ParentType,
            command.ParentId,
            command.CreationIntentId,
            FileAccessOperation.Upload,
            cancellationToken);

        if (parentResult.IsFailure)
        {
            return Result<UploadSessionProjection>.Failure(parentResult.Error);
        }

        // 5. Validate file count
        if (command.Files.Count == 0 || command.Files.Count > MaxFilesPerSession)
        {
            return Result<UploadSessionProjection>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID",
                    $"File count must be between 1 and {MaxFilesPerSession}."));
        }

        // 6. Validate each file slot and total size
        long totalSessionBytes = 0;
        foreach (var slot in command.Files)
        {
            if (string.IsNullOrWhiteSpace(slot.Filename))
            {
                return Result<UploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID", "Filename is required for each file slot."));
            }

            if (!AllowedMediaTypes.IsAllowed(slot.MediaType))
            {
                return Result<UploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"Media type '{slot.MediaType}' is not allowed. Accepted: image/webp, image/jpeg, image/png."));
            }

            if (slot.FileSizeBytes <= 0 || slot.FileSizeBytes > MaxFileSizeBytes)
            {
                return Result<UploadSessionProjection>.Failure(
                    new Error("FILE_UPLOAD_SESSION_INVALID",
                        $"File size must be between 1 byte and {MaxFileSizeBytes / (1024 * 1024)} MB."));
            }

            totalSessionBytes += slot.FileSizeBytes;
        }

        if (totalSessionBytes > MaxSessionSizeBytes)
        {
            return Result<UploadSessionProjection>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID",
                    $"Total session size exceeds the maximum limit of {MaxSessionSizeBytes / (1024 * 1024)} MB."));
        }

        // 7. Compute Idempotency hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var payloadCanonical = $"{command.ParentType.ToLowerInvariant()}:{command.ParentId}:{command.CreationIntentId}:" +
            string.Join(";", command.Files.Select(f => $"{f.Filename}:{f.MediaType.ToLowerInvariant()}:{f.FileSizeBytes}"));
        var payloadHash = Sha256Hex.Compute(payloadCanonical);

        // 8. Persist or replay session
        var sessionResult = await _fileStore.CreateSessionAsync(
            access,
            command.ParentType,
            command.ParentId,
            command.CreationIntentId,
            command.Files,
            keyHash,
            payloadHash,
            cancellationToken);

        if (sessionResult.IsFailure)
        {
            return Result<UploadSessionProjection>.Failure(sessionResult.Error);
        }

        var session = sessionResult.Value!;
        var slots = session.Slots
            .OrderBy(s => s.SlotIndex)
            .Select(s => new UploadSlotProjection(s.Id, s.Filename, s.MediaType, s.FileSizeBytes))
            .ToList();

        return Result<UploadSessionProjection>.Success(
            new UploadSessionProjection(session.Id, session.ExpiresAtUtc, slots));
    }
}
