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
}
