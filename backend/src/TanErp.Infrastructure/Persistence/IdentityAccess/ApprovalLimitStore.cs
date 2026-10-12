using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.ApprovalLimits;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public sealed class ApprovalLimitStore : IApprovalLimitStore
{
    private const string VersionConflict = "ADMIN_VERSION_CONFLICT";
    private const string CreateOperation = "admin.approval-limits.define";
    private const string OrgWideIndex = "ux_approval_limits_org_wide";
    private const string BranchIndex = "ux_approval_limits_branch";

    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public ApprovalLimitStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private sealed record LimitRow(ApprovalLimit Limit, string RoleName, string? BranchName);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private Task<Result<T>> RunAsync<T>(Func<Task<Result<T>>> work, CancellationToken ct) =>
        SerializableTransactionRunner.RunAsync(_db, work, VersionConflict, ct);

    private IQueryable<LimitRow> Rows(IQueryable<ApprovalLimit> source) =>
        from limit in source
        join role in _db.Roles.AsNoTracking() on limit.RoleId equals role.Id
        join branch in _db.Branches.AsNoTracking() on limit.BranchId equals (Guid?)branch.Id into branches
        from branch in branches.DefaultIfEmpty()
        select new LimitRow(limit, role.Name, branch == null ? null : branch.Name);

    private static ApprovalLimitView ToView(LimitRow r) => new(
        r.Limit.Id, r.Limit.RowVersion, new AdminRef(r.Limit.RoleId, r.RoleName), r.Limit.DocumentType,
        r.Limit.BranchId is null ? null : new AdminRef(r.Limit.BranchId.Value, r.BranchName!), r.Limit.Currency,
        r.Limit.IsUnlimited, r.Limit.MaxAmount, r.Limit.IsActive, r.Limit.CreatedAtUtc, r.Limit.UpdatedAtUtc);

    private async Task<Result<ApprovalLimitView>> LoadAsync(Guid organizationId, Guid limitId, CancellationToken ct)
    {
        _db.ChangeTracker.Clear();
        var row = await Rows(_db.ApprovalLimits.AsNoTracking().Where(l => l.OrganizationId == organizationId && l.Id == limitId))
            .FirstOrDefaultAsync(ct);
        return row is null ? Fail<ApprovalLimitView>("RESOURCE_NOT_FOUND", "Approval limit was not found.") : Result<ApprovalLimitView>.Success(ToView(row));
    }

    private void Audit(Guid organizationId, AdminActor actor, string action, Guid limitId, string traceId, object changes, Guid? before, Guid? after) =>
        IdentityAdministrationSupport.AddAudit(_db, organizationId, actor, action, "ApprovalLimit", limitId.ToString(), traceId, _clock.UtcNow, changes, before, after);

    // ---------------------------------------------------------------- reads

    public async Task<ApprovalLimitPage> ListAsync(Guid organizationId, ApprovalLimitListQuery q, CancellationToken ct)
    {
        var query = _db.ApprovalLimits.AsNoTracking().Where(l => l.OrganizationId == organizationId);
        if (q.RoleId is not null) query = query.Where(l => l.RoleId == q.RoleId);
        if (q.DocumentType is not null) query = query.Where(l => l.DocumentType == q.DocumentType);
        query = q.Status switch
        {
            "active" => query.Where(l => l.IsActive),
            "inactive" => query.Where(l => !l.IsActive),
            _ => query
        };

        var total = await query.CountAsync(ct);
        // The id is a stable tie-breaker so paging never skips or repeats rows.
        var rows = await Rows(query)
            .OrderBy(r => r.RoleName).ThenBy(r => r.Limit.DocumentType).ThenBy(r => r.BranchName).ThenBy(r => r.Limit.Id)
            .Skip((q.Page - 1) * q.Limit).Take(q.Limit).ToListAsync(ct);
        return new ApprovalLimitPage(rows.Select(ToView).ToList(), total, q.Page, q.Limit);
    }

    public async Task<ApprovalLimitOptions> GetOptionsAsync(Guid organizationId, CancellationToken ct)
    {
        var roles = await _db.Roles.AsNoTracking().Where(r => r.OrganizationId == organizationId && r.IsActive)
            .OrderBy(r => r.NormalizedName).Select(r => new AdminRef(r.Id, r.Name)).ToListAsync(ct);
        var branches = await _db.Branches.AsNoTracking().Where(b => b.OrganizationId == organizationId && b.IsActive)
            .OrderBy(b => b.Code).Select(b => new AdminRef(b.Id, b.Name)).ToListAsync(ct);
        return new ApprovalLimitOptions(roles, branches, ApprovalDocumentType.Enforced, new[] { ApprovalLimitRules.Currency });
    }

    public Task<Result<ApprovalLimitView>> GetAsync(Guid organizationId, Guid limitId, CancellationToken ct) =>
        LoadAsync(organizationId, limitId, ct);

    // ---------------------------------------------------------------- writes

    public Task<Result<ApprovalLimitView>> CreateAsync(
        Guid organizationId, Guid roleId, string documentType, Guid? branchId, string currency, bool isUnlimited, decimal? maxAmount,
        AdminActor actor, string keyHash, string payloadHash, string traceId, CancellationToken ct) =>
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
                    ? Fail<ApprovalLimitView>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.")
                    : await LoadAsync(organizationId, Guid.Parse(replay.ResourceId), ct);
            }

            if (!await _db.Roles.AsNoTracking().AnyAsync(r => r.Id == roleId && r.OrganizationId == organizationId && r.IsActive, ct))
                return Fail<ApprovalLimitView>("APPROVAL_LIMIT_ROLE_NOT_FOUND", "An active role with this id was not found in the organization.");
            if (branchId is not null &&
                !await _db.Branches.AsNoTracking().AnyAsync(b => b.Id == branchId && b.OrganizationId == organizationId && b.IsActive, ct))
                return Fail<ApprovalLimitView>("BRANCH_NOT_FOUND", "An active branch with this id was not found in the organization.");

            var now = _clock.UtcNow;
            ApprovalLimit limit;
            try
            {
                limit = ApprovalLimit.Create(Guid.NewGuid(), organizationId, roleId, documentType, branchId, currency, isUnlimited, maxAmount, now);
            }
            catch (ApprovalLimitDomainException ex)
            {
                return Fail<ApprovalLimitView>(ex.Code, ex.Message);
            }

            _db.ApprovalLimits.Add(limit);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, CreateOperation, keyHash, payloadHash, limit.Id.ToString(), now));
            Audit(organizationId, actor, "approval-limits.created", limit.Id, traceId,
                new { roleId, documentType, branchId, currency, isUnlimited, maxAmount }, null, limit.RowVersion);

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (SerializableTransactionRunner.IsUniqueViolation(ex, OrgWideIndex)
                                               || SerializableTransactionRunner.IsUniqueViolation(ex, BranchIndex))
            {
                return Fail<ApprovalLimitView>("APPROVAL_LIMIT_ALREADY_EXISTS", "A limit already exists for this role, document type and branch.");
            }

            return await LoadAsync(organizationId, limit.Id, ct);
        }, ct);

    public Task<Result<ApprovalLimitView>> UpdateAsync(
        Guid organizationId, Guid limitId, Guid ifMatch, bool isUnlimited, decimal? maxAmount, bool isActive,
        AdminActor actor, string traceId, CancellationToken ct) =>
        RunAsync(async () =>
        {
            var limit = await _db.ApprovalLimits.FirstOrDefaultAsync(l => l.Id == limitId && l.OrganizationId == organizationId, ct);
            if (limit is null) return Fail<ApprovalLimitView>("RESOURCE_NOT_FOUND", "Approval limit was not found.");
            if (limit.RowVersion != ifMatch) return Fail<ApprovalLimitView>(VersionConflict, "The approval limit was modified by another user.");

            var before = new { limit.IsUnlimited, limit.MaxAmount, limit.IsActive };
            var versionBefore = limit.RowVersion;
            try
            {
                limit.Update(isUnlimited, maxAmount, isActive, _clock.UtcNow);
            }
            catch (ApprovalLimitDomainException ex)
            {
                return Fail<ApprovalLimitView>(ex.Code, ex.Message);
            }

            Audit(organizationId, actor, "approval-limits.updated", limitId, traceId,
                new { limit.RoleId, limit.DocumentType, limit.BranchId, limit.Currency, before, after = new { isUnlimited, maxAmount, isActive } },
                versionBefore, limit.RowVersion);
            await _db.SaveChangesAsync(ct);
            return await LoadAsync(organizationId, limitId, ct);
        }, ct);
}
