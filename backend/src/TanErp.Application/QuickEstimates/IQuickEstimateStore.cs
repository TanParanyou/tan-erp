using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.QuickEstimates;

public interface IQuickEstimateStore
{
    Task<Result<PricingTemplateProjection>> CreateTemplateAsync(RequestAccessContext access, TemplateInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<PricingTemplateProjection>> NewTemplateVersionAsync(RequestAccessContext access, Guid templateId, string traceId, CancellationToken ct = default);
    Task<Result<PricingTemplateProjection>> UpdateTemplateAsync(RequestAccessContext access, Guid templateId, Guid expectedVersion, TemplateInput input, string traceId, CancellationToken ct = default);
    Task<Result<PricingTemplateProjection>> TemplateActionAsync(RequestAccessContext access, Guid templateId, Guid expectedVersion, TemplateAction action, string traceId, CancellationToken ct = default);
    Task<Result<PricingTemplateProjection>> DecideTemplateAsync(RequestAccessContext access, Guid templateId, Guid expectedVersion, bool approve, string? note, string traceId, CancellationToken ct = default);
    Task<PricingTemplateProjection?> GetTemplateAsync(Guid organizationId, Guid templateId, CancellationToken ct = default);
    Task<PagedTemplates> ListTemplatesAsync(Guid organizationId, TemplateListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<EffectiveTemplateProjection>> ListEffectiveTemplatesAsync(Guid organizationId, string? workType, DateOnly on, CancellationToken ct = default);

    Task<Result<QuickEstimateProjection>> CreateQuickEstimateAsync(RequestAccessContext access, Guid? customerId, Guid? opportunityId, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<QuickEstimateProjection>> PatchDraftAsync(RequestAccessContext access, Guid id, Guid expectedVersion, QuickEstimateDraftPatch patch, string traceId, CancellationToken ct = default);
    Task<Result<QuickEstimateProjection>> CalculateAsync(RequestAccessContext access, Guid id, Guid expectedVersion, string traceId, CancellationToken ct = default);
    Task<Result<QuickEstimateProjection>> SubmitReviewAsync(RequestAccessContext access, Guid id, int sourceVersion, string? note, string traceId, CancellationToken ct = default);
    Task<Result<QuickEstimateProjection>> DecideReviewAsync(RequestAccessContext access, Guid id, int sourceVersion, bool approved, string reasonCode, string? note, string traceId, CancellationToken ct = default);
    Task<Result<QuickEstimateProjection>> ShareAsync(RequestAccessContext access, Guid id, ShareInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<QuickEstimateProjection>> ConvertAsync(RequestAccessContext access, Guid id, ConversionInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<QuickEstimateProjection?> GetQuickEstimateAsync(Guid organizationId, Guid id, CancellationToken ct = default);
    Task<string?> GetCalculationSnapshotAsync(Guid organizationId, Guid id, int version, CancellationToken ct = default);
    Task<PagedQuickEstimates> ListQuickEstimatesAsync(Guid organizationId, QuickEstimateListQuery query, CancellationToken ct = default);
}
