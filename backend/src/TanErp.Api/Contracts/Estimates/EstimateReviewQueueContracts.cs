using TanErp.Application.Estimates;
using TanErp.Api.ErrorHandling;

namespace TanErp.Api.Contracts.Estimates;

public sealed record EstimateReviewQueuePersonResponse(Guid Id, string DisplayName);
public sealed record EstimateReviewQueueReviewerResponse(Guid Id, string DisplayName, Guid MembershipId);
public sealed record EstimateReviewQueueCustomerResponse(Guid Id, string DisplayNameTh, string? DisplayNameEn);
public sealed record EstimateReviewQueueOpportunityResponse(Guid Id, string Title);
public sealed record EstimateReviewQueueBranchResponse(Guid Id, string Code, string Name);
public sealed record EstimateReviewFrozenRouteResponse(
    string PolicyCode, int PolicyVersion, string RouteHash, string ScopeType, Guid ScopeId,
    string PermissionKey, EstimateReviewQueueReviewerResponse Reviewer,
    IReadOnlyList<EstimateReviewApprovalTriggerResponse> Triggers,
    EstimateReviewApprovalThresholdsResponse? Thresholds);
public sealed record EstimateReviewApprovalTriggerResponse(string Code, decimal? ActualValue, decimal? ThresholdValue, string? Unit);
public sealed record EstimateReviewApprovalThresholdsResponse(
    decimal? ManagerAmountLimit, decimal? FinancialAmountLimit, decimal? DirectorAmountLimit,
    decimal? CheckerMinimumMarginRate, decimal? DirectorMinimumMarginRate, decimal? DiscountLimitRate);
public sealed record EstimateReviewReadinessReasonResponse(string Code, string TargetType, Guid? TargetId, string TargetField);
public sealed record EstimateReviewCostEvidenceResponse(
    string WorkItemCode, string WorkItemDescription, string Type, string Description,
    decimal Quantity, string UnitCode, decimal UnitCost, string Currency, decimal TotalCost,
    string CostOrigin, string? ItemCode, int? CostRecordVersion, string? CostSourceCode,
    string? SourceReference, Guid? EvidenceFileId, string? CostRecordReason,
    string? ProvisionalReasonCode, string? ProvisionalNote, bool IsProvisional);
public sealed record EstimateReviewPriceOverrideResponse(
    string WorkItemCode, string WorkItemDescription, decimal FixedPriceUnitAmount, string? ReasonCode);
public sealed record EstimateReviewRevisionDiffResponse(
    int PreviousRevisionNo, decimal PreviousGrandTotal, decimal GrandTotalDelta,
    int AddedWorkItems, int RemovedWorkItems, int ChangedWorkItems);
public sealed record EstimateReviewQueueItemResponse(
    Guid ApprovalRequestId, Guid EstimateId, string EstimateNumber, Guid EstimateRowVersion,
    Guid RevisionId, int RevisionNo, string RevisionStatus, int CalculationVersion,
    string Currency, decimal GrandTotal, decimal MarginRate, Guid OpportunityId,
    EstimateReviewQueueOpportunityResponse Opportunity, EstimateReviewQueueCustomerResponse Customer,
    EstimateReviewQueueBranchResponse Branch, EstimateReviewQueuePersonResponse RequestedBy,
    DateTimeOffset RequestedAtUtc, string? SubmissionNote, string CalculationInputHash,
    string CalculationSnapshotHash, EstimateReviewFrozenRouteResponse FrozenRoute,
    string Readiness, IReadOnlyList<EstimateReviewReadinessReasonResponse> ReadinessReasons,
    IReadOnlyList<EstimateReviewPriceOverrideResponse> PriceOverrides,
    IReadOnlyList<EstimateReviewCostEvidenceResponse> CostEvidence,
    EstimateReviewRevisionDiffResponse? RevisionDiff,
    int ProvisionalCostCount);
public sealed record EstimateReviewQueueResponse(
    IReadOnlyList<EstimateReviewQueueItemResponse> Items, int TotalCount, int PageNumber, int PageSize);

