using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TanErp.Domain.Estimates;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class EstimateCostComponentConfiguration : IEntityTypeConfiguration<EstimateCostComponent>
{
    public void Configure(EntityTypeBuilder<EstimateCostComponent> builder)
    {
        builder.ToTable("estimate_cost_components", "estimates", t =>
        {
            t.HasCheckConstraint("ck_estimate_cost_components_type", "type IN ('material', 'labor', 'subcontract', 'service', 'other_direct')");
            t.HasCheckConstraint("ck_estimate_cost_components_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_estimate_cost_components_unit_cost", "unit_cost >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.EstimateWorkItemId).HasColumnName("estimate_work_item_id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.UnitCost).HasColumnName("unit_cost").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(8).IsRequired();
        builder.Property(x => x.TotalCost).HasColumnName("total_cost").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

        // Catalog snapshot properties
        var optionalLocalizedConverter = new ValueConverter<LocalizedText?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(new { th = v.Thai, en = v.English }, (JsonSerializerOptions?)null),
            v => v == null ? null : ItemConfiguration.DeserializeLocalizedText(v));

        builder.Property(x => x.ItemId).HasColumnName("item_id");
        builder.Property(x => x.CostRecordId).HasColumnName("cost_record_id");
        builder.Property(x => x.CostRecordVersion).HasColumnName("cost_record_version");
        builder.Property(x => x.ItemCodeSnapshot).HasColumnName("item_code_snapshot").HasMaxLength(50);
        builder.Property(x => x.ItemNameSnapshot)
            .HasColumnName("item_name_snapshot")
            .HasColumnType("jsonb")
            .HasConversion(optionalLocalizedConverter);
        builder.Property(x => x.UnitSnapshot).HasColumnName("unit_snapshot").HasMaxLength(50);
        builder.Property(x => x.UnitCostSnapshot).HasColumnName("unit_cost_snapshot").HasPrecision(18, 4);
        builder.Property(x => x.CurrencySnapshot).HasColumnName("currency_snapshot").HasMaxLength(8);
        builder.Property(x => x.CostScopeSnapshot).HasColumnName("cost_scope_snapshot").HasMaxLength(32);
        builder.Property(x => x.CostEffectiveFromUtc).HasColumnName("cost_effective_from_utc").HasColumnType("timestamptz");
        builder.Property(x => x.CostPolicyVersion).HasColumnName("cost_policy_version").HasMaxLength(50);
        builder.Property(x => x.ResolvedAtUtc).HasColumnName("resolved_at_utc").HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.OrganizationId, x.EstimateWorkItemId });
        builder.HasIndex(x => new { x.OrganizationId, x.ItemId });

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<CostRecord>()
            .WithMany()
            .HasForeignKey(x => x.CostRecordId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
