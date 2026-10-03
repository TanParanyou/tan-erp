namespace TanErp.Api.Contracts.Surveys;

public sealed record SurveyTemplateVersionResponse(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    string SnapshotHashVersion,
    bool IsCurrent);

public sealed record SurveyTemplateVersionListResponse(
    IReadOnlyList<SurveyTemplateVersionResponse> Items);
