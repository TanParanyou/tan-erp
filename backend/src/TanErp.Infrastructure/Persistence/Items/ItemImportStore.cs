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
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Items;
using TanErp.Infrastructure.Persistence.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Items;

public class ItemImportStore : IItemImportStore
{
    private const string Operation = "item-master.item.import";

    private readonly AppDbContext _db;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly IClock _clock;

    public ItemImportStore(AppDbContext db, IDocumentNumberGenerator documentNumberGenerator, IClock clock)
    {
        _db = db;
        _documentNumberGenerator = documentNumberGenerator;
        _clock = clock;
    }

    public async Task<IReadOnlyList<ItemImportRowResult>> ValidateAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string mode,
        CancellationToken cancellationToken = default)
    {
        var evaluated = await EvaluateAsync(access.OrganizationId, drafts, mode, null, forWrite: false, cancellationToken);
        return evaluated.Select(e => e.Result).ToList();
    }

    public async Task<Result<ItemImportBatchOutcome>> StageBatchAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> batch,
        string mode,
        IReadOnlySet<int> duplicateRows,
        Guid importBatchId,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var evaluated = await EvaluateAsync(access.OrganizationId, batch, mode, duplicateRows, forWrite: true, cancellationToken);
        var invalid = evaluated.Where(e => !e.Result.IsValid).Select(e => e.Result).ToList();
        if (invalid.Count > 0)
        {
            return Result<ItemImportBatchOutcome>.Success(new ItemImportBatchOutcome(0, 0, 0, invalid));
        }

        var staged = await StageAsync(access, evaluated, importBatchId, traceId, _clock.UtcNow, cancellationToken);
        if (staged.IsFailure)
        {
            return Result<ItemImportBatchOutcome>.Failure(staged.Error);
        }

        var counts = staged.Value!;
        return Result<ItemImportBatchOutcome>.Success(new ItemImportBatchOutcome(counts.Created, counts.Updated, counts.Unchanged, []));
    }

    public async Task<Result<ItemImportCommitResult>> CommitAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string mode,
        string contentSha256,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        // Phase 1 (create) keeps the plain content hash so existing replay records stay valid; upsert binds the mode into the payload.
        var payloadHash = mode == ItemImportMode.Create ? contentSha256 : Sha256Hex.Compute($"{mode}:{contentSha256}");
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var lockKey = $"{orgId:N}:{Operation}:{keyHash}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", cancellationToken);

            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
                r => r.OrganizationId == orgId && r.Operation == Operation && r.KeyHash == keyHash, cancellationToken);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash)
                {
                    return Result<ItemImportCommitResult>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
                }

                var batchId = Guid.TryParse(replay.ResourceId, out var parsed) ? parsed : Guid.Empty;
                var summary = batchId == Guid.Empty
                    ? null
                    : await _db.AuditEvents.AsNoTracking().FirstOrDefaultAsync(
                        a => a.OrganizationId == orgId && a.Action == "items.import" && a.ResourceId == replay.ResourceId, cancellationToken);
                var replayCounts = summary is null ? new StageCounts(0, 0, 0) : ReadCounts(summary.ChangesJson);
                return Result<ItemImportCommitResult>.Success(new ItemImportCommitResult(
                    batchId, replayCounts.Created, contentSha256, Replayed: true, replayCounts.Updated, replayCounts.Unchanged));
            }

            var evaluated = await EvaluateAsync(orgId, drafts, mode, null, forWrite: true, cancellationToken);
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
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "items.import",
                "item-import",
                batch.ToString(),
                now,
                traceId,
                JsonSerializer.Serialize(new
                {
                    createdCount = counts.Created, updatedCount = counts.Updated, unchangedCount = counts.Unchanged, mode, contentSha256
                }),
                branchId: access.BranchId,
                actorMembershipId: access.MembershipId));
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, Operation, keyHash, payloadHash, batch.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                // A concurrent import or manual create took one of the codes after validation.
                return Result<ItemImportCommitResult>.Failure(
                    new Error("ITEM_CODE_CONFLICT", "The code already exists in this organization."));
            }

            return Result<ItemImportCommitResult>.Success(new ItemImportCommitResult(
                batch, counts.Created, contentSha256, Replayed: false, counts.Updated, counts.Unchanged));
        });
    }

    private sealed record Lookup(Guid CategoryId, Guid? BrandId, Guid BaseUnitId, string? TaxCategoryCode);

    private sealed record Evaluated(ItemImportRowResult Result, ItemImportRow? Row, Lookup? Lookup, Item? Existing);

    private sealed record StageCounts(int Created, int Updated, int Unchanged);

    /// <summary>Adds the new Items and the changes to existing ones to the context (no save) with one audit event per changed row.</summary>
    private async Task<Result<StageCounts>> StageAsync(
        RequestAccessContext access,
        IReadOnlyList<Evaluated> evaluated,
        Guid importBatchId,
        string traceId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var orgId = access.OrganizationId;
        int created = 0, updated = 0, unchanged = 0;

        foreach (var entry in evaluated)
        {
            var row = entry.Row!;
            var lookup = entry.Lookup!;
            var name = LocalizedText.Create(row.NameTh, row.NameEn);
            var description = LocalizedText.CreateOptional(row.DescriptionTh, row.DescriptionEn);
            var capabilities = new ItemCapabilities(row.CanSell, row.CanCost, row.CanPurchase, row.CanStock, row.CanProduce);

            if (entry.Existing is { } existing)
            {
                if (entry.Result.Outcome == ItemImportOutcomes.Unchanged)
                {
                    unchanged++;
                    continue;
                }

                var rowVersionBefore = existing.RowVersion;
                try
                {
                    // Only the overwritable fields come from the file; code, type, base unit, availability and attributes keep their stored value.
                    existing.UpdateDetails(
                        existing.Code, existing.ItemType, lookup.CategoryId, lookup.BrandId, name, description, existing.BaseUnitId,
                        existing.AvailabilityMode, capabilities, existing.Attributes, existing.AttributesSchemaVersion,
                        access.ActorUserId, now, lookup.TaxCategoryCode);
                }
                catch (ItemDomainException ex)
                {
                    _db.ChangeTracker.Clear();
                    return Result<StageCounts>.Failure(new Error(ex.Code, ex.Message));
                }

                _db.AuditEvents.Add(new AuditEvent(
                    Guid.NewGuid(), orgId, access.ActorUserId, "items.update", "item", existing.Id.ToString(), now, traceId,
                    JsonSerializer.Serialize(new { code = existing.Code, importBatchId }),
                    branchId: access.BranchId, actorMembershipId: access.MembershipId, requestId: null,
                    rowVersionBefore: rowVersionBefore, rowVersionAfter: existing.RowVersion));
                updated++;
                continue;
            }

            var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
                _documentNumberGenerator, orgId, DocumentTypes.Items, row.Code, 50, "ITEM_CODE_CONFLICT",
                async (code, token) => await _db.Items.AnyAsync(i => i.OrganizationId == orgId && i.NormalizedCode == code.Trim().ToUpperInvariant(), token),
                cancellationToken);
            if (resolvedCode.IsFailure)
            {
                return Result<StageCounts>.Failure(resolvedCode.Error);
            }

            Item item;
            try
            {
                item = Item.CreateDraft(
                    Guid.NewGuid(), orgId, resolvedCode.Value!, row.ItemType, lookup.CategoryId, lookup.BrandId, name, description,
                    lookup.BaseUnitId, ItemAvailabilityMode.AllBranches, capabilities, null, null, access.ActorUserId, now,
                    lookup.TaxCategoryCode);
            }
            catch (ItemDomainException ex)
            {
                _db.ChangeTracker.Clear();
                return Result<StageCounts>.Failure(new Error(ex.Code, ex.Message));
            }

            _db.Items.Add(item);
            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(), orgId, access.ActorUserId, "items.create", "item", item.Id.ToString(), now, traceId,
                JsonSerializer.Serialize(new { code = item.Code, codeGenerated = row.Code is null, importBatchId }),
                branchId: access.BranchId, actorMembershipId: access.MembershipId, requestId: null,
                rowVersionAfter: item.RowVersion));
            created++;
        }

        return Result<StageCounts>.Success(new StageCounts(created, updated, unchanged));
    }

    private async Task<IReadOnlyList<Evaluated>> EvaluateAsync(
        Guid orgId,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string mode,
        IReadOnlySet<int>? duplicateRows,
        bool forWrite,
        CancellationToken cancellationToken)
    {
        var isUpsert = mode == ItemImportMode.Upsert;
        var syntax = drafts.Select(d => (Draft: d, Check: ItemImportCsv.Validate(d, mode))).ToList();
        var rows = syntax.Where(s => s.Check.Row is not null).Select(s => s.Check.Row!).ToList();

        static string Norm(string value) => value.Trim().ToUpperInvariant();

        var categoryCodes = rows.Select(r => Norm(r.CategoryCode)).Distinct().ToList();
        var brandCodes = rows.Where(r => r.BrandCode is not null).Select(r => Norm(r.BrandCode!)).Distinct().ToList();
        var unitCodes = rows.Select(r => Norm(r.BaseUnitCode)).Distinct().ToList();
        var taxCodes = rows.Where(r => r.TaxCategoryCode is not null).Select(r => Norm(r.TaxCategoryCode!)).Distinct().ToList();
        var itemCodes = rows.Where(r => r.Code is not null).Select(r => Norm(r.Code!)).Distinct().ToList();

        var categories = await _db.ItemCategories.AsNoTracking()
            .Where(c => c.OrganizationId == orgId && categoryCodes.Contains(c.NormalizedCode))
            .ToDictionaryAsync(c => c.NormalizedCode, c => (c.Id, c.Status), cancellationToken);
        var brands = await _db.ItemBrands.AsNoTracking()
            .Where(b => b.OrganizationId == orgId && brandCodes.Contains(b.NormalizedCode))
            .ToDictionaryAsync(b => b.NormalizedCode, b => (b.Id, b.Status), cancellationToken);
        var units = await _db.Units.AsNoTracking()
            .Where(u => u.OrganizationId == orgId && unitCodes.Contains(u.NormalizedCode))
            .ToDictionaryAsync(u => u.NormalizedCode, u => (u.Id, u.Status), cancellationToken);
        var taxCategories = await _db.ItemTaxCategories.AsNoTracking()
            .Where(t => t.OrganizationId == orgId && taxCodes.Contains(t.NormalizedCode))
            .ToDictionaryAsync(t => t.NormalizedCode, t => (t.Code, t.Status), cancellationToken);

        var existingQuery = _db.Items.Where(i => i.OrganizationId == orgId && itemCodes.Contains(i.NormalizedCode));
        if (!forWrite || !isUpsert) existingQuery = existingQuery.AsNoTracking();
        var existingItems = (await existingQuery.ToListAsync(cancellationToken)).ToDictionary(i => i.NormalizedCode, StringComparer.Ordinal);

        var firstRowByCode = new Dictionary<string, int>(StringComparer.Ordinal);
        var results = new List<Evaluated>(syntax.Count);

        foreach (var (draft, check) in syntax)
        {
            var errors = new List<ItemImportRowError>(check.Errors);
            var row = check.Row;
            Lookup? lookup = null;
            Item? existing = null;
            string? outcome = null;

            if (row is not null)
            {
                Guid categoryId = Guid.Empty, baseUnitId = Guid.Empty;
                Guid? brandId = null;
                string? taxCode = null;

                if (!categories.TryGetValue(Norm(row.CategoryCode), out var category)) errors.Add(new("categoryCode", ItemImportErrorCodes.NotFound));
                else if (category.Status != ItemStatus.Active) errors.Add(new("categoryCode", ItemImportErrorCodes.Inactive));
                else categoryId = category.Id;

                if (!units.TryGetValue(Norm(row.BaseUnitCode), out var unit)) errors.Add(new("baseUnitCode", ItemImportErrorCodes.NotFound));
                else if (unit.Status != ItemStatus.Active) errors.Add(new("baseUnitCode", ItemImportErrorCodes.Inactive));
                else baseUnitId = unit.Id;

                if (row.BrandCode is not null)
                {
                    if (!brands.TryGetValue(Norm(row.BrandCode), out var brand)) errors.Add(new("brandCode", ItemImportErrorCodes.NotFound));
                    else if (brand.Status != ItemStatus.Active) errors.Add(new("brandCode", ItemImportErrorCodes.Inactive));
                    else brandId = brand.Id;
                }

                if (row.TaxCategoryCode is not null)
                {
                    if (!taxCategories.TryGetValue(Norm(row.TaxCategoryCode), out var tax)) errors.Add(new("taxCategoryCode", ItemImportErrorCodes.NotFound));
                    else if (tax.Status != ItemStatus.Active) errors.Add(new("taxCategoryCode", ItemImportErrorCodes.Inactive));
                    else taxCode = tax.Code;
                }

                if (row.Code is not null)
                {
                    var normalized = Norm(row.Code);
                    existingItems.TryGetValue(normalized, out existing);

                    if (existing is not null && !isUpsert) errors.Add(new("code", ItemImportErrorCodes.AlreadyExists));
                    else if (duplicateRows is not null ? duplicateRows.Contains(row.RowNumber) : !firstRowByCode.TryAdd(normalized, row.RowNumber))
                        errors.Add(new("code", ItemImportErrorCodes.DuplicateInFile));
                }

                if (errors.Count == 0)
                {
                    lookup = new Lookup(categoryId, brandId, baseUnitId, taxCode);
                    outcome = existing is null ? ItemImportOutcomes.Created : CompareToExisting(existing, row, lookup, errors);
                }
            }

            results.Add(new Evaluated(
                new ItemImportRowResult(draft.RowNumber, draft.Code, draft.NameTh, errors, errors.Count == 0 ? outcome : null),
                errors.Count == 0 ? row : null,
                lookup,
                errors.Count == 0 ? existing : null));
        }

        return results;
    }

    /// <summary>Upsert of an existing Item: the identity-like cells must match, lifecycle locks win, and an identical row is "unchanged".</summary>
    private static string? CompareToExisting(Item existing, ItemImportRow row, Lookup lookup, List<ItemImportRowError> errors)
    {
        if (!string.Equals(existing.ItemType, row.ItemType, StringComparison.OrdinalIgnoreCase)) errors.Add(new("itemType", ItemImportErrorCodes.Mismatch));
        if (existing.BaseUnitId != lookup.BaseUnitId) errors.Add(new("baseUnitCode", ItemImportErrorCodes.Mismatch));
        if (existing.Status == ItemStatus.Inactive) errors.Add(new("code", ItemImportErrorCodes.Locked));
        if (errors.Count > 0) return null;

        var name = LocalizedText.Create(row.NameTh, row.NameEn);
        var description = LocalizedText.CreateOptional(row.DescriptionTh, row.DescriptionEn);
        var capabilities = new ItemCapabilities(row.CanSell, row.CanCost, row.CanPurchase, row.CanStock, row.CanProduce);
        var taxCode = lookup.TaxCategoryCode?.Trim().ToUpperInvariant();

        var same = existing.CategoryId == lookup.CategoryId
            && existing.BrandId == lookup.BrandId
            && existing.Name == name
            && existing.Description == description
            && existing.Capabilities == capabilities
            && existing.TaxCategoryCode == taxCode;
        return same ? ItemImportOutcomes.Unchanged : ItemImportOutcomes.Updated;
    }

    private static StageCounts ReadCounts(string changesJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(changesJson);
            int Read(string name) => doc.RootElement.TryGetProperty(name, out var value) && value.TryGetInt32(out var parsed) ? parsed : 0;
            return new StageCounts(Read("createdCount"), Read("updatedCount"), Read("unchangedCount"));
        }
        catch (JsonException)
        {
            return new StageCounts(0, 0, 0);
        }
    }
}
