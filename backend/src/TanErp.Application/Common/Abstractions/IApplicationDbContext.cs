using Microsoft.EntityFrameworkCore;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Membership> Memberships { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<MembershipRole> MembershipRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }

    void AddAuditEvent(AuditEvent auditEvent);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
