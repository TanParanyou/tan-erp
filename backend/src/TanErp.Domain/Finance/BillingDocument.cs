using System.Security.Cryptography;
using System.Text;
using TanErp.Domain.Common;

namespace TanErp.Domain.Finance;

/// <summary>
/// A billing reference issued against a Project's contract amount. The amount, kind and dates never change after issue
/// (the reference hash proves it); a correction is a void followed by a new billing, or approved Credit/Debit notes.
/// </summary>
public class BillingDocument : Entity
{
    private readonly List<Payment> _payments = new();
    private readonly List<BillingLine> _lines = new();
    private readonly List<BillingAdjustmentNote> _adjustmentNotes = new();

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Kind { get; private set; } = BillingKind.Other;
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public string Currency { get; private set; } = "THB";
    public DateOnly? DueDate { get; private set; }
    public string Status { get; private set; } = BillingStatus.Issued;
    public string ReferenceHash { get; private set; } = string.Empty;
    public string? VoidReason { get; private set; }
    public DateTimeOffset? VoidedAtUtc { get; private set; }
    public Guid? VoidedByUserId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset IssuedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    // Wave D G-15 Thai Tax fields
    public string? TaxInvoiceNumber { get; private set; }
    public decimal VatRate { get; private set; }
    public decimal VatAmount { get; private set; }
    public decimal RetentionRate { get; private set; }
    public decimal RetentionAmount { get; private set; }
    public string RetentionStatus { get; private set; } = RetentionHoldingStatus.None;
    public DateTimeOffset? ReleasedRetentionAtUtc { get; private set; }
    public Guid? ReleasedRetentionByUserId { get; private set; }
    public decimal CreditNoteBaseAmount { get; private set; }
    public decimal CreditNoteVatAmount { get; private set; }
    public decimal DebitNoteBaseAmount { get; private set; }
    public decimal DebitNoteVatAmount { get; private set; }

    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<BillingLine> Lines => _lines.AsReadOnly();
    public IReadOnlyCollection<BillingAdjustmentNote> AdjustmentNotes => _adjustmentNotes.AsReadOnly();

    public decimal GrossTotal => Amount + VatAmount + DebitNoteBaseAmount + DebitNoteVatAmount - CreditNoteBaseAmount - CreditNoteVatAmount;
    public decimal EffectiveReceivable => GrossTotal - (RetentionStatus == RetentionHoldingStatus.Held ? RetentionAmount : 0m);
    public decimal Outstanding => Status == BillingStatus.Voided ? 0m : Math.Max(0m, EffectiveReceivable - PaidAmount);

    protected BillingDocument() { }

    public BillingDocument(
        Guid id, Guid organizationId, Guid branchId, Guid projectId, string number, string kind, string description,
        decimal amount, DateOnly? dueDate, Guid createdByUserId, DateTimeOffset now)
        : this(id, organizationId, branchId, projectId, number, kind, description, amount, dueDate, createdByUserId, now, 0m, 0m, null, null)
    {
    }

