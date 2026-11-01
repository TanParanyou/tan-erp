using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions", "identity_access", t =>
        {
            t.HasCheckConstraint("ck_user_sessions_client_platform", "client_platform IN ('web', 'mobile')");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.CurrentRefreshTokenHash).HasColumnName("current_refresh_token_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.PreviousRefreshTokenHash).HasColumnName("previous_refresh_token_hash").HasMaxLength(64);
        builder.Property(x => x.LastRotatedAtUtc).HasColumnName("last_rotated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.GracePeriodUntilUtc).HasColumnName("grace_period_until_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ClientPlatform).HasColumnName("client_platform").HasMaxLength(16).IsRequired();
        builder.Property(x => x.DeviceLabel).HasColumnName("device_label").HasMaxLength(128).IsRequired();
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(64).IsRequired();
        builder.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(512).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.LastUsedAtUtc).HasColumnName("last_used_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RevokedAtUtc).HasColumnName("revoked_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RevokeReason).HasColumnName("revoke_reason").HasMaxLength(64);

        builder.HasIndex(x => new { x.UserId, x.RevokedAtUtc, x.ExpiresAtUtc })
            .HasDatabaseName("ix_user_sessions_user_active");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
