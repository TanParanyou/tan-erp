namespace TanErp.Application.Projects.Control;

public sealed record BudgetLineInput(string Category, string Description, decimal Amount, Guid? WbsNodeId = null);

public sealed record ProjectBudgetLineProjection(
    Guid Id,
    string Category,
    string Description,
    decimal Amount,
    int SortOrder,
    Guid? WbsNodeId = null,
    string? WbsCode = null,
    string? WbsName = null);

public sealed record ProjectBudgetProjection(
    decimal? BaselineTotal,
    string? BaselineHash,
    bool IsFrozen,
    DateTimeOffset? FrozenAtUtc,
    decimal ApprovedBudgetDelta,
    decimal? CurrentTotal,
    decimal CommittedAmount,
    decimal ActualAmount,
    decimal? AvailableBudget,
    decimal? VarianceAmount,
    IReadOnlyList<ProjectBudgetLineProjection> Lines);

public sealed record ProjectMilestoneProjection(
    Guid Id,
    string Name,
    DateOnly? PlannedDate,
    int Weight,
    int SortOrder,
    DateTimeOffset? CompletedAtUtc,
    ProjectPersonProjection? CompletedBy,
    Guid RowVersion);

public sealed record ProjectProgressProjection(int TotalMilestones, int CompletedMilestones, int TotalWeight, int CompletedWeight, decimal Percent);

public sealed record ProjectChangeOrderProjection(
    Guid Id,
    string Number,
    string Title,
    string Reason,
    decimal BudgetDelta,
    decimal ContractDelta,
    string Status,
    ProjectPersonProjection CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProjectPersonProjection? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    Guid RowVersion);

public sealed record ProjectStatusHistoryProjection(
    Guid Id,
    string FromStatus,
    string ToStatus,
    string? Reason,
    ProjectPersonProjection Actor,
    DateTimeOffset OccurredAtUtc);

public sealed record ProjectContractProjection(decimal BaselineAmount, decimal ApprovedDelta, decimal CurrentAmount);

public sealed record ProjectWbsNodeProjection(
    Guid Id,
    Guid? ParentId,
    string Code,
    string Name,
    int Level,
    int SortOrder,
    Guid RowVersion);

public sealed record ProjectWbsCostSummaryProjection(
    Guid? WbsNodeId,
    string WbsCode,
    string WbsName,
    int Level,
    decimal BaselineBudget,
    decimal CommittedAmount,
    decimal ActualAmount,
    decimal RemainingBudget,
    bool IsOverrun);

public sealed record ProjectCostLedgerEntryProjection(
    Guid Id,
    Guid? WbsNodeId,
    string? WbsCode,
    string Category,
    string EntryType,
    string SourceType,
    Guid SourceId,
    string SourceNumber,
    string Description,
    decimal Amount,
    bool IsReversal,
    ProjectPersonProjection Actor,
    DateTimeOffset OccurredAtUtc);

public sealed record LaborRateProjection(
    Guid Id,
    ProjectPersonProjection Worker,
    decimal HourlyRate,
    DateTimeOffset UpdatedAtUtc,
    Guid RowVersion);

public sealed record LaborTimesheetProjection(
    Guid Id,
    string Number,
    ProjectPersonProjection Worker,
    DateOnly WorkDate,
    decimal Hours,
    decimal HourlyRate,
    decimal CostAmount,
    Guid? WbsNodeId,
    string? WbsCode,
    Guid? WorkOrderId,
    string? WorkOrderNumber,
    string TaskDescription,
    string Status,
    bool IsOvertimeWarning,
    ProjectPersonProjection CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProjectPersonProjection? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    Guid RowVersion);

public sealed record TimesheetInput(
    Guid WorkerUserId,
    DateOnly WorkDate,
    decimal Hours,
    Guid? WbsNodeId,
    Guid? WorkOrderId,
    string TaskDescription);

public sealed record ProjectControlProjection(
    Guid ProjectId,
    string Status,
    string? StatusReason,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    DateTimeOffset? ActivatedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    Guid RowVersion,
    ProjectBudgetProjection Budget,
    ProjectContractProjection Contract,
    ProjectProgressProjection Progress,
    IReadOnlyList<ProjectMilestoneProjection> Milestones,
    IReadOnlyList<ProjectChangeOrderProjection> ChangeOrders,
    IReadOnlyList<ProjectStatusHistoryProjection> History,
    IReadOnlyList<ProjectWbsNodeProjection> WbsNodes,
    IReadOnlyList<ProjectWbsCostSummaryProjection> WbsCostSummary,
    IReadOnlyList<ProjectCostLedgerEntryProjection> CostLedger,
    IReadOnlyList<LaborRateProjection> LaborRates,
    IReadOnlyList<LaborTimesheetProjection> Timesheets);
