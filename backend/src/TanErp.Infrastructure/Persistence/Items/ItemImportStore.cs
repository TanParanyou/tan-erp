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
        CancellationToken cancellationToken = default)
    {
        var evaluated = await EvaluateAsync(access.OrganizationId, drafts, cancellationToken);
        return evaluated.Select(e => e.Result).ToList();
    }

    public async Task<Result<ItemImportCommitResult>> CommitAsync(
        RequestAccessContext access,
        IReadOnlyList<ItemImportRowDraft> drafts,
        string contentSha256,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var keyHash = Sha256Hex.Compute(idempotencyKey);
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
                var created = summary is null ? 0 : ReadCreatedCount(summary.ChangesJson);
                return Result<ItemImportCommitResult>.Success(new ItemImportCommitResult(batchId, created, contentSha256, Replayed: true));
            }

            var evaluated = await EvaluateAsync(orgId, drafts, cancellationToken);
            if (evaluated.Any(e => !e.Result.IsValid))
            {
                return Result<ItemImportCommitResult>.Failure(
                    new Error("ITEM_IMPORT_VALIDATION_FAILED", "One or more rows are invalid; nothing was imported."));
            }

            var now = _clock.UtcNow;
            var batch = Guid.NewGuid();

            foreach (var entry in evaluated)
            {
                var row = entry.Row!;
                var lookup = entry.Lookup!;

                var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
                    _documentNumberGenerator, orgId, DocumentTypes.Items, row.Code, 50, "ITEM_CODE_CONFLICT",
                    async (code, token) => await _db.Items.AnyAsync(i => i.OrganizationId == orgId && i.NormalizedCode == code.Trim().ToUpperInvariant(), token),
                    cancellationToken);
                if (resolvedCode.IsFailure)
                {
                    return Result<ItemImportCommitResult>.Failure(resolvedCode.Error);
                }

                Item item;
                try
                {
                    item = Item.CreateDraft(
                        Guid.NewGuid(),
                        orgId,
                        resolvedCode.Value!,
                        row.ItemType,
                        lookup.CategoryId,
                        lookup.BrandId,
                        LocalizedText.Create(row.NameTh, row.NameEn),
                        LocalizedText.CreateOptional(row.DescriptionTh, row.DescriptionEn),
                        lookup.BaseUnitId,
                        ItemAvailabilityMode.AllBranches,
                        new ItemCapabilities(row.CanSell, row.CanCost, row.CanPurchase, row.CanStock, row.CanProduce),
                        null,
                        null,
                        access.ActorUserId,
                        now,
                        lookup.TaxCategoryCode);
                }
                catch (ItemDomainException ex)
                {
                    return Result<ItemImportCommitResult>.Failure(new Error(ex.Code, ex.Message));
                }

                _db.Items.Add(item);
                _db.AuditEvents.Add(new AuditEvent(
                    Guid.NewGuid(),
                    orgId,
                    access.ActorUserId,
                    "items.create",
                    "item",
                    item.Id.ToString(),
                    now,
                    traceId,
                    JsonSerializer.Serialize(new { code = item.Code, codeGenerated = row.Code is null, importBatchId = batch }),
                    branchId: access.BranchId,
                    actorMembershipId: access.MembershipId,
                    requestId: null,
                    rowVersionAfter: item.RowVersion));
            }

            _db.AuditEvents.Add(new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "items.import",
                "item-import",
                batch.ToString(),
                now,
                traceId,
                JsonSerializer.Serialize(new { createdCount = evaluated.Count, contentSha256 }),
                branchId: access.BranchId,
                actorMembershipId: access.MembershipId));
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, Operation, keyHash, contentSha256, batch.ToString(), now));

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

            return Result<ItemImportCommitResult>.Success(new ItemImportCommitResult(batch, evaluated.Count, contentSha256, Replayed: false));
        });
    }

    private sealed record Lookup(Guid CategoryId, Guid? BrandId, Guid BaseUnitId, string? TaxCategoryCode);

    private sealed record Evaluated(ItemImportRowResult Result, ItemImportRow? Row, Lookup? Lookup);

    private async Task<IReadOnlyList<Evaluated>> EvaluateAsync(
        Guid orgId,
        IReadOnlyList<ItemImportRowDraft> drafts,
        CancellationToken cancellationToken)
    {
        var syntax = drafts.Select(d => (Draft: d, Check: ItemImportCsv.Validate(d))).ToList();
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
        var existingItemCodes = (await _db.Items.AsNoTracking()
            .Where(i => i.OrganizationId == orgId && itemCodes.Contains(i.NormalizedCode))
            .Select(i => i.NormalizedCode)
            .ToListAsync(cancellationToken)).ToHashSet(StringComparer.Ordinal);

        var firstRowByCode = new Dictionary<string, int>(StringComparer.Ordinal);
        var results = new List<Evaluated>(syntax.Count);

        foreach (var (draft, check) in syntax)
        {
            var errors = new List<ItemImportRowError>(check.Errors);
            var row = check.Row;
            Lookup? lookup = null;

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
                    if (existingItemCodes.Contains(normalized)) errors.Add(new("code", ItemImportErrorCodes.AlreadyExists));
                    else if (!firstRowByCode.TryAdd(normalized, row.RowNumber)) errors.Add(new("code", ItemImportErrorCodes.DuplicateInFile));
                }

                if (errors.Count == 0)
                {
                    lookup = new Lookup(categoryId, brandId, baseUnitId, taxCode);
                }
            }

            results.Add(new Evaluated(
                new ItemImportRowResult(draft.RowNumber, draft.Code, draft.NameTh, errors),
                errors.Count == 0 ? row : null,
                lookup));
        }

        return results;
    }

    private static int ReadCreatedCount(string changesJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(changesJson);
            return doc.RootElement.TryGetProperty("createdCount", out var count) && count.TryGetInt32(out var value) ? value : 0;
        }
        catch (JsonException)
        {
            return 0;
        }
    }
}
