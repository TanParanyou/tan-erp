using System.Globalization;
using TanErp.Application.Estimates;

namespace TanErp.Api.Contracts.Estimates;

/// <summary>The unit conversion frozen on a cost component. <c>Factor</c> is a string (six decimals), like the Item Master conversion contracts.</summary>
public sealed record EstimateConversionSnapshotResponse(Guid ConversionId, string ConversionScope, string FromUnit, string ToUnit, string Factor)
{
    public static EstimateConversionSnapshotResponse? From(EstimateConversionSnapshotProjection? projection) => projection is null
        ? null
        : new EstimateConversionSnapshotResponse(projection.ConversionId, projection.Scope, projection.FromUnit, projection.ToUnit, FormatFactor(projection.Factor));

    internal static string FormatFactor(decimal factor) => factor.ToString("0.######", CultureInfo.InvariantCulture);
}

public sealed record EstimateUnitOptionResponse(Guid UnitId, string UnitCode, bool IsBase, Guid? ConversionId, string? ConversionScope, string? Factor);

public sealed record EstimateUnitOptionsResponse(bool ConversionsEnabled, string BaseUnitCode, IReadOnlyList<EstimateUnitOptionResponse> Units)
{
    public static EstimateUnitOptionsResponse From(EstimateUnitOptionsProjection projection) => new(
        projection.ConversionsEnabled,
        projection.BaseUnitCode,
        projection.Units.Select(unit => new EstimateUnitOptionResponse(
            unit.UnitId, unit.UnitCode, unit.IsBase, unit.ConversionId, unit.Scope,
            unit.Factor is null ? null : EstimateConversionSnapshotResponse.FormatFactor(unit.Factor.Value))).ToList());
}
