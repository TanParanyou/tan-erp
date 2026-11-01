using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Configurations;

public sealed class UserBackupCodeConfiguration : IEntityTypeConfiguration<UserBackupCode>
{
    public void Configure(EntityTypeBuilder<UserBackupCode> builder)
    {
        builder.ToTable("user_backup_codes", "identity_access");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.CodeHash).HasColumnName("code_hash").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.UsedAtUtc).HasColumnName("used_at_utc").HasColumnType("timestamptz");

        builder.HasIndex(x => new { x.UserId, x.UsedAtUtc })
            .HasDatabaseName("ix_user_backup_codes_user_id");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
