using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Inventory;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Persistence.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Inventory;

public class InventoryStore : IInventoryStore, IProductionStockPort
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;

    public InventoryStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> Fail<T>(InventoryDomainException ex) => Fail<T>(ex.Code, ex.Message);

    private void Audit(RequestAccessContext access, string action, string resourceType, Guid resourceId, string traceId, object changes, Guid? rowVersionAfter, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, resourceType, resourceId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId,
            rowVersionAfter: rowVersionAfter));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
            r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    // ===== warehouses ================================================================================

    private static WarehouseProjection ToProjection(Warehouse w) => new(w.Id, w.BranchId, w.Code, w.Name, w.Address, w.Status, w.RowVersion, w.CreatedAtUtc);

    public async Task<Result<WarehouseProjection>> CreateWarehouseAsync(
        RequestAccessContext access, WarehouseInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.warehouse.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<WarehouseProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WarehouseProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetWarehouseAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<WarehouseProjection>.Success(replayed);
            }

            var now = _clock.UtcNow;
            var code = await MasterDataCodeAllocator.ResolveAsync(
                _numbers, orgId, DocumentTypes.Warehouses, null, 64, "WAREHOUSE_CODE_CONFLICT",
                async (candidate, token) => await _db.Warehouses.AnyAsync(w => w.OrganizationId == orgId && w.NormalizedCode == candidate.Trim().ToUpperInvariant(), token), ct);
            if (code.IsFailure) return Result<WarehouseProjection>.Failure(code.Error);

            Warehouse warehouse;
            try
            {
                warehouse = new Warehouse(Guid.NewGuid(), orgId, access.BranchId.Value, code.Value!, input.Name ?? string.Empty, input.Address, access.ActorUserId, now);
            }
            catch (InventoryDomainException ex)
            {
                return Fail<WarehouseProjection>(ex);
            }

            _db.Warehouses.Add(warehouse);
            Audit(access, "warehouse.created", "Warehouse", warehouse.Id, traceId, new { code = warehouse.Code }, warehouse.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, warehouse.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<WarehouseProjection>("WAREHOUSE_CODE_CONFLICT", "The warehouse code already exists.");
            }

            return Result<WarehouseProjection>.Success(ToProjection(warehouse));
        });
    }

    private async Task<Result<WarehouseProjection>> MutateWarehouseAsync(
        RequestAccessContext access, Guid id, Guid expectedVersion, Action<Warehouse, DateTimeOffset> mutate, string audit, string traceId, CancellationToken ct)
    {
        var warehouse = await _db.Warehouses.FirstOrDefaultAsync(w => w.Id == id && w.OrganizationId == access.OrganizationId, ct);
        if (warehouse is null) return Fail<WarehouseProjection>("RESOURCE_NOT_FOUND", "Warehouse not found.");
        if (warehouse.RowVersion != expectedVersion) return Fail<WarehouseProjection>("WAREHOUSE_VERSION_CONFLICT", "Warehouse version conflict.");

        var now = _clock.UtcNow;
        try
        {
            mutate(warehouse, now);
        }
        catch (InventoryDomainException ex)
        {
            return Fail<WarehouseProjection>(ex);
        }

        Audit(access, audit, "Warehouse", warehouse.Id, traceId, new { code = warehouse.Code, status = warehouse.Status }, warehouse.RowVersion, now);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<WarehouseProjection>("WAREHOUSE_VERSION_CONFLICT", "Warehouse version conflict.");
        }

        return Result<WarehouseProjection>.Success(ToProjection(warehouse));
    }

    public Task<Result<WarehouseProjection>> UpdateWarehouseAsync(RequestAccessContext access, Guid warehouseId, Guid expectedVersion, WarehouseInput input, string traceId, CancellationToken ct = default) =>
        MutateWarehouseAsync(access, warehouseId, expectedVersion, (w, now) => w.Update(input.Name ?? string.Empty, input.Address, now), "warehouse.updated", traceId, ct);

    public async Task<Result<WarehouseProjection>> SetWarehouseActiveAsync(RequestAccessContext access, Guid warehouseId, Guid expectedVersion, bool active, string traceId, CancellationToken ct = default)
    {
        if (!active)
        {
            // A warehouse that still holds stock cannot be switched off, otherwise that stock would be stranded.
            var holdsStock = await _db.StockBalances.AsNoTracking()
                .AnyAsync(b => b.OrganizationId == access.OrganizationId && b.WarehouseId == warehouseId && (b.OnHand > 0 || b.Reserved > 0), ct);
            if (holdsStock) return Fail<WarehouseProjection>("WAREHOUSE_HAS_STOCK", "A warehouse that still holds stock cannot be deactivated.");
        }

        return await MutateWarehouseAsync(access, warehouseId, expectedVersion, (w, now) => w.SetActive(active, now), active ? "warehouse.activated" : "warehouse.deactivated", traceId, ct);
    }

    public async Task<WarehouseProjection?> GetWarehouseAsync(Guid organizationId, Guid warehouseId, CancellationToken ct = default)
    {
        var warehouse = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == warehouseId && w.OrganizationId == organizationId, ct);
        return warehouse is null ? null : ToProjection(warehouse);
    }

    public async Task<PagedWarehouses> ListWarehousesAsync(Guid organizationId, WarehouseListQuery query, CancellationToken ct = default)
    {
        var warehouses = _db.Warehouses.AsNoTracking().Where(w => w.OrganizationId == organizationId);
        if (query.Status is not null) warehouses = warehouses.Where(w => w.Status == query.Status);
        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            warehouses = warehouses.Where(w => w.NormalizedCode.ToLower().Contains(needle) || w.NormalizedName.Contains(needle));
        }

        var total = await warehouses.CountAsync(ct);
        var rows = await warehouses.OrderBy(w => w.NormalizedCode).ThenBy(w => w.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedWarehouses(rows.Select(ToProjection).ToList(), total, query.Page, query.PageSize);
    }

    // ===== posting helpers ===========================================================================

    private sealed record PostingItem(Item Item, Guid UnitId);

    private async Task<Result<Dictionary<Guid, PostingItem>>> ResolveItemsAsync(Guid orgId, IEnumerable<Guid> itemIds, CancellationToken ct)
    {
        var ids = itemIds.Distinct().ToList();
        var items = await _db.Items.AsNoTracking().Where(i => i.OrganizationId == orgId && ids.Contains(i.Id)).ToDictionaryAsync(i => i.Id, ct);
        var resolved = new Dictionary<Guid, PostingItem>();
        foreach (var id in ids)
        {
            if (!items.TryGetValue(id, out var item)) return Fail<Dictionary<Guid, PostingItem>>("RESOURCE_NOT_FOUND", $"Item '{id}' not found.");
            if (!item.Capabilities.CanStock) return Fail<Dictionary<Guid, PostingItem>>("INVENTORY_ITEM_NOT_STOCKABLE", $"Item '{item.Code}' is not a stocked item.");
            resolved[id] = new PostingItem(item, item.BaseUnitId);
        }

        return Result<Dictionary<Guid, PostingItem>>.Success(resolved);
    }

    private async Task<Result<Warehouse>> ResolveActiveWarehouseAsync(Guid orgId, Guid warehouseId, CancellationToken ct)
    {
        var warehouse = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == warehouseId && w.OrganizationId == orgId, ct);
        if (warehouse is null) return Fail<Warehouse>("RESOURCE_NOT_FOUND", "Warehouse not found.");
        if (warehouse.Status != WarehouseStatus.Active) return Fail<Warehouse>("INVENTORY_WAREHOUSE_INACTIVE", "The warehouse is not active.");
        return Result<Warehouse>.Success(warehouse);
    }

    /// <summary>
    /// Makes sure a balance row exists for every key and locks them in a fixed (warehouse, item) order so two concurrent
    /// postings serialize on the rows instead of deadlocking or overwriting each other.
    /// </summary>
    private async Task<Dictionary<(Guid Warehouse, Guid Item), StockBalance>> LockBalancesAsync(
        Guid orgId, IEnumerable<(Guid Warehouse, Guid Item, Guid Unit)> keys, DateTimeOffset now, CancellationToken ct)
    {
        var ordered = keys.Distinct().OrderBy(k => k.Warehouse).ThenBy(k => k.Item).ToList();
        var balances = new Dictionary<(Guid, Guid), StockBalance>();
        foreach (var (warehouse, item, unit) in ordered)
        {
            var newId = Guid.NewGuid();
            var rowVersion = Guid.NewGuid();
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO inventory.stock_balances (id, organization_id, warehouse_id, item_id, unit_id, on_hand, reserved, total_value, row_version, updated_at_utc)
                VALUES ({newId}, {orgId}, {warehouse}, {item}, {unit}, 0, 0, 0, {rowVersion}, {now.UtcDateTime})
                ON CONFLICT (organization_id, warehouse_id, item_id) DO NOTHING", ct);
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                SELECT 1 FROM inventory.stock_balances
                WHERE organization_id = {orgId} AND warehouse_id = {warehouse} AND item_id = {item} FOR UPDATE", ct);
            balances[(warehouse, item)] = await _db.StockBalances.FirstAsync(b => b.OrganizationId == orgId && b.WarehouseId == warehouse && b.ItemId == item, ct);
        }

        return balances;
    }

    private static StockMovement Movement(
        Guid orgId, StockDocument doc, StockBalance balance, string kind, decimal quantityDelta, decimal unitCost, decimal valueDelta, DateTimeOffset occurredAt, DateTimeOffset now) =>
        new(Guid.NewGuid(), orgId, doc.Id, balance.WarehouseId, balance.ItemId, balance.UnitId, kind, quantityDelta, unitCost, valueDelta, balance.OnHand, occurredAt, now);

    private static decimal CostPerUnit(decimal value, decimal quantity) => quantity == 0 ? 0m : decimal.Round(value / quantity, 4);

    /// <summary>Runs a posting inside one transaction with idempotent replay, then returns the stored document.</summary>
    private async Task<Result<StockDocumentProjection>> PostAsync(
        RequestAccessContext access,
        string operation,
        string keyHash,
        string payloadHash,
        Func<DateTimeOffset, Task<Result<StockDocument>>> build,
        string auditAction,
        string traceId,
        CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            // A caller that already opened a transaction (Production) gets stock movements inside it; otherwise this call owns one.
            var ownsTransaction = _db.Database.CurrentTransaction is null;
            await using var tx = ownsTransaction ? await _db.Database.BeginTransactionAsync(ct) : null;

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<StockDocumentProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetDocumentAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<StockDocumentProjection>.Success(replayed);
            }

            var now = _clock.UtcNow;
            Result<StockDocument> built;
            try
            {
                built = await build(now);
            }
            catch (InventoryDomainException ex)
            {
                return Fail<StockDocumentProjection>(ex);
            }

            if (built.IsFailure) return Result<StockDocumentProjection>.Failure(built.Error);

            var document = built.Value!;
            _db.StockDocuments.Add(document);
            Audit(access, auditAction, "StockDocument", document.Id, traceId,
                new { number = document.Number, type = document.DocumentType, lineCount = document.Movements.Count }, null, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, document.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                if (tx is not null) await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<StockDocumentProjection>("INVENTORY_VERSION_CONFLICT", "Stock changed concurrently; reload and try again.");
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<StockDocumentProjection>("INVENTORY_RECEIPT_ALREADY_POSTED", "This source document has already been put into stock.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation })
            {
                return Fail<StockDocumentProjection>("INVENTORY_INSUFFICIENT_STOCK", "Not enough available stock.");
            }

            return Result<StockDocumentProjection>.Success((await GetDocumentAsync(orgId, document.Id, ct))!);
        });
    }

    private async Task<string> NextNumberAsync(Guid orgId, string documentType, Guid branchId, DateTimeOffset now, CancellationToken ct)
    {
        try
        {
            return await _numbers.GenerateAsync(orgId, documentType, branchId, now, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InventoryDomainException("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate the stock document number.");
        }
    }

    // ===== receipts from goods receipts ==============================================================

    public Task<Result<StockDocumentProjection>> ReceiveGoodsReceiptAsync(
        RequestAccessContext access, Guid goodsReceiptId, Guid warehouseId, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        PostAsync(access, "inventory.receipt.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var receipt = await _db.GoodsReceipts.AsNoTracking().Include(r => r.Lines).FirstOrDefaultAsync(r => r.Id == goodsReceiptId && r.OrganizationId == orgId, ct);
            if (receipt is null) return Fail<StockDocument>("RESOURCE_NOT_FOUND", "Goods receipt not found.");

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);
            if (warehouse.Value!.BranchId != receipt.BranchId)
            {
                return Fail<StockDocument>("INVENTORY_WAREHOUSE_BRANCH_MISMATCH", "The warehouse must belong to the branch that received the goods.");
            }

            if (await _db.StockDocuments.AsNoTracking().AnyAsync(d => d.OrganizationId == orgId && d.SourceType == StockSourceType.GoodsReceipt && d.SourceId == goodsReceiptId, ct))
            {
                return Fail<StockDocument>("INVENTORY_RECEIPT_ALREADY_POSTED", "This goods receipt has already been put into stock.");
            }

            var items = await ResolveItemsAsync(orgId, receipt.Lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReceipts, receipt.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, receipt.BranchId, StockDocumentType.Receipt, number, warehouseId, null, receipt.ProjectId,
                StockSourceType.GoodsReceipt, goodsReceiptId, null, receipt.ReceivedAtUtc, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, receipt.Lines.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            foreach (var line in receipt.Lines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var value = balance.Receive(line.Quantity, line.UnitPrice, now);
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.Receipt, line.Quantity, line.UnitPrice, value, receipt.ReceivedAtUtc, now));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-receipt.posted", traceId, ct);

    // ===== issues ====================================================================================

    public Task<Result<StockDocumentProjection>> IssueAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, string? reason, IReadOnlyList<StockLineInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        PostAsync(access, "inventory.issue.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            if (projectId.HasValue)
            {
                var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId.Value && p.OrganizationId == orgId, ct);
                if (project is null) return Fail<StockDocument>("RESOURCE_NOT_FOUND", "Project not found.");
                if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
                {
                    return Fail<StockDocument>("INVENTORY_PROJECT_NOT_ACTIVE", "Stock cannot be issued to a completed or cancelled project.");
                }
            }

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockIssues, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Issue, number, warehouseId, null, projectId, null, null, reason, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, lines.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            foreach (var line in lines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var reservations = projectId.HasValue
                    ? await _db.StockReservations.Where(r => r.OrganizationId == orgId && r.WarehouseId == warehouseId && r.ItemId == line.ItemId && r.ProjectId == projectId.Value && r.Status == ReservationStatus.Active)
                        .OrderBy(r => r.CreatedAtUtc).ToListAsync(ct)
                    : new List<StockReservation>();

                var quantity = decimal.Round(line.Quantity, 4);
                var value = balance.Issue(quantity, reservations.Sum(r => r.Quantity), now);

                // A project issue first uses up that project's own reservation.
                var toConsume = quantity;
                var consumedTotal = 0m;
                foreach (var reservation in reservations)
                {
                    if (toConsume <= 0) break;
                    var consumed = reservation.Consume(toConsume, now);
                    consumedTotal += consumed;
                    toConsume -= consumed;
                }

                if (consumedTotal > 0) balance.ReleaseReservation(consumedTotal, now);
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.Issue, -quantity, CostPerUnit(value, quantity), -value, now, now));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-issue.posted", traceId, ct);

    // ===== transfers =================================================================================

    public Task<Result<StockDocumentProjection>> TransferAsync(
        RequestAccessContext access, Guid fromWarehouseId, Guid toWarehouseId, string? reason, IReadOnlyList<StockLineInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        PostAsync(access, "inventory.transfer.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var from = await ResolveActiveWarehouseAsync(orgId, fromWarehouseId, ct);
            if (from.IsFailure) return Result<StockDocument>.Failure(from.Error);
            var to = await ResolveActiveWarehouseAsync(orgId, toWarehouseId, ct);
            if (to.IsFailure) return Result<StockDocument>.Failure(to.Error);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockTransfers, from.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, from.Value.BranchId, StockDocumentType.Transfer, number, fromWarehouseId, toWarehouseId, null, null, null, reason, now, access.ActorUserId, now);

            // Both legs are locked together in a stable order, so a transfer is atomic and cannot deadlock with another one.
            var keys = lines.SelectMany(l => new[] { (fromWarehouseId, l.ItemId, items.Value![l.ItemId].UnitId), (toWarehouseId, l.ItemId, items.Value![l.ItemId].UnitId) });
            var balances = await LockBalancesAsync(orgId, keys, now, ct);
            foreach (var line in lines)
            {
                var source = balances[(fromWarehouseId, line.ItemId)];
                var target = balances[(toWarehouseId, line.ItemId)];
                var quantity = decimal.Round(line.Quantity, 4);

                var value = source.Issue(quantity, 0m, now);
                target.ReceiveAtValue(quantity, value, now);
                var unitCost = CostPerUnit(value, quantity);
                document.AddMovement(Movement(orgId, document, source, StockMovementKind.TransferOut, -quantity, unitCost, -value, now, now));
                document.AddMovement(Movement(orgId, document, target, StockMovementKind.TransferIn, quantity, unitCost, value, now, now));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-transfer.posted", traceId, ct);

    // ===== adjustments (stock count) =================================================================

    public Task<Result<StockDocumentProjection>> AdjustAsync(
        RequestAccessContext access, Guid warehouseId, string reason, IReadOnlyList<AdjustmentLineInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        PostAsync(access, "inventory.adjustment.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockAdjustments, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Adjustment, number, warehouseId, null, null, null, null, reason, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, lines.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            foreach (var line in lines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var delta = decimal.Round(line.CountedQuantity, 4) - balance.OnHand;
                if (delta == 0) continue;

                if (delta > 0)
                {
                    var cost = line.UnitCost ?? (balance.OnHand > 0 ? balance.AverageCost : (decimal?)null);
                    if (cost is null) return Fail<StockDocument>("INVENTORY_COST_REQUIRED", "A unit cost is required to add stock to an empty balance.");
                    var value = balance.Receive(delta, cost.Value, now);
                    document.AddMovement(Movement(orgId, document, balance, StockMovementKind.AdjustmentIn, delta, cost.Value, value, now, now));
                }
                else
                {
                    var quantity = -delta;
                    var value = balance.Issue(quantity, 0m, now);
                    document.AddMovement(Movement(orgId, document, balance, StockMovementKind.AdjustmentOut, -quantity, CostPerUnit(value, quantity), -value, now, now));
                }
            }

            return document.Movements.Count == 0
                ? Fail<StockDocument>("INVENTORY_NO_CHANGE", "The counted quantities match the system; nothing to adjust.")
                : Result<StockDocument>.Success(document);
        }, "stock-adjustment.posted", traceId, ct);

    // ===== reservations ==============================================================================

    public async Task<Result<ReservationProjection>> ReserveAsync(
        RequestAccessContext access, Guid warehouseId, Guid itemId, Guid projectId, decimal quantity, string? note, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.reservation.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<ReservationProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetReservationAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<ReservationProjection>.Success(replayed);
            }

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<ReservationProjection>.Failure(warehouse.Error);

            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.OrganizationId == orgId, ct);
            if (project is null) return Fail<ReservationProjection>("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
            {
                return Fail<ReservationProjection>("INVENTORY_PROJECT_NOT_ACTIVE", "Stock cannot be reserved for a completed or cancelled project.");
            }

            var items = await ResolveItemsAsync(orgId, new[] { itemId }, ct);
            if (items.IsFailure) return Result<ReservationProjection>.Failure(items.Error);

            var now = _clock.UtcNow;
            var balances = await LockBalancesAsync(orgId, new[] { (warehouseId, itemId, items.Value![itemId].UnitId) }, now, ct);
            StockReservation reservation;
            try
            {
                balances[(warehouseId, itemId)].Reserve(decimal.Round(quantity, 4), now);
                reservation = new StockReservation(Guid.NewGuid(), orgId, warehouseId, itemId, projectId, quantity, note, access.ActorUserId, now);
            }
            catch (InventoryDomainException ex)
            {
                return Fail<ReservationProjection>(ex);
            }

            _db.StockReservations.Add(reservation);
            Audit(access, "stock-reservation.created", "StockReservation", reservation.Id, traceId, new { itemId, projectId, quantity = reservation.Quantity }, reservation.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, reservation.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<ReservationProjection>("INVENTORY_VERSION_CONFLICT", "Stock changed concurrently; reload and try again.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation })
            {
                return Fail<ReservationProjection>("INVENTORY_INSUFFICIENT_STOCK", "Not enough available stock to reserve.");
            }

            return Result<ReservationProjection>.Success((await GetReservationAsync(orgId, reservation.Id, ct))!);
        });
    }

    public async Task<Result<ReservationProjection>> ReleaseReservationAsync(
        RequestAccessContext access, Guid reservationId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var reservation = await _db.StockReservations.FirstOrDefaultAsync(r => r.Id == reservationId && r.OrganizationId == orgId, ct);
            if (reservation is null) return Fail<ReservationProjection>("RESOURCE_NOT_FOUND", "Reservation not found.");
            if (reservation.RowVersion != expectedVersion) return Fail<ReservationProjection>("INVENTORY_RESERVATION_VERSION_CONFLICT", "Reservation version conflict.");

            var now = _clock.UtcNow;
            var item = await _db.Items.AsNoTracking().FirstAsync(i => i.Id == reservation.ItemId, ct);
            var balances = await LockBalancesAsync(orgId, new[] { (reservation.WarehouseId, reservation.ItemId, item.BaseUnitId) }, now, ct);
            try
            {
                var quantity = reservation.Quantity;
                reservation.Release(now);
                balances[(reservation.WarehouseId, reservation.ItemId)].ReleaseReservation(quantity, now);
            }
            catch (InventoryDomainException ex)
            {
                return Fail<ReservationProjection>(ex);
            }

            Audit(access, "stock-reservation.released", "StockReservation", reservation.Id, traceId, new { reservation.ItemId, reservation.ProjectId }, reservation.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<ReservationProjection>("INVENTORY_RESERVATION_VERSION_CONFLICT", "Reservation version conflict.");
            }

            return Result<ReservationProjection>.Success((await GetReservationAsync(orgId, reservation.Id, ct))!);
        });
    }


    // ===== production port (idempotent per source id, joins the caller's transaction) =================

    private static string SourceKey(string operation, Guid sourceId) => Application.Common.Security.Sha256Hex.Compute($"{operation}:{sourceId}");

    private static ProductionStockResult ToPortResult(StockDocumentProjection d) =>
        new(d.Id, d.Number, d.Movements.Select(m => new ProductionStockLine(m.Item.Id, Math.Abs(m.QuantityDelta), Math.Abs(m.ValueDelta))).ToList());

    private static Result<ProductionStockResult> ToPortResult(Result<StockDocumentProjection> result) =>
        result.IsFailure ? Result<ProductionStockResult>.Failure(result.Error) : Result<ProductionStockResult>.Success(ToPortResult(result.Value!));

    public async Task<Result<ProductionStockResult>> IssueMaterialsAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, string reason,
        IReadOnlyList<ProductionIssueLine> lines, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.production-issue.post";
        var result = await PostAsync(access, operation, SourceKey(operation, sourceId), SourceKey(operation + ":payload", sourceId), async now =>
        {
            var orgId = access.OrganizationId;
            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockIssues, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Issue, number, warehouseId, null, projectId,
                StockSourceType.WorkOrderIssue, sourceId, reason, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, lines.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            foreach (var line in lines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var reservations = projectId.HasValue
                    ? await _db.StockReservations.Where(r => r.OrganizationId == orgId && r.WarehouseId == warehouseId && r.ItemId == line.ItemId && r.ProjectId == projectId.Value && r.Status == ReservationStatus.Active)
                        .OrderBy(r => r.CreatedAtUtc).ToListAsync(ct)
                    : new List<StockReservation>();

                var quantity = decimal.Round(line.Quantity, 4);
                var value = balance.Issue(quantity, reservations.Sum(r => r.Quantity), now);
                var toConsume = quantity;
                var consumedTotal = 0m;
                foreach (var reservation in reservations)
                {
                    if (toConsume <= 0) break;
                    var consumed = reservation.Consume(toConsume, now);
                    consumedTotal += consumed;
                    toConsume -= consumed;
                }

                if (consumedTotal > 0) balance.ReleaseReservation(consumedTotal, now);
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.Issue, -quantity, CostPerUnit(value, quantity), -value, now, now));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-issue.posted", traceId, ct);
        return ToPortResult(result);
    }

    public async Task<Result<ProductionStockResult>> ReturnMaterialsAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, string reason,
        IReadOnlyList<ProductionReturnLine> lines, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.production-return.post";
        var result = await PostAsync(access, operation, SourceKey(operation, sourceId), SourceKey(operation + ":payload", sourceId), async now =>
        {
            var orgId = access.OrganizationId;
            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReturns, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Return, number, warehouseId, null, projectId,
                StockSourceType.WorkOrderReturn, sourceId, reason, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, lines.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            foreach (var line in lines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var quantity = decimal.Round(line.Quantity, 4);
                balance.ReceiveAtValue(quantity, line.Value, now);
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.ReturnIn, quantity, CostPerUnit(line.Value, quantity), line.Value, now, now));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-return.posted", traceId, ct);
        return ToPortResult(result);
    }

    public async Task<Result<ProductionStockResult>> ReceiveOutputAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, Guid itemId,
        decimal quantity, decimal totalValue, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.production-output.post";
        var result = await PostAsync(access, operation, SourceKey(operation, sourceId), SourceKey(operation + ":payload", sourceId), async now =>
        {
            var orgId = access.OrganizationId;
            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            var items = await ResolveItemsAsync(orgId, new[] { itemId }, ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReceipts, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Receipt, number, warehouseId, null, projectId,
                StockSourceType.WorkOrderCompletion, sourceId, null, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, new[] { (warehouseId, itemId, items.Value![itemId].UnitId) }, now, ct);
            var balance = balances[(warehouseId, itemId)];
            var rounded = decimal.Round(quantity, 4);
            balance.ReceiveAtValue(rounded, totalValue, now);
            document.AddMovement(Movement(orgId, document, balance, StockMovementKind.Receipt, rounded, CostPerUnit(totalValue, rounded), totalValue, now, now));
            return Result<StockDocument>.Success(document);
        }, "stock-receipt.posted", traceId, ct);
        return ToPortResult(result);
    }

    // ===== reads =====================================================================================

    private async Task<ReservationProjection?> GetReservationAsync(Guid orgId, Guid reservationId, CancellationToken ct) =>
        (await ReservationQueryable(orgId).Where(x => x.Reservation.Id == reservationId).Select(ToReservation()).FirstOrDefaultAsync(ct));

    private IQueryable<ReservationRow> ReservationQueryable(Guid orgId) =>
        from r in _db.StockReservations.AsNoTracking()
        where r.OrganizationId == orgId
        join w in _db.Warehouses.AsNoTracking() on r.WarehouseId equals w.Id
        join i in _db.Items.AsNoTracking() on r.ItemId equals i.Id
        join u in _db.Units.AsNoTracking() on i.BaseUnitId equals u.Id
        join p in _db.Projects.AsNoTracking() on r.ProjectId equals p.Id
        select new ReservationRow { Reservation = r, Warehouse = w, Item = i, Unit = u, Project = p };

    private sealed class ReservationRow
    {
        public StockReservation Reservation { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public UnitOfMeasure Unit { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }

    private static System.Linq.Expressions.Expression<Func<ReservationRow, ReservationProjection>> ToReservation() => x => new ReservationProjection(
        x.Reservation.Id,
        new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
        new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
        x.Project.Id, x.Project.Code, x.Reservation.Quantity, x.Reservation.Status, x.Reservation.Note, x.Reservation.RowVersion, x.Reservation.CreatedAtUtc);

    public async Task<PagedReservations> ListReservationsAsync(Guid organizationId, ReservationQuery query, CancellationToken ct = default)
    {
        var rows = ReservationQueryable(organizationId);
        if (query.ProjectId.HasValue) rows = rows.Where(x => x.Reservation.ProjectId == query.ProjectId.Value);
        if (query.WarehouseId.HasValue) rows = rows.Where(x => x.Reservation.WarehouseId == query.WarehouseId.Value);
        if (query.Status is not null) rows = rows.Where(x => x.Reservation.Status == query.Status);

        var total = await rows.CountAsync(ct);
        var items = await rows.OrderByDescending(x => x.Reservation.CreatedAtUtc).ThenByDescending(x => x.Reservation.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(ToReservation()).ToListAsync(ct);
        return new PagedReservations(items, total, query.Page, query.PageSize);
    }

    private sealed class BalanceRow
    {
        public StockBalance Balance { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public UnitOfMeasure Unit { get; set; } = null!;
    }

    private IQueryable<BalanceRow> BalanceQueryable(Guid orgId) =>
        from b in _db.StockBalances.AsNoTracking()
        where b.OrganizationId == orgId
        join w in _db.Warehouses.AsNoTracking() on b.WarehouseId equals w.Id
        join i in _db.Items.AsNoTracking() on b.ItemId equals i.Id
        join u in _db.Units.AsNoTracking() on b.UnitId equals u.Id
        select new BalanceRow { Balance = b, Warehouse = w, Item = i, Unit = u };

    private static StockBalanceProjection ToProjection(BalanceRow x) => new(
        x.Balance.Id,
        new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
        new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
        x.Balance.OnHand, x.Balance.Reserved, x.Balance.Available, x.Balance.AverageCost, x.Balance.TotalValue, x.Balance.UpdatedAtUtc);

    public async Task<PagedStockBalances> ListBalancesAsync(Guid organizationId, StockBalanceQuery query, CancellationToken ct = default)
    {
        var rows = BalanceQueryable(organizationId);
        if (query.WarehouseId.HasValue) rows = rows.Where(x => x.Balance.WarehouseId == query.WarehouseId.Value);
        if (query.ItemId.HasValue) rows = rows.Where(x => x.Balance.ItemId == query.ItemId.Value);
        if (query.InStockOnly) rows = rows.Where(x => x.Balance.OnHand > 0);
        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            rows = rows.Where(x => x.Item.NormalizedCode.ToLower().Contains(needle) || x.Item.Name.Thai.ToLower().Contains(needle));
        }

        var total = await rows.CountAsync(ct);
        var totalValue = await rows.SumAsync(x => (decimal?)x.Balance.TotalValue, ct) ?? 0m;
        var page = await rows.OrderBy(x => x.Warehouse.NormalizedCode).ThenBy(x => x.Item.NormalizedCode).ThenBy(x => x.Balance.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedStockBalances(page.Select(ToProjection).ToList(), total, query.Page, query.PageSize, totalValue);
    }

    private sealed class MovementRow
    {
        public StockMovement Movement { get; set; } = null!;
        public StockDocument Document { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public UnitOfMeasure Unit { get; set; } = null!;
    }

    private IQueryable<MovementRow> MovementQueryable(Guid orgId) =>
        from m in _db.StockMovements.AsNoTracking()
        where m.OrganizationId == orgId
        join d in _db.StockDocuments.AsNoTracking() on m.StockDocumentId equals d.Id
        join w in _db.Warehouses.AsNoTracking() on m.WarehouseId equals w.Id
        join i in _db.Items.AsNoTracking() on m.ItemId equals i.Id
        join u in _db.Units.AsNoTracking() on m.UnitId equals u.Id
        select new MovementRow { Movement = m, Document = d, Warehouse = w, Item = i, Unit = u };

    private static StockMovementProjection ToProjection(MovementRow x) => new(
        x.Movement.Id, x.Document.Id, x.Document.DocumentType, x.Document.Number,
        new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
        new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
        x.Movement.Kind, x.Movement.QuantityDelta, x.Movement.UnitCost, x.Movement.ValueDelta, x.Movement.OnHandAfter, x.Movement.OccurredAtUtc, x.Movement.PostedAtUtc);

    public async Task<PagedStockMovements> ListMovementsAsync(Guid organizationId, StockMovementQuery query, CancellationToken ct = default)
    {
        var rows = MovementQueryable(organizationId);
        if (query.WarehouseId.HasValue) rows = rows.Where(x => x.Movement.WarehouseId == query.WarehouseId.Value);
        if (query.ItemId.HasValue) rows = rows.Where(x => x.Movement.ItemId == query.ItemId.Value);
        if (query.Kind is not null) rows = rows.Where(x => x.Movement.Kind == query.Kind);
        if (query.DocumentId.HasValue) rows = rows.Where(x => x.Movement.StockDocumentId == query.DocumentId.Value);

        var total = await rows.CountAsync(ct);
        var page = await rows.OrderByDescending(x => x.Movement.PostedAtUtc).ThenByDescending(x => x.Movement.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedStockMovements(page.Select(ToProjection).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<StockDocumentProjection?> GetDocumentAsync(Guid organizationId, Guid documentId, CancellationToken ct = default)
    {
        var document = await _db.StockDocuments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == documentId && d.OrganizationId == organizationId, ct);
        if (document is null) return null;

        var movements = (await MovementQueryable(organizationId).Where(x => x.Movement.StockDocumentId == documentId)
            .OrderBy(x => x.Movement.PostedAtUtc).ThenBy(x => x.Movement.Id).ToListAsync(ct)).Select(ToProjection).ToList();
        var warehouseIds = new[] { document.WarehouseId, document.ToWarehouseId ?? Guid.Empty }.Where(id => id != Guid.Empty).ToList();
        var warehouses = await _db.Warehouses.AsNoTracking().Where(w => warehouseIds.Contains(w.Id)).ToDictionaryAsync(w => w.Id, ct);
        InventoryWarehouseRef Ref(Guid id) => new(id, warehouses[id].Code, warehouses[id].Name);
        var user = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == document.PostedByUserId, ct);

        return new StockDocumentProjection(
            document.Id, document.DocumentType, document.Number, Ref(document.WarehouseId),
            document.ToWarehouseId.HasValue ? Ref(document.ToWarehouseId.Value) : null,
            document.ProjectId, document.SourceType, document.SourceId, document.Reason, document.OccurredAtUtc,
            new InventoryPerson(user.Id, user.DisplayName, user.Email), document.PostedAtUtc, movements);
    }

    public async Task<ReconciliationProjection> ReconcileAsync(Guid organizationId, Guid? warehouseId, CancellationToken ct = default)
    {
        var balances = BalanceQueryable(organizationId);
        if (warehouseId.HasValue) balances = balances.Where(x => x.Balance.WarehouseId == warehouseId.Value);
        var balanceRows = await balances.ToListAsync(ct);

        var movements = _db.StockMovements.AsNoTracking().Where(m => m.OrganizationId == organizationId);
        if (warehouseId.HasValue) movements = movements.Where(m => m.WarehouseId == warehouseId.Value);
        var sums = await movements.GroupBy(m => new { m.WarehouseId, m.ItemId })
            .Select(g => new { g.Key.WarehouseId, g.Key.ItemId, Quantity = g.Sum(m => m.QuantityDelta), Value = g.Sum(m => m.ValueDelta) })
            .ToDictionaryAsync(x => (x.WarehouseId, x.ItemId), ct);

        var rows = balanceRows.Select(x =>
        {
            sums.TryGetValue((x.Balance.WarehouseId, x.Balance.ItemId), out var ledger);
            var ledgerQuantity = ledger?.Quantity ?? 0m;
            var ledgerValue = ledger?.Value ?? 0m;
            return new ReconciliationRow(
                new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
                new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
                x.Balance.OnHand, ledgerQuantity, x.Balance.TotalValue, ledgerValue,
                x.Balance.OnHand == ledgerQuantity && x.Balance.TotalValue == ledgerValue);
        }).OrderBy(r => r.Warehouse.Code).ThenBy(r => r.Item.Code).ToList();

        return new ReconciliationProjection(rows.Count, rows.Count(r => !r.IsConsistent), rows);
    }
}
