namespace TanErp.Application.Crm.Opportunities.WorkImages.ListWorkImages;

public sealed record ListWorkImagesQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    string? Stage,
    int Limit = 25,
    string? Cursor = null,
    string TraceId = "");
