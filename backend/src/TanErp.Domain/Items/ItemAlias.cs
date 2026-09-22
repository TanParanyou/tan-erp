using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class ItemAlias : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public LocalizedText Alias { get; private set; } = null!;
    public string NormalizedTh { get; private set; } = string.Empty;
    public string? NormalizedEn { get; private set; }
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    public Item? Item { get; private set; }

    protected ItemAlias() : base() { }

    public ItemAlias(
        Guid id,
        Guid organizationId,
        Guid itemId,
        LocalizedText alias,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        OrganizationId = organizationId;
        ItemId = itemId;
        Alias = alias;
        NormalizedTh = alias.Thai.Trim().ToLowerInvariant();
        NormalizedEn = alias.English?.Trim().ToLowerInvariant();
        Status = ItemStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = createdAtUtc;
        UpdatedByUserId = createdByUserId;
    }

    public void Update(LocalizedText alias, Guid updatedByUserId, DateTimeOffset updatedAtUtc)
    {
        Alias = alias;
        NormalizedTh = alias.Thai.Trim().ToLowerInvariant();
        NormalizedEn = alias.English?.Trim().ToLowerInvariant();
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public void Deactivate(Guid updatedByUserId, DateTimeOffset updatedAtUtc)
    {
        Status = ItemStatus.Inactive;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }
}
