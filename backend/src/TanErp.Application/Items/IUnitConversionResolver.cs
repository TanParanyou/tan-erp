using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

/// <summary>The conversion that applies today: 1 FromUnit = Factor ToUnit, ToUnit being the item's base unit.</summary>
public sealed record ResolvedUnitConversion(
    Guid ConversionId, string Scope, Guid FromUnitId, string FromUnitCode, Guid ToUnitId, string ToUnitCode, decimal Factor);

/// <summary>A unit an estimate line of this item may use. The base unit has no conversion.</summary>
public sealed record EstimateUnitOption(
    Guid UnitId, string UnitCode, bool IsBase, Guid? ConversionId, string? Scope, decimal? Factor);

public interface IUnitConversionResolver
{
    /// <summary>
    /// Resolves the active conversion from the unit with <paramref name="fromUnitCode"/> to the item's base unit on <paramref name="onDate"/>.
    /// Item-specific conversions win over shared ones. Fails with ITEM_NOT_FOUND, ESTIMATE_UNIT_INVALID (unknown, inactive or the base unit itself),
    /// ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH or ESTIMATE_UNIT_CONVERSION_NOT_FOUND.
    /// </summary>
    Task<Result<ResolvedUnitConversion>> ResolveToBaseUnitAsync(
        Guid organizationId, Guid itemId, string fromUnitCode, DateOnly onDate, CancellationToken ct);

    /// <summary>The base unit first, then every same-dimension unit with an active conversion to it on <paramref name="onDate"/>.</summary>
    Task<Result<IReadOnlyList<EstimateUnitOption>>> ListEstimateUnitsAsync(
        Guid organizationId, Guid itemId, DateOnly onDate, CancellationToken ct);
}
