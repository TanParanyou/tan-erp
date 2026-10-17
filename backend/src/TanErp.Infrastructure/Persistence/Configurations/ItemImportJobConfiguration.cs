using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class ItemImportJobConfiguration : IEntityTypeConfiguration<ItemImportJob>
{
    public void Configure(EntityTypeBuilder<ItemImportJob> builder)
    {
        builder.ToTable("item_import_jobs", "item_master", t =>
        {
            t.HasCheckConstraint("CK_item_import_jobs_kind", "kind IN ('items', 'costs')");
            t.HasCheckConstraint("CK_item_import_jobs_mode", "mode IN ('create', 'upsert')");
            t.HasCheckConstraint("CK_item_import_jobs_status", "status IN ('queued', 'running', 'completed', 'completed_with_errors', 'failed')");
            t.HasCheckConstraint("CK_item_import_jobs_progress", "processed_rows >= 0 AND processed_rows <= total_rows AND next_batch_index >= 0 AND next_batch_index <= total_batches");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Mode).HasColumnName("mode").HasMaxLength(16).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ContentSha256).HasColumnName("content_sha256").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Content).HasColumnName("content");
        builder.Property(x => x.TotalRows).HasColumnName("total_rows").IsRequired();
        builder.Property(x => x.BatchSize).HasColumnName("batch_size").IsRequired();
        builder.Property(x => x.TotalBatches).HasColumnName("total_batches").IsRequired();
        builder.Property(x => x.NextBatchIndex).HasColumnName("next_batch_index").IsRequired();
        builder.Property(x => x.ProcessedRows).HasColumnName("processed_rows").IsRequired();
        builder.Property(x => x.CreatedCount).HasColumnName("created_count").IsRequired();
        builder.Property(x => x.UpdatedCount).HasColumnName("updated_count").IsRequired();
        builder.Property(x => x.UnchangedCount).HasColumnName("unchanged_count").IsRequired();
        builder.Property(x => x.RejectedBatchCount).HasColumnName("rejected_batch_count").IsRequired();
        builder.Property(x => x.ErrorRowCount).HasColumnName("error_row_count").IsRequired();
        builder.Property(x => x.ErrorsJson).HasColumnName("errors_json").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.FailureCode).HasColumnName("failure_code").HasMaxLength(64);
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired();
        builder.Property(x => x.RequestedByMembershipId).HasColumnName("requested_by_membership_id").IsRequired();
        builder.Property(x => x.RequestedByFirebaseUid).HasColumnName("requested_by_firebase_uid").HasMaxLength(128).IsRequired();
        builder.Property(x => x.IdempotencyKeyHash).HasColumnName("idempotency_key_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.TraceId).HasColumnName("trace_id").HasMaxLength(128).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.StartedAtUtc).HasColumnName("started_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.CompletedAtUtc).HasColumnName("completed_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.LeaseExpiresAtUtc).HasColumnName("lease_expires_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.IdempotencyKeyHash }).IsUnique();
        builder.HasIndex(x => new { x.Status, x.CreatedAtUtc });
    }
}
