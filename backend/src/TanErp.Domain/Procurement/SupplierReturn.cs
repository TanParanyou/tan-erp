using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

/// <summary>An immutable record of goods returned to a supplier against a specific goods receipt.</summary>
public class SupplierReturn : Entity
{
    private readonly List<SupplierReturnLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public Guid GoodsReceiptId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public DateTimeOffset ReturnedAtUtc { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public decimal TotalReturnAmount { get; private set; }
    public Guid? StockDocumentId { get; private set; }
    public string? StockDocumentNumber { get; private set; }
    public Guid ReturnedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<SupplierReturnLine> Lines => _lines.AsReadOnly();

    protected SupplierReturn() { }

    public SupplierReturn(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid purchaseOrderId,
        Guid goodsReceiptId,
        Guid supplierId,
        Guid? projectId,
        string number,
        DateTimeOffset returnedAtUtc,
        string? reason,
        Guid returnedByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || purchaseOrderId == Guid.Empty || goodsReceiptId == Guid.Empty || supplierId == Guid.Empty)
        {
            throw new ArgumentException("Organization, branch, purchase order, goods receipt, and supplier are required.");
        }

        if (returnedByUserId == Guid.Empty) throw new ArgumentException("Returned by user ID cannot be empty.", nameof(returnedByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Supplier return number cannot be blank.", nameof(number));

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", "A reason is required to record a supplier return.");
        }

        var trimmedReason = reason.Trim();
        if (trimmedReason.Length > 500)
        {
            throw new ProcurementDomainException("SUPPLIER_RETURN_INVALID", "Reason cannot exceed 500 characters.");
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        PurchaseOrderId = purchaseOrderId;
        GoodsReceiptId = goodsReceiptId;
        SupplierId = supplierId;
        ProjectId = projectId;
        Number = number.Trim();
        ReturnedAtUtc = returnedAtUtc.ToUniversalTime();
        Reason = trimmedReason;
        ReturnedByUserId = returnedByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    public void AddLine(SupplierReturnLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
        TotalReturnAmount = decimal.Round(_lines.Sum(l => l.LineReturnAmount), 2, MidpointRounding.AwayFromZero);
    }

    public void SetStockDocument(Guid stockDocumentId, string stockDocumentNumber)
    {
        if (stockDocumentId == Guid.Empty) throw new ArgumentException("Stock document ID cannot be empty.", nameof(stockDocumentId));
        if (string.IsNullOrWhiteSpace(stockDocumentNumber)) throw new ArgumentException("Stock document number cannot be blank.", nameof(stockDocumentNumber));
        StockDocumentId = stockDocumentId;
        StockDocumentNumber = stockDocumentNumber.Trim();
    }
}
