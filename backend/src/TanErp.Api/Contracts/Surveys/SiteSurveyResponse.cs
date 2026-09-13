namespace TanErp.Api.Contracts.Surveys;

public sealed record SiteSurveyMeasurementResponse(
    Guid Id,
    Guid SiteSurveyAreaId,
    string MeasurementType,
    decimal Value,
    string UnitCode,
    string CaptureMethod,
    string? Notes,
    int SortOrder);

public sealed record SiteSurveyAreaResponse(
    Guid Id,
    Guid SiteSurveyRevisionId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    IReadOnlyList<SiteSurveyMeasurementResponse> Measurements);

public sealed record SiteSurveyRevisionResponse(
    Guid Id,
    Guid SiteSurveyId,
    int RevisionNumber,
    string SurveyTemplateVersion,
    DateTimeOffset? VisitedAtUtc,
    string? ScopeSummary,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Constraints,
    IReadOnlyList<string> MissingDetails,
    string Readiness,
    string Status,
    DateTimeOffset? ReadyAtUtc,
    Guid? ReadyByUserId,
    string? SnapshotHash,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    Guid CreatedByUserId,
    IReadOnlyList<SiteSurveyAreaResponse>? Areas = null);


public sealed record SurveyorSummaryResponse(
    Guid Id,
    string DisplayName,
    string? Email);

public sealed record SiteSummaryResponse(
    Guid Id,
    string Label,
    string AddressLine1);

public sealed record SiteSurveyResponse(
    Guid Id,
    Guid OrganizationId,
    Guid BranchId,
    Guid OpportunityId,
    Guid SiteId,
    string SurveyNumber,
    Guid AssignedSurveyorId,
    DateTimeOffset? ScheduledStartUtc,
    DateTimeOffset? ScheduledEndUtc,
    string Status,
    Guid RowVersion,
    DateTimeOffset CreatedAtUtc,
    Guid CreatedByUserId,
    SiteSurveyRevisionResponse? CurrentRevision = null,
    SurveyorSummaryResponse? AssignedSurveyor = null,
    SiteSummaryResponse? Site = null);
