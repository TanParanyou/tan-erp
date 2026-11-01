namespace TanErp.Application.IdentityAccess.Authentication;

public static class AuthStatusValues
{
    public const string Authenticated = "authenticated";
    public const string RequiresTwoFactor = "requires_two_factor";
    public const string RequiresTwoFactorSetup = "requires_two_factor_setup";
}

public sealed record AuthUserSummary(Guid Id, string Uid, string Email, string DisplayName);

public sealed record TotpSetupPayload(string SecretKey, string OtpAuthUri);

public sealed record AuthFlowResult(
    string Status,
    string? AccessToken = null,
    DateTimeOffset? ExpiresAtUtc = null,
    string? ClientRefreshToken = null,
    string? CookieRefreshToken = null,
    DateTimeOffset? SessionExpiresAtUtc = null,
    Guid? SessionId = null,
    AuthUserSummary? User = null,
    string? ChallengeToken = null,
    TotpSetupPayload? TotpSetup = null,
    IReadOnlyList<string>? BackupCodes = null);

public sealed record ClientRequestMetadata(
    string? ClientPlatform,
    string? IpAddress,
    string? UserAgent,
    string TraceId);

public sealed record LoginCommand(
    string? Email,
    string? Password,
    ClientRequestMetadata Metadata);

public sealed record GoogleLoginCommand(
    string? IdToken,
    ClientRequestMetadata Metadata);

public sealed record VerifyTwoFactorLoginCommand(
    string? ChallengeToken,
    string? Code,
    ClientRequestMetadata Metadata);

public sealed record RefreshSessionCommand(
    string? RefreshToken,
    string? ClientPlatform);

public sealed record LogoutCommand(
    string? AuthenticatedSubjectUid,
    Guid? AuthenticatedSessionId,
    string? RefreshToken);

public sealed record MySecurityStatus(
    bool HasPassword,
    bool HasGoogleLinked,
    bool IsTotpEnabled,
    bool RequireTwoFactor,
    int RemainingBackupCodesCount,
    DateTimeOffset? TotpEnabledAtUtc,
    DateTimeOffset? PasswordChangedAtUtc);

public sealed record ChangeMyPasswordCommand(
    string SubjectUid,
    Guid? CurrentSessionId,
    string? CurrentPassword,
    string? NewPassword);

public sealed record ActiveSessionItem(
    Guid Id,
    string DeviceLabel,
    string ClientPlatform,
    string IpAddress,
    string UserAgent,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastUsedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    bool IsCurrent);
