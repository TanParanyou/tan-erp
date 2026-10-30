using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Mrp;
using TanErp.Application.Notifications;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Items;
using TanErp.Domain.Mrp;
using TanErp.Domain.Procurement;
using TanErp.Domain.Production;

namespace TanErp.Infrastructure.Persistence.Mrp;

public class MrpStore : IMrpStore
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly INotificationPublisher _notifications;

    public MrpStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _notifications = notifications;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private void Audit(RequestAccessContext access, string action, Guid resourceId, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "MrpRun", resourceId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
            r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    /// <summary>
    /// Reads every planning input once. Called inside a REPEATABLE READ transaction so stock, orders and BOMs all come from
    /// the same point in time even if the warehouse keeps posting while the run is being built.
    /// </summary>
    private async Task<Result<MrpSnapshot>> BuildSnapshotAsync(Guid orgId, Guid? branchId, MrpRunInput input, CancellationToken ct)
    {
        var asOf = input.AsOfDate;
        var parameters = new MrpParameters(asOf, input.PurchaseLeadTimeDays, input.ProductionLeadTimeDays);
        var demands = new List<MrpDemandInput>();
        var supplies = new List<MrpSupplyInput>();

        var demandedItemIds = input.Demands.Select(d => d.ItemId).Distinct().ToList();
        var demandedItems = demandedItemIds.Count == 0
            ? new Dictionary<Guid, Item>()
            : await _db.Items.AsNoTracking()
                .Where(i => i.OrganizationId == orgId && demandedItemIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id, ct);

        var missing = demandedItemIds.FirstOrDefault(id => !demandedItems.ContainsKey(id));
        if (missing != Guid.Empty) return Fail<MrpSnapshot>("RESOURCE_NOT_FOUND", $"Item '{missing}' not found.");
        var inactive = demandedItems.Values.FirstOrDefault(i => i.Status != ItemStatus.Active);
        if (inactive is not null) return Fail<MrpSnapshot>("MRP_DEMAND_INVALID", $"Item '{inactive.Code}' is not active.");

        foreach (var d in input.Demands)
        {
            var item = demandedItems[d.ItemId];
            var qty = d.Quantity;

            if (d.UnitId.HasValue && d.UnitId.Value != Guid.Empty && d.UnitId.Value != item.BaseUnitId)
            {
                var fromUnitId = d.UnitId.Value;
                var toUnitId = item.BaseUnitId;

                var itemConv = await _db.ItemUnitConversions.AsNoTracking()
                    .Where(c => c.OrganizationId == orgId
                        && c.ItemId == item.Id
                        && c.FromUnitId == fromUnitId
                        && c.ToUnitId == toUnitId
                        && c.Status == ItemStatus.Active
                        && c.EffectiveFrom <= asOf
                        && (!c.EffectiveTo.HasValue || c.EffectiveTo.Value >= asOf))
                    .OrderByDescending(c => c.EffectiveFrom)
                    .FirstOrDefaultAsync(ct);

                decimal? factor = itemConv?.Factor;
                if (!factor.HasValue)
                {
                    var globalConv = await _db.UnitConversions.AsNoTracking()
                        .Where(c => c.OrganizationId == orgId
                            && c.FromUnitId == fromUnitId
                            && c.ToUnitId == toUnitId
                            && c.Status == ItemStatus.Active
                            && c.EffectiveFrom <= asOf
                            && (!c.EffectiveTo.HasValue || c.EffectiveTo.Value >= asOf))
                        .OrderByDescending(c => c.EffectiveFrom)
                        .FirstOrDefaultAsync(ct);
                    factor = globalConv?.Factor;
                }

                if (!factor.HasValue)
                {
                    return Fail<MrpSnapshot>("MRP_UNIT_CONVERSION_NOT_FOUND", "No active unit conversion to base unit was found for the demand item.");
                }

                qty = decimal.Round(d.Quantity * factor.Value, 4, MidpointRounding.AwayFromZero);
            }

            demands.Add(new MrpDemandInput(d.ItemId, qty, d.NeedBy, MrpDemandSource.Manual, string.IsNullOrWhiteSpace(d.Reference) ? "manual" : d.Reference.Trim()));
        }

        var openOrders = await _db.WorkOrders.AsNoTracking().Include(o => o.Materials)
            .Where(o => o.OrganizationId == orgId && (o.Status == WorkOrderStatus.Released || o.Status == WorkOrderStatus.InProgress))
            .OrderBy(o => o.Number).ToListAsync(ct);
        foreach (var order in openOrders)
        {
            var outputRemaining = order.PlannedQuantity - order.CompletedQuantity;
            if (outputRemaining > 0) supplies.Add(new MrpSupplyInput(order.ItemId, outputRemaining, asOf.AddDays(input.ProductionLeadTimeDays), "work_order", order.Number));

            if (!input.IncludeOpenWorkOrders) continue;
            foreach (var material in order.Materials)
            {
                // Materials already completed into output are consumed; the rest still has to be issued.
                var consumed = order.PlannedQuantity == 0 ? 0m : material.RequiredQuantity * order.CompletedQuantity / order.PlannedQuantity;
                var stillNeeded = material.RequiredQuantity - Math.Max(material.NetIssuedQuantity, consumed);
                if (stillNeeded > 0) demands.Add(new MrpDemandInput(material.ItemId, decimal.Round(stillNeeded, 4), asOf, MrpDemandSource.WorkOrder, order.Number));
            }
        }

        var openPoLines = await (from l in _db.PurchaseOrderLines.AsNoTracking()
                                 join o in _db.PurchaseOrders.AsNoTracking() on l.PurchaseOrderId equals o.Id
                                 where o.OrganizationId == orgId && (o.Status == PurchaseOrderStatus.Approved || o.Status == PurchaseOrderStatus.PartiallyReceived)
                                 orderby o.Number, l.LineNo
                                 select new { l.ItemId, l.Quantity, l.ReceivedQuantity, o.Number, o.ExpectedDeliveryDate }).ToListAsync(ct);
        foreach (var line in openPoLines)
        {
            var remaining = line.Quantity - line.ReceivedQuantity;
            if (remaining > 0) supplies.Add(new MrpSupplyInput(line.ItemId, remaining, line.ExpectedDeliveryDate ?? asOf.AddDays(input.PurchaseLeadTimeDays), "purchase_order", line.Number));
        }

        var boms = await _db.Boms.AsNoTracking().Include(b => b.Revisions).ThenInclude(r => r.Lines)
            .Where(b => b.OrganizationId == orgId).OrderBy(b => b.Code).ToListAsync(ct);
        var bomInputs = new List<MrpBomInput>();
        foreach (var bom in boms)
        {
            var approved = bom.Revisions.FirstOrDefault(r => r.Status == BomRevisionStatus.Approved);
            if (approved is null) continue;
            bomInputs.Add(new MrpBomInput(bom.ItemId, bom.Code, approved.RevisionNo, approved.OutputQuantity,
                approved.Lines.OrderBy(l => l.SortOrder).Select(l => new MrpBomComponentInput(l.ComponentItemId, l.Quantity, l.ScrapPercent)).ToList()));
        }

        var stockRows = await _db.StockBalances.AsNoTracking().Where(b => b.OrganizationId == orgId).ToListAsync(ct);
        var stock = stockRows.GroupBy(b => b.ItemId).Select(g => new MrpStockInput(g.Key, g.Sum(b => b.OnHand - b.Reserved))).OrderBy(s => s.ItemId).ToList();

        var itemIds = new HashSet<Guid>(demands.Select(d => d.ItemId));
        foreach (var bom in bomInputs) { itemIds.Add(bom.ItemId); foreach (var c in bom.Components) itemIds.Add(c.ItemId); }
        foreach (var s in supplies) itemIds.Add(s.ItemId);

        var planningParams = await _db.MrpItemPlanningParameters.AsNoTracking()
            .Where(p => p.OrganizationId == orgId && itemIds.Contains(p.ItemId))
            .ToListAsync(ct);
        var paramsByItem = planningParams.GroupBy(p => p.ItemId).ToDictionary(g => g.Key, g => g.ToList());

        var items = await _db.Items.AsNoTracking().Where(i => i.OrganizationId == orgId && itemIds.Contains(i.Id)).ToListAsync(ct);
        var itemInputs = items.OrderBy(i => i.Id).Select(i =>
        {
            if (!paramsByItem.TryGetValue(i.Id, out var rows) || rows.Count == 0)
            {
                return new MrpItemInput(i.Id, i.Code, i.Capabilities.CanPurchase);
            }

            var orgRow = rows.FirstOrDefault(r => !r.WarehouseId.HasValue);
            var whRows = rows.Where(r => r.WarehouseId.HasValue).ToList();
            var safetyStock = whRows.Count > 0 ? whRows.Sum(r => r.SafetyStock) : (orgRow?.SafetyStock ?? 0m);
            var primary = orgRow ?? whRows.OrderBy(r => r.WarehouseId).FirstOrDefault();

            return new MrpItemInput(
                i.Id,
                i.Code,
                i.Capabilities.CanPurchase,
                safetyStock,
                primary?.MinOrderQuantity ?? 0m,
                primary?.LotMultiple ?? 0m,
                primary?.LeadTimeDays);
        }).ToList();

        var holidays = await _db.WorkingCalendarHolidays.AsNoTracking()
            .Where(h => h.OrganizationId == orgId && (!h.BranchId.HasValue || h.BranchId == branchId))
            .Select(h => h.HolidayDate)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(ct);

        return Result<MrpSnapshot>.Success(new MrpSnapshot(
            parameters,
            itemInputs,
            bomInputs,
            demands,
            supplies,
            stock,
            holidays.Count > 0 ? holidays : null));
    }

    public async Task<Result<MrpRunProjection>> CreateRunAsync(
        RequestAccessContext access, MrpRunInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "mrp.run.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<MrpRunProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<MrpRunProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetRunAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<MrpRunProjection>.Success(replayed);
            }

            var snapshot = await BuildSnapshotAsync(orgId, access.BranchId, input, ct);
            if (snapshot.IsFailure) return Result<MrpRunProjection>.Failure(snapshot.Error);

            MrpPlan plan;
            try
            {
                plan = MrpEngine.Plan(snapshot.Value!);
            }
            catch (MrpDomainException ex)
            {
                return Fail<MrpRunProjection>(ex.Code, ex.Message);
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.MrpRuns, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<MrpRunProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate planning run document number.");
            }

            var run = new MrpRun(Guid.NewGuid(), orgId, access.BranchId.Value, number, snapshot.Value!.Parameters, plan.InputHash, JsonSerializer.Serialize(snapshot.Value, Json), access.ActorUserId, now);
            var line = 1;
            foreach (var order in plan.Orders)
            {
                var reasons = order.Reasons.Select(r => new MrpReasonProjection(r.SourceType, r.SourceRef, r.Quantity, r.NeedBy)).ToList();
                run.AddRecommendation(new MrpRecommendation(Guid.NewGuid(), orgId, run.Id, line++, order, JsonSerializer.Serialize(reasons, Json)));
            }

            _db.MrpRuns.Add(run);
            Audit(access, "mrp.run.created", run.Id, traceId, new { number, inputHash = plan.InputHash, recommendations = plan.Orders.Count }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, run.Id.ToString(), now));
            if (plan.Orders.Count > 0)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.MrpRunCreated(orgId, access.BranchId.Value, run.Id, access.ActorUserId, number),
                    ct);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<MrpRunProjection>.Success((await GetRunAsync(orgId, run.Id, ct))!);
        });
    }

    public async Task<MrpRunProjection?> GetRunAsync(Guid organizationId, Guid runId, CancellationToken ct = default)
    {
        var run = await _db.MrpRuns.AsNoTracking().Include(r => r.Recommendations)
            .FirstOrDefaultAsync(r => r.Id == runId && r.OrganizationId == organizationId, ct);
        if (run is null) return null;

        var itemIds = run.Recommendations.Select(r => r.ItemId).Distinct().ToList();
        var items = await _db.Items.AsNoTracking().Include(i => i.BaseUnit).Where(i => i.OrganizationId == organizationId && itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => new MrpItemRef(i.Id, i.Code, i.Name.Thai, i.BaseUnit != null ? i.BaseUnit.Code : string.Empty), ct);
        var userIds = run.Recommendations.Where(r => r.DecidedByUserId.HasValue).Select(r => r.DecidedByUserId!.Value).Append(run.CreatedByUserId).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new MrpPerson(u.Id, u.DisplayName, u.Email), ct);
        MrpPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new MrpPerson(id, string.Empty, null);

        var snapshot = JsonSerializer.Deserialize<MrpSnapshot>(run.SnapshotJson, Json)!;
        return new MrpRunProjection(
            run.Id, run.Number, run.AsOfDate, run.PurchaseLeadTimeDays, run.ProductionLeadTimeDays, run.InputHash,
            new MrpSnapshotSummary(snapshot.Demands.Count, snapshot.Supplies.Count, snapshot.Boms.Count, snapshot.Stock.Count),
            Person(run.CreatedByUserId), run.CreatedAtUtc,
            run.Recommendations.OrderBy(r => r.LineNo).Select(r => new MrpRecommendationProjection(
                r.Id, r.LineNo, items[r.ItemId], r.Action, r.Quantity, r.NeedBy, r.OrderBy, r.Level, r.GrossRequirement, r.StockUsed, r.ScheduledReceiptsUsed,
                JsonSerializer.Deserialize<List<MrpReasonProjection>>(r.ReasonsJson, Json)!, r.Status,
                r.DecidedByUserId.HasValue ? Person(r.DecidedByUserId.Value) : null, r.DecidedAtUtc,
                r.ConvertedId.HasValue ? new MrpConvertedProjection(r.ConvertedType ?? string.Empty, r.ConvertedId.Value, r.ConvertedNumber ?? string.Empty) : null,
                r.RowVersion)).ToList());
    }

    public async Task<PagedMrpRuns> ListRunsAsync(Guid organizationId, MrpRunListQuery query, CancellationToken ct = default)
    {
        var runs = _db.MrpRuns.AsNoTracking().Where(r => r.OrganizationId == organizationId);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            runs = runs.Where(r => r.Number.ToUpper().Contains(needle));
        }

        var total = await runs.CountAsync(ct);
        var rows = await runs.OrderByDescending(r => r.CreatedAtUtc).ThenBy(r => r.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(r => new MrpRunListItemProjection(
                r.Id, r.Number, r.AsOfDate, r.Recommendations.Count,
                r.Recommendations.Count(x => x.Action == MrpAction.Shortage),
                r.Recommendations.Count(x => x.Status == MrpRecommendationStatus.Proposed || x.Status == MrpRecommendationStatus.Approved),
                r.CreatedAtUtc)).ToListAsync(ct);
        return new PagedMrpRuns(rows, total, query.Page, query.PageSize);
    }

    public async Task<Result<MrpRunProjection>> DecideAsync(
        RequestAccessContext access, Guid runId, Guid recommendationId, Guid expectedVersion, bool approve, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var run = await _db.MrpRuns.FirstOrDefaultAsync(r => r.Id == runId && r.OrganizationId == orgId, ct);
        var rec = await _db.MrpRecommendations.FirstOrDefaultAsync(r => r.Id == recommendationId && r.RunId == runId && r.OrganizationId == orgId, ct);
        if (run is null || rec is null) return Fail<MrpRunProjection>("RESOURCE_NOT_FOUND", "Recommendation not found.");
        if (rec.RowVersion != expectedVersion) return Fail<MrpRunProjection>("MRP_VERSION_CONFLICT", "Recommendation version conflict.");

        var now = _clock.UtcNow;
        try
        {
            rec.Decide(approve, access.ActorUserId, run.CreatedByUserId, now);
        }
        catch (MrpDomainException ex)
        {
            return Fail<MrpRunProjection>(ex.Code, ex.Message);
        }

        Audit(access, approve ? "mrp.recommendation.approved" : "mrp.recommendation.rejected", run.Id, traceId, new { run = run.Number, line = rec.LineNo }, now);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<MrpRunProjection>("MRP_VERSION_CONFLICT", "Recommendation version conflict.");
        }

        return Result<MrpRunProjection>.Success((await GetRunAsync(orgId, runId, ct))!);
    }

    public async Task<MrpConvertTarget?> GetConvertTargetAsync(Guid organizationId, Guid runId, Guid recommendationId, CancellationToken ct = default) =>
        await (from r in _db.MrpRecommendations.AsNoTracking()
               join run in _db.MrpRuns.AsNoTracking() on r.RunId equals run.Id
               where r.Id == recommendationId && r.RunId == runId && r.OrganizationId == organizationId
               select new MrpConvertTarget(r.Id, run.Id, run.Number, r.LineNo, r.ItemId, r.Action, r.Quantity, r.NeedBy, r.Status, r.RowVersion)).FirstOrDefaultAsync(ct);

    public async Task<Result<MrpRunProjection>> MarkConvertedAsync(
        RequestAccessContext access, Guid runId, Guid recommendationId, Guid expectedVersion, string type, Guid targetId, string targetNumber, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var rec = await _db.MrpRecommendations.FirstOrDefaultAsync(r => r.Id == recommendationId && r.RunId == runId && r.OrganizationId == orgId, ct);
        if (rec is null) return Fail<MrpRunProjection>("RESOURCE_NOT_FOUND", "Recommendation not found.");
        if (rec.RowVersion != expectedVersion) return Fail<MrpRunProjection>("MRP_VERSION_CONFLICT", "Recommendation version conflict.");

        try
        {
            rec.MarkConverted(type, targetId, targetNumber);
        }
        catch (MrpDomainException ex)
        {
            return Fail<MrpRunProjection>(ex.Code, ex.Message);
        }

        Audit(access, "mrp.recommendation.converted", runId, traceId, new { line = rec.LineNo, type, targetNumber }, _clock.UtcNow);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<MrpRunProjection>("MRP_VERSION_CONFLICT", "Recommendation version conflict.");
        }

        return Result<MrpRunProjection>.Success((await GetRunAsync(orgId, runId, ct))!);
    }

    // ===== Planning Parameters & Working Calendar Holidays (G-14) ======================================

    public async Task<PagedMrpPlanningParameters> ListPlanningParametersAsync(
        Guid organizationId, MrpPlanningParameterListQuery query, CancellationToken ct = default)
    {
        var baseQuery = from p in _db.MrpItemPlanningParameters.AsNoTracking()
                        join i in _db.Items.AsNoTracking().Include(x => x.BaseUnit) on p.ItemId equals i.Id
                        join w in _db.Warehouses.AsNoTracking() on p.WarehouseId equals w.Id into whJoin
                        from wh in whJoin.DefaultIfEmpty()
                        where p.OrganizationId == organizationId
                        select new { Parameter = p, Item = i, Warehouse = wh };

        if (query.ItemId.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.Parameter.ItemId == query.ItemId.Value);
        }

        if (query.WarehouseId.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.Parameter.WarehouseId == query.WarehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var needle = query.Search.ToUpperInvariant();
            baseQuery = baseQuery.Where(x => x.Item.Code.ToUpper().Contains(needle) || x.Item.Name.Thai.ToUpper().Contains(needle));
        }

        var total = await baseQuery.CountAsync(ct);
        var rows = await baseQuery
            .OrderBy(x => x.Item.Code)
            .ThenBy(x => x.Parameter.WarehouseId)
            .ThenBy(x => x.Parameter.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var items = rows.Select(x => new MrpPlanningParameterProjection(
            x.Parameter.Id,
            new MrpItemRef(x.Item.Id, x.Item.Code, x.Item.Name.Thai, x.Item.BaseUnit != null ? x.Item.BaseUnit.Code : string.Empty),
            x.Warehouse is null ? null : new MrpWarehouseRef(x.Warehouse.Id, x.Warehouse.Code, x.Warehouse.Name),
            x.Parameter.SafetyStock,
            x.Parameter.MinOrderQuantity,
            x.Parameter.LotMultiple,
            x.Parameter.LeadTimeDays,
            x.Parameter.UpdatedAtUtc,
            x.Parameter.RowVersion)).ToList();

        return new PagedMrpPlanningParameters(items, total, query.Page, query.PageSize);
    }

    public async Task<Result<MrpPlanningParameterProjection>> UpsertPlanningParameterAsync(
        RequestAccessContext access, MrpPlanningParameterInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var item = await _db.Items.AsNoTracking().Include(x => x.BaseUnit)
            .FirstOrDefaultAsync(i => i.Id == input.ItemId && i.OrganizationId == orgId, ct);
        if (item is null) return Fail<MrpPlanningParameterProjection>("RESOURCE_NOT_FOUND", "Item not found.");

        var warehouseId = input.WarehouseId.HasValue && input.WarehouseId.Value != Guid.Empty ? input.WarehouseId : null;
        TanErp.Domain.Inventory.Warehouse? warehouse = null;
        if (warehouseId.HasValue)
        {
            warehouse = await _db.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == warehouseId.Value && w.OrganizationId == orgId, ct);
            if (warehouse is null) return Fail<MrpPlanningParameterProjection>("RESOURCE_NOT_FOUND", "Warehouse not found.");
        }

        var existing = await _db.MrpItemPlanningParameters
            .FirstOrDefaultAsync(p => p.OrganizationId == orgId && p.ItemId == input.ItemId && p.WarehouseId == warehouseId, ct);

        var now = _clock.UtcNow;
        MrpItemPlanningParameter entity;
        try
        {
            if (existing is null)
            {
                entity = new MrpItemPlanningParameter(
                    Guid.NewGuid(),
                    orgId,
                    input.ItemId,
                    warehouseId,
                    input.SafetyStock,
                    input.MinOrderQuantity,
                    input.LotMultiple,
                    input.LeadTimeDays,
                    access.ActorUserId,
                    now);
                _db.MrpItemPlanningParameters.Add(entity);
            }
            else
            {
                existing.Update(input.SafetyStock, input.MinOrderQuantity, input.LotMultiple, input.LeadTimeDays, access.ActorUserId, now);
                entity = existing;
            }
        }
        catch (MrpDomainException ex)
        {
            return Fail<MrpPlanningParameterProjection>(ex.Code, ex.Message);
        }

        Audit(access, "mrp.planning_parameter.upserted", entity.Id, traceId,
            new { itemId = entity.ItemId, warehouseId = entity.WarehouseId, safetyStock = entity.SafetyStock, minOrderQuantity = entity.MinOrderQuantity, lotMultiple = entity.LotMultiple, leadTimeDays = entity.LeadTimeDays }, now);
        await _db.SaveChangesAsync(ct);

        return Result<MrpPlanningParameterProjection>.Success(new MrpPlanningParameterProjection(
            entity.Id,
            new MrpItemRef(item.Id, item.Code, item.Name.Thai, item.BaseUnit != null ? item.BaseUnit.Code : string.Empty),
            warehouse is null ? null : new MrpWarehouseRef(warehouse.Id, warehouse.Code, warehouse.Name),
            entity.SafetyStock,
            entity.MinOrderQuantity,
            entity.LotMultiple,
            entity.LeadTimeDays,
            entity.UpdatedAtUtc,
            entity.RowVersion));
    }

    public async Task<Result<bool>> DeletePlanningParameterAsync(
        RequestAccessContext access, Guid id, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var entity = await _db.MrpItemPlanningParameters.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId, ct);
        if (entity is null) return Fail<bool>("RESOURCE_NOT_FOUND", "Planning parameter not found.");

        _db.MrpItemPlanningParameters.Remove(entity);
        Audit(access, "mrp.planning_parameter.deleted", entity.Id, traceId, new { itemId = entity.ItemId, warehouseId = entity.WarehouseId }, _clock.UtcNow);
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task<IReadOnlyList<WorkingCalendarHolidayProjection>> ListHolidaysAsync(
        Guid organizationId, Guid? branchId, DateOnly? fromDate, DateOnly? toDate, CancellationToken ct = default)
    {
        var query = _db.WorkingCalendarHolidays.AsNoTracking().Where(h => h.OrganizationId == organizationId);
        if (branchId.HasValue)
        {
            query = query.Where(h => !h.BranchId.HasValue || h.BranchId == branchId.Value);
        }
        if (fromDate.HasValue)
        {
            query = query.Where(h => h.HolidayDate >= fromDate.Value);
        }
        if (toDate.HasValue)
        {
            query = query.Where(h => h.HolidayDate <= toDate.Value);
        }

        var rows = await query.OrderBy(h => h.HolidayDate).ThenBy(h => h.Id).ToListAsync(ct);
        return rows.Select(h => new WorkingCalendarHolidayProjection(h.Id, h.BranchId, h.HolidayDate, h.Name, h.CreatedAtUtc)).ToList();
    }

    public async Task<Result<WorkingCalendarHolidayProjection>> CreateHolidayAsync(
        RequestAccessContext access, WorkingCalendarHolidayInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "mrp.holiday.create";
        var orgId = access.OrganizationId;

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WorkingCalendarHolidayProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                if (Guid.TryParse(replay.ResourceId, out var replayId))
                {
                    var replayed = await _db.WorkingCalendarHolidays.AsNoTracking().FirstOrDefaultAsync(h => h.Id == replayId && h.OrganizationId == orgId, ct);
                    if (replayed is not null)
                    {
                        return Result<WorkingCalendarHolidayProjection>.Success(
                            new WorkingCalendarHolidayProjection(replayed.Id, replayed.BranchId, replayed.HolidayDate, replayed.Name, replayed.CreatedAtUtc));
                    }
                }
            }

            var branchId = input.BranchId.HasValue && input.BranchId.Value != Guid.Empty ? input.BranchId : null;
            if (branchId.HasValue)
            {
                var branchExists = await _db.Branches.AsNoTracking().AnyAsync(b => b.Id == branchId.Value && b.OrganizationId == orgId, ct);
                if (!branchExists) return Fail<WorkingCalendarHolidayProjection>("RESOURCE_NOT_FOUND", "Branch not found.");
            }

            var duplicate = await _db.WorkingCalendarHolidays.AsNoTracking()
                .AnyAsync(h => h.OrganizationId == orgId && h.BranchId == branchId && h.HolidayDate == input.HolidayDate, ct);
            if (duplicate)
            {
                return Fail<WorkingCalendarHolidayProjection>("MRP_HOLIDAY_ALREADY_EXISTS", "A working calendar holiday already exists on this date.");
            }

            var now = _clock.UtcNow;
            WorkingCalendarHoliday holiday;
            try
            {
                holiday = new WorkingCalendarHoliday(Guid.NewGuid(), orgId, branchId, input.HolidayDate, input.Name, access.ActorUserId, now);
            }
            catch (MrpDomainException ex)
            {
                return Fail<WorkingCalendarHolidayProjection>(ex.Code, ex.Message);
            }

            _db.WorkingCalendarHolidays.Add(holiday);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, holiday.Id.ToString(), now));
            Audit(access, "mrp.holiday.created", holiday.Id, traceId, new { branchId = holiday.BranchId, holidayDate = holiday.HolidayDate, name = holiday.Name }, now);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<WorkingCalendarHolidayProjection>.Success(
                new WorkingCalendarHolidayProjection(holiday.Id, holiday.BranchId, holiday.HolidayDate, holiday.Name, holiday.CreatedAtUtc));
        });
    }

    public async Task<Result<bool>> DeleteHolidayAsync(
        RequestAccessContext access, Guid id, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var entity = await _db.WorkingCalendarHolidays.FirstOrDefaultAsync(h => h.Id == id && h.OrganizationId == orgId, ct);
        if (entity is null) return Fail<bool>("RESOURCE_NOT_FOUND", "Holiday not found.");

        _db.WorkingCalendarHolidays.Remove(entity);
        Audit(access, "mrp.holiday.deleted", entity.Id, traceId, new { holidayDate = entity.HolidayDate, name = entity.Name }, _clock.UtcNow);
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
