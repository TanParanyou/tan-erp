namespace TanErp.Api.Contracts.Items;

public sealed record PreviewItemImportRequest(string Content);

public sealed record CommitItemImportRequest(string Content, string? ExpectedContentSha256);

public sealed record ItemImportRowErrorResponse(string Field, string Code);

public sealed record ItemImportRowResponse(
    int RowNumber,
    string? Code,
    string? NameTh,
    bool IsValid,
    IReadOnlyList<ItemImportRowErrorResponse> Errors);

public sealed record ItemImportPreviewResponse(
    string ContentSha256,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<ItemImportRowResponse> Rows);

public sealed record ItemImportCommitResponse(
    Guid BatchId,
    int CreatedCount,
    string ContentSha256,
    bool Replayed);
