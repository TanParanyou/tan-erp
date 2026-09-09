namespace TanErp.Application.Crm.Opportunities.CreateOpportunity;

public sealed record CreateOpportunityCommand(
    string FirebaseUid,
    Guid MembershipId,
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
    string? NextActionNote,
    string IdempotencyKey,
    string TraceId);
