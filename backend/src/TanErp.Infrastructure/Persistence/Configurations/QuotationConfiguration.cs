using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Commercial;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Estimates;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("quotations", "commercial", t =>
        {
            t.HasCheckConstraint("ck_quotations_status", "status IN ('draft', 'issued', 'accepted', 'rejected', 'expired', 'superseded', 'voided')");
            t.HasCheckConstraint("ck_quotations_total_amount", "total_amount >= 0");
            t.HasCheckConstraint("ck_quotations_void_fields", "(status = 'voided') = (void_reason IS NOT NULL)");
            t.HasCheckConstraint("ck_quotations_superseded_fields", "(status = 'superseded') = (superseded_by_quotation_id IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.OpportunityId).HasColumnName("opportunity_id").IsRequired();
        builder.Property(x => x.EstimateId).HasColumnName("estimate_id").IsRequired();
        builder.Property(x => x.EstimateRevisionId).HasColumnName("estimate_revision_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SnapshotHash).HasColumnName("snapshot_hash").HasMaxLength(128);
        builder.Property(x => x.CustomerBillingSnapshotJson).HasColumnName("customer_billing_snapshot_json").HasColumnType("jsonb");
        builder.Property(x => x.CustomerBillingSnapshotHash).HasColumnName("customer_billing_snapshot_hash").HasMaxLength(128);
        builder.Property(x => x.IssuedAtUtc).HasColumnName("issued_at_utc").IsRequired();
        builder.Property(x => x.AcceptedAtUtc).HasColumnName("accepted_at_utc");
        builder.Property(x => x.SupersedesQuotationId).HasColumnName("supersedes_quotation_id");
        builder.Property(x => x.SupersededByQuotationId).HasColumnName("superseded_by_quotation_id");
        builder.Property(x => x.AmendmentReason).HasColumnName("amendment_reason").HasMaxLength(500);
        builder.Property(x => x.VoidedAtUtc).HasColumnName("voided_at_utc");
        builder.Property(x => x.VoidedByUserId).HasColumnName("voided_by_user_id");
        builder.Property(x => x.VoidReason).HasColumnName("void_reason").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.OpportunityId });
        builder.HasIndex(x => new { x.OrganizationId, x.EstimateId });
        // At most one live (issued or accepted) quotation per estimate; superseded and voided ones are history.
        builder.HasIndex(x => x.EstimateId).IsUnique().HasFilter("status IN ('issued', 'accepted')").HasDatabaseName("ux_quotations_one_live_per_estimate");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Opportunity>()
            .WithMany()
            .HasForeignKey(x => x.OpportunityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Estimate>()
            .WithMany()
            .HasForeignKey(x => x.EstimateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<EstimateRevision>()
            .WithMany()
            .HasForeignKey(x => x.EstimateRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
