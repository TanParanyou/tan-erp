using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Production;

public interface IProductionStore
{
    Task<Result<BomProjection>> CreateBomAsync(RequestAccessContext access, BomInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<BomProjection>> CreateBomRevisionAsync(RequestAccessContext access, Guid bomId, BomDraftInput input, string traceId, CancellationToken ct = default);
    Task<Result<BomProjection>> UpdateBomDraftAsync(RequestAccessContext access, Guid bomId, Guid revisionId, Guid expectedVersion, BomDraftInput input, string traceId, CancellationToken ct = default);
    Task<Result<BomProjection>> BomActionAsync(RequestAccessContext access, Guid bomId, Guid revisionId, Guid expectedVersion, BomAction action, string traceId, CancellationToken ct = default);
    Task<BomProjection?> GetBomAsync(Guid organizationId, Guid bomId, CancellationToken ct = default);
    Task<PagedBoms> ListBomsAsync(Guid organizationId, BomListQuery query, CancellationToken ct = default);

    Task<Result<WorkOrderProjection>> CreateWorkOrderAsync(RequestAccessContext access, WorkOrderInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<WorkOrderProjection>> WorkOrderActionAsync(RequestAccessContext access, Guid workOrderId, Guid expectedVersion, WorkOrderAction action, string? reason, string traceId, CancellationToken ct = default);
    Task<Result<WorkOrderProjection>> IssueMaterialsAsync(RequestAccessContext access, Guid workOrderId, IReadOnlyList<WorkOrderMaterialQuantityInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<WorkOrderProjection>> ReturnMaterialsAsync(RequestAccessContext access, Guid workOrderId, IReadOnlyList<WorkOrderMaterialQuantityInput> lines, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<WorkOrderProjection>> CompleteAsync(RequestAccessContext access, Guid workOrderId, decimal quantity, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<WorkOrderProjection?> GetWorkOrderAsync(Guid organizationId, Guid workOrderId, CancellationToken ct = default);
    Task<PagedWorkOrders> ListWorkOrdersAsync(Guid organizationId, WorkOrderListQuery query, CancellationToken ct = default);
}
