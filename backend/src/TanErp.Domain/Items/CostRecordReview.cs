using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class CostRecordReview : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid CostRecordId { get; private set; }
    public string Decision { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public Guid ReviewerUserId { get; private set; }
    public string? AuthoritySnapshot { get; private set; }
    public DateTimeOffset DecidedAtUtc { get; private set; }

    protected CostRecordReview() { }

    public CostRecordReview(
        Guid id,
        Guid organizationId,
        Guid costRecordId,
        string decision,
        string? reason,
        Guid reviewerUserId,
        string? authoritySnapshot,
        DateTimeOffset decidedAtUtc) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ItemValidationException("COST_REVIEW_ORG_REQUIRED", "Organization is required.");
        if (costRecordId == Guid.Empty)
            throw new ItemValidationException("COST_REVIEW_RECORD_REQUIRED", "Cost record is required.");
        if (string.IsNullOrWhiteSpace(decision))
            throw new ItemValidationException("COST_REVIEW_DECISION_REQUIRED", "Decision is required.");
        if (reviewerUserId == Guid.Empty)
            throw new ItemValidationException("COST_REVIEWER_REQUIRED", "Reviewer user ID is required.");

        OrganizationId = organizationId;
        CostRecordId = costRecordId;
        Decision = decision.Trim().ToLowerInvariant();
        Reason = reason?.Trim();
        ReviewerUserId = reviewerUserId;
        AuthoritySnapshot = authoritySnapshot;
        DecidedAtUtc = decidedAtUtc;
    }
}
