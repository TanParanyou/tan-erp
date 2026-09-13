namespace TanErp.Application.Crm.Opportunities;

public sealed record ActorSummaryProjection(
    Guid Id,
    string DisplayName);

public sealed record OpportunityStageHistoryProjection(
    Guid Id,
    Guid OpportunityId,
    string FromStage,
    string ToStage,
    string? ReasonCode,
    string? Note,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string PolicyVersion,
    string TraceId,
    ActorSummaryProjection? Actor = null);
