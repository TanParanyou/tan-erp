using TanErp.Domain.Common;
using TanErp.Domain.Items;

namespace TanErp.Domain.Surveys;

public class SurveyDesignStyle : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public LocalizedText Name { get; private set; } = null!;
    public LocalizedText? Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();
    public Guid? ImageFileId { get; private set; }
    public IReadOnlyList<string> ImageFileIds { get; private set; } = Array.Empty<string>();

    protected SurveyDesignStyle() { }

    public SurveyDesignStyle(
        Guid id,
        Guid organizationId,
        string code,
        LocalizedText name,
        LocalizedText? description = null,
        int sortOrder = 0,
        bool isActive = true,
        DateTimeOffset? createdAtUtc = null,
        Guid? imageFileId = null,
        IReadOnlyList<string>? imageFileIds = null) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty.", nameof(code));
        ArgumentNullException.ThrowIfNull(name);

        var fileIds = (imageFileIds ?? Array.Empty<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (imageFileId.HasValue && !fileIds.Contains(imageFileId.Value.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            fileIds.Insert(0, imageFileId.Value.ToString());
        }

        OrganizationId = organizationId;
        Code = code.Trim().ToLowerInvariant();
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
        ImageFileIds = fileIds.AsReadOnly();
        ImageFileId = fileIds.Count > 0 && Guid.TryParse(fileIds[0], out var primary) ? primary : imageFileId;
    }

    public void Update(
        LocalizedText name,
        LocalizedText? description,
        int sortOrder,
        bool isActive,
        Guid? imageFileId = null,
        IReadOnlyList<string>? imageFileIds = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        var fileIds = (imageFileIds ?? Array.Empty<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (imageFileId.HasValue && !fileIds.Contains(imageFileId.Value.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            fileIds.Insert(0, imageFileId.Value.ToString());
        }

        Name = name;
        Description = description;
        SortOrder = sortOrder;
        IsActive = isActive;
        ImageFileIds = fileIds.AsReadOnly();
        ImageFileId = fileIds.Count > 0 && Guid.TryParse(fileIds[0], out var primary) ? primary : imageFileId;
        RowVersion = Guid.NewGuid();
    }
}
