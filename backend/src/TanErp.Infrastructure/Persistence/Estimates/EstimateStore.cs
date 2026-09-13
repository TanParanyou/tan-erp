using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Estimates;
using TanErp.Domain.Common;
using TanErp.Domain.Estimates;

namespace TanErp.Infrastructure.Persistence.Estimates;

public class EstimateStore : IEstimateStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public EstimateStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<EstimateDetailProjection> CreateDraftAsync(
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid opportunityId,
        Guid? siteSurveyRevisionId,
        string? siteSurveySnapshotHash,
        string currency,
        Guid actorUserId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // Check if estimate already exists for this opportunity
            var existing = await _db.Estimates
                .Include(e => e.Revisions)
                    .ThenInclude(r => r.Sections)
                        .ThenInclude(s => s.WorkItems)
                            .ThenInclude(w => w.CostComponents)
                .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.OpportunityId == opportunityId, cancellationToken);

            if (existing is not null)
            {
                return MapToDetailProjection(existing);
            }

            var year = _clock.UtcNow.Year;
            var prefix = $"EST-{year}-";
            var count = await _db.Estimates
                .CountAsync(e => e.OrganizationId == organizationId && e.Number.StartsWith(prefix), cancellationToken);
            var number = $"{prefix}{(count + 1):D4}";

            var estimate = Estimate.CreateDraft(
                Guid.NewGuid(),
                organizationId,
                branchId,
                customerId,
                opportunityId,
                number,
                siteSurveyRevisionId,
                siteSurveySnapshotHash,
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

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return MapToDetailProjection(estimate);
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
