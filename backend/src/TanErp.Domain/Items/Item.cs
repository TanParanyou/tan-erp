using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class Item : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public string ItemType { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Guid? BrandId { get; private set; }
    public LocalizedText Name { get; private set; } = null!;
    public LocalizedText? Description { get; private set; }
    public Guid BaseUnitId { get; private set; }
    public string? TaxCategoryCode { get; private set; }
    public string AvailabilityMode { get; private set; } = ItemAvailabilityMode.AllBranches;
    public ItemCapabilities Capabilities { get; private set; } = null!;
    public Dictionary<string, string>? Attributes { get; private set; }
    public int? AttributesSchemaVersion { get; private set; }
    public string Status { get; private set; } = ItemStatus.Draft;
    public bool ActivatedOnce { get; private set; }
    public DateTimeOffset? ActivatedAtUtc { get; private set; }
    public Guid? ActivatedByUserId { get; private set; }
    public DateTimeOffset? InactiveAtUtc { get; private set; }
    public Guid? InactiveByUserId { get; private set; }
    public string? InactiveReasonCode { get; private set; }
    public string? InactiveReason { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    // Navigation properties
    public ItemCategory? Category { get; private set; }
    public ItemBrand? Brand { get; private set; }
    public UnitOfMeasure? BaseUnit { get; private set; }
    public List<ItemAlias> Aliases { get; private set; } = [];
    public List<ItemBranchAvailability> BranchAvailabilities { get; private set; } = [];
    public List<ItemImage> Images { get; private set; } = [];

    // EF constructor
    protected Item() : base() { }

    private Item(
        Guid id,
        Guid organizationId,
        string code,
        string itemType,
        Guid categoryId,
        Guid? brandId,
        LocalizedText name,
        LocalizedText? description,
        Guid baseUnitId,
        string availabilityMode,
        ItemCapabilities capabilities,
        Dictionary<string, string>? attributes,
        int? attributesSchemaVersion,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        OrganizationId = organizationId;
        Code = code;
        NormalizedCode = NormalizeCode(code);
        ItemType = itemType;
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Description = description;
        BaseUnitId = baseUnitId;
        AvailabilityMode = availabilityMode;
        Capabilities = capabilities;
        Attributes = attributes;
        AttributesSchemaVersion = attributesSchemaVersion;
        Status = ItemStatus.Draft;
        ActivatedOnce = false;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = createdAtUtc;
        UpdatedByUserId = createdByUserId;
    }

    public static Item CreateDraft(
        Guid id,
        Guid organizationId,
        string code,
        string itemType,
        Guid categoryId,
        Guid? brandId,
        LocalizedText name,
        LocalizedText? description,
        Guid baseUnitId,
        string availabilityMode,
        ItemCapabilities capabilities,
        Dictionary<string, string>? attributes,
        int? attributesSchemaVersion,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Item code is required.");
        }

        if (!Items.ItemType.IsValid(itemType))
        {
            throw new ItemValidationException("ITEM_TYPE_INVALID", $"Item type '{itemType}' is invalid.");
        }

        if (!ItemAvailabilityMode.IsValid(availabilityMode))
        {
            throw new ItemValidationException("ITEM_AVAILABILITY_MODE_INVALID", $"Availability mode '{availabilityMode}' is invalid.");
        }

        ValidateAttributes(attributes);

        return new Item(
            id,
            organizationId,
            code.Trim(),
            itemType.ToLowerInvariant(),
            categoryId,
            brandId,
            name,
            description,
            baseUnitId,
            availabilityMode.ToLowerInvariant(),
            capabilities,
            attributes,
            attributesSchemaVersion,
            createdByUserId,
            createdAtUtc);
    }

    public void UpdateDetails(
        string code,
        string itemType,
        Guid categoryId,
        Guid? brandId,
        LocalizedText name,
        LocalizedText? description,
        Guid baseUnitId,
        string availabilityMode,
        ItemCapabilities capabilities,
        Dictionary<string, string>? attributes,
        int? attributesSchemaVersion,
        Guid updatedByUserId,
        DateTimeOffset updatedAtUtc)
    {
        if (Status == ItemStatus.Inactive)
        {
            throw new ItemValidationException("ITEM_INACTIVE_LOCKED", "Item is inactive and cannot be modified.");
        }

        var normalizedNewCode = NormalizeCode(code);
        if (ActivatedOnce && normalizedNewCode != NormalizedCode)
        {
            throw new ItemValidationException("ITEM_CODE_IMMUTABLE", "Item code cannot be changed once the item has been activated.");
        }

        if (!Items.ItemType.IsValid(itemType))
        {
            throw new ItemValidationException("ITEM_TYPE_INVALID", $"Item type '{itemType}' is invalid.");
        }

        if (!ItemAvailabilityMode.IsValid(availabilityMode))
        {
            throw new ItemValidationException("ITEM_AVAILABILITY_MODE_INVALID", $"Availability mode '{availabilityMode}' is invalid.");
        }

        ValidateAttributes(attributes);

        Code = code.Trim();
        NormalizedCode = normalizedNewCode;
        ItemType = itemType.ToLowerInvariant();
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Description = description;
        BaseUnitId = baseUnitId;
        AvailabilityMode = availabilityMode.ToLowerInvariant();
        Capabilities = capabilities;
        Attributes = attributes;
        AttributesSchemaVersion = attributesSchemaVersion;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public void Activate(Guid actorUserId, DateTimeOffset now, bool hasActiveSelectedBranch)
    {
        if (string.IsNullOrWhiteSpace(Name.Thai))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Thai name is required for activation.");
        }

        if (CategoryId == Guid.Empty)
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Category is required for activation.");
        }

        if (BaseUnitId == Guid.Empty)
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Base unit is required for activation.");
        }

        if (!Capabilities.HasAnyActive())
        {
            throw new ItemValidationException("ITEM_CAPABILITY_REQUIRED", "At least one capability must be enabled for activation.");
        }

        if (AvailabilityMode == ItemAvailabilityMode.SelectedBranches && !hasActiveSelectedBranch)
        {
            throw new ItemValidationException("ITEM_BRANCH_REQUIRED", "At least one active selected branch is required when availability mode is selected_branches.");
        }

        Status = ItemStatus.Active;
        ActivatedOnce = true;
        ActivatedAtUtc = now;
        ActivatedByUserId = actorUserId;
        InactiveAtUtc = null;
        InactiveByUserId = null;
        InactiveReasonCode = null;
        InactiveReason = null;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    public void Deactivate(Guid actorUserId, DateTimeOffset now, string reasonCode, string? reason)
    {
        if (string.IsNullOrWhiteSpace(reasonCode))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Deactivation reason code is required.");
        }

        Status = ItemStatus.Inactive;
        InactiveAtUtc = now;
        InactiveByUserId = actorUserId;
        InactiveReasonCode = reasonCode.Trim();
        InactiveReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static void ValidateAttributes(Dictionary<string, string>? attributes)
    {
        if (attributes is null) return;

        string[] reservedKeys = ["price", "status", "permission", "currency", "unitcost", "cost"];
        foreach (var key in attributes.Keys)
        {
            if (reservedKeys.Contains(key.Trim().ToLowerInvariant()))
            {
                throw new ItemValidationException("ITEM_ATTRIBUTE_RESERVED", $"Attribute key '{key}' is reserved.");
            }
        }
    }
}
