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

// ===== Purchase Requests =====

public sealed record PurchaseRequestLineInput(Guid ItemId, decimal Quantity, decimal EstimatedUnitPrice, string? Note = null);

public sealed record PurchaseRequestInput(
    Guid? ProjectId,
    DateOnly? RequiredByDate,
    string? Note,
    IReadOnlyList<PurchaseRequestLineInput> Lines);

public sealed record PurchaseRequestUnitPriceOverrideInput(Guid ItemId, decimal UnitPrice);

public sealed record ConvertPurchaseRequestsInput(
    IReadOnlyList<Guid> PurchaseRequestIds,
    Guid SupplierId,
    DateOnly? ExpectedDeliveryDate,
    string? Note,
    IReadOnlyList<PurchaseRequestUnitPriceOverrideInput>? UnitPriceOverrides);

public sealed record PurchaseRequestLineProjection(
    Guid Id,
    int LineNo,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    Guid UnitId,
    string UnitCode,
    decimal Quantity,
    decimal EstimatedUnitPrice,
    decimal EstimatedLineTotal,
    string? Note);

public sealed record PurchaseRequestConvertedOrderProjection(Guid Id, string Number, string Status);

public sealed record PurchaseRequestProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    string Currency,
    decimal TotalEstimatedAmount,
    DateOnly? RequiredByDate,
    string? Note,
    PurchaseOrderProjectProjection? Project,
    ProcurementPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProcurementPerson? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    string? CancelReason,
    PurchaseRequestConvertedOrderProjection? ConvertedPurchaseOrder,
    DateTimeOffset? ConvertedAtUtc,
    Guid RowVersion,
    IReadOnlyList<PurchaseRequestLineProjection> Lines);

public sealed record PurchaseRequestListItemProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    decimal TotalEstimatedAmount,
    string? ProjectCode,
    DateOnly? RequiredByDate,
    Guid? ConvertedPurchaseOrderId,
    string? ConvertedPurchaseOrderNumber,
    DateTimeOffset CreatedAtUtc);

public sealed record PurchaseRequestListQuery(string? Search, string? Status, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedPurchaseRequests(IReadOnlyList<PurchaseRequestListItemProjection> Items, int TotalCount, int Page, int PageSize);

public enum PurchaseRequestAction
{
    Submit,
    Approve,
    Reject,
    Cancel
}

// ===== Purchase Orders, Goods Receipts & Supplier Returns =====

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
    decimal RemainingQuantity,
    decimal InvoicedQuantity,
    decimal UninvoicedReceivedQuantity);

public sealed record PurchaseOrderSupplierProjection(Guid Id, string Code, string NameTh, string? NameEn);

public sealed record PurchaseOrderProjectProjection(Guid Id, string Code, string Name);

public sealed record GoodsReceiptLineProjection(
    Guid Id,
    Guid PurchaseOrderLineId,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    string UnitCode,
    decimal Quantity,
    decimal UnitPrice,
    decimal ReturnedQuantity,
    decimal RemainingReturnableQuantity);

public sealed record GoodsReceiptSummaryProjection(
    Guid Id,
    string Number,
    DateTimeOffset ReceivedAtUtc,
    string? Note,
    ProcurementPerson ReceivedBy,
    int LineCount,
    decimal TotalQuantity,
    string? StockDocumentNumber,
    IReadOnlyList<GoodsReceiptLineProjection> Lines);

public sealed record SupplierReturnLineInput(Guid GoodsReceiptLineId, decimal Quantity);

public sealed record SupplierReturnInput(
    Guid GoodsReceiptId,
    DateTimeOffset? ReturnedAtUtc,
    string? Reason,
    IReadOnlyList<SupplierReturnLineInput> Lines);

public sealed record SupplierReturnLineProjection(
    Guid Id,
    Guid GoodsReceiptLineId,
    Guid PurchaseOrderLineId,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    string UnitCode,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineReturnAmount);