    public BillingDocument(
        Guid id, Guid organizationId, Guid branchId, Guid projectId, string number, string kind, string description,
        decimal amount, DateOnly? dueDate, Guid createdByUserId, DateTimeOffset now,
        decimal vatRate, decimal retentionRate = 0m, string? taxInvoiceNumber = null, IEnumerable<BillingLine>? lines = null) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || projectId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Organization, branch, project and actor are required.");
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number cannot be blank.", nameof(number));
        if (!BillingKind.All.Contains(kind)) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The billing kind is invalid.");
        var text = description?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 200) throw new FinanceDomainException("BILLING_FIELD_INVALID", "A description of 1-200 characters is required.");
        if (amount <= 0 || amount > 999_999_999m || decimal.Round(amount, 2) != amount) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The amount must be above zero with at most two decimals.");
        if (vatRate < 0m || vatRate > 0.20m) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The VAT rate must be between 0 and 0.20.");
        if (retentionRate < 0m || retentionRate > 0.30m) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The retention rate must be between 0 and 0.30.");

        OrganizationId = organizationId;
        BranchId = branchId;
        ProjectId = projectId;
        Number = number.Trim();
        Kind = kind;
        Description = text;
        Amount = amount;
        DueDate = dueDate;
        IssuedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = IssuedAtUtc;
        CreatedByUserId = createdByUserId;
        RowVersion = Guid.NewGuid();

        VatRate = vatRate;
        RetentionRate = retentionRate;
        RetentionAmount = decimal.Round(amount * retentionRate, 2, MidpointRounding.AwayFromZero);
        RetentionStatus = RetentionAmount > 0m ? RetentionHoldingStatus.Held : RetentionHoldingStatus.None;
        TaxInvoiceNumber = string.IsNullOrWhiteSpace(taxInvoiceNumber) ? null : taxInvoiceNumber.Trim();

        if (lines != null)
        {
            var lineList = lines.ToList();
            if (lineList.Count > 0)
            {
                var sumTaxBase = lineList.Sum(l => l.TaxBaseAmount);
                if (sumTaxBase != amount)
                {
                    throw new FinanceDomainException("BILLING_FIELD_INVALID", "The sum of line tax base amounts must equal the billing amount.");
                }
                _lines.AddRange(lineList);
                VatAmount = lineList.Sum(l => l.VatAmount);
            }
            else
            {
                VatAmount = decimal.Round(amount * vatRate, 2, MidpointRounding.AwayFromZero);
            }
        }
        else
        {
            VatAmount = decimal.Round(amount * vatRate, 2, MidpointRounding.AwayFromZero);
        }

        ReferenceHash = ComputeReferenceHash();
    }

    /// <summary>Hash over the fields that must never change, so any later tampering is detectable.</summary>
    public string ComputeReferenceHash()
    {
        var canonical = (VatRate == 0m && RetentionRate == 0m && string.IsNullOrEmpty(TaxInvoiceNumber))
            ? $"{OrganizationId}|{ProjectId}|{Number}|{Kind}|{Amount:0.00}|{Currency}|{IssuedAtUtc:O}"
            : $"{OrganizationId}|{ProjectId}|{Number}|{Kind}|{Amount:0.00}|{Currency}|{IssuedAtUtc:O}|{VatRate:0.0000}|{VatAmount:0.00}|{RetentionRate:0.0000}|{RetentionAmount:0.00}|{TaxInvoiceNumber ?? ""}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    public void AddPaymentRecord(Payment payment) => _payments.Add(payment);
    public void AddLineRecord(BillingLine line) => _lines.Add(line);
    public void AddAdjustmentNoteRecord(BillingAdjustmentNote note) => _adjustmentNotes.Add(note);

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    private void Refresh()
    {
        Status = PaidAmount <= 0
            ? BillingStatus.Issued
            : (PaidAmount >= GrossTotal && RetentionStatus != RetentionHoldingStatus.Held)
                ? BillingStatus.Paid
                : BillingStatus.PartiallyPaid;
    }

    /// <summary>Partial payments are allowed; paying more than the outstanding amount is refused.</summary>
    public void ApplyPayment(decimal amount, DateTimeOffset now)
    {
        if (Status == BillingStatus.Voided) throw new FinanceDomainException("BILLING_INVALID_STATE", "A voided billing cannot receive payments.");
        if (amount <= 0 || decimal.Round(amount, 2) != amount) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "The payment must be above zero with at most two decimals.");
        if (amount > Outstanding) throw new FinanceDomainException("PAYMENT_EXCEEDS_OUTSTANDING", "The payment exceeds the outstanding amount.");
        PaidAmount += amount;
        Refresh();
        Touch(now);
    }

    public void RemovePayment(decimal amount, DateTimeOffset now)
    {
        if (Status == BillingStatus.Voided) throw new FinanceDomainException("BILLING_INVALID_STATE", "A voided billing has no payments to reverse.");
        if (amount <= 0 || amount > PaidAmount) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "The reversal exceeds what was paid.");
        PaidAmount -= amount;
        Refresh();
        Touch(now);
    }

    /// <summary>A billing with money received cannot be voided; reverse the payments first.</summary>
    public void Void(string reason, Guid actorUserId, DateTimeOffset now)
    {
        if (Status == BillingStatus.Voided) throw new FinanceDomainException("BILLING_INVALID_STATE", "The billing is already voided.");
        if (PaidAmount > 0) throw new FinanceDomainException("BILLING_HAS_PAYMENTS", "Reverse the payments before voiding the billing.");
        var text = reason?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new FinanceDomainException("BILLING_REASON_REQUIRED", "A reason of 1-500 characters is required.");
        Status = BillingStatus.Voided;
        VoidReason = text;
        VoidedByUserId = actorUserId;
        VoidedAtUtc = now.ToUniversalTime();
        Touch(now);
    }

    public void ReleaseRetention(Guid actorUserId, DateTimeOffset now)
    {
        if (Status == BillingStatus.Voided || RetentionStatus != RetentionHoldingStatus.Held)
        {
            throw new FinanceDomainException("BILLING_INVALID_STATE", "Retention can only be released for an active billing with held retention.");
        }

        RetentionStatus = RetentionHoldingStatus.Released;
        ReleasedRetentionAtUtc = now.ToUniversalTime();
        ReleasedRetentionByUserId = actorUserId;
        Refresh();
        Touch(now);
    }

    public void ApplyApprovedNote(BillingAdjustmentNote note, DateTimeOffset now)
    {
        if (Status == BillingStatus.Voided)
        {
            throw new FinanceDomainException("BILLING_INVALID_STATE", "A voided billing cannot receive adjustment notes.");
        }

        if (note.NoteType == BillingNoteType.Credit)
        {
            if (note.TaxBaseDelta > (Amount + DebitNoteBaseAmount - CreditNoteBaseAmount) ||
                (GrossTotal - note.TotalDelta) < PaidAmount)
            {
                throw new FinanceDomainException("BILLING_NOTE_EXCEEDS_ORIGINAL", "Credit note exceeds remaining billing amount.");
            }

            CreditNoteBaseAmount += note.TaxBaseDelta;
            CreditNoteVatAmount += note.VatDelta;
        }
        else if (note.NoteType == BillingNoteType.Debit)
        {
            DebitNoteBaseAmount += note.TaxBaseDelta;
            DebitNoteVatAmount += note.VatDelta;
        }

        Refresh();
        Touch(now);
    }
}

