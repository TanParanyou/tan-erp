using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurveyChecklistResult : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteSurveyRevisionId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string Result { get; private set; } = string.Empty;
    public string? Note { get; private set; }

    protected SiteSurveyChecklistResult() { }

    public SiteSurveyChecklistResult(
        Guid id,
        Guid organizationId,
        Guid siteSurveyRevisionId,
        string itemCode,
        string result,
        string? note) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (siteSurveyRevisionId == Guid.Empty) throw new ArgumentException("Site survey revision ID cannot be empty.", nameof(siteSurveyRevisionId));
        if (string.IsNullOrWhiteSpace(itemCode)) throw new ArgumentException("Checklist item code cannot be blank.", nameof(itemCode));
        if (!ChecklistResultValue.IsValid(result)) throw new ArgumentException($"Invalid checklist result: '{result}'.", nameof(result));

        OrganizationId = organizationId;
        SiteSurveyRevisionId = siteSurveyRevisionId;
        ItemCode = itemCode.Trim();
        Result = result.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
