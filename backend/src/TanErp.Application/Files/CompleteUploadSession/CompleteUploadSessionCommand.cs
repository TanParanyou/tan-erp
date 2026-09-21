namespace TanErp.Application.Files.CompleteUploadSession;

/// <summary>
/// Represents one file being submitted for completion and verification.
/// </summary>
public sealed record FileCompletionInput(
    Guid SlotId,
    string OriginalFilename,
    string MediaType,
    long FileSizeBytes,
    Stream Content);

public sealed record CompleteUploadSessionCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid SessionId,
    IReadOnlyList<FileCompletionInput> Files,
    string TraceId);
