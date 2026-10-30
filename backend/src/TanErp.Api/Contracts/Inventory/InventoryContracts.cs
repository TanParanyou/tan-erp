namespace TanErp.Api.Contracts.Inventory;

public sealed record WarehouseRequest(string? Name, string? Address);

public sealed record WarehouseResponse(Guid Id, Guid BranchId, string Code, string Name, string? Address, string Status, Guid RowVersion, DateTimeOffset CreatedAtUtc);

public sealed record InventoryPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record WarehouseListResponse(IReadOnlyList<WarehouseResponse> Items, InventoryPaginationResponse Pagination);

public sealed record CreateWarehouseLocationRequest(string? Code, string? Name);

public sealed record UpdateWarehouseLocationRequest(string? Name, bool? Active);

public sealed record WarehouseLocationResponse(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    bool IsDefault,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record WarehouseLocationListResponse(IReadOnlyList<WarehouseLocationResponse> Items);

public sealed record InventoryWarehouseRefResponse(Guid Id, string Code, string Name);

public sealed record InventoryLocationRefResponse(Guid Id, string Code, string Name, bool IsDefault);

public sealed record InventoryItemRefResponse(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record InventoryPersonResponse(Guid Id, string DisplayName, string? Email);

public sealed record StockBalanceResponse(
    Guid Id,
    InventoryWarehouseRefResponse Warehouse,
    InventoryItemRefResponse Item,
    decimal OnHand,
    decimal Reserved,
    decimal Available,
    decimal AverageCost,
    decimal TotalValue,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockBalanceListResponse(IReadOnlyList<StockBalanceResponse> Items, InventoryPaginationResponse Pagination, decimal TotalValue);

public sealed record StockLocationBalanceResponse(
    Guid Id,
    InventoryWarehouseRefResponse Warehouse,
    InventoryLocationRefResponse Location,
    InventoryItemRefResponse Item,
    string LotNumber,
    decimal OnHand,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockLocationBalanceListResponse(
    IReadOnlyList<StockLocationBalanceResponse> Items,
    InventoryPaginationResponse Pagination);

public sealed record StockSerialResponse(
    Guid Id,
    InventoryItemRefResponse Item,
    string SerialNumber,
    InventoryWarehouseRefResponse? Warehouse,
    InventoryLocationRefResponse? Location,
    string LotNumber,
    string Status,
    Guid LastStockDocumentId,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockSerialListResponse(
    IReadOnlyList<StockSerialResponse> Items,
    InventoryPaginationResponse Pagination);

public sealed record StockMovementResponse(
    Guid Id,
    Guid StockDocumentId,
    string DocumentType,
    string DocumentNumber,
    InventoryWarehouseRefResponse Warehouse,
    InventoryItemRefResponse Item,
    string Kind,
    decimal QuantityDelta,
    decimal UnitCost,
    decimal ValueDelta,
    decimal OnHandAfter,
    DateTimeOffset OccurredAtUtc,
    DateTimeOffset PostedAtUtc,
    InventoryLocationRefResponse? Location = null,
    string LotNumber = "",
    IReadOnlyList<string>? SerialNumbers = null);

public sealed record StockMovementListResponse(IReadOnlyList<StockMovementResponse> Items, InventoryPaginationResponse Pagination);

public sealed record StockDocumentResponse(
    Guid Id,
    string DocumentType,
    string Number,
    InventoryWarehouseRefResponse Warehouse,
    InventoryWarehouseRefResponse? ToWarehouse,
    Guid? ProjectId,
    string? SourceType,
    Guid? SourceId,
    string? Reason,
    DateTimeOffset OccurredAtUtc,
    InventoryPersonResponse PostedBy,
    DateTimeOffset PostedAtUtc,
    IReadOnlyList<StockMovementResponse> Movements);

public sealed record ReceiveGoodsReceiptLineRequest(
    Guid ItemId,
    decimal? Quantity = null,
    Guid? LocationId = null,
    string? LotNumber = null,
    List<string>? SerialNumbers = null);

public sealed record ReceiveGoodsReceiptRequest(
    Guid GoodsReceiptId,
    Guid WarehouseId,
    Guid? LocationId = null,
    List<ReceiveGoodsReceiptLineRequest>? Lines = null);

public sealed record StockLineRequest(
    Guid ItemId,
    decimal Quantity,
    Guid? LocationId = null,
    Guid? ToLocationId = null,
    string? LotNumber = null,
    List<string>? SerialNumbers = null);

public sealed record IssueStockRequest(
    Guid WarehouseId,
    Guid? ProjectId,
    string? Reason,
    List<StockLineRequest>? Lines,
    Guid? LocationId = null,
    DateTimeOffset? OccurredAtUtc = null);

public sealed record TransferStockRequest(
    Guid FromWarehouseId,
    Guid ToWarehouseId,
    string? Reason,
    List<StockLineRequest>? Lines,
    Guid? FromLocationId = null,
    Guid? ToLocationId = null,
    DateTimeOffset? OccurredAtUtc = null);

public sealed record AdjustmentLineRequest(
    Guid ItemId,
    decimal CountedQuantity,
    decimal? UnitCost,
    Guid? LocationId = null,
    string? LotNumber = null,
    List<string>? SerialNumbers = null);

public sealed record AdjustStockRequest(
    Guid WarehouseId,
    string? Reason,
    List<AdjustmentLineRequest>? Lines,
    Guid? LocationId = null,
    DateTimeOffset? OccurredAtUtc = null);

public sealed record ReturnStockLineRequest(
    Guid ItemId,
    decimal Quantity,
    decimal? UnitCost = null,
    Guid? LocationId = null,
    string? LotNumber = null,
    List<string>? SerialNumbers = null);

public sealed record ReturnStockRequest(
    Guid WarehouseId,
    Guid? ProjectId,
    string? Reason,
    List<ReturnStockLineRequest>? Lines,
    Guid? LocationId = null,
    Guid? SourceIssueDocumentId = null,
    DateTimeOffset? OccurredAtUtc = null);

public sealed record ReserveStockRequest(Guid WarehouseId, Guid ItemId, Guid ProjectId, decimal Quantity, string? Note);

public sealed record ReleaseReservationRequest(Guid ExpectedVersion);

public sealed record ReservationResponse(
    Guid Id,
    InventoryWarehouseRefResponse Warehouse,
    InventoryItemRefResponse Item,
    Guid ProjectId,
    string ProjectCode,
    decimal Quantity,
    string Status,
    string? Note,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc);

public sealed record ReservationListResponse(IReadOnlyList<ReservationResponse> Items, InventoryPaginationResponse Pagination);

public sealed record ReconciliationRowResponse(
    InventoryWarehouseRefResponse Warehouse,
    InventoryItemRefResponse Item,
    decimal BalanceOnHand,
    decimal LedgerQuantity,
    decimal BalanceValue,
    decimal LedgerValue,
    bool IsConsistent);

public sealed record ReconciliationResponse(int RowCount, int InconsistentCount, IReadOnlyList<ReconciliationRowResponse> Rows);

public sealed record CloseInventoryPeriodRequest(int PeriodYear, int PeriodMonth, string? Note = null);

public sealed record InventoryPeriodCloseLineResponse(
    Guid Id,
    InventoryWarehouseRefResponse Warehouse,
    InventoryItemRefResponse Item,
    decimal OnHandQuantity,
    decimal AverageCost,
    decimal TotalValue);

public sealed record InventoryPeriodCloseResponse(
    Guid Id,
    int PeriodYear,
    int PeriodMonth,
    DateOnly PeriodEndDate,
    string? Note,
    int LineCount,
    decimal TotalOnHandQuantity,
    decimal TotalValuationAmount,
    InventoryPersonResponse ClosedBy,
    DateTimeOffset ClosedAtUtc,
    IReadOnlyList<InventoryPeriodCloseLineResponse> Lines);

public sealed record InventoryPeriodCloseListResponse(IReadOnlyList<InventoryPeriodCloseResponse> Items);
