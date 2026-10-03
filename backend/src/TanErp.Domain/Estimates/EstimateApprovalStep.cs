namespace TanErp.Domain.Estimates;

public static class EstimateApprovalStepStatus
{
    public const string Pending = "pending";
    public const string Approved = "approved";
    public const string Returned = "returned";
}

public sealed class EstimateApprovalStep
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateApprovalRequestId { get; private set; }
    public int Sequence { get; private set; }
    public Guid ReviewerUserId { get; private set; }
    public Guid ReviewerMembershipId { get; private set; }
    public string PermissionKey { get; private set; } = "estimates.approve";
    public string ScopeType { get; private set; } = "branch";
    public Guid ScopeId { get; private set; }
    public string Status { get; private set; } = EstimateApprovalStepStatus.Pending;
    public Guid RowVersion { get; private set; }

    private EstimateApprovalStep() { }

    public EstimateApprovalStep(Guid id, Guid organizationId, Guid requestId, int sequence,
        Guid reviewerUserId, Guid reviewerMembershipId, string scopeType, Guid scopeId)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || requestId == Guid.Empty || reviewerUserId == Guid.Empty || reviewerMembershipId == Guid.Empty || scopeId == Guid.Empty)
            throw new ArgumentException("Approval step identifiers must be provided.");
        if (sequence <= 0 || scopeType is not ("organization" or "branch"))
            throw new ArgumentException("Approval step sequence or scope is invalid.");

        Id = id;
        OrganizationId = organizationId;
        EstimateApprovalRequestId = requestId;
        Sequence = sequence;
        ReviewerUserId = reviewerUserId;
        ReviewerMembershipId = reviewerMembershipId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        RowVersion = Guid.NewGuid();
    }

    public void Decide(string decision)
    {
        if (Status != EstimateApprovalStepStatus.Pending)
            throw new EstimateInvalidStateException("Only a pending approval step can be decided.");
        if (decision is not (EstimateApprovalStepStatus.Approved or EstimateApprovalStepStatus.Returned))
            throw new ArgumentException("Approval step decision is unsupported.", nameof(decision));

        Status = decision;
        RowVersion = Guid.NewGuid();
    }
}
