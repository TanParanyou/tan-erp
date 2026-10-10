using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// Stages notification rows in the shared AppDbContext. It never persists: the calling store's own save and transaction commit them with
/// the business change, and a failed change leaves nothing behind. Programming errors (unknown type, payload outside the allowlist,
/// unknown actor) throw NotificationDomainException so the business transaction fails loudly instead of silently losing a notification.
/// </summary>
public class NotificationPublisher : INotificationPublisher
{
    private readonly AppDbContext _db;
    private readonly INotificationRecipientResolver _recipients;
    private readonly IClock _clock;

    public NotificationPublisher(AppDbContext db, INotificationRecipientResolver recipients, IClock clock)
    {
        _db = db;
        _recipients = recipients;
        _clock = clock;
    }

    public async Task PublishAsync(NotificationEvent evt, CancellationToken ct = default)
    {
        var descriptor = NotificationTypeRegistry.Find(evt.Type)
            ?? throw new NotificationDomainException("NOTIFICATION_TYPE_INVALID", $"Notification type '{evt.Type}' is not registered.");

        var actorName = await _db.Users.AsNoTracking()
            .Where(u => u.Id == evt.ActorUserId)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "The actor user was not found.");

        var now = _clock.UtcNow;
        IReadOnlyCollection<Guid> candidates = evt.ExplicitRecipientUserIds is { } explicitIds
            ? await ActiveExplicitRecipientsAsync(evt, explicitIds, now, ct)
            : await _recipients.ResolveAsync(evt.OrganizationId, evt.BranchId, descriptor.TargetPermission, evt.ExcludedUserIds, now, ct);

        var plan = NotificationPublishPlanner.Plan(evt, actorName, candidates);
        if (plan.IsFailure) throw new NotificationDomainException(plan.Error.Code, plan.Error.Message);
        if (plan.Value!.Count == 0) return;

        // Idempotent per (recipient, transition): skip rows already stored or already staged in this unit of work.
        var dedupeKey = plan.Value[0].DedupeKey;
        var recipientIds = plan.Value.Select(p => p.RecipientUserId).ToArray();
        var stored = await _db.Notifications.AsNoTracking()
            .Where(n => n.OrganizationId == evt.OrganizationId && n.DedupeKey == dedupeKey && recipientIds.Contains(n.RecipientUserId))
            .Select(n => n.RecipientUserId)
            .ToListAsync(ct);
        var staged = _db.ChangeTracker.Entries<Notification>()
            .Where(e => e.State == EntityState.Added && e.Entity.OrganizationId == evt.OrganizationId && e.Entity.DedupeKey == dedupeKey)
            .Select(e => e.Entity.RecipientUserId);
        var skip = new HashSet<Guid>(stored.Concat(staged));

        foreach (var item in plan.Value.Where(p => !skip.Contains(p.RecipientUserId)))
        {
            _db.Notifications.Add(new Notification(Guid.NewGuid(), evt.OrganizationId, item.RecipientUserId, item.Type, item.PayloadJson, item.DedupeKey, now));
        }
    }

    /// <summary>
    /// Explicit recipients are trusted only for the choice of reviewer, not for membership: anyone without an active membership in
    /// the event's organization/branch (another organization, inactive or expired) is silently dropped.
    /// </summary>
    private async Task<IReadOnlyCollection<Guid>> ActiveExplicitRecipientsAsync(
        NotificationEvent evt, IReadOnlyCollection<Guid> explicitRecipientIds, DateTimeOffset now, CancellationToken ct)
    {
        var requested = explicitRecipientIds.Distinct().ToArray();
        return await NotificationMembershipFilter.ActiveIn(_db, evt.OrganizationId, evt.BranchId, now)
            .Where(m => requested.Contains(m.UserId))
            .Select(m => m.UserId)
            .Distinct()
            .ToListAsync(ct);
    }
}
