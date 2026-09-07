using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Customers.ListCustomers;

public class ListCustomersHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ICustomerReadStore _store;

    public ListCustomersHandler(
        IRequestAccessResolver accessResolver,
        ICustomerReadStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<ListCustomersResult>> Handle(
        ListCustomersQuery query,
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
            return Result<ListCustomersResult>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Validate cursor if provided
        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            if (!CustomerCursor.TryDecode(query.Cursor, out _))
            {
                return Result<ListCustomersResult>.Failure(
                    new Error("CUSTOMER_CURSOR_INVALID", "Invalid customer list pagination cursor."));
            }
        }

        // 3. Resolve optional customer-contacts.manage permission for PII visibility
        var manageContactResult = await _accessResolver.ResolveAsync(
            query.FirebaseUid,
            query.MembershipId,
            "customer-contacts.manage",
            cancellationToken);

        var includeContactPii = manageContactResult.IsSuccess;

        // 4. Validate limit
        var limit = query.Limit;
        if (limit < 1) limit = 25;
        if (limit > 100) limit = 100;

        var filter = new CustomerListFilter(
            string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            string.IsNullOrWhiteSpace(query.Status) ? null : query.Status.Trim(),
            limit,
            query.Cursor);

        var page = await _store.ListAsync(access.OrganizationId, filter, includeContactPii, cancellationToken);
        return Result<ListCustomersResult>.Success(new ListCustomersResult(page.Items, page.NextCursor));
    }
}
