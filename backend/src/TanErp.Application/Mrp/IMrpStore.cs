using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Mrp;

public interface IMrpStore
{
    Task<Result<MrpRunProjection>> CreateRunAsync(RequestAccessContext access, MrpRunInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<MrpRunProjection?> GetRunAsync(Guid organizationId, Guid runId, CancellationToken ct = default);
    Task<PagedMrpRuns> ListRunsAsync(Guid organizationId, MrpRunListQuery query, CancellationToken ct = default);
    Task<Result<MrpRunProjection>> DecideAsync(RequestAccessContext access, Guid runId, Guid recommendationId, Guid expectedVersion, bool approve, string traceId, CancellationToken ct = default);
    Task<MrpConvertTarget?> GetConvertTargetAsync(Guid organizationId, Guid runId, Guid recommendationId, CancellationToken ct = default);
    Task<Result<MrpRunProjection>> MarkConvertedAsync(RequestAccessContext access, Guid runId, Guid recommendationId, Guid expectedVersion, string type, Guid targetId, string targetNumber, string traceId, CancellationToken ct = default);
}
