namespace TanErp.Application.Files.CreateUploadSession;

/// <summary>
/// Represents a single upload slot returned to the client.
/// The client uploads one file per slot using the provided upload URL.
/// </summary>
public sealed record UploadSlotProjection(
    string SlotId,
    string UploadUrl);

public sealed record UploadSessionProjection(
    string SessionId,
    IReadOnlyList<UploadSlotProjection> Slots);
