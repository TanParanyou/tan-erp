namespace TanErp.Application.Crm.Customers.ChangeCustomerStatus;

public sealed record ChangeCustomerStatusCommand(string FirebaseUid, Guid MembershipId, Guid CustomerId, Guid ExpectedRowVersion, string IdempotencyKey, string? Reason, bool Reactivate, string TraceId);
