using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Files;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.ToTable("item_images", "item_master", t =>
        {
            t.HasCheckConstraint("CK_item_images_role", "role IN ('primary', 'gallery', 'technical')");
            t.HasCheckConstraint("CK_item_images_status", "status IN ('active', 'inactive')");
            t.HasCheckConstraint("CK_item_images_display_order", "display_order >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(32).IsRequired();
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").IsRequired();
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();

        // JSONB Converters
        var localizedConverter = new ValueConverter<LocalizedText, string>(
            v => JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => ItemConfiguration.DeserializeLocalizedText(v));

        var optionalLocalizedConverter = new ValueConverter<LocalizedText?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => v == null ? null : ItemConfiguration.DeserializeLocalizedText(v));

        builder.Property(x => x.AltText)
            .HasColumnName("alt_text")
            .HasColumnType("jsonb")
            .HasConversion(localizedConverter)
            .IsRequired();

        builder.Property(x => x.Caption)
            .HasColumnName("caption")
            .HasColumnType("jsonb")
            .HasConversion(optionalLocalizedConverter);

        // Indexes
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.FileId }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.Status, x.DisplayOrder, x.Id });

        // Partial unique index: at most one active primary image per item
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId })
            .IsUnique()
            .HasFilter("is_primary = true AND status = 'active'")
            .HasDatabaseName("ix_item_images_single_active_primary");

        // Composite FK to Item
        builder.HasOne(x => x.Item)
            .WithMany(x => x.Images)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        // Composite FK to UploadedFile
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.FileId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
