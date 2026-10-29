using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Surveys.CreateSiteSurvey;

namespace TanErp.Application.Surveys;

public interface ISiteSurveyStore
{
    Task<Result<SiteSurveyProjection>> CreateAsync(
        RequestAccessContext access,
        CreateSiteSurveyCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<SiteSurveyProjection?> GetByOpportunityIdAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default);

    Task<AssignedSurveySyncListProjection> ListAssignedSurveysAsync(
        RequestAccessContext access,
        DateTimeOffset? updatedSinceUtc,
        bool onlyMine,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(new AssignedSurveySyncListProjection(Array.Empty<AssignedSurveySyncItemProjection>(), DateTimeOffset.UtcNow));

    Task<Result<SiteSurveyRevisionProjection>> UpdateDraftAsync(
        RequestAccessContext access,
        UpdateSurveyDraft.UpdateSurveyDraftCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<SiteSurveyRevisionProjection>> MarkReadyAsync(
        RequestAccessContext access,
        MarkSurveyReady.MarkSurveyReadyCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<SiteSurveyRevisionProjection>> CloneRevisionAsync(
        RequestAccessContext access,
        CloneSurveyRevision.CloneSurveyRevisionCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);

    Task<Result<SiteSurveyRevisionProjection>> VoidRevisionAsync(
        RequestAccessContext access,
        VoidSurveyRevision.VoidSurveyRevisionCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default);
}
