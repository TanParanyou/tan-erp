namespace TanErp.Domain.Estimates;

public sealed class EstimateCalculationSnapshot
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid EstimateRevisionId { get; private set; }
    public int CalculationVersion { get; private set; }
    public string InputHash { get; private set; } = string.Empty;
    public string SnapshotJson { get; private set; } = string.Empty;
    public Guid? CalculationPolicyVersionId { get; private set; }
    public Guid? TaxPolicyVersionId { get; private set; }
    public string CalculationPolicyVersion { get; private set; } = string.Empty;
    public string TaxPolicyVersion { get; private set; } = string.Empty;
    public string? CalculationPolicyHash { get; private set; }
    public string? TaxPolicyHash { get; private set; }
    public Guid CapturedByUserId { get; private set; }
    public DateTimeOffset CapturedAtUtc { get; private set; }

    private EstimateCalculationSnapshot() { }

    public EstimateCalculationSnapshot(
        Guid id,
        Guid organizationId,
        Guid estimateRevisionId,
        int calculationVersion,
        string inputHash,
        string snapshotJson,
        CalculationPolicyVersion calculationPolicy,
        TaxPolicyVersion taxPolicy,
        Guid capturedByUserId,
        DateTimeOffset capturedAtUtc)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || estimateRevisionId == Guid.Empty || capturedByUserId == Guid.Empty)
            throw new ArgumentException("Snapshot identifiers must be provided.");
        if (calculationVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(calculationVersion));
        if (string.IsNullOrWhiteSpace(inputHash) || string.IsNullOrWhiteSpace(snapshotJson))
            throw new ArgumentException("Input hash and snapshot payload are required.");
        ArgumentNullException.ThrowIfNull(calculationPolicy);
        ArgumentNullException.ThrowIfNull(taxPolicy);
        if (calculationPolicy.OrganizationId != organizationId || taxPolicy.OrganizationId != organizationId ||
            calculationPolicy.ContentHash is null || taxPolicy.ContentHash is null)
            throw new ArgumentException("Published policies from the snapshot organization are required.");

        Id = id;
        OrganizationId = organizationId;
        EstimateRevisionId = estimateRevisionId;
        CalculationVersion = calculationVersion;
        InputHash = inputHash.Trim();
        SnapshotJson = snapshotJson;
        CalculationPolicyVersionId = calculationPolicy.Id;
        TaxPolicyVersionId = taxPolicy.Id;
        CalculationPolicyVersion = $"{calculationPolicy.PolicyCode}-v{calculationPolicy.Version}";
        TaxPolicyVersion = $"{taxPolicy.PolicyCode}-v{taxPolicy.Version}";
        CalculationPolicyHash = calculationPolicy.ContentHash;
        TaxPolicyHash = taxPolicy.ContentHash;
        CapturedByUserId = capturedByUserId;
        CapturedAtUtc = capturedAtUtc;
    }
}
