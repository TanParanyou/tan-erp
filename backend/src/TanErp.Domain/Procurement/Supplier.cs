using TanErp.Domain.Common;

namespace TanErp.Domain.Procurement;

/// <summary>A vendor we buy from. Distinct from an Item Master Cost Source, which only records where a price came from.</summary>
public class Supplier : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NormalizedCode { get; private set; } = string.Empty;
    public string NameTh { get; private set; } = string.Empty;
    public string? NameEn { get; private set; }
    public string NormalizedName { get; private set; } = string.Empty;
    public string? TaxId { get; private set; }
    public string? ContactName { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public int PaymentTermDays { get; private set; }
    public string Status { get; private set; } = SupplierStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected Supplier() { }

    public Supplier(
        Guid id, Guid organizationId, string code, string nameTh, string? nameEn, string? taxId, string? contactName,
        string? phone, string? email, int paymentTermDays, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Supplier code cannot be blank.", nameof(code));

        OrganizationId = organizationId;
        Code = code.Trim();
        NormalizedCode = Code.ToUpperInvariant();
        Apply(nameTh, nameEn, taxId, contactName, phone, email, paymentTermDays);
        Status = SupplierStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now.ToUniversalTime();
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void Update(string nameTh, string? nameEn, string? taxId, string? contactName, string? phone, string? email, int paymentTermDays, DateTimeOffset now)
    {
        Apply(nameTh, nameEn, taxId, contactName, phone, email, paymentTermDays);
        Touch(now);
    }

    public void SetActive(bool active, DateTimeOffset now)
    {
        var target = active ? SupplierStatus.Active : SupplierStatus.Inactive;
        if (Status == target)
        {
            throw new ProcurementDomainException("SUPPLIER_INVALID_STATE", $"The supplier is already {Status}.");
        }

        Status = target;
        Touch(now);
    }

    private void Apply(string nameTh, string? nameEn, string? taxId, string? contactName, string? phone, string? email, int paymentTermDays)
    {
        var th = nameTh?.Trim() ?? string.Empty;
        if (th.Length == 0 || th.Length > 200)
        {
            throw new ProcurementDomainException("SUPPLIER_FIELD_INVALID", "Supplier Thai name is required and cannot exceed 200 characters.");
        }

        string? Clean(string? value, int max, string field)
        {
            var trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            if (trimmed is { } v && v.Length > max)
            {
                throw new ProcurementDomainException("SUPPLIER_FIELD_INVALID", $"{field} cannot exceed {max} characters.");
            }

            return trimmed;
        }

        if (paymentTermDays is < 0 or > 365)
        {
            throw new ProcurementDomainException("SUPPLIER_FIELD_INVALID", "Payment term must be between 0 and 365 days.");
        }

        NameTh = th;
        NameEn = Clean(nameEn, 200, "English name");
        NormalizedName = th.ToLowerInvariant();
        TaxId = Clean(taxId, 20, "Tax ID");
        ContactName = Clean(contactName, 200, "Contact name");
        Phone = Clean(phone, 50, "Phone");
        Email = Clean(email, 200, "Email");
        PaymentTermDays = paymentTermDays;
    }

    private void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
