namespace TanErp.Domain.Notifications;

public class NotificationDomainException : Exception
{
    public string Code { get; }

    public NotificationDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Code-defined whitelist of notification types. A new type also needs a NotificationTypeRegistry descriptor
/// and a NotificationEvents factory (enforced by tests).
/// </summary>
public static class NotificationTypes
{
    public const string EstimateApprovalRequested = "estimate.approval-requested";
    public const string CostRecordApprovalRequested = "cost-record.approval-requested";
    public const string PurchaseOrderApprovalRequested = "purchase-order.approval-requested";
    public const string ChangeOrderApprovalRequested = "change-order.approval-requested";
    public const string MrpRunApprovalRequested = "mrp-run.approval-requested";
    public const string RoleAssignmentApprovalRequested = "role-assignment.approval-requested";

    private static readonly HashSet<string> Registered = new(StringComparer.Ordinal)
    {
        EstimateApprovalRequested,
        CostRecordApprovalRequested,
        PurchaseOrderApprovalRequested,
        ChangeOrderApprovalRequested,
        MrpRunApprovalRequested,
        RoleAssignmentApprovalRequested
    };

    public static IReadOnlyCollection<string> All => Registered;

    public static bool IsRegistered(string? type) => type is not null && Registered.Contains(type);

    /// <summary>Returns the canonical (trimmed, lower-case) type, or null when it is not registered.</summary>
    public static string? Normalize(string? type)
    {
        var candidate = type?.Trim().ToLowerInvariant();
        return IsRegistered(candidate) ? candidate : null;
    }
}
