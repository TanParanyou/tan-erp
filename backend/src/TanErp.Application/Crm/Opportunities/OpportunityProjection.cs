namespace TanErp.Application.Crm.Opportunities;

public sealed record OwnerSummaryProjection(
    Guid Id,
    string DisplayName,
    string? Email);

public sealed record BranchSummaryProjection(
    Guid Id,
    string Name);

public sealed record CustomerSummaryProjection(
    Guid Id,
    string Code,
    string DisplayNameTh,
    string? DisplayNameEn,
    string Status);

public sealed record SiteSummaryProjection(
    Guid Id,
    string Label,
    string AddressLine1,
    string Subdistrict,
    string District,
    string Province,
    string PostalCode);

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
    DateTimeOffset CreatedAtUtc,
    OwnerSummaryProjection? Owner = null,
    BranchSummaryProjection? Branch = null,
    CustomerSummaryProjection? Customer = null,
    SiteSummaryProjection? PrimarySite = null);
