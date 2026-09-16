using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Files;

namespace TanErp.Application.Files.CreateUploadSession;

/// <summary>
/// Validates the upload session request and returns a list of pre-authorized upload slots.
/// The actual binary is NOT touched here — the client uploads directly to each slot URL,
/// then calls CompleteUploadSessionHandler to verify and persist metadata.
/// </summary>
public class CreateUploadSessionHandler
{
    private const int MaxFilesPerSession = 20;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IRequestAccessResolver _accessResolver;

    public CreateUploadSessionHandler(IRequestAccessResolver accessResolver)
    {
        _accessResolver = accessResolver;
    }

    public async Task<Result<UploadSessionProjection>> Handle(
        CreateUploadSessionCommand command,
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
            return Result<UploadSessionProjection>.Failure(accessResult.Error);
        }

        // 2. Validate file count
        if (command.Files.Count == 0 || command.Files.Count > MaxFilesPerSession)
        {
            return Result<UploadSessionProjection>.Failure(
                new Error("FILE_UPLOAD_SESSION_INVALID",
                    $"File count must be between 1 and {MaxFilesPerSession}."));
        }

        // 3. Validate each file slot
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
        }

        // 4. Generate a deterministic session ID from the idempotency key
        var sessionId = command.IdempotencyKey;

        // 5. Build upload slots — each slot gets a pre-authorized upload URL
        var access = accessResult.Value!;
        var slots = command.Files
            .Select((f, i) => new UploadSlotProjection(
                SlotId: $"{sessionId}_{i}",
                UploadUrl: $"/api/v1/files/sessions/{sessionId}/slots/{i}"))
            .ToList();

        return Result<UploadSessionProjection>.Success(
            new UploadSessionProjection(sessionId, slots));
    }
}
