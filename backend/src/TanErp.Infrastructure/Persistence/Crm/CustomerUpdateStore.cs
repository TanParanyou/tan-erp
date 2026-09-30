using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Customers.UpdateCustomer;
using TanErp.Domain.Common;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Crm;

public sealed class CustomerUpdateStore(AppDbContext db, TanErp.Application.Common.Abstractions.IClock clock) : ICustomerUpdateStore
{
    private const string Operation = "crm.customer.update";

    public async Task<Result<CustomerProjection>> UpdateAsync(RequestAccessContext access, UpdateCustomerCommand command, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken = default)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
            var replay = await db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == access.OrganizationId && x.Operation == Operation && x.KeyHash == keyHash, cancellationToken);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Result<CustomerProjection>.Failure(new Error("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload."));
                var replayCustomer = await db.Customers.Include(x => x.Contacts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.CustomerId && x.OrganizationId == access.OrganizationId, cancellationToken);
                return replayCustomer is null
                    ? Result<CustomerProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."))
                    : Result<CustomerProjection>.Success(Project(replayCustomer));
            }

            var customer = await db.Customers.Include(x => x.Contacts).FirstOrDefaultAsync(x => x.Id == command.CustomerId && x.OrganizationId == access.OrganizationId, cancellationToken);
            if (customer is null) return Result<CustomerProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Customer not found."));
            if (customer.RowVersion != command.ExpectedRowVersion) return Result<CustomerProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Customer version conflict."));
            if (!customer.UpdateProfile(command.ExpectedRowVersion, command.CustomerType, command.DisplayNameTh, command.DisplayNameEn, command.PreferredLocale, command.LeadSource, command.LeadSourceNote, command.ImageFileId))
                return Result<CustomerProjection>.Failure(new Error("CUSTOMER_INVALID_STATE", "Customer cannot be edited in its current state."));
            try
            {
                customer.UpdateCommercialProfile(command.LegalName, command.HasTaxIdentifier ? command.TaxIdentifier : customer.TaxIdentifier, command.BranchCode,
                    command.HasCreditProfile ? command.CreditTermDays ?? 0 : customer.CreditTermDays,
                    command.HasCreditProfile ? command.CreditLimit : customer.CreditLimit,
                    command.HasCreditProfile ? command.CurrencyCode ?? "THB" : customer.CurrencyCode,
                    command.HasCreditProfile ? command.BillingCycle : customer.BillingCycle,
                    command.HasCreditProfile ? command.BillingDay : customer.BillingDay,
                    command.HasCreditProfile ? command.PaymentConditionNote : customer.PaymentConditionNote);
            }
            catch (ArgumentException)
            {
                return Result<CustomerProjection>.Failure(new Error("CUSTOMER_INVALID_PROFILE", "Customer commercial profile is invalid."));
            }

            var now = clock.UtcNow;
            var record = new IdempotencyRecord(Guid.NewGuid(), access.OrganizationId, Operation, keyHash, payloadHash, customer.Id.ToString(), now);
            db.IdempotencyRecords.Add(record);
            db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), access.OrganizationId, access.ActorUserId, "customer.updated", "Customer", customer.Id.ToString(), now, traceId,
                JsonSerializer.Serialize(new { changedFields = new[] { "customerType", "displayNameTh", "displayNameEn", "preferredLocale", "leadSource", "leadSourceNote", "legalName", "taxIdentifier", "branchCode", "creditTermDays", "creditLimit", "currencyCode", "billingCycle", "billingDay", "paymentConditionNote" } })));
            try
            {
                await db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(cancellationToken);
                return Result<CustomerProjection>.Failure(new Error("CUSTOMER_VERSION_CONFLICT", "Customer version conflict."));
            }
            return Result<CustomerProjection>.Success(Project(customer));
        });
    }

    private static CustomerProjection Project(Customer customer)
    {
        var contact = customer.Contacts.FirstOrDefault(x => x.IsPrimary && x.Status == ContactStatus.Active) ?? customer.Contacts.FirstOrDefault();
        return new CustomerProjection(customer.Id, customer.Code, customer.CustomerType, customer.DisplayNameTh, customer.DisplayNameEn, customer.PreferredLocale, customer.Status,
            contact is null ? new CustomerContactProjection(string.Empty, null, null, null, ContactChannel.Phone, false) :
            new CustomerContactProjection(contact.Name, contact.RoleTitle, contact.Phone, contact.Email, contact.PreferredChannel, false, contact.LineId),
            customer.RowVersion, customer.CreatedAtUtc, customer.LeadSource, customer.LeadSourceNote, customer.ImageFileId, customer.LegalName,
            customer.TaxIdentifier, customer.BranchCode, customer.CreditTermDays, customer.CreditLimit, customer.CurrencyCode, customer.BillingCycle,
            customer.BillingDay, customer.PaymentConditionNote, customer.InactiveReason);
    }
}
