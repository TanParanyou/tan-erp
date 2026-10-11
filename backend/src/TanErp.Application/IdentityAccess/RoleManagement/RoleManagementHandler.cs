using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;

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
}
