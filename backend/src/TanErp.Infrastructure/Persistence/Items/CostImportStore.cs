using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Items;
using TanErp.Application.Items.Import;
using TanErp.Domain.Common;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

/// <summary>
/// Cost import. Every new row becomes a Draft Cost Record created by the importing user, so the existing maker-checker
/// (submit, a different user approves, publish) still applies. Rows are never published or superseded here and an
/// already published record is never read for update.
/// </summary>
public class CostImportStore : ICostImportStore
{
    private const string Operation = "item-costs.import";

    // Statuses where an identical row means "already imported/known"; superseded and disabled records do not block a new draft.
    private static readonly string[] LiveStatuses =
    [
        CostRecordStatus.Draft, CostRecordStatus.Submitted, CostRecordStatus.Returned, CostRecordStatus.Approved, CostRecordStatus.Published
    ];

    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public CostImportStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<ItemImportRowResult>> ValidateAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> drafts,
        CancellationToken cancellationToken = default)
    {
        var duplicates = FindDuplicateRows(drafts);
        var evaluated = await EvaluateAsync(access, drafts, duplicates, cancellationToken);
        return evaluated.Select(e => e.Result).ToList();
    }

    public async Task<Result<ItemImportBatchOutcome>> StageBatchAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> batch,
        IReadOnlySet<int> duplicateRows,
        Guid importBatchId,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var evaluated = await EvaluateAsync(access, batch, duplicateRows, cancellationToken);
        var invalid = evaluated.Where(e => !e.Result.IsValid).Select(e => e.Result).ToList();
        if (invalid.Count > 0)
        {
            return Result<ItemImportBatchOutcome>.Success(new ItemImportBatchOutcome(0, 0, 0, invalid));
        }

        var staged = await StageAsync(access, evaluated, importBatchId, traceId, _clock.UtcNow, cancellationToken);
        return staged.IsFailure
            ? Result<ItemImportBatchOutcome>.Failure(staged.Error)
            : Result<ItemImportBatchOutcome>.Success(new ItemImportBatchOutcome(staged.Value!.Created, 0, staged.Value.Unchanged, []));
    }

    public async Task<Result<ItemImportCommitResult>> CommitAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> drafts,
        string contentSha256,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var duplicates = FindDuplicateRows(drafts);

        return await SerializableTransactionRunner.RunAsync(_db, async () =>
        {
            var lockKey = $"{orgId:N}:{Operation}:{keyHash}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", cancellationToken);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == orgId && r.Operation == Operation && r.KeyHash == keyHash, cancellationToken);
            if (replay is not null)
            {
                if (replay.PayloadHash != contentSha256)
                {
                    return Result<ItemImportCommitResult>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
                }

                var batchId = Guid.TryParse(replay.ResourceId, out var parsed) ? parsed : Guid.Empty;
                var summary = batchId == Guid.Empty
                    ? null
                    : await _db.AuditEvents.AsNoTracking().FirstOrDefaultAsync(
                        a => a.OrganizationId == orgId && a.Action == "items.import" && a.ResourceId == replay.ResourceId, cancellationToken);
                var (created, unchanged) = summary is null ? (0, 0) : ReadCounts(summary.ChangesJson);
                return Result<ItemImportCommitResult>.Success(new ItemImportCommitResult(
                    batchId, created, contentSha256, Replayed: true, UpdatedCount: 0, unchanged));
            }

            var evaluated = await EvaluateAsync(access, drafts, duplicates, cancellationToken);
            if (evaluated.Any(e => !e.Result.IsValid))
            {
                return Result<ItemImportCommitResult>.Failure(
                    new Error("ITEM_IMPORT_VALIDATION_FAILED", "One or more rows are invalid; nothing was imported."));
            }

            var now = _clock.UtcNow;
            var batch = Guid.NewGuid();
            var staged = await StageAsync(access, evaluated, batch, traceId, now, cancellationToken);
            if (staged.IsFailure)
            {
                return Result<ItemImportCommitResult>.Failure(staged.Error);
            }

            var counts = staged.Value!;
            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(), orgId, access.ActorUserId, "items.import", "cost-import", batch.ToString(), now, traceId,
                JsonSerializer.Serialize(new { kind = ItemImportKind.Costs, createdCount = counts.Created, unchangedCount = counts.Unchanged, contentSha256 }),
                branchId: access.BranchId, actorMembershipId: access.MembershipId));
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, Operation, keyHash, contentSha256, batch.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return Result<ItemImportCommitResult>.Failure(
                    new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            }

            return Result<ItemImportCommitResult>.Success(new ItemImportCommitResult(
                batch, counts.Created, contentSha256, Replayed: false, UpdatedCount: 0, counts.Unchanged));
        }, "ITEM_COST_VERSION_CONFLICT", cancellationToken);
    }

    /// <summary>Rows that repeat an earlier row's identity (item, scope, branch, unit, currency, minimum quantity, start).</summary>
    public static IReadOnlySet<int> FindDuplicateRows(IEnumerable<CostImportRowDraft> drafts)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var duplicates = new HashSet<int>();
        foreach (var draft in drafts)
        {
            var (row, errors) = CostImportCsv.Validate(draft);
            if (row is null || errors.Count > 0) continue;
            var key = string.Join('|', row.ItemCode.ToUpperInvariant(), row.Scope, row.BranchCode?.ToUpperInvariant(),
                row.UnitCode.ToUpperInvariant(), row.Currency, row.MinimumQuantity.ToString("0.####"), row.EffectiveFromUtc.UtcTicks);
            if (!seen.Add(key)) duplicates.Add(row.RowNumber);
        }

        return duplicates;
    }

    private sealed record Evaluated(
        ItemImportRowResult Result, CostImportRow? Row, Guid ItemId, Guid UnitId, Guid? BranchId, Guid CostSourceId, bool IsKnown);

    private sealed record StageCounts(int Created, int Unchanged);

    private async Task<Result<StageCounts>> StageAsync(
        RequestAccessContext access,
        IReadOnlyList<Evaluated> evaluated,
        Guid importBatchId,
        string traceId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var orgId = access.OrganizationId;
        var itemIds = evaluated.Select(e => e.ItemId).Distinct().ToList();
        var versions = await _db.CostRecords.AsNoTracking()
            .Where(c => c.OrganizationId == orgId && itemIds.Contains(c.ItemId))
            .GroupBy(c => c.ItemId)
            .Select(g => new { ItemId = g.Key, Max = g.Max(c => c.Version) })
            .ToDictionaryAsync(x => x.ItemId, x => x.Max, cancellationToken);

        int created = 0, unchanged = 0;
        foreach (var entry in evaluated)
        {
            if (entry.IsKnown)
            {
                unchanged++;
                continue;
            }

            var row = entry.Row!;
            var version = versions.GetValueOrDefault(entry.ItemId) + 1;
            versions[entry.ItemId] = version;

            CostRecord record;
            try
            {
                record = CostRecord.CreateDraft(
                    Guid.NewGuid(), orgId, entry.ItemId, row.Scope, entry.BranchId, entry.UnitId, row.Currency, row.Amount,
                    row.MinimumQuantity, row.MaximumQuantity, row.EffectiveFromUtc, row.EffectiveToUtc, version,
                    entry.CostSourceId, row.SourceReference, row.Reason, null, access.ActorUserId, now);
            }
            catch (ItemValidationException ex)
            {
                _db.ChangeTracker.Clear();
                return Result<StageCounts>.Failure(new Error(ex.Code, ex.Message));
            }

            _db.CostRecords.Add(record);
            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(), orgId, access.ActorUserId, "items.create-cost-draft", "cost_record", record.Id.ToString(), now, traceId,
                JsonSerializer.Serialize(new { record.ItemId, record.Amount, record.Currency, record.Scope, record.Version, importBatchId }),
                branchId: access.BranchId, actorMembershipId: access.MembershipId, requestId: null, rowVersionAfter: record.RowVersion));
            created++;
        }

        return Result<StageCounts>.Success(new StageCounts(created, unchanged));
    }

    private async Task<IReadOnlyList<Evaluated>> EvaluateAsync(
        RequestAccessContext access,
        IReadOnlyList<CostImportRowDraft> drafts,
        IReadOnlySet<int> duplicateRows,
        CancellationToken cancellationToken)
    {
        var orgId = access.OrganizationId;
        var syntax = drafts.Select(d => (Draft: d, Check: CostImportCsv.Validate(d))).ToList();
        var rows = syntax.Where(s => s.Check.Row is not null).Select(s => s.Check.Row!).ToList();

        static string Norm(string value) => value.Trim().ToUpperInvariant();

        var itemCodes = rows.Select(r => Norm(r.ItemCode)).Distinct().ToList();
        var unitCodes = rows.Select(r => Norm(r.UnitCode)).Distinct().ToList();
        var branchCodes = rows.Where(r => r.BranchCode is not null).Select(r => Norm(r.BranchCode!)).Distinct().ToList();
        var sourceCodes = rows.Select(r => Norm(r.CostSourceCode)).Distinct().ToList();

        var items = await _db.Items.AsNoTracking()
            .Where(i => i.OrganizationId == orgId && itemCodes.Contains(i.NormalizedCode))
            .ToDictionaryAsync(i => i.NormalizedCode, i => i.Id, cancellationToken);
        var units = await _db.Units.AsNoTracking()
            .Where(u => u.OrganizationId == orgId && unitCodes.Contains(u.NormalizedCode))
            .ToDictionaryAsync(u => u.NormalizedCode, u => (u.Id, u.Status), cancellationToken);
        var branches = await _db.Branches.AsNoTracking()
            .Where(b => b.OrganizationId == orgId && branchCodes.Contains(b.Code.ToUpper()))
            .ToDictionaryAsync(b => b.Code.ToUpper(), b => (b.Id, b.IsActive), cancellationToken);
        var sources = await _db.CostSources.AsNoTracking()
            .Where(s => s.OrganizationId == orgId && sourceCodes.Contains(s.Code))
            .ToDictionaryAsync(s => s.Code, s => (s.Id, s.IsActive, s.SourceType), cancellationToken);

        var itemIds = items.Values.ToList();
        var known = await _db.CostRecords.AsNoTracking()
            .Where(c => c.OrganizationId == orgId && itemIds.Contains(c.ItemId) && LiveStatuses.Contains(c.Status))
            .Select(c => new KnownCost(c.ItemId, c.Scope, c.BranchId, c.UnitId, c.Currency, c.Amount, c.MinimumQuantity, c.MaximumQuantity, c.EffectiveFromUtc, c.EffectiveToUtc))
            .ToListAsync(cancellationToken);

        var results = new List<Evaluated>(syntax.Count);
        foreach (var (draft, check) in syntax)
        {
            var errors = new List<ItemImportRowError>(check.Errors);
            var row = check.Row;
            Guid itemId = Guid.Empty, unitId = Guid.Empty, sourceId = Guid.Empty;
            Guid? branchId = null;
            var isKnown = false;

            if (row is not null)
            {
                if (!items.TryGetValue(Norm(row.ItemCode), out itemId)) errors.Add(new("itemCode", ItemImportErrorCodes.NotFound));

                if (!units.TryGetValue(Norm(row.UnitCode), out var unit)) errors.Add(new("unitCode", ItemImportErrorCodes.NotFound));
                else if (unit.Status != ItemStatus.Active) errors.Add(new("unitCode", ItemImportErrorCodes.Inactive));
                else unitId = unit.Id;

                if (row.BranchCode is not null)
                {
                    if (!branches.TryGetValue(Norm(row.BranchCode), out var branch)) errors.Add(new("branchCode", ItemImportErrorCodes.NotFound));
                    else if (!branch.IsActive) errors.Add(new("branchCode", ItemImportErrorCodes.Inactive));
                    else if (!access.HasBranchAccess(branch.Id)) errors.Add(new("branchCode", ItemImportErrorCodes.BranchAccessDenied));
                    else branchId = branch.Id;
                }

                if (!sources.TryGetValue(Norm(row.CostSourceCode), out var source)) errors.Add(new("costSourceCode", ItemImportErrorCodes.NotFound));
                else if (!source.IsActive || source.SourceType != CostSource.ManualType) errors.Add(new("costSourceCode", ItemImportErrorCodes.Inactive));
                else sourceId = source.Id;

                if (duplicateRows.Contains(row.RowNumber)) errors.Add(new("itemCode", ItemImportErrorCodes.DuplicateCost));

                if (errors.Count == 0)
                {
                    isKnown = known.Any(k => k.ItemId == itemId && k.Scope == row.Scope && k.BranchId == branchId && k.UnitId == unitId
                        && k.Currency == row.Currency && k.Amount == row.Amount && k.MinimumQuantity == row.MinimumQuantity
                        && k.MaximumQuantity == row.MaximumQuantity && k.EffectiveFromUtc == row.EffectiveFromUtc && k.EffectiveToUtc == row.EffectiveToUtc);
                }
            }

            var valid = errors.Count == 0;
            results.Add(new Evaluated(
                new ItemImportRowResult(draft.RowNumber, draft.ItemCode, null, errors,
                    valid ? (isKnown ? ItemImportOutcomes.Unchanged : ItemImportOutcomes.Created) : null),
                valid ? row : null, itemId, unitId, branchId, sourceId, isKnown));
        }

        return results;
    }

    private sealed record KnownCost(
        Guid ItemId, string Scope, Guid? BranchId, Guid UnitId, string Currency, decimal Amount, decimal MinimumQuantity,
        decimal? MaximumQuantity, DateTimeOffset EffectiveFromUtc, DateTimeOffset? EffectiveToUtc);

    private static (int Created, int Unchanged) ReadCounts(string changesJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(changesJson);
            int Read(string name) => doc.RootElement.TryGetProperty(name, out var value) && value.TryGetInt32(out var parsed) ? parsed : 0;
            return (Read("createdCount"), Read("unchangedCount"));
        }
        catch (JsonException)
        {
            return (0, 0);
        }
    }
}
