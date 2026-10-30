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
using TanErp.Domain.Procurement;
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
        // Serialises every stock-affecting or cost-affecting operation on one work order.
        await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM production.work_orders WHERE id = {workOrderId} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
    }

    // ===== Work Centers ==============================================================================

    private static WorkCenterProjection ToProjection(WorkCenter wc) => new(
        wc.Id,
        wc.BranchId,
        wc.Code,
        wc.Name,
        wc.DailyCapacityHours,
        wc.HourlyRate,
        wc.Status,
        wc.RowVersion,
        wc.CreatedAtUtc,
        wc.UpdatedAtUtc);

    private static WorkCenterRef ToRef(WorkCenter wc) => new(
        wc.Id,
        wc.Code,
        wc.Name,
        wc.DailyCapacityHours,
        wc.HourlyRate);

    public async Task<Result<WorkCenterProjection>> CreateWorkCenterAsync(
        RequestAccessContext access, WorkCenterInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "production.work-center.create";
        var orgId = access.OrganizationId;
        var branchId = input.BranchId ?? access.BranchId;
        if (!branchId.HasValue || branchId.Value == Guid.Empty)
        {
            return Fail<WorkCenterProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");
        }

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WorkCenterProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetWorkCenterAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<WorkCenterProjection>.Success(replayed);
            }

            var branchExists = await _db.Branches.AsNoTracking().AnyAsync(b => b.Id == branchId.Value && b.OrganizationId == orgId, ct);
            if (!branchExists) return Fail<WorkCenterProjection>("RESOURCE_NOT_FOUND", "Branch not found.");

            var requestedCode = string.IsNullOrWhiteSpace(input.Code) ? null : input.Code.Trim();
            var codeResult = await MasterDataCodeAllocator.ResolveAsync(
                _numbers, orgId, DocumentTypes.WorkCenters, requestedCode, 64, "PRODUCTION_WORK_CENTER_CODE_CONFLICT",
                async (candidate, token) => await _db.WorkCenters.AnyAsync(w => w.OrganizationId == orgId && w.NormalizedCode == candidate.Trim().ToUpperInvariant(), token), ct);
            if (codeResult.IsFailure) return Result<WorkCenterProjection>.Failure(codeResult.Error);

            var now = _clock.UtcNow;
            try
            {
                var wc = new WorkCenter(
                    Guid.NewGuid(),
                    orgId,
                    branchId.Value,
                    codeResult.Value!,
                    input.Name,
                    input.DailyCapacityHours ?? WorkCenter.DefaultDailyCapacityHours,
                    input.HourlyRate ?? 0m,
                    access.ActorUserId,
                    now);

                _db.WorkCenters.Add(wc);
                Audit(access, "work-center.created", "WorkCenter", wc.Id, traceId,
                    new { code = wc.Code, name = wc.Name, dailyCapacityHours = wc.DailyCapacityHours, hourlyRate = wc.HourlyRate }, wc.RowVersion, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, wc.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<WorkCenterProjection>.Success(ToProjection(wc));
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkCenterProjection>(ex);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<WorkCenterProjection>("PRODUCTION_WORK_CENTER_CODE_CONFLICT", "A work center with this code already exists.");
            }
        });
    }

    public async Task<Result<WorkCenterProjection>> UpdateWorkCenterAsync(
        RequestAccessContext access, Guid workCenterId, Guid expectedVersion, WorkCenterUpdateInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var wc = await _db.WorkCenters.FirstOrDefaultAsync(w => w.Id == workCenterId && w.OrganizationId == orgId, ct);
            if (wc is null) return Fail<WorkCenterProjection>("RESOURCE_NOT_FOUND", "Work center not found.");
            if (wc.RowVersion != expectedVersion) return Fail<WorkCenterProjection>("PRODUCTION_WORK_CENTER_VERSION_CONFLICT", "Work center version conflict.");

            var now = _clock.UtcNow;
            try
            {
                wc.Update(input.Name, input.DailyCapacityHours, input.HourlyRate, now);
                Audit(access, "work-center.updated", "WorkCenter", wc.Id, traceId,
                    new { code = wc.Code, name = wc.Name, dailyCapacityHours = wc.DailyCapacityHours, hourlyRate = wc.HourlyRate }, wc.RowVersion, now);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<WorkCenterProjection>.Success(ToProjection(wc));
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkCenterProjection>(ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<WorkCenterProjection>("PRODUCTION_WORK_CENTER_VERSION_CONFLICT", "Work center version conflict.");
            }
        });
    }

    public async Task<Result<WorkCenterProjection>> SetWorkCenterActiveAsync(
        RequestAccessContext access, Guid workCenterId, Guid expectedVersion, bool active, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var wc = await _db.WorkCenters.FirstOrDefaultAsync(w => w.Id == workCenterId && w.OrganizationId == orgId, ct);
            if (wc is null) return Fail<WorkCenterProjection>("RESOURCE_NOT_FOUND", "Work center not found.");
            if (wc.RowVersion != expectedVersion) return Fail<WorkCenterProjection>("PRODUCTION_WORK_CENTER_VERSION_CONFLICT", "Work center version conflict.");

            if (!active)
            {
                var usedInApprovedBom = await (from s in _db.BomRoutingSteps.AsNoTracking()
                                               join r in _db.BomRevisions.AsNoTracking() on s.BomRevisionId equals r.Id
                                               where s.OrganizationId == orgId && s.WorkCenterId == workCenterId && r.Status == BomRevisionStatus.Approved
                                               select s.Id).AnyAsync(ct);

                var usedInOpenWorkOrder = await (from op in _db.WorkOrderOperations.AsNoTracking()
                                                 join wo in _db.WorkOrders.AsNoTracking() on op.WorkOrderId equals wo.Id
                                                 where op.OrganizationId == orgId
                                                     && op.WorkCenterId == workCenterId
                                                     && op.Status == WorkOrderOperationStatus.Pending
                                                     && (wo.Status == WorkOrderStatus.Draft || wo.Status == WorkOrderStatus.Released || wo.Status == WorkOrderStatus.InProgress)
                                                 select op.Id).AnyAsync(ct);

                if (usedInApprovedBom || usedInOpenWorkOrder)
                {
                    return Fail<WorkCenterProjection>("PRODUCTION_WORK_CENTER_IN_USE", "Cannot deactivate a work center that is referenced by an approved BOM or an open work order.");
                }
            }

            var now = _clock.UtcNow;
            try
            {
                wc.SetActive(active, now);
                Audit(access, active ? "work-center.activated" : "work-center.deactivated", "WorkCenter", wc.Id, traceId,
                    new { code = wc.Code, status = wc.Status }, wc.RowVersion, now);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<WorkCenterProjection>.Success(ToProjection(wc));
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkCenterProjection>(ex);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<WorkCenterProjection>("PRODUCTION_WORK_CENTER_VERSION_CONFLICT", "Work center version conflict.");
            }
        });
    }

    public async Task<WorkCenterProjection?> GetWorkCenterAsync(Guid organizationId, Guid workCenterId, CancellationToken ct = default)
    {
        var wc = await _db.WorkCenters.AsNoTracking().FirstOrDefaultAsync(w => w.Id == workCenterId && w.OrganizationId == organizationId, ct);
        return wc is null ? null : ToProjection(wc);
    }

    public async Task<PagedWorkCenters> ListWorkCentersAsync(Guid organizationId, WorkCenterListQuery query, CancellationToken ct = default)
    {
        var workCenters = _db.WorkCenters.AsNoTracking().Where(w => w.OrganizationId == organizationId);
        if (query.BranchId.HasValue) workCenters = workCenters.Where(w => w.BranchId == query.BranchId.Value);
        if (query.Status is not null) workCenters = workCenters.Where(w => w.Status == query.Status);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            workCenters = workCenters.Where(w => w.NormalizedCode.Contains(needle) || w.Name.ToUpper().Contains(needle));
        }

        var total = await workCenters.CountAsync(ct);
        var rows = await workCenters.OrderBy(w => w.NormalizedCode).ThenBy(w => w.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedWorkCenters(rows.Select(ToProjection).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<Result<WorkCenterCapacityViewProjection>> GetCapacityViewAsync(
        Guid organizationId, Guid? branchId, DateOnly? fromDate, DateOnly? toDate, CancellationToken ct = default)
    {
        var startDate = fromDate ?? DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime);
        var endDate = toDate ?? startDate.AddDays(6);
        if (endDate < startDate || (endDate.DayNumber - startDate.DayNumber) > 30)
        {
            return Fail<WorkCenterCapacityViewProjection>("PRODUCTION_FIELD_INVALID", "Capacity view date range must be between 1 and 31 days.");
        }

        var wcQuery = _db.WorkCenters.AsNoTracking()
            .Where(w => w.OrganizationId == organizationId && w.Status == WorkCenterStatus.Active);
        if (branchId.HasValue) wcQuery = wcQuery.Where(w => w.BranchId == branchId.Value);
        var workCenters = await wcQuery.OrderBy(w => w.NormalizedCode).ThenBy(w => w.Id).ToListAsync(ct);

        var wcIds = workCenters.Select(w => w.Id).ToList();
        var openOps = wcIds.Count == 0
            ? new List<(Guid WorkCenterId, DateOnly Start, DateOnly End, decimal RemainingHours)>()
            : (await (from op in _db.WorkOrderOperations.AsNoTracking()
                      join wo in _db.WorkOrders.AsNoTracking() on op.WorkOrderId equals wo.Id
                      where op.OrganizationId == organizationId
                          && op.WorkCenterId.HasValue
                          && wcIds.Contains(op.WorkCenterId.Value)
                          && !op.IsSubcontracted
                          && op.Status == WorkOrderOperationStatus.Pending
                          && (wo.Status == WorkOrderStatus.Released || wo.Status == WorkOrderStatus.InProgress)
                          && wo.PlannedStartDate <= endDate
                          && wo.PlannedEndDate >= startDate
                      select new
                      {
                          WorkCenterId = op.WorkCenterId!.Value,
                          wo.PlannedStartDate,
                          wo.PlannedEndDate,
                          op.PlannedSetupHours,
                          op.PlannedRunHours,
                          op.ActualHours
                      }).ToListAsync(ct))
              .Select(x => (
                  WorkCenterId: x.WorkCenterId,
                  Start: x.PlannedStartDate,
                  End: x.PlannedEndDate,
                  RemainingHours: Math.Max(0m, x.PlannedSetupHours + x.PlannedRunHours - x.ActualHours)))
              .ToList();

        var dayCount = endDate.DayNumber - startDate.DayNumber + 1;
        var holidays = await _db.WorkingCalendarHolidays.AsNoTracking()
            .Where(h => h.OrganizationId == organizationId && h.HolidayDate >= startDate && h.HolidayDate <= endDate)
            .Select(h => new { h.BranchId, h.HolidayDate })
            .ToListAsync(ct);
        var rows = new List<WorkCenterCapacityRowProjection>(workCenters.Count);

        foreach (var wc in workCenters)
        {
            var wcHolidays = holidays
                .Where(h => !h.BranchId.HasValue || h.BranchId.Value == wc.BranchId)
                .Select(h => h.HolidayDate)
                .ToHashSet();

            var dayBuckets = new Dictionary<DateOnly, decimal>(dayCount);
            for (var i = 0; i < dayCount; i++)
            {
                dayBuckets[startDate.AddDays(i)] = 0m;
            }

            foreach (var op in openOps.Where(o => o.WorkCenterId == wc.Id && o.RemainingHours > 0m))
            {
                var span = Math.Max(1, op.End.DayNumber - op.Start.DayNumber + 1);
                var perDay = decimal.Round(op.RemainingHours / span, 4);
                var startDay = op.Start < startDate ? startDate : op.Start;
                var endDay = op.End > endDate ? endDate : op.End;
                for (var d = startDay; d <= endDay; d = d.AddDays(1))
                {
                    dayBuckets[d] += perDay;
                }
            }

            var days = new List<WorkCenterDailyCapacityPointProjection>(dayCount);
            for (var i = 0; i < dayCount; i++)
            {
                var d = startDate.AddDays(i);
                var planned = decimal.Round(dayBuckets[d], 2);
                var cap = wcHolidays.Contains(d) ? 0m : wc.DailyCapacityHours;
                var remaining = Math.Max(0m, cap - planned);
                var util = cap > 0m ? decimal.Round(planned * 100m / cap, 2) : (planned > 0m ? 100m : 0m);
                days.Add(new WorkCenterDailyCapacityPointProjection(d, cap, planned, remaining, util, planned > cap));
            }

            var totalCap = decimal.Round(days.Sum(x => x.CapacityHours), 2);
            var totalPlanned = decimal.Round(days.Sum(x => x.PlannedHours), 2);
            var totalUtil = totalCap > 0m ? decimal.Round(totalPlanned * 100m / totalCap, 2) : (totalPlanned > 0m ? 100m : 0m);
            var isOverloaded = days.Any(x => x.IsOverloaded) || totalPlanned > totalCap;

            rows.Add(new WorkCenterCapacityRowProjection(
                wc.Id, wc.BranchId, wc.Code, wc.Name, wc.DailyCapacityHours, wc.HourlyRate,
                totalCap, totalPlanned, totalUtil, isOverloaded, days));
        }

        return Result<WorkCenterCapacityViewProjection>.Success(new WorkCenterCapacityViewProjection(startDate, endDate, rows));
    }

    // ===== BOM =======================================================================================

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

    private async Task<Result<bool>> ValidateRoutingWorkCentersAsync(Guid orgId, IReadOnlyList<BomRoutingStepInput>? steps, CancellationToken ct)
    {
        if (steps is null || steps.Count == 0) return Result<bool>.Success(true);

        var wcIds = steps.Where(s => s.WorkCenterId.HasValue && s.WorkCenterId.Value != Guid.Empty)
            .Select(s => s.WorkCenterId!.Value)
            .Distinct()
            .ToList();
        if (wcIds.Count == 0) return Result<bool>.Success(true);

        var workCenters = await _db.WorkCenters.AsNoTracking()
            .Where(w => w.OrganizationId == orgId && wcIds.Contains(w.Id))
            .ToListAsync(ct);

        foreach (var wcId in wcIds)
        {
            var wc = workCenters.FirstOrDefault(w => w.Id == wcId);
            if (wc is null) return Fail<bool>("RESOURCE_NOT_FOUND", $"Work center '{wcId}' not found.");
            if (wc.Status != WorkCenterStatus.Active)
            {
                return Fail<bool>("PRODUCTION_WORK_CENTER_INACTIVE", $"Work center '{wc.Code}' is not active.");
            }
        }

        return Result<bool>.Success(true);
    }

    private static List<BomLine> BuildBomLines(Guid orgId, Guid revisionId, IReadOnlyList<BomLineInput> lines) =>
        lines.Select((l, i) => new BomLine(Guid.NewGuid(), orgId, revisionId, l.ComponentItemId, l.Quantity, l.ScrapPercent, i + 1)).ToList();

    private static List<BomRoutingStep> BuildBomRoutingSteps(Guid orgId, Guid revisionId, IReadOnlyList<BomRoutingStepInput>? steps) =>
        steps is null || steps.Count == 0
            ? new List<BomRoutingStep>()
            : steps.OrderBy(s => s.Sequence)
                .Select(s => new BomRoutingStep(
                    Guid.NewGuid(), orgId, revisionId, s.Sequence, s.OperationCode, s.OperationName,
                    s.WorkCenterId, s.SetupHours, s.RunHours, s.IsSubcontracted, s.StdSubcontractCost, s.Note))
                .ToList();

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

            var routingValid = await ValidateRoutingWorkCentersAsync(orgId, input.RoutingSteps, ct);
            if (routingValid.IsFailure) return Result<BomProjection>.Failure(routingValid.Error);

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
                foreach (var step in BuildBomRoutingSteps(orgId, revision.Id, input.RoutingSteps)) revision.AddRoutingStep(step);
                bom.AddRevision(revision);
                _db.Boms.Add(bom);
                Audit(access, "bom.created", "Bom", bom.Id, traceId, new { code = bom.Code, itemId = bom.ItemId, lineCount = input.Lines.Count, routingCount = input.RoutingSteps?.Count ?? 0 }, revision.RowVersion, now);
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

            var routingValid = await ValidateRoutingWorkCentersAsync(orgId, input.RoutingSteps, ct);
            if (routingValid.IsFailure) return Result<BomProjection>.Failure(routingValid.Error);

            var now = _clock.UtcNow;
            try
            {
                var revision = new BomRevision(Guid.NewGuid(), orgId, bom.Id, bom.Revisions.Max(r => r.RevisionNo) + 1, input.OutputQuantity, input.Note, access.ActorUserId, now);
                foreach (var line in BuildBomLines(orgId, revision.Id, input.Lines)) revision.AddLine(line);
                foreach (var step in BuildBomRoutingSteps(orgId, revision.Id, input.RoutingSteps)) revision.AddRoutingStep(step);
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
            var revision = await _db.BomRevisions.Include(r => r.Lines).Include(r => r.RoutingSteps)
                .FirstOrDefaultAsync(r => r.Id == revisionId && r.BomId == bomId && r.OrganizationId == orgId, ct);
            if (bom is null || revision is null) return Fail<BomProjection>("RESOURCE_NOT_FOUND", "BOM revision not found.");
            if (revision.RowVersion != expectedVersion) return Fail<BomProjection>("BOM_VERSION_CONFLICT", "BOM revision version conflict.");

            var components = await ValidateComponentsAsync(orgId, bom.ItemId, input.Lines, ct);
            if (components.IsFailure) return Result<BomProjection>.Failure(components.Error);

            var routingValid = await ValidateRoutingWorkCentersAsync(orgId, input.RoutingSteps, ct);
            if (routingValid.IsFailure) return Result<BomProjection>.Failure(routingValid.Error);

            var now = _clock.UtcNow;
            try
            {
                var oldLines = revision.Lines.ToList();
                var oldSteps = revision.RoutingSteps.ToList();
                revision.EditDraft(input.OutputQuantity, input.Note);
                var newLines = BuildBomLines(orgId, revision.Id, input.Lines);
                var newSteps = BuildBomRoutingSteps(orgId, revision.Id, input.RoutingSteps);
                revision.ReplaceLines(newLines);
                revision.ReplaceRoutingSteps(newSteps);
                _db.BomLines.RemoveRange(oldLines);
                _db.BomLines.AddRange(newLines);
                _db.BomRoutingSteps.RemoveRange(oldSteps);
                _db.BomRoutingSteps.AddRange(newSteps);
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
            var bom = await _db.Boms
                .Include(b => b.Revisions).ThenInclude(r => r.Lines)
                .Include(b => b.Revisions).ThenInclude(r => r.RoutingSteps)
                .FirstOrDefaultAsync(b => b.Id == bomId && b.OrganizationId == orgId, ct);
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

                    var routingSteps = revision.RoutingSteps.Select(s => new BomRoutingStepInput(
                        s.Sequence, s.OperationCode, s.OperationName, s.WorkCenterId, s.SetupHours, s.RunHours, s.IsSubcontracted, s.StdSubcontractCost, s.Note)).ToList();
                    var routingValid = await ValidateRoutingWorkCentersAsync(orgId, routingSteps, ct);
                    if (routingValid.IsFailure) return Result<BomProjection>.Failure(routingValid.Error);

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

    private async Task<Dictionary<Guid, WorkCenterRef>> WorkCenterRefsAsync(Guid orgId, IEnumerable<Guid> ids, CancellationToken ct)
    {
        var list = ids.Distinct().ToList();
        if (list.Count == 0) return new Dictionary<Guid, WorkCenterRef>();
        var wcs = await _db.WorkCenters.AsNoTracking().Where(w => w.OrganizationId == orgId && list.Contains(w.Id)).ToListAsync(ct);
        return wcs.ToDictionary(w => w.Id, ToRef);
    }

    private async Task<Dictionary<Guid, ProductionPerson>> PeopleAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var list = ids.Distinct().ToList();
        return await _db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new ProductionPerson(u.Id, u.DisplayName, u.Email), ct);
    }

    public async Task<BomProjection?> GetBomAsync(Guid organizationId, Guid bomId, CancellationToken ct = default)
    {
        var bom = await _db.Boms.AsNoTracking()
            .Include(b => b.Revisions).ThenInclude(r => r.Lines)
            .Include(b => b.Revisions).ThenInclude(r => r.RoutingSteps)
            .FirstOrDefaultAsync(b => b.Id == bomId && b.OrganizationId == organizationId, ct);
        if (bom is null) return null;

        var itemIds = bom.Revisions.SelectMany(r => r.Lines.Select(l => l.ComponentItemId)).Append(bom.ItemId);
        var items = await ItemRefsAsync(organizationId, itemIds, ct);
        var wcIds = bom.Revisions.SelectMany(r => r.RoutingSteps.Where(s => s.WorkCenterId.HasValue).Select(s => s.WorkCenterId!.Value));
        var wcs = await WorkCenterRefsAsync(organizationId, wcIds, ct);
        var userIds = bom.Revisions.SelectMany(r => r.ApprovedByUserId.HasValue ? new[] { r.CreatedByUserId, r.ApprovedByUserId.Value } : new[] { r.CreatedByUserId });
        var people = await PeopleAsync(userIds, ct);
        ProductionPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProductionPerson(id, string.Empty, null);

        return new BomProjection(
            bom.Id, bom.Code, items[bom.ItemId], bom.CreatedAtUtc,
            bom.Revisions.OrderByDescending(r => r.RevisionNo).Select(r => new BomRevisionProjection(
                r.Id, r.RevisionNo, r.Status, r.OutputQuantity, r.Note, Person(r.CreatedByUserId), r.CreatedAtUtc,
                r.ApprovedByUserId.HasValue ? Person(r.ApprovedByUserId.Value) : null, r.ApprovedAtUtc, r.RowVersion,
                r.Lines.OrderBy(l => l.SortOrder).Select(l => new BomLineProjection(
                    l.Id, items[l.ComponentItemId], l.Quantity, l.ScrapPercent, decimal.Round(l.Quantity * (1 + l.ScrapPercent / 100m), 4))).ToList(),
                r.RoutingSteps.OrderBy(s => s.Sequence).Select(s => new BomRoutingStepProjection(
                    s.Id, s.Sequence, s.OperationCode, s.OperationName,
                    s.WorkCenterId.HasValue && wcs.TryGetValue(s.WorkCenterId.Value, out var wc) ? wc : null,
                    s.SetupHours, s.RunHours, s.IsSubcontracted, s.StdSubcontractCost, s.Note)).ToList())).ToList());
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

            var bom = await _db.Boms.AsNoTracking()
                .Include(b => b.Revisions).ThenInclude(r => r.Lines)
                .Include(b => b.Revisions).ThenInclude(r => r.RoutingSteps)
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
                var order = new WorkOrder(
                    Guid.NewGuid(), orgId, access.BranchId.Value, number, input.ItemId, revision.Id,
                    input.WarehouseId, input.ProjectId, input.PlannedQuantity, input.Note, access.ActorUserId, now,
                    input.PlannedStartDate, input.PlannedEndDate);

                var sort = 1;
                foreach (var line in revision.Lines.OrderBy(l => l.SortOrder))
                {
                    var required = decimal.Round(line.Quantity * (1 + line.ScrapPercent / 100m) * input.PlannedQuantity / revision.OutputQuantity, 4);
                    order.AddMaterial(new WorkOrderMaterial(Guid.NewGuid(), orgId, order.Id, line.ComponentItemId, required, sort++));
                }

                foreach (var step in revision.RoutingSteps.OrderBy(s => s.Sequence))
                {
                    var plannedRun = decimal.Round(step.RunHours * input.PlannedQuantity / revision.OutputQuantity, 4);
                    order.AddOperation(new WorkOrderOperation(
                        Guid.NewGuid(), orgId, order.Id, step.Sequence, step.OperationCode, step.OperationName,
                        step.WorkCenterId, step.SetupHours, plannedRun, step.IsSubcontracted, step.Note));
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
            var order = await _db.WorkOrders.Include(o => o.Materials).Include(o => o.Operations)
                .FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == orgId, ct);
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

    public async Task<Result<WorkOrderProjection>> CompleteOperationAsync(
        RequestAccessContext access, Guid workOrderId, Guid operationId, CompleteOperationInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var opKey = $"wo.op.complete:{operationId}";
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, opKey, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WorkOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = await GetWorkOrderAsync(orgId, workOrderId, ct);
                if (replayed is not null) return Result<WorkOrderProjection>.Success(replayed);
            }

            await LockWorkOrderAsync(orgId, workOrderId, ct);
            var order = await _db.WorkOrders.Include(o => o.Operations)
                .FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Work order not found.");

            var now = _clock.UtcNow;
            try
            {
                order.CompleteOperation(operationId, input.ActualHours, input.Note, now);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkOrderProjection>(ex);
            }

            Audit(access, "work-order.operation-completed", "WorkOrder", order.Id, traceId,
                new { number = order.Number, operationId, actualHours = input.ActualHours }, order.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, opKey, keyHash, payloadHash, order.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<WorkOrderProjection>.Success((await GetWorkOrderAsync(orgId, order.Id, ct))!);
        });
    }

    public async Task<Result<WorkOrderProjection>> RecordSubcontractAsync(
        RequestAccessContext access, Guid workOrderId, Guid operationId, RecordSubcontractInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var opKey = $"wo.op.subcontract:{operationId}";
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, opKey, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<WorkOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = await GetWorkOrderAsync(orgId, workOrderId, ct);
                if (replayed is not null) return Result<WorkOrderProjection>.Success(replayed);
            }

            await LockWorkOrderAsync(orgId, workOrderId, ct);
            var order = await _db.WorkOrders.Include(o => o.Operations)
                .FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Work order not found.");

            var po = await _db.PurchaseOrders.AsNoTracking().Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Id == input.PurchaseOrderId && p.OrganizationId == orgId, ct);
            if (po is null) return Fail<WorkOrderProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.");
            if (po.Status is not (PurchaseOrderStatus.Approved or PurchaseOrderStatus.PartiallyReceived or PurchaseOrderStatus.Received))
            {
                return Fail<WorkOrderProjection>("PRODUCTION_SUBCONTRACT_PO_NOT_APPROVED", "The subcontract purchase order must be approved or received.");
            }

            var poTotal = po.Lines.Sum(l => decimal.Round(l.Quantity * l.UnitPrice, 2));
            if (decimal.Round(input.CostAmount, 2) > poTotal)
            {
                return Fail<WorkOrderProjection>("PRODUCTION_SUBCONTRACT_COST_EXCEEDS_PO", "Subcontract cost amount cannot exceed the purchase order total.");
            }

            var now = _clock.UtcNow;
            try
            {
                order.RecordSubcontract(operationId, po.Id, input.CostAmount, input.Note, now);
            }
            catch (ProductionDomainException ex)
            {
                return Fail<WorkOrderProjection>(ex);
            }

            Audit(access, "work-order.operation-subcontracted", "WorkOrder", order.Id, traceId,
                new { number = order.Number, operationId, purchaseOrderId = po.Id, purchaseOrderNumber = po.Number, costAmount = input.CostAmount }, order.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, opKey, keyHash, payloadHash, order.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

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
            var order = await _db.WorkOrders.Include(o => o.Materials).Include(o => o.Operations)
                .FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == orgId, ct);
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
        var order = await _db.WorkOrders.AsNoTracking()
            .Include(o => o.Materials)
            .Include(o => o.Operations)
            .FirstOrDefaultAsync(o => o.Id == workOrderId && o.OrganizationId == organizationId, ct);
        if (order is null) return null;

        var revision = await _db.BomRevisions.AsNoTracking().FirstAsync(r => r.Id == order.BomRevisionId, ct);
        var bom = await _db.Boms.AsNoTracking().FirstAsync(b => b.Id == revision.BomId, ct);
        var warehouse = await _db.Warehouses.AsNoTracking().FirstAsync(w => w.Id == order.WarehouseId, ct);
        var project = order.ProjectId.HasValue ? await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == order.ProjectId.Value, ct) : null;
        var transactions = await _db.WorkOrderTransactions.AsNoTracking().Where(t => t.WorkOrderId == order.Id).OrderBy(t => t.CreatedAtUtc).ThenBy(t => t.Id).ToListAsync(ct);
        var timesheets = await _db.LaborTimesheetEntries.AsNoTracking()
            .Where(t => t.OrganizationId == organizationId && t.WorkOrderId == order.Id && t.Status == TimesheetStatus.Approved)
            .OrderBy(t => t.WorkDate).ThenBy(t => t.CreatedAtUtc)
            .ToListAsync(ct);

        var wcIds = order.Operations.Where(op => op.WorkCenterId.HasValue).Select(op => op.WorkCenterId!.Value);
        var wcs = await WorkCenterRefsAsync(organizationId, wcIds, ct);

        var poIds = order.Operations.Where(op => op.SubcontractPurchaseOrderId.HasValue).Select(op => op.SubcontractPurchaseOrderId!.Value).Distinct().ToList();
        var pos = poIds.Count == 0
            ? new Dictionary<Guid, WorkOrderRef>()
            : (await (from p in _db.PurchaseOrders.AsNoTracking()
                      join s in _db.Suppliers.AsNoTracking() on p.SupplierId equals s.Id
                      where p.OrganizationId == organizationId && poIds.Contains(p.Id)
                      select new { p.Id, p.Number, SupplierName = s.NameTh }).ToListAsync(ct))
              .ToDictionary(x => x.Id, x => new WorkOrderRef(x.Id, x.Number, x.SupplierName));

        var items = await ItemRefsAsync(organizationId, order.Materials.Select(m => m.ItemId).Append(order.ItemId), ct);
        var userIds = transactions.Select(t => t.ActorUserId)
            .Concat(timesheets.Select(t => t.WorkerUserId))
            .Append(order.CreatedByUserId);
        var people = await PeopleAsync(userIds, ct);
        ProductionPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProductionPerson(id, string.Empty, null);

        return new WorkOrderProjection(
            order.Id, order.BranchId, order.Number, order.Status, items[order.ItemId], revision.Id, bom.Code, revision.RevisionNo,
            new WorkOrderWarehouseRef(warehouse.Id, warehouse.Code, warehouse.Name),
            project is null ? null : new WorkOrderRef(project.Id, project.Code, project.Name),
            order.PlannedQuantity, order.CompletedQuantity, order.PlannedStartDate, order.PlannedEndDate,
            order.NetIssuedValue, order.LaborHours, order.LaborCost, order.SubcontractCost, order.TotalManufacturingCost, order.CostAllocated,
            order.Note, order.CancelReason,
            Person(order.CreatedByUserId), order.CreatedAtUtc, order.RowVersion,
            order.Materials.OrderBy(m => m.SortOrder).Select(m => new WorkOrderMaterialProjection(
                m.Id, items[m.ItemId], m.RequiredQuantity, m.IssuedQuantity, m.ReturnedQuantity, m.NetIssuedQuantity,
                Math.Max(0m, m.RequiredQuantity - m.NetIssuedQuantity), m.IssuedValue, m.ReturnedValue)).ToList(),
            order.Operations.OrderBy(op => op.Sequence).Select(op => new WorkOrderOperationProjection(
                op.Id, op.Sequence, op.OperationCode, op.OperationName,
                op.WorkCenterId.HasValue && wcs.TryGetValue(op.WorkCenterId.Value, out var wc) ? wc : null,
                op.PlannedSetupHours, op.PlannedRunHours, op.ActualHours, op.IsSubcontracted,
                op.SubcontractPurchaseOrderId.HasValue && pos.TryGetValue(op.SubcontractPurchaseOrderId.Value, out var po) ? po : null,
                op.SubcontractCost, op.Status, op.Note, op.CompletedAtUtc)).ToList(),
            timesheets.Select(t => new WorkOrderLaborEntryProjection(
                t.Id, t.Number, Person(t.WorkerUserId), t.WorkDate, t.Hours, t.HourlyRate, t.CostAmount, t.TaskDescription, t.DecidedAtUtc)).ToList(),
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
        var rows = await orders.Include(o => o.Materials).OrderByDescending(o => o.CreatedAtUtc).ThenBy(o => o.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var items = await ItemRefsAsync(organizationId, rows.Select(o => o.ItemId), ct);
        var projectIds = rows.Where(o => o.ProjectId.HasValue).Select(o => o.ProjectId!.Value).Distinct().ToList();
        var projects = await _db.Projects.AsNoTracking().Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Code, ct);
        return new PagedWorkOrders(rows.Select(o => new WorkOrderListItemProjection(
            o.Id, o.Number, o.Status, items[o.ItemId].Code, items[o.ItemId].NameTh, o.PlannedQuantity, o.CompletedQuantity,
            o.PlannedStartDate, o.PlannedEndDate, o.TotalManufacturingCost,
            o.ProjectId.HasValue && projects.TryGetValue(o.ProjectId.Value, out var code) ? code : null, o.CreatedAtUtc)).ToList(), total, query.Page, query.PageSize);
    }
}
