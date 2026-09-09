using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Crm;

public class OpportunityStore : IOpportunityStore
{
    private readonly AppDbContext _db;

    public OpportunityStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<OpportunityProjection>> CreateAsync(
        RequestAccessContext access,
        CreateOpportunityCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "opportunities.create";
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency record
            var existingRecord = await _db.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.OrganizationId == orgId &&
                    r.Operation == operation &&
                    r.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord != null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                }

                if (Guid.TryParse(existingRecord.ResourceId, out var existingOppId))
                {
                    var existingOpp = await _db.Opportunities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id == existingOppId && o.OrganizationId == orgId, cancellationToken);

                    if (existingOpp != null)
                    {
                        return Result<OpportunityProjection>.Success(ToProjection(existingOpp));
                    }
                }
            }

            // 2. Validate Customer exists, belongs to org, and is Active
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == command.CustomerId && c.OrganizationId == orgId, cancellationToken);

            if (customer == null)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            }

            if (customer.Status != CustomerStatus.Active)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("CUSTOMER_INVALID_STATE", "Opportunities can only be created for active customers."));
            }

            // 3. Validate Branch exists, belongs to org, and is Active
            var branchId = access.BranchId!.Value;
            var branch = await _db.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.OrganizationId == orgId, cancellationToken);

            if (branch == null || !branch.IsActive)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Active branch not found."));
            }

            // 4. Validate PrimarySiteId if provided: must belong to the exact same customer and organization, and be Active
            if (command.PrimarySiteId.HasValue)
            {
                var site = await _db.Sites
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == command.PrimarySiteId.Value && s.CustomerId == command.CustomerId && s.OrganizationId == orgId, cancellationToken);

                if (site == null)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Primary site not found for this customer."));
                }

                if (site.Status != SiteStatus.Active)
                {
                    return Result<OpportunityProjection>.Failure(
                        new Error("RESOURCE_NOT_FOUND", "Primary site is not active."));
                }
            }

            // 5. Create Opportunity aggregate
            var now = DateTimeOffset.UtcNow;
            var oppId = Guid.NewGuid();

            var opp = Opportunity.CreateDraft(
                oppId,
                orgId,
                branchId,
                customer.Id,
                command.PrimarySiteId,
                access.ActorUserId,
                access.ActorUserId,
                command.Title,
                command.ScopeSummary,
                command.WorkTypes,
                command.SourceCode,
                command.ExpectedBudget,
                command.CurrencyCode,
                command.TargetDecisionDate,
                command.NextActionAtUtc,
                command.NextActionNote,
                now);

            _db.Opportunities.Add(opp);

            // 6. Record idempotency
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                oppId.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 7. Record audit event (no raw PII)
            const string auditChanges = "{\"changedFields\":[\"branchId\",\"customerId\",\"primarySiteId\",\"ownerUserId\",\"title\",\"scopeSummary\",\"workTypes\",\"sourceCode\",\"expectedBudget\",\"currencyCode\",\"targetDecisionDate\",\"nextActionAtUtc\",\"nextActionNote\",\"stage\"]}";
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "opportunity.created",
                "Opportunity",
                oppId.ToString(),
                now,
                command.TraceId,
                auditChanges);
            _db.AddAuditEvent(auditEvent);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result<OpportunityProjection>.Success(ToProjection(opp));
        });
    }

    public async Task<OpportunityPage> ListAsync(
        Guid organizationId,
        OpportunityListFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Opportunities
            .AsNoTracking()
            .Where(o => o.OrganizationId == organizationId);

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == filter.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Stage))
        {
            var stage = filter.Stage.Trim().ToLowerInvariant();
            query = query.Where(o => o.Stage == stage);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var normSearch = OpportunityNormalizer.CollapseWhitespace(filter.Search);
            query = query.Where(o => o.NormalizedTitle.Contains(normSearch) || o.Code.Contains(normSearch));
        }

        // Keyset Pagination cursor: NextActionAtUtc ASC NULLS LAST, Id ASC
        if (!string.IsNullOrWhiteSpace(filter.Cursor))
        {
            var decoded = OpportunityCursor.TryDecode(filter.Cursor);
            if (decoded != null)
            {
                if (decoded.NextActionAtUtc.HasValue)
                {
                    var cursorTime = decoded.NextActionAtUtc.Value;
                    var cursorId = decoded.Id;
                    query = query.Where(o =>
                        o.NextActionAtUtc == null ||
                        o.NextActionAtUtc > cursorTime ||
                        (o.NextActionAtUtc == cursorTime && o.Id > cursorId));
                }
                else
                {
                    // cursor was on a null next_action_at_utc row: all previous rows with non-null were before it
                    var cursorId = decoded.Id;
                    query = query.Where(o => o.NextActionAtUtc == null && o.Id > cursorId);
                }
            }
        }

        // Query limit + 1 items to determine if there's a next page
        var items = await query
            .OrderBy(o => o.NextActionAtUtc.HasValue ? 0 : 1)
            .ThenBy(o => o.NextActionAtUtc)
            .ThenBy(o => o.Id)
            .Take(filter.Limit + 1)
            .ToListAsync(cancellationToken);

        string? nextCursor = null;
        if (items.Count > filter.Limit)
        {
            var lastItem = items[filter.Limit - 1];
            nextCursor = OpportunityCursor.Encode(lastItem.NextActionAtUtc, lastItem.Id);
            items.RemoveAt(filter.Limit);
        }

        var projections = items.Select(ToProjection).ToList();
        return new OpportunityPage(projections, nextCursor);
    }

    public async Task<OpportunityProjection?> GetAsync(
        Guid organizationId,
        Guid opportunityId,
        CancellationToken cancellationToken = default)
    {
        var opp = await _db.Opportunities
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == opportunityId && o.OrganizationId == organizationId, cancellationToken);

        return opp != null ? ToProjection(opp) : null;
    }

    private static OpportunityProjection ToProjection(Opportunity o) => new(
        o.Id,
        o.Code,
        o.CustomerId,
        o.PrimarySiteId,
        o.BranchId,
        o.OwnerUserId,
        o.Title,
        o.ScopeSummary,
        o.WorkTypes,
        o.SourceCode,
        o.ExpectedBudget,
        o.CurrencyCode,
        o.TargetDecisionDate,
        o.NextActionAtUtc,
        o.NextActionNote,
        o.Stage,
        o.RowVersion,
        o.CreatedAtUtc);
}
