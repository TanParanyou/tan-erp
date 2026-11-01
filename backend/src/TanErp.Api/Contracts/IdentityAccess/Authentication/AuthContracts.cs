using TanErp.Application.IdentityAccess.Authentication;

namespace TanErp.Api.Contracts.IdentityAccess.Authentication;

public sealed record LoginRequest(
    string? Email,
    string? Password,
    string? ClientPlatform = null);

public sealed record GoogleLoginRequest(
    string? IdToken,
    string? ClientPlatform = null);

public sealed record VerifyTwoFactorLoginRequest(
    string? ChallengeToken,
    string? Code,
    string? ClientPlatform = null);

public sealed record RefreshSessionRequest(
    string? RefreshToken = null,
    string? ClientPlatform = null);

public sealed record LogoutRequest(
    string? RefreshToken = null);

public sealed record AuthUserSummaryDto(
    Guid Id,
    string Uid,
    string Email,
    string DisplayName);

public sealed record TotpSetupPayloadDto(
    string SecretKey,
    string OtpAuthUri);

public sealed record AuthResultResponse(
    string Status,
    string? AccessToken,
    DateTimeOffset? ExpiresAtUtc,
    string? RefreshToken,
    Guid? SessionId,
    AuthUserSummaryDto? User,
    string? ChallengeToken,
    TotpSetupPayloadDto? TotpSetup,
    IReadOnlyList<string>? BackupCodes)
{
    public static AuthResultResponse From(AuthFlowResult result) => new(
        result.Status,
        result.AccessToken,
        result.ExpiresAtUtc,
        result.ClientRefreshToken,
        result.SessionId,
        result.User is null
            ? null
            : new AuthUserSummaryDto(result.User.Id, result.User.Uid, result.User.Email, result.User.DisplayName),
        result.ChallengeToken,
        result.TotpSetup is null
            ? null
            : new TotpSetupPayloadDto(result.TotpSetup.SecretKey, result.TotpSetup.OtpAuthUri),
        result.BackupCodes);
}
