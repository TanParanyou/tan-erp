using TanErp.Application.Common.Results;

namespace TanErp.Application.Estimates;

public interface IEstimateStore
{
    Task<Result<EstimateDetailProjection>> CreateRevisionAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, string reason, Guid actorUserId,
        string keyHash, string payloadHash, CancellationToken cancellationToken);

    Task<Result<EstimateDetailProjection>> CancelAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, string reason, Guid actorUserId,
        Guid actorMembershipId, string keyHash, string payloadHash, CancellationToken cancellationToken);

    Task<Result<EstimateDetailProjection>> CreateDraftAsync(
        Guid organizationId,
        Guid opportunityId,
        Guid siteSurveyRevisionId,
        string currency,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken);

    Task<EstimateDetailProjection?> GetByIdAsync(
        Guid organizationId,
        Guid estimateId,
        CancellationToken cancellationToken);

    Task<EstimateDetailProjection?> GetByOpportunityIdAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<EstimateCalculationSnapshotProjection>> GetCalculationSnapshotsAsync(
        Guid organizationId,
        Guid estimateId,
        Guid revisionId,
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
        TanErp.Domain.Estimates.EstimateDiscount discount,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken);

    Task<Result<EstimateDetailProjection>> SubmitAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, int revisionNo,
        int calculationVersion, string? note, Guid actorUserId, string keyHash,
        string payloadHash, CancellationToken cancellationToken);

    Task<Result<EstimateDetailProjection>> ReviewAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, int revisionNo,
        string decision, string? reasonCode, string? note, Guid actorUserId,
        Guid reviewerMembershipId, string keyHash, string payloadHash,
        CancellationToken cancellationToken);

    Task<Result<QuotationDetailProjection>> IssueQuotationAsync(
        Guid organizationId,
        Guid estimateId,
        Guid expectedEstimateVersion,
        Guid expectedOpportunityVersion,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken cancellationToken);

    Task<Result<AcceptQuotationProjection>> AcceptQuotationAsync(
        Guid organizationId,
        Guid estimateId,
        Guid expectedOpportunityVersion,
        string? decisionNote,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken cancellationToken);

    Task<Result<TanErp.Application.Estimates.GetQuotationDocument.QuotationDocumentProjection>> GetQuotationDocumentAsync(
        Guid organizationId,
        Guid estimateId,
        string locale,
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
    int SortOrder,
    Guid? ItemId = null,
    Guid? CostRecordId = null,
    int? CostRecordVersion = null,
    string? ProvisionalReasonCode = null,
    string? ProvisionalNote = null);

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
    IReadOnlyList<EstimateCostComponentDraftDto> CostComponents,
    string? SellingRuleReasonCode = null,
    Guid? ItemId = null,
    string? OverrideReasonCode = null,
    string? OverrideReason = null);

public sealed record EstimateSectionDraftDto(
    Guid? Id,
    string Code,
    string NameTh,
    string? NameEn,
    int SortOrder,
    IReadOnlyList<EstimateWorkItemDraftDto> WorkItems);
