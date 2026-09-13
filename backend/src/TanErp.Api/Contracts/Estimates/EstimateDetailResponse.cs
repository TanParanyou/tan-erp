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
    int SortOrder);

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
    decimal UnitCost,
    decimal TotalCost,
    decimal UnitSellingPrice,
    decimal TotalSellingPrice,
    int SortOrder,
    IReadOnlyList<EstimateCostComponentResponse> CostComponents);

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

public sealed record EstimateRevisionResponse(
    Guid Id,
    Guid EstimateId,
    int RevisionNo,
    string Status,
    string Currency,
    int CalculationVersion,
    string CalculationPolicyVersion,
    string TaxPolicyVersion,
    decimal NetCost,
    decimal SellingBeforeDiscount,
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
    IReadOnlyList<EstimateSectionResponse> Sections);

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
                    c.SortOrder)).ToList())).ToList())).ToList();

        return new EstimateRevisionResponse(
            r.Id,
            r.EstimateId,
            r.RevisionNo,
            r.Status,
            r.Currency,
            r.CalculationVersion,
            r.CalculationPolicyVersion,
            r.TaxPolicyVersion,
            r.NetCost,
            r.SellingBeforeDiscount,
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
            sections);
    }
}
