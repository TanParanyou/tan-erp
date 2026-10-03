using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

/// <summary>
/// Current stock of one item in one warehouse, derived from the movement ledger. Quantities never go negative
/// and the value follows a moving weighted average: receipts add cost, issues leave at the current average.
/// </summary>
public class StockBalance : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UnitId { get; private set; }
    public decimal OnHand { get; private set; }
    public decimal Reserved { get; private set; }
    public decimal TotalValue { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected StockBalance() { }

    public StockBalance(Guid id, Guid organizationId, Guid warehouseId, Guid itemId, Guid unitId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || warehouseId == Guid.Empty || itemId == Guid.Empty || unitId == Guid.Empty)
        {
            throw new ArgumentException("Organization, warehouse, item and unit are required.");
        }

        OrganizationId = organizationId;
        WarehouseId = warehouseId;
        ItemId = itemId;
        UnitId = unitId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    public decimal Available => OnHand - Reserved;

    public decimal AverageCost => OnHand > 0 ? decimal.Round(TotalValue / OnHand, 4) : 0m;

    /// <summary>Adds stock at the given unit cost; returns the value added.</summary>
    public decimal Receive(decimal quantity, decimal unitCost, DateTimeOffset now)
    {
        RequirePositive(quantity);
        if (unitCost < 0) throw new InventoryDomainException("INVENTORY_COST_INVALID", "Unit cost cannot be negative.");

        var value = decimal.Round(quantity * unitCost, 2);
        OnHand += quantity;
        TotalValue += value;
        Touch(now);
        return value;
    }

    /// <summary>Adds stock together with an exact value (used by transfers so no rounding value is created or lost).</summary>
    public void ReceiveAtValue(decimal quantity, decimal value, DateTimeOffset now)
    {
        RequirePositive(quantity);
        if (value < 0) throw new InventoryDomainException("INVENTORY_COST_INVALID", "Value cannot be negative.");

        OnHand += quantity;
        TotalValue += value;
        Touch(now);
    }

    /// <summary>
    /// Removes stock at the current average cost; returns the value removed. <paramref name="reservationAllowance"/> is the
    /// part of the reserved quantity the caller is entitled to consume (its own project reservation).
    /// </summary>
    public decimal Issue(decimal quantity, decimal reservationAllowance, DateTimeOffset now)
    {
        RequirePositive(quantity);
        if (quantity > OnHand || quantity > Available + reservationAllowance)
        {
            throw new InventoryDomainException("INVENTORY_INSUFFICIENT_STOCK", "Not enough available stock.");
        }

        // The last unit takes the remaining value so rounding never leaves residue in an empty bin.
        var value = quantity == OnHand ? TotalValue : decimal.Round(quantity * TotalValue / OnHand, 2);
        OnHand -= quantity;
        TotalValue -= value;
        Touch(now);
        return value;
    }

    public void Reserve(decimal quantity, DateTimeOffset now)
    {
        RequirePositive(quantity);
        if (quantity > Available)
        {
            throw new InventoryDomainException("INVENTORY_INSUFFICIENT_STOCK", "Not enough available stock to reserve.");
        }

        Reserved += quantity;
        Touch(now);
    }

    public void ReleaseReservation(decimal quantity, DateTimeOffset now)
    {
        RequirePositive(quantity);
        if (quantity > Reserved)
        {
            throw new InventoryDomainException("INVENTORY_RESERVATION_INVALID", "Cannot release more than is reserved.");
        }

        Reserved -= quantity;
        Touch(now);
    }

    private static void RequirePositive(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new InventoryDomainException("INVENTORY_QUANTITY_INVALID", "Quantity must be greater than zero.");
        }
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
