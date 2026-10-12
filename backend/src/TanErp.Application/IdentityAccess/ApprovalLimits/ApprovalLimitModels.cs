using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.IdentityAccess.ApprovalLimits;

public static class ApprovalLimitPermissions
{
    public const string Manage = "approval-limits.manage";
}

/// <summary>Nullable flags let the handler reject a missing field instead of silently defaulting it.</summary>
public sealed record ApprovalLimitInput(
    Guid RoleId, string? DocumentType, Guid? BranchId, string? Currency, bool? IsUnlimited, decimal? MaxAmount);

public sealed record ApprovalLimitUpdateInput(bool? IsUnlimited, decimal? MaxAmount, bool? IsActive);

public sealed record ApprovalLimitListQuery(Guid? RoleId, string? DocumentType, string? Status, int Page, int Limit);

public sealed record ApprovalLimitView(
    Guid Id, Guid RowVersion, AdminRef Role, string DocumentType, AdminRef? Branch, string Currency,
    bool IsUnlimited, decimal? MaxAmount, bool IsActive, DateTimeOffset CreatedAtUtc, DateTimeOffset UpdatedAtUtc);

public sealed record ApprovalLimitPage(IReadOnlyList<ApprovalLimitView> Items, int TotalCount, int Page, int PageSize);

public sealed record ApprovalLimitOptions(
    IReadOnlyList<AdminRef> Roles, IReadOnlyList<AdminRef> Branches, IReadOnlyList<string> DocumentTypes, IReadOnlyList<string> Currencies);
