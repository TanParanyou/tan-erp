namespace TanErp.Api.Contracts.Inventory;

public sealed record WarehouseRequest(string? Name, string? Address);

public sealed record WarehouseResponse(Guid Id, Guid BranchId, string Code, string Name, string? Address, string Status, Guid RowVersion, DateTimeOffset CreatedAtUtc);

public sealed record InventoryPaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record WarehouseListResponse(IReadOnlyList<WarehouseResponse> Items, InventoryPaginationResponse Pagination);

public sealed record InventoryWarehouseRefResponse(Guid Id, string Code, string Name);

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
    DateTimeOffset PostedAtUtc);

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

public sealed record ReceiveGoodsReceiptRequest(Guid GoodsReceiptId, Guid WarehouseId);

public sealed record StockLineRequest(Guid ItemId, decimal Quantity);

public sealed record IssueStockRequest(Guid WarehouseId, Guid? ProjectId, string? Reason, List<StockLineRequest>? Lines);

public sealed record TransferStockRequest(Guid FromWarehouseId, Guid ToWarehouseId, string? Reason, List<StockLineRequest>? Lines);

public sealed record AdjustmentLineRequest(Guid ItemId, decimal CountedQuantity, decimal? UnitCost);

public sealed record AdjustStockRequest(Guid WarehouseId, string? Reason, List<AdjustmentLineRequest>? Lines);

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
