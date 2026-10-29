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

    Task<Result<PurchaseRequestProjection>> CreatePurchaseRequestAsync(RequestAccessContext access, PurchaseRequestInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseRequestProjection>> UpdatePurchaseRequestAsync(RequestAccessContext access, Guid purchaseRequestId, Guid expectedVersion, PurchaseRequestInput input, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseRequestProjection>> PurchaseRequestActionAsync(RequestAccessContext access, Guid purchaseRequestId, Guid expectedVersion, PurchaseRequestAction action, string? note, string traceId, CancellationToken ct = default);
    Task<PurchaseRequestProjection?> GetPurchaseRequestAsync(Guid organizationId, Guid purchaseRequestId, CancellationToken ct = default);
    Task<PagedPurchaseRequests> ListPurchaseRequestsAsync(Guid organizationId, PurchaseRequestListQuery query, CancellationToken ct = default);
    Task<Result<PurchaseOrderProjection>> ConvertPurchaseRequestsAsync(RequestAccessContext access, ConvertPurchaseRequestsInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);

    Task<Result<PurchaseOrderProjection>> CreatePurchaseOrderAsync(RequestAccessContext access, PurchaseOrderInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseOrderProjection>> UpdatePurchaseOrderAsync(RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderInput input, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseOrderProjection>> PurchaseOrderActionAsync(RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, PurchaseOrderAction action, string? note, string traceId, CancellationToken ct = default);
    Task<PurchaseOrderProjection?> GetPurchaseOrderAsync(Guid organizationId, Guid purchaseOrderId, CancellationToken ct = default);
    Task<PagedPurchaseOrders> ListPurchaseOrdersAsync(Guid organizationId, PurchaseOrderListQuery query, CancellationToken ct = default);

    Task<Result<PurchaseOrderProjection>> PostGoodsReceiptAsync(RequestAccessContext access, Guid purchaseOrderId, GoodsReceiptInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<PurchaseOrderProjection>> PostSupplierReturnAsync(RequestAccessContext access, Guid purchaseOrderId, Guid expectedVersion, SupplierReturnInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);

    Task<Result<SupplierInvoiceProjection>> CreateSupplierInvoiceAsync(RequestAccessContext access, SupplierInvoiceInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<SupplierInvoiceProjection>> UpdateSupplierInvoiceAsync(RequestAccessContext access, Guid supplierInvoiceId, Guid expectedVersion, SupplierInvoiceInput input, string traceId, CancellationToken ct = default);
    Task<Result<SupplierInvoiceProjection>> SupplierInvoiceActionAsync(RequestAccessContext access, Guid supplierInvoiceId, Guid expectedVersion, SupplierInvoiceAction action, string? note, string traceId, CancellationToken ct = default);
    Task<SupplierInvoiceProjection?> GetSupplierInvoiceAsync(Guid organizationId, Guid supplierInvoiceId, CancellationToken ct = default);
    Task<PagedSupplierInvoices> ListSupplierInvoicesAsync(Guid organizationId, SupplierInvoiceListQuery query, CancellationToken ct = default);
}
