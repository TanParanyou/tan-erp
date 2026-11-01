using System.Security.Cryptography;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Application.IdentityAccess.Authentication;

public class CurrentUserSecurityHandler
{
    private readonly IAuthenticationStore _store;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecretProtector _secretProtector;
    private readonly ITotpService _totpService;
    private readonly IClock _clock;

    public CurrentUserSecurityHandler(
        IAuthenticationStore store,
        IPasswordHasher passwordHasher,
        ISecretProtector secretProtector,
        ITotpService totpService,
        IClock clock)
    {
        _store = store;
        _passwordHasher = passwordHasher;
        _secretProtector = secretProtector;
        _totpService = totpService;
        _clock = clock;
    }

    public async Task<Result<MySecurityStatus>> GetSecurityStatusAsync(string subjectUid, CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<MySecurityStatus>.Failure(userResult.Error);

        var user = userResult.Value!;
        var remainingCodes = user.IsTotpEnabled
            ? await _store.CountRemainingBackupCodesAsync(user.Id, cancellationToken)
            : 0;

        return Result<MySecurityStatus>.Success(new MySecurityStatus(
            HasPassword: !string.IsNullOrWhiteSpace(user.PasswordHash),
            HasGoogleLinked: !string.IsNullOrWhiteSpace(user.GoogleSubjectId),
            IsTotpEnabled: user.IsTotpEnabled,
            RequireTwoFactor: user.RequireTwoFactor,
            RemainingBackupCodesCount: remainingCodes,
            TotpEnabledAtUtc: user.TotpEnabledAtUtc,
            PasswordChangedAtUtc: user.PasswordChangedAtUtc));
    }

