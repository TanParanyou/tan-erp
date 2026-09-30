using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("customer_addresses", "crm", table =>
        {
            table.HasCheckConstraint("ck_customer_addresses_type", "address_type IN ('billing', 'contact')");
            table.HasCheckConstraint("ck_customer_addresses_status", "status IN ('active', 'inactive')");
            table.HasCheckConstraint("ck_customer_addresses_postal_code", "postal_code ~ '^[0-9]{5}$'");
        });
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.AddressType).HasColumnName("address_type").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Label).HasColumnName("label").HasMaxLength(100).IsRequired();
        builder.Property(x => x.AddressLine1).HasColumnName("address_line1").HasMaxLength(250).IsRequired();
        builder.Property(x => x.Subdistrict).HasColumnName("subdistrict").HasMaxLength(100).IsRequired();
        builder.Property(x => x.District).HasColumnName("district").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Province).HasColumnName("province").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(5).IsRequired();
        builder.Property(x => x.CountryCode).HasColumnName("country_code").HasMaxLength(2).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.HasIndex(x => x.CustomerId).HasFilter("status = 'active' AND is_primary = true AND address_type = 'billing'").IsUnique();
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => new { x.CustomerId, x.OrganizationId }).HasPrincipalKey(x => new { x.Id, x.OrganizationId }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
