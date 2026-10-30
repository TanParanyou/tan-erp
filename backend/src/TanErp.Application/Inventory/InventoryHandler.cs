using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Inventory;

namespace TanErp.Application.Inventory;

public sealed record InventoryCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>
/// Inventory use cases. Each call resolves the caller's permission from PostgreSQL and validates the request shape;
/// the store owns the transaction, the row locks and the ledger invariants.
/// </summary>
public class InventoryHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    public const int MaxLines = 200;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IInventoryStore _store;

    public InventoryHandler(IRequestAccessResolver accessResolver, IInventoryStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(InventoryCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static (int Page, int PageSize) Paging(int page, int pageSize) =>
        (Math.Max(1, page), pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize));

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string CleanLot(string? lot) => string.IsNullOrWhiteSpace(lot) ? string.Empty : lot.Trim().ToUpperInvariant();

    private static string Hash(string key) => Sha256Hex.Compute(key);

    private static string SerialsPayload(IReadOnlyList<string>? serials) =>
        serials is null || serials.Count == 0
            ? string.Empty
            : string.Join(",", serials.Select(s => s?.Trim().ToUpperInvariant() ?? string.Empty));

    private static string Lines(IEnumerable<StockLineInput> lines) =>
        string.Join(";", lines
            .OrderBy(l => l.ItemId)
            .ThenBy(l => l.LocationId)
            .ThenBy(l => l.ToLocationId)
            .ThenBy(l => CleanLot(l.LotNumber))
            .Select(l => $"{l.ItemId}:{l.Quantity.ToString(CultureInfo.InvariantCulture)}:{l.LocationId}:{l.ToLocationId}:{CleanLot(l.LotNumber)}:{SerialsPayload(l.SerialNumbers)}"));

    private static Result<IReadOnlyList<StockLineInput>> ValidateLines(IReadOnlyList<StockLineInput>? lines)
    {
        if (lines is null || lines.Count == 0 || lines.Count > MaxLines)
        {
            return Fail<IReadOnlyList<StockLineInput>>("INVENTORY_LINE_INVALID", $"A document needs between 1 and {MaxLines} lines.");
        }

        if (lines.Any(l => l.ItemId == Guid.Empty || l.Quantity <= 0)
            || lines.GroupBy(l => (l.ItemId, l.LocationId, l.ToLocationId, CleanLot(l.LotNumber))).Any(g => g.Count() > 1))
        {
            return Fail<IReadOnlyList<StockLineInput>>("INVENTORY_LINE_INVALID", "Each item/location/lot combination can appear once with a quantity above zero.");
        }

        return Result<IReadOnlyList<StockLineInput>>.Success(lines);
    }

    // ----- warehouses ------------------------------------------------------------------------------

    public async Task<Result<WarehouseProjection>> CreateWarehouseAsync(InventoryCaller caller, string idempotencyKey, WarehouseInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.manage", ct);
        if (access.IsFailure) return Result<WarehouseProjection>.Failure(access.Error);
        return await _store.CreateWarehouseAsync(access.Value!, input, Hash(idempotencyKey), Sha256Hex.Compute($"{input.Name?.Trim()}|{input.Address?.Trim()}"), caller.TraceId, ct);
    }

    public async Task<Result<WarehouseProjection>> UpdateWarehouseAsync(InventoryCaller caller, Guid id, Guid expectedVersion, WarehouseInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.manage", ct);
        if (access.IsFailure) return Result<WarehouseProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<WarehouseProjection>("INVENTORY_FIELD_REQUIRED", "Expected version is required.");
        return await _store.UpdateWarehouseAsync(access.Value!, id, expectedVersion, input, caller.TraceId, ct);
    }

    public async Task<Result<WarehouseProjection>> SetWarehouseActiveAsync(InventoryCaller caller, Guid id, Guid expectedVersion, bool active, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.manage", ct);
        if (access.IsFailure) return Result<WarehouseProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<WarehouseProjection>("INVENTORY_FIELD_REQUIRED", "Expected version is required.");
        return await _store.SetWarehouseActiveAsync(access.Value!, id, expectedVersion, active, caller.TraceId, ct);
    }

    public async Task<Result<WarehouseProjection>> GetWarehouseAsync(InventoryCaller caller, Guid id, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.read", ct);
        if (access.IsFailure) return Result<WarehouseProjection>.Failure(access.Error);
        var warehouse = await _store.GetWarehouseAsync(access.Value!.OrganizationId, id, ct);
        return warehouse is null ? Fail<WarehouseProjection>("RESOURCE_NOT_FOUND", "Warehouse not found.") : Result<WarehouseProjection>.Success(warehouse);
    }

    public async Task<Result<PagedWarehouses>> ListWarehousesAsync(InventoryCaller caller, string? search, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.read", ct);
        if (access.IsFailure) return Result<PagedWarehouses>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && status.Trim() is not ("active" or "inactive"))
        {
            return Fail<PagedWarehouses>("INVENTORY_FIELD_INVALID", "Warehouse status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedWarehouses>.Success(await _store.ListWarehousesAsync(access.Value!.OrganizationId, new WarehouseListQuery(Clean(search), Clean(status), p, size), ct));
    }

    // ----- warehouse locations (bins) --------------------------------------------------------------

    public async Task<Result<IReadOnlyList<WarehouseLocationProjection>>> ListWarehouseLocationsAsync(InventoryCaller caller, Guid warehouseId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.read", ct);
        if (access.IsFailure) return Result<IReadOnlyList<WarehouseLocationProjection>>.Failure(access.Error);
        return await _store.ListWarehouseLocationsAsync(access.Value!.OrganizationId, warehouseId, ct);
    }

    public async Task<Result<WarehouseLocationProjection>> CreateWarehouseLocationAsync(
        InventoryCaller caller, string idempotencyKey, Guid warehouseId, WarehouseLocationCreateInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.manage", ct);
        if (access.IsFailure) return Result<WarehouseLocationProjection>.Failure(access.Error);
        if (warehouseId == Guid.Empty) return Fail<WarehouseLocationProjection>("INVENTORY_FIELD_REQUIRED", "Warehouse is required.");
        var payload = Sha256Hex.Compute($"{warehouseId}|{input.Code?.Trim().ToUpperInvariant()}|{input.Name?.Trim()}");
        return await _store.CreateWarehouseLocationAsync(access.Value!, warehouseId, input, Hash(idempotencyKey), payload, caller.TraceId, ct);
    }

    public async Task<Result<WarehouseLocationProjection>> UpdateWarehouseLocationAsync(
        InventoryCaller caller, Guid warehouseId, Guid locationId, Guid expectedVersion, WarehouseLocationUpdateInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.manage", ct);
        if (access.IsFailure) return Result<WarehouseLocationProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<WarehouseLocationProjection>("INVENTORY_FIELD_REQUIRED", "Expected version is required.");
        return await _store.UpdateWarehouseLocationAsync(access.Value!, warehouseId, locationId, expectedVersion, input, caller.TraceId, ct);
    }

    // ----- postings --------------------------------------------------------------------------------

    public async Task<Result<StockDocumentProjection>> ReceiveGoodsReceiptAsync(
        InventoryCaller caller,
        string idempotencyKey,
        Guid goodsReceiptId,
        Guid warehouseId,
        Guid? locationId = null,
        IReadOnlyList<ReceiveGoodsReceiptLineInput>? lines = null,
        CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.receive", ct);
        if (access.IsFailure) return Result<StockDocumentProjection>.Failure(access.Error);
        if (goodsReceiptId == Guid.Empty || warehouseId == Guid.Empty) return Fail<StockDocumentProjection>("INVENTORY_FIELD_REQUIRED", "A goods receipt and a warehouse are required.");

        var linesPayload = lines is null || lines.Count == 0
            ? string.Empty
            : string.Join(";", lines.Select(l => $"{l.ItemId}:{l.Quantity?.ToString(CultureInfo.InvariantCulture)}:{l.LocationId}:{CleanLot(l.LotNumber)}:{SerialsPayload(l.SerialNumbers)}"));
        var payload = Sha256Hex.Compute($"{goodsReceiptId}|{warehouseId}|{locationId}|{linesPayload}");
        return await _store.ReceiveGoodsReceiptAsync(access.Value!, goodsReceiptId, warehouseId, locationId, lines, Hash(idempotencyKey), payload, caller.TraceId, ct);
    }

    public async Task<Result<StockDocumentProjection>> IssueAsync(
        InventoryCaller caller,
        string idempotencyKey,
        Guid warehouseId,
        Guid? projectId,
        string? reason,
        IReadOnlyList<StockLineInput>? lines,
        Guid? locationId = null,
        DateTimeOffset? occurredAtUtc = null,
        CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.issue", ct);
        if (access.IsFailure) return Result<StockDocumentProjection>.Failure(access.Error);
        if (warehouseId == Guid.Empty) return Fail<StockDocumentProjection>("INVENTORY_FIELD_REQUIRED", "A warehouse is required.");
        var valid = ValidateLines(lines);
        if (valid.IsFailure) return Result<StockDocumentProjection>.Failure(valid.Error);
        var payload = Sha256Hex.Compute($"{warehouseId}|{locationId}|{projectId}|{occurredAtUtc?.UtcDateTime:O}|{Clean(reason)}|{Lines(valid.Value!)}");
        return await _store.IssueAsync(access.Value!, warehouseId, locationId, projectId, occurredAtUtc, Clean(reason), valid.Value!, Hash(idempotencyKey), payload, caller.TraceId, ct);
    }

    public async Task<Result<StockDocumentProjection>> TransferAsync(
        InventoryCaller caller,
        string idempotencyKey,
        Guid fromWarehouseId,
        Guid toWarehouseId,
        string? reason,
        IReadOnlyList<StockLineInput>? lines,
        Guid? fromLocationId = null,
        Guid? toLocationId = null,
        DateTimeOffset? occurredAtUtc = null,
        CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.transfer", ct);
        if (access.IsFailure) return Result<StockDocumentProjection>.Failure(access.Error);
        if (fromWarehouseId == Guid.Empty || toWarehouseId == Guid.Empty) return Fail<StockDocumentProjection>("INVENTORY_FIELD_REQUIRED", "Source and destination warehouses are required.");

        var valid = ValidateLines(lines);
        if (valid.IsFailure) return Result<StockDocumentProjection>.Failure(valid.Error);

        if (fromWarehouseId == toWarehouseId)
        {
            var hasDistinctLocations = valid.Value!.All(l =>
            {
                var srcLoc = l.LocationId ?? fromLocationId;
                var dstLoc = l.ToLocationId ?? toLocationId;
                return srcLoc.HasValue && dstLoc.HasValue && srcLoc.Value != dstLoc.Value;
            });
            if (!hasDistinctLocations)
            {
                return Fail<StockDocumentProjection>("INVENTORY_TRANSFER_SAME_WAREHOUSE", "Source and destination must differ.");
            }
        }

        var payload = Sha256Hex.Compute($"{fromWarehouseId}|{toWarehouseId}|{fromLocationId}|{toLocationId}|{occurredAtUtc?.UtcDateTime:O}|{Clean(reason)}|{Lines(valid.Value!)}");
        return await _store.TransferAsync(access.Value!, fromWarehouseId, toWarehouseId, fromLocationId, toLocationId, occurredAtUtc, Clean(reason), valid.Value!, Hash(idempotencyKey), payload, caller.TraceId, ct);
    }

    public async Task<Result<StockDocumentProjection>> AdjustAsync(
        InventoryCaller caller,
        string idempotencyKey,
        Guid warehouseId,
        string? reason,
        IReadOnlyList<AdjustmentLineInput>? lines,
        Guid? locationId = null,
        DateTimeOffset? occurredAtUtc = null,
        CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.adjust", ct);
        if (access.IsFailure) return Result<StockDocumentProjection>.Failure(access.Error);
        if (warehouseId == Guid.Empty) return Fail<StockDocumentProjection>("INVENTORY_FIELD_REQUIRED", "A warehouse is required.");
        if (string.IsNullOrWhiteSpace(reason)) return Fail<StockDocumentProjection>("INVENTORY_REASON_REQUIRED", "A reason is required for a stock adjustment.");
        if (lines is null || lines.Count == 0 || lines.Count > MaxLines
            || lines.Any(l => l.ItemId == Guid.Empty || l.CountedQuantity < 0 || (l.UnitCost.HasValue && l.UnitCost.Value < 0))
            || lines.GroupBy(l => (l.ItemId, l.LocationId, CleanLot(l.LotNumber))).Any(g => g.Count() > 1))
        {
            return Fail<StockDocumentProjection>("INVENTORY_LINE_INVALID", "Each item/location/lot combination can appear once with a non-negative counted quantity.");
        }

        var payload = string.Join(";", lines
            .OrderBy(l => l.ItemId)
            .ThenBy(l => l.LocationId)
            .ThenBy(l => CleanLot(l.LotNumber))
            .Select(l => $"{l.ItemId}:{l.CountedQuantity.ToString(CultureInfo.InvariantCulture)}:{l.UnitCost?.ToString(CultureInfo.InvariantCulture)}:{l.LocationId}:{CleanLot(l.LotNumber)}:{SerialsPayload(l.SerialNumbers)}"));
        return await _store.AdjustAsync(access.Value!, warehouseId, locationId, occurredAtUtc, reason.Trim(), lines, Hash(idempotencyKey), Sha256Hex.Compute($"{warehouseId}|{locationId}|{occurredAtUtc?.UtcDateTime:O}|{reason.Trim()}|{payload}"), caller.TraceId, ct);
    }

    public async Task<Result<StockDocumentProjection>> ReturnAsync(
        InventoryCaller caller,
        string idempotencyKey,
        Guid warehouseId,
        Guid? locationId,
        Guid? projectId,
        Guid? sourceIssueDocumentId,
        DateTimeOffset? occurredAtUtc,
        string? reason,
        IReadOnlyList<ReturnLineInput>? lines,
        CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.receive", ct);
        if (access.IsFailure) return Result<StockDocumentProjection>.Failure(access.Error);
        if (warehouseId == Guid.Empty) return Fail<StockDocumentProjection>("INVENTORY_FIELD_REQUIRED", "A warehouse is required.");
        if (string.IsNullOrWhiteSpace(reason)) return Fail<StockDocumentProjection>("INVENTORY_REASON_REQUIRED", "A reason is required for a stock return.");
        if (lines is null || lines.Count == 0 || lines.Count > MaxLines
            || lines.Any(l => l.ItemId == Guid.Empty || l.Quantity <= 0 || (l.UnitCost.HasValue && l.UnitCost.Value < 0))
            || lines.GroupBy(l => (l.ItemId, l.LocationId, CleanLot(l.LotNumber))).Any(g => g.Count() > 1))
        {
            return Fail<StockDocumentProjection>("INVENTORY_LINE_INVALID", "Each item/location/lot combination can appear once with a quantity above zero.");
        }

        var linesPayload = string.Join(";", lines
            .OrderBy(l => l.ItemId)
            .ThenBy(l => l.LocationId)
            .ThenBy(l => CleanLot(l.LotNumber))
            .Select(l => $"{l.ItemId}:{l.Quantity.ToString(CultureInfo.InvariantCulture)}:{l.UnitCost?.ToString(CultureInfo.InvariantCulture)}:{l.LocationId}:{CleanLot(l.LotNumber)}:{SerialsPayload(l.SerialNumbers)}"));
        var payload = Sha256Hex.Compute($"{warehouseId}|{locationId}|{projectId}|{sourceIssueDocumentId}|{occurredAtUtc?.UtcDateTime:O}|{reason.Trim()}|{linesPayload}");
        return await _store.ReturnAsync(access.Value!, warehouseId, locationId, projectId, sourceIssueDocumentId, occurredAtUtc, reason.Trim(), lines, Hash(idempotencyKey), payload, caller.TraceId, ct);
    }

    public async Task<Result<ReservationProjection>> ReserveAsync(InventoryCaller caller, string idempotencyKey, Guid warehouseId, Guid itemId, Guid projectId, decimal quantity, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.reserve", ct);
        if (access.IsFailure) return Result<ReservationProjection>.Failure(access.Error);
        if (warehouseId == Guid.Empty || itemId == Guid.Empty || projectId == Guid.Empty || quantity <= 0)
        {
            return Fail<ReservationProjection>("INVENTORY_FIELD_REQUIRED", "A warehouse, item, project and quantity above zero are required.");
        }

        return await _store.ReserveAsync(access.Value!, warehouseId, itemId, projectId, quantity, Clean(note), Hash(idempotencyKey),
            Sha256Hex.Compute($"{warehouseId}|{itemId}|{projectId}|{quantity.ToString(CultureInfo.InvariantCulture)}|{Clean(note)}"), caller.TraceId, ct);
    }

    public async Task<Result<ReservationProjection>> ReleaseReservationAsync(InventoryCaller caller, Guid reservationId, Guid expectedVersion, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.reserve", ct);
        if (access.IsFailure) return Result<ReservationProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<ReservationProjection>("INVENTORY_FIELD_REQUIRED", "Expected version is required.");
        return await _store.ReleaseReservationAsync(access.Value!, reservationId, expectedVersion, caller.TraceId, ct);
    }

    // ----- period closes ---------------------------------------------------------------------------

    public async Task<Result<InventoryPeriodCloseProjection>> ClosePeriodAsync(
        InventoryCaller caller, string idempotencyKey, int periodYear, int periodMonth, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "warehouses.manage", ct);
        if (access.IsFailure) return Result<InventoryPeriodCloseProjection>.Failure(access.Error);
        var payload = Sha256Hex.Compute($"{periodYear}|{periodMonth}|{Clean(note)}");
        return await _store.ClosePeriodAsync(access.Value!, periodYear, periodMonth, Clean(note), Hash(idempotencyKey), payload, caller.TraceId, ct);
    }

    public async Task<Result<IReadOnlyList<InventoryPeriodCloseProjection>>> ListPeriodClosesAsync(InventoryCaller caller, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<IReadOnlyList<InventoryPeriodCloseProjection>>.Failure(access.Error);
        return Result<IReadOnlyList<InventoryPeriodCloseProjection>>.Success(await _store.ListPeriodClosesAsync(access.Value!.OrganizationId, ct));
    }

    public async Task<Result<InventoryPeriodCloseProjection>> GetPeriodCloseAsync(InventoryCaller caller, Guid id, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<InventoryPeriodCloseProjection>.Failure(access.Error);
        var close = await _store.GetPeriodCloseAsync(access.Value!.OrganizationId, id, ct);
        return close is null ? Fail<InventoryPeriodCloseProjection>("RESOURCE_NOT_FOUND", "Inventory period close not found.") : Result<InventoryPeriodCloseProjection>.Success(close);
    }

    // ----- reads -----------------------------------------------------------------------------------

    public async Task<Result<StockDocumentProjection>> GetDocumentAsync(InventoryCaller caller, Guid documentId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<StockDocumentProjection>.Failure(access.Error);
        var document = await _store.GetDocumentAsync(access.Value!.OrganizationId, documentId, ct);
        return document is null ? Fail<StockDocumentProjection>("RESOURCE_NOT_FOUND", "Stock document not found.") : Result<StockDocumentProjection>.Success(document);
    }

    public async Task<Result<PagedStockBalances>> ListBalancesAsync(InventoryCaller caller, Guid? warehouseId, Guid? itemId, string? search, bool inStockOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<PagedStockBalances>.Failure(access.Error);
        var (p, size) = Paging(page, pageSize);
        return Result<PagedStockBalances>.Success(await _store.ListBalancesAsync(access.Value!.OrganizationId, new StockBalanceQuery(warehouseId, itemId, Clean(search), inStockOnly, p, size), ct));
    }

    public async Task<Result<PagedStockLocationBalances>> ListLocationBalancesAsync(
        InventoryCaller caller, Guid? warehouseId, Guid? locationId, Guid? itemId, string? lotNumber, bool inStockOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<PagedStockLocationBalances>.Failure(access.Error);
        var (p, size) = Paging(page, pageSize);
        return Result<PagedStockLocationBalances>.Success(await _store.ListLocationBalancesAsync(
            access.Value!.OrganizationId, new StockLocationBalanceQuery(warehouseId, locationId, itemId, Clean(lotNumber)?.ToUpperInvariant(), inStockOnly, p, size), ct));
    }

    public async Task<Result<PagedStockSerials>> ListSerialsAsync(
        InventoryCaller caller, Guid? warehouseId, Guid? locationId, Guid? itemId, string? status, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<PagedStockSerials>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !StockSerialStatus.All.Contains(status.Trim()))
        {
            return Fail<PagedStockSerials>("INVENTORY_FIELD_INVALID", "Serial status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedStockSerials>.Success(await _store.ListSerialsAsync(
            access.Value!.OrganizationId, new StockSerialQuery(warehouseId, locationId, itemId, Clean(status), Clean(search), p, size), ct));
    }

    public async Task<Result<PagedStockMovements>> ListMovementsAsync(InventoryCaller caller, Guid? warehouseId, Guid? itemId, string? kind, Guid? documentId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<PagedStockMovements>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(kind) && !StockMovementKind.All.Contains(kind.Trim()))
        {
            return Fail<PagedStockMovements>("INVENTORY_FIELD_INVALID", "Movement kind filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedStockMovements>.Success(await _store.ListMovementsAsync(access.Value!.OrganizationId, new StockMovementQuery(warehouseId, itemId, Clean(kind), documentId, p, size), ct));
    }

    public async Task<Result<PagedReservations>> ListReservationsAsync(InventoryCaller caller, Guid? projectId, Guid? warehouseId, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<PagedReservations>.Failure(access.Error);
        var (p, size) = Paging(page, pageSize);
        return Result<PagedReservations>.Success(await _store.ListReservationsAsync(access.Value!.OrganizationId, new ReservationQuery(projectId, warehouseId, Clean(status), p, size), ct));
    }

    public async Task<Result<ReconciliationProjection>> ReconcileAsync(InventoryCaller caller, Guid? warehouseId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "inventory.read", ct);
        if (access.IsFailure) return Result<ReconciliationProjection>.Failure(access.Error);
        return Result<ReconciliationProjection>.Success(await _store.ReconcileAsync(access.Value!.OrganizationId, warehouseId, ct));
    }
}
