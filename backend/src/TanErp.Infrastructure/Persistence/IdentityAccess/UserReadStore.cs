using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.IdentityAccess.Users;

namespace TanErp.Infrastructure.Persistence.IdentityAccess;

public class UserReadStore : IUserReadStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public UserReadStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<UserListItem>> ListActiveUsersAsync(
        Guid organizationId,
        Guid? branchId,
        string? search,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var now = _clock.UtcNow;

        var query = _db.Users
            .AsNoTracking()
            .Where(u => u.IsActive && _db.Memberships.Any(m =>
                m.UserId == u.Id &&
                m.OrganizationId == organizationId &&
                (!branchId.HasValue || m.BranchId == branchId.Value) &&
                m.IsActive &&
                (m.StartsAtUtc == null || m.StartsAtUtc <= now) &&
                (m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.DisplayName, $"%{search}%") ||
                EF.Functions.ILike(u.Email, $"%{search}%"));
        }

        var items = await query
            .OrderBy(u => u.DisplayName)
            .ThenBy(u => u.Id)
            .Take(limit)
            .Select(u => new UserListItem(
                u.Id,
                u.DisplayName,
                u.Email,
                branchId))
            .ToListAsync(cancellationToken);

        return items;
    }
}
