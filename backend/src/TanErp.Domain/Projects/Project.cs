using TanErp.Domain.Common;

namespace TanErp.Domain.Projects;

/// <summary>
/// A project created by handing over an accepted quotation. The baseline fields are an immutable copy of the
/// commercial sources at handover time; later estimate, survey or master-data changes never alter them.
/// </summary>
public class Project : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? SiteId { get; private set; }
    public Guid OpportunityId { get; private set; }
    public Guid QuotationId { get; private set; }
    public Guid EstimateId { get; private set; }
    public Guid EstimateRevisionId { get; private set; }
    public Guid? SiteSurveyRevisionId { get; private set; }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Status { get; private set; } = ProjectStatus.Planned;
    public Guid OwnerUserId { get; private set; }
    public DateOnly? PlannedStartDate { get; private set; }
    public DateOnly? PlannedEndDate { get; private set; }
    public decimal? BaselineBudgetTotal { get; private set; }
    public string? BaselineBudgetHash { get; private set; }
    public DateTimeOffset? BudgetFrozenAtUtc { get; private set; }
    public DateTimeOffset? ActivatedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public string? StatusReason { get; private set; }

    public string BaselineQuotationNumber { get; private set; } = string.Empty;
    public decimal BaselineContractAmount { get; private set; }
    public string? BaselineQuotationSnapshotHash { get; private set; }
    public string? BaselineSurveySnapshotHash { get; private set; }
    public string BaselineHash { get; private set; } = string.Empty;

    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    protected Project() { }

    private Project(Guid id) : base(id) { }

    public static Project CreateFromHandover(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid? siteId,
        Guid opportunityId,
        Guid quotationId,
        Guid estimateId,
        Guid estimateRevisionId,
        Guid? siteSurveyRevisionId,
        string code,
        string name,
        Guid ownerUserId,
        DateOnly? plannedStartDate,
        string quotationNumber,
        decimal contractAmount,
        string? quotationSnapshotHash,
        string? surveySnapshotHash,
        string baselineHash,
        Guid createdByUserId,
        DateTimeOffset now)
    {
        if (id == Guid.Empty) throw new ArgumentException("Project ID cannot be empty.", nameof(id));
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));
        if (branchId == Guid.Empty) throw new ArgumentException("Branch ID cannot be empty.", nameof(branchId));
        if (customerId == Guid.Empty) throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));
        if (opportunityId == Guid.Empty) throw new ArgumentException("Opportunity ID cannot be empty.", nameof(opportunityId));
        if (quotationId == Guid.Empty) throw new ArgumentException("Quotation ID cannot be empty.", nameof(quotationId));
        if (ownerUserId == Guid.Empty) throw new ArgumentException("Owner user ID cannot be empty.", nameof(ownerUserId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Created by user ID cannot be empty.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Project code cannot be blank.", nameof(code));
        if (string.IsNullOrWhiteSpace(baselineHash)) throw new ArgumentException("Baseline hash cannot be blank.", nameof(baselineHash));
        if (contractAmount < 0) throw new ArgumentOutOfRangeException(nameof(contractAmount), "Contract amount cannot be negative.");

        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length == 0)
        {
            throw new ProjectDomainException("PROJECT_FIELD_REQUIRED", "Project name is required.");
        }

        if (trimmedName.Length > 200)
        {
            throw new ProjectDomainException("PROJECT_FIELD_INVALID", "Project name cannot exceed 200 characters.");
        }

        return new Project(id)
        {
            OrganizationId = organizationId,
            BranchId = branchId,
            CustomerId = customerId,
            SiteId = siteId,
            OpportunityId = opportunityId,
            QuotationId = quotationId,
            EstimateId = estimateId,
            EstimateRevisionId = estimateRevisionId,
            SiteSurveyRevisionId = siteSurveyRevisionId,
            Code = code.Trim(),
            Name = trimmedName,
            Status = ProjectStatus.Planned,
            OwnerUserId = ownerUserId,
            PlannedStartDate = plannedStartDate,
            BaselineQuotationNumber = quotationNumber.Trim(),
            BaselineContractAmount = contractAmount,
            BaselineQuotationSnapshotHash = string.IsNullOrWhiteSpace(quotationSnapshotHash) ? null : quotationSnapshotHash.Trim(),
            BaselineSurveySnapshotHash = string.IsNullOrWhiteSpace(surveySnapshotHash) ? null : surveySnapshotHash.Trim(),
            BaselineHash = baselineHash.Trim(),
            RowVersion = Guid.NewGuid(),
            CreatedAtUtc = now.ToUniversalTime(),
            CreatedByUserId = createdByUserId,
            UpdatedAtUtc = now.ToUniversalTime()
        };
    }

    public bool IsBudgetFrozen => BudgetFrozenAtUtc.HasValue;

    public void SetPlan(DateOnly? plannedStart, DateOnly? plannedEnd, DateTimeOffset now)
    {
        if (Status is not (ProjectStatus.Planned or ProjectStatus.Active or ProjectStatus.OnHold))
        {
            throw new ProjectDomainException("PROJECT_INVALID_STATE", $"The plan cannot be changed while the project is '{Status}'.");
        }

        if (plannedStart.HasValue && plannedEnd.HasValue && plannedEnd.Value < plannedStart.Value)
        {
            throw new ProjectDomainException("PROJECT_PLAN_INVALID", "Planned end date cannot be before the planned start date.");
        }

        PlannedStartDate = plannedStart;
        PlannedEndDate = plannedEnd;
        Touch(now);
    }

    /// <summary>Records the baseline budget (planned status only); the totals are frozen when the project is activated.</summary>
    public void SetBaselineBudget(decimal total, string hash, DateTimeOffset now)
    {
        if (Status != ProjectStatus.Planned || IsBudgetFrozen)
        {
            throw new ProjectDomainException("PROJECT_BUDGET_FROZEN", "The baseline budget can only be changed while the project is planned.");
        }

        if (total < 0) throw new ArgumentOutOfRangeException(nameof(total), "Budget total cannot be negative.");
        BaselineBudgetTotal = total;
        BaselineBudgetHash = hash;
        Touch(now);
    }

    public void TransitionTo(string target, string? reason, int budgetLineCount, DateTimeOffset now)
    {
        if (!ProjectStatus.IsValid(target))
        {
            throw new ProjectDomainException("PROJECT_FIELD_INVALID", $"Unknown project status '{target}'.");
        }

        var allowed = Status switch
        {
            ProjectStatus.Planned => new[] { ProjectStatus.Active, ProjectStatus.Cancelled },
            ProjectStatus.Active => new[] { ProjectStatus.OnHold, ProjectStatus.ReadyForHandover, ProjectStatus.Cancelled },
            ProjectStatus.OnHold => new[] { ProjectStatus.Active, ProjectStatus.Cancelled },
            ProjectStatus.ReadyForHandover => new[] { ProjectStatus.Active, ProjectStatus.Completed },
            _ => Array.Empty<string>()
        };

        if (!allowed.Contains(target))
        {
            throw new ProjectDomainException("PROJECT_INVALID_TRANSITION", $"Cannot move a project from '{Status}' to '{target}'.");
        }

        var needsReason = target is ProjectStatus.OnHold or ProjectStatus.Cancelled
            || (Status == ProjectStatus.ReadyForHandover && target == ProjectStatus.Active);
        if (needsReason && string.IsNullOrWhiteSpace(reason))
        {
            throw new ProjectDomainException("PROJECT_REASON_REQUIRED", "A reason is required for this transition.");
        }

        if (Status == ProjectStatus.Planned && target == ProjectStatus.Active)
        {
            if (!PlannedStartDate.HasValue || !PlannedEndDate.HasValue)
            {
                throw new ProjectDomainException("PROJECT_NOT_READY", "Planned start and end dates are required before activation.");
            }

            if (budgetLineCount == 0 || BaselineBudgetTotal is null)
            {
                throw new ProjectDomainException("PROJECT_NOT_READY", "A baseline budget is required before activation.");
            }

            BudgetFrozenAtUtc = now.ToUniversalTime();
            ActivatedAtUtc = now.ToUniversalTime();
        }

        if (target == ProjectStatus.Completed)
        {
            CompletedAtUtc = now.ToUniversalTime();
        }

        Status = target;
        StatusReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        Touch(now);
    }

    public void Touch(DateTimeOffset now)
    {
        RowVersion = Guid.NewGuid();
        UpdatedAtUtc = now.ToUniversalTime();
    }
}
