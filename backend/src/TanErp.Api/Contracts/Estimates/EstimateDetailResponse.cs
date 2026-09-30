using System.ComponentModel.DataAnnotations;
using TanErp.Api.Contracts.Items;
using TanErp.Application.Estimates;

namespace TanErp.Api.Contracts.Estimates;

public sealed record EstimateCostComponentResponse(
    Guid Id,
    Guid EstimateWorkItemId,
    string Type,
    string Description,
    decimal Quantity,
    string UnitCode,
    decimal UnitCost,
    string Currency,
    decimal TotalCost,
    int SortOrder,
    Guid? ItemId = null,
    Guid? CostRecordId = null,
    int? CostRecordVersion = null,
    string? ItemCodeSnapshot = null,
    LocalizedTextResponse? ItemNameSnapshot = null,
    string? UnitSnapshot = null,
    decimal? UnitCostSnapshot = null,
    string? CurrencySnapshot = null,
    string? CostScopeSnapshot = null,
    DateTimeOffset? CostEffectiveFromUtc = null,
    string? CostPolicyVersion = null,
    DateTimeOffset? ResolvedAtUtc = null,
    string? CostOrigin = null,
    Guid? CostSourceIdSnapshot = null,
    string? CostSourceCodeSnapshot = null,
    string? CostSourceReferenceSnapshot = null,
    Guid? CostEvidenceFileIdSnapshot = null,
    string? CostRecordReasonSnapshot = null,
    string? ProvisionalReasonCode = null,
    string? ProvisionalNote = null,
    bool IsProvisional = false);

public sealed record EstimateCalculationSnapshotResponse(
    Guid Id,
    Guid EstimateRevisionId,
    int CalculationVersion,
    string InputHash,
    string SnapshotJson,
    Guid? CalculationPolicyVersionId,
    Guid? TaxPolicyVersionId,
    string CalculationPolicyVersion,
    string TaxPolicyVersion,
    string? CalculationPolicyHash,
    string? TaxPolicyHash,
    Guid CapturedByUserId,
    DateTimeOffset CapturedAtUtc)
{
    public static EstimateCalculationSnapshotResponse FromProjection(EstimateCalculationSnapshotProjection projection) => new(
        projection.Id,
        projection.EstimateRevisionId,
        projection.CalculationVersion,
        projection.InputHash,
        projection.SnapshotJson,
        projection.CalculationPolicyVersionId,
        projection.TaxPolicyVersionId,
        projection.CalculationPolicyVersion,
        projection.TaxPolicyVersion,
        projection.CalculationPolicyHash,
        projection.TaxPolicyHash,
        projection.CapturedByUserId,
        projection.CapturedAtUtc);
}

public sealed record EstimateWorkItemItemResponse(
    [property: Required] Guid Id,
    [property: Required] string Code,
    [property: Required] string NameTh,
    string? NameEn);

public sealed record EstimateWorkItemResponse(
    Guid Id,
    Guid EstimateSectionId,
    string Code,
    string DescriptionTh,
    string? DescriptionEn,
    decimal Quantity,
    string UnitCode,
    string SellingRuleType,
    decimal SellingRuleValue,
    string? SellingRuleReasonCode,
    decimal UnitCost,
    decimal TotalCost,
    decimal UnitSellingPrice,
    decimal TotalSellingPrice,
    int SortOrder,
    IReadOnlyList<EstimateCostComponentResponse> CostComponents,
    EstimateWorkItemItemResponse? Item = null,
    string? OverrideReasonCode = null,
    string? OverrideReason = null);

public sealed record EstimateSectionResponse(
    Guid Id,
    Guid EstimateRevisionId,
    string Code,
    string NameTh,
    string? NameEn,
    int SortOrder,
    decimal SubtotalCost,
    decimal SubtotalSellingPrice,
    IReadOnlyList<EstimateWorkItemResponse> WorkItems);

public sealed record EstimateReadinessReasonResponse(
    string Code,
    string TargetType,
    Guid? TargetId,
    string TargetField);

public sealed record EstimateRevisionResponse(
    Guid Id,
    Guid EstimateId,
    int RevisionNo,
    string Status,
    string Currency,
    int CalculationVersion,
    bool CalculationOutdated,
    string CalculationPolicyVersion,
    string TaxPolicyVersion,
    decimal NetCost,
    decimal SellingBeforeDiscount,
    string DiscountType,
    decimal DiscountValue,
    string? DiscountReasonCode,
    decimal DiscountAmount,
    decimal NetBeforeTax,
    decimal TaxAmount,
    decimal GrandTotal,
    decimal MarginAmount,
    decimal MarginRate,
    decimal MarkupRate,
    string? CalculationSnapshotJson,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<EstimateSectionResponse> Sections,
    string Readiness,
    IReadOnlyList<EstimateReadinessReasonResponse> ReadinessReasons);

