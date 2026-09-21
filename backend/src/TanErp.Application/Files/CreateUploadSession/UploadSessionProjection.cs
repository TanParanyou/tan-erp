namespace TanErp.Application.Files.CreateUploadSession;

public sealed record UploadSlotProjection(
    Guid SlotId,
    string Filename,
    string MediaType,
    long FileSizeBytes);

public sealed record UploadSessionProjection(
    Guid SessionId,
    DateTimeOffset ExpiresAtUtc,
    IReadOnlyList<UploadSlotProjection> Slots);
