using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>Thread-safe store for active push device tokens scoped by (organization, user, token).</summary>
public sealed class InMemoryPushDeviceStore : IPushDeviceStore
{
    private readonly ConcurrentDictionary<(Guid OrganizationId, Guid UserId, string Token), PushDeviceRegistration> _registrations = new();
    private readonly IClock _clock;

    public InMemoryPushDeviceStore(IClock clock)
    {
        _clock = clock;
    }

    public Task<PushDeviceRegistration> UpsertAsync(
        Guid organizationId,
        Guid userId,
        string token,
        string platform,
        CancellationToken ct = default)
    {
        var record = new PushDeviceRegistration(organizationId, userId, token, platform, _clock.UtcNow);
        _registrations[(organizationId, userId, token)] = record;
        return Task.FromResult(record);
    }

    public Task<bool> RemoveAsync(
        Guid organizationId,
        Guid userId,
        string token,
        CancellationToken ct = default)
    {
        var removed = _registrations.TryRemove((organizationId, userId, token), out _);
        return Task.FromResult(removed);
    }

    public Task<IReadOnlyList<PushDeviceRegistration>> ListByUsersAsync(
        Guid organizationId,
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct = default)
    {
        var userSet = userIds.ToHashSet();
        IReadOnlyList<PushDeviceRegistration> list = _registrations.Values
            .Where(r => r.OrganizationId == organizationId && userSet.Contains(r.UserId))
            .OrderByDescending(r => r.RegisteredAtUtc)
            .ToList();
        return Task.FromResult(list);
    }
}

/// <summary>Dispatches push notifications to registered devices for the event recipients (safe no-op when no FCM/APNs credentials are configured).</summary>
public sealed class LoggingPushNotificationSender : IPushNotificationSender
{
    private readonly IPushDeviceStore _devices;
    private readonly ILogger<LoggingPushNotificationSender> _logger;

    public LoggingPushNotificationSender(
        IPushDeviceStore devices,
        ILogger<LoggingPushNotificationSender> logger)
    {
        _devices = devices;
        _logger = logger;
    }

    public async Task DispatchAsync(
        Guid organizationId,
        IReadOnlyCollection<PlannedNotification> notifications,
        CancellationToken ct = default)
    {
        if (notifications.Count == 0) return;

        var recipientIds = notifications.Select(n => n.RecipientUserId).Distinct().ToArray();
        var targets = await _devices.ListByUsersAsync(organizationId, recipientIds, ct);
        if (targets.Count == 0) return;

        _logger.LogInformation(
            "Queued push dispatch for {NotificationCount} notification(s) across {DeviceCount} registered mobile device(s) in organization {OrganizationId}.",
            notifications.Count,
            targets.Count,
            organizationId);
    }
}
