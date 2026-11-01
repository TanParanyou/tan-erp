using System.Security.Cryptography;
using System.Text;
using TanErp.Domain.Common;

namespace TanErp.Domain.IdentityAccess;

public enum SessionRefreshDecision
{
    Rotated,
    GraceHit,
    ReuseDetected,
    Invalid
}

public class UserSession : Entity
{
    public const string PlatformWeb = "web";
    public const string PlatformMobile = "mobile";

    public static readonly TimeSpan DefaultSessionLifetime = TimeSpan.FromDays(14);
    public static readonly TimeSpan RefreshGracePeriod = TimeSpan.FromSeconds(15);

    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string CurrentRefreshTokenHash { get; private set; } = string.Empty;
    public string? PreviousRefreshTokenHash { get; private set; }
    public DateTimeOffset LastRotatedAtUtc { get; private set; }
    public DateTimeOffset? GracePeriodUntilUtc { get; private set; }
    public string ClientPlatform { get; private set; } = PlatformWeb;
    public string DeviceLabel { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset LastUsedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public string? RevokeReason { get; private set; }

    protected UserSession() { }

    public UserSession(
        Guid id,
        Guid userId,
        string initialRefreshTokenHash,
        string? clientPlatform,
        string? ipAddress,
        string? userAgent,
        DateTimeOffset now)
        : base(id)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(initialRefreshTokenHash))
            throw new ArgumentException("Refresh token hash is required.", nameof(initialRefreshTokenHash));

        var normalizedPlatform = NormalizePlatform(clientPlatform);
        var sanitizedUa = SanitizeUserAgent(userAgent);
        var sanitizedIp = SanitizeIpAddress(ipAddress);

