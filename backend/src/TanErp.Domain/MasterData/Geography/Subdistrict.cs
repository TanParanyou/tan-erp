using TanErp.Domain.Common;

namespace TanErp.Domain.MasterData.Geography;

public class Subdistrict : Entity
{
    public Guid DistrictId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NameTh { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public decimal? DefaultLatitude { get; private set; }
    public decimal? DefaultLongitude { get; private set; }

    protected Subdistrict() { }

    public Subdistrict(
        Guid id,
        Guid districtId,
        string code,
        string nameTh,
        string nameEn,
        string postalCode,
        decimal? defaultLatitude = null,
        decimal? defaultLongitude = null) : base(id)
    {
        if (districtId == Guid.Empty)
            throw new ArgumentException("DistrictId cannot be empty.", nameof(districtId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Subdistrict code cannot be blank.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameTh))
            throw new ArgumentException("Subdistrict name (TH) cannot be blank.", nameof(nameTh));
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("Subdistrict name (EN) cannot be blank.", nameof(nameEn));
        DistrictId = districtId;
        Code = code.Trim();
        NameTh = nameTh.Trim();
        NameEn = nameEn.Trim();
        PostalCode = string.IsNullOrWhiteSpace(postalCode) ? string.Empty : postalCode.Trim();
        DefaultLatitude = defaultLatitude;
        DefaultLongitude = defaultLongitude;
    }
}
