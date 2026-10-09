namespace TanErp.Application.Notifications;

/// <summary>
/// Stages notifications in the caller's current unit of work. It never calls SaveChanges:
/// the module's own SaveChanges/transaction commits them together with the business change, so a rolled-back change leaves nothing behind.
/// A bad event (unknown type, payload outside the allowlist) throws NotificationDomainException so the bug is loud, never silent.
/// </summary>
public interface INotificationPublisher
{
    Task PublishAsync(NotificationEvent evt, CancellationToken ct = default);
}

/// <summary>Active users who hold <paramref name="permissionKey"/> in the organization (and the branch when given), minus the excluded ones.</summary>
public interface INotificationRecipientResolver
{
    Task<IReadOnlyList<Guid>> ResolveAsync(
        Guid organizationId, Guid? branchId, string permissionKey, IReadOnlyCollection<Guid> excludedUserIds, DateTimeOffset atUtc, CancellationToken ct = default);
}

/// <summary>Reads and updates a user's own notifications. Every method is keyed by (organization, user): another user's row is never reachable.</summary>
public interface INotificationStore
{
    Task<NotificationRowPage> ListAsync(Guid organizationId, Guid userId, NotificationListQuery query, CancellationToken ct = default);

    Task<int> CountUnreadAsync(Guid organizationId, Guid userId, CancellationToken ct = default);

    /// <summary>Marks one notification read (idempotent) and returns it, or null when it is not this user's in this organization.</summary>
    Task<NotificationRow?> MarkReadAsync(Guid organizationId, Guid userId, Guid notificationId, CancellationToken ct = default);

    Task<int> MarkAllReadAsync(Guid organizationId, Guid userId, CancellationToken ct = default);
}
