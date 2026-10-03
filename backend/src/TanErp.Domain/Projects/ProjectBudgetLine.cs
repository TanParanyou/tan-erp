using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

public class ProjectBudgetLine : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public int SortOrder { get; private set; }

    protected ProjectBudgetLine() { }

    public ProjectBudgetLine(Guid id, Guid organizationId, Guid projectId, string category, string description, decimal amount, int sortOrder) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
        if (!ProjectBudgetCategory.IsValid(category)) throw new ProjectDomainException("PROJECT_BUDGET_INVALID", $"Invalid budget category '{category}'.");
        if (string.IsNullOrWhiteSpace(description)) throw new ProjectDomainException("PROJECT_BUDGET_INVALID", "A budget line needs a description.");
        if (description.Trim().Length > 200) throw new ProjectDomainException("PROJECT_BUDGET_INVALID", "Budget line description cannot exceed 200 characters.");
        if (amount < 0) throw new ProjectDomainException("PROJECT_BUDGET_INVALID", "Budget amount cannot be negative.");

        OrganizationId = organizationId;
        ProjectId = projectId;
        Category = category.Trim();
        Description = description.Trim();
        Amount = decimal.Round(amount, 2);
        SortOrder = sortOrder;
    }
}
