using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Common;

namespace TanErp.Application.Crm.Customers;

public sealed class CustomerAddressHandler(IRequestAccessResolver accessResolver, ICustomerAddressStore store)
{
    public async Task<Result<IReadOnlyList<CustomerAddressProjection>>> ListAsync(string uid, Guid membershipId, Guid customerId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        if (access.IsFailure) return Result<IReadOnlyList<CustomerAddressProjection>>.Failure(access.Error);
        var list = await store.ListAsync(access.Value!.OrganizationId, customerId, ct);
        return list is null ? Result<IReadOnlyList<CustomerAddressProjection>>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found.")) : Result<IReadOnlyList<CustomerAddressProjection>>.Success(list);
    }

    public async Task<Result<CustomerAddressProjection>> CreateAsync(string uid, Guid membershipId, Guid customerId, CreateCustomerAddressData data, string key, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        if (access.IsFailure) return Result<CustomerAddressProjection>.Failure(access.Error);
        if (data.AddressType is not ("billing" or "contact") || !IsValidAddress(data.Label, data.AddressLine1, data.Subdistrict, data.District, data.Province, data.PostalCode, data.CountryCode))
            return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_ADDRESS_INVALID", "Customer address details are invalid."));
        var keyHash = Sha256Hex.Compute(key);
        var payloadHash = Sha256Hex.Compute($"{customerId:D}|{data.AddressType}|{data.Label}|{data.AddressLine1}|{data.Subdistrict}|{data.District}|{data.Province}|{data.PostalCode}|{data.CountryCode}|{data.IsPrimary}");
        return await store.CreateAsync(access.Value!, customerId, data, keyHash, payloadHash, traceId, ct);
    }

    public async Task<Result<CustomerAddressProjection>> UpdateAsync(string uid, Guid membershipId, Guid customerId, Guid addressId, Guid version, UpdateCustomerAddressData data, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        if (access.IsFailure) return Result<CustomerAddressProjection>.Failure(access.Error);
        if (!IsValidAddress(data.Label, data.AddressLine1, data.Subdistrict, data.District, data.Province, data.PostalCode, data.CountryCode))
            return Result<CustomerAddressProjection>.Failure(new Error("CUSTOMER_ADDRESS_INVALID", "Customer address details are invalid."));
        return await store.UpdateAsync(access.Value!, customerId, addressId, version, data, traceId, ct);
    }

    public async Task<Result<CustomerAddressProjection>> SetPrimaryAsync(string uid, Guid membershipId, Guid customerId, Guid addressId, Guid version, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        return access.IsFailure ? Result<CustomerAddressProjection>.Failure(access.Error) : await store.SetPrimaryAsync(access.Value!, customerId, addressId, version, traceId, ct);
    }

    public async Task<Result<CustomerAddressProjection>> DeactivateAsync(string uid, Guid membershipId, Guid customerId, Guid addressId, Guid version, string traceId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(uid, membershipId, "customer-contacts.manage", ct);
        return access.IsFailure ? Result<CustomerAddressProjection>.Failure(access.Error) : await store.DeactivateAsync(access.Value!, customerId, addressId, version, traceId, ct);
    }

    private static bool IsValidAddress(string label, string addressLine1, string subdistrict, string district, string province, string postalCode, string countryCode) =>
        !string.IsNullOrWhiteSpace(label) && label.Length <= 100 && !string.IsNullOrWhiteSpace(addressLine1) && addressLine1.Length <= 250 &&
        AddressLocationValidator.IsValid(subdistrict, district, province, postalCode, countryCode);
}
