using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class CostSource : Entity
{
    public const string ManualType = "manual";
    public const string LegacyType = "legacy";
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string SourceType { get; private set; } = ManualType;
    public LocalizedText Name { get; private set; } = null!;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected CostSource() { }

    public CostSource(
        Guid id,
        Guid organizationId,
        string code,
        LocalizedText name,
        int priority,
        bool isActive,
        DateTimeOffset createdAtUtc,
        string sourceType = ManualType) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ItemValidationException("COST_SOURCE_ORG_REQUIRED", "Organization is required.");
        if (string.IsNullOrWhiteSpace(code))
            throw new ItemValidationException("COST_SOURCE_CODE_REQUIRED", "Cost source code is required.");
        if (name == null || string.IsNullOrWhiteSpace(name.Thai))
            throw new ItemValidationException("COST_SOURCE_NAME_REQUIRED", "Thai name is required.");

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        if (sourceType is not (ManualType or LegacyType))
            throw new ItemValidationException("COST_SOURCE_TYPE_INVALID", "Cost source type is invalid.");
        SourceType = sourceType;
        Name = name;
        Priority = priority;
        IsActive = isActive;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public bool Update(string code, LocalizedText name, int priority, Guid expectedRowVersion, DateTimeOffset nowUtc)
    {
        if (RowVersion != expectedRowVersion) return false;
        if (string.IsNullOrWhiteSpace(code))
            throw new ItemValidationException("COST_SOURCE_CODE_REQUIRED", "Cost source code is required.");
        if (name == null || string.IsNullOrWhiteSpace(name.Thai))
            throw new ItemValidationException("COST_SOURCE_NAME_REQUIRED", "Thai name is required.");
        Code = code.Trim().ToUpperInvariant();
        Name = name;
        Priority = priority;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = nowUtc;
        return true;
    }

    public bool Deactivate(Guid expectedRowVersion, DateTimeOffset nowUtc)
    {
        if (RowVersion != expectedRowVersion) return false;
        if (!IsActive) return true;
        IsActive = false;
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = nowUtc;
        return true;
    }
}
