using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

/// <summary>An immutable record of goods received against an approved purchase order. This is the hand-off contract to Inventory.</summary>
public class GoodsReceipt : Entity
{
    private readonly List<GoodsReceiptLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateTimeOffset ReceivedAtUtc { get; private set; }
    public string? Note { get; private set; }
    public Guid ReceivedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<GoodsReceiptLine> Lines => _lines.AsReadOnly();

    protected GoodsReceipt() { }

    public GoodsReceipt(
        Guid id, Guid organizationId, Guid branchId, Guid purchaseOrderId, Guid supplierId, Guid? projectId, string number,
        DateTimeOffset receivedAtUtc, string? note, Guid receivedByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || purchaseOrderId == Guid.Empty || supplierId == Guid.Empty)
        {
            throw new ArgumentException("Organization, branch, purchase order and supplier are required.");
        }

        if (receivedByUserId == Guid.Empty) throw new ArgumentException("Received by user ID cannot be empty.", nameof(receivedByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Receipt number cannot be blank.", nameof(number));

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > 500 })
        {
            throw new ProcurementDomainException("GOODS_RECEIPT_INVALID", "Note cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        PurchaseOrderId = purchaseOrderId;
        SupplierId = supplierId;
        ProjectId = projectId;
        Number = number.Trim();
        ReceivedAtUtc = receivedAtUtc.ToUniversalTime();
        Note = trimmedNote;
        ReceivedByUserId = receivedByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    public void AddLine(GoodsReceiptLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
    }
}

public class GoodsReceiptLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid GoodsReceiptId { get; private set; }
    public Guid PurchaseOrderLineId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UnitId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    protected GoodsReceiptLine() { }

    public GoodsReceiptLine(Guid id, Guid organizationId, Guid goodsReceiptId, Guid purchaseOrderLineId, Guid itemId, Guid unitId, decimal quantity, decimal unitPrice) : base(id)
    {
        if (quantity <= 0) throw new ProcurementDomainException("GOODS_RECEIPT_INVALID", "Received quantity must be greater than zero.");
        OrganizationId = organizationId;
        GoodsReceiptId = goodsReceiptId;
        PurchaseOrderLineId = purchaseOrderLineId;
        ItemId = itemId;
        UnitId = unitId;
        Quantity = decimal.Round(quantity, 4);
        UnitPrice = unitPrice;
    }
}
