using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ItemBrandConfiguration : IEntityTypeConfiguration<ItemBrand>
{
    public void Configure(EntityTypeBuilder<ItemBrand> builder)
    {
        builder.ToTable("item_brands", "item_master", t =>
        {
            t.HasCheckConstraint("CK_item_brands_status", "status IN ('active', 'inactive')");
            t.HasCheckConstraint("CK_item_brands_sort_order", "sort_order >= 0");
            t.HasCheckConstraint("CK_item_brands_name_shape", "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");
            t.HasCheckConstraint("CK_item_brands_description_shape", "description IS NULL OR (jsonb_typeof(description) = 'object' AND (description - 'th' - 'en') = '{}'::jsonb)");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(30).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
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

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("jsonb")
            .HasConversion(localizedConverter)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("jsonb")
            .HasConversion(optionalLocalizedConverter);

        // Indexes
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.SortOrder, x.Id });

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
