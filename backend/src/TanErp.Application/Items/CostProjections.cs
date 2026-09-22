using TanErp.Domain.Items;

namespace TanErp.Application.Items;

public sealed record CostRecordDetailProjection(
    Guid Id,
    Guid OrganizationId,
    Guid ItemId,
    string Scope,
    Guid? BranchId,
    Guid UnitId,
    LocalizedText UnitName,
    string Currency,
    decimal Amount,
    decimal MinimumQuantity,
    decimal? MaximumQuantity,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc,
    string Status,
    int Version,
    Guid? CostSourceId,
    LocalizedText? CostSourceName,
    string? SourceReference,
    string? Reason,
    Guid? EvidenceFileId,
    Guid CreatedByUserId,
    Guid LastFinancialEditorId,
    Guid? ApprovedByUserId,
    Guid? PublishedByUserId,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record CostRecordReviewProjection(
    Guid Id,
    Guid CostRecordId,
    string Decision,
    string? Reason,
    Guid ReviewerUserId,
    string? AuthoritySnapshot,
    DateTimeOffset DecidedAtUtc);

public sealed record ResolveCostRequest(
    Guid OrganizationId,
    Guid BranchId,
    Guid ItemId,
    Guid UnitId,
    string Currency,
    decimal Quantity,
    DateTimeOffset EffectiveAtUtc,
    string? PolicyVersion = null);

public sealed record ResolvedCostProjection(
    Guid CostRecordId,
    int Version,
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
    string? CostSourceCode,
    string? SourceReference,
    DateTimeOffset ResolvedAtUtc,
    string? PolicyVersion = null);
