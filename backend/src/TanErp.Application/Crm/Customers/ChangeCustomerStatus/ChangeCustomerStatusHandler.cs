using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Crm.Customers.ChangeCustomerStatus;

public sealed class ChangeCustomerStatusHandler(IRequestAccessResolver accessResolver, ICustomerLifecycleStore store)
{
    public async Task<Result<CustomerProjection>> Handle(ChangeCustomerStatusCommand command, CancellationToken ct = default)
    {
        var permission = command.Reactivate ? "customers.activate" : "customers.deactivate";
        var access = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, permission, ct);
        if (access.IsFailure) return Result<CustomerProjection>.Failure(access.Error);
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var payloadHash = Sha256Hex.Compute($"{command.CustomerId:D}|{command.ExpectedRowVersion:D}|{command.Reason}|{command.Reactivate}");
        var result = command.Reactivate
            ? await store.ReactivateAsync(access.Value!, command.CustomerId, command.ExpectedRowVersion, keyHash, payloadHash, command.TraceId, ct)
            : await store.DeactivateAsync(access.Value!, command.CustomerId, command.ExpectedRowVersion, command.Reason ?? string.Empty, keyHash, payloadHash, command.TraceId, ct);
        if (result.IsFailure) return result;
        var creditRead = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customers.credit.read", ct);
        var piiRead = await accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customer-contacts.manage", ct);
        return Result<CustomerProjection>.Success(result.Value! with
        {
            CreditTermDays = creditRead.IsSuccess ? result.Value.CreditTermDays : null,
            CreditLimit = creditRead.IsSuccess ? result.Value.CreditLimit : null,
            BillingCycle = creditRead.IsSuccess ? result.Value.BillingCycle : null,
            BillingDay = creditRead.IsSuccess ? result.Value.BillingDay : null,
            PaymentConditionNote = creditRead.IsSuccess ? result.Value.PaymentConditionNote : null,
            TaxIdentifier = piiRead.IsSuccess ? result.Value.TaxIdentifier : null
        });
    }
}