        UserId = userId;
        CurrentRefreshTokenHash = initialRefreshTokenHash.Trim();
        LastRotatedAtUtc = now;
        ClientPlatform = normalizedPlatform;
        DeviceLabel = ParseDeviceLabel(sanitizedUa, normalizedPlatform);
        IpAddress = sanitizedIp;
        UserAgent = sanitizedUa;
        CreatedAtUtc = now;
        LastUsedAtUtc = now;
        ExpiresAtUtc = now.Add(DefaultSessionLifetime);
    }

    public bool IsActiveAt(DateTimeOffset now) =>
        RevokedAtUtc is null && ExpiresAtUtc > now;

    public bool MatchesRefreshTokenHash(string candidateHash) =>
        FixedTimeEqualsAscii(candidateHash, CurrentRefreshTokenHash)
        || (PreviousRefreshTokenHash is not null && FixedTimeEqualsAscii(candidateHash, PreviousRefreshTokenHash));

    public SessionRefreshDecision EvaluateRefresh(string presentedTokenHash, string nextTokenHash, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(presentedTokenHash) || string.IsNullOrWhiteSpace(nextTokenHash))
        {
            return SessionRefreshDecision.Invalid;
        }

        if (!IsActiveAt(now))
        {
            return SessionRefreshDecision.Invalid;
        }

        if (FixedTimeEqualsAscii(presentedTokenHash, CurrentRefreshTokenHash))
        {
            PreviousRefreshTokenHash = CurrentRefreshTokenHash;
            CurrentRefreshTokenHash = nextTokenHash.Trim();
            LastRotatedAtUtc = now;
            LastUsedAtUtc = now;
            GracePeriodUntilUtc = now.Add(RefreshGracePeriod);
            ExpiresAtUtc = now.Add(DefaultSessionLifetime);
            return SessionRefreshDecision.Rotated;
        }

        if (PreviousRefreshTokenHash is not null
            && GracePeriodUntilUtc.HasValue
            && now <= GracePeriodUntilUtc.Value
            && FixedTimeEqualsAscii(presentedTokenHash, PreviousRefreshTokenHash))
        {
            LastUsedAtUtc = now;
            return SessionRefreshDecision.GraceHit;
        }

        Revoke("token_reuse_detected", now);
        return SessionRefreshDecision.ReuseDetected;
    }

    public void Revoke(string reason, DateTimeOffset now)
    {
        if (RevokedAtUtc.HasValue)
        {
            return;
        }

        RevokedAtUtc = now;
        var trimmed = reason?.Trim();
        RevokeReason = string.IsNullOrEmpty(trimmed) ? "revoked" : (trimmed.Length <= 64 ? trimmed : trimmed[..64]);
    }

    public static string NormalizePlatform(string? clientPlatform) =>
        string.Equals(clientPlatform?.Trim(), PlatformMobile, StringComparison.OrdinalIgnoreCase)
            ? PlatformMobile
            : PlatformWeb;

    public static string ParseDeviceLabel(string? userAgent, string? clientPlatform)
    {
        var platform = NormalizePlatform(clientPlatform);
        var ua = (userAgent ?? string.Empty).Trim();

        if (ua.Contains("TAN Field", StringComparison.OrdinalIgnoreCase)
            || (platform == PlatformMobile && (ua.Length == 0
                || ua.Contains("Dart", StringComparison.OrdinalIgnoreCase)
                || ua.Contains("Flutter", StringComparison.OrdinalIgnoreCase))))
        {
            if (ua.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "TAN Field Mobile (Android)";
            if (ua.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || ua.Contains("iOS", StringComparison.OrdinalIgnoreCase) || ua.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "TAN Field Mobile (iOS)";
            return "TAN Field Mobile";
        }

        string browser;
        if (ua.Contains("Edg/", StringComparison.OrdinalIgnoreCase) || ua.Contains("Edge/", StringComparison.OrdinalIgnoreCase))
            browser = "Edge";
        else if (ua.Contains("OPR/", StringComparison.OrdinalIgnoreCase) || ua.Contains("Opera", StringComparison.OrdinalIgnoreCase))
            browser = "Opera";
        else if (ua.Contains("Firefox/", StringComparison.OrdinalIgnoreCase) || ua.Contains("FxiOS/", StringComparison.OrdinalIgnoreCase))
            browser = "Firefox";
        else if (ua.Contains("CriOS/", StringComparison.OrdinalIgnoreCase) || ua.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
            browser = "Chrome";
        else if (ua.Contains("Safari/", StringComparison.OrdinalIgnoreCase))
            browser = "Safari";
        else
            browser = platform == PlatformMobile ? "TAN Field Mobile" : "Web Browser";

        string? os = null;
        if (ua.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
            os = "iPhone";
        else if (ua.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            os = "iPad";
        else if (ua.Contains("Android", StringComparison.OrdinalIgnoreCase))
            os = "Android";
        else if (ua.Contains("Mac OS X", StringComparison.OrdinalIgnoreCase) || ua.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
            os = "macOS";
        else if (ua.Contains("Windows", StringComparison.OrdinalIgnoreCase))
            os = "Windows";
        else if (ua.Contains("Linux", StringComparison.OrdinalIgnoreCase))
            os = "Linux";

        var label = os is not null ? $"{browser} on {os}" : browser;
        return label.Length <= 128 ? label : label[..128];
    }

    private static string SanitizeUserAgent(string? userAgent)
    {
        var trimmed = (userAgent ?? string.Empty).Trim();
        return trimmed.Length <= 512 ? trimmed : trimmed[..512];
    }

    private static string SanitizeIpAddress(string? ipAddress)
    {
        var trimmed = (ipAddress ?? string.Empty).Trim();
        if (trimmed.Length == 0) return "unknown";
        return trimmed.Length <= 64 ? trimmed : trimmed[..64];
    }

    private static bool FixedTimeEqualsAscii(string a, string b)
    {
        var aBytes = Encoding.ASCII.GetBytes(a.Trim());
        var bBytes = Encoding.ASCII.GetBytes(b.Trim());
        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
