namespace TanErp.Domain.Estimates;

public static class EstimatePolicyStatus
{
    public const string Draft = "draft";
    public const string Published = "published";
    public const string Superseded = "superseded";
    public const string Disabled = "disabled";
}

public sealed class CalculationPolicyVersion
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string PolicyCode { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public string Status { get; private set; } = EstimatePolicyStatus.Draft;
    public string OverheadMethod { get; private set; } = "none";
    public decimal OverheadValue { get; private set; }
    public string RoundingMode { get; private set; } = "away-from-zero";
    public DateTimeOffset EffectiveFromUtc { get; private set; }
    public DateTimeOffset? EffectiveToUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? PublishedByUserId { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public string? ContentHash { get; private set; }

    private CalculationPolicyVersion() { }

    public CalculationPolicyVersion(
        Guid id, Guid organizationId, Guid? branchId, string policyCode, int version,
        string overheadMethod, decimal overheadValue, string roundingMode,
        DateTimeOffset effectiveFromUtc, DateTimeOffset? effectiveToUtc,
        Guid createdByUserId)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Policy identifiers must be provided.");
        if (string.IsNullOrWhiteSpace(policyCode) || version <= 0)
            throw new ArgumentException("Policy code and a positive version are required.");
        if (overheadMethod is not ("none" or "percent-direct-cost" or "fixed-amount"))
            throw new ArgumentException("Overhead method is unsupported.", nameof(overheadMethod));
        if (overheadValue < 0m || (overheadMethod == "percent-direct-cost" && overheadValue > 1m))
            throw new ArgumentOutOfRangeException(nameof(overheadValue));
        if (roundingMode != "away-from-zero")
            throw new ArgumentException("Rounding mode is unsupported.", nameof(roundingMode));
        if (effectiveToUtc is not null && effectiveToUtc <= effectiveFromUtc)
            throw new ArgumentException("Policy effective period is invalid.");

        Id = id;
        OrganizationId = organizationId;
        BranchId = branchId;
        PolicyCode = policyCode.Trim().ToUpperInvariant();
        Version = version;
        OverheadMethod = overheadMethod;
        OverheadValue = overheadValue;
        RoundingMode = roundingMode;
        EffectiveFromUtc = effectiveFromUtc;
        EffectiveToUtc = effectiveToUtc;
        CreatedByUserId = createdByUserId;
    }

    public void Publish(Guid publisherUserId, DateTimeOffset publishedAtUtc)
    {
        if (Status != EstimatePolicyStatus.Draft)
            throw new EstimateInvalidStateException("Only a draft calculation policy can be published.");
        if (publisherUserId == Guid.Empty || publisherUserId == CreatedByUserId)
            throw new EstimateInvalidStateException("Calculation policy publication requires an independent checker.");

        Status = EstimatePolicyStatus.Published;
        PublishedByUserId = publisherUserId;
        PublishedAtUtc = publishedAtUtc;
        ContentHash = CalculateContentHash();
    }

    public bool IsEffectiveAt(DateTimeOffset instant) =>
        Status == EstimatePolicyStatus.Published &&
        instant >= EffectiveFromUtc &&
        (EffectiveToUtc is null || instant < EffectiveToUtc.Value);

    private string CalculateContentHash()
    {
        var value = FormattableString.Invariant($"{OrganizationId:N}|{BranchId:N}|{PolicyCode}|{Version}|{OverheadMethod}|{OverheadValue}|{RoundingMode}|{EffectiveFromUtc:O}|{EffectiveToUtc:O}");
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }
}
