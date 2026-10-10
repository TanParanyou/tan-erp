using System.Text.RegularExpressions;
using TanErp.Domain.Common;

namespace TanErp.Application.Common.Security;

/// <summary>
/// Signature evidence rules shared by External Acceptance (CP-07) and shared signature capture (G-01).
/// Behavior is intentionally identical to what CP-07 shipped; change both consumers' tests before changing it.
/// </summary>
public static class SignatureEvidenceRules
{
    private const string PngDataUrlPrefix = "data:image/png;base64,";
    private static readonly Regex LowerSha256 = new("^[0-9a-f]{64}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>Validates and normalizes the signer. The role is measured before trimming, exactly as CP-07 did.</summary>
    public static bool TryNormalizeSigner(string? name, string? role, out string signerName, out string? signerRole)
    {
        signerName = name?.Trim() ?? string.Empty;
        signerRole = string.IsNullOrWhiteSpace(role) ? null : role.Trim();

        var nameOk = signerName.Length is >= SignatureEvidenceLimits.MinSignerNameLength and <= SignatureEvidenceLimits.MaxSignerNameLength;
        var roleOk = role is null || role.Length <= SignatureEvidenceLimits.MaxSignerRoleLength;
        return nameOk && roleOk;
    }

    /// <summary>True when the bytes start with the PNG signature (first four bytes) and are longer than eight bytes.</summary>
    public static bool IsPngSignature(ReadOnlySpan<byte> bytes) =>
        bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;

    /// <summary>Accepts a raw base64 payload or a PNG data URL; the payload must decode to a PNG.</summary>
    public static bool IsPngBase64(string value)
    {
        var payload = value.StartsWith(PngDataUrlPrefix, StringComparison.Ordinal) ? value[PngDataUrlPrefix.Length..] : value;
        try
        {
            return IsPngSignature(Convert.FromBase64String(payload));
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>SHA-256 of the submitted string (CP-07 hashes the base64 text it received, not the decoded bytes).</summary>
    public static string HashSubmittedImage(string submittedImage) => Sha256Hex.Compute(submittedImage);

    public static bool IsSha256Hex(string? value) => value is not null && LowerSha256.IsMatch(value);
}
