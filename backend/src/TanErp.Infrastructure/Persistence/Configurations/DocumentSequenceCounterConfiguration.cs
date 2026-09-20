using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.DocumentNumbering;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class DocumentSequenceCounterConfiguration : IEntityTypeConfiguration<DocumentSequenceCounter>
{
    public void Configure(EntityTypeBuilder<DocumentSequenceCounter> builder)
    {
        builder.ToTable("sequence_counters", "common");

        builder.HasKey(x => new { x.OrganizationId, x.DocumentType, x.BranchId, x.PeriodKey });

        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.DocumentType).HasColumnName("document_type").HasMaxLength(64).IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.PeriodKey).HasColumnName("period_key").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CurrentValue).HasColumnName("current_val").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
    }
}
