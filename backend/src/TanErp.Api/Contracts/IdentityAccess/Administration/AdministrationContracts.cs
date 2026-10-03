using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Api.Contracts.IdentityAccess.Administration;

public record AdminRefResponse(Guid Id, string Name);

public record AdminPendingRoleRequestResponse(Guid Id, Guid RowVersion, AdminRefResponse Role, AdminRefResponse RequestedBy, DateTimeOffset RequestedAtUtc);

public record AdminMembershipResponse(
    Guid Id,
    bool IsActive,
    Guid RowVersion,
    AdminRefResponse? Branch,
    DateTimeOffset? StartsAtUtc,
    DateTimeOffset? ExpiresAtUtc,
    IReadOnlyList<AdminRefResponse> Roles,
    IReadOnlyList<AdminPendingRoleRequestResponse> PendingRoleRequests);

public record AdminUserResponse(
    Guid Id,
    string DisplayName,
    string Email,
    string Status,
    Guid RowVersion,
    IReadOnlyList<AdminMembershipResponse> Memberships)
{
    public static AdminUserResponse From(AdminUser user) => new(
        user.Id,
        user.DisplayName,
        user.Email,
        user.Status,
        user.RowVersion,
        user.Memberships.Select(m => new AdminMembershipResponse(
            m.Id,
            m.IsActive,
            m.RowVersion,
            m.Branch is null ? null : new AdminRefResponse(m.Branch.Id, m.Branch.Name),
            m.StartsAtUtc,
            m.ExpiresAtUtc,
            m.Roles.Select(r => new AdminRefResponse(r.Id, r.Name)).ToList(),
            m.PendingRoleRequests.Select(p => new AdminPendingRoleRequestResponse(
                p.Id,
                p.RowVersion,
                new AdminRefResponse(p.Role.Id, p.Role.Name),
                new AdminRefResponse(p.RequestedBy.Id, p.RequestedBy.Name),
                p.RequestedAtUtc)).ToList())).ToList());
}

public record AdminPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public record AdminUserListResponse(IReadOnlyList<AdminUserResponse> Items, AdminPaginationResponse Pagination);

public record AdminRoleResponse(Guid Id, string Name, bool Assignable, bool RequiresApproval, IReadOnlyList<string> PermissionKeys);

public record AdminRoleListResponse(IReadOnlyList<AdminRoleResponse> Items);

public record AdminRoleRequestResponse(
    Guid Id,
    Guid RowVersion,
    string Status,
    AdminRefResponse Role,
    AdminRefResponse Membership,
    AdminRefResponse RequestedBy,
    DateTimeOffset RequestedAtUtc)
{
    public static AdminRoleRequestResponse From(AdminRoleRequest request) => new(
        request.Id,
        request.RowVersion,
        request.Status,
        new AdminRefResponse(request.Role.Id, request.Role.Name),
        new AdminRefResponse(request.Membership.Id, request.Membership.Name),
        new AdminRefResponse(request.RequestedBy.Id, request.RequestedBy.Name),
        request.RequestedAtUtc);
}

public record AdminRoleRequestListResponse(IReadOnlyList<AdminRoleRequestResponse> Items);

public record AdminAssignRoleResponse(AdminUserResponse User, AdminRoleRequestResponse? PendingRequest);

public record CreateAdminUserRequest(string? DisplayName, string? Email, Guid? BranchId, IReadOnlyList<Guid>? RoleIds);

public record RenameAdminUserRequest(string? DisplayName);

public record UpdateAdminMembershipRequest(Guid? BranchId, DateTimeOffset? StartsAtUtc, DateTimeOffset? ExpiresAtUtc);

public record AssignAdminRoleRequest(Guid RoleId);
