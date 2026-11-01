using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.IdentityAccess.Authentication;
using TanErp.Domain.IdentityAccess;
using TanErp.Infrastructure.Identity;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class SelfHostedAuthCryptoAndDomainTests
{
    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTimeOffset utcNow) => UtcNow = utcNow;
        public DateTimeOffset UtcNow { get; set; }
    }

    private static SelfHostedAuthOptions CreateTestOptions() =>
        new(
            "unit-test-jwt-signing-key-with-at-least-32-characters!",
            "unit-test-aes-gcm-encryption-key-at-least-32-chars!",
            "test-google-client-id.apps.googleusercontent.com");

    [Fact]
    public void Pbkdf2PasswordHasher_HashesWithExpectedFormat_AndVerifiesConstantTime()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash1 = hasher.HashPassword("StrongPass123!");
        var hash2 = hasher.HashPassword("StrongPass123!");

        Assert.StartsWith("pbkdf2-sha512$210000$", hash1, StringComparison.Ordinal);
        Assert.NotEqual(hash1, hash2);
        Assert.Equal(4, hash1.Split('$').Length);

        Assert.True(hasher.VerifyPassword("StrongPass123!", hash1));
        Assert.True(hasher.VerifyPassword("StrongPass123!", hash2));
        Assert.False(hasher.VerifyPassword("WrongPass123!", hash1));
        Assert.False(hasher.VerifyPassword("StrongPass123!", "malformed-hash"));
        Assert.False(hasher.VerifyPassword("", hash1));
    }

    [Theory]
    [InlineData(null, "PASSWORD_TOO_SHORT")]
    [InlineData("", "PASSWORD_TOO_SHORT")]
    [InlineData("1234567", "PASSWORD_TOO_SHORT")]
    [InlineData("       ", "PASSWORD_TOO_SHORT")]
    public void PasswordPolicy_RejectsTooShortPasswords(string? candidate, string expectedCode)
    {
        var result = PasswordPolicy.Validate(candidate);

        Assert.True(result.IsFailure);
        Assert.Equal(expectedCode, result.Error.Code);
    }

    [Fact]
    public void PasswordPolicy_AcceptsValidBounds_AndRejectsOverMaxLength()
    {
        Assert.True(PasswordPolicy.Validate("12345678").IsSuccess);
        Assert.True(PasswordPolicy.Validate(new string('A', 128)).IsSuccess);

        var tooLong = PasswordPolicy.Validate(new string('A', 129));
        Assert.True(tooLong.IsFailure);
        Assert.Equal("REQUEST_VALIDATION_FAILED", tooLong.Error.Code);
    }

    [Fact]
    public void AesGcmSecretProtector_RoundTrips_UsesRandomNonce_AndRejectsTampering()
    {
        var protector = new AesGcmSecretProtector(CreateTestOptions());
        const string secret = "JBSWY3DPEHPK3PXPJBSWY3DPEHPK3PXP";

        var encrypted1 = protector.Protect(secret);
        var encrypted2 = protector.Protect(secret);

        Assert.NotEqual(encrypted1, encrypted2);
        Assert.Equal(secret, protector.Unprotect(encrypted1));
        Assert.Equal(secret, protector.Unprotect(encrypted2));

        var parts = encrypted1.Split('.');
        Assert.Equal("v1", parts[0]);
        var tagBytes = System.Buffers.Text.Base64Url.DecodeFromChars(parts[3]);
        tagBytes[^1] ^= 0xFF;
        var tampered = $"{parts[0]}.{parts[1]}.{parts[2]}.{System.Buffers.Text.Base64Url.EncodeToString(tagBytes)}";

        Assert.ThrowsAny<CryptographicException>(() => protector.Unprotect(tampered));
    }

    private sealed class DictConfiguration : IConfiguration
    {
        private readonly IReadOnlyDictionary<string, string?> _values;
        public DictConfiguration(IReadOnlyDictionary<string, string?>? values = null) =>
            _values = values ?? new Dictionary<string, string?>();
        public string? this[string key]
        {
            get => _values.TryGetValue(key, out var val) ? val : null;
            set => throw new NotSupportedException();
        }
        public IEnumerable<IConfigurationSection> GetChildren() => Array.Empty<IConfigurationSection>();
        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken() => throw new NotSupportedException();
        public IConfigurationSection GetSection(string key) => throw new NotSupportedException();
    }

    [Fact]
    public void SelfHostedAuthOptions_ProductionFailsClosedWhenKeysMissingOrShort()
    {
        var emptyConfig = new DictConfiguration();
        Assert.Throws<InvalidOperationException>(() =>
            SelfHostedAuthOptions.FromConfiguration(emptyConfig, "Production"));

        var shortKeyConfig = new DictConfiguration(new Dictionary<string, string?>
        {
            ["Authentication:JwtSigningKey"] = "short-key",
            ["Authentication:EncryptionKey"] = "0123456789abcdef0123456789abcdef"
        });
        Assert.Throws<InvalidOperationException>(() =>
            SelfHostedAuthOptions.FromConfiguration(shortKeyConfig, "Production"));

        var devOptions = SelfHostedAuthOptions.FromConfiguration(emptyConfig, "Development");
        Assert.True(devOptions.JwtSigningKey.Length >= 32);
        Assert.True(devOptions.EncryptionKey.Length >= 32);
    }

    [Fact]
    public void Rfc6238TotpService_GeneratesValidSecrets_VerifiesWithOneStepSkew_AndNormalizesBackupCodes()
    {
        var totp = new Rfc6238TotpService();
        var secret = totp.GenerateSecretKey();

        Assert.DoesNotContain("=", secret, StringComparison.Ordinal);
        Assert.Equal(20, Rfc6238TotpService.DecodeBase32(secret).Length);

        var uri = totp.BuildOtpAuthUri("staff+1@tan-erp.test", secret);
        Assert.Equal(
            $"otpauth://totp/tan-erp:staff%2B1%40tan-erp.test?secret={secret}&issuer=tan-erp&algorithm=SHA1&digits=6&period=30",
            uri);

        var t0 = new DateTimeOffset(2026, 11, 1, 10, 0, 15, TimeSpan.Zero);
        var codeAtT0 = totp.ComputeCode(secret, t0);
        Assert.Equal(6, codeAtT0.Length);

        Assert.True(totp.VerifyCode(secret, codeAtT0, t0));
        Assert.True(totp.VerifyCode(secret, codeAtT0, t0.AddSeconds(30)));
        Assert.True(totp.VerifyCode(secret, codeAtT0, t0.AddSeconds(-30)));
        Assert.False(totp.VerifyCode(secret, codeAtT0, t0.AddSeconds(90)));
        Assert.False(totp.VerifyCode(secret, "000000" == codeAtT0 ? "111111" : "000000", t0));

        var backupCodes = totp.GenerateBackupCodes(8);
        Assert.Equal(8, backupCodes.Count);
        Assert.Equal(8, backupCodes.Distinct(StringComparer.Ordinal).Count());
        foreach (var bc in backupCodes)
        {
            Assert.Equal(9, bc.Length);
            Assert.Equal('-', bc[4]);
            Assert.DoesNotContain('0', bc);
            Assert.DoesNotContain('O', bc);
            Assert.DoesNotContain('1', bc);
            Assert.DoesNotContain('I', bc);
            Assert.True(totp.IsBackupCodeCandidate(bc));
            Assert.Equal(totp.HashBackupCode(bc), totp.HashBackupCode(bc.Replace("-", " ").ToLowerInvariant()));
        }
    }

    [Fact]
    public async Task SelfHostedJwtService_StrictlySeparatesApiAccessAndTwoFactorAudiences()
    {
        var now = new DateTimeOffset(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);
        var clock = new FixedClock(now);
        var jwt = new SelfHostedJwtService(CreateTestOptions());
        var verifier = new SelfHostedTokenVerifier(jwt, clock);

        var userId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var subjectUid = $"usr_{userId:N}";

        var access = jwt.CreateAccessToken(userId, subjectUid, sessionId, "user@example.test", now);
        Assert.Equal(now.AddMinutes(15), access.ExpiresAtUtc);

        var identity = await verifier.VerifyIdentityAsync(access.AccessToken);
        Assert.NotNull(identity);
        Assert.Equal(subjectUid, identity.Uid);
        Assert.Equal("user@example.test", identity.Email);
        Assert.True(identity.EmailVerified);
        Assert.Equal(sessionId, identity.SessionId);

        // Access token must never be accepted as a 2FA challenge token
        Assert.Null(jwt.VerifyTwoFactorChallengeToken(access.AccessToken, now));

        // 2FA challenge & setup tokens must NEVER be accepted by API authentication
        var verifyChallenge = jwt.CreateTwoFactorChallengeToken(userId, subjectUid, SelfHostedJwtConstants.PurposeTwoFactorVerify, now);
        var setupChallenge = jwt.CreateTwoFactorChallengeToken(userId, subjectUid, SelfHostedJwtConstants.PurposeTwoFactorSetup, now);

        Assert.Null(await verifier.VerifyIdentityAsync(verifyChallenge));
        Assert.Null(await verifier.VerifyIdentityAsync(setupChallenge));

        var verifyClaims = jwt.VerifyTwoFactorChallengeToken(verifyChallenge, now);
        Assert.NotNull(verifyClaims);
        Assert.Equal(userId, verifyClaims.UserId);
        Assert.Equal(SelfHostedJwtConstants.PurposeTwoFactorVerify, verifyClaims.Purpose);
        Assert.Null(jwt.VerifyTwoFactorChallengeToken(verifyChallenge, now.AddMinutes(6)));

        var setupClaims = jwt.VerifyTwoFactorChallengeToken(setupChallenge, now.AddMinutes(9));
        Assert.NotNull(setupClaims);
        Assert.Equal(SelfHostedJwtConstants.PurposeTwoFactorSetup, setupClaims.Purpose);
        Assert.Null(jwt.VerifyTwoFactorChallengeToken(setupChallenge, now.AddMinutes(11)));

        // Expired access token rejected
        clock.UtcNow = now.AddMinutes(16);
        Assert.Null(await verifier.VerifyIdentityAsync(access.AccessToken));
    }

    [Fact]
    public void User_EnsureSubjectUid_AndLockoutInvariants()
    {
        var now = new DateTimeOffset(2026, 11, 1, 8, 0, 0, TimeSpan.Zero);
        var pending = User.CreatePending(Guid.NewGuid(), "Pending Staff", "pending@example.test", now);
        Assert.True(pending.IsPending);

        var generatedUid = pending.EnsureSubjectUid();
        Assert.Equal($"usr_{pending.Id:N}", generatedUid);
        Assert.False(pending.IsPending);

        var seeded = new User(Guid.NewGuid(), "foundation-user-test-only", "Seeded", "seeded@example.test");
        Assert.Equal("foundation-user-test-only", seeded.EnsureSubjectUid());

        for (var i = 0; i < 4; i++)
        {
            pending.RecordFailedLogin(now);
            Assert.False(pending.IsLockedOut(now));
        }

        pending.RecordFailedLogin(now);
        Assert.True(pending.IsLockedOut(now));
        Assert.True(pending.IsLockedOut(now.AddMinutes(14)));
        Assert.False(pending.IsLockedOut(now.AddMinutes(15)));

        pending.ResetLoginLockout();
        Assert.Equal(0, pending.FailedLoginCount);
        Assert.False(pending.IsLockedOut(now));
    }

    [Fact]
    public void UserSession_RotatingRefresh_GraceWindow_AndReuseDetection()
    {
        var t0 = new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero);
        var session = new UserSession(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "hash-v1",
            "web",
            "203.0.113.10",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 Chrome/131.0.0.0 Safari/537.36",
            t0);

        Assert.Equal("Chrome on macOS", session.DeviceLabel);
        Assert.Equal("TAN Field Mobile (iOS)", UserSession.ParseDeviceLabel("Dart/3.5 (iOS)", "mobile"));
        Assert.Equal("Safari on iPhone", UserSession.ParseDeviceLabel("Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) Version/18.0 Mobile/15E148 Safari/604.1", "web"));

        // 1. Valid rotation from hash-v1 -> hash-v2
        var tRotate = t0.AddMinutes(5);
        var decision1 = session.EvaluateRefresh("hash-v1", "hash-v2", tRotate);
        Assert.Equal(SessionRefreshDecision.Rotated, decision1);
        Assert.Equal("hash-v2", session.CurrentRefreshTokenHash);
        Assert.Equal("hash-v1", session.PreviousRefreshTokenHash);

        // 2. Concurrent multi-tab presenting hash-v1 within 15s grace window -> GraceHit without rotating hash-v2
        var decisionGrace = session.EvaluateRefresh("hash-v1", "hash-unused", tRotate.AddSeconds(10));
        Assert.Equal(SessionRefreshDecision.GraceHit, decisionGrace);
        Assert.Equal("hash-v2", session.CurrentRefreshTokenHash);
        Assert.True(session.IsActiveAt(tRotate.AddSeconds(10)));

        // 3. Replaying hash-v1 AFTER 15s grace window -> ReuseDetected & immediate revocation!
        var tAfterGrace = tRotate.AddSeconds(16);
        var decisionReuse = session.EvaluateRefresh("hash-v1", "hash-v3", tAfterGrace);
        Assert.Equal(SessionRefreshDecision.ReuseDetected, decisionReuse);
        Assert.False(session.IsActiveAt(tAfterGrace));
        Assert.Equal("token_reuse_detected", session.RevokeReason);

        // 4. Subsequent presentation of hash-v2 now fails because session is revoked
        Assert.Equal(SessionRefreshDecision.Invalid, session.EvaluateRefresh("hash-v2", "hash-v4", tAfterGrace.AddSeconds(1)));
    }
}
