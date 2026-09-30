using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class EstimateApprovalDecisionConfiguration : IEntityTypeConfiguration<EstimateApprovalDecision>
{
    public void Configure(EntityTypeBuilder<EstimateApprovalDecision> builder)
    {
        builder.ToTable("estimate_approval_decisions", "estimates", table =>
            table.HasCheckConstraint("ck_estimate_approval_decisions_decision", "decision IN ('approved', 'returned')"));
        builder.HasKey(row => row.Id);
        builder.HasAlternateKey(row => new { row.Id, row.OrganizationId });
        builder.Property(row => row.Id).HasColumnName("id");
        builder.Property(row => row.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(row => row.EstimateApprovalRequestId).HasColumnName("estimate_approval_request_id").IsRequired();
        builder.Property(row => row.EstimateApprovalStepId).HasColumnName("estimate_approval_step_id").IsRequired();
        builder.Property(row => row.ReviewerUserId).HasColumnName("reviewer_user_id").IsRequired();
        builder.Property(row => row.ReviewerMembershipId).HasColumnName("reviewer_membership_id").IsRequired();
        builder.Property(row => row.Decision).HasColumnName("decision").HasMaxLength(24).IsRequired();
        builder.Property(row => row.ReasonCode).HasColumnName("reason_code").HasMaxLength(64);
        builder.Property(row => row.Note).HasColumnName("note").HasMaxLength(2000);
        builder.Property(row => row.CalculationSnapshotHash).HasColumnName("calculation_snapshot_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.RouteHash).HasColumnName("route_hash").HasMaxLength(64).IsRequired();
        builder.Property(row => row.DecidedAtUtc).HasColumnName("decided_at_utc").IsRequired();
        builder.HasIndex(row => new { row.OrganizationId, row.EstimateApprovalStepId }).IsUnique();
        builder.HasOne<EstimateApprovalRequest>().WithMany().HasForeignKey(row => new { row.EstimateApprovalRequestId, row.OrganizationId })
            .HasPrincipalKey(request => new { request.Id, request.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<EstimateApprovalStep>().WithMany().HasForeignKey(row => new { row.EstimateApprovalStepId, row.OrganizationId })
            .HasPrincipalKey(step => new { step.Id, step.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
    }
}
