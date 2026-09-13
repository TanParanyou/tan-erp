using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Surveys.MarkSurveyReady;

public class MarkSurveyReadyHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteSurveyStore _store;

    public MarkSurveyReadyHandler(
        IRequestAccessResolver accessResolver,
        ISiteSurveyStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteSurveyRevisionProjection>> Handle(
        MarkSurveyReadyCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "surveys.mark-ready",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (!access.BranchId.HasValue)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to mark a survey revision ready."));
        }

        if (command.SiteSurveyId == Guid.Empty || command.RevisionId == Guid.Empty)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Site survey ID and Revision ID are required."));
        }

        if (command.ExpectedRevisionVersion == Guid.Empty)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Expected revision version is required."));
        }

        if (command.ExpectedOpportunityVersion == Guid.Empty)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Expected opportunity version is required."));
        }

        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = $"{command.SiteSurveyId}|{command.RevisionId}|{command.ExpectedRevisionVersion}|{command.ExpectedOpportunityVersion}";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        return await _store.MarkReadyAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
