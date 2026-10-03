using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Inventory;
using TanErp.Application.Production;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Production;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Persistence.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Production;

public class ProductionStore : IProductionStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly IProductionStockPort _stock;

    public ProductionStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, IProductionStockPort stock)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _stock = stock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> Fail<T>(ProductionDomainException ex) => Fail<T>(ex.Code, ex.Message);

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

    private async Task LockWorkOrderAsync(Guid orgId, Guid workOrderId, CancellationToken ct)
    {
        // Serialises every stock-affecting operation on one work order.
        await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM production.work_orders WHERE id = {workOrderId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
    }

    // ===== BOM =======================================================================================

    private sealed record ResolvedBomLines(List<BomLineInput> Lines);

    private async Task<Result<Item>> ResolveProducedItemAsync(Guid orgId, Guid itemId, CancellationToken ct)
    {
        var item = await _db.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == itemId && i.OrganizationId == orgId, ct);
        if (item is null) return Fail<Item>("RESOURCE_NOT_FOUND", "Item not found.");
        if (item.Status != ItemStatus.Active || !item.Capabilities.CanProduce || !item.Capabilities.CanStock)
        {
            return Fail<Item>("BOM_ITEM_NOT_PRODUCIBLE", $"Item '{item.Code}' is not an active, producible and stockable item.");
        }

        return Result<Item>.Success(item);
    }

    private async Task<Result<bool>> ValidateComponentsAsync(Guid orgId, Guid producedItemId, IReadOnlyList<BomLineInput> lines, CancellationToken ct)
    {
        if (lines.Any(l => l.ComponentItemId == producedItemId))
        {
            return Fail<bool>("BOM_CYCLE", "A BOM cannot use its own produced item as a component.");
        }

        var ids = lines.Select(l => l.ComponentItemId).ToList();
        var items = await _db.Items.AsNoTracking().Where(i => i.OrganizationId == orgId && ids.Contains(i.Id)).ToListAsync(ct);
        foreach (var line in lines)
        {
            var item = items.FirstOrDefault(i => i.Id == line.ComponentItemId);
            if (item is null) return Fail<bool>("RESOURCE_NOT_FOUND", $"Item '{line.ComponentItemId}' not found.");
            if (item.Status != ItemStatus.Active || !item.Capabilities.CanStock)
            {
                return Fail<bool>("BOM_COMPONENT_NOT_STOCKABLE", $"Item '{item.Code}' is not an active, stockable item.");
            }
        }

        // Cycle detection over approved BOMs: from every component, walk down its own components looking for the produced item.
        var edges = await (from l in _db.BomLines.AsNoTracking()
                           join r in _db.BomRevisions.AsNoTracking() on l.BomRevisionId equals r.Id
                           join b in _db.Boms.AsNoTracking() on r.BomId equals b.Id
                           where b.OrganizationId == orgId && r.Status == BomRevisionStatus.Approved
                           select new { Parent = b.ItemId, Child = l.ComponentItemId }).ToListAsync(ct);
        var children = edges.GroupBy(e => e.Parent).ToDictionary(g => g.Key, g => g.Select(e => e.Child).ToList());
        var visited = new HashSet<Guid>();
        var stack = new Stack<Guid>(ids);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current == producedItemId) return Fail<bool>("BOM_CYCLE", "The BOM would create a circular component structure.");
            if (!visited.Add(current)) continue;
            if (children.TryGetValue(current, out var next)) foreach (var child in next) stack.Push(child);
        }

        return Result<bool>.Success(true);
    }

    private static List<BomLine> BuildBomLines(Guid orgId, Guid revisionId, IReadOnlyList<BomLineInput> lines) =>
        lines.Select((l, i) => new BomLine(Guid.NewGuid(), orgId, revisionId, l.ComponentItemId, l.Quantity, l.ScrapPercent, i + 1)).ToList();

    public async Task<Result<BomProjection>> CreateBomAsync(
        RequestAccessContext access, BomInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "production.bom.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<BomProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetBomAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<BomProjection>.Success(replayed);
            }

            var produced = await ResolveProducedItemAsync(orgId, input.ItemId, ct);
            if (produced.IsFailure) return Result<BomProjection>.Failure(produced.Error);
            if (await _db.Boms.AnyAsync(b => b.OrganizationId == orgId && b.ItemId == input.ItemId, ct))
            {
                return Fail<BomProjection>("BOM_ALREADY_EXISTS", "This item already has a BOM; create a new revision instead.");
            }

            var components = await ValidateComponentsAsync(orgId, input.ItemId, input.Lines, ct);
            if (components.IsFailure) return Result<BomProjection>.Failure(components.Error);

            var now = _clock.UtcNow;
            var code = await MasterDataCodeAllocator.ResolveAsync(
                _numbers, orgId, DocumentTypes.Boms, null, 64, "BOM_CODE_CONFLICT",
                async (candidate, token) => await _db.Boms.AnyAsync(b => b.OrganizationId == orgId && b.NormalizedCode == candidate.Trim().ToUpperInvariant(), token), ct);
            if (code.IsFailure) return Result<BomProjection>.Failure(code.Error);

            try
            {
                var bom = new Bom(Guid.NewGuid(), orgId, input.ItemId, code.Value!, access.ActorUserId, now);
                var revision = new BomRevision(Guid.NewGuid(), orgId, bom.Id, 1, input.OutputQuantity, input.Note, access.ActorUserId, now);
                foreach (var line in BuildBomLines(orgId, revision.Id, input.Lines)) revision.AddLine(line);
                bom.AddRevision(revision);
                _db.Boms.Add(bom);
                Audit(access, "bom.created", "Bom", bom.Id, traceId, new { code = bom.Code, itemId = bom.ItemId, lineCount = input.Lines.Count }, revision.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, bom.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<BomProjection>.Success((await GetBomAsync(orgId, bom.Id, ct))!);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<BomProjection>(ex);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<BomProjection>("BOM_ALREADY_EXISTS", "This item already has a BOM; create a new revision instead.");
            }
        });
    }

    public async Task<Result<BomProjection>> CreateBomRevisionAsync(
        RequestAccessContext access, Guid bomId, BomDraftInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var bom = await _db.Boms.Include(b => b.Revisions).FirstOrDefaultAsync(b => b.Id == bomId && b.OrganizationId == orgId, ct);
            if (bom is null) return Fail<BomProjection>("RESOURCE_NOT_FOUND", "BOM not found.");
            if (bom.Revisions.Any(r => r.Status == BomRevisionStatus.Draft))
            {
                return Fail<BomProjection>("BOM_INVALID_STATE", "This BOM already has a draft revision.");
            }

            var components = await ValidateComponentsAsync(orgId, bom.ItemId, input.Lines, ct);
            if (components.IsFailure) return Result<BomProjection>.Failure(components.Error);

            var now = _clock.UtcNow;
            try
            {
                var revision = new BomRevision(Guid.NewGuid(), orgId, bom.Id, bom.Revisions.Max(r => r.RevisionNo) + 1, input.OutputQuantity, input.Note, access.ActorUserId, now);
                foreach (var line in BuildBomLines(orgId, revision.Id, input.Lines)) revision.AddLine(line);
                _db.BomRevisions.Add(revision);
                Audit(access, "bom.revision.created", "Bom", bom.Id, traceId, new { code = bom.Code, revisionNo = revision.RevisionNo }, revision.RowVersion, now);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<BomProjection>(ex);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<BomProjection>("BOM_INVALID_STATE", "A concurrent change created another revision; reload and retry.");
            }

            return Result<BomProjection>.Success((await GetBomAsync(orgId, bom.Id, ct))!);
        });
    }

    public async Task<Result<BomProjection>> UpdateBomDraftAsync(
        RequestAccessContext access, Guid bomId, Guid revisionId, Guid expectedVersion, BomDraftInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var bom = await _db.Boms.FirstOrDefaultAsync(b => b.Id == bomId && b.OrganizationId == orgId, ct);
            var revision = await _db.BomRevisions.Include(r => r.Lines).FirstOrDefaultAsync(r => r.Id == revisionId && r.BomId == bomId && r.OrganizationId == orgId, ct);
            if (bom is null || revision is null) return Fail<BomProjection>("RESOURCE_NOT_FOUND", "BOM revision not found.");
            if (revision.RowVersion != expectedVersion) return Fail<BomProjection>("BOM_VERSION_CONFLICT", "BOM revision version conflict.");

            var components = await ValidateComponentsAsync(orgId, bom.ItemId, input.Lines, ct);
            if (components.IsFailure) return Result<BomProjection>.Failure(components.Error);

            var now = _clock.UtcNow;
            try
            {
                var oldLines = revision.Lines.ToList();
                revision.EditDraft(input.OutputQuantity, input.Note);
                var newLines = BuildBomLines(orgId, revision.Id, input.Lines);
                revision.ReplaceLines(newLines);
                _db.BomLines.RemoveRange(oldLines);
                _db.BomLines.AddRange(newLines);
                Audit(access, "bom.revision.updated", "Bom", bom.Id, traceId, new { code = bom.Code, revisionNo = revision.RevisionNo }, revision.RowVersion, now);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<BomProjection>(ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<BomProjection>("BOM_VERSION_CONFLICT", "BOM revision version conflict.");
            }

            return Result<BomProjection>.Success((await GetBomAsync(orgId, bom.Id, ct))!);
        });
    }

    public async Task<Result<BomProjection>> BomActionAsync(
        RequestAccessContext access, Guid bomId, Guid revisionId, Guid expectedVersion, BomAction action, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var bom = await _db.Boms.Include(b => b.Revisions).ThenInclude(r => r.Lines).FirstOrDefaultAsync(b => b.Id == bomId && b.OrganizationId == orgId, ct);
            var revision = bom?.Revisions.FirstOrDefault(r => r.Id == revisionId);
            if (bom is null || revision is null) return Fail<BomProjection>("RESOURCE_NOT_FOUND", "BOM revision not found.");
            if (revision.RowVersion != expectedVersion) return Fail<BomProjection>("BOM_VERSION_CONFLICT", "BOM revision version conflict.");

            var now = _clock.UtcNow;
            try
            {
                if (action == BomAction.Approve)
                {
                    var components = await ValidateComponentsAsync(orgId, bom.ItemId, revision.Lines.Select(l => new BomLineInput(l.ComponentItemId, l.Quantity, l.ScrapPercent)).ToList(), ct);
                    if (components.IsFailure) return Result<BomProjection>.Failure(components.Error);

                    var current = bom.Revisions.FirstOrDefault(r => r.Status == BomRevisionStatus.Approved);
                    revision.Approve(access.ActorUserId, now);
                    if (current is not null)
                    {
                        current.Obsolete();
                        // Free the "one approved revision" slot before the new approval is written.
                        _db.Entry(revision).Property(r => r.Status).CurrentValue = BomRevisionStatus.Draft;
                        await _db.SaveChangesAsync(ct);
                        _db.Entry(revision).Property(r => r.Status).CurrentValue = BomRevisionStatus.Approved;
                    }
                }
                else
                {
                    revision.Obsolete();
                }

                Audit(access, action == BomAction.Approve ? "bom.revision.approved" : "bom.revision.obsoleted", "Bom", bom.Id, traceId, new { code = bom.Code, revisionNo = revision.RevisionNo }, revision.RowVersion, now);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<BomProjection>(ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<BomProjection>("BOM_VERSION_CONFLICT", "BOM revision version conflict.");
            }

            return Result<BomProjection>.Success((await GetBomAsync(orgId, bom.Id, ct))!);
        });
    }

    private static ProductionItemRef ItemRef(Item item, string unitCode) => new(item.Id, item.Code, item.Name.Thai, unitCode);

    private async Task<Dictionary<Guid, ProductionItemRef>> ItemRefsAsync(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct)
    {
        var list = ids.Distinct().ToList();
        var items = await _db.Items.AsNoTracking().Include(i => i.BaseUnit).Where(i => i.OrganizationId == orgId && list.Contains(i.Id)).ToListAsync(ct);
        return items.ToDictionary(i => i.Id, i => ItemRef(i, i.BaseUnit?.Code ?? string.Empty));
    }

    private async Task<Dictionary<Guid, ProductionPerson>> PeopleAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var list = ids.Distinct().ToList();
        return await _db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new ProductionPerson(u.Id, u.DisplayName, u.Email), ct);
    }

    public async Task<BomProjection?> GetBomAsync(Guid organizationId, Guid bomId, CancellationToken ct = default)
    {
        var bom = await _db.Boms.AsNoTracking().Include(b => b.Revisions).ThenInclude(r => r.Lines)
            .FirstOrDefaultAsync(b => b.Id == bomId && b.OrganizationId == organizationId, ct);
        if (bom is null) return null;

        var itemIds = bom.Revisions.SelectMany(r => r.Lines.Select(l => l.ComponentItemId)).Append(bom.ItemId);
        var items = await ItemRefsAsync(organizationId, itemIds, ct);
        var userIds = bom.Revisions.SelectMany(r => r.ApprovedByUserId.HasValue ? new[] { r.CreatedByUserId, r.ApprovedByUserId.Value } : new[] { r.CreatedByUserId });
        var people = await PeopleAsync(userIds, ct);
        ProductionPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProductionPerson(id, string.Empty, null);

        return new BomProjection(
            bom.Id, bom.Code, items[bom.ItemId], bom.CreatedAtUtc,
            bom.Revisions.OrderByDescending(r => r.RevisionNo).Select(r => new BomRevisionProjection(
                r.Id, r.RevisionNo, r.Status, r.OutputQuantity, r.Note, Person(r.CreatedByUserId), r.CreatedAtUtc,
                r.ApprovedByUserId.HasValue ? Person(r.ApprovedByUserId.Value) : null, r.ApprovedAtUtc, r.RowVersion,
                r.Lines.OrderBy(l => l.SortOrder).Select(l => new BomLineProjection(
                    l.Id, items[l.ComponentItemId], l.Quantity, l.ScrapPercent, decimal.Round(l.Quantity * (1 + l.ScrapPercent / 100m), 4))).ToList())).ToList());
    }

    public async Task<PagedBoms> ListBomsAsync(Guid organizationId, BomListQuery query, CancellationToken ct = default)
    {
        var boms = _db.Boms.AsNoTracking().Where(b => b.OrganizationId == organizationId);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            boms = boms.Where(b => b.NormalizedCode.Contains(needle)
                || _db.Items.Any(i => i.Id == b.ItemId && (i.NormalizedCode.Contains(needle) || i.Name.Thai.ToUpper().Contains(needle))));
        }

        var total = await boms.CountAsync(ct);
        var rows = await boms.Include(b => b.Revisions).OrderBy(b => b.NormalizedCode).ThenBy(b => b.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var items = await ItemRefsAsync(organizationId, rows.Select(b => b.ItemId), ct);
        return new PagedBoms(rows.Select(b =>
        {
            var latest = b.Revisions.OrderByDescending(r => r.RevisionNo).First();
            var approved = b.Revisions.FirstOrDefault(r => r.Status == BomRevisionStatus.Approved);
            return new BomListItemProjection(b.Id, b.Code, items[b.ItemId], approved?.RevisionNo, latest.RevisionNo, latest.Status, b.CreatedAtUtc);
        }).ToList(), total, query.Page, query.PageSize);
    }

    // ===== work orders ===============================================================================

    public async Task<Result<WorkOrderProjection>> CreateWorkOrderAsync(
        RequestAccessContext access, WorkOrderInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "production.work-order.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<WorkOrderProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WorkOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetWorkOrderAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<WorkOrderProjection>.Success(replayed);
            }

            var produced = await ResolveProducedItemAsync(orgId, input.ItemId, ct);
            if (produced.IsFailure) return Result<WorkOrderProjection>.Failure(produced.Error);

            var bom = await _db.Boms.AsNoTracking().Include(b => b.Revisions).ThenInclude(r => r.Lines)
                .FirstOrDefaultAsync(b => b.OrganizationId == orgId && b.ItemId == input.ItemId, ct);
            var revision = bom?.Revisions.FirstOrDefault(r => r.Status == BomRevisionStatus.Approved);
            if (bom is null || revision is null) return Fail<WorkOrderProjection>("PRODUCTION_BOM_NOT_APPROVED", "The item has no approved BOM revision.");

            var warehouse = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == input.WarehouseId && w.OrganizationId == orgId, ct);
            if (warehouse is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Warehouse not found.");
            if (warehouse.Status != WarehouseStatus.Active) return Fail<WorkOrderProjection>("INVENTORY_WAREHOUSE_INACTIVE", "The warehouse is not active.");
            if (warehouse.BranchId != access.BranchId.Value) return Fail<WorkOrderProjection>("INVENTORY_WAREHOUSE_BRANCH_MISMATCH", "The warehouse does not belong to the active branch.");

            if (input.ProjectId.HasValue)
            {
                var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.OrganizationId == orgId, ct);
                if (project is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Project not found.");
                if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
                {
                    return Fail<WorkOrderProjection>("PRODUCTION_PROJECT_NOT_ACTIVE", "Work orders cannot be raised for a completed or cancelled project.");
                }
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.WorkOrders, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<WorkOrderProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate work order document number.");
            }

            try
            {
                var order = new WorkOrder(Guid.NewGuid(), orgId, access.BranchId.Value, number, input.ItemId, revision.Id, input.WarehouseId, input.ProjectId, input.PlannedQuantity, input.Note, access.ActorUserId, now);
                var sort = 1;
                foreach (var line in revision.Lines.OrderBy(l => l.SortOrder))
                {
                    var required = decimal.Round(line.Quantity * (1 + line.ScrapPercent / 100m) * input.PlannedQuantity / revision.OutputQuantity, 4);
                    order.AddMaterial(new WorkOrderMaterial(Guid.NewGuid(), orgId, order.Id, line.ComponentItemId, required, sort++));
                }

                _db.WorkOrders.Add(order);
                Audit(access, "work-order.created", "WorkOrder", order.Id, traceId, new { number, itemId = order.ItemId, planned = order.PlannedQuantity, bomRevision = revision.RevisionNo }, order.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, order.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<WorkOrderProjection>.Success((await GetWorkOrderAsync(orgId, order.Id, ct))!);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkOrderProjection>(ex);
            }
        });
    }

    public async Task<Result<WorkOrderProjection>> WorkOrderActionAsync(
        RequestAccessContext access, Guid workOrderId, Guid expectedVersion, WorkOrderAction action, string? reason, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await LockWorkOrderAsync(orgId, workOrderId, ct);
            var order = await _db.WorkOrders.Include(o => o.Materials).FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Work order not found.");
            if (order.RowVersion != expectedVersion) return Fail<WorkOrderProjection>("PRODUCTION_VERSION_CONFLICT", "Work order version conflict.");

            var now = _clock.UtcNow;
            try
            {
                if (action == WorkOrderAction.Release) order.Release(now);
                else order.Cancel(reason ?? string.Empty, now);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkOrderProjection>(ex);
            }

            Audit(access, action == WorkOrderAction.Release ? "work-order.released" : "work-order.cancelled", "WorkOrder", order.Id, traceId, new { number = order.Number, reason }, order.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<WorkOrderProjection>("PRODUCTION_VERSION_CONFLICT", "Work order version conflict.");
            }

            return Result<WorkOrderProjection>.Success((await GetWorkOrderAsync(orgId, order.Id, ct))!);
        });
    }

    /// <summary>
    /// Shared skeleton of the stock-affecting operations: replay check, work order row lock, the operation itself
    /// (which posts through the stock port inside this transaction), then audit + idempotency record and commit.
    /// </summary>
    private async Task<Result<WorkOrderProjection>> RunStockOperationAsync(
        RequestAccessContext access, Guid workOrderId, string operation, string keyHash, string payloadHash, string auditAction, string traceId,
        Func<WorkOrder, Guid, DateTimeOffset, Task<Result<WorkOrderTransaction>>> execute, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, $"{operation}:{workOrderId}", keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WorkOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = await GetWorkOrderAsync(orgId, workOrderId, ct);
                if (replayed is not null) return Result<WorkOrderProjection>.Success(replayed);
            }

            await LockWorkOrderAsync(orgId, workOrderId, ct);
            var order = await _db.WorkOrders.Include(o => o.Materials).FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Work order not found.");

            var now = _clock.UtcNow;
            var sourceId = Guid.NewGuid();
            Result<WorkOrderTransaction> executed;
            try
            {
                executed = await execute(order, sourceId, now);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkOrderProjection>(ex);
            }

            if (executed.IsFailure) return Result<WorkOrderProjection>.Failure(executed.Error);

            _db.WorkOrderTransactions.Add(executed.Value!);
            Audit(access, auditAction, "WorkOrder", order.Id, traceId, new { number = order.Number, stockDocument = executed.Value!.StockDocumentNumber, quantity = executed.Value.Quantity, value = executed.Value.Value }, order.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, $"{operation}:{workOrderId}", keyHash, payloadHash, order.Id.ToString(), now));
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<WorkOrderProjection>("PRODUCTION_VERSION_CONFLICT", "Work order version conflict.");
            }

            return Result<WorkOrderProjection>.Success((await GetWorkOrderAsync(orgId, order.Id, ct))!);
        });
    }

    public Task<Result<WorkOrderProjection>> IssueMaterialsAsync(
        RequestAccessContext access, Guid workOrderId, IReadOnlyList<WorkOrderMaterialQuantityInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        RunStockOperationAsync(access, workOrderId, "production.work-order.issue", keyHash, payloadHash, "work-order.materials-issued", traceId, async (order, sourceId, now) =>
        {
            foreach (var line in lines) order.ValidateIssue(line.ItemId, line.Quantity);

            var posted = await _stock.IssueMaterialsAsync(
                access, order.WarehouseId, order.ProjectId, sourceId, $"Work order {order.Number}",
                lines.Select(l => new ProductionIssueLine(l.ItemId, l.Quantity)).ToList(), traceId, ct);
            if (posted.IsFailure) return Result<WorkOrderTransaction>.Failure(posted.Error);

            foreach (var posting in posted.Value!.Lines) order.RecordIssue(posting.ItemId, posting.Quantity, posting.Value, now);
            return Result<WorkOrderTransaction>.Success(new WorkOrderTransaction(
                sourceId, order.OrganizationId, order.Id, WorkOrderTransactionKind.Issue, posted.Value.DocumentId, posted.Value.DocumentNumber,
                posted.Value.Lines.Sum(l => l.Quantity), posted.Value.Lines.Sum(l => l.Value), access.ActorUserId, now));
        }, ct);

    public Task<Result<WorkOrderProjection>> ReturnMaterialsAsync(
        RequestAccessContext access, Guid workOrderId, IReadOnlyList<WorkOrderMaterialQuantityInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        RunStockOperationAsync(access, workOrderId, "production.work-order.return", keyHash, payloadHash, "work-order.materials-returned", traceId, async (order, sourceId, now) =>
        {
            var valued = new List<ProductionReturnLine>();
            foreach (var line in lines)
            {
                order.ValidateReturn(line.ItemId, line.Quantity);
                valued.Add(new ProductionReturnLine(line.ItemId, line.Quantity, order.ReturnValueFor(line.ItemId, line.Quantity)));
            }

            var posted = await _stock.ReturnMaterialsAsync(access, order.WarehouseId, order.ProjectId, sourceId, $"Work order {order.Number}", valued, traceId, ct);
            if (posted.IsFailure) return Result<WorkOrderTransaction>.Failure(posted.Error);

            foreach (var line in valued) order.RecordReturn(line.ItemId, line.Quantity, line.Value, now);
            return Result<WorkOrderTransaction>.Success(new WorkOrderTransaction(
                sourceId, order.OrganizationId, order.Id, WorkOrderTransactionKind.Return, posted.Value!.DocumentId, posted.Value.DocumentNumber,
                valued.Sum(l => l.Quantity), valued.Sum(l => l.Value), access.ActorUserId, now));
        }, ct);

    public Task<Result<WorkOrderProjection>> CompleteAsync(
        RequestAccessContext access, Guid workOrderId, decimal quantity, string keyHash, string payloadHash, string traceId, CancellationToken ct = default) =>
        RunStockOperationAsync(access, workOrderId, "production.work-order.complete", keyHash, payloadHash, "work-order.completed", traceId, async (order, sourceId, now) =>
        {
            var rounded = decimal.Round(quantity, 4);
            var value = order.Complete(rounded, now);
            var posted = await _stock.ReceiveOutputAsync(access, order.WarehouseId, order.ProjectId, sourceId, order.ItemId, rounded, value, traceId, ct);
            if (posted.IsFailure) return Result<WorkOrderTransaction>.Failure(posted.Error);

            return Result<WorkOrderTransaction>.Success(new WorkOrderTransaction(
                sourceId, order.OrganizationId, order.Id, WorkOrderTransactionKind.Completion, posted.Value!.DocumentId, posted.Value.DocumentNumber,
                rounded, value, access.ActorUserId, now));
        }, ct);

    public async Task<WorkOrderProjection?> GetWorkOrderAsync(Guid organizationId, Guid workOrderId, CancellationToken ct = default)
    {
        var order = await _db.WorkOrders.AsNoTracking().Include(o => o.Materials)
            .FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == organizationId, ct);
        if (order is null) return null;

        var revision = await _db.BomRevisions.AsNoTracking().FirstAsync(r => r.Id == order.BomRevisionId, ct);
        var bom = await _db.Boms.AsNoTracking().FirstAsync(b => b.Id == revision.BomId, ct);
        var warehouse = await _db.Warehouses.AsNoTracking().FirstAsync(w => w.Id == order.WarehouseId, ct);
        var project = order.ProjectId.HasValue ? await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == order.ProjectId.Value, ct) : null;
        var transactions = await _db.WorkOrderTransactions.AsNoTracking().Where(t => t.WorkOrderId == order.Id).OrderBy(t => t.CreatedAtUtc).ThenBy(t => t.Id).ToListAsync(ct);
        var items = await ItemRefsAsync(organizationId, order.Materials.Select(m => m.ItemId).Append(order.ItemId), ct);
        var people = await PeopleAsync(transactions.Select(t => t.ActorUserId).Append(order.CreatedByUserId), ct);
        ProductionPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProductionPerson(id, string.Empty, null);

        return new WorkOrderProjection(
            order.Id, order.BranchId, order.Number, order.Status, items[order.ItemId], revision.Id, bom.Code, revision.RevisionNo,
            new WorkOrderWarehouseRef(warehouse.Id, warehouse.Code, warehouse.Name),
            project is null ? null : new WorkOrderRef(project.Id, project.Code, project.Name),
            order.PlannedQuantity, order.CompletedQuantity, order.CostAllocated, order.Note, order.CancelReason,
            Person(order.CreatedByUserId), order.CreatedAtUtc, order.RowVersion,
            order.Materials.OrderBy(m => m.SortOrder).Select(m => new WorkOrderMaterialProjection(
                m.Id, items[m.ItemId], m.RequiredQuantity, m.IssuedQuantity, m.ReturnedQuantity, m.NetIssuedQuantity,
                Math.Max(0m, m.RequiredQuantity - m.NetIssuedQuantity), m.IssuedValue, m.ReturnedValue)).ToList(),
            transactions.Select(t => new WorkOrderTransactionProjection(t.Id, t.Kind, t.StockDocumentId, t.StockDocumentNumber, t.Quantity, t.Value, Person(t.ActorUserId), t.CreatedAtUtc)).ToList());
    }

    public async Task<PagedWorkOrders> ListWorkOrdersAsync(Guid organizationId, WorkOrderListQuery query, CancellationToken ct = default)
    {
        var orders = _db.WorkOrders.AsNoTracking().Where(o => o.OrganizationId == organizationId);
        if (query.Status is not null) orders = orders.Where(o => o.Status == query.Status);
        if (query.ProjectId.HasValue) orders = orders.Where(o => o.ProjectId == query.ProjectId);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            orders = orders.Where(o => o.Number.ToUpper().Contains(needle)
                || _db.Items.Any(i => i.Id == o.ItemId && (i.NormalizedCode.Contains(needle) || i.Name.Thai.ToUpper().Contains(needle))));
        }

        var total = await orders.CountAsync(ct);
        var rows = await orders.OrderByDescending(o => o.CreatedAtUtc).ThenBy(o => o.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var items = await ItemRefsAsync(organizationId, rows.Select(o => o.ItemId), ct);
        var projectIds = rows.Where(o => o.ProjectId.HasValue).Select(o => o.ProjectId!.Value).Distinct().ToList();
        var projects = await _db.Projects.AsNoTracking().Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Code, ct);
        return new PagedWorkOrders(rows.Select(o => new WorkOrderListItemProjection(
            o.Id, o.Number, o.Status, items[o.ItemId].Code, items[o.ItemId].NameTh, o.PlannedQuantity, o.CompletedQuantity,
            o.ProjectId.HasValue && projects.TryGetValue(o.ProjectId.Value, out var code) ? code : null, o.CreatedAtUtc)).ToList(), total, query.Page, query.PageSize);
    }
}
