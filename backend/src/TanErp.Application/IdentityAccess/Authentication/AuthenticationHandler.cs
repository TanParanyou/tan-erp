using System.Security.Cryptography;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.Authentication;

public class AuthenticationHandler
{
    private readonly IAuthenticationStore _store;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecretProtector _secretProtector;
    private readonly ITotpService _totpService;
    private readonly ISelfHostedJwtService _jwtService;
    private readonly IGoogleTokenVerifier _googleVerifier;
    private readonly IClock _clock;
    private readonly Lazy<string> _dummyPasswordHash;

    public AuthenticationHandler(
        IAuthenticationStore store,
        IPasswordHasher passwordHasher,
        ISecretProtector secretProtector,
        ITotpService totpService,
        ISelfHostedJwtService jwtService,
        IGoogleTokenVerifier googleVerifier,
        IClock clock)
    {
        _store = store;
        _passwordHasher = passwordHasher;
        _secretProtector = secretProtector;
        _totpService = totpService;
        _jwtService = jwtService;
        _googleVerifier = googleVerifier;
        _clock = clock;
        _dummyPasswordHash = new Lazy<string>(() => _passwordHasher.HashPassword("tan-erp-timing-shield-placeholder"));
    }

    public async Task<Result<AuthFlowResult>> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = User.NormalizeEmail(command.Email);
        var password = command.Password ?? string.Empty;

        if (normalizedEmail.Length == 0 || string.IsNullOrEmpty(password) || password.Length > PasswordPolicy.MaxLength)
        {
            _ = _passwordHasher.VerifyPassword("invalid", _dummyPasswordHash.Value);
            return Result<AuthFlowResult>.Failure(new Error("INVALID_CREDENTIALS", "Invalid email or password."));
        }

