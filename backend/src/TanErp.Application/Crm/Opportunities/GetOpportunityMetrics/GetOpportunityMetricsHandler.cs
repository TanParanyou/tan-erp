using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Opportunities.GetOpportunityMetrics;

public class GetOpportunityMetricsHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public GetOpportunityMetricsHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityMetricsProjection>> Handle(
        GetOpportunityMetricsQuery query,
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
            return Result<OpportunityMetricsProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Load aggregated metrics
        var metrics = await _store.GetMetricsAsync(
            access.OrganizationId,
            access.BranchId,
            cancellationToken);

        return Result<OpportunityMetricsProjection>.Success(metrics);
    }
}
