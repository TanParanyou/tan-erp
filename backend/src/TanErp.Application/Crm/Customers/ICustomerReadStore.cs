namespace TanErp.Application.Crm.Customers;

public sealed record CustomerListFilter(
    string? Search,
    string? Status,
    int Limit = 25,
    string? Cursor = null);

public sealed record CustomerPage(
    IReadOnlyList<CustomerProjection> Items,
    string? NextCursor);

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
}
