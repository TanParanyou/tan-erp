using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Application.Items.Catalog;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public class CostResolver : ICostResolver
{
    private readonly AppDbContext _db;

    public CostResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ResolvedCostProjection>> ResolveAsync(ResolveCostRequest request, CancellationToken ct)
    {
        var upperCurrency = request.Currency.Trim().ToUpperInvariant();

        var records = await _db.CostRecords
            .AsNoTracking()
            .Include(c => c.CostSource)
            .Where(c => c.OrganizationId == request.OrganizationId
                && c.ItemId == request.ItemId
                && c.UnitId == request.UnitId
                && c.Currency == upperCurrency
                && (c.Status == CostRecordStatus.Published || c.Status == CostRecordStatus.Superseded)
                && c.MinimumQuantity <= request.Quantity
                && (c.MaximumQuantity == null || c.MaximumQuantity >= request.Quantity)
                && (c.Scope == CostScopeType.Organization || (c.Scope == CostScopeType.Branch && c.BranchId == request.BranchId)))
            .ToListAsync(ct);

        var effectiveRecords = records.Where(record => record.EffectiveFromUtc <= request.EffectiveAtUtc &&
            (record.EffectiveToUtc is null || record.EffectiveToUtc >= request.EffectiveAtUtc)).ToList();
        if (effectiveRecords.Count == 0 && records.Count > 0)
        {
            return Result<ResolvedCostProjection>.Failure(
                new Error("ITEM_COST_STALE", "Published costs exist for this item, but none are effective at the requested date."));
        }

        var selection = SelectWinner(effectiveRecords, request.BranchId, request.Quantity);
        if (selection.IsFailure)
        {
            return Result<ResolvedCostProjection>.Failure(
                selection.Error);
        }
        var top = selection.Value!;

        var projection = new ResolvedCostProjection(
            top.Id,
            top.Version,
            top.Scope,
            top.BranchId,
            top.UnitId,
            top.Currency,
            top.Amount,
            top.MinimumQuantity,
            top.MaximumQuantity,
            top.EffectiveFromUtc,
            top.EffectiveToUtc,
            top.CostSourceId,
            top.CostSource?.Code,
            top.SourceReference,
            top.Reason,
            top.EvidenceFileId,
            DateTimeOffset.UtcNow,
            request.PolicyVersion ?? "v1");

        return Result<ResolvedCostProjection>.Success(projection);
    }

    public CatalogResolvedCostProjection? ResolveForCatalog(IReadOnlyList<CostRecord> records, Guid branchId)
    {
        var now = DateTimeOffset.UtcNow;
        var valid = records.Where(r =>
            (r.Status == CostRecordStatus.Published || r.Status == CostRecordStatus.Superseded) &&
            r.EffectiveFromUtc <= now &&
            (r.EffectiveToUtc == null || r.EffectiveToUtc >= now) &&
            r.MinimumQuantity <= 1m &&
            (r.MaximumQuantity == null || r.MaximumQuantity >= 1m) &&
            (r.Scope == CostScopeType.Organization || (r.Scope == CostScopeType.Branch && r.BranchId == branchId))
        ).ToList();
        var selection = SelectWinner(valid, branchId, 1m);
        if (selection.IsFailure) return null;
        var winner = selection.Value!;

        return new CatalogResolvedCostProjection(
            winner.Id,
            winner.Version,
            winner.Amount,
            winner.Currency,
            winner.Unit?.Code ?? string.Empty,
            winner.Scope,
            winner.EffectiveFromUtc,
            "COST-RESOLVE-v1",
            winner.CostSourceId,
            winner.CostSource?.Code,
            winner.SourceReference,
            winner.EvidenceFileId);
    }

    private static Result<CostRecord> SelectWinner(IReadOnlyList<CostRecord> records, Guid branchId, decimal quantity)
    {
        var eligible = records.Where(r => r.MinimumQuantity <= quantity
            && (r.MaximumQuantity == null || r.MaximumQuantity >= quantity)).ToList();
        if (eligible.Count == 0)
            return Result<CostRecord>.Failure(new Error("ITEM_COST_NOT_FOUND", "No published cost record found matching the criteria."));

        var branchRecords = eligible.Where(r => r.Scope == CostScopeType.Branch && r.BranchId == branchId).ToList();
        var candidates = branchRecords.Count > 0 ? branchRecords : eligible;
        var newestEffectiveFrom = candidates.Max(r => r.EffectiveFromUtc);
        var newest = candidates.Where(r => r.EffectiveFromUtc == newestEffectiveFrom).ToList();
        var greatestMinimum = newest.Max(r => r.MinimumQuantity);
        var winners = newest.Where(r => r.MinimumQuantity == greatestMinimum).ToList();
        if (winners.Count != 1)
            return Result<CostRecord>.Failure(new Error("ITEM_COST_AMBIGUOUS", "Multiple cost records match with ambiguous precedence."));

        return Result<CostRecord>.Success(winners[0]);
    }
}
