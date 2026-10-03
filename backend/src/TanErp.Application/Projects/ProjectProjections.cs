namespace TanErp.Application.Projects;

public sealed record ProjectPersonProjection(Guid Id, string DisplayName, string? Email);

public sealed record ProjectCustomerProjection(Guid Id, string Code, string DisplayNameTh, string? DisplayNameEn);

public sealed record ProjectSiteProjection(Guid Id, string Label);

public sealed record ProjectOpportunityProjection(Guid Id, string Code, string Title);

public sealed record ProjectBaselineProjection(
    Guid QuotationId,
    string QuotationNumber,
    decimal ContractAmount,
    string? QuotationSnapshotHash,
    Guid EstimateId,
    Guid EstimateRevisionId,
    Guid? SiteSurveyRevisionId,
    string? SiteSurveySnapshotHash,
    string BaselineHash);

public sealed record ProjectDetailProjection(
    Guid Id,
    Guid BranchId,
    string Code,
    string Name,
    string Status,
    DateOnly? PlannedStartDate,
    ProjectPersonProjection Owner,
    ProjectCustomerProjection Customer,
    ProjectSiteProjection? Site,
    ProjectOpportunityProjection Opportunity,
    ProjectBaselineProjection Baseline,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc);

public sealed record ProjectListItemProjection(
    Guid Id,
    string Code,
    string Name,
    string Status,
    DateOnly? PlannedStartDate,
    ProjectPersonProjection Owner,
    ProjectCustomerProjection Customer,
    decimal ContractAmount,
    DateTimeOffset CreatedAtUtc);

public sealed record ProjectListQuery(
    string? Search,
    string? Status,
    int Page,
    int PageSize);

public sealed record PagedProjectsProjection(
    IReadOnlyList<ProjectListItemProjection> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record ProjectHandoverSourceProjection(
    Guid QuotationId,
    string QuotationNumber,
    string QuotationStatus,
    Guid QuotationRowVersion,
    decimal ContractAmount,
    string OpportunityStage,
    Guid? ExistingProjectId,
    string? ExistingProjectCode);
