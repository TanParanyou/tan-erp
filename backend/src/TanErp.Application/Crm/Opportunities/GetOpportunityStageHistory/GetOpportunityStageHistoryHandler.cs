using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Opportunities.GetOpportunityStageHistory;

public class GetOpportunityStageHistoryHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public GetOpportunityStageHistoryHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<IReadOnlyList<OpportunityStageHistoryProjection>>> Handle(
        GetOpportunityStageHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve access with opportunities.read permission
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "opportunities.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<IReadOnlyList<OpportunityStageHistoryProjection>>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var orgId = access.OrganizationId;

        // 2. Check if opportunity exists in this organization scope
        var opp = await _store.GetAsync(orgId, query.OpportunityId, cancellationToken);
        if (opp == null)
        {
            return Result<IReadOnlyList<OpportunityStageHistoryProjection>>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
        }

        // 3. Fetch stage history
        var items = await _store.GetStageHistoryAsync(orgId, query.OpportunityId, cancellationToken);
        return Result<IReadOnlyList<OpportunityStageHistoryProjection>>.Success(items);
    }
}
