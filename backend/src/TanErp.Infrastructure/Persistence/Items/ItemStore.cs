using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Application.Files;
using TanErp.Domain.Common;
using TanErp.Domain.Items;
using TanErp.Domain.DocumentNumbering;
using TanErp.Infrastructure.Persistence.DocumentNumbering;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Persistence.Items;

public class ItemStore : IItemStore
{
    private readonly AppDbContext _db;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly IFileStore _fileStore;

    public ItemStore(AppDbContext db, IDocumentNumberGenerator documentNumberGenerator, IFileStore fileStore)
    {
        _db = db;
        _documentNumberGenerator = documentNumberGenerator;
        _fileStore = fileStore;
    }

    public async Task<Result<ItemDetailProjection>> CreateItemAsync(
        CreateItemData data,
        RequestAccessContext access,
        string idempotencyKey,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        const string operation = "item-master.item.create";
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<ItemDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await GetItemAsync(orgId, replayId, ct);
            return replayed is null ? Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created item no longer exists.")) : Result<ItemDetailProjection>.Success(replayed);
        }

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

        var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
            _documentNumberGenerator, orgId, DocumentTypes.Items, data.Code, 50, "ITEM_CODE_CONFLICT",
            async (code, token) => await _db.Items.AnyAsync(i => i.OrganizationId == orgId && i.NormalizedCode == code.Trim().ToUpperInvariant(), token), ct);
        if (resolvedCode.IsFailure) return Result<ItemDetailProjection>.Failure(resolvedCode.Error);
        var itemCode = resolvedCode.Value!;

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
                itemCode,
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
                now,
                data.TaxCategoryCode);
        }
        catch (ItemDomainException ex)
        {
            return Result<ItemDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

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
            JsonSerializer.Serialize(new { code = item.Code, name = item.Name, codeGenerated = data.Code is null }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: item.RowVersion);

        _db.AuditEvents.Add(audit);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, itemId.ToString(), now));

        try { await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
        catch (DbUpdateException exception) when (IsMasterDataCodeUniqueViolation(exception))
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", "The code already exists in this organization."));
        }

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

        var orderedQuery = (query.SortBy, query.SortOrder) switch
        {
            (ItemSortKey.ItemType, ItemSortOrder.Desc) => q.OrderByDescending(i => i.ItemType).ThenBy(i => i.Id),
            (ItemSortKey.ItemType, _) => q.OrderBy(i => i.ItemType).ThenBy(i => i.Id),
            (ItemSortKey.Status, ItemSortOrder.Desc) => q.OrderByDescending(i => i.Status).ThenBy(i => i.Id),
            (ItemSortKey.Status, _) => q.OrderBy(i => i.Status).ThenBy(i => i.Id),
            (ItemSortKey.Code, ItemSortOrder.Desc) => q.OrderByDescending(i => i.NormalizedCode).ThenBy(i => i.Id),
            _ => q.OrderBy(i => i.NormalizedCode).ThenBy(i => i.Id)
        };

        var items = await orderedQuery
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
            .Include(i => i.BranchAvailabilities)
            .FirstOrDefaultAsync(i => i.Id == data.ItemId && i.OrganizationId == orgId, ct);

        if (item == null)
        {
            return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        }

        if (item.RowVersion != data.ExpectedRowVersion)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Item has been modified by another user."));
        }

        if (data.SelectedBranchIds is null && item.AvailabilityMode != data.AvailabilityMode)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_BRANCH_IDS_REQUIRED", "Branch IDs are required when changing item availability mode."));
        }

        // Verify category exists in same org
        var categoryExists = await _db.ItemCategories
            .AnyAsync(c => c.Id == data.CategoryId && c.OrganizationId == orgId, ct);
        if (!categoryExists)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_CATEGORY_NOT_FOUND", "Category not found in organization."));
        }

        // Verify base unit exists in same org
        var unitExists = await _db.Units
            .AnyAsync(u => u.Id == data.BaseUnitId && u.OrganizationId == orgId, ct);
        if (!unitExists)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_UNIT_NOT_FOUND", "Base unit not found in organization."));
        }

        // Verify brand if provided exists in same org
        if (data.BrandId.HasValue)
        {
            var brandExists = await _db.ItemBrands
                .AnyAsync(b => b.Id == data.BrandId.Value && b.OrganizationId == orgId, ct);
            if (!brandExists)
            {
                return Result<ItemDetailProjection>.Failure(new Error("ITEM_BRAND_NOT_FOUND", "Brand not found in organization."));
            }
        }

        if (data.SelectedBranchIds is not null)
        {
            var requestedBranchIds = data.SelectedBranchIds.Distinct().ToArray();
            var activeBranchCount = await _db.Branches.CountAsync(
                branch => branch.OrganizationId == orgId && branch.IsActive && requestedBranchIds.Contains(branch.Id), ct);
            if (activeBranchCount != requestedBranchIds.Length)
            {
                return Result<ItemDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "One or more selected branches are unavailable in this organization."));
            }
        }

        // Check duplicate code if changed
        var normalizedCode = data.Code.Trim().ToUpperInvariant();
        var codeConflict = await _db.Items
            .AnyAsync(i => i.OrganizationId == orgId && i.Id != item.Id && i.NormalizedCode == normalizedCode, ct);
        if (codeConflict)
        {
            return Result<ItemDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Item code '{data.Code}' already exists."));
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
                now,
                data.TaxCategoryCode);
        }
        catch (ItemValidationException ex)
        {
            return Result<ItemDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        if (data.SelectedBranchIds is not null)
        {
            var selectedBranchIds = data.AvailabilityMode == ItemAvailabilityMode.SelectedBranches
                ? data.SelectedBranchIds.Distinct().ToHashSet()
                : new HashSet<Guid>();
            foreach (var existing in item.BranchAvailabilities)
            {
                if (existing.Status == ItemStatus.Active && !selectedBranchIds.Contains(existing.BranchId))
                {
                    existing.Deactivate("Availability updated", access.ActorUserId, now);
                }
            }

            foreach (var branchId in selectedBranchIds)
            {
                var existing = item.BranchAvailabilities.FirstOrDefault(availability => availability.BranchId == branchId);
                if (existing is null)
                {
                    _db.ItemBranchAvailabilities.Add(new ItemBranchAvailability(Guid.NewGuid(), orgId, item.Id, branchId, null, null, access.ActorUserId, now));
                }
                else if (existing.Status != ItemStatus.Active)
                {
                    existing.Reactivate(null, null, access.ActorUserId, now);
                }
            }
        }

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

        var cat = await _db.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == item.CategoryId && c.OrganizationId == organizationId, ct);

        var unit = await _db.Units
            .FirstOrDefaultAsync(u => u.Id == item.BaseUnitId && u.OrganizationId == organizationId, ct);

        ItemBrand? brand = null;
        if (item.BrandId.HasValue)
        {
            brand = await _db.ItemBrands
                .FirstOrDefaultAsync(b => b.Id == item.BrandId.Value && b.OrganizationId == organizationId, ct);
        }

        var hasActiveBranch = item.BranchAvailabilities.Any(b => b.Status == ItemStatus.Active);

        try
        {
            item.Activate(
                access.ActorUserId,
                now,
                hasActiveBranch,
                cat?.Status == ItemStatus.Active,
                unit?.Status == ItemStatus.Active,
                !item.BrandId.HasValue || brand?.Status == ItemStatus.Active);
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
            now,
            item.TaxCategoryCode);

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
        string idempotencyKey,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        const string operation = "item-master.category.create";
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<ItemCategoryDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await GetCategoryAsync(orgId, replayId, ct);
            return replayed is null ? Result<ItemCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created category no longer exists.")) : Result<ItemCategoryDetailProjection>.Success(replayed);
        }
        var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
            _documentNumberGenerator, orgId, DocumentTypes.ItemCategories, data.Code, 30, "ITEM_CODE_CONFLICT",
            async (code, token) => await _db.ItemCategories.AnyAsync(c => c.OrganizationId == orgId && c.NormalizedCode == code.Trim().ToUpperInvariant(), token), ct);
        if (resolvedCode.IsFailure) return Result<ItemCategoryDetailProjection>.Failure(resolvedCode.Error);
        var categoryCode = resolvedCode.Value!;

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
            categoryCode,
            name,
            desc,
            data.ParentCategoryId,
            allowedTypes,
            data.SortOrder,
            access.ActorUserId,
            now);

        _db.ItemCategories.Add(category);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "item-categories.create",
            "item-category",
            category.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { code = category.Code, name = category.Name, codeGenerated = data.Code is null }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: category.RowVersion);
        _db.AuditEvents.Add(audit);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, category.Id.ToString(), now));
        try { await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
        catch (DbUpdateException exception) when (IsMasterDataCodeUniqueViolation(exception))
        {
            return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", "The code already exists in this organization."));
        }
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

        var imageValidation = await _fileStore.ValidateVerifiedFilesForParentAsync(
            orgId, access.ActorUserId, TanErp.Domain.Files.FileParentTypes.ItemCategory, category.Id, null,
            data.ImageFileId.HasValue ? new[] { data.ImageFileId.Value } : Array.Empty<Guid>(), ct);
        if (imageValidation.IsFailure)
        {
            return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_IMAGE_NOT_READY", imageValidation.Error.Message ?? "File is not ready or verified."));
        }

        if (data.ParentCategoryId.HasValue)
        {
            var currentParentId = (Guid?)data.ParentCategoryId.Value;
            var visited = new HashSet<Guid> { data.CategoryId };
            while (currentParentId.HasValue)
            {
                if (visited.Contains(currentParentId.Value))
                {
                    return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_CATEGORY_CYCLE", "Category hierarchy cannot contain circular references."));
                }
                visited.Add(currentParentId.Value);

                var parentCat = await _db.ItemCategories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == currentParentId.Value && c.OrganizationId == orgId, ct);
                if (parentCat == null)
                {
                    return Result<ItemCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Parent category not found in organization."));
                }
                currentParentId = parentCat.ParentCategoryId;
            }
        }

        var normalizedCatCode = data.Code.Trim().ToUpperInvariant();
        var catCodeConflict = await _db.ItemCategories
            .AnyAsync(c => c.OrganizationId == orgId && c.Id != category.Id && c.NormalizedCode == normalizedCatCode, ct);
        if (catCodeConflict)
        {
            return Result<ItemCategoryDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Category code '{data.Code}' already exists."));
        }

        var rowVersionBefore = category.RowVersion;
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
            now,
            data.ImageFileId);

        var updateAudit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "item-categories.update",
            "item-category",
            category.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { code = category.Code, name = category.Name, imageFileId = category.ImageFileId }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionBefore: rowVersionBefore,
            rowVersionAfter: category.RowVersion);
        _db.AuditEvents.Add(updateAudit);

        await _db.SaveChangesAsync(ct);
        return Result<ItemCategoryDetailProjection>.Success(await GetCategoryAsync(orgId, category.Id, ct) ?? null!);
    }

    public async Task<IReadOnlyList<CategoryAttributeTemplate>> GetCategoryAttributeTemplatesAsync(
        Guid organizationId,
        Guid categoryId,
        CancellationToken ct)
    {
        var category = await _db.ItemCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.OrganizationId == organizationId, ct);

        if (category == null) return Array.Empty<CategoryAttributeTemplate>();

        // Collect hierarchy chain from root to current category
        var hierarchy = new List<ItemCategory> { category };
        var currentParentId = category.ParentCategoryId;
        var visited = new HashSet<Guid> { category.Id };

        while (currentParentId.HasValue && !visited.Contains(currentParentId.Value))
        {
            visited.Add(currentParentId.Value);
            var parent = await _db.ItemCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == currentParentId.Value && c.OrganizationId == organizationId, ct);

            if (parent == null) break;
            hierarchy.Insert(0, parent); // insert at top so root templates come first
            currentParentId = parent.ParentCategoryId;
        }

        // Merge templates: root templates first, child can override or append by Key
        var merged = new Dictionary<string, CategoryAttributeTemplate>(StringComparer.OrdinalIgnoreCase);
        foreach (var cat in hierarchy)
        {
            foreach (var template in cat.AttributeTemplates)
            {
                merged[template.Key] = template;
            }
        }

        return merged.Values.ToList();
    }

    public async Task<Result<IReadOnlyList<CategoryAttributeTemplate>>> SetCategoryAttributeTemplatesAsync(
        Guid organizationId,
        Guid categoryId,
        List<CategoryAttributeTemplate> templates,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var category = await _db.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.OrganizationId == organizationId, ct);

        if (category == null)
        {
            return Result<IReadOnlyList<CategoryAttributeTemplate>>.Failure(new Error("RESOURCE_NOT_FOUND", "Category not found."));
        }

        var now = DateTimeOffset.UtcNow;
        category.SetAttributeTemplates(templates, access.ActorUserId, now);

        await _db.SaveChangesAsync(ct);
        return Result<IReadOnlyList<CategoryAttributeTemplate>>.Success(category.AttributeTemplates);
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
        string idempotencyKey,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        const string operation = "item-master.brand.create";
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<ItemBrandDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await GetBrandAsync(orgId, replayId, ct);
            return replayed is null ? Result<ItemBrandDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created brand no longer exists.")) : Result<ItemBrandDetailProjection>.Success(replayed);
        }
        var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
            _documentNumberGenerator, orgId, DocumentTypes.ItemBrands, data.Code, 30, "ITEM_CODE_CONFLICT",
            async (code, token) => await _db.ItemBrands.AnyAsync(b => b.OrganizationId == orgId && b.NormalizedCode == code.Trim().ToUpperInvariant(), token), ct);
        if (resolvedCode.IsFailure) return Result<ItemBrandDetailProjection>.Failure(resolvedCode.Error);
        var brandCode = resolvedCode.Value!;

        var brandId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var desc = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);

        var brand = new ItemBrand(brandId, orgId, brandCode, name, desc, data.SortOrder, access.ActorUserId, now);
        _db.ItemBrands.Add(brand);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, brandId.ToString(), now));
        _db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), orgId, access.ActorUserId, "item-brands.create", "item-brand", brandId.ToString(), now, string.Empty,
            JsonSerializer.Serialize(new { code = brand.Code, name = brand.Name, codeGenerated = data.Code is null }), branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: brand.RowVersion));
        try { await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
        catch (DbUpdateException exception) when (IsMasterDataCodeUniqueViolation(exception))
        {
            return Result<ItemBrandDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", "The code already exists in this organization."));
        }

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

        var imageValidation = await _fileStore.ValidateVerifiedFilesForParentAsync(
            orgId, access.ActorUserId, TanErp.Domain.Files.FileParentTypes.ItemBrand, brand.Id, null,
            data.ImageFileId.HasValue ? new[] { data.ImageFileId.Value } : Array.Empty<Guid>(), ct);
        if (imageValidation.IsFailure)
        {
            return Result<ItemBrandDetailProjection>.Failure(new Error("ITEM_IMAGE_NOT_READY", imageValidation.Error.Message ?? "File is not ready or verified."));
        }

        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);
        var desc = LocalizedText.CreateOptional(data.Description?.Thai, data.Description?.English);

        brand.Update(data.Code, name, desc, data.SortOrder, access.ActorUserId, now, data.ImageFileId);
        _db.AddAuditEvent(new AuditEvent(
            Guid.NewGuid(), orgId, access.ActorUserId, "item-brands.update", "item-brand", brand.Id.ToString(), now,
            string.Empty, JsonSerializer.Serialize(new { code = brand.Code, name = brand.Name, imageFileId = brand.ImageFileId }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionBefore: data.ExpectedRowVersion, rowVersionAfter: brand.RowVersion));
        await _db.SaveChangesAsync(ct);

        return Result<ItemBrandDetailProjection>.Success(MapBrand(brand));
    }

    // Tax Category Implementation
    public async Task<IReadOnlyList<ItemTaxCategoryDetailProjection>> ListTaxCategoriesAsync(Guid organizationId, CancellationToken ct)
    {
        var categories = await _db.ItemTaxCategories.AsNoTracking()
            .Where(category => category.OrganizationId == organizationId)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.NormalizedCode)
            .ToListAsync(ct);
        return categories.Select(MapTaxCategory).ToList();
    }

    public async Task<ItemTaxCategoryDetailProjection?> GetTaxCategoryAsync(Guid organizationId, Guid taxCategoryId, CancellationToken ct)
    {
        var category = await _db.ItemTaxCategories.AsNoTracking()
            .FirstOrDefaultAsync(row => row.Id == taxCategoryId && row.OrganizationId == organizationId, ct);
        return category is null ? null : MapTaxCategory(category);
    }

    public async Task<Result<ItemTaxCategoryDetailProjection>> CreateTaxCategoryAsync(
        CreateTaxCategoryData data,
        RequestAccessContext access,
        string idempotencyKey,
        CancellationToken ct)
    {
        var organizationId = access.OrganizationId;
        const string operation = "item-master.tax-category.create";
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(organizationId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<ItemTaxCategoryDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await GetTaxCategoryAsync(organizationId, replayId, ct);
            return replayed is null ? Result<ItemTaxCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created tax category no longer exists.")) : Result<ItemTaxCategoryDetailProjection>.Success(replayed);
        }

        var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
            _documentNumberGenerator, organizationId, DocumentTypes.ItemTaxCategories, data.Code, 30, "ITEM_CODE_CONFLICT",
            async (code, token) => await _db.ItemTaxCategories.AnyAsync(row => row.OrganizationId == organizationId && row.NormalizedCode == code.Trim().ToUpperInvariant(), token), ct);
        if (resolvedCode.IsFailure) return Result<ItemTaxCategoryDetailProjection>.Failure(resolvedCode.Error);

        var now = DateTimeOffset.UtcNow;
        var category = new ItemTaxCategory(Guid.NewGuid(), organizationId, resolvedCode.Value!, LocalizedText.Create(data.Name.Thai, data.Name.English), data.SortOrder, access.ActorUserId, now);
        _db.ItemTaxCategories.Add(category);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation, keyHash, payloadHash, category.Id.ToString(), now));
        _db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), organizationId, access.ActorUserId, "item-tax-categories.create", "item-tax-category", category.Id.ToString(), now, string.Empty,
            JsonSerializer.Serialize(new { code = category.Code, name = category.Name, codeGenerated = data.Code is null }), branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: category.RowVersion));
        try { await _db.SaveChangesAsync(ct); await transaction.CommitAsync(ct); }
        catch (DbUpdateException exception) when (IsMasterDataCodeUniqueViolation(exception))
        {
            return Result<ItemTaxCategoryDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", "The code already exists in this organization."));
        }
        return Result<ItemTaxCategoryDetailProjection>.Success(MapTaxCategory(category));
    }

    public async Task<Result<ItemTaxCategoryDetailProjection>> UpdateTaxCategoryAsync(UpdateTaxCategoryData data, RequestAccessContext access, CancellationToken ct)
    {
        var category = await _db.ItemTaxCategories.FirstOrDefaultAsync(row => row.Id == data.TaxCategoryId && row.OrganizationId == access.OrganizationId, ct);
        if (category is null) return Result<ItemTaxCategoryDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Tax category not found."));
        if (category.RowVersion != data.ExpectedRowVersion) return Result<ItemTaxCategoryDetailProjection>.Failure(new Error("ITEM_VERSION_CONFLICT", "Tax category has been modified by another user."));
        var normalizedCode = data.Code.Trim().ToUpperInvariant();
        if (await _db.ItemTaxCategories.AnyAsync(row => row.OrganizationId == access.OrganizationId && row.Id != category.Id && row.NormalizedCode == normalizedCode, ct))
            return Result<ItemTaxCategoryDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", $"Tax category code '{data.Code}' already exists."));

        category.Update(data.Code, LocalizedText.Create(data.Name.Thai, data.Name.English), data.SortOrder, access.ActorUserId, DateTimeOffset.UtcNow);
        await _db.SaveChangesAsync(ct);
        return Result<ItemTaxCategoryDetailProjection>.Success(MapTaxCategory(category));
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
        string idempotencyKey,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        const string operation = "item-master.unit.create";
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<UnitOfMeasureDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await GetUnitAsync(orgId, replayId, ct);
            return replayed is null ? Result<UnitOfMeasureDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created unit no longer exists.")) : Result<UnitOfMeasureDetailProjection>.Success(replayed);
        }
        var resolvedCode = await MasterDataCodeAllocator.ResolveAsync(
            _documentNumberGenerator, orgId, DocumentTypes.UnitsOfMeasure, data.Code, 20, "ITEM_CODE_CONFLICT",
            async (code, token) => await _db.Units.AnyAsync(u => u.OrganizationId == orgId && u.NormalizedCode == code.Trim().ToUpperInvariant(), token), ct);
        if (resolvedCode.IsFailure) return Result<UnitOfMeasureDetailProjection>.Failure(resolvedCode.Error);

        var unitId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var name = LocalizedText.Create(data.Name.Thai, data.Name.English);

        var unit = new UnitOfMeasure(
            unitId,
            orgId,
            resolvedCode.Value!,
            name,
            data.Symbol,
            data.Dimension,
            data.DecimalScale,
            data.RoundingMode,
            access.ActorUserId,
            now);

        _db.Units.Add(unit);
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, unitId.ToString(), now));
        _db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), orgId, access.ActorUserId, "units.create", "unit-of-measure", unitId.ToString(), now, string.Empty,
            JsonSerializer.Serialize(new { code = unit.Code, name = unit.Name, codeGenerated = data.Code is null }), branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: unit.RowVersion));
        try { await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct); }
        catch (DbUpdateException exception) when (IsMasterDataCodeUniqueViolation(exception))
        {
            return Result<UnitOfMeasureDetailProjection>.Failure(new Error("ITEM_CODE_CONFLICT", "The code already exists in this organization."));
        }

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
        var normEn = aliasText.English?.Trim().ToLowerInvariant();

        var exists = await _db.ItemAliases
            .AnyAsync(a => a.OrganizationId == organizationId && a.ItemId == itemId &&
                           (a.NormalizedTh == normTh || (normEn != null && a.NormalizedEn == normEn)) &&
                           a.Status == ItemStatus.Active, ct);
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

    public async Task<IReadOnlyList<ItemBarcodeProjection>> ListBarcodesAsync(Guid organizationId, Guid itemId, CancellationToken ct)
    {
        var barcodes = await _db.ItemBarcodes.AsNoTracking()
            .Include(b => b.Item)
            .Include(b => b.Unit)
            .Where(b => b.OrganizationId == organizationId && b.ItemId == itemId)
            .OrderBy(b => b.PackagingLevel).ThenByDescending(b => b.IsPrimary).ThenBy(b => b.Id)
            .Take(100)
            .ToListAsync(ct);
        return barcodes.Select(MapBarcode).ToList();
    }

    public async Task<IReadOnlyList<ItemUnitConversionProjection>> ListItemUnitConversionsAsync(Guid organizationId, Guid itemId, CancellationToken ct)
    {
        var rows = await (
            from conversion in _db.ItemUnitConversions.AsNoTracking()
            join item in _db.Items.AsNoTracking() on new { conversion.ItemId, conversion.OrganizationId } equals new { ItemId = item.Id, item.OrganizationId }
            join fromUnit in _db.Units.AsNoTracking() on new { UnitId = conversion.FromUnitId, conversion.OrganizationId } equals new { UnitId = fromUnit.Id, fromUnit.OrganizationId }
            join toUnit in _db.Units.AsNoTracking() on new { UnitId = conversion.ToUnitId, conversion.OrganizationId } equals new { UnitId = toUnit.Id, toUnit.OrganizationId }
            where conversion.OrganizationId == organizationId && conversion.ItemId == itemId
            orderby fromUnit.Code, conversion.EffectiveFrom descending
            select new { conversion, item, fromUnit, toUnit })
            .Take(200).ToListAsync(ct);
        return rows.Select(pair => new ItemUnitConversionProjection(pair.conversion.Id, pair.item.Id, pair.item.Code,
            pair.fromUnit.Id, pair.fromUnit.Code, pair.toUnit.Id, pair.toUnit.Code, pair.conversion.Factor,
            pair.conversion.EffectiveFrom, pair.conversion.EffectiveTo, pair.conversion.Reason,
            pair.conversion.Status, pair.conversion.RowVersion)).ToList();
    }

    public async Task<Result<ItemUnitConversionProjection>> CreateItemUnitConversionAsync(CreateItemUnitConversionData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct)
    {
        const string operation = "item-master.item-unit-conversion.create";
        var organizationId = access.OrganizationId;
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(organizationId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash)
                return Result<ItemUnitConversionProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var projection = replayId == Guid.Empty ? null : await LoadItemUnitConversionAsync(organizationId, data.ItemId, replayId, ct);
            return projection is null
                ? Result<ItemUnitConversionProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created conversion no longer exists."))
                : Result<ItemUnitConversionProjection>.Success(projection);
        }

        var itemLockKey = $"item-unit-conversion:{organizationId:N}:{data.ItemId:N}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({itemLockKey}, 0))", ct);
        var item = await _db.Items.AsNoTracking().Where(x => x.Id == data.ItemId && x.OrganizationId == organizationId)
            .Select(x => new { x.BaseUnitId }).FirstOrDefaultAsync(ct);
        if (item is null) return Result<ItemUnitConversionProjection>.Failure(new Error("ITEM_NOT_FOUND", "Item not found."));
        if (data.ToUnitId != item.BaseUnitId)
            return Result<ItemUnitConversionProjection>.Failure(new Error("ITEM_UNIT_CONVERSION_TARGET_INVALID", "Item conversion must target the item's base unit."));

        var fromUnit = await _db.Units.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.FromUnitId && x.OrganizationId == organizationId, ct);
        var toUnit = await _db.Units.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.ToUnitId && x.OrganizationId == organizationId, ct);
        if (fromUnit is null || toUnit is null || fromUnit.Status != ItemStatus.Active || toUnit.Status != ItemStatus.Active)
            return Result<ItemUnitConversionProjection>.Failure(new Error("ITEM_UNIT_CONVERSION_UNIT_INVALID", "Conversion units must be active units in the same organization."));
        if (!string.Equals(fromUnit.Dimension, toUnit.Dimension, StringComparison.Ordinal))
            return Result<ItemUnitConversionProjection>.Failure(new Error("ITEM_UNIT_CONVERSION_DIMENSION_MISMATCH", "Conversion units must have the same dimension."));

        ItemUnitConversion conversion;
        try
        {
            conversion = new ItemUnitConversion(Guid.NewGuid(), organizationId, data.ItemId, data.FromUnitId, data.ToUnitId,
                data.Factor, data.EffectiveFrom, data.EffectiveTo, data.Reason, access.ActorUserId, DateTimeOffset.UtcNow);
        }
        catch (ItemDomainException error)
        {
            return Result<ItemUnitConversionProjection>.Failure(new Error(error.Code, error.Message));
        }

        var overlap = await _db.ItemUnitConversions.AnyAsync(x => x.OrganizationId == organizationId && x.ItemId == data.ItemId
            && x.FromUnitId == data.FromUnitId && x.Status == ItemStatus.Active
            && (!data.EffectiveTo.HasValue || x.EffectiveFrom <= data.EffectiveTo.Value)
            && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= data.EffectiveFrom), ct);
        if (overlap)
            return Result<ItemUnitConversionProjection>.Failure(new Error("ITEM_UNIT_CONVERSION_PERIOD_OVERLAP", "An active conversion already covers part of this period."));

        var now = DateTimeOffset.UtcNow;
        _db.ItemUnitConversions.Add(conversion);
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), organizationId, access.ActorUserId, "item-unit-conversion.created", "item-unit-conversion", conversion.Id.ToString(), now,
            string.Empty, JsonSerializer.Serialize(new { conversion.ItemId, conversion.FromUnitId, conversion.ToUnitId, conversion.Factor, conversion.EffectiveFrom, conversion.EffectiveTo, conversion.Reason }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: conversion.RowVersion));
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation, keyHash, payloadHash, conversion.Id.ToString(), now));
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException error) when (error.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        {
            await tx.RollbackAsync(ct);
            return Result<ItemUnitConversionProjection>.Failure(new Error("ITEM_UNIT_CONVERSION_PERIOD_OVERLAP", "An active conversion already covers part of this period."));
        }
        await tx.CommitAsync(ct);
        return Result<ItemUnitConversionProjection>.Success((await LoadItemUnitConversionAsync(organizationId, data.ItemId, conversion.Id, ct))!);
    }

    public async Task<IReadOnlyList<UnitConversionProjection>> ListUnitConversionsAsync(Guid organizationId, CancellationToken ct)
    {
        var rows = await (
            from conversion in _db.UnitConversions.AsNoTracking()
            join fromUnit in _db.Units.AsNoTracking() on new { UnitId = conversion.FromUnitId, conversion.OrganizationId } equals new { UnitId = fromUnit.Id, fromUnit.OrganizationId }
            join toUnit in _db.Units.AsNoTracking() on new { UnitId = conversion.ToUnitId, conversion.OrganizationId } equals new { UnitId = toUnit.Id, toUnit.OrganizationId }
            where conversion.OrganizationId == organizationId
            orderby fromUnit.Code, toUnit.Code, conversion.EffectiveFrom descending
            select new { conversion, fromUnit, toUnit })
            .Take(500).ToListAsync(ct);
        return rows.Select(pair => new UnitConversionProjection(pair.conversion.Id, pair.fromUnit.Id, pair.fromUnit.Code,
            pair.toUnit.Id, pair.toUnit.Code, pair.conversion.Factor, pair.conversion.EffectiveFrom,
            pair.conversion.EffectiveTo, pair.conversion.Reason, pair.conversion.Status, pair.conversion.RowVersion)).ToList();
    }

    public async Task<Result<UnitConversionProjection>> CreateUnitConversionAsync(CreateUnitConversionData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct)
    {
        const string operation = "item-master.unit-conversion.create";
        var organizationId = access.OrganizationId;
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(organizationId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash)
                return Result<UnitConversionProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await LoadUnitConversionAsync(organizationId, replayId, ct);
            return replayed is null
                ? Result<UnitConversionProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created conversion no longer exists."))
                : Result<UnitConversionProjection>.Success(replayed);
        }

        var lockKey = $"unit-conversion:{organizationId:N}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        var units = await _db.Units.AsNoTracking().Where(x => x.OrganizationId == organizationId
            && (x.Id == data.FromUnitId || x.Id == data.ToUnitId)).ToListAsync(ct);
        var fromUnit = units.FirstOrDefault(x => x.Id == data.FromUnitId);
        var toUnit = units.FirstOrDefault(x => x.Id == data.ToUnitId);
        if (fromUnit is null || toUnit is null || fromUnit.Status != ItemStatus.Active || toUnit.Status != ItemStatus.Active)
            return Result<UnitConversionProjection>.Failure(new Error("UNIT_CONVERSION_UNIT_INVALID", "Conversion units must be active units in this organization."));
        if (!string.Equals(fromUnit.Dimension, toUnit.Dimension, StringComparison.Ordinal))
            return Result<UnitConversionProjection>.Failure(new Error("UNIT_CONVERSION_DIMENSION_MISMATCH", "Conversion units must have the same dimension."));

        UnitConversion conversion;
        try
        {
            conversion = new UnitConversion(Guid.NewGuid(), organizationId, data.FromUnitId, data.ToUnitId,
                data.Factor, data.EffectiveFrom, data.EffectiveTo, data.Reason, access.ActorUserId, DateTimeOffset.UtcNow);
        }
        catch (ItemDomainException error)
        {
            return Result<UnitConversionProjection>.Failure(new Error(error.Code, error.Message));
        }

        var activeEdges = await _db.UnitConversions.AsNoTracking()
            .Where(x => x.OrganizationId == organizationId && x.Status == ItemStatus.Active)
            .Select(x => new { x.FromUnitId, x.ToUnitId }).ToListAsync(ct);
        var edges = activeEdges.GroupBy(x => x.FromUnitId).ToDictionary(group => group.Key, group => group.Select(x => x.ToUnitId).ToArray());
        if (HasPath(edges, data.ToUnitId, data.FromUnitId))
            return Result<UnitConversionProjection>.Failure(new Error("UNIT_CONVERSION_CYCLE", "Conversion would create a cycle."));

        var overlaps = await _db.UnitConversions.AnyAsync(x => x.OrganizationId == organizationId
            && x.FromUnitId == data.FromUnitId && x.ToUnitId == data.ToUnitId && x.Status == ItemStatus.Active
            && (!data.EffectiveTo.HasValue || x.EffectiveFrom <= data.EffectiveTo.Value)
            && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= data.EffectiveFrom), ct);
        if (overlaps)
            return Result<UnitConversionProjection>.Failure(new Error("UNIT_CONVERSION_PERIOD_OVERLAP", "An active conversion already covers part of this period."));

        var now = DateTimeOffset.UtcNow;
        _db.UnitConversions.Add(conversion);
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), organizationId, access.ActorUserId, "unit-conversion.created", "unit-conversion", conversion.Id.ToString(), now,
            string.Empty, JsonSerializer.Serialize(new { conversion.FromUnitId, conversion.ToUnitId, conversion.Factor, conversion.EffectiveFrom, conversion.EffectiveTo, conversion.Reason }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: conversion.RowVersion));
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation, keyHash, payloadHash, conversion.Id.ToString(), now));
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException error) when (error.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        {
            await tx.RollbackAsync(ct);
            return Result<UnitConversionProjection>.Failure(new Error("UNIT_CONVERSION_PERIOD_OVERLAP", "An active conversion already covers part of this period."));
        }
        await tx.CommitAsync(ct);
        return Result<UnitConversionProjection>.Success((await LoadUnitConversionAsync(organizationId, conversion.Id, ct))!);
    }

    private static bool HasPath(IReadOnlyDictionary<Guid, Guid[]> edges, Guid start, Guid target)
    {
        var pending = new Stack<Guid>();
        var visited = new HashSet<Guid>();
        pending.Push(start);
        while (pending.TryPop(out var current))
        {
            if (current == target) return true;
            if (!visited.Add(current) || !edges.TryGetValue(current, out var next)) continue;
            foreach (var unitId in next) pending.Push(unitId);
        }
        return false;
    }

    private async Task<UnitConversionProjection?> LoadUnitConversionAsync(Guid organizationId, Guid id, CancellationToken ct)
    {
        return await _db.UnitConversions.AsNoTracking()
            .Where(x => x.OrganizationId == organizationId && x.Id == id)
            .Join(_db.Units.AsNoTracking(), conversion => new { UnitId = conversion.FromUnitId, conversion.OrganizationId }, unit => new { UnitId = unit.Id, unit.OrganizationId },
                (conversion, fromUnit) => new { conversion, fromUnit })
            .Join(_db.Units.AsNoTracking(), pair => new { UnitId = pair.conversion.ToUnitId, pair.conversion.OrganizationId }, unit => new { UnitId = unit.Id, unit.OrganizationId },
                (pair, toUnit) => new UnitConversionProjection(pair.conversion.Id, pair.fromUnit.Id, pair.fromUnit.Code,
                    toUnit.Id, toUnit.Code, pair.conversion.Factor, pair.conversion.EffectiveFrom, pair.conversion.EffectiveTo,
                    pair.conversion.Reason, pair.conversion.Status, pair.conversion.RowVersion)).FirstOrDefaultAsync(ct);
    }

    private async Task<ItemUnitConversionProjection?> LoadItemUnitConversionAsync(Guid organizationId, Guid itemId, Guid id, CancellationToken ct)
    {
        return await _db.ItemUnitConversions.AsNoTracking()
            .Where(x => x.OrganizationId == organizationId && x.ItemId == itemId && x.Id == id)
            .Join(_db.Items.AsNoTracking(), conversion => new { conversion.ItemId, conversion.OrganizationId }, item => new { ItemId = item.Id, item.OrganizationId },
                (conversion, item) => new { conversion, item })
            .Join(_db.Units.AsNoTracking(), pair => new { UnitId = pair.conversion.FromUnitId, pair.conversion.OrganizationId }, unit => new { UnitId = unit.Id, unit.OrganizationId },
                (pair, fromUnit) => new { pair.conversion, pair.item, fromUnit })
            .Join(_db.Units.AsNoTracking(), pair => new { UnitId = pair.conversion.ToUnitId, pair.conversion.OrganizationId }, unit => new { UnitId = unit.Id, unit.OrganizationId },
                (pair, toUnit) => new ItemUnitConversionProjection(pair.conversion.Id, pair.item.Id, pair.item.Code,
                    pair.fromUnit.Id, pair.fromUnit.Code, toUnit.Id, toUnit.Code, pair.conversion.Factor,
                    pair.conversion.EffectiveFrom, pair.conversion.EffectiveTo, pair.conversion.Reason,
                    pair.conversion.Status, pair.conversion.RowVersion)).FirstOrDefaultAsync(ct);
    }

    public async Task<Result<ItemBarcodeProjection>> CreateBarcodeAsync(CreateItemBarcodeData data, RequestAccessContext access, string idempotencyKey, CancellationToken ct)
    {
        const string operation = "item-master.item-barcode.create";
        var orgId = access.OrganizationId;
        var keyHash = Hash(idempotencyKey);
        var payloadHash = Hash(JsonSerializer.Serialize(data));
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var replay = await LockAndFindReplayAsync(orgId, operation, keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash)
                return Result<ItemBarcodeProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "Idempotency key was reused with a different payload."));
            var replayId = Guid.TryParse(replay.ResourceId, out var parsedId) ? parsedId : Guid.Empty;
            var replayed = replayId == Guid.Empty ? null : await LoadBarcodeAsync(orgId, data.ItemId, replayId, ct);
            return replayed is null
                ? Result<ItemBarcodeProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Created barcode no longer exists."))
                : Result<ItemBarcodeProjection>.Success(replayed);
        }

        var item = await _db.Items.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == data.ItemId && i.OrganizationId == orgId, ct);
        if (item is null) return Result<ItemBarcodeProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Item not found."));
        if (item.Status == ItemStatus.Inactive)
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_INACTIVE_LOCKED", "Inactive item cannot receive a barcode."));
        var unit = await _db.Units.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == data.UnitId && u.OrganizationId == orgId, ct);
        if (unit is null) return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_UNIT_INVALID", "Unit not found in organization."));
        if (unit.Status != ItemStatus.Active)
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_UNIT_INVALID", "Barcode unit must be active."));
        if (unit.Id != item.BaseUnitId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var conversion = await _db.ItemUnitConversions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganizationId == orgId && x.ItemId == data.ItemId
                    && x.FromUnitId == unit.Id && x.ToUnitId == item.BaseUnitId
                    && x.Status == ItemStatus.Active && x.EffectiveFrom <= today
                    && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= today), ct);
            var globalConversion = conversion is null
                ? await _db.UnitConversions.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == orgId
                    && x.FromUnitId == unit.Id && x.ToUnitId == item.BaseUnitId
                    && x.Status == ItemStatus.Active && x.EffectiveFrom <= today
                    && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= today), ct)
                : null;
            var conversionFactor = conversion?.Factor ?? globalConversion?.Factor;
            if (!conversionFactor.HasValue || conversionFactor.Value != data.QuantityInBaseUnit)
                return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_UNIT_CONVERSION_REQUIRED", "Barcode quantity must match an active item unit conversion."));
        }

        ItemBarcode barcode;
        try
        {
            barcode = new ItemBarcode(Guid.NewGuid(), orgId, data.ItemId, data.IdentifierType, data.Value,
                data.UnitId, data.QuantityInBaseUnit, data.PackagingLevel, data.IsPrimary, access.ActorUserId, DateTimeOffset.UtcNow);
        }
        catch (ItemDomainException error)
        {
            return Result<ItemBarcodeProjection>.Failure(new Error(error.Code, error.Message));
        }

        if (await _db.ItemBarcodes.AnyAsync(b => b.OrganizationId == orgId && b.NormalizedValue == barcode.NormalizedValue, ct))
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_CONFLICT", "Barcode already exists in organization."));

        var now = DateTimeOffset.UtcNow;
        if (barcode.IsPrimary)
        {
            var previous = await _db.ItemBarcodes
                .Where(b => b.OrganizationId == orgId && b.ItemId == data.ItemId && b.PackagingLevel == barcode.PackagingLevel && b.IsPrimary && b.Status == ItemStatus.Active)
                .ToListAsync(ct);
            foreach (var current in previous) current.ClearPrimary(access.ActorUserId, now);
            foreach (var current in previous)
            {
                _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), orgId, access.ActorUserId, "item-barcode.primary-cleared", "item-barcode", current.Id.ToString(), now,
                    string.Empty, JsonSerializer.Serialize(new { current.ItemId, current.PackagingLevel }),
                    branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: current.RowVersion));
            }
            if (previous.Count > 0)
            {
                try { await _db.SaveChangesAsync(ct); }
                catch (DbUpdateConcurrencyException)
                {
                    await tx.RollbackAsync(ct);
                    return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode primary status changed concurrently."));
                }
            }
        }

        _db.ItemBarcodes.Add(barcode);
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), orgId, access.ActorUserId, "item-barcode.created", "item-barcode", barcode.Id.ToString(), now,
            string.Empty, JsonSerializer.Serialize(new { barcode.ItemId, barcode.IdentifierType, barcode.Value, barcode.UnitId, barcode.QuantityInBaseUnit, barcode.PackagingLevel }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId, requestId: null, rowVersionAfter: barcode.RowVersion));
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, barcode.Id.ToString(), now));
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException error) when (error.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        {
            await tx.RollbackAsync(ct);
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_CONFLICT", "Barcode already exists in organization."));
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode changed concurrently."));
        }
        await tx.CommitAsync(ct);

        var created = await LoadBarcodeAsync(orgId, data.ItemId, barcode.Id, ct);
        return Result<ItemBarcodeProjection>.Success(created!);
    }

    public async Task<Result<ItemBarcodeProjection>> SetBarcodePrimaryAsync(Guid organizationId, Guid itemId, Guid barcodeId, Guid expectedRowVersion, RequestAccessContext access, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var barcode = await _db.ItemBarcodes.FirstOrDefaultAsync(b => b.Id == barcodeId && b.ItemId == itemId && b.OrganizationId == organizationId, ct);
        if (barcode is null) return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_NOT_FOUND", "Barcode not found."));
        if (barcode.RowVersion != expectedRowVersion)
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode version changed."));
        if (barcode.Status != ItemStatus.Active)
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_INACTIVE", "Inactive barcode cannot be primary."));
        var now = DateTimeOffset.UtcNow;
        var previous = await _db.ItemBarcodes
            .Where(b => b.OrganizationId == organizationId && b.ItemId == itemId && b.PackagingLevel == barcode.PackagingLevel && b.Id != barcodeId && b.IsPrimary && b.Status == ItemStatus.Active)
            .ToListAsync(ct);
        foreach (var current in previous) current.ClearPrimary(access.ActorUserId, now);
        foreach (var current in previous)
        {
            _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), organizationId, access.ActorUserId, "item-barcode.primary-cleared", "item-barcode", current.Id.ToString(), now,
                string.Empty, JsonSerializer.Serialize(new { current.ItemId, current.PackagingLevel }),
                branchId: access.BranchId, actorMembershipId: access.MembershipId, rowVersionAfter: current.RowVersion));
        }
        if (previous.Count > 0) await _db.SaveChangesAsync(ct);
        barcode.SetPrimary(access.ActorUserId, now);
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), organizationId, access.ActorUserId, "item-barcode.primary-changed", "item-barcode", barcode.Id.ToString(), now,
            string.Empty, JsonSerializer.Serialize(new { barcode.ItemId, barcode.PackagingLevel }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId, requestId: null, rowVersionAfter: barcode.RowVersion));
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException error) when (error.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        {
            await tx.RollbackAsync(ct);
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode primary status changed concurrently."));
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode primary status changed concurrently."));
        }
        await tx.CommitAsync(ct);
        return Result<ItemBarcodeProjection>.Success((await LoadBarcodeAsync(organizationId, itemId, barcodeId, ct))!);
    }

    public async Task<Result<ItemBarcodeProjection>> DeactivateBarcodeAsync(Guid organizationId, Guid itemId, Guid barcodeId, Guid expectedRowVersion, RequestAccessContext access, CancellationToken ct)
    {
        var barcode = await _db.ItemBarcodes.FirstOrDefaultAsync(b => b.Id == barcodeId && b.ItemId == itemId && b.OrganizationId == organizationId, ct);
        if (barcode is null) return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_NOT_FOUND", "Barcode not found."));
        if (barcode.RowVersion != expectedRowVersion)
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode version changed."));
        try { barcode.Deactivate(access.ActorUserId, DateTimeOffset.UtcNow); }
        catch (ItemDomainException error) { return Result<ItemBarcodeProjection>.Failure(new Error(error.Code, error.Message)); }
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), organizationId, access.ActorUserId, "item-barcode.deactivated", "item-barcode", barcode.Id.ToString(), DateTimeOffset.UtcNow,
            string.Empty, JsonSerializer.Serialize(new { barcode.ItemId, barcode.Value }),
            branchId: access.BranchId, actorMembershipId: access.MembershipId, requestId: null, rowVersionAfter: barcode.RowVersion));
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException)
        {
            return Result<ItemBarcodeProjection>.Failure(new Error("ITEM_BARCODE_VERSION_CONFLICT", "Barcode changed concurrently."));
        }
        return Result<ItemBarcodeProjection>.Success((await LoadBarcodeAsync(organizationId, itemId, barcodeId, ct))!);
    }

    public async Task<ItemBarcodeProjection?> FindActiveBarcodeAsync(Guid organizationId, Guid? branchId, string value, CancellationToken ct)
    {
        var normalized = ItemBarcode.NormalizeForLookup(value);
        var barcode = await _db.ItemBarcodes.AsNoTracking().Include(b => b.Item).Include(b => b.Unit)
            .FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.NormalizedValue == normalized && b.Status == ItemStatus.Active && b.Item!.Status == ItemStatus.Active &&
                (b.Item.AvailabilityMode == ItemAvailabilityMode.AllBranches || (branchId.HasValue && b.Item.BranchAvailabilities.Any(availability => availability.BranchId == branchId.Value && availability.Status == ItemStatus.Active))), ct);
        return barcode is null ? null : MapBarcode(barcode);
    }

    private async Task<ItemBarcodeProjection?> LoadBarcodeAsync(Guid organizationId, Guid itemId, Guid barcodeId, CancellationToken ct)
    {
        var barcode = await _db.ItemBarcodes.AsNoTracking().Include(b => b.Item).Include(b => b.Unit)
            .FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.ItemId == itemId && b.Id == barcodeId, ct);
        return barcode is null ? null : MapBarcode(barcode);
    }

    private static ItemBarcodeProjection MapBarcode(ItemBarcode barcode) => new(
        barcode.Id, barcode.ItemId, barcode.Item!.Code,
        new LocalizedTextDto(barcode.Item.Name.Thai, barcode.Item.Name.English), barcode.Item.Status,
        barcode.IdentifierType, barcode.Value, barcode.UnitId, barcode.Unit!.Code,
        new LocalizedTextDto(barcode.Unit.Name.Thai, barcode.Unit.Name.English), barcode.Unit.Symbol,
        barcode.QuantityInBaseUnit, barcode.PackagingLevel, barcode.IsPrimary, barcode.Status, barcode.RowVersion);

    private async Task<ItemDetailProjection> MapToDetailProjectionAsync(Item item, CancellationToken ct)
    {
        CategorySummaryDto categoryDto;
        if (item.Category != null)
        {
            categoryDto = new CategorySummaryDto(
                item.Category.Id,
                item.Category.Code,
                new LocalizedTextDto(item.Category.Name.Thai, item.Category.Name.English),
                item.Category.ParentCategoryId,
                item.Category.ImageFileId);
        }
        else
        {
            var cat = await _db.ItemCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == item.CategoryId, ct);
            categoryDto = new CategorySummaryDto(
                cat!.Id,
                cat.Code,
                new LocalizedTextDto(cat.Name.Thai, cat.Name.English),
                cat.ParentCategoryId,
                cat.ImageFileId);
        }

        BrandSummaryDto? brandDto = null;
        if (item.Brand != null)
        {
            brandDto = new BrandSummaryDto(
                item.Brand.Id,
                item.Brand.Code,
                new LocalizedTextDto(item.Brand.Name.Thai, item.Brand.Name.English),
                item.Brand.ImageFileId);
        }
        else if (item.BrandId.HasValue)
        {
            var b = await _db.ItemBrands.AsNoTracking().FirstOrDefaultAsync(br => br.Id == item.BrandId.Value, ct);
            if (b != null)
            {
                brandDto = new BrandSummaryDto(
                    b.Id,
                    b.Code,
                    new LocalizedTextDto(b.Name.Thai, b.Name.English),
                    b.ImageFileId);
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
                parentSummary = new CategorySummaryDto(p.Id, p.Code, new LocalizedTextDto(p.Name.Thai, p.Name.English), p.ParentCategoryId, p.ImageFileId);
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
            category.ImageFileId,
            category.RowVersion,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }

    private static ItemCategoryDetailProjection MapCategory(ItemCategory category, ItemCategory? parent)
    {
        CategorySummaryDto? parentSummary = null;
        if (parent != null)
        {
            parentSummary = new CategorySummaryDto(parent.Id, parent.Code, new LocalizedTextDto(parent.Name.Thai, parent.Name.English), parent.ParentCategoryId, parent.ImageFileId);
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
            category.ImageFileId,
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
            brand.ImageFileId,
            brand.RowVersion,
            brand.CreatedAtUtc,
            brand.UpdatedAtUtc);
    }

    private static ItemTaxCategoryDetailProjection MapTaxCategory(ItemTaxCategory category) => new(
        category.Id,
        category.OrganizationId,
        category.Code,
        new LocalizedTextDto(category.Name.Thai, category.Name.English),
        category.SortOrder,
        category.Status,
        category.RowVersion,
        category.CreatedAtUtc,
        category.UpdatedAtUtc);

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

    private async Task<IdempotencyRecord?> LockAndFindReplayAsync(Guid organizationId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{organizationId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(record =>
            record.OrganizationId == organizationId && record.Operation == operation && record.KeyHash == keyHash, ct);
    }

    private static bool IsMasterDataCodeUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException postgresException &&
        postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
        postgresException.ConstraintName is
            "IX_items_organization_id_normalized_code" or
            "IX_item_categories_organization_id_normalized_code" or
            "IX_item_brands_organization_id_normalized_code" or
            "IX_item_tax_categories_organization_id_normalized_code" or
            "IX_units_organization_id_normalized_code";

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
