namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record CustomerListItemResponse(
    Guid Id,
    string Code,
    string CustomerType,
    string DisplayNameTh,
    string? DisplayNameEn,
    string PreferredLocale,
    string Status,
    CustomerContactResponse PrimaryContact,
    string? LeadSource = null);

public sealed record CustomerListResponse(
    IReadOnlyList<CustomerListItemResponse> Items,
    string? NextCursor,
    int TotalCount = 0,
    int Page = 1,
    int PageSize = 25,
    int TotalPages = 1);
