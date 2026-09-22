using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence;

public class RequestAccessResolver : IRequestAccessResolver
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public RequestAccessResolver(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<RequestAccessContext>> ResolveAsync(
        string firebaseUid,
        Guid membershipId,
        string permissionKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return Result<RequestAccessContext>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        var now = _clock.UtcNow;

        var context = await _db.Memberships
            .AsNoTracking()
            .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .SelectMany(m => m.MembershipRoles
                .Where(mr => mr.Role!.IsActive)
                .SelectMany(mr => mr.Role!.RolePermissions
                    .Where(rp => rp.Permission!.IsActive
                        && rp.Permission.Key == permissionKey
                        && rp.Scope == PermissionScope.Organization
                        && rp.ScopeId == m.OrganizationId)
                    .Select(rp => new RequestAccessContext(
                        m.UserId,
                        m.Id,
                        m.OrganizationId,
                        m.BranchId,
                        rp.Permission!.Key,
                        rp.Scope))))
            .FirstOrDefaultAsync(cancellationToken);

        if (context != null)
        {
            return Result<RequestAccessContext>.Success(context);
        }

        // Check if active membership exists for this user to distinguish ACTIVE_MEMBERSHIP_REQUIRED vs PERMISSION_DENIED
        var activeMembershipExists = await _db.Memberships
            .AsNoTracking()
            .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .AnyAsync(cancellationToken);

        if (!activeMembershipExists)
        {
            return Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "Active organization membership is required."));
        }

        return Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
    }

    public async Task<Result<RequestAccessContext>> ResolveBranchAccessAsync(
        string firebaseUid,
        Guid membershipId,
        string permissionKey,
        Guid targetBranchId,
        CancellationToken cancellationToken = default)
    {
        var baseResult = await ResolveAsync(firebaseUid, membershipId, permissionKey, cancellationToken);
        if (baseResult.IsFailure) return baseResult;

        var context = baseResult.Value!;

        // 1. Verify branch exists in caller's organization
        var branch = await _db.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == targetBranchId && b.OrganizationId == context.OrganizationId, cancellationToken);

        if (branch == null)
        {
            return Result<RequestAccessContext>.Failure(new Error("BRANCH_NOT_FOUND", "Branch not found."));
        }

        // 2. Reject deactivated branches
        if (!branch.IsActive)
        {
            return Result<RequestAccessContext>.Failure(new Error("BRANCH_INACTIVE", "Branch is inactive."));
        }

        // 3. Branch scope: If user is scoped to a branch and attempting to access another branch,
        // verify if user holds organization-wide catalog permissions
        if (context.BranchId.HasValue && context.BranchId.Value != targetBranchId)
        {
            var now = _clock.UtcNow;
            var hasOrgWideScope = await _db.Memberships
                .AsNoTracking()
                .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
                .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
                .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
                .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
                .SelectMany(m => m.MembershipRoles
                    .Where(mr => mr.Role!.IsActive)
                    .SelectMany(mr => mr.Role!.RolePermissions
                        .Where(rp => rp.Permission!.IsActive
                            && (rp.Permission.Key == "items.manage-branches" || rp.Permission.Key == "items.create" || rp.Permission.Key == "organizations.manage")
                            && rp.Scope == PermissionScope.Organization
                            && rp.ScopeId == m.OrganizationId)))
                .AnyAsync(cancellationToken);

            if (!hasOrgWideScope)
            {
                return Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "Access is denied for target branch."));
            }
        }

        return Result<RequestAccessContext>.Success(context);
    }

    public async Task<Result<RequestAccessContext>> ResolveAnyAsync(
        string firebaseUid,
        Guid membershipId,
        IReadOnlyCollection<string> permissionKeys,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return Result<RequestAccessContext>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        var now = _clock.UtcNow;

        var context = await _db.Memberships
            .AsNoTracking()
            .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .SelectMany(m => m.MembershipRoles
                .Where(mr => mr.Role!.IsActive)
                .SelectMany(mr => mr.Role!.RolePermissions
                    .Where(rp => rp.Permission!.IsActive
                        && permissionKeys.Contains(rp.Permission.Key)
                        && rp.Scope == PermissionScope.Organization
                        && rp.ScopeId == m.OrganizationId)
                    .Select(rp => new RequestAccessContext(
                        m.UserId,
                        m.Id,
                        m.OrganizationId,
                        m.BranchId,
                        rp.Permission!.Key,
                        rp.Scope))))
            .FirstOrDefaultAsync(cancellationToken);

        if (context != null)
        {
            return Result<RequestAccessContext>.Success(context);
        }

        var activeMembershipExists = await _db.Memberships
            .AsNoTracking()
            .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .AnyAsync(cancellationToken);

        if (!activeMembershipExists)
        {
            return Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "Active organization membership is required."));
        }

        return Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "Access is denied for the requested operation."));
    }
}
