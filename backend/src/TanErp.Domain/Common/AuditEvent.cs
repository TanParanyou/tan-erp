namespace TanErp.Domain.Common;

public class AuditEvent : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ActorUserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string ResourceType { get; private set; } = string.Empty;
    public string ResourceId { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public string TraceId { get; private set; } = string.Empty;
    public string ChangesJson { get; private set; } = "{}";

    protected AuditEvent() { }

    public AuditEvent(
        Guid id,
        Guid organizationId,
        Guid actorUserId,
        string action,
        string resourceType,
        string resourceId,
        DateTimeOffset occurredAtUtc,
        string traceId,
        string changesJson = "{}") : base(id)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be blank.", nameof(action));
        if (string.IsNullOrWhiteSpace(resourceType))
            throw new ArgumentException("ResourceType cannot be blank.", nameof(resourceType));

        OrganizationId = organizationId;
        ActorUserId = actorUserId;
        Action = action.Trim();
        ResourceType = resourceType.Trim();
        ResourceId = resourceId?.Trim() ?? string.Empty;
        OccurredAtUtc = occurredAtUtc;
        TraceId = traceId?.Trim() ?? string.Empty;
        ChangesJson = string.IsNullOrWhiteSpace(changesJson) ? "{}" : changesJson;
    }
}
