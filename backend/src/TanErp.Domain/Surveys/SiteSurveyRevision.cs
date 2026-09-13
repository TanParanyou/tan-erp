using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurveyRevision : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid SiteSurveyId { get; private set; }
    public int RevisionNumber { get; private set; }
    public string SurveyTemplateVersion { get; private set; } = string.Empty;
    public DateTimeOffset? VisitedAtUtc { get; private set; }
    public string? ScopeSummary { get; private set; }
    public IReadOnlyCollection<string> Assumptions { get; private set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Constraints { get; private set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> MissingDetails { get; private set; } = Array.Empty<string>();
    public string Readiness { get; private set; } = SurveyReadiness.Incomplete;
    public string Status { get; private set; } = SurveyRevisionStatus.Draft;
    public DateTimeOffset? ReadyAtUtc { get; private set; }
    public Guid? ReadyByUserId { get; private set; }
    public string? SnapshotHash { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    protected SiteSurveyRevision() { }

    private SiteSurveyRevision(
        Guid id,
        Guid organizationId,
        Guid siteSurveyId,
        int revisionNumber,
        string surveyTemplateVersion,
        Guid createdByUserId,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (siteSurveyId == Guid.Empty) throw new ArgumentException("Site survey ID cannot be empty.", nameof(siteSurveyId));
        if (revisionNumber <= 0) throw new ArgumentException("Revision number must be positive.", nameof(revisionNumber));
        if (string.IsNullOrWhiteSpace(surveyTemplateVersion)) throw new ArgumentException("Survey template version cannot be blank.", nameof(surveyTemplateVersion));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));

        OrganizationId = organizationId;
        SiteSurveyId = siteSurveyId;
        RevisionNumber = revisionNumber;
        SurveyTemplateVersion = surveyTemplateVersion.Trim();
        Readiness = SurveyReadiness.Incomplete;
        Status = SurveyRevisionStatus.Draft;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now.ToUniversalTime();
        CreatedByUserId = createdByUserId;
    }

    public static SiteSurveyRevision CreateBaseline(
        Guid organizationId,
        Guid siteSurveyId,
        Guid createdByUserId,
        DateTimeOffset now,
        string surveyTemplateVersion = SurveyDefaults.BaselineTemplateVersion)
    {
        return new SiteSurveyRevision(
            Guid.NewGuid(),
            organizationId,
            siteSurveyId,
            revisionNumber: 1,
            surveyTemplateVersion: surveyTemplateVersion,
            createdByUserId: createdByUserId,
            now: now);
    }
}
