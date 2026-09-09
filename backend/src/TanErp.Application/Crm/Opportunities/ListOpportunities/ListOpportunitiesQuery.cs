namespace TanErp.Application.Crm.Opportunities.ListOpportunities;

public sealed record ListOpportunitiesQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? Search,
    Guid? CustomerId,
    string? Stage,
    int Limit,
    string? Cursor,
    string TraceId);
