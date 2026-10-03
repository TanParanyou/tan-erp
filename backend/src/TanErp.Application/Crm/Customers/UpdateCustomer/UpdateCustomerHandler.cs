using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Crm.Customers.UpdateCustomer;

public sealed class UpdateCustomerHandler(IRequestAccessResolver accessResolver, ICustomerUpdateStore store)
{
    public async Task<Result<CustomerProjection>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var accessResult = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customers.update", cancellationToken);
        if (accessResult.IsFailure) return Result<CustomerProjection>.Failure(accessResult.Error);
        var access = accessResult.Value!;
        if (command.HasCreditProfile)
        {
            var creditAccess = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customers.credit.manage", cancellationToken);
            if (creditAccess.IsFailure) return Result<CustomerProjection>.Failure(creditAccess.Error);
        }
        if (command.HasTaxIdentifier)
        {
            var piiAccess = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customer-contacts.manage", cancellationToken);
            if (piiAccess.IsFailure) return Result<CustomerProjection>.Failure(piiAccess.Error);
        }
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{command.CustomerId:D}|{command.ExpectedRowVersion:D}|{command.CustomerType}|{command.DisplayNameTh}|{command.DisplayNameEn}|{command.PreferredLocale}|{command.LeadSource}|{command.LeadSourceNote}|{command.ImageFileId}|{command.LegalName}|{command.TaxIdentifier}|{command.HasTaxIdentifier}|{command.BranchCode}|{command.HasCreditProfile}|{command.CreditTermDays}|{command.CreditLimit}|{command.CurrencyCode}|{command.BillingCycle}|{command.BillingDay}|{command.PaymentConditionNote}");
        var result = await store.UpdateAsync(access, command, keyHash, payloadHash, command.TraceId, cancellationToken);
        if (result.IsFailure) return result;
        var creditReadAccess = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customers.credit.read", cancellationToken);
        var piiReadAccess = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customer-contacts.manage", cancellationToken);
        var safeProjection = result.Value! with
        {
            CreditTermDays = creditReadAccess.IsSuccess ? result.Value.CreditTermDays : null,
            CreditLimit = creditReadAccess.IsSuccess ? result.Value.CreditLimit : null,
            BillingCycle = creditReadAccess.IsSuccess ? result.Value.BillingCycle : null,
            BillingDay = creditReadAccess.IsSuccess ? result.Value.BillingDay : null,
            PaymentConditionNote = creditReadAccess.IsSuccess ? result.Value.PaymentConditionNote : null,
            TaxIdentifier = piiReadAccess.IsSuccess ? result.Value.TaxIdentifier : null
        };
        return Result<CustomerProjection>.Success(safeProjection);
    }
}
