using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.Notifications;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence.Notifications;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public class IdentityAdministrationStore : IIdentityAdministrationStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly INotificationPublisher _notifications;

    public IdentityAdministrationStore(AppDbContext db, IClock clock, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _notifications = notifications;
    }

    // ---------------------------------------------------------------- reads

    public async Task<AdminUserPage> ListUsersAsync(
        Guid organizationId, string? search, string? status, string sortBy, bool descending, int page, int limit, CancellationToken cancellationToken)
    {
        var query = _db.Users.AsNoTracking()
            .Where(u => u.Memberships.Any(m => m.OrganizationId == organizationId));

        if (!string.IsNullOrEmpty(search))
        {
            var pattern = $"%{EscapeLike(search)}%";
            query = query.Where(u => EF.Functions.ILike(u.DisplayName, pattern, "\\") || EF.Functions.ILike(u.Email, pattern, "\\"));
        }

        query = status switch
        {
            "pending" => query.Where(u => u.IsActive && u.FirebaseUid == null),
            "active" => query.Where(u => u.IsActive && u.FirebaseUid != null),
            "inactive" => query.Where(u => !u.IsActive),
            _ => query
        };

        var ordered = (sortBy, descending) switch
        {
            (AdminUserSortKey.DisplayName, false) => query.OrderBy(u => u.DisplayName),
            (AdminUserSortKey.DisplayName, true) => query.OrderByDescending(u => u.DisplayName),
            (AdminUserSortKey.Email, false) => query.OrderBy(u => u.NormalizedEmail),
            (AdminUserSortKey.Email, true) => query.OrderByDescending(u => u.NormalizedEmail),
            (_, false) => query.OrderBy(u => u.CreatedAtUtc),
            _ => query.OrderByDescending(u => u.CreatedAtUtc)
        };

        var total = await query.CountAsync(cancellationToken);
        // The id is a stable tie-breaker so paging never skips or repeats rows.
        var ids = await ordered
            .ThenBy(u => u.Id)
            .Skip((page - 1) * limit).Take(limit)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        var items = await LoadUsersAsync(organizationId, ids, cancellationToken);
        return new AdminUserPage(items, total, page, limit);
    }

    public async Task<AdminUser?> GetUserAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken)
    {
        var users = await LoadUsersAsync(organizationId, new[] { userId }, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<IReadOnlyList<AdminRole>> ListRolesAsync(Guid organizationId, Guid actorMembershipId, CancellationToken cancellationToken)
    {
        var actorPermissions = await LoadPermissionsAsync(actorMembershipId, cancellationToken);
        var roles = await _db.Roles.AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && r.IsActive)
            .OrderBy(r => r.Name)
            .Select(r => new
            {
                r.Id,
                r.Name,
                Permissions = r.RolePermissions
                    .Where(rp => rp.Permission!.IsActive)
                    .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId))
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return roles.Select(r => new AdminRole(
            r.Id,
            r.Name,
            AdministrationPolicy.Covers(actorPermissions, organizationId, r.Permissions),
            AdministrationPolicy.RequiresApproval(r.Permissions.Select(p => p.Key)),
            r.Permissions.Select(p => p.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList())).ToList();
    }

    public async Task<IReadOnlyList<AdminRoleRequest>> ListRoleRequestsAsync(Guid organizationId, string status, CancellationToken cancellationToken)
    {
        var rows = await RoleRequestQuery(organizationId, r => r.Status == status)
            .Take(200)
            .ToListAsync(cancellationToken);
        return rows.Select(ToRoleRequest).ToList();
    }

    // ---------------------------------------------------------------- users

    public Task<Result<AdminUser>> CreateUserAsync(
        Guid organizationId, CreateAdminUserInput input, AdminActor actor, string keyHash, string payloadHash, string traceId,
        CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            const string operation = "admin.users.create";
            var replay = await FindReplayAsync(organizationId, operation, keyHash, payloadHash, cancellationToken);
            if (replay.IsFailure) return Result<AdminUser>.Failure(replay.Error);
            if (replay.Value is not null && Guid.TryParse(replay.Value.ResourceId, out var replayedUserId))
                return await ReloadUserAsync(organizationId, replayedUserId, cancellationToken);

            var now = _clock.UtcNow;

            Branch? branch = null;
            if (input.BranchId.HasValue)
            {
                branch = await _db.Branches.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == input.BranchId && b.OrganizationId == organizationId, cancellationToken);
                if (branch is null) return Fail<AdminUser>("BRANCH_NOT_FOUND", "Branch not found.");
                if (!branch.IsActive) return Fail<AdminUser>("BRANCH_INACTIVE", "Branch is inactive.");
            }

            var roles = await LoadRolesWithPermissionsAsync(organizationId, input.RoleIds, cancellationToken);
            if (roles.Count != input.RoleIds.Count) return NotFound<AdminUser>("Role");

            var actorPermissions = await LoadPermissionsAsync(actor.MembershipId, cancellationToken);
            if (roles.Any(r => !AdministrationPolicy.Covers(actorPermissions, organizationId, r.Permissions)))
                return Fail<AdminUser>("ROLE_ESCALATION_DENIED", "A role grants permissions the caller does not hold.");

            var normalizedEmail = User.NormalizeEmail(input.Email);
            if (await _db.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken))
                return Fail<AdminUser>("USER_EMAIL_ALREADY_EXISTS", "A user with this email already exists.");

            var user = User.CreatePending(Guid.NewGuid(), input.DisplayName, input.Email, now);
            var membership = new Membership(Guid.NewGuid(), organizationId, input.BranchId, user.Id, isActive: true, createdAtUtc: now);
            _db.Users.Add(user);
            _db.Memberships.Add(membership);

            var requestedRoleIds = new List<Guid>();
            foreach (var role in roles)
            {
                if (role.RequiresApproval)
                {
                    var request = new RoleAssignmentRequest(Guid.NewGuid(), organizationId, membership.Id, role.Id, actor.UserId, now);
                    _db.RoleAssignmentRequests.Add(request);
                    await _notifications.PublishAsync(
                        NotificationEvents.RoleAssignmentRequested(
                            organizationId, request.Id, actor.UserId, user.Id, input.DisplayName, role.Name),
                        cancellationToken);
                    requestedRoleIds.Add(role.Id);
                }
                else
                {
                    _db.MembershipRoles.Add(new MembershipRole(membership.Id, role.Id, organizationId, now));
                }
            }

            AddAudit(organizationId, actor, "users.created", "User", user.Id.ToString(), traceId, now,
                new
                {
                    membershipId = membership.Id,
                    branchId = input.BranchId,
                    assignedRoleIds = roles.Where(r => !r.RequiresApproval).Select(r => r.Id).ToArray(),
                    requestedRoleIds
                });

            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(), organizationId, operation, keyHash, payloadHash, user.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<AdminUser>("USER_EMAIL_ALREADY_EXISTS", "A user with this email already exists.");
            }

            return await ReloadUserAsync(organizationId, user.Id, cancellationToken);
        }, cancellationToken);

    public Task<Result<AdminUser>> RenameUserAsync(
        Guid organizationId, Guid userId, Guid expectedRowVersion, string displayName, AdminActor actor, string traceId, CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            var user = await FindUserInOrganizationAsync(organizationId, userId, cancellationToken);
            if (user is null) return NotFound<AdminUser>("User");
            if (user.RowVersion != expectedRowVersion) return VersionConflict<AdminUser>();

            var before = user.RowVersion;
            user.Rename(displayName);
            AddAudit(organizationId, actor, "users.renamed", "User", user.Id.ToString(), traceId, _clock.UtcNow, new { renamed = true }, before, user.RowVersion);
            await _db.SaveChangesAsync(cancellationToken);
            return await ReloadUserAsync(organizationId, user.Id, cancellationToken);
        }, cancellationToken);

    public Task<Result<AdminUser>> SetUserActiveAsync(
        Guid organizationId, Guid userId, Guid expectedRowVersion, bool active, AdminActor actor, string traceId, CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            var user = await FindUserInOrganizationAsync(organizationId, userId, cancellationToken);
            if (user is null) return NotFound<AdminUser>("User");
            if (user.RowVersion != expectedRowVersion) return VersionConflict<AdminUser>();

            // The user record is global; one organization's administrator must not switch off access to other organizations.
            var sharedAcrossOrganizations = await _db.Memberships.AsNoTracking()
                .AnyAsync(m => m.UserId == userId && m.OrganizationId != organizationId, cancellationToken);
            if (sharedAcrossOrganizations)
                return Fail<AdminUser>("USER_SHARED_ACROSS_ORGANIZATIONS", "The user has memberships in other organizations.");

            var before = user.RowVersion;
            if (!active && user.IsActive)
            {
                if (await WouldRemoveLastAdministratorAsync(organizationId, row => row.UserId == userId, cancellationToken))
                    return Fail<AdminUser>("LAST_ADMINISTRATOR_REQUIRED", "The organization must keep at least one active administrator.");
                user.Deactivate();
            }
            else if (active && !user.IsActive)
            {
                user.Activate();
            }

            AddAudit(organizationId, actor, active ? "users.activated" : "users.deactivated", "User", user.Id.ToString(), traceId, _clock.UtcNow,
                new { isActive = active }, before, user.RowVersion);
            await _db.SaveChangesAsync(cancellationToken);
            return await ReloadUserAsync(organizationId, user.Id, cancellationToken);
        }, cancellationToken);

    // ---------------------------------------------------------------- memberships

    public Task<Result<AdminUser>> UpdateMembershipAsync(
        Guid organizationId, Guid membershipId, Guid expectedRowVersion, Guid? branchId, DateTimeOffset? startsAtUtc, DateTimeOffset? expiresAtUtc,
        AdminActor actor, string traceId, CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            var membership = await _db.Memberships
                .FirstOrDefaultAsync(m => m.Id == membershipId && m.OrganizationId == organizationId, cancellationToken);
            if (membership is null) return NotFound<AdminUser>("Membership");
            if (membership.RowVersion != expectedRowVersion) return VersionConflict<AdminUser>();

            if (branchId.HasValue)
            {
                var branch = await _db.Branches.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == organizationId, cancellationToken);
                if (branch is null) return Fail<AdminUser>("BRANCH_NOT_FOUND", "Branch not found.");
                if (!branch.IsActive) return Fail<AdminUser>("BRANCH_INACTIVE", "Branch is inactive.");
            }

            var now = _clock.UtcNow;
            var staysValid = (!startsAtUtc.HasValue || startsAtUtc <= now) && (!expiresAtUtc.HasValue || expiresAtUtc > now);
            if (!staysValid && await WouldRemoveLastAdministratorAsync(organizationId, row => row.MembershipId == membershipId, cancellationToken))
                return Fail<AdminUser>("LAST_ADMINISTRATOR_REQUIRED", "The organization must keep at least one active administrator.");

            var before = membership.RowVersion;
            membership.UpdateAssignment(branchId, startsAtUtc, expiresAtUtc);
            AddAudit(organizationId, actor, "memberships.updated", "Membership", membership.Id.ToString(), traceId, now,
                new { branchId, startsAtUtc, expiresAtUtc }, before, membership.RowVersion);
            await _db.SaveChangesAsync(cancellationToken);
            return await ReloadUserAsync(organizationId, membership.UserId, cancellationToken);
        }, cancellationToken);

    public Task<Result<AdminUser>> SetMembershipActiveAsync(
        Guid organizationId, Guid membershipId, Guid expectedRowVersion, bool active, AdminActor actor, string traceId, CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            var membership = await _db.Memberships
                .FirstOrDefaultAsync(m => m.Id == membershipId && m.OrganizationId == organizationId, cancellationToken);
            if (membership is null) return NotFound<AdminUser>("Membership");
            if (membership.RowVersion != expectedRowVersion) return VersionConflict<AdminUser>();

            var before = membership.RowVersion;
            if (!active && membership.IsActive)
            {
                if (await WouldRemoveLastAdministratorAsync(organizationId, row => row.MembershipId == membershipId, cancellationToken))
                    return Fail<AdminUser>("LAST_ADMINISTRATOR_REQUIRED", "The organization must keep at least one active administrator.");
                membership.Deactivate();
            }
            else if (active && !membership.IsActive)
            {
                membership.Activate();
            }

            AddAudit(organizationId, actor, active ? "memberships.activated" : "memberships.deactivated", "Membership", membership.Id.ToString(),
                traceId, _clock.UtcNow, new { isActive = active }, before, membership.RowVersion);
            await _db.SaveChangesAsync(cancellationToken);
            return await ReloadUserAsync(organizationId, membership.UserId, cancellationToken);
        }, cancellationToken);

    // ---------------------------------------------------------------- roles

    public Task<Result<AssignRoleOutcome>> AssignRoleAsync(
        Guid organizationId, Guid membershipId, Guid roleId, AdminActor actor, string keyHash, string payloadHash, string traceId,
        CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            const string operation = "admin.roles.assign";
            var replay = await FindReplayAsync(organizationId, operation, keyHash, payloadHash, cancellationToken);
            if (replay.IsFailure) return Result<AssignRoleOutcome>.Failure(replay.Error);
            if (replay.Value is not null)
                return await ReplayAssignAsync(organizationId, replay.Value.ResourceId, cancellationToken);

            var membership = await _db.Memberships
                .FirstOrDefaultAsync(m => m.Id == membershipId && m.OrganizationId == organizationId, cancellationToken);
            if (membership is null) return NotFound<AssignRoleOutcome>("Membership");
            if (membership.UserId == actor.UserId)
                return Fail<AssignRoleOutcome>("SELF_ROLE_CHANGE_FORBIDDEN", "Roles of the caller's own membership cannot be changed.");

            var roles = await LoadRolesWithPermissionsAsync(organizationId, new[] { roleId }, cancellationToken);
            var role = roles.FirstOrDefault();
            if (role is null) return NotFound<AssignRoleOutcome>("Role");

            if (await _db.MembershipRoles.AsNoTracking().AnyAsync(mr => mr.MembershipId == membershipId && mr.RoleId == roleId, cancellationToken))
                return Fail<AssignRoleOutcome>("ROLE_ALREADY_ASSIGNED", "The role is already assigned.");

            var actorPermissions = await LoadPermissionsAsync(actor.MembershipId, cancellationToken);
            if (!AdministrationPolicy.Covers(actorPermissions, organizationId, role.Permissions))
                return Fail<AssignRoleOutcome>("ROLE_ESCALATION_DENIED", "The role grants permissions the caller does not hold.");

            var now = _clock.UtcNow;
            if (role.RequiresApproval)
            {
                if (await _db.RoleAssignmentRequests.AsNoTracking()
                        .AnyAsync(r => r.MembershipId == membershipId && r.RoleId == roleId && r.Status == RoleAssignmentRequestStatus.Pending, cancellationToken))
                    return Fail<AssignRoleOutcome>("ROLE_ASSIGNMENT_REQUEST_PENDING", "A request for this role is already pending.");

                var request = new RoleAssignmentRequest(Guid.NewGuid(), organizationId, membershipId, roleId, actor.UserId, now);
                _db.RoleAssignmentRequests.Add(request);
                AddAudit(organizationId, actor, "roles.assignment-requested", "RoleAssignmentRequest", request.Id.ToString(), traceId, now,
                    new { membershipId, roleId });
                _db.IdempotencyRecords.Add(new IdempotencyRecord(
                    Guid.NewGuid(), organizationId, operation, keyHash, payloadHash, $"request:{request.Id}", now));
                var subjectName = await _db.Users.AsNoTracking()
                    .Where(u => u.Id == membership.UserId)
                    .Select(u => u.DisplayName)
                    .FirstAsync(cancellationToken);
                await _notifications.PublishAsync(
                    NotificationEvents.RoleAssignmentRequested(
                        organizationId, request.Id, actor.UserId, membership.UserId, subjectName, role.Name),
                    cancellationToken);
                try
                {
                    await _db.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
                {
                    // A concurrent duplicate already committed this transition's notification: report the same outcome as a lost race.
                    return Fail<AssignRoleOutcome>("ADMIN_VERSION_CONFLICT", "The change conflicted with a concurrent update; reload and retry.");
                }

                var user = await ReloadUserAsync(organizationId, membership.UserId, cancellationToken);
                var requestId = request.Id;
                var created = await RoleRequestQuery(organizationId, r => r.Id == requestId).FirstAsync(cancellationToken);
                return Result<AssignRoleOutcome>.Success(new AssignRoleOutcome(true, user.Value!, ToRoleRequest(created)));
            }

            var before = membership.RowVersion;
            _db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, organizationId, now));
            membership.AdvanceVersion();
            AddAudit(organizationId, actor, "roles.assigned", "Membership", membershipId.ToString(), traceId, now,
                new { roleId }, before, membership.RowVersion);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(), organizationId, operation, keyHash, payloadHash, $"membership:{membershipId}", now));
            await _db.SaveChangesAsync(cancellationToken);

            var reloaded = await ReloadUserAsync(organizationId, membership.UserId, cancellationToken);
            return Result<AssignRoleOutcome>.Success(new AssignRoleOutcome(false, reloaded.Value!, null));
        }, cancellationToken);

    public Task<Result<AdminUser>> RevokeRoleAsync(
        Guid organizationId, Guid membershipId, Guid roleId, AdminActor actor, string traceId, CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            var membership = await _db.Memberships
                .FirstOrDefaultAsync(m => m.Id == membershipId && m.OrganizationId == organizationId, cancellationToken);
            if (membership is null) return NotFound<AdminUser>("Membership");
            if (membership.UserId == actor.UserId)
                return Fail<AdminUser>("SELF_ROLE_CHANGE_FORBIDDEN", "Roles of the caller's own membership cannot be changed.");

            var assignment = await _db.MembershipRoles
                .FirstOrDefaultAsync(mr => mr.MembershipId == membershipId && mr.RoleId == roleId && mr.OrganizationId == organizationId, cancellationToken);
            if (assignment is null) return Fail<AdminUser>("ROLE_NOT_ASSIGNED", "The role is not assigned to this membership.");

            if (await WouldRemoveLastAdministratorAsync(organizationId, row => row.MembershipId == membershipId && row.RoleId == roleId, cancellationToken))
                return Fail<AdminUser>("LAST_ADMINISTRATOR_REQUIRED", "The organization must keep at least one active administrator.");

            var before = membership.RowVersion;
            _db.MembershipRoles.Remove(assignment);
            membership.AdvanceVersion();
            AddAudit(organizationId, actor, "roles.revoked", "Membership", membershipId.ToString(), traceId, _clock.UtcNow,
                new { roleId }, before, membership.RowVersion);
            await _db.SaveChangesAsync(cancellationToken);
            return await ReloadUserAsync(organizationId, membership.UserId, cancellationToken);
        }, cancellationToken);

    public Task<Result<AdminRoleRequest>> DecideRoleRequestAsync(
        Guid organizationId, Guid requestId, Guid expectedRowVersion, RoleRequestDecision decision, AdminActor actor, string traceId,
        CancellationToken cancellationToken) =>
        RunAsync(async () =>
        {
            var request = await _db.RoleAssignmentRequests
                .FirstOrDefaultAsync(r => r.Id == requestId && r.OrganizationId == organizationId, cancellationToken);
            if (request is null) return NotFound<AdminRoleRequest>("Role assignment request");
            if (request.RowVersion != expectedRowVersion) return VersionConflict<AdminRoleRequest>();
            if (!request.IsPending)
                return Fail<AdminRoleRequest>("ROLE_ASSIGNMENT_REQUEST_NOT_PENDING", "The request has already been decided.");

            var now = _clock.UtcNow;
            var before = request.RowVersion;

            if (decision == RoleRequestDecision.Cancel)
            {
                if (!request.TryCancel(actor.UserId, now))
                    return Fail<AdminRoleRequest>("PERMISSION_DENIED", "Only the requester can cancel a request.");
                AddAudit(organizationId, actor, "roles.assignment-cancelled", "RoleAssignmentRequest", request.Id.ToString(), traceId, now,
                    new { request.MembershipId, request.RoleId }, before, request.RowVersion);
            }
            else
            {
                if (actor.UserId == request.RequestedByUserId)
                    return Fail<AdminRoleRequest>("ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED", "The decision must be made by someone other than the requester.");

                if (decision == RoleRequestDecision.Reject)
                {
                    request.TryReject(actor.UserId, now);
                    AddAudit(organizationId, actor, "roles.assignment-rejected", "RoleAssignmentRequest", request.Id.ToString(), traceId, now,
                        new { request.MembershipId, request.RoleId }, before, request.RowVersion);
                }
                else
                {
                    var membership = await _db.Memberships
                        .FirstOrDefaultAsync(m => m.Id == request.MembershipId && m.OrganizationId == organizationId, cancellationToken);
                    if (membership is null) return NotFound<AdminRoleRequest>("Membership");
                    if (membership.UserId == actor.UserId)
                        return Fail<AdminRoleRequest>("SELF_ROLE_CHANGE_FORBIDDEN", "Roles of the caller's own membership cannot be changed.");

                    var roles = await LoadRolesWithPermissionsAsync(organizationId, new[] { request.RoleId }, cancellationToken);
                    var role = roles.FirstOrDefault();
                    if (role is null) return NotFound<AdminRoleRequest>("Role");

                    var actorPermissions = await LoadPermissionsAsync(actor.MembershipId, cancellationToken);
                    if (!AdministrationPolicy.Covers(actorPermissions, organizationId, role.Permissions))
                        return Fail<AdminRoleRequest>("ROLE_ESCALATION_DENIED", "The role grants permissions the caller does not hold.");

                    if (await _db.MembershipRoles.AsNoTracking()
                            .AnyAsync(mr => mr.MembershipId == request.MembershipId && mr.RoleId == request.RoleId, cancellationToken))
                        return Fail<AdminRoleRequest>("ROLE_ALREADY_ASSIGNED", "The role is already assigned.");

                    request.TryApprove(actor.UserId, now);
                    _db.MembershipRoles.Add(new MembershipRole(request.MembershipId, request.RoleId, organizationId, now));
                    membership.AdvanceVersion();
                    AddAudit(organizationId, actor, "roles.assignment-approved", "RoleAssignmentRequest", request.Id.ToString(), traceId, now,
                        new { request.MembershipId, request.RoleId }, before, request.RowVersion);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            var row = await RoleRequestQuery(organizationId, r => r.Id == requestId).FirstAsync(cancellationToken);
            return Result<AdminRoleRequest>.Success(ToRoleRequest(row));
        }, cancellationToken);

    // ---------------------------------------------------------------- helpers

    private sealed record RoleWithPermissions(Guid Id, string Name, IReadOnlyList<PermissionGrant> Permissions)
    {
        public bool RequiresApproval => AdministrationPolicy.RequiresApproval(Permissions.Select(p => p.Key));
    }

    private sealed record RoleRequestRow(
        RoleAssignmentRequest Request,
        string RoleName,
        string MembershipUserName,
        string RequestedByName);

    private async Task<Result<T>> RunAsync<T>(Func<Task<Result<T>>> work, CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var result = await work();
                if (result.IsSuccess)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
                else
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                return result;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Fail<T>("ADMIN_VERSION_CONFLICT", "The record was modified by another user.");
            }
            catch (Exception ex) when (IsSerializationFailure(ex))
            {
                await transaction.RollbackAsync(cancellationToken);
                return Fail<T>("ADMIN_VERSION_CONFLICT", "The change conflicted with a concurrent update; reload and retry.");
            }
        });
    }

    private static bool IsSerializationFailure(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException!)
        {
            if (current is PostgresException { SqlState: PostgresErrorCodes.SerializationFailure or PostgresErrorCodes.DeadlockDetected })
                return true;
            if (current.InnerException is null) break;
        }

        return false;
    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    /// <summary>Replay check: same key and payload returns the recorded resource; same key with another payload is rejected.</summary>
    private async Task<Result<IdempotencyRecord?>> FindReplayAsync(
        Guid organizationId, string operation, string keyHash, string payloadHash, CancellationToken cancellationToken)
    {
        var record = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
            r => r.OrganizationId == organizationId && r.Operation == operation && r.KeyHash == keyHash, cancellationToken);
        if (record is not null && record.PayloadHash != payloadHash)
            return Result<IdempotencyRecord?>.Failure(new Error(
                "IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));

        return Result<IdempotencyRecord?>.Success(record);
    }

    private async Task<Result<AssignRoleOutcome>> ReplayAssignAsync(Guid organizationId, string resourceId, CancellationToken cancellationToken)
    {
        if (resourceId.StartsWith("request:", StringComparison.Ordinal) && Guid.TryParse(resourceId["request:".Length..], out var requestId))
        {
            var row = await RoleRequestQuery(organizationId, r => r.Id == requestId).FirstOrDefaultAsync(cancellationToken);
            if (row is null) return NotFound<AssignRoleOutcome>("Role assignment request");

            var pendingUser = await ReloadUserAsync(organizationId, await MembershipUserIdAsync(row.Request.MembershipId, cancellationToken), cancellationToken);
            return pendingUser.IsFailure
                ? Result<AssignRoleOutcome>.Failure(pendingUser.Error)
                : Result<AssignRoleOutcome>.Success(new AssignRoleOutcome(true, pendingUser.Value!, ToRoleRequest(row)));
        }

        if (resourceId.StartsWith("membership:", StringComparison.Ordinal) && Guid.TryParse(resourceId["membership:".Length..], out var membershipId))
        {
            var user = await ReloadUserAsync(organizationId, await MembershipUserIdAsync(membershipId, cancellationToken), cancellationToken);
            return user.IsFailure
                ? Result<AssignRoleOutcome>.Failure(user.Error)
                : Result<AssignRoleOutcome>.Success(new AssignRoleOutcome(false, user.Value!, null));
        }

        return NotFound<AssignRoleOutcome>("Role assignment");
    }

    private async Task<Guid> MembershipUserIdAsync(Guid membershipId, CancellationToken cancellationToken) =>
        await _db.Memberships.AsNoTracking().Where(m => m.Id == membershipId).Select(m => m.UserId).FirstAsync(cancellationToken);

    private async Task<User?> FindUserInOrganizationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken) =>
        await _db.Users.FirstOrDefaultAsync(
            u => u.Id == userId && u.Memberships.Any(m => m.OrganizationId == organizationId), cancellationToken);

    private async Task<Result<AdminUser>> ReloadUserAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken)
    {
        _db.ChangeTracker.Clear();
        var user = await GetUserAsync(organizationId, userId, cancellationToken);
        return user is null ? NotFound<AdminUser>("User") : Result<AdminUser>.Success(user);
    }

    private async Task<IReadOnlyList<AdminUser>> LoadUsersAsync(Guid organizationId, IReadOnlyList<Guid> userIds, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0) return Array.Empty<AdminUser>();

        var users = await _db.Users.AsNoTracking()
            .Where(u => userIds.Contains(u.Id) && u.Memberships.Any(m => m.OrganizationId == organizationId))
            .Select(u => new { u.Id, u.DisplayName, u.Email, u.IsActive, u.FirebaseUid, u.RowVersion })
            .ToListAsync(cancellationToken);

        var memberships = await _db.Memberships.AsNoTracking()
            .Where(m => userIds.Contains(m.UserId) && m.OrganizationId == organizationId)
            .OrderBy(m => m.CreatedAtUtc).ThenBy(m => m.Id)
            .Select(m => new
            {
                m.Id,
                m.UserId,
                m.IsActive,
                m.RowVersion,
                BranchId = m.Branch != null ? (Guid?)m.Branch.Id : null,
                BranchName = m.Branch != null ? m.Branch.Name : null,
                m.StartsAtUtc,
                m.ExpiresAtUtc,
                Roles = m.MembershipRoles.OrderBy(mr => mr.Role!.Name).Select(mr => new AdminRef(mr.Role!.Id, mr.Role.Name)).ToList()
            })
            .ToListAsync(cancellationToken);

        var membershipIds = memberships.Select(m => m.Id).ToList();
        var pending = await RoleRequestQuery(
                organizationId, r => r.Status == RoleAssignmentRequestStatus.Pending && membershipIds.Contains(r.MembershipId))
            .ToListAsync(cancellationToken);
        var pendingByMembership = pending.ToLookup(x => x.Request.MembershipId);

        var byId = users.ToDictionary(u => u.Id);
        var result = new List<AdminUser>();
        foreach (var id in userIds)
        {
            if (!byId.TryGetValue(id, out var u)) continue;
            var items = memberships.Where(m => m.UserId == id).Select(m => new AdminMembership(
                m.Id,
                m.IsActive,
                m.RowVersion,
                m.BranchId.HasValue ? new AdminRef(m.BranchId.Value, m.BranchName!) : null,
                m.StartsAtUtc,
                m.ExpiresAtUtc,
                m.Roles,
                pendingByMembership[m.Id].Select(x => new AdminPendingRoleRequest(
                    x.Request.Id,
                    x.Request.RowVersion,
                    new AdminRef(x.Request.RoleId, x.RoleName),
                    new AdminRef(x.Request.RequestedByUserId, x.RequestedByName),
                    x.Request.RequestedAtUtc)).ToList())).ToList();

            var status = !u.IsActive ? "inactive" : u.FirebaseUid is null ? "pending" : "active";
            result.Add(new AdminUser(u.Id, u.DisplayName, u.Email, status, u.RowVersion, items));
        }

        return result;
    }

    private IQueryable<RoleRequestRow> RoleRequestQuery(
        Guid organizationId, System.Linq.Expressions.Expression<Func<RoleAssignmentRequest, bool>> filter) =>
        from request in _db.RoleAssignmentRequests.AsNoTracking().Where(r => r.OrganizationId == organizationId).Where(filter)
        join role in _db.Roles.AsNoTracking() on request.RoleId equals role.Id
        join membership in _db.Memberships.AsNoTracking() on request.MembershipId equals membership.Id
        join target in _db.Users.AsNoTracking() on membership.UserId equals target.Id
        join requester in _db.Users.AsNoTracking() on request.RequestedByUserId equals requester.Id
        orderby request.RequestedAtUtc, request.Id
        select new RoleRequestRow(request, role.Name, target.DisplayName, requester.DisplayName);

    private static AdminRoleRequest ToRoleRequest(RoleRequestRow row) => new(
        row.Request.Id,
        row.Request.RowVersion,
        row.Request.Status,
        new AdminRef(row.Request.RoleId, row.RoleName),
        new AdminRef(row.Request.MembershipId, row.MembershipUserName),
        new AdminRef(row.Request.RequestedByUserId, row.RequestedByName),
        row.Request.RequestedAtUtc);

    private async Task<List<PermissionGrant>> LoadPermissionsAsync(Guid membershipId, CancellationToken cancellationToken) =>
        await _db.MembershipRoles.AsNoTracking()
            .Where(mr => mr.MembershipId == membershipId && mr.Role!.IsActive)
            .SelectMany(mr => mr.Role!.RolePermissions)
            .Where(rp => rp.Permission!.IsActive)
            .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId))
            .ToListAsync(cancellationToken);

    private async Task<List<RoleWithPermissions>> LoadRolesWithPermissionsAsync(
        Guid organizationId, IReadOnlyCollection<Guid> roleIds, CancellationToken cancellationToken)
    {
        var rows = await _db.Roles.AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && r.IsActive && roleIds.Contains(r.Id))
            .Select(r => new
            {
                r.Id,
                r.Name,
                Permissions = r.RolePermissions
                    .Where(rp => rp.Permission!.IsActive)
                    .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId))
                    .ToList()
            })
            .ToListAsync(cancellationToken);
        return rows.Select(r => new RoleWithPermissions(r.Id, r.Name, r.Permissions)).ToList();
    }

    private async Task<bool> WouldRemoveLastAdministratorAsync(
        Guid organizationId, Func<AdministratorRow, bool> removed, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow;
        var rows = await _db.Memberships.AsNoTracking()
            .Where(m => m.OrganizationId == organizationId && m.IsActive)
            .Where(m => m.User!.IsActive && m.User.FirebaseUid != null && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .SelectMany(m => m.MembershipRoles
                .Where(mr => mr.Role!.IsActive)
                .Where(mr => mr.Role!.RolePermissions.Any(rp =>
                    rp.Permission!.IsActive &&
                    rp.Permission.Key == AdministrationPermissions.UsersManage &&
                    rp.Scope == PermissionScope.Organization &&
                    rp.ScopeId == organizationId))
                .Select(mr => new AdministratorRow(m.UserId, m.Id, mr.RoleId)))
            .ToListAsync(cancellationToken);

        return AdministrationPolicy.WouldRemoveLastAdministrator(rows, removed);
    }

    private void AddAudit(
        Guid organizationId, AdminActor actor, string action, string resourceType, string resourceId, string traceId,
        DateTimeOffset now, object changes, Guid? rowVersionBefore = null, Guid? rowVersionAfter = null)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            actor.UserId,
            action,
            resourceType,
            resourceId,
            now,
            traceId,
            JsonSerializer.Serialize(changes),
            actorMembershipId: actor.MembershipId,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: rowVersionAfter));
    }

    private static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> NotFound<T>(string resource) => Fail<T>("RESOURCE_NOT_FOUND", $"{resource} was not found.");

    private static Result<T> VersionConflict<T>() => Fail<T>("ADMIN_VERSION_CONFLICT", "The record was modified by another user.");
}
