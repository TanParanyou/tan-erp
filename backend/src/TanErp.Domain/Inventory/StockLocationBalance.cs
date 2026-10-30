using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

public class StockLocationBalance : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid LocationId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UnitId { get; private set; }
    public string LotNumber { get; private set; } = string.Empty;
    public decimal OnHand { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected StockLocationBalance() { }

    public StockLocationBalance(
        Guid id,
        Guid organizationId,
        Guid warehouseId,
        Guid locationId,
        Guid itemId,
        Guid unitId,
        string? lotNumber,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || warehouseId == Guid.Empty || locationId == Guid.Empty || itemId == Guid.Empty || unitId == Guid.Empty)
        {
            throw new ArgumentException("Organization, warehouse, location, item and unit are required.");
        }

        OrganizationId = organizationId;
        WarehouseId = warehouseId;
        LocationId = locationId;
        ItemId = itemId;
        UnitId = unitId;
        LotNumber = NormalizeLot(lotNumber);
        OnHand = 0m;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    public static string NormalizeLot(string? lotNumber)
    {
        if (string.IsNullOrWhiteSpace(lotNumber))
        {
            return string.Empty;
        }

        var normalized = lotNumber.Trim().ToUpperInvariant();
        if (normalized.Length > 64)
        {
            throw new InventoryDomainException("INVENTORY_FIELD_INVALID", "Lot number cannot exceed 64 characters.");
        }

        return normalized;
    }

    public void Receive(decimal quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
        {
            throw new InventoryDomainException("INVENTORY_QUANTITY_INVALID", "Quantity must be greater than zero.");
        }

        OnHand += decimal.Round(quantity, 4);
        Touch(now);
    }

    public void Issue(decimal quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
        {
            throw new InventoryDomainException("INVENTORY_QUANTITY_INVALID", "Quantity must be greater than zero.");
        }

        var rounded = decimal.Round(quantity, 4);
        if (rounded > OnHand)
        {
            throw new InventoryDomainException("INVENTORY_INSUFFICIENT_STOCK", "Not enough available stock in the specified location and lot.");
        }

        OnHand -= rounded;
        Touch(now);
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
