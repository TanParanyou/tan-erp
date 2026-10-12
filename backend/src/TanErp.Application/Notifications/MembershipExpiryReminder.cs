using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace TanErp.Application.Notifications;

/// <summary>
/// Settings under <c>MembershipExpiryReminder</c>. The thresholds are TEST_ONLY defaults pending the System Admin's real policy.
/// Out-of-range values are clamped, never rejected, so a typo cannot stop the application from starting.
/// </summary>
public sealed class MembershipExpiryReminderOptions
{
    public const string SectionName = "MembershipExpiryReminder";

    /// <summary>Ascending. Used when <see cref="ThresholdDays"/> is missing or has no valid value.</summary>
    public static IReadOnlyList<int> DefaultThresholdDays { get; } = [1, 7, 30];

    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
    public int StartupDelaySeconds { get; set; } = 60;
    public int MaxMembershipsPerRun { get; set; } = 200;

    /// <summary>Null (not an initialized array) so configuration binding replaces rather than appends.</summary>
    public int[]? ThresholdDays { get; set; }

    public TimeSpan EffectiveInterval => TimeSpan.FromMinutes(Math.Clamp(IntervalMinutes, 5, 1440));
    public TimeSpan EffectiveStartupDelay => TimeSpan.FromSeconds(Math.Clamp(StartupDelaySeconds, 0, 3600));
    public int EffectiveMaxMemberships => Math.Clamp(MaxMembershipsPerRun, 1, 1000);
}

/// <summary>Pure rules of the reminder: which threshold applies and which transition id makes a reminder unique.</summary>
public static class MembershipExpiryReminderPlanner
{
    public const int MaxThresholdDays = 365;

    /// <summary>Valid values (1-365) de-duplicated and ascending; the default list when none is valid.</summary>
    public static IReadOnlyList<int> NormalizeThresholds(IEnumerable<int>? configured)
    {
        var values = configured?.Where(d => d is >= 1 and <= MaxThresholdDays).Distinct().OrderBy(d => d).ToArray() ?? [];
        return values.Length == 0 ? MembershipExpiryReminderOptions.DefaultThresholdDays : values;
    }

    /// <summary>
    /// The smallest threshold the remaining time fits in (so 20 days left with 30/7/1 is "30", 6 days is "7", 12 hours is "1"),
    /// or null when the membership is already expired or still beyond the largest threshold. Expired memberships are never reminded.
    /// </summary>
    public static int? SelectThreshold(DateTimeOffset now, DateTimeOffset expiresAtUtc, IReadOnlyList<int> thresholdsAscending)
    {
        var remaining = expiresAtUtc - now;
        if (remaining <= TimeSpan.Zero) return null;
        foreach (var days in thresholdsAscending)
        {
            if (remaining <= TimeSpan.FromDays(days)) return days;
        }

        return null;
    }

    /// <summary>
    /// Deterministic id of one reminder: the same (membership, threshold, expiry) always gives the same value, so the notification
    /// dedupe key stores it once; a changed expiry gives a new value and re-arms the reminder.
    /// </summary>
    public static Guid TransitionId(Guid membershipId, int thresholdDays, DateTimeOffset expiresAtUtc)
    {
        var text = string.Create(CultureInfo.InvariantCulture, $"membership-expiring|{membershipId:N}|{thresholdDays}|{expiresAtUtc.UtcTicks}");
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return new Guid(hash.AsSpan(0, 16));
    }
}

/// <summary>What one scan did. <c>SkippedBecauseLocked</c> means another instance holds the advisory lock.</summary>
public sealed record MembershipExpiryReminderRunResult(int Scanned, int Created, int Failed, bool SkippedBecauseLocked);

/// <summary>Runs one scan. Everything time-based is read from <see cref="Common.Abstractions.IClock"/>; the schedule lives in the host.</summary>
public interface IMembershipExpiryReminderService
{
    Task<MembershipExpiryReminderRunResult> RunOnceAsync(CancellationToken ct = default);
}
