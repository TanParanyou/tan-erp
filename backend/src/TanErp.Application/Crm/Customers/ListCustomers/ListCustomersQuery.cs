namespace TanErp.Application.Crm.Customers.ListCustomers;

public sealed record ListCustomersQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? Search = null,
    string? Status = null,
    string? CustomerType = null,
    string? SortBy = null,
    string? SortOrder = null,
    int? Page = null,
    int Limit = 25,
    string? Cursor = null);
