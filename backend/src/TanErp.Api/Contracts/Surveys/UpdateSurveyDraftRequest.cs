namespace TanErp.Api.Contracts.Surveys;

public sealed record UpdateSurveyMeasurementRequest(
    Guid? Id,
    string MeasurementType,
    decimal Value,
    string UnitCode,
    string CaptureMethod,
    string? Notes,
    int SortOrder);

public sealed record UpdateSurveyAreaRequest(
    Guid? Id,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    List<UpdateSurveyMeasurementRequest> Measurements);

public sealed record UpdateSurveyDraftRequest(
    Guid ExpectedRevisionVersion,
    DateTimeOffset? VisitedAtUtc,
    string? ScopeSummary,
    List<string>? Assumptions,
    List<string>? Constraints,
    List<string>? MissingDetails,
    List<UpdateSurveyAreaRequest>? Areas);
