namespace TanErp.Application.Commercial;

/// <summary>Section <c>AcceptanceOtp</c>. <see cref="Required"/> is the feature switch (default off keeps the pre-G-05 behaviour).</summary>
public sealed class AcceptanceOtpOptions
{
    public const string SectionName = "AcceptanceOtp";
    public const int MinHmacKeyChars = 32;

    public bool Required { get; set; }
    public string HmacKey { get; set; } = string.Empty;
    public int CodeLength { get; set; } = 6;
    public int ExpiryMinutes { get; set; } = 10;
    public int MaxAttempts { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
    public int MaxSendsPerHour { get; set; } = 3;
    public int VerificationMinutes { get; set; } = 15;

    /// <summary>Startup validation; throws so a half-configured OTP never reaches production.</summary>
    public void Validate()
    {
        if (!Required) return;
        if (HmacKey.Length < MinHmacKeyChars) throw new InvalidOperationException($"AcceptanceOtp:HmacKey must be at least {MinHmacKeyChars} characters when AcceptanceOtp:Required is true.");
        if (CodeLength is < 4 or > 9) throw new InvalidOperationException("AcceptanceOtp:CodeLength must be between 4 and 9.");
        if (ExpiryMinutes < 1 || MaxAttempts < 1 || ResendCooldownSeconds < 0 || MaxSendsPerHour < 1 || VerificationMinutes < 1) throw new InvalidOperationException("AcceptanceOtp limits must be positive.");
    }
}

public sealed record OtpChallengeRecord(Guid Id, string CodeHash, int FailedAttempts);

public sealed record OtpSendStats(int SendsInWindow, DateTimeOffset? LastSentAtUtc);

public sealed record OtpVerificationRecord(Guid ChallengeId, DateTimeOffset VerifiedAtUtc);

/// <summary>Identical for every link, whether or not an email is on file.</summary>
public sealed record OtpRequestResult(int ResendAfterSeconds, int ExpiresInSeconds);

public sealed record OtpVerifyResult(string VerificationToken, DateTimeOffset ExpiresAtUtc);

/// <summary>What the accept transaction stores in the evidence. Never the code.</summary>
public sealed record OtpEvidence(string Channel, DateTimeOffset VerifiedAtUtc, Guid ChallengeId);

public interface IAcceptanceOtpStore
{
    Task<OtpSendStats> GetSendStatsAsync(Guid linkId, DateTimeOffset since, CancellationToken ct = default);
    Task<Guid> CreateChallengeAsync(AcceptanceContext context, string channel, string codeHash, DateTimeOffset now, DateTimeOffset expiresAtUtc, string traceId, CancellationToken ct = default);

    /// <summary>The newest challenge that is unexpired, unverified and unconsumed.</summary>
    Task<OtpChallengeRecord?> FindActiveChallengeAsync(Guid linkId, DateTimeOffset now, CancellationToken ct = default);

    /// <summary>Counts a wrong code; at <paramref name="maxAttempts"/> it locks the link and returns true.</summary>
    Task<bool> RegisterFailureAsync(AcceptanceContext context, Guid challengeId, int maxAttempts, DateTimeOffset now, string traceId, CancellationToken ct = default);

    Task MarkVerifiedAsync(AcceptanceContext context, Guid challengeId, string verificationTokenHash, DateTimeOffset now, string traceId, CancellationToken ct = default);

    /// <summary>A verified, unconsumed challenge of this link matching the token hash.</summary>
    Task<OtpVerificationRecord?> FindVerificationAsync(Guid linkId, string verificationTokenHash, CancellationToken ct = default);
}
