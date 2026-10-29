namespace TanErp.Domain.Projects;

public static class ProjectStatus
{
    public const string Planned = "planned";
    public const string Active = "active";
    public const string OnHold = "on_hold";
    public const string ReadyForHandover = "ready_for_handover";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Planned, Active, OnHold, ReadyForHandover, Completed, Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public class ProjectDomainException : Exception
{
    public string Code { get; }

    public ProjectDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class ProjectBudgetCategory
{
    public const string Material = "material";
    public const string Labor = "labor";
    public const string Subcontract = "subcontract";
    public const string Service = "service";
    public const string Other = "other";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Material, Labor, Subcontract, Service, Other
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class ChangeOrderStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Draft, Submitted, Approved, Rejected, Cancelled
    };
}

public static class ProjectCostEntryType
{
    public const string Commitment = "commitment";
    public const string Actual = "actual";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Commitment, Actual
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class ProjectCostSourceType
{
    public const string PurchaseOrder = "purchase_order";
    public const string GoodsReceipt = "goods_receipt";
    public const string SupplierReturn = "supplier_return";
    public const string SupplierInvoice = "supplier_invoice";
    public const string StockIssue = "stock_issue";
    public const string WorkOrderIssue = "work_order_issue";
    public const string WorkOrderReturn = "work_order_return";
    public const string LaborTimesheet = "labor_timesheet";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        PurchaseOrder, GoodsReceipt, SupplierReturn, SupplierInvoice, StockIssue, WorkOrderIssue, WorkOrderReturn, LaborTimesheet
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class TimesheetStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Draft, Submitted, Approved, Rejected, Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}
