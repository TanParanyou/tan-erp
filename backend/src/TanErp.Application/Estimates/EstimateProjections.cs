using TanErp.Domain.Items;

namespace TanErp.Application.Estimates;

public sealed record EstimateCostComponentProjection(
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
    LocalizedText? ItemNameSnapshot = null,
    string? UnitSnapshot = null,
    decimal? UnitCostSnapshot = null,
    string? CurrencySnapshot = null,
    string? CostScopeSnapshot = null,
    DateTimeOffset? CostEffectiveFromUtc = null,
    string? CostPolicyVersion = null,
    DateTimeOffset? ResolvedAtUtc = null);

public sealed record EstimateWorkItemProjection(
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
    IReadOnlyList<EstimateCostComponentProjection> CostComponents);

public sealed record EstimateSectionProjection(
    Guid Id,
    Guid EstimateRevisionId,
    string Code,
    string NameTh,
    string? NameEn,
    int SortOrder,
    decimal SubtotalCost,
    decimal SubtotalSellingPrice,
    IReadOnlyList<EstimateWorkItemProjection> WorkItems);

public sealed record EstimateRevisionProjection(
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
    IReadOnlyList<EstimateSectionProjection> Sections);

public sealed record EstimateDetailProjection(
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
    EstimateRevisionProjection? CurrentRevision);

public sealed record QuotationDetailProjection(
    Guid QuotationId,
    Guid EstimateId,
    Guid OpportunityId,
    string Number,
    string Status,
    decimal GrandTotal,
    DateTimeOffset IssuedAtUtc,
    Guid EstimateRevisionId,
    int RevisionNo,
    string OpportunityStage,
    Guid OpportunityRowVersion,
    Guid EstimateRowVersion);

public sealed record AcceptQuotationProjection(
    Guid QuotationId,
    Guid OpportunityId,
    string OpportunityStage,
    Guid OpportunityRowVersion,
    DateTimeOffset AcceptedAtUtc);
