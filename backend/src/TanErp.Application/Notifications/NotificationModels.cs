namespace TanErp.Application.Notifications;

public sealed record NotificationCaller(string FirebaseUid, Guid MembershipId, string TraceId);

public static class NotificationLimits
{
    public const int MaxRecipientsPerEvent = 50;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 50;
}

/// <summary>Payload field names. The set each type may carry is declared in <see cref="NotificationTypeRegistry"/>.</summary>
public static class NotificationFields
{
    public const string ResourceId = "resourceId";
    public const string ParentId = "parentId";
    public const string DocumentNumber = "documentNumber";
    public const string ActorDisplayName = "actorDisplayName";
    public const string SubjectDisplayName = "subjectDisplayName";
    public const string RoleName = "roleName";
}

/// <summary>
/// Something that happened to a document and that other users must act on. <c>Fields</c> never contains <c>actorDisplayName</c>
/// (the publisher adds it from the actor's user record). <c>ExplicitRecipientUserIds</c> is used when the module already chose the
/// reviewers (Estimate approval route); otherwise recipients are the holders of the type's target permission in the organization/branch.
/// <c>TransitionId</c> changes every time the document enters the pending-approval state, so resubmission notifies again.
/// </summary>
public sealed record NotificationEvent(
    string Type,
    Guid OrganizationId,
    Guid? BranchId,
    Guid TransitionId,
    Guid ActorUserId,
    IReadOnlyDictionary<string, string> Fields,
    IReadOnlyCollection<Guid>? ExplicitRecipientUserIds,
    IReadOnlyCollection<Guid> ExcludedUserIds);

public sealed record PlannedNotification(Guid RecipientUserId, string Type, string PayloadJson, string DedupeKey);

public sealed record NotificationRow(Guid Id, string Type, string PayloadJson, DateTimeOffset CreatedAtUtc, DateTimeOffset? ReadAtUtc);

public sealed record NotificationRowPage(IReadOnlyList<NotificationRow> Items, int TotalCount);

public sealed record NotificationListQuery(bool UnreadOnly, int Page, int PageSize);

public sealed record NotificationProjection(
    Guid Id, string Type, IReadOnlyDictionary<string, string> Payload, string? DeepLink, DateTimeOffset CreatedAtUtc, DateTimeOffset? ReadAtUtc);

public sealed record NotificationPage(IReadOnlyList<NotificationProjection> Items, int Page, int PageSize, int TotalCount, int TotalPages);
