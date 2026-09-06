using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions", "identity_access", t =>
        {
            t.HasCheckConstraint("CK_role_permissions_scope", "scope IN ('organization', 'branch', 'own')");
            t.HasCheckConstraint("CK_role_permissions_org_scope", "scope <> 'organization' OR (scope_id = organization_id AND branch_id IS NULL)");
            t.HasCheckConstraint("CK_role_permissions_branch_scope", "scope <> 'branch' OR (branch_id IS NOT NULL AND scope_id = branch_id)");
            t.HasCheckConstraint("CK_role_permissions_own_scope", "scope <> 'own' OR (scope_id IS NULL AND branch_id IS NULL)");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.PermissionId).HasColumnName("permission_id").IsRequired();
        builder.Property(x => x.Scope).HasColumnName("scope").HasMaxLength(50).IsRequired();
        builder.Property(x => x.ScopeId).HasColumnName("scope_id");
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        builder.Property(x => x.AssignedAtUtc).HasColumnName("assigned_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.RoleId, x.OrganizationId })
            .HasDatabaseName("ix_role_permissions_role_id_organization_id");

        builder.HasIndex(x => new { x.BranchId, x.OrganizationId })
            .HasDatabaseName("ix_role_permissions_branch_id_organization_id");

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => new { x.RoleId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => new { x.BranchId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
