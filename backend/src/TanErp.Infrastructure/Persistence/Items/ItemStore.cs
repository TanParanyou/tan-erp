using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Domain.Common;
using TanErp.Domain.Items;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Persistence.Items;

public class ItemStore : IItemStore
{
    private readonly AppDbContext _db;

    public ItemStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ItemDetailProjection>> CreateItemAsync(
        CreateItemData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;

        // Verify category exists in same org
        var categoryExists = await _db.ItemCategories
            .AnyAsync(c => c.Id == data.CategoryId && c.OrganizationId == orgId, ct);
        if (!categoryExists)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_CATEGORY_NOT_FOUND", "Category not found in organization."));
        }

        // Verify brand if provided
        if (data.BrandId.HasValue)
        {
            var brandExists = await _db.ItemBrands
                .AnyAsync(b => b.Id == data.BrandId.Value && b.OrganizationId == orgId, ct);
            if (!brandExists)
            {
                return Result<ItemDetailProjection>.Failure(new Error("ITEM_BRAND_NOT_FOUND", "Brand not found in organization."));
            }
        }

        // Verify base unit exists
        var unitExists = await _db.Units
            .AnyAsync(u => u.Id == data.BaseUnitId && u.OrganizationId == orgId, ct);
        if (!unitExists)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_UNIT_NOT_FOUND", "Base unit not found in organization."));
        }

        // Check duplicate code
        var normalizedCode = data.Code.Trim().ToUpperInvariant();
        var codeExists = await _db.Items
            .AnyAsync(i => i.OrganizationId == orgId && i.NormalizedCode == normalizedCode, ct);
        if (codeExists)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Item code '{data.Code}' already exists."));
        }

        var itemId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var description = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);
        var capabilities = new ItemCapabilities(
            data.Capabilities.CanSell,
            data.Capabilities.CanCost,
            data.Capabilities.CanPurchase,
            data.Capabilities.CanStock,
            data.Capabilities.CanProduce);

        Item item;
        try
        {
            item = Item.CreateDraft(
                itemId,
                orgId,
                data.Code,
                data.ItemType,
                data.CategoryId,
                data.BrandId,
                name,
                description,
                data.BaseUnitId,
                data.AvailabilityMode,
                capabilities,
                data.Attributes,
                data.AttributesSchemaVersion,
                access.ActorUserId,
                now);
        }
        catch (ItemDomainException ex)
        {
            return Result<ItemDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        _db.Items.Add(item);

        // Branch availability
        if (data.AvailabilityMode == ItemAvailabilityMode.SelectedBranches && data.SelectedBranchIds != null)
        {
            foreach (var branchId in data.SelectedBranchIds)
            {
                var branchExists = await _db.Branches
                    .AnyAsync(b => b.Id == branchId && b.OrganizationId == orgId, ct);
                if (!branchExists)
                {
                    await tx.RollbackAsync(ct);
                    return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", $"Branch '{branchId}' not found in organization."));
                }

                var avail = new ItemBranchAvailability(Guid.NewGuid(), orgId, itemId, branchId, null, null, access.ActorUserId, now);
                _db.ItemBranchAvailabilities.Add(avail);
            }
        }

        // Aliases
        if (data.Aliases != null)
        {
            foreach (var aliasDto in data.Aliases)
            {
                var aliasText = LocalizedText.Create(aliasDto.Thai, aliasDto.English);
                var alias = new ItemAlias(Guid.NewGuid(), orgId, itemId, aliasText, access.ActorUserId, now);
                _db.ItemAliases.Add(alias);
            }
        }

        // Audit Event
        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.create",
            "item",
            itemId.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { code = item.Code, name = item.Name }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: item.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var result = await GetItemAsync(orgId, itemId, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    public async Task<ItemDetailProjection?> GetItemAsync(Guid organizationId, Guid itemId, CancellationToken ct)
    {
        var item = await _db.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Brand)
            .Include(i => i.BaseUnit)
            .Include(i => i.Aliases.Where(a => a.Status == ItemStatus.Active))
            .Include(i => i.BranchAvailabilities.Where(b => b.Status == ItemStatus.Active))
            .Include(i => i.Images.Where(im => im.Status == ItemStatus.Active))
            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrganizationId == organizationId, ct);

        if (item == null) return null;

        return await MapToDetailProjectionAsync(item, ct);
    }

    public async Task<PagedItemsResult> ListItemsAsync(ItemQuery query, CancellationToken ct)
    {
        var q = _db.Items
            .AsNoTracking()
            .Where(i => i.OrganizationId == query.OrganizationId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            q = q.Where(i =>
                i.NormalizedCode.Contains(search.ToUpperInvariant())
                || i.Name.Thai.ToLower().Contains(search)
                || (i.Name.English != null && i.Name.English.ToLower().Contains(search))
                || i.Aliases.Any(a => a.NormalizedTh.Contains(search) || (a.NormalizedEn != null && a.NormalizedEn.Contains(search))));
        }

        if (!string.IsNullOrWhiteSpace(query.ItemType))
        {
            q = q.Where(i => i.ItemType == query.ItemType.ToLowerInvariant());
        }

        if (query.CategoryId.HasValue)
        {
            q = q.Where(i => i.CategoryId == query.CategoryId.Value);
        }

        if (query.BrandId.HasValue)
        {
            q = q.Where(i => i.BrandId == query.BrandId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            q = q.Where(i => i.Status == query.Status.ToLowerInvariant());
        }

        var totalCount = await q.CountAsync(ct);

        var items = await q
            .OrderBy(i => i.NormalizedCode)
            .ThenBy(i => i.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(i => i.Category)
            .Include(i => i.Brand)
            .Include(i => i.BaseUnit)
            .Include(i => i.Aliases.Where(a => a.Status == ItemStatus.Active))
            .Include(i => i.BranchAvailabilities.Where(b => b.Status == ItemStatus.Active))
            .Include(i => i.Images.Where(im => im.Status == ItemStatus.Active))
            .ToListAsync(ct);

        var projections = new List<ItemDetailProjection>();
        foreach (var item in items)
        {
            projections.Add(await MapToDetailProjectionAsync(item, ct));
        }

        return new PagedItemsResult(projections, totalCount, query.PageNumber, query.PageSize);
    }

    public async Task<Result<ItemDetailProjection>> UpdateItemAsync(
        UpdateItemData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var item = await _db.Items
            .FirstOrDefaultAsync(i => i.Id == data.ItemId && i.OrganizationId == orgId, ct);

        if (item == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        }

        if (item.RowVersion != data.ExpectedRowVersion)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Item has been modified by another user."));
        }

        var rowVersionBefore = item.RowVersion;
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var description = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);
        var capabilities = new ItemCapabilities(
            data.Capabilities.CanSell,
            data.Capabilities.CanCost,
            data.Capabilities.CanPurchase,
            data.Capabilities.CanStock,
            data.Capabilities.CanProduce);

        try
        {
            item.UpdateDetails(
                data.Code,
                data.ItemType,
                data.CategoryId,
                data.BrandId,
                name,
                description,
                data.BaseUnitId,
                data.AvailabilityMode,
                capabilities,
                data.Attributes,
                data.AttributesSchemaVersion,
                access.ActorUserId,
                now);
        }
        catch (ItemValidationException ex)
        {
            return Result<ItemDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.update",
            "item",
            item.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { code = item.Code, name = item.Name }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: item.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var result = await GetItemAsync(orgId, item.Id, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    public async Task<Result<ItemDetailProjection>> ActivateItemAsync(
        Guid organizationId,
        Guid itemId,
        Guid expectedRowVersion,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var item = await _db.Items
            .Include(i => i.BranchAvailabilities)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrganizationId == organizationId, ct);

        if (item == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        }

        if (item.RowVersion != expectedRowVersion)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Item has been modified by another user."));
        }

        var rowVersionBefore = item.RowVersion;
        var now = DateTimeOffset.UtcNow;
        var hasActiveBranch = item.BranchAvailabilities.Any(b => b.Status == ItemStatus.Active);

        try
        {
            item.Activate(access.ActorUserId, now, hasActiveBranch);
        }
        catch (ItemValidationException ex)
        {
            return Result<ItemDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            access.ActorUserId,
            "items.activate",
            "item",
            item.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { status = item.Status }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: item.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var result = await GetItemAsync(organizationId, item.Id, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    public async Task<Result<ItemDetailProjection>> DeactivateItemAsync(
        Guid organizationId,
        Guid itemId,
        Guid expectedRowVersion,
        string reasonCode,
        string? reason,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var item = await _db.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrganizationId == organizationId, ct);

        if (item == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        }

        if (item.RowVersion != expectedRowVersion)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Item has been modified by another user."));
        }

        var rowVersionBefore = item.RowVersion;
        var now = DateTimeOffset.UtcNow;

        try
        {
            item.Deactivate(access.ActorUserId, now, reasonCode, reason);
        }
        catch (ItemValidationException ex)
        {
            return Result<ItemDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            access.ActorUserId,
            "items.deactivate",
            "item",
            item.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { status = item.Status, reasonCode, reason }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: item.RowVersion,
            reason: reasonCode);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var result = await GetItemAsync(organizationId, item.Id, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    public async Task<Result<ItemDetailProjection>> SetBranchAvailabilityAsync(
        Guid organizationId,
        Guid itemId,
        Guid expectedRowVersion,
        string mode,
        IReadOnlyList<Guid> branchIds,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var item = await _db.Items
            .Include(i => i.BranchAvailabilities)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrganizationId == organizationId, ct);

        if (item == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        }

        if (item.RowVersion != expectedRowVersion)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Item has been modified by another user."));
        }

        var rowVersionBefore = item.RowVersion;
        var now = DateTimeOffset.UtcNow;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Deactivate old branch availabilities
        foreach (var existing in item.BranchAvailabilities)
        {
            existing.Deactivate("Availability updated", access.ActorUserId, now);
        }

        if (mode == ItemAvailabilityMode.SelectedBranches)
        {
            foreach (var branchId in branchIds)
            {
                var branchExists = await _db.Branches
                    .AnyAsync(b => b.Id == branchId && b.OrganizationId == organizationId, ct);
                if (!branchExists)
                {
                    await tx.RollbackAsync(ct);
                    return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", $"Branch '{branchId}' not found in organization."));
                }

                var existing = item.BranchAvailabilities.FirstOrDefault(b => b.BranchId == branchId);
                if (existing != null)
                {
                    existing.Reactivate(null, null, access.ActorUserId, now);
                }
                else
                {
                    var avail = new ItemBranchAvailability(Guid.NewGuid(), organizationId, itemId, branchId, null, null, access.ActorUserId, now);
                    _db.ItemBranchAvailabilities.Add(avail);
                }
            }
        }

        item.UpdateDetails(
            item.Code,
            item.ItemType,
            item.CategoryId,
            item.BrandId,
            item.Name,
            item.Description,
            item.BaseUnitId,
            mode,
            item.Capabilities,
            item.Attributes,
            item.AttributesSchemaVersion,
            access.ActorUserId,
            now);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            access.ActorUserId,
            "items.update-branches",
            "item",
            item.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { mode, branchIds }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: item.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var result = await GetItemAsync(organizationId, item.Id, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    // Taxonomy Category Implementation
    public async Task<IReadOnlyList<ItemCategoryDetailProjection>> ListCategoriesAsync(Guid organizationId, CancellationToken ct)
    {
        var categories = await _db.ItemCategories
            .AsNoTracking()
            .Where(c => c.OrganizationId == organizationId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.NormalizedCode)
            .ToListAsync(ct);

        return categories.Select(c => MapCategory(c, categories)).ToList();
    }

    public async Task<ItemCategoryDetailProjection?> GetCategoryAsync(Guid organizationId, Guid categoryId, CancellationToken ct)
    {
        var category = await _db.ItemCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.OrganizationId == organizationId, ct);

        if (category == null) return null;

        ItemCategory? parent = null;
        if (category.ParentCategoryId.HasValue)
        {
            parent = await _db.ItemCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.ParentCategoryId.Value && c.OrganizationId == organizationId, ct);
        }

        return MapCategory(category, parent);
    }

    public async Task<Result<ItemCategoryDetailProjection>> CreateCategoryAsync(
        CreateCategoryData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var normalizedCode = data.Code.Trim().ToUpperInvariant();

        var codeExists = await _db.ItemCategories
            .AnyAsync(c => c.OrganizationId == orgId && c.NormalizedCode == normalizedCode, ct);
        if (codeExists)
        {
            return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Category code '{data.Code}' already exists."));
        }

        if (data.ParentCategoryId.HasValue)
        {
            var parentExists = await _db.ItemCategories
                .AnyAsync(c => c.Id == data.ParentCategoryId.Value && c.OrganizationId == orgId, ct);
            if (!parentExists)
            {
                return Result<ItemCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Parent category not found in organization."));
            }
        }

        var catId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var desc = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);
        var allowedTypes = data.AllowedItemTypes?.ToArray() ?? ItemType.All;

        var category = new ItemCategory(
            catId,
            orgId,
            data.Code,
            name,
            desc,
            data.ParentCategoryId,
            allowedTypes,
            data.SortOrder,
            access.ActorUserId,
            now);

        _db.ItemCategories.Add(category);
        await _db.SaveChangesAsync(ct);

        return Result<ItemCategoryDetailProjection>.Success(await GetCategoryAsync(orgId, catId, ct) ?? null!);
    }

    public async Task<Result<ItemCategoryDetailProjection>> UpdateCategoryAsync(
        UpdateCategoryData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var category = await _db.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == data.CategoryId && c.OrganizationId == orgId, ct);

        if (category == null)
        {
            return Result<ItemCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Category not found."));
        }

        if (category.RowVersion != data.ExpectedRowVersion)
        {
            return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Category has been modified by another user."));
        }

        if (data.ParentCategoryId.HasValue)
        {
            if (data.ParentCategoryId.Value == data.CategoryId)
            {
                return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_CATEGORY_CYCLE", "Category cannot be its own parent."));
            }

            var parentExists = await _db.ItemCategories
                .AnyAsync(c => c.Id == data.ParentCategoryId.Value && c.OrganizationId == orgId, ct);
            if (!parentExists)
            {
                return Result<ItemCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Parent category not found in organization."));
            }
        }

        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var desc = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);
        var allowedTypes = data.AllowedItemTypes?.ToArray() ?? ItemType.All;

        category.Update(
            data.Code,
            name,
            desc,
            data.ParentCategoryId,
            allowedTypes,
            data.SortOrder,
            access.ActorUserId,
            now);

        await _db.SaveChangesAsync(ct);
        return Result<ItemCategoryDetailProjection>.Success(await GetCategoryAsync(orgId, category.Id, ct) ?? null!);
    }

    // Taxonomy Brand Implementation
    public async Task<IReadOnlyList<ItemBrandDetailProjection>> ListBrandsAsync(Guid organizationId, CancellationToken ct)
    {
        var brands = await _db.ItemBrands
            .AsNoTracking()
            .Where(b => b.OrganizationId == organizationId)
            .OrderBy(b => b.SortOrder)
            .ThenBy(b => b.NormalizedCode)
            .ToListAsync(ct);

        return brands.Select(MapBrand).ToList();
    }

    public async Task<ItemBrandDetailProjection?> GetBrandAsync(Guid organizationId, Guid brandId, CancellationToken ct)
    {
        var brand = await _db.ItemBrands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == brandId && b.OrganizationId == organizationId, ct);

        return brand == null ? null : MapBrand(brand);
    }

    public async Task<Result<ItemBrandDetailProjection>> CreateBrandAsync(
        CreateBrandData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var normalizedCode = data.Code.Trim().ToUpperInvariant();

        var exists = await _db.ItemBrands
            .AnyAsync(b => b.OrganizationId == orgId && b.NormalizedCode == normalizedCode, ct);
        if (exists)
        {
            return Result<ItemBrandDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Brand code '{data.Code}' already exists."));
        }

        var brandId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var desc = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);

        var brand = new ItemBrand(brandId, orgId, data.Code, name, desc, data.SortOrder, access.ActorUserId, now);
        _db.ItemBrands.Add(brand);
        await _db.SaveChangesAsync(ct);

        return Result<ItemBrandDetailProjection>.Success(MapBrand(brand));
    }

    public async Task<Result<ItemBrandDetailProjection>> UpdateBrandAsync(
        UpdateBrandData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var brand = await _db.ItemBrands
            .FirstOrDefaultAsync(b => b.Id == data.BrandId && b.OrganizationId == orgId, ct);

        if (brand == null)
        {
            return Result<ItemBrandDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Brand not found."));
        }

        if (brand.RowVersion != data.ExpectedRowVersion)
        {
            return Result<ItemBrandDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Brand has been modified by another user."));
        }

        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var desc = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);

        brand.Update(data.Code, name, desc, data.SortOrder, access.ActorUserId, now);
        await _db.SaveChangesAsync(ct);

        return Result<ItemBrandDetailProjection>.Success(MapBrand(brand));
    }

    // Taxonomy Unit Implementation
    public async Task<IReadOnlyList<UnitOfMeasureDetailProjection>> ListUnitsAsync(Guid organizationId, CancellationToken ct)
    {
        var units = await _db.Units
            .AsNoTracking()
            .Where(u => u.OrganizationId == organizationId)
            .OrderBy(u => u.NormalizedCode)
            .ToListAsync(ct);

        return units.Select(MapUnit).ToList();
    }

    public async Task<UnitOfMeasureDetailProjection?> GetUnitAsync(Guid organizationId, Guid unitId, CancellationToken ct)
    {
        var unit = await _db.Units
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == unitId && u.OrganizationId == organizationId, ct);

        return unit == null ? null : MapUnit(unit);
    }

    public async Task<Result<UnitOfMeasureDetailProjection>> CreateUnitAsync(
        CreateUnitData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var normalizedCode = data.Code.Trim().ToUpperInvariant();

        var exists = await _db.Units
            .AnyAsync(u => u.OrganizationId == orgId && u.NormalizedCode == normalizedCode, ct);
        if (exists)
        {
            return Result<UnitOfMeasureDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Unit code '{data.Code}' already exists."));
        }

        var unitId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);

        var unit = new UnitOfMeasure(
            unitId,
            orgId,
            data.Code,
            name,
            data.Symbol,
            data.Dimension,
            data.DecimalScale,
            data.RoundingMode,
            access.ActorUserId,
            now);

        _db.Units.Add(unit);
        await _db.SaveChangesAsync(ct);

        return Result<UnitOfMeasureDetailProjection>.Success(MapUnit(unit));
    }

    public async Task<Result<UnitOfMeasureDetailProjection>> UpdateUnitAsync(
        UpdateUnitData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var unit = await _db.Units
            .FirstOrDefaultAsync(u => u.Id == data.UnitId && u.OrganizationId == orgId, ct);

        if (unit == null)
        {
            return Result<UnitOfMeasureDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Unit not found."));
        }

        if (unit.RowVersion != data.ExpectedRowVersion)
        {
            return Result<UnitOfMeasureDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Unit has been modified by another user."));
        }

        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);

        unit.Update(
            data.Code,
            name,
            data.Symbol,
            data.Dimension,
            data.DecimalScale,
            data.RoundingMode,
            access.ActorUserId,
            now);

        await _db.SaveChangesAsync(ct);
        return Result<UnitOfMeasureDetailProjection>.Success(MapUnit(unit));
    }

    // Alias Implementation
    public async Task<Result<ItemDetailProjection>> AddAliasAsync(
        Guid organizationId,
        Guid itemId,
        LocalizedTextDto aliasDto,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var item = await _db.Items
            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrganizationId == organizationId, ct);

        if (item == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        }

        var aliasText = LocalizedText.Create(aliasDto.Thai, aliasDto.English);
        var normTh = aliasText.Thai.Trim().ToLowerInvariant();

        var exists = await _db.ItemAliases
            .AnyAsync(a => a.OrganizationId == organizationId && a.ItemId == itemId && a.NormalizedTh == normTh && a.Status == ItemStatus.Active, ct);
        if (exists)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_ALIAS_CONFLICT", "Alias already exists for this item."));
        }

        var aliasId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var alias = new ItemAlias(aliasId, organizationId, itemId, aliasText, access.ActorUserId, now);

        _db.ItemAliases.Add(alias);
        await _db.SaveChangesAsync(ct);

        var result = await GetItemAsync(organizationId, itemId, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    public async Task<Result<ItemDetailProjection>> RemoveAliasAsync(
        Guid organizationId,
        Guid itemId,
        Guid aliasId,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var alias = await _db.ItemAliases
            .FirstOrDefaultAsync(a => a.Id == aliasId && a.ItemId == itemId && a.OrganizationId == organizationId, ct);

        if (alias == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Alias not found."));
        }

        var now = DateTimeOffset.UtcNow;
        alias.Deactivate(access.ActorUserId, now);
        await _db.SaveChangesAsync(ct);

        var result = await GetItemAsync(organizationId, itemId, ct);
        return Result<ItemDetailProjection>.Success(result!);
    }

    private async Task<ItemDetailProjection> MapToDetailProjectionAsync(Item item, CancellationToken ct)
    {
        CategorySummaryDto categoryDto;
        if (item.Category != null)
        {
            categoryDto = new CategorySummaryDto(
                item.Category.Id,
                item.Category.Code,
                new LocalizedTextDto(item.Category.Name.Thai, item.Category.Name.English),
                item.Category.ParentCategoryId);
        }
        else
        {
            var cat = await _db.ItemCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == item.CategoryId, ct);
            categoryDto = new CategorySummaryDto(
                cat!.Id,
                cat.Code,
                new LocalizedTextDto(cat.Name.Thai, cat.Name.English),
                cat.ParentCategoryId);
        }

        BrandSummaryDto? brandDto = null;
        if (item.Brand != null)
        {
            brandDto = new BrandSummaryDto(
                item.Brand.Id,
                item.Brand.Code,
                new LocalizedTextDto(item.Brand.Name.Thai, item.Brand.Name.English));
        }
        else if (item.BrandId.HasValue)
        {
            var b = await _db.ItemBrands.AsNoTracking().FirstOrDefaultAsync(br => br.Id == item.BrandId.Value, ct);
            if (b != null)
            {
                brandDto = new BrandSummaryDto(
                    b.Id,
                    b.Code,
                    new LocalizedTextDto(b.Name.Thai, b.Name.English));
            }
        }

        UnitSummaryDto baseUnitDto;
        if (item.BaseUnit != null)
        {
            baseUnitDto = new UnitSummaryDto(
                item.BaseUnit.Id,
                item.BaseUnit.Code,
                item.BaseUnit.Symbol,
                new LocalizedTextDto(item.BaseUnit.Name.Thai, item.BaseUnit.Name.English));
        }
        else
        {
            var u = await _db.Units.AsNoTracking().FirstOrDefaultAsync(unit => unit.Id == item.BaseUnitId, ct);
            baseUnitDto = new UnitSummaryDto(
                u!.Id,
                u.Code,
                u.Symbol,
                new LocalizedTextDto(u.Name.Thai, u.Name.English));
        }

        // Branch Availabilities
        var branchAvailDtos = new List<ItemBranchAvailabilityDto>();
        var activeAvails = item.BranchAvailabilities.Where(b => b.Status == ItemStatus.Active).ToList();
        if (activeAvails.Count > 0)
        {
            var branchIds = activeAvails.Select(b => b.BranchId).ToList();
            var branches = await _db.Branches.AsNoTracking()
                .Where(b => branchIds.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id, ct);

            foreach (var ba in activeAvails)
            {
                if (branches.TryGetValue(ba.BranchId, out var br))
                {
                    branchAvailDtos.Add(new ItemBranchAvailabilityDto(
                        ba.Id,
                        ba.BranchId,
                        br.Code,
                        br.Name,
                        ba.Status,
                        ba.EffectiveFromUtc,
                        ba.EffectiveToUtc));
                }
            }
        }

        // Aliases
        var aliasDtos = item.Aliases
            .Where(a => a.Status == ItemStatus.Active)
            .Select(a => new ItemAliasDto(a.Id, new LocalizedTextDto(a.Alias.Thai, a.Alias.English), a.Status))
            .ToList();

        // Primary Image
        ItemImageSummaryDto? primaryImageDto = null;
        var primaryImage = item.Images.FirstOrDefault(im => im.IsPrimary && im.Status == ItemStatus.Active);
        if (primaryImage != null)
        {
            primaryImageDto = new ItemImageSummaryDto(
                primaryImage.Id,
                primaryImage.FileId,
                primaryImage.Role,
                primaryImage.IsPrimary,
                primaryImage.DisplayOrder,
                new LocalizedTextDto(primaryImage.AltText.Thai, primaryImage.AltText.English),
                primaryImage.Caption == null ? null : new LocalizedTextDto(primaryImage.Caption.Thai, primaryImage.Caption.English));
        }

        return new ItemDetailProjection(
            item.Id,
            item.OrganizationId,
            item.Code,
            item.ItemType,
            categoryDto,
            brandDto,
            baseUnitDto,
            new LocalizedTextDto(item.Name.Thai, item.Name.English),
            item.Description == null ? null : new LocalizedTextDto(item.Description.Thai, item.Description.English),
            item.TaxCategoryCode,
            item.AvailabilityMode,
            new ItemCapabilitiesDto(
                item.Capabilities.CanSell,
                item.Capabilities.CanCost,
                item.Capabilities.CanPurchase,
                item.Capabilities.CanStock,
                item.Capabilities.CanProduce),
            item.Attributes,
            item.AttributesSchemaVersion,
            item.Status,
            item.ActivatedOnce,
            item.ActivatedAtUtc,
            item.ActivatedByUserId,
            item.InactiveAtUtc,
            item.InactiveByUserId,
            item.InactiveReasonCode,
            item.InactiveReason,
            aliasDtos,
            branchAvailDtos,
            primaryImageDto,
            item.RowVersion,
            item.CreatedAtUtc,
            item.CreatedByUserId,
            item.UpdatedAtUtc,
            item.UpdatedByUserId);
    }

    private static ItemCategoryDetailProjection MapCategory(ItemCategory category, IEnumerable<ItemCategory> all)
    {
        CategorySummaryDto? parentSummary = null;
        if (category.ParentCategoryId.HasValue)
        {
            var p = all.FirstOrDefault(c => c.Id == category.ParentCategoryId.Value);
            if (p != null)
            {
                parentSummary = new CategorySummaryDto(p.Id, p.Code, new LocalizedTextDto(p.Name.Thai, p.Name.English), p.ParentCategoryId);
            }
        }

        return new ItemCategoryDetailProjection(
            category.Id,
            category.OrganizationId,
            category.Code,
            new LocalizedTextDto(category.Name.Thai, category.Name.English),
            category.Description == null ? null : new LocalizedTextDto(category.Description.Thai, category.Description.English),
            category.ParentCategoryId,
            parentSummary,
            category.AllowedItemTypes,
            category.SortOrder,
            category.Status,
            category.RowVersion,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }

    private static ItemCategoryDetailProjection MapCategory(ItemCategory category, ItemCategory? parent)
    {
        CategorySummaryDto? parentSummary = null;
        if (parent != null)
        {
            parentSummary = new CategorySummaryDto(parent.Id, parent.Code, new LocalizedTextDto(parent.Name.Thai, parent.Name.English), parent.ParentCategoryId);
        }

        return new ItemCategoryDetailProjection(
            category.Id,
            category.OrganizationId,
            category.Code,
            new LocalizedTextDto(category.Name.Thai, category.Name.English),
            category.Description == null ? null : new LocalizedTextDto(category.Description.Thai, category.Description.English),
            category.ParentCategoryId,
            parentSummary,
            category.AllowedItemTypes,
            category.SortOrder,
            category.Status,
            category.RowVersion,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }

    private static ItemBrandDetailProjection MapBrand(ItemBrand brand)
    {
        return new ItemBrandDetailProjection(
            brand.Id,
            brand.OrganizationId,
            brand.Code,
            new LocalizedTextDto(brand.Name.Thai, brand.Name.English),
            brand.Description == null ? null : new LocalizedTextDto(brand.Description.Thai, brand.Description.English),
            brand.SortOrder,
            brand.Status,
            brand.RowVersion,
            brand.CreatedAtUtc,
            brand.UpdatedAtUtc);
    }

    private static UnitOfMeasureDetailProjection MapUnit(UnitOfMeasure unit)
    {
        return new UnitOfMeasureDetailProjection(
            unit.Id,
            unit.OrganizationId,
            unit.Code,
            new LocalizedTextDto(unit.Name.Thai, unit.Name.English),
            unit.Symbol,
            unit.Dimension,
            unit.DecimalScale,
            unit.RoundingMode,
            unit.Status,
            unit.RowVersion,
            unit.CreatedAtUtc,
            unit.UpdatedAtUtc);
    }
}
