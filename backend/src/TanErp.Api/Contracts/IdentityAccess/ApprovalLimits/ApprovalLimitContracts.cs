using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.ApprovalLimits;

namespace TanErp.Api.Contracts.IdentityAccess.ApprovalLimits;

public record ApprovalLimitResponse(
    Guid Id, Guid RowVersion, AdminRefResponse Role, string DocumentType, AdminRefResponse? Branch, string Currency,
    bool IsUnlimited, decimal? MaxAmount, bool IsActive, DateTimeOffset CreatedAtUtc, DateTimeOffset UpdatedAtUtc)
{
    public static ApprovalLimitResponse From(ApprovalLimitView v) => new(
        v.Id, v.RowVersion, new AdminRefResponse(v.Role.Id, v.Role.Name), v.DocumentType,
        v.Branch is null ? null : new AdminRefResponse(v.Branch.Id, v.Branch.Name), v.Currency,
        v.IsUnlimited, v.MaxAmount, v.IsActive, v.CreatedAtUtc, v.UpdatedAtUtc);
}

public record ApprovalLimitListResponse(IReadOnlyList<ApprovalLimitResponse> Items, AdminPaginationResponse Pagination)
{
    public static ApprovalLimitListResponse From(ApprovalLimitPage page) => new(
        page.Items.Select(ApprovalLimitResponse.From).ToList(),
        new AdminPaginationResponse(page.Page, page.PageSize, page.TotalCount, (int)Math.Ceiling(page.TotalCount / (double)page.PageSize)));
}

public record ApprovalLimitOptionsResponse(
    IReadOnlyList<AdminRefResponse> Roles, IReadOnlyList<AdminRefResponse> Branches, IReadOnlyList<string> DocumentTypes, IReadOnlyList<string> Currencies)
{
    public static ApprovalLimitOptionsResponse From(ApprovalLimitOptions o) => new(
        o.Roles.Select(r => new AdminRefResponse(r.Id, r.Name)).ToList(),
        o.Branches.Select(b => new AdminRefResponse(b.Id, b.Name)).ToList(),
        o.DocumentTypes, o.Currencies);
}

public record CreateApprovalLimitRequest(Guid? RoleId, string? DocumentType, Guid? BranchId, string? Currency, bool? IsUnlimited, decimal? MaxAmount)
{
    public ApprovalLimitInput ToInput() => new(RoleId ?? Guid.Empty, DocumentType, BranchId, Currency, IsUnlimited, MaxAmount);
}

public record UpdateApprovalLimitRequest(bool? IsUnlimited, decimal? MaxAmount, bool? IsActive)
{
    public ApprovalLimitUpdateInput ToInput() => new(IsUnlimited, MaxAmount, IsActive);
}
