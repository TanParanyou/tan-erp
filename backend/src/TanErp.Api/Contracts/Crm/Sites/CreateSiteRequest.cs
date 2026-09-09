namespace TanErp.Api.Contracts.Crm.Sites;

public sealed record CreateSiteRequest(
    string Label,
    string AddressLine1,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode,
    string CountryCode,
    decimal? Latitude = null,
    decimal? Longitude = null,
    string? AccessNote = null);