public sealed record EstimateDetailResponse(
    Guid Id,
    Guid OrganizationId,
    Guid BranchId,
    Guid CustomerId,
    Guid OpportunityId,
    Guid? SiteSurveyRevisionId,
    string? SiteSurveySnapshotHash,
    string Number,
    string Status,
    int CurrentRevisionNo,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    EstimateRevisionResponse? CurrentRevision)
{
    public static EstimateDetailResponse FromProjection(EstimateDetailProjection p)
    {
        var rev = p.CurrentRevision is null ? null : EstimateRevisionResponseFromProjection(p.CurrentRevision);

        return new EstimateDetailResponse(
            p.Id,
            p.OrganizationId,
            p.BranchId,
            p.CustomerId,
            p.OpportunityId,
            p.SiteSurveyRevisionId,
            p.SiteSurveySnapshotHash,
            p.Number,
            p.Status,
            p.CurrentRevisionNo,
            p.RowVersion,
            p.CreatedAtUtc,
            p.UpdatedAtUtc,
            rev);
    }

    public static EstimateRevisionResponse EstimateRevisionResponseFromProjection(EstimateRevisionProjection r)
    {
        var sections = r.Sections.Select(s => new EstimateSectionResponse(
            s.Id,
            s.EstimateRevisionId,
            s.Code,
            s.NameTh,
            s.NameEn,
            s.SortOrder,
            s.SubtotalCost,
            s.SubtotalSellingPrice,
            s.WorkItems.Select(w => new EstimateWorkItemResponse(
                w.Id,
                w.EstimateSectionId,
                w.Code,
                w.DescriptionTh,
                w.DescriptionEn,
                w.Quantity,
                w.UnitCode,
                w.SellingRuleType,
                w.SellingRuleValue,
                w.SellingRuleReasonCode,
                w.UnitCost,
                w.TotalCost,
                w.UnitSellingPrice,
                w.TotalSellingPrice,
                w.SortOrder,
                w.CostComponents.Select(c => new EstimateCostComponentResponse(
                    c.Id,
                    c.EstimateWorkItemId,
                    c.Type,
                    c.Description,
                    c.Quantity,
                    c.UnitCode,
                    c.UnitCost,
                    c.Currency,
                    c.TotalCost,
                    c.SortOrder,
                    c.ItemId,
                    c.CostRecordId,
                    c.CostRecordVersion,
                    c.ItemCodeSnapshot,
                    c.ItemNameSnapshot == null ? null : new LocalizedTextResponse { Thai = c.ItemNameSnapshot.Thai, English = c.ItemNameSnapshot.English },
                    c.UnitSnapshot,
                    c.UnitCostSnapshot,
                    c.CurrencySnapshot,
                    c.CostScopeSnapshot,
                    c.CostEffectiveFromUtc,
                    c.CostPolicyVersion,
                    c.ResolvedAtUtc,
                    c.CostOrigin,
                    c.CostSourceIdSnapshot,
                    c.CostSourceCodeSnapshot,
                    c.CostSourceReferenceSnapshot,
                    c.CostEvidenceFileIdSnapshot,
                    c.CostRecordReasonSnapshot,
                    c.ProvisionalReasonCode,
                    c.ProvisionalNote,
                    c.IsProvisional)).ToList(),
                w.ItemId.HasValue && w.ItemCodeSnapshot is not null && w.ItemNameThSnapshot is not null
                    ? new EstimateWorkItemItemResponse(w.ItemId.Value, w.ItemCodeSnapshot, w.ItemNameThSnapshot, w.ItemNameEnSnapshot)
                    : null,
                w.OverrideReasonCode,
                w.OverrideReason)).ToList())).ToList();

        return new EstimateRevisionResponse(
            r.Id,
            r.EstimateId,
            r.RevisionNo,
            r.Status,
            r.Currency,
            r.CalculationVersion,
            r.CalculationOutdated,
            r.CalculationPolicyVersion,
            r.TaxPolicyVersion,
            r.NetCost,
            r.SellingBeforeDiscount,
            r.DiscountType,
            r.DiscountValue,
            r.DiscountReasonCode,
            r.DiscountAmount,
            r.NetBeforeTax,
            r.TaxAmount,
            r.GrandTotal,
            r.MarginAmount,
            r.MarginRate,
            r.MarkupRate,
            r.CalculationSnapshotJson,
            r.RowVersion,
            r.CreatedAtUtc,
            r.UpdatedAtUtc,
            sections,
            r.Readiness.Status,
            r.Readiness.Reasons.Select(reason => new EstimateReadinessReasonResponse(
                reason.Code, reason.TargetType, reason.TargetId, reason.TargetField)).ToList());
    }
}
