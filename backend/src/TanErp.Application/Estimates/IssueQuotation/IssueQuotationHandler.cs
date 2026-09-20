using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.IssueQuotation;

public class IssueQuotationHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public IssueQuotationHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<QuotationDetailProjection>> HandleAsync(
        IssueQuotationCommand command,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "quotations.issue",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<QuotationDetailProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        try
        {
            var result = await _store.IssueQuotationAsync(
                access.OrganizationId,
                command.EstimateId,
                command.ExpectedEstimateVersion,
                command.ExpectedOpportunityVersion,
                access.ActorUserId,
                idempotencyKey,
                command.TraceId,
                cancellationToken);

            return Result<QuotationDetailProjection>.Success(result);
        }
        catch (EstimateNotFoundException)
        {
            return Result<QuotationDetailProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate '{command.EstimateId}' was not found."));
        }
        catch (OpportunityVersionException)
        {
            return Result<QuotationDetailProjection>.Failure(
                new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
        }
        catch (OpportunityTransitionException ex)
        {
            return Result<QuotationDetailProjection>.Failure(
                new Error("OPPORTUNITY_INVALID_TRANSITION", ex.Message));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<QuotationDetailProjection>.Failure(
                new Error("ESTIMATE_VERSION_CONFLICT", "The estimate has been modified by another user."));
        }
        catch (EstimateInvalidStateException ex)
        {
            return Result<QuotationDetailProjection>.Failure(
                new Error("ESTIMATE_INVALID_STATE", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result<QuotationDetailProjection>.Failure(
                new Error("INVALID_STATE", ex.Message));
        }
    }
}
