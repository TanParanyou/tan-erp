using TanErp.Application.Common.Results;
using TanErp.Domain.Items;

namespace TanErp.Application.Items.Catalog;

public sealed record EstimateCatalogQuery(
    Guid OrganizationId,
    Guid BranchId,
    string? Search = null,
    string? ItemType = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    bool? HasCost = null,
    string? Cursor = null,
    int PageSize = 25);

public sealed record EstimateCatalogItemProjection(
    Guid Id,
    string Code,
    LocalizedTextDto Name,
    LocalizedTextDto? Description,
    string ItemType,
    CategorySummaryDto Category,
    BrandSummaryDto? Brand,
    UnitSummaryDto BaseUnit,
    Dictionary<string, string>? Attributes,
    CatalogPrimaryImageProjection? PrimaryImage,
    CatalogResolvedCostProjection? ResolvedCost);

public sealed record CatalogPrimaryImageProjection(
    Guid FileId,
    LocalizedTextDto AltText);

public sealed record CatalogResolvedCostProjection(
    Guid CostRecordId,
    int Version,
    decimal Amount,
    string Currency,
    string UnitCode,
    string Scope,
    DateTimeOffset EffectiveFromUtc,
    string PolicyVersion = "COST-RESOLVE-v1");

public sealed record CatalogFacetItem(string Value, int Count);
public sealed record CatalogCategoryFacet(Guid Id, LocalizedTextDto Name, int Count);
public sealed record CatalogBrandFacet(Guid Id, LocalizedTextDto Name, int Count);

public sealed record EstimateCatalogFacets(
    IReadOnlyList<CatalogFacetItem> ItemTypes,
    IReadOnlyList<CatalogCategoryFacet> Categories,
    IReadOnlyList<CatalogBrandFacet> Brands);

public sealed record EstimateCatalogResult(
    IReadOnlyList<EstimateCatalogItemProjection> Items,
    EstimateCatalogFacets Facets,
    string? NextCursor,
    bool HasNextPage);

public interface IEstimateCatalogReader
{
    Task<Result<EstimateCatalogResult>> SearchAsync(EstimateCatalogQuery query, CancellationToken ct);
}
