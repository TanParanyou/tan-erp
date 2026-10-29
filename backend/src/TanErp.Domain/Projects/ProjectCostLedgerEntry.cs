using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>
/// Append-only project cost ledger entry recording a commitment or actual cost change.
/// Entries are never updated or deleted; corrections and commitment reductions are posted as new entries with IsReversal = true.
/// </summary>
public class ProjectCostLedgerEntry : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? WbsNodeId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string EntryType { get; private set; } = string.Empty;
    public string SourceType { get; private set; } = string.Empty;
    public Guid SourceId { get; private set; }
    public string SourceNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public bool IsReversal { get; private set; }
    public Guid ActorUserId { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }

    protected ProjectCostLedgerEntry() { }

    public ProjectCostLedgerEntry(
        Guid id,
        Guid organizationId,
        Guid projectId,
        Guid? wbsNodeId,
        string category,
        string entryType,
        string sourceType,
        Guid sourceId,
        string sourceNumber,
        string description,
        decimal amount,
        bool isReversal,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
        if (sourceId == Guid.Empty) throw new ArgumentException("Source ID cannot be empty.", nameof(sourceId));
        if (actorUserId == Guid.Empty) throw new ArgumentException("Actor user ID cannot be empty.", nameof(actorUserId));

        if (!ProjectBudgetCategory.IsValid(category))
        {
            throw new ProjectDomainException("PROJECT_COST_ENTRY_INVALID", $"Invalid project cost category '{category}'.");
        }

        if (!ProjectCostEntryType.IsValid(entryType))
        {
            throw new ProjectDomainException("PROJECT_COST_ENTRY_INVALID", $"Invalid project cost entry type '{entryType}'.");
        }

        if (!ProjectCostSourceType.IsValid(sourceType))
        {
            throw new ProjectDomainException("PROJECT_COST_ENTRY_INVALID", $"Invalid project cost source type '{sourceType}'.");
        }

        var trimmedSourceNumber = sourceNumber?.Trim() ?? string.Empty;
        if (trimmedSourceNumber.Length == 0 || trimmedSourceNumber.Length > 64)
        {
            throw new ProjectDomainException("PROJECT_COST_ENTRY_INVALID", "Source number is required and cannot exceed 64 characters.");
        }

        var trimmedDescription = description?.Trim() ?? string.Empty;
        if (trimmedDescription.Length == 0 || trimmedDescription.Length > 500)
        {
            throw new ProjectDomainException("PROJECT_COST_ENTRY_INVALID", "Cost entry description is required and cannot exceed 500 characters.");
        }

        var roundedAmount = decimal.Round(amount, 2);
        if (roundedAmount == 0m)
        {
            throw new ProjectDomainException("PROJECT_COST_ENTRY_INVALID", "Cost ledger entry amount cannot be zero.");
        }

        OrganizationId = organizationId;
        ProjectId = projectId;
        WbsNodeId = wbsNodeId;
        Category = category.Trim();
        EntryType = entryType.Trim();
        SourceType = sourceType.Trim();
        SourceId = sourceId;
        SourceNumber = trimmedSourceNumber;
        Description = trimmedDescription;
        Amount = roundedAmount;
        IsReversal = isReversal;
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc.ToUniversalTime();
    }

    public ProjectCostLedgerEntry CreateReversal(Guid id, Guid actorUserId, DateTimeOffset occurredAtUtc, string? description = null) =>
        new(
            id,
            OrganizationId,
            ProjectId,
            WbsNodeId,
            Category,
            EntryType,
            SourceType,
            SourceId,
            SourceNumber,
            string.IsNullOrWhiteSpace(description) ? $"Reversal of {SourceNumber}" : description,
            -Amount,
            true,
            actorUserId,
            occurredAtUtc);
}
