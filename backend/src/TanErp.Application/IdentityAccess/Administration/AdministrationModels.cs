using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.Administration;

public static class AdministrationPermissions
{
    public const string UsersRead = "users.read";
    public const string UsersManage = "users.manage";
    public const string MembershipsManage = "memberships.manage";
    public const string RolesAssign = "roles.assign";
    public const string RolesAssignApproval = "roles.assign-approval";
}

/// <summary>A permission a role grants or a caller holds: key plus the scope it applies to.</summary>
public sealed record PermissionGrant(string Key, string Scope, Guid? ScopeId);

/// <summary>One active administrator membership holding a role that grants <c>users.manage</c>.</summary>
public sealed record AdministratorRow(Guid UserId, Guid MembershipId, Guid RoleId);

public static class AdministrationPolicy
{
    /// <summary>
    /// A Role containing any of these permissions needs an independent checker (maker-checker) before it takes effect.
    /// </summary>
    public static readonly IReadOnlySet<string> ApprovalPermissionKeys = new HashSet<string>(StringComparer.Ordinal)
    {
        "estimates.approve",
        "cost-records.approve",
        "cost-records.publish"
    };

    public static bool RequiresApproval(IEnumerable<string> rolePermissionKeys) =>
        rolePermissionKeys.Any(ApprovalPermissionKeys.Contains);

    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    public const int MaxRolesPerRequest = 20;

    /// <summary>
    /// Anti-escalation: a caller may only hand out permissions it holds. A wanted permission is covered when the
    /// caller holds the same key with organization scope in this organization, or with an identical narrower scope.
    /// </summary>
    public static bool Covers(IReadOnlyCollection<PermissionGrant> held, Guid organizationId, IReadOnlyCollection<PermissionGrant> wanted) =>
        wanted.All(w => held.Any(h =>
            h.Key == w.Key &&
            ((h.Scope == PermissionScope.Organization && h.ScopeId == organizationId) ||
             (h.Scope == w.Scope && h.ScopeId == w.ScopeId))));

    /// <summary>
    /// Last-administrator guard: true when <paramref name="removed"/> matches at least one current administrator row
    /// and no other row would remain. A change that touches no administrator never trips the guard.
    /// </summary>
    public static bool WouldRemoveLastAdministrator(IReadOnlyCollection<AdministratorRow> administrators, Func<AdministratorRow, bool> removed) =>
        administrators.Any(removed) && !administrators.Any(row => !removed(row));
}

public static class AdminUserSortKey
{
    public const string DisplayName = "displayName";
    public const string Email = "email";
    public const string CreatedAt = "createdAt";

    public static bool IsValid(string value) => value is DisplayName or Email or CreatedAt;
}

public sealed record AdminActor(Guid UserId, Guid MembershipId);

public sealed record AdminRef(Guid Id, string Name);

public sealed record AdminPendingRoleRequest(Guid Id, Guid RowVersion, AdminRef Role, AdminRef RequestedBy, DateTimeOffset RequestedAtUtc);

public sealed record AdminMembership(
    Guid Id,
    bool IsActive,
    Guid RowVersion,
    AdminRef? Branch,
    DateTimeOffset? StartsAtUtc,
    DateTimeOffset? ExpiresAtUtc,
    IReadOnlyList<AdminRef> Roles,
    IReadOnlyList<AdminPendingRoleRequest> PendingRoleRequests);

public sealed record AdminUser(
    Guid Id,
    string DisplayName,
    string Email,
    string Status,
    Guid RowVersion,
    IReadOnlyList<AdminMembership> Memberships);

public sealed record AdminUserPage(IReadOnlyList<AdminUser> Items, int TotalCount, int Page, int PageSize);

public sealed record AdminRole(Guid Id, string Name, bool Assignable, bool RequiresApproval, IReadOnlyList<string> PermissionKeys);

public sealed record AdminRoleRequest(
    Guid Id,
    Guid RowVersion,
    string Status,
    AdminRef Role,
    AdminRef Membership,
    AdminRef RequestedBy,
    DateTimeOffset RequestedAtUtc);

public sealed record CreateAdminUserInput(string DisplayName, string Email, Guid? BranchId, IReadOnlyList<Guid> RoleIds);

public sealed record AssignRoleOutcome(bool Pending, AdminUser User, AdminRoleRequest? Request);
