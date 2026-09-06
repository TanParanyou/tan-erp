using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;

namespace TanErp.Infrastructure.Persistence;

public class CurrentUserReader : ICurrentUserReader
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;

    public CurrentUserReader(AppDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<Result<GetCurrentUserResult>> GetAsync(string firebaseUid, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return Result<GetCurrentUserResult>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);

        if (user == null)
        {
            return Result<GetCurrentUserResult>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "No active membership found for user."));
        }

        if (!user.IsActive)
        {
            return Result<GetCurrentUserResult>.Failure(new Error("USER_ACCESS_DISABLED", "User account is disabled."));
        }

        var now = _clock.UtcNow;

        var membershipsData = await _dbContext.Memberships.AsNoTracking()
            .Where(m => m.UserId == user.Id
                && m.IsActive
                && (!m.StartsAtUtc.HasValue || now >= m.StartsAtUtc.Value)
                && (!m.ExpiresAtUtc.HasValue || now < m.ExpiresAtUtc.Value)
                && m.Organization!.IsActive
                && (m.Branch == null || m.Branch.IsActive))
            .Select(m => new
            {
                m.Id,
                OrgId = m.Organization!.Id,
                OrgName = m.Organization.Name,
                BranchId = m.Branch != null ? (Guid?)m.Branch.Id : null,
                BranchName = m.Branch != null ? m.Branch.Name : null,
                RolePermissions = m.MembershipRoles
                    .Where(mr => mr.Role!.IsActive)
                    .SelectMany(mr => mr.Role!.RolePermissions)
                    .Select(rp => new
                    {
                        rp.Permission!.Key,
                        rp.Scope,
                        rp.ScopeId
                    })
            })
            .ToListAsync(cancellationToken);

        if (membershipsData.Count == 0)
        {
            return Result<GetCurrentUserResult>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "No active membership found for user."));
        }

        var membershipContexts = membershipsData
            .OrderBy(m => m.OrgName, StringComparer.Ordinal)
            .ThenBy(m => m.Id)
            .Select(m =>
            {
                var permissions = m.RolePermissions
                    .GroupBy(p => new { p.Key, p.Scope, p.ScopeId })
                    .Select(g => new PermissionContext(g.Key.Key, g.Key.Scope, g.Key.ScopeId))
                    .OrderBy(p => p.Key, StringComparer.Ordinal)
                    .ThenBy(p => p.Scope, StringComparer.Ordinal)
                    .ThenBy(p => p.ScopeId)
                    .ToList();

                return new MembershipContext(
                    m.Id,
                    new OrganizationContext(m.OrgId, m.OrgName),
                    m.BranchId.HasValue ? new BranchContext(m.BranchId.Value, m.BranchName!) : null,
                    permissions);
            })
            .ToList();

        var result = new GetCurrentUserResult(
            new UserContext(user.Id, user.DisplayName, user.Email),
            membershipContexts);

        return Result<GetCurrentUserResult>.Success(result);
    }
}
