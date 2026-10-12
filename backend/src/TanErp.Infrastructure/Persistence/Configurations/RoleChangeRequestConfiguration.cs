using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class RoleChangeRequestConfiguration : IEntityTypeConfiguration<RoleChangeRequest>
{
    public void Configure(EntityTypeBuilder<RoleChangeRequest> builder)
    {
        builder.ToTable("role_change_requests", "identity_access", t =>
        {
            t.HasCheckConstraint("ck_role_change_requests_status", "status IN ('pending', 'approved', 'rejected', 'cancelled')");
            t.HasCheckConstraint("ck_role_change_requests_independent_checker", "decided_by_user_id IS NULL OR status = 'cancelled' OR decided_by_user_id <> requested_by_user_id");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired();
        builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.BaseRoleRowVersion).HasColumnName("base_role_row_version").IsRequired();
        builder.Property(x => x.ProposedName).HasColumnName("proposed_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProposedDescription).HasColumnName("proposed_description").HasMaxLength(500);
        builder.Property(x => x.ProposedGrantsJson).HasColumnName("proposed_grants").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Status });
        builder.HasIndex(x => x.RoleId)
            .IsUnique()
            .HasFilter("status = 'pending'")
            .HasDatabaseName("ux_role_change_requests_pending");

        builder.HasOne<TanErp.Domain.Organization.Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
