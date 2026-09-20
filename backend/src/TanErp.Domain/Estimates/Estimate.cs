namespace TanErp.Domain.Estimates;

public class Estimate
{
    private readonly List<EstimateRevision> _revisions = new();

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OpportunityId { get; private set; }
    public Guid? SiteSurveyRevisionId { get; private set; }
    public string? SiteSurveySnapshotHash { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Status { get; private set; } = EstimateStatus.Draft;
    public int CurrentRevisionNo { get; private set; } = 1;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<EstimateRevision> Revisions => _revisions.AsReadOnly();
    public EstimateRevision? CurrentRevision => _revisions.FirstOrDefault(r => r.RevisionNo == CurrentRevisionNo);

    private Estimate() { }

    public static Estimate CreateDraft(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid opportunityId,
        string number,
        Guid? siteSurveyRevisionId = null,
        string? siteSurveySnapshotHash = null,
        string currency = EstimateDefaults.DefaultCurrency)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Estimate ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty)
            throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));
        if (opportunityId == Guid.Empty)
            throw new ArgumentException("Opportunity ID cannot be empty.", nameof(opportunityId));
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Estimate number cannot be empty.", nameof(number));

        var estimate = new Estimate
        {
            Id = id,
            OrganizationId = organizationId,
            BranchId = branchId,
            CustomerId = customerId,
            OpportunityId = opportunityId,
            SiteSurveyRevisionId = siteSurveyRevisionId,
            SiteSurveySnapshotHash = string.IsNullOrWhiteSpace(siteSurveySnapshotHash) ? null : siteSurveySnapshotHash.Trim(),
            Number = number.Trim(),
            Status = EstimateStatus.Draft,
            CurrentRevisionNo = 1,
            RowVersion = Guid.NewGuid(),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        var initialRevision = new EstimateRevision(
            Guid.NewGuid(),
            organizationId,
            estimate.Id,
            1,
            EstimateRevisionStatus.Draft,
            currency);

        estimate._revisions.Add(initialRevision);
        return estimate;
    }

    public void AddRevision(EstimateRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        _revisions.Add(revision);
        CurrentRevisionNo = revision.RevisionNo;
        Status = revision.Status;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkQuoted()
    {
        CurrentRevision?.MarkQuoted();
        Status = EstimateStatus.Quoted;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
