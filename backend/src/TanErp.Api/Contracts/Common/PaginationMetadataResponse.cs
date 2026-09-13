namespace TanErp.Api.Contracts.Common;

public sealed record PaginationMetadataResponse(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    string? NextCursor = null);
