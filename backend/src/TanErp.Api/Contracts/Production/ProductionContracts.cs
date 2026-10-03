namespace TanErp.Api.Contracts.Production;

public sealed record ProductionPersonResponse(Guid Id, string DisplayName, string? Email);

public sealed record ProductionItemResponse(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record ProductionPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record BomLineRequest(Guid ComponentItemId, decimal Quantity, decimal ScrapPercent);

public sealed record BomRequest(Guid ItemId, decimal OutputQuantity, string? Note, List<BomLineRequest>? Lines);

public sealed record BomDraftRequest(decimal OutputQuantity, string? Note, List<BomLineRequest>? Lines);

public sealed record BomLineResponse(Guid Id, ProductionItemResponse Component, decimal Quantity, decimal ScrapPercent, decimal GrossQuantity);

public sealed record BomRevisionResponse(
    Guid Id,
    int RevisionNo,
    string Status,
    decimal OutputQuantity,
    string? Note,
    ProductionPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    ProductionPersonResponse? ApprovedBy,
    DateTimeOffset? ApprovedAtUtc,
    Guid RowVersion,
    IReadOnlyList<BomLineResponse> Lines);

public sealed record BomResponse(Guid Id, string Code, ProductionItemResponse Item, DateTimeOffset CreatedAtUtc, IReadOnlyList<BomRevisionResponse> Revisions);

public sealed record BomListItemResponse(Guid Id, string Code, ProductionItemResponse Item, int? ApprovedRevisionNo, int LatestRevisionNo, string LatestStatus, DateTimeOffset CreatedAtUtc);

public sealed record BomListResponse(IReadOnlyList<BomListItemResponse> Items, ProductionPaginationResponse Pagination);

public sealed record WorkOrderRequest(Guid ItemId, Guid WarehouseId, Guid? ProjectId, decimal PlannedQuantity, string? Note);

public sealed record WorkOrderActionRequest(string? Reason);

public sealed record WorkOrderMaterialLineRequest(Guid ItemId, decimal Quantity);

public sealed record WorkOrderMaterialsRequest(List<WorkOrderMaterialLineRequest>? Lines);

public sealed record WorkOrderCompleteRequest(decimal Quantity);

public sealed record WorkOrderMaterialResponse(
    Guid Id,
    ProductionItemResponse Item,
    decimal RequiredQuantity,
    decimal IssuedQuantity,
    decimal ReturnedQuantity,
    decimal NetIssuedQuantity,
    decimal RemainingQuantity,
    decimal IssuedValue,
    decimal ReturnedValue);

public sealed record WorkOrderTransactionResponse(
    Guid Id,
    string Kind,
    Guid StockDocumentId,
    string StockDocumentNumber,
    decimal Quantity,
    decimal Value,
    ProductionPersonResponse Actor,
    DateTimeOffset CreatedAtUtc);

public sealed record WorkOrderRefResponse(Guid Id, string Code, string Name);

public sealed record WorkOrderResponse(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    ProductionItemResponse Item,
    Guid BomRevisionId,
    string BomCode,
    int BomRevisionNo,
    WorkOrderRefResponse Warehouse,
    WorkOrderRefResponse? Project,
    decimal PlannedQuantity,
    decimal CompletedQuantity,
    decimal CostAllocated,
    string? Note,
    string? CancelReason,
    ProductionPersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<WorkOrderMaterialResponse> Materials,
    IReadOnlyList<WorkOrderTransactionResponse> Transactions);

public sealed record WorkOrderListItemResponse(
    Guid Id,
    string Number,
    string Status,
    string ItemCode,
    string ItemNameTh,
    decimal PlannedQuantity,
    decimal CompletedQuantity,
    string? ProjectCode,
    DateTimeOffset CreatedAtUtc);

public sealed record WorkOrderListResponse(IReadOnlyList<WorkOrderListItemResponse> Items, ProductionPaginationResponse Pagination);
