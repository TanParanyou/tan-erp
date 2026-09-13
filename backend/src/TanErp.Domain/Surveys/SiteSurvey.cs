using TanErp.Domain.Common;

namespace TanErp.Domain.Surveys;

public class SiteSurvey : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid OpportunityId { get; private set; }
    public Guid SiteId { get; private set; }
    public string SurveyNumber { get; private set; } = string.Empty;
    public Guid AssignedSurveyorId { get; private set; }
    public DateTimeOffset? ScheduledStartUtc { get; private set; }
    public DateTimeOffset? ScheduledEndUtc { get; private set; }
    public string Status { get; private set; } = SiteSurveyStatus.Scheduled;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private readonly List<SiteSurveyRevision> _revisions = new();
    public IReadOnlyCollection<SiteSurveyRevision> Revisions => _revisions.AsReadOnly();

    protected SiteSurvey() { }

    private SiteSurvey(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid opportunityId,
        Guid siteId,
        Guid assignedSurveyorId,
        Guid createdByUserId,
        DateTimeOffset? scheduledStartUtc,
        DateTimeOffset? scheduledEndUtc,
        DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (opportunityId == Guid.Empty) throw new ArgumentException("Opportunity ID cannot be empty.", nameof(opportunityId));
        if (siteId == Guid.Empty) throw new ArgumentException("Site ID cannot be empty.", nameof(siteId));
        if (assignedSurveyorId == Guid.Empty) throw new ArgumentException("Assigned surveyor ID cannot be empty.", nameof(assignedSurveyorId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));

        if (scheduledStartUtc.HasValue && scheduledEndUtc.HasValue && scheduledEndUtc.Value <= scheduledStartUtc.Value)
        {
            throw new ArgumentException("Scheduled end time must be strictly after scheduled start time.", nameof(scheduledEndUtc));
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        OpportunityId = opportunityId;
        SiteId = siteId;
        SurveyNumber = GenerateSurveyNumber(id, now);
        AssignedSurveyorId = assignedSurveyorId;
        ScheduledStartUtc = scheduledStartUtc?.ToUniversalTime();
        ScheduledEndUtc = scheduledEndUtc?.ToUniversalTime();
        Status = SiteSurveyStatus.Scheduled;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now.ToUniversalTime();
        CreatedByUserId = createdByUserId;
    }

    public static SiteSurvey CreateAppointment(
        Guid organizationId,
        Guid branchId,
        Guid opportunityId,
        Guid siteId,
        Guid assignedSurveyorId,
        Guid createdByUserId,
        DateTimeOffset? scheduledStartUtc,
        DateTimeOffset? scheduledEndUtc,
        DateTimeOffset now)
    {
        return new SiteSurvey(
            Guid.NewGuid(),
            organizationId,
            branchId,
            opportunityId,
            siteId,
            assignedSurveyorId,
            createdByUserId,
            scheduledStartUtc,
            scheduledEndUtc,
            now);
    }

    public static string GenerateSurveyNumber(Guid id, DateTimeOffset now)
    {
        var yearMonth = now.ToString("yyyyMM");
        var hex = id.ToString("N")[..6].ToUpperInvariant();
        return $"SRV-{yearMonth}-{hex}";
    }
}
