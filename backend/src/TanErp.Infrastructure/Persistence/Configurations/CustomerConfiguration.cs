using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", "crm", table =>
        {
            table.HasCheckConstraint("ck_customers_credit_term_days", "credit_term_days >= 0");
            table.HasCheckConstraint("ck_customers_credit_limit", "credit_limit IS NULL OR credit_limit >= 0");
            table.HasCheckConstraint("ck_customers_billing_day", "billing_day IS NULL OR billing_day BETWEEN 1 AND 31");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        builder.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.NormalizedDisplayName, x.Id });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CustomerType).HasColumnName("customer_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.DisplayNameTh).HasColumnName("display_name_th").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DisplayNameEn).HasColumnName("display_name_en").HasMaxLength(255);
        builder.Property(x => x.NormalizedDisplayName).HasColumnName("normalized_display_name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.InactiveReason).HasColumnName("inactive_reason").HasMaxLength(500);
        builder.Property(x => x.PreferredLocale).HasColumnName("preferred_locale").HasMaxLength(10).IsRequired();
        builder.Property(x => x.LeadSource).HasColumnName("lead_source").HasMaxLength(50);
        builder.Property(x => x.LeadSourceNote).HasColumnName("lead_source_note").HasMaxLength(200);
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id");
        builder.Property(x => x.LegalName).HasColumnName("legal_name").HasMaxLength(250);
        builder.Property(x => x.TaxIdentifier).HasColumnName("tax_identifier").HasMaxLength(13);
        builder.Property(x => x.NormalizedTaxIdentifier).HasColumnName("normalized_tax_identifier").HasMaxLength(13);
        builder.Property(x => x.BranchCode).HasColumnName("branch_code").HasMaxLength(5);
        builder.Property(x => x.CreditTermDays).HasColumnName("credit_term_days").IsRequired();
        builder.Property(x => x.CreditLimit).HasColumnName("credit_limit").HasPrecision(18, 2);
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3).HasDefaultValue("THB").IsRequired();
        builder.Property(x => x.BillingCycle).HasColumnName("billing_cycle").HasMaxLength(20);
        builder.Property(x => x.BillingDay).HasColumnName("billing_day");
        builder.Property(x => x.PaymentConditionNote).HasColumnName("payment_condition_note").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        builder.HasIndex(x => x.ImageFileId);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TanErp.Domain.Files.UploadedFile>()
            .WithMany()
            .HasForeignKey(x => x.ImageFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Contacts)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => new { x.CustomerId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedTaxIdentifier, x.BranchCode });
    }
}
