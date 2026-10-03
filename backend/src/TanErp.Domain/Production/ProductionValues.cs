namespace TanErp.Domain.Production;

public class ProductionDomainException : Exception
{
    public string Code { get; }

    public ProductionDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class BomRevisionStatus
{
    public const string Draft = "draft";
    public const string Approved = "approved";
    public const string Obsolete = "obsolete";
}

public static class WorkOrderStatus
{
    public const string Draft = "draft";
    public const string Released = "released";
    public const string InProgress = "in_progress";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Draft, Released, InProgress, Completed, Cancelled
    };
}

public static class WorkOrderTransactionKind
{
    public const string Issue = "issue";
    public const string Return = "return";
    public const string Completion = "completion";
}
