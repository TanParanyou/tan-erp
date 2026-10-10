using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Attachments;
using TanErp.Domain.Files;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SignatureCaptureConfiguration : IEntityTypeConfiguration<SignatureCapture>
{
    public void Configure(EntityTypeBuilder<SignatureCapture> builder)
    {
        builder.ToTable("signature_captures", "files", t =>
        {
            t.HasCheckConstraint("ck_signature_captures_owner_type_format", "owner_type ~ '^[a-z][a-z0-9-]{1,39}$'");
            t.HasCheckConstraint("ck_signature_captures_purpose_format", "purpose ~ '^[a-z][a-z-]{1,31}$'");
            t.HasCheckConstraint("ck_signature_captures_content_hash", "content_hash ~ '^[0-9a-f]{64}$'");
            t.HasCheckConstraint("ck_signature_captures_signer_name", "char_length(signer_name) BETWEEN 1 AND 200");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.OwnerType).HasColumnName("owner_type").HasMaxLength(40).IsRequired();
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SignerName).HasColumnName("signer_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.SignerRole).HasColumnName("signer_role").HasMaxLength(100);
        builder.Property(x => x.SignedAtUtc).HasColumnName("signed_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ImageFileId).HasColumnName("image_file_id").IsRequired();
        builder.Property(x => x.ConsentTextVersion).HasColumnName("consent_text_version").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ContentHash).HasColumnName("content_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CapturedByUserId).HasColumnName("captured_by_user_id").IsRequired();

        // One image file can be the evidence of one signature only.
        builder.HasIndex(x => x.ImageFileId).IsUnique().HasDatabaseName("ux_signature_captures_image_file");
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerType, x.OwnerId, x.SignedAtUtc }).HasDatabaseName("ix_signature_captures_owner");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CapturedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.ImageFileId, x.OrganizationId })
            .HasPrincipalKey(f => new { f.Id, f.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
