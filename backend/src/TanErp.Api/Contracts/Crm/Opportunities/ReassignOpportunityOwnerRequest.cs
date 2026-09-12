namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record ReassignOpportunityOwnerRequest(
    Guid TargetOwnerUserId,
    Guid ExpectedVersion);
