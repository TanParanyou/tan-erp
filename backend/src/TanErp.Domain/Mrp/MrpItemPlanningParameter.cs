using TanErp.Domain.Common;

namespace TanErp.Domain.Mrp;

public class MrpItemPlanningParameter : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public decimal SafetyStock { get; private set; }
    public decimal MinOrderQuantity { get; private set; }
    public decimal LotMultiple { get; private set; }
    public int? LeadTimeDays { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected MrpItemPlanningParameter() { }

    public MrpItemPlanningParameter(
        Guid id,
        Guid organizationId,
        Guid itemId,
        Guid? warehouseId,
        decimal safetyStock,
        decimal minOrderQuantity,
        decimal lotMultiple,
        int? leadTimeDays,
        Guid actorUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || itemId == Guid.Empty || actorUserId == Guid.Empty)
        {
            throw new ArgumentException("Organization, item and actor are required.");
        }

        if (warehouseId.HasValue && warehouseId.Value == Guid.Empty)
        {
            warehouseId = null;
        }

        OrganizationId = organizationId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        Apply(safetyStock, minOrderQuantity, lotMultiple, leadTimeDays, actorUserId, now);
    }

    public void Update(
        decimal safetyStock,
        decimal minOrderQuantity,
        decimal lotMultiple,
        int? leadTimeDays,
        Guid actorUserId,
        DateTimeOffset now)
    {
        if (actorUserId == Guid.Empty)
        {
            throw new ArgumentException("Actor is required.", nameof(actorUserId));
        }

        Apply(safetyStock, minOrderQuantity, lotMultiple, leadTimeDays, actorUserId, now);
    }

    private void Apply(
        decimal safetyStock,
        decimal minOrderQuantity,
        decimal lotMultiple,
        int? leadTimeDays,
        Guid actorUserId,
        DateTimeOffset now)
    {
        if (safetyStock < 0m || safetyStock > 999_999m
            || minOrderQuantity < 0m || minOrderQuantity > 999_999m
            || lotMultiple < 0m || lotMultiple > 999_999m
            || (leadTimeDays.HasValue && (leadTimeDays.Value < 0 || leadTimeDays.Value > 365)))
        {
            throw new MrpDomainException(
                "MRP_PLANNING_PARAMETER_INVALID",
                "Safety stock, minimum order quantity, and lot multiple must be non-negative, and lead time days must be between 0 and 365.");
        }

        SafetyStock = decimal.Round(safetyStock, 4);
        MinOrderQuantity = decimal.Round(minOrderQuantity, 4);
        LotMultiple = decimal.Round(lotMultiple, 4);
        LeadTimeDays = leadTimeDays;
        UpdatedByUserId = actorUserId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }
}
