using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments", "organization");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(255);
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Code })
            .IsUnique()
            .HasDatabaseName("ix_departments_organization_id_code");

        builder.HasOne(x => x.Organization)
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.JobTitles)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
