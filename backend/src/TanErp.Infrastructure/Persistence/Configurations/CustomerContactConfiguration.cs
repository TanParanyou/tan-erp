using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Customers;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("customer_contacts", "crm", t =>
        {
            t.HasCheckConstraint("CK_customer_contacts_phone_or_email", "normalized_phone IS NOT NULL OR normalized_email IS NOT NULL");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.RoleTitle).HasColumnName("role_title").HasMaxLength(100);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(x => x.NormalizedPhone).HasColumnName("normalized_phone").HasMaxLength(50);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
        builder.Property(x => x.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(255);
        builder.Property(x => x.LineId).HasColumnName("line_id").HasMaxLength(100);
        builder.Property(x => x.PreferredChannel).HasColumnName("preferred_channel").HasMaxLength(32).IsRequired();
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        builder.HasIndex(x => x.CustomerId)
            .HasFilter("is_primary = true AND status = 'active'")
            .IsUnique();

        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedPhone });
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedEmail });
    }
}
