using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "identity_access");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.FirebaseUid).HasColumnName("firebase_uid").HasMaxLength(128);
        builder.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => x.FirebaseUid)
            .IsUnique()
            .HasFilter("firebase_uid IS NOT NULL")
            .HasDatabaseName("ix_users_firebase_uid");

        // One user per email: administrators invite by email and the first verified login links the identity.
        builder.HasIndex(x => x.NormalizedEmail)
            .IsUnique()
            .HasFilter("normalized_email <> ''")
            .HasDatabaseName("ix_users_normalized_email");

        builder.HasMany(x => x.Memberships)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
