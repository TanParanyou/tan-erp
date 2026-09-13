using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.Surveys.CreateSiteSurvey;

public class CreateSiteSurveyHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISiteSurveyStore _store;

    public CreateSiteSurveyHandler(
        IRequestAccessResolver accessResolver,
        ISiteSurveyStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    public async Task<Result<SiteSurveyProjection>> Handle(
        CreateSiteSurveyCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Resolve required surveys.create permission
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "surveys.create",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<SiteSurveyProjection>.Failure(accessResult.Error);
        }

        var access = accessResult.Value!;

        // 2. Validate Active Branch requirement
        if (!access.BranchId.HasValue)
        {
            return Result<SiteSurveyProjection>.Failure(
                new Error("ACTIVE_BRANCH_REQUIRED", "An active branch is required to create a survey appointment."));
        }

        // 3. Field validations
        if (command.OpportunityId == Guid.Empty)
        {
            return Result<SiteSurveyProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Opportunity ID is required."));
        }

        if (command.SiteId == Guid.Empty)
        {
            return Result<SiteSurveyProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Site ID is required."));
        }

        if (command.AssignedSurveyorId == Guid.Empty)
        {
            return Result<SiteSurveyProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Assigned surveyor ID is required."));
        }

        if (command.ExpectedOpportunityVersion == Guid.Empty)
        {
            return Result<SiteSurveyProjection>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", "Expected opportunity version is required."));
        }

        if (command.ScheduledStartUtc.HasValue && command.ScheduledEndUtc.HasValue &&
            command.ScheduledEndUtc.Value <= command.ScheduledStartUtc.Value)
        {
            return Result<SiteSurveyProjection>.Failure(
                new Error("SURVEY_SCHEDULE_INVALID", "Scheduled end time must be strictly after scheduled start time."));
        }

        // 4. Compute deterministic hashes
        var keyHash = Sha256Hex.Compute(command.IdempotencyKey);
        var startStr = command.ScheduledStartUtc?.ToUniversalTime().ToString("O") ?? "";
        var endStr = command.ScheduledEndUtc?.ToUniversalTime().ToString("O") ?? "";
        var canonicalPayload = $"{command.OpportunityId}|{command.SiteId}|{command.AssignedSurveyorId}|{startStr}|{endStr}|{command.ExpectedOpportunityVersion}";
        var payloadHash = Sha256Hex.Compute(canonicalPayload);

        // 5. Delegate to atomic store
        return await _store.CreateAsync(access, command, keyHash, payloadHash, cancellationToken);
    }
}
