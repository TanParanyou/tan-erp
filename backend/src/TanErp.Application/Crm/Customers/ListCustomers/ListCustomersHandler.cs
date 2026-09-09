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

        // 4. Validate customerType if provided
        string? customerType = null;
        if (!string.IsNullOrWhiteSpace(query.CustomerType))
        {
            var trimmedType = query.CustomerType.Trim();
            if (!TanErp.Domain.Crm.Customers.CustomerType.IsValid(trimmedType))
            {
                return Result<ListCustomersResult>.Failure(
                    new Error("CUSTOMER_TYPE_INVALID", "Invalid customer type filter."));
            }
            customerType = trimmedType;
        }

        // 5. Validate sortBy and sortOrder
        string? sortBy = null;
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            var trimmedSortBy = query.SortBy.Trim();
            if (!TanErp.Domain.Crm.Customers.CustomerSortKey.IsValid(trimmedSortBy))
            {
                return Result<ListCustomersResult>.Failure(
                    new Error("CUSTOMER_SORT_INVALID", "Invalid customer sort key."));
            }
            sortBy = trimmedSortBy;
        }

        string? sortOrder = null;
        if (!string.IsNullOrWhiteSpace(query.SortOrder))
        {
            var trimmedOrder = query.SortOrder.Trim();
            if (!TanErp.Domain.Crm.Customers.CustomerSortOrder.IsValid(trimmedOrder))
            {
                return Result<ListCustomersResult>.Failure(
                    new Error("CUSTOMER_SORT_ORDER_INVALID", "Invalid customer sort order."));
            }
            sortOrder = trimmedOrder.ToLowerInvariant();
        }

        // 6. Validate page and limit
        int? pageNumber = query.Page;
        if (pageNumber.HasValue && pageNumber.Value < 1)
        {
            pageNumber = 1;
        }

        var limit = query.Limit;
        if (limit < 1) limit = 25;
        if (limit > 100) limit = 100;

        var filter = new CustomerListFilter(
            string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            string.IsNullOrWhiteSpace(query.Status) ? null : query.Status.Trim(),
            customerType,
            sortBy,
            sortOrder,
            pageNumber,
            limit,
            query.Cursor);

        var page = await _store.ListAsync(access.OrganizationId, filter, includeContactPii, cancellationToken);
        return Result<ListCustomersResult>.Success(new ListCustomersResult(page.Items, page.NextCursor, page.TotalCount, page.Page, page.PageSize));
    }
}