public class BillingLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BillingDocumentId { get; private set; }
    public int LineNumber { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal TaxBaseAmount { get; private set; }
    public decimal VatRate { get; private set; }
    public decimal VatAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    protected BillingLine() { }

    public BillingLine(Guid id, Guid organizationId, Guid billingDocumentId, int lineNumber, string description, decimal taxBaseAmount, decimal vatRate) : base(id)
    {
        if (organizationId == Guid.Empty || billingDocumentId == Guid.Empty) throw new ArgumentException("Organization and billing document are required.");
        if (lineNumber < 1) throw new FinanceDomainException("BILLING_FIELD_INVALID", "Line number must be at least 1.");
        var text = description?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 200) throw new FinanceDomainException("BILLING_FIELD_INVALID", "A line description of 1-200 characters is required.");
        if (taxBaseAmount <= 0 || taxBaseAmount > 999_999_999m || decimal.Round(taxBaseAmount, 2) != taxBaseAmount)
            throw new FinanceDomainException("BILLING_FIELD_INVALID", "The tax base amount must be above zero with at most two decimals.");
        if (vatRate < 0m || vatRate > 0.20m || decimal.Round(vatRate, 4) != vatRate)
            throw new FinanceDomainException("BILLING_FIELD_INVALID", "The VAT rate must be between 0 and 0.20 with at most four decimals.");

        OrganizationId = organizationId;
        BillingDocumentId = billingDocumentId;
        LineNumber = lineNumber;
        Description = text;
        TaxBaseAmount = taxBaseAmount;
        VatRate = vatRate;
        VatAmount = decimal.Round(taxBaseAmount * vatRate, 2, MidpointRounding.AwayFromZero);
        TotalAmount = taxBaseAmount + VatAmount;
    }
}

