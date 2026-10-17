using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Application.Audit;

namespace TanErp.Api.Contracts.Audit;

public record AuditEventResponse(
    Guid Id, DateTimeOffset OccurredAtUtc, string Action, string ResourceType, string ResourceId, AdminRefResponse Actor,
    Guid? BranchId, string TraceId, string? RequestId, string? Reason, string ChangesJson)
{
    public static AuditEventResponse From(AuditEventView v) => new(
        v.Id, v.OccurredAtUtc, v.Action, v.ResourceType, v.ResourceId, new AdminRefResponse(v.Actor.Id, v.Actor.Name),
        v.BranchId, v.TraceId, v.RequestId, v.Reason, v.ChangesJson);
}

public record AuditEventListResponse(IReadOnlyList<AuditEventResponse> Items, AdminPaginationResponse Pagination)
{
    public static AuditEventListResponse From(AuditEventPage page) => new(
        page.Items.Select(AuditEventResponse.From).ToList(),
        new AdminPaginationResponse(page.Page, page.PageSize, page.TotalCount, (int)Math.Ceiling(page.TotalCount / (double)page.PageSize)));
}
