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

public sealed record SetProjectPlanRequest(DateOnly? PlannedStartDate, DateOnly? PlannedEndDate);

public sealed record ProjectBudgetLineRequest(string Category, string Description, decimal Amount);

public sealed record ReplaceProjectBudgetRequest(List<ProjectBudgetLineRequest>? Lines);

public sealed record AddProjectMilestoneRequest(string Name, DateOnly? PlannedDate, int Weight);

public sealed record UpdateProjectMilestoneRequest(string Name, DateOnly? PlannedDate, int Weight, Guid ExpectedVersion);

public sealed record ProjectMilestoneVersionRequest(Guid ExpectedVersion);

public sealed record TransitionProjectRequest(string TargetStatus, string? Reason);

public sealed record CreateProjectChangeOrderRequest(string Title, string Reason, decimal BudgetDelta, decimal ContractDelta);

public sealed record ProjectChangeOrderActionRequest(Guid ExpectedVersion, string? Note);

public sealed record ProjectBudgetLineResponse(Guid Id, string Category, string Description, decimal Amount, int SortOrder);

public sealed record ProjectBudgetResponse(
    decimal? BaselineTotal,
    string? BaselineHash,
    bool IsFrozen,
    DateTimeOffset? FrozenAtUtc,
    decimal ApprovedBudgetDelta,
    decimal? CurrentTotal,
    decimal CommittedAmount,
    decimal? AvailableBudget,
    IReadOnlyList<ProjectBudgetLineResponse> Lines);

public sealed record ProjectMilestoneResponse(
    Guid Id,
    string Name,
    DateOnly? PlannedDate,
    int Weight,
    int SortOrder,
    DateTimeOffset? CompletedAtUtc,
    ProjectPersonResponse? CompletedBy,
    Guid RowVersion);

public sealed record ProjectProgressResponse(int TotalMilestones, int CompletedMilestones, int TotalWeight, int CompletedWeight, decimal Percent);

public sealed record ProjectChangeOrderResponse(
    Guid Id,
    string Number,
    string Title,
    string Reason,
    decimal BudgetDelta,
    decimal ContractDelta,
    string Status,
    ProjectPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProjectPersonResponse? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    Guid RowVersion);

public sealed record ProjectStatusHistoryResponse(
    Guid Id,
    string FromStatus,
    string ToStatus,
    string? Reason,
    ProjectPersonResponse Actor,
    DateTimeOffset OccurredAtUtc);

public sealed record ProjectContractResponse(decimal BaselineAmount, decimal ApprovedDelta, decimal CurrentAmount);

public sealed record ProjectControlResponse(
    Guid ProjectId,
    string Status,
    string? StatusReason,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    DateTimeOffset? ActivatedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    Guid RowVersion,
    ProjectBudgetResponse Budget,
    ProjectContractResponse Contract,
    ProjectProgressResponse Progress,
    IReadOnlyList<ProjectMilestoneResponse> Milestones,
    IReadOnlyList<ProjectChangeOrderResponse> ChangeOrders,
    IReadOnlyList<ProjectStatusHistoryResponse> History);