public class BillingAdjustmentNote : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BillingDocumentId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string NoteType { get; private set; } = BillingNoteType.Credit;
    public string Reason { get; private set; } = string.Empty;
    public decimal TaxBaseDelta { get; private set; }
    public decimal VatRate { get; private set; }
    public decimal VatDelta { get; private set; }
    public decimal TotalDelta { get; private set; }
    public string Status { get; private set; } = BillingNoteStatus.PendingApproval;
    public Guid CreatedByUserId { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected BillingAdjustmentNote() { }

    public BillingAdjustmentNote(
        Guid id, Guid organizationId, Guid branchId, Guid billingDocumentId, string number,
        string noteType, string reason, decimal taxBaseDelta, decimal vatRate,
        Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || billingDocumentId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Organization, branch, billing document and actor are required.");
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number cannot be blank.", nameof(number));
        if (!BillingNoteType.All.Contains(noteType)) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The note type is invalid.");
        var text = reason?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new FinanceDomainException("BILLING_FIELD_INVALID", "A reason of 1-500 characters is required.");
        if (taxBaseDelta <= 0 || taxBaseDelta > 999_999_999m || decimal.Round(taxBaseDelta, 2) != taxBaseDelta)
            throw new FinanceDomainException("BILLING_FIELD_INVALID", "The tax base delta must be above zero with at most two decimals.");
        if (vatRate < 0m || vatRate > 0.20m) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The VAT rate must be between 0 and 0.20.");

        OrganizationId = organizationId;
        BranchId = branchId;
        BillingDocumentId = billingDocumentId;
        Number = number.Trim();
        NoteType = noteType;
        Reason = text;
        TaxBaseDelta = taxBaseDelta;
        VatRate = vatRate;
        VatDelta = decimal.Round(taxBaseDelta * vatRate, 2, MidpointRounding.AwayFromZero);
        TotalDelta = taxBaseDelta + VatDelta;
        Status = BillingNoteStatus.PendingApproval;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void Approve(Guid approverUserId, DateTimeOffset now)
    {
        if (Status != BillingNoteStatus.PendingApproval)
            throw new FinanceDomainException("BILLING_NOTE_INVALID_STATE", "The note is not pending approval.");
        if (approverUserId == CreatedByUserId)
            throw new FinanceDomainException("BILLING_NOTE_SELF_APPROVAL", "The creator cannot approve their own note.");

        Status = BillingNoteStatus.Approved;
        DecidedByUserId = approverUserId;
        DecidedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }
}

public class Payment : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BillingDocumentId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Method { get; private set; } = PaymentMethod.Transfer;
    public string Reference { get; private set; } = string.Empty;
    public DateOnly ReceivedDate { get; private set; }
    public string Status { get; private set; } = PaymentStatus.Recorded;
    public string? ReversalReason { get; private set; }
    public DateTimeOffset? ReversedAtUtc { get; private set; }
    public Guid? ReversedByUserId { get; private set; }
    public Guid RecordedByUserId { get; private set; }
    public DateTimeOffset RecordedAtUtc { get; private set; }

    // Wave D G-15 Thai Tax fields
    public string? ReceiptNumber { get; private set; }
    public decimal WhtRate { get; private set; }
    public decimal WhtAmount { get; private set; }
    public decimal NetCashReceived { get; private set; }
    public string? WhtType { get; private set; }
    public string? WhtCertificateNumber { get; private set; }

    protected Payment() { }

    public Payment(Guid id, Guid organizationId, Guid billingDocumentId, string number, decimal amount, string method, string reference, DateOnly receivedDate, DateOnly today, Guid recordedBy, DateTimeOffset now)
        : this(id, organizationId, billingDocumentId, number, amount, method, reference, receivedDate, today, recordedBy, now, 0m, null, null, null)
    {
    }

    public Payment(
        Guid id, Guid organizationId, Guid billingDocumentId, string number, decimal amount, string method, string reference,
        DateOnly receivedDate, DateOnly today, Guid recordedBy, DateTimeOffset now,
        decimal whtRate = 0m, string? whtType = null, string? whtCertificateNumber = null, string? receiptNumber = null) : base(id)
    {
        if (!PaymentMethod.All.Contains(method)) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "The payment method is invalid.");
        var refText = reference?.Trim() ?? string.Empty;
        if (refText.Length is 0 or > 100) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "A payment reference of 1-100 characters is required.");
        if (receivedDate > today) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "The received date cannot be in the future.");
        if (whtRate < 0m || whtRate > 0.30m) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "WHT rate must be between 0 and 0.30.");

        var certText = whtCertificateNumber?.Trim();
        if (certText is not null && certText.Length > 100) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "WHT certificate number cannot exceed 100 characters.");
        if (whtRate > 0m && string.IsNullOrWhiteSpace(certText)) throw new FinanceDomainException("PAYMENT_WHT_CERTIFICATE_REQUIRED", "A 50 Bis WHT certificate number is required when WHT is applied.");

        OrganizationId = organizationId;
        BillingDocumentId = billingDocumentId;
        Number = number;
        Amount = amount;
        Method = method;
        Reference = refText;
        ReceivedDate = receivedDate;
        RecordedByUserId = recordedBy;
        RecordedAtUtc = now.ToUniversalTime();

        WhtRate = whtRate;
        WhtAmount = decimal.Round(amount * whtRate, 2, MidpointRounding.AwayFromZero);
        NetCashReceived = amount - WhtAmount;
        WhtType = string.IsNullOrWhiteSpace(whtType) ? null : whtType.Trim();
        WhtCertificateNumber = certText;
        ReceiptNumber = string.IsNullOrWhiteSpace(receiptNumber) ? null : receiptNumber.Trim();
    }

    public void Reverse(string reason, Guid actorUserId, DateTimeOffset now)
    {
        if (Status == PaymentStatus.Reversed) throw new FinanceDomainException("PAYMENT_INVALID_STATE", "The payment is already reversed.");
        var text = reason?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 500) throw new FinanceDomainException("BILLING_REASON_REQUIRED", "A reason of 1-500 characters is required.");
        Status = PaymentStatus.Reversed;
        ReversalReason = text;
        ReversedByUserId = actorUserId;
        ReversedAtUtc = now.ToUniversalTime();
    }
}

