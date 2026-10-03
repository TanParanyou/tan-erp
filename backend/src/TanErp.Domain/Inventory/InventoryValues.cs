namespace TanErp.Domain.Inventory;

public class InventoryDomainException : Exception
{
    public string Code { get; }

    public InventoryDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public static class WarehouseStatus
{
    public const string Active = "active";
    public const string Inactive = "inactive";
}

public static class StockDocumentType
{
    public const string Receipt = "receipt";
    public const string Issue = "issue";
    public const string Transfer = "transfer";
    public const string Adjustment = "adjustment";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal) { Receipt, Issue, Transfer, Adjustment };
}

public static class StockSourceType
{
    public const string GoodsReceipt = "goods_receipt";
}

public static class StockMovementKind
{
    public const string Receipt = "receipt";
    public const string Issue = "issue";
    public const string TransferOut = "transfer_out";
    public const string TransferIn = "transfer_in";
    public const string AdjustmentIn = "adjustment_in";
    public const string AdjustmentOut = "adjustment_out";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Receipt, Issue, TransferOut, TransferIn, AdjustmentIn, AdjustmentOut
    };
}

public static class ReservationStatus
{
    public const string Active = "active";
    public const string Released = "released";
    public const string Consumed = "consumed";
}
