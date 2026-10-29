using System.Text.Json;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// A user's own notifications. Order of checks: active membership (401/403) → store keyed by (organization, user) → 404 for anything
/// that is not the caller's. There is no permission key: access is "the row is mine". Deep links follow the reader's current permission.
/// </summary>
public class NotificationHandler
{
    private static readonly HashSet<string> AllowedPlatforms = new(StringComparer.OrdinalIgnoreCase) { "ios", "android", "web" };

    private readonly IRequestAccessResolver _access;
    private readonly INotificationStore _store;
    private readonly IPushDeviceStore? _pushDevices;

    public NotificationHandler(IRequestAccessResolver access, INotificationStore store, IPushDeviceStore? pushDevices = null)
    {
        _access = access;
        _store = store;
        _pushDevices = pushDevices;
    }

    private Task<Result<RequestAccessContext>> OwnerAsync(NotificationCaller caller, CancellationToken ct) =>
        _access.ResolveMembershipAsync(caller.FirebaseUid, caller.MembershipId, ct);

    public async Task<Result<NotificationPage>> ListAsync(NotificationCaller caller, bool unreadOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<NotificationPage>.Failure(owner.Error);

        var normalizedPage = Math.Max(page, 1);
        var size = pageSize < 1 ? NotificationLimits.DefaultPageSize : Math.Min(pageSize, NotificationLimits.MaxPageSize);
        var rows = await _store.ListAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, new NotificationListQuery(unreadOnly, normalizedPage, size), ct);

        var granted = new Dictionary<string, bool>(StringComparer.Ordinal);
        var items = new List<NotificationProjection>(rows.Items.Count);
        foreach (var row in rows.Items) items.Add(await ProjectAsync(caller, row, granted, ct));

        var totalPages = (int)Math.Ceiling(rows.TotalCount / (double)size);
        return Result<NotificationPage>.Success(new NotificationPage(items, normalizedPage, size, rows.TotalCount, totalPages));
    }

    public async Task<Result<int>> CountUnreadAsync(NotificationCaller caller, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<int>.Failure(owner.Error);
        return Result<int>.Success(await _store.CountUnreadAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, ct));
    }

    public async Task<Result<NotificationProjection>> MarkReadAsync(NotificationCaller caller, Guid notificationId, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<NotificationProjection>.Failure(owner.Error);

        var row = await _store.MarkReadAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, notificationId, ct);
        if (row is null) return Result<NotificationProjection>.Failure(new Error("NOTIFICATION_NOT_FOUND", "Notification not found."));
        return Result<NotificationProjection>.Success(await ProjectAsync(caller, row, new Dictionary<string, bool>(StringComparer.Ordinal), ct));
    }

    public async Task<Result<int>> MarkAllReadAsync(NotificationCaller caller, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<int>.Failure(owner.Error);
        return Result<int>.Success(await _store.MarkAllReadAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, ct));
    }

    public async Task<Result<PushDeviceProjection>> RegisterDeviceAsync(NotificationCaller caller, string? token, string? platform, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<PushDeviceProjection>.Failure(owner.Error);

        var trimmedToken = token?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(trimmedToken) || trimmedToken.Length > 512)
        {
            return Result<PushDeviceProjection>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Push device token must be between 1 and 512 characters."));
        }

        var normalizedPlatform = platform?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!AllowedPlatforms.Contains(normalizedPlatform))
        {
            return Result<PushDeviceProjection>.Failure(new Error("APP_PLATFORM_INVALID", "Platform must be ios, android, or web."));
        }

        if (_pushDevices is null)
        {
            return Result<PushDeviceProjection>.Success(new PushDeviceProjection(trimmedToken, normalizedPlatform, DateTimeOffset.UtcNow));
        }

        var saved = await _pushDevices.UpsertAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, trimmedToken, normalizedPlatform, ct);
        return Result<PushDeviceProjection>.Success(new PushDeviceProjection(saved.Token, saved.Platform, saved.RegisteredAtUtc));
    }

    public async Task<Result<bool>> UnregisterDeviceAsync(NotificationCaller caller, string? token, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<bool>.Failure(owner.Error);

        var trimmedToken = token?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(trimmedToken) || trimmedToken.Length > 512)
        {
            return Result<bool>.Failure(new Error("REQUEST_VALIDATION_FAILED", "Push device token must be between 1 and 512 characters."));
        }

        if (_pushDevices is null)
        {
            return Result<bool>.Success(false);
        }

        var removed = await _pushDevices.RemoveAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, trimmedToken, ct);
        return Result<bool>.Success(removed);
    }

    private async Task<NotificationProjection> ProjectAsync(NotificationCaller caller, NotificationRow row, Dictionary<string, bool> granted, CancellationToken ct)
    {
        // Stored payloads passed the allowlist when written and the column is a jsonb object, so a flat string map is the contract.
        var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(row.PayloadJson)
            ?? throw new InvalidOperationException($"Notification {row.Id} has a null payload.");

        string? deepLink = null;
        var descriptor = NotificationTypeRegistry.Find(row.Type);
        if (descriptor is not null)
        {
            if (!granted.TryGetValue(descriptor.TargetPermission, out var allowed))
            {
                allowed = (await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, descriptor.TargetPermission, ct)).IsSuccess;
                granted[descriptor.TargetPermission] = allowed;
            }

            if (allowed) deepLink = NotificationTypeRegistry.RenderDeepLink(descriptor, payload);
        }

        return new NotificationProjection(row.Id, row.Type, payload, deepLink, row.CreatedAtUtc, row.ReadAtUtc);
    }
}
