using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Files;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.ToTable("uploaded_files", "files", t =>
        {
            t.HasCheckConstraint(
                "CK_uploaded_files_status",
                "status IN ('verified')");
            t.HasCheckConstraint(
                "CK_uploaded_files_file_size_bytes",
                "file_size_bytes > 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.StoragePath).HasColumnName("storage_path").HasMaxLength(500).IsRequired();
        builder.Property(x => x.OriginalFilename).HasColumnName("original_filename").HasMaxLength(255).IsRequired();
        builder.Property(x => x.MediaType).HasColumnName("media_type").HasMaxLength(100).IsRequired();
        builder.Property(x => x.FileSizeBytes).HasColumnName("file_size_bytes").IsRequired();
        builder.Property(x => x.UploadSessionId).HasColumnName("upload_session_id").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.UploadedByUserId).HasColumnName("uploaded_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.Id });
        builder.HasIndex(x => x.UploadSessionId);
    }
}
