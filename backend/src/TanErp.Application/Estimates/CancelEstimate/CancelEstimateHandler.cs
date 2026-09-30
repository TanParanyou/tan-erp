using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Estimates.CancelEstimate;

public sealed class CancelEstimateHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
{
    public async Task<Result<EstimateDetailProjection>> HandleAsync(
        CancelEstimateCommand command, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        var accessResult = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "estimates.cancel", cancellationToken);
        if (accessResult.IsFailure)
            return Result<EstimateDetailProjection>.Failure(accessResult.Error);

        var access = accessResult.Value!;
        try
        {
            return await store.CancelAsync(access.OrganizationId, command.EstimateId,
                command.ExpectedEstimateVersion, command.Reason, access.ActorUserId, command.MembershipId,
                Sha256Hex.Compute(idempotencyKey),
                Sha256Hex.Compute($"{command.EstimateId:N}|{command.ExpectedEstimateVersion:N}|{command.Reason.Trim()}"),
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate version changed while cancelling."));
        }
    }
}
