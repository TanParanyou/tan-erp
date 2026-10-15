using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.QuickEstimates;

public interface IBranchRateOverrideStore
{
    Task<Result<BranchOverrideProjection>> RequestAsync(RequestAccessContext access, Guid templateId, BranchOverrideInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<IReadOnlyList<BranchOverrideProjection>> ListAsync(RequestAccessContext access, Guid templateId, CancellationToken ct = default);
    Task<Result<BranchOverrideProjection>> DecideAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, bool approve, string? note, string traceId, CancellationToken ct = default);
    Task<Result<BranchOverrideProjection>> RevokeAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, string? reason, string traceId, CancellationToken ct = default);
}
