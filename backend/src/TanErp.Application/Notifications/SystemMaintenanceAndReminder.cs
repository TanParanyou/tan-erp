using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace TanErp.Application.Notifications;

public sealed class SystemMaintenanceAndReminderOptions
{
    public const string SectionName = "SystemMaintenanceAndReminder";

    public static IReadOnlyList<int> DefaultWarrantyThresholdDays { get; } = [1, 7, 30];

    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
    public int StartupDelaySeconds { get; set; } = 60;
    public int MaxItemsPerRun { get; set; } = 200;
    public int OrphanFileGraceHours { get; set; } = 24;
    public int[]? WarrantyThresholdDays { get; set; }

    public TimeSpan EffectiveInterval => TimeSpan.FromMinutes(Math.Clamp(IntervalMinutes, 5, 1440));
    public TimeSpan EffectiveStartupDelay => TimeSpan.FromSeconds(Math.Clamp(StartupDelaySeconds, 0, 3600));
    public int EffectiveMaxItems => Math.Clamp(MaxItemsPerRun, 1, 1000);
    public TimeSpan EffectiveOrphanGrace => TimeSpan.FromHours(Math.Clamp(OrphanFileGraceHours, 1, 720));
}

public static class SystemMaintenanceAndReminderPlanner
{
    public const int MaxThresholdDays = 365;

    public static IReadOnlyList<int> NormalizeWarrantyThresholds(IEnumerable<int>? configured)
    {
        var values = configured?.Where(d => d is >= 1 and <= MaxThresholdDays).Distinct().OrderBy(d => d).ToArray() ?? [];
        return values.Length == 0 ? SystemMaintenanceAndReminderOptions.DefaultWarrantyThresholdDays : values;
    }

    public static int? SelectWarrantyThreshold(DateOnly today, DateOnly endDate, IReadOnlyList<int> thresholdsAscending)
    {
        var remainingDays = endDate.DayNumber - today.DayNumber;
        if (remainingDays < 0) return null;
        foreach (var days in thresholdsAscending)
        {
            if (remainingDays <= days) return days;
        }

        return null;
    }

    public static Guid WarrantyTransitionId(Guid warrantyId, int thresholdDays, DateOnly endDate)
    {
        var text = string.Create(CultureInfo.InvariantCulture, $"warranty-expiring|{warrantyId:N}|{thresholdDays}|{endDate.DayNumber}");
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return new Guid(hash.AsSpan(0, 16));
    }

    public static Guid ServiceSlaTransitionId(Guid serviceRequestId, string slaStatus)
    {
        var text = string.Create(CultureInfo.InvariantCulture, $"service-sla|{serviceRequestId:N}|{slaStatus}");
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return new Guid(hash.AsSpan(0, 16));
    }
}

public sealed record SystemMaintenanceAndReminderRunResult(
    int SlaScanned,
    int SlaNotificationsCreated,
    int WarrantiesScanned,
    int WarrantyNotificationsCreated,
    int ExpiredSessionsDeleted,
    int OrphanFilesDeleted,
    int ExpiredAuditEventsDeleted,
    bool SkippedBecauseLocked);

public interface ISystemMaintenanceAndReminderService
{
    Task<SystemMaintenanceAndReminderRunResult> RunOnceAsync(CancellationToken ct = default);
}
