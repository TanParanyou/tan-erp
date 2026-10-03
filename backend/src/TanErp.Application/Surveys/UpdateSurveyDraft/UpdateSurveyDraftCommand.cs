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

public sealed record ChecklistInput(
    string ItemCode,
    string Result,
    string? Note);

public sealed record EvidenceInput(
    Guid FileId,
    string Kind,
    string? Caption,
    int SortOrder);

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
    IReadOnlyList<ChecklistInput>? Checklist,
    IReadOnlyList<EvidenceInput>? Evidence,
    string FirebaseUid,
    Guid MembershipId,
    string TraceId);
