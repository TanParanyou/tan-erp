using System.Text.Json;
using TanErp.Domain.Common;

namespace TanErp.Domain.QuickEstimates;

public sealed record TemplateGrade(string Code, string Name, decimal Factor);

public sealed record TemplateComplexity(string Code, string Name, decimal Factor, decimal RiskModifier);

public sealed record TemplateAddOn(string Code, string Name, decimal Amount, bool PerLine);

/// <summary>Everything about a template that changes a price. Stored as one JSON document so a version is one immutable unit.</summary>
public sealed record TemplateConfig(
    IReadOnlyList<TemplateGrade> Grades,
    IReadOnlyList<TemplateComplexity> Complexities,
    IReadOnlyList<TemplateAddOn> AddOns,
    decimal LowConfidenceModifier,
    decimal MediumConfidenceModifier,
    decimal CustomMaterialModifier,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Exclusions)
{
    public static TemplateConfig Empty { get; } = new(Array.Empty<TemplateGrade>(), Array.Empty<TemplateComplexity>(), Array.Empty<TemplateAddOn>(), 0m, 0m, 0m, Array.Empty<string>(), Array.Empty<string>());

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public string ToJson() => JsonSerializer.Serialize(this, Json);

    public static TemplateConfig FromJson(string json) => JsonSerializer.Deserialize<TemplateConfig>(json, Json) ?? Empty;

    /// <summary>Rejects unusable factors and codes before a template can even be saved.</summary>
    public void Validate()
    {
        if (Grades.Count == 0 || Grades.Count > 20) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "A template needs 1-20 material grades.");
        if (Grades.Any(g => string.IsNullOrWhiteSpace(g.Code) || string.IsNullOrWhiteSpace(g.Name) || g.Factor < 0.1m || g.Factor > 10m)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Grade factors must be between 0.1 and 10.");
        if (Complexities.Count > 20 || Complexities.Any(c => string.IsNullOrWhiteSpace(c.Code) || c.Factor < 0.5m || c.Factor > 5m || c.RiskModifier < 0m || c.RiskModifier > 0.5m)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Complexity factors must be 0.5-5 with a risk modifier of 0-0.5.");
        if (AddOns.Count > 30 || AddOns.Any(a => string.IsNullOrWhiteSpace(a.Code) || a.Amount < 0m || a.Amount > 99_999_999m)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Add-ons need a code and a non-negative amount.");
        if (new[] { LowConfidenceModifier, MediumConfidenceModifier, CustomMaterialModifier }.Any(m => m < 0m || m > 0.5m)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Risk modifiers must be between 0 and 0.5.");
        var codes = Grades.Select(g => g.Code).Concat(Complexities.Select(c => c.Code)).Concat(AddOns.Select(a => a.Code)).ToList();
        if (Grades.Select(g => g.Code).Distinct().Count() != Grades.Count || Complexities.Select(c => c.Code).Distinct().Count() != Complexities.Count || AddOns.Select(a => a.Code).Distinct().Count() != AddOns.Count)
        {
            throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Grade, complexity and add-on codes must be unique within their lists.");
        }

        if (Assumptions.Count > 20 || Exclusions.Count > 20 || Assumptions.Concat(Exclusions).Any(t => string.IsNullOrWhiteSpace(t) || t.Length > 300)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Assumptions and exclusions are limited to 20 lines of up to 300 characters.");
        _ = codes;
    }
}

/// <summary>
/// One version of the pricing rules for one work type. Content never changes after submission; a change in any rate, factor or
/// policy is a new version. Lifecycle: draft → submitted → approved → calibration → active → superseded/disabled.
/// </summary>
public class PricingTemplate : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public string WorkType { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Status { get; private set; } = TemplateStatus.Draft;
    public string MeasurementRule { get; private set; } = QuickEstimates.MeasurementRule.Area;
    public string UnitCode { get; private set; } = string.Empty;
    public decimal ReferenceRate { get; private set; }
    public decimal MinimumCharge { get; private set; }
    public decimal BaseRangeRate { get; private set; }
    public decimal MaxRangeRate { get; private set; }
    public decimal RoundingStep { get; private set; }
    public int ValidityDays { get; private set; }
    public decimal TaxRate { get; private set; }
    public string TaxDisplay { get; private set; } = QuickEstimates.TaxDisplay.Exclusive;
    public decimal DirectShareLimit { get; private set; }
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string ConfigJson { get; private set; } = "{}";
    public Guid CreatedByUserId { get; private set; }
    public Guid? SubmittedByUserId { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public string? DecisionNote { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected PricingTemplate() { }

    public PricingTemplate(
        Guid id, Guid organizationId, string code, int version, string workType, string name, string measurementRule, string unitCode,
        decimal referenceRate, decimal minimumCharge, decimal baseRangeRate, decimal maxRangeRate, decimal roundingStep, int validityDays,
        decimal taxRate, string taxDisplay, decimal directShareLimit, DateOnly? effectiveFrom, DateOnly? effectiveTo, TemplateConfig config,
        Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization and actor are required.");
        OrganizationId = organizationId;
        Version = version;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        Code = code?.Trim().ToUpperInvariant() ?? string.Empty;
        Apply(workType, name, measurementRule, unitCode, referenceRate, minimumCharge, baseRangeRate, maxRangeRate, roundingStep, validityDays, taxRate, taxDisplay, directShareLimit, effectiveFrom, effectiveTo, config);
        RowVersion = Guid.NewGuid();
    }

    private void Apply(
        string workType, string name, string measurementRule, string unitCode, decimal referenceRate, decimal minimumCharge, decimal baseRangeRate, decimal maxRangeRate,
        decimal roundingStep, int validityDays, decimal taxRate, string taxDisplay, decimal directShareLimit, DateOnly? effectiveFrom, DateOnly? effectiveTo, TemplateConfig config)
    {
        if (Code.Length is 0 or > 40) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "A template code of 1-40 characters is required.");
        if (!QuickEstimates.WorkType.All.Contains(workType)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The work type is invalid.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "A name of 1-200 characters is required.");
        if (!QuickEstimates.MeasurementRule.All.Contains(measurementRule)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The measurement rule is invalid.");
        if (string.IsNullOrWhiteSpace(unitCode) || unitCode.Trim().Length > 20) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "A unit code of 1-20 characters is required.");
        if (referenceRate <= 0 || referenceRate > 99_999_999m) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The reference rate must be above zero.");
        if (minimumCharge < 0 || minimumCharge > 99_999_999m) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The minimum charge cannot be negative.");
        if (baseRangeRate < 0 || maxRangeRate < baseRangeRate || maxRangeRate > 0.9m) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Range rates need 0 <= base <= max <= 0.9.");
        if (roundingStep <= 0 || roundingStep > 1_000_000m) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The rounding step must be above zero.");
        if (validityDays is < 1 or > 365) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "Validity must be 1-365 days.");
        if (taxRate < 0 || taxRate > 0.3m || (taxDisplay != QuickEstimates.TaxDisplay.Exclusive && taxDisplay != QuickEstimates.TaxDisplay.Inclusive)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The tax rate or display policy is invalid.");
        if (directShareLimit < 0 || directShareLimit > 999_999_999m) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The direct-share limit cannot be negative.");
        if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveTo < effectiveFrom) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The effective period is invalid.");
        config.Validate();

        WorkType = workType;
        Name = name.Trim();
        MeasurementRule = measurementRule;
        UnitCode = unitCode.Trim();
        ReferenceRate = referenceRate;
        MinimumCharge = minimumCharge;
        BaseRangeRate = baseRangeRate;
        MaxRangeRate = maxRangeRate;
        RoundingStep = roundingStep;
        ValidityDays = validityDays;
        TaxRate = taxRate;
        TaxDisplay = taxDisplay;
        DirectShareLimit = directShareLimit;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        ConfigJson = config.ToJson();
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }

    private void Require(string action, params string[] statuses)
    {
        if (!statuses.Contains(Status)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID_STATE", $"Cannot {action} a template that is '{Status}'.");
    }

    public TemplateConfig Config => TemplateConfig.FromJson(ConfigJson);

    /// <summary>Only a draft or a returned (draft) version can be edited.</summary>
    public void Edit(
        string workType, string name, string measurementRule, string unitCode, decimal referenceRate, decimal minimumCharge, decimal baseRangeRate, decimal maxRangeRate,
        decimal roundingStep, int validityDays, decimal taxRate, string taxDisplay, decimal directShareLimit, DateOnly? effectiveFrom, DateOnly? effectiveTo, TemplateConfig config, DateTimeOffset now)
    {
        Require("edit", TemplateStatus.Draft);
        Apply(workType, name, measurementRule, unitCode, referenceRate, minimumCharge, baseRangeRate, maxRangeRate, roundingStep, validityDays, taxRate, taxDisplay, directShareLimit, effectiveFrom, effectiveTo, config);
        Touch(now);
    }

    public void Submit(Guid actorUserId, DateTimeOffset now)
    {
        Require("submit", TemplateStatus.Draft);
        Status = TemplateStatus.Submitted;
        SubmittedByUserId = actorUserId;
        DecisionNote = null;
        Touch(now);
    }

    /// <summary>Maker–checker: the person who created or submitted a template cannot decide it. A returned template is a draft again.</summary>
    public void Decide(bool approve, string? note, Guid deciderUserId, DateTimeOffset now)
    {
        Require("decide", TemplateStatus.Submitted);
        if (deciderUserId == CreatedByUserId || deciderUserId == SubmittedByUserId)
        {
            throw new QuickEstimateException("PRICING_TEMPLATE_SELF_APPROVAL", "The author of a template cannot decide it.");
        }

        var text = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (!approve && text is null) throw new QuickEstimateException("QUICK_ESTIMATE_REASON_REQUIRED", "A reason is required to return a template.");
        Status = approve ? TemplateStatus.Approved : TemplateStatus.Draft;
        DecidedByUserId = deciderUserId;
        DecisionNote = text;
        Touch(now);
    }

    public void StartCalibration(DateTimeOffset now)
    {
        Require("start calibration of", TemplateStatus.Approved);
        Status = TemplateStatus.Calibration;
        Touch(now);
    }

    public void Activate(DateTimeOffset now)
    {
        Require("activate", TemplateStatus.Calibration, TemplateStatus.Approved);
        Status = TemplateStatus.Active;
        Touch(now);
    }

    public void Supersede(DateTimeOffset now)
    {
        Require("supersede", TemplateStatus.Active, TemplateStatus.Calibration);
        Status = TemplateStatus.Superseded;
        Touch(now);
    }

    public void Disable(DateTimeOffset now)
    {
        Require("disable", TemplateStatus.Active, TemplateStatus.Calibration, TemplateStatus.Approved);
        Status = TemplateStatus.Disabled;
        Touch(now);
    }

    /// <summary>Usable means in a pricing state and inside its effective period on that day.</summary>
    public bool IsUsableOn(DateOnly date) =>
        TemplateStatus.IsUsable(Status) && (!EffectiveFrom.HasValue || date >= EffectiveFrom.Value) && (!EffectiveTo.HasValue || date <= EffectiveTo.Value);
}
