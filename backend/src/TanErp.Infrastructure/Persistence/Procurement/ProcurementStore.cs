using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Procurement;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Items;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Persistence.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Procurement;

public class ProcurementStore : IProcurementStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;

    public ProcurementStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> Fail<T>(ProcurementDomainException ex) => Fail<T>(ex.Code, ex.Message);

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

    // ===== suppliers =================================================================================

    private static SupplierProjection ToProjection(Supplier s) => new(
        s.Id, s.Code, s.NameTh, s.NameEn, s.TaxId, s.ContactName, s.Phone, s.Email, s.PaymentTermDays, s.Status, s.RowVersion, s.CreatedAtUtc);

    public async Task<Result<SupplierProjection>> CreateSupplierAsync(
        RequestAccessContext access, SupplierInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "procurement.supplier.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<SupplierProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetSupplierAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<SupplierProjection>.Success(replayed);
            }

            var now = _clock.UtcNow;
            var code = await MasterDataCodeAllocator.ResolveAsync(
                _numbers, orgId, DocumentTypes.Suppliers, null, 64, "SUPPLIER_CODE_CONFLICT",
                async (candidate, token) => await _db.Suppliers.AnyAsync(s => s.OrganizationId == orgId && s.NormalizedCode == candidate.Trim().ToUpperInvariant(), token), ct);
            if (code.IsFailure) return Result<SupplierProjection>.Failure(code.Error);

            Supplier supplier;
            try
            {
                supplier = new Supplier(Guid.NewGuid(), orgId, code.Value!, input.NameTh ?? string.Empty, input.NameEn, input.TaxId, input.ContactName, input.Phone, input.Email, input.PaymentTermDays, access.ActorUserId, now);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<SupplierProjection>(ex);
            }

            _db.Suppliers.Add(supplier);
            Audit(access, "supplier.created", "Supplier", supplier.Id, traceId, new { code = supplier.Code }, supplier.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, supplier.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<SupplierProjection>("SUPPLIER_CODE_CONFLICT", "The supplier code already exists.");
            }

            return Result<SupplierProjection>.Success(ToProjection(supplier));
        });
    }

    private async Task<Result<SupplierProjection>> MutateSupplierAsync(
        RequestAccessContext access, Guid supplierId, Guid expectedVersion, Action<Supplier, DateTimeOffset> mutate, string audit, string traceId, CancellationToken ct)
    {
        var supplier = await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == supplierId && s.OrganizationId == access.OrganizationId, ct);
        if (supplier is null) return Fail<SupplierProjection>("RESOURCE_NOT_FOUND", "Supplier not found.");
        if (supplier.RowVersion != expectedVersion) return Fail<SupplierProjection>("SUPPLIER_VERSION_CONFLICT", "Supplier version conflict.");

        var now = _clock.UtcNow;
        try
        {
            mutate(supplier, now);
        }
        catch (ProcurementDomainException ex)
        {
            return Fail<SupplierProjection>(ex);
        }

        Audit(access, audit, "Supplier", supplier.Id, traceId, new { code = supplier.Code, status = supplier.Status }, supplier.RowVersion, now);
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Fail<SupplierProjection>("SUPPLIER_VERSION_CONFLICT", "Supplier version conflict.");
        }

        return Result<SupplierProjection>.Success(ToProjection(supplier));
    }

    public Task<Result<SupplierProjection>> UpdateSupplierAsync(RequestAccessContext access, Guid supplierId, Guid expectedVersion, SupplierInput input, string traceId, CancellationToken ct = default) =>
        MutateSupplierAsync(access, supplierId, expectedVersion,
            (s, now) => s.Update(input.NameTh ?? string.Empty, input.NameEn, input.TaxId, input.ContactName, input.Phone, input.Email, input.PaymentTermDays, now),
            "supplier.updated", traceId, ct);

    public Task<Result<SupplierProjection>> SetSupplierActiveAsync(RequestAccessContext access, Guid supplierId, Guid expectedVersion, bool active, string traceId, CancellationToken ct = default) =>
        MutateSupplierAsync(access, supplierId, expectedVersion, (s, now) => s.SetActive(active, now), active ? "supplier.activated" : "supplier.deactivated", traceId, ct);

    public async Task<SupplierProjection?> GetSupplierAsync(Guid organizationId, Guid supplierId, CancellationToken ct = default)
    {
        var supplier = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == supplierId && s.OrganizationId == organizationId, ct);
        return supplier is null ? null : ToProjection(supplier);
    }

    public async Task<PagedSuppliers> ListSuppliersAsync(Guid organizationId, SupplierListQuery query, CancellationToken ct = default)
    {
        var suppliers = _db.Suppliers.AsNoTracking().Where(s => s.OrganizationId == organizationId);
        if (query.Status is not null) suppliers = suppliers.Where(s => s.Status == query.Status);
        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            suppliers = suppliers.Where(s => s.NormalizedCode.ToLower().Contains(needle) || s.NormalizedName.Contains(needle)
                || (s.NameEn != null && s.NameEn.ToLower().Contains(needle)) || (s.TaxId != null && s.TaxId.Contains(needle)));
        }

        var total = await suppliers.CountAsync(ct);
        var rows = await suppliers.OrderBy(s => s.NormalizedName).ThenBy(s => s.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedSuppliers(rows.Select(ToProjection).ToList(), total, query.Page, query.PageSize);
    }

    // ===== purchase orders ===========================================================================

    private sealed record ResolvedLine(PurchaseOrderLineInput Input, Item Item, string UnitCode);

    private sealed record ResolvedOrder(Supplier Supplier, Project? Project, List<ResolvedLine> Lines);

    private async Task<Result<ResolvedOrder>> ResolveOrderAsync(Guid orgId, PurchaseOrderInput input, CancellationToken ct)
    {
        var supplier = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == input.SupplierId && s.OrganizationId == orgId, ct);
        if (supplier is null) return Fail<ResolvedOrder>("RESOURCE_NOT_FOUND", "Supplier not found.");
        if (supplier.Status != SupplierStatus.Active) return Fail<ResolvedOrder>("PURCHASE_ORDER_SUPPLIER_INACTIVE", "The supplier is not active.");

        Project? project = null;
        if (input.ProjectId.HasValue)
        {
            project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.OrganizationId == orgId, ct);
            if (project is null) return Fail<ResolvedOrder>("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
            {
                return Fail<ResolvedOrder>("PURCHASE_ORDER_PROJECT_NOT_ACTIVE", "Orders cannot be raised for a completed or cancelled project.");
            }
        }

        var itemIds = input.Lines.Select(l => l.ItemId).ToList();
        var items = await _db.Items.AsNoTracking().Include(i => i.BaseUnit)
            .Where(i => i.OrganizationId == orgId && itemIds.Contains(i.Id)).ToDictionaryAsync(i => i.Id, ct);

        var lines = new List<ResolvedLine>();
        foreach (var line in input.Lines)
        {
            if (!items.TryGetValue(line.ItemId, out var item)) return Fail<ResolvedOrder>("RESOURCE_NOT_FOUND", $"Item '{line.ItemId}' not found.");
            if (item.Status != ItemStatus.Active || !item.Capabilities.CanPurchase || item.BaseUnit is null)
            {
                return Fail<ResolvedOrder>("PURCHASE_ORDER_ITEM_NOT_PURCHASABLE", $"Item '{item.Code}' is not an active, purchasable item.");
            }

            lines.Add(new ResolvedLine(line, item, item.BaseUnit.Code));
        }

        return Result<ResolvedOrder>.Success(new ResolvedOrder(supplier, project, lines));
    }

    private static List<PurchaseOrderLine> BuildLines(Guid orgId, Guid orderId, IReadOnlyList<ResolvedLine> resolved)
    {
        var lines = new List<PurchaseOrderLine>();
        for (var i = 0; i < resolved.Count; i++)
        {
            var r = resolved[i];
            lines.Add(new PurchaseOrderLine(Guid.NewGuid(), orgId, orderId, i + 1, r.Item.Id, r.Item.Code, r.Item.Name.Thai, r.Item.BaseUnitId, r.UnitCode, r.Input.Quantity, r.Input.UnitPrice));
        }

        return lines;
    }

    public async Task<Result<PurchaseOrderProjection>> CreatePurchaseOrderAsync(
        RequestAccessContext access, PurchaseOrderInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "procurement.purchase-order.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<PurchaseOrderProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<PurchaseOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetPurchaseOrderAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<PurchaseOrderProjection>.Success(replayed);
            }

            var resolved = await ResolveOrderAsync(orgId, input, ct);
            if (resolved.IsFailure) return Result<PurchaseOrderProjection>.Failure(resolved.Error);

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.PurchaseOrders, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<PurchaseOrderProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate purchase order document number.");
            }

            PurchaseOrder order;
            try
            {
                order = new PurchaseOrder(Guid.NewGuid(), orgId, access.BranchId.Value, input.SupplierId, input.ProjectId, number, input.ExpectedDeliveryDate, input.Note, access.ActorUserId, now);
                foreach (var line in BuildLines(orgId, order.Id, resolved.Value!.Lines)) order.AddLine(line);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseOrderProjection>(ex);
            }

            _db.PurchaseOrders.Add(order);
            Audit(access, "purchase-order.created", "PurchaseOrder", order.Id, traceId, new { number, total = order.TotalAmount, lineCount = order.Lines.Count }, order.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, order.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
    }

    public async Task<Result<PurchaseOrderProjection>> UpdatePurchaseOrderAsync(
        RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var order = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == purchaseOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.");
            if (order.RowVersion != expectedVersion) return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            if (order.SupplierId != input.SupplierId) return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_LINE_INVALID", "The supplier of an order cannot be changed; create a new order.");

            var resolved = await ResolveOrderAsync(orgId, input, ct);
            if (resolved.IsFailure) return Result<PurchaseOrderProjection>.Failure(resolved.Error);

            var now = _clock.UtcNow;
            try
            {
                var oldLines = order.Lines.ToList();
                order.EditDraft(input.ProjectId, input.ExpectedDeliveryDate, input.Note, now);
                var newLines = BuildLines(orgId, order.Id, resolved.Value!.Lines);
                order.ReplaceLines(newLines);
                _db.PurchaseOrderLines.RemoveRange(oldLines);
                _db.PurchaseOrderLines.AddRange(newLines);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseOrderProjection>(ex);
            }

            Audit(access, "purchase-order.updated", "PurchaseOrder", order.Id, traceId, new { number = order.Number, total = order.TotalAmount }, order.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            }

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
    }

    public async Task<Result<PurchaseOrderProjection>> PurchaseOrderActionAsync(
        RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderAction action, string? note, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var order = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == purchaseOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.");
            if (order.RowVersion != expectedVersion) return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");

            var now = _clock.UtcNow;
            try
            {
                switch (action)
                {
                    case PurchaseOrderAction.Submit:
                        order.Submit(now);
                        break;
                    case PurchaseOrderAction.Cancel:
                        order.Cancel(note ?? string.Empty, now);
                        break;
                    default:
                        if (action == PurchaseOrderAction.Approve)
                        {
                            var budgetFailure = await CheckApprovalAsync(orgId, order, ct);
                            if (budgetFailure is not null) return Result<PurchaseOrderProjection>.Failure(budgetFailure);
                        }

                        order.Decide(action == PurchaseOrderAction.Approve, access.ActorUserId, note, now);
                        break;
                }
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseOrderProjection>(ex);
            }

            Audit(access, $"purchase-order.{action.ToString().ToLowerInvariant()}", "PurchaseOrder", order.Id, traceId,
                new { number = order.Number, status = order.Status, total = order.TotalAmount }, order.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            }

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
    }

    private static readonly string[] CommittedStatuses =
    {
        PurchaseOrderStatus.Approved, PurchaseOrderStatus.PartiallyReceived, PurchaseOrderStatus.Received
    };

    /// <summary>Approval guards: the supplier must still be active, and a project order may not exceed the project's current budget.</summary>
    private async Task<Error?> CheckApprovalAsync(Guid orgId, PurchaseOrder order, CancellationToken ct)
    {
        var supplierActive = await _db.Suppliers.AsNoTracking().AnyAsync(s => s.Id == order.SupplierId && s.OrganizationId == orgId && s.Status == SupplierStatus.Active, ct);
        if (!supplierActive) return new Error("PURCHASE_ORDER_SUPPLIER_INACTIVE", "The supplier is not active.");

        if (!order.ProjectId.HasValue) return null;

        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == order.ProjectId.Value && p.OrganizationId == orgId, ct);
        if (project is null) return new Error("RESOURCE_NOT_FOUND", "Project not found.");
        if (project.Status is not (ProjectStatus.Active or ProjectStatus.OnHold))
        {
            return new Error("PURCHASE_ORDER_PROJECT_NOT_ACTIVE", "Orders can only be approved for an active or on-hold project.");
        }

        var approvedBudgetDelta = await _db.ProjectChangeOrders
            .Where(c => c.OrganizationId == orgId && c.ProjectId == project.Id && c.Status == ChangeOrderStatus.Approved)
            .SumAsync(c => (decimal?)c.BudgetDelta, ct) ?? 0m;
        var currentBudget = (project.BaselineBudgetTotal ?? 0m) + approvedBudgetDelta;
        var committed = await _db.PurchaseOrders
            .Where(o => o.OrganizationId == orgId && o.ProjectId == project.Id && o.Id != order.Id && CommittedStatuses.Contains(o.Status))
            .SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;

        return committed + order.TotalAmount > currentBudget
            ? new Error("PURCHASE_ORDER_OVER_BUDGET", "Approving this order would exceed the project's current budget.")
            : null;
    }

    // ----- reads -----------------------------------------------------------------------------------

    public async Task<PurchaseOrderProjection?> GetPurchaseOrderAsync(Guid organizationId, Guid purchaseOrderId, CancellationToken ct = default)
    {
        var order = await _db.PurchaseOrders.AsNoTracking().Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == purchaseOrderId && o.OrganizationId == organizationId, ct);
        if (order is null) return null;

        var supplier = await _db.Suppliers.AsNoTracking().FirstAsync(s => s.Id == order.SupplierId, ct);
        var project = order.ProjectId.HasValue ? await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == order.ProjectId.Value, ct) : null;
        var receipts = await _db.GoodsReceipts.AsNoTracking().Include(r => r.Lines)
            .Where(r => r.OrganizationId == organizationId && r.PurchaseOrderId == order.Id)
            .OrderByDescending(r => r.ReceivedAtUtc).ThenByDescending(r => r.Id).ToListAsync(ct);

        var userIds = new[] { order.CreatedByUserId, order.DecidedByUserId ?? Guid.Empty }.Concat(receipts.Select(r => r.ReceivedByUserId)).Where(id => id != Guid.Empty).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new ProcurementPerson(u.Id, u.DisplayName, u.Email), ct);
        ProcurementPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProcurementPerson(id, string.Empty, null);

        return new PurchaseOrderProjection(
            order.Id, order.BranchId, order.Number, order.Status, order.Currency, order.TotalAmount, order.ExpectedDeliveryDate, order.Note,
            new PurchaseOrderSupplierProjection(supplier.Id, supplier.Code, supplier.NameTh, supplier.NameEn),
            project is null ? null : new PurchaseOrderProjectProjection(project.Id, project.Code, project.Name),
            Person(order.CreatedByUserId), order.CreatedAtUtc, order.SubmittedAtUtc,
            order.DecidedByUserId.HasValue ? Person(order.DecidedByUserId.Value) : null,
            order.DecidedAtUtc, order.DecisionNote, order.CancelReason, order.RowVersion,
            order.Lines.OrderBy(l => l.LineNo).Select(l => new PurchaseOrderLineProjection(
                l.Id, l.LineNo, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode, l.Quantity, l.UnitPrice, l.LineTotal, l.ReceivedQuantity, l.RemainingQuantity)).ToList(),
            receipts.Select(r => new GoodsReceiptSummaryProjection(r.Id, r.Number, r.ReceivedAtUtc, r.Note, Person(r.ReceivedByUserId), r.Lines.Count, r.Lines.Sum(l => l.Quantity))).ToList());
    }

    public async Task<PagedPurchaseOrders> ListPurchaseOrdersAsync(Guid organizationId, PurchaseOrderListQuery query, CancellationToken ct = default)
    {
        var orders = _db.PurchaseOrders.AsNoTracking().Where(o => o.OrganizationId == organizationId);
        if (query.Status is not null) orders = orders.Where(o => o.Status == query.Status);
        if (query.SupplierId.HasValue) orders = orders.Where(o => o.SupplierId == query.SupplierId.Value);
        if (query.ProjectId.HasValue) orders = orders.Where(o => o.ProjectId == query.ProjectId.Value);

        var joined =
            from o in orders
            join s in _db.Suppliers.AsNoTracking() on o.SupplierId equals s.Id
            join p in _db.Projects.AsNoTracking() on o.ProjectId equals (Guid?)p.Id into projects
            from p in projects.DefaultIfEmpty()
            select new { Order = o, Supplier = s, ProjectCode = p == null ? null : p.Code };

        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            joined = joined.Where(x => x.Order.Number.ToLower().Contains(needle) || x.Supplier.NormalizedName.Contains(needle) || x.Supplier.NormalizedCode.ToLower().Contains(needle));
        }

        var total = await joined.CountAsync(ct);
        var rows = await joined.OrderByDescending(x => x.Order.CreatedAtUtc).ThenByDescending(x => x.Order.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedPurchaseOrders(
            rows.Select(x => new PurchaseOrderListItemProjection(
                x.Order.Id, x.Order.Number, x.Order.Status, x.Order.TotalAmount, x.Supplier.Code, x.Supplier.NameTh, x.ProjectCode, x.Order.ExpectedDeliveryDate, x.Order.CreatedAtUtc)).ToList(),
            total, query.Page, query.PageSize);
    }

    // ===== goods receipts ============================================================================

    public async Task<Result<PurchaseOrderProjection>> PostGoodsReceiptAsync(
        RequestAccessContext access, Guid purchaseOrderId, GoodsReceiptInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "procurement.goods-receipt.post";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<PurchaseOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, purchaseOrderId, ct))!);
            }

            var order = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == purchaseOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.");

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.GoodsReceipts, order.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<PurchaseOrderProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate goods receipt document number.");
            }

            GoodsReceipt receipt;
            try
            {
                receipt = new GoodsReceipt(Guid.NewGuid(), orgId, order.BranchId, order.Id, order.SupplierId, order.ProjectId, number, input.ReceivedAtUtc ?? now, input.Note, access.ActorUserId, now);
                foreach (var line in input.Lines)
                {
                    var orderLine = order.Lines.FirstOrDefault(l => l.Id == line.PurchaseOrderLineId)
                        ?? throw new ProcurementDomainException("GOODS_RECEIPT_INVALID", "A receipt line does not belong to this purchase order.");
                    receipt.AddLine(new GoodsReceiptLine(Guid.NewGuid(), orgId, receipt.Id, orderLine.Id, orderLine.ItemId, orderLine.UnitId, line.Quantity, orderLine.UnitPrice));
                }

                order.ApplyReceipt(input.Lines.Select(l => (l.PurchaseOrderLineId, l.Quantity)).ToList(), now);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseOrderProjection>(ex);
            }

            _db.GoodsReceipts.Add(receipt);
            Audit(access, "goods-receipt.posted", "GoodsReceipt", receipt.Id, traceId,
                new { number, purchaseOrderNumber = order.Number, lineCount = receipt.Lines.Count, orderStatus = order.Status }, null, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, receipt.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                // A concurrent receipt or cancel changed the order first; the caller can re-read and retry.
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation })
            {
                return Fail<PurchaseOrderProjection>("GOODS_RECEIPT_OVER_RECEIVED", "A line would be received above the ordered quantity.");
            }

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
    }
}
