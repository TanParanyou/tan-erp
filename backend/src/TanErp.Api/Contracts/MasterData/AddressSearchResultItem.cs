namespace TanErp.Api.Contracts.MasterData;

public record AddressSearchResultItem(
    string SubdistrictCode,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode,
    string CountryCode,
    decimal? Latitude,
    decimal? Longitude,
    string DisplayText);
