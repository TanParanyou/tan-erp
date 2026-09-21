namespace TanErp.Api.Contracts.Files;

/// <summary>
/// Represents one file slot when creating an upload session.
/// </summary>
public sealed record FileSlotRequest(
    string Filename,
    string MediaType,
    long FileSizeBytes);

/// <summary>
/// Request body for POST /api/v1/files/upload-sessions
/// </summary>
public sealed record CreateUploadSessionRequest(
    string ParentType,
    Guid? ParentId,
    Guid? CreationIntentId,
    IReadOnlyList<FileSlotRequest> Files);

/// <summary>
/// Response for an upload slot in a session.
/// </summary>
public sealed record UploadSlotResponse(
    Guid SlotId,
    string Filename,
    string MediaType,
    long FileSizeBytes);

/// <summary>
/// Response for POST /api/v1/files/upload-sessions
/// </summary>
public sealed record CreateUploadSessionResponse(
    Guid SessionId,
    DateTimeOffset ExpiresAtUtc,
    IReadOnlyList<UploadSlotResponse> Slots);

/// <summary>
/// Metadata of a successfully verified and stored file.
/// </summary>
public sealed record CompletedFileResponse(
    Guid FileId,
    string Filename,
    string MediaType,
    long FileSizeBytes,
    string ServingUrl);

/// <summary>
/// Response body for POST /api/v1/files/upload-sessions/{sessionId}/complete
/// </summary>
public sealed record CompleteUploadSessionResponse(
    Guid SessionId,
    IReadOnlyList<CompletedFileResponse> Files);
