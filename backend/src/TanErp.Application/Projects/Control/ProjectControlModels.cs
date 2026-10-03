namespace TanErp.Application.Projects.Control;

public sealed record BudgetLineInput(string Category, string Description, decimal Amount);

public sealed record ProjectBudgetLineProjection(Guid Id, string Category, string Description, decimal Amount, int SortOrder);

public sealed record ProjectBudgetProjection(
    decimal? BaselineTotal,
    string? BaselineHash,
    bool IsFrozen,
    DateTimeOffset? FrozenAtUtc,
    decimal ApprovedBudgetDelta,
    decimal? CurrentTotal,
    decimal CommittedAmount,
    decimal? AvailableBudget,
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
    IReadOnlyList<ProjectStatusHistoryProjection> History);
