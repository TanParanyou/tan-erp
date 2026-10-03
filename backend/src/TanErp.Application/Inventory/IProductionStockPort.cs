using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Inventory;

public sealed record ProductionStockLine(Guid ItemId, decimal Quantity, decimal Value);

public sealed record ProductionStockResult(Guid DocumentId, string DocumentNumber, IReadOnlyList<ProductionStockLine> Lines);

public sealed record ProductionIssueLine(Guid ItemId, decimal Quantity);

public sealed record ProductionReturnLine(Guid ItemId, decimal Quantity, decimal Value);

/// <summary>
/// The only way Production changes stock. The calls join the caller's transaction (when one is open) so a work order
/// update and its stock movement commit or roll back together; stock tables are never written by Production directly.
/// Each call is idempotent per <c>sourceId</c>: replaying it returns the original stock document.
/// </summary>
public interface IProductionStockPort
{
    Task<Result<ProductionStockResult>> IssueMaterialsAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, string reason,
        IReadOnlyList<ProductionIssueLine> lines, string traceId, CancellationToken ct = default);

    Task<Result<ProductionStockResult>> ReturnMaterialsAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, string reason,
        IReadOnlyList<ProductionReturnLine> lines, string traceId, CancellationToken ct = default);

    Task<Result<ProductionStockResult>> ReceiveOutputAsync(
        RequestAccessContext access, Guid warehouseId, Guid? projectId, Guid sourceId, Guid itemId,
        decimal quantity, decimal totalValue, string traceId, CancellationToken ct = default);
}
