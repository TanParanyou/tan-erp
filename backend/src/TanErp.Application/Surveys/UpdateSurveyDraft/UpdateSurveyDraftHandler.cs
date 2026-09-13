using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Surveys.UpdateSurveyDraft;

public class UpdateSurveyDraftHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteSurveyStore _store;

    public UpdateSurveyDraftHandler(
        IRequestAccessResolver accessResolver,
        ISiteSurveyStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteSurveyRevisionProjection>> Handle(
        UpdateSurveyDraftCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "surveys.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (!access.BranchId.HasValue)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to update a survey draft."));
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

        return await _store.UpdateDraftAsync(access, command, cancellationToken);
    }
}
