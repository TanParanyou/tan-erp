namespace TanErp.Application.Crm.Customers.ListCustomers;

public sealed record ListCustomersResult(
    IReadOnlyList<CustomerProjection> Items,
    string? NextCursor);
