using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record DeactivateCustomerRequest([Required, MinLength(3), MaxLength(500)] string Reason);
