using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.Administration;

/// <summary>
/// Use cases for identity administration. Authorizes each call against the live PostgreSQL permissions,
/// validates input and delegates the transactional work to <see cref="IIdentityAdministrationStore"/>.
/// </summary>
public class IdentityAdministrationService
{
    private static readonly HashSet<string> UserStatuses = new(StringComparer.Ordinal) { "pending", "active", "inactive" };
    private static readonly HashSet<string> RequestStatuses = new(StringComparer.Ordinal) { "pending", "approved", "rejected", "cancelled" };

    private readonly IRequestAccessResolver _access;
    private readonly IIdentityAdministrationStore _store;

    public IdentityAdministrationService(IRequestAccessResolver access, IIdentityAdministrationStore store)
    {
        _access = access;
        _store = store;
    }

    public async Task<Result<AdminUserPage>> ListUsersAsync(
        AdminCaller caller, string? search, string? status, string? sortBy, string? sortOrder, int page, int limit, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.UsersRead, cancellationToken);
        if (access.IsFailure) return Result<AdminUserPage>.Failure(access.Error);

        if (status is not null && !UserStatuses.Contains(status))
            return Result<AdminUserPage>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Unknown user status filter."));

        var resolvedSort = string.IsNullOrWhiteSpace(sortBy) ? AdminUserSortKey.CreatedAt : sortBy.Trim();
        if (!AdminUserSortKey.IsValid(resolvedSort))
            return Result<AdminUserPage>.Failure(new Error("USER_SORT_INVALID", "Invalid user sort key."));

        var resolvedOrder = string.IsNullOrWhiteSpace(sortOrder) ? "desc" : sortOrder.Trim().ToLowerInvariant();
        if (resolvedOrder is not ("asc" or "desc"))
            return Result<AdminUserPage>.Failure(new Error("USER_SORT_ORDER_INVALID", "Invalid sort order."));

