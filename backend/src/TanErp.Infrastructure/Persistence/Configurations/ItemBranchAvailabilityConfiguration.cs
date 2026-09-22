using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ItemBranchAvailabilityConfiguration : IEntityTypeConfiguration<ItemBranchAvailability>
{
    public void Configure(EntityTypeBuilder<ItemBranchAvailability> builder)
    {
        builder.ToTable("item_branch_availabilities", "item_master", t =>
        {
            t.HasCheckConstraint("CK_item_branch_availabilities_status", "status IN ('active', 'inactive')");
            t.HasCheckConstraint("CK_item_branch_availabilities_period", "effective_from_utc IS NULL OR effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.EffectiveFromUtc).HasColumnName("effective_from_utc").HasColumnType("timestamptz");
        builder.Property(x => x.EffectiveToUtc).HasColumnName("effective_to_utc").HasColumnType("timestamptz");
        builder.Property(x => x.InactiveReason).HasColumnName("inactive_reason").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.BranchId }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.Status });

        // Composite FKs
        builder.HasOne(x => x.Item)
            .WithMany(x => x.BranchAvailabilities)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => new { x.BranchId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
