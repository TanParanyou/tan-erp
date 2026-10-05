using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

public sealed record CreateItemData(
    string? Code,
    string ItemType,
    Guid CategoryId,
    Guid? BrandId,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    Guid BaseUnitId,
    string AvailabilityMode,
    ItemCapabilitiesDto Capabilities,
    IReadOnlyList<Guid>? SelectedBranchIds,
    IReadOnlyList<LocalizedTextDto>? Aliases,
    Dictionary<string, string>? Attributes,
    int? AttributesSchemaVersion,
    string? TaxCategoryCode = null);

public sealed record UpdateItemData(
    Guid ItemId,
    Guid ExpectedRowVersion,
    string Code,
    string ItemType,
    Guid CategoryId,
    Guid? BrandId,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    Guid BaseUnitId,
    string AvailabilityMode,
    ItemCapabilitiesDto Capabilities,
    Dictionary<string, string>? Attributes,
    int? AttributesSchemaVersion,
    string? TaxCategoryCode = null,
    IReadOnlyList<Guid>? SelectedBranchIds = null);

public sealed record ItemQuery(
    Guid OrganizationId,
    string? Search,
    string? ItemType,
    Guid? CategoryId,
    Guid? BrandId,
    string? Status,
    string SortBy,
    string SortOrder,
    int PageNumber,
    int PageSize);

