using System.Text.Json;
using TanErp.Domain.Common;

namespace TanErp.Domain.QuickEstimates;

/// <summary>
/// A quick, preliminary price range captured at the site. It is never a quotation: it is priced from one approved template version,
/// kept as immutable calculation versions, and only shared or converted after the share policy and (when needed) a review allow it.
/// </summary>
public class QuickEstimate : Entity
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Guid? CustomerId { get; private set; }
    public Guid? OpportunityId { get; private set; }
    public string Status { get; private set; } = QuickEstimateStatus.Draft;
    public Guid? TemplateId { get; private set; }
    public string PropertyType { get; private set; } = string.Empty;
    public string RoomOrArea { get; private set; } = string.Empty;
    public string GradeCode { get; private set; } = string.Empty;
    public string ComplexityCodesJson { get; private set; } = "[]";
    public string AddOnCodesJson { get; private set; } = "[]";
    public string MeasurementConfidence { get; private set; } = QuickEstimates.MeasurementConfidence.Medium;
    public bool CustomMaterial { get; private set; }
    public string MeasurementsJson { get; private set; } = "[]";
    public int CurrentCalculationVersion { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; }

    protected QuickEstimate() { }

    public QuickEstimate(Guid id, Guid organizationId, Guid branchId, string number, Guid? customerId, Guid? opportunityId, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, branch and actor are required.");
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number cannot be blank.", nameof(number));
        OrganizationId = organizationId;
        BranchId = branchId;
        Number = number.Trim();
        CustomerId = customerId;
        OpportunityId = opportunityId;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        UpdatedAtUtc = CreatedAtUtc;
        RowVersion = Guid.NewGuid();
    }

    public IReadOnlyList<string> ComplexityCodes => JsonSerializer.Deserialize<List<string>>(ComplexityCodesJson, Json) ?? new List<string>();

    public IReadOnlyList<string> AddOnCodes => JsonSerializer.Deserialize<List<string>>(AddOnCodesJson, Json) ?? new List<string>();

    public IReadOnlyList<MeasurementLine> Measurements => JsonSerializer.Deserialize<List<MeasurementLine>>(MeasurementsJson, Json) ?? new List<MeasurementLine>();

    public QuickEstimateInput ToInput() => new(PropertyType, RoomOrArea, GradeCode, ComplexityCodes, AddOnCodes, MeasurementConfidence, CustomMaterial, Measurements);

    /// <summary>
    /// Autosave of the draft. Any change to a priced input invalidates the calculation: the estimate goes back to draft so that nothing
    /// is shared, reviewed or converted from numbers that no longer match the inputs.
    /// </summary>
    public void UpdateDraft(
        Guid? templateId, string? propertyType, string? roomOrArea, string? gradeCode, IReadOnlyList<string>? complexityCodes, IReadOnlyList<string>? addOnCodes,
        string? measurementConfidence, bool? customMaterial, IReadOnlyList<MeasurementLine>? measurements, DateTimeOffset now)
    {
        if (Status == QuickEstimateStatus.Converted) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", "A converted quick estimate is closed.");
        if (Status == QuickEstimateStatus.PendingReview) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", "A quick estimate under review cannot be changed; wait for the decision.");
        if (measurementConfidence is not null && !QuickEstimates.MeasurementConfidence.All.Contains(measurementConfidence)) throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", "The measurement confidence is invalid.");
        if (measurements is { Count: > 50 }) throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", "At most 50 measurement lines are allowed.");
        if (propertyType is { Length: > 100 } || roomOrArea is { Length: > 200 } || gradeCode is { Length: > 40 }) throw new QuickEstimateException("QUICK_ESTIMATE_OPTION_INVALID", "A value is too long.");

        if (templateId.HasValue) TemplateId = templateId;
        if (propertyType is not null) PropertyType = propertyType.Trim();
        if (roomOrArea is not null) RoomOrArea = roomOrArea.Trim();
        if (gradeCode is not null) GradeCode = gradeCode.Trim();
        if (complexityCodes is not null) ComplexityCodesJson = JsonSerializer.Serialize(complexityCodes.Distinct().OrderBy(c => c, StringComparer.Ordinal).ToList(), Json);
        if (addOnCodes is not null) AddOnCodesJson = JsonSerializer.Serialize(addOnCodes.Distinct().OrderBy(c => c, StringComparer.Ordinal).ToList(), Json);
        if (measurementConfidence is not null) MeasurementConfidence = measurementConfidence;
        if (customMaterial.HasValue) CustomMaterial = customMaterial.Value;
        if (measurements is not null) MeasurementsJson = JsonSerializer.Serialize(measurements, Json);

        if (Status is QuickEstimateStatus.Calculated or QuickEstimateStatus.Approved or QuickEstimateStatus.Returned) Status = QuickEstimateStatus.Draft;
        Touch(now);
    }

    public void RecordCalculation(int version, DateTimeOffset now)
    {
        if (Status == QuickEstimateStatus.Converted) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", "A converted quick estimate is closed.");
        if (Status == QuickEstimateStatus.PendingReview) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", "A quick estimate under review cannot be recalculated.");
        CurrentCalculationVersion = version;
        Status = QuickEstimateStatus.Calculated;
        Touch(now);
    }

    public void SubmitForReview(DateTimeOffset now)
    {
        if (Status != QuickEstimateStatus.Calculated) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", $"Only a calculated quick estimate can be submitted; current status is '{Status}'.");
        Status = QuickEstimateStatus.PendingReview;
        Touch(now);
    }

    public void ApplyReviewDecision(bool approved, DateTimeOffset now)
    {
        if (Status != QuickEstimateStatus.PendingReview) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", $"Only a quick estimate under review can be decided; current status is '{Status}'.");
        Status = approved ? QuickEstimateStatus.Approved : QuickEstimateStatus.Returned;
        Touch(now);
    }

    public void MarkConverted(DateTimeOffset now)
    {
        if (Status == QuickEstimateStatus.Converted) return;
        if (Status is QuickEstimateStatus.Draft or QuickEstimateStatus.PendingReview or QuickEstimateStatus.Returned) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", $"A quick estimate that is '{Status}' cannot be converted.");
        Status = QuickEstimateStatus.Converted;
        Touch(now);
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}

