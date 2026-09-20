using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates.CreateEstimateDraft;

public class CreateEstimateDraftHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IEstimateStore _store;

    public CreateEstimateDraftHandler(IRequestAccessResolver accessResolver, IEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<EstimateDetailProjection>> HandleAsync(
        CreateEstimateDraftCommand command,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "estimates.create",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<EstimateDetailProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;
        var orgId = access.OrganizationId;
        var currency = string.IsNullOrWhiteSpace(command.Currency)
            ? EstimateDefaults.DefaultCurrency
            : command.Currency.Trim();

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var canonicalPayload = $"{command.OpportunityId}|{command.SiteSurveyRevisionId}|{currency}";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        return await _store.CreateDraftAsync(
            orgId,
            command.OpportunityId,
            command.SiteSurveyRevisionId,
            currency,
            access.ActorUserId,
            keyHash,
            payloadHash,
            cancellationToken);
    }
}
