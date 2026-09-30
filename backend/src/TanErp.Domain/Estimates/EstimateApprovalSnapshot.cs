namespace TanErp.Domain.Estimates;

public sealed class EstimateApprovalSnapshot
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateId { get; private set; }
    public Guid EstimateRevisionId { get; private set; }
    public Guid EstimateApprovalRequestId { get; private set; }
    public int CalculationVersion { get; private set; }
    public string CalculationInputHash { get; private set; } = string.Empty;
    public string CalculationSnapshotHash { get; private set; } = string.Empty;
    public string RouteHash { get; private set; } = string.Empty;
    public string SnapshotJson { get; private set; } = string.Empty;
    public Guid ApprovedByUserId { get; private set; }
    public DateTimeOffset ApprovedAtUtc { get; private set; }

    private EstimateApprovalSnapshot() { }

    public EstimateApprovalSnapshot(Guid id, Guid organizationId, Guid estimateId, Guid revisionId,
        Guid requestId, int calculationVersion, string calculationInputHash,
        string calculationSnapshotHash, string routeHash, string snapshotJson,
        Guid approvedByUserId, DateTimeOffset approvedAtUtc)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || estimateId == Guid.Empty || revisionId == Guid.Empty || requestId == Guid.Empty || approvedByUserId == Guid.Empty)
            throw new ArgumentException("Approval snapshot identifiers must be provided.");
        if (calculationVersion <= 0 || string.IsNullOrWhiteSpace(calculationInputHash) || string.IsNullOrWhiteSpace(calculationSnapshotHash) ||
            string.IsNullOrWhiteSpace(routeHash) || string.IsNullOrWhiteSpace(snapshotJson))
            throw new ArgumentException("Approval snapshot versions, hashes, and payload are required.");

        Id = id;
        OrganizationId = organizationId;
        EstimateId = estimateId;
        EstimateRevisionId = revisionId;
        EstimateApprovalRequestId = requestId;
        CalculationVersion = calculationVersion;
        CalculationInputHash = calculationInputHash.Trim();
        CalculationSnapshotHash = calculationSnapshotHash.Trim();
        RouteHash = routeHash.Trim();
        SnapshotJson = snapshotJson;
        ApprovedByUserId = approvedByUserId;
        ApprovedAtUtc = approvedAtUtc;
    }
}
