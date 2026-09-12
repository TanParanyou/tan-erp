using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Crm.Opportunities;

namespace TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity;

public class UpdateOpenOpportunityHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public UpdateOpenOpportunityHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityProjection>> Handle(
        UpdateOpenOpportunityCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required opportunities.update permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<OpportunityProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Require active branch
        if (!access.BranchId.HasValue)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to update opportunity."));
        }

        // 3. ExpectedVersion required
        if (command.ExpectedVersion == Guid.Empty)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected version is required."));
        }

        // 4. Validate Title
        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Opportunity title cannot be blank."));
        }

        var normalizedTitle = OpportunityNormalizer.CollapseWhitespace(command.Title);
        if (normalizedTitle.Length > 200)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Opportunity title cannot exceed 200 characters."));
        }

        // 5. Validate work types canonical and non-empty
        if (command.WorkTypes == null || command.WorkTypes.Count == 0)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "At least one work type is required."));
        }

        var canonicalWorkTypes = new List<string>();
        foreach (var wt in command.WorkTypes)
        {
            if (!OpportunityWorkType.IsValid(wt))
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", $"Invalid work type: '{wt}'."));
            }
            var trimmed = wt.Trim();
            if (!canonicalWorkTypes.Contains(trimmed, StringComparer.Ordinal))
            {
                canonicalWorkTypes.Add(trimmed);
            }
        }

        // 6. Normalize text and validate length
        var normalizedScope = string.IsNullOrWhiteSpace(command.ScopeSummary) ? null : OpportunityNormalizer.CollapseWhitespace(command.ScopeSummary);
        if (normalizedScope != null && normalizedScope.Length > 2000)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Scope summary cannot exceed 2000 characters."));
        }

        var normalizedSource = string.IsNullOrWhiteSpace(command.SourceCode) ? null : command.SourceCode.Trim();
        if (normalizedSource != null && normalizedSource.Length > 50)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Source code cannot exceed 50 characters."));
        }

        if (command.ExpectedBudget.HasValue && command.ExpectedBudget.Value < 0)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected budget cannot be negative."));
        }

        var normalizedCurrency = command.ExpectedBudget.HasValue
            ? (string.IsNullOrWhiteSpace(command.CurrencyCode) ? "THB" : command.CurrencyCode.Trim().ToUpperInvariant())
            : null;

        var normalizedNote = string.IsNullOrWhiteSpace(command.NextActionNote) ? null : OpportunityNormalizer.CollapseWhitespace(command.NextActionNote);
        if (normalizedNote != null && normalizedNote.Length > 500)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Next action note cannot exceed 500 characters."));
        }

        // 7. Action date & note pair check
        if ((command.NextActionAtUtc.HasValue && normalizedNote == null) || (!command.NextActionAtUtc.HasValue && normalizedNote != null))
        {
            return Result<OpportunityProjection>.Failure(
                new Error("REQUEST_VALIDATION_FAILED", "Next action date and note must be provided together or both omitted."));
        }

        // 8. Deterministic hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = FormattableString.Invariant(
            $"{command.OpportunityId:D}|{command.ExpectedVersion:D}|{normalizedTitle}|{command.PrimarySiteId?.ToString("D") ?? ""}|{normalizedScope ?? ""}|{string.Join(",", canonicalWorkTypes)}|{normalizedSource ?? ""}|{command.ExpectedBudget?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? ""}|{normalizedCurrency ?? ""}|{command.TargetDecisionDate?.ToString("yyyy-MM-dd") ?? ""}|{command.NextActionAtUtc?.ToUniversalTime().ToString("O") ?? ""}|{normalizedNote ?? ""}");
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        var normalizedCommand = command with
        {
            Title = normalizedTitle,
            ScopeSummary = normalizedScope,
            WorkTypes = canonicalWorkTypes.AsReadOnly(),
            SourceCode = normalizedSource,
            CurrencyCode = normalizedCurrency,
            NextActionAtUtc = command.NextActionAtUtc?.ToUniversalTime(),
            NextActionNote = normalizedNote
        };

        // 9. Delegate to store
        return await _store.UpdateOpenAsync(access, normalizedCommand, keyHash, payloadHash, cancellationToken);
    }
}
