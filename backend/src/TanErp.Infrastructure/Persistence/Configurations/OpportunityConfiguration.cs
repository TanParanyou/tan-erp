using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        builder.ToTable("opportunities", "crm", t =>
        {
            t.HasCheckConstraint("CK_opportunities_stage", "stage IN ('draft', 'qualified', 'surveying', 'estimating', 'proposed', 'won', 'lost', 'cancelled')");
            t.HasCheckConstraint("CK_opportunities_expected_budget", "expected_budget IS NULL OR expected_budget > 0");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.PrimarySiteId).HasColumnName("primary_site_id");
        builder.Property(x => x.OwnerUserId).HasColumnName("owner_user_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(250).IsRequired();
        builder.Property(x => x.NormalizedTitle).HasColumnName("normalized_title").HasMaxLength(250).IsRequired();
        builder.Property(x => x.ScopeSummary).HasColumnName("scope_summary").HasMaxLength(2000);
        builder.Property(x => x.WorkTypes).HasColumnName("work_types").HasColumnType("text[]").IsRequired();
        builder.Property(x => x.SourceCode).HasColumnName("source_code").HasMaxLength(50);
        builder.Property(x => x.ExpectedBudget).HasColumnName("expected_budget").HasColumnType("numeric(12,2)");
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(x => x.TargetDecisionDate).HasColumnName("target_decision_date").HasColumnType("date");
        builder.Property(x => x.NextActionAtUtc).HasColumnName("next_action_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.NextActionNote).HasColumnName("next_action_note").HasMaxLength(500);
        builder.Property(x => x.Stage).HasColumnName("stage").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.Stage, x.OwnerUserId, x.NextActionAtUtc, x.Id });
        builder.HasIndex(x => new { x.OrganizationId, x.CustomerId, x.Stage, x.Id });

        // Tenant foreign key to Organization
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tenant composite FK to Branch (id, organization_id)
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => new { x.BranchId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        // Tenant composite FK to Customer (id, organization_id)
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => new { x.CustomerId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        // Cross-isolation FK to Site: primary_site_id must belong to the exact same Customer and Organization
        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => new { x.PrimarySiteId, x.CustomerId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.CustomerId, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
