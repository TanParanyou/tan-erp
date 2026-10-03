using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Finance;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class BillingDocumentConfiguration : IEntityTypeConfiguration<BillingDocument>
{
    public void Configure(EntityTypeBuilder<BillingDocument> builder)
    {
        builder.ToTable("billing_documents", "finance", t =>
        {
            t.HasCheckConstraint("ck_billing_documents_kind", "kind IN ('deposit', 'milestone', 'final', 'other')");
            t.HasCheckConstraint("ck_billing_documents_status", "status IN ('issued', 'partially_paid', 'paid', 'voided')");
            t.HasCheckConstraint("ck_billing_documents_amounts", "amount > 0 AND paid_amount >= 0 AND paid_amount <= amount");
            t.HasCheckConstraint("ck_billing_documents_void", "(status = 'voided') = (void_reason IS NOT NULL) AND (status <> 'voided' OR paid_amount = 0)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.PaidAmount).HasColumnName("paid_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.DueDate).HasColumnName("due_date");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ReferenceHash).HasColumnName("reference_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.VoidReason).HasColumnName("void_reason").HasMaxLength(500);
        builder.Property(x => x.VoidedAtUtc).HasColumnName("voided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.VoidedByUserId).HasColumnName("voided_by_user_id");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.IssuedAtUtc).HasColumnName("issued_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.VoidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Payments).WithOne().HasForeignKey(p => p.BillingDocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Payments).HasField("_payments").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", "finance", t =>
        {
            t.HasCheckConstraint("ck_payments_method", "method IN ('transfer', 'cheque', 'cash', 'card')");
            t.HasCheckConstraint("ck_payments_status", "status IN ('recorded', 'reversed')");
            t.HasCheckConstraint("ck_payments_amount", "amount > 0");
            t.HasCheckConstraint("ck_payments_reversal", "(status = 'reversed') = (reversal_reason IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BillingDocumentId).HasColumnName("billing_document_id").IsRequired();
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Method).HasColumnName("method").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Reference).HasColumnName("reference").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ReceivedDate).HasColumnName("received_date").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ReversalReason).HasColumnName("reversal_reason").HasMaxLength(500);
        builder.Property(x => x.ReversedAtUtc).HasColumnName("reversed_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ReversedByUserId).HasColumnName("reversed_by_user_id");
        builder.Property(x => x.RecordedByUserId).HasColumnName("recorded_by_user_id").IsRequired();
        builder.Property(x => x.RecordedAtUtc).HasColumnName("recorded_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        // The bank/slip reference identifies a real-world payment: it can be recorded once, ever.
        builder.HasIndex(x => new { x.OrganizationId, x.Reference }).IsUnique();
        builder.HasIndex(x => x.BillingDocumentId);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReversedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AccountingOutboxMessageConfiguration : IEntityTypeConfiguration<AccountingOutboxMessage>
{
    public void Configure(EntityTypeBuilder<AccountingOutboxMessage> builder)
    {
        builder.ToTable("accounting_outbox", "finance", t =>
        {
            t.HasCheckConstraint("ck_accounting_outbox_status", "status IN ('pending', 'sent', 'failed', 'dead')");
            t.HasCheckConstraint("ck_accounting_outbox_kind", "kind IN ('billing.issued', 'billing.voided', 'payment.recorded', 'payment.reversed')");
            t.HasCheckConstraint("ck_accounting_outbox_attempts", "attempts >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.DedupeKey).HasColumnName("dedupe_key").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ResourceId).HasColumnName("resource_id").IsRequired();
        builder.Property(x => x.ResourceNumber).HasColumnName("resource_number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.PayloadJson).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Attempts).HasColumnName("attempts").IsRequired();
        builder.Property(x => x.NextAttemptAtUtc).HasColumnName("next_attempt_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.LastError).HasColumnName("last_error").HasMaxLength(500);
        builder.Property(x => x.ExternalRef).HasColumnName("external_ref").HasMaxLength(100);
        builder.Property(x => x.ExternalAmount).HasColumnName("external_amount").HasPrecision(18, 2);
        builder.Property(x => x.ConfirmedAtUtc).HasColumnName("confirmed_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.SentAtUtc).HasColumnName("sent_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        // The same business event can be queued only once.
        builder.HasIndex(x => new { x.OrganizationId, x.DedupeKey }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.NextAttemptAtUtc });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
