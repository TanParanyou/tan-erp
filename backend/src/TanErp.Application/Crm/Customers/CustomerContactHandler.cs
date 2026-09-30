using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Application.Crm.Customers;

public sealed class CustomerContactHandler(IRequestAccessResolver accessResolver, ICustomerContactStore store)
{
    public async Task<Result<IReadOnlyList<CustomerContactDetailProjection>>> ListAsync(string firebaseUid, Guid membershipId, Guid customerId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(firebaseUid, membershipId, "customer-contacts.manage", ct);
        if (access.IsFailure) return Result<IReadOnlyList<CustomerContactDetailProjection>>.Failure(access.Error);
        var contacts = await store.ListAsync(access.Value!.OrganizationId, customerId, ct);
        return contacts is null ? Result<IReadOnlyList<CustomerContactDetailProjection>>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found.")) : Result<IReadOnlyList<CustomerContactDetailProjection>>.Success(contacts);
    }

    public async Task<Result<CustomerContactDetailProjection>> CreateAsync(string uid, Guid membershipId, Guid customerId, PrimaryContactInput input, string key, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        if (access.IsFailure) return Result<CustomerContactDetailProjection>.Failure(access.Error);
        var payload = Sha256Hex.Compute($"{customerId:D}|{input.Name}|{input.RoleTitle}|{input.Phone}|{input.Email}|{input.LineId}|{input.PreferredChannel}");
        return await store.CreateAsync(access.Value!, customerId, input, Sha256Hex.Compute(key), payload, traceId, ct);
    }

    public async Task<Result<CustomerContactDetailProjection>> UpdateAsync(string uid, Guid membershipId, Guid customerId, Guid contactId, Guid rowVersion, PrimaryContactInput input, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        return access.IsFailure ? Result<CustomerContactDetailProjection>.Failure(access.Error) : await store.UpdateAsync(access.Value!, customerId, contactId, rowVersion, input, traceId, ct);
    }

    public async Task<Result<CustomerContactDetailProjection>> SetPrimaryAsync(string uid, Guid membershipId, Guid customerId, Guid contactId, Guid rowVersion, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        return access.IsFailure ? Result<CustomerContactDetailProjection>.Failure(access.Error) : await store.SetPrimaryAsync(access.Value!, customerId, contactId, rowVersion, traceId, ct);
    }

    public async Task<Result<CustomerContactDetailProjection>> DeactivateAsync(string uid, Guid membershipId, Guid customerId, Guid contactId, Guid rowVersion, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        return access.IsFailure ? Result<CustomerContactDetailProjection>.Failure(access.Error) : await store.DeactivateAsync(access.Value!, customerId, contactId, rowVersion, traceId, ct);
    }
}
