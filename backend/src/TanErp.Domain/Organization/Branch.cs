using TanErp.Domain.Common;

namespace TanErp.Domain.Organization;

public class Branch : Entity
{
    public Guid OrganizationId { get; private set; }
    public Organization? Organization { get; set; }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public string? NameEn { get; private set; }
    public string? TaxBranchCode { get; private set; }
    public string? AddressTh { get; private set; }
    public string? AddressEn { get; private set; }
    public string? Phone { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    private readonly List<Membership> _memberships = new();
    public IReadOnlyCollection<Membership> Memberships => _memberships.AsReadOnly();

    protected Branch() { }

    public Branch(Guid id, Guid organizationId, string code, string name, bool isActive = true, DateTimeOffset? createdAtUtc = null)
        : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Branch code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Branch name cannot be empty.", nameof(name));

        OrganizationId = organizationId;
        Code = code.Trim();
        Name = name.Trim();
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public static Branch Create(
        Guid id, Guid organizationId, string code, string name, string? nameEn, string? taxBranchCode,
        string? addressTh, string? addressEn, string? phone, DateTimeOffset createdAtUtc)
    {
        var trimmedCode = code?.Trim();
        if (!BranchCode.IsValid(trimmedCode))
            throw new OrganizationDomainException("BRANCH_CODE_INVALID", "Branch code must be 1-50 characters of letters, digits, underscore or hyphen.");

        var branch = new Branch(id, organizationId, trimmedCode!, name, isActive: true, createdAtUtc);
        branch.UpdateDetails(name, nameEn, taxBranchCode, addressTh, addressEn, phone);
        return branch;
    }

    public void UpdateDetails(string name, string? nameEn, string? taxBranchCode, string? addressTh, string? addressEn, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new OrganizationDomainException("REQUEST_VALIDATION_FAILED", "Branch name cannot be empty.");
        var taxCode = ProfileText.Optional(taxBranchCode, 5, "Tax branch code");
        if (taxCode is not null && !TaxBranchCodeRule.IsValid(taxCode))
            throw new OrganizationDomainException("BRANCH_TAX_CODE_INVALID", "Tax branch code must be exactly 5 digits.");

        Name = ProfileText.Optional(name, OrganizationLimits.Name, "Name")!;
        NameEn = ProfileText.Optional(nameEn, OrganizationLimits.Name, "English name");
        TaxBranchCode = taxCode;
        AddressTh = ProfileText.Optional(addressTh, OrganizationLimits.Address, "Thai address");
        AddressEn = ProfileText.Optional(addressEn, OrganizationLimits.Address, "English address");
        Phone = ProfileText.Optional(phone, OrganizationLimits.Phone, "Phone");
        RowVersion = Guid.NewGuid();
    }

    public void Deactivate() { IsActive = false; RowVersion = Guid.NewGuid(); }
    public void Activate() { IsActive = true; RowVersion = Guid.NewGuid(); }
}
