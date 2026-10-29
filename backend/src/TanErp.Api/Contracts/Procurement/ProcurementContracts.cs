namespace TanErp.Api.Contracts.Procurement;

public sealed record SupplierRequest(string? NameTh, string? NameEn, string? TaxId, string? ContactName, string? Phone, string? Email, int PaymentTermDays);

public sealed record SupplierResponse(
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

public sealed record ProcurementPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record SupplierListResponse(IReadOnlyList<SupplierResponse> Items, ProcurementPaginationResponse Pagination);

public sealed record ProcurementPersonResponse(Guid Id, string DisplayName, string? Email);

// ===== Purchase Requests =====

public sealed record PurchaseRequestLineRequest(Guid ItemId, decimal Quantity, decimal EstimatedUnitPrice, string? Note = null);

public sealed record PurchaseRequestRequest(
    Guid? ProjectId,
    DateOnly? RequiredByDate,
    string? Note,
    List<PurchaseRequestLineRequest>? Lines);

public sealed record PurchaseRequestActionRequest(string? Note);

public sealed record PurchaseRequestUnitPriceOverrideRequest(Guid ItemId, decimal UnitPrice);

public sealed record ConvertPurchaseRequestsRequest(
    List<Guid>? PurchaseRequestIds,
    Guid SupplierId,
    DateOnly? ExpectedDeliveryDate,
    string? Note,
    List<PurchaseRequestUnitPriceOverrideRequest>? UnitPriceOverrides);

public sealed record PurchaseRequestLineResponse(
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

public sealed record PurchaseRequestConvertedOrderResponse(Guid Id, string Number, string Status);

public sealed record PurchaseRequestResponse(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    string Currency,
    decimal TotalEstimatedAmount,
    DateOnly? RequiredByDate,
    string? Note,
    PurchaseOrderProjectResponse? Project,
    ProcurementPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProcurementPersonResponse? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    string? CancelReason,
    PurchaseRequestConvertedOrderResponse? ConvertedPurchaseOrder,
    DateTimeOffset? ConvertedAtUtc,
    Guid RowVersion,
    IReadOnlyList<PurchaseRequestLineResponse> Lines);

public sealed record PurchaseRequestListItemResponse(
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

public sealed record PurchaseRequestListResponse(IReadOnlyList<PurchaseRequestListItemResponse> Items, ProcurementPaginationResponse Pagination);

// ===== Purchase Orders, Goods Receipts & Supplier Returns =====

public sealed record PurchaseOrderLineRequest(Guid ItemId, decimal Quantity, decimal UnitPrice);

public sealed record PurchaseOrderRequest(
    Guid SupplierId,
    Guid? ProjectId,
    DateOnly? ExpectedDeliveryDate,
    string? Note,
    List<PurchaseOrderLineRequest>? Lines);

public sealed record PurchaseOrderActionRequest(string? Note);

public sealed record GoodsReceiptLineRequest(Guid PurchaseOrderLineId, decimal Quantity);

public sealed record GoodsReceiptRequest(DateTimeOffset? ReceivedAtUtc, string? Note, List<GoodsReceiptLineRequest>? Lines);

public sealed record SupplierReturnLineRequest(Guid GoodsReceiptLineId, decimal Quantity);

public sealed record SupplierReturnRequest(
    Guid GoodsReceiptId,
    DateTimeOffset? ReturnedAtUtc,
    string? Reason,
    List<SupplierReturnLineRequest>? Lines);

public sealed record PurchaseOrderLineResponse(
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

public sealed record PurchaseOrderSupplierResponse(Guid Id, string Code, string NameTh, string? NameEn);

public sealed record PurchaseOrderProjectResponse(Guid Id, string Code, string Name);

public sealed record GoodsReceiptLineResponse(
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

public sealed record GoodsReceiptSummaryResponse(
    Guid Id,
    string Number,
    DateTimeOffset ReceivedAtUtc,
    string? Note,
    ProcurementPersonResponse ReceivedBy,
    int LineCount,
    decimal TotalQuantity,
    string? StockDocumentNumber,
    IReadOnlyList<GoodsReceiptLineResponse> Lines);

public sealed record SupplierReturnLineResponse(
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

public sealed record SupplierReturnResponse(
    Guid Id,
    string Number,
    Guid GoodsReceiptId,
    string GoodsReceiptNumber,
    DateTimeOffset ReturnedAtUtc,
    string Reason,
    decimal TotalReturnAmount,
    string? StockDocumentNumber,
    ProcurementPersonResponse ReturnedBy,
    IReadOnlyList<SupplierReturnLineResponse> Lines);

public sealed record PurchaseOrderInvoiceSummaryResponse(
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

public sealed record PurchaseOrderSourceRequestResponse(
    Guid Id,
    string Number,
    DateTimeOffset? ConvertedAtUtc);

public sealed record PurchaseOrderResponse(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    string Currency,
    decimal TotalAmount,
    DateOnly? ExpectedDeliveryDate,
    string? Note,
    PurchaseOrderSupplierResponse Supplier,
    PurchaseOrderProjectResponse? Project,
    ProcurementPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    ProcurementPersonResponse? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    string? CancelReason,
    Guid RowVersion,
    IReadOnlyList<PurchaseOrderLineResponse> Lines,
    IReadOnlyList<GoodsReceiptSummaryResponse> Receipts,
    IReadOnlyList<SupplierReturnResponse> Returns,
    IReadOnlyList<PurchaseOrderInvoiceSummaryResponse> Invoices,
    IReadOnlyList<PurchaseOrderSourceRequestResponse> SourcePurchaseRequests);

public sealed record PurchaseOrderListItemResponse(
    Guid Id,
    string Number,
    string Status,
    decimal TotalAmount,
    string SupplierCode,
    string SupplierNameTh,
    string? ProjectCode,
    DateOnly? ExpectedDeliveryDate,
    DateTimeOffset CreatedAtUtc);

public sealed record PurchaseOrderListResponse(IReadOnlyList<PurchaseOrderListItemResponse> Items, ProcurementPaginationResponse Pagination);

// ===== Supplier Invoices =====

public sealed record SupplierInvoiceLineRequest(Guid PurchaseOrderLineId, decimal Quantity, decimal UnitPrice);

public sealed record SupplierInvoiceRequest(
    Guid PurchaseOrderId,
    string? SupplierInvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    decimal TaxAmount,
    string? Note,
    List<SupplierInvoiceLineRequest>? Lines);

public sealed record SupplierInvoiceActionRequest(string? Note);

public sealed record SupplierInvoiceLineResponse(
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

public sealed record SupplierInvoiceOrderRefResponse(Guid Id, string Number, string Status);

public sealed record SupplierInvoiceResponse(
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
    SupplierInvoiceOrderRefResponse PurchaseOrder,
    PurchaseOrderSupplierResponse Supplier,
    PurchaseOrderProjectResponse? Project,
    ProcurementPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    DateTimeOffset? PostedAtUtc,
    ProcurementPersonResponse? DecidedBy,
    DateTimeOffset? DecidedAtUtc,
    string? DecisionNote,
    string? CancelReason,
    string? VoidReason,
    ProcurementPersonResponse? VoidedBy,
    DateTimeOffset? VoidedAtUtc,
    Guid RowVersion,
    IReadOnlyList<SupplierInvoiceLineResponse> Lines);

public sealed record SupplierInvoiceListItemResponse(
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

public sealed record SupplierInvoiceListResponse(IReadOnlyList<SupplierInvoiceListItemResponse> Items, ProcurementPaginationResponse Pagination);
