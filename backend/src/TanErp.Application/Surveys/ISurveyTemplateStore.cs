using TanErp.Application.Common.Results;
using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys;

public interface ISurveyTemplateStore
{
    Task<IReadOnlyList<SurveyTemplateVersion>> ListAsync(CancellationToken ct);

    Task<Result<SurveyTemplateVersion>> PublishAsync(PublishSurveyTemplateCommand command, Guid actorUserId, CancellationToken ct);

    Task<Result<SurveyTemplateVersion>> RetireAsync(string code, CancellationToken ct);
}

public sealed record PublishSurveyTemplateCommand(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    int MinimumAreaEvidenceCount,
    string SnapshotHashVersion,
    DateTimeOffset? EffectiveFromUtc);
