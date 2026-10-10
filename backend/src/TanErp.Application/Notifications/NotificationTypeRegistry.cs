using System.Text.RegularExpressions;
using TanErp.Domain.Notifications;

namespace TanErp.Application.Notifications;

/// <summary>
/// What a registered notification type requires. <c>TargetPermission</c> is the approval permission of the source document: it selects
/// the recipients and decides whether a reader still gets a link. <c>RequiredFields</c> is the exact payload allowlist (no optional fields).
/// <c>ReferenceField</c> is the field shown as the subject line in the UI.
/// </summary>
public sealed record NotificationTypeDescriptor(
    string Type,
    string TargetPermission,
    string DeepLinkTemplate,
    string ReferenceField,
    IReadOnlySet<string> RequiredFields);

/// <summary>Code-defined registry. Must stay in step with NotificationTypes (enforced by tests).</summary>
public static partial class NotificationTypeRegistry
{
    private static IReadOnlySet<string> Fields(params string[] names) => new HashSet<string>(names, StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, NotificationTypeDescriptor> Descriptors = new[]
    {
        new NotificationTypeDescriptor(
            NotificationTypes.EstimateApprovalRequested, "estimates.approve", "/estimates/review-queue", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.CostRecordApprovalRequested, "cost-records.approve", "/item-master/cost-reviews", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.PurchaseOrderApprovalRequested, "purchase-orders.approve", "/procurement/purchase-orders/{resourceId}", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.ChangeOrderApprovalRequested, "projects.change-orders.approve", "/projects/{parentId}", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.ParentId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.MrpRunApprovalRequested, "mrp.approve", "/production/mrp/{resourceId}", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.RoleAssignmentApprovalRequested, "roles.assign-approval", "/settings/role-requests", NotificationFields.SubjectDisplayName,
            Fields(NotificationFields.ResourceId, NotificationFields.SubjectDisplayName, NotificationFields.RoleName, NotificationFields.ActorDisplayName))
    }.ToDictionary(d => d.Type, StringComparer.Ordinal);

    public static IReadOnlyCollection<string> Types { get; } = Descriptors.Keys.ToArray();

    /// <summary>Distinct target permissions of every registered type.</summary>
    public static IReadOnlyCollection<string> TargetPermissions { get; } =
        Descriptors.Values.Select(d => d.TargetPermission).Distinct(StringComparer.Ordinal).ToArray();

    /// <summary>Exact (already normalized) type lookup.</summary>
    public static NotificationTypeDescriptor? Find(string? type) =>
        type is not null && Descriptors.TryGetValue(type, out var descriptor) ? descriptor : null;

    [GeneratedRegex(@"\{([A-Za-z]+)\}")]
    private static partial Regex TokenPattern();

    public static IReadOnlyList<string> TemplateTokens(string template) =>
        TokenPattern().Matches(template).Select(m => m.Groups[1].Value).ToArray();

    /// <summary>
    /// Fills the descriptor's deep-link template from the payload. A token is replaced only by a value that parses as a GUID
    /// (re-formatted, never copied verbatim), so a payload can never smuggle a path or query into the link. Returns null otherwise.
    /// </summary>
    public static string? RenderDeepLink(NotificationTypeDescriptor descriptor, IReadOnlyDictionary<string, string> payload)
    {
        var link = descriptor.DeepLinkTemplate;
        foreach (var token in TemplateTokens(link))
        {
            if (!payload.TryGetValue(token, out var raw) || !Guid.TryParse(raw, out var id) || id == Guid.Empty) return null;
            link = link.Replace("{" + token + "}", id.ToString("D"), StringComparison.Ordinal);
        }

        return link;
    }
}
