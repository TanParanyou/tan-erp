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

public sealed record EstimateRevisionLineProjection(
    string SectionCode,
    string WorkItemCode,
    string DescriptionTh,
    decimal Quantity,
    string UnitCode,
    decimal TotalCost,
    decimal TotalSellingPrice);

/// <param name="BranchId">Used by the caller for the branch access check; never sent to clients.</param>
public sealed record EstimateRevisionLinesProjection(Guid BranchId, IReadOnlyList<EstimateRevisionLineProjection> Lines);

public interface IEstimateHistoryReader
{
    /// <summary>Returns null when the estimate or the revision does not exist in the organization.</summary>
    Task<EstimateRevisionLinesProjection?> GetRevisionLinesAsync(Guid organizationId, Guid estimateId, Guid revisionId, CancellationToken cancellationToken);

    /// <summary>Returns null when the estimate does not exist in the organization.</summary>
    Task<EstimateHistoryProjection?> GetAsync(Guid organizationId, Guid estimateId, CancellationToken cancellationToken);
}
