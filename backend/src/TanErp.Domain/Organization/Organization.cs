using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Domain.Organization;

public class Organization : Entity
{
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public string? NameEn { get; private set; }
    public string? TaxIdentifier { get; private set; }
    public string? AddressTh { get; private set; }
    public string? AddressEn { get; private set; }
    public string? Phone { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();

    private readonly List<Membership> _memberships = new();
    public IReadOnlyCollection<Membership> Memberships => _memberships.AsReadOnly();

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    protected Organization() { }

    public Organization(Guid id, string name, bool isActive = true, DateTimeOffset? createdAtUtc = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name cannot be empty.", nameof(name));

        Name = name.Trim();
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public void UpdateProfile(string name, string? nameEn, string? taxIdentifier, string? addressTh, string? addressEn, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new OrganizationDomainException("REQUEST_VALIDATION_FAILED", "Organization name cannot be empty.");
        var tax = ProfileText.Optional(taxIdentifier, 13, "Tax identifier");
        if (tax is not null && !ThaiTaxIdentifier.IsValid(tax))
            throw new OrganizationDomainException("ORGANIZATION_TAX_ID_INVALID", "Tax identifier must be 13 digits with a valid check digit.");

        Name = ProfileText.Optional(name, OrganizationLimits.Name, "Name")!;
        NameEn = ProfileText.Optional(nameEn, OrganizationLimits.Name, "English name");
        TaxIdentifier = tax;
        AddressTh = ProfileText.Optional(addressTh, OrganizationLimits.Address, "Thai address");
        AddressEn = ProfileText.Optional(addressEn, OrganizationLimits.Address, "English address");
        Phone = ProfileText.Optional(phone, OrganizationLimits.Phone, "Phone");
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate() { IsActive = false; RowVersion = Guid.NewGuid(); }
    public void Activate() { IsActive = true; RowVersion = Guid.NewGuid(); }
}
