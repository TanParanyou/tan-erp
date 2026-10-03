namespace TanErp.Application.Procurement;

public sealed record ProcurementPerson(Guid Id, string DisplayName, string? Email);

public sealed record SupplierInput(string? NameTh, string? NameEn, string? TaxId, string? ContactName, string? Phone, string? Email, int PaymentTermDays);

public sealed record SupplierProjection(
    Guid Id,
    string Code,
    string NameTh,
    string? NameEn,
    string? TaxId,
    string? ContactName,
    string? Phone,
    string? Email,
    int PaymentTermDays,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc);

public sealed record SupplierListQuery(string? Search, string? Status, int Page, int PageSize);

public sealed record PagedSuppliers(IReadOnlyList<SupplierProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record PurchaseOrderLineInput(Guid ItemId, decimal Quantity, decimal UnitPrice);

public sealed record PurchaseOrderInput(
    Guid SupplierId,
    Guid? ProjectId,
    DateOnly? ExpectedDeliveryDate,
    string? Note,
    IReadOnlyList<PurchaseOrderLineInput> Lines);

public sealed record PurchaseOrderLineProjection(
    Guid Id,
    int LineNo,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    Guid UnitId,
    string UnitCode,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    decimal ReceivedQuantity,
    decimal RemainingQuantity);

public sealed record PurchaseOrderSupplierProjection(Guid Id, string Code, string NameTh, string? NameEn);

public sealed record PurchaseOrderProjectProjection(Guid Id, string Code, string Name);

public sealed record GoodsReceiptSummaryProjection(
    Guid Id,
    string Number,
    DateTimeOffset ReceivedAtUtc,
    string? Note,
    ProcurementPerson ReceivedBy,
    int LineCount,
    decimal TotalQuantity,
    string? StockDocumentNumber);

public sealed record PurchaseOrderProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    string Currency,
    decimal TotalAmount,
    DateOnly? ExpectedDeliveryDate,
    string? Note,
    PurchaseOrderSupplierProjection Supplier,
    PurchaseOrderProjectProjection? Project,
    ProcurementPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProcurementPerson? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    string? CancelReason,
    Guid RowVersion,
    IReadOnlyList<PurchaseOrderLineProjection> Lines,
    IReadOnlyList<GoodsReceiptSummaryProjection> Receipts);

public sealed record PurchaseOrderListItemProjection(
    Guid Id,
    string Number,
    string Status,
    decimal TotalAmount,
    string SupplierCode,
    string SupplierNameTh,
    string? ProjectCode,
    DateOnly? ExpectedDeliveryDate,
    DateTimeOffset CreatedAtUtc);

public sealed record PurchaseOrderListQuery(string? Search, string? Status, Guid? SupplierId, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedPurchaseOrders(IReadOnlyList<PurchaseOrderListItemProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record GoodsReceiptLineInput(Guid PurchaseOrderLineId, decimal Quantity);

public sealed record GoodsReceiptInput(DateTimeOffset? ReceivedAtUtc, string? Note, IReadOnlyList<GoodsReceiptLineInput> Lines);

public enum PurchaseOrderAction
{
    Submit,
    Approve,
    Reject,
    Cancel
}
