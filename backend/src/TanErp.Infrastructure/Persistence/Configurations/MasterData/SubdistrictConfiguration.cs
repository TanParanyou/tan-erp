using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.MasterData.Geography;

namespace TanErp.Infrastructure.Persistence.Configurations.MasterData;

public class SubdistrictConfiguration : IEntityTypeConfiguration<Subdistrict>
{
    public void Configure(EntityTypeBuilder<Subdistrict> builder)
    {
        builder.ToTable("subdistricts", "master_data");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.DistrictId).HasColumnName("district_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(10).IsRequired();
        builder.Property(x => x.NameTh).HasColumnName("name_th").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(10).IsRequired();
        builder.Property(x => x.DefaultLatitude).HasColumnName("default_latitude").HasColumnType("numeric(9,6)");
        builder.Property(x => x.DefaultLongitude).HasColumnName("default_longitude").HasColumnType("numeric(10,6)");

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => new { x.DistrictId, x.Code });
        builder.HasIndex(x => x.PostalCode);
        builder.HasIndex(x => x.NameTh);

        builder.HasOne<District>()
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
