namespace TanErp.Application.Crm.Opportunities.GetOpportunityMetrics;

public sealed record GetOpportunityMetricsQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? TraceId = null);
