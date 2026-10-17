using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Surveys.ListSurveyTemplateVersions;

public sealed record ListSurveyTemplateVersionsQuery(string FirebaseUid, Guid MembershipId);

public sealed record SurveyTemplateVersionProjection(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    int MinimumAreaEvidenceCount,
    string SnapshotHashVersion,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    bool IsCurrent);

public class ListSurveyTemplateVersionsHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISurveyTemplateStore _store;
    private readonly IClock _clock;

    public ListSurveyTemplateVersionsHandler(IRequestAccessResolver accessResolver, ISurveyTemplateStore store, IClock clock)
    {
        _accessResolver = accessResolver;
        _store = store;
        _clock = clock;
    }

    public async Task<Result<IReadOnlyList<SurveyTemplateVersionProjection>>> Handle(
        ListSurveyTemplateVersionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "surveys.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<IReadOnlyList<SurveyTemplateVersionProjection>>.Failure(accessResult.Error);
        }

        var now = _clock.UtcNow;
        var templates = await _store.ListAsync(cancellationToken);
        var currentCode = templates.Where(t => t.IsEffectiveAt(now)).OrderByDescending(t => t.EffectiveFromUtc).FirstOrDefault()?.Code;

        IReadOnlyList<SurveyTemplateVersionProjection> items = templates
            .Select(t => new SurveyTemplateVersionProjection(
                t.Code,
                t.RequiredChecklistItems,
                t.MinimumEvidenceCount,
                t.MinimumAreaEvidenceCount,
                t.SnapshotHashVersion,
                t.EffectiveFromUtc,
                t.EffectiveToUtc,
                t.Code == currentCode))
            .ToList();

        return Result<IReadOnlyList<SurveyTemplateVersionProjection>>.Success(items);
    }
}
