namespace TanErp.Application.IdentityAccess.Administration;

public static class AdministrationPermissions
{
    public const string UsersRead = "users.read";
    public const string UsersManage = "users.manage";
    public const string MembershipsManage = "memberships.manage";
    public const string RolesAssign = "roles.assign";
    public const string RolesAssignApproval = "roles.assign-approval";
}

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
    public const int MaxRolesPerRequest = 20;
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
