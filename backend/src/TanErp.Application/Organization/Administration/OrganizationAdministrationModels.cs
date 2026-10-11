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

public sealed record BranchDetail(
    Guid Id, string Code, string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn,
    string? Phone, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc);

public sealed record BranchInput(
    string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

public sealed record CreateBranchInput(string Code, BranchInput Details);

public enum BranchStatusFilter { All, Active, Inactive }

public sealed record BranchBlocker(string Type, int Count);

public sealed record BranchDeactivationCheck(bool CanDeactivate, IReadOnlyList<BranchBlocker> Blockers);

/// <summary>Blocker type names are part of the API contract and of the FE i18n keys.</summary>
public static class BranchBlockerTypes
{
    public const string Estimates = "estimates";
    public const string Quotations = "quotations";
    public const string PurchaseOrders = "purchase_orders";
    public const string Billings = "billings";
    public const string WorkOrders = "work_orders";
    public const string Projects = "projects";
    public const string Installations = "installations";
    public const string ServiceRequests = "service_requests";
    public const string SiteSurveys = "site_surveys";
    public const string Opportunities = "opportunities";
    public const string QuickEstimates = "quick_estimates";
    public const string MrpRuns = "mrp_runs";
    public const string Warehouses = "warehouses";
    public const string Memberships = "memberships";
    public const string LastActiveBranch = "last_active_branch";
}
