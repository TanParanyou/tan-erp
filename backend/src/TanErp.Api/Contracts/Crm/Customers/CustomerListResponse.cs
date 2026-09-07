namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record CustomerListItemResponse(
    Guid Id,
    string Code,
    string CustomerType,
    string DisplayNameTh,
    string? DisplayNameEn,
    string PreferredLocale,
    string Status,
    CustomerContactResponse PrimaryContact);

public sealed record CustomerListResponse(
    IReadOnlyList<CustomerListItemResponse> Items,
    string? NextCursor);
