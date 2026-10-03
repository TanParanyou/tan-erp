namespace TanErp.Application.Inventory;

public sealed record WarehouseInput(string? Name, string? Address);

public sealed record WarehouseProjection(
    Guid Id,
    Guid BranchId,
    string Code,
    string Name,
    string? Address,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc);

public sealed record WarehouseListQuery(string? Search, string? Status, int Page, int PageSize);

public sealed record PagedWarehouses(IReadOnlyList<WarehouseProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record InventoryWarehouseRef(Guid Id, string Code, string Name);

public sealed record InventoryItemRef(Guid Id, string Code, string NameTh, string UnitCode);

public sealed record InventoryPerson(Guid Id, string DisplayName, string? Email);

public sealed record StockBalanceProjection(
    Guid Id,
    InventoryWarehouseRef Warehouse,
    InventoryItemRef Item,
    decimal OnHand,
    decimal Reserved,
    decimal Available,
    decimal AverageCost,
    decimal TotalValue,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockBalanceQuery(Guid? WarehouseId, Guid? ItemId, string? Search, bool InStockOnly, int Page, int PageSize);

public sealed record PagedStockBalances(IReadOnlyList<StockBalanceProjection> Items, int TotalCount, int Page, int PageSize, decimal TotalValue);

public sealed record StockMovementProjection(
    Guid Id,
    Guid StockDocumentId,
    string DocumentType,
    string DocumentNumber,
    InventoryWarehouseRef Warehouse,
    InventoryItemRef Item,
    string Kind,
    decimal QuantityDelta,
    decimal UnitCost,
    decimal ValueDelta,
    decimal OnHandAfter,
    DateTimeOffset OccurredAtUtc,
    DateTimeOffset PostedAtUtc);

public sealed record StockMovementQuery(Guid? WarehouseId, Guid? ItemId, string? Kind, Guid? DocumentId, int Page, int PageSize);

public sealed record PagedStockMovements(IReadOnlyList<StockMovementProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record StockDocumentProjection(
    Guid Id,
    string DocumentType,
    string Number,
    InventoryWarehouseRef Warehouse,
    InventoryWarehouseRef? ToWarehouse,
    Guid? ProjectId,
    string? SourceType,
    Guid? SourceId,
    string? Reason,
    DateTimeOffset OccurredAtUtc,
    InventoryPerson PostedBy,
    DateTimeOffset PostedAtUtc,
    IReadOnlyList<StockMovementProjection> Movements);

public sealed record StockLineInput(Guid ItemId, decimal Quantity);

public sealed record AdjustmentLineInput(Guid ItemId, decimal CountedQuantity, decimal? UnitCost);

public sealed record ReservationProjection(
    Guid Id,
    InventoryWarehouseRef Warehouse,
    InventoryItemRef Item,
    Guid ProjectId,
    string ProjectCode,
    decimal Quantity,
    string Status,
    string? Note,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc);

public sealed record ReservationQuery(Guid? ProjectId, Guid? WarehouseId, string? Status, int Page, int PageSize);

public sealed record PagedReservations(IReadOnlyList<ReservationProjection> Items, int TotalCount, int Page, int PageSize);

public sealed record ReconciliationRow(
    InventoryWarehouseRef Warehouse,
    InventoryItemRef Item,
    decimal BalanceOnHand,
    decimal LedgerQuantity,
    decimal BalanceValue,
    decimal LedgerValue,
    bool IsConsistent);

public sealed record ReconciliationProjection(int RowCount, int InconsistentCount, IReadOnlyList<ReconciliationRow> Rows);
