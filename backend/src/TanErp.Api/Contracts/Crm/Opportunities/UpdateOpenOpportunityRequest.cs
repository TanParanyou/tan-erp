namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record UpdateOpenOpportunityRequest(
    string Title,
    Guid? PrimarySiteId,
    string? ScopeSummary,
    IReadOnlyList<string> WorkTypes,
    string? SourceCode,
    decimal? ExpectedBudget,
    string? CurrencyCode,
    DateOnly? TargetDecisionDate,
    DateTimeOffset? NextActionAtUtc,
    string? NextActionNote);
