namespace TanErp.Application.Crm.Customers.GetCustomer;

public sealed record GetCustomerQuery(
    string FirebaseUid,
    Guid MembershipId,
    Guid CustomerId);
