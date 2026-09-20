namespace TanErp.Application.Estimates;

public interface IEstimateStore
{
    Task<EstimateDetailProjection> CreateDraftAsync(
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid opportunityId,
        Guid? siteSurveyRevisionId,
        string? siteSurveySnapshotHash,
        string currency,
        Guid actorUserId,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<EstimateDetailProjection?> GetByIdAsync(
        Guid organizationId,
        Guid estimateId,
        CancellationToken cancellationToken);

    Task<EstimateDetailProjection?> GetByOpportunityIdAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken);

    Task<EstimateRevisionProjection> UpdateDraftAsync(
        Guid organizationId,
        Guid estimateId,
        Guid revisionId,
        Guid expectedRevisionVersion,
        IReadOnlyList<EstimateSectionDraftDto> sections,
        Guid actorUserId,
        CancellationToken cancellationToken);

    Task<EstimateRevisionProjection> CalculateAsync(
        Guid organizationId,
        Guid estimateId,
        Guid revisionId,
        Guid expectedRevisionVersion,
        decimal discountAmount,
        Guid actorUserId,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<QuotationDetailProjection> IssueQuotationAsync(
        Guid organizationId,
        Guid estimateId,
        Guid expectedEstimateVersion,
        Guid expectedOpportunityVersion,
        Guid actorUserId,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken);

    Task<AcceptQuotationProjection> AcceptQuotationAsync(
        Guid organizationId,
        Guid estimateId,
        Guid expectedOpportunityVersion,
        string? decisionNote,
        Guid actorUserId,
        string idempotencyKey,
        string traceId,
        CancellationToken cancellationToken);
}

public sealed record EstimateCostComponentDraftDto(
    Guid? Id,
    string Type,
    string Description,
    decimal Quantity,
    string UnitCode,
    decimal UnitCost,
    string? Currency,
    int SortOrder);

public sealed record EstimateWorkItemDraftDto(
    Guid? Id,
    string Code,
    string DescriptionTh,
    string? DescriptionEn,
    decimal Quantity,
    string UnitCode,
    string SellingRuleType,
    decimal SellingRuleValue,
    int SortOrder,
    IReadOnlyList<EstimateCostComponentDraftDto> CostComponents);

public sealed record EstimateSectionDraftDto(
    Guid? Id,
    string Code,
    string NameTh,
    string? NameEn,
    int SortOrder,
    IReadOnlyList<EstimateWorkItemDraftDto> WorkItems);
