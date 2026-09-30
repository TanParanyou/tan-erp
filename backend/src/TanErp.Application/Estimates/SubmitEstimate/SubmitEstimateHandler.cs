using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;

namespace TanErp.Application.Estimates.SubmitEstimate;

public sealed class SubmitEstimateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public SubmitEstimateHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<EstimateDetailProjection>> HandleAsync(
        SubmitEstimateCommand command, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "estimates.submit", cancellationToken);
        if (accessResult.IsFailure)
            return Result<EstimateDetailProjection>.Failure(accessResult.Error);

        var access = accessResult.Value!;
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{command.EstimateId:N}|{command.ExpectedEstimateVersion:N}|{command.RevisionNo}|{command.CalculationVersion}|{command.Note}");
        try
        {
            return await _store.SubmitAsync(access.OrganizationId, command.EstimateId, command.ExpectedEstimateVersion,
                command.RevisionNo, command.CalculationVersion, command.Note, access.ActorUserId,
                keyHash, payloadHash, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate has changed while submitting."));
        }
    }
}
