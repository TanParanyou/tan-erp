using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.DocumentNumbering;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class DocumentSequenceDefinitionConfiguration : IEntityTypeConfiguration<DocumentSequenceDefinition>
{
    public void Configure(EntityTypeBuilder<DocumentSequenceDefinition> builder)
    {
        builder.ToTable("document_sequence_definitions", "common");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.DocumentType).HasColumnName("document_type").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Prefix).HasColumnName("prefix").HasMaxLength(32).IsRequired();
        builder.Property(x => x.FormatPattern).HasColumnName("format_pattern").HasMaxLength(128).IsRequired();
        builder.Property(x => x.ResetPeriod).HasColumnName("reset_period").HasConversion<int>().IsRequired();
        builder.Property(x => x.Padding).HasColumnName("padding").IsRequired();
        builder.Property(x => x.IsBranchSpecific).HasColumnName("is_branch_specific").IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.DocumentType }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
