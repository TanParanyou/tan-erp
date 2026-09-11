namespace TanErp.Application.MasterData.Addresses.SearchAddresses;

public record SearchAddressItem(
    string SubdistrictCode,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode,
    string CountryCode,
    decimal? Latitude,
    decimal? Longitude,
    string DisplayText);
