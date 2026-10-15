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
