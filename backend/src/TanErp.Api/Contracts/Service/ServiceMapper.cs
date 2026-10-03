using TanErp.Application.Service;

namespace TanErp.Api.Contracts.Service;

internal static class ServiceMapper
{
    public static ServicePersonResponse To(ServicePerson x) => new(x.Id, x.DisplayName);
    public static ServiceProjectRefResponse To(ServiceProjectRef x) => new(x.Id, x.Code, x.Name);
    public static ChecklistItemResponse To(ChecklistItemProjection x) => new(x.Id, x.SortOrder, x.Title, x.Required, x.Done, x.DoneBy is null ? null : To(x.DoneBy), x.DoneAtUtc);
    public static DefectResponse To(DefectProjection x) => new(x.Id, x.No, x.Description, x.Severity, x.Status, To(x.ReportedBy), x.ReportedAtUtc, x.ResolvedBy is null ? null : To(x.ResolvedBy), x.ResolvedAtUtc, x.ResolutionNote, x.VerifiedBy is null ? null : To(x.VerifiedBy), x.VerifiedAtUtc, x.ReopenCount, x.LastReopenReason);
    public static HandoverResponse To(HandoverProjection x) => new(x.Date, x.SignerName, x.Note, x.WarrantyMonths);
    public static WarrantyRefResponse To(WarrantyRef x) => new(x.Id, x.Number, x.StartDate, x.EndDate);
    public static InstallationResponse To(InstallationProjection x) => new(x.Id, x.BranchId, x.Number, x.Status, To(x.Project), x.ScheduledStart, x.ScheduledEnd, x.CrewName, x.Note, x.CancelReason, x.ReadyAtUtc, x.Handover is null ? null : To(x.Handover), x.DisputeCount, x.LastDisputeNote, x.Warranty is null ? null : To(x.Warranty), To(x.CreatedBy), x.CreatedAtUtc, x.RowVersion, x.Checklist.Select(To).ToList(), x.Defects.Select(To).ToList());
    public static InstallationListItemResponse To(InstallationListItemProjection x) => new(x.Id, x.Number, x.Status, x.ProjectCode, x.ProjectName, x.ScheduledStart, x.ScheduledEnd, x.ChecklistDone, x.ChecklistTotal, x.OpenDefects, x.CreatedAtUtc);
    public static ServiceRequestEventResponse To(ServiceRequestEventProjection x) => new(x.FromStatus, x.ToStatus, x.Note, To(x.Actor), x.OccurredAtUtc);
    public static ServiceRequestResponse To(ServiceRequestProjection x) => new(x.Id, x.BranchId, x.Number, x.Title, x.Description, x.Priority, x.Status, x.InWarranty, x.Warranty is null ? null : To(x.Warranty), To(x.Project), x.ScheduledDate, x.ResolutionNote, x.ReopenCount, To(x.CreatedBy), x.CreatedAtUtc, x.RowVersion, x.Events.Select(To).ToList());
    public static ServiceRequestListItemResponse To(ServiceRequestListItemProjection x) => new(x.Id, x.Number, x.Title, x.Priority, x.Status, x.InWarranty, x.ProjectCode, x.CreatedAtUtc);
    public static WarrantyResponse To(WarrantyProjection x) => new(x.Id, x.Number, To(x.Project), x.InstallationJobId, x.InstallationNumber, x.StartDate, x.EndDate, x.Months, x.IsActive, x.ServiceRequestCount);
    public static ServicePaginationResponse Pagination(int page, int pageSize, int total) => new(page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
}
