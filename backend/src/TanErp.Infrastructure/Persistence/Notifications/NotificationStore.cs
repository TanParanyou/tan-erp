using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>Every query is keyed by (organization, recipient): another user's or organization's row is simply not found.</summary>
public class NotificationStore : INotificationStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public NotificationStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private IQueryable<Notification> Own(Guid organizationId, Guid userId) =>
        _db.Notifications.Where(n => n.OrganizationId == organizationId && n.RecipientUserId == userId);

    public async Task<NotificationRowPage> ListAsync(Guid organizationId, Guid userId, NotificationListQuery query, CancellationToken ct = default)
    {
        var rows = Own(organizationId, userId).AsNoTracking();
        if (query.UnreadOnly) rows = rows.Where(n => n.ReadAtUtc == null);

        var total = await rows.CountAsync(ct);
        var items = await rows
            .OrderByDescending(n => n.CreatedAtUtc).ThenByDescending(n => n.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(n => new NotificationRow(n.Id, n.Type, n.PayloadJson, n.CreatedAtUtc, n.ReadAtUtc))
            .ToListAsync(ct);
        return new NotificationRowPage(items, total);
    }

    public Task<int> CountUnreadAsync(Guid organizationId, Guid userId, CancellationToken ct = default) =>
        Own(organizationId, userId).AsNoTracking().CountAsync(n => n.ReadAtUtc == null, ct);

    public async Task<NotificationRow?> MarkReadAsync(Guid organizationId, Guid userId, Guid notificationId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;
        // Only unread rows are touched, so the first read time survives repeated calls.
        await Own(organizationId, userId)
            .Where(n => n.Id == notificationId && n.ReadAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAtUtc, now), ct);

        return await Own(organizationId, userId).AsNoTracking()
            .Where(n => n.Id == notificationId)
            .Select(n => new NotificationRow(n.Id, n.Type, n.PayloadJson, n.CreatedAtUtc, n.ReadAtUtc))
            .FirstOrDefaultAsync(ct);
    }

    public Task<int> MarkAllReadAsync(Guid organizationId, Guid userId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;
        return Own(organizationId, userId)
            .Where(n => n.ReadAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAtUtc, now), ct);
    }
}
