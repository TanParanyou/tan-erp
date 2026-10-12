using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.IdentityAccess.RoleManagement;

public static class RoleManagementPermissions
{
    public const string RolesManage = "roles.manage";
    public const string RolesAssignApproval = AdministrationPermissions.RolesAssignApproval;
}

public static class RoleSortKey
{
    public const string Name = "name";
    public const string CreatedAt = "createdAt";

    public static bool IsValid(string value) => value is Name or CreatedAt;
}

public sealed record PermissionCatalogEntry(string Key, string? Description, bool IsApprovalPermission);

public sealed record RoleGrantInput(string PermissionKey, string Scope, Guid? BranchId);

public sealed record RoleGrantView(string PermissionKey, string Scope, AdminRef? Branch);

public sealed record RoleInput(string Name, string? Description, IReadOnlyList<RoleGrantInput> Permissions);

public sealed record RoleListQuery(string? Search, string? Status, string SortBy, bool Descending, int Page, int Limit);

public sealed record RoleDefinitionSummary(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion,
    int MemberCount, int PermissionCount, bool RequiresApproval, bool HasPendingChange);

public sealed record RoleDefinitionPage(IReadOnlyList<RoleDefinitionSummary> Items, int TotalCount, int Page, int PageSize);

public sealed record RoleChangeRequestView(
    Guid Id, Guid RowVersion, string Status, AdminRef Role, AdminRef RequestedBy, DateTimeOffset RequestedAtUtc,
    string ProposedName, string? ProposedDescription,
    IReadOnlyList<string> AddedPermissionKeys, IReadOnlyList<string> RemovedPermissionKeys);

public sealed record RoleDefinition(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc,
    int MemberCount, int PendingAssignmentRequestCount, bool RequiresApproval,
    IReadOnlyList<RoleGrantView> Permissions, RoleChangeRequestView? PendingChange);

public sealed record RoleUpdateOutcome(bool Pending, RoleDefinition Role, RoleChangeRequestView? Request);
