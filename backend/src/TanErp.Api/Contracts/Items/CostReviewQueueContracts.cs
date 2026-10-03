using TanErp.Application.Items;

namespace TanErp.Api.Contracts.Items;

public sealed record CostReviewPersonResponse(Guid Id, string DisplayName);
public sealed record CostReviewReferenceResponse(Guid Id, string Code, LocalizedTextResponse Name);
public sealed record CostReviewQueueItemResponse(
    Guid Id, Guid ItemId, CostReviewReferenceResponse Item, string Scope, Guid UnitId, CostReviewReferenceResponse Unit,
    Guid? CostSourceId, CostReviewReferenceResponse? CostSource, decimal Amount, string Currency,
    decimal MinimumQuantity, decimal? MaximumQuantity, DateTimeOffset EffectiveFromUtc, DateTimeOffset? EffectiveToUtc,
    string Status, string? SourceReference, string? Reason, Guid? EvidenceFileId,
    CostReviewPersonResponse Maker, CostReviewPersonResponse LastEditor, Guid RowVersion, DateTimeOffset UpdatedAtUtc);
public sealed record CostReviewQueueResponse(IReadOnlyList<CostReviewQueueItemResponse> Items, int TotalCount, int PageNumber, int PageSize);

public static class CostReviewQueueResponseMapper
{
    public static CostReviewQueueResponse ToResponse(CostReviewQueuePage page) => new(
        page.Items.Select(row => new CostReviewQueueItemResponse(row.Id, row.ItemId, Reference(row.Item), row.Scope,
            row.UnitId, Reference(row.Unit), row.CostSourceId, row.CostSource is null ? null : Reference(row.CostSource),
            row.Amount, row.Currency, row.MinimumQuantity, row.MaximumQuantity, row.EffectiveFromUtc,
            row.EffectiveToUtc, row.Status, row.SourceReference, row.Reason, row.EvidenceFileId,
            new CostReviewPersonResponse(row.Maker.Id, row.Maker.DisplayName),
            new CostReviewPersonResponse(row.LastEditor.Id, row.LastEditor.DisplayName), row.RowVersion, row.UpdatedAtUtc)).ToList(),
        page.TotalCount, page.PageNumber, page.PageSize);

    private static CostReviewReferenceResponse Reference(CostReviewReference reference) => new(reference.Id, reference.Code,
        new LocalizedTextResponse { Thai = reference.Name.Thai, English = reference.Name.English });
}
