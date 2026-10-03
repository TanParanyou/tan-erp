using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Items;

namespace TanErp.Application.Items;

public sealed record CostSourceProjection(Guid Id, string Code, LocalizedText Name, string SourceType, int Priority, bool IsActive, Guid RowVersion);
public sealed record CreateCostSourceData(string? Code, LocalizedText Name);
public sealed record UpdateCostSourceData(Guid Id, Guid ExpectedRowVersion, string Code, LocalizedText Name);

public interface ICostSourceStore
{
    Task<IReadOnlyList<CostSourceProjection>> ListAsync(Guid organizationId, CancellationToken ct);
    Task<CostSourceProjection?> GetAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<Result<CostSourceProjection>> CreateAsync(CreateCostSourceData data, RequestAccessContext access, string idempotencyKey, string traceId, CancellationToken ct);
    Task<Result<CostSourceProjection>> UpdateAsync(UpdateCostSourceData data, RequestAccessContext access, string traceId, CancellationToken ct);
    Task<Result<CostSourceProjection>> DeactivateAsync(Guid id, Guid rowVersion, RequestAccessContext access, string traceId, CancellationToken ct);
}