        var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var safePage = Math.Max(page, 1);
        var safeLimit = limit < 1 ? AdministrationPolicy.DefaultPageSize : Math.Min(limit, AdministrationPolicy.MaxPageSize);
        return Result<AdminUserPage>.Success(
            await _store.ListUsersAsync(
                access.Value!.OrganizationId, normalizedSearch, status, resolvedSort, resolvedOrder == "desc", safePage, safeLimit, cancellationToken));
    }

    public async Task<Result<AdminUser>> GetUserAsync(AdminCaller caller, Guid userId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.UsersRead, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var user = await _store.GetUserAsync(access.Value!.OrganizationId, userId, cancellationToken);
        return user is null ? NotFound<AdminUser>("User") : Result<AdminUser>.Success(user);
    }

    public async Task<Result<AdminUser>> CreateUserAsync(
        AdminCaller caller, string idempotencyKey, string? displayName, string? email, Guid? branchId, IReadOnlyList<Guid>? roleIds,
        string traceId, CancellationToken cancellationToken)
    {
        var actorResult = await ResolveAllAsync(caller, cancellationToken,
            AdministrationPermissions.UsersManage, AdministrationPermissions.MembershipsManage, AdministrationPermissions.RolesAssign);
        if (actorResult.IsFailure) return Result<AdminUser>.Failure(actorResult.Error);
        var access = actorResult.Value!;

        var name = displayName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 255)
            return Validation<AdminUser>("Display name is required (max 255 characters).");

        var normalizedEmail = email?.Trim();
        if (!IsPlausibleEmail(normalizedEmail))
            return Validation<AdminUser>("A valid email is required.");

        var distinctRoles = (roleIds ?? Array.Empty<Guid>()).Distinct().ToList();
        if (distinctRoles.Count == 0 || distinctRoles.Count > AdministrationPolicy.MaxRolesPerRequest || distinctRoles.Contains(Guid.Empty))
            return Validation<AdminUser>("Between 1 and 20 roles are required.");

        var sortedRoles = string.Join(",", distinctRoles.OrderBy(id => id));
        var payloadHash = Sha256Hex.Compute($"{name}|{User.NormalizeEmail(normalizedEmail)}|{branchId}|{sortedRoles}");
        return await _store.CreateUserAsync(
            access.OrganizationId,
            new CreateAdminUserInput(name, normalizedEmail!, branchId, distinctRoles),
            new AdminActor(access.ActorUserId, access.MembershipId),
            Sha256Hex.Compute(idempotencyKey),
            payloadHash,
            traceId,
            cancellationToken);
    }

    public async Task<Result<AdminUser>> RenameUserAsync(
        AdminCaller caller, Guid userId, Guid expectedRowVersion, string? displayName, string traceId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.UsersManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var name = displayName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 255)
            return Validation<AdminUser>("Display name is required (max 255 characters).");

        var a = access.Value!;
        return await _store.RenameUserAsync(a.OrganizationId, userId, expectedRowVersion, name, new AdminActor(a.ActorUserId, a.MembershipId), traceId, cancellationToken);
    }

    public async Task<Result<AdminUser>> SetUserActiveAsync(
        AdminCaller caller, Guid userId, Guid expectedRowVersion, bool active, string traceId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.UsersManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.SetUserActiveAsync(a.OrganizationId, userId, expectedRowVersion, active, new AdminActor(a.ActorUserId, a.MembershipId), traceId, cancellationToken);
    }

    public async Task<Result<AdminUser>> UpdateMembershipAsync(
        AdminCaller caller, Guid membershipId, Guid expectedRowVersion, Guid? branchId, DateTimeOffset? startsAtUtc, DateTimeOffset? expiresAtUtc,
        string traceId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.MembershipsManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        if (startsAtUtc.HasValue && expiresAtUtc.HasValue && expiresAtUtc <= startsAtUtc)
            return Validation<AdminUser>("Expiry must be after the start time.");

        var a = access.Value!;
        return await _store.UpdateMembershipAsync(
            a.OrganizationId, membershipId, expectedRowVersion, branchId, startsAtUtc, expiresAtUtc, new AdminActor(a.ActorUserId, a.MembershipId), traceId, cancellationToken);
    }

    public async Task<Result<AdminUser>> SetMembershipActiveAsync(
        AdminCaller caller, Guid membershipId, Guid expectedRowVersion, bool active, string traceId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.MembershipsManage, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.SetMembershipActiveAsync(a.OrganizationId, membershipId, expectedRowVersion, active, new AdminActor(a.ActorUserId, a.MembershipId), traceId, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AdminRole>>> ListRolesAsync(AdminCaller caller, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.RolesAssign, cancellationToken);
        if (access.IsFailure) return Result<IReadOnlyList<AdminRole>>.Failure(access.Error);

        var roles = await _store.ListRolesAsync(access.Value!.OrganizationId, access.Value.MembershipId, cancellationToken);
        return Result<IReadOnlyList<AdminRole>>.Success(roles);
    }

    public async Task<Result<AssignRoleOutcome>> AssignRoleAsync(
        AdminCaller caller, string idempotencyKey, Guid membershipId, Guid roleId, string traceId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.RolesAssign, cancellationToken);
        if (access.IsFailure) return Result<AssignRoleOutcome>.Failure(access.Error);

        var a = access.Value!;
        return await _store.AssignRoleAsync(
            a.OrganizationId, membershipId, roleId, new AdminActor(a.ActorUserId, a.MembershipId),
            Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute($"{membershipId}|{roleId}"), traceId, cancellationToken);
    }

    public async Task<Result<AdminUser>> RevokeRoleAsync(
        AdminCaller caller, Guid membershipId, Guid roleId, string traceId, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.RolesAssign, cancellationToken);
        if (access.IsFailure) return Result<AdminUser>.Failure(access.Error);

        var a = access.Value!;
        return await _store.RevokeRoleAsync(a.OrganizationId, membershipId, roleId, new AdminActor(a.ActorUserId, a.MembershipId), traceId, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AdminRoleRequest>>> ListRoleRequestsAsync(
        AdminCaller caller, string? status, CancellationToken cancellationToken)
    {
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, AdministrationPermissions.RolesAssignApproval, cancellationToken);
        if (access.IsFailure) return Result<IReadOnlyList<AdminRoleRequest>>.Failure(access.Error);

        var effectiveStatus = string.IsNullOrWhiteSpace(status) ? "pending" : status.Trim();
        if (!RequestStatuses.Contains(effectiveStatus))
            return Validation<IReadOnlyList<AdminRoleRequest>>("Unknown request status filter.");

        return Result<IReadOnlyList<AdminRoleRequest>>.Success(
            await _store.ListRoleRequestsAsync(access.Value!.OrganizationId, effectiveStatus, cancellationToken));
    }

    public async Task<Result<AdminRoleRequest>> DecideRoleRequestAsync(
        AdminCaller caller, Guid requestId, Guid expectedRowVersion, RoleRequestDecision decision, string traceId, CancellationToken cancellationToken)
    {
        // Cancelling is the requester's own action (roles.assign); approving/rejecting is the checker's (roles.assign-approval).
        var permission = decision == RoleRequestDecision.Cancel ? AdministrationPermissions.RolesAssign : AdministrationPermissions.RolesAssignApproval;
        var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, cancellationToken);
        if (access.IsFailure) return Result<AdminRoleRequest>.Failure(access.Error);

        var a = access.Value!;
        return await _store.DecideRoleRequestAsync(a.OrganizationId, requestId, expectedRowVersion, decision, new AdminActor(a.ActorUserId, a.MembershipId), traceId, cancellationToken);
    }

    private async Task<Result<RequestAccessContext>> ResolveAllAsync(AdminCaller caller, CancellationToken cancellationToken, params string[] permissions)
    {
        RequestAccessContext? first = null;
        foreach (var permission in permissions)
        {
            var access = await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, cancellationToken);
            if (access.IsFailure) return access;
            first ??= access.Value;
        }

        return Result<RequestAccessContext>.Success(first!);
    }

    private static bool IsPlausibleEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && email.Length <= 255
        && email.Count(c => c == '@') == 1
        && !email.StartsWith('@')
        && !email.EndsWith('@')
        && !email.Any(char.IsWhiteSpace);

    private static Result<T> Validation<T>(string detail) => Result<T>.Failure(new Error("REQUEST_VALIDATION_FAILED", detail));

    private static Result<T> NotFound<T>(string resource) => Result<T>.Failure(new Error("RESOURCE_NOT_FOUND", $"{resource} was not found."));
}

public sealed record AdminCaller(string FirebaseUid, Guid MembershipId);
