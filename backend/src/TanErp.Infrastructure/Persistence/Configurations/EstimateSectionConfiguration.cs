using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Estimates;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class EstimateSectionConfiguration : IEntityTypeConfiguration<EstimateSection>
{
    public void Configure(EntityTypeBuilder<EstimateSection> builder)
    {
        builder.ToTable("estimate_sections", "estimates");

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.EstimateRevisionId).HasColumnName("estimate_revision_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.NameTh).HasColumnName("name_th").HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(200);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(x => x.SubtotalCost).HasColumnName("subtotal_cost").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SubtotalSellingPrice).HasColumnName("subtotal_selling_price").HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.EstimateRevisionId, x.Code }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.WorkItems)
            .WithOne()
            .HasForeignKey(x => new { x.EstimateSectionId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
