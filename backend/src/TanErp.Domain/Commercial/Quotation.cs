namespace TanErp.Domain.Commercial;

public class Quotation
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OpportunityId { get; private set; }
    public Guid EstimateId { get; private set; }
    public Guid EstimateRevisionId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Status { get; private set; } = "issued";
    public decimal TotalAmount { get; private set; }
    public string? SnapshotHash { get; private set; }
    public string? CustomerBillingSnapshotJson { get; private set; }
    public string? CustomerBillingSnapshotHash { get; private set; }
    public DateTimeOffset IssuedAtUtc { get; private set; }
    public DateTimeOffset? AcceptedAtUtc { get; private set; }
    public Guid? SupersedesQuotationId { get; private set; }
    public Guid? SupersededByQuotationId { get; private set; }
    public string? AmendmentReason { get; private set; }
    public DateTimeOffset? VoidedAtUtc { get; private set; }
    public Guid? VoidedByUserId { get; private set; }
    public string? VoidReason { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private Quotation() { }

    public Quotation(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid opportunityId,
        Guid estimateId,
        Guid estimateRevisionId,
        string number,
        decimal totalAmount,
        string? snapshotHash,
        DateTimeOffset issuedAtUtc,
        string? customerBillingSnapshotJson = null,
        string? customerBillingSnapshotHash = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("Quotation ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (customerId == Guid.Empty) throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));
        if (opportunityId == Guid.Empty) throw new ArgumentException("Opportunity ID cannot be empty.", nameof(opportunityId));
        if (estimateId == Guid.Empty) throw new ArgumentException("Estimate ID cannot be empty.", nameof(estimateId));
        if (estimateRevisionId == Guid.Empty) throw new ArgumentException("Estimate revision ID cannot be empty.", nameof(estimateRevisionId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Quotation number cannot be empty.", nameof(number));

        Id = id;
        OrganizationId = organizationId;
        BranchId = branchId;
        CustomerId = customerId;
        OpportunityId = opportunityId;
        EstimateId = estimateId;
        EstimateRevisionId = estimateRevisionId;
        Number = number.Trim();
        Status = "issued";
        TotalAmount = totalAmount;
        SnapshotHash = string.IsNullOrWhiteSpace(snapshotHash) ? null : snapshotHash.Trim();
        CustomerBillingSnapshotJson = customerBillingSnapshotJson;
        CustomerBillingSnapshotHash = string.IsNullOrWhiteSpace(customerBillingSnapshotHash) ? null : customerBillingSnapshotHash.Trim();
        IssuedAtUtc = issuedAtUtc;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = issuedAtUtc;
        UpdatedAtUtc = issuedAtUtc;
    }

    public void Accept(DateTimeOffset acceptedAtUtc)
    {
        if (Status != "issued")
            throw new QuotationLifecycleException("QUOTATION_INVALID_STATE", $"Cannot accept quotation in status '{Status}'.");

        Status = "accepted";
        AcceptedAtUtc = acceptedAtUtc;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = acceptedAtUtc;
    }

    /// <summary>A new quotation that replaces an issued one. The original document stays immutable and linked.</summary>
    public void MarkAsAmendmentOf(Guid supersededQuotationId, string reason)
    {
        if (supersededQuotationId == Guid.Empty) throw new ArgumentException("Superseded quotation ID cannot be empty.", nameof(supersededQuotationId));
        if (string.IsNullOrWhiteSpace(reason)) throw new QuotationLifecycleException("QUOTATION_REASON_REQUIRED", "A reason is required to amend a quotation.");
        SupersedesQuotationId = supersededQuotationId;
        AmendmentReason = reason.Trim();
    }

    public void Supersede(Guid replacementQuotationId, DateTimeOffset now)
    {
        EnsureIssued("amended");
        Status = QuotationStatus.Superseded;
        SupersededByQuotationId = replacementQuotationId;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
    }

    public void Void(string reason, Guid actorUserId, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new QuotationLifecycleException("QUOTATION_REASON_REQUIRED", "A reason is required to void a quotation.");
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor ID cannot be empty.", nameof(actorUserId));
        EnsureIssued("voided");
        Status = QuotationStatus.Voided;
        VoidReason = reason.Trim();
        VoidedByUserId = actorUserId;
        VoidedAtUtc = now;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now;
    }

    /// <summary>Only an issued quotation can change. An accepted one is locked because a Won opportunity and a Project may already depend on it.</summary>
    private void EnsureIssued(string verb)
    {
        if (Status == QuotationStatus.Accepted)
        {
            throw new QuotationLifecycleException("QUOTATION_ACCEPTED_LOCKED", $"An accepted quotation cannot be {verb}.");
        }

        if (Status != QuotationStatus.Issued)
        {
            throw new QuotationLifecycleException("QUOTATION_INVALID_STATE", $"A quotation in status '{Status}' cannot be {verb}.");
        }
    }
}

public static class QuotationStatus
{
    public const string Issued = "issued";
    public const string Accepted = "accepted";
    public const string Superseded = "superseded";
    public const string Voided = "voided";
}

public class QuotationLifecycleException : Exception
{
    public string Code { get; }

    public QuotationLifecycleException(string code, string message) : base(message)
    {
        Code = code;
    }
}