public sealed record PagedItemsResult(
    IReadOnlyList<ItemDetailProjection> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record CreateCategoryData(
    string? Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? AllowedItemTypes,
    int SortOrder);

public sealed record UpdateCategoryData(
    Guid CategoryId,
    Guid ExpectedRowVersion,
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? AllowedItemTypes,
    int SortOrder,
    Guid? ImageFileId = null);

public sealed record CreateBrandData(
    string? Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    int SortOrder);

public sealed record UpdateBrandData(
    Guid BrandId,
    Guid ExpectedRowVersion,
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    int SortOrder,
    Guid? ImageFileId = null);

public sealed record CreateTaxCategoryData(string? Code, LocalizedTextDto Name, int SortOrder);

public sealed record UpdateTaxCategoryData(Guid TaxCategoryId, Guid ExpectedRowVersion, string Code, LocalizedTextDto Name, int SortOrder);

public sealed record CreateUnitData(
    string? Code,
    LocalizedTextDto Name,
    string Symbol,
    string Dimension,
    int DecimalScale,
    string RoundingMode);

public sealed record UpdateUnitData(
    Guid UnitId,
    Guid ExpectedRowVersion,
    string Code,
    LocalizedTextDto Name,
    string Symbol,
    string Dimension,
    int DecimalScale,
    string RoundingMode);

public sealed record CreateItemBarcodeData(
    Guid ItemId,
    string IdentifierType,
    string Value,
    Guid UnitId,
    decimal QuantityInBaseUnit,
    string PackagingLevel,
    bool IsPrimary);

public sealed record ItemBarcodeProjection(
    Guid Id,
    Guid ItemId,
    string ItemCode,
    LocalizedTextDto ItemName,
    string ItemStatus,
    string IdentifierType,
    string Value,
    Guid UnitId,
    string UnitCode,
    LocalizedTextDto UnitName,
    string UnitSymbol,
    decimal QuantityInBaseUnit,
    string PackagingLevel,
    bool IsPrimary,
    string Status,
    Guid RowVersion);

public sealed record CreateItemUnitConversionData(
    Guid ItemId,
    Guid FromUnitId,
    Guid ToUnitId,
    decimal Factor,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Reason);

public sealed record ItemUnitConversionProjection(
    Guid Id,
    Guid ItemId,
    string ItemCode,
    Guid FromUnitId,
    string FromUnitCode,
    Guid ToUnitId,
    string ToUnitCode,
    decimal Factor,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Reason,
    string Status,
    Guid RowVersion);

public sealed record CreateUnitConversionData(
    Guid FromUnitId,
    Guid ToUnitId,
    decimal Factor,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Reason);

public sealed record UnitConversionProjection(
    Guid Id,
    Guid FromUnitId,
    string FromUnitCode,
    Guid ToUnitId,
    string ToUnitCode,
    decimal Factor,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Reason,
    string Status,
    Guid RowVersion);

public interface IItemStore
{
    Task<Result<ItemDetailProjection>> CreateItemAsync(CreateItemData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<ItemDetailProjection?> GetItemAsync(Guid organizationId, Guid itemId, CancellationToken ct);
    Task<PagedItemsResult> ListItemsAsync(ItemQuery query, CancellationToken ct);
    Task<Result<ItemDetailProjection>> UpdateItemAsync(UpdateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> ActivateItemAsync(Guid organizationId, Guid itemId, Guid expectedRowVersion, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> DeactivateItemAsync(Guid organizationId, Guid itemId, Guid expectedRowVersion, string reasonCode, string? reason, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> SetBranchAvailabilityAsync(Guid organizationId, Guid itemId, Guid expectedRowVersion, string mode, IReadOnlyList<Guid> branchIds, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<ItemCategoryDetailProjection>> ListCategoriesAsync(Guid organizationId, CancellationToken ct);
    Task<ItemCategoryDetailProjection?> GetCategoryAsync(Guid organizationId, Guid categoryId, CancellationToken ct);
    Task<Result<ItemCategoryDetailProjection>> CreateCategoryAsync(CreateCategoryData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<Result<ItemCategoryDetailProjection>> UpdateCategoryAsync(UpdateCategoryData data, RequestAccessContext access, CancellationToken ct);
    Task<IReadOnlyList<TanErp.Domain.Items.CategoryAttributeTemplate>> GetCategoryAttributeTemplatesAsync(Guid organizationId, Guid categoryId, CancellationToken ct);
    Task<Result<IReadOnlyList<TanErp.Domain.Items.CategoryAttributeTemplate>>> SetCategoryAttributeTemplatesAsync(Guid organizationId, Guid categoryId, List<TanErp.Domain.Items.CategoryAttributeTemplate> templates, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<ItemBrandDetailProjection>> ListBrandsAsync(Guid organizationId, CancellationToken ct);
    Task<ItemBrandDetailProjection?> GetBrandAsync(Guid organizationId, Guid brandId, CancellationToken ct);
    Task<Result<ItemBrandDetailProjection>> CreateBrandAsync(CreateBrandData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<Result<ItemBrandDetailProjection>> UpdateBrandAsync(UpdateBrandData data, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<ItemTaxCategoryDetailProjection>> ListTaxCategoriesAsync(Guid organizationId, CancellationToken ct);
    Task<ItemTaxCategoryDetailProjection?> GetTaxCategoryAsync(Guid organizationId, Guid taxCategoryId, CancellationToken ct);
    Task<Result<ItemTaxCategoryDetailProjection>> CreateTaxCategoryAsync(CreateTaxCategoryData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<Result<ItemTaxCategoryDetailProjection>> UpdateTaxCategoryAsync(UpdateTaxCategoryData data, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<UnitOfMeasureDetailProjection>> ListUnitsAsync(Guid organizationId, CancellationToken ct);
    Task<UnitOfMeasureDetailProjection?> GetUnitAsync(Guid organizationId, Guid unitId, CancellationToken ct);
    Task<Result<UnitOfMeasureDetailProjection>> CreateUnitAsync(CreateUnitData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<Result<UnitOfMeasureDetailProjection>> UpdateUnitAsync(UpdateUnitData data, RequestAccessContext access, CancellationToken ct);

    Task<Result<ItemDetailProjection>> AddAliasAsync(Guid organizationId, Guid itemId, LocalizedTextDto alias, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> RemoveAliasAsync(Guid organizationId, Guid itemId, Guid aliasId, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<ItemBarcodeProjection>> ListBarcodesAsync(Guid organizationId, Guid itemId, CancellationToken ct);
    Task<Result<ItemBarcodeProjection>> CreateBarcodeAsync(CreateItemBarcodeData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<Result<ItemBarcodeProjection>> SetBarcodePrimaryAsync(Guid organizationId, Guid itemId, Guid barcodeId, Guid expectedRowVersion, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemBarcodeProjection>> DeactivateBarcodeAsync(Guid organizationId, Guid itemId, Guid barcodeId, Guid expectedRowVersion, RequestAccessContext access, CancellationToken ct);
    Task<ItemBarcodeProjection?> FindActiveBarcodeAsync(Guid organizationId, Guid? branchId, string value, CancellationToken ct);
    Task<IReadOnlyList<ItemUnitConversionProjection>> ListItemUnitConversionsAsync(Guid organizationId, Guid itemId, CancellationToken ct);
    Task<Result<ItemUnitConversionProjection>> CreateItemUnitConversionAsync(CreateItemUnitConversionData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
    Task<IReadOnlyList<UnitConversionProjection>> ListUnitConversionsAsync(Guid organizationId, CancellationToken ct);
    Task<Result<UnitConversionProjection>> CreateUnitConversionAsync(CreateUnitConversionData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct);
}
