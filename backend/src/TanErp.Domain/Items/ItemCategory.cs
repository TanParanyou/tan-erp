using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class ItemCategory : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public LocalizedText Name { get; private set; } = null!;
    public LocalizedText? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public string[] AllowedItemTypes { get; private set; } = [];
    public int SortOrder { get; private set; }
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    public ItemCategory? ParentCategory { get; private set; }
    public List<ItemCategory> SubCategories { get; private set; } = [];

    protected ItemCategory() : base() { }

    public ItemCategory(
        Guid id,
        Guid organizationId,
        string code,
        LocalizedText name,
        LocalizedText? description,
        Guid? parentCategoryId,
        string[] allowedItemTypes,
        int sortOrder,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Category code is required.");
        }

        if (parentCategoryId.HasValue && parentCategoryId.Value == id)
        {
            throw new ItemValidationException("ITEM_CATEGORY_CYCLE", "Category cannot be its own parent.");
        }

        OrganizationId = organizationId;
        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        AllowedItemTypes = allowedItemTypes.Length == 0 ? ItemType.All : allowedItemTypes;
        SortOrder = Math.Max(0, sortOrder);
        Status = ItemStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = createdAtUtc;
        UpdatedByUserId = createdByUserId;
    }

    public void Update(
        string code,
        LocalizedText name,
        LocalizedText? description,
        Guid? parentCategoryId,
        string[] allowedItemTypes,
        int sortOrder,
        Guid updatedByUserId,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Category code is required.");
        }

        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
        {
            throw new ItemValidationException("ITEM_CATEGORY_CYCLE", "Category cannot be its own parent.");
        }

        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        AllowedItemTypes = allowedItemTypes.Length == 0 ? ItemType.All : allowedItemTypes;
        SortOrder = Math.Max(0, sortOrder);
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public void SetParent(Guid? parentId)
    {
        if (parentId.HasValue && parentId.Value == Id)
        {
            throw new ItemValidationException("ITEM_CATEGORY_CYCLE", "Category cannot be its own parent.");
        }

        ParentCategoryId = parentId;
        RowVersion = Guid.NewGuid();
    }
}
