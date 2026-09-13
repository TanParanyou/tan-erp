namespace TanErp.Application.Surveys;

public sealed record SiteSurveyMeasurementProjection(
    Guid Id,
    Guid SiteSurveyAreaId,
    string MeasurementType,
    decimal Value,
    string UnitCode,
    string CaptureMethod,
    string? Notes,
    int SortOrder);

public sealed record SiteSurveyAreaProjection(
    Guid Id,
    Guid SiteSurveyRevisionId,
    string Code,
    string Name,
    string? Description,
    int SortOrder,
    IReadOnlyList<SiteSurveyMeasurementProjection> Measurements);

public sealed record SiteSurveyRevisionProjection(
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
    IReadOnlyList<SiteSurveyAreaProjection>? Areas = null);


public sealed record SurveyorSummaryProjection(
    Guid Id,
    string DisplayName,
    string? Email);

public sealed record SiteSummaryProjection(
    Guid Id,
    string Label,
    string AddressLine1);

public sealed record SiteSurveyProjection(
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
    SiteSurveyRevisionProjection? CurrentRevision = null,
    SurveyorSummaryProjection? AssignedSurveyor = null,
    SiteSummaryProjection? Site = null);
