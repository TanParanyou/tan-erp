using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>
/// A Work Breakdown Structure (WBS) node in a project hierarchy (up to 3 levels: Phase -> Zone/Room -> Work Package).
/// </summary>
public class ProjectWbsNode : Entity
{
    public const int MaxDepth = 3;

    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Level { get; private set; }
    public int SortOrder { get; private set; }
    public Guid RowVersion { get; private set; }

    protected ProjectWbsNode() { }

    public ProjectWbsNode(
        Guid id,
        Guid organizationId,
        Guid projectId,
        Guid? parentId,
        string code,
        string name,
        int level,
        int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
        if (level < 1) throw new ProjectDomainException("PROJECT_WBS_INVALID", "WBS level must be at least 1.");
        if (level > MaxDepth) throw new ProjectDomainException("PROJECT_WBS_DEPTH_EXCEEDED", $"WBS hierarchy cannot exceed {MaxDepth} levels.");
        if (level == 1 && parentId.HasValue) throw new ProjectDomainException("PROJECT_WBS_INVALID", "A root WBS node cannot have a parent.");
        if (level > 1 && (!parentId.HasValue || parentId.Value == Guid.Empty)) throw new ProjectDomainException("PROJECT_WBS_INVALID", "A child WBS node requires a parent.");

        Apply(code, name);
        OrganizationId = organizationId;
        ProjectId = projectId;
        ParentId = parentId;
        Level = level;
        SortOrder = sortOrder;
        RowVersion = Guid.NewGuid();
    }

    public void Update(string code, string name)
    {
        Apply(code, name);
        RowVersion = Guid.NewGuid();
    }

    private void Apply(string code, string name)
    {
        var trimmedCode = code?.Trim() ?? string.Empty;
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedCode.Length == 0 || trimmedCode.Length > 32)
        {
            throw new ProjectDomainException("PROJECT_WBS_INVALID", "WBS code is required and cannot exceed 32 characters.");
        }

        if (trimmedName.Length == 0 || trimmedName.Length > 200)
        {
            throw new ProjectDomainException("PROJECT_WBS_INVALID", "WBS name is required and cannot exceed 200 characters.");
        }

        Code = trimmedCode;
        Name = trimmedName;
    }
}
