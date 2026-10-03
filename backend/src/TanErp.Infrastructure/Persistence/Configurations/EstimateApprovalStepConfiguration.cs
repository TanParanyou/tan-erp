using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class EstimateApprovalStepConfiguration : IEntityTypeConfiguration<EstimateApprovalStep>
{
    public void Configure(EntityTypeBuilder<EstimateApprovalStep> builder)
    {
        builder.ToTable("estimate_approval_steps", "estimates", table =>
        {
            table.HasCheckConstraint("ck_estimate_approval_steps_status", "status IN ('pending', 'approved', 'returned')");
            table.HasCheckConstraint("ck_estimate_approval_steps_scope", "scope_type IN ('organization', 'branch')");
        });
        builder.HasKey(row => row.Id);
        builder.HasAlternateKey(row => new { row.Id, row.OrganizationId });
        builder.Property(row => row.Id).HasColumnName("id");
        builder.Property(row => row.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(row => row.EstimateApprovalRequestId).HasColumnName("estimate_approval_request_id").IsRequired();
        builder.Property(row => row.Sequence).HasColumnName("sequence").IsRequired();
        builder.Property(row => row.ReviewerUserId).HasColumnName("reviewer_user_id").IsRequired();
        builder.Property(row => row.ReviewerMembershipId).HasColumnName("reviewer_membership_id").IsRequired();
        builder.Property(row => row.PermissionKey).HasColumnName("permission_key").HasMaxLength(64).IsRequired();
        builder.Property(row => row.ScopeType).HasColumnName("scope_type").HasMaxLength(24).IsRequired();
        builder.Property(row => row.ScopeId).HasColumnName("scope_id").IsRequired();
        builder.Property(row => row.Status).HasColumnName("status").HasMaxLength(24).IsRequired();
        builder.Property(row => row.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(row => new { row.OrganizationId, row.EstimateApprovalRequestId, row.Sequence }).IsUnique();
        builder.HasOne<EstimateApprovalRequest>().WithMany().HasForeignKey(row => new { row.EstimateApprovalRequestId, row.OrganizationId })
            .HasPrincipalKey(request => new { request.Id, request.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Membership>().WithMany().HasForeignKey(row => new { row.ReviewerMembershipId, row.OrganizationId })
            .HasPrincipalKey(membership => new { membership.Id, membership.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
    }
}
