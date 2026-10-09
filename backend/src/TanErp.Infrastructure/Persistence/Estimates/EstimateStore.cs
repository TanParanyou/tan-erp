using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates;
using TanErp.Application.Estimates.GetQuotationDocument;
using TanErp.Application.Items;
using TanErp.Application.Notifications;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Estimates;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Estimates;

public class EstimateStore : IEstimateStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly ICostResolver _costResolver;
    private readonly bool _useTestOnlyApprovalPolicy;
    private readonly ILogger<EstimateStore> _logger;
    private readonly INotificationPublisher _notifications;

    public EstimateStore(
        AppDbContext db,
        IClock clock,
        IDocumentNumberGenerator documentNumberGenerator,
        ICostResolver costResolver,
        IConfiguration configuration,
        ILogger<EstimateStore> logger,
        INotificationPublisher notifications)
    {
        _logger = logger;
        _notifications = notifications;
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
        _costResolver = costResolver;
        _useTestOnlyApprovalPolicy = string.Equals(configuration["ASPNETCORE_ENVIRONMENT"], "Test", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(configuration["Estimates:ApprovalPolicy"], TestOnlyEstimateApprovalPolicy.PolicyCode, StringComparison.Ordinal);
    }

    public async Task<Result<EstimateDetailProjection>> CreateDraftAsync(
        Guid organizationId,
        Guid opportunityId,
        Guid siteSurveyRevisionId,
        string currency,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken)
    {
        const string operation = "estimates.create";
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Replay check first
            var existingRecord = await _db.IdempotencyRecords
                .AsNoTracking()
                .SingleOrDefaultAsync(row =>
                    row.OrganizationId == organizationId &&
                    row.Operation == operation &&
                    row.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord is not null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<EstimateDetailProjection>.Failure(new Error(
                        "IDEMPOTENCY_KEY_REUSED",
                        "The idempotency key has already been used with a different payload."));
                }

                if (Guid.TryParse(existingRecord.ResourceId, out var replayEstimateId))
                {
                    var replayEstimate = await _db.Estimates
                        .Include(e => e.Revisions)
                            .ThenInclude(r => r.Sections)
                                .ThenInclude(s => s.WorkItems)
                                    .ThenInclude(w => w.CostComponents)
                        .FirstOrDefaultAsync(e => e.Id == replayEstimateId && e.OrganizationId == organizationId, cancellationToken);

                    if (replayEstimate is not null)
                    {
                        return Result<EstimateDetailProjection>.Success(MapToDetailProjection(replayEstimate));
                    }
                }
            }

            // 2. Load scoped Opportunity
            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.Id == opportunityId && o.OrganizationId == organizationId, cancellationToken);

            if (opp is null)
            {
                return Result<EstimateDetailProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Opportunity not found."));
            }

            if (opp.Stage != OpportunityStage.Estimating)
            {
                return Result<EstimateDetailProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Opportunity must be in 'estimating' stage to create an estimate draft."));
            }

            // 3. Load matching Ready/Superseded revision belonging to same Opportunity and organization
            var revision = await _db.SiteSurveyRevisions
                .FirstOrDefaultAsync(r => r.Id == siteSurveyRevisionId && r.OrganizationId == organizationId, cancellationToken);

            if (revision is null)
            {
                return Result<EstimateDetailProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey revision not found."));
            }

            var survey = await _db.SiteSurveys
                .FirstOrDefaultAsync(s => s.Id == revision.SiteSurveyId && s.OpportunityId == opp.Id && s.OrganizationId == organizationId, cancellationToken);

            if (survey is null)
            {
                return Result<EstimateDetailProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Site survey revision does not belong to this opportunity."));
            }

            if (revision.Status != SurveyRevisionStatus.Ready && revision.Status != SurveyRevisionStatus.Superseded)
            {
                return Result<EstimateDetailProjection>.Failure(
                    new Error("SURVEY_NOT_READY", "Site survey revision must be ready or superseded."));
            }

            // Check if estimate already exists for this opportunity
            var existing = await _db.Estimates
                .Include(e => e.Revisions)
                    .ThenInclude(r => r.Sections)
                        .ThenInclude(s => s.WorkItems)
                            .ThenInclude(w => w.CostComponents)
                .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.OpportunityId == opportunityId, cancellationToken);

            if (existing is not null)
            {
                return Result<EstimateDetailProjection>.Success(MapToDetailProjection(existing));
            }

            // 4. Derive customerId, branchId, and snapshotHash from server state
            var customerId = opp.CustomerId;
            var branchId = opp.BranchId;
            var snapshotHash = revision.SnapshotHash;

            var number = await _documentNumberGenerator.GenerateAsync(
                organizationId,
                TanErp.Domain.DocumentNumbering.DocumentTypes.Estimates,
                branchId,
                _clock.UtcNow,
                cancellationToken);

            var estimate = Estimate.CreateDraft(
                Guid.NewGuid(),
                organizationId,
                branchId,
                customerId,
                opportunityId,
                number,
                siteSurveyRevisionId,
                snapshotHash,
                currency);
            estimate.CurrentRevision!.MarkFinancialInputChanged(actorUserId);

            _db.Estimates.Add(estimate);

            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "estimates.created",
                "Estimate",
                estimate.Id.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new { estimate.Number, estimate.CustomerId, estimate.OpportunityId, estimate.SiteSurveyRevisionId }));

            _db.AddAuditEvent(auditEvent);

            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                organizationId,
                operation,
                keyHash,
                payloadHash,
                estimate.Id.ToString(),
                _clock.UtcNow);

            _db.IdempotencyRecords.Add(idempotencyRecord);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();
                var replay = await _db.IdempotencyRecords
                    .AsNoTracking()
                    .SingleOrDefaultAsync(row =>
                        row.OrganizationId == organizationId &&
                        row.Operation == operation &&
                        row.KeyHash == keyHash,
                        cancellationToken);

                if (replay is not null && replay.PayloadHash == payloadHash && Guid.TryParse(replay.ResourceId, out var reloadedId))
                {
                    var loaded = await _db.Estimates
                        .Include(e => e.Revisions)
                            .ThenInclude(r => r.Sections)
                                .ThenInclude(s => s.WorkItems)
                                    .ThenInclude(w => w.CostComponents)
                        .FirstOrDefaultAsync(e => e.Id == reloadedId && e.OrganizationId == organizationId, cancellationToken);
                    if (loaded is not null)
                    {
                        return Result<EstimateDetailProjection>.Success(MapToDetailProjection(loaded));
                    }
                }
                throw;
            }

            return Result<EstimateDetailProjection>.Success(MapToDetailProjection(estimate));
        });
    }

    public async Task<EstimateDetailProjection?> GetByIdAsync(
        Guid organizationId,
        Guid estimateId,
        CancellationToken cancellationToken)
    {
        var estimate = await _db.Estimates
            .AsNoTracking()
            .Include(e => e.Revisions)
                .ThenInclude(r => r.Sections.OrderBy(s => s.SortOrder))
                    .ThenInclude(s => s.WorkItems.OrderBy(w => w.SortOrder))
                        .ThenInclude(w => w.CostComponents.OrderBy(c => c.SortOrder))
            .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.Id == estimateId, cancellationToken);

        return estimate is null ? null : MapToDetailProjection(estimate);
    }

    public async Task<EstimateDetailProjection?> GetByOpportunityIdAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken)
    {
        var estimate = await _db.Estimates
            .AsNoTracking()
            .Include(e => e.Revisions)
                .ThenInclude(r => r.Sections.OrderBy(s => s.SortOrder))
                    .ThenInclude(s => s.WorkItems.OrderBy(w => w.SortOrder))
                        .ThenInclude(w => w.CostComponents.OrderBy(c => c.SortOrder))
            .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.OpportunityId == opportunityId, cancellationToken);

        return estimate is null ? null : MapToDetailProjection(estimate);
    }

    public async Task<IReadOnlyList<EstimateCalculationSnapshotProjection>> GetCalculationSnapshotsAsync(
        Guid organizationId,
        Guid estimateId,
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        return await _db.EstimateCalculationSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.OrganizationId == organizationId &&
                snapshot.EstimateRevisionId == revisionId &&
                _db.EstimateRevisions.Any(revision => revision.Id == revisionId &&
                    revision.EstimateId == estimateId && revision.OrganizationId == organizationId))
            .OrderBy(snapshot => snapshot.CalculationVersion)
            .Select(snapshot => new EstimateCalculationSnapshotProjection(
                snapshot.Id,
                snapshot.EstimateRevisionId,
                snapshot.CalculationVersion,
                snapshot.InputHash,
                snapshot.SnapshotJson,
                snapshot.CalculationPolicyVersionId,
                snapshot.TaxPolicyVersionId,
                snapshot.CalculationPolicyVersion,
                snapshot.TaxPolicyVersion,
                snapshot.CalculationPolicyHash,
                snapshot.TaxPolicyHash,
                snapshot.CapturedByUserId,
                snapshot.CapturedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<EstimateRevisionProjection> UpdateDraftAsync(
        Guid organizationId,
        Guid estimateId,
        Guid revisionId,
        Guid expectedRevisionVersion,
        IReadOnlyList<EstimateSectionDraftDto> sectionsDto,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var estimate = await _db.Estimates
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.Id == estimateId, cancellationToken);
            if (estimate is null)
                throw new EstimateNotFoundException(estimateId);

            var revision = await _db.EstimateRevisions
                .Include(r => r.Sections)
                    .ThenInclude(s => s.WorkItems)
                        .ThenInclude(w => w.CostComponents)
                .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.EstimateId == estimateId && r.Id == revisionId, cancellationToken);

            if (revision is null)
                throw new EstimateNotFoundException(estimateId);

            if (revision.RowVersion != expectedRevisionVersion)
                throw new DbUpdateConcurrencyException("Revision version conflict.");

            if (revision.Status != EstimateRevisionStatus.Draft && revision.Status != EstimateRevisionStatus.Returned)
                throw new EstimateInvalidStateException($"Cannot update estimate in status '{revision.Status}'.");

            // Remove existing relational structure
            if (revision.Sections.Count > 0)
            {
                _db.EstimateSections.RemoveRange(revision.Sections);
                await _db.SaveChangesAsync(cancellationToken);
                revision.ClearSections();
            }

            // Recreate sections, work items, and cost components
            foreach (var sDto in sectionsDto.OrderBy(s => s.SortOrder))
            {
                var section = new EstimateSection(
                    sDto.Id ?? Guid.NewGuid(),
                    organizationId,
                    revision.Id,
                    sDto.Code,
                    sDto.NameTh,
                    sDto.NameEn,
                    sDto.SortOrder);

                foreach (var wDto in sDto.WorkItems.OrderBy(w => w.SortOrder))
                {
                    var workItem = new EstimateWorkItem(
                        wDto.Id ?? Guid.NewGuid(),
                        organizationId,
                        section.Id,
                        wDto.Code,
                        wDto.DescriptionTh,
                        wDto.DescriptionEn,
                        wDto.Quantity,
                        wDto.UnitCode,
                        wDto.SellingRuleType,
                        wDto.SellingRuleValue,
                        wDto.SortOrder,
                        wDto.SellingRuleReasonCode);

                    if (wDto.ItemId.HasValue)
                    {
                        var catalogItem = await _db.Items.AsNoTracking().Include(item => item.BranchAvailabilities)
                            .FirstOrDefaultAsync(item => item.Id == wDto.ItemId.Value && item.OrganizationId == organizationId, cancellationToken);
                        if (catalogItem is null || catalogItem.Status != ItemStatus.Active || !catalogItem.Capabilities.CanCost)
                            throw new ItemCostConflictException("ITEM_NOT_AVAILABLE", "The selected work item is not available for estimates.");
                        if (catalogItem.AvailabilityMode == ItemAvailabilityMode.SelectedBranches &&
                            !catalogItem.BranchAvailabilities.Any(availability => availability.BranchId == estimate.BranchId && availability.Status == "active"))
                            throw new ItemCostConflictException("ITEM_NOT_AVAILABLE", "The selected work item is not available in this branch.");
                        workItem.SetItemMasterLink(catalogItem.Id, catalogItem.Code, catalogItem.Name.Thai, catalogItem.Name.English);
                    }
                    else
                    {
                        workItem.SetCustomWorkItemReason(wDto.OverrideReasonCode, wDto.OverrideReason);
                    }

                    foreach (var cDto in wDto.CostComponents.OrderBy(c => c.SortOrder))
                    {
                        var comp = new EstimateCostComponent(
                            cDto.Id ?? Guid.NewGuid(),
                            organizationId,
                            workItem.Id,
                            cDto.Type,
                            cDto.Description,
                            cDto.Quantity,
                            cDto.UnitCode,
                            cDto.UnitCost,
                            cDto.Currency ?? EstimateDefaults.DefaultCurrency,
                            cDto.SortOrder);

                        if (cDto.ItemId.HasValue)
                        {
                            var item = await _db.Items
                                .AsNoTracking()
                                .Include(i => i.BaseUnit)
                                .Include(i => i.BranchAvailabilities)
                                .FirstOrDefaultAsync(i => i.Id == cDto.ItemId.Value && i.OrganizationId == organizationId, cancellationToken);

                            if (item is null)
                            {
                                throw new ItemCostConflictException("ITEM_NOT_FOUND", $"Item '{cDto.ItemId.Value}' was not found.");
                            }

                            if (item.Status != ItemStatus.Active || !item.Capabilities.CanCost)
                            {
                                throw new ItemCostConflictException("ITEM_NOT_AVAILABLE", $"Item '{item.Code}' is not active or cannot be costed.");
                            }

                            if (item.AvailabilityMode == ItemAvailabilityMode.SelectedBranches &&
                                !item.BranchAvailabilities.Any(ba => ba.BranchId == estimate.BranchId && ba.Status == "active"))
                            {
                                throw new ItemCostConflictException("ITEM_NOT_AVAILABLE", $"Item '{item.Code}' is not available in branch '{estimate.BranchId}'.");
                            }

                            if (item.BaseUnit is null ||
                                !string.Equals(cDto.UnitCode.Trim(), item.BaseUnit.NormalizedCode, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new ItemCostConflictException("ESTIMATE_UNIT_INVALID",
                                    $"Cost component unit '{cDto.UnitCode}' must match the catalog item's base unit '{item.BaseUnit?.Code ?? string.Empty}'.");
                            }

                            var resolveResult = await _costResolver.ResolveAsync(new ResolveCostRequest(
                                organizationId,
                                estimate.BranchId,
                                item.Id,
                                item.BaseUnitId,
                                cDto.Currency ?? EstimateDefaults.DefaultCurrency,
                                cDto.Quantity,
                                _clock.UtcNow), cancellationToken);

                            if (resolveResult.IsFailure)
                            {
                                throw new ItemCostConflictException(resolveResult.Error.Code, resolveResult.Error.Message);
                            }

                            var resolved = resolveResult.Value!;

                            // Revalidation: If client supplied cost record ID, version, or unit cost, it must match current resolved cost
                            if (cDto.CostRecordId.HasValue && cDto.CostRecordId.Value != resolved.CostRecordId)
                            {
                                throw new ItemCostConflictException("ITEM_COST_VERSION_CONFLICT",
                                    $"Cost record ID mismatch for item '{item.Code}'. Expected {cDto.CostRecordId.Value}, current is {resolved.CostRecordId}.");
                            }

                            if (cDto.CostRecordVersion.HasValue && cDto.CostRecordVersion.Value != resolved.Version)
                            {
                                throw new ItemCostConflictException("ITEM_COST_VERSION_CONFLICT",
                                    $"Cost record version mismatch for item '{item.Code}'. Expected v{cDto.CostRecordVersion.Value}, current is v{resolved.Version}.");
                            }

                            if (cDto.UnitCost != resolved.Amount)
                            {
                                throw new ItemCostConflictException("ITEM_COST_VERSION_CONFLICT",
                                    $"Unit cost mismatch for item '{item.Code}'. Expected {cDto.UnitCost}, current resolved cost is {resolved.Amount}.");
                            }

                            var unitCode = item.BaseUnit?.Code ?? cDto.UnitCode;

                            comp.SetCatalogCostSnapshot(
                                item.Id,
                                resolved.CostRecordId,
                                resolved.Version,
                                item.Code,
                                item.Name,
                                unitCode,
                                resolved.Amount,
                                resolved.Currency,
                                resolved.Scope,
                                resolved.EffectiveFromUtc,
                                "v1",
                                resolved.ResolvedAtUtc,
                                resolved.CostSourceId,
                                resolved.CostSourceCode,
                                resolved.SourceReference,
                                resolved.EvidenceFileId,
                                resolved.Reason);
                        }

                        comp.SetProvisionalReason(cDto.ProvisionalReasonCode, cDto.ProvisionalNote);

                        workItem.AddCostComponent(comp);
                        _db.EstimateCostComponents.Add(comp);
                    }

                    section.AddWorkItem(workItem);
                    _db.EstimateWorkItems.Add(workItem);
                }

                revision.AddSection(section);
                _db.EstimateSections.Add(section);
            }

            // The saved inputs no longer match the last calculated snapshot.
            revision.MarkFinancialInputChanged(actorUserId);

            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "estimates.draft_updated",
                "Estimate",
                estimateId.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new { revisionId, revision.NetCost, revision.GrandTotal, sectionCount = sectionsDto.Count }));

            _db.AddAuditEvent(auditEvent);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return MapToRevisionProjection(revision);
        });
    }

    public async Task<EstimateRevisionProjection> CalculateAsync(
        Guid organizationId,
        Guid estimateId,
        Guid revisionId,
        Guid expectedRevisionVersion,
        EstimateDiscount discount,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            const string operation = "estimates.calculate";
            var idempotencyLockKey = $"{organizationId:N}:{operation}:{keyHash}";
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({idempotencyLockKey}, 0))", cancellationToken);
            var existingRecord = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(record =>
                record.OrganizationId == organizationId && record.Operation == operation && record.KeyHash == keyHash,
                cancellationToken);
            if (existingRecord is not null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                    throw new EstimateIdempotencyKeyReusedException();

                var resourceParts = existingRecord.ResourceId.Split('|');
                if (resourceParts.Length != 3 ||
                    !Guid.TryParseExact(resourceParts[0], "N", out var replayEstimateId) || replayEstimateId != estimateId ||
                    !Guid.TryParseExact(resourceParts[1], "N", out var replayRevisionId) || replayRevisionId != revisionId ||
                    !int.TryParse(resourceParts[2], out var replayCalculationVersion))
                    throw new EstimateIdempotencyKeyReusedException();

                var replayRevision = await _db.EstimateRevisions
                    .Include(row => row.Sections)
                        .ThenInclude(section => section.WorkItems)
                            .ThenInclude(workItem => workItem.CostComponents)
                    .FirstOrDefaultAsync(row => row.OrganizationId == organizationId && row.EstimateId == estimateId &&
                        row.Id == revisionId && row.CalculationVersion == replayCalculationVersion, cancellationToken);
                var hasReplaySnapshot = await _db.EstimateCalculationSnapshots.AsNoTracking().AnyAsync(snapshot =>
                    snapshot.OrganizationId == organizationId && snapshot.EstimateRevisionId == revisionId &&
                    snapshot.CalculationVersion == replayCalculationVersion, cancellationToken);
                if (replayRevision is null || !hasReplaySnapshot)
                    throw new EstimateNotFoundException(estimateId);

                await tx.CommitAsync(cancellationToken);
                return MapToRevisionProjection(replayRevision);
            }

            var estimate = await _db.Estimates
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.Id == estimateId, cancellationToken);
            if (estimate is null)
                throw new EstimateNotFoundException(estimateId);

            var revision = await _db.EstimateRevisions
                .Include(r => r.Sections)
                    .ThenInclude(s => s.WorkItems)
                        .ThenInclude(w => w.CostComponents)
                .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.EstimateId == estimateId && r.Id == revisionId, cancellationToken);

            if (revision is null)
                throw new EstimateNotFoundException(estimateId);

            if (revision.RowVersion != expectedRevisionVersion)
                throw new DbUpdateConcurrencyException("Revision version conflict.");

            var calculationPolicy = await ResolvePolicyAsync(
                _db.CalculationPolicyVersions,
                organizationId,
                estimate.BranchId,
                _clock.UtcNow,
                cancellationToken);
            var taxPolicy = await ResolvePolicyAsync(
                _db.TaxPolicyVersions,
                organizationId,
                estimate.BranchId,
                _clock.UtcNow,
                cancellationToken);
            if (calculationPolicy is null || taxPolicy is null)
                throw new EstimatePolicyUnavailableException("A single effective published calculation and tax policy are required.");

            revision.Calculate(discount, calculationPolicy, taxPolicy, _clock.UtcNow);
            var calculationSnapshot = new EstimateCalculationSnapshot(
                Guid.NewGuid(),
                organizationId,
                revision.Id,
                revision.CalculationVersion,
                CalculateInputHash(revision, discount, calculationPolicy, taxPolicy),
                revision.CalculationSnapshotJson!,
                calculationPolicy,
                taxPolicy,
                actorUserId,
                _clock.UtcNow);
            _db.EstimateCalculationSnapshots.Add(calculationSnapshot);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(), organizationId, operation, keyHash, payloadHash,
                $"{estimateId:N}|{revisionId:N}|{revision.CalculationVersion}", _clock.UtcNow));

            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "estimates.calculated",
                "Estimate",
                estimateId.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new { revisionId, revision.CalculationVersion, revision.NetCost, revision.GrandTotal, revision.MarginRate }));

            _db.AddAuditEvent(auditEvent);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return MapToRevisionProjection(revision);
        });
    }

    public async Task<Result<EstimateDetailProjection>> SubmitAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, int revisionNo,
        int calculationVersion, string? note, Guid actorUserId, string keyHash,
        string payloadHash, CancellationToken cancellationToken)
    {
        const string operation = "estimates.submit";
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            var replay = await GetIdempotencyReplayAsync(organizationId, estimateId, operation, keyHash, payloadHash, cancellationToken);
            if (replay is not null)
                return replay;

            var estimate = await LoadEstimateDetails()
                .FirstOrDefaultAsync(row => row.Id == estimateId && row.OrganizationId == organizationId, cancellationToken);
            if (estimate is null)
                return Result<EstimateDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Estimate not found."));
            if (estimate.RowVersion != expectedEstimateVersion)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate version does not match the current state."));

            var revision = estimate.CurrentRevision;
            if (revision is null || revision.RevisionNo != revisionNo || revision.CalculationVersion != calculationVersion ||
                revision.CalculationOutdated || string.IsNullOrWhiteSpace(revision.CalculationSnapshotJson))
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", "Submit requires the current revision and its latest calculation snapshot."));

            var readiness = revision.EvaluateReadiness();
            if (readiness.Status == EstimateReadinessStatus.Blocked)
            {
                var blockingReason = readiness.Reasons.First();
                return Result<EstimateDetailProjection>.Failure(new Error(blockingReason.Code, "The estimate is blocked by one or more readiness requirements."));
            }

            var calculationSnapshot = await _db.EstimateCalculationSnapshots
                .FirstOrDefaultAsync(snapshot => snapshot.OrganizationId == organizationId &&
                    snapshot.EstimateRevisionId == revision.Id && snapshot.CalculationVersion == calculationVersion,
                    cancellationToken);
            if (calculationSnapshot is null)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", "The latest calculation snapshot is unavailable."));

            var now = _clock.UtcNow;
            var approvalPolicy = _useTestOnlyApprovalPolicy
                ? ApprovalPolicyVersion.TestOnlyThailandEstimateV1
                : ApprovalPolicyVersion.BootstrapIndependentChecker;
            var testOnlyRoute = _useTestOnlyApprovalPolicy
                ? TestOnlyEstimateApprovalPolicy.Resolve(
                    revision.GrandTotal,
                    revision.MarginRate,
                    revision.SellingBeforeDiscount,
                    revision.DiscountAmount,
                    readiness.Reasons.Select(reason => reason.Code))
                : null;
            var requiredReviewerRoles = testOnlyRoute?.ReviewerRoleNames ?? ["INDEPENDENT_CHECKER"];
            var reviewers = new List<ApprovalCandidate>(requiredReviewerRoles.Count);
            foreach (var reviewerRole in requiredReviewerRoles)
            {
                var reviewer = await FindIndependentReviewerAsync(
                    organizationId,
                    estimate.BranchId,
                    actorUserId,
                    revision.LastFinancialEditorUserId,
                    now,
                    cancellationToken,
                    _useTestOnlyApprovalPolicy ? reviewerRole : null,
                    reviewers.Select(candidate => candidate.UserId).ToArray());
                if (reviewer is null)
                    return Result<EstimateDetailProjection>.Failure(new Error(
                        "ESTIMATE_POLICY_UNAVAILABLE",
                        $"No independent reviewer is available for required approval role '{reviewerRole}'."));
                reviewers.Add(reviewer);
            }

            var snapshotHash = HashSnapshot(calculationSnapshot.SnapshotJson);
            var route = JsonSerializer.Serialize(new
            {
                policyCode = approvalPolicy.PolicyCode,
                policyVersion = approvalPolicy.Version,
                approvalPolicyHash = testOnlyRoute?.PolicyHash ?? approvalPolicy.ContentHash,
                thresholdSnapshot = testOnlyRoute?.Thresholds,
                triggerSnapshot = testOnlyRoute?.Triggers ?? [],
                estimateId = estimate.Id,
                revisionId = revision.Id,
                revisionNo,
                calculationVersion,
                calculationInputHash = calculationSnapshot.InputHash,
                calculationSnapshotHash = snapshotHash,
                submissionNote = note,
                candidateRule = new { permissionKey = "estimates.approve", scope = new[] { "organization", "branch" } },
                steps = reviewers.Select((reviewer, index) => new
                {
                    sequence = index + 1,
                    requiredRole = requiredReviewerRoles[index],
                    reviewer.ScopeType,
                    reviewer.ScopeId,
                    reviewer.UserId,
                    reviewer.MembershipId,
                    permissionKey = "estimates.approve"
                })
            }, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var routeHash = HashSnapshot(route);
            var request = new EstimateApprovalRequest(
                Guid.NewGuid(), organizationId, estimate.Id, revision.Id, revisionNo,
                calculationVersion, calculationSnapshot.InputHash, snapshotHash,
                approvalPolicy.PolicyCode, approvalPolicy.Version, route, routeHash,
                actorUserId, now, note);
            var steps = reviewers.Select((reviewer, index) => new EstimateApprovalStep(
                Guid.NewGuid(), organizationId, request.Id, index + 1, reviewer.UserId,
                reviewer.MembershipId, reviewer.ScopeType, reviewer.ScopeId)).ToArray();

            try
            {
                estimate.SubmitCurrentRevision(actorUserId, now);
            }
            catch (EstimateInvalidStateException ex)
            {
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", ex.Message));
            }

            _db.EstimateApprovalRequests.Add(request);
            _db.EstimateApprovalSteps.AddRange(steps);
            AddEstimateAudit(organizationId, actorUserId, "estimates.submitted", estimate.Id,
                new { revisionId = revision.Id, revisionNo, calculationVersion, routeHash,
                    reviewerMembershipIds = reviewers.Select(reviewer => reviewer.MembershipId).ToArray(),
                    triggerCodes = testOnlyRoute?.Triggers.Select(trigger => trigger.Code).ToArray() ?? [] });
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation,
                keyHash, payloadHash, estimate.Id.ToString(), now));
            await _notifications.PublishAsync(
                NotificationEvents.EstimateSubmitted(
                    organizationId, estimate.BranchId, request.Id, actorUserId, estimate.Id, estimate.Number, steps[0].ReviewerUserId),
                cancellationToken);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
                return Result<EstimateDetailProjection>.Success(MapToDetailProjection(estimate));
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505")
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();
                var winner = await GetIdempotencyReplayAsync(organizationId, estimateId, operation, keyHash, payloadHash, cancellationToken);
                if (winner is not null)
                    return winner;
                throw;
            }
        });
    }

    public async Task<Result<EstimateDetailProjection>> CreateRevisionAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, string reason, Guid actorUserId,
        string keyHash, string payloadHash, CancellationToken cancellationToken)
    {
        const string operation = "estimates.revisions.create";
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            var replay = await GetIdempotencyReplayAsync(organizationId, estimateId, operation, keyHash, payloadHash, cancellationToken);
            if (replay is not null)
                return replay;

            var estimate = await LoadEstimateDetails()
                .FirstOrDefaultAsync(row => row.Id == estimateId && row.OrganizationId == organizationId, cancellationToken);
            if (estimate is null)
                return Result<EstimateDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Estimate not found."));
            if (estimate.RowVersion != expectedEstimateVersion)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate version does not match the current state."));

            EstimateRevision revision;
            try
            {
                revision = estimate.CreateNextDraftRevision(reason);
            }
            catch (EstimateInvalidStateException ex)
            {
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INPUT_INVALID", ex.Message));
            }

            var now = _clock.UtcNow;
            _db.EstimateRevisions.Add(revision);
            AddEstimateAudit(organizationId, actorUserId, "estimates.revision_created", estimate.Id,
                new { revisionId = revision.Id, revisionNo = revision.RevisionNo, sourceRevisionNo = revision.RevisionNo - 1, reason = reason.Trim() });
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation,
                keyHash, payloadHash, estimate.Id.ToString(), now));
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
                return Result<EstimateDetailProjection>.Success(MapToDetailProjection(estimate));
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505")
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();
                var winner = await GetIdempotencyReplayAsync(organizationId, estimateId, operation, keyHash, payloadHash, cancellationToken);
                if (winner is not null)
                    return winner;
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "A revision was created concurrently."));
            }
        });
    }

    public async Task<Result<EstimateDetailProjection>> ReviewAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, int revisionNo,
        string decision, string? reasonCode, string? note, Guid actorUserId,
        Guid reviewerMembershipId, string keyHash, string payloadHash,
        CancellationToken cancellationToken)
    {
        const string operation = "estimates.review";
        var strategy = _db.Database.CreateExecutionStrategy();
        try
        {
            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
                // Read the estimate and approval route only after the preceding decision commits.
                var reviewLockKey = $"{organizationId:N}:{operation}:{estimateId:N}";
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"SELECT pg_advisory_xact_lock(hashtextextended({reviewLockKey}, 0))", cancellationToken);
            var replay = await GetIdempotencyReplayAsync(organizationId, estimateId, operation, keyHash, payloadHash, cancellationToken);
            if (replay is not null)
                return replay;

            if (decision is not (EstimateApprovalStepStatus.Approved or EstimateApprovalStepStatus.Returned))
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_REVIEW_DECISION_INVALID", "Decision must be approved or returned."));
            if (decision == EstimateApprovalStepStatus.Returned && (string.IsNullOrWhiteSpace(reasonCode) || string.IsNullOrWhiteSpace(note)))
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_REVIEW_REASON_REQUIRED", "Returning an estimate requires a reason code and note."));

            var estimate = await LoadEstimateDetails()
                .FirstOrDefaultAsync(row => row.Id == estimateId && row.OrganizationId == organizationId, cancellationToken);
            if (estimate is null)
                return Result<EstimateDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Estimate not found."));
            if (estimate.RowVersion != expectedEstimateVersion)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate version does not match the current state."));

            var revision = estimate.CurrentRevision;
            if (revision is null || revision.RevisionNo != revisionNo || revision.Status != EstimateRevisionStatus.Submitted)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", "Only the submitted current revision can be reviewed."));

            var request = await _db.EstimateApprovalRequests
                .FirstOrDefaultAsync(row => row.OrganizationId == organizationId && row.EstimateId == estimateId &&
                    row.EstimateRevisionId == revision.Id && row.Status == EstimateApprovalRequestStatus.Open, cancellationToken);
            if (request is null)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", "An open approval request was not found."));
            var activeSequence = await _db.EstimateApprovalSteps
                .Where(row => row.OrganizationId == organizationId && row.EstimateApprovalRequestId == request.Id &&
                    row.Status == EstimateApprovalStepStatus.Pending)
                .Select(row => (int?)row.Sequence)
                .MinAsync(cancellationToken);
            var step = activeSequence.HasValue
                ? await _db.EstimateApprovalSteps.FirstOrDefaultAsync(row => row.OrganizationId == organizationId &&
                    row.EstimateApprovalRequestId == request.Id && row.Sequence == activeSequence.Value &&
                    row.ReviewerMembershipId == reviewerMembershipId && row.ReviewerUserId == actorUserId &&
                    row.Status == EstimateApprovalStepStatus.Pending, cancellationToken)
                : null;
            if (step is null)
                return Result<EstimateDetailProjection>.Failure(new Error("PERMISSION_DENIED", "This membership is not assigned to the active approval step."));

            var hasAuthority = await HasCurrentApprovalPermissionAsync(organizationId, estimate.BranchId,
                reviewerMembershipId, actorUserId, _clock.UtcNow, cancellationToken);
            if (!hasAuthority)
                return Result<EstimateDetailProjection>.Failure(new Error("PERMISSION_DENIED", "The assigned reviewer no longer has estimate approval authority."));
            if (actorUserId == revision.SubmittedByUserId || actorUserId == revision.LastFinancialEditorUserId)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_APPROVAL_SELF_REVIEW", "The submitter and last financial editor cannot approve or return this estimate."));

            var now = _clock.UtcNow;
            var approvalDecision = new EstimateApprovalDecision(
                Guid.NewGuid(), organizationId, request.Id, step.Id, actorUserId,
                reviewerMembershipId, decision, reasonCode, note,
                request.CalculationSnapshotHash, request.RouteHash, now);
            step.Decide(decision);
            _db.EstimateApprovalDecisions.Add(approvalDecision);

            var auditAction = "estimates.returned";
            if (decision == EstimateApprovalStepStatus.Approved)
            {
                var hasPendingStep = await _db.EstimateApprovalSteps.AnyAsync(row =>
                    row.OrganizationId == organizationId && row.EstimateApprovalRequestId == request.Id &&
                    row.Id != step.Id && row.Status == EstimateApprovalStepStatus.Pending, cancellationToken);
                if (hasPendingStep)
                {
                    auditAction = "estimates.approval_step_approved";
                }
                else
                {
                    request.Close(EstimateApprovalRequestStatus.Approved, now);
                    var priorDecisions = await _db.EstimateApprovalDecisions.AsNoTracking()
                        .Where(row => row.OrganizationId == organizationId && row.EstimateApprovalRequestId == request.Id)
                        .OrderBy(row => row.DecidedAtUtc)
                        .ToListAsync(cancellationToken);
                    priorDecisions.Add(approvalDecision);
                    var approvalSnapshotJson = JsonSerializer.Serialize(new
                    {
                        request.PolicyCode,
                        request.PolicyVersion,
                        request.CalculationInputHash,
                        request.CalculationSnapshotHash,
                        request.RouteHash,
                        request.RouteSnapshotJson,
                        decisions = priorDecisions
                    });
                    _db.EstimateApprovalSnapshots.Add(new EstimateApprovalSnapshot(
                        Guid.NewGuid(), organizationId, estimate.Id, revision.Id, request.Id,
                        request.CalculationVersion, request.CalculationInputHash,
                        request.CalculationSnapshotHash, request.RouteHash, approvalSnapshotJson,
                        actorUserId, now));
                    estimate.ApproveCurrentRevision(actorUserId, now, approvalSnapshotJson);
                    auditAction = "estimates.approved";
                }
            }
            else
            {
                request.Close(EstimateApprovalRequestStatus.Returned, now);
                estimate.ReturnCurrentRevision(actorUserId, now, reasonCode!, note!);
            }

            AddEstimateAudit(organizationId, actorUserId, auditAction, estimate.Id,
                new { revisionId = revision.Id, revisionNo, approvalRequestId = request.Id, decision, reasonCode });
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation,
                keyHash, payloadHash, estimate.Id.ToString(), now));
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
                return Result<EstimateDetailProjection>.Success(MapToDetailProjection(estimate));
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<EstimateDetailProjection>.Failure(new Error(
                "ESTIMATE_VERSION_CONFLICT", "The estimate changed while the review decision was being recorded."));
        }
        catch (DbUpdateException exception) when (exception.InnerException is Npgsql.PostgresException
            { SqlState: "23505" or "40001" or "40P01" })
        {
            return Result<EstimateDetailProjection>.Failure(new Error(
                "ESTIMATE_VERSION_CONFLICT", "The approval route changed while the review decision was being recorded."));
        }
    }

    public async Task<Result<EstimateDetailProjection>> CancelAsync(
        Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, string reason, Guid actorUserId,
        Guid actorMembershipId, string keyHash, string payloadHash, CancellationToken cancellationToken)
    {
        const string operation = "estimates.cancel";
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            var replay = await GetIdempotencyReplayAsync(organizationId, estimateId, operation, keyHash, payloadHash, cancellationToken);
            if (replay is not null)
                return replay;

            var estimate = await LoadEstimateDetails()
                .FirstOrDefaultAsync(row => row.Id == estimateId && row.OrganizationId == organizationId, cancellationToken);
            if (estimate is null)
                return Result<EstimateDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Estimate not found."));
            if (estimate.RowVersion != expectedEstimateVersion)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_VERSION_CONFLICT", "The estimate version does not match the current state."));
            if (string.IsNullOrWhiteSpace(reason))
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_CANCEL_REASON_REQUIRED", "A cancellation reason is required."));

            var revision = estimate.CurrentRevision;
            if (revision is null)
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", "Estimate has no current revision."));

            EstimateApprovalRequest? openRequest = null;
            if (revision.Status == EstimateRevisionStatus.Submitted)
            {
                openRequest = await _db.EstimateApprovalRequests.FirstOrDefaultAsync(request =>
                    request.OrganizationId == organizationId && request.EstimateId == estimateId &&
                    request.EstimateRevisionId == revision.Id && request.Status == EstimateApprovalRequestStatus.Open,
                    cancellationToken);
                if (openRequest is null)
                    return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", "An open approval request was not found."));

                var assignedStep = await _db.EstimateApprovalSteps.FirstOrDefaultAsync(step =>
                    step.OrganizationId == organizationId && step.EstimateApprovalRequestId == openRequest.Id &&
                    step.ReviewerMembershipId == actorMembershipId && step.ReviewerUserId == actorUserId &&
                    step.Status == EstimateApprovalStepStatus.Pending, cancellationToken);
                if (assignedStep is null)
                    return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_CANCEL_AUTHORITY_REQUIRED", "Cancelling a submitted estimate requires the assigned reviewer."));

                var hasCancelAuthority = await HasCurrentEstimatePermissionAsync(organizationId, estimate.BranchId,
                    actorMembershipId, actorUserId, "estimates.cancel", _clock.UtcNow, cancellationToken);
                if (!hasCancelAuthority)
                    return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_CANCEL_AUTHORITY_REQUIRED", "The assigned reviewer no longer has cancellation authority."));
            }
            else if (revision.Status is not (EstimateRevisionStatus.Draft or EstimateRevisionStatus.Returned))
            {
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", $"Cannot cancel estimate revision in status '{revision.Status}'."));
            }

            var now = _clock.UtcNow;
            try
            {
                estimate.CancelCurrentRevision(now, reason);
            }
            catch (EstimateInvalidStateException ex)
            {
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_INVALID_STATE", ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Result<EstimateDetailProjection>.Failure(new Error("ESTIMATE_CANCEL_REASON_REQUIRED", ex.Message));
            }

            openRequest?.Close(EstimateApprovalRequestStatus.Cancelled, now);
            AddEstimateAudit(organizationId, actorUserId, "estimates.cancelled", estimate.Id,
                new { revisionId = revision.Id, revisionNo = revision.RevisionNo, approvalRequestId = openRequest?.Id, routeHash = openRequest?.RouteHash, reason = reason.Trim() });
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), organizationId, operation,
                keyHash, payloadHash, estimate.Id.ToString(), now));
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return Result<EstimateDetailProjection>.Success(MapToDetailProjection(estimate));
        });
    }

    private IQueryable<Estimate> LoadEstimateDetails() => _db.Estimates
        .Include(estimate => estimate.Revisions)
            .ThenInclude(revision => revision.Sections)
                .ThenInclude(section => section.WorkItems)
                    .ThenInclude(workItem => workItem.CostComponents);

    private async Task<Result<EstimateDetailProjection>?> GetIdempotencyReplayAsync(
        Guid organizationId, Guid estimateId, string operation, string keyHash,
        string payloadHash, CancellationToken cancellationToken)
    {
        var existing = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(record =>
            record.OrganizationId == organizationId && record.Operation == operation && record.KeyHash == keyHash,
            cancellationToken);
        if (existing is null)
            return null;
        if (existing.PayloadHash != payloadHash)
            return Result<EstimateDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
        if (!Guid.TryParse(existing.ResourceId, out var replayEstimateId) || replayEstimateId != estimateId)
            return Result<EstimateDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key belongs to a different estimate."));
        var estimate = await LoadEstimateDetails().AsNoTracking().FirstOrDefaultAsync(row =>
            row.Id == estimateId && row.OrganizationId == organizationId, cancellationToken);
        return estimate is null
            ? Result<EstimateDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Estimate not found."))
            : Result<EstimateDetailProjection>.Success(MapToDetailProjection(estimate));
    }

    private async Task<ApprovalCandidate?> FindIndependentReviewerAsync(
        Guid organizationId, Guid branchId, Guid submitterUserId, Guid? lastEditorUserId,
        DateTimeOffset atUtc, CancellationToken cancellationToken,
        string? requiredRoleName = null, IReadOnlyCollection<Guid>? excludedReviewerUserIds = null)
    {
        var excludedUsers = excludedReviewerUserIds?.ToArray() ?? [];
        var candidates = await (from membership in _db.Memberships
            join membershipRole in _db.MembershipRoles on new { MembershipId = membership.Id, membership.OrganizationId } equals new { membershipRole.MembershipId, membershipRole.OrganizationId }
            join rolePermission in _db.RolePermissions on new { membershipRole.RoleId, membershipRole.OrganizationId } equals new { rolePermission.RoleId, rolePermission.OrganizationId }
            join permission in _db.Permissions on rolePermission.PermissionId equals permission.Id
            join role in _db.Roles on new { membershipRole.RoleId, membershipRole.OrganizationId } equals new { RoleId = role.Id, role.OrganizationId }
            join user in _db.Users on membership.UserId equals user.Id
            where membership.OrganizationId == organizationId && membership.IsActive && user.IsActive && role.IsActive && permission.IsActive &&
                permission.Key == "estimates.approve" && membership.UserId != submitterUserId &&
                (lastEditorUserId == null || membership.UserId != lastEditorUserId) &&
                !excludedUsers.Contains(membership.UserId) &&
                (requiredRoleName == null || role.NormalizedName == requiredRoleName) &&
                (membership.BranchId == null || membership.BranchId == branchId) &&
                (membership.StartsAtUtc == null || membership.StartsAtUtc <= atUtc) &&
                (membership.ExpiresAtUtc == null || membership.ExpiresAtUtc > atUtc) &&
                ((rolePermission.Scope == PermissionScope.Organization && rolePermission.ScopeId == organizationId) ||
                 (rolePermission.Scope == PermissionScope.Branch && rolePermission.BranchId == branchId))
            select new
            {
                UserId = membership.UserId,
                MembershipId = membership.Id,
                ScopeType = rolePermission.Scope == PermissionScope.Branch ? "branch" : "organization",
                ScopeId = rolePermission.Scope == PermissionScope.Branch ? branchId : organizationId
            })
            .Distinct().OrderBy(candidate => candidate.MembershipId).ToListAsync(cancellationToken);
        var candidate = candidates.FirstOrDefault();
        return candidate is null
            ? null
            : new ApprovalCandidate(candidate.UserId, candidate.MembershipId, candidate.ScopeType, candidate.ScopeId);
    }

    private async Task<bool> HasCurrentApprovalPermissionAsync(
        Guid organizationId, Guid branchId, Guid membershipId, Guid userId,
        DateTimeOffset atUtc, CancellationToken cancellationToken) =>
        await HasCurrentEstimatePermissionAsync(organizationId, branchId, membershipId, userId,
            "estimates.approve", atUtc, cancellationToken);

    private async Task<bool> HasCurrentEstimatePermissionAsync(
        Guid organizationId, Guid branchId, Guid membershipId, Guid userId, string permissionKey,
        DateTimeOffset atUtc, CancellationToken cancellationToken) =>
        await (from membership in _db.Memberships
            join membershipRole in _db.MembershipRoles on new { MembershipId = membership.Id, membership.OrganizationId } equals new { membershipRole.MembershipId, membershipRole.OrganizationId }
            join rolePermission in _db.RolePermissions on new { membershipRole.RoleId, membershipRole.OrganizationId } equals new { rolePermission.RoleId, rolePermission.OrganizationId }
            join permission in _db.Permissions on rolePermission.PermissionId equals permission.Id
            join role in _db.Roles on new { membershipRole.RoleId, membershipRole.OrganizationId } equals new { RoleId = role.Id, role.OrganizationId }
            join user in _db.Users on membership.UserId equals user.Id
            where membership.Id == membershipId && membership.OrganizationId == organizationId && membership.UserId == userId &&
                membership.IsActive && user.IsActive && role.IsActive && permission.IsActive && permission.Key == permissionKey &&
                (membership.BranchId == null || membership.BranchId == branchId) &&
                (membership.StartsAtUtc == null || membership.StartsAtUtc <= atUtc) &&
                (membership.ExpiresAtUtc == null || membership.ExpiresAtUtc > atUtc) &&
                ((rolePermission.Scope == PermissionScope.Organization && rolePermission.ScopeId == organizationId) ||
                 (rolePermission.Scope == PermissionScope.Branch && rolePermission.BranchId == branchId))
            select membership.Id).AnyAsync(cancellationToken);

    private void AddEstimateAudit(Guid organizationId, Guid actorUserId, string eventType, Guid estimateId, object payload)
    {
        _db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), organizationId, actorUserId, eventType,
            "Estimate", estimateId.ToString(), _clock.UtcNow, string.Empty, JsonSerializer.Serialize(payload)));
    }

    private static string HashSnapshot(string value) => Convert.ToHexString(
        System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static bool SnapshotsMatch(string left, string right)
    {
        try
        {
            return System.Text.Json.Nodes.JsonNode.DeepEquals(
                System.Text.Json.Nodes.JsonNode.Parse(left),
                System.Text.Json.Nodes.JsonNode.Parse(right));
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    private sealed record ApprovalCandidate(Guid UserId, Guid MembershipId, string ScopeType, Guid ScopeId);

    private static async Task<TPolicy?> ResolvePolicyAsync<TPolicy>(
        IQueryable<TPolicy> policies,
        Guid organizationId,
        Guid branchId,
        DateTimeOffset atUtc,
        CancellationToken cancellationToken)
        where TPolicy : class
    {
        if (typeof(TPolicy) == typeof(CalculationPolicyVersion))
        {
            var rows = await policies.Cast<CalculationPolicyVersion>()
                .Where(row => row.OrganizationId == organizationId && row.Status == EstimatePolicyStatus.Published &&
                    row.EffectiveFromUtc <= atUtc && (row.EffectiveToUtc == null || row.EffectiveToUtc > atUtc) &&
                    (row.BranchId == branchId || row.BranchId == null))
                .ToListAsync(cancellationToken);
            return (TPolicy?)(object?)SelectPolicy(rows, branchId);
        }

        if (typeof(TPolicy) == typeof(TaxPolicyVersion))
        {
            var rows = await policies.Cast<TaxPolicyVersion>()
                .Where(row => row.OrganizationId == organizationId && row.Status == EstimatePolicyStatus.Published &&
                    row.EffectiveFromUtc <= atUtc && (row.EffectiveToUtc == null || row.EffectiveToUtc > atUtc) &&
                    (row.BranchId == branchId || row.BranchId == null))
                .ToListAsync(cancellationToken);
            return (TPolicy?)(object?)SelectPolicy(rows, branchId);
        }

        throw new InvalidOperationException("Unsupported estimate policy type.");
    }

    private static TPolicy? SelectPolicy<TPolicy>(IReadOnlyCollection<TPolicy> rows, Guid branchId)
        where TPolicy : class
    {
        var branchRows = rows.Where(row => row switch
        {
            CalculationPolicyVersion calculation => calculation.BranchId == branchId,
            TaxPolicyVersion tax => tax.BranchId == branchId,
            _ => false
        }).ToArray();
        if (branchRows.Length > 1)
            return null;
        if (branchRows.Length == 1)
            return branchRows[0];

        var organizationRows = rows.Where(row => row switch
        {
            CalculationPolicyVersion calculation => calculation.BranchId is null,
            TaxPolicyVersion tax => tax.BranchId is null,
            _ => false
        }).ToArray();
        return organizationRows.Length == 1 ? organizationRows[0] : null;
    }

    public async Task<Result<QuotationDetailProjection>> IssueQuotationAsync(
        Guid organizationId,
        Guid estimateId,
        Guid expectedEstimateVersion,
        Guid expectedOpportunityVersion,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        const string operation = "quotations.issue";
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Replay check before version / stage checks
            var existingRecord = await _db.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    row => row.OrganizationId == organizationId &&
                           row.Operation == operation &&
                           row.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord is not null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<QuotationDetailProjection>.Failure(new Error(
                        "IDEMPOTENCY_KEY_REUSED",
                        "The idempotency key has already been used with a different payload."));
                }

                if (Guid.TryParse(existingRecord.ResourceId, out var replayQuotationId))
                {
                    var replayQuotation = await _db.Quotations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(q => q.Id == replayQuotationId && q.OrganizationId == organizationId, cancellationToken);

                    if (replayQuotation is not null)
                    {
                        var replayEstimate = await _db.Estimates
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.Id == replayQuotation.EstimateId && e.OrganizationId == organizationId, cancellationToken);
                        var replayOpp = await _db.Opportunities
                            .AsNoTracking()
                            .FirstOrDefaultAsync(o => o.Id == replayQuotation.OpportunityId && o.OrganizationId == organizationId, cancellationToken);

                        if (replayEstimate is not null && replayOpp is not null)
                        {
                            return Result<QuotationDetailProjection>.Success(new QuotationDetailProjection(
                                replayQuotation.Id,
                                replayEstimate.Id,
                                replayOpp.Id,
                                replayQuotation.Number,
                                replayQuotation.Status,
                                replayQuotation.TotalAmount,
                                replayQuotation.IssuedAtUtc,
                                replayQuotation.EstimateRevisionId,
                                replayEstimate.CurrentRevisionNo,
                                replayOpp.Stage,
                                replayOpp.RowVersion,
                                replayEstimate.RowVersion));
                        }
                    }
                }
            }

            // 2. Load scoped entities
            var estimate = await _db.Estimates
                .Include(e => e.Revisions)
                .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.Id == estimateId, cancellationToken);

            if (estimate is null)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", $"Estimate '{estimateId}' was not found."));
            }

            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.OrganizationId == organizationId && o.Id == estimate.OpportunityId, cancellationToken);

            if (opp is null)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", $"Opportunity '{estimate.OpportunityId}' was not found."));
            }

            // 3. Concurrency and stage checks
            if (estimate.RowVersion != expectedEstimateVersion)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("ESTIMATE_VERSION_CONFLICT", "The estimate has been modified by another user."));
            }

            if (opp.RowVersion != expectedOpportunityVersion)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            var currentRev = estimate.CurrentRevision;
            if (currentRev is null)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("ESTIMATE_INVALID_STATE", "Estimate has no revisions."));
            }

            if (currentRev.Status != EstimateRevisionStatus.Approved ||
                currentRev.CalculationOutdated ||
                string.IsNullOrWhiteSpace(currentRev.CalculationSnapshotJson) ||
                string.IsNullOrWhiteSpace(currentRev.ApprovalSnapshotJson))
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("ESTIMATE_INVALID_STATE", "Only an approved estimate revision with a calculation snapshot can be quoted."));
            }

            var calculationSnapshot = await _db.EstimateCalculationSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(snapshot => snapshot.OrganizationId == organizationId &&
                    snapshot.EstimateRevisionId == currentRev.Id &&
                    snapshot.CalculationVersion == currentRev.CalculationVersion,
                    cancellationToken);
            var approvalSnapshot = await _db.EstimateApprovalSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(snapshot => snapshot.OrganizationId == organizationId &&
                    snapshot.EstimateId == estimate.Id &&
                    snapshot.EstimateRevisionId == currentRev.Id,
                    cancellationToken);

            if (calculationSnapshot is null || approvalSnapshot is null ||
                approvalSnapshot.CalculationVersion != currentRev.CalculationVersion ||
                approvalSnapshot.CalculationInputHash != calculationSnapshot.InputHash ||
                approvalSnapshot.CalculationSnapshotHash != HashSnapshot(calculationSnapshot.SnapshotJson) ||
                !SnapshotsMatch(currentRev.CalculationSnapshotJson, calculationSnapshot.SnapshotJson) ||
                !SnapshotsMatch(currentRev.ApprovalSnapshotJson, approvalSnapshot.SnapshotJson))
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("ESTIMATE_INVALID_STATE", "The approved estimate snapshots do not match the current calculation."));
            }

            if (currentRev.GrandTotal <= 0)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("ESTIMATE_INVALID_STATE", "Cannot issue quotation for an estimate without calculated amount."));
            }

            if (opp.Stage != OpportunityStage.Estimating)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", $"Opportunity must be in 'estimating' stage to issue a quotation. Current stage is '{opp.Stage}'."));
            }

            var billing = await QuotationBillingSnapshotBuilder.BuildAsync(_db, organizationId, estimate.CustomerId, cancellationToken);
            if (billing.IsFailure)
                return Result<QuotationDetailProjection>.Failure(billing.Error);
            var billingSnapshotJson = billing.Value!.Json;
            var billingSnapshotHash = billing.Value.Hash;

            // 4. Atomic document numbering (no catch-all and no CountAsync()+1 fallback)
            string quotationNumber;
            try
            {
                quotationNumber = await _documentNumberGenerator.GenerateAsync(
                    organizationId,
                    DocumentTypes.Quotations,
                    estimate.BranchId,
                    _clock.UtcNow,
                    cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Result<QuotationDetailProjection>.Failure(
                    new Error("DOCUMENT_NUMBER_ALLOCATION_FAILED", "Failed to allocate quotation document number."));
            }

            var prevOppStage = opp.Stage;
            opp.EnterProposed(expectedOpportunityVersion);
            estimate.MarkQuoted();

            var snapshotHash = approvalSnapshot.CalculationSnapshotHash;

            var quotation = new Quotation(
                Guid.NewGuid(),
                organizationId,
                estimate.BranchId,
                estimate.CustomerId,
                opp.Id,
                estimate.Id,
                currentRev.Id,
                quotationNumber,
                currentRev.GrandTotal,
                snapshotHash,
                _clock.UtcNow,
                billingSnapshotJson,
                billingSnapshotHash);

            _db.Quotations.Add(quotation);

            var history = new OpportunityStageHistory(
                Guid.NewGuid(),
                organizationId,
                opp.Id,
                prevOppStage,
                OpportunityStage.Proposed,
                reasonCode: null,
                note: $"Quotation {quotation.Number} issued for Estimate {estimate.Number}.",
                actorUserId,
                _clock.UtcNow,
                OpportunityStagePolicy.Version,
                traceId);

            _db.OpportunityStageHistories.Add(history);

            var oppAudit = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "opportunity.stage-changed",
                "Opportunity",
                opp.Id.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new
                {
                    fromStage = prevOppStage,
                    toStage = OpportunityStage.Proposed,
                    quotationNumber = quotation.Number,
                    estimateNumber = estimate.Number
                }));

            var quoteAudit = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "quotations.issued",
                "Quotation",
                quotation.Id.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new
                {
                    quotationNumber = quotation.Number,
                    quotation.TotalAmount,
                    estimateNumber = estimate.Number
                }));

            _db.AddAuditEvent(oppAudit);
            _db.AddAuditEvent(quoteAudit);

            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(),
                organizationId,
                operation,
                keyHash,
                payloadHash,
                quotation.Id.ToString(),
                _clock.UtcNow));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505")
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();

                var winnerRecord = await _db.IdempotencyRecords
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        row => row.OrganizationId == organizationId &&
                               row.Operation == operation &&
                               row.KeyHash == keyHash,
                        cancellationToken);

                if (winnerRecord is not null)
                {
                    if (winnerRecord.PayloadHash != payloadHash)
                    {
                        return Result<QuotationDetailProjection>.Failure(new Error(
                            "IDEMPOTENCY_KEY_REUSED",
                            "The idempotency key has already been used with a different payload."));
                    }

                    if (Guid.TryParse(winnerRecord.ResourceId, out var winnerQuotationId))
                    {
                        var winnerQuotation = await _db.Quotations
                            .AsNoTracking()
                            .FirstOrDefaultAsync(q => q.Id == winnerQuotationId && q.OrganizationId == organizationId, cancellationToken);

                        if (winnerQuotation is not null)
                        {
                            var winnerEstimate = await _db.Estimates
                                .AsNoTracking()
                                .FirstOrDefaultAsync(e => e.Id == winnerQuotation.EstimateId && e.OrganizationId == organizationId, cancellationToken);
                            var winnerOpp = await _db.Opportunities
                                .AsNoTracking()
                                .FirstOrDefaultAsync(o => o.Id == winnerQuotation.OpportunityId && o.OrganizationId == organizationId, cancellationToken);

                            if (winnerEstimate is not null && winnerOpp is not null)
                            {
                                return Result<QuotationDetailProjection>.Success(new QuotationDetailProjection(
                                    winnerQuotation.Id,
                                    winnerEstimate.Id,
                                    winnerOpp.Id,
                                    winnerQuotation.Number,
                                    winnerQuotation.Status,
                                    winnerQuotation.TotalAmount,
                                    winnerQuotation.IssuedAtUtc,
                                    winnerQuotation.EstimateRevisionId,
                                    winnerEstimate.CurrentRevisionNo,
                                    winnerOpp.Stage,
                                    winnerOpp.RowVersion,
                                    winnerEstimate.RowVersion));
                            }
                        }
                    }
                }

                throw;
            }

            return Result<QuotationDetailProjection>.Success(new QuotationDetailProjection(
                quotation.Id,
                estimate.Id,
                opp.Id,
                quotation.Number,
                quotation.Status,
                quotation.TotalAmount,
                quotation.IssuedAtUtc,
                quotation.EstimateRevisionId,
                estimate.CurrentRevisionNo,
                opp.Stage,
                opp.RowVersion,
                estimate.RowVersion));
        });
    }

    public async Task<Result<AcceptQuotationProjection>> AcceptQuotationAsync(
        Guid organizationId,
        Guid estimateId,
        Guid expectedOpportunityVersion,
        string? decisionNote,
        Guid actorUserId,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken cancellationToken,
        Guid? expectedQuotationId = null)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        const string operation = "quotations.accept";
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Replay check precedes Won/state/version checks
            var existingRecord = await _db.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    row => row.OrganizationId == organizationId &&
                           row.Operation == operation &&
                           row.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord is not null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<AcceptQuotationProjection>.Failure(new Error(
                        "IDEMPOTENCY_KEY_REUSED",
                        "The idempotency key has already been used with a different payload."));
                }

                if (Guid.TryParse(existingRecord.ResourceId, out var replayQuotationId))
                {
                    var replayQuotation = await _db.Quotations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(q => q.Id == replayQuotationId && q.OrganizationId == organizationId, cancellationToken);

                    if (replayQuotation is not null)
                    {
                        var replayOpp = await _db.Opportunities
                            .AsNoTracking()
                            .FirstOrDefaultAsync(o => o.Id == replayQuotation.OpportunityId && o.OrganizationId == organizationId, cancellationToken);

                        if (replayOpp is not null)
                        {
                            return Result<AcceptQuotationProjection>.Success(new AcceptQuotationProjection(
                                replayQuotation.Id,
                                replayOpp.Id,
                                replayOpp.Stage,
                                replayOpp.RowVersion,
                                replayQuotation.AcceptedAtUtc ?? replayQuotation.UpdatedAtUtc));
                        }
                    }
                }
            }

            // 2. Load scoped entities
            var estimate = await _db.Estimates
                .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.Id == estimateId, cancellationToken);

            if (estimate is null)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", $"Estimate '{estimateId}' was not found."));
            }

            var opp = await _db.Opportunities
                .FirstOrDefaultAsync(o => o.OrganizationId == organizationId && o.Id == estimate.OpportunityId, cancellationToken);

            if (opp is null)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", $"Opportunity '{estimate.OpportunityId}' was not found."));
            }

            // Only the live quotation can be accepted; superseded and voided ones are history.
            var quotation = await _db.Quotations
                .FirstOrDefaultAsync(q => q.OrganizationId == organizationId && q.EstimateId == estimateId &&
                    (q.Status == QuotationStatus.Issued || q.Status == QuotationStatus.Accepted), cancellationToken);

            if (quotation is null)
            {
                var hasHistory = await _db.Quotations.AnyAsync(q => q.OrganizationId == organizationId && q.EstimateId == estimateId, cancellationToken);
                return Result<AcceptQuotationProjection>.Failure(hasHistory
                    ? new Error("QUOTATION_INVALID_STATE", "The quotation was voided or superseded; there is no live quotation to accept.")
                    : new Error("RESOURCE_NOT_FOUND", $"No quotation found for estimate '{estimateId}'."));
            }

            // A customer link points at one specific document; if it was replaced meanwhile, the live one must not be accepted by mistake.
            if (expectedQuotationId.HasValue && quotation.Id != expectedQuotationId.Value)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("QUOTATION_INVALID_STATE", "The quotation was voided or superseded; there is no live quotation to accept."));
            }

            // 3. Stage and Concurrency validation
            // If already Won and not a replay of the original winning idempotency key: reject!
            if (opp.Stage == OpportunityStage.Won)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", "Opportunity is already in 'won' stage."));
            }

            if (opp.RowVersion != expectedOpportunityVersion)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("OPPORTUNITY_VERSION_CONFLICT", "Opportunity version conflict."));
            }

            if (opp.Stage != OpportunityStage.Proposed)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("OPPORTUNITY_INVALID_TRANSITION", $"Opportunity must be in 'proposed' stage to accept quotation. Current stage is '{opp.Stage}'."));
            }

            // 4. Perform Transition and Accept
            var prevOppStage = opp.Stage;
            opp.MarkWon(expectedOpportunityVersion);
            quotation.Accept(_clock.UtcNow);

            var history = new OpportunityStageHistory(
                Guid.NewGuid(),
                organizationId,
                opp.Id,
                prevOppStage,
                OpportunityStage.Won,
                reasonCode: null,
                note: string.IsNullOrWhiteSpace(decisionNote) ? $"Quotation {quotation.Number} accepted by customer." : decisionNote.Trim(),
                actorUserId,
                _clock.UtcNow,
                OpportunityStagePolicy.Version,
                traceId);

            _db.OpportunityStageHistories.Add(history);

            // Audits omit decision note
            var oppAudit = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "opportunity.stage-changed",
                "Opportunity",
                opp.Id.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new
                {
                    fromStage = prevOppStage,
                    toStage = OpportunityStage.Won,
                    quotationNumber = quotation.Number
                }));

            var quoteAudit = new AuditEvent(
                Guid.NewGuid(),
                organizationId,
                actorUserId,
                "quotations.accepted",
                "Quotation",
                quotation.Id.ToString(),
                _clock.UtcNow,
                string.Empty,
                JsonSerializer.Serialize(new
                {
                    quotation.Number,
                    acceptedAtUtc = quotation.AcceptedAtUtc
                }));

            _db.AddAuditEvent(oppAudit);
            _db.AddAuditEvent(quoteAudit);

            _db.IdempotencyRecords.Add(new IdempotencyRecord(
                Guid.NewGuid(),
                organizationId,
                operation,
                keyHash,
                payloadHash,
                quotation.Id.ToString(),
                _clock.UtcNow));

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505")
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();

                var winnerRecord = await _db.IdempotencyRecords
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        row => row.OrganizationId == organizationId &&
                               row.Operation == operation &&
                               row.KeyHash == keyHash,
                        cancellationToken);

                if (winnerRecord is not null)
                {
                    if (winnerRecord.PayloadHash != payloadHash)
                    {
                        return Result<AcceptQuotationProjection>.Failure(new Error(
                            "IDEMPOTENCY_KEY_REUSED",
                            "The idempotency key has already been used with a different payload."));
                    }

                    if (Guid.TryParse(winnerRecord.ResourceId, out var winnerQuotationId))
                    {
                        var winnerQuotation = await _db.Quotations
                            .AsNoTracking()
                            .FirstOrDefaultAsync(q => q.Id == winnerQuotationId && q.OrganizationId == organizationId, cancellationToken);

                        if (winnerQuotation is not null)
                        {
                            var winnerOpp = await _db.Opportunities
                                .AsNoTracking()
                                .FirstOrDefaultAsync(o => o.Id == winnerQuotation.OpportunityId && o.OrganizationId == organizationId, cancellationToken);

                            if (winnerOpp is not null)
                            {
                                return Result<AcceptQuotationProjection>.Success(new AcceptQuotationProjection(
                                    winnerQuotation.Id,
                                    winnerOpp.Id,
                                    winnerOpp.Stage,
                                    winnerOpp.RowVersion,
                                    winnerQuotation.AcceptedAtUtc ?? winnerQuotation.UpdatedAtUtc));
                            }
                        }
                    }
                }

                throw;
            }

            return Result<AcceptQuotationProjection>.Success(new AcceptQuotationProjection(
                quotation.Id,
                opp.Id,
                opp.Stage,
                opp.RowVersion,
                quotation.AcceptedAtUtc!.Value));
        });
    }

    public async Task<Result<QuotationDocumentProjection>> GetQuotationDocumentAsync(
        Guid organizationId,
        Guid estimateId,
        string locale,
        CancellationToken cancellationToken)
    {
        var quotation = await _db.Quotations
            .AsNoTracking()
            .Where(q => q.OrganizationId == organizationId && q.EstimateId == estimateId)
            .OrderByDescending(q => q.IssuedAtUtc)
            .ThenByDescending(q => q.Number)
            .FirstOrDefaultAsync(cancellationToken);

        if (quotation is null)
        {
            return Result<QuotationDocumentProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Quotation for estimate '{estimateId}' was not found."));
        }

        var revision = await _db.EstimateRevisions
            .AsNoTracking()
            .Include(r => r.Sections)
                .ThenInclude(s => s.WorkItems)
            .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.Id == quotation.EstimateRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result<QuotationDocumentProjection>.Failure(
                new Error("RESOURCE_NOT_FOUND", $"Estimate revision '{quotation.EstimateRevisionId}' was not found."));
        }

        QuotationBillingSnapshotData? customerSnapshot = null;
        if (!string.IsNullOrWhiteSpace(quotation.CustomerBillingSnapshotJson))
        {
            try
            {
                customerSnapshot = JsonSerializer.Deserialize<QuotationBillingSnapshotData>(
                    quotation.CustomerBillingSnapshotJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                // Billing snapshot is stored data; never log its content (PII), only identifiers.
                _logger.LogError(ex, "Quotation {QuotationId} has an unreadable customer billing snapshot.", quotation.Id);
            }
        }

        if (customerSnapshot is null)
        {
            return Result<QuotationDocumentProjection>.Failure(
                new Error("ESTIMATE_INVALID_STATE", "Quotation billing snapshot is missing or corrupted."));
        }

        var isEn = locale.Equals("en", StringComparison.OrdinalIgnoreCase);
        var hasIncompleteTranslations = false;

        string? customerDisplayName;
        if (isEn)
        {
            customerDisplayName = string.IsNullOrWhiteSpace(customerSnapshot.DisplayNameEn)
                ? null
                : customerSnapshot.DisplayNameEn.Trim();
        }
        else
        {
            customerDisplayName = customerSnapshot.DisplayNameTh;
        }

        var customerAddress = customerSnapshot.Address is null
            ? null
            : new QuotationAddressProjection(
                customerSnapshot.Address.Label,
                customerSnapshot.Address.AddressLine1,
                customerSnapshot.Address.Subdistrict,
                customerSnapshot.Address.District,
                customerSnapshot.Address.Province,
                customerSnapshot.Address.PostalCode,
                customerSnapshot.Address.CountryCode);

        var customerDoc = new QuotationCustomerProjection(
            customerSnapshot.CustomerType,
            customerDisplayName,
            customerSnapshot.DisplayNameTh,
            customerSnapshot.DisplayNameEn,
            customerSnapshot.LegalName,
            customerSnapshot.TaxIdentifier,
            customerSnapshot.BranchCode,
            customerAddress);

        var sectionsDoc = new List<QuotationSectionProjection>();
        foreach (var section in revision.Sections.OrderBy(s => s.SortOrder))
        {
            string? sectionName;
            if (isEn)
            {
                if (string.IsNullOrWhiteSpace(section.NameEn))
                {
                    sectionName = null;
                    hasIncompleteTranslations = true;
                }
                else
                {
                    sectionName = section.NameEn.Trim();
                }
            }
            else
            {
                sectionName = section.NameTh;
            }

            var workItemsDoc = new List<QuotationWorkItemProjection>();
            foreach (var item in section.WorkItems.OrderBy(w => w.SortOrder))
            {
                string? itemDescription;
                if (isEn)
                {
                    if (string.IsNullOrWhiteSpace(item.DescriptionEn))
                    {
                        itemDescription = null;
                        hasIncompleteTranslations = true;
                    }
                    else
                    {
                        itemDescription = item.DescriptionEn.Trim();
                    }
                }
                else
                {
                    itemDescription = item.DescriptionTh;
                }

                workItemsDoc.Add(new QuotationWorkItemProjection(
                    item.Code,
                    itemDescription,
                    item.DescriptionTh,
                    item.DescriptionEn,
                    item.Quantity,
                    item.UnitCode,
                    item.UnitSellingPrice,
                    item.TotalSellingPrice));
            }

            sectionsDoc.Add(new QuotationSectionProjection(
                section.Code,
                sectionName,
                section.NameTh,
                section.NameEn,
                section.SubtotalSellingPrice,
                workItemsDoc));
        }

        var totalsDoc = new QuotationTotalsProjection(
            revision.SellingBeforeDiscount,
            revision.DiscountType,
            revision.DiscountValue,
            revision.DiscountAmount,
            revision.NetBeforeTax,
            revision.TaxAmount,
            revision.GrandTotal);

        var doc = new QuotationDocumentProjection(
            quotation.BranchId,
            quotation.Number,
            quotation.IssuedAtUtc,
            revision.Currency,
            locale,
            hasIncompleteTranslations,
            customerDoc,
            sectionsDoc,
            totalsDoc);

        return Result<QuotationDocumentProjection>.Success(doc);
    }

    private sealed record QuotationBillingSnapshotData(
        string CustomerType,
        string DisplayNameTh,
        string? DisplayNameEn,
        string? LegalName,
        string? TaxIdentifier,
        string? BranchCode,
        QuotationBillingSnapshotAddress? Address);

    private sealed record QuotationBillingSnapshotAddress(
        string? Label,
        string AddressLine1,
        string? Subdistrict,
        string? District,
        string? Province,
        string? PostalCode,
        string? CountryCode);

    private static EstimateDetailProjection MapToDetailProjection(Estimate estimate)
    {
        var rev = estimate.CurrentRevision;
        var revProj = rev is null ? null : MapToRevisionProjection(rev);

        return new EstimateDetailProjection(
            estimate.Id,
            estimate.OrganizationId,
            estimate.BranchId,
            estimate.CustomerId,
            estimate.OpportunityId,
            estimate.SiteSurveyRevisionId,
            estimate.SiteSurveySnapshotHash,
            estimate.Number,
            estimate.Status,
            estimate.CurrentRevisionNo,
            estimate.RowVersion,
            estimate.CreatedAtUtc,
            estimate.UpdatedAtUtc,
            revProj);
    }

    private static EstimateRevisionProjection MapToRevisionProjection(EstimateRevision rev)
    {
        var sectionsProj = rev.Sections
            .OrderBy(s => s.SortOrder)
            .Select(s => new EstimateSectionProjection(
                s.Id,
                s.EstimateRevisionId,
                s.Code,
                s.NameTh,
                s.NameEn,
                s.SortOrder,
                s.SubtotalCost,
                s.SubtotalSellingPrice,
                s.WorkItems
                    .OrderBy(w => w.SortOrder)
                    .Select(w => new EstimateWorkItemProjection(
                        w.Id,
                        w.EstimateSectionId,
                        w.Code,
                        w.DescriptionTh,
                        w.DescriptionEn,
                        w.Quantity,
                        w.UnitCode,
                        w.SellingRuleType,
                        w.SellingRuleValue,
                        w.SellingRuleReasonCode,
                        w.UnitCost,
                        w.TotalCost,
                        w.UnitSellingPrice,
                        w.TotalSellingPrice,
                        w.SortOrder,
                        w.CostComponents
                            .OrderBy(c => c.SortOrder)
                            .Select(c => new EstimateCostComponentProjection(
                                c.Id,
                                c.EstimateWorkItemId,
                                c.Type,
                                c.Description,
                                c.Quantity,
                                c.UnitCode,
                                c.UnitCost,
                                c.Currency,
                                c.TotalCost,
                                c.SortOrder,
                                c.ItemId,
                                c.CostRecordId,
                                c.CostRecordVersion,
                                c.ItemCodeSnapshot,
                                c.ItemNameSnapshot,
                                c.UnitSnapshot,
                                c.UnitCostSnapshot,
                                c.CurrencySnapshot,
                                c.CostScopeSnapshot,
                                c.CostEffectiveFromUtc,
                                c.CostPolicyVersion,
                                c.ResolvedAtUtc,
                                c.CostOrigin,
                                c.CostSourceIdSnapshot,
                                c.CostSourceCodeSnapshot,
                                c.CostSourceReferenceSnapshot,
                                c.CostEvidenceFileIdSnapshot,
                                c.CostRecordReasonSnapshot,
                                c.ProvisionalReasonCode,
                                c.ProvisionalNote,
                                c.IsProvisional))
                            .ToList(),
                        w.ItemId,
                        w.ItemCodeSnapshot,
                        w.ItemNameThSnapshot,
                        w.ItemNameEnSnapshot,
                        w.OverrideReasonCode,
                        w.OverrideReason))
                    .ToList()))
            .ToList();

        return new EstimateRevisionProjection(
            rev.Id,
            rev.EstimateId,
            rev.RevisionNo,
            rev.Status,
            rev.Currency,
            rev.CalculationVersion,
            rev.CalculationOutdated,
            rev.CalculationPolicyVersion,
            rev.TaxPolicyVersion,
            rev.NetCost,
            rev.SellingBeforeDiscount,
            rev.DiscountType,
            rev.DiscountValue,
            rev.DiscountReasonCode,
            rev.DiscountAmount,
            rev.NetBeforeTax,
            rev.TaxAmount,
            rev.GrandTotal,
            rev.MarginAmount,
            rev.MarginRate,
            rev.MarkupRate,
            rev.CalculationSnapshotJson,
            rev.RowVersion,
            rev.CreatedAtUtc,
            rev.UpdatedAtUtc,
            sectionsProj,
            rev.EvaluateReadiness());
    }

    private static string CalculateInputHash(
        EstimateRevision revision,
        EstimateDiscount discount,
        CalculationPolicyVersion calculationPolicy,
        TaxPolicyVersion taxPolicy)
    {
        var input = new
        {
            revision.Id,
            revision.RevisionNo,
            revision.Currency,
            discountType = discount.Type,
            discountValue = discount.Value,
            discountReasonCode = discount.ReasonCode,
            calculationPolicyHash = calculationPolicy.ContentHash,
            taxPolicyHash = taxPolicy.ContentHash,
            Sections = revision.Sections.OrderBy(section => section.SortOrder).Select(section => new
            {
                section.Code,
                section.SortOrder,
                WorkItems = section.WorkItems.OrderBy(workItem => workItem.SortOrder).Select(workItem => new
                {
                    workItem.Code,
                    workItem.Quantity,
                    workItem.UnitCode,
                    workItem.SellingRuleType,
                    workItem.SellingRuleValue,
                    workItem.SellingRuleReasonCode,
                    CostComponents = workItem.CostComponents.OrderBy(component => component.SortOrder).Select(component => new
                    {
                        component.Type,
                        component.Quantity,
                        component.UnitCode,
                        component.UnitCost,
                        component.Currency,
                        component.ItemId,
                        component.CostRecordId,
                        component.CostRecordVersion,
                        component.CostEffectiveFromUtc,
                        component.CostPolicyVersion,
                        component.CostOrigin,
                        component.CostSourceIdSnapshot,
                        component.CostSourceCodeSnapshot,
                        component.CostSourceReferenceSnapshot,
                        component.CostEvidenceFileIdSnapshot,
                        component.CostRecordReasonSnapshot,
                        component.ProvisionalReasonCode,
                        component.ProvisionalNote
                    })
                })
            })
        };

        var bytes = JsonSerializer.SerializeToUtf8Bytes(input);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)).ToLowerInvariant();
    }
}
