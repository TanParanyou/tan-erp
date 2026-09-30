using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Customers;

public class CustomerAddress : Entity
{
    public Guid CustomerId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string AddressType { get; private set; } = "billing";
    public string Label { get; private set; } = string.Empty;
    public string AddressLine1 { get; private set; } = string.Empty;
    public string Subdistrict { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = "TH";
    public string Status { get; private set; } = "active";
    public bool IsPrimary { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    protected CustomerAddress() { }

    public CustomerAddress(Guid id, Guid customerId, Guid organizationId, Guid createdByUserId, string addressType, string label,
        string addressLine1, string subdistrict, string district, string province, string postalCode, string countryCode, bool isPrimary, DateTimeOffset now) : base(id)
    {
        if (string.IsNullOrWhiteSpace(label) || string.IsNullOrWhiteSpace(addressLine1)) throw new ArgumentException("Address fields are required.");
        if (!AddressLocationValidator.IsValid(subdistrict, district, province, postalCode, countryCode))
            throw new ArgumentException("Address location is invalid.", nameof(postalCode));
        if (addressType is not ("billing" or "contact")) throw new ArgumentException("Address type is invalid.", nameof(addressType));
        Id = id; CustomerId = customerId; OrganizationId = organizationId; CreatedByUserId = createdByUserId;
        AddressType = addressType; Label = label.Trim(); AddressLine1 = addressLine1.Trim(); Subdistrict = subdistrict.Trim();
        District = district.Trim(); Province = province.Trim(); PostalCode = postalCode; CountryCode = countryCode.Trim().ToUpperInvariant();
        IsPrimary = isPrimary; RowVersion = Guid.NewGuid(); CreatedAtUtc = now;
    }

    public bool Update(Guid expectedRowVersion, string label, string addressLine1, string subdistrict, string district, string province, string postalCode, string countryCode)
    {
        if (RowVersion != expectedRowVersion || Status != "active") return false;
        if (string.IsNullOrWhiteSpace(label) || string.IsNullOrWhiteSpace(addressLine1)) throw new ArgumentException("Address fields are required.");
        if (!AddressLocationValidator.IsValid(subdistrict, district, province, postalCode, countryCode))
            throw new ArgumentException("Address location is invalid.", nameof(postalCode));
        Label = label.Trim(); AddressLine1 = addressLine1.Trim(); Subdistrict = subdistrict.Trim(); District = district.Trim(); Province = province.Trim();
        PostalCode = postalCode; CountryCode = countryCode.Trim().ToUpperInvariant(); RowVersion = Guid.NewGuid(); return true;
    }

    public void SetPrimary(bool value) { IsPrimary = value; RowVersion = Guid.NewGuid(); }
    public void Deactivate() { Status = "inactive"; IsPrimary = false; RowVersion = Guid.NewGuid(); }
}
