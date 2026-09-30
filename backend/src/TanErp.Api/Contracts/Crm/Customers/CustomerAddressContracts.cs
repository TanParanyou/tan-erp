using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record CustomerAddressResponse(Guid Id, Guid CustomerId, string AddressType, string Label, string AddressLine1, string Subdistrict, string District, string Province, string PostalCode, string CountryCode, string Status, bool IsPrimary, Guid RowVersion);
public sealed record CustomerAddressListResponse(IReadOnlyList<CustomerAddressResponse> Items);
public sealed record CreateCustomerAddressRequest([Required] string AddressType, [Required] string Label, [Required] string AddressLine1, [Required] string Subdistrict, [Required] string District, [Required] string Province, [Required] string PostalCode, [Required] string CountryCode, bool IsPrimary);
public sealed record UpdateCustomerAddressRequest([Required] string Label, [Required] string AddressLine1, [Required] string Subdistrict, [Required] string District, [Required] string Province, [Required] string PostalCode, [Required] string CountryCode);
