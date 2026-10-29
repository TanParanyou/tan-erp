using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

public class SupplierInvoice : Entity
{
    public const decimal PriceVarianceToleranceRatio = 0.02m;

    private readonly List<SupplierInvoiceLine> _lines = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string SupplierInvoiceNumber { get; private set; } = string.Empty;
    public string NormalizedSupplierInvoiceNumber { get; private set; } = string.Empty;
    public DateOnly InvoiceDate { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public string Currency { get; private set; } = "THB";
    public decimal SubtotalAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal TotalPoAmount { get; private set; }
    public decimal TotalVarianceAmount { get; private set; }
    public string Status { get; private set; } = SupplierInvoiceStatus.Draft;
    public string MatchStatus { get; private set; } = SupplierInvoiceMatchStatus.Pending;
    public string? Note { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public DateTimeOffset? PostedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }
    public string? CancelReason { get; private set; }
    public string? VoidReason { get; private set; }
    public Guid? VoidedByUserId { get; private set; }
    public DateTimeOffset? VoidedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<SupplierInvoiceLine> Lines => _lines.AsReadOnly();

    protected SupplierInvoice() { }

    public SupplierInvoice(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid purchaseOrderId,
        Guid supplierId,
        Guid? projectId,
        string number,
        string? supplierInvoiceNumber,
        DateOnly invoiceDate,
        DateOnly? dueDate,
        decimal taxAmount,
        string? note,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (purchaseOrderId == Guid.Empty) throw new ArgumentException("Purchase order ID cannot be empty.", nameof(purchaseOrderId));
        if (supplierId == Guid.Empty) throw new ArgumentException("Supplier ID cannot be empty.", nameof(supplierId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Supplier invoice number cannot be blank.", nameof(number));

        var cleanedSupplierInvNo = CleanSupplierInvoiceNumber(supplierInvoiceNumber);
        ValidateHeaderFields(invoiceDate, dueDate, taxAmount);

        OrganizationId = organizationId;
        BranchId = branchId;
        PurchaseOrderId = purchaseOrderId;
        SupplierId = supplierId;
        ProjectId = projectId;
        Number = number.Trim();
        SupplierInvoiceNumber = cleanedSupplierInvNo;
        NormalizedSupplierInvoiceNumber = cleanedSupplierInvNo.ToUpperInvariant();
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        TaxAmount = decimal.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
        TotalAmount = TaxAmount;
        Status = SupplierInvoiceStatus.Draft;
        MatchStatus = SupplierInvoiceMatchStatus.Pending;
        Note = CleanNote(note);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public void AddLine(SupplierInvoiceLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (_lines.Count >= 200)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "A supplier invoice can have at most 200 lines.");
        }

        if (_lines.Any(l => l.PurchaseOrderLineId == line.PurchaseOrderLineId))
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "Each purchase order line may appear only once on a supplier invoice.");
        }

        _lines.Add(line);
        RecalculateTotals();
    }

    public void EditDraft(string? supplierInvoiceNumber, DateOnly invoiceDate, DateOnly? dueDate, decimal taxAmount, string? note, DateTimeOffset now)
    {
        EnsureStatus(SupplierInvoiceStatus.Draft, "Only a draft supplier invoice can be edited.");
        var cleanedSupplierInvNo = CleanSupplierInvoiceNumber(supplierInvoiceNumber);
        ValidateHeaderFields(invoiceDate, dueDate, taxAmount);

        SupplierInvoiceNumber = cleanedSupplierInvNo;
        NormalizedSupplierInvoiceNumber = cleanedSupplierInvNo.ToUpperInvariant();
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        TaxAmount = decimal.Round(taxAmount, 2, MidpointRounding.AwayFromZero);
        Note = CleanNote(note);
        RecalculateTotals();
        Touch(now);
    }

    public void ReplaceLines(IEnumerable<SupplierInvoiceLine> lines)
    {
        EnsureStatus(SupplierInvoiceStatus.Draft, "Only a draft supplier invoice can be edited.");
        var materialized = lines.ToList();
        ValidateLineCollection(materialized);
        _lines.Clear();
        _lines.AddRange(materialized);
        RecalculateTotals();
    }

    /// <summary>Evaluates 3-way unit price match against PO line unit prices and moves the invoice to matched or on_hold.</summary>
    public void Submit(IReadOnlyDictionary<Guid, decimal> poUnitPricesByLineId, DateTimeOffset now)
    {
        EnsureStatus(SupplierInvoiceStatus.Draft, $"Only a draft supplier invoice can be submitted; current status is '{Status}'.");
        ValidateLineCollection(_lines);

        foreach (var line in _lines)
        {
            if (!poUnitPricesByLineId.TryGetValue(line.PurchaseOrderLineId, out var poUnitPrice))
            {
                throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "An invoice line does not belong to the referenced purchase order.");
            }

            line.EvaluateAgainstPoPrice(poUnitPrice);
        }

        RecalculateTotals();
        var utcNow = now.ToUniversalTime();
        SubmittedAtUtc = utcNow;

