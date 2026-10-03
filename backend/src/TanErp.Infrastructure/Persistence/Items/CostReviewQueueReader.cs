using Microsoft.EntityFrameworkCore;
using TanErp.Application.Items;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public sealed class CostReviewQueueReader : ICostReviewQueueReader
{
    private readonly AppDbContext _db;

    public CostReviewQueueReader(AppDbContext db) => _db = db;

    public async Task<CostReviewQueuePage> ListAsync(Guid organizationId, string? status, string? search, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = _db.CostRecords.AsNoTracking().Where(record => record.OrganizationId == organizationId &&
            (record.Status == CostRecordStatus.Submitted || record.Status == CostRecordStatus.Approved));
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(record => record.Status == status.Trim().ToLowerInvariant());
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(record => record.Item.Code.ToLower().Contains(term)
                || record.Item.Name.Thai.ToLower().Contains(term)
                || (record.Item.Name.English != null && record.Item.Name.English.ToLower().Contains(term))
                || record.CostSource!.Code.ToLower().Contains(term)
                || record.SourceReference!.ToLower().Contains(term));
        }

        var count = await query.CountAsync(ct);
        var rows = await (from record in query
            join maker in _db.Users.AsNoTracking() on record.CreatedByUserId equals maker.Id
            join editor in _db.Users.AsNoTracking() on record.LastFinancialEditorId equals editor.Id
            orderby record.UpdatedAtUtc descending, record.Id
            select new CostReviewQueueRow(
                record.Id, record.ItemId,
                new CostReviewReference(record.Item.Id, record.Item.Code, record.Item.Name),
                record.Scope, record.UnitId,
                new CostReviewReference(record.Unit.Id, record.Unit.Code, record.Unit.Name),
                record.CostSourceId,
                record.CostSource == null ? null : new CostReviewReference(record.CostSource.Id, record.CostSource.Code, record.CostSource.Name),
                record.Amount, record.Currency, record.MinimumQuantity, record.MaximumQuantity,
                record.EffectiveFromUtc, record.EffectiveToUtc, record.Status, record.SourceReference,
                record.Reason, record.EvidenceFileId,
                new CostReviewPerson(maker.Id, maker.DisplayName), new CostReviewPerson(editor.Id, editor.DisplayName),
                record.RowVersion, record.UpdatedAtUtc))
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new CostReviewQueuePage(rows, count, pageNumber, pageSize);
    }
}
