using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.UpdateEstimateDraft;

public class UpdateEstimateDraftHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public UpdateEstimateDraftHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<EstimateRevisionProjection>> HandleAsync(
        UpdateEstimateDraftCommand command,
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
        var fixedPriceItems = command.Sections
            .SelectMany(section => section.WorkItems)
            .Where(workItem => workItem.SellingRuleType == SellingRuleType.FixedPrice)
            .ToArray();
        if (fixedPriceItems.Any(workItem => string.IsNullOrWhiteSpace(workItem.SellingRuleReasonCode)))
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_FIXED_PRICE_REASON_REQUIRED", "A reason code is required when using a fixed selling price."));
        }

        if (fixedPriceItems.Length > 0)
        {
            var overrideAccessResult = await _accessResolver.ResolveBranchAccessAsync(
                command.FirebaseUid,
                command.MembershipId,
                "estimates.override-price",
                estimate.BranchId,
                cancellationToken);
            if (overrideAccessResult.IsFailure)
            {
                return Result<EstimateRevisionProjection>.Failure(overrideAccessResult.Error);
            }
        }

        try
        {
            var result = await _store.UpdateDraftAsync(
                access.OrganizationId,
                command.EstimateId,
                command.RevisionId,
                command.ExpectedRevisionVersion,
                command.Sections,
                access.ActorUserId,
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
        catch (EstimateFixedPriceReasonRequiredException)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_FIXED_PRICE_REASON_REQUIRED", "A reason code is required when using a fixed selling price."));
        }
        catch (ItemCostConflictException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error(ex.Code, ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result<EstimateRevisionProjection>.Failure(
                new Error("ESTIMATE_INPUT_INVALID", ex.Message));
        }
    }
}
