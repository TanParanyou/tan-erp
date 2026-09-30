namespace TanErp.Domain.Estimates;

public sealed class EstimateApprovalDecision
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateApprovalRequestId { get; private set; }
    public Guid EstimateApprovalStepId { get; private set; }
    public Guid ReviewerUserId { get; private set; }
    public Guid ReviewerMembershipId { get; private set; }
    public string Decision { get; private set; } = string.Empty;
    public string? ReasonCode { get; private set; }
    public string? Note { get; private set; }
    public string CalculationSnapshotHash { get; private set; } = string.Empty;
    public string RouteHash { get; private set; } = string.Empty;
    public DateTimeOffset DecidedAtUtc { get; private set; }

    private EstimateApprovalDecision() { }

    public EstimateApprovalDecision(Guid id, Guid organizationId, Guid requestId, Guid stepId,
        Guid reviewerUserId, Guid reviewerMembershipId, string decision, string? reasonCode,
        string? note, string calculationSnapshotHash, string routeHash, DateTimeOffset decidedAtUtc)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || requestId == Guid.Empty || stepId == Guid.Empty || reviewerUserId == Guid.Empty || reviewerMembershipId == Guid.Empty)
            throw new ArgumentException("Approval decision identifiers must be provided.");
        if (decision is not (EstimateApprovalStepStatus.Approved or EstimateApprovalStepStatus.Returned))
            throw new ArgumentException("Approval decision is unsupported.", nameof(decision));
        if (decision == EstimateApprovalStepStatus.Returned && (string.IsNullOrWhiteSpace(reasonCode) || string.IsNullOrWhiteSpace(note)))
            throw new ArgumentException("Return requires a reason code and note.");
        if (string.IsNullOrWhiteSpace(calculationSnapshotHash) || string.IsNullOrWhiteSpace(routeHash))
            throw new ArgumentException("Decision must reference the frozen calculation and route hashes.");

        Id = id;
        OrganizationId = organizationId;
        EstimateApprovalRequestId = requestId;
        EstimateApprovalStepId = stepId;
        ReviewerUserId = reviewerUserId;
        ReviewerMembershipId = reviewerMembershipId;
        Decision = decision;
        ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        CalculationSnapshotHash = calculationSnapshotHash.Trim();
        RouteHash = routeHash.Trim();
        DecidedAtUtc = decidedAtUtc;
    }
}
