namespace TanErp.Domain.Estimates;

public sealed class TaxPolicyVersion
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string PolicyCode { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public string Status { get; private set; } = EstimatePolicyStatus.Draft;
    public string TaxMode { get; private set; } = string.Empty;
    public decimal TaxRate { get; private set; }
    public string TaxCode { get; private set; } = string.Empty;
    public DateTimeOffset EffectiveFromUtc { get; private set; }
    public DateTimeOffset? EffectiveToUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? PublishedByUserId { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public string? ContentHash { get; private set; }

    private TaxPolicyVersion() { }

    public TaxPolicyVersion(
        Guid id, Guid organizationId, Guid? branchId, string policyCode, int version,
        string taxMode, decimal taxRate, string taxCode,
        DateTimeOffset effectiveFromUtc, DateTimeOffset? effectiveToUtc,
        Guid createdByUserId)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Policy identifiers must be provided.");
        if (string.IsNullOrWhiteSpace(policyCode) || version <= 0 || string.IsNullOrWhiteSpace(taxCode))
            throw new ArgumentException("Policy code, positive version and tax code are required.");
        if (taxMode is not ("exclusive" or "inclusive" or "exempt"))
            throw new ArgumentException("Tax mode is unsupported.", nameof(taxMode));
        if (taxRate < 0m || taxRate > 1m || (taxMode == "exempt" && taxRate != 0m))
            throw new ArgumentOutOfRangeException(nameof(taxRate));
        if (effectiveToUtc is not null && effectiveToUtc <= effectiveFromUtc)
            throw new ArgumentException("Policy effective period is invalid.");

        Id = id;
        OrganizationId = organizationId;
        BranchId = branchId;
        PolicyCode = policyCode.Trim().ToUpperInvariant();
        Version = version;
        TaxMode = taxMode;
        TaxRate = taxRate;
        TaxCode = taxCode.Trim().ToUpperInvariant();
        EffectiveFromUtc = effectiveFromUtc;
        EffectiveToUtc = effectiveToUtc;
        CreatedByUserId = createdByUserId;
    }

    public void Publish(Guid publisherUserId, DateTimeOffset publishedAtUtc)
    {
        if (Status != EstimatePolicyStatus.Draft)
            throw new EstimateInvalidStateException("Only a draft tax policy can be published.");
        if (publisherUserId == Guid.Empty || publisherUserId == CreatedByUserId)
            throw new EstimateInvalidStateException("Tax policy publication requires an independent checker.");

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
        var value = FormattableString.Invariant($"{OrganizationId:N}|{BranchId:N}|{PolicyCode}|{Version}|{TaxMode}|{TaxRate}|{TaxCode}|{EffectiveFromUtc:O}|{EffectiveToUtc:O}");
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }
}
