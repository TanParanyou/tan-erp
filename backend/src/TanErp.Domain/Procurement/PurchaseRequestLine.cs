using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class PurchaseRequestLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid PurchaseRequestId { get; private set; }
    public int LineNo { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemNameTh { get; private set; } = string.Empty;
    public Guid UnitId { get; private set; }
    public string UnitCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal EstimatedUnitPrice { get; private set; }
    public decimal EstimatedLineTotal { get; private set; }
    public string? Note { get; private set; }

    protected PurchaseRequestLine() { }

    public PurchaseRequestLine(
        Guid id,
        Guid organizationId,
        Guid purchaseRequestId,
        int lineNo,
        Guid itemId,
        string itemCode,
        string itemNameTh,
        Guid unitId,
        string unitCode,
        decimal quantity,
        decimal estimatedUnitPrice,
        string? note = null) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (purchaseRequestId == Guid.Empty) throw new ArgumentException("Purchase request ID cannot be empty.", nameof(purchaseRequestId));
        if (itemId == Guid.Empty || unitId == Guid.Empty) throw new ArgumentException("Item and unit are required.");
        if (quantity <= 0 || quantity > 999_999_999m)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "Quantity must be greater than zero.");
        }

        if (estimatedUnitPrice < 0 || estimatedUnitPrice > 999_999_999m)
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "Estimated unit price cannot be negative.");
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > 500 })
        {
            throw new ProcurementDomainException("PURCHASE_REQUEST_LINE_INVALID", "Line note cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        PurchaseRequestId = purchaseRequestId;
        LineNo = lineNo;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemNameTh = itemNameTh;
        UnitId = unitId;
        UnitCode = unitCode;
        Quantity = decimal.Round(quantity, 4, MidpointRounding.AwayFromZero);
        EstimatedUnitPrice = decimal.Round(estimatedUnitPrice, 4, MidpointRounding.AwayFromZero);
        EstimatedLineTotal = decimal.Round(Quantity * EstimatedUnitPrice, 2, MidpointRounding.AwayFromZero);
        Note = trimmedNote;
    }
}
