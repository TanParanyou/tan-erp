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

public sealed record ProcurementPersonResponse(Guid Id, string DisplayName, string? Email);

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
    decimal RemainingQuantity);

public sealed record PurchaseOrderSupplierResponse(Guid Id, string Code, string NameTh, string? NameEn);

public sealed record PurchaseOrderProjectResponse(Guid Id, string Code, string Name);

public sealed record GoodsReceiptSummaryResponse(
    Guid Id,
    string Number,
    DateTimeOffset ReceivedAtUtc,
    string? Note,
    ProcurementPersonResponse ReceivedBy,
    int LineCount,
    decimal TotalQuantity,
    string? StockDocumentNumber);

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
    IReadOnlyList<GoodsReceiptSummaryResponse> Receipts);

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
