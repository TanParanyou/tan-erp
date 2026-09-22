using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

public sealed record CreateCostRecordData(
    Guid ItemId,
    string Scope,
    Guid? BranchId,
    Guid UnitId,
    string Currency,
    decimal Amount,
    decimal MinimumQuantity,
    decimal? MaximumQuantity,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    Guid? CostSourceId,
    string? SourceReference,
    string? Reason,
    Guid? EvidenceFileId);

public sealed record UpdateCostRecordData(
    Guid CostId,
    Guid ItemId,
    decimal Amount,
    string Currency,
    Guid UnitId,
    decimal MinimumQuantity,
    decimal? MaximumQuantity,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    Guid? CostSourceId,
    string? SourceReference,
    string? Reason,
    Guid? EvidenceFileId);

public interface ICostRecordStore
{
    Task<Result<CostRecordDetailProjection>> CreateDraftAsync(CreateCostRecordData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<CostRecordDetailProjection>> UpdateDraftAsync(UpdateCostRecordData data, RequestAccessContext access, Guid? ifMatch, CancellationToken ct);
    Task<Result<CostRecordDetailProjection>> SubmitAsync(Guid orgId, Guid itemId, Guid costId, RequestAccessContext access, Guid? ifMatch, CancellationToken ct);
    Task<Result<CostRecordDetailProjection>> ApproveAsync(Guid orgId, Guid itemId, Guid costId, RequestAccessContext access, Guid? ifMatch, CancellationToken ct);
    Task<Result<CostRecordDetailProjection>> ReturnAsync(Guid orgId, Guid itemId, Guid costId, string reason, RequestAccessContext access, Guid? ifMatch, CancellationToken ct);
    Task<Result<CostRecordDetailProjection>> PublishAsync(Guid orgId, Guid itemId, Guid costId, RequestAccessContext access, Guid? ifMatch, CancellationToken ct);
    Task<Result<CostRecordDetailProjection>> DisableAsync(Guid orgId, Guid itemId, Guid costId, string reason, RequestAccessContext access, Guid? ifMatch, CancellationToken ct);
    Task<CostRecordDetailProjection?> GetByIdAsync(Guid orgId, Guid itemId, Guid costId, CancellationToken ct);
    Task<IReadOnlyList<CostRecordDetailProjection>> ListForItemAsync(Guid orgId, Guid itemId, CancellationToken ct);
}
