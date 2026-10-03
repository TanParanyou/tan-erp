using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Crm.Customers.ActivateCustomer;

public class ActivateCustomerHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICustomerLifecycleStore _store;

    public ActivateCustomerHandler(
        IRequestAccessResolver accessResolver,
        ICustomerLifecycleStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<CustomerProjection>> Handle(
        ActivateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "customers.activate",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<CustomerProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = $"{command.CustomerId:D}|{command.ExpectedRowVersion:D}|activate";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        var result = await _store.ActivateAsync(
            access,
            command.CustomerId,
            command.ExpectedRowVersion,
            keyHash,
            payloadHash,
            command.TraceId,
            cancellationToken);
        if (result.IsFailure) return result;
        var creditRead = await _accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customers.credit.read", cancellationToken);
        var piiRead = await _accessResolver.ResolveAsync(command.FirebaseUid, command.MembershipId, "customer-contacts.manage", cancellationToken);
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
