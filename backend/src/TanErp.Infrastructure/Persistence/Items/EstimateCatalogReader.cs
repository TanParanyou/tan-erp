using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Application.Items.Catalog;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public class EstimateCatalogReader : IEstimateCatalogReader
{
    private readonly AppDbContext _db;
    private readonly ICostResolver _costResolver;

    public EstimateCatalogReader(AppDbContext db, ICostResolver costResolver)
    {
        _db = db;
        _costResolver = costResolver;
    }

    private sealed class CursorData
    {
        public string Code { get; set; } = string.Empty;
        public Guid Id { get; set; }
    }

    public async Task<Result<EstimateCatalogResult>> SearchAsync(EstimateCatalogQuery query, CancellationToken ct)
    {
        var orgId = query.OrganizationId;
        var branchId = query.BranchId;
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        // 1. Base Query
        var queryable = _db.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Brand)
            .Include(i => i.BaseUnit)
            .Where(i => i.OrganizationId == orgId
                && i.Status == ItemStatus.Active
                && i.Capabilities.CanCost);

        // 2. Branch availability filter
        queryable = queryable.Where(i =>
            i.AvailabilityMode == ItemAvailabilityMode.AllBranches ||
            i.BranchAvailabilities.Any(ba => ba.BranchId == branchId && ba.Status == "active"));

        var now = DateTimeOffset.UtcNow;

        // 3. Only a unique resolver winner counts as an eligible priced item before pagination.
        var eligibleCosts = _db.CostRecords.AsNoTracking().Where(c =>
            c.OrganizationId == orgId
            && (c.Status == CostRecordStatus.Published || c.Status == CostRecordStatus.Superseded)
            && c.EffectiveFromUtc <= now
            && (c.EffectiveToUtc == null || c.EffectiveToUtc >= now)
            && c.MinimumQuantity <= 1m
            && (c.MaximumQuantity == null || c.MaximumQuantity >= 1m)
            && (c.Scope == CostScopeType.Organization || (c.Scope == CostScopeType.Branch && c.BranchId == branchId)));

        var uniqueWinners = eligibleCosts.Where(c => !eligibleCosts.Any(other =>
            other.ItemId == c.ItemId
            && other.UnitId == c.UnitId
            && ((other.Scope == CostScopeType.Branch && c.Scope == CostScopeType.Organization)
                || (other.Scope == c.Scope
                    && (other.EffectiveFromUtc > c.EffectiveFromUtc
                        || (other.EffectiveFromUtc == c.EffectiveFromUtc
                            && (other.MinimumQuantity > c.MinimumQuantity
                                || (other.MinimumQuantity == c.MinimumQuantity && other.Id != c.Id))))))));

        if (query.HasCost ?? true)
            queryable = queryable.Where(i => uniqueWinners.Any(c => c.ItemId == i.Id && c.UnitId == i.BaseUnitId));
        else
            queryable = queryable.Where(i => !uniqueWinners.Any(c => c.ItemId == i.Id && c.UnitId == i.BaseUnitId));

        // Keep filter options and counts stable within the authorized branch and cost scope.
        var facetsBase = queryable;

        // 4. Server-side Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            var searchUpper = query.Search.Trim().ToUpperInvariant();

            queryable = queryable.Where(i =>
                i.NormalizedCode.Contains(searchUpper) ||
                i.Name.Thai.ToLower().Contains(search) ||
                (i.Name.English != null && i.Name.English.ToLower().Contains(search)) ||
                i.Aliases.Any(a => a.Status == "active" &&
                    (a.NormalizedTh.Contains(search) || (a.NormalizedEn != null && a.NormalizedEn.Contains(search)))));
        }

        // 6. Category, Brand, and ItemType Filters
        if (!string.IsNullOrWhiteSpace(query.ItemType))
        {
            var itemType = query.ItemType.Trim().ToLowerInvariant();
            queryable = queryable.Where(i => i.ItemType == itemType);
        }

        if (query.CategoryId.HasValue)
        {
            queryable = queryable.Where(i => i.CategoryId == query.CategoryId.Value);
        }

        if (query.BrandId.HasValue)
        {
            queryable = queryable.Where(i => i.BrandId == query.BrandId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.AttributeKey))
        {
            var attributeKey = query.AttributeKey.Trim();
            if (!string.IsNullOrWhiteSpace(query.AttributeValue))
            {
                var attributeFilter = new Dictionary<string, string>
                {
                    [attributeKey] = query.AttributeValue.Trim()
                };
                queryable = queryable.Where(i => i.Attributes != null
                    && EF.Functions.JsonContains(i.Attributes, JsonSerializer.Serialize(attributeFilter)));
            }
            else
            {
                queryable = queryable.Where(i => i.Attributes != null
                    && EF.Functions.JsonExists(i.Attributes, attributeKey));
            }
        }

        // 6. Cursor Decoding
        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            try
            {
                var jsonBytes = Convert.FromBase64String(query.Cursor);
                var cursorData = JsonSerializer.Deserialize<CursorData>(jsonBytes);
                if (cursorData == null || string.IsNullOrWhiteSpace(cursorData.Code) || cursorData.Id == Guid.Empty)
                {
                    return Result<EstimateCatalogResult>.Failure(
                        new Error("ITEM_CATALOG_CURSOR_INVALID", "Catalog cursor is invalid."));
                }

                queryable = queryable.Where(i =>
                    string.Compare(i.NormalizedCode, cursorData.Code) > 0 ||
                    (i.NormalizedCode == cursorData.Code && i.Id.CompareTo(cursorData.Id) > 0));
            }
            catch
            {
                return Result<EstimateCatalogResult>.Failure(
                    new Error("ITEM_CATALOG_CURSOR_INVALID", "Catalog cursor is invalid."));
            }
        }

        // 7. Sort and Fetch pageSize + 1
        queryable = queryable
            .OrderBy(i => i.NormalizedCode)
            .ThenBy(i => i.Id);

        var fetchedItems = await queryable
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNextPage = fetchedItems.Count > pageSize;
        var pagedItems = hasNextPage ? fetchedItems.Take(pageSize).ToList() : fetchedItems;

        var itemIds = pagedItems.Select(i => i.Id).ToList();

        // 8. Batch Load Primary Images (Zero N+1)
        var primaryImages = await _db.ItemImages
            .AsNoTracking()
            .Where(im => im.OrganizationId == orgId && itemIds.Contains(im.ItemId) && im.IsPrimary && im.Status == ItemStatus.Active)
            .ToDictionaryAsync(im => im.ItemId, ct);

        // 9. Batch Load Costs (Zero N+1)
        var publishedCosts = await _db.CostRecords
            .AsNoTracking()
            .Include(c => c.Unit)
            .Include(c => c.CostSource)
            .Where(c => c.OrganizationId == orgId
                && itemIds.Contains(c.ItemId)
                && _db.Items.Any(i => i.OrganizationId == orgId && i.Id == c.ItemId && i.BaseUnitId == c.UnitId)
                && (c.Status == CostRecordStatus.Published || c.Status == CostRecordStatus.Superseded)
                && c.EffectiveFromUtc <= now
                && (c.EffectiveToUtc == null || c.EffectiveToUtc >= now)
                && c.MinimumQuantity <= 1m
                && (c.MaximumQuantity == null || c.MaximumQuantity >= 1m)
                && (c.Scope == CostScopeType.Organization || (c.Scope == CostScopeType.Branch && c.BranchId == branchId)))
            .ToListAsync(ct);

        var costsByItem = publishedCosts
            .GroupBy(c => c.ItemId)
            .ToDictionary(g => g.Key, g => _costResolver.ResolveForCatalog(g.ToList(), branchId));

        // 10. Map to Projections
        var projectedItems = new List<EstimateCatalogItemProjection>();
        foreach (var item in pagedItems)
        {
            var resolvedCost = costsByItem.GetValueOrDefault(item.Id);

            primaryImages.TryGetValue(item.Id, out var img);
            var primaryImageProj = img != null
                ? new CatalogPrimaryImageProjection(img.FileId, new LocalizedTextDto(img.AltText.Thai, img.AltText.English))
                : null;

            projectedItems.Add(new EstimateCatalogItemProjection(
                item.Id,
                item.Code,
                new LocalizedTextDto(item.Name.Thai, item.Name.English),
                item.Description != null ? new LocalizedTextDto(item.Description.Thai, item.Description.English) : null,
                item.ItemType,
                new CategorySummaryDto(item.Category!.Id, item.Category.Code, new LocalizedTextDto(item.Category.Name.Thai, item.Category.Name.English), item.Category.ParentCategoryId, item.Category.ImageFileId),
                item.Brand != null ? new BrandSummaryDto(item.Brand.Id, item.Brand.Code, new LocalizedTextDto(item.Brand.Name.Thai, item.Brand.Name.English), item.Brand.ImageFileId) : null,
                new UnitSummaryDto(item.BaseUnit!.Id, item.BaseUnit.Code, item.BaseUnit.Symbol, new LocalizedTextDto(item.BaseUnit.Name.Thai, item.BaseUnit.Name.English)),
                item.Attributes,
                primaryImageProj,
                resolvedCost));
        }

        // 11. Calculate Facets
        var facets = await CalculateFacetsAsync(facetsBase, orgId, ct);

        // 12. Next Cursor
        string? nextCursor = null;
        if (hasNextPage && pagedItems.Count > 0)
        {
            var last = pagedItems[^1];
            var cursorObj = new CursorData { Code = last.NormalizedCode, Id = last.Id };
            var json = JsonSerializer.Serialize(cursorObj);
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        return Result<EstimateCatalogResult>.Success(new EstimateCatalogResult(
            projectedItems,
            facets,
            nextCursor,
            hasNextPage));
    }

    private async Task<EstimateCatalogFacets> CalculateFacetsAsync(IQueryable<Item> baseQuery, Guid orgId, CancellationToken ct)
    {
        var itemTypes = await baseQuery
            .GroupBy(i => i.ItemType)
            .Select(g => new CatalogFacetItem(g.Key, g.Count()))
            .ToListAsync(ct);

        var categoryCounts = await baseQuery
            .GroupBy(i => i.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var categoryIds = categoryCounts.Select(c => c.CategoryId).ToList();
        var categoryEntities = await _db.ItemCategories
            .AsNoTracking()
            .Where(c => c.OrganizationId == orgId && categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, ct);

        var categories = categoryCounts
            .Where(c => categoryEntities.ContainsKey(c.CategoryId))
            .Select(c =>
            {
                var cat = categoryEntities[c.CategoryId];
                return new CatalogCategoryFacet(cat.Id, new LocalizedTextDto(cat.Name.Thai, cat.Name.English), cat.ImageFileId, c.Count);
            })
            .ToList();

        var brandCounts = await baseQuery
            .Where(i => i.BrandId != null)
            .GroupBy(i => i.BrandId!.Value)
            .Select(g => new { BrandId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var brandIds = brandCounts.Select(b => b.BrandId).ToList();
        var brandEntities = await _db.ItemBrands
            .AsNoTracking()
            .Where(b => b.OrganizationId == orgId && brandIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, ct);

        var brands = brandCounts
            .Where(b => brandEntities.ContainsKey(b.BrandId))
            .Select(b =>
            {
                var br = brandEntities[b.BrandId];
                return new CatalogBrandFacet(br.Id, new LocalizedTextDto(br.Name.Thai, br.Name.English), br.ImageFileId, b.Count);
            })
            .ToList();

        var attributeValues = await baseQuery
            .Where(i => i.Attributes != null)
            .Select(i => i.Attributes!)
            .ToListAsync(ct);

        var attributes = attributeValues
            .SelectMany(values => values)
            .GroupBy(attribute => new { attribute.Key, attribute.Value })
            .Select(group => new CatalogAttributeFacet(group.Key.Key, group.Key.Value, group.Count()))
            .OrderBy(attribute => attribute.Key)
            .ThenBy(attribute => attribute.Value)
            .ToList();

        return new EstimateCatalogFacets(itemTypes, categories, brands, attributes);
    }
}
