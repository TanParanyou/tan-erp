using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Common;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("audit_events", "audit");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.ActorUserId).HasColumnName("actor_user_id").IsRequired();
        builder.Property(x => x.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ResourceType).HasColumnName("resource_type").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ResourceId).HasColumnName("resource_id").HasMaxLength(100).IsRequired();
        builder.Property(x => x.OccurredAtUtc).HasColumnName("occurred_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.TraceId).HasColumnName("trace_id").HasMaxLength(128).IsRequired();
        builder.Property(x => x.ChangesJson).HasColumnName("changes").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        builder.Property(x => x.ActorMembershipId).HasColumnName("actor_membership_id");
        builder.Property(x => x.RequestId).HasColumnName("request_id").HasMaxLength(128);
        builder.Property(x => x.RowVersionBefore).HasColumnName("row_version_before");
        builder.Property(x => x.RowVersionAfter).HasColumnName("row_version_after");
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500);

        builder.HasIndex(x => new { x.OrganizationId, x.ResourceType, x.ResourceId, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.OccurredAtUtc });
    }
}
