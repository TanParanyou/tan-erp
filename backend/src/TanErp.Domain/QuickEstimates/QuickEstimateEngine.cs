using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TanErp.Domain.QuickEstimates;

public sealed record MeasurementLine(Guid LineId, string? WorkSubtype, decimal? WidthM, decimal? HeightM, decimal? DepthM, decimal Quantity);

public sealed record QuickEstimateInput(
    string PropertyType,
    string RoomOrArea,
    string GradeCode,
    IReadOnlyList<string> ComplexityCodes,
    IReadOnlyList<string> AddOnCodes,
    string MeasurementConfidence,
    bool CustomMaterial,
    IReadOnlyList<MeasurementLine> Measurements);

public sealed record EstimateLineResult(Guid LineId, string? WorkSubtype, decimal BillableQuantity, decimal BaseAmount, decimal AdjustedAmount);

public sealed record RiskPart(string Code, decimal Amount);

public sealed record CalculationResult(
    IReadOnlyList<EstimateLineResult> Lines,
    decimal AdjustedAmount,
    decimal MinimumCharge,
    decimal NetAmount,
    decimal TaxAmount,
    decimal RiskRate,
    IReadOnlyList<RiskPart> RiskParts,
    decimal RawLower,
    decimal RawUpper,
    decimal DisplayedLower,
    decimal DisplayedUpper,
    DateOnly ValidUntil,
    string ShareDecision,
    IReadOnlyList<string> ReasonCodes,
    string InputHash,
    string SnapshotJson);

