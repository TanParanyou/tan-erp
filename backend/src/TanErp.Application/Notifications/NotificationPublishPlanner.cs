using System.Text.Json;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// Pure part of publishing: validates the event, builds the final payload and picks who gets it. The maker (actor) and any
/// explicitly excluded user never receive it, duplicates collapse, and the list is capped and ordered so retries are stable.
/// A system-generated type has no actor: it must not carry one and gets no actorDisplayName. Every other type requires an actor.
/// Scope filtering (who holds the permission) happens before this, in the recipient resolver.
/// </summary>
public static class NotificationPublishPlanner
{
    public static Result<IReadOnlyList<PlannedNotification>> Plan(NotificationEvent evt, string? actorDisplayName, IReadOnlyCollection<Guid> candidateRecipientUserIds)
    {
        var descriptor = NotificationTypeRegistry.Find(evt.Type);
        if (descriptor is null) return Fail("NOTIFICATION_TYPE_INVALID", $"Notification type '{evt.Type}' is not registered.");

        var system = descriptor.SystemGenerated;
        if (evt.OrganizationId == Guid.Empty || evt.TransitionId == Guid.Empty || (!system && (evt.ActorUserId is null || evt.ActorUserId == Guid.Empty)))
        {
            return Fail("NOTIFICATION_FIELD_INVALID", "An organization, a transition and an actor are required.");
        }

        if (system && evt.ActorUserId is not null)
        {
            return Fail("NOTIFICATION_FIELD_INVALID", "A system-generated notification has no actor.");
        }

        if (evt.Fields.ContainsKey(NotificationFields.ActorDisplayName))
        {
            return Fail("NOTIFICATION_PAYLOAD_INVALID", "The actor display name is added by the publisher and must not be supplied.");
        }

        var fields = new Dictionary<string, string>(evt.Fields, StringComparer.Ordinal);
        if (!system) fields[NotificationFields.ActorDisplayName] = actorDisplayName ?? string.Empty;

        var payload = NotificationPayloadRules.Validate(descriptor, fields);
        if (payload.IsFailure) return Result<IReadOnlyList<PlannedNotification>>.Failure(payload.Error);

        var payloadJson = JsonSerializer.Serialize(payload.Value!);
        var dedupeKey = $"{descriptor.Type}:{evt.TransitionId:N}";
        var excluded = new HashSet<Guid>(evt.ExcludedUserIds);
        if (evt.ActorUserId is { } actorId) excluded.Add(actorId);

        IReadOnlyList<PlannedNotification> planned = candidateRecipientUserIds
            .Where(id => id != Guid.Empty && !excluded.Contains(id))
            .Distinct()
            .OrderBy(id => id)
            .Take(NotificationLimits.MaxRecipientsPerEvent)
            .Select(id => new PlannedNotification(id, descriptor.Type, payloadJson, dedupeKey))
            .ToList();
        return Result<IReadOnlyList<PlannedNotification>>.Success(planned);
    }

    private static Result<IReadOnlyList<PlannedNotification>> Fail(string code, string message) =>
        Result<IReadOnlyList<PlannedNotification>>.Failure(new Error(code, message));
}
