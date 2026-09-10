namespace TanErp.Application.Crm.Customers.CheckDuplicates;

public sealed record CheckCustomerDuplicatesQuery(
    string FirebaseUid,
    Guid MembershipId,
    string? Name,
    string? Phone,
    string? Email);
