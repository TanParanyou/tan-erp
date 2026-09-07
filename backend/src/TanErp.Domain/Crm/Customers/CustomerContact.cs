using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Customers;

public class CustomerContact : Entity
{
    public Guid CustomerId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? RoleTitle { get; private set; }
    public string? Phone { get; private set; }
    public string? NormalizedPhone { get; private set; }
    public string? Email { get; private set; }
    public string? NormalizedEmail { get; private set; }
    public string PreferredChannel { get; private set; } = ContactChannel.Phone;
    public bool IsPrimary { get; private set; }
    public string Status { get; private set; } = "active";
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    public Customer? Customer { get; private set; }

    protected CustomerContact() { }

    internal CustomerContact(
        Guid id,
        Guid customerId,
        Guid organizationId,
        string name,
        string? roleTitle,
        string? phone,
        string? email,
        string preferredChannel,
        bool isPrimary,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Contact name cannot be blank.", nameof(name));
        }

        var normalizedPhone = CustomerNormalizer.NormalizePhone(phone);
        var normalizedEmail = CustomerNormalizer.NormalizeEmail(email);

        if (string.IsNullOrWhiteSpace(normalizedPhone) && string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new ArgumentException("Contact must provide at least a phone number or an email address.", nameof(phone));
        }

        if (!ContactChannel.IsValid(preferredChannel))
        {
            throw new ArgumentException($"Invalid preferred channel: '{preferredChannel}'.", nameof(preferredChannel));
        }

        CustomerId = customerId;
        OrganizationId = organizationId;
        Name = CustomerNormalizer.CollapseWhitespace(name);
        RoleTitle = string.IsNullOrWhiteSpace(roleTitle) ? null : CustomerNormalizer.CollapseWhitespace(roleTitle);
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        NormalizedPhone = normalizedPhone;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        NormalizedEmail = normalizedEmail;
        PreferredChannel = preferredChannel.Trim();
        IsPrimary = isPrimary;
        Status = "active";
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
    }

    public override string ToString() => $"CustomerContact [Id={Id}, CustomerId={CustomerId}, IsPrimary={IsPrimary}]";
}
