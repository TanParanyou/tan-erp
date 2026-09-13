using TanErp.Api.Contracts.Crm.Sites;

namespace TanErp.Api.Contracts.Crm.Opportunities;

public sealed record OwnerSummaryResponse(
    Guid Id,
    string DisplayName,
    string? Email);

public sealed record BranchSummaryResponse(
    Guid Id,
    string Name);

public sealed record CustomerSummaryResponse(
    Guid Id,
    string Code,
    string DisplayNameTh,
    string? DisplayNameEn,
    string Status);

public sealed record OpportunityResponse(
    Guid Id,
    string Code,
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
    CustomerSummaryResponse? Customer = null,
    SiteSummaryResponse? PrimarySite = null,
    BranchSummaryResponse? Branch = null,
    OwnerSummaryResponse? Owner = null);
