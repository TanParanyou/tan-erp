using TanErp.Domain.Common;

namespace TanErp.Domain.Crm.Opportunities;

public class Opportunity : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? PrimarySiteId { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string NormalizedTitle { get; private set; } = string.Empty;
    public string? ScopeSummary { get; private set; }
    public IReadOnlyList<string> WorkTypes { get; private set; } = Array.Empty<string>();
    public string? SourceCode { get; private set; }
    public decimal? ExpectedBudget { get; private set; }
    public string? CurrencyCode { get; private set; }
    public DateOnly? TargetDecisionDate { get; private set; }
    public DateTimeOffset? NextActionAtUtc { get; private set; }
    public string? NextActionNote { get; private set; }
    public string Stage { get; private set; } = OpportunityStage.Draft;
    public Guid RowVersion { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    protected Opportunity() { }

    private Opportunity(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid? primarySiteId,
        Guid ownerUserId,
        Guid createdByUserId,
        string title,
        string? scopeSummary,
        IReadOnlyCollection<string> workTypes,
        string? sourceCode,
        decimal? expectedBudget,
        string? currencyCode,
        DateOnly? targetDecisionDate,
        DateTimeOffset? nextActionAtUtc,
        string? nextActionNote,
        DateTimeOffset now) : base(id)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Opportunity title cannot be blank.", nameof(title));

        if (workTypes == null || workTypes.Count == 0)
            throw new ArgumentException("At least one work type is required.", nameof(workTypes));

        var distinctWorkTypes = new List<string>();
        foreach (var wt in workTypes)
        {
            if (!OpportunityWorkType.IsValid(wt))
                throw new ArgumentException($"Invalid work type: '{wt}'.", nameof(workTypes));
            var trimmed = wt.Trim();
            if (!distinctWorkTypes.Contains(trimmed, StringComparer.Ordinal))
            {
                distinctWorkTypes.Add(trimmed);
            }
        }

        if (expectedBudget.HasValue)
        {
            if (expectedBudget.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(expectedBudget), "Expected budget must be positive.");
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required when budget is specified.", nameof(currencyCode));
        }

        OrganizationId = organizationId;
        BranchId = branchId;
        CustomerId = customerId;
        PrimarySiteId = primarySiteId;
        OwnerUserId = ownerUserId;
        CreatedByUserId = createdByUserId;
        Code = GenerateOpportunityCode(id);
        Title = OpportunityNormalizer.CollapseWhitespace(title);
        NormalizedTitle = OpportunityNormalizer.NormalizeTitle(title);
        ScopeSummary = string.IsNullOrWhiteSpace(scopeSummary) ? null : OpportunityNormalizer.CollapseWhitespace(scopeSummary);
        WorkTypes = distinctWorkTypes.AsReadOnly();
        SourceCode = string.IsNullOrWhiteSpace(sourceCode) ? null : sourceCode.Trim();
        ExpectedBudget = expectedBudget;
        CurrencyCode = string.IsNullOrWhiteSpace(currencyCode) ? null : currencyCode.Trim().ToUpperInvariant();
        TargetDecisionDate = targetDecisionDate;
        NextActionAtUtc = nextActionAtUtc;
        NextActionNote = string.IsNullOrWhiteSpace(nextActionNote) ? null : OpportunityNormalizer.CollapseWhitespace(nextActionNote);
        Stage = OpportunityStage.Draft;
        RowVersion = Guid.NewGuid();
        CreatedAtUtc = now;
    }

    public static Opportunity CreateDraft(
        Guid id,
        Guid organizationId,
        Guid branchId,
        Guid customerId,
        Guid? primarySiteId,
        Guid ownerUserId,
        Guid createdByUserId,
        string title,
        string? scopeSummary,
        IReadOnlyCollection<string> workTypes,
        string? sourceCode,
        decimal? expectedBudget,
        string? currencyCode,
        DateOnly? targetDecisionDate,
        DateTimeOffset? nextActionAtUtc,
        string? nextActionNote,
        DateTimeOffset now)
    {
        return new Opportunity(
            id, organizationId, branchId, customerId, primarySiteId, ownerUserId, createdByUserId,
            title, scopeSummary, workTypes, sourceCode, expectedBudget, currencyCode,
            targetDecisionDate, nextActionAtUtc, nextActionNote, now);
    }

    public static string GenerateOpportunityCode(Guid id)
    {
        var hex = id.ToString("N")[..12].ToUpperInvariant();
        return $"OPP-{hex}";
    }

    public override string ToString() => $"Opportunity [Id={Id}, Code={Code}, Title={Title}, Stage={Stage}]";
}
