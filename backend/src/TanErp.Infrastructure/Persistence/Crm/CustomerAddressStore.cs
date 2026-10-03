using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Crm;

public sealed class CustomerAddressStore(AppDbContext db, TanErp.Application.Common.Abstractions.IClock clock) : ICustomerAddressStore
{
    private const string CreateOperation = "crm.customer.address.create";

    public async Task<IReadOnlyList<CustomerAddressProjection>?> ListAsync(Guid organizationId, Guid customerId, CancellationToken ct = default)
    {
        if (!await db.Customers.AnyAsync(x => x.Id == customerId && x.OrganizationId == organizationId, ct)) return null;
        var rows = await db.CustomerAddresses.AsNoTracking().Where(x => x.CustomerId == customerId && x.OrganizationId == organizationId)
            .OrderByDescending(x => x.IsPrimary).ThenBy(x => x.AddressType).ThenBy(x => x.Label).ThenBy(x => x.Id).ToListAsync(ct);
        return rows.Select(Project).ToList();
    }

    public async Task<Result<CustomerAddressProjection>> CreateAsync(RequestAccessContext access, Guid customerId, CreateCustomerAddressData data, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var replay = await db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == access.OrganizationId && x.Operation == CreateOperation && x.KeyHash == keyHash, ct);
        if (replay is not null)
        {
            if (replay.PayloadHash != payloadHash) return Result<CustomerAddressProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has been used with a different payload."));
            if (!Guid.TryParse(replay.ResourceId, out var replayId)) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Address not found."));
            var existing = await db.CustomerAddresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == replayId && x.OrganizationId == access.OrganizationId, ct);
            return existing is null ? Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Address not found.")) : Result<CustomerAddressProjection>.Success(Project(existing));
        }
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == customerId && x.OrganizationId == access.OrganizationId, ct);
        if (customer is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
        var now = clock.UtcNow;
        if (data.IsPrimary)
        {
            var prior = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.OrganizationId == access.OrganizationId && x.AddressType == data.AddressType && x.Status == "active" && x.IsPrimary, ct);
            if (prior is not null) { prior.SetPrimary(false); await db.SaveChangesAsync(ct); }
        }
        var address = new CustomerAddress(Guid.NewGuid(), customerId, access.OrganizationId, access.ActorUserId, data.AddressType, data.Label, data.AddressLine1, data.Subdistrict, data.District, data.Province, data.PostalCode, data.CountryCode, data.IsPrimary, now);
        customer.AdvanceVersion();
        db.CustomerAddresses.Add(address);
        db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), access.OrganizationId, CreateOperation, keyHash, payloadHash, address.Id.ToString(), now));
        Audit(access, address.Id, "address.created", traceId, new[] { "addressType", "label", "addressLine1", "subdistrict", "district", "province", "postalCode", "countryCode", "isPrimary" }, now);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return Result<CustomerAddressProjection>.Success(Project(address));
    }

    public async Task<Result<CustomerAddressProjection>> UpdateAsync(RequestAccessContext access, Guid customerId, Guid addressId, Guid expectedVersion, UpdateCustomerAddressData data, string traceId, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var address = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.Id == addressId && x.CustomerId == customerId && x.OrganizationId == access.OrganizationId, ct);
        if (address is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Address not found."));
        if (address.RowVersion != expectedVersion) return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Address version conflict."));
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == address.CustomerId && x.OrganizationId == access.OrganizationId, ct);
        if (customer is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
        if (!address.Update(expectedVersion, data.Label, data.AddressLine1, data.Subdistrict, data.District, data.Province, data.PostalCode, data.CountryCode)) return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Address cannot be edited in its current state."));
        customer.AdvanceVersion(); var now = clock.UtcNow;
        Audit(access, address.Id, "address.updated", traceId, new[] { "label", "addressLine1", "subdistrict", "district", "province", "postalCode", "countryCode" }, now);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Address version conflict.")); }
        await tx.CommitAsync(ct); return Result<CustomerAddressProjection>.Success(Project(address));
    }

    public async Task<Result<CustomerAddressProjection>> SetPrimaryAsync(RequestAccessContext access, Guid customerId, Guid addressId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var address = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.Id == addressId && x.CustomerId == customerId && x.OrganizationId == access.OrganizationId, ct);
        if (address is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Address not found."));
        if (address.RowVersion != expectedVersion) return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Address version conflict."));
        if (address.Status != "active") return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Inactive address cannot be selected."));
        var prior = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.CustomerId == address.CustomerId && x.OrganizationId == access.OrganizationId && x.AddressType == address.AddressType && x.Status == "active" && x.IsPrimary && x.Id != address.Id, ct);
        if (prior is not null) { prior.SetPrimary(false); await db.SaveChangesAsync(ct); }
        address.SetPrimary(true);
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == address.CustomerId && x.OrganizationId == access.OrganizationId, ct);
        if (customer is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
        customer.AdvanceVersion(); var now = clock.UtcNow;
        Audit(access, address.Id, "address.updated", traceId, new[] { "isPrimary" }, now);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return Result<CustomerAddressProjection>.Success(Project(address));
    }

    public async Task<Result<CustomerAddressProjection>> DeactivateAsync(RequestAccessContext access, Guid customerId, Guid addressId, Guid expectedVersion, string traceId, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var address = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.Id == addressId && x.CustomerId == customerId && x.OrganizationId == access.OrganizationId, ct);
        if (address is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Address not found."));
        if (address.RowVersion != expectedVersion) return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Address version conflict."));
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == address.CustomerId && x.OrganizationId == access.OrganizationId, ct);
        if (customer is null) return Result<CustomerAddressProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
        address.Deactivate(); customer.AdvanceVersion(); var now = clock.UtcNow;
        Audit(access, address.Id, "address.deactivated", traceId, new[] { "status", "isPrimary" }, now);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Address version conflict.")); }
        await tx.CommitAsync(ct); return Result<CustomerAddressProjection>.Success(Project(address));
    }

    private void Audit(RequestAccessContext access, Guid id, string action, string traceId, string[] fields, DateTimeOffset now) =>
        db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "CustomerAddress", id.ToString(), now, traceId, JsonSerializer.Serialize(new { changedFields = fields })));

    private static CustomerAddressProjection Project(CustomerAddress address) => new(address.Id, address.CustomerId, address.AddressType, address.Label, address.AddressLine1, address.Subdistrict, address.District, address.Province, address.PostalCode, address.CountryCode, address.Status, address.IsPrimary, address.RowVersion);
}
