using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class ItemTaxCategory : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public LocalizedText Name { get; private set; } = null!;
    public string Status { get; private set; } = ItemStatus.Active;
    public int SortOrder { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    protected ItemTaxCategory() : base() { }

    public ItemTaxCategory(Guid id, Guid organizationId, string code, LocalizedText name, int sortOrder, Guid actorUserId, DateTimeOffset now) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Tax category code is required.");
        OrganizationId = organizationId;
        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        SortOrder = Math.Max(0, sortOrder);
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now;
        CreatedByUserId = actorUserId;
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }

    public void Update(string code, LocalizedText name, int sortOrder, Guid actorUserId, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ItemValidationException("ITEM_FIELD_REQUIRED", "Tax category code is required.");
        Code = code.Trim();
        NormalizedCode = code.Trim().ToUpperInvariant();
        Name = name;
        SortOrder = Math.Max(0, sortOrder);
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
        UpdatedByUserId = actorUserId;
    }
}
