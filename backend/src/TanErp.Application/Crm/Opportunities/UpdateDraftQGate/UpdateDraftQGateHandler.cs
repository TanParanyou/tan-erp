using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Crm.Opportunities;

namespace TanErp.Application.Crm.Opportunities.UpdateDraftQGate;

public class UpdateDraftQGateHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IOpportunityStore _store;

    public UpdateDraftQGateHandler(
        IRequestAccessResolver accessResolver,
        IOpportunityStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<OpportunityProjection>> Handle(
        UpdateDraftQGateCommand command,
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
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to update draft opportunity."));
        }

        // 3. ExpectedVersion required
        if (command.ExpectedVersion == Guid.Empty)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Expected version is required."));
        }

        // 4. Validate work types canonical and non-empty
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

        // 5. Normalize text and validate length
        var normalizedScope = string.IsNullOrWhiteSpace(command.ScopeSummary) ? null : OpportunityNormalizer.CollapseWhitespace(command.ScopeSummary);
        if (normalizedScope != null && normalizedScope.Length > 2000)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Scope summary cannot exceed 2000 characters."));
        }

        var normalizedNote = string.IsNullOrWhiteSpace(command.NextActionNote) ? null : OpportunityNormalizer.CollapseWhitespace(command.NextActionNote);
        if (normalizedNote != null && normalizedNote.Length > 500)
        {
            return Result<OpportunityProjection>.Failure(
                new Error("OPPORTUNITY_FIELD_REQUIRED", "Next action note cannot exceed 500 characters."));
        }

        // 6. Action date & note pair check
        if ((command.NextActionAtUtc.HasValue && normalizedNote == null) || (!command.NextActionAtUtc.HasValue && normalizedNote != null))
        {
            return Result<OpportunityProjection>.Failure(
                new Error("REQUEST_VALIDATION_FAILED", "Next action date and note must be provided together or both omitted."));
        }

        // 7. Compute deterministic hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var canonicalPayload = FormattableString.Invariant(
            $"{command.OpportunityId:D}|{command.ExpectedVersion:D}|{normalizedScope ?? ""}|{string.Join(",", canonicalWorkTypes)}|{command.NextActionAtUtc?.ToUniversalTime().ToString("O") ?? ""}|{normalizedNote ?? ""}");
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        var normalizedCommand = command with
        {
            ScopeSummary = normalizedScope,
            WorkTypes = canonicalWorkTypes.AsReadOnly(),
            NextActionAtUtc = command.NextActionAtUtc?.ToUniversalTime(),
            NextActionNote = normalizedNote
        };

        // 8. Delegate to store
        return await _store.UpdateDraftQGateAsync(access, normalizedCommand, keyHash, payloadHash, cancellationToken);
    }
}
