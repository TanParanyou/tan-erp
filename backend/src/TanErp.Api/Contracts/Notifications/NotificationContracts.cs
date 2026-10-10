using TanErp.Api.Contracts.Common;

namespace TanErp.Api.Contracts.Notifications;

/// <summary>One notification of the caller. <c>Payload</c> is display-safe strings only (document number, display names); <c>DeepLink</c> is null when the reader no longer holds the target permission.</summary>
public sealed record NotificationResponse(
    Guid Id,
    string Type,
    IReadOnlyDictionary<string, string> Payload,
    string? DeepLink,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ReadAtUtc);

public sealed record NotificationListResponse(IReadOnlyList<NotificationResponse> Items, PaginationMetadataResponse Pagination);

public sealed record UnreadCountResponse(int UnreadCount);

public sealed record MarkAllReadResponse(int UpdatedCount);
