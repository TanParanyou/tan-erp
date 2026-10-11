using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public sealed class RoleManagementStore : IRoleManagementStore
{
    private const string VersionConflict = "ADMIN_VERSION_CONFLICT";

    private const string CreateOperation = "admin.roles.define";
    private const string NameIndex = "ix_roles_organization_id_normalized_role_name";
    private const string PendingChangeIndex = "ux_role_change_requests_pending";

    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public RoleManagementStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> NotFound<T>(string resource) => Fail<T>("RESOURCE_NOT_FOUND", $"{resource} was not found.");

    private Task<Result<T>> RunAsync<T>(Func<Task<Result<T>>> work, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, work, VersionConflict, ct);

    // ---------------------------------------------------------------- reads

    public async Task<IReadOnlyList<PermissionCatalogEntry>> ListPermissionsAsync(CancellationToken ct)
    {
        var rows = await _db.Permissions.AsNoTracking().Where(p => p.IsActive)
            .Select(p => new { p.Key, p.Description }).ToListAsync(ct);
        // Ordinal order in memory: Postgres collation would order '-' and '.' differently from the documented key order.
        return rows.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => new PermissionCatalogEntry(p.Key, p.Description, AdministrationPolicy.ApprovalPermissionKeys.Contains(p.Key))).ToList();
    }

    public async Task<RoleDefinitionPage> ListRolesAsync(Guid organizationId, RoleListQuery q, CancellationToken ct)
    {
        var query = _db.Roles.AsNoTracking().Where(r => r.OrganizationId == organizationId);
        if (q.Search is not null)
        {
            var pattern = $"%{IdentityAdministrationSupport.EscapeLike(q.Search)}%";
            query = query.Where(r => EF.Functions.ILike(r.Name, pattern, "\\") ||
                                     (r.Description != null && EF.Functions.ILike(r.Description, pattern, "\\")));
        }

        query = q.Status switch
        {
            "active" => query.Where(r => r.IsActive),
            "inactive" => query.Where(r => !r.IsActive),
            _ => query
        };

        var ordered = (q.SortBy, q.Descending) switch
        {
            (RoleSortKey.CreatedAt, false) => query.OrderBy(r => r.CreatedAtUtc),
            (RoleSortKey.CreatedAt, true) => query.OrderByDescending(r => r.CreatedAtUtc),
            (_, true) => query.OrderByDescending(r => r.NormalizedName),
            _ => query.OrderBy(r => r.NormalizedName)
        };

        var total = await query.CountAsync(ct);
        // The id is a stable tie-breaker so paging never skips or repeats rows.
        var rows = await ordered.ThenBy(r => r.Id).Skip((q.Page - 1) * q.Limit).Take(q.Limit)
            .Select(r => new
            {
                r.Id, r.Name, r.Description, r.IsSystem, r.IsActive, r.RowVersion,
                MemberCount = r.MembershipRoles.Count(),
                Keys = r.RolePermissions.Where(rp => rp.Permission!.IsActive).Select(rp => rp.Permission!.Key).ToList(),
                HasPending = _db.RoleChangeRequests.Any(c => c.RoleId == r.Id && c.Status == RoleAssignmentRequestStatus.Pending)
            })
            .ToListAsync(ct);

        var items = rows.Select(r => new RoleDefinitionSummary(
            r.Id, r.Name, r.Description, r.IsSystem, r.IsActive, r.RowVersion, r.MemberCount, r.Keys.Distinct().Count(),
            AdministrationPolicy.RequiresApproval(r.Keys), r.HasPending)).ToList();
        return new RoleDefinitionPage(items, total, q.Page, q.Limit);
    }

    public Task<Result<RoleDefinition>> GetRoleAsync(Guid organizationId, Guid roleId, CancellationToken ct) =>
        LoadDefinitionAsync(organizationId, roleId, ct);

    // ---------------------------------------------------------------- shared helpers (used by the write members in Tasks 7-10)

    private async Task<Result<RoleDefinition>> LoadDefinitionAsync(Guid organizationId, Guid roleId, CancellationToken ct)
    {
        _db.ChangeTracker.Clear();
        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
        if (role is null) return NotFound<RoleDefinition>("Role");

        var grants = await LoadGrantViewsAsync(roleId, ct);
        var members = await _db.MembershipRoles.AsNoTracking().CountAsync(mr => mr.RoleId == roleId, ct);
        var pendingAssignments = await _db.RoleAssignmentRequests.AsNoTracking()
            .CountAsync(r => r.RoleId == roleId && r.Status == RoleAssignmentRequestStatus.Pending, ct);
        var pendingRow = await ChangeRequestQuery(organizationId, c => c.RoleId == roleId && c.Status == RoleAssignmentRequestStatus.Pending)
            .FirstOrDefaultAsync(ct);

        return Result<RoleDefinition>.Success(new RoleDefinition(
            role.Id, role.Name, role.Description, role.IsSystem, role.IsActive, role.RowVersion, role.CreatedAtUtc, members, pendingAssignments,
            AdministrationPolicy.RequiresApproval(grants.Select(g => g.PermissionKey)), grants,
            pendingRow is null ? null : ToChangeView(organizationId, pendingRow, grants)));
    }

    private async Task<List<RoleGrantView>> LoadGrantViewsAsync(Guid roleId, CancellationToken ct)
    {
        var rows = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == roleId && rp.Permission!.IsActive)
            .OrderBy(rp => rp.Permission!.Key).ThenBy(rp => rp.Scope)
            .Select(rp => new
            {
                Key = rp.Permission!.Key,
                rp.Scope,
                rp.BranchId,
                BranchName = _db.Branches.Where(b => b.Id == rp.BranchId).Select(b => b.Name).FirstOrDefault()
            })
            .ToListAsync(ct);
        return rows.Select(r => new RoleGrantView(r.Key, r.Scope, r.BranchId is null ? null : new AdminRef(r.BranchId.Value, r.BranchName!))).ToList();
    }

    private sealed record ChangeRow(RoleChangeRequest Request, string RoleName, string RequestedByName);

    private IQueryable<ChangeRow> ChangeRequestQuery(Guid organizationId, Expression<Func<RoleChangeRequest, bool>> filter) =>
        from request in _db.RoleChangeRequests.AsNoTracking().Where(c => c.OrganizationId == organizationId).Where(filter)
        join role in _db.Roles.AsNoTracking() on request.RoleId equals role.Id
        join requester in _db.Users.AsNoTracking() on request.RequestedByUserId equals requester.Id
        orderby request.RequestedAtUtc, request.Id
        select new ChangeRow(request, role.Name, requester.DisplayName);

    private static List<RoleGrantInput> ParseGrants(string json) =>
        JsonSerializer.Deserialize<List<RoleGrantInput>>(json) ?? new List<RoleGrantInput>();

    /// <summary>Added/removed permission keys are computed against the role as it is now, so the checker sees the real effect.</summary>
    private static RoleChangeRequestView ToChangeView(Guid organizationId, ChangeRow row, IReadOnlyList<RoleGrantView> currentGrants)
    {
        var current = currentGrants.Select(g => RoleManagementPolicy.ToGrant(organizationId, g.PermissionKey, g.Scope, g.Branch?.Id)).ToList();
        var proposed = ParseGrants(row.Request.ProposedGrantsJson)
            .Select(g => RoleManagementPolicy.ToGrant(organizationId, g.PermissionKey, g.Scope, g.BranchId)).ToList();
        var (added, removed) = RoleManagementPolicy.Diff(current, proposed);

        return new RoleChangeRequestView(
            row.Request.Id, row.Request.RowVersion, row.Request.Status,
            new AdminRef(row.Request.RoleId, row.RoleName), new AdminRef(row.Request.RequestedByUserId, row.RequestedByName),
            row.Request.RequestedAtUtc, row.Request.ProposedName, row.Request.ProposedDescription,
            added.Select(g => g.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList(),
            removed.Select(g => g.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList());
    }

    // ---------------------------------------------------------------- create

    private sealed record ResolvedGrant(Guid PermissionId, string Key, string Scope, Guid? BranchId);

    private static RolePermission NewGrant(Guid roleId, Guid organizationId, ResolvedGrant g, DateTimeOffset now) =>
        new(Guid.NewGuid(), roleId, organizationId, g.PermissionId, g.Scope, scopeId: null, branchId: g.BranchId, assignedAtUtc: now);

    private static List<string> KeysOf(IEnumerable<PermissionGrant> grants) =>
        grants.Select(g => g.Key).Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList();

    private void Audit(
        Guid organizationId, AdminActor actor, string action, string resourceType, Guid resourceId, string traceId,
        object changes, Guid? versionBefore = null, Guid? versionAfter = null) =>
        IdentityAdministrationSupport.AddAudit(
            _db, organizationId, actor, action, resourceType, resourceId.ToString(), traceId, _clock.UtcNow, changes, versionBefore, versionAfter);

    /// <summary>Checks every key against the active catalog and every branch against the organization.</summary>
    private async Task<Result<List<ResolvedGrant>>> ResolveGrantsAsync(Guid organizationId, IReadOnlyList<RoleGrantInput> inputs, CancellationToken ct)
    {
        var keys = inputs.Select(g => g.PermissionKey).Distinct().ToList();
        var permissions = await _db.Permissions.AsNoTracking()
            .Where(p => p.IsActive && keys.Contains(p.Key)).ToDictionaryAsync(p => p.Key, p => p.Id, ct);
        if (permissions.Count != keys.Count)
            return Fail<List<ResolvedGrant>>("ROLE_PERMISSION_UNKNOWN", "A permission key is not in the active catalog.");

        var branchIds = inputs.Where(g => g.BranchId.HasValue).Select(g => g.BranchId!.Value).Distinct().ToList();
        var branchCount = await _db.Branches.AsNoTracking()
            .CountAsync(b => b.OrganizationId == organizationId && b.IsActive && branchIds.Contains(b.Id), ct);
        if (branchCount != branchIds.Count) return Fail<List<ResolvedGrant>>("BRANCH_NOT_FOUND", "Branch not found.");

        return Result<List<ResolvedGrant>>.Success(
            inputs.Select(g => new ResolvedGrant(permissions[g.PermissionKey], g.PermissionKey, g.Scope, g.BranchId)).ToList());
    }

    public Task<Result<RoleDefinition>> CreateRoleAsync(
        Guid organizationId, RoleInput input, AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            // Taken INSIDE the transaction so concurrent requests with the same key serialize and the loser replays.
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({organizationId.ToString("N") + ":" + CreateOperation + ":" + keyHash}, 0))", ct);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == organizationId && r.Operation == CreateOperation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                return replay.PayloadHash != payloadHash
                    ? Fail<RoleDefinition>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.")
                    : await LoadDefinitionAsync(organizationId, Guid.Parse(replay.ResourceId), ct);
            }

            var resolved = await ResolveGrantsAsync(organizationId, input.Permissions, ct);
            if (resolved.IsFailure) return Result<RoleDefinition>.Failure(resolved.Error);

            // Anti-escalation: the caller may only hand out permissions it holds (CP-02 rule, same policy).
            var held = await IdentityAdministrationSupport.LoadHeldPermissionsAsync(_db, actor.MembershipId, ct);
            var wanted = resolved.Value!.Select(g => RoleManagementPolicy.ToGrant(organizationId, g.Key, g.Scope, g.BranchId)).ToList();
            if (!AdministrationPolicy.Covers(held, organizationId, wanted))
                return Fail<RoleDefinition>("ROLE_ESCALATION_DENIED", "The role grants permissions the caller does not hold.");

            var now = _clock.UtcNow;
            Role role;
            try
            {
                role = Role.CreateCustom(Guid.NewGuid(), organizationId, input.Name, input.Description, now);
            }
            catch (RoleDomainException ex)
            {
                return Fail<RoleDefinition>(ex.Code, ex.Message);
            }

            _db.Roles.Add(role);
            foreach (var grant in resolved.Value!) _db.RolePermissions.Add(NewGrant(role.Id, organizationId, grant, now));
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, CreateOperation, keyHash, payloadHash, role.Id.ToString(), now));
            Audit(organizationId, actor, "roles.created", "Role", role.Id, traceId,
                new { permissionKeys = KeysOf(wanted), requiresApproval = AdministrationPolicy.RequiresApproval(wanted.Select(g => g.Key)) }, null, role.RowVersion);

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, NameIndex))
            {
                return Fail<RoleDefinition>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");
            }

            return await LoadDefinitionAsync(organizationId, role.Id, ct);
        }, ct);

    // ---------------------------------------------------------------- update

    private sealed record ChangePlan(List<ResolvedGrant> Wanted, IReadOnlyList<PermissionGrant> Added, IReadOnlyList<PermissionGrant> Removed);

    private static PermissionGrant ToGrant(Guid organizationId, ResolvedGrant grant) =>
        RoleManagementPolicy.ToGrant(organizationId, grant.Key, grant.Scope, grant.BranchId);

    private Task<bool> HasPendingChangeAsync(Guid roleId, CancellationToken ct) =>
        _db.RoleChangeRequests.AsNoTracking().AnyAsync(c => c.RoleId == roleId && c.Status == RoleAssignmentRequestStatus.Pending, ct);

    private Task<bool> NameTakenAsync(Guid organizationId, Guid roleId, string name, CancellationToken ct)
    {
        var normalized = name.ToUpperInvariant();
        return _db.Roles.AsNoTracking().AnyAsync(r => r.OrganizationId == organizationId && r.NormalizedName == normalized && r.Id != roleId, ct);
    }

    /// <summary>
    /// Diffs the wanted grants against the role and runs the invariants for whoever applies the change
    /// (the editor now, or the checker at approval): anti-escalation on ADDED grants and the last-administrator guard.
    /// <paramref name="role"/> must be loaded with its RolePermissions and Permission.
    /// </summary>
    private async Task<Result<ChangePlan>> PlanChangeAsync(
        Role role, Guid organizationId, IReadOnlyList<RoleGrantInput> inputs, AdminActor actor, CancellationToken ct)
    {
        var resolved = await ResolveGrantsAsync(organizationId, inputs, ct);
        if (resolved.IsFailure) return Result<ChangePlan>.Failure(resolved.Error);

        var current = role.RolePermissions.Where(rp => rp.Permission!.IsActive)
            .Select(rp => new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId)).ToList();
        var (added, removed) = RoleManagementPolicy.Diff(current, resolved.Value!.Select(g => ToGrant(organizationId, g)).ToList());

        var held = await IdentityAdministrationSupport.LoadHeldPermissionsAsync(_db, actor.MembershipId, ct);
        if (!AdministrationPolicy.Covers(held, organizationId, added))
            return Fail<ChangePlan>("ROLE_ESCALATION_DENIED", "The change adds permissions the caller does not hold.");

        if (removed.Any(g => g.Key == AdministrationPermissions.UsersManage && g.Scope == PermissionScope.Organization))
        {
            var rows = await IdentityAdministrationSupport.LoadAdministratorRowsAsync(_db, organizationId, _clock.UtcNow, ct);
            if (AdministrationPolicy.WouldRemoveLastAdministrator(rows, row => row.RoleId == role.Id))
                return Fail<ChangePlan>("LAST_ADMINISTRATOR_REQUIRED", "The organization must keep at least one active administrator.");
        }

        return Result<ChangePlan>.Success(new ChangePlan(resolved.Value!, added, removed));
    }

    /// <summary>Applies name/description and the grant diff; throws <see cref="RoleDomainException"/> for a system role or bad name.</summary>
    private void ApplyDefinition(Role role, Guid organizationId, string name, string? description, ChangePlan plan, DateTimeOffset now)
    {
        role.UpdateDetails(name, description);
        var removable = role.RolePermissions
            .Where(rp => rp.Permission!.IsActive && plan.Removed.Contains(new PermissionGrant(rp.Permission!.Key, rp.Scope, rp.ScopeId)))
            .ToList();
        foreach (var grant in removable) _db.RolePermissions.Remove(grant);
        foreach (var grant in plan.Wanted.Where(g => plan.Added.Contains(ToGrant(organizationId, g))))
            _db.RolePermissions.Add(NewGrant(role.Id, organizationId, grant, now));
    }

    public Task<Result<RoleUpdateOutcome>> UpdateRoleAsync(
        Guid organizationId, Guid roleId, Guid ifMatch, RoleInput input, AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var role = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
            if (role is null) return NotFound<RoleUpdateOutcome>("Role");
            if (role.RowVersion != ifMatch) return Fail<RoleUpdateOutcome>(VersionConflict, "The role was modified by another user.");
            if (role.IsSystem) return Fail<RoleUpdateOutcome>("ROLE_SYSTEM_IMMUTABLE", "A system role cannot be changed.");
            if (await HasPendingChangeAsync(roleId, ct))
                return Fail<RoleUpdateOutcome>("ROLE_CHANGE_REQUEST_PENDING", "A change request is already pending for this role.");

            string name;
            string? description;
            try
            {
                (name, description) = RoleRules.Normalize(input.Name, input.Description);
            }
            catch (RoleDomainException ex)
            {
                return Fail<RoleUpdateOutcome>(ex.Code, ex.Message);
            }

            var plan = await PlanChangeAsync(role, organizationId, input.Permissions, actor, ct);
            if (plan.IsFailure) return Result<RoleUpdateOutcome>.Failure(plan.Error);

            var now = _clock.UtcNow;
            var addedKeys = KeysOf(plan.Value!.Added);
            var removedKeys = KeysOf(plan.Value.Removed);
            var hasHolders = await _db.MembershipRoles.AnyAsync(mr => mr.RoleId == roleId, ct);

            if (RoleManagementPolicy.NeedsChecker(hasHolders, plan.Value.Wanted.Select(g => g.Key), plan.Value.Added))
            {
                if (await NameTakenAsync(organizationId, roleId, name, ct))
                    return Fail<RoleUpdateOutcome>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");

                var request = new RoleChangeRequest(
                    Guid.NewGuid(), organizationId, roleId, actor.UserId, now, role.RowVersion, name, description,
                    JsonSerializer.Serialize(plan.Value.Wanted.Select(g => new RoleGrantInput(g.Key, g.Scope, g.BranchId)).ToList()));
                _db.RoleChangeRequests.Add(request);
                Audit(organizationId, actor, "roles.change-requested", "RoleChangeRequest", request.Id, traceId,
                    new { roleId, addedPermissionKeys = addedKeys, removedPermissionKeys = removedKeys }, null, request.RowVersion);
                try
                {
                    await _db.SaveChangesAsync(ct);
                }
                catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, PendingChangeIndex))
                {
                    // A concurrent editor created the pending request between our check and insert.
                    return Fail<RoleUpdateOutcome>("ROLE_CHANGE_REQUEST_PENDING", "A change request is already pending for this role.");
                }

                var pending = await LoadDefinitionAsync(organizationId, roleId, ct);
                return Result<RoleUpdateOutcome>.Success(new RoleUpdateOutcome(true, pending.Value!, pending.Value!.PendingChange));
            }

            var before = role.RowVersion;
            var changedFields = new List<string>();
            if (role.Name != name) changedFields.Add("name");
            if (role.Description != description) changedFields.Add("description");
            try
            {
                ApplyDefinition(role, organizationId, name, description, plan.Value, now);
            }
            catch (RoleDomainException ex)
            {
                return Fail<RoleUpdateOutcome>(ex.Code, ex.Message);
            }

            Audit(organizationId, actor, "roles.updated", "Role", roleId, traceId,
                new { changedFields, addedPermissionKeys = addedKeys, removedPermissionKeys = removedKeys }, before, role.RowVersion);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, NameIndex))
            {
                return Fail<RoleUpdateOutcome>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");
            }

            var updated = await LoadDefinitionAsync(organizationId, roleId, ct);
            return Result<RoleUpdateOutcome>.Success(new RoleUpdateOutcome(false, updated.Value!, null));
        }, ct);

    // ---------------------------------------------------------------- change request decisions (maker-checker)

    public async Task<IReadOnlyList<RoleChangeRequestView>> ListChangeRequestsAsync(Guid organizationId, string status, CancellationToken ct)
    {
        var rows = await ChangeRequestQuery(organizationId, c => c.Status == status).Take(200).ToListAsync(ct);
        var views = new List<RoleChangeRequestView>(rows.Count);
        foreach (var row in rows)
            views.Add(ToChangeView(organizationId, row, await LoadGrantViewsAsync(row.Request.RoleId, ct)));
        return views;
    }

    public Task<Result<RoleChangeRequestView>> DecideChangeRequestAsync(
        Guid organizationId, Guid requestId, Guid ifMatch, RoleRequestDecision decision, AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var request = await _db.RoleChangeRequests.FirstOrDefaultAsync(c => c.Id == requestId && c.OrganizationId == organizationId, ct);
            if (request is null) return NotFound<RoleChangeRequestView>("Role change request");
            if (request.RowVersion != ifMatch) return Fail<RoleChangeRequestView>(VersionConflict, "The request was modified by another user.");
            if (!request.IsPending)
                return Fail<RoleChangeRequestView>("ROLE_CHANGE_REQUEST_NOT_PENDING", "The request has already been decided.");

            var now = _clock.UtcNow;
            var before = request.RowVersion;
            var roleId = request.RoleId;
            // Grants before the change is applied; a decided approval reports the diff that was actually applied.
            IReadOnlyList<RoleGrantView>? baseline = null;
            string action;
            object changes = new { roleId };

            if (decision == RoleRequestDecision.Cancel)
            {
                if (!request.TryCancel(actor.UserId, now))
                    return Fail<RoleChangeRequestView>("PERMISSION_DENIED", "Only the requester can cancel a request.");
                action = "roles.change-cancelled";
            }
            else
            {
                if (actor.UserId == request.RequestedByUserId)
                    return Fail<RoleChangeRequestView>("ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED", "The decision must be made by someone other than the requester.");

                if (decision == RoleRequestDecision.Reject)
                {
                    request.TryReject(actor.UserId, now);
                    action = "roles.change-rejected";
                }
                else
                {
                    var role = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                        .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
                    if (role is null) return NotFound<RoleChangeRequestView>("Role");
                    if (role.RowVersion != request.BaseRoleRowVersion)
                        return Fail<RoleChangeRequestView>("ROLE_CHANGE_REQUEST_STALE", "The role changed after the request was made.");

                    // The checker hands out the added grants, so anti-escalation and the last-administrator guard run against the CHECKER.
                    var plan = await PlanChangeAsync(role, organizationId, ParseGrants(request.ProposedGrantsJson), actor, ct);
                    if (plan.IsFailure) return Result<RoleChangeRequestView>.Failure(plan.Error);

                    baseline = await LoadGrantViewsAsync(roleId, ct);
                    try
                    {
                        ApplyDefinition(role, organizationId, request.ProposedName, request.ProposedDescription, plan.Value!, now);
                    }
                    catch (RoleDomainException ex)
                    {
                        return Fail<RoleChangeRequestView>(ex.Code, ex.Message);
                    }

                    request.TryApprove(actor.UserId, now);
                    action = "roles.change-approved";
                    changes = new { roleId, addedPermissionKeys = KeysOf(plan.Value!.Added), removedPermissionKeys = KeysOf(plan.Value.Removed) };
                }
            }

            Audit(organizationId, actor, action, "RoleChangeRequest", request.Id, traceId, changes, before, request.RowVersion);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, NameIndex))
            {
                return Fail<RoleChangeRequestView>("ROLE_NAME_ALREADY_EXISTS", "A role with this name already exists.");
            }

            _db.ChangeTracker.Clear();
            var row = await ChangeRequestQuery(organizationId, c => c.Id == requestId).FirstAsync(ct);
            var grants = baseline ?? await LoadGrantViewsAsync(roleId, ct);
            return Result<RoleChangeRequestView>.Success(ToChangeView(organizationId, row, grants));
        }, ct);

    // ---------------------------------------------------------------- activation

    public Task<Result<RoleDefinition>> SetRoleActiveAsync(
        Guid organizationId, Guid roleId, bool active, Guid ifMatch, AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId, ct);
            if (role is null) return NotFound<RoleDefinition>("Role");
            if (role.RowVersion != ifMatch) return Fail<RoleDefinition>(VersionConflict, "The role was modified by another user.");
            if (role.IsSystem) return Fail<RoleDefinition>("ROLE_SYSTEM_IMMUTABLE", "A system role cannot be changed.");
            if (role.IsActive == active) return await LoadDefinitionAsync(organizationId, roleId, ct);

            if (!active)
            {
                // Deactivation silently revokes the role from every holder (an inactive role grants nothing), so it is refused while in use.
                if (await HasPendingChangeAsync(roleId, ct))
                    return Fail<RoleDefinition>("ROLE_CHANGE_REQUEST_PENDING", "A change request is pending for this role.");
                var inUse = await _db.MembershipRoles.AnyAsync(mr => mr.RoleId == roleId, ct)
                    || await _db.RoleAssignmentRequests.AnyAsync(r => r.RoleId == roleId && r.Status == RoleAssignmentRequestStatus.Pending, ct);
                if (inUse) return Fail<RoleDefinition>("ROLE_IN_USE", "The role is held by users or has pending assignment requests.");
            }

            var before = role.RowVersion;
            if (active) role.Activate(); else role.Deactivate();
            Audit(organizationId, actor, active ? "roles.activated" : "roles.deactivated", "Role", roleId, traceId, new { roleId }, before, role.RowVersion);
            await _db.SaveChangesAsync(ct);
            return await LoadDefinitionAsync(organizationId, roleId, ct);
        }, ct);
}
