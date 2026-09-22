using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class ItemBrand : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public LocalizedText Name { get; private set; } = null!;
    public LocalizedText? Description { get; private set; }
    public int SortOrder { get; private set; }
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    protected ItemBrand() : base() { }

    public ItemBrand(
        Guid id,
        Guid organizationId,
        string code,
        LocalizedText name,
        LocalizedText? description,
        int sortOrder,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Brand code is required.");
        }

        OrganizationId = organizationId;
        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        Description = description;
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
        int sortOrder,
        Guid updatedByUserId,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Brand code is required.");
        }

        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        Description = description;
        SortOrder = Math.Max(0, sortOrder);
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }
}
