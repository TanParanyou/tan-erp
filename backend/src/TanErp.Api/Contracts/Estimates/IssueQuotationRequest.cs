using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Estimates;

public sealed record IssueQuotationRequest(
    [Required] Guid ExpectedEstimateVersion,
    [Required] Guid ExpectedOpportunityVersion);

public sealed record QuotationResponse(
    Guid QuotationId,
    Guid EstimateId,
    Guid OpportunityId,
    string Number,
    string Status,
    decimal GrandTotal,
    DateTimeOffset IssuedAtUtc,
    Guid EstimateRevisionId,
    int RevisionNo,
    string OpportunityStage,
    Guid OpportunityRowVersion,
    Guid EstimateRowVersion);
