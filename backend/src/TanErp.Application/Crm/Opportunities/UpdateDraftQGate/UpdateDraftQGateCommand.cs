namespace TanErp.Application.Crm.Opportunities.UpdateDraftQGate;

public sealed record UpdateDraftQGateCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid OpportunityId,
    Guid ExpectedVersion,
    string? ScopeSummary,
    IReadOnlyList<string> WorkTypes,
    DateTimeOffset? NextActionAtUtc,
    string? NextActionNote,
    string IdempotencyKey,
    string TraceId);
