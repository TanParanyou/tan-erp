using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Customers.GetCustomer;

public class GetCustomerHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICustomerReadStore _store;

    public GetCustomerHandler(
        IRequestAccessResolver accessResolver,
        ICustomerReadStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<GetCustomerResult>> Handle(
        GetCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required customers.read permission
        var accessResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "customers.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<GetCustomerResult>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Resolve optional customer-contacts.manage permission for PII visibility
        var manageContactResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "customer-contacts.manage",
            cancellationToken);

        var includeContactPii = manageContactResult.IsSuccess;

        // 3. Query Customer
        var customer = await _store.GetAsync(access.OrganizationId, query.CustomerId, includeContactPii, cancellationToken);
        if (customer == null)
        {
            return Result<GetCustomerResult>.Failure(
                new Error("RESOURCE_NOT_FOUND", "Customer was not found in the selected organization."));
        }

        return Result<GetCustomerResult>.Success(new GetCustomerResult(customer));
    }
}
