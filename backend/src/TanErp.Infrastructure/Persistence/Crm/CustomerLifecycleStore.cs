using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;

namespace TanErp.Infrastructure.Persistence.Crm;

public class CustomerLifecycleStore : ICustomerLifecycleStore
{
    private const string ActivateOperation = "crm.customer.activate";
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public CustomerLifecycleStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<CustomerProjection>> ActivateAsync(
        RequestAccessContext access,
        Guid customerId,
        Guid expectedRowVersion,
        string keyHash,
        string payloadHash,
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            // 1. Check idempotency record first
            var existingRecord = await _db.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.OrganizationId == orgId &&
                    r.Operation == ActivateOperation &&
                    r.KeyHash == keyHash,
                    cancellationToken);

            if (existingRecord != null)
            {
                if (existingRecord.PayloadHash != payloadHash)
                {
                    return Result<CustomerProjection>.Failure(
                        new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                }

                // Replay: load and project current customer state
                var reloaded = await _db.Customers
                    .Include(c => c.Contacts)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == customerId && c.OrganizationId == orgId, cancellationToken);

                if (reloaded != null)
                {
                    return Result<CustomerProjection>.Success(ProjectCustomer(reloaded));
                }

                return Result<CustomerProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            }

            // 2. Load customer with contacts for update
            var customer = await _db.Customers
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == customerId && c.OrganizationId == orgId, cancellationToken);

            if (customer == null)
            {
                return Result<CustomerProjection>.Failure(
                    new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            }

            // 3. Perform domain transition
            var outcome = customer.Activate(expectedRowVersion);
            if (outcome == CustomerActivationOutcome.VersionConflict)
            {
                return Result<CustomerProjection>.Failure(
                    new Error("CUSTOMER_VERSION_CONFLICT", "Customer row version conflict."));
            }

            if (outcome == CustomerActivationOutcome.InvalidState)
            {
                return Result<CustomerProjection>.Failure(
                    new Error("CUSTOMER_INVALID_STATE", "Customer status is not draft or primary contact is not active."));
            }

            // 4. Record idempotency record
            var now = _clock.UtcNow;
            var idempotencyRecord = new IdempotencyRecord(
                Guid.NewGuid(),
                orgId,
                ActivateOperation,
                keyHash,
                payloadHash,
                customer.Id.ToString(),
                now);

            _db.IdempotencyRecords.Add(idempotencyRecord);

            // 5. Record audit event
            var auditPayload = JsonSerializer.Serialize(new { changedFields = new[] { "status" } });
            var auditEvent = new AuditEvent(
                Guid.NewGuid(),
                orgId,
                access.ActorUserId,
                "customer.activated",
                "Customer",
                customer.Id.ToString(),
                now,
                traceId,
                auditPayload);

            _db.AddAuditEvent(auditEvent);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<CustomerProjection>.Failure(
                    new Error("CUSTOMER_VERSION_CONFLICT", "Customer row version conflict."));
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
            {
                await tx.RollbackAsync(cancellationToken);

                var winner = await _db.IdempotencyRecords
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.OrganizationId == orgId &&
                        r.Operation == ActivateOperation &&
                        r.KeyHash == keyHash,
                        cancellationToken);

                if (winner != null)
                {
                    if (winner.PayloadHash != payloadHash)
                    {
                        return Result<CustomerProjection>.Failure(
                            new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                    }

                    var reloadedWinner = await _db.Customers
                        .Include(c => c.Contacts)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == customerId && c.OrganizationId == orgId, cancellationToken);

                    if (reloadedWinner != null)
                    {
                        return Result<CustomerProjection>.Success(ProjectCustomer(reloadedWinner));
                    }
                }

                throw;
            }

            return Result<CustomerProjection>.Success(ProjectCustomer(customer));
        });
    }

    public Task<Result<CustomerProjection>> DeactivateAsync(RequestAccessContext access, Guid customerId, Guid expectedRowVersion, string reason, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(access, customerId, expectedRowVersion, reason, keyHash, payloadHash, traceId, reactivate: false, cancellationToken);

    public Task<Result<CustomerProjection>> ReactivateAsync(RequestAccessContext access, Guid customerId, Guid expectedRowVersion, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(access, customerId, expectedRowVersion, null, keyHash, payloadHash, traceId, reactivate: true, cancellationToken);

    private async Task<Result<CustomerProjection>> ChangeStatusAsync(RequestAccessContext access, Guid customerId, Guid expectedRowVersion, string? reason, string keyHash, string payloadHash, string traceId, bool reactivate, CancellationToken cancellationToken)
    {
        var operation = reactivate ? "crm.customer.reactivate" : "crm.customer.deactivate";
        var orgId = access.OrganizationId;
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == orgId && x.Operation == operation && x.KeyHash == keyHash, cancellationToken);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<CustomerProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has been used with a different payload."));
            var replayCustomer = await _db.Customers.Include(x => x.Contacts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerId && x.OrganizationId == orgId, cancellationToken);
            return replayCustomer is null ? Result<CustomerProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found.")) : Result<CustomerProjection>.Success(ProjectCustomer(replayCustomer));
        }
        var customer = await _db.Customers.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == customerId && x.OrganizationId == orgId, cancellationToken);
        if (customer is null) return Result<CustomerProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
        if (customer.RowVersion != expectedRowVersion) return Result<CustomerProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Customer version conflict."));
        if (!reactivate)
        {
            var hasOpenOpportunity = await _db.Opportunities.AsNoTracking().AnyAsync(opportunity =>
                opportunity.OrganizationId == orgId && opportunity.CustomerId == customerId &&
                (opportunity.Stage == OpportunityStage.Draft || opportunity.Stage == OpportunityStage.Qualified || opportunity.Stage == OpportunityStage.Surveying || opportunity.Stage == OpportunityStage.Estimating || opportunity.Stage == OpportunityStage.Proposed), cancellationToken);
            if (hasOpenOpportunity) return Result<CustomerProjection>.Failure(new Error("CUSTOMER_HAS_OPEN_OPPORTUNITY", "Close or transfer open opportunities before deactivating the customer."));
        }
        var changed = reactivate ? customer.Reactivate(expectedRowVersion) : customer.Deactivate(expectedRowVersion, reason ?? string.Empty);
        if (!changed) return Result<CustomerProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Customer status cannot change in its current state."));
        var now = _clock.UtcNow;
        _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, customer.Id.ToString(), now));
        _db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), orgId, access.ActorUserId, reactivate ? "customer.activated" : "customer.deactivated", "Customer", customer.Id.ToString(), now, traceId,
            JsonSerializer.Serialize(new { changedFields = reactivate ? new[] { "status", "inactiveReason" } : new[] { "status", "inactiveReason" } })));
        try { await _db.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException) { return Result<CustomerProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Customer version conflict.")); }
        await tx.CommitAsync(cancellationToken);
        return Result<CustomerProjection>.Success(ProjectCustomer(customer));
    }

    private static CustomerProjection ProjectCustomer(Customer customer)
    {
        var contact = customer.Contacts.FirstOrDefault(c => c.IsPrimary) ?? customer.Contacts.FirstOrDefault();
        var contactProjection = contact != null
            ? new CustomerContactProjection(
                contact.Name,
                contact.RoleTitle,
                contact.Phone,
                contact.Email,
                contact.PreferredChannel,
                IsMasked: false,
                contact.LineId)
            : new CustomerContactProjection(string.Empty, null, null, null, ContactChannel.Phone, IsMasked: false, null);

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
            customer.CreatedAtUtc,
            customer.LeadSource,
            customer.LeadSourceNote,
            customer.ImageFileId,
            customer.LegalName,
            customer.TaxIdentifier,
            customer.BranchCode,
            customer.CreditTermDays,
            customer.CreditLimit,
            customer.CurrencyCode,
            customer.BillingCycle,
            customer.BillingDay,
            customer.PaymentConditionNote,
            customer.InactiveReason);

    }
}
