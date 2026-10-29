using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.ApprovalLimits;
using TanErp.Application.Inventory;
using TanErp.Application.Notifications;
using TanErp.Application.Procurement;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Finance;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;
using TanErp.Infrastructure.Persistence.DocumentNumbering;
using TanErp.Infrastructure.Persistence.Notifications;

namespace TanErp.Infrastructure.Persistence.Procurement;

public class ProcurementStore : IProcurementStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly INotificationPublisher _notifications;
    private readonly IApprovalAuthority _approvalAuthority;
    private readonly IProcurementStockPort _stockPort;

    public ProcurementStore(
        AppDbContext db,
        IClock clock,
        IDocumentNumberGenerator numbers,
        INotificationPublisher notifications,
        IApprovalAuthority approvalAuthority,
        IProcurementStockPort stockPort)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _notifications = notifications;
        _approvalAuthority = approvalAuthority;
        _stockPort = stockPort;
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

    private void EnqueueOutbox(Guid orgId, string dedupeKey, string kind, Guid resourceId, string resourceNumber, decimal amount, object payload, DateTimeOffset now)
    {
        _db.AccountingOutbox.Add(new AccountingOutboxMessage(
            Guid.NewGuid(), orgId, dedupeKey, kind, resourceId, resourceNumber, amount, JsonSerializer.Serialize(payload), now));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(
            r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    private Task LockPurchaseOrderAsync(Guid orgId, Guid purchaseOrderId, CancellationToken ct) =>
        _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM procurement.purchase_orders WHERE id = {purchaseOrderId} AND organization_id = {orgId} FOR UPDATE", ct);

    private Task LockSupplierInvoiceAsync(Guid orgId, Guid supplierInvoiceId, CancellationToken ct) =>
        _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM procurement.supplier_invoices WHERE id = {supplierInvoiceId} AND organization_id = {orgId} FOR UPDATE", ct);

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

    // ===== purchase requests =========================================================================

    private sealed record ResolvedRequestLine(PurchaseRequestLineInput Input, Item Item, string UnitCode);

    private sealed record ResolvedRequest(Project? Project, List<ResolvedRequestLine> Lines);

    private async Task<Result<ResolvedRequest>> ResolveRequestAsync(Guid orgId, PurchaseRequestInput input, CancellationToken ct)
    {
        Project? project = null;
        if (input.ProjectId.HasValue)
        {
            project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.OrganizationId == orgId, ct);
            if (project is null) return Fail<ResolvedRequest>("RESOURCE_NOT_FOUND", "Project not found.");
            if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
            {
                return Fail<ResolvedRequest>("PURCHASE_ORDER_PROJECT_NOT_ACTIVE", "Purchase requests cannot be raised for a completed or cancelled project.");
            }
        }

        var itemIds = input.Lines.Select(l => l.ItemId).ToList();
        var items = await _db.Items.AsNoTracking().Include(i => i.BaseUnit)
            .Where(i => i.OrganizationId == orgId && itemIds.Contains(i.Id)).ToDictionaryAsync(i => i.Id, ct);

        var lines = new List<ResolvedRequestLine>();
        foreach (var line in input.Lines)
        {
            if (!items.TryGetValue(line.ItemId, out var item)) return Fail<ResolvedRequest>("RESOURCE_NOT_FOUND", $"Item '{line.ItemId}' not found.");
            if (item.Status != ItemStatus.Active || !item.Capabilities.CanPurchase || item.BaseUnit is null)
            {
                return Fail<ResolvedRequest>("PURCHASE_ORDER_ITEM_NOT_PURCHASABLE", $"Item '{item.Code}' is not an active, purchasable item.");
            }

            lines.Add(new ResolvedRequestLine(line, item, item.BaseUnit.Code));
        }

        return Result<ResolvedRequest>.Success(new ResolvedRequest(project, lines));
    }

    private static List<PurchaseRequestLine> BuildRequestLines(Guid orgId, Guid requestId, IReadOnlyList<ResolvedRequestLine> resolved)
    {
        var lines = new List<PurchaseRequestLine>();
        for (var i = 0; i < resolved.Count; i++)
        {
            var r = resolved[i];
            lines.Add(new PurchaseRequestLine(
                Guid.NewGuid(), orgId, requestId, i + 1, r.Item.Id, r.Item.Code, r.Item.Name.Thai,
                r.Item.BaseUnitId, r.UnitCode, r.Input.Quantity, r.Input.EstimatedUnitPrice, r.Input.Note));
        }

        return lines;
    }

    public async Task<Result<PurchaseRequestProjection>> CreatePurchaseRequestAsync(
        RequestAccessContext access, PurchaseRequestInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "procurement.purchase-request.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<PurchaseRequestProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<PurchaseRequestProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetPurchaseRequestAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<PurchaseRequestProjection>.Success(replayed);
            }

            var resolved = await ResolveRequestAsync(orgId, input, ct);
            if (resolved.IsFailure) return Result<PurchaseRequestProjection>.Failure(resolved.Error);

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.PurchaseRequests, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<PurchaseRequestProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate purchase request document number.");
            }

            PurchaseRequest request;
            try
            {
                request = new PurchaseRequest(Guid.NewGuid(), orgId, access.BranchId.Value, input.ProjectId, number, input.RequiredByDate, input.Note, access.ActorUserId, now);
                foreach (var line in BuildRequestLines(orgId, request.Id, resolved.Value!.Lines)) request.AddLine(line);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseRequestProjection>(ex);
            }

            _db.PurchaseRequests.Add(request);
            Audit(access, "purchase-request.created", "PurchaseRequest", request.Id, traceId,
                new { number, totalEstimated = request.TotalEstimatedAmount, lineCount = request.Lines.Count }, request.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, request.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<PurchaseRequestProjection>.Success((await GetPurchaseRequestAsync(orgId, request.Id, ct))!);
        });
    }

    public async Task<Result<PurchaseRequestProjection>> UpdatePurchaseRequestAsync(
        RequestAccessContext access, Guid purchaseRequestId, Guid expectedVersion, PurchaseRequestInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var request = await _db.PurchaseRequests.Include(r => r.Lines).FirstOrDefaultAsync(r => r.Id == purchaseRequestId && r.OrganizationId == orgId, ct);
            if (request is null) return Fail<PurchaseRequestProjection>("RESOURCE_NOT_FOUND", "Purchase request not found.");
            if (request.RowVersion != expectedVersion) return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_VERSION_CONFLICT", "Purchase request version conflict.");

            var resolved = await ResolveRequestAsync(orgId, input, ct);
            if (resolved.IsFailure) return Result<PurchaseRequestProjection>.Failure(resolved.Error);

            var now = _clock.UtcNow;
            List<PurchaseRequestLine> oldLines;
            List<PurchaseRequestLine> newLines;
            try
            {
                oldLines = request.Lines.ToList();
                request.EditDraft(input.ProjectId, input.RequiredByDate, input.Note, now);
                newLines = BuildRequestLines(orgId, request.Id, resolved.Value!.Lines);
                request.ReplaceLines(newLines);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseRequestProjection>(ex);
            }

            foreach (var old in oldLines) _db.Entry(old).State = EntityState.Detached;
            await _db.PurchaseRequestLines.Where(l => l.PurchaseRequestId == request.Id).ExecuteDeleteAsync(ct);
            _db.PurchaseRequestLines.AddRange(newLines);

            Audit(access, "purchase-request.updated", "PurchaseRequest", request.Id, traceId,
                new { number = request.Number, totalEstimated = request.TotalEstimatedAmount }, request.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_VERSION_CONFLICT", "Purchase request version conflict.");
            }

            return Result<PurchaseRequestProjection>.Success((await GetPurchaseRequestAsync(orgId, request.Id, ct))!);
        });
    }

    public async Task<Result<PurchaseRequestProjection>> PurchaseRequestActionAsync(
        RequestAccessContext access, Guid purchaseRequestId, Guid expectedVersion, PurchaseRequestAction action, string? note, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var request = await _db.PurchaseRequests.Include(r => r.Lines).FirstOrDefaultAsync(r => r.Id == purchaseRequestId && r.OrganizationId == orgId, ct);
            if (request is null) return Fail<PurchaseRequestProjection>("RESOURCE_NOT_FOUND", "Purchase request not found.");
            if (request.RowVersion != expectedVersion) return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_VERSION_CONFLICT", "Purchase request version conflict.");

            var now = _clock.UtcNow;
            try
            {
                switch (action)
                {
                    case PurchaseRequestAction.Submit:
                        request.Submit(now);
                        break;
                    case PurchaseRequestAction.Cancel:
                        request.Cancel(note ?? string.Empty, now);
                        break;
                    default:
                        if (action == PurchaseRequestAction.Approve)
                        {
                            if (request.Status != PurchaseRequestStatus.Submitted)
                            {
                                return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_INVALID_STATE", $"Only a submitted purchase request can be approved; current status is '{request.Status}'.");
                            }

                            if (access.ActorUserId == request.CreatedByUserId)
                            {
                                return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_SELF_APPROVAL", "A purchase request cannot be decided by its creator.");
                            }

                            if (request.ProjectId.HasValue)
                            {
                                var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProjectId.Value && p.OrganizationId == orgId, ct);
                                if (project is null) return Fail<PurchaseRequestProjection>("RESOURCE_NOT_FOUND", "Project not found.");
                                if (project.Status is not (ProjectStatus.Active or ProjectStatus.OnHold))
                                {
                                    return Fail<PurchaseRequestProjection>("PURCHASE_ORDER_PROJECT_NOT_ACTIVE", "Purchase requests can only be approved for an active or on-hold project.");
                                }
                            }

                            var authorityError = await _approvalAuthority.EnsureAsync(new ApprovalAuthorityCheck(
                                orgId, access.MembershipId, TanErp.Domain.IdentityAccess.ApprovalDocumentType.PurchaseOrder,
                                request.BranchId, request.TotalEstimatedAmount, request.Currency), ct);
                            if (authorityError is not null) return Result<PurchaseRequestProjection>.Failure(authorityError);
                        }

                        request.Decide(action == PurchaseRequestAction.Approve, access.ActorUserId, note, now);
                        break;
                }
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseRequestProjection>(ex);
            }

            Audit(access, $"purchase-request.{action.ToString().ToLowerInvariant()}", "PurchaseRequest", request.Id, traceId,
                new { number = request.Number, status = request.Status, totalEstimated = request.TotalEstimatedAmount }, request.RowVersion, now);
            if (action == PurchaseRequestAction.Submit)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.PurchaseRequestSubmitted(
                        orgId, request.BranchId, request.RowVersion, access.ActorUserId, request.Id, request.Number, request.CreatedByUserId),
                    ct);
            }

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_VERSION_CONFLICT", "Purchase request version conflict.");
            }
            catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
            {
                return Fail<PurchaseRequestProjection>("PURCHASE_REQUEST_VERSION_CONFLICT", "Purchase request version conflict.");
            }

            return Result<PurchaseRequestProjection>.Success((await GetPurchaseRequestAsync(orgId, request.Id, ct))!);
        });
    }

    public async Task<PurchaseRequestProjection?> GetPurchaseRequestAsync(Guid organizationId, Guid purchaseRequestId, CancellationToken ct = default)
    {
        var request = await _db.PurchaseRequests.AsNoTracking().Include(r => r.Lines)
            .FirstOrDefaultAsync(r => r.Id == purchaseRequestId && r.OrganizationId == organizationId, ct);
        if (request is null) return null;

        var project = request.ProjectId.HasValue
            ? await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProjectId.Value, ct)
            : null;
        var convertedPo = request.ConvertedPurchaseOrderId.HasValue
            ? await _db.PurchaseOrders.AsNoTracking()
                .Where(o => o.Id == request.ConvertedPurchaseOrderId.Value)
                .Select(o => new PurchaseRequestConvertedOrderProjection(o.Id, o.Number, o.Status))
                .FirstOrDefaultAsync(ct)
            : null;

        var userIds = new[] { request.CreatedByUserId, request.DecidedByUserId ?? Guid.Empty }.Where(id => id != Guid.Empty).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new ProcurementPerson(u.Id, u.DisplayName, u.Email), ct);
        ProcurementPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProcurementPerson(id, string.Empty, null);

        return new PurchaseRequestProjection(
            request.Id,
            request.BranchId,
            request.Number,
            request.Status,
            request.Currency,
            request.TotalEstimatedAmount,
            request.RequiredByDate,
            request.Note,
            project is null ? null : new PurchaseOrderProjectProjection(project.Id, project.Code, project.Name),
            Person(request.CreatedByUserId),
            request.CreatedAtUtc,
            request.SubmittedAtUtc,
            request.DecidedByUserId.HasValue ? Person(request.DecidedByUserId.Value) : null,
            request.DecidedAtUtc,
            request.DecisionNote,
            request.CancelReason,
            convertedPo,
            request.ConvertedAtUtc,
            request.RowVersion,
            request.Lines.OrderBy(l => l.LineNo).Select(l => new PurchaseRequestLineProjection(
                l.Id, l.LineNo, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode,
                l.Quantity, l.EstimatedUnitPrice, l.EstimatedLineTotal, l.Note)).ToList());
    }

    public async Task<PagedPurchaseRequests> ListPurchaseRequestsAsync(Guid organizationId, PurchaseRequestListQuery query, CancellationToken ct = default)
    {
        var requests = _db.PurchaseRequests.AsNoTracking().Where(r => r.OrganizationId == organizationId);
        if (query.Status is not null) requests = requests.Where(r => r.Status == query.Status);
        if (query.ProjectId.HasValue) requests = requests.Where(r => r.ProjectId == query.ProjectId.Value);

        var joined =
            from r in requests
            join p in _db.Projects.AsNoTracking() on r.ProjectId equals (Guid?)p.Id into projects
            from p in projects.DefaultIfEmpty()
            join po in _db.PurchaseOrders.AsNoTracking() on r.ConvertedPurchaseOrderId equals (Guid?)po.Id into orders
            from po in orders.DefaultIfEmpty()
            select new
            {
                Request = r,
                ProjectCode = p == null ? null : p.Code,
                ConvertedOrderNumber = po == null ? null : po.Number
            };

        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            joined = joined.Where(x => x.Request.Number.ToLower().Contains(needle)
                || (x.ProjectCode != null && x.ProjectCode.ToLower().Contains(needle))
                || (x.Request.Note != null && x.Request.Note.ToLower().Contains(needle)));
        }

        var total = await joined.CountAsync(ct);
        var rows = await joined.OrderByDescending(x => x.Request.CreatedAtUtc).ThenByDescending(x => x.Request.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedPurchaseRequests(
            rows.Select(x => new PurchaseRequestListItemProjection(
                x.Request.Id, x.Request.BranchId, x.Request.Number, x.Request.Status, x.Request.TotalEstimatedAmount,
                x.ProjectCode, x.Request.RequiredByDate, x.Request.ConvertedPurchaseOrderId, x.ConvertedOrderNumber, x.Request.CreatedAtUtc)).ToList(),
            total, query.Page, query.PageSize);
    }

    public async Task<Result<PurchaseOrderProjection>> ConvertPurchaseRequestsAsync(
        RequestAccessContext access, ConvertPurchaseRequestsInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "procurement.purchase-request.convert";
        var orgId = access.OrganizationId;
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

            foreach (var prId in input.PurchaseRequestIds.OrderBy(x => x))
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"SELECT 1 FROM procurement.purchase_requests WHERE id = {prId} AND organization_id = {orgId} FOR UPDATE", ct);
            }

            var prsById = await _db.PurchaseRequests.Include(r => r.Lines)
                .Where(r => r.OrganizationId == orgId && input.PurchaseRequestIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, ct);

            if (prsById.Count != input.PurchaseRequestIds.Count)
            {
                return Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "One or more purchase requests were not found.");
            }

            var orderedPrs = input.PurchaseRequestIds.Select(id => prsById[id]).ToList();
            if (orderedPrs.Any(r => r.Status != PurchaseRequestStatus.Approved))
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_REQUEST_INVALID_STATE", "Only approved purchase requests can be converted into a purchase order.");
            }

            var distinctBranches = orderedPrs.Select(r => r.BranchId).Distinct().ToList();
            if (distinctBranches.Count > 1)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE", "Purchase requests from different branches cannot be consolidated into one purchase order.");
            }

            var nonNullProjects = orderedPrs.Where(r => r.ProjectId.HasValue).Select(r => r.ProjectId!.Value).Distinct().ToList();
            if (nonNullProjects.Count > 1)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_REQUEST_CONVERSION_INCOMPATIBLE", "Purchase requests from different projects cannot be consolidated into one purchase order.");
            }

            var targetBranchId = distinctBranches[0];
            Guid? targetProjectId = nonNullProjects.Count == 1 ? nonNullProjects[0] : null;
            DateOnly? expectedDeliveryDate = input.ExpectedDeliveryDate
                ?? orderedPrs.Where(r => r.RequiredByDate.HasValue).Select(r => r.RequiredByDate!.Value).DefaultIfEmpty().Min();
            if (expectedDeliveryDate == default) expectedDeliveryDate = null;

            var overrideMap = input.UnitPriceOverrides?.ToDictionary(o => o.ItemId, o => o.UnitPrice)
                ?? new Dictionary<Guid, decimal>();

            var consolidatedGroups = orderedPrs
                .SelectMany(r => r.Lines.OrderBy(l => l.LineNo))
                .GroupBy(l => l.ItemId)
                .ToList();

            var consolidatedItemSet = consolidatedGroups.Select(g => g.Key).ToHashSet();
            if (overrideMap.Keys.Any(k => !consolidatedItemSet.Contains(k)))
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_LINE_INVALID", "A unit price override references an item not present on the selected purchase requests.");
            }

            var consolidatedLineInputs = new List<PurchaseOrderLineInput>();
            foreach (var group in consolidatedGroups)
            {
                var totalQty = decimal.Round(group.Sum(l => l.Quantity), 4, MidpointRounding.AwayFromZero);
                decimal unitPrice;
                if (overrideMap.TryGetValue(group.Key, out var overridden))
                {
                    unitPrice = decimal.Round(overridden, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    var weightedTotal = group.Sum(l => l.Quantity * l.EstimatedUnitPrice);
                    unitPrice = totalQty > 0m
                        ? decimal.Round(weightedTotal / totalQty, 4, MidpointRounding.AwayFromZero)
                        : 0m;
                }

                consolidatedLineInputs.Add(new PurchaseOrderLineInput(group.Key, totalQty, unitPrice));
            }

            if (consolidatedLineInputs.Count is < 1 or > 200)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_LINE_INVALID", "Consolidated purchase order must contain between 1 and 200 distinct items.");
            }

            var orderInput = new PurchaseOrderInput(input.SupplierId, targetProjectId, expectedDeliveryDate, input.Note, consolidatedLineInputs);
            var resolved = await ResolveOrderAsync(orgId, orderInput, ct);
            if (resolved.IsFailure) return Result<PurchaseOrderProjection>.Failure(resolved.Error);

            var now = _clock.UtcNow;
            string poNumber;
            try
            {
                poNumber = await _numbers.GenerateAsync(orgId, DocumentTypes.PurchaseOrders, targetBranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<PurchaseOrderProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate purchase order document number.");
            }

            PurchaseOrder order;
            try
            {
                order = new PurchaseOrder(Guid.NewGuid(), orgId, targetBranchId, input.SupplierId, targetProjectId, poNumber, expectedDeliveryDate, input.Note, access.ActorUserId, now);
                foreach (var line in BuildLines(orgId, order.Id, resolved.Value!.Lines)) order.AddLine(line);
                foreach (var pr in orderedPrs)
                {
                    pr.MarkConverted(order.Id, now);
                }
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseOrderProjection>(ex);
            }

            _db.PurchaseOrders.Add(order);
            Audit(access, "purchase-order.created", "PurchaseOrder", order.Id, traceId,
                new { number = poNumber, total = order.TotalAmount, lineCount = order.Lines.Count, sourcePurchaseRequests = orderedPrs.Select(p => p.Number).ToArray() },
                order.RowVersion, now);
            foreach (var pr in orderedPrs)
            {
                Audit(access, "purchase-request.converted", "PurchaseRequest", pr.Id, traceId,
                    new { number = pr.Number, purchaseOrderId = order.Id, purchaseOrderNumber = poNumber }, pr.RowVersion, now);
            }

            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, order.Id.ToString(), now));
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_REQUEST_VERSION_CONFLICT", "A purchase request changed concurrently during conversion.");
            }

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
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
            var wasCommitted = CommittedStatuses.Contains(order.Status);
            try
            {
                switch (action)
                {
                    case PurchaseOrderAction.Submit:
                        order.Submit(now);
                        break;
                    case PurchaseOrderAction.Cancel:
                        if (await _db.GoodsReceipts.AsNoTracking().AnyAsync(r => r.OrganizationId == orgId && r.PurchaseOrderId == order.Id, ct))
                        {
                            return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_HAS_RECEIPTS", "A purchase order with receipts cannot be cancelled.");
                        }

                        order.Cancel(note ?? string.Empty, now);
                        if (wasCommitted && order.ProjectId.HasValue)
                        {
                            var remainingCommitment = decimal.Round(order.Lines.Sum(l => l.RemainingQuantity * l.UnitPrice), 2);
                            if (remainingCommitment > 0m)
                            {
                                _db.ProjectCostLedgerEntries.Add(new ProjectCostLedgerEntry(
                                    Guid.NewGuid(), orgId, order.ProjectId.Value, null,
                                    ProjectBudgetCategory.Material, ProjectCostEntryType.Commitment, ProjectCostSourceType.PurchaseOrder,
                                    order.Id, order.Number, $"Purchase order {order.Number} cancelled",
                                    -remainingCommitment, true, access.ActorUserId, now));
                            }
                        }
                        break;
                    default:
                        if (action == PurchaseOrderAction.Approve)
                        {
                            var budgetFailure = await CheckApprovalAsync(orgId, order, ct);
                            if (budgetFailure is not null) return Result<PurchaseOrderProjection>.Failure(budgetFailure);

                            var authorityError = await _approvalAuthority.EnsureAsync(new ApprovalAuthorityCheck(
                                orgId, access.MembershipId, TanErp.Domain.IdentityAccess.ApprovalDocumentType.PurchaseOrder,
                                order.BranchId, order.TotalAmount, order.Currency), ct);
                            if (authorityError is not null) return Result<PurchaseOrderProjection>.Failure(authorityError);
                        }

                        order.Decide(action == PurchaseOrderAction.Approve, access.ActorUserId, note, now);
                        if (action == PurchaseOrderAction.Approve && order.ProjectId.HasValue && order.TotalAmount > 0m)
                        {
                            _db.ProjectCostLedgerEntries.Add(new ProjectCostLedgerEntry(
                                Guid.NewGuid(), orgId, order.ProjectId.Value, null,
                                ProjectBudgetCategory.Material, ProjectCostEntryType.Commitment, ProjectCostSourceType.PurchaseOrder,
                                order.Id, order.Number, $"Purchase order {order.Number} approved",
                                order.TotalAmount, false, access.ActorUserId, now));
                        }
                        break;
                }
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<PurchaseOrderProjection>(ex);
            }

            Audit(access, $"purchase-order.{action.ToString().ToLowerInvariant()}", "PurchaseOrder", order.Id, traceId,
                new { number = order.Number, status = order.Status, total = order.TotalAmount }, order.RowVersion, now);
            if (action == PurchaseOrderAction.Submit)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.PurchaseOrderSubmitted(
                        orgId, order.BranchId, order.RowVersion, access.ActorUserId, order.Id, order.Number, order.CreatedByUserId),
                    ct);
            }

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            }
            catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
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

        var returns = await _db.SupplierReturns.AsNoTracking().Include(r => r.Lines)
            .Where(r => r.OrganizationId == organizationId && r.PurchaseOrderId == order.Id)
            .OrderByDescending(r => r.ReturnedAtUtc).ThenByDescending(r => r.Id).ToListAsync(ct);

        var invoices = await _db.SupplierInvoices.AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && i.PurchaseOrderId == order.Id)
            .OrderByDescending(i => i.CreatedAtUtc).ThenByDescending(i => i.Id).ToListAsync(ct);

        var sourceRequests = await _db.PurchaseRequests.AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && r.ConvertedPurchaseOrderId == order.Id)
            .OrderBy(r => r.Number)
            .Select(r => new PurchaseOrderSourceRequestProjection(r.Id, r.Number, r.ConvertedAtUtc))
            .ToListAsync(ct);

        var receiptIds = receipts.Select(r => (Guid?)r.Id).ToList();
        var stockDocuments = await _db.StockDocuments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId && d.SourceType == StockSourceType.GoodsReceipt && receiptIds.Contains(d.SourceId))
            .ToDictionaryAsync(d => d.SourceId!.Value, d => d.Number, ct);

        var userIds = new[] { order.CreatedByUserId, order.DecidedByUserId ?? Guid.Empty }
            .Concat(receipts.Select(r => r.ReceivedByUserId))
            .Concat(returns.Select(r => r.ReturnedByUserId))
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new ProcurementPerson(u.Id, u.DisplayName, u.Email), ct);
        ProcurementPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProcurementPerson(id, string.Empty, null);

        var poLineById = order.Lines.ToDictionary(l => l.Id);
        var grNumberById = receipts.ToDictionary(r => r.Id, r => r.Number);

        return new PurchaseOrderProjection(
            order.Id, order.BranchId, order.Number, order.Status, order.Currency, order.TotalAmount, order.ExpectedDeliveryDate, order.Note,
            new PurchaseOrderSupplierProjection(supplier.Id, supplier.Code, supplier.NameTh, supplier.NameEn),
            project is null ? null : new PurchaseOrderProjectProjection(project.Id, project.Code, project.Name),
            Person(order.CreatedByUserId), order.CreatedAtUtc, order.SubmittedAtUtc,
            order.DecidedByUserId.HasValue ? Person(order.DecidedByUserId.Value) : null,
            order.DecidedAtUtc, order.DecisionNote, order.CancelReason, order.RowVersion,
            order.Lines.OrderBy(l => l.LineNo).Select(l => new PurchaseOrderLineProjection(
                l.Id, l.LineNo, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode,
                l.Quantity, l.UnitPrice, l.LineTotal, l.ReceivedQuantity, l.RemainingQuantity, l.InvoicedQuantity, l.UninvoicedReceivedQuantity)).ToList(),
            receipts.Select(r => new GoodsReceiptSummaryProjection(
                r.Id, r.Number, r.ReceivedAtUtc, r.Note, Person(r.ReceivedByUserId), r.Lines.Count, r.Lines.Sum(l => l.Quantity),
                stockDocuments.TryGetValue(r.Id, out var stockNumber) ? stockNumber : null,
                r.Lines.Select(gl =>
                {
                    poLineById.TryGetValue(gl.PurchaseOrderLineId, out var pl);
                    return new GoodsReceiptLineProjection(
                        gl.Id, gl.PurchaseOrderLineId, gl.ItemId, pl?.ItemCode ?? string.Empty, pl?.ItemNameTh ?? string.Empty,
                        pl?.UnitCode ?? string.Empty, gl.Quantity, gl.UnitPrice, gl.ReturnedQuantity, gl.RemainingReturnableQuantity);
                }).ToList())).ToList(),
            returns.Select(sr => new SupplierReturnProjection(
                sr.Id, sr.Number, sr.GoodsReceiptId, grNumberById.TryGetValue(sr.GoodsReceiptId, out var grNo) ? grNo : string.Empty,
                sr.ReturnedAtUtc, sr.Reason, sr.TotalReturnAmount, sr.StockDocumentNumber, Person(sr.ReturnedByUserId),
                sr.Lines.Select(sl =>
                {
                    poLineById.TryGetValue(sl.PurchaseOrderLineId, out var pl);
                    return new SupplierReturnLineProjection(
                        sl.Id, sl.GoodsReceiptLineId, sl.PurchaseOrderLineId, sl.ItemId, pl?.ItemCode ?? string.Empty,
                        pl?.ItemNameTh ?? string.Empty, pl?.UnitCode ?? string.Empty, sl.Quantity, sl.UnitPrice, sl.LineReturnAmount);
                }).ToList())).ToList(),
            invoices.Select(inv => new PurchaseOrderInvoiceSummaryProjection(
                inv.Id, inv.Number, inv.SupplierInvoiceNumber, inv.Status, inv.MatchStatus, inv.InvoiceDate,
                inv.SubtotalAmount, inv.TaxAmount, inv.TotalAmount, inv.TotalVarianceAmount, inv.CreatedAtUtc)).ToList(),
            sourceRequests);
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

    // ===== goods receipts & supplier returns =========================================================

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

            await LockPurchaseOrderAsync(orgId, purchaseOrderId, ct);
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
            if (order.ProjectId.HasValue)
            {
                var receiptValue = decimal.Round(receipt.Lines.Sum(l => l.Quantity * l.UnitPrice), 2);
                if (receiptValue > 0m)
                {
                    _db.ProjectCostLedgerEntries.Add(new ProjectCostLedgerEntry(
                        Guid.NewGuid(), orgId, order.ProjectId.Value, null,
                        ProjectBudgetCategory.Material, ProjectCostEntryType.Commitment, ProjectCostSourceType.GoodsReceipt,
                        receipt.Id, receipt.Number, $"Goods receipt {receipt.Number} against {order.Number}",
                        -receiptValue, true, access.ActorUserId, now));
                }
            }

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
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation })
            {
                return Fail<PurchaseOrderProjection>("GOODS_RECEIPT_OVER_RECEIVED", "A line would be received above the ordered quantity.");
            }

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
    }

    public async Task<Result<PurchaseOrderProjection>> PostSupplierReturnAsync(
        RequestAccessContext access,
        Guid purchaseOrderId,
        Guid expectedVersion,
        SupplierReturnInput input,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken ct = default)
    {
        const string operation = "procurement.supplier-return.post";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<PurchaseOrderProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = await GetPurchaseOrderAsync(orgId, purchaseOrderId, ct);
                if (replayed is not null) return Result<PurchaseOrderProjection>.Success(replayed);
            }

            await LockPurchaseOrderAsync(orgId, purchaseOrderId, ct);
            var order = await _db.PurchaseOrders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == purchaseOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.");
            if (order.RowVersion != expectedVersion) return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");

            var receipt = await _db.GoodsReceipts.Include(r => r.Lines)
                .FirstOrDefaultAsync(r => r.Id == input.GoodsReceiptId && r.OrganizationId == orgId, ct);
            if (receipt is null) return Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "Goods receipt not found.");
            if (receipt.PurchaseOrderId != order.Id)
            {
                return Fail<PurchaseOrderProjection>("SUPPLIER_RETURN_INVALID", "The specified goods receipt does not belong to this purchase order.");
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.SupplierReturns, order.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<PurchaseOrderProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate supplier return document number.");
            }

            SupplierReturn supplierReturn;
            try
            {
                supplierReturn = new SupplierReturn(
                    Guid.NewGuid(), orgId, order.BranchId, order.Id, receipt.Id, order.SupplierId, order.ProjectId,
                    number, input.ReturnedAtUtc ?? now, input.Reason, access.ActorUserId, now);

                receipt.ApplyReturn(input.Lines.Select(l => (l.GoodsReceiptLineId, l.Quantity)).ToList());

                var grLinesById = receipt.Lines.ToDictionary(l => l.Id);
                var poReturnQuantities = new List<(Guid LineId, decimal Quantity)>();
                foreach (var line in input.Lines)
                {
                    var grLine = grLinesById[line.GoodsReceiptLineId];
                    supplierReturn.AddLine(new SupplierReturnLine(
                        Guid.NewGuid(), orgId, supplierReturn.Id, grLine.Id, grLine.PurchaseOrderLineId,
                        grLine.ItemId, grLine.UnitId, line.Quantity, grLine.UnitPrice));
                    poReturnQuantities.Add((grLine.PurchaseOrderLineId, line.Quantity));
                }

                order.ApplyReturn(poReturnQuantities, now);
            }
            catch (ProcurementDomainException ex)
            {
                _db.ChangeTracker.Clear();
                return Fail<PurchaseOrderProjection>(ex);
            }

            var grStockDoc = await _db.StockDocuments.AsNoTracking()
                .FirstOrDefaultAsync(d => d.OrganizationId == orgId
                    && d.SourceType == StockSourceType.GoodsReceipt
                    && d.SourceId == receipt.Id, ct);

            if (grStockDoc is not null)
            {
                var stockLines = supplierReturn.Lines
                    .Select(l => new ProcurementReturnStockLine(l.ItemId, l.Quantity))
                    .ToList();
                var stockResult = await _stockPort.ReturnToSupplierAsync(
                    access,
                    grStockDoc.WarehouseId,
                    order.ProjectId,
                    supplierReturn.Id,
                    $"Supplier return {supplierReturn.Number}: {supplierReturn.Reason}",
                    supplierReturn.ReturnedAtUtc,
                    stockLines,
                    traceId,
                    ct);
                if (stockResult.IsFailure)
                {
                    _db.ChangeTracker.Clear();
                    return Result<PurchaseOrderProjection>.Failure(stockResult.Error);
                }

                supplierReturn.SetStockDocument(stockResult.Value!.DocumentId, stockResult.Value.DocumentNumber);
            }

            _db.SupplierReturns.Add(supplierReturn);
            if (order.ProjectId.HasValue && supplierReturn.TotalReturnAmount > 0m)
            {
                _db.ProjectCostLedgerEntries.Add(new ProjectCostLedgerEntry(
                    Guid.NewGuid(), orgId, order.ProjectId.Value, null,
                    ProjectBudgetCategory.Material, ProjectCostEntryType.Commitment, ProjectCostSourceType.SupplierReturn,
                    supplierReturn.Id, supplierReturn.Number,
                    $"Supplier return {supplierReturn.Number} against {order.Number}",
                    supplierReturn.TotalReturnAmount, true, access.ActorUserId, now));
            }

            Audit(access, "supplier-return.posted", "SupplierReturn", supplierReturn.Id, traceId,
                new
                {
                    number = supplierReturn.Number,
                    purchaseOrderNumber = order.Number,
                    goodsReceiptNumber = receipt.Number,
                    totalReturnAmount = supplierReturn.TotalReturnAmount,
                    stockDocumentNumber = supplierReturn.StockDocumentNumber,
                    orderStatus = order.Status
                },
                order.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, supplierReturn.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_VERSION_CONFLICT", "Purchase order version conflict.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation })
            {
                return Fail<PurchaseOrderProjection>("SUPPLIER_RETURN_OVER_RETURNED", "Returned quantity exceeds the allowed returnable quantity.");
            }

            return Result<PurchaseOrderProjection>.Success((await GetPurchaseOrderAsync(orgId, order.Id, ct))!);
        });
    }

    // ===== supplier invoices =========================================================================

    private static readonly string[] InactiveSupplierInvoiceStatuses =
    {
        SupplierInvoiceStatus.Cancelled, SupplierInvoiceStatus.Voided, SupplierInvoiceStatus.Rejected
    };

    private Task<bool> SupplierInvoiceNumberExistsAsync(Guid orgId, Guid supplierId, string normalizedInvoiceNumber, Guid? excludeInvoiceId, CancellationToken ct) =>
        _db.SupplierInvoices.AsNoTracking().AnyAsync(i =>
            i.OrganizationId == orgId
            && i.SupplierId == supplierId
            && i.NormalizedSupplierInvoiceNumber == normalizedInvoiceNumber
            && !InactiveSupplierInvoiceStatuses.Contains(i.Status)
            && (!excludeInvoiceId.HasValue || i.Id != excludeInvoiceId.Value), ct);

    private static List<SupplierInvoiceLine> BuildInvoiceLines(Guid orgId, Guid invoiceId, PurchaseOrder order, IReadOnlyList<SupplierInvoiceLineInput> lineInputs)
    {
        var poLinesById = order.Lines.ToDictionary(l => l.Id);
        var built = new List<SupplierInvoiceLine>();
        for (var i = 0; i < lineInputs.Count; i++)
        {
            var input = lineInputs[i];
            if (!poLinesById.TryGetValue(input.PurchaseOrderLineId, out var poLine))
            {
                throw new ProcurementDomainException("SUPPLIER_INVOICE_LINE_INVALID", "An invoice line does not belong to the referenced purchase order.");
            }

            built.Add(new SupplierInvoiceLine(
                Guid.NewGuid(), orgId, invoiceId, i + 1, poLine.Id, poLine.ItemId, poLine.ItemCode, poLine.ItemNameTh,
                poLine.UnitId, poLine.UnitCode, input.Quantity, poLine.UnitPrice, input.UnitPrice));
        }

        return built;
    }

    public async Task<Result<SupplierInvoiceProjection>> CreateSupplierInvoiceAsync(
        RequestAccessContext access, SupplierInvoiceInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "procurement.supplier-invoice.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<SupplierInvoiceProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = Guid.TryParse(replay.ResourceId, out var id) ? await GetSupplierInvoiceAsync(orgId, id, ct) : null;
                if (replayed is not null) return Result<SupplierInvoiceProjection>.Success(replayed);
            }

            var order = await _db.PurchaseOrders.AsNoTracking().Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == input.PurchaseOrderId && o.OrganizationId == orgId, ct);
            if (order is null) return Fail<SupplierInvoiceProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.");
            if (order.Status is not (PurchaseOrderStatus.Approved or PurchaseOrderStatus.PartiallyReceived or PurchaseOrderStatus.Received))
            {
                return Fail<SupplierInvoiceProjection>("PURCHASE_ORDER_INVALID_STATE", $"Supplier invoices cannot be created against a '{order.Status}' purchase order.");
            }

            var normalizedInvNo = (input.SupplierInvoiceNumber ?? string.Empty).Trim().ToUpperInvariant();
            if (await SupplierInvoiceNumberExistsAsync(orgId, order.SupplierId, normalizedInvNo, null, ct))
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_DUPLICATE_NUMBER", "An active supplier invoice with this number already exists for this supplier.");
            }

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.SupplierInvoices, order.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<SupplierInvoiceProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate supplier invoice document number.");
            }

            SupplierInvoice invoice;
            try
            {
                invoice = new SupplierInvoice(
                    Guid.NewGuid(), orgId, order.BranchId, order.Id, order.SupplierId, order.ProjectId,
                    number, input.SupplierInvoiceNumber, input.InvoiceDate, input.DueDate, input.TaxAmount, input.Note, access.ActorUserId, now);
                foreach (var line in BuildInvoiceLines(orgId, invoice.Id, order, input.Lines))
                {
                    invoice.AddLine(line);
                }
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<SupplierInvoiceProjection>(ex);
            }

            _db.SupplierInvoices.Add(invoice);
            Audit(access, "supplier-invoice.created", "SupplierInvoice", invoice.Id, traceId,
                new { number, supplierInvoiceNumber = invoice.SupplierInvoiceNumber, purchaseOrderNumber = order.Number, totalAmount = invoice.TotalAmount },
                invoice.RowVersion, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, invoice.Id.ToString(), now));

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_DUPLICATE_NUMBER", "An active supplier invoice with this number already exists for this supplier.");
            }

            return Result<SupplierInvoiceProjection>.Success((await GetSupplierInvoiceAsync(orgId, invoice.Id, ct))!);
        });
    }

    public async Task<Result<SupplierInvoiceProjection>> UpdateSupplierInvoiceAsync(
        RequestAccessContext access, Guid supplierInvoiceId, Guid expectedVersion, SupplierInvoiceInput input, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var invoice = await _db.SupplierInvoices.Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == supplierInvoiceId && i.OrganizationId == orgId, ct);
            if (invoice is null) return Fail<SupplierInvoiceProjection>("RESOURCE_NOT_FOUND", "Supplier invoice not found.");
            if (invoice.RowVersion != expectedVersion) return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_VERSION_CONFLICT", "Supplier invoice version conflict.");
            if (invoice.PurchaseOrderId != input.PurchaseOrderId)
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_LINE_INVALID", "The purchase order of a supplier invoice cannot be changed.");
            }

            var normalizedInvNo = (input.SupplierInvoiceNumber ?? string.Empty).Trim().ToUpperInvariant();
            if (await SupplierInvoiceNumberExistsAsync(orgId, invoice.SupplierId, normalizedInvNo, invoice.Id, ct))
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_DUPLICATE_NUMBER", "An active supplier invoice with this number already exists for this supplier.");
            }

            var order = await _db.PurchaseOrders.AsNoTracking().Include(o => o.Lines)
                .FirstAsync(o => o.Id == invoice.PurchaseOrderId && o.OrganizationId == orgId, ct);

            var now = _clock.UtcNow;
            List<SupplierInvoiceLine> oldLines;
            List<SupplierInvoiceLine> newLines;
            try
            {
                oldLines = invoice.Lines.ToList();
                invoice.EditDraft(input.SupplierInvoiceNumber, input.InvoiceDate, input.DueDate, input.TaxAmount, input.Note, now);
                newLines = BuildInvoiceLines(orgId, invoice.Id, order, input.Lines);
                invoice.ReplaceLines(newLines);
            }
            catch (ProcurementDomainException ex)
            {
                return Fail<SupplierInvoiceProjection>(ex);
            }

            foreach (var old in oldLines) _db.Entry(old).State = EntityState.Detached;
            await _db.SupplierInvoiceLines.Where(l => l.SupplierInvoiceId == invoice.Id).ExecuteDeleteAsync(ct);
            _db.SupplierInvoiceLines.AddRange(newLines);

            Audit(access, "supplier-invoice.updated", "SupplierInvoice", invoice.Id, traceId,
                new { number = invoice.Number, supplierInvoiceNumber = invoice.SupplierInvoiceNumber, totalAmount = invoice.TotalAmount },
                invoice.RowVersion, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_VERSION_CONFLICT", "Supplier invoice version conflict.");
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_DUPLICATE_NUMBER", "An active supplier invoice with this number already exists for this supplier.");
            }

            return Result<SupplierInvoiceProjection>.Success((await GetSupplierInvoiceAsync(orgId, invoice.Id, ct))!);
        });
    }

    public async Task<Result<SupplierInvoiceProjection>> SupplierInvoiceActionAsync(
        RequestAccessContext access, Guid supplierInvoiceId, Guid expectedVersion, SupplierInvoiceAction action, string? note, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var targetPoId = await _db.SupplierInvoices.AsNoTracking()
                .Where(i => i.Id == supplierInvoiceId && i.OrganizationId == orgId)
                .Select(i => (Guid?)i.PurchaseOrderId)
                .FirstOrDefaultAsync(ct);
            if (!targetPoId.HasValue) return Fail<SupplierInvoiceProjection>("RESOURCE_NOT_FOUND", "Supplier invoice not found.");

            // Lock PO before SINV in a fixed order so concurrent invoice submissions and returns never deadlock.
            await LockPurchaseOrderAsync(orgId, targetPoId.Value, ct);
            await LockSupplierInvoiceAsync(orgId, supplierInvoiceId, ct);

            var invoice = await _db.SupplierInvoices.Include(i => i.Lines)
                .FirstAsync(i => i.Id == supplierInvoiceId && i.OrganizationId == orgId, ct);
            if (invoice.RowVersion != expectedVersion)
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_VERSION_CONFLICT", "Supplier invoice version conflict.");
            }

            var order = await _db.PurchaseOrders.Include(o => o.Lines)
                .FirstAsync(o => o.Id == invoice.PurchaseOrderId && o.OrganizationId == orgId, ct);

            var now = _clock.UtcNow;
            var lineQuantities = invoice.Lines.Select(l => (l.PurchaseOrderLineId, l.Quantity)).ToList();

            try
            {
                switch (action)
                {
                    case SupplierInvoiceAction.Submit:
                        if (invoice.Status != SupplierInvoiceStatus.Draft)
                        {
                            return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_INVALID_STATE", $"Only a draft supplier invoice can be submitted; current status is '{invoice.Status}'.");
                        }

                        if (await SupplierInvoiceNumberExistsAsync(orgId, invoice.SupplierId, invoice.NormalizedSupplierInvoiceNumber, invoice.Id, ct))
                        {
                            return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_DUPLICATE_NUMBER", "An active supplier invoice with this number already exists for this supplier.");
                        }

                        order.ReserveInvoiced(lineQuantities, now);
                        invoice.Submit(order.Lines.ToDictionary(l => l.Id, l => l.UnitPrice), now);

                        if (invoice.Status == SupplierInvoiceStatus.Matched)
                        {
                            await PostApprovedOrMatchedInvoiceEffectsAsync(orgId, invoice, order, access.ActorUserId, now, ct);
                        }
                        else
                        {
                            await _notifications.PublishAsync(
                                NotificationEvents.SupplierInvoiceHeldForApproval(
                                    orgId, invoice.BranchId, invoice.RowVersion, access.ActorUserId, invoice.Id, invoice.Number, invoice.CreatedByUserId),
                                ct);
                        }
                        break;

                    case SupplierInvoiceAction.Approve:
                        if (invoice.Status != SupplierInvoiceStatus.OnHold)
                        {
                            return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_INVALID_STATE", $"Only an on-hold supplier invoice can be approved; current status is '{invoice.Status}'.");
                        }

                        if (access.ActorUserId == invoice.CreatedByUserId)
                        {
                            return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_SELF_APPROVAL", "A supplier invoice cannot be decided by its creator.");
                        }

                        var authorityError = await _approvalAuthority.EnsureAsync(new ApprovalAuthorityCheck(
                            orgId, access.MembershipId, TanErp.Domain.IdentityAccess.ApprovalDocumentType.PurchaseOrder,
                            invoice.BranchId, invoice.TotalAmount, invoice.Currency), ct);
                        if (authorityError is not null) return Result<SupplierInvoiceProjection>.Failure(authorityError);

                        invoice.DecideHold(true, access.ActorUserId, note, now);
                        await PostApprovedOrMatchedInvoiceEffectsAsync(orgId, invoice, order, access.ActorUserId, now, ct);
                        break;

                    case SupplierInvoiceAction.Reject:
                        invoice.DecideHold(false, access.ActorUserId, note, now);
                        order.ReleaseInvoiced(lineQuantities, now);
                        break;

                    case SupplierInvoiceAction.Cancel:
                        invoice.Cancel(note, now);
                        break;

                    case SupplierInvoiceAction.Void:
                        invoice.Void(note, access.ActorUserId, now);
                        order.ReleaseInvoiced(lineQuantities, now);
                        EnqueueOutbox(
                            orgId,
                            $"supplier-invoice.voided:{invoice.Id}",
                            OutboxKind.SupplierInvoiceVoided,
                            invoice.Id,
                            invoice.Number,
                            invoice.TotalAmount,
                            new
                            {
                                kind = OutboxKind.SupplierInvoiceVoided,
                                number = invoice.Number,
                                supplierInvoiceNumber = invoice.SupplierInvoiceNumber,
                                purchaseOrderNumber = order.Number,
                                supplierId = invoice.SupplierId,
                                subtotalAmount = invoice.SubtotalAmount,
                                taxAmount = invoice.TaxAmount,
                                totalAmount = invoice.TotalAmount,
                                currency = invoice.Currency,
                                voidedAtUtc = now
                            },
                            now);

                        if (invoice.ProjectId.HasValue && invoice.TotalVarianceAmount != 0m)
                        {
                            _db.ProjectCostLedgerEntries.Add(new ProjectCostLedgerEntry(
                                Guid.NewGuid(), orgId, invoice.ProjectId.Value, null,
                                ProjectBudgetCategory.Material, ProjectCostEntryType.Actual, ProjectCostSourceType.SupplierInvoice,
                                invoice.Id, invoice.Number,
                                $"Voided supplier invoice {invoice.Number} price variance reversal",
                                -invoice.TotalVarianceAmount,
                                isReversal: true,
                                access.ActorUserId, now));
                        }
                        break;
                }
            }
            catch (ProcurementDomainException ex)
            {
                _db.ChangeTracker.Clear();
                return Fail<SupplierInvoiceProjection>(ex);
            }

            Audit(access, $"supplier-invoice.{action.ToString().ToLowerInvariant()}", "SupplierInvoice", invoice.Id, traceId,
                new
                {
                    number = invoice.Number,
                    status = invoice.Status,
                    matchStatus = invoice.MatchStatus,
                    totalAmount = invoice.TotalAmount,
                    totalVarianceAmount = invoice.TotalVarianceAmount
                },
                invoice.RowVersion, now);

            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_VERSION_CONFLICT", "Supplier invoice version conflict.");
            }
            catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_VERSION_CONFLICT", "Supplier invoice version conflict.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation })
            {
                return Fail<SupplierInvoiceProjection>("SUPPLIER_INVOICE_OVER_RECEIVED", "Invoice quantity exceeds uninvoiced received quantity.");
            }

            return Result<SupplierInvoiceProjection>.Success((await GetSupplierInvoiceAsync(orgId, invoice.Id, ct))!);
        });
    }

    private async Task PostApprovedOrMatchedInvoiceEffectsAsync(
        Guid orgId, SupplierInvoice invoice, PurchaseOrder order, Guid actorUserId, DateTimeOffset now, CancellationToken ct)
    {
        EnqueueOutbox(
            orgId,
            $"supplier-invoice.posted:{invoice.Id}",
            OutboxKind.SupplierInvoicePosted,
            invoice.Id,
            invoice.Number,
            invoice.TotalAmount,
            new
            {
                kind = OutboxKind.SupplierInvoicePosted,
                number = invoice.Number,
                supplierInvoiceNumber = invoice.SupplierInvoiceNumber,
                purchaseOrderNumber = order.Number,
                supplierId = invoice.SupplierId,
                subtotalAmount = invoice.SubtotalAmount,
                taxAmount = invoice.TaxAmount,
                totalAmount = invoice.TotalAmount,
                currency = invoice.Currency,
                invoiceDate = invoice.InvoiceDate,
                dueDate = invoice.DueDate,
                postedAtUtc = now
            },
            now);

        if (invoice.ProjectId.HasValue && invoice.TotalVarianceAmount != 0m)
        {
            var entry = new ProjectCostLedgerEntry(
                Guid.NewGuid(), orgId, invoice.ProjectId.Value, null,
                ProjectBudgetCategory.Material, ProjectCostEntryType.Actual, ProjectCostSourceType.SupplierInvoice,
                invoice.Id, invoice.Number,
                $"Supplier invoice {invoice.Number} price variance against {order.Number}",
                invoice.TotalVarianceAmount,
                isReversal: invoice.TotalVarianceAmount < 0m,
                actorUserId, now);
            _db.ProjectCostLedgerEntries.Add(entry);

            if (invoice.TotalVarianceAmount > 0m)
            {
                var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.OrganizationId == orgId && p.Id == invoice.ProjectId.Value, ct);
                if (project is not null && project.BaselineBudgetTotal.HasValue)
                {
                    var approvedDelta = await _db.ProjectChangeOrders.AsNoTracking()
                        .Where(c => c.OrganizationId == orgId && c.ProjectId == project.Id && c.Status == ChangeOrderStatus.Approved)
                        .SumAsync(c => (decimal?)c.BudgetDelta, ct) ?? 0m;
                    var currentBudget = decimal.Round(project.BaselineBudgetTotal.Value + approvedDelta, 2);
                    if (currentBudget > 0m)
                    {
                        var existingTotal = await _db.ProjectCostLedgerEntries.AsNoTracking()
                            .Where(e => e.OrganizationId == orgId && e.ProjectId == project.Id)
                            .SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;
                        if (existingTotal + invoice.TotalVarianceAmount > currentBudget)
                        {
                            await _notifications.PublishAsync(
                                NotificationEvents.ProjectBudgetOverrun(orgId, project.BranchId, entry.Id, actorUserId, project.Id, project.Code),
                                ct);
                        }
                    }
                }
            }
        }
    }

    public async Task<SupplierInvoiceProjection?> GetSupplierInvoiceAsync(Guid organizationId, Guid supplierInvoiceId, CancellationToken ct = default)
    {
        var invoice = await _db.SupplierInvoices.AsNoTracking().Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == supplierInvoiceId && i.OrganizationId == organizationId, ct);
        if (invoice is null) return null;

        var order = await _db.PurchaseOrders.AsNoTracking()
            .Where(o => o.Id == invoice.PurchaseOrderId)
            .Select(o => new SupplierInvoiceOrderRefProjection(o.Id, o.Number, o.Status))
            .FirstAsync(ct);
        var supplier = await _db.Suppliers.AsNoTracking().FirstAsync(s => s.Id == invoice.SupplierId, ct);
        var project = invoice.ProjectId.HasValue
            ? await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == invoice.ProjectId.Value, ct)
            : null;

        var userIds = new[] { invoice.CreatedByUserId, invoice.DecidedByUserId ?? Guid.Empty, invoice.VoidedByUserId ?? Guid.Empty }
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => new ProcurementPerson(u.Id, u.DisplayName, u.Email), ct);
        ProcurementPerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new ProcurementPerson(id, string.Empty, null);

        return new SupplierInvoiceProjection(
            invoice.Id,
            invoice.BranchId,
            invoice.Number,
            invoice.SupplierInvoiceNumber,
            invoice.Status,
            invoice.MatchStatus,
            invoice.Currency,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.SubtotalAmount,
            invoice.TaxAmount,
            invoice.TotalAmount,
            invoice.TotalPoAmount,
            invoice.TotalVarianceAmount,
            invoice.Note,
            order,
            new PurchaseOrderSupplierProjection(supplier.Id, supplier.Code, supplier.NameTh, supplier.NameEn),
            project is null ? null : new PurchaseOrderProjectProjection(project.Id, project.Code, project.Name),
            Person(invoice.CreatedByUserId),
            invoice.CreatedAtUtc,
            invoice.SubmittedAtUtc,
            invoice.PostedAtUtc,
            invoice.DecidedByUserId.HasValue ? Person(invoice.DecidedByUserId.Value) : null,
            invoice.DecidedAtUtc,
            invoice.DecisionNote,
            invoice.CancelReason,
            invoice.VoidReason,
            invoice.VoidedByUserId.HasValue ? Person(invoice.VoidedByUserId.Value) : null,
            invoice.VoidedAtUtc,
            invoice.RowVersion,
            invoice.Lines.OrderBy(l => l.LineNo).Select(l => new SupplierInvoiceLineProjection(
                l.Id, l.LineNo, l.PurchaseOrderLineId, l.ItemId, l.ItemCode, l.ItemNameTh, l.UnitId, l.UnitCode,
                l.Quantity, l.PoUnitPrice, l.UnitPrice, l.LineTotal, l.PriceVarianceAmount, l.PriceVariancePercent, l.ExceedsTolerance)).ToList());
    }

    public async Task<PagedSupplierInvoices> ListSupplierInvoicesAsync(Guid organizationId, SupplierInvoiceListQuery query, CancellationToken ct = default)
    {
        var invoices = _db.SupplierInvoices.AsNoTracking().Where(i => i.OrganizationId == organizationId);
        if (query.Status is not null) invoices = invoices.Where(i => i.Status == query.Status);
        if (query.SupplierId.HasValue) invoices = invoices.Where(i => i.SupplierId == query.SupplierId.Value);
        if (query.PurchaseOrderId.HasValue) invoices = invoices.Where(i => i.PurchaseOrderId == query.PurchaseOrderId.Value);

        var joined =
            from i in invoices
            join o in _db.PurchaseOrders.AsNoTracking() on i.PurchaseOrderId equals o.Id
            join s in _db.Suppliers.AsNoTracking() on i.SupplierId equals s.Id
            join p in _db.Projects.AsNoTracking() on i.ProjectId equals (Guid?)p.Id into projects
            from p in projects.DefaultIfEmpty()
            select new
            {
                Invoice = i,
                OrderNumber = o.Number,
                Supplier = s,
                ProjectCode = p == null ? null : p.Code
            };

        if (query.Search is not null)
        {
            var needle = query.Search.ToLowerInvariant();
            joined = joined.Where(x =>
                x.Invoice.Number.ToLower().Contains(needle)
                || x.Invoice.NormalizedSupplierInvoiceNumber.ToLower().Contains(needle)
                || x.OrderNumber.ToLower().Contains(needle)
                || x.Supplier.NormalizedName.Contains(needle)
                || x.Supplier.NormalizedCode.ToLower().Contains(needle));
        }

        var total = await joined.CountAsync(ct);
        var rows = await joined.OrderByDescending(x => x.Invoice.CreatedAtUtc).ThenByDescending(x => x.Invoice.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new PagedSupplierInvoices(
            rows.Select(x => new SupplierInvoiceListItemProjection(
                x.Invoice.Id, x.Invoice.Number, x.Invoice.SupplierInvoiceNumber, x.Invoice.Status, x.Invoice.MatchStatus,
                x.Invoice.PurchaseOrderId, x.OrderNumber, x.Supplier.Code, x.Supplier.NameTh, x.ProjectCode,
                x.Invoice.InvoiceDate, x.Invoice.DueDate, x.Invoice.SubtotalAmount, x.Invoice.TaxAmount,
                x.Invoice.TotalAmount, x.Invoice.TotalVarianceAmount, x.Invoice.CreatedAtUtc)).ToList(),
            total, query.Page, query.PageSize);
    }
}
