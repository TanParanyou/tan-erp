namespace TanErp.Application.Files.CompleteUploadSession;

public sealed record FileCompletionInput(
    string SlotId,
    string OriginalFilename,
    string MediaType,
    long FileSizeBytes,
    Stream Content);

public sealed record CompleteUploadSessionCommand(
    string FirebaseUid,
    Guid MembershipId,
    string SessionId,
    IReadOnlyList<FileCompletionInput> Files,
    string TraceId);
