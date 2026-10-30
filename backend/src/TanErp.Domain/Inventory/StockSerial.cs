using TanErp.Domain.Common;

namespace TanErp.Domain.Inventory;

public class StockSerial : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ItemId { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public string NormalizedSerialNumber { get; private set; } = string.Empty;
    public Guid? WarehouseId { get; private set; }
    public Guid? LocationId { get; private set; }
    public string LotNumber { get; private set; } = string.Empty;
    public string Status { get; private set; } = StockSerialStatus.InStock;
    public Guid LastStockDocumentId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected StockSerial() { }

    public StockSerial(
        Guid id,
        Guid organizationId,
        Guid itemId,
        string serialNumber,
        Guid warehouseId,
        Guid locationId,
        string? lotNumber,
        Guid stockDocumentId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || itemId == Guid.Empty || warehouseId == Guid.Empty || locationId == Guid.Empty || stockDocumentId == Guid.Empty)
        {
            throw new ArgumentException("Organization, item, warehouse, location and stock document are required.");
        }

        var (trimmed, normalized) = NormalizeSerial(serialNumber);

        OrganizationId = organizationId;
        ItemId = itemId;
        SerialNumber = trimmed;
        NormalizedSerialNumber = normalized;
        WarehouseId = warehouseId;
        LocationId = locationId;
        LotNumber = StockLocationBalance.NormalizeLot(lotNumber);
        Status = StockSerialStatus.InStock;
        LastStockDocumentId = stockDocumentId;
        UpdatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public static (string Trimmed, string Normalized) NormalizeSerial(string? serialNumber)
    {
        var trimmed = (serialNumber ?? string.Empty).Trim();
        if (trimmed.Length is 0 or > 100)
        {
            throw new InventoryDomainException("INVENTORY_FIELD_INVALID", "Serial number is required and cannot exceed 100 characters.");
        }

        return (trimmed, trimmed.ToUpperInvariant());
    }

    public void IssueOut(Guid stockDocumentId, DateTimeOffset now)
    {
        if (Status != StockSerialStatus.InStock)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{SerialNumber}' is not in stock.");
        }

        Status = StockSerialStatus.Issued;
        WarehouseId = null;
        LocationId = null;
        LastStockDocumentId = stockDocumentId;
        Touch(now);
    }

    public void ReturnToSupplier(Guid stockDocumentId, DateTimeOffset now)
    {
        if (Status != StockSerialStatus.InStock)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{SerialNumber}' is not in stock.");
        }

        Status = StockSerialStatus.ReturnedOut;
        WarehouseId = null;
        LocationId = null;
        LastStockDocumentId = stockDocumentId;
        Touch(now);
    }

    public void TransferTo(Guid toWarehouseId, Guid toLocationId, Guid stockDocumentId, DateTimeOffset now)
    {
        if (Status != StockSerialStatus.InStock)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{SerialNumber}' is not in stock.");
        }

        WarehouseId = toWarehouseId;
        LocationId = toLocationId;
        LastStockDocumentId = stockDocumentId;
        Touch(now);
    }

    public void ReturnFromProject(Guid warehouseId, Guid locationId, string? lotNumber, Guid stockDocumentId, DateTimeOffset now)
    {
        if (Status != StockSerialStatus.Issued)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{SerialNumber}' cannot be returned because it is not currently issued.");
        }

        Status = StockSerialStatus.InStock;
        WarehouseId = warehouseId;
        LocationId = locationId;
        if (!string.IsNullOrWhiteSpace(lotNumber))
        {
            LotNumber = StockLocationBalance.NormalizeLot(lotNumber);
        }

        LastStockDocumentId = stockDocumentId;
        Touch(now);
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
