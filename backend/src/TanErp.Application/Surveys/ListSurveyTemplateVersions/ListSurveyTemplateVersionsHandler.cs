using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.Surveys;

namespace TanErp.Application.Surveys.ListSurveyTemplateVersions;

public sealed record ListSurveyTemplateVersionsQuery(string FirebaseUid, Guid MembershipId);

public sealed record SurveyTemplateVersionProjection(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    string SnapshotHashVersion,
    bool IsCurrent);

public class ListSurveyTemplateVersionsHandler
{
    private readonly IRequestAccessResolver _accessResolver;

    public ListSurveyTemplateVersionsHandler(IRequestAccessResolver accessResolver)
    {
        _accessResolver = accessResolver;
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

        IReadOnlyList<SurveyTemplateVersionProjection> items = SurveyTemplates.All
            .Select(t => new SurveyTemplateVersionProjection(
                t.Code,
                t.RequiredChecklistItems,
                t.MinimumEvidenceCount,
                t.SnapshotHashVersion,
                t.Code == SurveyDefaults.CurrentTemplateVersion))
            .ToList();

        return Result<IReadOnlyList<SurveyTemplateVersionProjection>>.Success(items);
    }
}
