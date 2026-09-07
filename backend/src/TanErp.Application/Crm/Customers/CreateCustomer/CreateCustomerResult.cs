namespace TanErp.Application.Crm.Customers.CreateCustomer;

public sealed record CreateCustomerResult(
    CustomerProjection Customer,
    IReadOnlyList<DuplicateCustomerProjection> DuplicateCandidates,
    bool WasReplayed);
