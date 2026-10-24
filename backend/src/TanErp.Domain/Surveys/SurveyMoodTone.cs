using TanErp.Domain.Common;
using TanErp.Domain.Items;

namespace TanErp.Domain.Surveys;

public class SurveyMoodTone : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public LocalizedText Name { get; private set; } = null!;
    public LocalizedText? Description { get; private set; }
    public IReadOnlyList<string> ColorPalette { get; private set; } = Array.Empty<string>();
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    protected SurveyMoodTone() { }

    public SurveyMoodTone(
        Guid id,
        Guid organizationId,
        string code,
        LocalizedText name,
        LocalizedText? description = null,
        IEnumerable<string>? colorPalette = null,
        int sortOrder = 0,
        bool isActive = true,
        DateTimeOffset? createdAtUtc = null) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty.", nameof(code));
        ArgumentNullException.ThrowIfNull(name);

        OrganizationId = organizationId;
        Code = code.Trim().ToLowerInvariant();
        Name = name;
        Description = description;
        ColorPalette = colorPalette?.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim().ToUpperInvariant()).ToList().AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>();
        SortOrder = sortOrder;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public void Update(LocalizedText name, LocalizedText? description, IEnumerable<string>? colorPalette, int sortOrder, bool isActive)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        Description = description;
        ColorPalette = colorPalette?.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim().ToUpperInvariant()).ToList().AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>();
        SortOrder = sortOrder;
        IsActive = isActive;
        RowVersion = Guid.NewGuid();
    }
}
