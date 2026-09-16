namespace TanErp.Api.Contracts.Files;

/// <summary>
/// Represents one file slot when creating an upload session.
/// </summary>
public sealed record FileSlotRequest(
    string Filename,
    string MediaType,
    long FileSizeBytes);

/// <summary>
/// Request body for POST /api/v1/files/sessions
/// </summary>
public sealed record CreateUploadSessionRequest(
    IReadOnlyList<FileSlotRequest> Files);
