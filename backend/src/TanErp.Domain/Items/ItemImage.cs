using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class ItemImage : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid FileId { get; private set; }
    public string Role { get; private set; } = ItemImageRole.Primary;
    public bool IsPrimary { get; private set; }
    public int DisplayOrder { get; private set; }
    public LocalizedText AltText { get; private set; } = null!;
    public LocalizedText? Caption { get; private set; }
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    public Item? Item { get; private set; }

    protected ItemImage() : base() { }

    public ItemImage(
        Guid id,
        Guid organizationId,
        Guid itemId,
        Guid fileId,
        string role,
        bool isPrimary,
        int displayOrder,
        LocalizedText altText,
        LocalizedText? caption,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (!ItemImageRole.All.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            throw new ItemValidationException("ITEM_IMAGE_ROLE_INVALID", $"Image role '{role}' is invalid.");
        }

        OrganizationId = organizationId;
        ItemId = itemId;
        FileId = fileId;
        Role = role.ToLowerInvariant();
        IsPrimary = isPrimary;
        DisplayOrder = Math.Max(0, displayOrder);
        AltText = altText;
        Caption = caption;
        Status = ItemStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = createdAtUtc;
        UpdatedByUserId = createdByUserId;
    }

    public void UpdateMetadata(
        string role,
        LocalizedText altText,
        LocalizedText? caption,
        int displayOrder,
        Guid updatedByUserId,
        DateTimeOffset updatedAtUtc)
    {
        if (!ItemImageRole.All.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            throw new ItemValidationException("ITEM_IMAGE_ROLE_INVALID", $"Image role '{role}' is invalid.");
        }

        Role = role.ToLowerInvariant();
        AltText = altText;
        Caption = caption;
        DisplayOrder = Math.Max(0, displayOrder);
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public void SetPrimary(bool isPrimary, Guid updatedByUserId, DateTimeOffset updatedAtUtc)
    {
        IsPrimary = isPrimary;
        if (isPrimary)
        {
            Role = ItemImageRole.Primary;
        }
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public void Deactivate(Guid updatedByUserId, DateTimeOffset updatedAtUtc)
    {
        Status = ItemStatus.Inactive;
        IsPrimary = false;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }
}
