using System.ComponentModel.DataAnnotations;
using System.Globalization;
using TanErp.Application.Items;

namespace TanErp.Api.Contracts.Items;

public sealed class CreateItemUnitConversionRequest
{
    [Required] public Guid FromUnitId { get; set; }
    [Required] public Guid ToUnitId { get; set; }
    [Required] public string Factor { get; set; } = string.Empty;
    [Required] public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    [Required, MaxLength(500)] public string Reason { get; set; } = string.Empty;
}

public sealed record ItemUnitConversionResponse(
    Guid Id,
    Guid ItemId,
    string ItemCode,
    Guid FromUnitId,
    string FromUnitCode,
    Guid ToUnitId,
    string ToUnitCode,
    string Factor,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Reason,
    string Status,
    Guid RowVersion);

public sealed record UnitConversionResponse(
    Guid Id,
    Guid FromUnitId,
    string FromUnitCode,
    Guid ToUnitId,
    string ToUnitCode,
    string Factor,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Reason,
    string Status,
    Guid RowVersion);

public static class ItemUnitConversionResponseMapper
{
    public static ItemUnitConversionResponse ToResponse(ItemUnitConversionProjection projection) => new(
        projection.Id, projection.ItemId, projection.ItemCode, projection.FromUnitId, projection.FromUnitCode,
        projection.ToUnitId, projection.ToUnitCode, projection.Factor.ToString("0.######", CultureInfo.InvariantCulture),
        projection.EffectiveFrom, projection.EffectiveTo, projection.Reason, projection.Status, projection.RowVersion);
}

public static class UnitConversionResponseMapper
{
    public static UnitConversionResponse ToResponse(UnitConversionProjection projection) => new(
        projection.Id, projection.FromUnitId, projection.FromUnitCode, projection.ToUnitId, projection.ToUnitCode,
        projection.Factor.ToString("0.######", CultureInfo.InvariantCulture), projection.EffectiveFrom,
        projection.EffectiveTo, projection.Reason, projection.Status, projection.RowVersion);
}
