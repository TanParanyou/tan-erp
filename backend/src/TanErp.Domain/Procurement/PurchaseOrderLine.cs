using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class PurchaseOrderLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public int LineNo { get; private set; }
    public Guid ItemId { get; private set; }
    // Snapshots keep the order readable if item master data changes later.
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemNameTh { get; private set; } = string.Empty;
    public Guid UnitId { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal { get; private set; }
    public decimal ReceivedQuantity { get; private set; }

    protected PurchaseOrderLine() { }

    public PurchaseOrderLine(
        Guid id, Guid organizationId, Guid purchaseOrderId, int lineNo, Guid itemId, string itemCode, string itemNameTh,
        Guid unitId, string unitCode, decimal quantity, decimal unitPrice) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (purchaseOrderId == Guid.Empty) throw new ArgumentException("Purchase order ID cannot be empty.", nameof(purchaseOrderId));
        if (itemId == Guid.Empty || unitId == Guid.Empty) throw new ArgumentException("Item and unit are required.");
        if (quantity <= 0 || quantity > 999_999_999m)
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_LINE_INVALID", "Quantity must be greater than zero.");
        }

        if (unitPrice < 0 || unitPrice > 999_999_999m)
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_LINE_INVALID", "Unit price cannot be negative.");
        }

        OrganizationId = organizationId;
        PurchaseOrderId = purchaseOrderId;
        LineNo = lineNo;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemNameTh = itemNameTh;
        UnitId = unitId;
        UnitCode = unitCode;
        Quantity = decimal.Round(quantity, 4);
        UnitPrice = decimal.Round(unitPrice, 4);
        LineTotal = decimal.Round(Quantity * UnitPrice, 2);
    }

    public decimal RemainingQuantity => Quantity - ReceivedQuantity;

    /// <summary>No over-receipt: a receipt may never take the received quantity above the ordered quantity.</summary>
    public void Receive(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ProcurementDomainException("GOODS_RECEIPT_INVALID", "Received quantity must be greater than zero.");
        }

        var rounded = decimal.Round(quantity, 4);
        if (ReceivedQuantity + rounded > Quantity)
        {
            throw new ProcurementDomainException("GOODS_RECEIPT_OVER_RECEIVED", $"Line {LineNo} would be received above the ordered quantity.");
        }

        ReceivedQuantity += rounded;
    }
}
