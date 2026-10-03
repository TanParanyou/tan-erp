using TanErp.Domain.Items;

namespace TanErp.Application.Items;

public sealed record CostReviewPerson(Guid Id, string DisplayName);
public sealed record CostReviewReference(Guid Id, string Code, LocalizedText Name);
public sealed record CostReviewQueueRow(
    Guid Id, Guid ItemId, CostReviewReference Item, string Scope, Guid UnitId, CostReviewReference Unit,
    Guid? CostSourceId, CostReviewReference? CostSource, decimal Amount, string Currency,
    decimal MinimumQuantity, decimal? MaximumQuantity, DateTimeOffset EffectiveFromUtc, DateTimeOffset? EffectiveToUtc,
    string Status, string? SourceReference, string? Reason, Guid? EvidenceFileId,
    CostReviewPerson Maker, CostReviewPerson LastEditor, Guid RowVersion, DateTimeOffset UpdatedAtUtc);
public sealed record CostReviewQueuePage(IReadOnlyList<CostReviewQueueRow> Items, int TotalCount, int PageNumber, int PageSize);

public interface ICostReviewQueueReader
{
    Task<CostReviewQueuePage> ListAsync(Guid organizationId, string? status, string? search, int pageNumber, int pageSize, CancellationToken ct);
}
