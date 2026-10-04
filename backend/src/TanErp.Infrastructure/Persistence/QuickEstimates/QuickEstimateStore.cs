using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates;
using TanErp.Application.QuickEstimates;
using TanErp.Domain.Common;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Infrastructure.Persistence.QuickEstimates;

public class QuickEstimateStore : IQuickEstimateStore
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _numbers;
    private readonly IEstimateStore _estimates;

    public QuickEstimateStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, IEstimateStore estimates)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _estimates = estimates;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private static Result<T> Fail<T>(QuickEstimateException ex) => Fail<T>(ex.Code, ex.Message);

    private DateOnly Today => DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime);

    private void Audit(RequestAccessContext access, string action, string resourceType, Guid resourceId, string traceId, object changes, DateTimeOffset now)
    {
        _db.AuditEvents.Add(new AuditEvent(
            Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, resourceType, resourceId.ToString(), now, traceId,
            JsonSerializer.Serialize(changes),
            branchId: access.BranchId,
            actorMembershipId: access.MembershipId));
    }

    private async Task<IdempotencyRecord?> FindReplayAsync(Guid orgId, string operation, string keyHash, CancellationToken ct)
    {
        var lockKey = $"{orgId:N}:{operation}:{keyHash}";
        await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
    }

    private async Task<Dictionary<Guid, QePerson>> PeopleAsync(IEnumerable<Guid?> ids, CancellationToken ct)
    {
        var list = ids.Where(i => i.HasValue).Select(i => i!.Value).Distinct().ToList();
        return await _db.Users.AsNoTracking().Where(u => list.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new QePerson(u.Id, u.DisplayName), ct);
    }

    // ===== pricing templates =========================================================================

    private static TemplateConfig ToConfig(TemplateInput t) => new(
        t.Grades.Select(g => new TemplateGrade(g.Code.Trim(), g.Name.Trim(), g.Factor)).ToList(),
        t.Complexities.Select(c => new TemplateComplexity(c.Code.Trim(), c.Name.Trim(), c.Factor, c.RiskModifier)).ToList(),
        t.AddOns.Select(a => new TemplateAddOn(a.Code.Trim(), a.Name.Trim(), a.Amount, a.PerLine)).ToList(),
        t.LowConfidenceModifier, t.MediumConfidenceModifier, t.CustomMaterialModifier,
        t.Assumptions.Select(a => a.Trim()).ToList(), t.Exclusions.Select(e => e.Trim()).ToList());

    private PricingTemplateProjection ToProjection(PricingTemplate t, Dictionary<Guid, QePerson> people)
    {
        var c = t.Config;
        QePerson Person(Guid id) => people.TryGetValue(id, out var p) ? p : new QePerson(id, string.Empty);
        return new PricingTemplateProjection(
            t.Id, t.Code, t.Version, t.WorkType, t.Name, t.Status, t.MeasurementRule, t.UnitCode, t.ReferenceRate, t.MinimumCharge, t.BaseRangeRate, t.MaxRangeRate, t.RoundingStep,
            t.ValidityDays, t.TaxRate, t.TaxDisplay, t.DirectShareLimit, t.EffectiveFrom, t.EffectiveTo,
            c.Grades.Select(g => new GradeInput(g.Code, g.Name, g.Factor)).ToList(),
            c.Complexities.Select(x => new ComplexityInput(x.Code, x.Name, x.Factor, x.RiskModifier)).ToList(),
            c.AddOns.Select(a => new AddOnInput(a.Code, a.Name, a.Amount, a.PerLine)).ToList(),
            c.LowConfidenceModifier, c.MediumConfidenceModifier, c.CustomMaterialModifier, c.Assumptions, c.Exclusions,
            Person(t.CreatedByUserId), t.DecidedByUserId.HasValue ? Person(t.DecidedByUserId.Value) : null, t.DecisionNote, t.CreatedAtUtc, t.RowVersion);
    }

    private async Task<PricingTemplateProjection> ProjectAsync(PricingTemplate t, CancellationToken ct) =>
        ToProjection(t, await PeopleAsync(new Guid?[] { t.CreatedByUserId, t.DecidedByUserId }, ct));

    public async Task<Result<PricingTemplateProjection>> CreateTemplateAsync(
        RequestAccessContext access, TemplateInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quick-estimate.template.create";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<PricingTemplateProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var existing = Guid.TryParse(replay.ResourceId, out var id) ? await GetTemplateAsync(orgId, id, ct) : null;
                if (existing is not null) return Result<PricingTemplateProjection>.Success(existing);
            }

            var code = input.Code.Trim().ToUpperInvariant();
            if (await _db.PricingTemplates.AnyAsync(t => t.OrganizationId == orgId && t.Code == code, ct))
            {
                return Fail<PricingTemplateProjection>("PRICING_TEMPLATE_CODE_EXISTS", "A template with this code already exists; create a new version instead.");
            }

            var now = _clock.UtcNow;
            try
            {
                var template = new PricingTemplate(Guid.NewGuid(), orgId, code, 1, input.WorkType, input.Name, input.MeasurementRule, input.UnitCode, input.ReferenceRate, input.MinimumCharge,
                    input.BaseRangeRate, input.MaxRangeRate, input.RoundingStep, input.ValidityDays, input.TaxRate, input.TaxDisplay, input.DirectShareLimit, input.EffectiveFrom, input.EffectiveTo,
                    ToConfig(input), access.ActorUserId, now);
                _db.PricingTemplates.Add(template);
                Audit(access, "pricing-template.created", "PricingTemplate", template.Id, traceId, new { code, version = 1 }, now);
                _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, template.Id.ToString(), now));
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return Result<PricingTemplateProjection>.Success(await ProjectAsync(template, ct));
            }
            catch (QuickEstimateException ex)
            {
                return Fail<PricingTemplateProjection>(ex);
            }
        });
    }

    public async Task<Result<PricingTemplateProjection>> NewTemplateVersionAsync(RequestAccessContext access, Guid templateId, string traceId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var source = await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == templateId && t.OrganizationId == orgId, ct);
            if (source is null) return Fail<PricingTemplateProjection>("RESOURCE_NOT_FOUND", "Pricing template not found.");
            var versions = await _db.PricingTemplates.AsNoTracking().Where(t => t.OrganizationId == orgId && t.Code == source.Code).ToListAsync(ct);
            if (versions.Any(t => t.Status is TemplateStatus.Draft or TemplateStatus.Submitted))
            {
                return Fail<PricingTemplateProjection>("PRICING_TEMPLATE_DRAFT_EXISTS", "This template already has a version being prepared.");
            }

            var now = _clock.UtcNow;
            // The new version starts as a copy; any change to a rate, factor or policy is made on it, never on the source.
            var copy = new PricingTemplate(Guid.NewGuid(), orgId, source.Code, versions.Max(t => t.Version) + 1, source.WorkType, source.Name, source.MeasurementRule, source.UnitCode,
                source.ReferenceRate, source.MinimumCharge, source.BaseRangeRate, source.MaxRangeRate, source.RoundingStep, source.ValidityDays, source.TaxRate, source.TaxDisplay,
                source.DirectShareLimit, source.EffectiveFrom, source.EffectiveTo, source.Config, access.ActorUserId, now);
            _db.PricingTemplates.Add(copy);
            Audit(access, "pricing-template.version-created", "PricingTemplate", copy.Id, traceId, new { code = copy.Code, version = copy.Version }, now);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<PricingTemplateProjection>.Success(await ProjectAsync(copy, ct));
        });
    }

    private async Task<Result<PricingTemplateProjection>> MutateTemplateAsync(
        RequestAccessContext access, Guid templateId, Guid expectedVersion, Func<PricingTemplate, DateTimeOffset, Task<string>> mutate, string traceId, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var template = await _db.PricingTemplates.FirstOrDefaultAsync(t => t.Id == templateId && t.OrganizationId == orgId, ct);
            if (template is null) return Fail<PricingTemplateProjection>("RESOURCE_NOT_FOUND", "Pricing template not found.");
            if (template.RowVersion != expectedVersion) return Fail<PricingTemplateProjection>("PRICING_TEMPLATE_VERSION_CONFLICT", "Pricing template version conflict.");

            var now = _clock.UtcNow;
            string audit;
            try
            {
                audit = await mutate(template, now);
            }
            catch (QuickEstimateException ex)
            {
                return Fail<PricingTemplateProjection>(ex);
            }

            Audit(access, audit, "PricingTemplate", template.Id, traceId, new { code = template.Code, version = template.Version, status = template.Status }, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<PricingTemplateProjection>("PRICING_TEMPLATE_VERSION_CONFLICT", "Pricing template version conflict.");
            }

            return Result<PricingTemplateProjection>.Success(await ProjectAsync(template, ct));
        });
    }

    public Task<Result<PricingTemplateProjection>> UpdateTemplateAsync(RequestAccessContext access, Guid templateId, Guid expectedVersion, TemplateInput input, string traceId, CancellationToken ct = default) =>
        MutateTemplateAsync(access, templateId, expectedVersion, (t, now) =>
        {
            if (!string.Equals(t.Code, input.Code.Trim(), StringComparison.OrdinalIgnoreCase)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID", "The code of a template cannot change.");
            t.Edit(input.WorkType, input.Name, input.MeasurementRule, input.UnitCode, input.ReferenceRate, input.MinimumCharge, input.BaseRangeRate, input.MaxRangeRate, input.RoundingStep,
                input.ValidityDays, input.TaxRate, input.TaxDisplay, input.DirectShareLimit, input.EffectiveFrom, input.EffectiveTo, ToConfig(input), now);
            return Task.FromResult("pricing-template.updated");
        }, traceId, ct);

    public Task<Result<PricingTemplateProjection>> DecideTemplateAsync(RequestAccessContext access, Guid templateId, Guid expectedVersion, bool approve, string? note, string traceId, CancellationToken ct = default) =>
        MutateTemplateAsync(access, templateId, expectedVersion, (t, now) =>
        {
            t.Decide(approve, note, access.ActorUserId, now);
            return Task.FromResult(approve ? "pricing-template.approved" : "pricing-template.returned");
        }, traceId, ct);

    public Task<Result<PricingTemplateProjection>> TemplateActionAsync(RequestAccessContext access, Guid templateId, Guid expectedVersion, TemplateAction action, string traceId, CancellationToken ct = default) =>
        MutateTemplateAsync(access, templateId, expectedVersion, async (t, now) =>
        {
            switch (action)
            {
                case TemplateAction.Submit:
                    t.Submit(access.ActorUserId, now);
                    return "pricing-template.submitted";
                case TemplateAction.Calibrate:
                    t.StartCalibration(now);
                    return "pricing-template.calibration-started";
                case TemplateAction.Activate:
                {
                    if (t.Status is not (TemplateStatus.Calibration or TemplateStatus.Approved)) throw new QuickEstimateException("PRICING_TEMPLATE_INVALID_STATE", $"Cannot activate a template that is '{t.Status}'.");
                    // Older versions of the same code are retired first so the "one active version" index never sees two.
                    var others = await _db.PricingTemplates.Where(x => x.OrganizationId == t.OrganizationId && x.Code == t.Code && x.Id != t.Id && (x.Status == TemplateStatus.Active || x.Status == TemplateStatus.Calibration)).ToListAsync();
                    foreach (var other in others) other.Supersede(now);
                    if (others.Count > 0) await _db.SaveChangesAsync();
                    t.Activate(now);
                    return "pricing-template.activated";
                }
                default:
                    t.Disable(now);
                    return "pricing-template.disabled";
            }
        }, traceId, ct);

    public async Task<PricingTemplateProjection?> GetTemplateAsync(Guid organizationId, Guid templateId, CancellationToken ct = default)
    {
        var template = await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == templateId && t.OrganizationId == organizationId, ct);
        return template is null ? null : await ProjectAsync(template, ct);
    }

    public async Task<PagedTemplates> ListTemplatesAsync(Guid organizationId, TemplateListQuery query, CancellationToken ct = default)
    {
        var templates = _db.PricingTemplates.AsNoTracking().Where(t => t.OrganizationId == organizationId);
        if (query.Status is not null) templates = templates.Where(t => t.Status == query.Status);
        if (query.WorkType is not null) templates = templates.Where(t => t.WorkType == query.WorkType);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            templates = templates.Where(t => t.Code.Contains(needle) || t.Name.ToUpper().Contains(needle));
        }

        var total = await templates.CountAsync(ct);
        var rows = await templates.OrderBy(t => t.Code).ThenByDescending(t => t.Version).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(t => new PricingTemplateListItemProjection(t.Id, t.Code, t.Version, t.WorkType, t.Name, t.Status, t.UpdatedAtUtc)).ToListAsync(ct);
        return new PagedTemplates(rows, total, query.Page, query.PageSize);
    }

    public async Task<IReadOnlyList<EffectiveTemplateProjection>> ListEffectiveTemplatesAsync(Guid organizationId, string? workType, DateOnly on, CancellationToken ct = default)
    {
        var candidates = await _db.PricingTemplates.AsNoTracking()
            .Where(t => t.OrganizationId == organizationId && (t.Status == TemplateStatus.Active || t.Status == TemplateStatus.Calibration) && (workType == null || t.WorkType == workType))
            .OrderBy(t => t.Code).ThenByDescending(t => t.Version).ToListAsync(ct);
        return candidates.Where(t => t.IsUsableOn(on)).Select(t =>
        {
            var c = t.Config;
            return new EffectiveTemplateProjection(
                t.Id, t.Code, t.Version, t.WorkType, t.Name, t.Status, t.MeasurementRule, t.UnitCode, t.TaxDisplay, t.ValidityDays,
                c.Grades.Select(g => new EffectiveOption(g.Code, g.Name)).ToList(), c.Complexities.Select(x => new EffectiveOption(x.Code, x.Name)).ToList(),
                c.AddOns.Select(a => new EffectiveAddOn(a.Code, a.Name, a.PerLine)).ToList(), c.Assumptions, c.Exclusions);
        }).ToList();
    }

    // ===== quick estimates ===========================================================================

    public async Task<Result<QuickEstimateProjection>> CreateQuickEstimateAsync(
        RequestAccessContext access, Guid? customerId, Guid? opportunityId, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quick-estimate.create";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<QuickEstimateProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<QuickEstimateProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var existing = Guid.TryParse(replay.ResourceId, out var id) ? await GetQuickEstimateAsync(orgId, id, ct) : null;
                if (existing is not null) return Result<QuickEstimateProjection>.Success(existing);
            }

            // The organization always comes from the trusted context; references from the body must belong to it.
            if (customerId.HasValue && !await _db.Customers.AnyAsync(c => c.Id == customerId && c.OrganizationId == orgId, ct)) return Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Customer not found.");
            if (opportunityId.HasValue && !await _db.Opportunities.AnyAsync(o => o.Id == opportunityId && o.OrganizationId == orgId, ct)) return Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Opportunity not found.");

            var now = _clock.UtcNow;
            string number;
            try
            {
                number = await _numbers.GenerateAsync(orgId, DocumentTypes.QuickEstimates, access.BranchId, now, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Fail<QuickEstimateProjection>("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate quick estimate document number.");
            }

            var estimate = new QuickEstimate(Guid.NewGuid(), orgId, access.BranchId.Value, number, customerId, opportunityId, access.ActorUserId, now);
            _db.QuickEstimates.Add(estimate);
            Audit(access, "quick-estimate.created", "QuickEstimate", estimate.Id, traceId, new { number }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, estimate.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<QuickEstimateProjection>.Success((await GetQuickEstimateAsync(orgId, estimate.Id, ct))!);
        });
    }

    private async Task<Result<QuickEstimateProjection>> MutateAsync(
        RequestAccessContext access, Guid id, Func<QuickEstimate, DateTimeOffset, Task<(string Audit, Result<QuickEstimateProjection>? Early)>> mutate, string traceId, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM quick_estimate.quick_estimates WHERE id = {id} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
            var estimate = await _db.QuickEstimates.FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == orgId, ct);
            if (estimate is null) return Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Quick estimate not found.");

            var now = _clock.UtcNow;
            (string Audit, Result<QuickEstimateProjection>? Early) outcome;
            try
            {
                outcome = await mutate(estimate, now);
            }
            catch (QuickEstimateInputException ex)
            {
                return Fail<QuickEstimateProjection>(ex.Code, $"{ex.Message} {string.Join(", ", ex.Fields)}");
            }
            catch (QuickEstimateException ex)
            {
                return Fail<QuickEstimateProjection>(ex);
            }

            if (outcome.Early is not null) return outcome.Early;
            Audit(access, outcome.Audit, "QuickEstimate", estimate.Id, traceId, new { number = estimate.Number, status = estimate.Status }, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_VERSION_CONFLICT", "Quick estimate version conflict.");
            }

            return Result<QuickEstimateProjection>.Success((await GetQuickEstimateAsync(orgId, estimate.Id, ct))!);
        });
    }

    private static readonly Task<(string, Result<QuickEstimateProjection>?)> NoEarly = Task.FromResult<(string, Result<QuickEstimateProjection>?)>(("", null));

    public Task<Result<QuickEstimateProjection>> PatchDraftAsync(RequestAccessContext access, Guid id, Guid expectedVersion, QuickEstimateDraftPatch patch, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, id, async (e, now) =>
        {
            if (e.RowVersion != expectedVersion) return ("", Fail<QuickEstimateProjection>("QUICK_ESTIMATE_VERSION_CONFLICT", "Quick estimate version conflict."));
            if (patch.TemplateId.HasValue)
            {
                var template = await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == patch.TemplateId && t.OrganizationId == access.OrganizationId, ct);
                if (template is null) return ("", Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Pricing template not found."));
                if (!template.IsUsableOn(Today)) return ("", Fail<QuickEstimateProjection>("PRICING_TEMPLATE_NOT_USABLE", "The pricing template is not usable."));
            }

            e.UpdateDraft(patch.TemplateId, patch.PropertyType, patch.RoomOrArea, patch.GradeCode, patch.ComplexityCodes, patch.AddOnCodes, patch.MeasurementConfidence, patch.CustomMaterial,
                patch.Measurements?.Select(m => new MeasurementLine(m.LineId == Guid.Empty ? Guid.NewGuid() : m.LineId, m.WorkSubtype, m.WidthM, m.HeightM, m.DepthM, m.Quantity)).ToList(), now);
            return ("quick-estimate.draft-updated", null);
        }, traceId, ct);

    public Task<Result<QuickEstimateProjection>> CalculateAsync(RequestAccessContext access, Guid id, Guid expectedVersion, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, id, async (e, now) =>
        {
            if (e.RowVersion != expectedVersion) return ("", Fail<QuickEstimateProjection>("QUICK_ESTIMATE_VERSION_CONFLICT", "Quick estimate version conflict."));
            if (!e.TemplateId.HasValue) return ("", Fail<QuickEstimateProjection>("QUICK_ESTIMATE_TEMPLATE_REQUIRED", "Choose a pricing template first."));
            var template = await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == e.TemplateId && t.OrganizationId == access.OrganizationId, ct);
            if (template is null || !template.IsUsableOn(Today)) return ("", Fail<QuickEstimateProjection>("PRICING_TEMPLATE_NOT_USABLE", "The pricing template is not usable."));

            var result = QuickEstimateEngine.Calculate(template, e.ToInput(), Today);
            var latest = await _db.QuickEstimateCalculations.AsNoTracking().Where(c => c.QuickEstimateId == e.Id).OrderByDescending(c => c.Version).FirstOrDefaultAsync(ct);
            if (latest is not null && latest.InputHash == result.InputHash && latest.TemplateId == template.Id)
            {
                // Nothing that affects the price changed since the last calculation: keep that version.
                e.RecordCalculation(latest.Version, now);
                return ("quick-estimate.calculation-reused", null);
            }

            var version = (latest?.Version ?? 0) + 1;
            _db.QuickEstimateCalculations.Add(new QuickEstimateCalculation(Guid.NewGuid(), access.OrganizationId, e.Id, version, template, result, access.ActorUserId, now));
            e.RecordCalculation(version, now);
            return ("quick-estimate.calculated", null);
        }, traceId, ct);

    private async Task<QuickEstimateCalculation?> CurrentCalculationAsync(QuickEstimate e, int sourceVersion, CancellationToken ct) =>
        await _db.QuickEstimateCalculations.AsNoTracking().FirstOrDefaultAsync(c => c.QuickEstimateId == e.Id && c.Version == sourceVersion, ct);

    public Task<Result<QuickEstimateProjection>> SubmitReviewAsync(RequestAccessContext access, Guid id, int sourceVersion, string? note, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, id, async (e, now) =>
        {
            var existing = await _db.QuickEstimateReviews.AsNoTracking().FirstOrDefaultAsync(r => r.QuickEstimateId == e.Id && r.SourceVersion == sourceVersion, ct);
            // A retried submit finds the review it already created.
            if (existing is not null) return ("", Result<QuickEstimateProjection>.Success((await GetQuickEstimateAsync(access.OrganizationId, e.Id, ct))!));
            if (sourceVersion != e.CurrentCalculationVersion || e.CurrentCalculationVersion == 0) return ("", Fail<QuickEstimateProjection>("QUICK_ESTIMATE_STALE_VERSION", "The calculation version is no longer current."));
            e.SubmitForReview(now);
            _db.QuickEstimateReviews.Add(new QuickEstimateReview(Guid.NewGuid(), access.OrganizationId, e.Id, sourceVersion, note, access.ActorUserId, now));
            return ("quick-estimate.review-requested", null);
        }, traceId, ct);

    public Task<Result<QuickEstimateProjection>> DecideReviewAsync(RequestAccessContext access, Guid id, int sourceVersion, bool approved, string reasonCode, string? note, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, id, async (e, now) =>
        {
            var review = await _db.QuickEstimateReviews.FirstOrDefaultAsync(r => r.QuickEstimateId == e.Id && r.SourceVersion == sourceVersion, ct);
            if (review is null) return ("", Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Review not found."));
            if (sourceVersion != e.CurrentCalculationVersion) return ("", Fail<QuickEstimateProjection>("QUICK_ESTIMATE_STALE_VERSION", "The calculation version is no longer current."));
            review.Decide(approved, reasonCode, note, access.ActorUserId, e.CreatedByUserId, now);
            e.ApplyReviewDecision(approved, now);
            return (approved ? "quick-estimate.review-approved" : "quick-estimate.review-returned", null);
        }, traceId, ct);

    /// <summary>Whether this calculation may leave the building: shareable on its own, or reviewed and approved.</summary>
    private async Task<Result<QuickEstimateCalculation>> CheckReleasableAsync(QuickEstimate e, int sourceVersion, CancellationToken ct)
    {
        var calc = await CurrentCalculationAsync(e, sourceVersion, ct);
        if (calc is null) return Fail<QuickEstimateCalculation>("RESOURCE_NOT_FOUND", "Calculation not found.");
        if (sourceVersion != e.CurrentCalculationVersion || e.Status == QuickEstimateStatus.Draft) return Fail<QuickEstimateCalculation>("QUICK_ESTIMATE_STALE_VERSION", "The calculation version is no longer current.");
        if (calc.ValidUntil < Today) return Fail<QuickEstimateCalculation>("QUICK_ESTIMATE_EXPIRED", "The calculation has expired; calculate again.");
        if (calc.ShareDecision == ShareDecision.Blocked) return Fail<QuickEstimateCalculation>("QUICK_ESTIMATE_SHARE_BLOCKED", "This estimate cannot be shared.");
        if (calc.ShareDecision == ShareDecision.PendingReview)
        {
            var approved = await _db.QuickEstimateReviews.AsNoTracking().AnyAsync(r => r.QuickEstimateId == e.Id && r.SourceVersion == sourceVersion && r.Status == ReviewStatus.Approved, ct);
            if (!approved) return Fail<QuickEstimateCalculation>("QUICK_ESTIMATE_REVIEW_REQUIRED", "A reviewer must approve this estimate first.");
        }

        return Result<QuickEstimateCalculation>.Success(calc);
    }

    public async Task<Result<QuickEstimateProjection>> ShareAsync(
        RequestAccessContext access, Guid id, ShareInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quick-estimate.share";
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var replay = await FindReplayAsync(orgId, operation, keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<QuickEstimateProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var replayed = await GetQuickEstimateAsync(orgId, id, ct);
                if (replayed is not null) return Result<QuickEstimateProjection>.Success(replayed);
            }

            var estimate = await _db.QuickEstimates.FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == orgId, ct);
            if (estimate is null) return Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Quick estimate not found.");
            var releasable = await CheckReleasableAsync(estimate, input.SourceVersion, ct);
            if (releasable.IsFailure) return Result<QuickEstimateProjection>.Failure(releasable.Error);

            var calc = releasable.Value!;
            var template = await _db.PricingTemplates.AsNoTracking().FirstAsync(t => t.Id == calc.TemplateId, ct);
            var config = template.Config;
            // The customer summary is built from the frozen result only: no factors, cost, reviewer notes or thresholds.
            var summary = JsonSerializer.Serialize(new
            {
                estimate.Number,
                workType = template.WorkType,
                range = new { lower = calc.DisplayedLower, upper = calc.DisplayedUpper, currency = "THB" },
                taxDisplay = template.TaxDisplay,
                validUntil = calc.ValidUntil,
                locale = input.Locale,
                assumptions = config.Assumptions,
                exclusions = config.Exclusions,
                preliminary = true
            }, Json);

            var now = _clock.UtcNow;
            _db.QuickEstimateShares.Add(new QuickEstimateShare(Guid.NewGuid(), orgId, estimate.Id, input.SourceVersion, input.Channel, input.Recipient, input.Locale, summary, access.ActorUserId, now));
            Audit(access, "quick-estimate.shared", "QuickEstimate", estimate.Id, traceId, new { number = estimate.Number, version = input.SourceVersion, channel = input.Channel }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, estimate.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<QuickEstimateProjection>.Success((await GetQuickEstimateAsync(orgId, estimate.Id, ct))!);
        });
    }

    /// <summary>
    /// Creates the official estimate draft through its own use case (which runs its own transaction), keyed by this request so a retry
    /// returns the same draft, and only then records the link in a second transaction. A failure between the two is safe to retry.
    /// </summary>
    public async Task<Result<QuickEstimateProjection>> ConvertAsync(
        RequestAccessContext access, Guid id, ConversionInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quick-estimate.convert";
        var orgId = access.OrganizationId;

        var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Fail<QuickEstimateProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
            var replayed = await GetQuickEstimateAsync(orgId, id, ct);
            if (replayed is not null) return Result<QuickEstimateProjection>.Success(replayed);
        }

        var estimate = await _db.QuickEstimates.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == orgId, ct);
        if (estimate is null) return Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Quick estimate not found.");
        if (estimate.Status == QuickEstimateStatus.Converted || await _db.QuickEstimateConversions.AnyAsync(c => c.QuickEstimateId == id && c.SourceVersion == input.SourceVersion, ct))
        {
            return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_ALREADY_CONVERTED", "This calculation has already been converted.");
        }

        if (!estimate.OpportunityId.HasValue) return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_OPPORTUNITY_REQUIRED", "Link the quick estimate to an opportunity before converting.");
        var releasable = await CheckReleasableAsync(estimate, input.SourceVersion, ct);
        if (releasable.IsFailure) return Result<QuickEstimateProjection>.Failure(releasable.Error);

        var created = await _estimates.CreateDraftAsync(orgId, estimate.OpportunityId.Value, input.SiteSurveyRevisionId, "THB", access.ActorUserId, keyHash, Application.Common.Security.Sha256Hex.Compute($"qe-convert:{payloadHash}"), ct);
        if (created.IsFailure) return Result<QuickEstimateProjection>.Failure(created.Error);

        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            _db.ChangeTracker.Clear();
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            await _db.Database.SqlQuery<Guid>($"SELECT id AS \"Value\" FROM quick_estimate.quick_estimates WHERE id = {id} AND organization_id = {orgId} FOR UPDATE").ToListAsync(ct);
            var locked = await _db.QuickEstimates.FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == orgId, ct);
            if (locked is null) return Fail<QuickEstimateProjection>("RESOURCE_NOT_FOUND", "Quick estimate not found.");
            if (await _db.QuickEstimateConversions.AnyAsync(c => c.QuickEstimateId == id && c.SourceVersion == input.SourceVersion, ct))
            {
                return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_ALREADY_CONVERTED", "This calculation has already been converted.");
            }

            var now = _clock.UtcNow;
            var calc = (await CurrentCalculationAsync(locked, input.SourceVersion, ct))!;
            // Scope, measurements and assumptions are kept as the source of the new estimate. The range itself is not copied as a price.
            var source = JsonSerializer.Serialize(new { quickEstimate = locked.Number, calculationVersion = calc.Version, calc.TemplateCode, calc.TemplateVersion, locked.PropertyType, locked.RoomOrArea, locked.GradeCode, locked.ComplexityCodes, locked.AddOnCodes, measurements = locked.Measurements }, Json);
            _db.QuickEstimateConversions.Add(new QuickEstimateConversion(Guid.NewGuid(), orgId, locked.Id, input.SourceVersion, created.Value!.Id, source, access.ActorUserId, now));
            locked.MarkConverted(now);
            Audit(access, "quick-estimate.converted", "QuickEstimate", locked.Id, traceId, new { number = locked.Number, version = input.SourceVersion, officialEstimateId = created.Value.Id }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, locked.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<QuickEstimateProjection>.Success((await GetQuickEstimateAsync(orgId, locked.Id, ct))!);
        });
    }

    public async Task<QuickEstimateProjection?> GetQuickEstimateAsync(Guid organizationId, Guid id, CancellationToken ct = default)
    {
        var e = await _db.QuickEstimates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, ct);
        if (e is null) return null;

        var template = e.TemplateId.HasValue ? await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == e.TemplateId, ct) : null;
        var calc = e.CurrentCalculationVersion > 0 ? await _db.QuickEstimateCalculations.AsNoTracking().FirstOrDefaultAsync(c => c.QuickEstimateId == e.Id && c.Version == e.CurrentCalculationVersion, ct) : null;
        var reviews = await _db.QuickEstimateReviews.AsNoTracking().Where(r => r.QuickEstimateId == e.Id).OrderBy(r => r.SourceVersion).ToListAsync(ct);
        var shares = await _db.QuickEstimateShares.AsNoTracking().Where(s => s.QuickEstimateId == e.Id).OrderBy(s => s.CreatedAtUtc).ToListAsync(ct);
        var conversions = await _db.QuickEstimateConversions.AsNoTracking().Where(c => c.QuickEstimateId == e.Id).OrderBy(c => c.CreatedAtUtc).ToListAsync(ct);
        var people = await PeopleAsync(
            reviews.SelectMany(r => new Guid?[] { r.RequestedByUserId, r.DecidedByUserId }).Concat(shares.Select(s => (Guid?)s.CreatedByUserId)).Append(e.CreatedByUserId).Append(calc?.CreatedByUserId), ct);
        QePerson? Person(Guid? pid) => pid.HasValue && people.TryGetValue(pid.Value, out var p) ? p : null;

        CalculationProjection? calcProjection = null;
        if (calc is not null)
        {
            var snapshot = JsonDocument.Parse(calc.SnapshotJson).RootElement;
            var lines = snapshot.GetProperty("lines").EnumerateArray().Select(l => new CalculationLineProjection(
                l.GetProperty("lineId").GetGuid(), l.TryGetProperty("workSubtype", out var ws) && ws.ValueKind == JsonValueKind.String ? ws.GetString() : null,
                l.GetProperty("billableQuantity").GetDecimal(), l.GetProperty("baseAmount").GetDecimal(), l.GetProperty("adjustedAmount").GetDecimal())).ToList();
            calcProjection = new CalculationProjection(
                calc.Version, calc.TemplateCode, calc.TemplateVersion, calc.NetAmount, calc.TaxAmount, calc.DisplayedLower, calc.DisplayedUpper, calc.ValidUntil, calc.ShareDecision,
                JsonSerializer.Deserialize<List<string>>(calc.ReasonCodesJson) ?? new List<string>(), calc.InputHash, lines, Person(calc.CreatedByUserId)!, calc.CreatedAtUtc);
        }

        return new QuickEstimateProjection(
            e.Id, e.BranchId, e.Number, e.Status, e.CustomerId, e.OpportunityId,
            template is null ? null : new QuickEstimateTemplateRef(template.Id, template.Code, template.Version, template.Name, template.Status),
            e.PropertyType, e.RoomOrArea, e.GradeCode, e.ComplexityCodes, e.AddOnCodes, e.MeasurementConfidence, e.CustomMaterial,
            e.Measurements.Select(m => new MeasurementInput(m.LineId, m.WorkSubtype, m.WidthM, m.HeightM, m.DepthM, m.Quantity)).ToList(),
            e.CurrentCalculationVersion, calcProjection,
            reviews.Select(r => new ReviewProjection(r.Id, r.SourceVersion, r.Status, r.RequestNote, Person(r.RequestedByUserId)!, r.RequestedAtUtc, Person(r.DecidedByUserId), r.DecidedAtUtc, r.ReasonCode, r.DecisionNote)).ToList(),
            shares.Select(s => new ShareProjection(s.Id, s.SourceVersion, s.Channel, s.Recipient, s.Locale, Person(s.CreatedByUserId)!, s.CreatedAtUtc, s.SummaryJson)).ToList(),
            conversions.Select(c => new ConversionProjection(c.Id, c.SourceVersion, c.OfficialEstimateId, c.CreatedAtUtc)).ToList(),
            Person(e.CreatedByUserId)!, e.CreatedAtUtc, e.UpdatedAtUtc, e.RowVersion);
    }

    public async Task<string?> GetCalculationSnapshotAsync(Guid organizationId, Guid id, int version, CancellationToken ct = default) =>
        await _db.QuickEstimateCalculations.AsNoTracking().Where(c => c.OrganizationId == organizationId && c.QuickEstimateId == id && c.Version == version).Select(c => c.SnapshotJson).FirstOrDefaultAsync(ct);

    public async Task<PagedQuickEstimates> ListQuickEstimatesAsync(Guid organizationId, QuickEstimateListQuery query, CancellationToken ct = default)
    {
        var estimates = _db.QuickEstimates.AsNoTracking().Where(e => e.OrganizationId == organizationId);
        if (query.Status is not null) estimates = estimates.Where(e => e.Status == query.Status);
        if (query.Search is not null)
        {
            var needle = query.Search.ToUpperInvariant();
            estimates = estimates.Where(e => e.Number.ToUpper().Contains(needle) || e.RoomOrArea.ToUpper().Contains(needle));
        }

        var total = await estimates.CountAsync(ct);
        var rows = await estimates.OrderByDescending(e => e.UpdatedAtUtc).ThenBy(e => e.Id).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        var ids = rows.Select(r => r.Id).ToList();
        var calcs = await _db.QuickEstimateCalculations.AsNoTracking().Where(c => ids.Contains(c.QuickEstimateId)).ToListAsync(ct);
        return new PagedQuickEstimates(rows.Select(e =>
        {
            var calc = calcs.FirstOrDefault(c => c.QuickEstimateId == e.Id && c.Version == e.CurrentCalculationVersion);
            return new QuickEstimateListItemProjection(e.Id, e.Number, e.Status, e.RoomOrArea, calc?.TemplateCode, calc?.DisplayedLower, calc?.DisplayedUpper, calc?.ShareDecision, e.UpdatedAtUtc);
        }).ToList(), total, query.Page, query.PageSize);
    }
}
