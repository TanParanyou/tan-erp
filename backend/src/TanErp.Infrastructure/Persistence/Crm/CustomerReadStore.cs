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

        var limit = filter.Limit;
        var rawItems = await query
            .OrderBy(c => c.NormalizedDisplayName)
            .ThenBy(c => c.Id)
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        string? nextCursor = null;
        if (rawItems.Count > limit)
        {
            rawItems.RemoveAt(rawItems.Count - 1);
            var last = rawItems[^1];
            nextCursor = CustomerCursor.Encode(last.NormalizedDisplayName, last.Id);
        }

        var projected = rawItems.Select(c => ProjectCustomer(c, includeContactPii)).ToList();
        return new CustomerPage(projected, nextCursor);
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
                IsMasked: !includeContactPii)
            : new CustomerContactProjection(string.Empty, null, null, null, ContactChannel.Phone, IsMasked: false);

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
            customer.CreatedAtUtc);
    }
}
