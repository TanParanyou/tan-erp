using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Crm.Customers;

public sealed record UpdateCustomerRequest(
    [Required] string CustomerType,
    [Required] string DisplayNameTh,
    string? DisplayNameEn,
    [Required] string PreferredLocale,
    string? LeadSource,
    string? LeadSourceNote,
    string? LegalName = null,
    string? TaxIdentifier = null,
    string? BranchCode = null,
    int? CreditTermDays = null,
    decimal? CreditLimit = null,
    string? CurrencyCode = null,
    string? BillingCycle = null,
    int? BillingDay = null,
    string? PaymentConditionNote = null,
    bool HasTaxIdentifier = false);