/// <summary>
/// Deterministic pricing of a quick estimate from one template version. It reads only its arguments (no clock, no database), so the
/// same input and template always give the same range and the same snapshot.
/// </summary>
public static class QuickEstimateEngine
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static decimal Round2(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    public static decimal RoundDown(decimal value, decimal step) => Math.Floor(value / step) * step;

    public static decimal RoundUp(decimal value, decimal step) => Math.Ceiling(value / step) * step;

    /// <summary>The fields a measurement line must have for the template's rule; used to report what is missing.</summary>
    public static IReadOnlyList<string> RequiredDimensions(string rule) => rule switch
    {
        MeasurementRule.Area => new[] { "widthM", "heightM" },
        MeasurementRule.Length => new[] { "widthM" },
        MeasurementRule.Volume => new[] { "widthM", "heightM", "depthM" },
        _ => Array.Empty<string>()
    };

    public static CalculationResult Calculate(PricingTemplate template, QuickEstimateInput input, DateOnly today)
    {
        var config = template.Config;
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(input.PropertyType)) missing.Add("propertyType");
        if (string.IsNullOrWhiteSpace(input.RoomOrArea)) missing.Add("roomOrArea");
        if (input.Measurements.Count == 0) missing.Add("measurements");
        var required = RequiredDimensions(template.MeasurementRule);
        for (var i = 0; i < input.Measurements.Count; i++)
        {
            var line = input.Measurements[i];
            if (line.Quantity <= 0) missing.Add($"measurements[{i}].quantity");
            foreach (var dimension in required)
            {
                var value = dimension switch { "widthM" => line.WidthM, "heightM" => line.HeightM, _ => line.DepthM };
                if (!value.HasValue || value.Value <= 0) missing.Add($"measurements[{i}].{dimension}");
            }
        }

        if (missing.Count > 0) throw new QuickEstimateInputException(missing);

        var grade = config.Grades.FirstOrDefault(g => g.Code == input.GradeCode) ?? throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", $"Grade '{input.GradeCode}' is not part of this template.");
        var complexities = input.ComplexityCodes.Distinct().OrderBy(code => code, StringComparer.Ordinal).Select(code => config.Complexities.FirstOrDefault(c => c.Code == code) ?? throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", $"Complexity '{code}' is not part of this template.")).ToList();
        var addOns = input.AddOnCodes.Distinct().OrderBy(code => code, StringComparer.Ordinal).Select(code => config.AddOns.FirstOrDefault(a => a.Code == code) ?? throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", $"Add-on '{code}' is not part of this template.")).ToList();
        if (!MeasurementConfidence.All.Contains(input.MeasurementConfidence)) throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", "The measurement confidence is invalid.");

        var complexityFactor = complexities.Aggregate(1m, (acc, c) => acc * c.Factor);
        var lineResults = new List<EstimateLineResult>();
        foreach (var line in input.Measurements)
        {
            var billable = template.MeasurementRule switch
            {
                MeasurementRule.Area => line.WidthM!.Value * line.HeightM!.Value * line.Quantity,
                MeasurementRule.Length => line.WidthM!.Value * line.Quantity,
                MeasurementRule.Volume => line.WidthM!.Value * line.HeightM!.Value * line.DepthM!.Value * line.Quantity,
                _ => line.Quantity
            };
            var baseAmount = billable * template.ReferenceRate;
            var perLineAddOns = addOns.Where(a => a.PerLine).Sum(a => a.Amount) * line.Quantity;
            var adjusted = baseAmount * grade.Factor * complexityFactor + perLineAddOns;
            lineResults.Add(new EstimateLineResult(line.LineId, line.WorkSubtype, decimal.Round(billable, 4), Round2(baseAmount), Round2(adjusted)));
        }

        var adjustedTotal = lineResults.Sum(l => l.AdjustedAmount) + addOns.Where(a => !a.PerLine).Sum(a => a.Amount);
        var net = Round2(Math.Max(adjustedTotal, template.MinimumCharge));

        var riskParts = new List<RiskPart> { new("BASE", template.BaseRangeRate) };
        if (input.MeasurementConfidence == MeasurementConfidence.Low) riskParts.Add(new RiskPart("MEASUREMENT_CONFIDENCE_LOW", config.LowConfidenceModifier));
        if (input.MeasurementConfidence == MeasurementConfidence.Medium) riskParts.Add(new RiskPart("MEASUREMENT_CONFIDENCE_MEDIUM", config.MediumConfidenceModifier));
        if (input.CustomMaterial) riskParts.Add(new RiskPart("CUSTOM_MATERIAL", config.CustomMaterialModifier));
        foreach (var c in complexities.Where(c => c.RiskModifier > 0)) riskParts.Add(new RiskPart($"COMPLEXITY_{c.Code.ToUpperInvariant()}", c.RiskModifier));
        var rawRisk = riskParts.Sum(p => p.Amount);
        var risk = Math.Min(template.MaxRangeRate, rawRisk);

        var rawLower = net * (1 - risk);
        var rawUpper = net * (1 + risk);
        var taxMultiplier = template.TaxDisplay == TaxDisplay.Inclusive ? 1 + template.TaxRate : 1m;
        var displayLower = RoundDown(rawLower * taxMultiplier, template.RoundingStep);
        var displayUpper = RoundUp(rawUpper * taxMultiplier, template.RoundingStep);
        var taxAmount = Round2(net * template.TaxRate);

        var validUntil = today.AddDays(template.ValidityDays);
        if (template.EffectiveTo.HasValue && template.EffectiveTo.Value < validUntil) validUntil = template.EffectiveTo.Value;

        var reasons = new List<string>();
        if (input.CustomMaterial) reasons.Add("CUSTOM_MATERIAL");
        if (template.Status == TemplateStatus.Calibration) reasons.Add("TEMPLATE_IN_CALIBRATION");
        if (input.MeasurementConfidence == MeasurementConfidence.Low) reasons.Add("MEASUREMENT_CONFIDENCE_LOW");
        if (rawRisk >= template.MaxRangeRate && template.MaxRangeRate > 0) reasons.Add("RANGE_AT_MAXIMUM");
        if (displayUpper > template.DirectShareLimit) reasons.Add("ABOVE_DIRECT_SHARE_LIMIT");
        var decision = reasons.Count > 0 ? ShareDecision.PendingReview : ShareDecision.Shareable;

        var snapshot = new
        {
            template = new { template.Code, template.Version, template.WorkType, template.MeasurementRule, template.UnitCode, template.ReferenceRate, template.MinimumCharge, template.BaseRangeRate, template.MaxRangeRate, template.RoundingStep, template.TaxRate, template.TaxDisplay, template.DirectShareLimit, template.ValidityDays },
            input = new { input.PropertyType, input.RoomOrArea, input.GradeCode, complexity = complexities.Select(c => c.Code).OrderBy(c => c), addOns = addOns.Select(a => a.Code).OrderBy(c => c), input.MeasurementConfidence, input.CustomMaterial },
            factors = new { gradeFactor = grade.Factor, complexityFactor },
            lines = lineResults,
            adjustedTotal = Round2(adjustedTotal),
            net,
            tax = new { template.TaxRate, taxAmount, template.TaxDisplay },
            risk = new { parts = riskParts, rawRisk, applied = risk },
            raw = new { lower = Round2(rawLower), upper = Round2(rawUpper) },
            displayed = new { lower = displayLower, upper = displayUpper },
            validUntil,
            reasons
        };
        var snapshotJson = JsonSerializer.Serialize(snapshot, Json);
        // The input hash identifies what was priced (template version + inputs), not when, so repeating a calculation is detectable.
        var canonicalInput = JsonSerializer.Serialize(new
        {
            template.Code,
            template.Version,
            input.PropertyType,
            input.RoomOrArea,
            input.GradeCode,
            complexity = complexities.Select(c => c.Code).OrderBy(c => c, StringComparer.Ordinal),
            addOns = addOns.Select(a => a.Code).OrderBy(c => c, StringComparer.Ordinal),
            input.MeasurementConfidence,
            input.CustomMaterial,
            measurements = input.Measurements.OrderBy(m => m.LineId)
        }, Json);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalInput))).ToLowerInvariant();
        return new CalculationResult(lineResults, Round2(adjustedTotal), template.MinimumCharge, net, taxAmount, risk, riskParts, Round2(rawLower), Round2(rawUpper), displayLower, displayUpper, validUntil, decision, reasons, hash, snapshotJson);
    }
}

/// <summary>Carries the names of every missing or invalid field so the client can mark them all at once.</summary>
public class QuickEstimateInputException : QuickEstimateException
{
    public IReadOnlyList<string> Fields { get; }

    public QuickEstimateInputException(IReadOnlyList<string> fields) : base("QUICK_ESTIMATE_INPUT_INCOMPLETE", "Required measurements are missing or invalid.")
    {
        Fields = fields;
    }
}
