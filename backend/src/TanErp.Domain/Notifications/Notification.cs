using System.Text.Json;
using TanErp.Domain.Common;

namespace TanErp.Domain.Notifications;

/// <summary>
/// One in-app message to one user in one organization. Created in the same transaction as the business change that caused it.
/// The payload is a flat JSON object of display-safe values (document number, display names); it never carries figures.
/// </summary>
public class Notification : Entity
{
    public const int MaxPayloadLength = 2000;
    public const int MaxDedupeKeyLength = 160;

    public Guid OrganizationId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = "{}";
    public string DedupeKey { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ReadAtUtc { get; private set; }

    public bool IsRead => ReadAtUtc.HasValue;

    protected Notification() { }

    public Notification(
        Guid id, Guid organizationId, Guid recipientUserId, string type, string payloadJson, string dedupeKey, DateTimeOffset createdAtUtc) : base(id)
    {
        if (organizationId == Guid.Empty || recipientUserId == Guid.Empty)
        {
            throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "An organization and a recipient are required.");
        }

        var normalizedType = NotificationTypes.Normalize(type)
            ?? throw new NotificationDomainException("NOTIFICATION_TYPE_INVALID", $"Notification type '{type}' is not registered.");

        var key = dedupeKey?.Trim() ?? string.Empty;
        if (key.Length is 0 or > MaxDedupeKeyLength)
        {
            throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", $"A dedupe key of 1-{MaxDedupeKeyLength} characters is required.");
        }

        if (!IsJsonObject(payloadJson) || payloadJson.Length > MaxPayloadLength)
        {
            throw new NotificationDomainException("NOTIFICATION_PAYLOAD_INVALID", $"The payload must be a JSON object of at most {MaxPayloadLength} characters.");
        }

        OrganizationId = organizationId;
        RecipientUserId = recipientUserId;
        Type = normalizedType;
        PayloadJson = payloadJson;
        DedupeKey = key;
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    /// <summary>Marks the notification as read. Returns false (keeping the first read time) when it was already read.</summary>
    public bool MarkRead(DateTimeOffset now)
    {
        if (ReadAtUtc.HasValue) return false;
        ReadAtUtc = now.ToUniversalTime();
        return true;
    }

    private static bool IsJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
