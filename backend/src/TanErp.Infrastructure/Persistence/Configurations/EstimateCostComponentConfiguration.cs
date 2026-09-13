using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;
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

        builder.HasIndex(x => new { x.OrganizationId, x.EstimateWorkItemId });

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
