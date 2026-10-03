using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class TaxPolicyVersionConfiguration : IEntityTypeConfiguration<TaxPolicyVersion>
{
    public void Configure(EntityTypeBuilder<TaxPolicyVersion> builder)
    {
        builder.ToTable("tax_policy_versions", "estimates", table =>
        {
            table.HasCheckConstraint("ck_estimate_tax_policy_status", "status IN ('draft', 'published', 'superseded', 'disabled')");
            table.HasCheckConstraint("ck_estimate_tax_policy_period", "effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
            table.HasCheckConstraint("ck_estimate_tax_policy_rate", "tax_rate >= 0 AND tax_rate <= 1 AND (tax_mode <> 'exempt' OR tax_rate = 0)");
            table.HasCheckConstraint("ck_estimate_tax_policy_mode", "tax_mode IN ('exclusive', 'inclusive', 'exempt')");
        });
        builder.HasKey(row => row.Id);
        builder.HasAlternateKey(row => new { row.Id, row.OrganizationId });
        builder.Property(row => row.Id).HasColumnName("id");
        builder.Property(row => row.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(row => row.BranchId).HasColumnName("branch_id");
        builder.Property(row => row.PolicyCode).HasColumnName("policy_code").HasMaxLength(64).IsRequired();
        builder.Property(row => row.Version).HasColumnName("version").IsRequired();
        builder.Property(row => row.Status).HasColumnName("status").HasMaxLength(24).IsRequired();
        builder.Property(row => row.TaxMode).HasColumnName("tax_mode").HasMaxLength(16).IsRequired();
        builder.Property(row => row.TaxRate).HasColumnName("tax_rate").HasPrecision(12, 6).IsRequired();
        builder.Property(row => row.TaxCode).HasColumnName("tax_code").HasMaxLength(32).IsRequired();
        builder.Property(row => row.EffectiveFromUtc).HasColumnName("effective_from_utc").IsRequired();
        builder.Property(row => row.EffectiveToUtc).HasColumnName("effective_to_utc");
        builder.Property(row => row.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(row => row.PublishedByUserId).HasColumnName("published_by_user_id");
        builder.Property(row => row.PublishedAtUtc).HasColumnName("published_at_utc");
        builder.Property(row => row.ContentHash).HasColumnName("content_hash").HasMaxLength(64);
        builder.HasIndex(row => new { row.OrganizationId, row.BranchId, row.PolicyCode, row.Version })
            .IsUnique().HasFilter("branch_id IS NOT NULL");
        builder.HasIndex(row => new { row.OrganizationId, row.PolicyCode, row.Version })
            .IsUnique().HasFilter("branch_id IS NULL");
        builder.HasOne<Organization>().WithMany().HasForeignKey(row => row.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany()
            .HasForeignKey(row => new { row.BranchId, row.OrganizationId })
            .HasPrincipalKey(branch => new { branch.Id, branch.OrganizationId })
            .IsRequired(false).OnDelete(DeleteBehavior.Restrict);
    }
}
