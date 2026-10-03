using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects", "projects", t =>
        {
            t.HasCheckConstraint("ck_projects_status", "status IN ('planned', 'active', 'on_hold', 'ready_for_handover', 'completed', 'cancelled')");
            t.HasCheckConstraint("ck_projects_baseline_contract_amount", "baseline_contract_amount >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.SiteId).HasColumnName("site_id");
        builder.Property(x => x.OpportunityId).HasColumnName("opportunity_id").IsRequired();
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.EstimateId).HasColumnName("estimate_id").IsRequired();
        builder.Property(x => x.EstimateRevisionId).HasColumnName("estimate_revision_id").IsRequired();
        builder.Property(x => x.SiteSurveyRevisionId).HasColumnName("site_survey_revision_id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.OwnerUserId).HasColumnName("owner_user_id").IsRequired();
        builder.Property(x => x.PlannedStartDate).HasColumnName("planned_start_date");
        builder.Property(x => x.BaselineQuotationNumber).HasColumnName("baseline_quotation_number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.BaselineContractAmount).HasColumnName("baseline_contract_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.BaselineQuotationSnapshotHash).HasColumnName("baseline_quotation_snapshot_hash").HasMaxLength(128);
        builder.Property(x => x.BaselineSurveySnapshotHash).HasColumnName("baseline_survey_snapshot_hash").HasMaxLength(128);
        builder.Property(x => x.BaselineHash).HasColumnName("baseline_hash").HasMaxLength(128).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();
        // One project per accepted quotation (decided cardinality); the database is the last line of defence against duplicate handover.
        builder.HasIndex(x => new { x.OrganizationId, x.QuotationId }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerUserId });

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Opportunity>().WithMany().HasForeignKey(x => x.OpportunityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Quotation>().WithMany().HasForeignKey(x => x.QuotationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
