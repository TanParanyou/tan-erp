namespace TanErp.Api.Contracts.Files;

public sealed record UploadSlotResponse(
    string SlotId,
    string UploadUrl);

public sealed record CreateUploadSessionResponse(
    string SessionId,
    IReadOnlyList<UploadSlotResponse> Slots);

public sealed record CompletedFileResponse(
    Guid FileId,
    string Filename,
    string MediaType,
    long FileSizeBytes,
    string ServingUrl);

public sealed record CompleteUploadSessionResponse(
    string SessionId,
    IReadOnlyList<CompletedFileResponse> Files);
