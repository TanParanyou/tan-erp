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
    public DateTimeOffset IssuedAtUtc { get; private set; }
    public DateTimeOffset? AcceptedAtUtc { get; private set; }
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
        DateTimeOffset issuedAtUtc)
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
        IssuedAtUtc = issuedAtUtc;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = issuedAtUtc;
        UpdatedAtUtc = issuedAtUtc;
    }

    public void Accept(DateTimeOffset acceptedAtUtc)
    {
        if (Status != "issued")
            throw new InvalidOperationException($"Cannot accept quotation in status '{Status}'.");

        Status = "accepted";
        AcceptedAtUtc = acceptedAtUtc;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = acceptedAtUtc;
    }
}
