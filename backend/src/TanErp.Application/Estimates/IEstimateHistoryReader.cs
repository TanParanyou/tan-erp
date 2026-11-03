namespace TanErp.Application.Estimates;

public sealed record EstimateRevisionSummaryProjection(
    Guid Id,
    int RevisionNo,
    string Status,
    string Currency,
    decimal NetCost,
    decimal NetBeforeTax,
    decimal GrandTotal,
    decimal MarginRate,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record EstimateHistoryEventProjection(
    DateTimeOffset OccurredAtUtc,
    string Action,
    Guid ActorUserId,
    string ActorDisplayName);

/// <param name="BranchId">Used by the caller for the branch access check; never sent to clients.</param>
public sealed record EstimateHistoryProjection(
    Guid BranchId,
    IReadOnlyList<EstimateRevisionSummaryProjection> Revisions,
    IReadOnlyList<EstimateHistoryEventProjection> Events);

public interface IEstimateHistoryReader
{
    /// <summary>Returns null when the estimate does not exist in the organization.</summary>
    Task<EstimateHistoryProjection?> GetAsync(Guid organizationId, Guid estimateId, CancellationToken cancellationToken);
}
