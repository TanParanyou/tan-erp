using System.Globalization;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Application.QuickEstimates;

public sealed record QeCaller(string FirebaseUid, Guid MembershipId, string TraceId);

/// <summary>Pricing template governance and quick-estimate use cases. Permission is resolved from PostgreSQL per call.</summary>
public class QuickEstimateHandler
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;
    private static readonly HashSet<string> Channels = new(StringComparer.Ordinal) { "onscreen", "pdf", "line", "email" };
    private static readonly HashSet<string> Locales = new(StringComparer.Ordinal) { "th", "en" };

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IQuickEstimateStore _store;

    public QuickEstimateHandler(IRequestAccessResolver accessResolver, IQuickEstimateStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(QeCaller caller, string permission, CancellationToken ct) =>
        _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static (int Page, int PageSize) Paging(int page, int pageSize) =>
        (Math.Max(1, page), pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize));

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string Num(decimal value) => value.ToString(CultureInfo.InvariantCulture);

    // ----- templates --------------------------------------------------------------------------------

    private static string TemplateKey(TemplateInput t) =>
        string.Join("|", t.Code.Trim().ToUpperInvariant(), t.WorkType, t.Name.Trim(), t.MeasurementRule, t.UnitCode, Num(t.ReferenceRate), Num(t.MinimumCharge), Num(t.BaseRangeRate), Num(t.MaxRangeRate), Num(t.RoundingStep), t.ValidityDays,
            Num(t.TaxRate), t.TaxDisplay, Num(t.DirectShareLimit), t.EffectiveFrom?.ToString("O"), t.EffectiveTo?.ToString("O"),
            string.Join(",", t.Grades.Select(g => $"{g.Code}:{Num(g.Factor)}")), string.Join(",", t.Complexities.Select(c => $"{c.Code}:{Num(c.Factor)}:{Num(c.RiskModifier)}")),
            string.Join(",", t.AddOns.Select(a => $"{a.Code}:{Num(a.Amount)}:{a.PerLine}")), Num(t.LowConfidenceModifier), Num(t.MediumConfidenceModifier), Num(t.CustomMaterialModifier),
            string.Join(";", t.Assumptions), string.Join(";", t.Exclusions));

    public async Task<Result<PricingTemplateProjection>> CreateTemplateAsync(QeCaller caller, string idempotencyKey, TemplateInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.manage", ct);
        if (access.IsFailure) return Result<PricingTemplateProjection>.Failure(access.Error);
        if (input is null || string.IsNullOrWhiteSpace(input.Code)) return Fail<PricingTemplateProjection>("PRICING_TEMPLATE_INVALID", "A template code is required.");
        input = NormalizeLists(input);
        return await _store.CreateTemplateAsync(access.Value!, input, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute(TemplateKey(input)), caller.TraceId, ct);
    }

    private static TemplateInput NormalizeLists(TemplateInput t) => t with
    {
        Grades = t.Grades ?? Array.Empty<GradeInput>(),
        Complexities = t.Complexities ?? Array.Empty<ComplexityInput>(),
        AddOns = t.AddOns ?? Array.Empty<AddOnInput>(),
        Assumptions = t.Assumptions ?? Array.Empty<string>(),
        Exclusions = t.Exclusions ?? Array.Empty<string>()
    };

    public async Task<Result<PricingTemplateProjection>> NewTemplateVersionAsync(QeCaller caller, Guid templateId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.manage", ct);
        if (access.IsFailure) return Result<PricingTemplateProjection>.Failure(access.Error);
        return await _store.NewTemplateVersionAsync(access.Value!, templateId, caller.TraceId, ct);
    }

    public async Task<Result<PricingTemplateProjection>> UpdateTemplateAsync(QeCaller caller, Guid templateId, Guid expectedVersion, TemplateInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.manage", ct);
        if (access.IsFailure) return Result<PricingTemplateProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PricingTemplateProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        if (input is null) return Fail<PricingTemplateProjection>("PRICING_TEMPLATE_INVALID", "A template body is required.");
        return await _store.UpdateTemplateAsync(access.Value!, templateId, expectedVersion, NormalizeLists(input), caller.TraceId, ct);
    }

    public async Task<Result<PricingTemplateProjection>> TemplateActionAsync(QeCaller caller, Guid templateId, Guid expectedVersion, TemplateAction action, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.manage", ct);
        if (access.IsFailure) return Result<PricingTemplateProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PricingTemplateProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        return await _store.TemplateActionAsync(access.Value!, templateId, expectedVersion, action, caller.TraceId, ct);
    }

    public async Task<Result<PricingTemplateProjection>> DecideTemplateAsync(QeCaller caller, Guid templateId, Guid expectedVersion, string? decision, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.approve", ct);
        if (access.IsFailure) return Result<PricingTemplateProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<PricingTemplateProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        if (decision is not ("approved" or "returned")) return Fail<PricingTemplateProjection>("QUICK_ESTIMATE_FIELD_INVALID", "The decision must be approved or returned.");
        return await _store.DecideTemplateAsync(access.Value!, templateId, expectedVersion, decision == "approved", Clean(note), caller.TraceId, ct);
    }

    public async Task<Result<PricingTemplateProjection>> GetTemplateAsync(QeCaller caller, Guid templateId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.read", ct);
        if (access.IsFailure) return Result<PricingTemplateProjection>.Failure(access.Error);
        var template = await _store.GetTemplateAsync(access.Value!.OrganizationId, templateId, ct);
        return template is null ? Fail<PricingTemplateProjection>("RESOURCE_NOT_FOUND", "Pricing template not found.") : Result<PricingTemplateProjection>.Success(template);
    }

    public async Task<Result<PagedTemplates>> ListTemplatesAsync(QeCaller caller, string? search, string? status, string? workType, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.read", ct);
        if (access.IsFailure) return Result<PagedTemplates>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !TemplateStatus.All.Contains(status.Trim())) return Fail<PagedTemplates>("QUICK_ESTIMATE_FIELD_INVALID", "The status filter is invalid.");
        if (!string.IsNullOrWhiteSpace(workType) && !WorkType.All.Contains(workType.Trim())) return Fail<PagedTemplates>("QUICK_ESTIMATE_FIELD_INVALID", "The work type filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedTemplates>.Success(await _store.ListTemplatesAsync(access.Value!.OrganizationId, new TemplateListQuery(Clean(search), Clean(status), Clean(workType), p, size), ct));
    }

    public async Task<Result<IReadOnlyList<EffectiveTemplateProjection>>> ListEffectiveAsync(QeCaller caller, string? workType, DateOnly? on, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.read", ct);
        if (access.IsFailure) return Result<IReadOnlyList<EffectiveTemplateProjection>>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(workType) && !WorkType.All.Contains(workType.Trim())) return Fail<IReadOnlyList<EffectiveTemplateProjection>>("QUICK_ESTIMATE_FIELD_INVALID", "The work type is invalid.");
        return Result<IReadOnlyList<EffectiveTemplateProjection>>.Success(await _store.ListEffectiveTemplatesAsync(access.Value!.OrganizationId, Clean(workType), on ?? DateOnly.FromDateTime(DateTime.UtcNow), ct));
    }

    // ----- quick estimates --------------------------------------------------------------------------

    public async Task<Result<QuickEstimateProjection>> CreateAsync(QeCaller caller, string idempotencyKey, Guid? customerId, Guid? opportunityId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.create", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        var payloadHash = Sha256Hex.Compute($"{customerId}|{opportunityId}");
        return await _store.CreateQuickEstimateAsync(access.Value!, customerId, opportunityId, Sha256Hex.Compute(idempotencyKey), payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> PatchAsync(QeCaller caller, Guid id, Guid expectedVersion, QuickEstimateDraftPatch? patch, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.update", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        if (patch is null) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_INVALID", "A patch body is required.");
        return await _store.PatchDraftAsync(access.Value!, id, expectedVersion, patch, caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> CalculateAsync(QeCaller caller, Guid id, Guid expectedVersion, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.update", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        return await _store.CalculateAsync(access.Value!, id, expectedVersion, caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> SubmitReviewAsync(QeCaller caller, Guid id, int sourceVersion, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.update", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        if (note is { Length: > 500 }) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_INVALID", "The note cannot exceed 500 characters.");
        return await _store.SubmitReviewAsync(access.Value!, id, sourceVersion, Clean(note), caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> DecideReviewAsync(QeCaller caller, Guid id, int sourceVersion, string? decision, string? reasonCode, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.review", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        if (decision is not ("approved" or "returned")) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_INVALID", "The decision must be approved or returned.");
        if (string.IsNullOrWhiteSpace(reasonCode)) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_REASON_REQUIRED", "A reason code is required.");
        if (note is { Length: > 500 }) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_INVALID", "The note cannot exceed 500 characters.");
        return await _store.DecideReviewAsync(access.Value!, id, sourceVersion, decision == "approved", reasonCode.Trim(), Clean(note), caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> ShareAsync(QeCaller caller, Guid id, string idempotencyKey, ShareInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.share", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        if (input is null || !Channels.Contains(input.Channel ?? string.Empty) || !Locales.Contains(input.Locale ?? string.Empty) || input.Recipient is { Length: > 200 })
        {
            return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_INVALID", "A valid channel, locale and recipient are required.");
        }

        var payloadHash = Sha256Hex.Compute($"{id}|{input.SourceVersion}|{input.Channel}|{Clean(input.Recipient)}|{input.Locale}");
        return await _store.ShareAsync(access.Value!, id, input, Sha256Hex.Compute(idempotencyKey), payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> ConvertAsync(QeCaller caller, Guid id, string idempotencyKey, ConversionInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.convert", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        var estimates = await AccessAsync(caller, "estimates.create", ct);
        if (estimates.IsFailure) return Result<QuickEstimateProjection>.Failure(estimates.Error);
        if (input is null || input.SiteSurveyRevisionId == Guid.Empty) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "A site survey revision is required to start an official estimate.");
        var payloadHash = Sha256Hex.Compute($"{id}|{input.SourceVersion}|{input.SiteSurveyRevisionId}");
        return await _store.ConvertAsync(access.Value!, id, input, Sha256Hex.Compute(idempotencyKey), payloadHash, caller.TraceId, ct);
    }

    public async Task<Result<QuickEstimateProjection>> GetAsync(QeCaller caller, Guid id, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.read", ct);
        if (access.IsFailure) return Result<QuickEstimateProjection>.Failure(access.Error);
        var estimate = await _store.GetQuickEstimateAsync(access.Value!.OrganizationId, id, ct);
        return estimate is null ? Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Quick estimate not found.") : Result<QuickEstimateProjection>.Success(estimate);
    }

    /// <summary>The full calculation snapshot holds internal factors, so it needs the review permission.</summary>
    public async Task<Result<string>> GetSnapshotAsync(QeCaller caller, Guid id, int version, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.review", ct);
        if (access.IsFailure) return Result<string>.Failure(access.Error);
        var snapshot = await _store.GetCalculationSnapshotAsync(access.Value!.OrganizationId, id, version, ct);
        return snapshot is null ? Fail<string>("RESOURCE_NOT_FOUND", "Calculation not found.") : Result<string>.Success(snapshot);
    }

    public async Task<Result<PagedQuickEstimates>> ListAsync(QeCaller caller, string? search, string? status, int page, int pageSize, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "quick-estimates.read", ct);
        if (access.IsFailure) return Result<PagedQuickEstimates>.Failure(access.Error);
        if (!string.IsNullOrWhiteSpace(status) && !QuickEstimateStatus.All.Contains(status.Trim())) return Fail<PagedQuickEstimates>("QUICK_ESTIMATE_FIELD_INVALID", "The status filter is invalid.");
        var (p, size) = Paging(page, pageSize);
        return Result<PagedQuickEstimates>.Success(await _store.ListQuickEstimatesAsync(access.Value!.OrganizationId, new QuickEstimateListQuery(Clean(search), Clean(status), p, size), ct));
    }
}
