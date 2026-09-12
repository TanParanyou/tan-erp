namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record UpdateDraftQGateRequest(
    string? ScopeSummary,
    IReadOnlyList<string> WorkTypes,
    DateTimeOffset? NextActionAtUtc,
    string? NextActionNote);
