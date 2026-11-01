using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

/// <summary>Supplier quoted price and lead time for one item, valid over a date range. TEST_ONLY rules pending Procurement/Finance sign-off.</summary>
public class SupplierItemPrice : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid ItemId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal MinOrderQuantity { get; private set; }
    public int LeadTimeDays { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsPreferred { get; private set; }
    public Guid RowVersion { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected SupplierItemPrice() { }

    public SupplierItemPrice(
        Guid id, Guid organizationId, Guid supplierId, Guid itemId, decimal unitPrice, decimal minOrderQuantity,
        int leadTimeDays, DateOnly effectiveFrom, DateOnly? effectiveTo, bool isPreferred, Guid actorUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || supplierId == Guid.Empty || itemId == Guid.Empty || actorUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization, supplier, item and actor are required.");
        }

        OrganizationId = organizationId;
        SupplierId = supplierId;
        ItemId = itemId;
        CreatedAtUtc = now.ToUniversalTime();
        Apply(unitPrice, minOrderQuantity, leadTimeDays, effectiveFrom, effectiveTo, isPreferred, actorUserId, now);
    }

    public void Update(
        decimal unitPrice, decimal minOrderQuantity, int leadTimeDays, DateOnly effectiveFrom, DateOnly? effectiveTo,
        bool isPreferred, Guid actorUserId, DateTimeOffset now)
    {
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor is required.", nameof(actorUserId));
        Apply(unitPrice, minOrderQuantity, leadTimeDays, effectiveFrom, effectiveTo, isPreferred, actorUserId, now);
    }

    public void ClearPreferred(Guid actorUserId, DateTimeOffset now)
    {
        IsPreferred = false;
        UpdatedByUserId = actorUserId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public bool Overlaps(DateOnly from, DateOnly? to) =>
        from <= (EffectiveTo ?? DateOnly.MaxValue) && EffectiveFrom <= (to ?? DateOnly.MaxValue);

    private void Apply(
        decimal unitPrice, decimal minOrderQuantity, int leadTimeDays, DateOnly effectiveFrom, DateOnly? effectiveTo,
        bool isPreferred, Guid actorUserId, DateTimeOffset now)
    {
        if (unitPrice < 0m || unitPrice > 999_999_999m
            || minOrderQuantity < 0m || minOrderQuantity > 999_999m
            || leadTimeDays < 0 || leadTimeDays > 365
            || (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom))
        {
            throw new ProcurementDomainException(
                "SUPPLIER_PRICE_INVALID",
                "Unit price and minimum order quantity must be non-negative, lead time must be 0-365 days, and the end date cannot precede the start date.");
        }

        UnitPrice = decimal.Round(unitPrice, 4);
        MinOrderQuantity = decimal.Round(minOrderQuantity, 4);
        LeadTimeDays = leadTimeDays;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        IsPreferred = isPreferred;
        UpdatedByUserId = actorUserId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }
}
