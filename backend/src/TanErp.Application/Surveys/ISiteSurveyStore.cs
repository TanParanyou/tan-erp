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
}

