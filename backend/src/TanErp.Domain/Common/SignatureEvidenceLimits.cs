namespace TanErp.Domain.Common;

/// <summary>Limits for signature evidence shared by External Acceptance (CP-07) and shared signature capture (G-01).</summary>
public static class SignatureEvidenceLimits
{
    public const int MinSignerNameLength = 2;
    public const int MaxSignerNameLength = 200;
    public const int MaxSignerRoleLength = 100;
    public const int MaxConsentVersionLength = 32;
    public const int MaxImageChars = 150_000;
    public const int Sha256HexLength = 64;
}
