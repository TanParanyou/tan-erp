using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

public sealed record CreateItemData(
    string Code,
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
    int? AttributesSchemaVersion);

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
    int? AttributesSchemaVersion);

public sealed record ItemQuery(
    Guid OrganizationId,
    string? Search,
    string? ItemType,
    Guid? CategoryId,
    Guid? BrandId,
    string? Status,
    int PageNumber,
    int PageSize);

public sealed record PagedItemsResult(
    IReadOnlyList<ItemDetailProjection> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record CreateCategoryData(
    string Code,
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
    int SortOrder);

public sealed record CreateBrandData(
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    int SortOrder);

public sealed record UpdateBrandData(
    Guid BrandId,
    Guid ExpectedRowVersion,
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    int SortOrder);

public sealed record CreateUnitData(
    string Code,
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

public interface IItemStore
{
    Task<Result<ItemDetailProjection>> CreateItemAsync(CreateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<ItemDetailProjection?> GetItemAsync(Guid organizationId, Guid itemId, CancellationToken ct);
    Task<PagedItemsResult> ListItemsAsync(ItemQuery query, CancellationToken ct);
    Task<Result<ItemDetailProjection>> UpdateItemAsync(UpdateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> ActivateItemAsync(Guid organizationId, Guid itemId, Guid expectedRowVersion, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> DeactivateItemAsync(Guid organizationId, Guid itemId, Guid expectedRowVersion, string reasonCode, string? reason, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> SetBranchAvailabilityAsync(Guid organizationId, Guid itemId, Guid expectedRowVersion, string mode, IReadOnlyList<Guid> branchIds, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<ItemCategoryDetailProjection>> ListCategoriesAsync(Guid organizationId, CancellationToken ct);
    Task<ItemCategoryDetailProjection?> GetCategoryAsync(Guid organizationId, Guid categoryId, CancellationToken ct);
    Task<Result<ItemCategoryDetailProjection>> CreateCategoryAsync(CreateCategoryData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemCategoryDetailProjection>> UpdateCategoryAsync(UpdateCategoryData data, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<ItemBrandDetailProjection>> ListBrandsAsync(Guid organizationId, CancellationToken ct);
    Task<ItemBrandDetailProjection?> GetBrandAsync(Guid organizationId, Guid brandId, CancellationToken ct);
    Task<Result<ItemBrandDetailProjection>> CreateBrandAsync(CreateBrandData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemBrandDetailProjection>> UpdateBrandAsync(UpdateBrandData data, RequestAccessContext access, CancellationToken ct);

    Task<IReadOnlyList<UnitOfMeasureDetailProjection>> ListUnitsAsync(Guid organizationId, CancellationToken ct);
    Task<UnitOfMeasureDetailProjection?> GetUnitAsync(Guid organizationId, Guid unitId, CancellationToken ct);
    Task<Result<UnitOfMeasureDetailProjection>> CreateUnitAsync(CreateUnitData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<UnitOfMeasureDetailProjection>> UpdateUnitAsync(UpdateUnitData data, RequestAccessContext access, CancellationToken ct);

    Task<Result<ItemDetailProjection>> AddAliasAsync(Guid organizationId, Guid itemId, LocalizedTextDto alias, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemDetailProjection>> RemoveAliasAsync(Guid organizationId, Guid itemId, Guid aliasId, RequestAccessContext access, CancellationToken ct);
}
