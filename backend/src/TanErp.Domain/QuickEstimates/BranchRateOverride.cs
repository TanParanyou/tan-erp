using TanErp.Domain.Common;

namespace TanErp.Domain.QuickEstimates;

public static class OverrideStatus
{
    public const string Requested = "requested", Approved = "approved", Returned = "returned", Revoked = "revoked";
}

/// <summary>The rate the engine prices with instead of a template's standard rate (one approved branch override).</summary>
public sealed record RateOverride(Guid Id, decimal ReferenceRate, decimal MinimumCharge);

/// <summary>
/// A branch-specific reference rate and minimum charge for one template version over a limited period; it never edits the template.
/// Maker–checker: the requester cannot decide it. It applies only to quick estimates of its own branch while approved and in period.
/// </summary>
public class BranchRateOverride : Entity
{
    public const int MaxPeriodDays = 366;
    public Guid OrganizationId { get; private set; } public Guid BranchId { get; private set; }
    public Guid TemplateId { get; private set; } public string Status { get; private set; } = OverrideStatus.Requested;
    public decimal ReferenceRate { get; private set; } public decimal MinimumCharge { get; private set; }
    public DateOnly EffectiveFrom { get; private set; } public DateOnly EffectiveTo { get; private set; }
    public string ReasonCode { get; private set; } = string.Empty; public string ReasonDetail { get; private set; } = string.Empty;
    public string? SourceReference { get; private set; } public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; } public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; } public string? DecisionNote { get; private set; }
    public Guid? ClosedByUserId { get; private set; } public DateTimeOffset? ClosedAtUtc { get; private set; }
    public string? CloseNote { get; private set; } public Guid RowVersion { get; private set; }

    protected BranchRateOverride() { }

    public BranchRateOverride(
        Guid id, Guid organizationId, Guid branchId, Guid templateId, decimal referenceRate, decimal minimumCharge, DateOnly effectiveFrom, DateOnly effectiveTo,
        string? reasonCode, string? reasonDetail, string? sourceReference, Guid requestedByUserId, DateOnly today, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || templateId == Guid.Empty || requestedByUserId == Guid.Empty) throw new ArgumentException("Organization, branch, template and actor are required.");
        if (referenceRate <= 0 || referenceRate > 99_999_999m) throw Invalid("The reference rate must be above zero.");
        if (minimumCharge < 0 || minimumCharge > 99_999_999m) throw Invalid("The minimum charge cannot be negative.");
        if (effectiveTo < effectiveFrom || effectiveTo < today) throw Invalid("The override period is invalid or has already ended.");
        if (effectiveTo.DayNumber - effectiveFrom.DayNumber + 1 > MaxPeriodDays) throw Invalid($"An override can last at most {MaxPeriodDays} days.");
        var code = Clean(reasonCode);
        var detail = Clean(reasonDetail);
        var source = Clean(sourceReference);
        if (code is null or { Length: > 64 } || detail is null or { Length: > 500 } || source is { Length: > 200 }) throw Invalid("A reason code (max 64) and a reason detail (max 500) are required.");
        OrganizationId = organizationId; BranchId = branchId; TemplateId = templateId; ReferenceRate = referenceRate; MinimumCharge = minimumCharge;
        EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo; ReasonCode = code; ReasonDetail = detail; SourceReference = source;
        RequestedByUserId = requestedByUserId; RequestedAtUtc = now.ToUniversalTime(); RowVersion = Guid.NewGuid();
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static QuickEstimateException Invalid(string message) => new("BRANCH_OVERRIDE_INVALID", message);

    private void Require(string status)
    {
        if (Status != status) throw new QuickEstimateException("BRANCH_OVERRIDE_INVALID_STATE", $"Cannot do this to an override that is '{Status}'.");
    }

    /// <summary>Maker–checker: the requester cannot decide. A returned override is final; the branch files a new request.</summary>
    public void Decide(bool approve, string? note, Guid deciderUserId, DateOnly today, DateTimeOffset now)
    {
        Require(OverrideStatus.Requested);
        if (deciderUserId == RequestedByUserId) throw new QuickEstimateException("BRANCH_OVERRIDE_SELF_APPROVAL", "The requester of an override cannot decide it.");
        var text = Clean(note);
        if (text is { Length: > 500 }) throw Invalid("The note cannot exceed 500 characters.");
        if (!approve && text is null) throw new QuickEstimateException("QUICK_ESTIMATE_REASON_REQUIRED", "A reason is required to return an override.");
        if (approve && EffectiveTo < today) throw Invalid("The override period has already ended.");
        Status = approve ? OverrideStatus.Approved : OverrideStatus.Returned;
        DecidedByUserId = deciderUserId; DecidedAtUtc = now.ToUniversalTime(); DecisionNote = text; RowVersion = Guid.NewGuid();
    }

    public void Revoke(string? reason, Guid actorUserId, DateTimeOffset now)
    {
        Require(OverrideStatus.Approved);
        var text = Clean(reason) ?? throw new QuickEstimateException("QUICK_ESTIMATE_REASON_REQUIRED", "A reason is required to revoke an override.");
        if (text.Length > 500) throw Invalid("The reason cannot exceed 500 characters.");
        Status = OverrideStatus.Revoked;
        ClosedByUserId = actorUserId; ClosedAtUtc = now.ToUniversalTime(); CloseNote = text; RowVersion = Guid.NewGuid();
    }
}
