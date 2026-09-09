namespace TanErp.Application.Crm.Opportunities;

public sealed record OpportunityProjection(
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
    DateTimeOffset CreatedAtUtc);
