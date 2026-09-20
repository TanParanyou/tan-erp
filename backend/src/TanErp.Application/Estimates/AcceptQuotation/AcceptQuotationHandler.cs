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
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var canonicalPayload = $"{command.EstimateId}|{command.ExpectedOpportunityVersion}";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        return await _store.AcceptQuotationAsync(
            access.OrganizationId,
            command.EstimateId,
            command.ExpectedOpportunityVersion,
            command.DecisionNote,
            access.ActorUserId,
            keyHash,
            payloadHash,
            command.TraceId,
            cancellationToken);
    }
}
