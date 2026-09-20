namespace TanErp.Application.Estimates.AcceptQuotation;

public sealed record AcceptQuotationCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid ExpectedOpportunityVersion,
    string? DecisionNote,
    string TraceId);
