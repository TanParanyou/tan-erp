namespace TanErp.Application.Crm.Opportunities.GetOpportunityStageHistory;

public sealed record GetOpportunityStageHistoryQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    string TraceId);
