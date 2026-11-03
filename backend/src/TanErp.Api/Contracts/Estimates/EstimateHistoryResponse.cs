using System.ComponentModel.DataAnnotations;
using TanErp.Application.Estimates;

namespace TanErp.Api.Contracts.Estimates;

public sealed record EstimateActorResponse(
    [property: Required] Guid Id,
    [property: Required] string DisplayName);

public sealed record EstimateRevisionSummaryResponse(
    [property: Required] Guid Id,
    [property: Required] int RevisionNo,
    [property: Required] string Status,
    [property: Required] string Currency,
    [property: Required] decimal NetCost,
    [property: Required] decimal NetBeforeTax,
    [property: Required] decimal GrandTotal,
    [property: Required] decimal MarginRate,
    [property: Required] DateTimeOffset CreatedAtUtc,
    [property: Required] DateTimeOffset UpdatedAtUtc);

public sealed record EstimateHistoryEventResponse(
    [property: Required] DateTimeOffset OccurredAtUtc,
    [property: Required] string Action,
    [property: Required] EstimateActorResponse Actor);

public sealed record EstimateHistoryResponse(
    [property: Required] IReadOnlyList<EstimateRevisionSummaryResponse> Revisions,
    [property: Required] IReadOnlyList<EstimateHistoryEventResponse> Events)
{
    public static EstimateHistoryResponse FromProjection(EstimateHistoryProjection projection) => new(
        projection.Revisions.Select(r => new EstimateRevisionSummaryResponse(
            r.Id, r.RevisionNo, r.Status, r.Currency, r.NetCost, r.NetBeforeTax, r.GrandTotal, r.MarginRate, r.CreatedAtUtc, r.UpdatedAtUtc)).ToList(),
        projection.Events.Select(e => new EstimateHistoryEventResponse(
            e.OccurredAtUtc, e.Action, new EstimateActorResponse(e.ActorUserId, e.ActorDisplayName))).ToList());
}
