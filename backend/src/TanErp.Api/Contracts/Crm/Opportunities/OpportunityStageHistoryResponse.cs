namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record ActorSummaryResponse(
    Guid Id,
    string DisplayName);

public sealed record OpportunityStageHistoryItemResponse(
    Guid Id,
    Guid OpportunityId,
    string FromStage,
    string ToStage,
    string? ReasonCode,
    string? Note,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string PolicyVersion,
    ActorSummaryResponse? Actor = null);

public sealed record OpportunityStageHistoryListResponse(
    IReadOnlyList<OpportunityStageHistoryItemResponse> Items);
