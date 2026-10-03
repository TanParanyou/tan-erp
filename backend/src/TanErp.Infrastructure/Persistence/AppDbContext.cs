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
    public DbSet<RoleAssignmentRequest> RoleAssignmentRequests => Set<RoleAssignmentRequest>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<TanErp.Domain.Crm.Customers.Customer> Customers => Set<TanErp.Domain.Crm.Customers.Customer>();
    public DbSet<TanErp.Domain.Crm.Customers.CustomerContact> CustomerContacts => Set<TanErp.Domain.Crm.Customers.CustomerContact>();
    public DbSet<TanErp.Domain.Crm.Customers.CustomerAddress> CustomerAddresses => Set<TanErp.Domain.Crm.Customers.CustomerAddress>();
    public DbSet<TanErp.Domain.Crm.Sites.Site> Sites => Set<TanErp.Domain.Crm.Sites.Site>();
    public DbSet<TanErp.Domain.Crm.Sites.SiteImage> SiteImages => Set<TanErp.Domain.Crm.Sites.SiteImage>();
    public DbSet<TanErp.Domain.Crm.Opportunities.Opportunity> Opportunities => Set<TanErp.Domain.Crm.Opportunities.Opportunity>();
    public DbSet<TanErp.Domain.Crm.Opportunities.OpportunityStageHistory> OpportunityStageHistories => Set<TanErp.Domain.Crm.Opportunities.OpportunityStageHistory>();
    public DbSet<TanErp.Domain.Crm.Opportunities.OpportunityWorkImage> OpportunityWorkImages => Set<TanErp.Domain.Crm.Opportunities.OpportunityWorkImage>();
    public DbSet<TanErp.Domain.Surveys.SiteSurvey> SiteSurveys => Set<TanErp.Domain.Surveys.SiteSurvey>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyRevision> SiteSurveyRevisions => Set<TanErp.Domain.Surveys.SiteSurveyRevision>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyChecklistResult> SiteSurveyChecklistResults => Set<TanErp.Domain.Surveys.SiteSurveyChecklistResult>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyEvidence> SiteSurveyEvidence => Set<TanErp.Domain.Surveys.SiteSurveyEvidence>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyArea> SiteSurveyAreas => Set<TanErp.Domain.Surveys.SiteSurveyArea>();
    public DbSet<TanErp.Domain.Surveys.SiteSurveyMeasurement> SiteSurveyMeasurements => Set<TanErp.Domain.Surveys.SiteSurveyMeasurement>();
    public DbSet<TanErp.Domain.Projects.Project> Projects => Set<TanErp.Domain.Projects.Project>();
    public DbSet<TanErp.Domain.Projects.ProjectBudgetLine> ProjectBudgetLines => Set<TanErp.Domain.Projects.ProjectBudgetLine>();
    public DbSet<TanErp.Domain.Projects.ProjectMilestone> ProjectMilestones => Set<TanErp.Domain.Projects.ProjectMilestone>();
    public DbSet<TanErp.Domain.Projects.ProjectChangeOrder> ProjectChangeOrders => Set<TanErp.Domain.Projects.ProjectChangeOrder>();
    public DbSet<TanErp.Domain.Projects.ProjectStatusHistory> ProjectStatusHistories => Set<TanErp.Domain.Projects.ProjectStatusHistory>();
    public DbSet<TanErp.Domain.Estimates.Estimate> Estimates => Set<TanErp.Domain.Estimates.Estimate>();
    public DbSet<TanErp.Domain.Estimates.EstimateRevision> EstimateRevisions => Set<TanErp.Domain.Estimates.EstimateRevision>();
    public DbSet<TanErp.Domain.Estimates.EstimateCalculationSnapshot> EstimateCalculationSnapshots => Set<TanErp.Domain.Estimates.EstimateCalculationSnapshot>();
    public DbSet<TanErp.Domain.Estimates.CalculationPolicyVersion> CalculationPolicyVersions => Set<TanErp.Domain.Estimates.CalculationPolicyVersion>();
    public DbSet<TanErp.Domain.Estimates.TaxPolicyVersion> TaxPolicyVersions => Set<TanErp.Domain.Estimates.TaxPolicyVersion>();
    public DbSet<TanErp.Domain.Estimates.EstimateApprovalRequest> EstimateApprovalRequests => Set<TanErp.Domain.Estimates.EstimateApprovalRequest>();
    public DbSet<TanErp.Domain.Estimates.EstimateApprovalStep> EstimateApprovalSteps => Set<TanErp.Domain.Estimates.EstimateApprovalStep>();
    public DbSet<TanErp.Domain.Estimates.EstimateApprovalDecision> EstimateApprovalDecisions => Set<TanErp.Domain.Estimates.EstimateApprovalDecision>();
    public DbSet<TanErp.Domain.Estimates.EstimateApprovalSnapshot> EstimateApprovalSnapshots => Set<TanErp.Domain.Estimates.EstimateApprovalSnapshot>();
    public DbSet<TanErp.Domain.Estimates.EstimateSection> EstimateSections => Set<TanErp.Domain.Estimates.EstimateSection>();
    public DbSet<TanErp.Domain.Estimates.EstimateWorkItem> EstimateWorkItems => Set<TanErp.Domain.Estimates.EstimateWorkItem>();
    public DbSet<TanErp.Domain.Estimates.EstimateCostComponent> EstimateCostComponents => Set<TanErp.Domain.Estimates.EstimateCostComponent>();
    public DbSet<TanErp.Domain.Commercial.Quotation> Quotations => Set<TanErp.Domain.Commercial.Quotation>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<TanErp.Domain.MasterData.Geography.Province> Provinces => Set<TanErp.Domain.MasterData.Geography.Province>();
    public DbSet<TanErp.Domain.MasterData.Geography.District> Districts => Set<TanErp.Domain.MasterData.Geography.District>();
    public DbSet<TanErp.Domain.MasterData.Geography.Subdistrict> Subdistricts => Set<TanErp.Domain.MasterData.Geography.Subdistrict>();
    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();
    public DbSet<TanErp.Domain.Files.FileUploadSession> FileUploadSessions => Set<TanErp.Domain.Files.FileUploadSession>();
    public DbSet<TanErp.Domain.Files.FileUploadSlot> FileUploadSlots => Set<TanErp.Domain.Files.FileUploadSlot>();
    public DbSet<TanErp.Domain.DocumentNumbering.DocumentSequenceDefinition> DocumentSequenceDefinitions => Set<TanErp.Domain.DocumentNumbering.DocumentSequenceDefinition>();
    public DbSet<TanErp.Domain.DocumentNumbering.DocumentSequenceCounter> DocumentSequenceCounters => Set<TanErp.Domain.DocumentNumbering.DocumentSequenceCounter>();
    public DbSet<TanErp.Domain.Items.Item> Items => Set<TanErp.Domain.Items.Item>();
    public DbSet<TanErp.Domain.Items.ItemCategory> ItemCategories => Set<TanErp.Domain.Items.ItemCategory>();
    public DbSet<TanErp.Domain.Items.ItemBrand> ItemBrands => Set<TanErp.Domain.Items.ItemBrand>();
    public DbSet<TanErp.Domain.Items.ItemTaxCategory> ItemTaxCategories => Set<TanErp.Domain.Items.ItemTaxCategory>();
    public DbSet<TanErp.Domain.Items.ItemAlias> ItemAliases => Set<TanErp.Domain.Items.ItemAlias>();
    public DbSet<TanErp.Domain.Items.UnitOfMeasure> Units => Set<TanErp.Domain.Items.UnitOfMeasure>();
    public DbSet<TanErp.Domain.Items.ItemBranchAvailability> ItemBranchAvailabilities => Set<TanErp.Domain.Items.ItemBranchAvailability>();
    public DbSet<TanErp.Domain.Items.ItemImage> ItemImages => Set<TanErp.Domain.Items.ItemImage>();
    public DbSet<TanErp.Domain.Items.ItemBarcode> ItemBarcodes => Set<TanErp.Domain.Items.ItemBarcode>();
    public DbSet<TanErp.Domain.Items.ItemUnitConversion> ItemUnitConversions => Set<TanErp.Domain.Items.ItemUnitConversion>();
    public DbSet<TanErp.Domain.Items.UnitConversion> UnitConversions => Set<TanErp.Domain.Items.UnitConversion>();
    public DbSet<TanErp.Domain.Items.CostRecord> CostRecords => Set<TanErp.Domain.Items.CostRecord>();
    public DbSet<TanErp.Domain.Items.CostRecordReview> CostRecordReviews => Set<TanErp.Domain.Items.CostRecordReview>();
    public DbSet<TanErp.Domain.Items.CostSource> CostSources => Set<TanErp.Domain.Items.CostSource>();

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