    public async Task<Result<bool>> ChangePasswordAsync(ChangeMyPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(command.SubjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<bool>.Failure(userResult.Error);

        var policyResult = PasswordPolicy.Validate(command.NewPassword);
        if (policyResult.IsFailure) return policyResult;

        var user = userResult.Value!;
        var newPassword = command.NewPassword!;

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            if (string.IsNullOrEmpty(command.CurrentPassword)
                || !_passwordHasher.VerifyPassword(command.CurrentPassword, user.PasswordHash))
            {
                return Result<bool>.Failure(new Error("CURRENT_PASSWORD_INVALID", "Current password is invalid."));
            }

            if (_passwordHasher.VerifyPassword(newPassword, user.PasswordHash))
            {
                return Result<bool>.Failure(new Error("PASSWORD_SAME_AS_CURRENT", "New password must differ from current password."));
            }
        }

        var now = _clock.UtcNow;
        var newHash = _passwordHasher.HashPassword(newPassword);
        user.SetPassword(newHash, now);

        await _store.RevokeOtherActiveSessionsAsync(user.Id, command.CurrentSessionId, "password_changed", now, cancellationToken);
        await _store.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<TotpSetupPayload>> SetupTwoFactorAsync(string subjectUid, CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<TotpSetupPayload>.Failure(userResult.Error);

        var user = userResult.Value!;
        var secret = _totpService.GenerateSecretKey();
        var otpAuthUri = _totpService.BuildOtpAuthUri(user.Email, secret);

        user.BeginTotpSetup(_secretProtector.Protect(secret));
        await _store.SaveChangesAsync(cancellationToken);

        return Result<TotpSetupPayload>.Success(new TotpSetupPayload(secret, otpAuthUri));
    }

    public async Task<Result<IReadOnlyList<string>>> EnableTwoFactorAsync(
        string subjectUid,
        string? code,
        CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<IReadOnlyList<string>>.Failure(userResult.Error);

        var user = userResult.Value!;
        if (string.IsNullOrWhiteSpace(user.PendingTotpSecretEncrypted))
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_SETUP_REQUIRED", "Initiate 2FA setup before enabling."));
        }

        var rawCode = (code ?? string.Empty).Trim();
        if (!_totpService.IsSixDigitTotp(rawCode))
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "A valid 6-digit TOTP code is required."));
        }

        string secret;
        try
        {
            secret = _secretProtector.Unprotect(user.PendingTotpSecretEncrypted);
        }
        catch (CryptographicException)
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_SETUP_REQUIRED", "Pending 2FA setup secret is invalid; please set up again."));
        }

        var now = _clock.UtcNow;
        if (!_totpService.VerifyCode(secret, rawCode, now))
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Verification code is invalid."));
        }

        user.EnableTotp(now);
        var plaintextCodes = _totpService.GenerateBackupCodes(8);
        var entities = plaintextCodes
            .Select(c => new UserBackupCode(Guid.NewGuid(), user.Id, _totpService.HashBackupCode(c), now))
            .ToList();

        await _store.ReplaceBackupCodesAsync(user.Id, entities, cancellationToken);
        await _store.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<string>>.Success(plaintextCodes);
    }

    public async Task<Result<bool>> DisableTwoFactorAsync(
        string subjectUid,
        string? code,
        CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<bool>.Failure(userResult.Error);

        var user = userResult.Value!;
        if (!user.IsTotpEnabled || string.IsNullOrWhiteSpace(user.TotpSecretEncrypted))
        {
            return Result<bool>.Failure(new Error("TWO_FACTOR_NOT_ENABLED", "Two-factor authentication is not enabled."));
        }

        if (user.RequireTwoFactor)
        {
            return Result<bool>.Failure(new Error("TWO_FACTOR_REQUIRED_BY_ADMIN", "Two-factor authentication is required by administrator policy."));
        }

        var rawCode = (code ?? string.Empty).Trim();
        var now = _clock.UtcNow;
        var verified = false;

        if (_totpService.IsSixDigitTotp(rawCode))
        {
            try
            {
                var secret = _secretProtector.Unprotect(user.TotpSecretEncrypted);
                verified = _totpService.VerifyCode(secret, rawCode, now);
            }
            catch (CryptographicException)
            {
                verified = false;
            }
        }
        else if (_totpService.IsBackupCodeCandidate(rawCode))
        {
            verified = await _store.TryConsumeBackupCodeAsync(user.Id, _totpService.HashBackupCode(rawCode), now, cancellationToken);
        }

        if (!verified)
        {
            return Result<bool>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Verification code is invalid."));
        }

        user.DisableTotp();
        await _store.DeleteAllBackupCodesAsync(user.Id, cancellationToken);
        await _store.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<IReadOnlyList<string>>> RegenerateBackupCodesAsync(
        string subjectUid,
        string? code,
        CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<IReadOnlyList<string>>.Failure(userResult.Error);

        var user = userResult.Value!;
        if (!user.IsTotpEnabled || string.IsNullOrWhiteSpace(user.TotpSecretEncrypted))
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_NOT_ENABLED", "Two-factor authentication is not enabled."));
        }

        var rawCode = (code ?? string.Empty).Trim();
        if (!_totpService.IsSixDigitTotp(rawCode))
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "A valid 6-digit TOTP code is required."));
        }

        string secret;
        try
        {
            secret = _secretProtector.Unprotect(user.TotpSecretEncrypted);
        }
        catch (CryptographicException)
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Stored TOTP secret is invalid."));
        }

        var now = _clock.UtcNow;
        if (!_totpService.VerifyCode(secret, rawCode, now))
        {
            return Result<IReadOnlyList<string>>.Failure(new Error("TWO_FACTOR_CODE_INVALID", "Verification code is invalid."));
        }

        var plaintextCodes = _totpService.GenerateBackupCodes(8);
        var entities = plaintextCodes
            .Select(c => new UserBackupCode(Guid.NewGuid(), user.Id, _totpService.HashBackupCode(c), now))
            .ToList();

        await _store.ReplaceBackupCodesAsync(user.Id, entities, cancellationToken);
        await _store.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<string>>.Success(plaintextCodes);
    }

    public async Task<Result<IReadOnlyList<ActiveSessionItem>>> ListSessionsAsync(
        string subjectUid,
        Guid? currentSessionId,
        CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<IReadOnlyList<ActiveSessionItem>>.Failure(userResult.Error);

        var user = userResult.Value!;
        var now = _clock.UtcNow;
        var sessions = await _store.ListActiveSessionsAsync(user.Id, now, cancellationToken);

        var items = sessions
            .OrderByDescending(s => currentSessionId.HasValue && s.Id == currentSessionId.Value)
            .ThenByDescending(s => s.LastUsedAtUtc)
            .ThenBy(s => s.Id)
            .Select(s => new ActiveSessionItem(
                s.Id,
                s.DeviceLabel,
                s.ClientPlatform,
                s.IpAddress,
                s.UserAgent,
                s.CreatedAtUtc,
                s.LastUsedAtUtc,
                s.ExpiresAtUtc,
                currentSessionId.HasValue && s.Id == currentSessionId.Value))
            .ToList();

        return Result<IReadOnlyList<ActiveSessionItem>>.Success(items);
    }

    public async Task<Result<bool>> RevokeSessionAsync(
        string subjectUid,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<bool>.Failure(userResult.Error);

        var user = userResult.Value!;
        var now = _clock.UtcNow;
        var session = await _store.FindUserSessionAsync(user.Id, sessionId, cancellationToken);
        if (session is null || !session.IsActiveAt(now))
        {
            return Result<bool>.Failure(new Error("SESSION_NOT_FOUND", "Active session not found."));
        }

        session.Revoke("user_revoked", now);
        await _store.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> RevokeOtherSessionsAsync(
        string subjectUid,
        Guid? currentSessionId,
        CancellationToken cancellationToken = default)
    {
        var userResult = await ResolveActiveUserAsync(subjectUid, cancellationToken);
        if (userResult.IsFailure) return Result<int>.Failure(userResult.Error);

        var user = userResult.Value!;
        var now = _clock.UtcNow;
        var revokedCount = await _store.RevokeOtherActiveSessionsAsync(
            user.Id,
            currentSessionId,
            "user_revoked_others",
            now,
            cancellationToken);
        await _store.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(revokedCount);
    }

    private async Task<Result<User>> ResolveActiveUserAsync(string subjectUid, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(subjectUid))
        {
            return Result<User>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        var user = await _store.FindUserBySubjectUidAsync(subjectUid.Trim(), cancellationToken);
        if (user is null)
        {
            return Result<User>.Failure(new Error("AUTHENTICATION_INVALID", "Authenticated user not found."));
        }

        if (!user.IsActive)
        {
            return Result<User>.Failure(new Error("USER_ACCESS_DISABLED", "User account is disabled."));
        }

        return Result<User>.Success(user);
    }
}
