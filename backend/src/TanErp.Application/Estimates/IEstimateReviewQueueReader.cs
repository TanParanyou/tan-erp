using TanErp.Domain.Estimates;

namespace TanErp.Application.Estimates;

public sealed record EstimateReviewQueueCustomer(Guid Id, string DisplayNameTh, string? DisplayNameEn);
public sealed record EstimateReviewQueueOpportunity(Guid Id, string Title);
public sealed record EstimateReviewQueueBranch(Guid Id, string Code, string Name);
public sealed record EstimateReviewQueuePerson(Guid Id, string DisplayName);
public sealed record EstimateReviewQueueReviewer(Guid Id, string DisplayName, Guid MembershipId);
public sealed record EstimateReviewCostEvidence(
    string WorkItemCode, string WorkItemDescription, string Type, string Description,
    decimal Quantity, string UnitCode, decimal UnitCost, string Currency, decimal TotalCost,
    string CostOrigin, string? ItemCode, int? CostRecordVersion, string? CostSourceCode,
    string? SourceReference, Guid? EvidenceFileId, string? CostRecordReason,
    string? ProvisionalReasonCode, string? ProvisionalNote, bool IsProvisional);
public sealed record EstimateReviewPriceOverride(
    string WorkItemCode, string WorkItemDescription, decimal FixedPriceUnitAmount, string? ReasonCode);
public sealed record EstimateReviewRevisionDiff(
    int PreviousRevisionNo, decimal PreviousGrandTotal, decimal GrandTotalDelta,
    int AddedWorkItems, int RemovedWorkItems, int ChangedWorkItems);
public sealed record EstimateReviewFrozenRoute(
    string PolicyCode,
    int PolicyVersion,
    string RouteHash,
    string ScopeType,
    Guid ScopeId,
    string PermissionKey,
    EstimateReviewQueueReviewer Reviewer,
    IReadOnlyList<EstimateReviewApprovalTrigger> Triggers,
    EstimateReviewApprovalThresholds? Thresholds);
public sealed record EstimateReviewApprovalTrigger(string Code, decimal? ActualValue, decimal? ThresholdValue, string? Unit);
public sealed record EstimateReviewApprovalThresholds(
    decimal? ManagerAmountLimit, decimal? FinancialAmountLimit, decimal? DirectorAmountLimit,
    decimal? CheckerMinimumMarginRate, decimal? DirectorMinimumMarginRate, decimal? DiscountLimitRate);

public sealed record EstimateReviewQueueItem(
    Guid ApprovalRequestId,
    Guid EstimateId,
    string EstimateNumber,
    Guid EstimateRowVersion,
    Guid RevisionId,
    int RevisionNo,
    string RevisionStatus,
    int CalculationVersion,
    string Currency,
    decimal GrandTotal,
    decimal MarginRate,
    Guid OpportunityId,
    EstimateReviewQueueOpportunity Opportunity,
    EstimateReviewQueueCustomer Customer,
    EstimateReviewQueueBranch Branch,
    EstimateReviewQueuePerson RequestedBy,
    DateTimeOffset RequestedAtUtc,
    string? SubmissionNote,
    string CalculationInputHash,
    string CalculationSnapshotHash,
    EstimateReviewFrozenRoute FrozenRoute,
    string Readiness,
    IReadOnlyList<EstimateReadinessReason> ReadinessReasons,
    IReadOnlyList<EstimateReviewPriceOverride> PriceOverrides,
    IReadOnlyList<EstimateReviewCostEvidence> CostEvidence,
    EstimateReviewRevisionDiff? RevisionDiff,
    int ProvisionalCostCount);

public sealed record EstimateReviewQueuePage(
    IReadOnlyList<EstimateReviewQueueItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public interface IEstimateReviewQueueReader
{
    Task<EstimateReviewQueuePage> ListAsync(
        Guid organizationId,
        Guid reviewerMembershipId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
