using TanErp.Domain.Common;

namespace TanErp.Domain.MasterData.Geography;

public class District : Entity
{
    public Guid ProvinceId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NameTh { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;

    protected District() { }

    public District(Guid id, Guid provinceId, string code, string nameTh, string nameEn) : base(id)
    {
        if (provinceId == Guid.Empty)
            throw new ArgumentException("ProvinceId cannot be empty.", nameof(provinceId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("District code cannot be blank.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameTh))
            throw new ArgumentException("District name (TH) cannot be blank.", nameof(nameTh));
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("District name (EN) cannot be blank.", nameof(nameEn));

        ProvinceId = provinceId;
        Code = code.Trim();
        NameTh = nameTh.Trim();
        NameEn = nameEn.Trim();
    }
}
