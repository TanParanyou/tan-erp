using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Application.IdentityAccess.RoleManagement;

namespace TanErp.Api.Contracts.IdentityAccess.RoleManagement;

public record PermissionCatalogEntryResponse(string Key, string? Description, bool IsApprovalPermission);

public record PermissionCatalogResponse(IReadOnlyList<PermissionCatalogEntryResponse> Items)
{
    public static PermissionCatalogResponse From(IReadOnlyList<PermissionCatalogEntry> items) =>
        new(items.Select(i => new PermissionCatalogEntryResponse(i.Key, i.Description, i.IsApprovalPermission)).ToList());
}

public record RoleDefinitionSummaryResponse(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion,
    int MemberCount, int PermissionCount, bool RequiresApproval, bool HasPendingChange);

public record RoleDefinitionListResponse(IReadOnlyList<RoleDefinitionSummaryResponse> Items, AdminPaginationResponse Pagination)
{
    public static RoleDefinitionListResponse From(RoleDefinitionPage page) => new(
        page.Items.Select(i => new RoleDefinitionSummaryResponse(
            i.Id, i.Name, i.Description, i.IsSystem, i.IsActive, i.RowVersion, i.MemberCount, i.PermissionCount, i.RequiresApproval, i.HasPendingChange)).ToList(),
        new AdminPaginationResponse(page.Page, page.PageSize, page.TotalCount, (int)Math.Ceiling(page.TotalCount / (double)page.PageSize)));
}

public record RoleGrantRequest(string? PermissionKey, string? Scope, Guid? BranchId);

public record SaveRoleDefinitionRequest(string? Name, string? Description, IReadOnlyList<RoleGrantRequest>? Permissions)
{
    public RoleInput ToInput() => new(
        Name ?? string.Empty,
        Description,
        (Permissions ?? Array.Empty<RoleGrantRequest>())
            .Select(g => new RoleGrantInput(g.PermissionKey ?? string.Empty, g.Scope ?? string.Empty, g.BranchId)).ToList());
}

public record RoleGrantResponse(string PermissionKey, string Scope, AdminRefResponse? Branch);

public record RoleChangeRequestResponse(
    Guid Id, Guid RowVersion, string Status, AdminRefResponse Role, AdminRefResponse RequestedBy, DateTimeOffset RequestedAtUtc,
    string ProposedName, string? ProposedDescription,
    IReadOnlyList<string> AddedPermissionKeys, IReadOnlyList<string> RemovedPermissionKeys)
{
    public static RoleChangeRequestResponse From(RoleChangeRequestView v) => new(
        v.Id, v.RowVersion, v.Status, new AdminRefResponse(v.Role.Id, v.Role.Name), new AdminRefResponse(v.RequestedBy.Id, v.RequestedBy.Name),
        v.RequestedAtUtc, v.ProposedName, v.ProposedDescription, v.AddedPermissionKeys, v.RemovedPermissionKeys);
}

public record RoleChangeRequestListResponse(IReadOnlyList<RoleChangeRequestResponse> Items)
{
    public static RoleChangeRequestListResponse From(IReadOnlyList<RoleChangeRequestView> items) =>
        new(items.Select(i => RoleChangeRequestResponse.From(i)).ToList());
}

public record UpdateRoleDefinitionResponse(RoleDefinitionResponse Role, RoleChangeRequestResponse? PendingRequest);

public record RoleDefinitionResponse(
    Guid Id, string Name, string? Description, bool IsSystem, bool IsActive, Guid RowVersion, DateTimeOffset CreatedAtUtc,
    int MemberCount, int PendingAssignmentRequestCount, bool RequiresApproval,
    IReadOnlyList<RoleGrantResponse> Permissions, RoleChangeRequestResponse? PendingChange)
{
    public static RoleDefinitionResponse From(RoleDefinition r) => new(
        r.Id, r.Name, r.Description, r.IsSystem, r.IsActive, r.RowVersion, r.CreatedAtUtc, r.MemberCount, r.PendingAssignmentRequestCount, r.RequiresApproval,
        r.Permissions.Select(p => new RoleGrantResponse(p.PermissionKey, p.Scope, p.Branch is null ? null : new AdminRefResponse(p.Branch.Id, p.Branch.Name))).ToList(),
        r.PendingChange is null ? null : RoleChangeRequestResponse.From(r.PendingChange));
}
