namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record CreateOpportunityRequest(
    Guid CustomerId,
    Guid? PrimarySiteId,
    string Title,
    string? ScopeSummary,
    IReadOnlyList<string> WorkTypes,
    string? SourceCode,
    decimal? ExpectedBudget,
    string? CurrencyCode,
    DateOnly? TargetDecisionDate,
    DateTimeOffset? NextActionAtUtc,
    string? NextActionNote);
