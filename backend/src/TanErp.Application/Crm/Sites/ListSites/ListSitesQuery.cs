namespace TanErp.Application.Crm.Sites.ListSites;

public sealed record ListSitesQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid CustomerId,
    string TraceId);
