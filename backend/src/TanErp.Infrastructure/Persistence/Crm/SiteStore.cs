using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Sites;
using TanErp.Application.Crm.Sites.CreateSite;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Sites;

namespace TanErp.Infrastructure.Persistence.Crm;

public class SiteStore : ISiteStore
{
    private readonly AppDbContext _db;

    public SiteStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<SiteProjection>> CreateAsync(
        RequestAccessContext access,
        CreateSiteCommand command,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        const string operation = "sites.create";
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
                    return Result<SiteProjection>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                }

                if (Guid.TryParse(existingRecord.ResourceId, out var existingSiteId))
                {
                    var existingSite = await _db.Sites
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == existingSiteId && s.OrganizationId == orgId, cancellationToken);

                    if (existingSite != null)
                    {
                        return Result<SiteProjection>.Success(ToProjection(existingSite));
                    }
                }
            }

            // 2. Validate Customer exists, belongs to organization, and is Active
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == command.CustomerId && c.OrganizationId == orgId, cancellationToken);

            if (customer == null)
            {
                return Result<SiteProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            }

            if (customer.Status != CustomerStatus.Active)
            {
                return Result<SiteProjection>.Failure(
                    new Error("CUSTOMER_INVALID_STATE", "Sites can only be created for active customers."));
            }

            // 3. Create Site aggregate
            var now = DateTimeOffset.UtcNow;
            var siteId = Guid.NewGuid();
            var address = new SiteAddressInput(
                command.AddressLine1,
                command.Subdistrict,
                command.District,
                command.Province,
                command.PostalCode,
                command.CountryCode);

            var site = Site.CreateActive(
                siteId,
                orgId,
                customer.Id,
                access.ActorUserId,
                command.Label,
                address,
                command.Latitude,
                command.Longitude,
                command.AccessNote,
                now);

            _db.Sites.Add(site);

            // 4. Record idempotency
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                operation,
                keyHash,
                payloadHash,
                siteId.ToString(),
                now);
            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 5. Record audit event (no raw PII)
            const string auditChanges = "{\"changedFields\":[\"customerId\",\"label\",\"addressLine1\",\"subdistrict\",\"district\",\"province\",\"postalCode\",\"countryCode\",\"latitude\",\"longitude\",\"accessNote\",\"status\"]}";
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "site.created",
                "Site",
                siteId.ToString(),
                now,
                command.TraceId,
                auditChanges);
            _db.AddAuditEvent(auditEvent);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result<SiteProjection>.Success(ToProjection(site));
        });
    }

    public async Task<IReadOnlyList<SiteProjection>?> ListByCustomerAsync(
        Guid organizationId,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        // 1. Verify Customer exists and belongs to organization
        var customerExists = await _db.Customers
            .AsNoTracking()
            .AnyAsync(c => c.Id == customerId && c.OrganizationId == organizationId, cancellationToken);

        if (!customerExists)
        {
            return null; // Signals customer not found / forbidden scope
        }

        // 2. Fetch sites ordered by normalized_label ASC, id ASC
        var sites = await _db.Sites
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && s.CustomerId == customerId)
            .OrderBy(s => s.NormalizedLabel)
            .ThenBy(s => s.Id)
            .Select(s => ToProjection(s))
            .ToListAsync(cancellationToken);

        return sites;
    }

    private static SiteProjection ToProjection(Site s) => new(
        s.Id,
        s.Code,
        s.CustomerId,
        s.Label,
        s.AddressLine1,
        s.Subdistrict,
        s.District,
        s.Province,
        s.PostalCode,
        s.CountryCode,
        s.Latitude,
        s.Longitude,
        s.AccessNote,
        s.Status,
        s.RowVersion,
        s.CreatedAtUtc);
}
