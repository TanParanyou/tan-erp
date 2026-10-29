namespace TanErp.Domain.Procurement;

public class ProcurementDomainException : Exception
{
    public string Code { get; }

    public ProcurementDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class SupplierStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";

    public static bool IsValid(string? value) => value is Active or Inactive;
}

public static class PurchaseOrderStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string PartiallyReceived = "partially_received";
    public const string Received = "received";
    public const string Cancelled = "cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Draft, Submitted, Approved, Rejected, PartiallyReceived, Received, Cancelled
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class PurchaseRequestStatus
{
    public const string Draft = "draft";
    public const string Submitted = "submitted";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Cancelled = "cancelled";
    public const string Converted = "converted";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Draft, Submitted, Approved, Rejected, Cancelled, Converted
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class SupplierInvoiceStatus
{
    public const string Draft = "draft";
    public const string OnHold = "on_hold";
    public const string Matched = "matched";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Cancelled = "cancelled";
    public const string Voided = "voided";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Draft, OnHold, Matched, Approved, Rejected, Cancelled, Voided
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}

public static class SupplierInvoiceMatchStatus
{
    public const string Pending = "pending";
    public const string Matched = "matched";
    public const string PriceVariance = "price_variance";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Pending, Matched, PriceVariance
    };

    public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value) && All.Contains(value.Trim());
}
