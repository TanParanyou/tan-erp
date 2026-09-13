using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Opportunities.ListOpportunities;

public class ListOpportunitiesHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public ListOpportunitiesHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityPage>> Handle(
        ListOpportunitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required opportunities.read permission
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "opportunities.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<OpportunityPage>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Validate cursor if supplied
        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            var decodedCursor = OpportunityCursor.TryDecode(query.Cursor);
            if (decodedCursor == null)
            {
                return Result<OpportunityPage>.Failure(
                    new Error("INVALID_CURSOR", "Provided pagination cursor is invalid."));
            }
        }

        // 3. Validate sortBy and sortOrder
        string? sortBy = null;
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            var trimmedSortBy = query.SortBy.Trim();
            if (!TanErp.Domain.Crm.Opportunities.OpportunitySortKey.IsValid(trimmedSortBy))
            {
                return Result<OpportunityPage>.Failure(
                    new Error("OPPORTUNITY_SORT_INVALID", "Invalid opportunity sort key."));
            }
            sortBy = trimmedSortBy;
        }

        string? sortOrder = null;
        if (!string.IsNullOrWhiteSpace(query.SortOrder))
        {
            var trimmedOrder = query.SortOrder.Trim();
            if (!TanErp.Domain.Crm.Opportunities.OpportunitySortOrder.IsValid(trimmedOrder))
            {
                return Result<OpportunityPage>.Failure(
                    new Error("OPPORTUNITY_SORT_ORDER_INVALID", "Invalid opportunity sort order."));
            }
            sortOrder = trimmedOrder.ToLowerInvariant();
        }

        // 4. Validate page and limit
        int? pageNumber = query.Page;
        if (pageNumber.HasValue && pageNumber.Value < 1)
        {
            pageNumber = 1;
        }

        var clampedLimit = Math.Clamp(query.Limit <= 0 ? 25 : query.Limit, 1, 100);

        var filter = new OpportunityListFilter(
            string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            query.CustomerId,
            string.IsNullOrWhiteSpace(query.Stage) ? null : query.Stage.Trim(),
            sortBy,
            sortOrder,
            pageNumber,
            clampedLimit,
            query.Cursor);

        var page = await _store.ListAsync(access.OrganizationId, filter, cancellationToken);
        return Result<OpportunityPage>.Success(page);
    }
}
