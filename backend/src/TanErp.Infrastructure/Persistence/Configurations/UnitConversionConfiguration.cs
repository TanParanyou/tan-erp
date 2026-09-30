using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class UnitConversionConfiguration : IEntityTypeConfiguration<UnitConversion>
{
    public void Configure(EntityTypeBuilder<UnitConversion> builder)
    {
        builder.ToTable("unit_conversions", "item_master", table =>
        {
            table.HasCheckConstraint("CK_unit_conversions_factor", "factor > 0");
            table.HasCheckConstraint("CK_unit_conversions_distinct_units", "from_unit_id <> to_unit_id");
            table.HasCheckConstraint("CK_unit_conversions_effective_period", "effective_to IS NULL OR effective_to >= effective_from");
            table.HasCheckConstraint("CK_unit_conversions_status", "status IN ('active', 'inactive')");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.FromUnitId).HasColumnName("from_unit_id").IsRequired();
        builder.Property(x => x.ToUnitId).HasColumnName("to_unit_id").IsRequired();
        builder.Property(x => x.Factor).HasColumnName("factor").HasPrecision(18, 6).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.FromUnitId, x.ToUnitId, x.EffectiveFrom }).IsUnique();
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => new { x.FromUnitId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => new { x.ToUnitId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
