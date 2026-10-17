using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.QuickEstimates;
using TanErp.Domain.Common;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Infrastructure.Persistence.QuickEstimates;

public class BranchRateOverrideStore : IBranchRateOverrideStore
{
    private const string VersionConflict = "PRICING_TEMPLATE_VERSION_CONFLICT";
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public BranchRateOverrideStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private DateOnly Today => DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime);

    // Audit rows carry ids and keys only (no PII) and are saved in the same transaction as the change.
    private void Audit(RequestAccessContext access, string action, Guid id, string traceId, object changes, DateTimeOffset now) =>
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "BranchRateOverride", id.ToString(), now, traceId,
            JsonSerializer.Serialize(changes), branchId: access.BranchId, actorMembershipId: access.MembershipId));

    private async Task<List<BranchOverrideProjection>> ProjectAsync(IReadOnlyList<BranchRateOverride> rows, CancellationToken ct)
    {
        var userIds = rows.SelectMany(o => new Guid?[] { o.RequestedByUserId, o.DecidedByUserId }).Where(i => i.HasValue).Select(i => i!.Value).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new QePerson(u.Id, u.DisplayName), ct);
        var branchIds = rows.Select(o => o.BranchId).Distinct().ToList();
        var codes = await _db.Branches.AsNoTracking().Where(b => branchIds.Contains(b.Id)).ToDictionaryAsync(b => b.Id, b => b.Code, ct);
        return rows.Select(o => new BranchOverrideProjection(o.Id, o.TemplateId, o.BranchId, codes[o.BranchId], o.Status, o.ReferenceRate, o.MinimumCharge, o.EffectiveFrom, o.EffectiveTo,
            o.ReasonCode, o.ReasonDetail, o.SourceReference, people[o.RequestedByUserId], o.RequestedAtUtc, o.DecidedByUserId.HasValue ? people[o.DecidedByUserId.Value] : null, o.DecidedAtUtc, o.DecisionNote, o.RowVersion)).ToList();
    }

    public async Task<IReadOnlyList<BranchOverrideProjection>> ListAsync(RequestAccessContext access, Guid templateId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var branch = access.BranchId;
        // A branch-bound membership sees only its own branch; an organization-wide membership sees every branch of its organization.
        var rows = await _db.BranchRateOverrides.AsNoTracking().Where(o => o.OrganizationId == orgId && o.TemplateId == templateId && (branch == null || o.BranchId == branch))
            .OrderByDescending(o => o.RequestedAtUtc).ToListAsync(ct);
        return await ProjectAsync(rows, ct);
    }

    public Task<Result<BranchOverrideProjection>> RequestAsync(
        RequestAccessContext access, Guid templateId, BranchOverrideInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quick-estimate.branch-override.request";
        var orgId = access.OrganizationId;
        // The branch always comes from the caller's membership; the request body has no branch field.
        if (!access.BranchId.HasValue) return Task.FromResult(Fail<BranchOverrideProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required."));
        var branchId = access.BranchId.Value;
        return SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            var keyLock = $"{orgId:N}:{operation}:{keyHash}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({keyLock}, 0))", ct);
            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<BranchOverrideProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var existing = Guid.TryParse(replay.ResourceId, out var existingId) ? await _db.BranchRateOverrides.AsNoTracking().FirstOrDefaultAsync(o => o.Id == existingId && o.OrganizationId == orgId, ct) : null;
                if (existing is not null) return Result<BranchOverrideProjection>.Success((await ProjectAsync(new[] { existing }, ct))[0]);
            }

            var template = await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == templateId && t.OrganizationId == orgId, ct);
            if (template is null) return Fail<BranchOverrideProjection>("RESOURCE_NOT_FOUND", "Pricing template not found.");
            if (!TemplateStatus.IsUsable(template.Status)) return Fail<BranchOverrideProjection>("PRICING_TEMPLATE_NOT_USABLE", "The pricing template is not usable.");

            // Serializes concurrent requests for the same branch and template so the overlap check below cannot race.
            var scopeLock = $"{orgId:N}:branch-override:{templateId:N}:{branchId:N}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({scopeLock}, 0))", ct);

            var now = _clock.UtcNow;
            BranchRateOverride entity;
            try
            {
                entity = new BranchRateOverride(Guid.NewGuid(), orgId, branchId, templateId, input.ReferenceRate, input.MinimumCharge, input.EffectiveFrom, input.EffectiveTo,
                    input.ReasonCode, input.ReasonDetail, input.SourceReference, access.ActorUserId, Today, now);
            }
            catch (QuickEstimateException ex)
            {
                return Fail<BranchOverrideProjection>(ex.Code, ex.Message);
            }

            var from = entity.EffectiveFrom;
            var to = entity.EffectiveTo;
            var overlaps = await _db.BranchRateOverrides.AsNoTracking().AnyAsync(o => o.OrganizationId == orgId && o.TemplateId == templateId && o.BranchId == branchId
                && (o.Status == OverrideStatus.Requested || o.Status == OverrideStatus.Approved) && o.EffectiveFrom <= to && from <= o.EffectiveTo, ct);
            if (overlaps) return Fail<BranchOverrideProjection>("BRANCH_OVERRIDE_OVERLAP", "An override request for this branch and template already covers part of this period.");

            _db.BranchRateOverrides.Add(entity);
            Audit(access, "branch-override.requested", entity.Id, traceId, new { templateId, branchId, entity.EffectiveFrom, entity.EffectiveTo }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, entity.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            return Result<BranchOverrideProjection>.Success((await ProjectAsync(new[] { entity }, ct))[0]);
        }, VersionConflict, ct);
    }

    private Task<Result<BranchOverrideProjection>> MutateAsync(
        RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, Func<BranchRateOverride, DateTimeOffset, Task<string>> mutate, string traceId, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        return SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            var row = await _db.BranchRateOverrides.FirstOrDefaultAsync(o => o.Id == overrideId && o.TemplateId == templateId && o.OrganizationId == orgId, ct);
            // Another branch's override is indistinguishable from a missing one.
            if (row is null || !access.HasBranchAccess(row.BranchId)) return Fail<BranchOverrideProjection>("RESOURCE_NOT_FOUND", "Branch override not found.");
            if (row.RowVersion != expectedVersion) return Fail<BranchOverrideProjection>(VersionConflict, "Branch override version conflict.");
            var now = _clock.UtcNow;
            string audit;
            try
            {
                audit = await mutate(row, now);
            }
            catch (QuickEstimateException ex)
            {
                return Fail<BranchOverrideProjection>(ex.Code, ex.Message);
            }

            Audit(access, audit, row.Id, traceId, new { templateId, row.BranchId, status = row.Status }, now);
            await _db.SaveChangesAsync(ct);
            return Result<BranchOverrideProjection>.Success((await ProjectAsync(new[] { row }, ct))[0]);
        }, VersionConflict, ct);
    }

    public Task<Result<BranchOverrideProjection>> DecideAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, bool approve, string? note, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, templateId, overrideId, expectedVersion, async (o, now) =>
        {
            if (approve && !TemplateStatus.IsUsable((await _db.PricingTemplates.AsNoTracking().FirstAsync(t => t.Id == o.TemplateId, ct)).Status)) throw new QuickEstimateException("PRICING_TEMPLATE_NOT_USABLE", "The pricing template is not usable.");
            o.Decide(approve, note, access.ActorUserId, Today, now);
            return approve ? "branch-override.approved" : "branch-override.returned";
        }, traceId, ct);

    public Task<Result<BranchOverrideProjection>> RevokeAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, string? reason, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, templateId, overrideId, expectedVersion, (o, now) =>
        {
            o.Revoke(reason, access.ActorUserId, now);
            return Task.FromResult("branch-override.revoked");
        }, traceId, ct);
}
