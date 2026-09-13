using TanErp.Api.Contracts.Common;

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
    PaginationMetadataResponse Pagination);
