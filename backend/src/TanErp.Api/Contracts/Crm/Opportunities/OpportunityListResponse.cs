namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record OpportunityListResponse(
    IReadOnlyList<OpportunityResponse> Items,
    string? NextCursor);
