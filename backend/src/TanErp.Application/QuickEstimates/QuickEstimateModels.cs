namespace TanErp.Application.QuickEstimates;

public sealed record QePerson(Guid Id, string DisplayName);

// ----- pricing templates ------------------------------------------------------------------------

public sealed record GradeInput(string Code, string Name, decimal Factor);

public sealed record ComplexityInput(string Code, string Name, decimal Factor, decimal RiskModifier);

public sealed record AddOnInput(string Code, string Name, decimal Amount, bool PerLine);

public sealed record TemplateInput(
    string Code,
    string WorkType,
    string Name,
    string MeasurementRule,
    string UnitCode,
    decimal ReferenceRate,
    decimal MinimumCharge,
    decimal BaseRangeRate,
    decimal MaxRangeRate,
    decimal RoundingStep,
    int ValidityDays,
    decimal TaxRate,
    string TaxDisplay,
    decimal DirectShareLimit,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    IReadOnlyList<GradeInput> Grades,
    IReadOnlyList<ComplexityInput> Complexities,
    IReadOnlyList<AddOnInput> AddOns,
    decimal LowConfidenceModifier,
    decimal MediumConfidenceModifier,
    decimal CustomMaterialModifier,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Exclusions);

public sealed record PricingTemplateProjection(
    Guid Id,
    string Code,
    int Version,
    string WorkType,
    string Name,
    string Status,
    string MeasurementRule,
    string UnitCode,
    decimal ReferenceRate,
    decimal MinimumCharge,
    decimal BaseRangeRate,
    decimal MaxRangeRate,
    decimal RoundingStep,
    int ValidityDays,
    decimal TaxRate,
    string TaxDisplay,
    decimal DirectShareLimit,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    IReadOnlyList<GradeInput> Grades,
    IReadOnlyList<ComplexityInput> Complexities,
    IReadOnlyList<AddOnInput> AddOns,
    decimal LowConfidenceModifier,
    decimal MediumConfidenceModifier,
    decimal CustomMaterialModifier,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Exclusions,
    QePerson CreatedBy,
    QePerson? DecidedBy,
    string? DecisionNote,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion);

public sealed record PricingTemplateListItemProjection(Guid Id, string Code, int Version, string WorkType, string Name, string Status, DateTimeOffset UpdatedAtUtc);

public sealed record TemplateListQuery(string? Search, string? Status, string? WorkType, int Page, int PageSize);

public sealed record PagedTemplates(IReadOnlyList<PricingTemplateListItemProjection> Items, int TotalCount, int Page, int PageSize);

public enum TemplateAction
{
    Submit,
    Calibrate,
    Activate,
    Disable
}

/// <summary>What the capture form needs to offer choices. Rates and factors are deliberately absent.</summary>
public sealed record EffectiveOption(string Code, string Name);

public sealed record EffectiveAddOn(string Code, string Name, bool PerLine);

public sealed record EffectiveTemplateProjection(
    Guid Id,
    string Code,
    int Version,
    string WorkType,
    string Name,
    string Status,
    string MeasurementRule,
    string UnitCode,
    string TaxDisplay,
    int ValidityDays,
    IReadOnlyList<EffectiveOption> Grades,
    IReadOnlyList<EffectiveOption> Complexities,
    IReadOnlyList<EffectiveAddOn> AddOns,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Exclusions);

// ----- quick estimates --------------------------------------------------------------------------

public sealed record MeasurementInput(Guid LineId, string? WorkSubtype, decimal? WidthM, decimal? HeightM, decimal? DepthM, decimal Quantity);

/// <summary>Merge-patch style: only fields that are present change.</summary>
public sealed record QuickEstimateDraftPatch(
    Guid? TemplateId,
    string? PropertyType,
    string? RoomOrArea,
    string? GradeCode,
    IReadOnlyList<string>? ComplexityCodes,
    IReadOnlyList<string>? AddOnCodes,
    string? MeasurementConfidence,
    bool? CustomMaterial,
    IReadOnlyList<MeasurementInput>? Measurements);

public sealed record CalculationLineProjection(Guid LineId, string? WorkSubtype, decimal BillableQuantity, decimal BaseAmount, decimal AdjustedAmount);

public sealed record CalculationProjection(
    int Version,
    string TemplateCode,
    int TemplateVersion,
    decimal NetAmount,
    decimal TaxAmount,
    decimal DisplayedLower,
    decimal DisplayedUpper,
    DateOnly ValidUntil,
    string ShareDecision,
    IReadOnlyList<string> ReasonCodes,
    string InputHash,
    IReadOnlyList<CalculationLineProjection> Lines,
    QePerson CreatedBy,
    DateTimeOffset CreatedAtUtc);

public sealed record ReviewProjection(
    Guid Id,
    int SourceVersion,
    string Status,
    string? RequestNote,
    QePerson RequestedBy,
    DateTimeOffset RequestedAtUtc,
    QePerson? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? ReasonCode,
    string? DecisionNote);

public sealed record ShareProjection(Guid Id, int SourceVersion, string Channel, string? Recipient, string Locale, QePerson CreatedBy, DateTimeOffset CreatedAtUtc, string SummaryJson);

public sealed record ConversionProjection(Guid Id, int SourceVersion, Guid OfficialEstimateId, DateTimeOffset CreatedAtUtc);

public sealed record QuickEstimateTemplateRef(Guid Id, string Code, int Version, string Name, string Status);

public sealed record QuickEstimateProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    Guid? CustomerId,
    Guid? OpportunityId,
    QuickEstimateTemplateRef? Template,
    string PropertyType,
    string RoomOrArea,
    string GradeCode,
    IReadOnlyList<string> ComplexityCodes,
    IReadOnlyList<string> AddOnCodes,
    string MeasurementConfidence,
    bool CustomMaterial,
    IReadOnlyList<MeasurementInput> Measurements,
    int CurrentCalculationVersion,
    CalculationProjection? CurrentCalculation,
    IReadOnlyList<ReviewProjection> Reviews,
    IReadOnlyList<ShareProjection> Shares,
    IReadOnlyList<ConversionProjection> Conversions,
    QePerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    Guid RowVersion);

public sealed record QuickEstimateListItemProjection(
    Guid Id,
    string Number,
    string Status,
    string RoomOrArea,
    string? TemplateCode,
    decimal? DisplayedLower,
    decimal? DisplayedUpper,
    string? ShareDecision,
    DateTimeOffset UpdatedAtUtc);

public sealed record QuickEstimateListQuery(string? Search, string? Status, int Page, int PageSize);

public sealed record PagedQuickEstimates(IReadOnlyList<QuickEstimateListItemProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record ShareInput(int SourceVersion, string Channel, string? Recipient, string Locale);

public sealed record ConversionInput(int SourceVersion, Guid SiteSurveyRevisionId);
