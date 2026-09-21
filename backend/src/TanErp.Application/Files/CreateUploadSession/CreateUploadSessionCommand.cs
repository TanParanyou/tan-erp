namespace TanErp.Application.Files.CreateUploadSession;

/// <summary>
/// Represents one file slot in an upload session request.
/// </summary>
public sealed record FileSlotInput(
    string Filename,
    string MediaType,
    long FileSizeBytes);

public sealed record CreateUploadSessionCommand(
    string FirebaseUid,
    Guid MembershipId,
    string ParentType,
    Guid? ParentId,
    Guid? CreationIntentId,
    IReadOnlyList<FileSlotInput> Files,
    string IdempotencyKey,
    string TraceId);
