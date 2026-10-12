using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ApprovalLimitConfiguration : IEntityTypeConfiguration<ApprovalLimit>
{
    public void Configure(EntityTypeBuilder<ApprovalLimit> builder)
    {
        builder.ToTable("approval_limits", "identity_access", t =>
        {
            // credit_note is reserved for G-15: the DB accepts it, the API rejects it until it is enforced.
            t.HasCheckConstraint("ck_approval_limits_document_type", "document_type IN ('estimate', 'purchase_order', 'change_order', 'credit_note')");
            t.HasCheckConstraint("ck_approval_limits_amount", "(is_unlimited AND max_amount IS NULL) OR (NOT is_unlimited AND max_amount IS NOT NULL AND max_amount >= 0)");
            t.HasCheckConstraint("ck_approval_limits_currency", "char_length(currency) = 3");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(x => x.DocumentType).HasColumnName("document_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.IsUnlimited).HasColumnName("is_unlimited").IsRequired();
        builder.Property(x => x.MaxAmount).HasColumnName("max_amount").HasColumnType("numeric(18,2)");
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        // One row per scope, active or not (edit the existing row instead of adding a duplicate).
        builder.HasIndex(x => new { x.OrganizationId, x.RoleId, x.DocumentType, x.Currency })
            .IsUnique()
            .HasFilter("branch_id IS NULL")
            .HasDatabaseName("ux_approval_limits_org_wide");
        builder.HasIndex(x => new { x.OrganizationId, x.RoleId, x.DocumentType, x.BranchId, x.Currency })
            .IsUnique()
            .HasFilter("branch_id IS NOT NULL")
            .HasDatabaseName("ux_approval_limits_branch");
        builder.HasIndex(x => new { x.OrganizationId, x.DocumentType, x.IsActive });

        builder.HasOne<TanErp.Domain.Organization.Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TanErp.Domain.Organization.Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
