using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class EstimateCalculationSnapshotConfiguration : IEntityTypeConfiguration<EstimateCalculationSnapshot>
{
    public void Configure(EntityTypeBuilder<EstimateCalculationSnapshot> builder)
    {
        builder.ToTable("estimate_calculation_snapshots", "estimates", table =>
        {
            table.HasCheckConstraint("ck_estimate_calculation_snapshots_version", "calculation_version > 0");
        });

        builder.HasKey(row => row.Id);
        builder.HasAlternateKey(row => new { row.Id, row.OrganizationId });
        builder.Property(row => row.Id).HasColumnName("id");
        builder.Property(row => row.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(row => row.EstimateRevisionId).HasColumnName("estimate_revision_id").IsRequired();
        builder.Property(row => row.CalculationVersion).HasColumnName("calculation_version").IsRequired();
        builder.Property(row => row.InputHash).HasColumnName("input_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.SnapshotJson).HasColumnName("snapshot_json").HasColumnType("jsonb").IsRequired();
        builder.Property(row => row.CalculationPolicyVersion).HasColumnName("calculation_policy_version").HasMaxLength(64).IsRequired();
        builder.Property(row => row.TaxPolicyVersion).HasColumnName("tax_policy_version").HasMaxLength(64).IsRequired();
        builder.Property(row => row.CalculationPolicyVersionId).HasColumnName("calculation_policy_version_id");
        builder.Property(row => row.TaxPolicyVersionId).HasColumnName("tax_policy_version_id");
        builder.Property(row => row.CalculationPolicyHash).HasColumnName("calculation_policy_hash").HasMaxLength(64);
        builder.Property(row => row.TaxPolicyHash).HasColumnName("tax_policy_hash").HasMaxLength(64);
        builder.Property(row => row.CapturedByUserId).HasColumnName("captured_by_user_id").IsRequired();
        builder.Property(row => row.CapturedAtUtc).HasColumnName("captured_at_utc").IsRequired();

        builder.HasIndex(row => new { row.OrganizationId, row.EstimateRevisionId, row.CalculationVersion }).IsUnique();
        builder.HasOne<EstimateRevision>()
            .WithMany()
            .HasForeignKey(row => new { row.EstimateRevisionId, row.OrganizationId })
            .HasPrincipalKey(revision => new { revision.Id, revision.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CalculationPolicyVersion>()
            .WithMany()
            .HasForeignKey(row => new { row.CalculationPolicyVersionId, row.OrganizationId })
            .HasPrincipalKey(policy => new { policy.Id, policy.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TaxPolicyVersion>()
            .WithMany()
            .HasForeignKey(row => new { row.TaxPolicyVersionId, row.OrganizationId })
            .HasPrincipalKey(policy => new { policy.Id, policy.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
