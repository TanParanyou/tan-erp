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

        // 3. Clamp limit between 1 and 100
        var clampedLimit = Math.Clamp(query.Limit <= 0 ? 25 : query.Limit, 1, 100);

        var filter = new OpportunityListFilter(
            query.Search,
            query.CustomerId,
            query.Stage,
            clampedLimit,
            query.Cursor);

        var page = await _store.ListAsync(access.OrganizationId, filter, cancellationToken);
        return Result<OpportunityPage>.Success(page);
    }
}
