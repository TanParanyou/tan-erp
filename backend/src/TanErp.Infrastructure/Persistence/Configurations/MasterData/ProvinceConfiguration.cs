using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.MasterData.Geography;

namespace TanErp.Infrastructure.Persistence.Configurations.MasterData;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("provinces", "master_data");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(10).IsRequired();
        builder.Property(x => x.NameTh).HasColumnName("name_th").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(100).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.NameTh);
    }
}
