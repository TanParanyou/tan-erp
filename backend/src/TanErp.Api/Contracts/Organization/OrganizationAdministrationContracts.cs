using TanErp.Application.Organization.Administration;

namespace TanErp.Api.Contracts.Organization;

public sealed record UpdateOrganizationProfileRequest(
    string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone);

public sealed record OrganizationProfileResponse(
    Guid Id, string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone, Guid RowVersion)
{
    public static OrganizationProfileResponse From(OrganizationProfile p) =>
        new(p.Id, p.Name, p.NameEn, p.TaxIdentifier, p.AddressTh, p.AddressEn, p.Phone, p.RowVersion);
}
