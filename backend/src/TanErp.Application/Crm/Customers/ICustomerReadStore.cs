namespace TanErp.Application.Crm.Customers;

public sealed record CustomerListFilter(
    string? Search,
    string? Status,
    string? CustomerType = null,
    string? SortBy = null,
    string? SortOrder = null,
    int? Page = null,
    int Limit = 25,
    string? Cursor = null);

public sealed record CustomerPage(
    IReadOnlyList<CustomerProjection> Items,
    string? NextCursor,
    int TotalCount,
    int Page,
    int PageSize);

public interface ICustomerReadStore
{
    Task<CustomerPage> ListAsync(
        Guid organizationId,
        CustomerListFilter filter,
        bool includeContactPii,
        CancellationToken cancellationToken = default);

    Task<CustomerProjection?> GetAsync(
        Guid organizationId,
        Guid customerId,
        bool includeContactPii,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DuplicateCustomerProjection>> FindDuplicatesAsync(
        Guid organizationId,
        string? normalizedName,
        string? normalizedPhone,
        string? normalizedEmail,
        bool includeContactPii,
        CancellationToken cancellationToken = default);
}
