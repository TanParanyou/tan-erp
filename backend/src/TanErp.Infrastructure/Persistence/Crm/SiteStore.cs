using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
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
    private readonly IClock _clock;

    public SiteStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
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
            var existingReplay = await TryLoadReplayAsync(orgId, operation, keyHash, payloadHash, cancellationToken);
            if (existingReplay is not null)
            {
                return existingReplay;
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
            var now = _clock.UtcNow;
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

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();
                var replay = await TryLoadReplayAsync(
                    orgId, operation, keyHash, payloadHash, cancellationToken);
                if (replay is not null) return replay;
                throw;
            }

            return Result<SiteProjection>.Success(ToProjection(site));
        });
    }

    private async Task<Result<SiteProjection>?> TryLoadReplayAsync(
        Guid organizationId,
        string operation,
        string keyHash,
        string payloadHash,
        CancellationToken cancellationToken)
    {
        var record = await _db.IdempotencyRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(row =>
                row.OrganizationId == organizationId &&
                row.Operation == operation &&
                row.KeyHash == keyHash,
                cancellationToken);

        if (record is null) return null;
        if (record.PayloadHash != payloadHash)
            return Result<SiteProjection>.Failure(new Error(
                "IDEMPOTENCY_KEY_REUSED",
                "The idempotency key has already been used with a different payload."));

        if (!Guid.TryParse(record.ResourceId, out var siteId)) return null;
        var site = await _db.Sites.AsNoTracking().SingleOrDefaultAsync(
            candidate => candidate.Id == siteId && candidate.OrganizationId == organizationId,
            cancellationToken);
        return site is null ? null : Result<SiteProjection>.Success(ToProjection(site));
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
