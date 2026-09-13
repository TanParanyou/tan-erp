namespace TanErp.Application.Surveys.UpdateSurveyDraft;

public sealed record MeasurementInput(
    Guid? Id,
    string MeasurementType,
    decimal Value,
    string UnitCode,
    string CaptureMethod,
    string? Notes,
    int SortOrder);

public sealed record AreaInput(
    Guid? Id,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    IReadOnlyList<MeasurementInput> Measurements);

public sealed record UpdateSurveyDraftCommand(
    Guid SiteSurveyId,
    Guid RevisionId,
    Guid ExpectedRevisionVersion,
    DateTimeOffset? VisitedAtUtc,
    string? ScopeSummary,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Constraints,
    IReadOnlyList<string> MissingDetails,
    IReadOnlyList<AreaInput> Areas,
    string FirebaseUid,
    Guid MembershipId,
    string TraceId);
