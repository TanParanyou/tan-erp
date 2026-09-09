using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Crm.Opportunities;

namespace TanErp.Application.Crm.Opportunities.CreateOpportunity;

public class CreateOpportunityHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public CreateOpportunityHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityProjection>> Handle(
        CreateOpportunityCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required opportunities.create permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "opportunities.create",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<OpportunityProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Validate Active Branch requirement
        if (!access.BranchId.HasValue)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to create an opportunity."));
        }

        // 3. Field validations
        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Opportunity title is required."));
        }

        if (command.WorkTypes == null || command.WorkTypes.Count == 0)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "At least one work type is required."));
        }

        foreach (var wt in command.WorkTypes)
        {
            if (!OpportunityWorkType.IsValid(wt))
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", $"Invalid work type: '{wt}'."));
            }
        }

        if (command.ExpectedBudget.HasValue)
        {
            if (command.ExpectedBudget.Value <= 0)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected budget must be positive."));
            }

            if (string.IsNullOrWhiteSpace(command.CurrencyCode) || command.CurrencyCode.Trim().Length != 3)
            {
                return Result<OpportunityProjection>.Failure(
                    new Error("OPPORTUNITY_FIELD_REQUIRED", "Currency code must be a 3-letter ISO code when expected budget is provided."));
            }
        }

        // 4. Compute deterministic hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var normTitle = OpportunityNormalizer.CollapseWhitespace(command.Title);
        var normScope = command.ScopeSummary != null ? OpportunityNormalizer.CollapseWhitespace(command.ScopeSummary) : "";
        var distinctWorkTypes = command.WorkTypes.Select(w => w.Trim().ToLowerInvariant()).Distinct().OrderBy(w => w).ToList();
        var workTypesJoined = string.Join(",", distinctWorkTypes);
        var sourceCode = command.SourceCode?.Trim() ?? "";
        var budgetStr = command.ExpectedBudget?.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) ?? "";
        var currCode = command.CurrencyCode?.Trim().ToUpperInvariant() ?? "";
        var targetDate = command.TargetDecisionDate?.ToString("yyyy-MM-dd") ?? "";
        var nextActionAt = command.NextActionAtUtc?.ToUniversalTime().ToString("O") ?? "";
        var nextNote = command.NextActionNote != null ? OpportunityNormalizer.CollapseWhitespace(command.NextActionNote) : "";

        var canonicalPayload = $"{command.CustomerId}|{command.PrimarySiteId?.ToString() ?? ""}|{normTitle}|{normScope}|{workTypesJoined}|{sourceCode}|{budgetStr}|{currCode}|{targetDate}|{nextActionAt}|{nextNote}";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        // 5. Delegate to atomic store
        return await _store.CreateAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
