using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class EstimateApprovalSnapshotConfiguration : IEntityTypeConfiguration<EstimateApprovalSnapshot>
{
    public void Configure(EntityTypeBuilder<EstimateApprovalSnapshot> builder)
    {
        builder.ToTable("estimate_approval_snapshots", "estimates", table =>
            table.HasCheckConstraint("ck_estimate_approval_snapshots_calculation_version", "calculation_version > 0"));
        builder.HasKey(row => row.Id);
        builder.HasAlternateKey(row => new { row.Id, row.OrganizationId });
        builder.Property(row => row.Id).HasColumnName("id");
        builder.Property(row => row.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(row => row.EstimateId).HasColumnName("estimate_id").IsRequired();
        builder.Property(row => row.EstimateRevisionId).HasColumnName("estimate_revision_id").IsRequired();
        builder.Property(row => row.EstimateApprovalRequestId).HasColumnName("estimate_approval_request_id").IsRequired();
        builder.Property(row => row.CalculationVersion).HasColumnName("calculation_version").IsRequired();
        builder.Property(row => row.CalculationInputHash).HasColumnName("calculation_input_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.CalculationSnapshotHash).HasColumnName("calculation_snapshot_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.RouteHash).HasColumnName("route_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.SnapshotJson).HasColumnName("snapshot_json").HasColumnType("jsonb").IsRequired();
        builder.Property(row => row.ApprovedByUserId).HasColumnName("approved_by_user_id").IsRequired();
        builder.Property(row => row.ApprovedAtUtc).HasColumnName("approved_at_utc").IsRequired();
        builder.HasIndex(row => new { row.OrganizationId, row.EstimateRevisionId }).IsUnique();
        builder.HasOne<Estimate>().WithMany().HasForeignKey(row => new { row.EstimateId, row.OrganizationId })
            .HasPrincipalKey(estimate => new { estimate.Id, estimate.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<EstimateRevision>().WithMany().HasForeignKey(row => new { row.EstimateRevisionId, row.OrganizationId })
            .HasPrincipalKey(revision => new { revision.Id, revision.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<EstimateApprovalRequest>().WithMany().HasForeignKey(row => new { row.EstimateApprovalRequestId, row.OrganizationId })
            .HasPrincipalKey(request => new { request.Id, request.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
    }
}
