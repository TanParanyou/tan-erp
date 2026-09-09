using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("sites", "crm", t =>
        {
            t.HasCheckConstraint("CK_sites_status", "status IN ('active', 'inactive')");
            t.HasCheckConstraint("CK_sites_coordinates_paired", "(latitude IS NULL AND longitude IS NULL) OR (latitude IS NOT NULL AND longitude IS NOT NULL)");
            t.HasCheckConstraint("CK_sites_latitude_range", "latitude IS NULL OR (latitude >= -90.000000 AND latitude <= 90.000000)");
            t.HasCheckConstraint("CK_sites_longitude_range", "longitude IS NULL OR (longitude >= -180.000000 AND longitude <= 180.000000)");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        builder.HasAlternateKey(x => new { x.Id, x.CustomerId, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NormalizedLabel).HasColumnName("normalized_label").HasMaxLength(100).IsRequired();
        builder.Property(x => x.AddressLine1).HasColumnName("address_line1").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Subdistrict).HasColumnName("subdistrict").HasMaxLength(100).IsRequired();
        builder.Property(x => x.District).HasColumnName("district").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Province).HasColumnName("province").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CountryCode).HasColumnName("country_code").HasMaxLength(10).IsRequired();
        builder.Property(x => x.Latitude).HasColumnName("latitude").HasColumnType("numeric(9,6)");
        builder.Property(x => x.Longitude).HasColumnName("longitude").HasColumnType("numeric(10,6)");
        builder.Property(x => x.AccessNote).HasColumnName("access_note").HasMaxLength(1000);
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.CustomerId, x.Status, x.NormalizedLabel, x.Id });

        // Tenant foreign key to Organization
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite foreign key to Customer (id, organization_id)
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => new { x.CustomerId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
