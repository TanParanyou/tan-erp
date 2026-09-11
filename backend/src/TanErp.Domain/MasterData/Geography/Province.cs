using TanErp.Domain.Common;

namespace TanErp.Domain.MasterData.Geography;

public class Province : Entity
{
    public string Code { get; private set; } = string.Empty;
    public string NameTh { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;

    protected Province() { }

    public Province(Guid id, string code, string nameTh, string nameEn) : base(id)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Province code cannot be blank.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameTh))
            throw new ArgumentException("Province name (TH) cannot be blank.", nameof(nameTh));
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("Province name (EN) cannot be blank.", nameof(nameEn));

        Code = code.Trim();
        NameTh = nameTh.Trim();
        NameEn = nameEn.Trim();
    }
}
