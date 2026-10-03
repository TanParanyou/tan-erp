using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class RoleAssignmentRequestConfiguration : IEntityTypeConfiguration<RoleAssignmentRequest>
{
    public void Configure(EntityTypeBuilder<RoleAssignmentRequest> builder)
    {
        builder.ToTable("role_assignment_requests", "identity_access", t =>
        {
            t.HasCheckConstraint("ck_role_assignment_requests_status", "status IN ('pending', 'approved', 'rejected', 'cancelled')");
            t.HasCheckConstraint("ck_role_assignment_requests_independent_checker", "decided_by_user_id IS NULL OR status = 'cancelled' OR decided_by_user_id <> requested_by_user_id");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.MembershipId).HasColumnName("membership_id").IsRequired();
        builder.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired();
        builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Status });
        builder.HasIndex(x => new { x.MembershipId, x.RoleId })
            .IsUnique()
            .HasFilter("status = 'pending'")
            .HasDatabaseName("ux_role_assignment_requests_pending");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Membership>().WithMany().HasForeignKey(x => x.MembershipId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
