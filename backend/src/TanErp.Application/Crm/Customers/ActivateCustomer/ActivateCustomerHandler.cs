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

        return await _store.ActivateAsync(
            access,
            command.CustomerId,
            command.ExpectedRowVersion,
            keyHash,
            payloadHash,
            command.TraceId,
            cancellationToken);
    }
}
