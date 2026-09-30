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
    public string? InactiveReason { get; private set; }
    public string PreferredLocale { get; private set; } = TanErp.Domain.Crm.Customers.PreferredLocale.Thai;
    public string? LeadSource { get; private set; }
    public string? LeadSourceNote { get; private set; }
    public Guid? ImageFileId { get; private set; }
    public string? LegalName { get; private set; }
    public string? TaxIdentifier { get; private set; }
    public string? NormalizedTaxIdentifier { get; private set; }
    public string? BranchCode { get; private set; }
    public int CreditTermDays { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public string CurrencyCode { get; private set; } = "THB";
    public string? BillingCycle { get; private set; }
    public int? BillingDay { get; private set; }
    public string? PaymentConditionNote { get; private set; }
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
        string? leadSourceNote,
        Guid? imageFileId,
        DateTimeOffset createdAtUtc,
        string? customerCode = null) : base(id)
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

        if (leadSourceNote?.Trim().Length > 200)
        {
            throw new ArgumentException("Lead source note cannot exceed 200 characters.", nameof(leadSourceNote));
        }

        OrganizationId = organizationId;
        CreatedByUserId = createdByUserId;
        CustomerType = customerType.Trim();
        DisplayNameTh = CustomerNormalizer.CollapseWhitespace(displayNameTh);
        DisplayNameEn = string.IsNullOrWhiteSpace(displayNameEn) ? null : CustomerNormalizer.CollapseWhitespace(displayNameEn);
        NormalizedDisplayName = CustomerNormalizer.NormalizeName(displayNameTh);
        Code = !string.IsNullOrWhiteSpace(customerCode) ? customerCode.Trim() : GenerateCustomerCode(id);
        Status = CustomerStatus.Draft;
        PreferredLocale = preferredLocale.Trim();
        LeadSource = string.IsNullOrWhiteSpace(leadSource) ? null : leadSource.Trim();
        LeadSourceNote = string.IsNullOrWhiteSpace(leadSourceNote) ? null : leadSourceNote.Trim();
        if (imageFileId.HasValue && imageFileId != ImageFileId) throw new ArgumentException("Customer image changes require a verified upload session.", nameof(imageFileId));
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
        string? leadSource = null,
        string? leadSourceNote = null,
        Guid? imageFileId = null,
        string? customerCode = null)
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
            leadSourceNote,
            imageFileId,
            now,
            customerCode);


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

    public CustomerActivationOutcome Activate(Guid expectedRowVersion)
    {
        if (RowVersion != expectedRowVersion) return CustomerActivationOutcome.VersionConflict;
        if (Status != CustomerStatus.Draft) return CustomerActivationOutcome.InvalidState;
        if (_contacts.Count(c => c.IsPrimary && c.Status == ContactStatus.Active) != 1)
            return CustomerActivationOutcome.InvalidState;
        Status = CustomerStatus.Active;
        RowVersion = Guid.NewGuid();
        return CustomerActivationOutcome.Activated;
    }

    public bool UpdateProfile(
        Guid expectedRowVersion,
        string customerType,
        string displayNameTh,
        string? displayNameEn,
        string preferredLocale,
        string? leadSource,
        string? leadSourceNote,
        Guid? imageFileId)
    {
        if (RowVersion != expectedRowVersion || Status == CustomerStatus.Inactive) return false;
        if (Status != CustomerStatus.Draft && customerType != CustomerType) return false;
        if (string.IsNullOrWhiteSpace(displayNameTh)) throw new ArgumentException("Customer Thai display name cannot be blank.", nameof(displayNameTh));
        if (!Customers.CustomerType.IsValid(customerType)) throw new ArgumentException("Customer type is invalid.", nameof(customerType));
        if (!Customers.PreferredLocale.IsValid(preferredLocale)) throw new ArgumentException("Preferred locale is invalid.", nameof(preferredLocale));
        if (!string.IsNullOrWhiteSpace(leadSource) && !CustomerLeadSource.IsValid(leadSource)) throw new ArgumentException("Lead source is invalid.", nameof(leadSource));
        if (leadSourceNote?.Trim().Length > 200) throw new ArgumentException("Lead source note cannot exceed 200 characters.", nameof(leadSourceNote));

        CustomerType = customerType.Trim();
        DisplayNameTh = CustomerNormalizer.CollapseWhitespace(displayNameTh);
        DisplayNameEn = string.IsNullOrWhiteSpace(displayNameEn) ? null : CustomerNormalizer.CollapseWhitespace(displayNameEn);
        NormalizedDisplayName = CustomerNormalizer.NormalizeName(displayNameTh);
        PreferredLocale = preferredLocale.Trim();
        LeadSource = string.IsNullOrWhiteSpace(leadSource) ? null : leadSource.Trim();
        LeadSourceNote = string.IsNullOrWhiteSpace(leadSourceNote) ? null : leadSourceNote.Trim();
        ImageFileId = imageFileId;
        RowVersion = Guid.NewGuid();
        return true;
    }

    public bool Deactivate(Guid expectedRowVersion, string reason)
    {
        if (RowVersion != expectedRowVersion || Status != CustomerStatus.Active || string.IsNullOrWhiteSpace(reason)) return false;
        Status = CustomerStatus.Inactive;
        InactiveReason = CustomerNormalizer.CollapseWhitespace(reason);
        RowVersion = Guid.NewGuid();
        return true;
    }

    public void AdvanceVersion() => RowVersion = Guid.NewGuid();

    public bool Reactivate(Guid expectedRowVersion)
    {
        if (RowVersion != expectedRowVersion || Status != CustomerStatus.Inactive) return false;
        Status = CustomerStatus.Active;
        InactiveReason = null;
        RowVersion = Guid.NewGuid();
        return true;
    }

    public CustomerContact AddContact(Guid contactId, Guid createdByUserId, PrimaryContactInput input, DateTimeOffset now)
    {
        var contact = new CustomerContact(contactId, Id, OrganizationId, input.Name, input.RoleTitle, input.Phone, input.Email, input.LineId, input.PreferredChannel, false, createdByUserId, now);
        _contacts.Add(contact);
        RowVersion = Guid.NewGuid();
        return contact;
    }

    public bool SetPrimaryContact(Guid contactId)
    {
        var next = _contacts.SingleOrDefault(contact => contact.Id == contactId);
        if (next is null || next.Status != ContactStatus.Active) return false;
        foreach (var contact in _contacts.Where(contact => contact.IsPrimary && contact.Id != contactId)) contact.ClearPrimary();
        if (!next.IsPrimary && !next.MakePrimary(next.RowVersion)) return false;
        RowVersion = Guid.NewGuid();
        return true;
    }

    public bool UpdateContact(Guid contactId, Guid expectedContactVersion, PrimaryContactInput input)
    {
        var contact = _contacts.SingleOrDefault(candidate => candidate.Id == contactId);
        if (contact is null || !contact.Update(expectedContactVersion, input.Name, input.RoleTitle, input.Phone, input.Email, input.LineId, input.PreferredChannel)) return false;
        RowVersion = Guid.NewGuid();
        return true;
    }

    public bool DeactivateContact(Guid contactId, Guid expectedContactVersion)
    {
        var contact = _contacts.SingleOrDefault(candidate => candidate.Id == contactId);
        if (contact is null || contact.IsPrimary || !contact.Deactivate(expectedContactVersion)) return false;
        RowVersion = Guid.NewGuid();
        return true;
    }

    public void UpdateCommercialProfile(
        string? legalName, string? taxIdentifier, string? branchCode,
        int creditTermDays, decimal? creditLimit, string currencyCode,
        string? billingCycle, int? billingDay, string? paymentConditionNote)
    {
        if (creditTermDays < 0 || creditLimit < 0 || billingDay is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(creditTermDays), "Credit and billing values are outside the allowed range.");
        var normalizedTaxIdentifier = string.IsNullOrWhiteSpace(taxIdentifier) ? null : new string(taxIdentifier.Where(char.IsDigit).ToArray());
        if (normalizedTaxIdentifier is not null && (normalizedTaxIdentifier.Length != 13 || !TaxIdentifierValidator.IsValid(normalizedTaxIdentifier)))
            throw new ArgumentException("Tax identifier is invalid.", nameof(taxIdentifier));
        if (!string.IsNullOrWhiteSpace(branchCode) && (branchCode.Length != 5 || branchCode.Any(c => !char.IsDigit(c))))
            throw new ArgumentException("Branch code must contain five digits.", nameof(branchCode));
        LegalName = string.IsNullOrWhiteSpace(legalName) ? null : CustomerNormalizer.CollapseWhitespace(legalName);
        TaxIdentifier = normalizedTaxIdentifier;
        NormalizedTaxIdentifier = normalizedTaxIdentifier;
        BranchCode = string.IsNullOrWhiteSpace(branchCode) ? null : branchCode.Trim();
        CreditTermDays = creditTermDays;
        CreditLimit = creditLimit;
        CurrencyCode = string.IsNullOrWhiteSpace(currencyCode) ? "THB" : currencyCode.Trim().ToUpperInvariant();
        BillingCycle = string.IsNullOrWhiteSpace(billingCycle) ? null : billingCycle.Trim();
        BillingDay = billingDay;
        PaymentConditionNote = string.IsNullOrWhiteSpace(paymentConditionNote) ? null : CustomerNormalizer.CollapseWhitespace(paymentConditionNote);
        RowVersion = Guid.NewGuid();
    }

    public override string ToString() => $"Customer [Id={Id}, Code={Code}, Status={Status}]";
}
