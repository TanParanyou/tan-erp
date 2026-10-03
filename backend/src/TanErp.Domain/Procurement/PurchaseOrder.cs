using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class PurchaseOrder : Entity
{
    private readonly List<PurchaseOrderLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Status { get; private set; } = PurchaseOrderStatus.Draft;
    public string Currency { get; private set; } = "THB";
    public decimal TotalAmount { get; private set; }
    public DateOnly? ExpectedDeliveryDate { get; private set; }
    public string? Note { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }
    public string? CancelReason { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<PurchaseOrderLine> Lines => _lines.AsReadOnly();

    protected PurchaseOrder() { }

    public PurchaseOrder(
        Guid id, Guid organizationId, Guid branchId, Guid supplierId, Guid? projectId, string number,
        DateOnly? expectedDeliveryDate, string? note, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (supplierId == Guid.Empty) throw new ArgumentException("Supplier ID cannot be empty.", nameof(supplierId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Purchase order number cannot be blank.", nameof(number));

        OrganizationId = organizationId;
        BranchId = branchId;
        SupplierId = supplierId;
        ProjectId = projectId;
        Number = number.Trim();
        Status = PurchaseOrderStatus.Draft;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Note = CleanNote(note);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public void AddLine(PurchaseOrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        _lines.Add(line);
        TotalAmount = decimal.Round(_lines.Sum(l => l.LineTotal), 2);
    }

    /// <summary>Replaces the header details; lines are replaced by the store. Only a draft can be edited.</summary>
    public void EditDraft(Guid? projectId, DateOnly? expectedDeliveryDate, string? note, DateTimeOffset now)
    {
        EnsureStatus(PurchaseOrderStatus.Draft, "Only a draft purchase order can be edited.");
        ProjectId = projectId;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Note = CleanNote(note);
        Touch(now);
    }

    public void ReplaceLines(IEnumerable<PurchaseOrderLine> lines)
    {
        EnsureStatus(PurchaseOrderStatus.Draft, "Only a draft purchase order can be edited.");
        _lines.Clear();
        foreach (var line in lines) _lines.Add(line);
        TotalAmount = decimal.Round(_lines.Sum(l => l.LineTotal), 2);
    }

    public void Submit(DateTimeOffset now)
    {
        EnsureStatus(PurchaseOrderStatus.Draft, $"Only a draft purchase order can be submitted; current status is '{Status}'.");
        if (_lines.Count == 0)
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_LINE_INVALID", "A purchase order needs at least one line.");
        }

        Status = PurchaseOrderStatus.Submitted;
        SubmittedAtUtc = now.ToUniversalTime();
        Touch(now);
    }

    /// <summary>Maker–checker: whoever created the order cannot decide it.</summary>
    public void Decide(bool approve, Guid deciderUserId, string? note, DateTimeOffset now)
    {
        EnsureStatus(PurchaseOrderStatus.Submitted, $"Only a submitted purchase order can be decided; current status is '{Status}'.");
        if (deciderUserId == CreatedByUserId)
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_SELF_APPROVAL", "A purchase order cannot be decided by its creator.");
        }

        if (!approve && string.IsNullOrWhiteSpace(note))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", "A note is required to reject a purchase order.");
        }

        Status = approve ? PurchaseOrderStatus.Approved : PurchaseOrderStatus.Rejected;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
        DecisionNote = CleanNote(note);
        Touch(now);
    }

    public void Cancel(string reason, DateTimeOffset now)
    {
        if (_lines.Any(l => l.ReceivedQuantity > 0))
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_HAS_RECEIPTS", "A purchase order with receipts cannot be cancelled.");
        }

        if (Status is not (PurchaseOrderStatus.Draft or PurchaseOrderStatus.Submitted or PurchaseOrderStatus.Approved))
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_INVALID_STATE", $"A {Status} purchase order cannot be cancelled.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", "A reason is required to cancel a purchase order.");
        }

        Status = PurchaseOrderStatus.Cancelled;
        CancelReason = reason.Trim();
        Touch(now);
    }

    /// <summary>Applies a receipt to the order lines and moves the status to partially_received / received.</summary>
    public void ApplyReceipt(IReadOnlyCollection<(Guid LineId, decimal Quantity)> quantities, DateTimeOffset now)
    {
        if (Status is not (PurchaseOrderStatus.Approved or PurchaseOrderStatus.PartiallyReceived))
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_INVALID_STATE", $"Goods can only be received against an approved order; current status is '{Status}'.");
        }

        foreach (var (lineId, quantity) in quantities)
        {
            var line = _lines.FirstOrDefault(l => l.Id == lineId)
                ?? throw new ProcurementDomainException("GOODS_RECEIPT_INVALID", "A receipt line does not belong to this purchase order.");
            line.Receive(quantity);
        }

        Status = _lines.All(l => l.ReceivedQuantity >= l.Quantity) ? PurchaseOrderStatus.Received : PurchaseOrderStatus.PartiallyReceived;
        Touch(now);
    }

    private void EnsureStatus(string expected, string message)
    {
        if (Status != expected)
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_INVALID_STATE", message);
        }
    }

    private static string? CleanNote(string? note)
    {
        var trimmed = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmed is { Length: > 500 })
        {
            throw new ProcurementDomainException("PURCHASE_ORDER_LINE_INVALID", "Note cannot exceed 500 characters.");
        }

        return trimmed;
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
