namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record OwnerSummaryResponse(
    Guid Id,
    string DisplayName,
    string? Email);

public sealed record OpportunityResponse(
    Guid Id,
    string Code,
    Guid CustomerId,
    Guid? PrimarySiteId,
    Guid BranchId,
    Guid OwnerUserId,
    string Title,
    string? ScopeSummary,
    IReadOnlyList<string> WorkTypes,
    string? SourceCode,
    decimal? ExpectedBudget,
    string? CurrencyCode,
    DateOnly? TargetDecisionDate,
    DateTimeOffset? NextActionAtUtc,
    string? NextActionNote,
    string Stage,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    OwnerSummaryResponse? Owner = null);
