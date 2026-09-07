namespace TanErp.Application.Crm.Customers.ListCustomers;

public sealed record ListCustomersQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? Search = null,
    string? Status = null,
    int Limit = 25,
    string? Cursor = null);
