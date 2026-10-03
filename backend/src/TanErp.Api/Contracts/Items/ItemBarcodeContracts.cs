using System.ComponentModel.DataAnnotations;
using System.Globalization;
using TanErp.Application.Items;

namespace TanErp.Api.Contracts.Items;

public sealed class CreateItemBarcodeRequest
{
    [Required] public string IdentifierType { get; set; } = string.Empty;
    [Required] public string Value { get; set; } = string.Empty;
    [Required] public Guid UnitId { get; set; }
    [Required] public string QuantityInBaseUnit { get; set; } = string.Empty;
    [Required] public string PackagingLevel { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public sealed record ItemBarcodeItemResponse(Guid Id, string Code, LocalizedTextResponse Name, string Status);
public sealed record ItemBarcodeUnitResponse(Guid Id, string Code, LocalizedTextResponse Name, string Symbol);
public sealed record ItemBarcodeResponse(
    Guid Id,
    ItemBarcodeItemResponse Item,
    string IdentifierType,
    string Value,
    ItemBarcodeUnitResponse Unit,
    string QuantityInBaseUnit,
    string PackagingLevel,
    bool IsPrimary,
    string Status,
    Guid RowVersion);

public static class ItemBarcodeResponseMapper
{
    public static ItemBarcodeResponse ToResponse(ItemBarcodeProjection projection) => new(
        projection.Id,
        new ItemBarcodeItemResponse(projection.ItemId, projection.ItemCode,
            ItemResponseMapper.ToResponse(projection.ItemName), projection.ItemStatus),
        projection.IdentifierType,
        projection.Value,
        new ItemBarcodeUnitResponse(projection.UnitId, projection.UnitCode,
            ItemResponseMapper.ToResponse(projection.UnitName), projection.UnitSymbol),
        projection.QuantityInBaseUnit.ToString("0.0000", CultureInfo.InvariantCulture),
        projection.PackagingLevel,
        projection.IsPrimary,
        projection.Status,
        projection.RowVersion);
}
