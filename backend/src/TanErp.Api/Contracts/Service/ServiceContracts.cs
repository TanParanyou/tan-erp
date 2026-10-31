namespace TanErp.Api.Contracts.Service;

public sealed record ServicePersonResponse(Guid Id, string DisplayName);

public sealed record ServiceProjectRefResponse(Guid Id, string Code, string Name);

public sealed record ChecklistItemResponse(Guid Id, int SortOrder, string Title, bool Required, bool Done, ServicePersonResponse? DoneBy, DateTimeOffset? DoneAtUtc);

public sealed record DefectResponse(Guid Id, int No, string Description, string Severity, string Status, ServicePersonResponse ReportedBy, DateTimeOffset ReportedAtUtc, ServicePersonResponse? ResolvedBy, DateTimeOffset? ResolvedAtUtc, string? ResolutionNote, ServicePersonResponse? VerifiedBy, DateTimeOffset? VerifiedAtUtc, int ReopenCount, string? LastReopenReason);

public sealed record HandoverResponse(DateOnly Date, string SignerName, string? Note, int WarrantyMonths);

public sealed record WarrantyRefResponse(Guid Id, string Number, DateOnly StartDate, DateOnly EndDate);

public sealed record InstallationResponse(Guid Id, Guid BranchId, string Number, string Status, ServiceProjectRefResponse Project, DateOnly ScheduledStart, DateOnly ScheduledEnd, string? CrewName, string? Note, string? CancelReason, DateTimeOffset? ReadyAtUtc, HandoverResponse? Handover, int DisputeCount, string? LastDisputeNote, WarrantyRefResponse? Warranty, ServicePersonResponse CreatedBy, DateTimeOffset CreatedAtUtc, Guid RowVersion, IReadOnlyList<ChecklistItemResponse> Checklist, IReadOnlyList<DefectResponse> Defects);

public sealed record InstallationListItemResponse(Guid Id, string Number, string Status, string ProjectCode, string ProjectName, DateOnly ScheduledStart, DateOnly ScheduledEnd, int ChecklistDone, int ChecklistTotal, int OpenDefects, DateTimeOffset CreatedAtUtc);

public sealed record ServiceRequestEventResponse(string? FromStatus, string ToStatus, string? Note, ServicePersonResponse Actor, DateTimeOffset OccurredAtUtc);

public sealed record ServiceSlaResponse(int ResponseHours, int ResolveHours, decimal ElapsedHours, long PausedSeconds, string Status);

public sealed record ServiceRequestResponse(
    Guid Id,
    Guid BranchId,
    string Number,
    string Title,
    string Description,
    string Priority,
    string Status,
    bool InWarranty,
    WarrantyRefResponse? Warranty,
    ServiceProjectRefResponse Project,
    DateOnly? ScheduledDate,
    string? ResolutionNote,
    int ReopenCount,
    ServiceSlaResponse Sla,
    decimal ServiceChargeAmount,
    string? ServiceChargeNote,
    Guid? BillingDocumentId,
    string? BillingDocumentNumber,
    ServicePersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid RowVersion,
    IReadOnlyList<ServiceRequestEventResponse> Events);

public sealed record ServiceRequestListItemResponse(
    Guid Id,
    string Number,
    string Title,
    string Priority,
    string Status,
    bool InWarranty,
    string ProjectCode,
    ServiceSlaResponse Sla,
    decimal ServiceChargeAmount,
    Guid? BillingDocumentId,
    string? BillingDocumentNumber,
    DateTimeOffset CreatedAtUtc);

public sealed record WarrantyResponse(Guid Id, string Number, ServiceProjectRefResponse Project, Guid InstallationJobId, string InstallationNumber, DateOnly StartDate, DateOnly EndDate, int Months, bool IsActive, int ServiceRequestCount);

public sealed record WarrantiesListResponse(IReadOnlyList<WarrantyResponse> Items, ServicePaginationResponse Pagination);

public sealed record ServiceRequestsListResponse(IReadOnlyList<ServiceRequestListItemResponse> Items, ServicePaginationResponse Pagination);

public sealed record InstallationsListResponse(IReadOnlyList<InstallationListItemResponse> Items, ServicePaginationResponse Pagination);

public sealed record ServicePaginationResponse(int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record ChecklistItemRequest(string? Title, bool Required);

public sealed record InstallationRequest(Guid ProjectId, DateOnly ScheduledStart, DateOnly ScheduledEnd, string? CrewName, string? Note, List<ChecklistItemRequest>? Checklist);

public sealed record ChecklistDoneRequest(bool Done);

public sealed record DefectRequest(string? Description, string? Severity);

public sealed record NoteRequest(string? Note);

public sealed record ReasonRequest(string? Reason);

public sealed record HandoverRequest(string? Outcome, string? SignerName, string? Note, int? WarrantyMonths, DateOnly? HandoverDate);

public sealed record ServiceRequestRequest(Guid ProjectId, string? Title, string? Description, string? Priority);

public sealed record ScheduleRequest(DateOnly Date);

public sealed record ChargeServiceRequestRequest(decimal Amount, string? Note);

public sealed record CreateTechnicianAssignmentRequest(
    Guid TechnicianUserId,
    DateOnly WorkDate,
    string? AssignmentKind,
    Guid? InstallationJobId,
    Guid? ServiceRequestId,
    string? Note,
    bool AllowConflict = false);

public sealed record TechnicianAssignmentResponse(
    Guid Id,
    Guid BranchId,
    ServicePersonResponse Technician,
    DateOnly WorkDate,
    string AssignmentKind,
    Guid? InstallationJobId,
    string? InstallationJobNumber,
    Guid? ServiceRequestId,
    string? ServiceRequestNumber,
    string? Note,
    ServicePersonResponse CreatedBy,
    DateTimeOffset CreatedAtUtc,
    bool HasConflict);

public sealed record TechnicianSchedulesListResponse(IReadOnlyList<TechnicianAssignmentResponse> Items);

