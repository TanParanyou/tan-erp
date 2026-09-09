namespace TanErp.Application.Crm.Sites;

public sealed record SiteProjection(
    Guid Id,
    string Code,
    Guid CustomerId,
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
    DateTimeOffset CreatedAtUtc);
