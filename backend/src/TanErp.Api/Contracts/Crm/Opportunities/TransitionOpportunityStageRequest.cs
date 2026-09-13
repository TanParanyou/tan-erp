namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record TransitionOpportunityStageRequest(
    string TargetStage,
    Guid ExpectedVersion,
    string? ReasonCode = null,
    string? Note = null);
