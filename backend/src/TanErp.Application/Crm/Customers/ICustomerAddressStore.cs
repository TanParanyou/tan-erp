using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Customers;

public interface ICustomerAddressStore
{
    Task<IReadOnlyList<CustomerAddressProjection>?> ListAsync(Guid organizationId, Guid customerId, CancellationToken ct = default);
    Task<Result<CustomerAddressProjection>> CreateAsync(RequestAccessContext access, Guid customerId, CreateCustomerAddressData data, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<Result<CustomerAddressProjection>> UpdateAsync(RequestAccessContext access, Guid customerId, Guid addressId, Guid expectedVersion, UpdateCustomerAddressData data, string traceId, CancellationToken ct = default);
    Task<Result<CustomerAddressProjection>> SetPrimaryAsync(RequestAccessContext access, Guid customerId, Guid addressId, Guid expectedVersion, string traceId, CancellationToken ct = default);
    Task<Result<CustomerAddressProjection>> DeactivateAsync(RequestAccessContext access, Guid customerId, Guid addressId, Guid expectedVersion, string traceId, CancellationToken ct = default);
}

public sealed record CreateCustomerAddressData(string AddressType, string Label, string AddressLine1, string Subdistrict, string District, string Province, string PostalCode, string CountryCode, bool IsPrimary);
public sealed record UpdateCustomerAddressData(string Label, string AddressLine1, string Subdistrict, string District, string Province, string PostalCode, string CountryCode);
