namespace TanErp.Application.Service;

public sealed record ServicePerson(Guid Id, string DisplayName);

public sealed record ServiceProjectRef(Guid Id, string Code, string Name);

// ----- installation ----------------------------------------------------------------------------

public sealed record ChecklistInput(string Title, bool Required);

public sealed record InstallationInput(Guid ProjectId, DateOnly ScheduledStart, DateOnly ScheduledEnd, string? CrewName, string? Note, IReadOnlyList<ChecklistInput> Checklist);

public sealed record ChecklistItemProjection(Guid Id, int SortOrder, string Title, bool Required, bool Done, ServicePerson? DoneBy, DateTimeOffset? DoneAtUtc);

public sealed record DefectProjection(
    Guid Id,
    int No,
    string Description,
    string Severity,
    string Status,
    ServicePerson ReportedBy,
    DateTimeOffset ReportedAtUtc,
    ServicePerson? ResolvedBy,
    DateTimeOffset? ResolvedAtUtc,
    string? ResolutionNote,
    ServicePerson? VerifiedBy,
    DateTimeOffset? VerifiedAtUtc,
    int ReopenCount,
    string? LastReopenReason);

public sealed record HandoverProjection(DateOnly Date, string SignerName, string? Note, int WarrantyMonths);

public sealed record WarrantyRef(Guid Id, string Number, DateOnly StartDate, DateOnly EndDate);

public sealed record InstallationProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Status,
    ServiceProjectRef Project,
    DateOnly ScheduledStart,
    DateOnly ScheduledEnd,
    string? CrewName,
    string? Note,
    string? CancelReason,
    DateTimeOffset? ReadyAtUtc,
    HandoverProjection? Handover,
    int DisputeCount,
    string? LastDisputeNote,
    WarrantyRef? Warranty,
    ServicePerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<ChecklistItemProjection> Checklist,
    IReadOnlyList<DefectProjection> Defects);

public sealed record InstallationListItemProjection(
    Guid Id,
    string Number,
    string Status,
    string ProjectCode,
    string ProjectName,
    DateOnly ScheduledStart,
    DateOnly ScheduledEnd,
    int ChecklistDone,
    int ChecklistTotal,
    int OpenDefects,
    DateTimeOffset CreatedAtUtc);

public sealed record InstallationListQuery(string? Search, string? Status, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedInstallations(IReadOnlyList<InstallationListItemProjection> Items, int TotalCount, int Page, int PageSize);

/// <summary>One thing that can be done to an installation. The handler checks the permission each one needs.</summary>
public abstract record InstallationCommand;

public sealed record StartInstallation : InstallationCommand;

public sealed record SetChecklistItem(Guid ItemId, bool Done) : InstallationCommand;

public sealed record ReportDefect(string Description, string Severity) : InstallationCommand;

public sealed record ResolveDefect(Guid DefectId, string Note) : InstallationCommand;

public sealed record VerifyDefect(Guid DefectId) : InstallationCommand;

public sealed record ReopenDefect(Guid DefectId, string Reason) : InstallationCommand;

public sealed record MarkInstallationReady : InstallationCommand;

public sealed record RecordHandover(string Outcome, string SignerName, string? Note, int? WarrantyMonths, DateOnly? HandoverDate) : InstallationCommand;

public sealed record CancelInstallation(string Reason) : InstallationCommand;

// ----- warranty --------------------------------------------------------------------------------

public sealed record WarrantyProjection(
    Guid Id,
    string Number,
    ServiceProjectRef Project,
    Guid InstallationJobId,
    string InstallationNumber,
    DateOnly StartDate,
    DateOnly EndDate,
    int Months,
    bool IsActive,
    int ServiceRequestCount);

public sealed record WarrantyListQuery(string? Search, string? State, Guid? ProjectId, int Page, int PageSize);

public sealed record PagedWarranties(IReadOnlyList<WarrantyProjection> Items, int TotalCount, int Page, int PageSize);

// ----- service requests ------------------------------------------------------------------------

public sealed record ServiceRequestInput(Guid ProjectId, string Title, string Description, string Priority);

public sealed record ServiceRequestEventProjection(string? FromStatus, string ToStatus, string? Note, ServicePerson Actor, DateTimeOffset OccurredAtUtc);

public sealed record ServiceRequestProjection(
    Guid Id,
    Guid BranchId,
    string Number,
    string Title,
    string Description,
    string Priority,
    string Status,
    bool InWarranty,
    WarrantyRef? Warranty,
    ServiceProjectRef Project,
    DateOnly? ScheduledDate,
    string? ResolutionNote,
    int ReopenCount,
    ServicePerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<ServiceRequestEventProjection> Events);

public sealed record ServiceRequestListItemProjection(
    Guid Id,
    string Number,
    string Title,
    string Priority,
    string Status,
    bool InWarranty,
    string ProjectCode,
    DateTimeOffset CreatedAtUtc);

public sealed record ServiceRequestListQuery(string? Search, string? Status, Guid? ProjectId, bool? InWarranty, int Page, int PageSize);

public sealed record PagedServiceRequests(IReadOnlyList<ServiceRequestListItemProjection> Items, int TotalCount, int Page, int PageSize);

public abstract record ServiceRequestCommand;

public sealed record ScheduleServiceRequest(DateOnly Date) : ServiceRequestCommand;

public sealed record StartServiceRequest : ServiceRequestCommand;

public sealed record ResolveServiceRequest(string Note) : ServiceRequestCommand;

public sealed record CloseServiceRequest : ServiceRequestCommand;

public sealed record ReopenServiceRequest(string Reason) : ServiceRequestCommand;
