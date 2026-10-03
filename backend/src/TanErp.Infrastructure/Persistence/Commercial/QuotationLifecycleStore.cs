using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Commercial;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Estimates;
using TanErp.Infrastructure.Persistence.Estimates;

namespace TanErp.Infrastructure.Persistence.Commercial;

public class QuotationLifecycleStore : IQuotationLifecycleStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;

    public QuotationLifecycleStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private void Audit(RequestAccessContext access, string action, Guid quotationId, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "Quotation", quotationId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId));
    }

    private static string HashSnapshot(string value) => Convert.ToHexString(
        System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static bool SnapshotsMatch(string left, string right)
    {
        try
        {
            return System.Text.Json.Nodes.JsonNode.DeepEquals(
                System.Text.Json.Nodes.JsonNode.Parse(left),
                System.Text.Json.Nodes.JsonNode.Parse(right));
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public async Task<Result<QuotationHistoryProjection>> VoidAsync(
        RequestAccessContext access, Guid quotationId, Guid expectedVersion, string reason, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var quotation = await _db.Quotations.FirstOrDefaultAsync(q => q.Id == quotationId && q.OrganizationId == orgId, ct);
            if (quotation is null) return Fail<QuotationHistoryProjection>("RESOURCE_NOT_FOUND", "Quotation not found.");
            if (quotation.RowVersion != expectedVersion) return Fail<QuotationHistoryProjection>("QUOTATION_VERSION_CONFLICT", "Quotation version conflict.");

            var now = _clock.UtcNow;
            try
            {
                quotation.Void(reason, access.ActorUserId, now);
            }
            catch (QuotationLifecycleException ex)
            {
                return Fail<QuotationHistoryProjection>(ex.Code, ex.Message);
            }

            // The reason is stored on the quotation; the audit trail carries identifiers only.
            Audit(access, "quotations.voided", quotation.Id, traceId, new { quotation.Number }, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<QuotationHistoryProjection>("QUOTATION_VERSION_CONFLICT", "Quotation version conflict.");
            }

            return Result<QuotationHistoryProjection>.Success((await GetHistoryAsync(orgId, quotation.EstimateId, ct))!);
        });
    }

    public async Task<Result<QuotationHistoryProjection>> AmendAsync(
        RequestAccessContext access, Guid quotationId, Guid expectedVersion, string reason, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quotations.amend";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var lockKey = $"{orgId:N}:{operation}:{keyHash}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<QuotationHistoryProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var newId)
                    ? await _db.Quotations.AsNoTracking().Where(q => q.Id == newId && q.OrganizationId == orgId).Select(q => (Guid?)q.EstimateId).FirstOrDefaultAsync(ct)
                    : null;
                if (replayed.HasValue) return Result<QuotationHistoryProjection>.Success((await GetHistoryAsync(orgId, replayed.Value, ct))!);
            }

            var old = await _db.Quotations.FirstOrDefaultAsync(q => q.Id == quotationId && q.OrganizationId == orgId, ct);
            if (old is null) return Fail<QuotationHistoryProjection>("RESOURCE_NOT_FOUND", "Quotation not found.");
            if (old.RowVersion != expectedVersion) return Fail<QuotationHistoryProjection>("QUOTATION_VERSION_CONFLICT", "Quotation version conflict.");
            if (old.Status == QuotationStatus.Accepted) return Fail<QuotationHistoryProjection>("QUOTATION_ACCEPTED_LOCKED", "An accepted quotation cannot be amended.");
            if (old.Status != QuotationStatus.Issued) return Fail<QuotationHistoryProjection>("QUOTATION_INVALID_STATE", $"A quotation in status '{old.Status}' cannot be amended.");

            var estimate = await _db.Estimates.Include(e => e.Revisions).FirstOrDefaultAsync(e => e.Id == old.EstimateId && e.OrganizationId == orgId, ct);
            if (estimate is null) return Fail<QuotationHistoryProjection>("RESOURCE_NOT_FOUND", "Estimate not found.");

            // Quote the latest approved (or already quoted) revision: the same one for a billing/terms refresh, a newer one for a price change.
            var target = estimate.Revisions
                .Where(r => r.Status == EstimateRevisionStatus.Approved || r.Status == EstimateRevisionStatus.Quoted)
                .OrderByDescending(r => r.RevisionNo).FirstOrDefault();
            if (target is null || target.CalculationOutdated || string.IsNullOrWhiteSpace(target.CalculationSnapshotJson) || string.IsNullOrWhiteSpace(target.ApprovalSnapshotJson) || target.GrandTotal <= 0)
            {
                return Fail<QuotationHistoryProjection>("ESTIMATE_INVALID_STATE", "There is no approved estimate revision with frozen snapshots to quote.");
            }

            var calculationSnapshot = await _db.EstimateCalculationSnapshots.AsNoTracking().FirstOrDefaultAsync(
                s => s.OrganizationId == orgId && s.EstimateRevisionId == target.Id && s.CalculationVersion == target.CalculationVersion, ct);
            var approvalSnapshot = await _db.EstimateApprovalSnapshots.AsNoTracking().FirstOrDefaultAsync(
                s => s.OrganizationId == orgId && s.EstimateId == estimate.Id && s.EstimateRevisionId == target.Id, ct);
            if (calculationSnapshot is null || approvalSnapshot is null ||
                approvalSnapshot.CalculationVersion != target.CalculationVersion ||
                approvalSnapshot.CalculationInputHash != calculationSnapshot.InputHash ||
                approvalSnapshot.CalculationSnapshotHash != HashSnapshot(calculationSnapshot.SnapshotJson) ||
                !SnapshotsMatch(target.CalculationSnapshotJson, calculationSnapshot.SnapshotJson) ||
                !SnapshotsMatch(target.ApprovalSnapshotJson, approvalSnapshot.SnapshotJson))
            {
                return Fail<QuotationHistoryProjection>("ESTIMATE_INVALID_STATE", "The approved estimate snapshots do not match the current calculation.");
            }

            var billing = await QuotationBillingSnapshotBuilder.BuildAsync(_db, orgId, estimate.CustomerId, ct);
            if (billing.IsFailure) return Result<QuotationHistoryProjection>.Failure(billing.Error);

            var now = _clock.UtcNow;
            try
            {
                if (target.Status == EstimateRevisionStatus.Approved)
                {
                    // A newer revision is being quoted for the first time; it must be the estimate's current one.
                    if (estimate.CurrentRevision?.Id != target.Id) return Fail<QuotationHistoryProjection>("QUOTATION_AMEND_REVISION_PENDING", "A newer estimate revision is still being prepared; finish or cancel it first.");
                    estimate.MarkQuoted();
                }
            }
            catch (EstimateInvalidStateException ex)
            {
                return Fail<QuotationHistoryProjection>("ESTIMATE_INVALID_STATE", ex.Message);
            }

            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.Quotations, estimate.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<QuotationHistoryProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate quotation document number.");
            }

            var replacement = new Quotation(
                Guid.NewGuid(), orgId, estimate.BranchId, estimate.CustomerId, old.OpportunityId, estimate.Id, target.Id, number,
                target.GrandTotal, approvalSnapshot.CalculationSnapshotHash, now, billing.Value!.Json, billing.Value.Hash);
            try
            {
                replacement.MarkAsAmendmentOf(old.Id, reason);
                old.Supersede(replacement.Id, now);
            }
            catch (QuotationLifecycleException ex)
            {
                return Fail<QuotationHistoryProjection>(ex.Code, ex.Message);
            }

            try
            {
                // Retire the old document first so the "one live quotation per estimate" index never sees two.
                await _db.SaveChangesAsync(ct);
                _db.Quotations.Add(replacement);
                Audit(access, "quotations.amended", replacement.Id, traceId, new { replacement.Number, supersedes = old.Number, total = replacement.TotalAmount }, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, replacement.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<QuotationHistoryProjection>("QUOTATION_VERSION_CONFLICT", "Quotation version conflict.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return Fail<QuotationHistoryProjection>("QUOTATION_VERSION_CONFLICT", "The quotation was changed by someone else.");
            }

            return Result<QuotationHistoryProjection>.Success((await GetHistoryAsync(orgId, estimate.Id, ct))!);
        });
    }

    public async Task<QuotationHistoryProjection?> GetHistoryAsync(Guid organizationId, Guid estimateId, CancellationToken ct = default)
    {
        var rows = await _db.Quotations.AsNoTracking()
            .Where(q => q.OrganizationId == organizationId && q.EstimateId == estimateId)
            .OrderBy(q => q.IssuedAtUtc).ThenBy(q => q.Number).ToListAsync(ct);
        if (rows.Count == 0) return null;

        var numbers = rows.ToDictionary(q => q.Id, q => q.Number);
        var revisionIds = rows.Select(q => q.EstimateRevisionId).Distinct().ToList();
        var revisionNos = await _db.EstimateRevisions.AsNoTracking().Where(r => revisionIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, r => r.RevisionNo, ct);
        var userIds = rows.Where(q => q.VoidedByUserId.HasValue).Select(q => q.VoidedByUserId!.Value).Distinct().ToList();
        var users = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.DisplayName, ct);

        string? NumberOf(Guid? id) => id.HasValue && numbers.TryGetValue(id.Value, out var n) ? n : null;
        return new QuotationHistoryProjection(estimateId, rows.Select(q => new QuotationHistoryItem(
            q.Id, q.Number, q.Status, q.TotalAmount, q.EstimateRevisionId, revisionNos.GetValueOrDefault(q.EstimateRevisionId), q.IssuedAtUtc, q.AcceptedAtUtc,
            q.SupersedesQuotationId, NumberOf(q.SupersedesQuotationId), q.SupersededByQuotationId, NumberOf(q.SupersededByQuotationId), q.AmendmentReason,
            q.VoidedAtUtc, q.VoidedByUserId.HasValue ? new QuotationPersonRef(q.VoidedByUserId.Value, users.GetValueOrDefault(q.VoidedByUserId.Value) ?? string.Empty) : null,
            q.VoidReason, q.RowVersion)).ToList());
    }
}
