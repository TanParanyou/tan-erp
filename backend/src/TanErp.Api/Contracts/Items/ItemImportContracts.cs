namespace TanErp.Api.Contracts.Items;

public sealed record PreviewItemImportRequest(string Content, string? Kind = null, string? Mode = null);

public sealed record CommitItemImportRequest(string Content, string? ExpectedContentSha256, string? Kind = null, string? Mode = null);

public sealed record ItemImportRowErrorResponse(string Field, string Code);

public sealed record ItemImportRowResponse(
    int RowNumber,
    string? Code,
    string? NameTh,
    bool IsValid,
    IReadOnlyList<ItemImportRowErrorResponse> Errors,
    string? Outcome = null);

public sealed record ItemImportPreviewResponse(
    string ContentSha256,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<ItemImportRowResponse> Rows,
    int CreateCount = 0,
    int UpdateCount = 0,
    int UnchangedCount = 0);

public sealed record ItemImportCommitResponse(
    Guid BatchId,
    int CreatedCount,
    string ContentSha256,
    bool Replayed,
    int UpdatedCount = 0,
    int UnchangedCount = 0);

public sealed record CreateItemImportJobRequest(string Kind, string Mode, string Content, string? ExpectedContentSha256);

public sealed record ItemImportJobRowResponse(
    int RowNumber,
    string? Code,
    string? NameTh,
    IReadOnlyList<ItemImportRowErrorResponse> Errors);

public sealed record ItemImportJobResponse(
    Guid Id,
    string Kind,
    string Mode,
    string Status,
    string ContentSha256,
    int TotalRows,
    int ProcessedRows,
    int BatchSize,
    int TotalBatches,
    int CompletedBatches,
    int RejectedBatches,
    int CreatedCount,
    int UpdatedCount,
    int UnchangedCount,
    int ErrorRowCount,
    bool ErrorsTruncated,
    IReadOnlyList<ItemImportJobRowResponse> Errors,
    string? FailureCode,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    bool Replayed);
