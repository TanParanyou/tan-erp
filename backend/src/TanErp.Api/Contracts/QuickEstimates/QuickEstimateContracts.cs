namespace TanErp.Api.Contracts.QuickEstimates;

public sealed record QePersonResponse(Guid Id, string DisplayName);

public sealed record GradeResponse(string Code, string Name, decimal Factor);

public sealed record ComplexityResponse(string Code, string Name, decimal Factor, decimal RiskModifier);

public sealed record AddOnResponse(string Code, string Name, decimal Amount, bool PerLine);

public sealed record PricingTemplateResponse(Guid Id, string Code, int Version, string WorkType, string Name, string Status, string MeasurementRule, string UnitCode, decimal ReferenceRate, decimal MinimumCharge, decimal BaseRangeRate, decimal MaxRangeRate, decimal RoundingStep, int ValidityDays, decimal TaxRate, string TaxDisplay, decimal DirectShareLimit, DateOnly? EffectiveFrom, DateOnly? EffectiveTo, IReadOnlyList<GradeResponse> Grades, IReadOnlyList<ComplexityResponse> Complexities, IReadOnlyList<AddOnResponse> AddOns, decimal LowConfidenceModifier, decimal MediumConfidenceModifier, decimal CustomMaterialModifier, IReadOnlyList<string> Assumptions, IReadOnlyList<string> Exclusions, QePersonResponse CreatedBy, QePersonResponse? DecidedBy, string? DecisionNote, DateTimeOffset CreatedAtUtc, Guid RowVersion);

public sealed record PricingTemplateListItemResponse(Guid Id, string Code, int Version, string WorkType, string Name, string Status, DateTimeOffset UpdatedAtUtc);

public sealed record EffectiveOptionResponse(string Code, string Name);

public sealed record EffectiveAddOnResponse(string Code, string Name, bool PerLine);

public sealed record EffectiveTemplateResponse(Guid Id, string Code, int Version, string WorkType, string Name, string Status, string MeasurementRule, string UnitCode, string TaxDisplay, int ValidityDays, IReadOnlyList<EffectiveOptionResponse> Grades, IReadOnlyList<EffectiveOptionResponse> Complexities, IReadOnlyList<EffectiveAddOnResponse> AddOns, IReadOnlyList<string> Assumptions, IReadOnlyList<string> Exclusions);

public sealed record MeasurementResponse(Guid LineId, string? WorkSubtype, decimal? WidthM, decimal? HeightM, decimal? DepthM, decimal Quantity);

public sealed record CalculationLineResponse(Guid LineId, string? WorkSubtype, decimal BillableQuantity, decimal BaseAmount, decimal AdjustedAmount);

public sealed record CalculationResponse(int Version, string TemplateCode, int TemplateVersion, decimal NetAmount, decimal TaxAmount, decimal DisplayedLower, decimal DisplayedUpper, DateOnly ValidUntil, string ShareDecision, IReadOnlyList<string> ReasonCodes, string InputHash, IReadOnlyList<CalculationLineResponse> Lines, QePersonResponse CreatedBy, DateTimeOffset CreatedAtUtc);

public sealed record ReviewResponse(Guid Id, int SourceVersion, string Status, string? RequestNote, QePersonResponse RequestedBy, DateTimeOffset RequestedAtUtc, QePersonResponse? DecidedBy, DateTimeOffset? DecidedAtUtc, string? ReasonCode, string? DecisionNote);

public sealed record ShareResponse(Guid Id, int SourceVersion, string Channel, string? Recipient, string Locale, QePersonResponse CreatedBy, DateTimeOffset CreatedAtUtc, string SummaryJson);

public sealed record ConversionResponse(Guid Id, int SourceVersion, Guid OfficialEstimateId, DateTimeOffset CreatedAtUtc);

public sealed record QuickEstimateTemplateRefResponse(Guid Id, string Code, int Version, string Name, string Status);

public sealed record QuickEstimateResponse(Guid Id, Guid BranchId, string Number, string Status, Guid? CustomerId, Guid? OpportunityId, QuickEstimateTemplateRefResponse? Template, string PropertyType, string RoomOrArea, string GradeCode, IReadOnlyList<string> ComplexityCodes, IReadOnlyList<string> AddOnCodes, string MeasurementConfidence, bool CustomMaterial, IReadOnlyList<MeasurementResponse> Measurements, int CurrentCalculationVersion, CalculationResponse? CurrentCalculation, IReadOnlyList<ReviewResponse> Reviews, IReadOnlyList<ShareResponse> Shares, IReadOnlyList<ConversionResponse> Conversions, QePersonResponse CreatedBy, DateTimeOffset CreatedAtUtc, DateTimeOffset UpdatedAtUtc, Guid RowVersion);

public sealed record QuickEstimateListItemResponse(Guid Id, string Number, string Status, string RoomOrArea, string? TemplateCode, decimal? DisplayedLower, decimal? DisplayedUpper, string? ShareDecision, DateTimeOffset UpdatedAtUtc);

public sealed record PricingTemplateListResponse(IReadOnlyList<PricingTemplateListItemResponse> Items, QuickEstimatePaginationResponse Pagination);

public sealed record QuickEstimateListResponse(IReadOnlyList<QuickEstimateListItemResponse> Items, QuickEstimatePaginationResponse Pagination);

public sealed record QuickEstimatePaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record GradeRequest(string? Code, string? Name, decimal Factor);

public sealed record ComplexityRequest(string? Code, string? Name, decimal Factor, decimal RiskModifier);

public sealed record AddOnRequest(string? Code, string? Name, decimal Amount, bool PerLine);

public sealed record PricingTemplateRequest(
    string? Code,
    string? WorkType,
    string? Name,
    string? MeasurementRule,
    string? UnitCode,
    decimal ReferenceRate,
    decimal MinimumCharge,
    decimal BaseRangeRate,
    decimal MaxRangeRate,
    decimal RoundingStep,
    int ValidityDays,
    decimal TaxRate,
    string? TaxDisplay,
    decimal DirectShareLimit,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    List<GradeRequest>? Grades,
    List<ComplexityRequest>? Complexities,
    List<AddOnRequest>? AddOns,
    decimal LowConfidenceModifier,
    decimal MediumConfidenceModifier,
    decimal CustomMaterialModifier,
    List<string>? Assumptions,
    List<string>? Exclusions);

public sealed record TemplateDecisionRequest(string? Decision, string? Note);

public sealed record CreateQuickEstimateRequest(Guid? CustomerId, Guid? OpportunityId);

public sealed record MeasurementRequest(Guid LineId, string? WorkSubtype, decimal? WidthM, decimal? HeightM, decimal? DepthM, decimal Quantity);

public sealed record QuickEstimateDraftRequest(
    Guid? TemplateId,
    string? PropertyType,
    string? RoomOrArea,
    string? GradeCode,
    List<string>? ComplexityCodes,
    List<string>? AddOnCodes,
    string? MeasurementConfidence,
    bool? CustomMaterial,
    List<MeasurementRequest>? Measurements);

public sealed record SubmitReviewRequest(int SourceVersion, string? Note);

public sealed record ReviewDecisionRequest(int SourceVersion, string? Decision, string? ReasonCode, string? Note);

public sealed record ShareRequest(int SourceVersion, string? Channel, string? Recipient, string? Locale);

public sealed record ConversionRequest(int SourceVersion, Guid SiteSurveyRevisionId);

public sealed record CalculationSnapshotResponse(int Version, string SnapshotJson);
