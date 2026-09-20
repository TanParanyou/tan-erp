using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.AcceptQuotation;

public class AcceptQuotationHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public AcceptQuotationHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<AcceptQuotationProjection>> HandleAsync(
        AcceptQuotationCommand command,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "quotations.accept",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<AcceptQuotationProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        try
        {
            var result = await _store.AcceptQuotationAsync(
                access.OrganizationId,
                command.EstimateId,
                command.ExpectedOpportunityVersion,
                command.DecisionNote,
                access.ActorUserId,
                idempotencyKey,
                command.TraceId,
                cancellationToken);

            return Result<AcceptQuotationProjection>.Success(result);
        }
        catch (EstimateNotFoundException)
        {
            return Result<AcceptQuotationProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{command.EstimateId}' was not found."));
        }
        catch (OpportunityVersionException)
        {
            return Result<AcceptQuotationProjection>.Failure(
                new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
        }
        catch (OpportunityTransitionException ex)
        {
            return Result<AcceptQuotationProjection>.Failure(
                new Error("OPPORTUNITY_INVALID_TRANSITION", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result<AcceptQuotationProjection>.Failure(
                new Error("INVALID_STATE", ex.Message));
        }
    }
}
