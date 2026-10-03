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
}
