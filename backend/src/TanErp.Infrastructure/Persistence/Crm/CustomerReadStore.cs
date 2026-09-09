using Microsoft.EntityFrameworkCore;
using TanErp.Application.Crm.Customers;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Crm;

public class CustomerReadStore : ICustomerReadStore
{
    private readonly AppDbContext _db;

    public CustomerReadStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CustomerPage> ListAsync(
        Guid organizationId,
        CustomerListFilter filter,
        bool includeContactPii,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Customers
            .AsNoTracking()
            .Include(c => c.Contacts)
            .Where(c => c.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(c => c.Status == filter.Status);
        }

        if (!string.IsNullOrWhiteSpace(filter.CustomerType))
        {
            query = query.Where(c => c.CustomerType == filter.CustomerType);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            var normSearch = CustomerNormalizer.NormalizeName(search);
            var upperCode = search.ToUpperInvariant();

            query = query.Where(c => c.NormalizedDisplayName.Contains(normSearch) || c.Code.Contains(upperCode));
        }

        if (!string.IsNullOrWhiteSpace(filter.Cursor) && CustomerCursor.TryDecode(filter.Cursor, out var cursorData) && cursorData != null)
        {
            query = query.Where(c =>
                string.Compare(c.NormalizedDisplayName, cursorData.Name) > 0 ||
                (c.NormalizedDisplayName == cursorData.Name && c.Id.CompareTo(cursorData.Id) > 0));
        }

        var isDesc = string.Equals(filter.SortOrder, CustomerSortOrder.Desc, StringComparison.OrdinalIgnoreCase);
        var sortBy = filter.SortBy?.ToLowerInvariant();

        query = sortBy switch
        {
            "code" => isDesc
                ? query.OrderByDescending(c => c.Code).ThenBy(c => c.Id)
                : query.OrderBy(c => c.Code).ThenBy(c => c.Id),
            "status" => isDesc
                ? query.OrderByDescending(c => c.Status).ThenBy(c => c.NormalizedDisplayName).ThenBy(c => c.Id)
                : query.OrderBy(c => c.Status).ThenBy(c => c.NormalizedDisplayName).ThenBy(c => c.Id),
            "customertype" => isDesc
                ? query.OrderByDescending(c => c.CustomerType).ThenBy(c => c.NormalizedDisplayName).ThenBy(c => c.Id)
                : query.OrderBy(c => c.CustomerType).ThenBy(c => c.NormalizedDisplayName).ThenBy(c => c.Id),
            "createdat" => isDesc
                ? query.OrderByDescending(c => c.CreatedAtUtc).ThenBy(c => c.Id)
                : query.OrderBy(c => c.CreatedAtUtc).ThenBy(c => c.Id),
            _ => isDesc
                ? query.OrderByDescending(c => c.NormalizedDisplayName).ThenBy(c => c.Id)
                : query.OrderBy(c => c.NormalizedDisplayName).ThenBy(c => c.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var limit = filter.Limit;
        List<Customer> rawItems;
        string? nextCursor = null;
        int currentPage = filter.Page ?? 1;

        if (filter.Page.HasValue)
        {
            var skip = (currentPage - 1) * limit;
            rawItems = await query
                .Skip(skip)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        else
        {
            rawItems = await query
                .Take(limit + 1)
                .ToListAsync(cancellationToken);

            if (rawItems.Count > limit)
            {
                rawItems.RemoveAt(rawItems.Count - 1);
                var last = rawItems[^1];
                nextCursor = CustomerCursor.Encode(last.NormalizedDisplayName, last.Id);
            }
        }

        var projected = rawItems.Select(c => ProjectCustomer(c, includeContactPii)).ToList();
        return new CustomerPage(projected, nextCursor, totalCount, currentPage, limit);
    }

    public async Task<CustomerProjection?> GetAsync(
        Guid organizationId,
        Guid customerId,
        bool includeContactPii,
        CancellationToken cancellationToken = default)
    {
        var customer = await _db.Customers
            .AsNoTracking()
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.OrganizationId == organizationId && c.Id == customerId, cancellationToken);

        return customer != null ? ProjectCustomer(customer, includeContactPii) : null;
    }

    private static CustomerProjection ProjectCustomer(Customer customer, bool includeContactPii)
    {
        var contact = customer.Contacts.FirstOrDefault(c => c.IsPrimary) ?? customer.Contacts.FirstOrDefault();
        var contactProjection = contact != null
            ? new CustomerContactProjection(
                contact.Name,
                contact.RoleTitle,
                includeContactPii ? contact.Phone : CustomerCreationStore.MaskPhone(contact.Phone),
                includeContactPii ? contact.Email : CustomerCreationStore.MaskEmail(contact.Email),
                contact.PreferredChannel,
                IsMasked: !includeContactPii,
                includeContactPii ? contact.LineId : CustomerCreationStore.MaskLineId(contact.LineId))
            : new CustomerContactProjection(string.Empty, null, null, null, ContactChannel.Phone, IsMasked: false, null);

        return new CustomerProjection(
            customer.Id,
            customer.Code,
            customer.CustomerType,
            customer.DisplayNameTh,
            customer.DisplayNameEn,
            customer.PreferredLocale,
            customer.Status,
            contactProjection,
            customer.RowVersion,
            customer.CreatedAtUtc,
            customer.LeadSource);
    }
}
