namespace TanErp.Domain.Surveys;

/// <summary>
/// System-owned, immutable survey template version. Changes ship as a new version through a reviewed
/// code change; published versions never change so Ready revisions that reference them stay valid.
/// </summary>
public sealed record SurveyTemplateVersion(
    string Code,
    IReadOnlyList<string> RequiredChecklistItems,
    int MinimumEvidenceCount,
    string SnapshotHashVersion);

public static class SurveyChecklistItems
{
    public const string SiteAccessConfirmed = "site_access_confirmed";
    public const string UtilitiesChecked = "utilities_checked";
    public const string ExistingConditionsInspected = "existing_conditions_inspected";
    public const string CustomerRequirementsConfirmed = "customer_requirements_confirmed";
}

public static class SurveyTemplates
{
    public static readonly SurveyTemplateVersion BaselineV1 = new(
        SurveyDefaults.BaselineTemplateVersion,
        Array.Empty<string>(),
        MinimumEvidenceCount: 0,
        SnapshotHashVersion: "v2");

    public static readonly SurveyTemplateVersion BaselineV2 = new(
        SurveyDefaults.ChecklistTemplateVersion,
        new[]
        {
            SurveyChecklistItems.SiteAccessConfirmed,
            SurveyChecklistItems.UtilitiesChecked,
            SurveyChecklistItems.ExistingConditionsInspected,
            SurveyChecklistItems.CustomerRequirementsConfirmed
        },
        MinimumEvidenceCount: 1,
        SnapshotHashVersion: "v3");

    public static IReadOnlyList<SurveyTemplateVersion> All { get; } = new[] { BaselineV1, BaselineV2 };

    public static SurveyTemplateVersion? Find(string? code) =>
        All.FirstOrDefault(t => string.Equals(t.Code, code?.Trim(), StringComparison.Ordinal));
}
