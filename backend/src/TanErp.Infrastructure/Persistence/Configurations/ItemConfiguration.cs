using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items", "item_master", t =>
        {
            t.HasCheckConstraint("CK_items_item_type", "item_type IN ('material', 'labor', 'service', 'subcontract', 'other')");
            t.HasCheckConstraint("CK_items_status", "status IN ('draft', 'active', 'inactive')");
            t.HasCheckConstraint("CK_items_availability_mode", "availability_mode IN ('all_branches', 'selected_branches')");
            t.HasCheckConstraint("CK_items_at_least_one_capability", "can_sell OR can_cost OR can_purchase OR can_stock OR can_produce");
            t.HasCheckConstraint("CK_items_name_shape", "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");
            t.HasCheckConstraint("CK_items_description_shape", "description IS NULL OR (jsonb_typeof(description) = 'object' AND (description - 'th' - 'en') = '{}'::jsonb)");
            t.HasCheckConstraint("CK_items_active_requires_thai_name", "status != 'active' OR (name->>'th' IS NOT NULL AND length(trim(name->>'th')) > 0)");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.ItemType).HasColumnName("item_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(x => x.BrandId).HasColumnName("brand_id");
        builder.Property(x => x.BaseUnitId).HasColumnName("base_unit_id").IsRequired();
        builder.Property(x => x.TaxCategoryCode).HasColumnName("tax_category_code").HasMaxLength(30);
        builder.Property(x => x.AvailabilityMode).HasColumnName("availability_mode").HasMaxLength(32).IsRequired();
        builder.Property(x => x.AttributesSchemaVersion).HasColumnName("attributes_schema_version");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ActivatedOnce).HasColumnName("activated_once").IsRequired();
        builder.Property(x => x.ActivatedAtUtc).HasColumnName("activated_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ActivatedByUserId).HasColumnName("activated_by_user_id");
        builder.Property(x => x.InactiveAtUtc).HasColumnName("inactive_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.InactiveByUserId).HasColumnName("inactive_by_user_id");
        builder.Property(x => x.InactiveReasonCode).HasColumnName("inactive_reason_code").HasMaxLength(50);
        builder.Property(x => x.InactiveReason).HasColumnName("inactive_reason").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id").IsRequired();

        // Complex Property for Capabilities
        builder.ComplexProperty(x => x.Capabilities, cb =>
        {
            cb.Property(c => c.CanSell).HasColumnName("can_sell").IsRequired();
            cb.Property(c => c.CanCost).HasColumnName("can_cost").IsRequired();
            cb.Property(c => c.CanPurchase).HasColumnName("can_purchase").IsRequired();
            cb.Property(c => c.CanStock).HasColumnName("can_stock").IsRequired();
            cb.Property(c => c.CanProduce).HasColumnName("can_produce").IsRequired();
        });

        // JSONB Converters
        var localizedConverter = new ValueConverter<LocalizedText, string>(
            v => JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => DeserializeLocalizedText(v));

        var optionalLocalizedConverter = new ValueConverter<LocalizedText?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => v == null ? null : DeserializeLocalizedText(v));

        var attributesConverter = new ValueConverter<Dictionary<string, string>?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => v == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null));

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("jsonb")
            .HasConversion(localizedConverter)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("jsonb")
            .HasConversion(optionalLocalizedConverter);

        builder.Property(x => x.Attributes)
            .HasColumnName("attributes")
            .HasColumnType("jsonb")
            .HasConversion(attributesConverter);

        // Indexes
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.NormalizedCode });
        builder.HasIndex(x => new { x.OrganizationId, x.CategoryId });
        builder.HasIndex(x => new { x.OrganizationId, x.BrandId });
        builder.HasIndex(x => new { x.OrganizationId, x.BaseUnitId });

        // Relationships
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => new { x.CategoryId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Brand)
            .WithMany()
            .HasForeignKey(x => new { x.BrandId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BaseUnit)
            .WithMany()
            .HasForeignKey(x => new { x.BaseUnitId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Aliases)
            .WithOne(x => x.Item)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BranchAvailabilities)
            .WithOne(x => x.Item)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Item)
            .HasForeignKey(x => new { x.ItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }

    internal static LocalizedText DeserializeLocalizedText(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var th = root.TryGetProperty("th", out var pTh) ? pTh.GetString() ?? string.Empty : string.Empty;
        var en = root.TryGetProperty("en", out var pEn) && pEn.ValueKind != JsonValueKind.Null ? pEn.GetString() : null;
        return new LocalizedText(th, en);
    }
}