public sealed record SupplierReturnProjection(
    Guid Id,
    string Number,
    Guid GoodsReceiptId,
    string GoodsReceiptNumber,
    DateTimeOffset ReturnedAtUtc,
    string Reason,
    decimal TotalReturnAmount,
    string? StockDocumentNumber,
    ProcurementPerson ReturnedBy,
    IReadOnlyList<SupplierReturnLineProjection> Lines);

public sealed record PurchaseOrderInvoiceSummaryProjection(
    Guid Id,
    string Number,
    string SupplierInvoiceNumber,
    string Status,
    string MatchStatus,
    DateOnly InvoiceDate,
    decimal SubtotalAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal TotalVarianceAmount,
    DateTimeOffset CreatedAtUtc);

public sealed record PurchaseOrderSourceRequestProjection(
    Guid Id,
    string Number,
    DateTimeOffset? ConvertedAtUtc);

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
    IReadOnlyList<GoodsReceiptSummaryProjection> Receipts,
    IReadOnlyList<SupplierReturnProjection> Returns,
    IReadOnlyList<PurchaseOrderInvoiceSummaryProjection> Invoices,
    IReadOnlyList<PurchaseOrderSourceRequestProjection> SourcePurchaseRequests);

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

// ===== Supplier Invoices =====

public sealed record SupplierInvoiceLineInput(Guid PurchaseOrderLineId, decimal Quantity, decimal UnitPrice);

public sealed record SupplierInvoiceInput(
    Guid PurchaseOrderId,
    string? SupplierInvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    decimal TaxAmount,
    string? Note,
    IReadOnlyList<SupplierInvoiceLineInput> Lines);

public sealed record SupplierInvoiceLineProjection(
    Guid Id,
    int LineNo,
    Guid PurchaseOrderLineId,
    Guid ItemId,
    string ItemCode,
    string ItemNameTh,
    Guid UnitId,
    string UnitCode,
    decimal Quantity,
    decimal PoUnitPrice,
    decimal UnitPrice,
    decimal LineTotal,
    decimal PriceVarianceAmount,
    decimal PriceVariancePercent,
    bool ExceedsTolerance);

public sealed record SupplierInvoiceOrderRefProjection(Guid Id, string Number, string Status);

public sealed record SupplierInvoiceProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string SupplierInvoiceNumber,
    string Status,
    string MatchStatus,
    string Currency,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    decimal SubtotalAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal TotalPoAmount,
    decimal TotalVarianceAmount,
    string? Note,
    SupplierInvoiceOrderRefProjection PurchaseOrder,
    PurchaseOrderSupplierProjection Supplier,
    PurchaseOrderProjectProjection? Project,
    ProcurementPerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    DateTimeOffset? PostedAtUtc,
    ProcurementPerson? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    string? CancelReason,
    string? VoidReason,
    ProcurementPerson? VoidedBy,
    DateTimeOffset? VoidedAtUtc,
    Guid RowVersion,
    IReadOnlyList<SupplierInvoiceLineProjection> Lines);

public sealed record SupplierInvoiceListItemProjection(
    Guid Id,
    string Number,
    string SupplierInvoiceNumber,
    string Status,
    string MatchStatus,
    Guid PurchaseOrderId,
    string PurchaseOrderNumber,
    string SupplierCode,
    string SupplierNameTh,
    string? ProjectCode,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    decimal SubtotalAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal TotalVarianceAmount,
    DateTimeOffset CreatedAtUtc);

public sealed record SupplierInvoiceListQuery(string? Search, string? Status, Guid? SupplierId, Guid? PurchaseOrderId, int Page, int PageSize);

public sealed record PagedSupplierInvoices(IReadOnlyList<SupplierInvoiceListItemProjection> Items, int TotalCount, int Page, int PageSize);

public enum SupplierInvoiceAction
{
    Submit,
    Approve,
    Reject,
    Cancel,
    Void
}
