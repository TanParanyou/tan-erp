using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Files;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class FileUploadSlotConfiguration : IEntityTypeConfiguration<FileUploadSlot>
{
    public void Configure(EntityTypeBuilder<FileUploadSlot> builder)
    {
        builder.ToTable("file_upload_slots");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.SessionId).HasColumnName("session_id").IsRequired();
        builder.Property(s => s.SlotIndex).HasColumnName("slot_index").IsRequired();
        builder.Property(s => s.Filename).HasColumnName("filename").HasMaxLength(255).IsRequired();
        builder.Property(s => s.MediaType).HasColumnName("media_type").HasMaxLength(100).IsRequired();
        builder.Property(s => s.FileSizeBytes).HasColumnName("file_size_bytes").IsRequired();
        builder.Property(s => s.UploadedFileId).HasColumnName("uploaded_file_id");

        builder.HasIndex(s => new { s.SessionId, s.SlotIndex })
            .IsUnique()
            .HasDatabaseName("ix_file_upload_slots_session_slot_index");
    }
}
