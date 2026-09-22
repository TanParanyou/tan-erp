using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
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
                && c.Status == CostRecordStatus.Published
                && c.EffectiveFromUtc <= request.EffectiveAtUtc
                && (c.EffectiveToUtc == null || c.EffectiveToUtc >= request.EffectiveAtUtc)
                && c.MinimumQuantity <= request.Quantity
                && (c.MaximumQuantity == null || c.MaximumQuantity >= request.Quantity)
                && (c.Scope == CostScopeType.Organization || (c.Scope == CostScopeType.Branch && c.BranchId == request.BranchId)))
            .ToListAsync(ct);

        if (records.Count == 0)
        {
            return Result<ResolvedCostProjection>.Failure(
                new Error("ITEM_COST_NOT_FOUND", "No published cost record found matching the criteria."));
        }

        // 1. Branch scope takes precedence over Organization scope
        var branchRecords = records.Where(r => r.Scope == CostScopeType.Branch && r.BranchId == request.BranchId).ToList();
        var candidates = branchRecords.Count > 0 ? branchRecords : records;

        // 2. Sort candidates by:
        //    a. Source priority DESC
        //    b. EffectiveFromUtc DESC
        //    c. Version DESC
        var sorted = candidates
            .OrderByDescending(r => r.CostSource?.Priority ?? 0)
            .ThenByDescending(r => r.EffectiveFromUtc)
            .ThenByDescending(r => r.Version)
            .ToList();

        var top = sorted[0];
        var topSourcePriority = top.CostSource?.Priority ?? 0;

        // 3. Ambiguity check: if there are other candidates with identical top precedence (same scope, same source priority, same effective from) but DIFFERENT amount
        var ambiguous = sorted.Where(r =>
            (r.CostSource?.Priority ?? 0) == topSourcePriority &&
            r.EffectiveFromUtc == top.EffectiveFromUtc &&
            r.Amount != top.Amount).ToList();

        if (ambiguous.Count > 0)
        {
            return Result<ResolvedCostProjection>.Failure(
                new Error("ITEM_COST_AMBIGUOUS", "Multiple cost records match with ambiguous precedence."));
        }

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
            DateTimeOffset.UtcNow);

        return Result<ResolvedCostProjection>.Success(projection);
    }
}
