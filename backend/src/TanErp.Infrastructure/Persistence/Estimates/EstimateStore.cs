using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Estimates;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Estimates;

public class EstimateStore : IEstimateStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;

    public EstimateStore(AppDbContext db, IClock clock, IDocumentNumberGenerator documentNumberGenerator)
    {
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
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
                        wDto.SortOrder);

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

                        workItem.AddCostComponent(comp);
                        _db.EstimateCostComponents.Add(comp);
                    }

                    section.AddWorkItem(workItem);
                    _db.EstimateWorkItems.Add(workItem);
                }

                revision.AddSection(section);
                _db.EstimateSections.Add(section);
            }

            // Recalculate financial summary
            revision.Calculate(revision.DiscountAmount);

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
        decimal discountAmount,
        Guid actorUserId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            var revision = await _db.EstimateRevisions
                .Include(r => r.Sections)
                    .ThenInclude(s => s.WorkItems)
                        .ThenInclude(w => w.CostComponents)
                .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.EstimateId == estimateId && r.Id == revisionId, cancellationToken);

            if (revision is null)
                throw new EstimateNotFoundException(estimateId);

            if (revision.RowVersion != expectedRevisionVersion)
                throw new DbUpdateConcurrencyException("Revision version conflict.");

            revision.Calculate(discountAmount);

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

            var snapshotHash = !string.IsNullOrWhiteSpace(currentRev.CalculationSnapshotJson)
                ? Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(currentRev.CalculationSnapshotJson))).ToLowerInvariant()
                : null;

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
                _clock.UtcNow);

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
        CancellationToken cancellationToken)
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

            var quotation = await _db.Quotations
                .FirstOrDefaultAsync(q => q.OrganizationId == organizationId && q.EstimateId == estimateId, cancellationToken);

            if (quotation is null)
            {
                return Result<AcceptQuotationProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", $"No quotation found for estimate '{estimateId}'."));
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
                                c.SortOrder))
                            .ToList()))
                    .ToList()))
            .ToList();

        return new EstimateRevisionProjection(
            rev.Id,
            rev.EstimateId,
            rev.RevisionNo,
            rev.Status,
            rev.Currency,
            rev.CalculationVersion,
            rev.CalculationPolicyVersion,
            rev.TaxPolicyVersion,
            rev.NetCost,
            rev.SellingBeforeDiscount,
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
            sectionsProj);
    }
}