/// <summary>
/// One message to the accounting system. The payload is a frozen snapshot, the dedupe key stops the same event being queued twice,
/// and the outcome of each delivery attempt is kept so a failure can be retried or investigated.
/// </summary>
public class AccountingOutboxMessage : Entity
{
    public const int MaxAttempts = 5;

    public Guid OrganizationId { get; private set; }
    public string DedupeKey { get; private set; } = string.Empty;
    public string Kind { get; private set; } = string.Empty;
    public Guid ResourceId { get; private set; }
    public string ResourceNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string PayloadJson { get; private set; } = "{}";
    public string Status { get; private set; } = OutboxStatus.Pending;
    public int Attempts { get; private set; }
    public DateTimeOffset NextAttemptAtUtc { get; private set; }
    public string? LastError { get; private set; }
    public string? ExternalRef { get; private set; }
    public decimal? ExternalAmount { get; private set; }
    public DateTimeOffset? ConfirmedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? SentAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected AccountingOutboxMessage() { }

    public AccountingOutboxMessage(Guid id, Guid organizationId, string dedupeKey, string kind, Guid resourceId, string resourceNumber, decimal amount, string payloadJson, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        DedupeKey = dedupeKey;
        Kind = kind;
        ResourceId = resourceId;
        ResourceNumber = resourceNumber;
        Amount = amount;
        PayloadJson = payloadJson;
        NextAttemptAtUtc = now.ToUniversalTime();
        CreatedAtUtc = NextAttemptAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public bool IsDue(DateTimeOffset now) => Status is OutboxStatus.Pending or OutboxStatus.Failed && NextAttemptAtUtc <= now;

    /// <summary>Delivery succeeded; the accounting system's reference is recorded.</summary>
    public void MarkSent(string? externalRef, DateTimeOffset now)
    {
        Attempts++;
        Status = OutboxStatus.Sent;
        ExternalRef = string.IsNullOrWhiteSpace(externalRef) ? null : externalRef.Trim();
        SentAtUtc = now.ToUniversalTime();
        LastError = null;
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Delivery failed. Retried with growing delays (2, 4, 8, 16 minutes) and parked as dead after the last attempt.</summary>
    public void MarkFailed(string error, DateTimeOffset now)
    {
        Attempts++;
        LastError = error.Length > 500 ? error[..500] : error;
        if (Attempts >= MaxAttempts)
        {
            Status = OutboxStatus.Dead;
        }
        else
        {
            Status = OutboxStatus.Failed;
            NextAttemptAtUtc = now.ToUniversalTime().AddMinutes(Math.Pow(2, Attempts));
        }

        RowVersion = Guid.NewGuid();
    }

    /// <summary>Puts a dead or failed message back in the queue after someone has dealt with the cause.</summary>
    public void Requeue(DateTimeOffset now)
    {
        if (Status is not (OutboxStatus.Dead or OutboxStatus.Failed))
        {
            throw new FinanceDomainException("OUTBOX_INVALID_STATE", $"Only a failed or dead message can be requeued; current status is '{Status}'.");
        }

        Status = OutboxStatus.Pending;
        Attempts = 0;
        NextAttemptAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    /// <summary>The accounting system's confirmation (callback). Repeating the same confirmation is harmless; a different reference is a conflict.</summary>
    public void Confirm(string externalRef, decimal externalAmount, DateTimeOffset now)
    {
        var reference = externalRef?.Trim() ?? string.Empty;
        if (reference.Length is 0 or > 100) throw new FinanceDomainException("OUTBOX_FIELD_INVALID", "An external reference of 1-100 characters is required.");
        if (externalAmount < 0) throw new FinanceDomainException("OUTBOX_FIELD_INVALID", "The confirmed amount cannot be negative.");
        if (ConfirmedAtUtc.HasValue)
        {
            if (ExternalRef != reference || ExternalAmount != externalAmount)
            {
                throw new FinanceDomainException("OUTBOX_CONFIRMATION_CONFLICT", "The message was already confirmed with a different reference or amount.");
            }

            return;
        }

        ExternalRef = reference;
        ExternalAmount = externalAmount;
        ConfirmedAtUtc = TruncateToMicroseconds(now.ToUniversalTime());
        if (Status != OutboxStatus.Sent)
        {
            Status = OutboxStatus.Sent;
            SentAtUtc = ConfirmedAtUtc;
            LastError = null;
        }

        RowVersion = Guid.NewGuid();
    }

    // PostgreSQL timestamptz keeps microseconds while Linux clocks give 100ns ticks; truncate so the first response equals what a later read returns.
    private static DateTimeOffset TruncateToMicroseconds(DateTimeOffset value) =>
        value.AddTicks(-(value.Ticks % (TimeSpan.TicksPerMillisecond / 1000)));
}
