namespace TanErp.Application.Crm.Customers;

public sealed record CustomerAddressProjection(Guid Id, Guid CustomerId, string AddressType, string Label, string AddressLine1, string Subdistrict, string District, string Province, string PostalCode, string CountryCode, string Status, bool IsPrimary, Guid RowVersion);
