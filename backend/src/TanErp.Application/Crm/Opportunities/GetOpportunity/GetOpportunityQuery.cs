namespace TanErp.Application.Crm.Opportunities.GetOpportunity;

public sealed record GetOpportunityQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    string TraceId);
