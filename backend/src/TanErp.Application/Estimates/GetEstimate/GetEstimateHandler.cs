using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;

namespace TanErp.Application.Estimates.GetEstimate;

public class GetEstimateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public GetEstimateHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<EstimateDetailProjection>> HandleByIdAsync(
        GetEstimateQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "estimates.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<EstimateDetailProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var estimate = await _store.GetByIdAsync(access.OrganizationId, query.EstimateId, cancellationToken);
        if (estimate is null || !access.HasBranchAccess(estimate.BranchId))
        {
            return Result<EstimateDetailProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{query.EstimateId}' was not found."));
        }

        return Result<EstimateDetailProjection>.Success(estimate);
    }

    public async Task<Result<IReadOnlyList<EstimateCalculationSnapshotProjection>>> HandleCalculationSnapshotsAsync(
        GetEstimateQuery query,
        Guid revisionId,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "estimates.read",
            cancellationToken);

        if (accessResult.IsFailure)
            return Result<IReadOnlyList<EstimateCalculationSnapshotProjection>>.Failure(accessResult.Error);

        var access = accessResult.Value!;
        var estimate = await _store.GetByIdAsync(access.OrganizationId, query.EstimateId, cancellationToken);
        if (estimate is null || !access.HasBranchAccess(estimate.BranchId))
            return Result<IReadOnlyList<EstimateCalculationSnapshotProjection>>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{query.EstimateId}' was not found."));
        var snapshots = await _store.GetCalculationSnapshotsAsync(
            access.OrganizationId, query.EstimateId, revisionId, cancellationToken);
        return Result<IReadOnlyList<EstimateCalculationSnapshotProjection>>.Success(snapshots);
    }

    public async Task<Result<EstimateDetailProjection?>> HandleByOpportunityIdAsync(
        GetOpportunityEstimateQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "estimates.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<EstimateDetailProjection?>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var estimate = await _store.GetByOpportunityIdAsync(access.OrganizationId, query.OpportunityId, cancellationToken);
        if (estimate is not null && !access.HasBranchAccess(estimate.BranchId))
            return Result<EstimateDetailProjection?>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate for opportunity '{query.OpportunityId}' was not found."));
        return Result<EstimateDetailProjection?>.Success(estimate);
    }
}