        if (_lines.Any(l => l.ExceedsTolerance))
        {
            MatchStatus = SupplierInvoiceMatchStatus.PriceVariance;
            Status = SupplierInvoiceStatus.OnHold;
        }
        else
        {
            MatchStatus = SupplierInvoiceMatchStatus.Matched;
            Status = SupplierInvoiceStatus.Matched;
            PostedAtUtc = utcNow;
        }

        Touch(now);
    }

    /// <summary>Maker-checker decision on an on_hold supplier invoice.</summary>
    public void DecideHold(bool approve, Guid deciderUserId, string? note, DateTimeOffset now)
    {
        EnsureStatus(SupplierInvoiceStatus.OnHold, $"Only an on-hold supplier invoice can be decided; current status is '{Status}'.");
        if (deciderUserId == CreatedByUserId)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_SELF_APPROVAL", "A supplier invoice cannot be decided by its creator.");
        }

        if (!approve && string.IsNullOrWhiteSpace(note))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", "A note is required to reject a held supplier invoice.");
        }

        var utcNow = now.ToUniversalTime();
        Status = approve ? SupplierInvoiceStatus.Approved : SupplierInvoiceStatus.Rejected;
        if (approve)
        {
            PostedAtUtc = utcNow;
        }

        DecidedByUserId = deciderUserId;
        DecidedAtUtc = utcNow;
        DecisionNote = CleanNote(note);
        Touch(now);
    }

    public void Cancel(string? reason, DateTimeOffset now)
    {
        EnsureStatus(SupplierInvoiceStatus.Draft, $"Only a draft supplier invoice can be cancelled; current status is '{Status}'.");
        var cleanedReason = RequireReason(reason, "A reason is required to cancel a supplier invoice.");
        Status = SupplierInvoiceStatus.Cancelled;
        CancelReason = cleanedReason;
        Touch(now);
    }

    public void Void(string? reason, Guid actorUserId, DateTimeOffset now)
    {
        if (Status is not (SupplierInvoiceStatus.Matched or SupplierInvoiceStatus.Approved))
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_INVALID_STATE", $"Only a matched or approved supplier invoice can be voided; current status is '{Status}'.");
        }

        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor user ID cannot be empty.", nameof(actorUserId));
        var cleanedReason = RequireReason(reason, "A reason is required to void a supplier invoice.");

        Status = SupplierInvoiceStatus.Voided;
        VoidReason = cleanedReason;
        VoidedByUserId = actorUserId;
        VoidedAtUtc = now.ToUniversalTime();
        Touch(now);
    }

    private void RecalculateTotals()
    {
        SubtotalAmount = decimal.Round(_lines.Sum(l => l.LineTotal), 2, MidpointRounding.AwayFromZero);
        TotalPoAmount = decimal.Round(_lines.Sum(l => l.PoLineTotal), 2, MidpointRounding.AwayFromZero);
        TotalVarianceAmount = decimal.Round(_lines.Sum(l => l.PriceVarianceAmount), 2, MidpointRounding.AwayFromZero);
        TotalAmount = decimal.Round(SubtotalAmount + TaxAmount, 2, MidpointRounding.AwayFromZero);
    }

    private static void ValidateLineCollection(IReadOnlyCollection<SupplierInvoiceLine> lines)
    {
        if (lines.Count is < 1 or > 200)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "A supplier invoice must have between 1 and 200 lines.");
        }

        if (lines.Select(l => l.PurchaseOrderLineId).Distinct().Count() != lines.Count)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "Each purchase order line may appear only once on a supplier invoice.");
        }
    }

    private static string CleanSupplierInvoiceNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ProcurementDomainException("PROCUREMENT_FIELD_REQUIRED", "Supplier invoice number is required.");
        }

        var trimmed = value.Trim();
        if (trimmed.Length > 100)
        {
            throw new ProcurementDomainException("PROCUREMENT_FIELD_INVALID", "Supplier invoice number cannot exceed 100 characters.");
        }

        return trimmed;
    }

    private static void ValidateHeaderFields(DateOnly invoiceDate, DateOnly? dueDate, decimal taxAmount)
    {
        if (dueDate.HasValue && dueDate.Value < invoiceDate)
        {
            throw new ProcurementDomainException("PROCUREMENT_FIELD_INVALID", "Due date cannot be earlier than the invoice date.");
        }

        if (taxAmount < 0 || taxAmount > 999_999_999m)
        {
            throw new ProcurementDomainException("PROCUREMENT_FIELD_INVALID", "Tax amount cannot be negative.");
        }
    }

    private static string RequireReason(string? reason, string message)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ProcurementDomainException("PROCUREMENT_REASON_REQUIRED", message);
        }

        var trimmed = reason.Trim();
        if (trimmed.Length > 500)
        {
            throw new ProcurementDomainException("PROCUREMENT_FIELD_INVALID", "Reason cannot exceed 500 characters.");
        }

        return trimmed;
    }

    private void EnsureStatus(string expected, string message)
    {
        if (Status != expected)
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_INVALID_STATE", message);
        }
    }

    private static string? CleanNote(string? note)
    {
        var trimmed = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmed is { Length: > 500 })
        {
            throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "Note cannot exceed 500 characters.");
        }

        return trimmed;
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
