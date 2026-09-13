using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class EstimateWorkItemConfiguration : IEntityTypeConfiguration<EstimateWorkItem>
{
    public void Configure(EntityTypeBuilder<EstimateWorkItem> builder)
    {
        builder.ToTable("estimate_work_items", "estimates", t =>
        {
            t.HasCheckConstraint("ck_estimate_work_items_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_estimate_work_items_selling_rule_type", "selling_rule_type IN ('margin', 'markup', 'fixed_price')");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.EstimateSectionId).HasColumnName("estimate_section_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.DescriptionTh).HasColumnName("description_th").HasMaxLength(500).IsRequired();
        builder.Property(x => x.DescriptionEn).HasColumnName("description_en").HasMaxLength(500);
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SellingRuleType).HasColumnName("selling_rule_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SellingRuleValue).HasColumnName("selling_rule_value").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitCost).HasColumnName("unit_cost").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TotalCost).HasColumnName("total_cost").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UnitSellingPrice).HasColumnName("unit_selling_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TotalSellingPrice).HasColumnName("total_selling_price").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.EstimateSectionId, x.Code }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.CostComponents)
            .WithOne()
            .HasForeignKey(x => new { x.EstimateWorkItemId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
