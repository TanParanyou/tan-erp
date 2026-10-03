using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

public class ProjectMilestone : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly? PlannedDate { get; private set; }
    public int Weight { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public Guid? CompletedByUserId { get; private set; }
    public Guid RowVersion { get; private set; }

    protected ProjectMilestone() { }

    public ProjectMilestone(Guid id, Guid organizationId, Guid projectId, string name, DateOnly? plannedDate, int weight, int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
        Apply(name, plannedDate, weight);
        OrganizationId = organizationId;
        ProjectId = projectId;
        SortOrder = sortOrder;
        RowVersion = Guid.NewGuid();
    }

    public void Update(string name, DateOnly? plannedDate, int weight)
    {
        if (CompletedAtUtc.HasValue)
        {
            throw new ProjectDomainException("PROJECT_MILESTONE_COMPLETED", "A completed milestone cannot be edited.");
        }

        Apply(name, plannedDate, weight);
        RowVersion = Guid.NewGuid();
    }

    public void Complete(Guid actorUserId, DateTimeOffset now)
    {
        if (CompletedAtUtc.HasValue)
        {
            throw new ProjectDomainException("PROJECT_MILESTONE_COMPLETED", "The milestone is already completed.");
        }

        CompletedAtUtc = now.ToUniversalTime();
        CompletedByUserId = actorUserId;
        RowVersion = Guid.NewGuid();
    }

    private void Apply(string name, DateOnly? plannedDate, int weight)
    {
        var trimmed = name?.Trim() ?? string.Empty;
        if (trimmed.Length == 0 || trimmed.Length > 200)
        {
            throw new ProjectDomainException("PROJECT_MILESTONE_INVALID", "Milestone name is required and cannot exceed 200 characters.");
        }

        if (weight is < 1 or > 1000)
        {
            throw new ProjectDomainException("PROJECT_MILESTONE_INVALID", "Milestone weight must be between 1 and 1000.");
        }

        Name = trimmed;
        PlannedDate = plannedDate;
        Weight = weight;
    }
}
