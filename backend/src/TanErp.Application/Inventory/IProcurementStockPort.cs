using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Inventory;

public sealed record ProcurementReturnStockLine(Guid ItemId, decimal Quantity);

public sealed record ProcurementStockResult(Guid DocumentId, string DocumentNumber);

/// <summary>
/// The only way Procurement reduces warehouse stock when returning already-received goods to a supplier.
/// Joins the caller's open transaction so the supplier return and stock deduction commit or roll back atomically.
/// </summary>
public interface IProcurementStockPort
{
    Task<Result<ProcurementStockResult>> ReturnToSupplierAsync(
        RequestAccessContext access,
        Guid warehouseId,
        Guid? projectId,
        Guid supplierReturnId,
        string reason,
        DateTimeOffset occurredAtUtc,
        IReadOnlyList<ProcurementReturnStockLine> lines,
        string traceId,
        CancellationToken ct = default);
}
