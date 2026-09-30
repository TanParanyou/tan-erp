namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record CustomerContactDetailResponse(Guid Id, Guid CustomerId, string Name, string? RoleTitle, string? Phone, string? Email, string? LineId, string PreferredChannel, bool IsPrimary, string Status, Guid RowVersion);

public sealed record CustomerContactListResponse(IReadOnlyList<CustomerContactDetailResponse> Items);