/// <summary>One immutable pricing result. A changed input creates version N+1; earlier versions stay readable.</summary>
public class QuickEstimateCalculation : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid QuickEstimateId { get; private set; }
    public int Version { get; private set; }
    public Guid TemplateId { get; private set; }
    public string TemplateCode { get; private set; } = string.Empty;
    public int TemplateVersion { get; private set; }
    public string InputHash { get; private set; } = string.Empty;
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DisplayedLower { get; private set; }
    public decimal DisplayedUpper { get; private set; }
    public DateOnly ValidUntil { get; private set; }
    public string ShareDecision { get; private set; } = QuickEstimates.ShareDecision.Blocked;
    public string ReasonCodesJson { get; private set; } = "[]";
    public string SnapshotJson { get; private set; } = "{}";
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected QuickEstimateCalculation() { }

    public QuickEstimateCalculation(Guid id, Guid organizationId, Guid quickEstimateId, int version, PricingTemplate template, CalculationResult result, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        QuickEstimateId = quickEstimateId;
        Version = version;
        TemplateId = template.Id;
        TemplateCode = template.Code;
        TemplateVersion = template.Version;
        InputHash = result.InputHash;
        NetAmount = result.NetAmount;
        TaxAmount = result.TaxAmount;
        DisplayedLower = result.DisplayedLower;
        DisplayedUpper = result.DisplayedUpper;
        ValidUntil = result.ValidUntil;
        ShareDecision = result.ShareDecision;
        ReasonCodesJson = JsonSerializer.Serialize(result.ReasonCodes);
        SnapshotJson = result.SnapshotJson;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }
}

public class QuickEstimateReview : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid QuickEstimateId { get; private set; }
    public int SourceVersion { get; private set; }
    public string Status { get; private set; } = ReviewStatus.Requested;
    public string? RequestNote { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? ReasonCode { get; private set; }
    public string? DecisionNote { get; private set; }

    protected QuickEstimateReview() { }

    public QuickEstimateReview(Guid id, Guid organizationId, Guid quickEstimateId, int sourceVersion, string? note, Guid requestedBy, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        QuickEstimateId = quickEstimateId;
        SourceVersion = sourceVersion;
        RequestNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RequestedByUserId = requestedBy;
        RequestedAtUtc = now.ToUniversalTime();
    }

    /// <summary>Maker–checker: the person who built the estimate (or asked for the review) cannot decide it.</summary>
    public void Decide(bool approved, string reasonCode, string? note, Guid deciderUserId, Guid estimateOwnerUserId, DateTimeOffset now)
    {
        if (Status != ReviewStatus.Requested) throw new QuickEstimateException("QUICK_ESTIMATE_INVALID_STATE", "The review has already been decided.");
        if (deciderUserId == estimateOwnerUserId || deciderUserId == RequestedByUserId) throw new QuickEstimateException("QUICK_ESTIMATE_SELF_REVIEW", "The author of a quick estimate cannot review it.");
        if (string.IsNullOrWhiteSpace(reasonCode) || reasonCode.Trim().Length > 64) throw new QuickEstimateException("QUICK_ESTIMATE_REASON_REQUIRED", "A reason code is required.");
        Status = approved ? ReviewStatus.Approved : ReviewStatus.Returned;
        ReasonCode = reasonCode.Trim();
        DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
    }
}

public class QuickEstimateShare : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid QuickEstimateId { get; private set; }
    public int SourceVersion { get; private set; }
    public string Channel { get; private set; } = string.Empty;
    public string? Recipient { get; private set; }
    public string Locale { get; private set; } = "th";
    public string SummaryJson { get; private set; } = "{}";
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected QuickEstimateShare() { }

    public QuickEstimateShare(Guid id, Guid organizationId, Guid quickEstimateId, int sourceVersion, string channel, string? recipient, string locale, string summaryJson, Guid createdBy, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        QuickEstimateId = quickEstimateId;
        SourceVersion = sourceVersion;
        Channel = channel;
        Recipient = string.IsNullOrWhiteSpace(recipient) ? null : recipient.Trim();
        Locale = locale;
        SummaryJson = summaryJson;
        CreatedByUserId = createdBy;
        CreatedAtUtc = now.ToUniversalTime();
    }
}

public class QuickEstimateConversion : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid QuickEstimateId { get; private set; }
    public int SourceVersion { get; private set; }
    public Guid OfficialEstimateId { get; private set; }
    public string SourceSnapshotJson { get; private set; } = "{}";
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected QuickEstimateConversion() { }

    public QuickEstimateConversion(Guid id, Guid organizationId, Guid quickEstimateId, int sourceVersion, Guid officialEstimateId, string sourceSnapshotJson, Guid createdBy, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        QuickEstimateId = quickEstimateId;
        SourceVersion = sourceVersion;
        OfficialEstimateId = officialEstimateId;
        SourceSnapshotJson = sourceSnapshotJson;
        CreatedByUserId = createdBy;
        CreatedAtUtc = now.ToUniversalTime();
    }
}
