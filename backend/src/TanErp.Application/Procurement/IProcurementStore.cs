using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Procurement;

public interface IProcurementStore
{
    Task<Result<SupplierProjection>> CreateSupplierAsync(RequestAccessContext access, SupplierInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<SupplierProjection>> UpdateSupplierAsync(RequestAccessContext access, Guid supplierId, Guid expectedVersion, SupplierInput input, string traceId, CancellationToken ct = default);
    Task<Result<SupplierProjection>> SetSupplierActiveAsync(RequestAccessContext access, Guid supplierId, Guid expectedVersion, bool active, string traceId, CancellationToken ct = default);
    Task<SupplierProjection?> GetSupplierAsync(Guid organizationId, Guid supplierId, CancellationToken ct = default);
    Task<PagedSuppliers> ListSuppliersAsync(Guid organizationId, SupplierListQuery query, CancellationToken ct = default);

    Task<Result<PurchaseOrderProjection>> CreatePurchaseOrderAsync(RequestAccessContext access, PurchaseOrderInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseOrderProjection>> UpdatePurchaseOrderAsync(RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderInput input, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseOrderProjection>> PurchaseOrderActionAsync(RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderAction action, string? note, string traceId, CancellationToken ct = default);
    Task<PurchaseOrderProjection?> GetPurchaseOrderAsync(Guid organizationId, Guid purchaseOrderId, CancellationToken ct = default);
    Task<PagedPurchaseOrders> ListPurchaseOrdersAsync(Guid organizationId, PurchaseOrderListQuery query, CancellationToken ct = default);

    Task<Result<PurchaseOrderProjection>> PostGoodsReceiptAsync(RequestAccessContext access, Guid purchaseOrderId, GoodsReceiptInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
}
