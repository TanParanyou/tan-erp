using TanErp.Application.IdentityAccess.Administration;

namespace TanErp.Application.Audit;

public static class AuditPermissions
{
    public const string Read = "audit.read";
}

public sealed record AuditEventListQuery(
    string? ResourceType, string? ResourceId, string? Action, Guid? ActorUserId, DateTimeOffset? From, DateTimeOffset? To, int Page, int Limit);

/// <summary>The actor is assembled by the backend; the caller never needs to look a user up.</summary>
public sealed record AuditEventView(
    Guid Id, DateTimeOffset OccurredAtUtc, string Action, string ResourceType, string ResourceId, AdminRef Actor,
    Guid? BranchId, string TraceId, string? RequestId, string? Reason, string ChangesJson);

public sealed record AuditEventPage(IReadOnlyList<AuditEventView> Items, int TotalCount, int Page, int PageSize);
