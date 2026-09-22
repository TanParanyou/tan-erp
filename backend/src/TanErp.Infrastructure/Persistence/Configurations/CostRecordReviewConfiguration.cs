using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class CostRecordReviewConfiguration : IEntityTypeConfiguration<CostRecordReview>
{
    public void Configure(EntityTypeBuilder<CostRecordReview> builder)
    {
        builder.ToTable("cost_record_reviews", "item_master", t =>
        {
            t.HasCheckConstraint("CK_cost_record_reviews_decision", "decision IN ('approved', 'returned')");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.CostRecordId).HasColumnName("cost_record_id").IsRequired();
        builder.Property(x => x.Decision).HasColumnName("decision").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(x => x.ReviewerUserId).HasColumnName("reviewer_user_id").IsRequired();
        builder.Property(x => x.AuthoritySnapshot).HasColumnName("authority_snapshot").HasMaxLength(256);
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.CostRecordId, x.DecidedAtUtc });

        builder.HasOne<CostRecord>()
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => new { x.CostRecordId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
