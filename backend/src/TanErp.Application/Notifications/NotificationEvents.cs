using TanErp.Domain.Notifications;

namespace TanErp.Application.Notifications;

/// <summary>
/// The only place that decides which fields each source puts in a notification. Stores pass identifiers and display strings,
/// never amounts: there is no parameter here that could carry a figure.
/// </summary>
public static class NotificationEvents
{
    private static string Id(Guid id) => id.ToString("D");

    private static Dictionary<string, string> Fields(params (string Key, string Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);

    /// <summary>Estimate submitted for approval. Notifies the first reviewer of the approval route (already independent of the submitter).</summary>
    public static NotificationEvent EstimateSubmitted(
        Guid organizationId, Guid branchId, Guid approvalRequestId, Guid actorUserId, Guid estimateId, string estimateNumber, Guid firstReviewerUserId) =>
        new(NotificationTypes.EstimateApprovalRequested, organizationId, branchId, approvalRequestId, actorUserId,
            Fields((NotificationFields.ResourceId, Id(estimateId)), (NotificationFields.DocumentNumber, estimateNumber)),
            [firstReviewerUserId], []);

    /// <summary>Cost record submitted. The creator and the last financial editor cannot approve it, so they are not notified.</summary>
    public static NotificationEvent CostRecordSubmitted(
        Guid organizationId, Guid? branchId, Guid newRowVersion, Guid actorUserId, Guid costRecordId, string itemCode, IReadOnlyCollection<Guid> makerUserIds) =>
        new(NotificationTypes.CostRecordApprovalRequested, organizationId, branchId, newRowVersion, actorUserId,
            Fields((NotificationFields.ResourceId, Id(costRecordId)), (NotificationFields.DocumentNumber, itemCode)),
            null, makerUserIds);

    public static NotificationEvent PurchaseOrderSubmitted(
        Guid organizationId, Guid branchId, Guid newRowVersion, Guid actorUserId, Guid purchaseOrderId, string number, Guid creatorUserId) =>
        new(NotificationTypes.PurchaseOrderApprovalRequested, organizationId, branchId, newRowVersion, actorUserId,
            Fields((NotificationFields.ResourceId, Id(purchaseOrderId)), (NotificationFields.DocumentNumber, number)),
            null, [creatorUserId]);

    public static NotificationEvent ChangeOrderSubmitted(
        Guid organizationId, Guid branchId, Guid newRowVersion, Guid actorUserId, Guid changeOrderId, Guid projectId, string number, Guid creatorUserId) =>
        new(NotificationTypes.ChangeOrderApprovalRequested, organizationId, branchId, newRowVersion, actorUserId,
            Fields((NotificationFields.ResourceId, Id(changeOrderId)), (NotificationFields.ParentId, Id(projectId)), (NotificationFields.DocumentNumber, number)),
            null, [creatorUserId]);

    /// <summary>A planning run with at least one proposed recommendation. The run's creator cannot decide it.</summary>
    public static NotificationEvent MrpRunCreated(Guid organizationId, Guid branchId, Guid runId, Guid actorUserId, string number) =>
        new(NotificationTypes.MrpRunApprovalRequested, organizationId, branchId, runId, actorUserId,
            Fields((NotificationFields.ResourceId, Id(runId)), (NotificationFields.DocumentNumber, number)),
            null, []);

    /// <summary>Role assignment that needs a checker. The user receiving the role cannot approve their own change.</summary>
    public static NotificationEvent RoleAssignmentRequested(
        Guid organizationId, Guid requestId, Guid actorUserId, Guid subjectUserId, string subjectDisplayName, string roleName) =>
        new(NotificationTypes.RoleAssignmentApprovalRequested, organizationId, null, requestId, actorUserId,
            Fields((NotificationFields.ResourceId, Id(requestId)), (NotificationFields.SubjectDisplayName, subjectDisplayName), (NotificationFields.RoleName, roleName)),
            null, [subjectUserId]);
}
