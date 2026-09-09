using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Sites;

public class Site : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public string NormalizedLabel { get; private set; } = string.Empty;
    public string AddressLine1 { get; private set; } = string.Empty;
    public string Subdistrict { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string? AccessNote { get; private set; }
    public string Status { get; private set; } = SiteStatus.Active;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    protected Site() { }

    private Site(
        Guid id,
        Guid organizationId,
        Guid customerId,
        Guid createdByUserId,
        string label,
        SiteAddressInput address,
        decimal? latitude,
        decimal? longitude,
        string? accessNote,
        DateTimeOffset now) : base(id)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Site label cannot be blank.", nameof(label));

        if (address == null)
            throw new ArgumentNullException(nameof(address), "Site address cannot be null.");

        if (string.IsNullOrWhiteSpace(address.AddressLine1))
            throw new ArgumentException("AddressLine1 cannot be blank.", nameof(address));
        if (string.IsNullOrWhiteSpace(address.Subdistrict))
            throw new ArgumentException("Subdistrict cannot be blank.", nameof(address));
        if (string.IsNullOrWhiteSpace(address.District))
            throw new ArgumentException("District cannot be blank.", nameof(address));
        if (string.IsNullOrWhiteSpace(address.Province))
            throw new ArgumentException("Province cannot be blank.", nameof(address));
        if (string.IsNullOrWhiteSpace(address.PostalCode))
            throw new ArgumentException("PostalCode cannot be blank.", nameof(address));
        if (string.IsNullOrWhiteSpace(address.CountryCode))
            throw new ArgumentException("CountryCode cannot be blank.", nameof(address));

        if ((latitude.HasValue && !longitude.HasValue) || (!latitude.HasValue && longitude.HasValue))
        {
            throw new ArgumentException("Latitude and Longitude must both be provided or both be null.");
        }

        if (latitude.HasValue)
        {
            if (latitude.Value < -90.0m || latitude.Value > 90.0m)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
            if (longitude!.Value < -180.0m || longitude.Value > 180.0m)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
        }

        OrganizationId = organizationId;
        CustomerId = customerId;
        CreatedByUserId = createdByUserId;
        Code = GenerateSiteCode(id);
        Label = SiteNormalizer.CollapseWhitespace(label);
        NormalizedLabel = SiteNormalizer.NormalizeLabel(label);
        AddressLine1 = SiteNormalizer.CollapseWhitespace(address.AddressLine1);
        Subdistrict = SiteNormalizer.CollapseWhitespace(address.Subdistrict);
        District = SiteNormalizer.CollapseWhitespace(address.District);
        Province = SiteNormalizer.CollapseWhitespace(address.Province);
        PostalCode = address.PostalCode.Trim();
        CountryCode = address.CountryCode.Trim().ToUpperInvariant();
        Latitude = latitude;
        Longitude = longitude;
        AccessNote = string.IsNullOrWhiteSpace(accessNote) ? null : SiteNormalizer.CollapseWhitespace(accessNote);
        Status = SiteStatus.Active;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now;
    }

    public static Site CreateActive(
        Guid id,
        Guid organizationId,
        Guid customerId,
        Guid createdByUserId,
        string label,
        SiteAddressInput address,
        decimal? latitude,
        decimal? longitude,
        string? accessNote,
        DateTimeOffset now)
    {
        return new Site(id, organizationId, customerId, createdByUserId, label, address, latitude, longitude, accessNote, now);
    }

    public static string GenerateSiteCode(Guid id)
    {
        var hex = id.ToString("N")[..12].ToUpperInvariant();
        return $"SITE-{hex}";
    }

    public override string ToString() => $"Site [Id={Id}, Code={Code}, Label={Label}]";
}
