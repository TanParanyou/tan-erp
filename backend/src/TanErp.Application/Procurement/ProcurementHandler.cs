using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

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
        var lines = string.Join(";", order.Lines.Select(l => $"{l.ItemId}:{l.Quantity.ToString(CultureInfo.InvariantCulture)}:{l.UnitPrice.ToString(CultureInfo.InvariantCulture)}"));
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
        if (!string.IsNullOrWhiteSpace(status) && !Domain.Procurement.PurchaseOrderStatus.IsValid(status))
        {
            return Fail<PagedPurchaseOrders>("PROCUREMENT_FIELD_INVALID", "Purchase order status filter is invalid.");
        }

        var (p, size) = Paging(page, pageSize);
        return Result<PagedPurchaseOrders>.Success(await _store.ListPurchaseOrdersAsync(
            access.Value!.OrganizationId, new PurchaseOrderListQuery(Clean(search), Clean(status), supplierId, projectId, p, size), ct));
    }

    // ----- goods receipts --------------------------------------------------------------------------

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

        var lines = string.Join(";", input.Lines.OrderBy(l => l.PurchaseOrderLineId).Select(l => $"{l.PurchaseOrderLineId}:{l.Quantity.ToString(CultureInfo.InvariantCulture)}"));
        var keyHash = Sha256Hex.Compute(idempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{purchaseOrderId}|{input.ReceivedAtUtc:O}|{input.Note?.Trim()}|{lines}");
        return await _store.PostGoodsReceiptAsync(access.Value!, purchaseOrderId, input, keyHash, payloadHash, caller.TraceId, ct);
    }
}
