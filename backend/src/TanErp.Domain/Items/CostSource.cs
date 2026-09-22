using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public class CostSource : Entity
{
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
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
        DateTimeOffset createdAtUtc) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ItemValidationException("COST_SOURCE_ORG_REQUIRED", "Organization is required.");
        if (string.IsNullOrWhiteSpace(code))
            throw new ItemValidationException("COST_SOURCE_CODE_REQUIRED", "Cost source code is required.");
        if (name == null || string.IsNullOrWhiteSpace(name.Thai))
            throw new ItemValidationException("COST_SOURCE_NAME_REQUIRED", "Thai name is required.");

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name;
        Priority = priority;
        IsActive = isActive;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }
}
