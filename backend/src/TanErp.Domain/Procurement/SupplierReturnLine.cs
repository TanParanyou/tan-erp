using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class SupplierReturnLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SupplierReturnId { get; private set; }
    public Guid GoodsReceiptLineId { get; private set; }
    public Guid PurchaseOrderLineId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UnitId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineReturnAmount { get; private set; }

    protected SupplierReturnLine() { }

    public SupplierReturnLine(
        Guid id,
        Guid organizationId,
        Guid supplierReturnId,
        Guid goodsReceiptLineId,
        Guid purchaseOrderLineId,
        Guid itemId,
        Guid unitId,
        decimal quantity,
        decimal unitPrice) : base(id)
    {
        if (organizationId == Guid.Empty || supplierReturnId == Guid.Empty || goodsReceiptLineId == Guid.Empty || purchaseOrderLineId == Guid.Empty || itemId == Guid.Empty || unitId == Guid.Empty)
        {
            throw new ArgumentException("All identifiers are required on a supplier return line.");
        }

        if (quantity <= 0 || quantity > 999_999_999m)
        {
            throw new ProcurementDomainException("SUPPLIER_RETURN_INVALID", "Returned quantity must be greater than zero.");
        }

        if (unitPrice < 0 || unitPrice > 999_999_999m)
        {
            throw new ProcurementDomainException("SUPPLIER_RETURN_INVALID", "Unit price cannot be negative.");
        }

        OrganizationId = organizationId;
        SupplierReturnId = supplierReturnId;
        GoodsReceiptLineId = goodsReceiptLineId;
        PurchaseOrderLineId = purchaseOrderLineId;
        ItemId = itemId;
        UnitId = unitId;
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitPrice = decimal.Round(unitPrice, 4, MidpointRounding.AwayFromZero);
        LineReturnAmount = decimal.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);
    }
}
