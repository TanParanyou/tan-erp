namespace TanErp.Application.Crm.Opportunities.ListOpportunities;

public sealed record ListOpportunitiesQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? Search = null,
    Guid? CustomerId = null,
    string? Stage = null,
    string? SortBy = null,
    string? SortOrder = null,
    int? Page = null,
    int Limit = 25,
    string? Cursor = null,
    string? TraceId = null);
