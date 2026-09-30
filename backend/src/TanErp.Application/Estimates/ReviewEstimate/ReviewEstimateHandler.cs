using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;

namespace TanErp.Application.Estimates.ReviewEstimate;

public sealed class ReviewEstimateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public ReviewEstimateHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<EstimateDetailProjection>> HandleAsync(
        ReviewEstimateCommand command, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "estimates.approve", cancellationToken);
        if (accessResult.IsFailure)
            return Result<EstimateDetailProjection>.Failure(accessResult.Error);

        var access = accessResult.Value!;
        var estimate = await _store.GetByIdAsync(access.OrganizationId, command.EstimateId, cancellationToken);
        if (estimate is null || !access.HasBranchAccess(estimate.BranchId))
            return Result<EstimateDetailProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{command.EstimateId}' was not found."));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{command.EstimateId:N}|{command.ExpectedEstimateVersion:N}|{command.RevisionNo}|{command.Decision}|{command.ReasonCode}|{command.Note}");
        try
        {
            return await _store.ReviewAsync(access.OrganizationId, command.EstimateId, command.ExpectedEstimateVersion,
                command.RevisionNo, command.Decision, command.ReasonCode, command.Note, access.ActorUserId,
                command.MembershipId, keyHash, payloadHash, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate has changed while being reviewed."));
        }
    }
}
