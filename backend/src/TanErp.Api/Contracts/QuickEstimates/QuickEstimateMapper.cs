using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Contracts.QuickEstimates;

internal static class QuickEstimateMapper
{
    public static QePersonResponse To(QePerson x) => new(x.Id, x.DisplayName);
    public static GradeResponse To(GradeInput x) => new(x.Code, x.Name, x.Factor);
    public static ComplexityResponse To(ComplexityInput x) => new(x.Code, x.Name, x.Factor, x.RiskModifier);
    public static AddOnResponse To(AddOnInput x) => new(x.Code, x.Name, x.Amount, x.PerLine);
    public static PricingTemplateResponse To(PricingTemplateProjection x) => new(x.Id, x.Code, x.Version, x.WorkType, x.Name, x.Status, x.MeasurementRule, x.UnitCode, x.ReferenceRate, x.MinimumCharge, x.BaseRangeRate, x.MaxRangeRate, x.RoundingStep, x.ValidityDays, x.TaxRate, x.TaxDisplay, x.DirectShareLimit, x.EffectiveFrom, x.EffectiveTo, x.Grades.Select(To).ToList(), x.Complexities.Select(To).ToList(), x.AddOns.Select(To).ToList(), x.LowConfidenceModifier, x.MediumConfidenceModifier, x.CustomMaterialModifier, x.Assumptions, x.Exclusions, To(x.CreatedBy), x.DecidedBy is null ? null : To(x.DecidedBy), x.DecisionNote, x.CreatedAtUtc, x.RowVersion);
    public static PricingTemplateListItemResponse To(PricingTemplateListItemProjection x) => new(x.Id, x.Code, x.Version, x.WorkType, x.Name, x.Status, x.UpdatedAtUtc);
    public static EffectiveOptionResponse To(EffectiveOption x) => new(x.Code, x.Name);
    public static EffectiveAddOnResponse To(EffectiveAddOn x) => new(x.Code, x.Name, x.PerLine);
    public static EffectiveTemplateResponse To(EffectiveTemplateProjection x) => new(x.Id, x.Code, x.Version, x.WorkType, x.Name, x.Status, x.MeasurementRule, x.UnitCode, x.TaxDisplay, x.ValidityDays, x.Grades.Select(To).ToList(), x.Complexities.Select(To).ToList(), x.AddOns.Select(To).ToList(), x.Assumptions, x.Exclusions);
    public static MeasurementResponse To(MeasurementInput x) => new(x.LineId, x.WorkSubtype, x.WidthM, x.HeightM, x.DepthM, x.Quantity);
    public static CalculationLineResponse To(CalculationLineProjection x) => new(x.LineId, x.WorkSubtype, x.BillableQuantity, x.BaseAmount, x.AdjustedAmount);
    public static CalculationResponse To(CalculationProjection x) => new(x.Version, x.TemplateCode, x.TemplateVersion, x.NetAmount, x.TaxAmount, x.DisplayedLower, x.DisplayedUpper, x.ValidUntil, x.ShareDecision, x.ReasonCodes, x.InputHash, x.Lines.Select(To).ToList(), To(x.CreatedBy), x.CreatedAtUtc);
    public static ReviewResponse To(ReviewProjection x) => new(x.Id, x.SourceVersion, x.Status, x.RequestNote, To(x.RequestedBy), x.RequestedAtUtc, x.DecidedBy is null ? null : To(x.DecidedBy), x.DecidedAtUtc, x.ReasonCode, x.DecisionNote);
    public static ShareResponse To(ShareProjection x) => new(x.Id, x.SourceVersion, x.Channel, x.Recipient, x.Locale, To(x.CreatedBy), x.CreatedAtUtc, x.SummaryJson);
    public static ConversionResponse To(ConversionProjection x) => new(x.Id, x.SourceVersion, x.OfficialEstimateId, x.CreatedAtUtc);
    public static QuickEstimateTemplateRefResponse To(QuickEstimateTemplateRef x) => new(x.Id, x.Code, x.Version, x.Name, x.Status);
    public static QuickEstimateResponse To(QuickEstimateProjection x) => new(x.Id, x.BranchId, x.Number, x.Status, x.CustomerId, x.OpportunityId, x.Template is null ? null : To(x.Template), x.PropertyType, x.RoomOrArea, x.GradeCode, x.ComplexityCodes, x.AddOnCodes, x.MeasurementConfidence, x.CustomMaterial, x.Measurements.Select(To).ToList(), x.CurrentCalculationVersion, x.CurrentCalculation is null ? null : To(x.CurrentCalculation), x.Reviews.Select(To).ToList(), x.Shares.Select(To).ToList(), x.Conversions.Select(To).ToList(), To(x.CreatedBy), x.CreatedAtUtc, x.UpdatedAtUtc, x.RowVersion);
    public static QuickEstimateListItemResponse To(QuickEstimateListItemProjection x) => new(x.Id, x.Number, x.Status, x.RoomOrArea, x.TemplateCode, x.DisplayedLower, x.DisplayedUpper, x.ShareDecision, x.UpdatedAtUtc);
    public static QuickEstimatePaginationResponse Pagination(int page, int pageSize, int total) => new(page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
}
