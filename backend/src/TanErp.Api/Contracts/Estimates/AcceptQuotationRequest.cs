namespace TanErp.Api.Contracts.Estimates;

public sealed record AcceptQuotationRequest(
    Guid ExpectedOpportunityVersion,
    string? DecisionNote);

public sealed record AcceptQuotationResponse(
    Guid QuotationId,
    Guid OpportunityId,
    string OpportunityStage,
    Guid OpportunityRowVersion,
    DateTimeOffset AcceptedAtUtc);
