using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Application.Files;
using TanErp.Domain.Common;
using TanErp.Domain.Files;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public class ItemImageStore : IItemImageStore
{
    private const int MaxImagesPerItem = 10;
    private readonly AppDbContext _db;
    private readonly IFileStore _fileStore;

    public ItemImageStore(AppDbContext db, IFileStore fileStore)
    {
        _db = db;
        _fileStore = fileStore;
    }

    public async Task<Result<ItemImageDetailProjection>> AttachImageAsync(
        AttachItemImageData data,
        RequestAccessContext access,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var itemId = data.ItemId;

        var itemExists = await _db.Items
            .AnyAsync(i => i.Id == itemId && i.OrganizationId == orgId, ct);
        if (!itemExists)
        {
            return Result<ItemImageDetailProjection>.Failure(new Error("ITEM_NOT_FOUND", "Item not found in organization."));
        }

        // Validate verified file in files schema belonging to this parent item
        var validation = await _fileStore.ValidateVerifiedFilesForParentAsync(
            orgId,
            access.ActorUserId,
            FileParentTypes.Item,
            itemId,
            null,
            new[] { data.FileId },
            ct);

        if (validation.IsFailure)
        {
            return Result<ItemImageDetailProjection>.Failure(
                new Error("ITEM_IMAGE_NOT_READY", validation.Error.Message ?? "File is not ready or verified."));
        }

        var file = await _db.UploadedFiles
            .FirstOrDefaultAsync(f => f.Id == data.FileId && f.OrganizationId == orgId, ct);
        if (file == null)
        {
            return Result<ItemImageDetailProjection>.Failure(new Error("FILE_NOT_FOUND", "Uploaded file not found in organization."));
        }

        // Check already attached
        var alreadyAttached = await _db.ItemImages
            .AnyAsync(im => im.OrganizationId == orgId && im.ItemId == itemId && im.FileId == data.FileId && im.Status == ItemStatus.Active, ct);
        if (alreadyAttached)
        {
            return Result<ItemImageDetailProjection>.Failure(new Error("ITEM_IMAGE_ALREADY_ATTACHED", "File is already attached to this item."));
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var activeImages = await _db.ItemImages
            .Where(im => im.OrganizationId == orgId && im.ItemId == itemId && im.Status == ItemStatus.Active)
            .ToListAsync(ct);

        if (activeImages.Count >= MaxImagesPerItem)
        {
            await tx.RollbackAsync(ct);
            return Result<ItemImageDetailProjection>.Failure(new Error("ITEM_IMAGE_LIMIT_EXCEEDED", $"Maximum {MaxImagesPerItem} active images allowed per item."));
        }

        var shouldBePrimary = data.IsPrimary || activeImages.Count == 0;
        var now = DateTimeOffset.UtcNow;

        if (shouldBePrimary)
        {
            foreach (var existing in activeImages.Where(im => im.IsPrimary))
            {
                existing.SetPrimary(false, access.ActorUserId, now);
            }
        }

        var maxOrder = activeImages.Count > 0 ? activeImages.Max(im => im.DisplayOrder) : -1;
        var displayOrder = maxOrder + 1;

        var altText = LocalizedText.Create(data.AltText.Thai, data.AltText.English);
        var caption = LocalizedText.CreateOptional(data.Caption?.Thai, data.Caption?.English);

        ItemImage image;
        try
        {
            image = new ItemImage(
                Guid.NewGuid(),
                orgId,
                itemId,
                data.FileId,
                shouldBePrimary ? ItemImageRole.Primary : data.Role,
                shouldBePrimary,
                displayOrder,
                altText,
                caption,
                access.ActorUserId,
                now);
        }
        catch (ItemDomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<ItemImageDetailProjection>.Failure(new Error(ex.Code, ex.Message));
        }

        _db.ItemImages.Add(image);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            orgId,
            access.ActorUserId,
            "items.attach-image",
            "item_image",
            image.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { itemId, fileId = data.FileId, isPrimary = shouldBePrimary }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: image.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<ItemImageDetailProjection>.Success(MapToProjection(image, file));
    }

    public async Task<IReadOnlyList<ItemImageDetailProjection>> ListImagesAsync(
        Guid organizationId,
        Guid itemId,
        CancellationToken ct)
    {
        var images = await _db.ItemImages
            .AsNoTracking()
            .Where(im => im.OrganizationId == organizationId && im.ItemId == itemId && im.Status == ItemStatus.Active)
            .OrderBy(im => im.DisplayOrder)
            .ThenBy(im => im.Id)
            .ToListAsync(ct);

        if (images.Count == 0)
        {
            return Array.Empty<ItemImageDetailProjection>();
        }

        var fileIds = images.Select(im => im.FileId).ToList();
        var files = await _db.UploadedFiles
            .AsNoTracking()
            .Where(f => fileIds.Contains(f.Id) && f.OrganizationId == organizationId)
            .ToDictionaryAsync(f => f.Id, ct);

        return images
            .Where(im => files.ContainsKey(im.FileId))
            .Select(im => MapToProjection(im, files[im.FileId]))
            .ToList();
    }

    public async Task<Result<ItemImageDetailProjection>> SetPrimaryImageAsync(
        Guid organizationId,
        Guid itemId,
        Guid imageId,
        RequestAccessContext access,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var images = await _db.ItemImages
            .Where(im => im.OrganizationId == organizationId && im.ItemId == itemId && im.Status == ItemStatus.Active)
            .ToListAsync(ct);

        var target = images.FirstOrDefault(im => im.Id == imageId);
        if (target == null)
        {
            await tx.RollbackAsync(ct);
            return Result<ItemImageDetailProjection>.Failure(new Error("ITEM_IMAGE_NOT_FOUND", "Item image not found."));
        }

        var now = DateTimeOffset.UtcNow;
        var existingPrimaries = images.Where(im => im.IsPrimary && im.Id != imageId).ToList();
        if (existingPrimaries.Count > 0)
        {
            foreach (var img in existingPrimaries)
            {
                img.SetPrimary(false, access.ActorUserId, now);
            }
            await _db.SaveChangesAsync(ct);
        }

        target.SetPrimary(true, access.ActorUserId, now);

        var file = await _db.UploadedFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == target.FileId && f.OrganizationId == organizationId, ct);

        var audit = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            access.ActorUserId,
            "items.set-primary-image",
            "item_image",
            target.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { itemId, imageId }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: target.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<ItemImageDetailProjection>.Success(MapToProjection(target, file!));
    }

    public async Task<Result<IReadOnlyList<ItemImageDetailProjection>>> ReorderImagesAsync(
        Guid organizationId,
        Guid itemId,
        IReadOnlyList<Guid> orderedImageIds,
        RequestAccessContext access,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var images = await _db.ItemImages
            .Where(im => im.OrganizationId == organizationId && im.ItemId == itemId && im.Status == ItemStatus.Active)
            .ToListAsync(ct);

        var now = DateTimeOffset.UtcNow;
        for (int i = 0; i < orderedImageIds.Count; i++)
        {
            var img = images.FirstOrDefault(m => m.Id == orderedImageIds[i]);
            if (img != null)
            {
                img.UpdateMetadata(img.Role, img.AltText, img.Caption, i, access.ActorUserId, now);
            }
        }

        var audit = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            access.ActorUserId,
            "items.reorder-images",
            "item",
            itemId.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { orderedImageIds }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var fileIds = images.Select(im => im.FileId).ToList();
        var files = await _db.UploadedFiles
            .AsNoTracking()
            .Where(f => fileIds.Contains(f.Id) && f.OrganizationId == organizationId)
            .ToDictionaryAsync(f => f.Id, ct);

        var result = images
            .OrderBy(im => im.DisplayOrder)
            .Select(im => MapToProjection(im, files.GetValueOrDefault(im.FileId)!))
            .ToList();

        return Result<IReadOnlyList<ItemImageDetailProjection>>.Success(result);
    }

    public async Task<Result<bool>> DetachImageAsync(
        Guid organizationId,
        Guid itemId,
        Guid imageId,
        RequestAccessContext access,
        CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var images = await _db.ItemImages
            .Where(im => im.OrganizationId == organizationId && im.ItemId == itemId && im.Status == ItemStatus.Active)
            .ToListAsync(ct);

        var target = images.FirstOrDefault(im => im.Id == imageId);
        if (target == null)
        {
            await tx.RollbackAsync(ct);
            return Result<bool>.Failure(new Error("ITEM_IMAGE_NOT_FOUND", "Item image not found."));
        }

        var now = DateTimeOffset.UtcNow;
        var wasPrimary = target.IsPrimary;
        target.Deactivate(access.ActorUserId, now);
        await _db.SaveChangesAsync(ct);

        // If it was primary, promote remaining image with lowest display order if any
        if (wasPrimary)
        {
            var nextPrimary = images
                .Where(im => im.Id != imageId && im.Status == ItemStatus.Active)
                .OrderBy(im => im.DisplayOrder)
                .FirstOrDefault();

            if (nextPrimary != null)
            {
                nextPrimary.SetPrimary(true, access.ActorUserId, now);
            }
        }

        var audit = new AuditEvent(
            Guid.NewGuid(),
            organizationId,
            access.ActorUserId,
            "items.detach-image",
            "item_image",
            target.Id.ToString(),
            now,
            string.Empty,
            JsonSerializer.Serialize(new { itemId, imageId }),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            requestId: null,
            rowVersionAfter: target.RowVersion);

        _db.AuditEvents.Add(audit);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Result<bool>.Success(true);
    }

    private static ItemImageDetailProjection MapToProjection(ItemImage image, UploadedFile file)
    {
        return new ItemImageDetailProjection(
            image.Id,
            image.OrganizationId,
            image.ItemId,
            image.FileId,
            image.Role,
            image.IsPrimary,
            image.DisplayOrder,
            new LocalizedTextDto(image.AltText.Thai, image.AltText.English),
            image.Caption != null ? new LocalizedTextDto(image.Caption.Thai, image.Caption.English) : null,
            image.Status,
            image.RowVersion,
            image.CreatedAtUtc,
            image.CreatedByUserId,
            file?.OriginalFilename ?? string.Empty,
            file?.MediaType ?? "image/jpeg",
            file?.FileSizeBytes ?? 0,
            file?.Width,
            file?.Height);
    }
}
