using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Customers;

public class Customer : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string CustomerType { get; private set; } = string.Empty;
    public string DisplayNameTh { get; private set; } = string.Empty;
    public string? DisplayNameEn { get; private set; }
    public string NormalizedDisplayName { get; private set; } = string.Empty;
    public string Status { get; private set; } = CustomerStatus.Draft;
    public string PreferredLocale { get; private set; } = TanErp.Domain.Crm.Customers.PreferredLocale.Thai;
    public string? LeadSource { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private readonly List<CustomerContact> _contacts = new();
    public IReadOnlyCollection<CustomerContact> Contacts => _contacts.AsReadOnly();

    protected Customer() { }

    private Customer(
        Guid id,
        Guid organizationId,
        Guid createdByUserId,
        string customerType,
        string displayNameTh,
        string? displayNameEn,
        string preferredLocale,
        string? leadSource,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(displayNameTh))
        {
            throw new ArgumentException("Customer Thai display name cannot be blank.", nameof(displayNameTh));
        }

        if (!Customers.CustomerType.IsValid(customerType))
        {
            throw new ArgumentException($"Invalid customer type: '{customerType}'.", nameof(customerType));
        }

        if (!Customers.PreferredLocale.IsValid(preferredLocale))
        {
            throw new ArgumentException($"Invalid preferred locale: '{preferredLocale}'.", nameof(preferredLocale));
        }

        if (!string.IsNullOrWhiteSpace(leadSource) && !CustomerLeadSource.IsValid(leadSource))
        {
            throw new ArgumentException($"Invalid lead source: '{leadSource}'.", nameof(leadSource));
        }

        OrganizationId = organizationId;
        CreatedByUserId = createdByUserId;
        CustomerType = customerType.Trim();
        DisplayNameTh = CustomerNormalizer.CollapseWhitespace(displayNameTh);
        DisplayNameEn = string.IsNullOrWhiteSpace(displayNameEn) ? null : CustomerNormalizer.CollapseWhitespace(displayNameEn);
        NormalizedDisplayName = CustomerNormalizer.NormalizeName(displayNameTh);
        Code = GenerateCustomerCode(id);
        Status = CustomerStatus.Draft;
        PreferredLocale = preferredLocale.Trim();
        LeadSource = string.IsNullOrWhiteSpace(leadSource) ? null : leadSource.Trim();
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
    }

    public static Customer CreateDraft(
        Guid id,
        Guid organizationId,
        Guid createdByUserId,
        string customerType,
        string displayNameTh,
        string? displayNameEn,
        string preferredLocale,
        PrimaryContactInput primaryContact,
        DateTimeOffset now,
        string? leadSource = null)
    {
        if (primaryContact == null)
        {
            throw new ArgumentNullException(nameof(primaryContact), "Primary contact input is required.");
        }

        var customer = new Customer(
            id,
            organizationId,
            createdByUserId,
            customerType,
            displayNameTh,
            displayNameEn,
            preferredLocale,
            leadSource,
            now);

        var contact = new CustomerContact(
            Guid.NewGuid(),
            id,
            organizationId,
            primaryContact.Name,
            primaryContact.RoleTitle,
            primaryContact.Phone,
            primaryContact.Email,
            primaryContact.LineId,
            primaryContact.PreferredChannel,
            isPrimary: true,
            createdByUserId: createdByUserId,
            createdAtUtc: now);

        customer._contacts.Add(contact);
        return customer;
    }

    public static string GenerateCustomerCode(Guid id)
    {
        var hex = id.ToString("N")[..12].ToUpperInvariant();
        return $"CUS-{hex}";
    }

    public override string ToString() => $"Customer [Id={Id}, Code={Code}, Status={Status}]";
}
