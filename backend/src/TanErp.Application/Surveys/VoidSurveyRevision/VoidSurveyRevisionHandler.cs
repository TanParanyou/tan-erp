using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Surveys.CloneSurveyRevision;

namespace TanErp.Application.Surveys.VoidSurveyRevision;

public class VoidSurveyRevisionHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteSurveyStore _store;

    public VoidSurveyRevisionHandler(
        IRequestAccessResolver accessResolver,
        ISiteSurveyStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteSurveyRevisionProjection>> Handle(
        VoidSurveyRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "surveys.void",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (!access.BranchId.HasValue)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to void a survey revision."));
        }

        if (command.OpportunityId == Guid.Empty || command.SiteSurveyId == Guid.Empty || command.RevisionId == Guid.Empty)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Opportunity ID, site survey ID and revision ID are required."));
        }

        if (command.ExpectedRevisionVersion == Guid.Empty)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Expected revision version is required."));
        }

        var reason = command.Reason?.Trim();
        if (string.IsNullOrEmpty(reason) || reason.Length > CloneSurveyRevisionHandler.MaxReasonLength)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", $"A reason of 1-{CloneSurveyRevisionHandler.MaxReasonLength} characters is required."));
        }

        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{command.OpportunityId}|{command.SiteSurveyId}|{command.RevisionId}|{command.ExpectedRevisionVersion}|{reason}");

        return await _store.VoidRevisionAsync(access, command with { Reason = reason }, keyHash, payloadHash, cancellationToken);
    }
}
