using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Crm;

public class CustomerCreationStore : ICustomerCreationStore
{
    private readonly AppDbContext _db;

    public CustomerCreationStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PersistCustomerCreationResult>> CreateAsync(
        PersistCustomerCreation request,
        CancellationToken cancellationToken = default)
    {
        var orgId = request.Customer.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency record
            var existingRecord = await _db.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.OrganizationId == orgId &&
                    r.Operation == request.Operation &&
                    r.KeyHash == request.IdempotencyKeyHash,
                    cancellationToken);

            if (existingRecord != null)
            {
                if (existingRecord.PayloadHash != request.PayloadHash)
                {
                    return Result<PersistCustomerCreationResult>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                }

                // Same key + same payload: reload existing customer
                if (Guid.TryParse(existingRecord.ResourceId, out var existingCustomerId))
                {
                    var existingCustomer = await _db.Customers
                        .Include(c => c.Contacts)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == existingCustomerId && c.OrganizationId == orgId, cancellationToken);

                    if (existingCustomer != null)
                    {
                        var replayedProjection = ProjectCustomer(existingCustomer, request.IncludeContactPii);
                        return Result<PersistCustomerCreationResult>.Success(
                            new PersistCustomerCreationResult(replayedProjection, Array.Empty<DuplicateCustomerProjection>(), WasReplayed: true));
                    }
                }
            }

            // 2. Query duplicate candidates in same organization
            var customer = request.Customer;
            var primaryContact = customer.Contacts.FirstOrDefault();
            var normName = customer.NormalizedDisplayName;
            var normPhone = primaryContact?.NormalizedPhone;
            var normEmail = primaryContact?.NormalizedEmail;

            var duplicateQuery = _db.Customers
                .AsNoTracking()
                .Include(c => c.Contacts)
                .Where(c => c.OrganizationId == orgId && c.Id != customer.Id);

            var duplicateList = await duplicateQuery
                .Where(c =>
                    c.NormalizedDisplayName == normName ||
                    (normPhone != null && c.Contacts.Any(ct => ct.NormalizedPhone == normPhone)) ||
                    (normEmail != null && c.Contacts.Any(ct => ct.NormalizedEmail == normEmail)))
                .Take(5)
                .ToListAsync(cancellationToken);

            var duplicateCandidates = duplicateList.Select(d =>
            {
                var dContact = d.Contacts.FirstOrDefault(ct => ct.IsPrimary) ?? d.Contacts.FirstOrDefault();
                return new DuplicateCustomerProjection(
                    d.Id,
                    d.Code,
                    d.DisplayNameTh,
                    MaskPhone(dContact?.Phone),
                    MaskEmail(dContact?.Email));
            }).ToList();

            // 3. Persist Customer aggregate
            _db.Customers.Add(customer);

            // 4. Persist Idempotency Record
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                request.Operation,
                request.IdempotencyKeyHash,
                request.PayloadHash,
                customer.Id.ToString(),
                customer.CreatedAtUtc);

            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 5. Persist Audit Events
            foreach (var auditEvent in request.AuditEvents)
            {
                _db.AddAuditEvent(auditEvent);
            }

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
            {
                // Concurrency unique race: rollback and check idempotency winner
                await tx.RollbackAsync(cancellationToken);

                var winnerRecord = await _db.IdempotencyRecords
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.OrganizationId == orgId &&
                        r.Operation == request.Operation &&
                        r.KeyHash == request.IdempotencyKeyHash,
                        cancellationToken);

                if (winnerRecord != null)
                {
                    if (winnerRecord.PayloadHash != request.PayloadHash)
                    {
                        return Result<PersistCustomerCreationResult>.Failure(
                            new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                    }

                    if (Guid.TryParse(winnerRecord.ResourceId, out var existingId))
                    {
                        var reloaded = await _db.Customers
                            .Include(c => c.Contacts)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == existingId && c.OrganizationId == orgId, cancellationToken);

                        if (reloaded != null)
                        {
                            return Result<PersistCustomerCreationResult>.Success(
                                new PersistCustomerCreationResult(ProjectCustomer(reloaded, request.IncludeContactPii), Array.Empty<DuplicateCustomerProjection>(), WasReplayed: true));
                        }
                    }
                }

                throw;
            }

            var createdProjection = ProjectCustomer(customer, request.IncludeContactPii);
            return Result<PersistCustomerCreationResult>.Success(
                new PersistCustomerCreationResult(createdProjection, duplicateCandidates, WasReplayed: false));
        });
    }

    private static CustomerProjection ProjectCustomer(Customer customer, bool includeContactPii)
    {
        var contact = customer.Contacts.FirstOrDefault(c => c.IsPrimary) ?? customer.Contacts.FirstOrDefault();
        var contactProjection = contact != null
            ? new CustomerContactProjection(
                contact.Name,
                contact.RoleTitle,
                includeContactPii ? contact.Phone : MaskPhone(contact.Phone),
                includeContactPii ? contact.Email : MaskEmail(contact.Email),
                contact.PreferredChannel,
                IsMasked: !includeContactPii)
            : new CustomerContactProjection(string.Empty, null, null, null, ContactChannel.Phone, IsMasked: false);

        return new CustomerProjection(
            customer.Id,
            customer.Code,
            customer.CustomerType,
            customer.DisplayNameTh,
            customer.DisplayNameEn,
            customer.PreferredLocale,
            customer.Status,
            contactProjection,
            customer.RowVersion,
            customer.CreatedAtUtc);
    }

    public static string? MaskPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        var trimmed = phone.Trim();
        if (trimmed.Length <= 4) return "****";
        var prefixLen = trimmed.StartsWith("+") ? 3 : 2;
        if (trimmed.Length <= prefixLen + 3) return trimmed[..prefixLen] + "****";
        var prefix = trimmed[..prefixLen];
        var suffix = trimmed[^3..];
        return $"{prefix}******{suffix}";
    }

    public static string? MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var trimmed = email.Trim();
        var atIndex = trimmed.IndexOf('@');
        if (atIndex <= 1) return "***" + trimmed[atIndex..];
        var domain = trimmed[atIndex..];
        return $"{trimmed[0]}***{domain}";
    }
}
