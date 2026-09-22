using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class ItemBranchAvailability : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Status { get; private set; } = ItemStatus.Active;
    public DateTimeOffset? EffectiveFromUtc { get; private set; }
    public DateTimeOffset? EffectiveToUtc { get; private set; }
    public string? InactiveReason { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid UpdatedByUserId { get; private set; }

    public Item? Item { get; private set; }

    protected ItemBranchAvailability() : base() { }

    public ItemBranchAvailability(
        Guid id,
        Guid organizationId,
        Guid itemId,
        Guid branchId,
        DateTimeOffset? effectiveFromUtc,
        DateTimeOffset? effectiveToUtc,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (effectiveFromUtc.HasValue && effectiveToUtc.HasValue && effectiveToUtc.Value <= effectiveFromUtc.Value)
        {
            throw new ItemValidationException("ITEM_PERIOD_INVALID", "Effective to date must be greater than effective from date.");
        }

        OrganizationId = organizationId;
        ItemId = itemId;
        BranchId = branchId;
        Status = ItemStatus.Active;
        EffectiveFromUtc = effectiveFromUtc;
        EffectiveToUtc = effectiveToUtc;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = createdAtUtc;
        UpdatedByUserId = createdByUserId;
    }

    public void Deactivate(string? reason, Guid updatedByUserId, DateTimeOffset updatedAtUtc)
    {
        Status = ItemStatus.Inactive;
        InactiveReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public void Reactivate(DateTimeOffset? effectiveFromUtc, DateTimeOffset? effectiveToUtc, Guid updatedByUserId, DateTimeOffset updatedAtUtc)
    {
        if (effectiveFromUtc.HasValue && effectiveToUtc.HasValue && effectiveToUtc.Value <= effectiveFromUtc.Value)
        {
            throw new ItemValidationException("ITEM_PERIOD_INVALID", "Effective to date must be greater than effective from date.");
        }

        Status = ItemStatus.Active;
        EffectiveFromUtc = effectiveFromUtc;
        EffectiveToUtc = effectiveToUtc;
        InactiveReason = null;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }
}
