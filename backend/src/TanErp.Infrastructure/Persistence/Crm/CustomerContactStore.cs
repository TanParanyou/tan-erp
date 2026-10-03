using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Crm;

public sealed class CustomerContactStore(AppDbContext db, TanErp.Application.Common.Abstractions.IClock clock) : ICustomerContactStore
{
    private const string CreateOperation = "crm.customer.contact.create";

    public async Task<IReadOnlyList<CustomerContactDetailProjection>?> ListAsync(Guid organizationId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var exists = await db.Customers.AnyAsync(x => x.Id == customerId && x.OrganizationId == organizationId, cancellationToken);
        if (!exists) return null;
        var contacts = await db.CustomerContacts.AsNoTracking().Where(x => x.CustomerId == customerId && x.OrganizationId == organizationId)
            .OrderByDescending(x => x.IsPrimary).ThenBy(x => x.Name).ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
        return contacts.Select(Project).ToList();
    }

    public async Task<Result<CustomerContactDetailProjection>> CreateAsync(RequestAccessContext access, Guid customerId, PrimaryContactInput input, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        var replay = await db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == access.OrganizationId && x.Operation == CreateOperation && x.KeyHash == keyHash, cancellationToken);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<CustomerContactDetailProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has been used with a different payload."));
            if (!Guid.TryParse(replay.ResourceId, out var replayId)) return Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Contact not found."));
            var existing = await db.CustomerContacts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == replayId && x.CustomerId == customerId && x.OrganizationId == access.OrganizationId, cancellationToken);
            return existing is null ? Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Contact not found.")) : Result<CustomerContactDetailProjection>.Success(Project(existing));
        }
        var customer = await db.Customers.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == customerId && x.OrganizationId == access.OrganizationId, cancellationToken);
        if (customer is null) return Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
        if (customer.Status == CustomerStatus.Inactive) return Result<CustomerContactDetailProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Inactive customer contacts cannot be changed."));
        var now = clock.UtcNow;
        var contact = customer.AddContact(Guid.NewGuid(), access.ActorUserId, input, now);
        db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), access.OrganizationId, CreateOperation, keyHash, payloadHash, contact.Id.ToString(), now));
        AddAudit(access, contact.Id, "contact.created", traceId, new[] { "name", "roleTitle", "phone", "email", "lineId", "preferredChannel" }, now);
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return Result<CustomerContactDetailProjection>.Success(Project(contact));
    }

    public Task<Result<CustomerContactDetailProjection>> UpdateAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, PrimaryContactInput input, string traceId, CancellationToken cancellationToken = default) =>
        MutateAsync(access, customerId, contactId, expectedRowVersion, "contact.updated", traceId, customer => customer.UpdateContact(contactId, expectedRowVersion, input), cancellationToken);

    public async Task<Result<CustomerContactDetailProjection>> SetPrimaryAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, string traceId, CancellationToken cancellationToken = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        var customer = await db.Customers.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == customerId && x.OrganizationId == access.OrganizationId, cancellationToken);
        if (customer is null) return Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Contact not found."));
        var target = customer.Contacts.SingleOrDefault(x => x.Id == contactId);
        if (target is null) return Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Contact not found."));
        if (target.RowVersion != expectedRowVersion) return Result<CustomerContactDetailProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Contact version conflict."));
        var prior = customer.Contacts.SingleOrDefault(x => x.IsPrimary && x.Status == ContactStatus.Active);
        if (prior?.Id != target.Id && prior is not null)
        {
            prior.ClearPrimary();
            await db.SaveChangesAsync(cancellationToken);
        }
        if (!customer.SetPrimaryContact(contactId)) return Result<CustomerContactDetailProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Contact cannot be selected as primary."));
        var now = clock.UtcNow;
        AddAudit(access, customer.Id, "contact.updated", traceId, new[] { "isPrimary" }, now);
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return Result<CustomerContactDetailProjection>.Success(Project(target));
    }

    public Task<Result<CustomerContactDetailProjection>> DeactivateAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, string traceId, CancellationToken cancellationToken = default) =>
        MutateAsync(access, customerId, contactId, expectedRowVersion, "contact.deactivated", traceId, customer => customer.DeactivateContact(contactId, expectedRowVersion), cancellationToken);

    private async Task<Result<CustomerContactDetailProjection>> MutateAsync(RequestAccessContext access, Guid customerId, Guid contactId, Guid expectedRowVersion, string auditAction, string traceId, Func<Customer, bool> mutation, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        var customer = await db.Customers.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == customerId && x.OrganizationId == access.OrganizationId, cancellationToken);
        if (customer is null) return Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Contact not found."));
        var contact = customer.Contacts.SingleOrDefault(x => x.Id == contactId);
        if (contact is null) return Result<CustomerContactDetailProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Contact not found."));
        if (contact.RowVersion != expectedRowVersion) return Result<CustomerContactDetailProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Contact version conflict."));
        if (!mutation(customer)) return Result<CustomerContactDetailProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Contact cannot be changed in its current state."));
        var now = clock.UtcNow;
        AddAudit(access, contact.Id, auditAction, traceId, auditAction == "contact.updated" ? new[] { "name", "roleTitle", "phone", "email", "lineId", "preferredChannel" } : new[] { "status" }, now);
        try { await db.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException) { return Result<CustomerContactDetailProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Contact version conflict.")); }
        await tx.CommitAsync(cancellationToken);
        return Result<CustomerContactDetailProjection>.Success(Project(contact));
    }

    private void AddAudit(RequestAccessContext access, Guid resourceId, string action, string traceId, string[] fields, DateTimeOffset now) =>
        db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "CustomerContact", resourceId.ToString(), now, traceId, JsonSerializer.Serialize(new { changedFields = fields })));

    private static CustomerContactDetailProjection Project(CustomerContact contact) =>
        new(contact.Id, contact.CustomerId, contact.Name, contact.RoleTitle, contact.Phone, contact.Email, contact.LineId, contact.PreferredChannel, contact.IsPrimary, contact.Status, contact.RowVersion);
}
