using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.CalculateEstimate;

public class CalculateEstimateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public CalculateEstimateHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<EstimateRevisionProjection>> HandleAsync(
        CalculateEstimateCommand command,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "estimates.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<EstimateRevisionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var estimate = await _store.GetByIdAsync(access.OrganizationId, command.EstimateId, cancellationToken);
        if (estimate is null || !access.HasBranchAccess(estimate.BranchId))
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{command.EstimateId}' was not found."));
        }
        try
        {
            var result = await _store.CalculateAsync(
                access.OrganizationId,
                command.EstimateId,
                command.RevisionId,
                command.ExpectedRevisionVersion,
                command.Discount,
                access.ActorUserId,
                Sha256Hex.Compute(idempotencyKey),
                Sha256Hex.Compute(FormattableString.Invariant(
                    $"{command.EstimateId:N}|{command.RevisionId:N}|{command.ExpectedRevisionVersion:N}|{command.Discount.Type}|{command.Discount.Value}|{command.Discount.ReasonCode}")),
                cancellationToken);

            return Result<EstimateRevisionProjection>.Success(result);
        }
        catch (EstimateNotFoundException)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{command.EstimateId}' was not found."));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_VERSION_CONFLICT", "The estimate draft has been modified by another user."));
        }
        catch (EstimateInvalidStateException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_INVALID_STATE", ex.Message));
        }
        catch (EstimatePolicyUnavailableException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_POLICY_UNAVAILABLE", ex.Message));
        }
        catch (EstimateDiscountReasonRequiredException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_DISCOUNT_REASON_REQUIRED", ex.Message));
        }
        catch (EstimateIdempotencyKeyReusedException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("IDEMPOTENCY_KEY_REUSED", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_INPUT_INVALID", ex.Message));
        }
    }
}
