namespace TanErp.Application.IdentityAccess.Authentication;

public static class SelfHostedJwtConstants
{
    public const string Issuer = "tan-erp";
    public const string ApiAudience = "tan-erp-api";
    public const string TwoFactorAudience = "tan-erp-2fa";
    public const string PurposeTwoFactorVerify = "2fa_verify";
    public const string PurposeTwoFactorSetup = "2fa_setup";

    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan TwoFactorVerifyLifetime = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan TwoFactorSetupLifetime = TimeSpan.FromMinutes(10);
}

public sealed record AccessTokenIssueResult(string AccessToken, DateTimeOffset ExpiresAtUtc);

public sealed record AccessTokenClaims(
    Guid UserId,
    string SubjectUid,
    Guid SessionId,
    string Email,
    bool EmailVerified,
    DateTimeOffset ExpiresAtUtc);

public sealed record TwoFactorChallengeClaims(
    Guid UserId,
    string SubjectUid,
    string Purpose,
    DateTimeOffset ExpiresAtUtc);

public interface ISelfHostedJwtService
{
    AccessTokenIssueResult CreateAccessToken(
        Guid userId,
        string subjectUid,
        Guid sessionId,
        string email,
        DateTimeOffset now);

    AccessTokenClaims? VerifyAccessToken(string token, DateTimeOffset now);

    string CreateTwoFactorChallengeToken(
        Guid userId,
        string subjectUid,
        string purpose,
        DateTimeOffset now);

    TwoFactorChallengeClaims? VerifyTwoFactorChallengeToken(string token, DateTimeOffset now);
}
