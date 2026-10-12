using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.RoleManagement;

/// <summary>Every operation resolves its permission first; the store is never reached (and never queried) without it.</summary>
public sealed class RoleManagementHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly IRoleManagementStore _store;

    public RoleManagementHandler(IRequestAccessResolver access, IRoleManagementStore store)
    {
        _access = access;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(AdminCaller caller, string permission, CancellationToken ct) =>
        _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static AdminActor Actor(RequestAccessContext access) => new(access.ActorUserId, access.MembershipId);

    private static Result<T> Denied<T>(Result<RequestAccessContext> access) => Result<T>.Failure(access.Error);

    public async Task<Result<IReadOnlyList<PermissionCatalogEntry>>> ListPermissionsAsync(AdminCaller caller, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Result<IReadOnlyList<PermissionCatalogEntry>>.Failure(access.Error);
        return Result<IReadOnlyList<PermissionCatalogEntry>>.Success(await _store.ListPermissionsAsync(ct));
    }

    public async Task<Result<RoleDefinitionPage>> ListRolesAsync(
        AdminCaller caller, string? search, string? status, string? sortBy, string? sortOrder, int page, int limit, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Denied<RoleDefinitionPage>(access);

        if (status is not (null or "active" or "inactive"))
            return AdminHandlerSupport.Validation<RoleDefinitionPage>("Status must be 'active' or 'inactive'.");
        var key = string.IsNullOrWhiteSpace(sortBy) ? RoleSortKey.Name : sortBy.Trim();
        if (!RoleSortKey.IsValid(key)) return AdminHandlerSupport.Validation<RoleDefinitionPage>("Unknown role sort key.");
        var order = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder.Trim().ToLowerInvariant();
        if (order is not ("asc" or "desc")) return AdminHandlerSupport.Validation<RoleDefinitionPage>("Sort order must be 'asc' or 'desc'.");

        var query = new RoleListQuery(
            string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            status, key, order == "desc", Math.Max(page, 1),
            limit < 1 ? AdministrationPolicy.DefaultPageSize : Math.Min(limit, AdministrationPolicy.MaxPageSize));
        return Result<RoleDefinitionPage>.Success(await _store.ListRolesAsync(access.Value!.OrganizationId, query, ct));
    }

    public async Task<Result<RoleDefinition>> GetRoleAsync(AdminCaller caller, Guid roleId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        return access.IsFailure ? Denied<RoleDefinition>(access) : await _store.GetRoleAsync(access.Value!.OrganizationId, roleId, ct);
    }

    public async Task<Result<RoleDefinition>> CreateRoleAsync(
        AdminCaller caller, string idempotencyKey, RoleInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Denied<RoleDefinition>(access);

        var invalid = RoleManagementPolicy.ValidateShape(input);
        if (invalid is not null) return Result<RoleDefinition>.Failure(invalid);

        var canonical = RoleManagementPolicy.Canonicalize(input);
        return await _store.CreateRoleAsync(
            access.Value!.OrganizationId, canonical, Actor(access.Value), Sha256Hex.Compute(idempotencyKey), PayloadHash(canonical), traceId, ct);
    }

    public async Task<Result<RoleUpdateOutcome>> UpdateRoleAsync(
        AdminCaller caller, Guid roleId, Guid ifMatch, RoleInput input, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        if (access.IsFailure) return Denied<RoleUpdateOutcome>(access);

        var invalid = RoleManagementPolicy.ValidateShape(input);
        if (invalid is not null) return Result<RoleUpdateOutcome>.Failure(invalid);

        return await _store.UpdateRoleAsync(
            access.Value!.OrganizationId, roleId, ifMatch, RoleManagementPolicy.Canonicalize(input), Actor(access.Value), traceId, ct);
    }

    private static readonly IReadOnlySet<string> RequestStatuses = new HashSet<string>(StringComparer.Ordinal)
    {
        RoleAssignmentRequestStatus.Pending,
        RoleAssignmentRequestStatus.Approved,
        RoleAssignmentRequestStatus.Rejected,
        RoleAssignmentRequestStatus.Cancelled
    };

    public async Task<Result<IReadOnlyList<RoleChangeRequestView>>> ListChangeRequestsAsync(AdminCaller caller, string? status, CancellationToken ct)
    {
        // The checker queue is visible only to holders of roles.assign-approval (the same gate as the decisions).
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesAssignApproval, ct);
        if (access.IsFailure) return Result<IReadOnlyList<RoleChangeRequestView>>.Failure(access.Error);

        var wanted = string.IsNullOrWhiteSpace(status) ? RoleAssignmentRequestStatus.Pending : status.Trim();
        if (!RequestStatuses.Contains(wanted))
            return AdminHandlerSupport.Validation<IReadOnlyList<RoleChangeRequestView>>("Unknown request status filter.");

        return Result<IReadOnlyList<RoleChangeRequestView>>.Success(
            await _store.ListChangeRequestsAsync(access.Value!.OrganizationId, wanted, ct));
    }

    /// <summary>Cancelling is the requester's own action (roles.manage); approving or rejecting is the checker's (roles.assign-approval).</summary>
    public async Task<Result<RoleChangeRequestView>> DecideChangeRequestAsync(
        AdminCaller caller, Guid requestId, Guid ifMatch, RoleRequestDecision decision, string traceId, CancellationToken ct)
    {
        var permission = decision == RoleRequestDecision.Cancel ? RoleManagementPermissions.RolesManage : RoleManagementPermissions.RolesAssignApproval;
        var access = await AccessAsync(caller, permission, ct);
        return access.IsFailure
            ? Denied<RoleChangeRequestView>(access)
            : await _store.DecideChangeRequestAsync(access.Value!.OrganizationId, requestId, ifMatch, decision, Actor(access.Value), traceId, ct);
    }

    /// <summary>Deactivation is refused while the role is in use; activation is always allowed (no holders are added by it).</summary>
    public async Task<Result<RoleDefinition>> SetRoleActiveAsync(AdminCaller caller, Guid roleId, bool active, Guid ifMatch, string traceId, CancellationToken ct)
    {
        var access = await AccessAsync(caller, RoleManagementPermissions.RolesManage, ct);
        return access.IsFailure
            ? Denied<RoleDefinition>(access)
            : await _store.SetRoleActiveAsync(access.Value!.OrganizationId, roleId, active, ifMatch, Actor(access.Value), traceId, ct);
    }

    private static string PayloadHash(RoleInput input) => Sha256Hex.Compute(
        $"{input.Name.Trim()}|{input.Description?.Trim()}|{string.Join(",", input.Permissions.Select(g => $"{g.PermissionKey}:{g.Scope}:{g.BranchId}"))}");
}
