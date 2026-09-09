using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<MembershipRole> MembershipRoles => Set<MembershipRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<TanErp.Domain.Crm.Customers.Customer> Customers => Set<TanErp.Domain.Crm.Customers.Customer>();
    public DbSet<TanErp.Domain.Crm.Customers.CustomerContact> CustomerContacts => Set<TanErp.Domain.Crm.Customers.CustomerContact>();
    public DbSet<TanErp.Domain.Crm.Sites.Site> Sites => Set<TanErp.Domain.Crm.Sites.Site>();
    public DbSet<TanErp.Domain.Crm.Opportunities.Opportunity> Opportunities => Set<TanErp.Domain.Crm.Opportunities.Opportunity>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    public void AddAuditEvent(AuditEvent auditEvent)
    {
        AuditEvents.Add(auditEvent);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
