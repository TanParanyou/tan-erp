namespace TanErp.Application.Crm.Customers.CreateCustomer;

public sealed record CreatePrimaryContact(
    string Name,
    string? RoleTitle,
    string? Phone,
    string? Email,
    string PreferredChannel,
    string? LineId = null);

public sealed record CreateCustomerCommand(
    string FirebaseUid,
    Guid MembershipId,
    string IdempotencyKey,
    string CustomerType,
    string DisplayNameTh,
    string? DisplayNameEn,
    string PreferredLocale,
    CreatePrimaryContact PrimaryContact,
    string TraceId,
    string? LeadSource = null);
