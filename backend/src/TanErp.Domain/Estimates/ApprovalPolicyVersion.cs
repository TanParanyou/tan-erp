namespace TanErp.Domain.Estimates;

public sealed class ApprovalPolicyVersion
{
    public static ApprovalPolicyVersion BootstrapIndependentChecker { get; } = new(
        "SYSTEM_BOOTSTRAP_INDEPENDENT_CHECKER", 1, requireIndependentChecker: true);
    public static ApprovalPolicyVersion TestOnlyThailandEstimateV1 { get; } = new(
        TestOnlyEstimateApprovalPolicy.PolicyCode, TestOnlyEstimateApprovalPolicy.Version,
        requireIndependentChecker: true, isTestOnly: true);

    public string PolicyCode { get; }
    public int Version { get; }
    public bool RequireIndependentChecker { get; }
    public bool IsTestOnly { get; }
    public string ContentHash { get; }

    private ApprovalPolicyVersion(string policyCode, int version, bool requireIndependentChecker, bool isTestOnly = false)
    {
        PolicyCode = policyCode;
        Version = version;
        RequireIndependentChecker = requireIndependentChecker;
        IsTestOnly = isTestOnly;
        var canonicalValue = FormattableString.Invariant($"{policyCode}|{version}|{requireIndependentChecker}|{isTestOnly}");
        ContentHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(canonicalValue))).ToLowerInvariant();
    }
}
