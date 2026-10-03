using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Surveys.CloneSurveyRevision;

public class CloneSurveyRevisionHandler
{
    public const int MaxReasonLength = 500;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteSurveyStore _store;

    public CloneSurveyRevisionHandler(
        IRequestAccessResolver accessResolver,
        ISiteSurveyStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteSurveyRevisionProjection>> Handle(
        CloneSurveyRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "surveys.create-revision",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        if (!access.BranchId.HasValue)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to clone a survey revision."));
        }

        if (command.OpportunityId == Guid.Empty || command.SiteSurveyId == Guid.Empty || command.SourceRevisionId == Guid.Empty)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Opportunity ID, site survey ID and source revision ID are required."));
        }

        var reason = command.Reason?.Trim();
        if (string.IsNullOrEmpty(reason) || reason.Length > MaxReasonLength)
        {
            return Result<SiteSurveyRevisionProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", $"A reason of 1-{MaxReasonLength} characters is required."));
        }

        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{command.OpportunityId}|{command.SiteSurveyId}|{command.SourceRevisionId}|{reason}");

        return await _store.CloneRevisionAsync(access, command with { Reason = reason }, keyHash, payloadHash, cancellationToken);
    }
}
