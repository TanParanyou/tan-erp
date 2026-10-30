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

public sealed record WarehouseLocationCreateInput(string? Code, string? Name);

public sealed record WarehouseLocationUpdateInput(string? Name, bool? Active);

public sealed record WarehouseLocationProjection(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    bool IsDefault,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record InventoryWarehouseRef(Guid Id, string Code, string Name);

public sealed record InventoryLocationRef(Guid Id, string Code, string Name, bool IsDefault);

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

public sealed record StockLocationBalanceProjection(
    Guid Id,
    InventoryWarehouseRef Warehouse,
    InventoryLocationRef Location,
    InventoryItemRef Item,
    string LotNumber,
    decimal OnHand,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockLocationBalanceQuery(
    Guid? WarehouseId,
    Guid? LocationId,
    Guid? ItemId,
    string? LotNumber,
    bool InStockOnly,
    int Page,
    int PageSize);

public sealed record PagedStockLocationBalances(
    IReadOnlyList<StockLocationBalanceProjection> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record StockSerialProjection(
    Guid Id,
    InventoryItemRef Item,
    string SerialNumber,
    InventoryWarehouseRef? Warehouse,
    InventoryLocationRef? Location,
    string LotNumber,
    string Status,
    Guid LastStockDocumentId,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockSerialQuery(
    Guid? WarehouseId,
    Guid? LocationId,
    Guid? ItemId,
    string? Status,
    string? Search,
    int Page,
    int PageSize);

public sealed record PagedStockSerials(
    IReadOnlyList<StockSerialProjection> Items,
    int TotalCount,
    int Page,
    int PageSize);

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
    DateTimeOffset PostedAtUtc,
    InventoryLocationRef? Location = null,
    string LotNumber = "",
    IReadOnlyList<string>? SerialNumbers = null);

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

public sealed record ReceiveGoodsReceiptLineInput(
    Guid ItemId,
    decimal? Quantity = null,
    Guid? LocationId = null,
    string? LotNumber = null,
    IReadOnlyList<string>? SerialNumbers = null);

public sealed record StockLineInput(
    Guid ItemId,
    decimal Quantity,
    Guid? LocationId = null,
    Guid? ToLocationId = null,
    string? LotNumber = null,
    IReadOnlyList<string>? SerialNumbers = null);

public sealed record AdjustmentLineInput(
    Guid ItemId,
    decimal CountedQuantity,
    decimal? UnitCost,
    Guid? LocationId = null,
    string? LotNumber = null,
    IReadOnlyList<string>? SerialNumbers = null);

public sealed record ReturnLineInput(
    Guid ItemId,
    decimal Quantity,
    decimal? UnitCost = null,
    Guid? LocationId = null,
    string? LotNumber = null,
    IReadOnlyList<string>? SerialNumbers = null);

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

public sealed record InventoryPeriodCloseLineProjection(
    Guid Id,
    InventoryWarehouseRef Warehouse,
    InventoryItemRef Item,
    decimal OnHandQuantity,
    decimal AverageCost,
    decimal TotalValue);

public sealed record InventoryPeriodCloseProjection(
    Guid Id,
    int PeriodYear,
    int PeriodMonth,
    DateOnly PeriodEndDate,
    string? Note,
    int LineCount,
    decimal TotalOnHandQuantity,
    decimal TotalValuationAmount,
    InventoryPerson ClosedBy,
    DateTimeOffset ClosedAtUtc,
    IReadOnlyList<InventoryPeriodCloseLineProjection> Lines);
