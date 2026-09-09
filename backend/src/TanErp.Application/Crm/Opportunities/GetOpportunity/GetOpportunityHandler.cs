using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Opportunities.GetOpportunity;

public class GetOpportunityHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public GetOpportunityHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityProjection>> Handle(
        GetOpportunityQuery query,
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
            return Result<OpportunityProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        var opportunity = await _store.GetAsync(
            access.OrganizationId,
            query.OpportunityId,
            cancellationToken);

        if (opportunity == null)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Opportunity not found or access denied."));
        }

        return Result<OpportunityProjection>.Success(opportunity);
    }
}
