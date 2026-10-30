using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Inventory;
using TanErp.Application.Notifications;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Persistence.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Inventory;

public class InventoryStore : IInventoryStore, IProductionStockPort, IProcurementStockPort
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly INotificationPublisher _notifications;

    public InventoryStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _notifications = notifications;
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

    // ===== warehouses & locations ====================================================================

    private static WarehouseProjection ToProjection(Warehouse w) =>
        new(w.Id, w.BranchId, w.Code, w.Name, w.Address, w.Status, w.RowVersion, w.CreatedAtUtc);

    private static WarehouseLocationProjection ToProjection(WarehouseLocation l) =>
        new(l.Id, l.WarehouseId, l.Code, l.Name, l.IsDefault, l.Status, l.RowVersion, l.CreatedAtUtc, l.UpdatedAtUtc);

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

            var defaultLocation = WarehouseLocation.CreateDefault(orgId, warehouse.Id, now);
            _db.Warehouses.Add(warehouse);
            _db.WarehouseLocations.Add(defaultLocation);
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

    private async Task<WarehouseLocation> EnsureDefaultLocationAsync(Guid orgId, Guid warehouseId, DateTimeOffset now, CancellationToken ct)
    {
        var existing = await _db.WarehouseLocations.AsNoTracking()
            .FirstOrDefaultAsync(l => l.OrganizationId == orgId && l.WarehouseId == warehouseId && l.IsDefault, ct);
        if (existing is not null) return existing;

        var newId = Guid.NewGuid();
        var rowVersion = Guid.NewGuid();
        await _db.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO inventory.warehouse_locations (id, organization_id, warehouse_id, code, normalized_code, name, is_default, status, row_version, created_at_utc, updated_at_utc)
            VALUES ({newId}, {orgId}, {warehouseId}, {WarehouseLocation.DefaultCode}, {WarehouseLocation.DefaultCode}, {WarehouseLocation.DefaultName}, TRUE, {WarehouseLocationStatus.Active}, {rowVersion}, {now.UtcDateTime}, {now.UtcDateTime})
            ON CONFLICT (organization_id, warehouse_id, normalized_code) DO NOTHING", ct);

        return await _db.WarehouseLocations.AsNoTracking()
            .FirstAsync(l => l.OrganizationId == orgId && l.WarehouseId == warehouseId && l.NormalizedCode == WarehouseLocation.DefaultCode, ct);
    }

    public async Task<Result<IReadOnlyList<WarehouseLocationProjection>>> ListWarehouseLocationsAsync(Guid organizationId, Guid warehouseId, CancellationToken ct = default)
    {
        var exists = await _db.Warehouses.AsNoTracking().AnyAsync(w => w.Id == warehouseId && w.OrganizationId == organizationId, ct);
        if (!exists) return Fail<IReadOnlyList<WarehouseLocationProjection>>("RESOURCE_NOT_FOUND", "Warehouse not found.");

        await EnsureDefaultLocationAsync(organizationId, warehouseId, _clock.UtcNow, ct);
        var locations = await _db.WarehouseLocations.AsNoTracking()
            .Where(l => l.OrganizationId == organizationId && l.WarehouseId == warehouseId)
            .OrderByDescending(l => l.IsDefault)
            .ThenBy(l => l.NormalizedCode)
            .ThenBy(l => l.Id)
            .ToListAsync(ct);

        return Result<IReadOnlyList<WarehouseLocationProjection>>.Success(locations.Select(ToProjection).ToList());
    }

    public async Task<Result<WarehouseLocationProjection>> CreateWarehouseLocationAsync(
        RequestAccessContext access, Guid warehouseId, WarehouseLocationCreateInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.warehouse-location.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WarehouseLocationProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                if (Guid.TryParse(replay.ResourceId, out var replayId))
                {
                    var replayed = await _db.WarehouseLocations.AsNoTracking().FirstOrDefaultAsync(l => l.Id == replayId && l.OrganizationId == orgId, ct);
                    if (replayed is not null) return Result<WarehouseLocationProjection>.Success(ToProjection(replayed));
                }
            }

            var warehouse = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == warehouseId && w.OrganizationId == orgId, ct);
            if (warehouse is null) return Fail<WarehouseLocationProjection>("RESOURCE_NOT_FOUND", "Warehouse not found.");

            var now = _clock.UtcNow;
            await EnsureDefaultLocationAsync(orgId, warehouseId, now, ct);

            WarehouseLocation location;
            try
            {
                location = new WarehouseLocation(Guid.NewGuid(), orgId, warehouseId, input.Code ?? string.Empty, input.Name ?? string.Empty, false, now);
            }
            catch (InventoryDomainException ex)
            {
                return Fail<WarehouseLocationProjection>(ex);
            }

            if (await _db.WarehouseLocations.AsNoTracking().AnyAsync(l => l.OrganizationId == orgId && l.WarehouseId == warehouseId && l.NormalizedCode == location.NormalizedCode, ct))
            {
                return Fail<WarehouseLocationProjection>("LOCATION_CODE_CONFLICT", "The location code already exists in this warehouse.");
            }

            _db.WarehouseLocations.Add(location);
            Audit(access, "warehouse-location.created", "WarehouseLocation", location.Id, traceId, new { warehouseId, code = location.Code, name = location.Name }, location.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, location.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<WarehouseLocationProjection>("LOCATION_CODE_CONFLICT", "The location code already exists in this warehouse.");
            }

            return Result<WarehouseLocationProjection>.Success(ToProjection(location));
        });
    }

    public async Task<Result<WarehouseLocationProjection>> UpdateWarehouseLocationAsync(
        RequestAccessContext access, Guid warehouseId, Guid locationId, Guid expectedVersion, WarehouseLocationUpdateInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var location = await _db.WarehouseLocations.FirstOrDefaultAsync(l => l.Id == locationId && l.WarehouseId == warehouseId && l.OrganizationId == orgId, ct);
        if (location is null) return Fail<WarehouseLocationProjection>("RESOURCE_NOT_FOUND", "Warehouse location not found.");
        if (location.RowVersion != expectedVersion) return Fail<WarehouseLocationProjection>("LOCATION_VERSION_CONFLICT", "Warehouse location version conflict.");

        if (input.Active == false && !location.IsDefault)
        {
            var holdsStock = await _db.StockLocationBalances.AsNoTracking()
                .AnyAsync(b => b.OrganizationId == orgId && b.WarehouseId == warehouseId && b.LocationId == locationId && b.OnHand > 0, ct);
            if (holdsStock) return Fail<WarehouseLocationProjection>("LOCATION_HAS_STOCK", "A location that still holds stock cannot be deactivated.");
        }

        var now = _clock.UtcNow;
        try
        {
            location.Update(input.Name ?? string.Empty, input.Active, now);
        }
        catch (InventoryDomainException ex)
        {
            return Fail<WarehouseLocationProjection>(ex);
        }

        Audit(access, "warehouse-location.updated", "WarehouseLocation", location.Id, traceId, new { warehouseId, code = location.Code, status = location.Status }, location.RowVersion, now);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<WarehouseLocationProjection>("LOCATION_VERSION_CONFLICT", "Warehouse location version conflict.");
        }

        return Result<WarehouseLocationProjection>.Success(ToProjection(location));
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

    private async Task<Result<WarehouseLocation>> ResolveActiveLocationAsync(Guid orgId, Guid warehouseId, Guid? locationId, DateTimeOffset now, CancellationToken ct)
    {
        if (!locationId.HasValue)
        {
            var defaultLoc = await EnsureDefaultLocationAsync(orgId, warehouseId, now, ct);
            return Result<WarehouseLocation>.Success(defaultLoc);
        }

        var location = await _db.WarehouseLocations.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == locationId.Value && l.WarehouseId == warehouseId && l.OrganizationId == orgId, ct);
        if (location is null) return Fail<WarehouseLocation>("RESOURCE_NOT_FOUND", "Warehouse location not found.");
        if (location.Status != WarehouseLocationStatus.Active) return Fail<WarehouseLocation>("INVENTORY_LOCATION_INACTIVE", "The warehouse location is not active.");
        return Result<WarehouseLocation>.Success(location);
    }

    private async Task<Result<bool>> EnsurePeriodOpenAsync(Guid orgId, DateTimeOffset occurredAtUtc, CancellationToken ct)
    {
        var occurredDate = DateOnly.FromDateTime(occurredAtUtc.UtcDateTime);
        var closed = await _db.InventoryPeriodCloses.AsNoTracking()
            .AnyAsync(p => p.OrganizationId == orgId && occurredDate <= p.PeriodEndDate, ct);
        if (closed)
        {
            return Fail<bool>("INVENTORY_PERIOD_CLOSED", $"Cannot post inventory movements on or before a closed period ({occurredDate:yyyy-MM-dd}).");
        }

        return Result<bool>.Success(true);
    }

    private static string ValidateAndNormalizeLot(Item item, string? rawLotNumber)
    {
        var lot = StockLocationBalance.NormalizeLot(rawLotNumber);
        if (item.Capabilities.TrackLot && string.IsNullOrEmpty(lot))
        {
            throw new InventoryDomainException("INVENTORY_LOT_REQUIRED", $"Item '{item.Code}' requires a lot number.");
        }

        return lot;
    }

    private static IReadOnlyList<(string Trimmed, string Normalized)> ValidateAndNormalizeSerials(
        Item item, decimal quantity, IReadOnlyList<string>? rawSerials)
    {
        if (!item.Capabilities.TrackSerial)
        {
            if (rawSerials is null || rawSerials.Count == 0)
            {
                return Array.Empty<(string, string)>();
            }

            return rawSerials.Where(s => !string.IsNullOrWhiteSpace(s)).Select(StockSerial.NormalizeSerial).ToList();
        }

        var rounded = decimal.Round(quantity, 4);
        if (rounded <= 0 || rounded != decimal.Truncate(rounded))
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_COUNT_MISMATCH", $"Serial-tracked item '{item.Code}' requires a positive whole number quantity.");
        }

        var expectedCount = (int)rounded;
        if (rawSerials is null || rawSerials.Count != expectedCount)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_COUNT_MISMATCH", $"Item '{item.Code}' requires {expectedCount} serial number(s).");
        }

        var normalized = rawSerials.Select(StockSerial.NormalizeSerial).ToList();
        if (normalized.Select(x => x.Normalized).Distinct(StringComparer.Ordinal).Count() != expectedCount)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_COUNT_MISMATCH", "Duplicate serial numbers in the same line are not allowed.");
        }

        return normalized;
    }

    private static string? SerializeSerials(IReadOnlyList<(string Trimmed, string Normalized)> serials) =>
        serials.Count == 0 ? null : JsonSerializer.Serialize(serials.Select(s => s.Trimmed).ToList());

    private async Task ReceiveSerialsInAsync(
        Guid orgId, Guid itemId, Guid warehouseId, Guid locationId, string lotNumber,
        IReadOnlyList<(string Trimmed, string Normalized)> serials, Guid documentId, DateTimeOffset now, CancellationToken ct)
    {
        if (serials.Count == 0) return;

        var normalizedSet = serials.Select(s => s.Normalized).ToHashSet(StringComparer.Ordinal);
        if (_db.StockSerials.Local.Any(s => s.OrganizationId == orgId && s.ItemId == itemId && normalizedSet.Contains(s.NormalizedSerialNumber)))
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_DUPLICATE", "One or more serial numbers already exist for this item.");
        }

        var existingInDb = await _db.StockSerials.AsNoTracking()
            .AnyAsync(s => s.OrganizationId == orgId && s.ItemId == itemId && normalizedSet.Contains(s.NormalizedSerialNumber), ct);
        if (existingInDb)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_DUPLICATE", "One or more serial numbers already exist for this item.");
        }

        foreach (var s in serials)
        {
            _db.StockSerials.Add(new StockSerial(Guid.NewGuid(), orgId, itemId, s.Trimmed, warehouseId, locationId, lotNumber, documentId, now));
        }
    }

    private async Task IssueSerialsOutAsync(
        Guid orgId, Guid itemId, Guid warehouseId, Guid locationId, string lotNumber,
        IReadOnlyList<(string Trimmed, string Normalized)> serials, Guid documentId, DateTimeOffset now, bool isSupplierReturn, CancellationToken ct)
    {
        if (serials.Count == 0) return;

        var normalizedSet = serials.Select(s => s.Normalized).ToList();
        var rows = await _db.StockSerials
            .Where(s => s.OrganizationId == orgId && s.ItemId == itemId && normalizedSet.Contains(s.NormalizedSerialNumber))
            .ToListAsync(ct);

        if (rows.Count != serials.Count)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", "One or more serial numbers were not found in stock.");
        }

        foreach (var row in rows)
        {
            if (row.Status != StockSerialStatus.InStock || row.WarehouseId != warehouseId || row.LocationId != locationId || row.LotNumber != lotNumber)
            {
                throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{row.SerialNumber}' is not available in the specified warehouse, location, and lot.");
            }

            if (isSupplierReturn)
            {
                row.ReturnToSupplier(documentId, now);
            }
            else
            {
                row.IssueOut(documentId, now);
            }
        }
    }

    private async Task TransferSerialsAsync(
        Guid orgId, Guid itemId, Guid fromWarehouseId, Guid fromLocationId, Guid toWarehouseId, Guid toLocationId, string lotNumber,
        IReadOnlyList<(string Trimmed, string Normalized)> serials, Guid documentId, DateTimeOffset now, CancellationToken ct)
    {
        if (serials.Count == 0) return;

        var normalizedSet = serials.Select(s => s.Normalized).ToList();
        var rows = await _db.StockSerials
            .Where(s => s.OrganizationId == orgId && s.ItemId == itemId && normalizedSet.Contains(s.NormalizedSerialNumber))
            .ToListAsync(ct);

        if (rows.Count != serials.Count)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", "One or more serial numbers were not found in stock.");
        }

        foreach (var row in rows)
        {
            if (row.Status != StockSerialStatus.InStock || row.WarehouseId != fromWarehouseId || row.LocationId != fromLocationId || row.LotNumber != lotNumber)
            {
                throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{row.SerialNumber}' is not available in the source warehouse, location, and lot.");
            }

            row.TransferTo(toWarehouseId, toLocationId, documentId, now);
        }
    }

    private async Task ReturnSerialsFromProjectAsync(
        Guid orgId, Guid itemId, Guid warehouseId, Guid locationId, string lotNumber,
        IReadOnlyList<(string Trimmed, string Normalized)> serials, Guid documentId, DateTimeOffset now, CancellationToken ct)
    {
        if (serials.Count == 0) return;

        var normalizedSet = serials.Select(s => s.Normalized).ToList();
        var rows = await _db.StockSerials
            .Where(s => s.OrganizationId == orgId && s.ItemId == itemId && normalizedSet.Contains(s.NormalizedSerialNumber))
            .ToListAsync(ct);

        if (rows.Count != serials.Count)
        {
            throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", "One or more returned serial numbers were not found.");
        }

        foreach (var row in rows)
        {
            if (row.Status != StockSerialStatus.Issued || (!string.IsNullOrEmpty(lotNumber) && row.LotNumber != lotNumber))
            {
                throw new InventoryDomainException("INVENTORY_SERIAL_NOT_AVAILABLE", $"Serial '{row.SerialNumber}' is not currently issued with the matching lot.");
            }

            row.ReturnFromProject(warehouseId, locationId, lotNumber, documentId, now);
        }
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

    private async Task<Dictionary<(Guid Warehouse, Guid Location, Guid Item, string Lot), StockLocationBalance>> LockLocationBalancesAsync(
        Guid orgId,
        IEnumerable<(Guid Warehouse, Guid Location, Guid Item, Guid Unit, string Lot)> keys,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var ordered = keys.Distinct()
            .OrderBy(k => k.Warehouse)
            .ThenBy(k => k.Location)
            .ThenBy(k => k.Item)
            .ThenBy(k => k.Lot, StringComparer.Ordinal)
            .ToList();

        var balances = new Dictionary<(Guid, Guid, Guid, string), StockLocationBalance>();
        foreach (var (warehouse, location, item, unit, lot) in ordered)
        {
            var newId = Guid.NewGuid();
            var rowVersion = Guid.NewGuid();
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO inventory.stock_location_balances (id, organization_id, warehouse_id, location_id, item_id, unit_id, lot_number, on_hand, row_version, updated_at_utc)
                VALUES ({newId}, {orgId}, {warehouse}, {location}, {item}, {unit}, {lot}, 0, {rowVersion}, {now.UtcDateTime})
                ON CONFLICT (organization_id, warehouse_id, location_id, item_id, lot_number) DO NOTHING", ct);
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                SELECT 1 FROM inventory.stock_location_balances
                WHERE organization_id = {orgId} AND warehouse_id = {warehouse} AND location_id = {location} AND item_id = {item} AND lot_number = {lot} FOR UPDATE", ct);
            balances[(warehouse, location, item, lot)] = await _db.StockLocationBalances.FirstAsync(
                b => b.OrganizationId == orgId && b.WarehouseId == warehouse && b.LocationId == location && b.ItemId == item && b.LotNumber == lot, ct);
        }

        return balances;
    }

    private static StockMovement Movement(
        Guid orgId, StockDocument doc, StockBalance balance, string kind, decimal quantityDelta, decimal unitCost, decimal valueDelta,
        DateTimeOffset occurredAt, DateTimeOffset now, Guid? locationId = null, string? lotNumber = null, string? serialNumbersJson = null) =>
        new(Guid.NewGuid(), orgId, doc.Id, balance.WarehouseId, balance.ItemId, balance.UnitId, kind, quantityDelta, unitCost, valueDelta, balance.OnHand,
            occurredAt, now, locationId, lotNumber, serialNumbersJson);

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
                if (ex.InnerException is PostgresException pg && pg.ConstraintName == "IX_stock_serials_organization_id_item_id_normalized_serial_num~")
                {
                    return Fail<StockDocumentProjection>("INVENTORY_SERIAL_DUPLICATE", "One or more serial numbers already exist for this item.");
                }

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
        RequestAccessContext access,
        Guid goodsReceiptId,
        Guid warehouseId,
        Guid? locationId,
        IReadOnlyList<ReceiveGoodsReceiptLineInput>? lines,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken ct = default) =>
        PostAsync(access, "inventory.receipt.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var receipt = await _db.GoodsReceipts.AsNoTracking().Include(r => r.Lines).FirstOrDefaultAsync(r => r.Id == goodsReceiptId && r.OrganizationId == orgId, ct);
            if (receipt is null) return Fail<StockDocument>("RESOURCE_NOT_FOUND", "Goods receipt not found.");

            var periodCheck = await EnsurePeriodOpenAsync(orgId, receipt.ReceivedAtUtc, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

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

            // Build receipt line allocations
            var allocations = new List<(Guid ItemId, decimal Quantity, decimal UnitPrice, Guid LocationId, string LotNumber, IReadOnlyList<(string Trimmed, string Normalized)> Serials)>();
            if (lines is null || lines.Count == 0)
            {
                var defaultLoc = await ResolveActiveLocationAsync(orgId, warehouseId, locationId, now, ct);
                if (defaultLoc.IsFailure) return Result<StockDocument>.Failure(defaultLoc.Error);

                foreach (var grLine in receipt.Lines)
                {
                    var item = items.Value![grLine.ItemId].Item;
                    var lot = ValidateAndNormalizeLot(item, null);
                    var serials = ValidateAndNormalizeSerials(item, grLine.Quantity, null);
                    allocations.Add((grLine.ItemId, grLine.Quantity, grLine.UnitPrice, defaultLoc.Value!.Id, lot, serials));
                }
            }
            else
            {
                var grByItem = receipt.Lines
                    .GroupBy(l => l.ItemId)
                    .ToDictionary(g => g.Key, g => (Quantity: decimal.Round(g.Sum(x => x.Quantity), 4), UnitPrice: g.First().UnitPrice));

                foreach (var inputLine in lines)
                {
                    if (!grByItem.TryGetValue(inputLine.ItemId, out var grInfo))
                    {
                        return Fail<StockDocument>("INVENTORY_LINE_INVALID", $"Item '{inputLine.ItemId}' is not on the goods receipt.");
                    }

                    var qty = decimal.Round(inputLine.Quantity ?? grInfo.Quantity, 4);
                    if (qty <= 0) return Fail<StockDocument>("INVENTORY_LINE_INVALID", "Quantity must be greater than zero.");

                    var resolvedLoc = await ResolveActiveLocationAsync(orgId, warehouseId, inputLine.LocationId ?? locationId, now, ct);
                    if (resolvedLoc.IsFailure) return Result<StockDocument>.Failure(resolvedLoc.Error);

                    var item = items.Value![inputLine.ItemId].Item;
                    var lot = ValidateAndNormalizeLot(item, inputLine.LotNumber);
                    var serials = ValidateAndNormalizeSerials(item, qty, inputLine.SerialNumbers);
                    allocations.Add((inputLine.ItemId, qty, grInfo.UnitPrice, resolvedLoc.Value!.Id, lot, serials));
                }

                // Verify totals match receipt.Lines per item
                foreach (var (itemId, grInfo) in grByItem)
                {
                    var allocatedQty = decimal.Round(allocations.Where(a => a.ItemId == itemId).Sum(a => a.Quantity), 4);
                    if (allocatedQty != grInfo.Quantity)
                    {
                        return Fail<StockDocument>("INVENTORY_LINE_INVALID", "Allocated receipt line quantities must match the goods receipt quantities.");
                    }
                }
            }

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReceipts, receipt.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, receipt.BranchId, StockDocumentType.Receipt, number, warehouseId, null, receipt.ProjectId,
                StockSourceType.GoodsReceipt, goodsReceiptId, null, receipt.ReceivedAtUtc, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, allocations.Select(a => (warehouseId, a.ItemId, items.Value![a.ItemId].UnitId)), now, ct);
            var locBalances = await LockLocationBalancesAsync(
                orgId,
                allocations.Select(a => (warehouseId, a.LocationId, a.ItemId, items.Value![a.ItemId].UnitId, a.LotNumber)),
                now,
                ct);

            foreach (var alloc in allocations)
            {
                var balance = balances[(warehouseId, alloc.ItemId)];
                var locBalance = locBalances[(warehouseId, alloc.LocationId, alloc.ItemId, alloc.LotNumber)];

                await ReceiveSerialsInAsync(orgId, alloc.ItemId, warehouseId, alloc.LocationId, alloc.LotNumber, alloc.Serials, document.Id, now, ct);

                var value = balance.Receive(alloc.Quantity, alloc.UnitPrice, now);
                locBalance.Receive(alloc.Quantity, now);
                document.AddMovement(Movement(
                    orgId, document, balance, StockMovementKind.Receipt, alloc.Quantity, alloc.UnitPrice, value,
                    receipt.ReceivedAtUtc, now, alloc.LocationId, alloc.LotNumber, SerializeSerials(alloc.Serials)));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-receipt.posted", traceId, ct);

    // ===== issues ====================================================================================

    public Task<Result<StockDocumentProjection>> IssueAsync(
        RequestAccessContext access,
        Guid warehouseId,
        Guid? locationId,
        Guid? projectId,
        DateTimeOffset? occurredAtUtc,
        string? reason,
        IReadOnlyList<StockLineInput> lines,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken ct = default) =>
        PostAsync(access, "inventory.issue.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var occurredAt = (occurredAtUtc ?? now).ToUniversalTime();
            var periodCheck = await EnsurePeriodOpenAsync(orgId, occurredAt, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

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

            var resolvedLines = new List<(StockLineInput Line, Guid LocationId, string LotNumber, IReadOnlyList<(string Trimmed, string Normalized)> Serials)>();
            foreach (var line in lines)
            {
                var loc = await ResolveActiveLocationAsync(orgId, warehouseId, line.LocationId ?? locationId, now, ct);
                if (loc.IsFailure) return Result<StockDocument>.Failure(loc.Error);
                var item = items.Value![line.ItemId].Item;
                var lot = ValidateAndNormalizeLot(item, line.LotNumber);
                var serials = ValidateAndNormalizeSerials(item, line.Quantity, line.SerialNumbers);
                resolvedLines.Add((line, loc.Value!.Id, lot, serials));
            }

            var number = await NextNumberAsync(orgId, DocumentTypes.StockIssues, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Issue, number, warehouseId, null, projectId, null, null, reason, occurredAt, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, resolvedLines.Select(l => (warehouseId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId)), now, ct);
            var locBalances = await LockLocationBalancesAsync(
                orgId,
                resolvedLines.Select(l => (warehouseId, l.LocationId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId, l.LotNumber)),
                now,
                ct);

            foreach (var (line, locId, lot, serials) in resolvedLines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var locBalance = locBalances[(warehouseId, locId, line.ItemId, lot)];
                var reservations = projectId.HasValue
                    ? await _db.StockReservations.Where(r => r.OrganizationId == orgId && r.WarehouseId == warehouseId && r.ItemId == line.ItemId && r.ProjectId == projectId.Value && r.Status == ReservationStatus.Active)
                        .OrderBy(r => r.CreatedAtUtc).ToListAsync(ct)
                    : new List<StockReservation>();

                var quantity = decimal.Round(line.Quantity, 4);
                locBalance.Issue(quantity, now);
                await IssueSerialsOutAsync(orgId, line.ItemId, warehouseId, locId, lot, serials, document.Id, now, isSupplierReturn: false, ct);

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
                document.AddMovement(Movement(
                    orgId, document, balance, StockMovementKind.Issue, -quantity, CostPerUnit(value, quantity), -value,
                    occurredAt, now, locId, lot, SerializeSerials(serials)));
            }

            if (projectId.HasValue)
            {
                var issuedValue = decimal.Round(document.Movements.Sum(m => Math.Abs(m.ValueDelta)), 2);
                if (issuedValue > 0m)
                {
                    await RecordProjectMaterialCostAsync(orgId, projectId.Value, ProjectCostSourceType.StockIssue, document.Id, document.Number,
                        $"Stock issue {document.Number}", issuedValue, false, access.ActorUserId, now, ct);
                }
            }

            return Result<StockDocument>.Success(document);
        }, "stock-issue.posted", traceId, ct);

    // ===== transfers =================================================================================

    public Task<Result<StockDocumentProjection>> TransferAsync(
        RequestAccessContext access,
        Guid fromWarehouseId,
        Guid toWarehouseId,
        Guid? fromLocationId,
        Guid? toLocationId,
        DateTimeOffset? occurredAtUtc,
        string? reason,
        IReadOnlyList<StockLineInput> lines,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken ct = default) =>
        PostAsync(access, "inventory.transfer.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var occurredAt = (occurredAtUtc ?? now).ToUniversalTime();
            var periodCheck = await EnsurePeriodOpenAsync(orgId, occurredAt, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var from = await ResolveActiveWarehouseAsync(orgId, fromWarehouseId, ct);
            if (from.IsFailure) return Result<StockDocument>.Failure(from.Error);
            var to = await ResolveActiveWarehouseAsync(orgId, toWarehouseId, ct);
            if (to.IsFailure) return Result<StockDocument>.Failure(to.Error);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var resolvedLines = new List<(StockLineInput Line, Guid FromLocationId, Guid ToLocationId, string LotNumber, IReadOnlyList<(string Trimmed, string Normalized)> Serials)>();
            foreach (var line in lines)
            {
                var srcLoc = await ResolveActiveLocationAsync(orgId, fromWarehouseId, line.LocationId ?? fromLocationId, now, ct);
                if (srcLoc.IsFailure) return Result<StockDocument>.Failure(srcLoc.Error);
                var dstLoc = await ResolveActiveLocationAsync(orgId, toWarehouseId, line.ToLocationId ?? toLocationId, now, ct);
                if (dstLoc.IsFailure) return Result<StockDocument>.Failure(dstLoc.Error);

                if (fromWarehouseId == toWarehouseId && srcLoc.Value!.Id == dstLoc.Value!.Id)
                {
                    return Fail<StockDocument>("INVENTORY_TRANSFER_SAME_WAREHOUSE", "Source and destination must differ.");
                }

                var item = items.Value![line.ItemId].Item;
                var lot = ValidateAndNormalizeLot(item, line.LotNumber);
                var serials = ValidateAndNormalizeSerials(item, line.Quantity, line.SerialNumbers);
                resolvedLines.Add((line, srcLoc.Value!.Id, dstLoc.Value!.Id, lot, serials));
            }

            var number = await NextNumberAsync(orgId, DocumentTypes.StockTransfers, from.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, from.Value.BranchId, StockDocumentType.Transfer, number, fromWarehouseId, toWarehouseId, null, null, null, reason, occurredAt, access.ActorUserId, now);

            // Both legs are locked together in a stable order, so a transfer is atomic and cannot deadlock with another one.
            var keys = resolvedLines.SelectMany(l => new[]
            {
                (fromWarehouseId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId),
                (toWarehouseId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId)
            });
            var balances = await LockBalancesAsync(orgId, keys, now, ct);

            var locKeys = resolvedLines.SelectMany(l => new[]
            {
                (fromWarehouseId, l.FromLocationId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId, l.LotNumber),
                (toWarehouseId, l.ToLocationId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId, l.LotNumber)
            });
            var locBalances = await LockLocationBalancesAsync(orgId, locKeys, now, ct);

            foreach (var (line, srcLocId, dstLocId, lot, serials) in resolvedLines)
            {
                var source = balances[(fromWarehouseId, line.ItemId)];
                var target = balances[(toWarehouseId, line.ItemId)];
                var srcLocBalance = locBalances[(fromWarehouseId, srcLocId, line.ItemId, lot)];
                var dstLocBalance = locBalances[(toWarehouseId, dstLocId, line.ItemId, lot)];
                var quantity = decimal.Round(line.Quantity, 4);

                srcLocBalance.Issue(quantity, now);
                dstLocBalance.Receive(quantity, now);
                await TransferSerialsAsync(orgId, line.ItemId, fromWarehouseId, srcLocId, toWarehouseId, dstLocId, lot, serials, document.Id, now, ct);

                var value = source.Issue(quantity, 0m, now);
                target.ReceiveAtValue(quantity, value, now);
                var unitCost = CostPerUnit(value, quantity);
                var serialsJson = SerializeSerials(serials);
                document.AddMovement(Movement(orgId, document, source, StockMovementKind.TransferOut, -quantity, unitCost, -value, occurredAt, now, srcLocId, lot, serialsJson));
                document.AddMovement(Movement(orgId, document, target, StockMovementKind.TransferIn, quantity, unitCost, value, occurredAt, now, dstLocId, lot, serialsJson));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-transfer.posted", traceId, ct);

    // ===== adjustments (stock count) =================================================================

    public Task<Result<StockDocumentProjection>> AdjustAsync(
        RequestAccessContext access,
        Guid warehouseId,
        Guid? locationId,
        DateTimeOffset? occurredAtUtc,
        string reason,
        IReadOnlyList<AdjustmentLineInput> lines,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken ct = default) =>
        PostAsync(access, "inventory.adjustment.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var occurredAt = (occurredAtUtc ?? now).ToUniversalTime();
            var periodCheck = await EnsurePeriodOpenAsync(orgId, occurredAt, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var resolvedLines = new List<(AdjustmentLineInput Line, Guid LocationId, string LotNumber)>();
            foreach (var line in lines)
            {
                var loc = await ResolveActiveLocationAsync(orgId, warehouseId, line.LocationId ?? locationId, now, ct);
                if (loc.IsFailure) return Result<StockDocument>.Failure(loc.Error);
                var item = items.Value![line.ItemId].Item;
                var lot = ValidateAndNormalizeLot(item, line.LotNumber);
                resolvedLines.Add((line, loc.Value!.Id, lot));
            }

            var number = await NextNumberAsync(orgId, DocumentTypes.StockAdjustments, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Adjustment, number, warehouseId, null, null, null, null, reason, occurredAt, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, resolvedLines.Select(l => (warehouseId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId)), now, ct);
            var locBalances = await LockLocationBalancesAsync(
                orgId,
                resolvedLines.Select(l => (warehouseId, l.LocationId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId, l.LotNumber)),
                now,
                ct);

            foreach (var (line, locId, lot) in resolvedLines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var locBalance = locBalances[(warehouseId, locId, line.ItemId, lot)];
                var item = items.Value![line.ItemId].Item;
                var delta = decimal.Round(line.CountedQuantity, 4) - locBalance.OnHand;
                if (delta == 0) continue;

                if (delta > 0)
                {
                    var cost = line.UnitCost ?? (balance.OnHand > 0 ? balance.AverageCost : (decimal?)null);
                    if (cost is null) return Fail<StockDocument>("INVENTORY_COST_REQUIRED", "A unit cost is required to add stock to an empty balance.");
                    var serials = ValidateAndNormalizeSerials(item, delta, line.SerialNumbers);
                    await ReceiveSerialsInAsync(orgId, line.ItemId, warehouseId, locId, lot, serials, document.Id, now, ct);

                    var value = balance.Receive(delta, cost.Value, now);
                    locBalance.Receive(delta, now);
                    document.AddMovement(Movement(
                        orgId, document, balance, StockMovementKind.AdjustmentIn, delta, cost.Value, value,
                        occurredAt, now, locId, lot, SerializeSerials(serials)));
                }
                else
                {
                    var quantity = -delta;
                    var serials = ValidateAndNormalizeSerials(item, quantity, line.SerialNumbers);
                    locBalance.Issue(quantity, now);
                    await IssueSerialsOutAsync(orgId, line.ItemId, warehouseId, locId, lot, serials, document.Id, now, isSupplierReturn: false, ct);

                    var value = balance.Issue(quantity, 0m, now);
                    document.AddMovement(Movement(
                        orgId, document, balance, StockMovementKind.AdjustmentOut, -quantity, CostPerUnit(value, quantity), -value,
                        occurredAt, now, locId, lot, SerializeSerials(serials)));
                }
            }

            return document.Movements.Count == 0
                ? Fail<StockDocument>("INVENTORY_NO_CHANGE", "The counted quantities match the system; nothing to adjust.")
                : Result<StockDocument>.Success(document);
        }, "stock-adjustment.posted", traceId, ct);

    // ===== customer / project returns ================================================================

    public Task<Result<StockDocumentProjection>> ReturnAsync(
        RequestAccessContext access,
        Guid warehouseId,
        Guid? locationId,
        Guid? projectId,
        Guid? sourceIssueDocumentId,
        DateTimeOffset? occurredAtUtc,
        string reason,
        IReadOnlyList<ReturnLineInput> lines,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken ct = default) =>
        PostAsync(access, "inventory.return.post", keyHash, payloadHash, async now =>
        {
            var orgId = access.OrganizationId;
            var occurredAt = (occurredAtUtc ?? now).ToUniversalTime();
            var periodCheck = await EnsurePeriodOpenAsync(orgId, occurredAt, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);

            StockDocument? sourceIssueDoc = null;
            if (sourceIssueDocumentId.HasValue)
            {
                sourceIssueDoc = await _db.StockDocuments.AsNoTracking().Include(d => d.Movements)
                    .FirstOrDefaultAsync(d => d.Id == sourceIssueDocumentId.Value && d.OrganizationId == orgId, ct);
                if (sourceIssueDoc is null || sourceIssueDoc.DocumentType != StockDocumentType.Issue)
                {
                    return Fail<StockDocument>("RESOURCE_NOT_FOUND", "Source stock issue document not found.");
                }

                if (projectId.HasValue && sourceIssueDoc.ProjectId != projectId.Value)
                {
                    return Fail<StockDocument>("INVENTORY_FIELD_INVALID", "Source issue document does not belong to the specified project.");
                }
            }

            var effectiveProjectId = projectId ?? sourceIssueDoc?.ProjectId;
            if (effectiveProjectId.HasValue)
            {
                var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == effectiveProjectId.Value && p.OrganizationId == orgId, ct);
                if (project is null) return Fail<StockDocument>("RESOURCE_NOT_FOUND", "Project not found.");
                if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
                {
                    return Fail<StockDocument>("INVENTORY_PROJECT_NOT_ACTIVE", "Stock cannot be returned for a completed or cancelled project.");
                }
            }

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var resolvedLines = new List<(ReturnLineInput Line, Guid LocationId, string LotNumber, IReadOnlyList<(string Trimmed, string Normalized)> Serials)>();
            foreach (var line in lines)
            {
                var loc = await ResolveActiveLocationAsync(orgId, warehouseId, line.LocationId ?? locationId, now, ct);
                if (loc.IsFailure) return Result<StockDocument>.Failure(loc.Error);
                var item = items.Value![line.ItemId].Item;
                var lot = ValidateAndNormalizeLot(item, line.LotNumber);
                var serials = ValidateAndNormalizeSerials(item, line.Quantity, line.SerialNumbers);
                resolvedLines.Add((line, loc.Value!.Id, lot, serials));
            }

            // Validate project return limits and compute issue unit costs per item
            var projectIssueCosts = new Dictionary<Guid, decimal>();
            if (effectiveProjectId.HasValue)
            {
                var itemIds = lines.Select(l => l.ItemId).Distinct().ToList();
                var projectMovements = await (
                    from m in _db.StockMovements.AsNoTracking()
                    join d in _db.StockDocuments.AsNoTracking() on m.StockDocumentId equals d.Id
                    where m.OrganizationId == orgId && d.ProjectId == effectiveProjectId.Value && itemIds.Contains(m.ItemId)
                          && (m.Kind == StockMovementKind.Issue || m.Kind == StockMovementKind.ReturnIn)
                    select new { m.ItemId, m.Kind, m.QuantityDelta, m.ValueDelta, m.StockDocumentId })
                    .ToListAsync(ct);

                foreach (var group in lines.GroupBy(l => l.ItemId))
                {
                    var itemId = group.Key;
                    var requestedReturnQty = decimal.Round(group.Sum(x => x.Quantity), 4);

                    var issuedMovements = projectMovements.Where(m => m.ItemId == itemId && m.Kind == StockMovementKind.Issue).ToList();
                    var returnedMovements = projectMovements.Where(m => m.ItemId == itemId && m.Kind == StockMovementKind.ReturnIn).ToList();

                    var totalIssuedQty = decimal.Round(issuedMovements.Sum(m => Math.Abs(m.QuantityDelta)), 4);
                    var totalReturnedQty = decimal.Round(returnedMovements.Sum(m => Math.Abs(m.QuantityDelta)), 4);
                    var remainingReturnable = totalIssuedQty - totalReturnedQty;

                    if (requestedReturnQty > remainingReturnable)
                    {
                        return Fail<StockDocument>("INVENTORY_RETURN_EXCEEDS_ISSUED", "Returned quantity cannot exceed the net issued quantity for the project.");
                    }

                    if (sourceIssueDoc is not null)
                    {
                        var sourceIssuedQty = decimal.Round(
                            sourceIssueDoc.Movements.Where(m => m.ItemId == itemId && m.Kind == StockMovementKind.Issue).Sum(m => Math.Abs(m.QuantityDelta)), 4);
                        if (requestedReturnQty > sourceIssuedQty)
                        {
                            return Fail<StockDocument>("INVENTORY_RETURN_EXCEEDS_ISSUED", "Returned quantity cannot exceed the source issue document quantity.");
                        }

                        var sourceIssuedVal = sourceIssueDoc.Movements.Where(m => m.ItemId == itemId && m.Kind == StockMovementKind.Issue).Sum(m => Math.Abs(m.ValueDelta));
                        if (sourceIssuedQty > 0)
                        {
                            projectIssueCosts[itemId] = decimal.Round(sourceIssuedVal / sourceIssuedQty, 4);
                        }
                    }

                    if (!projectIssueCosts.ContainsKey(itemId) && totalIssuedQty > 0)
                    {
                        var totalIssuedVal = issuedMovements.Sum(m => Math.Abs(m.ValueDelta));
                        projectIssueCosts[itemId] = decimal.Round(totalIssuedVal / totalIssuedQty, 4);
                    }
                }
            }

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReturns, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(
                Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Return, number, warehouseId, null, effectiveProjectId,
                StockSourceType.ProjectReturn, sourceIssueDocumentId, reason, occurredAt, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, resolvedLines.Select(l => (warehouseId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId)), now, ct);
            var locBalances = await LockLocationBalancesAsync(
                orgId,
                resolvedLines.Select(l => (warehouseId, l.LocationId, l.Line.ItemId, items.Value![l.Line.ItemId].UnitId, l.LotNumber)),
                now,
                ct);

            foreach (var (line, locId, lot, serials) in resolvedLines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var locBalance = locBalances[(warehouseId, locId, line.ItemId, lot)];
                var quantity = decimal.Round(line.Quantity, 4);

                decimal? unitCost = line.UnitCost;
                if (!unitCost.HasValue && projectIssueCosts.TryGetValue(line.ItemId, out var issueCost))
                {
                    unitCost = issueCost;
                }
                if (!unitCost.HasValue && balance.OnHand > 0)
                {
                    unitCost = balance.AverageCost;
                }
                if (!unitCost.HasValue)
                {
                    return Fail<StockDocument>("INVENTORY_COST_REQUIRED", "A unit cost is required to return stock when no prior cost is available.");
                }

                await ReturnSerialsFromProjectAsync(orgId, line.ItemId, warehouseId, locId, lot, serials, document.Id, now, ct);

                var value = decimal.Round(quantity * unitCost.Value, 2);
                balance.ReceiveAtValue(quantity, value, now);
                locBalance.Receive(quantity, now);

                document.AddMovement(Movement(
                    orgId, document, balance, StockMovementKind.ReturnIn, quantity, CostPerUnit(value, quantity), value,
                    occurredAt, now, locId, lot, SerializeSerials(serials)));
            }

            if (effectiveProjectId.HasValue)
            {
                var returnedValue = decimal.Round(document.Movements.Sum(m => Math.Abs(m.ValueDelta)), 2);
                if (returnedValue > 0m)
                {
                    await RecordProjectMaterialCostAsync(orgId, effectiveProjectId.Value, ProjectCostSourceType.StockReturn, document.Id, document.Number,
                        $"Stock return {document.Number}", -returnedValue, true, access.ActorUserId, now, ct);
                }
            }

            return Result<StockDocument>.Success(document);
        }, "stock-return.posted", traceId, ct);

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

    // ===== period closes =============================================================================

    public async Task<Result<InventoryPeriodCloseProjection>> ClosePeriodAsync(
        RequestAccessContext access, int periodYear, int periodMonth, string? note, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.period.close";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<InventoryPeriodCloseProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetPeriodCloseAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<InventoryPeriodCloseProjection>.Success(replayed);
            }

            var now = _clock.UtcNow;
            InventoryPeriodClose periodClose;
            try
            {
                periodClose = new InventoryPeriodClose(Guid.NewGuid(), orgId, periodYear, periodMonth, note, access.ActorUserId, now);
            }
            catch (InventoryDomainException ex)
            {
                return Fail<InventoryPeriodCloseProjection>(ex);
            }

            if (await _db.InventoryPeriodCloses.AsNoTracking().AnyAsync(p => p.OrganizationId == orgId && p.PeriodYear == periodYear && p.PeriodMonth == periodMonth, ct))
            {
                return Fail<InventoryPeriodCloseProjection>("INVENTORY_PERIOD_ALREADY_CLOSED", "This inventory period has already been closed.");
            }

            var balances = await _db.StockBalances.AsNoTracking()
                .Where(b => b.OrganizationId == orgId && (b.OnHand > 0 || b.TotalValue > 0))
                .OrderBy(b => b.WarehouseId)
                .ThenBy(b => b.ItemId)
                .ToListAsync(ct);

            foreach (var b in balances)
            {
                periodClose.AddLine(new InventoryPeriodCloseLine(
                    Guid.NewGuid(), periodClose.Id, b.WarehouseId, b.ItemId, b.UnitId, b.OnHand, b.AverageCost, b.TotalValue));
            }

            _db.InventoryPeriodCloses.Add(periodClose);
            Audit(access, "inventory-period.closed", "InventoryPeriodClose", periodClose.Id, traceId,
                new { periodYear, periodMonth, lineCount = periodClose.LineCount, totalValue = periodClose.TotalValuationAmount }, null, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, periodClose.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<InventoryPeriodCloseProjection>("INVENTORY_PERIOD_ALREADY_CLOSED", "This inventory period has already been closed.");
            }

            return Result<InventoryPeriodCloseProjection>.Success((await GetPeriodCloseAsync(orgId, periodClose.Id, ct))!);
        });
    }

    public async Task<IReadOnlyList<InventoryPeriodCloseProjection>> ListPeriodClosesAsync(Guid organizationId, CancellationToken ct = default)
    {
        var closes = await _db.InventoryPeriodCloses.AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.PeriodYear)
            .ThenByDescending(p => p.PeriodMonth)
            .ToListAsync(ct);

        if (closes.Count == 0) return Array.Empty<InventoryPeriodCloseProjection>();

        var userIds = closes.Select(c => c.ClosedByUserId).Distinct().ToList();
        var users = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);

        return closes.Select(c =>
        {
            var user = users[c.ClosedByUserId];
            return new InventoryPeriodCloseProjection(
                c.Id, c.PeriodYear, c.PeriodMonth, c.PeriodEndDate, c.Note, c.LineCount, c.TotalOnHandQuantity, c.TotalValuationAmount,
                new InventoryPerson(user.Id, user.DisplayName, user.Email), c.ClosedAtUtc, Array.Empty<InventoryPeriodCloseLineProjection>());
        }).ToList();
    }

    public async Task<InventoryPeriodCloseProjection?> GetPeriodCloseAsync(Guid organizationId, Guid periodCloseId, CancellationToken ct = default)
    {
        var close = await _db.InventoryPeriodCloses.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == periodCloseId && p.OrganizationId == organizationId, ct);
        if (close is null) return null;

        var lines = await (
            from l in _db.InventoryPeriodCloseLines.AsNoTracking()
            where l.PeriodCloseId == periodCloseId
            join w in _db.Warehouses.AsNoTracking() on l.WarehouseId equals w.Id
            join i in _db.Items.AsNoTracking() on l.ItemId equals i.Id
            join u in _db.Units.AsNoTracking() on l.UnitId equals u.Id
            orderby w.NormalizedCode, i.NormalizedCode
            select new InventoryPeriodCloseLineProjection(
                l.Id,
                new InventoryWarehouseRef(w.Id, w.Code, w.Name),
                new InventoryItemRef(i.Id, i.Code, i.Name.Thai, u.Code),
                l.OnHandQuantity,
                l.AverageCost,
                l.TotalValue))
            .ToListAsync(ct);

        var user = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == close.ClosedByUserId, ct);
        return new InventoryPeriodCloseProjection(
            close.Id, close.PeriodYear, close.PeriodMonth, close.PeriodEndDate, close.Note, close.LineCount, close.TotalOnHandQuantity, close.TotalValuationAmount,
            new InventoryPerson(user.Id, user.DisplayName, user.Email), close.ClosedAtUtc, lines);
    }

    // ===== production & procurement ports ============================================================

    private static string SourceKey(string operation, Guid sourceId) => Application.Common.Security.Sha256Hex.Compute($"{operation}:{sourceId}");

    private static ProductionStockResult ToPortResult(StockDocumentProjection d) =>
        new(d.Id, d.Number, d.Movements.Select(m => new ProductionStockLine(m.Item.Id, Math.Abs(m.QuantityDelta), Math.Abs(m.ValueDelta))).ToList());

    private static Result<ProductionStockResult> ToPortResult(Result<StockDocumentProjection> result) =>
        result.IsFailure ? Result<ProductionStockResult>.Failure(result.Error) : Result<ProductionStockResult>.Success(ToPortResult(result.Value!));

    private async Task DeductFromLocationBalancesForPortAsync(
        Guid orgId, Guid warehouseId, Guid defaultLocationId, Guid itemId, Guid unitId, decimal quantity, DateTimeOffset now, CancellationToken ct)
    {
        var defaultBalances = await LockLocationBalancesAsync(orgId, new[] { (warehouseId, defaultLocationId, itemId, unitId, string.Empty) }, now, ct);
        var defaultLocBal = defaultBalances[(warehouseId, defaultLocationId, itemId, string.Empty)];
        if (defaultLocBal.OnHand >= quantity)
        {
            defaultLocBal.Issue(quantity, now);
            return;
        }

        // If stock is distributed across other locations/lots in this warehouse, drain them in deterministic order.
        var remaining = quantity;
        if (defaultLocBal.OnHand > 0)
        {
            var take = defaultLocBal.OnHand;
            defaultLocBal.Issue(take, now);
            remaining -= take;
        }

        var otherBins = await _db.StockLocationBalances
            .Where(b => b.OrganizationId == orgId && b.WarehouseId == warehouseId && b.ItemId == itemId && b.OnHand > 0
                        && !(b.LocationId == defaultLocationId && b.LotNumber == string.Empty))
            .OrderBy(b => b.UpdatedAtUtc)
            .ThenBy(b => b.Id)
            .ToListAsync(ct);

        foreach (var bin in otherBins)
        {
            if (remaining <= 0) break;
            var take = Math.Min(remaining, bin.OnHand);
            bin.Issue(take, now);
            remaining -= take;
        }
    }

    public async Task<Result<ProductionStockResult>> IssueMaterialsAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, string reason,
        IReadOnlyList<ProductionIssueLine> lines, string traceId, CancellationToken ct = default)
    {
        const string operation = "inventory.production-issue.post";
        var result = await PostAsync(access, operation, SourceKey(operation, sourceId), SourceKey(operation + ":payload", sourceId), async now =>
        {
            var orgId = access.OrganizationId;
            var periodCheck = await EnsurePeriodOpenAsync(orgId, now, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);
            var defaultLoc = await EnsureDefaultLocationAsync(orgId, warehouseId, now, ct);

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
                await DeductFromLocationBalancesForPortAsync(orgId, warehouseId, defaultLoc.Id, line.ItemId, balance.UnitId, quantity, now, ct);

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
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.Issue, -quantity, CostPerUnit(value, quantity), -value, now, now, defaultLoc.Id));
            }

            if (projectId.HasValue)
            {
                var issuedValue = decimal.Round(document.Movements.Sum(m => Math.Abs(m.ValueDelta)), 2);
                if (issuedValue > 0m)
                {
                    await RecordProjectMaterialCostAsync(orgId, projectId.Value, ProjectCostSourceType.WorkOrderIssue, document.Id, document.Number,
                        $"Work order material issue {document.Number}", issuedValue, false, access.ActorUserId, now, ct);
                }
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
            var periodCheck = await EnsurePeriodOpenAsync(orgId, now, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);
            var defaultLoc = await EnsureDefaultLocationAsync(orgId, warehouseId, now, ct);

            var items = await ResolveItemsAsync(orgId, lines.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReturns, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Return, number, warehouseId, null, projectId,
                StockSourceType.WorkOrderReturn, sourceId, reason, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, lines.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            var locBalances = await LockLocationBalancesAsync(orgId, lines.Select(l => (warehouseId, defaultLoc.Id, l.ItemId, items.Value![l.ItemId].UnitId, string.Empty)), now, ct);
            foreach (var line in lines)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var locBalance = locBalances[(warehouseId, defaultLoc.Id, line.ItemId, string.Empty)];
                var quantity = decimal.Round(line.Quantity, 4);
                balance.ReceiveAtValue(quantity, line.Value, now);
                locBalance.Receive(quantity, now);
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.ReturnIn, quantity, CostPerUnit(line.Value, quantity), line.Value, now, now, defaultLoc.Id));
            }

            if (projectId.HasValue)
            {
                var returnedValue = decimal.Round(document.Movements.Sum(m => Math.Abs(m.ValueDelta)), 2);
                if (returnedValue > 0m)
                {
                    await RecordProjectMaterialCostAsync(orgId, projectId.Value, ProjectCostSourceType.WorkOrderReturn, document.Id, document.Number,
                        $"Work order material return {document.Number}", -returnedValue, true, access.ActorUserId, now, ct);
                }
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
            var periodCheck = await EnsurePeriodOpenAsync(orgId, now, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);
            var defaultLoc = await EnsureDefaultLocationAsync(orgId, warehouseId, now, ct);

            var items = await ResolveItemsAsync(orgId, new[] { itemId }, ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReceipts, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Receipt, number, warehouseId, null, projectId,
                StockSourceType.WorkOrderCompletion, sourceId, null, now, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, new[] { (warehouseId, itemId, items.Value![itemId].UnitId) }, now, ct);
            var locBalances = await LockLocationBalancesAsync(orgId, new[] { (warehouseId, defaultLoc.Id, itemId, items.Value![itemId].UnitId, string.Empty) }, now, ct);
            var balance = balances[(warehouseId, itemId)];
            var locBalance = locBalances[(warehouseId, defaultLoc.Id, itemId, string.Empty)];
            var rounded = decimal.Round(quantity, 4);
            balance.ReceiveAtValue(rounded, totalValue, now);
            locBalance.Receive(rounded, now);
            document.AddMovement(Movement(orgId, document, balance, StockMovementKind.Receipt, rounded, CostPerUnit(totalValue, rounded), totalValue, now, now, defaultLoc.Id));
            return Result<StockDocument>.Success(document);
        }, "stock-receipt.posted", traceId, ct);
        return ToPortResult(result);
    }

    public async Task<Result<ProcurementStockResult>> ReturnToSupplierAsync(
        RequestAccessContext access,
        Guid warehouseId,
        Guid? projectId,
        Guid supplierReturnId,
        string reason,
        DateTimeOffset occurredAtUtc,
        IReadOnlyList<ProcurementReturnStockLine> lines,
        string traceId,
        CancellationToken ct = default)
    {
        const string operation = "inventory.supplier-return.post";
        var result = await PostAsync(access, operation, SourceKey(operation, supplierReturnId), SourceKey(operation + ":payload", supplierReturnId), async now =>
        {
            var orgId = access.OrganizationId;
            var periodCheck = await EnsurePeriodOpenAsync(orgId, occurredAtUtc, ct);
            if (periodCheck.IsFailure) return Result<StockDocument>.Failure(periodCheck.Error);

            var warehouse = await ResolveActiveWarehouseAsync(orgId, warehouseId, ct);
            if (warehouse.IsFailure) return Result<StockDocument>.Failure(warehouse.Error);
            var defaultLoc = await EnsureDefaultLocationAsync(orgId, warehouseId, now, ct);

            var grouped = lines
                .GroupBy(l => l.ItemId)
                .Select(g => (ItemId: g.Key, Quantity: decimal.Round(g.Sum(x => x.Quantity), 4)))
                .ToList();

            var items = await ResolveItemsAsync(orgId, grouped.Select(l => l.ItemId), ct);
            if (items.IsFailure) return Result<StockDocument>.Failure(items.Error);

            var number = await NextNumberAsync(orgId, DocumentTypes.StockReturns, warehouse.Value!.BranchId, now, ct);
            var document = new StockDocument(
                Guid.NewGuid(), orgId, warehouse.Value.BranchId, StockDocumentType.Return, number, warehouseId, null, projectId,
                StockSourceType.SupplierReturn, supplierReturnId, reason, occurredAtUtc, access.ActorUserId, now);

            var balances = await LockBalancesAsync(orgId, grouped.Select(l => (warehouseId, l.ItemId, items.Value![l.ItemId].UnitId)), now, ct);
            foreach (var line in grouped)
            {
                var balance = balances[(warehouseId, line.ItemId)];
                var value = balance.Issue(line.Quantity, 0m, now);
                await DeductFromLocationBalancesForPortAsync(orgId, warehouseId, defaultLoc.Id, line.ItemId, balance.UnitId, line.Quantity, now, ct);
                document.AddMovement(Movement(orgId, document, balance, StockMovementKind.ReturnOut, -line.Quantity, CostPerUnit(value, line.Quantity), -value, occurredAtUtc, now, defaultLoc.Id));
            }

            return Result<StockDocument>.Success(document);
        }, "stock-return.posted", traceId, ct);

        return result.IsFailure
            ? Result<ProcurementStockResult>.Failure(result.Error)
            : Result<ProcurementStockResult>.Success(new ProcurementStockResult(result.Value!.Id, result.Value.Number));
    }

    private async Task RecordProjectMaterialCostAsync(
        Guid orgId, Guid projectId, string sourceType, Guid sourceDocumentId, string sourceDocumentNumber,
        string description, decimal amount, bool isReversal, Guid actorUserId, DateTimeOffset now, CancellationToken ct)
    {
        var entry = new ProjectCostLedgerEntry(
            Guid.NewGuid(),
            orgId,
            projectId,
            null,
            ProjectBudgetCategory.Material,
            ProjectCostEntryType.Actual,
            sourceType,
            sourceDocumentId,
            sourceDocumentNumber,
            description,
            amount,
            isReversal,
            actorUserId,
            now);
        _db.ProjectCostLedgerEntries.Add(entry);

        if (amount > 0m)
        {
            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.OrganizationId == orgId && p.Id == projectId, ct);
            if (project is not null && project.BaselineBudgetTotal.HasValue)
            {
                var approvedDelta = await _db.ProjectChangeOrders.AsNoTracking()
                    .Where(c => c.OrganizationId == orgId && c.ProjectId == projectId && c.Status == ChangeOrderStatus.Approved)
                    .SumAsync(c => (decimal?)c.BudgetDelta, ct) ?? 0m;
                var currentBudget = decimal.Round(project.BaselineBudgetTotal.Value + approvedDelta, 2);

                if (currentBudget > 0m)
                {
                    var existingTotal = await _db.ProjectCostLedgerEntries.AsNoTracking()
                        .Where(e => e.OrganizationId == orgId && e.ProjectId == projectId)
                        .SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;

                    if (existingTotal + amount > currentBudget)
                    {
                        await _notifications.PublishAsync(
                            NotificationEvents.ProjectBudgetOverrun(orgId, project.BranchId, entry.Id, actorUserId, project.Id, project.Code),
                            ct);
                    }
                }
            }
        }
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

    public async Task<PagedStockLocationBalances> ListLocationBalancesAsync(
        Guid organizationId, StockLocationBalanceQuery query, CancellationToken ct = default)
    {
        var rows =
            from b in _db.StockLocationBalances.AsNoTracking()
            where b.OrganizationId == organizationId
            join w in _db.Warehouses.AsNoTracking() on b.WarehouseId equals w.Id
            join l in _db.WarehouseLocations.AsNoTracking() on b.LocationId equals l.Id
            join i in _db.Items.AsNoTracking() on b.ItemId equals i.Id
            join u in _db.Units.AsNoTracking() on b.UnitId equals u.Id
            select new { Balance = b, Warehouse = w, Location = l, Item = i, Unit = u };

        if (query.WarehouseId.HasValue) rows = rows.Where(x => x.Balance.WarehouseId == query.WarehouseId.Value);
        if (query.LocationId.HasValue) rows = rows.Where(x => x.Balance.LocationId == query.LocationId.Value);
        if (query.ItemId.HasValue) rows = rows.Where(x => x.Balance.ItemId == query.ItemId.Value);
        if (query.LotNumber is not null) rows = rows.Where(x => x.Balance.LotNumber == query.LotNumber);
        if (query.InStockOnly) rows = rows.Where(x => x.Balance.OnHand > 0);

        var total = await rows.CountAsync(ct);
        var page = await rows
            .OrderBy(x => x.Warehouse.NormalizedCode)
            .ThenByDescending(x => x.Location.IsDefault)
            .ThenBy(x => x.Location.NormalizedCode)
            .ThenBy(x => x.Item.NormalizedCode)
            .ThenBy(x => x.Balance.LotNumber)
            .ThenBy(x => x.Balance.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var items = page.Select(x => new StockLocationBalanceProjection(
            x.Balance.Id,
            new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
            new InventoryLocationRef(x.Location.Id, x.Location.Code, x.Location.Name, x.Location.IsDefault),
            new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
            x.Balance.LotNumber,
            x.Balance.OnHand,
            x.Balance.UpdatedAtUtc)).ToList();

        return new PagedStockLocationBalances(items, total, query.Page, query.PageSize);
    }

    public async Task<PagedStockSerials> ListSerialsAsync(
        Guid organizationId, StockSerialQuery query, CancellationToken ct = default)
    {
        var rows =
            from s in _db.StockSerials.AsNoTracking()
            where s.OrganizationId == organizationId
            join i in _db.Items.AsNoTracking() on s.ItemId equals i.Id
            join u in _db.Units.AsNoTracking() on i.BaseUnitId equals u.Id
            join w in _db.Warehouses.AsNoTracking() on s.WarehouseId equals w.Id into wJoin
            from w in wJoin.DefaultIfEmpty()
            join l in _db.WarehouseLocations.AsNoTracking() on s.LocationId equals l.Id into lJoin
            from l in lJoin.DefaultIfEmpty()
            select new { Serial = s, Item = i, Unit = u, Warehouse = w, Location = l };

        if (query.WarehouseId.HasValue) rows = rows.Where(x => x.Serial.WarehouseId == query.WarehouseId.Value);
        if (query.LocationId.HasValue) rows = rows.Where(x => x.Serial.LocationId == query.LocationId.Value);
        if (query.ItemId.HasValue) rows = rows.Where(x => x.Serial.ItemId == query.ItemId.Value);
        if (query.Status is not null) rows = rows.Where(x => x.Serial.Status == query.Status);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            rows = rows.Where(x => x.Serial.NormalizedSerialNumber.Contains(needle) || x.Serial.LotNumber.Contains(needle));
        }

        var total = await rows.CountAsync(ct);
        var page = await rows
            .OrderBy(x => x.Item.NormalizedCode)
            .ThenBy(x => x.Serial.NormalizedSerialNumber)
            .ThenBy(x => x.Serial.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var items = page.Select(x => new StockSerialProjection(
            x.Serial.Id,
            new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
            x.Serial.SerialNumber,
            x.Warehouse is null ? null : new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
            x.Location is null ? null : new InventoryLocationRef(x.Location.Id, x.Location.Code, x.Location.Name, x.Location.IsDefault),
            x.Serial.LotNumber,
            x.Serial.Status,
            x.Serial.LastStockDocumentId,
            x.Serial.UpdatedAtUtc)).ToList();

        return new PagedStockSerials(items, total, query.Page, query.PageSize);
    }

    private sealed class MovementRow
    {
        public StockMovement Movement { get; set; } = null!;
        public StockDocument Document { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public WarehouseLocation? Location { get; set; }
        public Item Item { get; set; } = null!;
        public UnitOfMeasure Unit { get; set; } = null!;
    }

    private IQueryable<MovementRow> MovementQueryable(Guid orgId) =>
        from m in _db.StockMovements.AsNoTracking()
        where m.OrganizationId == orgId
        join d in _db.StockDocuments.AsNoTracking() on m.StockDocumentId equals d.Id
        join w in _db.Warehouses.AsNoTracking() on m.WarehouseId equals w.Id
        join l in _db.WarehouseLocations.AsNoTracking() on m.LocationId equals l.Id into lJoin
        from l in lJoin.DefaultIfEmpty()
        join i in _db.Items.AsNoTracking() on m.ItemId equals i.Id
        join u in _db.Units.AsNoTracking() on m.UnitId equals u.Id
        select new MovementRow { Movement = m, Document = d, Warehouse = w, Location = l, Item = i, Unit = u };

    private static StockMovementProjection ToProjection(MovementRow x)
    {
        IReadOnlyList<string>? serials = null;
        if (!string.IsNullOrWhiteSpace(x.Movement.SerialNumbersJson))
        {
            serials = JsonSerializer.Deserialize<List<string>>(x.Movement.SerialNumbersJson);
        }

        return new StockMovementProjection(
            x.Movement.Id, x.Document.Id, x.Document.DocumentType, x.Document.Number,
            new InventoryWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
            new InventoryItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Unit.Code),
            x.Movement.Kind, x.Movement.QuantityDelta, x.Movement.UnitCost, x.Movement.ValueDelta, x.Movement.OnHandAfter,
            x.Movement.OccurredAtUtc, x.Movement.PostedAtUtc,
            x.Location is null ? null : new InventoryLocationRef(x.Location.Id, x.Location.Code, x.Location.Name, x.Location.IsDefault),
            x.Movement.LotNumber,
            serials);
    }

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
