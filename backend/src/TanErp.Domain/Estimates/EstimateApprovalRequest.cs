namespace TanErp.Domain.Estimates;

public static class EstimateApprovalRequestStatus
{
    public const string Open = "open";
    public const string Approved = "approved";
    public const string Returned = "returned";
    public const string Cancelled = "cancelled";
}

public sealed class EstimateApprovalRequest
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateId { get; private set; }
    public Guid EstimateRevisionId { get; private set; }
    public int RevisionNo { get; private set; }
    public int CalculationVersion { get; private set; }
    public string CalculationInputHash { get; private set; } = string.Empty;
    public string CalculationSnapshotHash { get; private set; } = string.Empty;
    public string PolicyCode { get; private set; } = string.Empty;
    public int PolicyVersion { get; private set; }
    public string RouteSnapshotJson { get; private set; } = string.Empty;
    public string RouteHash { get; private set; } = string.Empty;
    public string? SubmissionNote { get; private set; }
    public string Status { get; private set; } = EstimateApprovalRequestStatus.Open;
    public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; }
    public DateTimeOffset? ClosedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    private EstimateApprovalRequest() { }

    public EstimateApprovalRequest(
        Guid id, Guid organizationId, Guid estimateId, Guid estimateRevisionId,
        int revisionNo, int calculationVersion, string calculationInputHash,
        string calculationSnapshotHash, string policyCode, int policyVersion,
        string routeSnapshotJson, string routeHash, Guid requestedByUserId,
        DateTimeOffset requestedAtUtc, string? submissionNote = null)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || estimateId == Guid.Empty || estimateRevisionId == Guid.Empty || requestedByUserId == Guid.Empty)
            throw new ArgumentException("Approval request identifiers must be provided.");
        if (revisionNo <= 0 || calculationVersion <= 0 || policyVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(revisionNo));
        if (string.IsNullOrWhiteSpace(calculationInputHash) || string.IsNullOrWhiteSpace(calculationSnapshotHash) ||
            string.IsNullOrWhiteSpace(policyCode) || string.IsNullOrWhiteSpace(routeSnapshotJson) || string.IsNullOrWhiteSpace(routeHash))
            throw new ArgumentException("Approval request snapshots and hashes are required.");

        Id = id;
        OrganizationId = organizationId;
        EstimateId = estimateId;
        EstimateRevisionId = estimateRevisionId;
        RevisionNo = revisionNo;
        CalculationVersion = calculationVersion;
        CalculationInputHash = calculationInputHash.Trim();
        CalculationSnapshotHash = calculationSnapshotHash.Trim();
        PolicyCode = policyCode.Trim();
        PolicyVersion = policyVersion;
        RouteSnapshotJson = routeSnapshotJson;
        RouteHash = routeHash.Trim();
        SubmissionNote = string.IsNullOrWhiteSpace(submissionNote) ? null : submissionNote.Trim();
        RequestedByUserId = requestedByUserId;
        RequestedAtUtc = requestedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public void Close(string status, DateTimeOffset closedAtUtc)
    {
        if (Status != EstimateApprovalRequestStatus.Open)
            throw new EstimateInvalidStateException("Only an open approval request can be closed.");
        if (status is not (EstimateApprovalRequestStatus.Approved or EstimateApprovalRequestStatus.Returned or EstimateApprovalRequestStatus.Cancelled))
            throw new ArgumentException("Approval request close status is unsupported.", nameof(status));

        Status = status;
        ClosedAtUtc = closedAtUtc;
        RowVersion = Guid.NewGuid();
    }
}
