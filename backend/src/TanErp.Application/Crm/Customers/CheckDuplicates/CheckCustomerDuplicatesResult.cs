namespace TanErp.Application.Crm.Customers.CheckDuplicates;

public sealed record CheckCustomerDuplicatesResult(
    IReadOnlyList<DuplicateCustomerProjection> Candidates);
