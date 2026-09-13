using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class EstimateRevisionConfiguration : IEntityTypeConfiguration<EstimateRevision>
{
    public void Configure(EntityTypeBuilder<EstimateRevision> builder)
    {
        builder.ToTable("estimate_revisions", "estimates", t =>
        {
            t.HasCheckConstraint("ck_estimate_revisions_status", "status IN ('draft', 'submitted', 'returned', 'approved', 'quoted', 'cancelled')");
            t.HasCheckConstraint("ck_estimate_revisions_revision_no", "revision_no >= 1");
            t.HasCheckConstraint("ck_estimate_revisions_net_cost", "net_cost >= 0");
            t.HasCheckConstraint("ck_estimate_revisions_grand_total", "grand_total >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.EstimateId).HasColumnName("estimate_id").IsRequired();
        builder.Property(x => x.RevisionNo).HasColumnName("revision_no").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(8).IsRequired();
        builder.Property(x => x.CalculationVersion).HasColumnName("calculation_version").IsRequired();
        builder.Property(x => x.CalculationPolicyVersion).HasColumnName("calculation_policy_version").HasMaxLength(64).IsRequired();
        builder.Property(x => x.TaxPolicyVersion).HasColumnName("tax_policy_version").HasMaxLength(64).IsRequired();

        builder.Property(x => x.NetCost).HasColumnName("net_cost").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SellingBeforeDiscount).HasColumnName("selling_before_discount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DiscountAmount).HasColumnName("discount_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.NetBeforeTax).HasColumnName("net_before_tax").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.GrandTotal).HasColumnName("grand_total").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.MarginAmount).HasColumnName("margin_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.MarginRate).HasColumnName("margin_rate").HasPrecision(10, 4).IsRequired();
        builder.Property(x => x.MarkupRate).HasColumnName("markup_rate").HasPrecision(10, 4).IsRequired();

        builder.Property(x => x.CalculationSnapshotJson).HasColumnName("calculation_snapshot_json");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.EstimateId, x.RevisionNo }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sections)
            .WithOne()
            .HasForeignKey(x => new { x.EstimateRevisionId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
