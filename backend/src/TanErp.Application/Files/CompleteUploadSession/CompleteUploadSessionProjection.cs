namespace TanErp.Application.Files.CompleteUploadSession;

public sealed record CompletedFileProjection(
    Guid FileId,
    string Filename,
    string MediaType,
    long FileSizeBytes,
    string ServingUrl);

public sealed record CompleteUploadSessionProjection(
    Guid SessionId,
    IReadOnlyList<CompletedFileProjection> Files);
