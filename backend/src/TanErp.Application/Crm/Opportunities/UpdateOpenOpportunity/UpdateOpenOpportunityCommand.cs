namespace TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity;

public sealed record UpdateOpenOpportunityCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid ExpectedVersion,
    string Title,
    Guid? PrimarySiteId,
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
