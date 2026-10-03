namespace TanErp.Api.Contracts.Projects;

public sealed record CreateProjectFromHandoverRequest(
    Guid QuotationId,
    Guid ExpectedQuotationVersion,
    Guid OwnerUserId,
    DateOnly? PlannedStartDate,
    string? Name);

public sealed record ProjectPersonResponse(Guid Id, string DisplayName, string? Email);

public sealed record ProjectCustomerResponse(Guid Id, string Code, string DisplayNameTh, string? DisplayNameEn);

public sealed record ProjectSiteResponse(Guid Id, string Label);

public sealed record ProjectOpportunityResponse(Guid Id, string Code, string Title);

public sealed record ProjectBaselineResponse(
    Guid QuotationId,
    string QuotationNumber,
    decimal ContractAmount,
    string? QuotationSnapshotHash,
    Guid EstimateId,
    Guid EstimateRevisionId,
    Guid? SiteSurveyRevisionId,
    string? SiteSurveySnapshotHash,
    string BaselineHash);

public sealed record ProjectResponse(
    Guid Id,
    Guid BranchId,
    string Code,
    string Name,
    string Status,
    DateOnly? PlannedStartDate,
    ProjectPersonResponse Owner,
    ProjectCustomerResponse Customer,
    ProjectSiteResponse? Site,
    ProjectOpportunityResponse Opportunity,
    ProjectBaselineResponse Baseline,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc);

public sealed record ProjectListItemResponse(
    Guid Id,
    string Code,
    string Name,
    string Status,
    DateOnly? PlannedStartDate,
    ProjectPersonResponse Owner,
    ProjectCustomerResponse Customer,
    decimal ContractAmount,
    DateTimeOffset CreatedAtUtc);

public sealed record ProjectPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record ProjectListResponse(
    IReadOnlyList<ProjectListItemResponse> Items,
    ProjectPaginationResponse Pagination);

public sealed record ProjectHandoverSourceResponse(
    Guid QuotationId,
    string QuotationNumber,
    string QuotationStatus,
    Guid QuotationRowVersion,
    decimal ContractAmount,
    string OpportunityStage,
    Guid? ExistingProjectId,
    string? ExistingProjectCode);
