using System.Security.Cryptography;
using System.Text;
using TanErp.Domain.Common;

namespace TanErp.Domain.Finance;

/// <summary>
/// A billing reference issued against a Project's contract amount. The amount, kind and dates never change after issue
/// (the reference hash proves it); a correction is a void followed by a new billing.
/// </summary>
public class BillingDocument : Entity
{
    private readonly List<Payment> _payments = new();

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

    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public decimal Outstanding => Status == BillingStatus.Voided ? 0m : Amount - PaidAmount;

    protected BillingDocument() { }

    public BillingDocument(Guid id, Guid organizationId, Guid branchId, Guid projectId, string number, string kind, string description, decimal amount, DateOnly? dueDate, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || projectId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, branch, project and actor are required.");
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number cannot be blank.", nameof(number));
        if (!BillingKind.All.Contains(kind)) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The billing kind is invalid.");
        var text = description?.Trim() ?? string.Empty;
        if (text.Length is 0 or > 200) throw new FinanceDomainException("BILLING_FIELD_INVALID", "A description of 1-200 characters is required.");
        if (amount <= 0 || amount > 999_999_999m || decimal.Round(amount, 2) != amount) throw new FinanceDomainException("BILLING_FIELD_INVALID", "The amount must be above zero with at most two decimals.");

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
        ReferenceHash = ComputeReferenceHash();
    }

    /// <summary>Hash over the fields that must never change, so any later tampering is detectable.</summary>
    public string ComputeReferenceHash()
    {
        var canonical = $"{OrganizationId}|{ProjectId}|{Number}|{Kind}|{Amount:0.00}|{Currency}|{IssuedAtUtc:O}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    public void AddPaymentRecord(Payment payment) => _payments.Add(payment);

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    private void Refresh()
    {
        Status = PaidAmount <= 0 ? BillingStatus.Issued : PaidAmount >= Amount ? BillingStatus.Paid : BillingStatus.PartiallyPaid;
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

    protected Payment() { }

    public Payment(Guid id, Guid organizationId, Guid billingDocumentId, string number, decimal amount, string method, string reference, DateOnly receivedDate, DateOnly today, Guid recordedBy, DateTimeOffset now) : base(id)
    {
        if (!PaymentMethod.All.Contains(method)) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "The payment method is invalid.");
        var refText = reference?.Trim() ?? string.Empty;
        if (refText.Length is 0 or > 100) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "A payment reference of 1-100 characters is required.");
        if (receivedDate > today) throw new FinanceDomainException("PAYMENT_FIELD_INVALID", "The received date cannot be in the future.");
        OrganizationId = organizationId;
        BillingDocumentId = billingDocumentId;
        Number = number;
        Amount = amount;
        Method = method;
        Reference = refText;
        ReceivedDate = receivedDate;
        RecordedByUserId = recordedBy;
        RecordedAtUtc = now.ToUniversalTime();
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
