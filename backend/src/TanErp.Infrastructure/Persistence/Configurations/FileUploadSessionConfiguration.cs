using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Files;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class FileUploadSessionConfiguration : IEntityTypeConfiguration<FileUploadSession>
{
    public void Configure(EntityTypeBuilder<FileUploadSession> builder)
    {
        builder.ToTable("file_upload_sessions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(s => s.ParentType).HasColumnName("parent_type").HasMaxLength(50).IsRequired();
        builder.Property(s => s.ParentId).HasColumnName("parent_id");
        builder.Property(s => s.CreationIntentId).HasColumnName("creation_intent_id");
        builder.Property(s => s.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(s => s.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(s => s.ExpiresAtUtc).HasColumnName("expires_at_utc").IsRequired();
        builder.Property(s => s.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(s => s.IdempotencyKeyHash).HasColumnName("idempotency_key_hash").HasMaxLength(128).IsRequired();
        builder.Property(s => s.RequestPayloadHash).HasColumnName("request_payload_hash").HasMaxLength(128).IsRequired();

        builder.HasMany(s => s.Slots)
            .WithOne()
            .HasForeignKey(slot => slot.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.OrganizationId, s.CreatedByUserId, s.IdempotencyKeyHash })
            .IsUnique()
            .HasDatabaseName("ux_file_upload_sessions_org_actor_idempotency");

        builder.HasIndex(s => new { s.OrganizationId, s.ParentType, s.ParentId })
            .HasDatabaseName("ix_file_upload_sessions_parent");

        builder.HasIndex(s => new { s.OrganizationId, s.CreationIntentId })
            .HasDatabaseName("ix_file_upload_sessions_creation_intent");
    }
}
