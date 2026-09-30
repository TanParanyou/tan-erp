using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class ItemBarcodeConfiguration : IEntityTypeConfiguration<ItemBarcode>
{
    public void Configure(EntityTypeBuilder<ItemBarcode> builder)
    {
        builder.ToTable("item_barcodes", "item_master", table =>
        {
            table.HasCheckConstraint("CK_item_barcodes_identifier_type", "identifier_type IN ('gtin', 'internal')");
            table.HasCheckConstraint("CK_item_barcodes_packaging_level", "packaging_level IN ('each', 'inner', 'case', 'pallet')");
            table.HasCheckConstraint("CK_item_barcodes_status", "status IN ('active', 'inactive')");
            table.HasCheckConstraint("CK_item_barcodes_quantity", "quantity_in_base_unit > 0");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.IdentifierType).HasColumnName("identifier_type").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Value).HasColumnName("value").HasMaxLength(64).IsRequired();
        builder.Property(x => x.NormalizedValue).HasColumnName("normalized_value").HasMaxLength(64).IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.QuantityInBaseUnit).HasColumnName("quantity_in_base_unit").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.PackagingLevel).HasColumnName("packaging_level").HasMaxLength(16).IsRequired();
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedValue }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.PackagingLevel })
            .IsUnique()
            .HasFilter("is_primary = true AND status = 'active'")
            .HasDatabaseName("ix_item_barcodes_single_active_primary");

        builder.HasOne(x => x.Item)
            .WithMany(x => x.Barcodes)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => new { x.UnitId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