        var now = _clock.UtcNow;
        var user = await _store.FindUserByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            _ = _passwordHasher.VerifyPassword(password, _dummyPasswordHash.Value);
            return Result<AuthFlowResult>.Failure(new Error("INVALID_CREDENTIALS", "Invalid email or password."));
        }

        if (user.IsLockedOut(now))
        {
            return Result<AuthFlowResult>.Failure(new Error("ACCOUNT_LOCKED_OUT", "Account is temporarily locked due to failed sign-in attempts."));
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash) || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            user.RecordFailedLogin(now);
            await _store.SaveChangesAsync(cancellationToken);

            if (user.IsLockedOut(now))
            {
                return Result<AuthFlowResult>.Failure(new Error("ACCOUNT_LOCKED_OUT", "Account is temporarily locked due to failed sign-in attempts."));
            }

            return Result<AuthFlowResult>.Failure(new Error("INVALID_CREDENTIALS", "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return Result<AuthFlowResult>.Failure(new Error("USER_ACCESS_DISABLED", "User account is disabled."));
        }

        user.ResetLoginLockout();
        return await CompletePrimaryAuthAsync(user, command.Metadata, now, cancellationToken);
    }

    public async Task<Result<AuthFlowResult>> GoogleLoginAsync(GoogleLoginCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.IdToken))
        {
            return Result<AuthFlowResult>.Failure(new Error("GOOGLE_TOKEN_INVALID", "Google identity token is required."));
        }

        var verified = await _googleVerifier.VerifyIdTokenAsync(command.IdToken.Trim(), cancellationToken);
        if (verified.IsFailure)
        {
            return Result<AuthFlowResult>.Failure(verified.Error);
        }

        var identity = verified.Value!;
        var now = _clock.UtcNow;

        var user = await _store.FindUserByGoogleSubjectIdAsync(identity.SubjectId, cancellationToken);
        if (user is null)
        {
            var normalizedEmail = User.NormalizeEmail(identity.Email);
            user = await _store.FindUserByNormalizedEmailAsync(normalizedEmail, cancellationToken);
            if (user is null)
            {
                return Result<AuthFlowResult>.Failure(new Error("USER_NOT_INVITED", "User has not been invited to this workspace."));
            }

            if (!string.IsNullOrWhiteSpace(user.GoogleSubjectId)
                && !string.Equals(user.GoogleSubjectId, identity.SubjectId, StringComparison.Ordinal))
            {
                return Result<AuthFlowResult>.Failure(new Error("GOOGLE_ACCOUNT_MISMATCH", "User is already linked to a different Google account."));
            }

            if (!user.IsActive)
            {
                return Result<AuthFlowResult>.Failure(new Error("USER_ACCESS_DISABLED", "User account is disabled."));
            }

            user.LinkGoogleAccount(identity.SubjectId);
            await _store.RecordGoogleLinkedAuditAsync(user.Id, command.Metadata.TraceId, now, cancellationToken);
        }
        else if (!user.IsActive)
        {
            return Result<AuthFlowResult>.Failure(new Error("USER_ACCESS_DISABLED", "User account is disabled."));
        }

        user.ResetLoginLockout();
        return await CompletePrimaryAuthAsync(user, command.Metadata, now, cancellationToken);
    }

    public async Task<Result<AuthFlowResult>> VerifyTwoFactorLoginAsync(
        VerifyTwoFactorLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.ChallengeToken))
        {
            return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "Two-factor challenge token is required."));
        }

        var now = _clock.UtcNow;
        var claims = _jwtService.VerifyTwoFactorChallengeToken(command.ChallengeToken.Trim(), now);
        if (claims is null)
        {
            return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "Two-factor challenge token is invalid or expired."));
        }

        var user = await _store.FindUserByIdAsync(claims.UserId, cancellationToken);
        if (user is null)
        {
            return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "Two-factor challenge user not found."));
        }

        if (!user.IsActive)
        {
            return Result<AuthFlowResult>.Failure(new Error("USER_ACCESS_DISABLED", "User account is disabled."));
        }

        if (string.IsNullOrWhiteSpace(command.Code))
        {
            return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Verification code is required."));
        }

        var rawCode = command.Code.Trim();

        if (claims.Purpose == SelfHostedJwtConstants.PurposeTwoFactorVerify)
        {
            if (!user.IsTotpEnabled || string.IsNullOrWhiteSpace(user.TotpSecretEncrypted))
            {
                return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "Two-factor authentication is not active."));
            }

            var codeAccepted = await VerifyTotpOrBackupCodeAsync(user, rawCode, now, cancellationToken);
            if (!codeAccepted)
            {
                return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Verification code is invalid."));
            }

            return await IssueAuthenticatedSessionAsync(user, command.Metadata, now, backupCodes: null, cancellationToken);
        }

        if (claims.Purpose == SelfHostedJwtConstants.PurposeTwoFactorSetup)
        {
            if (string.IsNullOrWhiteSpace(user.PendingTotpSecretEncrypted))
            {
                return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "No pending two-factor setup found."));
            }

            if (!_totpService.IsSixDigitTotp(rawCode))
            {
                return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "A 6-digit authenticator code is required to complete setup."));
            }

            string pendingSecret;
            try
            {
                pendingSecret = _secretProtector.Unprotect(user.PendingTotpSecretEncrypted);
            }
            catch (CryptographicException)
            {
                return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "Pending two-factor secret could not be decrypted."));
            }

            if (!_totpService.VerifyCode(pendingSecret, rawCode, now))
            {
                return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Verification code is invalid."));
            }

            user.EnableTotp(now);
            var plaintextBackupCodes = _totpService.GenerateBackupCodes(8);
            var backupEntities = plaintextBackupCodes
                .Select(c => new UserBackupCode(Guid.NewGuid(), user.Id, _totpService.HashBackupCode(c), now))
                .ToList();
            await _store.ReplaceBackupCodesAsync(user.Id, backupEntities, cancellationToken);

            return await IssueAuthenticatedSessionAsync(user, command.Metadata, now, plaintextBackupCodes, cancellationToken);
        }

        return Result<AuthFlowResult>.Failure(new Error("TWO_FACTOR_CHALLENGE_INVALID", "Unsupported challenge purpose."));
    }

    public async Task<Result<AuthFlowResult>> RefreshSessionAsync(
        RefreshSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var parsed = RefreshTokenFormat.TryParse(command.RefreshToken);
        if (parsed is null)
        {
            return Result<AuthFlowResult>.Failure(new Error("AUTHENTICATION_INVALID", "Refresh token is missing or invalid."));
        }

        var session = await _store.FindSessionWithUserAsync(parsed.SessionId, cancellationToken);
        if (session is null || session.User is null)
        {
            return Result<AuthFlowResult>.Failure(new Error("AUTHENTICATION_INVALID", "Session not found."));
        }

        var now = _clock.UtcNow;
        if (!session.User.IsActive)
        {
            session.Revoke("user_deactivated", now);
            await _store.SaveChangesAsync(cancellationToken);
            return Result<AuthFlowResult>.Failure(new Error("AUTHENTICATION_INVALID", "User account is disabled."));
        }

        var nextRefresh = RefreshTokenFormat.Generate(session.Id);
        var decision = session.EvaluateRefresh(parsed.SecretHash, nextRefresh.SecretHash, now);

        switch (decision)
        {
            case SessionRefreshDecision.Invalid:
                return Result<AuthFlowResult>.Failure(new Error("AUTHENTICATION_INVALID", "Session is revoked or expired."));

            case SessionRefreshDecision.ReuseDetected:
                await _store.SaveChangesAsync(cancellationToken);
                return Result<AuthFlowResult>.Failure(new Error("AUTHENTICATION_INVALID", "Refresh token reuse detected; session revoked."));

            case SessionRefreshDecision.GraceHit:
            {
                var subjectUid = session.User.EnsureSubjectUid();
                await _store.SaveChangesAsync(cancellationToken);

                var access = _jwtService.CreateAccessToken(session.User.Id, subjectUid, session.Id, session.User.Email, now);
                return Result<AuthFlowResult>.Success(new AuthFlowResult(
                    Status: AuthStatusValues.Authenticated,
                    AccessToken: access.AccessToken,
                    ExpiresAtUtc: access.ExpiresAtUtc,
                    ClientRefreshToken: null,
                    CookieRefreshToken: null,
                    SessionExpiresAtUtc: null,
                    SessionId: session.Id,
                    User: new AuthUserSummary(session.User.Id, subjectUid, session.User.Email, session.User.DisplayName)));
            }

            case SessionRefreshDecision.Rotated:
            {
                var subjectUid = session.User.EnsureSubjectUid();
                await _store.SaveChangesAsync(cancellationToken);

                var access = _jwtService.CreateAccessToken(session.User.Id, subjectUid, session.Id, session.User.Email, now);
                var isMobile = session.ClientPlatform == UserSession.PlatformMobile;

                return Result<AuthFlowResult>.Success(new AuthFlowResult(
                    Status: AuthStatusValues.Authenticated,
                    AccessToken: access.AccessToken,
                    ExpiresAtUtc: access.ExpiresAtUtc,
                    ClientRefreshToken: isMobile ? nextRefresh.WireToken : null,
                    CookieRefreshToken: isMobile ? null : nextRefresh.WireToken,
                    SessionExpiresAtUtc: session.ExpiresAtUtc,
                    SessionId: session.Id,
                    User: new AuthUserSummary(session.User.Id, subjectUid, session.User.Email, session.User.DisplayName)));
            }

            default:
                return Result<AuthFlowResult>.Failure(new Error("AUTHENTICATION_INVALID", "Session refresh failed."));
        }
    }

    public async Task LogoutAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        var now = _clock.UtcNow;
        var changed = false;

        if (command.AuthenticatedSessionId.HasValue && !string.IsNullOrWhiteSpace(command.AuthenticatedSubjectUid))
        {
            var session = await _store.FindSessionWithUserAsync(command.AuthenticatedSessionId.Value, cancellationToken);
            if (session?.User is not null
                && string.Equals(session.User.FirebaseUid, command.AuthenticatedSubjectUid, StringComparison.Ordinal)
                && session.RevokedAtUtc is null)
            {
                session.Revoke("logout", now);
                changed = true;
            }
        }

        var parsed = RefreshTokenFormat.TryParse(command.RefreshToken);
        if (parsed is not null && parsed.SessionId != command.AuthenticatedSessionId)
        {
            var session = await _store.FindSessionWithUserAsync(parsed.SessionId, cancellationToken);
            if (session is not null
                && session.RevokedAtUtc is null
                && session.MatchesRefreshTokenHash(parsed.SecretHash))
            {
                session.Revoke("logout", now);
                changed = true;
            }
        }
        else if (parsed is not null && !changed)
        {
            var session = await _store.FindSessionWithUserAsync(parsed.SessionId, cancellationToken);
            if (session is not null
                && session.RevokedAtUtc is null
                && session.MatchesRefreshTokenHash(parsed.SecretHash))
            {
                session.Revoke("logout", now);
                changed = true;
            }
        }

        if (changed)
        {
            await _store.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<Result<AuthFlowResult>> CompletePrimaryAuthAsync(
        User user,
        ClientRequestMetadata metadata,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var subjectUid = user.EnsureSubjectUid();

        if (user.IsTotpEnabled)
        {
            var challenge = _jwtService.CreateTwoFactorChallengeToken(
                user.Id,
                subjectUid,
                SelfHostedJwtConstants.PurposeTwoFactorVerify,
                now);
            await _store.SaveChangesAsync(cancellationToken);

            return Result<AuthFlowResult>.Success(new AuthFlowResult(
                Status: AuthStatusValues.RequiresTwoFactor,
                ChallengeToken: challenge));
        }

        if (user.RequireTwoFactor)
        {
            var secret = _totpService.GenerateSecretKey();
            var otpAuthUri = _totpService.BuildOtpAuthUri(user.Email, secret);
            user.BeginTotpSetup(_secretProtector.Protect(secret));

            var setupToken = _jwtService.CreateTwoFactorChallengeToken(
                user.Id,
                subjectUid,
                SelfHostedJwtConstants.PurposeTwoFactorSetup,
                now);
            await _store.SaveChangesAsync(cancellationToken);

            return Result<AuthFlowResult>.Success(new AuthFlowResult(
                Status: AuthStatusValues.RequiresTwoFactorSetup,
                ChallengeToken: setupToken,
                TotpSetup: new TotpSetupPayload(secret, otpAuthUri)));
        }

        return await IssueAuthenticatedSessionAsync(user, metadata, now, backupCodes: null, cancellationToken);
    }

    private async Task<Result<AuthFlowResult>> IssueAuthenticatedSessionAsync(
        User user,
        ClientRequestMetadata metadata,
        DateTimeOffset now,
        IReadOnlyList<string>? backupCodes,
        CancellationToken cancellationToken)
    {
        var subjectUid = user.EnsureSubjectUid();
        var sessionId = Guid.NewGuid();
        var refresh = RefreshTokenFormat.Generate(sessionId);
        var platform = UserSession.NormalizePlatform(metadata.ClientPlatform);

        var session = new UserSession(
            sessionId,
            user.Id,
            refresh.SecretHash,
            platform,
            metadata.IpAddress,
            metadata.UserAgent,
            now);

        _store.AddSession(session);
        await _store.SaveChangesAsync(cancellationToken);

        var access = _jwtService.CreateAccessToken(user.Id, subjectUid, session.Id, user.Email, now);
        var isMobile = platform == UserSession.PlatformMobile;

        return Result<AuthFlowResult>.Success(new AuthFlowResult(
            Status: AuthStatusValues.Authenticated,
            AccessToken: access.AccessToken,
            ExpiresAtUtc: access.ExpiresAtUtc,
            ClientRefreshToken: isMobile ? refresh.WireToken : null,
            CookieRefreshToken: isMobile ? null : refresh.WireToken,
            SessionExpiresAtUtc: session.ExpiresAtUtc,
            SessionId: session.Id,
            User: new AuthUserSummary(user.Id, subjectUid, user.Email, user.DisplayName),
            BackupCodes: backupCodes));
    }

    private async Task<bool> VerifyTotpOrBackupCodeAsync(
        User user,
        string rawCode,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (_totpService.IsSixDigitTotp(rawCode) && !string.IsNullOrWhiteSpace(user.TotpSecretEncrypted))
        {
            try
            {
                var totpSecret = _secretProtector.Unprotect(user.TotpSecretEncrypted);
                if (_totpService.VerifyCode(totpSecret, rawCode, now))
                {
                    return true;
                }
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        if (_totpService.IsBackupCodeCandidate(rawCode))
        {
            var codeHash = _totpService.HashBackupCode(rawCode);
            return await _store.TryConsumeBackupCodeAsync(user.Id, codeHash, now, cancellationToken);
        }

        return false;
    }
}
