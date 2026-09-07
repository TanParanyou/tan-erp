using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Common;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("idempotency_records", "audit", t =>
        {
            t.HasCheckConstraint("CK_idempotency_records_hashes_not_empty", "payload_hash <> '' AND resource_id <> ''");
        });

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.OrganizationId, x.Operation, x.KeyHash }).IsUnique();

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Operation).HasColumnName("operation").HasMaxLength(100).IsRequired();
        builder.Property(x => x.KeyHash).HasColumnName("key_hash").HasMaxLength(128).IsRequired();
        builder.Property(x => x.PayloadHash).HasColumnName("payload_hash").HasMaxLength(128).IsRequired();
        builder.Property(x => x.ResourceId).HasColumnName("resource_id").HasMaxLength(128).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
