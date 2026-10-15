using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Domain.Estimates;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public sealed class UnitConversionResolver : IUnitConversionResolver
{
    private readonly AppDbContext _db;

    public UnitConversionResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ResolvedUnitConversion>> ResolveToBaseUnitAsync(
        Guid organizationId, Guid itemId, string fromUnitCode, DateOnly onDate, CancellationToken ct)
    {
        var item = await _db.Items.AsNoTracking().Include(row => row.BaseUnit)
            .FirstOrDefaultAsync(row => row.Id == itemId && row.OrganizationId == organizationId, ct);
        if (item?.BaseUnit is null)
            return Result<ResolvedUnitConversion>.Failure(new Error("ITEM_NOT_FOUND", "Item not found."));

        var normalizedCode = fromUnitCode.Trim().ToUpperInvariant();
        var fromUnit = await _db.Units.AsNoTracking().FirstOrDefaultAsync(unit =>
            unit.OrganizationId == organizationId && unit.NormalizedCode == normalizedCode && unit.Status == ItemStatus.Active, ct);
        if (fromUnit is null || fromUnit.Id == item.BaseUnitId)
            return Result<ResolvedUnitConversion>.Failure(new Error("ESTIMATE_UNIT_INVALID", $"Unit '{fromUnitCode}' is not an active convertible unit for this item."));

        // The dimension is editable after a conversion was created, so it is checked again here.
        if (!string.Equals(fromUnit.Dimension, item.BaseUnit.Dimension, StringComparison.Ordinal))
            return Result<ResolvedUnitConversion>.Failure(new Error("ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH", "The unit has a different dimension than the item's base unit."));

        var itemConversion = await _db.ItemUnitConversions.AsNoTracking().SingleOrDefaultAsync(row =>
            row.OrganizationId == organizationId && row.ItemId == itemId && row.FromUnitId == fromUnit.Id && row.ToUnitId == item.BaseUnitId
            && row.Status == ItemStatus.Active && row.EffectiveFrom <= onDate && (row.EffectiveTo == null || row.EffectiveTo >= onDate), ct);
        if (itemConversion is not null)
        {
            return Result<ResolvedUnitConversion>.Success(new ResolvedUnitConversion(
                itemConversion.Id, UnitConversionScopes.Item, fromUnit.Id, fromUnit.Code, item.BaseUnit.Id, item.BaseUnit.Code, itemConversion.Factor));
        }

        var sharedConversion = await _db.UnitConversions.AsNoTracking().SingleOrDefaultAsync(row =>
            row.OrganizationId == organizationId && row.FromUnitId == fromUnit.Id && row.ToUnitId == item.BaseUnitId
            && row.Status == ItemStatus.Active && row.EffectiveFrom <= onDate && (row.EffectiveTo == null || row.EffectiveTo >= onDate), ct);
        if (sharedConversion is not null)
        {
            return Result<ResolvedUnitConversion>.Success(new ResolvedUnitConversion(
                sharedConversion.Id, UnitConversionScopes.Shared, fromUnit.Id, fromUnit.Code, item.BaseUnit.Id, item.BaseUnit.Code, sharedConversion.Factor));
        }

        return Result<ResolvedUnitConversion>.Failure(new Error("ESTIMATE_UNIT_CONVERSION_NOT_FOUND", "No active unit conversion to the item's base unit exists."));
    }

    public async Task<Result<IReadOnlyList<EstimateUnitOption>>> ListEstimateUnitsAsync(
        Guid organizationId, Guid itemId, DateOnly onDate, CancellationToken ct)
    {
        var item = await _db.Items.AsNoTracking().Include(row => row.BaseUnit)
            .FirstOrDefaultAsync(row => row.Id == itemId && row.OrganizationId == organizationId, ct);
        if (item?.BaseUnit is null)
            return Result<IReadOnlyList<EstimateUnitOption>>.Failure(new Error("ITEM_NOT_FOUND", "Item not found."));

        var baseUnitId = item.BaseUnitId;
        var dimension = item.BaseUnit.Dimension;
        var options = new List<EstimateUnitOption> { new(item.BaseUnit.Id, item.BaseUnit.Code, true, null, null, null) };

        var itemRows = await (
            from conversion in _db.ItemUnitConversions.AsNoTracking()
            join unit in _db.Units.AsNoTracking() on new { UnitId = conversion.FromUnitId, conversion.OrganizationId } equals new { UnitId = unit.Id, unit.OrganizationId }
            where conversion.OrganizationId == organizationId && conversion.ItemId == itemId && conversion.ToUnitId == baseUnitId
                && conversion.Status == ItemStatus.Active && conversion.EffectiveFrom <= onDate
                && (conversion.EffectiveTo == null || conversion.EffectiveTo >= onDate)
                && unit.Status == ItemStatus.Active && unit.Dimension == dimension
            orderby unit.Code
            select new { conversion.Id, UnitId = unit.Id, unit.Code, conversion.Factor }).ToListAsync(ct);
        options.AddRange(itemRows.Select(row => new EstimateUnitOption(row.UnitId, row.Code, false, row.Id, UnitConversionScopes.Item, row.Factor)));

        var sharedRows = await (
            from conversion in _db.UnitConversions.AsNoTracking()
            join unit in _db.Units.AsNoTracking() on new { UnitId = conversion.FromUnitId, conversion.OrganizationId } equals new { UnitId = unit.Id, unit.OrganizationId }
            where conversion.OrganizationId == organizationId && conversion.ToUnitId == baseUnitId
                && conversion.Status == ItemStatus.Active && conversion.EffectiveFrom <= onDate
                && (conversion.EffectiveTo == null || conversion.EffectiveTo >= onDate)
                && unit.Status == ItemStatus.Active && unit.Dimension == dimension
            orderby unit.Code
            select new { conversion.Id, UnitId = unit.Id, unit.Code, conversion.Factor }).ToListAsync(ct);
        var coveredUnitIds = itemRows.Select(row => row.UnitId).ToHashSet();
        options.AddRange(sharedRows.Where(row => !coveredUnitIds.Contains(row.UnitId))
            .Select(row => new EstimateUnitOption(row.UnitId, row.Code, false, row.Id, UnitConversionScopes.Shared, row.Factor)));

        return Result<IReadOnlyList<EstimateUnitOption>>.Success(options);
    }
}
