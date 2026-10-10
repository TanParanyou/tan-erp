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

public sealed record CreateBranchRequest(
    string Code, string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

/// <summary>No Code property by design: the branch code is immutable after creation.</summary>
public sealed record UpdateBranchRequest(
    string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

public sealed record BranchResponse(
    Guid Id, string Code, string Name, string? NameEn, string? TaxBranchCode, string? AddressTh, string? AddressEn,
    string? Phone, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc)
{
    public static BranchResponse From(BranchDetail b) => new(
        b.Id, b.Code, b.Name, b.NameEn, b.TaxBranchCode, b.AddressTh, b.AddressEn, b.Phone, b.IsActive, b.RowVersion, b.CreatedAtUtc);
}

public sealed record DeactivateBranchRequest(string? Reason);

public sealed record BranchBlockerResponse(string Type, int Count);

public sealed record BranchDeactivationCheckResponse(bool CanDeactivate, IReadOnlyList<BranchBlockerResponse> Blockers)
{
    public static BranchDeactivationCheckResponse From(BranchDeactivationCheck c) =>
        new(c.CanDeactivate, c.Blockers.Select(b => new BranchBlockerResponse(b.Type, b.Count)).ToArray());
}
