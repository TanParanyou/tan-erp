using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Inventory;

public interface IInventoryStore
{
    Task<Result<WarehouseProjection>> CreateWarehouseAsync(RequestAccessContext access, WarehouseInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<WarehouseProjection>> UpdateWarehouseAsync(RequestAccessContext access, Guid warehouseId, Guid expectedVersion, WarehouseInput input, string traceId, CancellationToken ct = default);
    Task<Result<WarehouseProjection>> SetWarehouseActiveAsync(RequestAccessContext access, Guid warehouseId, Guid expectedVersion, bool active, string traceId, CancellationToken ct = default);
    Task<WarehouseProjection?> GetWarehouseAsync(Guid organizationId, Guid warehouseId, CancellationToken ct = default);
    Task<PagedWarehouses> ListWarehousesAsync(Guid organizationId, WarehouseListQuery query, CancellationToken ct = default);

    Task<Result<StockDocumentProjection>> ReceiveGoodsReceiptAsync(RequestAccessContext access, Guid goodsReceiptId, Guid warehouseId, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<StockDocumentProjection>> IssueAsync(RequestAccessContext access, Guid warehouseId, Guid? projectId, string? reason, IReadOnlyList<StockLineInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<StockDocumentProjection>> TransferAsync(RequestAccessContext access, Guid fromWarehouseId, Guid toWarehouseId, string? reason, IReadOnlyList<StockLineInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<StockDocumentProjection>> AdjustAsync(RequestAccessContext access, Guid warehouseId, string reason, IReadOnlyList<AdjustmentLineInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);

    Task<Result<ReservationProjection>> ReserveAsync(RequestAccessContext access, Guid warehouseId, Guid itemId, Guid projectId, decimal quantity, string? note, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<ReservationProjection>> ReleaseReservationAsync(RequestAccessContext access, Guid reservationId, Guid expectedVersion, string traceId, CancellationToken ct = default);

    Task<StockDocumentProjection?> GetDocumentAsync(Guid organizationId, Guid documentId, CancellationToken ct = default);
    Task<PagedStockBalances> ListBalancesAsync(Guid organizationId, StockBalanceQuery query, CancellationToken ct = default);
    Task<PagedStockMovements> ListMovementsAsync(Guid organizationId, StockMovementQuery query, CancellationToken ct = default);
    Task<PagedReservations> ListReservationsAsync(Guid organizationId, ReservationQuery query, CancellationToken ct = default);
    Task<ReconciliationProjection> ReconcileAsync(Guid organizationId, Guid? warehouseId, CancellationToken ct = default);
}
