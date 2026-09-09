namespace TanErp.Application.Crm.Customers.ActivateCustomer;

public sealed record ActivateCustomerCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid CustomerId,
    Guid ExpectedRowVersion,
    string IdempotencyKey,
    string TraceId);
