using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates;
using TanErp.Application.Items;

namespace TanErp.Infrastructure.Persistence.Estimates;

public sealed class EstimateUnitOptionsReader : IEstimateUnitOptionsReader
{
    private readonly AppDbContext _db;
    private readonly IUnitConversionResolver _conversions;
    private readonly IClock _clock;

    public EstimateUnitOptionsReader(AppDbContext db, IUnitConversionResolver conversions, IClock clock)
    {
        _db = db;
        _conversions = conversions;
        _clock = clock;
    }

    public async Task<Result<EstimateUnitOptionsProjection>> ListAsync(Guid organizationId, Guid branchId, Guid itemId, CancellationToken ct)
    {
        var units = await _conversions.ListEstimateUnitsAsync(organizationId, itemId, DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime), ct);
        if (units.IsFailure)
            return Result<EstimateUnitOptionsProjection>.Failure(units.Error);

        var enabled = await EstimateStore.IsUnitConversionEnabledAsync(_db, organizationId, branchId, _clock.UtcNow, ct);
        var all = units.Value!;
        var baseUnit = all.First(unit => unit.IsBase);
        var offered = enabled ? all : all.Where(unit => unit.IsBase).ToList();
        return Result<EstimateUnitOptionsProjection>.Success(new EstimateUnitOptionsProjection(enabled, baseUnit.UnitCode, offered));
    }
}
