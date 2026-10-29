using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Procurement;

namespace TanErp.Application.Procurement;

public sealed record ProcurementCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>
/// Procurement use cases. Each call resolves the caller's permission from PostgreSQL, validates the request
/// shape and then delegates to the store, which owns the transaction.
/// </summary>
public class ProcurementHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    public const int MaxOrderLines = 200;
    public const int MaxConvertBatch = 50;

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IProcurementStore _store;

    public ProcurementHandler(IRequestAccessResolver accessResolver, IProcurementStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(ProcurementCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static (int Page, int PageSize) Paging(int page, int pageSize) =>
        (Math.Max(1, page), pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize));

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string Num(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    // ----- suppliers -------------------------------------------------------------------------------

    public async Task<Result<SupplierProjection>> CreateSupplierAsync(ProcurementCaller caller, string idempotencyKey, SupplierInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.manage", ct);
        if (access.IsFailure) return Result<SupplierProjection>.Failure(access.Error);

        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{input.NameTh?.Trim()}|{input.NameEn?.Trim()}|{input.TaxId?.Trim()}|{input.ContactName?.Trim()}|{input.Phone?.Trim()}|{input.Email?.Trim()}|{input.PaymentTermDays}");
        return await _store.CreateSupplierAsync(access.Value!, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<SupplierProjection>> UpdateSupplierAsync(ProcurementCaller caller, Guid supplierId, Guid expectedVersion, SupplierInput input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.manage", ct);
        if (access.IsFailure) return Result<SupplierProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<SupplierProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        return await _store.UpdateSupplierAsync(access.Value!, supplierId, expectedVersion, input, caller.TraceId, ct);
    }

    public async Task<Result<SupplierProjection>> SetSupplierActiveAsync(ProcurementCaller caller, Guid supplierId, Guid expectedVersion, bool active, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.manage", ct);
        if (access.IsFailure) return Result<SupplierProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<SupplierProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        return await _store.SetSupplierActiveAsync(access.Value!, supplierId, expectedVersion, active, caller.TraceId, ct);
    }

    public async Task<Result<SupplierProjection>> GetSupplierAsync(ProcurementCaller caller, Guid supplierId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.read", ct);
        if (access.IsFailure) return Result<SupplierProjection>.Failure(access.Error);
        var supplier = await _store.GetSupplierAsync(access.Value!.OrganizationId, supplierId, ct);
        return supplier is null ? Fail<SupplierProjection>("RESOURCE_NOT_FOUND", "Supplier not found.") : Result<SupplierProjection>.Success(supplier);
    }

    public async Task<Result<PagedSuppliers>> ListSuppliersAsync(ProcurementCaller caller, string? search, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "suppliers.read", ct);
        if (access.IsFailure) return Result<PagedSuppliers>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && status.Trim() is not ("active" or "inactive"))
        {
            return Fail<PagedSuppliers>("PROCUREMENT_FIELD_INVALID", "Supplier status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedSuppliers>.Success(await _store.ListSuppliersAsync(access.Value!.OrganizationId, new SupplierListQuery(Clean(search), Clean(status), p, size), ct));
    }

    // ----- purchase requests -----------------------------------------------------------------------

    private static Result<PurchaseRequestInput> ValidateRequest(PurchaseRequestInput? input)
    {
        if (input is null || input.Lines is null || input.Lines.Count == 0 || input.Lines.Count > MaxOrderLines)
        {
            return Fail<PurchaseRequestInput>("PURCHASE_REQUEST_LINE_INVALID", $"A purchase request needs between 1 and {MaxOrderLines} lines.");
        }

        if (input.Lines.Any(l => l.ItemId == Guid.Empty || l.Quantity <= 0 || l.EstimatedUnitPrice < 0))
        {
            return Fail<PurchaseRequestInput>("PURCHASE_REQUEST_LINE_INVALID", "Every line must specify an item, positive quantity, and non-negative estimated unit price.");
        }

        if (input.Lines.GroupBy(l => l.ItemId).Any(g => g.Count() > 1))
        {
            return Fail<PurchaseRequestInput>("PURCHASE_REQUEST_LINE_INVALID", "Each item can appear on a purchase request only once.");
        }

        return Result<PurchaseRequestInput>.Success(input);
    }

    public async Task<Result<PurchaseRequestProjection>> CreatePurchaseRequestAsync(ProcurementCaller caller, string idempotencyKey, PurchaseRequestInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<PurchaseRequestProjection>.Failure(access.Error);
        var valid = ValidateRequest(input);
        if (valid.IsFailure) return Result<PurchaseRequestProjection>.Failure(valid.Error);

        var req = valid.Value!;
        var lines = string.Join(";", req.Lines.Select(l => $"{l.ItemId}:{Num(l.Quantity)}:{Num(l.EstimatedUnitPrice)}:{l.Note?.Trim()}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{req.ProjectId}|{req.RequiredByDate:O}|{req.Note?.Trim()}|{lines}");
        return await _store.CreatePurchaseRequestAsync(access.Value!, req, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<PurchaseRequestProjection>> UpdatePurchaseRequestAsync(ProcurementCaller caller, Guid purchaseRequestId, Guid expectedVersion, PurchaseRequestInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<PurchaseRequestProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PurchaseRequestProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        var valid = ValidateRequest(input);
        if (valid.IsFailure) return Result<PurchaseRequestProjection>.Failure(valid.Error);
        return await _store.UpdatePurchaseRequestAsync(access.Value!, purchaseRequestId, expectedVersion, valid.Value!, caller.TraceId, ct);
    }

    public async Task<Result<PurchaseRequestProjection>> PurchaseRequestActionAsync(ProcurementCaller caller, Guid purchaseRequestId, Guid expectedVersion, PurchaseRequestAction action, string? note, CancellationToken ct = default)
    {
        var permission = action is PurchaseRequestAction.Approve or PurchaseRequestAction.Reject ? "purchase-orders.approve" : "purchase-orders.create";
        var access = await AccessAsync(caller, permission, ct);
        if (access.IsFailure) return Result<PurchaseRequestProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PurchaseRequestProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        if (note is { Length: > 500 }) return Fail<PurchaseRequestProjection>("PROCUREMENT_FIELD_INVALID", "Note cannot exceed 500 characters.");
        return await _store.PurchaseRequestActionAsync(access.Value!, purchaseRequestId, expectedVersion, action, Clean(note), caller.TraceId, ct);
    }

    public async Task<Result<PurchaseRequestProjection>> GetPurchaseRequestAsync(ProcurementCaller caller, Guid purchaseRequestId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.read", ct);
        if (access.IsFailure) return Result<PurchaseRequestProjection>.Failure(access.Error);
        var pr = await _store.GetPurchaseRequestAsync(access.Value!.OrganizationId, purchaseRequestId, ct);
        return pr is null ? Fail<PurchaseRequestProjection>("RESOURCE_NOT_FOUND", "Purchase request not found.") : Result<PurchaseRequestProjection>.Success(pr);
    }

    public async Task<Result<PagedPurchaseRequests>> ListPurchaseRequestsAsync(ProcurementCaller caller, string? search, string? status, Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.read", ct);
        if (access.IsFailure) return Result<PagedPurchaseRequests>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !PurchaseRequestStatus.IsValid(status))
        {
            return Fail<PagedPurchaseRequests>("PROCUREMENT_FIELD_INVALID", "Purchase request status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedPurchaseRequests>.Success(await _store.ListPurchaseRequestsAsync(
            access.Value!.OrganizationId, new PurchaseRequestListQuery(Clean(search), Clean(status), projectId, p, size), ct));
    }

    public async Task<Result<PurchaseOrderProjection>> ConvertPurchaseRequestsAsync(ProcurementCaller caller, string idempotencyKey, ConvertPurchaseRequestsInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        if (input is null || input.SupplierId == Guid.Empty)
        {
            return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_REQUIRED", "A supplier and at least one purchase request are required.");
        }

        if (input.PurchaseRequestIds is null || input.PurchaseRequestIds.Count == 0 || input.PurchaseRequestIds.Count > MaxConvertBatch || input.PurchaseRequestIds.Any(id => id == Guid.Empty))
        {
            return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_INVALID", $"Between 1 and {MaxConvertBatch} purchase request IDs are required.");
        }

        if (input.PurchaseRequestIds.Distinct().Count() != input.PurchaseRequestIds.Count)
        {
            return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_INVALID", "Duplicate purchase request IDs are not allowed.");
        }

        if (input.Note is { Length: > 500 })
        {
            return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_INVALID", "Note cannot exceed 500 characters.");
        }

        if (input.UnitPriceOverrides is not null)
        {
            if (input.UnitPriceOverrides.Any(o => o.ItemId == Guid.Empty || o.UnitPrice < 0))
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_LINE_INVALID", "Unit price overrides must specify valid items and non-negative prices.");
            }

            if (input.UnitPriceOverrides.Select(o => o.ItemId).Distinct().Count() != input.UnitPriceOverrides.Count)
            {
                return Fail<PurchaseOrderProjection>("PURCHASE_ORDER_LINE_INVALID", "Each item may have at most one unit price override.");
            }
        }

        var prIdsPart = string.Join(",", input.PurchaseRequestIds.OrderBy(id => id));
        var overridesPart = input.UnitPriceOverrides is null
            ? string.Empty
            : string.Join(";", input.UnitPriceOverrides.OrderBy(o => o.ItemId).Select(o => $"{o.ItemId}:{Num(o.UnitPrice)}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{prIdsPart}|{input.SupplierId}|{input.ExpectedDeliveryDate:O}|{input.Note?.Trim()}|{overridesPart}");
        return await _store.ConvertPurchaseRequestsAsync(access.Value!, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    // ----- purchase orders -------------------------------------------------------------------------

    private static Result<PurchaseOrderInput> ValidateOrder(PurchaseOrderInput? input)
    {
        if (input is null || input.SupplierId == Guid.Empty)
        {
            return Fail<PurchaseOrderInput>("PROCUREMENT_FIELD_REQUIRED", "A supplier is required.");
        }

        if (input.Lines is null || input.Lines.Count == 0 || input.Lines.Count > MaxOrderLines)
        {
            return Fail<PurchaseOrderInput>("PURCHASE_ORDER_LINE_INVALID", $"A purchase order needs between 1 and {MaxOrderLines} lines.");
        }

        if (input.Lines.GroupBy(l => l.ItemId).Any(g => g.Count() > 1))
        {
            return Fail<PurchaseOrderInput>("PURCHASE_ORDER_LINE_INVALID", "Each item can appear on a purchase order only once.");
        }

        return Result<PurchaseOrderInput>.Success(input);
    }

    public async Task<Result<PurchaseOrderProjection>> CreatePurchaseOrderAsync(ProcurementCaller caller, string idempotencyKey, PurchaseOrderInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        var valid = ValidateOrder(input);
        if (valid.IsFailure) return Result<PurchaseOrderProjection>.Failure(valid.Error);

        var order = valid.Value!;
        var lines = string.Join(";", order.Lines.Select(l => $"{l.ItemId}:{Num(l.Quantity)}:{Num(l.UnitPrice)}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{order.SupplierId}|{order.ProjectId}|{order.ExpectedDeliveryDate:O}|{order.Note?.Trim()}|{lines}");
        return await _store.CreatePurchaseOrderAsync(access.Value!, order, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<PurchaseOrderProjection>> UpdatePurchaseOrderAsync(ProcurementCaller caller, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        var valid = ValidateOrder(input);
        if (valid.IsFailure) return Result<PurchaseOrderProjection>.Failure(valid.Error);
        return await _store.UpdatePurchaseOrderAsync(access.Value!, purchaseOrderId, expectedVersion, valid.Value!, caller.TraceId, ct);
    }

    public async Task<Result<PurchaseOrderProjection>> ActionAsync(ProcurementCaller caller, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderAction action, string? note, CancellationToken ct = default)
    {
        var permission = action is PurchaseOrderAction.Approve or PurchaseOrderAction.Reject ? "purchase-orders.approve" : "purchase-orders.create";
        var access = await AccessAsync(caller, permission, ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        if (note is { Length: > 500 }) return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_INVALID", "Note cannot exceed 500 characters.");
        return await _store.PurchaseOrderActionAsync(access.Value!, purchaseOrderId, expectedVersion, action, Clean(note), caller.TraceId, ct);
    }

    public async Task<Result<PurchaseOrderProjection>> GetPurchaseOrderAsync(ProcurementCaller caller, Guid purchaseOrderId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.read", ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        var order = await _store.GetPurchaseOrderAsync(access.Value!.OrganizationId, purchaseOrderId, ct);
        return order is null ? Fail<PurchaseOrderProjection>("RESOURCE_NOT_FOUND", "Purchase order not found.") : Result<PurchaseOrderProjection>.Success(order);
    }

    public async Task<Result<PagedPurchaseOrders>> ListPurchaseOrdersAsync(ProcurementCaller caller, string? search, string? status, Guid? supplierId, Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.read", ct);
        if (access.IsFailure) return Result<PagedPurchaseOrders>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !PurchaseOrderStatus.IsValid(status))
        {
            return Fail<PagedPurchaseOrders>("PROCUREMENT_FIELD_INVALID", "Purchase order status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedPurchaseOrders>.Success(await _store.ListPurchaseOrdersAsync(
            access.Value!.OrganizationId, new PurchaseOrderListQuery(Clean(search), Clean(status), supplierId, projectId, p, size), ct));
    }

    // ----- goods receipts & supplier returns -------------------------------------------------------

    public async Task<Result<PurchaseOrderProjection>> PostGoodsReceiptAsync(ProcurementCaller caller, Guid purchaseOrderId, string idempotencyKey, GoodsReceiptInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "goods-receipts.create", ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        if (input is null || input.Lines is null || input.Lines.Count == 0)
        {
            return Fail<PurchaseOrderProjection>("GOODS_RECEIPT_INVALID", "A receipt needs at least one line.");
        }

        if (input.Lines.Any(l => l.Quantity <= 0) || input.Lines.GroupBy(l => l.PurchaseOrderLineId).Any(g => g.Count() > 1))
        {
            return Fail<PurchaseOrderProjection>("GOODS_RECEIPT_INVALID", "Receipt quantities must be positive and each order line can appear only once.");
        }

        var lines = string.Join(";", input.Lines.OrderBy(l => l.PurchaseOrderLineId).Select(l => $"{l.PurchaseOrderLineId}:{Num(l.Quantity)}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{purchaseOrderId}|{input.ReceivedAtUtc:O}|{input.Note?.Trim()}|{lines}");
        return await _store.PostGoodsReceiptAsync(access.Value!, purchaseOrderId, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<PurchaseOrderProjection>> PostSupplierReturnAsync(
        ProcurementCaller caller, Guid purchaseOrderId, Guid expectedVersion, string idempotencyKey, SupplierReturnInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "goods-receipts.create", ct);
        if (access.IsFailure) return Result<PurchaseOrderProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PurchaseOrderProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        if (input is null || input.GoodsReceiptId == Guid.Empty)
        {
            return Fail<PurchaseOrderProjection>("SUPPLIER_RETURN_INVALID", "A goods receipt is required.");
        }

        if (string.IsNullOrWhiteSpace(input.Reason))
        {
            return Fail<PurchaseOrderProjection>("PROCUREMENT_REASON_REQUIRED", "A reason is required to record a supplier return.");
        }

        if (input.Reason.Trim().Length > 500)
        {
            return Fail<PurchaseOrderProjection>("SUPPLIER_RETURN_INVALID", "Reason cannot exceed 500 characters.");
        }

        if (input.Lines is null || input.Lines.Count == 0 || input.Lines.Count > MaxOrderLines)
        {
            return Fail<PurchaseOrderProjection>("SUPPLIER_RETURN_INVALID", $"A supplier return needs between 1 and {MaxOrderLines} lines.");
        }

        if (input.Lines.Any(l => l.GoodsReceiptLineId == Guid.Empty || l.Quantity <= 0)
            || input.Lines.GroupBy(l => l.GoodsReceiptLineId).Any(g => g.Count() > 1))
        {
            return Fail<PurchaseOrderProjection>("SUPPLIER_RETURN_INVALID", "Return quantities must be positive and each goods receipt line can appear only once.");
        }

        var lines = string.Join(";", input.Lines.OrderBy(l => l.GoodsReceiptLineId).Select(l => $"{l.GoodsReceiptLineId}:{Num(l.Quantity)}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{purchaseOrderId}|{input.GoodsReceiptId}|{input.ReturnedAtUtc:O}|{input.Reason.Trim()}|{lines}");
        return await _store.PostSupplierReturnAsync(access.Value!, purchaseOrderId, expectedVersion, input, keyHash, payloadHash, caller.TraceId, ct);
    }

    // ----- supplier invoices -----------------------------------------------------------------------

    private static Result<SupplierInvoiceInput> ValidateInvoice(SupplierInvoiceInput? input)
    {
        if (input is null || input.PurchaseOrderId == Guid.Empty || string.IsNullOrWhiteSpace(input.SupplierInvoiceNumber))
        {
            return Fail<SupplierInvoiceInput>("PROCUREMENT_FIELD_REQUIRED", "Purchase order and supplier invoice number are required.");
        }

        if (input.SupplierInvoiceNumber.Trim().Length > 100)
        {
            return Fail<SupplierInvoiceInput>("PROCUREMENT_FIELD_INVALID", "Supplier invoice number cannot exceed 100 characters.");
        }

        if (input.DueDate.HasValue && input.DueDate.Value < input.InvoiceDate)
        {
            return Fail<SupplierInvoiceInput>("PROCUREMENT_FIELD_INVALID", "Due date cannot be earlier than the invoice date.");
        }

        if (input.TaxAmount < 0)
        {
            return Fail<SupplierInvoiceInput>("PROCUREMENT_FIELD_INVALID", "Tax amount cannot be negative.");
        }

        if (input.Lines is null || input.Lines.Count == 0 || input.Lines.Count > MaxOrderLines)
        {
            return Fail<SupplierInvoiceInput>("SUPPLIER_INVOICE_LINE_INVALID", $"A supplier invoice needs between 1 and {MaxOrderLines} lines.");
        }

        if (input.Lines.Any(l => l.PurchaseOrderLineId == Guid.Empty || l.Quantity <= 0 || l.UnitPrice < 0))
        {
            return Fail<SupplierInvoiceInput>("SUPPLIER_INVOICE_LINE_INVALID", "Invoice lines must specify a purchase order line, positive quantity, and non-negative unit price.");
        }

        if (input.Lines.GroupBy(l => l.PurchaseOrderLineId).Any(g => g.Count() > 1))
        {
            return Fail<SupplierInvoiceInput>("SUPPLIER_INVOICE_LINE_INVALID", "Each purchase order line can appear on a supplier invoice only once.");
        }

        return Result<SupplierInvoiceInput>.Success(input);
    }

    public async Task<Result<SupplierInvoiceProjection>> CreateSupplierInvoiceAsync(ProcurementCaller caller, string idempotencyKey, SupplierInvoiceInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<SupplierInvoiceProjection>.Failure(access.Error);
        var valid = ValidateInvoice(input);
        if (valid.IsFailure) return Result<SupplierInvoiceProjection>.Failure(valid.Error);

        var inv = valid.Value!;
        var lines = string.Join(";", inv.Lines.OrderBy(l => l.PurchaseOrderLineId).Select(l => $"{l.PurchaseOrderLineId}:{Num(l.Quantity)}:{Num(l.UnitPrice)}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{inv.PurchaseOrderId}|{inv.SupplierInvoiceNumber!.Trim()}|{inv.InvoiceDate:O}|{inv.DueDate:O}|{Num(inv.TaxAmount)}|{inv.Note?.Trim()}|{lines}");
        return await _store.CreateSupplierInvoiceAsync(access.Value!, inv, keyHash, payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<SupplierInvoiceProjection>> UpdateSupplierInvoiceAsync(ProcurementCaller caller, Guid supplierInvoiceId, Guid expectedVersion, SupplierInvoiceInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.create", ct);
        if (access.IsFailure) return Result<SupplierInvoiceProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<SupplierInvoiceProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        var valid = ValidateInvoice(input);
        if (valid.IsFailure) return Result<SupplierInvoiceProjection>.Failure(valid.Error);
        return await _store.UpdateSupplierInvoiceAsync(access.Value!, supplierInvoiceId, expectedVersion, valid.Value!, caller.TraceId, ct);
    }

    public async Task<Result<SupplierInvoiceProjection>> SupplierInvoiceActionAsync(ProcurementCaller caller, Guid supplierInvoiceId, Guid expectedVersion, SupplierInvoiceAction action, string? note, CancellationToken ct = default)
    {
        var permission = action is SupplierInvoiceAction.Approve or SupplierInvoiceAction.Reject ? "purchase-orders.approve" : "purchase-orders.create";
        var access = await AccessAsync(caller, permission, ct);
        if (access.IsFailure) return Result<SupplierInvoiceProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<SupplierInvoiceProjection>("PROCUREMENT_FIELD_REQUIRED", "Expected version is required.");
        if (note is { Length: > 500 }) return Fail<SupplierInvoiceProjection>("PROCUREMENT_FIELD_INVALID", "Note cannot exceed 500 characters.");
        return await _store.SupplierInvoiceActionAsync(access.Value!, supplierInvoiceId, expectedVersion, action, Clean(note), caller.TraceId, ct);
    }

    public async Task<Result<SupplierInvoiceProjection>> GetSupplierInvoiceAsync(ProcurementCaller caller, Guid supplierInvoiceId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.read", ct);
        if (access.IsFailure) return Result<SupplierInvoiceProjection>.Failure(access.Error);
        var inv = await _store.GetSupplierInvoiceAsync(access.Value!.OrganizationId, supplierInvoiceId, ct);
        return inv is null ? Fail<SupplierInvoiceProjection>("RESOURCE_NOT_FOUND", "Supplier invoice not found.") : Result<SupplierInvoiceProjection>.Success(inv);
    }

    public async Task<Result<PagedSupplierInvoices>> ListSupplierInvoicesAsync(ProcurementCaller caller, string? search, string? status, Guid? supplierId, Guid? purchaseOrderId, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "purchase-orders.read", ct);
        if (access.IsFailure) return Result<PagedSupplierInvoices>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !SupplierInvoiceStatus.IsValid(status))
        {
            return Fail<PagedSupplierInvoices>("PROCUREMENT_FIELD_INVALID", "Supplier invoice status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedSupplierInvoices>.Success(await _store.ListSupplierInvoicesAsync(
            access.Value!.OrganizationId, new SupplierInvoiceListQuery(Clean(search), Clean(status), supplierId, purchaseOrderId, p, size), ct));
    }
}
