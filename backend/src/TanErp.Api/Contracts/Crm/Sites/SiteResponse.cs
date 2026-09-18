namespace TanErp.Api.Contracts.Crm.Sites;

public sealed record SiteImageResponse(
    Guid Id,
    Guid FileId,
    string? Caption,
    int DisplayOrder,
    DateTimeOffset CreatedAtUtc);

public sealed record SiteSummaryResponse(
    Guid Id,
    string Label,
    string AddressLine1,
    string? Subdistrict = null,
    string? District = null,
    string? Province = null,
    string? PostalCode = null);

public sealed record SiteResponse(
    Guid Id,
    Guid CustomerId,
    string Code,
    string Label,
    string AddressLine1,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode,
    string CountryCode,
    decimal? Latitude,
    decimal? Longitude,
    string? AccessNote,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<SiteImageResponse>? Images = null);

public sealed record SiteListResponse(
    IReadOnlyList<SiteResponse> Items);