public static class EstimateReviewQueueResponseMapper
{
    public static EstimateReviewQueueResponse ToResponse(EstimateReviewQueuePage page) => new(
        page.Items.Select(item => new EstimateReviewQueueItemResponse(
            item.ApprovalRequestId, item.EstimateId, item.EstimateNumber, item.EstimateRowVersion,
            item.RevisionId, item.RevisionNo, item.RevisionStatus, item.CalculationVersion,
            item.Currency, item.GrandTotal, item.MarginRate, item.OpportunityId,
            new EstimateReviewQueueOpportunityResponse(item.Opportunity.Id, item.Opportunity.Title),
            new EstimateReviewQueueCustomerResponse(item.Customer.Id, item.Customer.DisplayNameTh, item.Customer.DisplayNameEn),
            new EstimateReviewQueueBranchResponse(item.Branch.Id, item.Branch.Code, item.Branch.Name),
            new EstimateReviewQueuePersonResponse(item.RequestedBy.Id, item.RequestedBy.DisplayName),
            item.RequestedAtUtc, item.SubmissionNote, item.CalculationInputHash, item.CalculationSnapshotHash,
            new EstimateReviewFrozenRouteResponse(item.FrozenRoute.PolicyCode, item.FrozenRoute.PolicyVersion,
                item.FrozenRoute.RouteHash, item.FrozenRoute.ScopeType, item.FrozenRoute.ScopeId,
                item.FrozenRoute.PermissionKey, new EstimateReviewQueueReviewerResponse(
                    item.FrozenRoute.Reviewer.Id, item.FrozenRoute.Reviewer.DisplayName,
                    item.FrozenRoute.Reviewer.MembershipId),
                item.FrozenRoute.Triggers.Select(trigger => new EstimateReviewApprovalTriggerResponse(
                    trigger.Code, trigger.ActualValue, trigger.ThresholdValue, trigger.Unit)).ToList(),
                item.FrozenRoute.Thresholds is null ? null : new EstimateReviewApprovalThresholdsResponse(
                    item.FrozenRoute.Thresholds.ManagerAmountLimit, item.FrozenRoute.Thresholds.FinancialAmountLimit,
                    item.FrozenRoute.Thresholds.DirectorAmountLimit, item.FrozenRoute.Thresholds.CheckerMinimumMarginRate,
                    item.FrozenRoute.Thresholds.DirectorMinimumMarginRate, item.FrozenRoute.Thresholds.DiscountLimitRate)),
            item.Readiness, item.ReadinessReasons.Select(reason =>
                new EstimateReviewReadinessReasonResponse(reason.Code, reason.TargetType, reason.TargetId, reason.TargetField)).ToList(),
            item.PriceOverrides.Select(priceOverride => new EstimateReviewPriceOverrideResponse(
                priceOverride.WorkItemCode, priceOverride.WorkItemDescription, priceOverride.FixedPriceUnitAmount,
                priceOverride.ReasonCode)).ToList(),
            item.CostEvidence.Select(evidence => new EstimateReviewCostEvidenceResponse(
                evidence.WorkItemCode, evidence.WorkItemDescription, evidence.Type, evidence.Description,
                evidence.Quantity, evidence.UnitCode, evidence.UnitCost, evidence.Currency, evidence.TotalCost,
                evidence.CostOrigin, evidence.ItemCode, evidence.CostRecordVersion, evidence.CostSourceCode,
                evidence.SourceReference, evidence.EvidenceFileId, evidence.CostRecordReason,
                evidence.ProvisionalReasonCode, evidence.ProvisionalNote, evidence.IsProvisional)).ToList(),
            item.RevisionDiff is null ? null : new EstimateReviewRevisionDiffResponse(
                item.RevisionDiff.PreviousRevisionNo, item.RevisionDiff.PreviousGrandTotal,
                item.RevisionDiff.GrandTotalDelta, item.RevisionDiff.AddedWorkItems,
                item.RevisionDiff.RemovedWorkItems, item.RevisionDiff.ChangedWorkItems),
            item.ProvisionalCostCount)).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
}
