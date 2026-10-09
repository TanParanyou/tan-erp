using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Notifications;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications", "notifications", t =>
        {
            // The real type list lives in code (NotificationTypes); the database only guards the shape, never more than the domain does.
            t.HasCheckConstraint("ck_notifications_type_format", "type ~ '^[a-z][a-z0-9.-]{1,59}$'");
            t.HasCheckConstraint("ck_notifications_dedupe_key_length", "char_length(btrim(dedupe_key)) BETWEEN 1 AND 160");
            t.HasCheckConstraint("ck_notifications_payload_object", "jsonb_typeof(payload) = 'object'");
        });

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.IsRead);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.RecipientUserId).HasColumnName("recipient_user_id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasMaxLength(60).IsRequired();
        builder.Property(x => x.PayloadJson).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.DedupeKey).HasColumnName("dedupe_key").HasMaxLength(Notification.MaxDedupeKeyLength).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ReadAtUtc).HasColumnName("read_at_utc").HasColumnType("timestamptz");

        // One message per (recipient, transition); also closes the race two concurrent submits could open.
        builder.HasIndex(x => new { x.OrganizationId, x.RecipientUserId, x.DedupeKey })
            .IsUnique()
            .HasDatabaseName("ux_notifications_recipient_dedupe");
        // Newest-first listing for one recipient.
        builder.HasIndex(x => new { x.OrganizationId, x.RecipientUserId, x.CreatedAtUtc, x.Id })
            .IsDescending(false, false, true, true)
            .HasDatabaseName("ix_notifications_recipient_created");
        // The bell badge only ever counts unread rows.
        builder.HasIndex(x => new { x.OrganizationId, x.RecipientUserId })
            .HasFilter("read_at_utc IS NULL")
            .HasDatabaseName("ix_notifications_unread");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RecipientUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
