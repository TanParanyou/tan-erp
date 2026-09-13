using TanErp.Api.Contracts.Common;

namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record OpportunityListResponse(
    IReadOnlyList<OpportunityResponse> Items,
    PaginationMetadataResponse Pagination);
