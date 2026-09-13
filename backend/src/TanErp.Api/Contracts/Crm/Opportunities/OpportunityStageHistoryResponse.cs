namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record OpportunityStageHistoryItemResponse(
    Guid Id,
    Guid OpportunityId,
    string FromStage,
    string ToStage,
    string? ReasonCode,
    string? Note,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string PolicyVersion);

public sealed record OpportunityStageHistoryListResponse(
    IReadOnlyList<OpportunityStageHistoryItemResponse> Items);
