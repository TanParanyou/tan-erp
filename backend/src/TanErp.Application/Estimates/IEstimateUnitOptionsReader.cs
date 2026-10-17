using TanErp.Application.Common.Results;
using TanErp.Application.Items;

namespace TanErp.Application.Estimates;

/// <param name="ConversionsEnabled">False when the branch's effective calculation policy is not in snapshot mode; then only the base unit is offered.</param>
public sealed record EstimateUnitOptionsProjection(bool ConversionsEnabled, string BaseUnitCode, IReadOnlyList<EstimateUnitOption> Units);

public interface IEstimateUnitOptionsReader
{
    Task<Result<EstimateUnitOptionsProjection>> ListAsync(Guid organizationId, Guid branchId, Guid itemId, CancellationToken ct);
}
