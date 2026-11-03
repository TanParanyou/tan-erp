using Microsoft.EntityFrameworkCore;
using TanErp.Application.Estimates;

namespace TanErp.Infrastructure.Persistence.Estimates;

public sealed class EstimateHistoryReader : IEstimateHistoryReader
{
    private const int MaxEvents = 200;
    private readonly AppDbContext _db;

    public EstimateHistoryReader(AppDbContext db) => _db = db;

    public async Task<EstimateRevisionLinesProjection?> GetRevisionLinesAsync(Guid organizationId, Guid estimateId, Guid revisionId, CancellationToken cancellationToken)
    {
        var branchId = await (from estimate in _db.Estimates.AsNoTracking()
            join revision in _db.EstimateRevisions.AsNoTracking() on estimate.Id equals revision.EstimateId
            where estimate.OrganizationId == organizationId && estimate.Id == estimateId && revision.Id == revisionId
            select (Guid?)estimate.BranchId).FirstOrDefaultAsync(cancellationToken);
        if (branchId is null) return null;

        var lines = await (from section in _db.EstimateSections.AsNoTracking()
            join work in _db.EstimateWorkItems.AsNoTracking() on section.Id equals work.EstimateSectionId
            where section.OrganizationId == organizationId && section.EstimateRevisionId == revisionId
            orderby section.SortOrder, work.SortOrder
            select new EstimateRevisionLineProjection(section.Code, work.Code, work.DescriptionTh, work.Quantity, work.UnitCode, work.TotalCost, work.TotalSellingPrice))
            .ToListAsync(cancellationToken);
        return new EstimateRevisionLinesProjection(branchId.Value, lines);
    }

    public async Task<EstimateHistoryProjection?> GetAsync(Guid organizationId, Guid estimateId, CancellationToken cancellationToken)
    {
        var branchId = await _db.Estimates.AsNoTracking()
            .Where(estimate => estimate.OrganizationId == organizationId && estimate.Id == estimateId)
            .Select(estimate => (Guid?)estimate.BranchId)
            .FirstOrDefaultAsync(cancellationToken);
        if (branchId is null) return null;

        var revisions = await _db.EstimateRevisions.AsNoTracking()
            .Where(revision => revision.OrganizationId == organizationId && revision.EstimateId == estimateId)
            .OrderBy(revision => revision.RevisionNo)
            .Select(revision => new EstimateRevisionSummaryProjection(
                revision.Id, revision.RevisionNo, revision.Status, revision.Currency, revision.NetCost, revision.NetBeforeTax,
                revision.GrandTotal, revision.MarginRate, revision.CreatedAtUtc, revision.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        var resourceId = estimateId.ToString();
        var events = await (from audit in _db.AuditEvents.AsNoTracking()
            join user in _db.Users.AsNoTracking() on audit.ActorUserId equals user.Id
            where audit.OrganizationId == organizationId && audit.ResourceType == "Estimate" && audit.ResourceId == resourceId
            orderby audit.OccurredAtUtc descending
            select new EstimateHistoryEventProjection(audit.OccurredAtUtc, audit.Action, user.Id, user.DisplayName))
            .Take(MaxEvents)
            .ToListAsync(cancellationToken);

        return new EstimateHistoryProjection(branchId.Value, revisions, events);
    }
}
