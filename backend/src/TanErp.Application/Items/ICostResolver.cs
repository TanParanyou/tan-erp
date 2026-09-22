using TanErp.Application.Common.Results;
using TanErp.Application.Items.Catalog;
using TanErp.Domain.Items;

namespace TanErp.Application.Items;

public interface ICostResolver
{
    Task<Result<ResolvedCostProjection>> ResolveAsync(ResolveCostRequest request, CancellationToken ct);

    CatalogResolvedCostProjection? ResolveForCatalog(IReadOnlyList<CostRecord> records, Guid branchId);
}
