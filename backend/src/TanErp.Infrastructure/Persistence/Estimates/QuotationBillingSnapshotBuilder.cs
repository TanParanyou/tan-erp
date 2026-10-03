using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Estimates;

internal sealed record QuotationBillingSnapshot(string Json, string Hash);

/// <summary>
/// Freezes the customer's billing identity at issue time. Shared by issuing and amending so both documents
/// carry exactly the same snapshot shape and the same readiness rules.
/// </summary>
internal static class QuotationBillingSnapshotBuilder
{
    public static async Task<Result<QuotationBillingSnapshot>> BuildAsync(AppDbContext db, Guid organizationId, Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(
            candidate => candidate.OrganizationId == organizationId && candidate.Id == customerId,
            cancellationToken);
        if (customer is null || customer.Status != CustomerStatus.Active)
            return Result<QuotationBillingSnapshot>.Failure(new Error("CUSTOMER_QUOTATION_BILLING_NOT_READY", "Customer must be active before issuing a quotation."));

        var billingAddress = await db.CustomerAddresses.AsNoTracking().FirstOrDefaultAsync(
            address => address.OrganizationId == organizationId && address.CustomerId == customer.Id &&
                address.AddressType == "billing" && address.Status == "active" && address.IsPrimary,
            cancellationToken);
        if (billingAddress is null || (customer.CustomerType == "organization" &&
                (string.IsNullOrWhiteSpace(customer.LegalName) || string.IsNullOrWhiteSpace(customer.TaxIdentifier) || string.IsNullOrWhiteSpace(customer.BranchCode))))
            return Result<QuotationBillingSnapshot>.Failure(new Error("CUSTOMER_QUOTATION_BILLING_NOT_READY", "Customer tax and billing details are incomplete."));

        var json = JsonSerializer.Serialize(new
        {
            customerId = customer.Id,
            customerType = customer.CustomerType,
            displayNameTh = customer.DisplayNameTh,
            displayNameEn = customer.DisplayNameEn,
            legalName = customer.LegalName,
            taxIdentifier = customer.TaxIdentifier,
            branchCode = customer.BranchCode,
            address = new
            {
                billingAddress.Label,
                billingAddress.AddressLine1,
                billingAddress.Subdistrict,
                billingAddress.District,
                billingAddress.Province,
                billingAddress.PostalCode,
                billingAddress.CountryCode
            }
        });
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
        return Result<QuotationBillingSnapshot>.Success(new QuotationBillingSnapshot(json, hash));
    }
}
