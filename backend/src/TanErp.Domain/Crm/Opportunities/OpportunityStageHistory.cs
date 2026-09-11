using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Opportunities;

public class OpportunityStageHistory : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid OpportunityId { get; private set; }
    public string FromStage { get; private set; } = string.Empty;
    public string ToStage { get; private set; } = string.Empty;
    public string? ReasonCode { get; private set; }
    public string? Note { get; private set; }
    public Guid ActorUserId { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public string PolicyVersion { get; private set; } = string.Empty;
    public string TraceId { get; private set; } = string.Empty;

    protected OpportunityStageHistory() { }

    public OpportunityStageHistory(
        Guid id,
        Guid organizationId,
        Guid opportunityId,
        string fromStage,
        string toStage,
        string? reasonCode,
        string? note,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string policyVersion,
        string traceId) : base(id)
    {
        if (string.IsNullOrWhiteSpace(fromStage))
            throw new ArgumentException("From stage cannot be blank.", nameof(fromStage));

        if (string.IsNullOrWhiteSpace(toStage))
            throw new ArgumentException("To stage cannot be blank.", nameof(toStage));

        if (string.IsNullOrWhiteSpace(policyVersion))
            throw new ArgumentException("Policy version cannot be blank.", nameof(policyVersion));

        if (string.IsNullOrWhiteSpace(traceId))
            throw new ArgumentException("Trace ID cannot be blank.", nameof(traceId));

        OrganizationId = organizationId;
        OpportunityId = opportunityId;
        FromStage = fromStage.Trim().ToLowerInvariant();
        ToStage = toStage.Trim().ToLowerInvariant();
        ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc;
        PolicyVersion = policyVersion.Trim();
        TraceId = traceId.Trim();
    }
}
