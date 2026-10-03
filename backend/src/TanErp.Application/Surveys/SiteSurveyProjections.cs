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

public sealed record SiteSurveyChecklistResultProjection(
    Guid Id,
    string ItemCode,
    string Result,
    string? Note);

public sealed record SiteSurveyEvidenceProjection(
    Guid Id,
    Guid FileId,
    string Kind,
    string? Caption,
    int SortOrder);

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
    IReadOnlyList<SiteSurveyAreaProjection>? Areas = null,
    IReadOnlyList<SiteSurveyChecklistResultProjection>? Checklist = null,
    IReadOnlyList<SiteSurveyEvidenceProjection>? Evidence = null);


public sealed record SiteSurveyReadyRevisionProjection(
    Guid Id,
    int RevisionNumber,
    string? SnapshotHash);

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
    SiteSummaryProjection? Site = null,
    SiteSurveyReadyRevisionProjection? LatestReadyRevision = null);
