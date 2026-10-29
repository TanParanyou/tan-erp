using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class SupplierInvoiceLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SupplierInvoiceId { get; private set; }
    public int LineNo { get; private set; }
    public Guid PurchaseOrderLineId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemNameTh { get; private set; } = string.Empty;
    public Guid UnitId { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal PoUnitPrice { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal { get; private set; }
    public decimal PriceVarianceAmount { get; private set; }
    public decimal PriceVariancePercent { get; private set; }

    public decimal PoLineTotal => decimal.Round(Quantity * PoUnitPrice, 2, MidpointRounding.AwayFromZero);

    public bool ExceedsTolerance =>
        PoUnitPrice == 0m
            ? UnitPrice > 0m
            : Math.Abs(UnitPrice - PoUnitPrice) > PoUnitPrice * SupplierInvoice.PriceVarianceToleranceRatio;

    protected SupplierInvoiceLine() { }

    public SupplierInvoiceLine(
        Guid id,
        Guid organizationId,
        Guid supplierInvoiceId,
        int lineNo,
        Guid purchaseOrderLineId,
        Guid itemId,
        string itemCode,
        string itemNameTh,
        Guid unitId,
        string unitCode,
        decimal quantity,
        decimal poUnitPrice,
        decimal unitPrice) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (supplierInvoiceId == Guid.Empty) throw new ArgumentException("Supplier invoice ID cannot be empty.", nameof(supplierInvoiceId));
        if (purchaseOrderLineId == Guid.Empty || itemId == Guid.Empty || unitId == Guid.Empty)
        {
            throw new ArgumentException("Purchase order line, item, and unit are required.");
        }

        if (quantity <= 0 || quantity > 999_999_999m)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "Quantity must be greater than zero.");
        }

        if (unitPrice < 0 || unitPrice > 999_999_999m)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "Unit price cannot be negative.");
        }

        if (poUnitPrice < 0 || poUnitPrice > 999_999_999m)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "PO unit price cannot be negative.");
        }

        OrganizationId = organizationId;
        SupplierInvoiceId = supplierInvoiceId;
        LineNo = lineNo;
        PurchaseOrderLineId = purchaseOrderLineId;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemNameTh = itemNameTh;
        UnitId = unitId;
        UnitCode = unitCode;
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        UnitPrice = decimal.Round(unitPrice, 4, MidpointRounding.AwayFromZero);
        LineTotal = decimal.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);
        EvaluateAgainstPoPrice(poUnitPrice);
    }

    public void EvaluateAgainstPoPrice(decimal poUnitPrice)
    {
        PoUnitPrice = decimal.Round(poUnitPrice, 4, MidpointRounding.AwayFromZero);
        var poLineTotal = decimal.Round(Quantity * PoUnitPrice, 2, MidpointRounding.AwayFromZero);
        PriceVarianceAmount = decimal.Round(LineTotal - poLineTotal, 2, MidpointRounding.AwayFromZero);
        PriceVariancePercent = PoUnitPrice == 0m
            ? (UnitPrice > 0m ? 100m : 0m)
            : decimal.Round((UnitPrice - PoUnitPrice) / PoUnitPrice * 100m, 4, MidpointRounding.AwayFromZero);
    }
}
