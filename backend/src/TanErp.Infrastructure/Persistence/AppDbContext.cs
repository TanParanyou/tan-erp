using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Domain.Common;
using TanErp.Domain.Files;
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
    public DbSet<TanErp.Domain.Crm.Opportunities.OpportunityStageHistory> OpportunityStageHistories => Set<TanErp.Domain.Crm.Opportunities.OpportunityStageHistory>();
    public DbSet<TanErp.Domain.Surveys.SiteSurvey> SiteSurveys => Set<TanErp.Domain.Surveys.SiteSurvey>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyRevision> SiteSurveyRevisions => Set<TanErp.Domain.Surveys.SiteSurveyRevision>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyArea> SiteSurveyAreas => Set<TanErp.Domain.Surveys.SiteSurveyArea>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyMeasurement> SiteSurveyMeasurements => Set<TanErp.Domain.Surveys.SiteSurveyMeasurement>();
    public DbSet<TanErp.Domain.Estimates.Estimate> Estimates => Set<TanErp.Domain.Estimates.Estimate>();
    public DbSet<TanErp.Domain.Estimates.EstimateRevision> EstimateRevisions => Set<TanErp.Domain.Estimates.EstimateRevision>();
    public DbSet<TanErp.Domain.Estimates.EstimateSection> EstimateSections => Set<TanErp.Domain.Estimates.EstimateSection>();
    public DbSet<TanErp.Domain.Estimates.EstimateWorkItem> EstimateWorkItems => Set<TanErp.Domain.Estimates.EstimateWorkItem>();
    public DbSet<TanErp.Domain.Estimates.EstimateCostComponent> EstimateCostComponents => Set<TanErp.Domain.Estimates.EstimateCostComponent>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<TanErp.Domain.MasterData.Geography.Province> Provinces => Set<TanErp.Domain.MasterData.Geography.Province>();
    public DbSet<TanErp.Domain.MasterData.Geography.District> Districts => Set<TanErp.Domain.MasterData.Geography.District>();
    public DbSet<TanErp.Domain.MasterData.Geography.Subdistrict> Subdistricts => Set<TanErp.Domain.MasterData.Geography.Subdistrict>();
    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();

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
