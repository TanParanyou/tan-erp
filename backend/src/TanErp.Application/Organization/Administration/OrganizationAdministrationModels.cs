namespace TanErp.Application.Organization.Administration;

public static class OrganizationAdminPermissions
{
    public const string OrganizationsRead = "organizations.read";
    public const string OrganizationsManage = "organizations.manage";
    public const string BranchesManage = "branches.manage";
}

public sealed record OrganizationProfile(
    Guid Id, string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone, Guid RowVersion);

public sealed record OrganizationProfileInput(
    string Name, string? NameEn, string? TaxIdentifier, string? AddressTh, string? AddressEn, string? Phone);
