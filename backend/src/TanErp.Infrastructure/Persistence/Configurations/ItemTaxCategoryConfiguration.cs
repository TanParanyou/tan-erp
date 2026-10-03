using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class ItemTaxCategoryConfiguration : IEntityTypeConfiguration<ItemTaxCategory>
{
    public void Configure(EntityTypeBuilder<ItemTaxCategory> builder)
    {
        builder.ToTable("item_tax_categories", "item_master", table =>
        {
            table.HasCheckConstraint("CK_item_tax_categories_status", "status IN ('active', 'inactive')");
            table.HasCheckConstraint("CK_item_tax_categories_sort_order", "sort_order >= 0");
            table.HasCheckConstraint("CK_item_tax_categories_name_shape", "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();

        var localizedConverter = new ValueConverter<LocalizedText, string>(
            value => JsonSerializer.Serialize(new { th = value.Thai, en = value.English }, (JsonSerializerOptions?)null),
            value => ItemConfiguration.DeserializeLocalizedText(value));
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("jsonb").HasConversion(localizedConverter).IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.SortOrder, x.Id });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
