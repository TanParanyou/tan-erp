using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class EstimateApprovalRequestConfiguration : IEntityTypeConfiguration<EstimateApprovalRequest>
{
    public void Configure(EntityTypeBuilder<EstimateApprovalRequest> builder)
    {
        builder.ToTable("estimate_approval_requests", "estimates", table =>
            table.HasCheckConstraint("ck_estimate_approval_requests_status", "status IN ('open', 'approved', 'returned', 'cancelled')"));
        builder.HasKey(row => row.Id);
        builder.HasAlternateKey(row => new { row.Id, row.OrganizationId });
        builder.Property(row => row.Id).HasColumnName("id");
        builder.Property(row => row.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(row => row.EstimateId).HasColumnName("estimate_id").IsRequired();
        builder.Property(row => row.EstimateRevisionId).HasColumnName("estimate_revision_id").IsRequired();
        builder.Property(row => row.RevisionNo).HasColumnName("revision_no").IsRequired();
        builder.Property(row => row.CalculationVersion).HasColumnName("calculation_version").IsRequired();
        builder.Property(row => row.CalculationInputHash).HasColumnName("calculation_input_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.CalculationSnapshotHash).HasColumnName("calculation_snapshot_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.PolicyCode).HasColumnName("policy_code").HasMaxLength(64).IsRequired();
        builder.Property(row => row.PolicyVersion).HasColumnName("policy_version").IsRequired();
        builder.Property(row => row.RouteSnapshotJson).HasColumnName("route_snapshot_json").HasColumnType("jsonb").IsRequired();
        builder.Property(row => row.RouteHash).HasColumnName("route_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.SubmissionNote).HasColumnName("submission_note").HasMaxLength(2000);
        builder.Property(row => row.Status).HasColumnName("status").HasMaxLength(24).IsRequired();
        builder.Property(row => row.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired();
        builder.Property(row => row.RequestedAtUtc).HasColumnName("requested_at_utc").IsRequired();
        builder.Property(row => row.ClosedAtUtc).HasColumnName("closed_at_utc");
        builder.Property(row => row.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(row => new { row.OrganizationId, row.EstimateRevisionId })
            .IsUnique().HasFilter("status = 'open'");
        builder.HasOne<Estimate>().WithMany().HasForeignKey(row => new { row.EstimateId, row.OrganizationId })
            .HasPrincipalKey(estimate => new { estimate.Id, estimate.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<EstimateRevision>().WithMany().HasForeignKey(row => new { row.EstimateRevisionId, row.OrganizationId })
            .HasPrincipalKey(revision => new { revision.Id, revision.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
    }
}
