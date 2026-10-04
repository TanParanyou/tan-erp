using TanErp.Domain.QuickEstimates;
using Xunit;

namespace TanErp.UnitTests.QuickEstimates;

public class QuickEstimateEngineTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Maker = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 4, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = new(2026, 10, 4);

    // TEST_ONLY values: they exercise the formula and are not business rates.
    private static TemplateConfig Config() => new(
        new[] { new TemplateGrade("standard", "Standard", 1.0m), new TemplateGrade("premium", "Premium", 1.3m) },
        new[] { new TemplateComplexity("curved", "Curved work", 1.2m, 0.03m), new TemplateComplexity("hidden", "Hidden system", 1.1m, 0.05m) },
        new[] { new TemplateAddOn("delivery", "Delivery", 5000m, false), new TemplateAddOn("hardware", "Hardware per line", 2000m, true) },
        0.08m, 0.02m, 0.06m,
        new[] { "Measurements are preliminary." }, new[] { "Demolition" });

    private static PricingTemplate Template(string status = TemplateStatus.Active, decimal directShareLimit = 200000m, string taxDisplay = TaxDisplay.Exclusive, string rule = MeasurementRule.Area, DateOnly? effectiveTo = null)
    {
        var t = new PricingTemplate(
            Guid.NewGuid(), Org, "QE-BI-WARDROBE", 1, WorkType.BuiltIn, "Wardrobe", rule, "m2", 8000m, 20000m, 0.10m, 0.25m, 1000m, 7, 0.07m, taxDisplay, directShareLimit, null, effectiveTo, Config(), Maker, Now);
        // Walk the lifecycle with a different approver so the status is real.
        if (status != TemplateStatus.Draft)
        {
            t.Submit(Maker, Now);
            t.Decide(true, null, Guid.NewGuid(), Now);
        }

        if (status is TemplateStatus.Calibration) t.StartCalibration(Now);
        if (status is TemplateStatus.Active) t.Activate(Now);
        return t;
    }

    private static QuickEstimateInput Input(string grade = "premium", string confidence = MeasurementConfidence.Medium, bool custom = false, string[]? complexity = null, string[]? addOns = null, params MeasurementLine[] lines) =>
        new("house", "ห้องนอน", grade, complexity ?? Array.Empty<string>(), addOns ?? new[] { "delivery" }, confidence, custom,
            lines.Length > 0 ? lines : new[] { new MeasurementLine(Guid.Parse("00000000-0000-0000-0000-000000000001"), "wardrobe", 3.00m, 2.60m, null, 1) });

    [Fact]
    public void Calculates_AreaRate_GradeFactor_AddOns_AndOutwardRoundedRange()
    {
        var result = QuickEstimateEngine.Calculate(Template(), Input(), Today);

        // 3.0 × 2.6 = 7.8 m² × 8,000 × 1.3 = 81,120 + 5,000 delivery = 86,120.
        Assert.Equal(7.8m, result.Lines.Single().BillableQuantity);
        Assert.Equal(81120m, result.Lines.Single().AdjustedAmount);
        Assert.Equal(86120m, result.NetAmount);
        // Risk = base 10% + medium confidence 2% = 12%.
        Assert.Equal(0.12m, result.RiskRate);
        Assert.Equal(75785.60m, result.RawLower);
        Assert.Equal(96454.40m, result.RawUpper);
        // Customer-facing bounds round outward: lower down, upper up.
        Assert.Equal((75000m, 97000m), (result.DisplayedLower, result.DisplayedUpper));
        Assert.Equal(Today.AddDays(7), result.ValidUntil);
        Assert.Equal(6028.40m, result.TaxAmount);
        Assert.Equal(ShareDecision.Shareable, result.ShareDecision);
    }

    [Fact]
    public void AppliesComplexityFactors_PerLineAddOns_AndTheMinimumCharge()
    {
        var curved = QuickEstimateEngine.Calculate(Template(), Input("standard", complexity: new[] { "curved" }, addOns: new[] { "hardware" }), Today);
        // 7.8 × 8,000 = 62,400 × 1.0 × 1.2 = 74,880 + 2,000 per line (qty 1).
        Assert.Equal(76880m, curved.Lines.Single().AdjustedAmount);
        Assert.Contains(curved.RiskParts, p => p.Code == "COMPLEXITY_CURVED" && p.Amount == 0.03m);

        var tiny = QuickEstimateEngine.Calculate(Template(), Input("standard", addOns: Array.Empty<string>(), lines: new MeasurementLine(Guid.NewGuid(), null, 0.5m, 0.5m, null, 1)), Today);
        Assert.Equal(20000m, tiny.NetAmount);
    }

    [Fact]
    public void RiskRate_IsCapped_AndWideRangeNeedsReview()
    {
        var result = QuickEstimateEngine.Calculate(Template(), Input(confidence: MeasurementConfidence.Low, custom: true, complexity: new[] { "curved", "hidden" }), Today);
        // 10% + 8% + 6% + 3% + 5% = 32% > the 25% maximum.
        Assert.Equal(0.25m, result.RiskRate);
        Assert.Equal(ShareDecision.PendingReview, result.ShareDecision);
        Assert.Contains("RANGE_AT_MAXIMUM", result.ReasonCodes);
        Assert.Contains("CUSTOM_MATERIAL", result.ReasonCodes);
        Assert.Contains("MEASUREMENT_CONFIDENCE_LOW", result.ReasonCodes);
    }

    [Fact]
    public void ReviewIsRequired_InCalibration_AndAboveTheDirectShareLimit()
    {
        Assert.Contains("TEMPLATE_IN_CALIBRATION", QuickEstimateEngine.Calculate(Template(TemplateStatus.Calibration), Input(), Today).ReasonCodes);
        var big = QuickEstimateEngine.Calculate(Template(directShareLimit: 90000m), Input(), Today);
        Assert.Equal((ShareDecision.PendingReview, "ABOVE_DIRECT_SHARE_LIMIT"), (big.ShareDecision, big.ReasonCodes.Single()));
    }

    [Fact]
    public void InclusiveTaxDisplay_AddsVatToTheBounds_AndValidityNeverOutlivesTheTemplate()
    {
        var inclusive = QuickEstimateEngine.Calculate(Template(taxDisplay: TaxDisplay.Inclusive, effectiveTo: Today.AddDays(3)), Input(), Today);
        Assert.Equal((81000m, 104000m), (inclusive.DisplayedLower, inclusive.DisplayedUpper));
        Assert.Equal(Today.AddDays(3), inclusive.ValidUntil);
    }

    [Fact]
    public void IncompleteInput_ListsEveryMissingField()
    {
        var ex = Assert.Throws<QuickEstimateInputException>(() => QuickEstimateEngine.Calculate(Template(), Input(lines: new MeasurementLine(Guid.NewGuid(), null, 3m, null, null, 0)), Today));
        Assert.Equal("QUICK_ESTIMATE_INPUT_INCOMPLETE", ex.Code);
        Assert.Contains("measurements[0].heightM", ex.Fields);
        Assert.Contains("measurements[0].quantity", ex.Fields);
        Assert.Contains("measurements", Assert.Throws<QuickEstimateInputException>(() => QuickEstimateEngine.Calculate(Template(), new QuickEstimateInput("house", "x", "premium", Array.Empty<string>(), Array.Empty<string>(), "medium", false, Array.Empty<MeasurementLine>()), Today)).Fields);
    }

    [Fact]
    public void UnknownGradeOrOption_IsRefused()
    {
        Assert.Equal("QUICK_ESTIMATE_OPTION_INVALID", Assert.Throws<QuickEstimateException>(() => QuickEstimateEngine.Calculate(Template(), Input("gold"), Today)).Code);
        Assert.Equal("QUICK_ESTIMATE_OPTION_INVALID", Assert.Throws<QuickEstimateException>(() => QuickEstimateEngine.Calculate(Template(), Input(complexity: new[] { "underwater" }), Today)).Code);
        Assert.Equal("QUICK_ESTIMATE_OPTION_INVALID", Assert.Throws<QuickEstimateException>(() => QuickEstimateEngine.Calculate(Template(), Input(confidence: "guess"), Today)).Code);
    }

    [Fact]
    public void Recalculating_GivesTheSameResult_AndTheInputHashIgnoresInputOrder()
    {
        var template = Template();
        var a = QuickEstimateEngine.Calculate(template, Input(complexity: new[] { "curved", "hidden" }), Today);
        var b = QuickEstimateEngine.Calculate(template, Input(complexity: new[] { "hidden", "curved" }), Today);
        Assert.Equal((a.DisplayedLower, a.DisplayedUpper, a.InputHash, a.SnapshotJson), (b.DisplayedLower, b.DisplayedUpper, b.InputHash, b.SnapshotJson));
        // Another day changes the validity in the snapshot but not what was priced.
        var later = QuickEstimateEngine.Calculate(template, Input(complexity: new[] { "curved", "hidden" }), Today.AddDays(2));
        Assert.Equal(a.InputHash, later.InputHash);
        Assert.NotEqual(a.SnapshotJson, later.SnapshotJson);
        Assert.NotEqual(a.InputHash, QuickEstimateEngine.Calculate(template, Input("standard"), Today).InputHash);
    }

    [Theory]
    [InlineData(MeasurementRule.Length, 3.0, 2.6, 2.0, 1, 3.0)]
    [InlineData(MeasurementRule.Volume, 3.0, 2.0, 0.5, 2, 6.0)]
    [InlineData(MeasurementRule.Count, 3.0, 2.0, 0.5, 4, 4.0)]
    public void MeasurementRules_DeriveTheBillableQuantity(string rule, double w, double h, double d, int qty, double expected)
    {
        var line = new MeasurementLine(Guid.NewGuid(), null, (decimal)w, (decimal)h, (decimal)d, qty);
        var result = QuickEstimateEngine.Calculate(Template(rule: rule), Input(lines: line), Today);
        Assert.Equal((decimal)expected, result.Lines.Single().BillableQuantity);
    }

    [Fact]
    public void TemplateLifecycle_NeedsADifferentApprover_AndOnlyDraftsCanBeEdited()
    {
        var t = Template(TemplateStatus.Draft);
        t.Submit(Maker, Now);
        Assert.Equal("PRICING_TEMPLATE_SELF_APPROVAL", Assert.Throws<QuickEstimateException>(() => t.Decide(true, null, Maker, Now)).Code);
        Assert.Equal("QUICK_ESTIMATE_REASON_REQUIRED", Assert.Throws<QuickEstimateException>(() => t.Decide(false, " ", Guid.NewGuid(), Now)).Code);
        t.Decide(false, "ปรับ rate", Guid.NewGuid(), Now);
        Assert.Equal(TemplateStatus.Draft, t.Status);
        t.Submit(Maker, Now);
        t.Decide(true, null, Guid.NewGuid(), Now);
        Assert.Equal("PRICING_TEMPLATE_INVALID_STATE", Assert.Throws<QuickEstimateException>(() => t.Edit(WorkType.BuiltIn, "x", MeasurementRule.Area, "m2", 1m, 0m, 0m, 0m, 1m, 7, 0m, TaxDisplay.Exclusive, 0m, null, null, Config(), Now)).Code);
        Assert.False(t.IsUsableOn(Today));
        t.StartCalibration(Now);
        Assert.True(t.IsUsableOn(Today));
        t.Disable(Now);
        Assert.False(t.IsUsableOn(Today));
    }

    [Fact]
    public void Template_RejectsUnusableConfiguration()
    {
        Assert.Equal("PRICING_TEMPLATE_INVALID", Assert.Throws<QuickEstimateException>(() => new PricingTemplate(Guid.NewGuid(), Org, "X", 1, WorkType.BuiltIn, "x", MeasurementRule.Area, "m2", 0m, 0m, 0m, 0m, 1m, 7, 0m, TaxDisplay.Exclusive, 0m, null, null, Config(), Maker, Now)).Code);
        Assert.Equal("PRICING_TEMPLATE_INVALID", Assert.Throws<QuickEstimateException>(() => new PricingTemplate(Guid.NewGuid(), Org, "X", 1, WorkType.BuiltIn, "x", MeasurementRule.Area, "m2", 100m, 0m, 0.3m, 0.2m, 1m, 7, 0m, TaxDisplay.Exclusive, 0m, null, null, Config(), Maker, Now)).Code);
        Assert.Equal("PRICING_TEMPLATE_INVALID", Assert.Throws<QuickEstimateException>(() => new PricingTemplate(Guid.NewGuid(), Org, "X", 1, WorkType.BuiltIn, "x", MeasurementRule.Area, "m2", 100m, 0m, 0.1m, 0.2m, 1m, 7, 0m, TaxDisplay.Exclusive, 0m, null, null, TemplateConfig.Empty, Maker, Now)).Code);
    }
}
