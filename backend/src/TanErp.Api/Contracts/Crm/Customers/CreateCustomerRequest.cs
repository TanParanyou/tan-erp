using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record CreatePrimaryContactRequest(
    [Required] string Name,
    string? RoleTitle,
    string? Phone,
    string? Email,
    [Required] string PreferredChannel);

public sealed record CreateCustomerRequest(
    [Required] string CustomerType,
    [Required] string DisplayNameTh,
    string? DisplayNameEn,
    [Required] string PreferredLocale,
    [Required] CreatePrimaryContactRequest PrimaryContact);
