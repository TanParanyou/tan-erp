using TanErp.Api.Contracts.Crm.Sites;

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

public sealed record SiteSurveyChecklistResultResponse(
    Guid Id,
    string ItemCode,
    string Result,
    string? Note);

public sealed record SiteSurveyEvidenceResponse(
    Guid Id,
    Guid FileId,
    string Kind,
    string? Caption,
    int SortOrder);

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
    IReadOnlyList<SiteSurveyAreaResponse>? Areas = null,
    IReadOnlyList<SiteSurveyChecklistResultResponse>? Checklist = null,
    IReadOnlyList<SiteSurveyEvidenceResponse>? Evidence = null);


public sealed record SiteSurveyReadyRevisionResponse(
    Guid Id,
    int RevisionNumber,
    string? SnapshotHash);

public sealed record SurveyorSummaryResponse(
    Guid Id,
    string DisplayName,
    string? Email);

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
    SiteSummaryResponse? Site = null,
    SiteSurveyReadyRevisionResponse? LatestReadyRevision = null);
