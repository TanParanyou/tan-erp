namespace TanErp.Application.Estimates.IssueQuotation;

public sealed record IssueQuotationCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid ExpectedEstimateVersion,
    Guid ExpectedOpportunityVersion,
    string TraceId);
