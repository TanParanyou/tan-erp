using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public sealed class ItemUnitConversion : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid FromUnitId { get; private set; }
    public Guid ToUnitId { get; private set; }
    public decimal Factor { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string Status { get; private set; } = ItemStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private ItemUnitConversion() : base() { }

    public ItemUnitConversion(Guid id, Guid organizationId, Guid itemId, Guid fromUnitId, Guid toUnitId,
        decimal factor, DateOnly effectiveFrom, DateOnly? effectiveTo, string reason,
        Guid createdByUserId, DateTimeOffset createdAtUtc) : base(id)
    {
        if (factor <= 0 || factor >= 1_000_000_000_000m || decimal.Round(factor, 6) != factor)
            throw new ItemValidationException("ITEM_UNIT_CONVERSION_FACTOR_INVALID", "Conversion factor must be greater than zero.");
        if (fromUnitId == toUnitId)
            throw new ItemValidationException("ITEM_UNIT_CONVERSION_SELF_LOOP", "A unit cannot convert to itself.");
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            throw new ItemValidationException("ITEM_UNIT_CONVERSION_PERIOD_INVALID", "Conversion end date cannot be before its start date.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ItemValidationException("ITEM_UNIT_CONVERSION_REASON_REQUIRED", "Conversion reason is required.");

        OrganizationId = organizationId;
        ItemId = itemId;
        FromUnitId = fromUnitId;
        ToUnitId = toUnitId;
        Factor = factor;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Reason = reason.Trim();
        Status = ItemStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
    }

    public bool IsEffectiveOn(DateOnly date) => Status == ItemStatus.Active
        && EffectiveFrom <= date
        && (!EffectiveTo.HasValue || EffectiveTo.Value >= date);
}
