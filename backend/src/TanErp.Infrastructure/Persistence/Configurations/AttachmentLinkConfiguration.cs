using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Attachments;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Files;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class AttachmentLinkConfiguration : IEntityTypeConfiguration<AttachmentLink>
{
    public void Configure(EntityTypeBuilder<AttachmentLink> builder)
    {
        builder.ToTable("attachment_links", "files", t =>
        {
            // The real owner-type list lives in code (AttachmentOwnerTypes); the database only guards the shape.
            t.HasCheckConstraint("ck_attachment_links_owner_type_format", "owner_type ~ '^[a-z][a-z0-9-]{1,39}$'");
            t.HasCheckConstraint("ck_attachment_links_purpose_format", "purpose ~ '^[a-z][a-z-]{1,31}$'");
            t.HasCheckConstraint("ck_attachment_links_removed_pair", "(removed_at_utc IS NULL) = (removed_by_user_id IS NULL)");
        });

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.IsActive);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.OwnerType).HasColumnName("owner_type").HasMaxLength(40).IsRequired();
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RemovedAtUtc).HasColumnName("removed_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RemovedByUserId).HasColumnName("removed_by_user_id");

        // One active link per (owner, file, purpose); also closes the race two concurrent requests could open.
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerType, x.OwnerId, x.FileId, x.Purpose })
            .IsUnique()
            .HasFilter("removed_at_utc IS NULL")
            .HasDatabaseName("ux_attachment_links_active_owner_file_purpose");
        builder.HasIndex(x => new { x.OrganizationId, x.OwnerType, x.OwnerId }).HasDatabaseName("ix_attachment_links_owner");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RemovedByUserId).OnDelete(DeleteBehavior.Restrict);

        // Composite key makes "file in the same organization as the link" a database guarantee.
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.FileId, x.OrganizationId })
            .HasPrincipalKey(f => new { f.Id, f.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
