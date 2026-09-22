using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ItemAliasConfiguration : IEntityTypeConfiguration<ItemAlias>
{
    public void Configure(EntityTypeBuilder<ItemAlias> builder)
    {
        builder.ToTable("item_aliases", "item_master", t =>
        {
            t.HasCheckConstraint("CK_item_aliases_status", "status IN ('active', 'inactive')");
            t.HasCheckConstraint("CK_item_aliases_alias_shape", "jsonb_typeof(alias) = 'object' AND (alias - 'th' - 'en') = '{}'::jsonb");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.NormalizedTh).HasColumnName("normalized_th").HasMaxLength(250).IsRequired();
        builder.Property(x => x.NormalizedEn).HasColumnName("normalized_en").HasMaxLength(250);
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();

        // JSONB Converter
        var localizedConverter = new ValueConverter<LocalizedText, string>(
            v => JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => ItemConfiguration.DeserializeLocalizedText(v));

        builder.Property(x => x.Alias)
            .HasColumnName("alias")
            .HasColumnType("jsonb")
            .HasConversion(localizedConverter)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.NormalizedTh }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId, x.NormalizedEn })
            .IsUnique()
            .HasFilter("normalized_en IS NOT NULL")
            .HasDatabaseName("ix_item_aliases_org_item_normalized_en");
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedTh });
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedEn });

        // Composite FK to Item
        builder.HasOne(x => x.Item)
            .WithMany(x => x.Aliases)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
