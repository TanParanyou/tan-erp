namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record TransitionOpportunityStageRequest(
    string TargetStage,
    Guid ExpectedVersion);
