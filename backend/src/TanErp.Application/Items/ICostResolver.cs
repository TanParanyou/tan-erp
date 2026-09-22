using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

public interface ICostResolver
{
    Task<Result<ResolvedCostProjection>> ResolveAsync(ResolveCostRequest request, CancellationToken ct);
}
