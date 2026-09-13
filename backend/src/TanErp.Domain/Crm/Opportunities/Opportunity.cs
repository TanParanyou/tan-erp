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
        NextActionAtUtc = nextActionAtUtc?.ToUniversalTime();
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

    public void Qualify(Guid expectedVersion)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();
        if (Stage != OpportunityStage.Draft) throw new OpportunityTransitionException(Stage, OpportunityStage.Qualified);
        if (string.IsNullOrWhiteSpace(ScopeSummary)) throw new OpportunityQualificationException(nameof(ScopeSummary));
        if (WorkTypes.Count == 0 || WorkTypes.Any(workType => !OpportunityWorkType.IsValid(workType)))
            throw new OpportunityQualificationException(nameof(WorkTypes));
        if (!NextActionAtUtc.HasValue) throw new OpportunityQualificationException(nameof(NextActionAtUtc));
        if (string.IsNullOrWhiteSpace(NextActionNote)) throw new OpportunityQualificationException(nameof(NextActionNote));
        Stage = OpportunityStage.Qualified;
        RowVersion = Guid.NewGuid();
    }

    public void EnterSurveying(Guid expectedVersion, Guid siteId)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();
        if (Stage != OpportunityStage.Qualified) throw new OpportunityTransitionException(Stage, OpportunityStage.Surveying);
        if (siteId == Guid.Empty) throw new ArgumentException("Site ID cannot be empty.", nameof(siteId));

        if (!PrimarySiteId.HasValue)
        {
            PrimarySiteId = siteId;
        }

        Stage = OpportunityStage.Surveying;
        RowVersion = Guid.NewGuid();
    }

    public void Close(Guid expectedVersion, string targetStage, string reasonCode, string? note = null)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();

        var normalizedTarget = targetStage?.Trim().ToLowerInvariant();
        if (normalizedTarget != OpportunityStage.Lost && normalizedTarget != OpportunityStage.Cancelled)
        {
            throw new OpportunityTransitionException(Stage, targetStage ?? string.Empty);
        }

        if (Stage == OpportunityStage.Won || Stage == OpportunityStage.Lost || Stage == OpportunityStage.Cancelled)
        {
            throw new OpportunityTransitionException(Stage, normalizedTarget);
        }

        if (normalizedTarget == OpportunityStage.Lost)
        {
            if (!OpportunityReasonCodes.IsValidLostReason(reasonCode))
                throw new ArgumentException($"Invalid lost reason code: '{reasonCode}'.", nameof(reasonCode));
        }
        else if (normalizedTarget == OpportunityStage.Cancelled)
        {
            if (!OpportunityReasonCodes.IsValidCancelledReason(reasonCode))
                throw new ArgumentException($"Invalid cancelled reason code: '{reasonCode}'.", nameof(reasonCode));
        }

        Stage = normalizedTarget;
        RowVersion = Guid.NewGuid();
    }

    public void Reopen(Guid expectedVersion, string targetStage, string reasonCode, string? note = null)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();

        if (Stage != OpportunityStage.Lost && Stage != OpportunityStage.Cancelled)
        {
            throw new OpportunityTransitionException(Stage, targetStage ?? string.Empty);
        }

        var normalizedTarget = targetStage?.Trim().ToLowerInvariant();
        if (normalizedTarget != OpportunityStage.Draft && normalizedTarget != OpportunityStage.Qualified)
        {
            throw new OpportunityTransitionException(Stage, targetStage ?? string.Empty);
        }

        if (!OpportunityReasonCodes.IsValidReopenReason(reasonCode))
        {
            throw new ArgumentException($"Invalid reopen reason code: '{reasonCode}'.", nameof(reasonCode));
        }

        Stage = normalizedTarget;
        RowVersion = Guid.NewGuid();
    }

    public void EditDraftQGate(
        Guid expectedVersion,
        string? scopeSummary,
        IReadOnlyCollection<string> workTypes,
        DateTimeOffset? nextActionAtUtc,
        string? nextActionNote)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();
        if (Stage != OpportunityStage.Draft) throw new OpportunityTransitionException(Stage, OpportunityStage.Draft);

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

        var normalizedScope = string.IsNullOrWhiteSpace(scopeSummary) ? null : OpportunityNormalizer.CollapseWhitespace(scopeSummary);
        if (normalizedScope != null && normalizedScope.Length > 2000)
            throw new ArgumentException("Scope summary cannot exceed 2000 characters.", nameof(scopeSummary));

        var normalizedNote = string.IsNullOrWhiteSpace(nextActionNote) ? null : OpportunityNormalizer.CollapseWhitespace(nextActionNote);
        if (normalizedNote != null && normalizedNote.Length > 500)
            throw new ArgumentException("Next action note cannot exceed 500 characters.", nameof(nextActionNote));

        if ((nextActionAtUtc.HasValue && normalizedNote == null) || (!nextActionAtUtc.HasValue && normalizedNote != null))
            throw new ArgumentException("Next action date and note must be provided together or both omitted.");

        ScopeSummary = normalizedScope;
        WorkTypes = distinctWorkTypes.AsReadOnly();
        NextActionAtUtc = nextActionAtUtc?.ToUniversalTime();
        NextActionNote = normalizedNote;
        RowVersion = Guid.NewGuid();
    }

    public void EditOpen(
        Guid expectedVersion,
        string title,
        Guid? primarySiteId,
        string? scopeSummary,
        IReadOnlyCollection<string> workTypes,
        string? sourceCode,
        decimal? expectedBudget,
        string? currencyCode,
        DateOnly? targetDecisionDate,
        DateTimeOffset? nextActionAtUtc,
        string? nextActionNote)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();
        if (Stage == OpportunityStage.Won || Stage == OpportunityStage.Lost || Stage == OpportunityStage.Cancelled)
            throw new OpportunityTransitionException(Stage, Stage);

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

        var normalizedTitle = OpportunityNormalizer.CollapseWhitespace(title);
        if (normalizedTitle.Length > 200)
            throw new ArgumentException("Opportunity title cannot exceed 200 characters.", nameof(title));

        var normalizedScope = string.IsNullOrWhiteSpace(scopeSummary) ? null : OpportunityNormalizer.CollapseWhitespace(scopeSummary);
        if (normalizedScope != null && normalizedScope.Length > 2000)
            throw new ArgumentException("Scope summary cannot exceed 2000 characters.", nameof(scopeSummary));

        var normalizedSource = string.IsNullOrWhiteSpace(sourceCode) ? null : sourceCode.Trim();
        if (normalizedSource != null && normalizedSource.Length > 50)
            throw new ArgumentException("Source code cannot exceed 50 characters.", nameof(sourceCode));

        if (expectedBudget.HasValue && expectedBudget.Value < 0)
            throw new ArgumentException("Expected budget cannot be negative.", nameof(expectedBudget));

        var normalizedCurrency = expectedBudget.HasValue ? (string.IsNullOrWhiteSpace(currencyCode) ? "THB" : currencyCode.Trim().ToUpperInvariant()) : null;

        var normalizedNote = string.IsNullOrWhiteSpace(nextActionNote) ? null : OpportunityNormalizer.CollapseWhitespace(nextActionNote);
        if (normalizedNote != null && normalizedNote.Length > 500)
            throw new ArgumentException("Next action note cannot exceed 500 characters.", nameof(nextActionNote));

        if ((nextActionAtUtc.HasValue && normalizedNote == null) || (!nextActionAtUtc.HasValue && normalizedNote != null))
            throw new ArgumentException("Next action date and note must be provided together or both omitted.");

        Title = normalizedTitle;
        NormalizedTitle = OpportunityNormalizer.NormalizeTitle(normalizedTitle);
        PrimarySiteId = primarySiteId;
        ScopeSummary = normalizedScope;
        WorkTypes = distinctWorkTypes.AsReadOnly();
        SourceCode = normalizedSource;
        ExpectedBudget = expectedBudget;
        CurrencyCode = normalizedCurrency;
        TargetDecisionDate = targetDecisionDate;
        NextActionAtUtc = nextActionAtUtc?.ToUniversalTime();
        NextActionNote = normalizedNote;
        RowVersion = Guid.NewGuid();
    }

    public void ReassignOwner(Guid expectedVersion, Guid newOwnerUserId)
    {
        if (RowVersion != expectedVersion) throw new OpportunityVersionException();
        if (Stage == OpportunityStage.Won || Stage == OpportunityStage.Lost || Stage == OpportunityStage.Cancelled)
            throw new OpportunityTransitionException(Stage, Stage);

        if (newOwnerUserId == Guid.Empty)
            throw new ArgumentException("Target owner user ID cannot be empty.", nameof(newOwnerUserId));

        OwnerUserId = newOwnerUserId;
        RowVersion = Guid.NewGuid();
    }

    public static string GenerateOpportunityCode(Guid id)
    {
        var hex = id.ToString("N")[..12].ToUpperInvariant();
        return $"OPP-{hex}";
    }

    public override string ToString() => $"Opportunity [Id={Id}, Code={Code}, Title={Title}, Stage={Stage}]";
}
