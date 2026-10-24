using TanErp.Domain.Common;

namespace TanErp.Domain.Organization;

public class JobTitle : Entity
{
    public Guid OrganizationId { get; private set; }
    public Organization? Organization { get; set; }

    public Guid? DepartmentId { get; private set; }
    public Department? Department { get; set; }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? NameEn { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    protected JobTitle() { }

    public JobTitle(
        Guid id,
        Guid organizationId,
        string code,
        string name,
        Guid? departmentId = null,
        string? nameEn = null,
        bool isActive = true,
        DateTimeOffset? createdAtUtc = null) : base(id)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Job title code cannot be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Job title name cannot be empty.", nameof(name));

        OrganizationId = organizationId;
        DepartmentId = departmentId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        NameEn = string.IsNullOrWhiteSpace(nameEn) ? null : nameEn.Trim();
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc ?? DateTimeOffset.UtcNow;
    }

    public void Update(string name, string? nameEn, Guid? departmentId, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Job title name cannot be empty.", nameof(name));

        Name = name.Trim();
        NameEn = string.IsNullOrWhiteSpace(nameEn) ? null : nameEn.Trim();
        DepartmentId = departmentId;
        IsActive = isActive;
        RowVersion = Guid.NewGuid();
    }
}
