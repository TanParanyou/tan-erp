using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>A post-sale scope change. Approved change orders adjust the project's current budget and contract value; the baseline never changes.</summary>
public class ProjectChangeOrder : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public decimal BudgetDelta { get; private set; }
    public decimal ContractDelta { get; private set; }
    public string Status { get; private set; } = ChangeOrderStatus.Draft;
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }
    public string? DecisionNote { get; private set; }
    public Guid RowVersion { get; private set; }

    protected ProjectChangeOrder() { }

    public ProjectChangeOrder(
        Guid id, Guid organizationId, Guid projectId, string number, string title, string reason,
        decimal budgetDelta, decimal contractDelta, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (projectId == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Change order number cannot be blank.", nameof(number));

        var trimmedTitle = title?.Trim() ?? string.Empty;
        var trimmedReason = reason?.Trim() ?? string.Empty;
        if (trimmedTitle.Length == 0 || trimmedTitle.Length > 200 || trimmedReason.Length == 0 || trimmedReason.Length > 1000)
        {
            throw new ProjectDomainException("PROJECT_CHANGE_ORDER_INVALID", "A title (≤200) and a reason (≤1000) are required.");
        }

        OrganizationId = organizationId;
        ProjectId = projectId;
        Number = number.Trim();
        Title = trimmedTitle;
        Reason = trimmedReason;
        BudgetDelta = decimal.Round(budgetDelta, 2);
        ContractDelta = decimal.Round(contractDelta, 2);
        Status = ChangeOrderStatus.Draft;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    public void Submit(DateTimeOffset now)
    {
        if (Status != ChangeOrderStatus.Draft)
        {
            throw new ProjectDomainException("PROJECT_CHANGE_ORDER_INVALID_STATE", $"Only a draft change order can be submitted; current status is '{Status}'.");
        }

        Status = ChangeOrderStatus.Submitted;
        SubmittedAtUtc = now.ToUniversalTime();
        RowVersion = Guid.NewGuid();
    }

    /// <summary>Approve or reject. The decider must differ from the creator (maker-checker).</summary>
    public void Decide(bool approve, Guid deciderUserId, string? note, DateTimeOffset now)
    {
        if (Status != ChangeOrderStatus.Submitted)
        {
            throw new ProjectDomainException("PROJECT_CHANGE_ORDER_INVALID_STATE", $"Only a submitted change order can be decided; current status is '{Status}'.");
        }

        if (deciderUserId == CreatedByUserId)
        {
            throw new ProjectDomainException("PROJECT_CHANGE_ORDER_SELF_APPROVAL", "A change order cannot be decided by its creator.");
        }

        if (!approve && string.IsNullOrWhiteSpace(note))
        {
            throw new ProjectDomainException("PROJECT_REASON_REQUIRED", "A note is required to reject a change order.");
        }

        Status = approve ? ChangeOrderStatus.Approved : ChangeOrderStatus.Rejected;
        DecidedByUserId = deciderUserId;
        DecidedAtUtc = now.ToUniversalTime();
        DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RowVersion = Guid.NewGuid();
    }

    public void Cancel()
    {
        if (Status is not (ChangeOrderStatus.Draft or ChangeOrderStatus.Submitted))
        {
            throw new ProjectDomainException("PROJECT_CHANGE_ORDER_INVALID_STATE", $"A {Status} change order cannot be cancelled.");
        }

        Status = ChangeOrderStatus.Cancelled;
        RowVersion = Guid.NewGuid();
    }
}
