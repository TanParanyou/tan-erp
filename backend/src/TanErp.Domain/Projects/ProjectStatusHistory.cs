using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>Append-only record of project status transitions.</summary>
public class ProjectStatusHistory : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string FromStatus { get; private set; } = string.Empty;
    public string ToStatus { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public Guid ActorUserId { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }

    protected ProjectStatusHistory() { }

    public ProjectStatusHistory(Guid id, Guid organizationId, Guid projectId, string fromStatus, string toStatus, string? reason, Guid actorUserId, DateTimeOffset occurredAtUtc) : base(id)
    {
        OrganizationId = organizationId;
        ProjectId = projectId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc.ToUniversalTime();
    }
}
