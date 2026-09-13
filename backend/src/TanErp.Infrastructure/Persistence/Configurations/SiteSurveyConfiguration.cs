using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteSurveyConfiguration : IEntityTypeConfiguration<SiteSurvey>
{
    public void Configure(EntityTypeBuilder<SiteSurvey> builder)
    {
        builder.ToTable("site_surveys", "crm", t =>
        {
            t.HasCheckConstraint("CK_site_surveys_status", "status IN ('scheduled', 'in_progress', 'completed', 'cancelled')");
            t.HasCheckConstraint("CK_site_surveys_schedule_range", "scheduled_start_utc IS NULL OR scheduled_end_utc IS NULL OR scheduled_end_utc > scheduled_start_utc");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.OpportunityId).HasColumnName("opportunity_id").IsRequired();
        builder.Property(x => x.SiteId).HasColumnName("site_id").IsRequired();
        builder.Property(x => x.SurveyNumber).HasColumnName("survey_number").HasMaxLength(32).IsRequired();
        builder.Property(x => x.AssignedSurveyorId).HasColumnName("assigned_surveyor_id").IsRequired();
        builder.Property(x => x.ScheduledStartUtc).HasColumnName("scheduled_start_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ScheduledEndUtc).HasColumnName("scheduled_end_utc").HasColumnType("timestamptz");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.SurveyNumber }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.OpportunityId, x.Id });
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.Status, x.ScheduledStartUtc });

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => new { x.BranchId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Opportunity>()
            .WithMany()
            .HasForeignKey(x => new { x.OpportunityId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => new { x.SiteId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AssignedSurveyorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Revisions)
            .WithOne()
            .HasForeignKey(x => x.SiteSurveyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
