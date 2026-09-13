using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteSurveyRevisionConfiguration : IEntityTypeConfiguration<SiteSurveyRevision>
{
    public void Configure(EntityTypeBuilder<SiteSurveyRevision> builder)
    {
        builder.ToTable("site_survey_revisions", "crm", t =>
        {
            t.HasCheckConstraint("CK_site_survey_revisions_status", "status IN ('draft', 'ready', 'superseded', 'void')");
            t.HasCheckConstraint("CK_site_survey_revisions_readiness", "readiness IN ('incomplete', 'requiresAttention', 'ready')");
            t.HasCheckConstraint("CK_site_survey_revisions_revision_number", "revision_number > 0");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SiteSurveyId).HasColumnName("site_survey_id").IsRequired();
        builder.Property(x => x.RevisionNumber).HasColumnName("revision_number").IsRequired();
        builder.Property(x => x.SurveyTemplateVersion).HasColumnName("survey_template_version").HasMaxLength(64).IsRequired();
        builder.Property(x => x.VisitedAtUtc).HasColumnName("visited_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ScopeSummary).HasColumnName("scope_summary").HasMaxLength(2000);
        builder.Property(x => x.Assumptions).HasColumnName("assumptions").HasColumnType("text[]").IsRequired();
        builder.Property(x => x.Constraints).HasColumnName("constraints").HasColumnType("text[]").IsRequired();
        builder.Property(x => x.MissingDetails).HasColumnName("missing_details").HasColumnType("text[]").IsRequired();
        builder.Property(x => x.Readiness).HasColumnName("readiness").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ReadyAtUtc).HasColumnName("ready_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.ReadyByUserId).HasColumnName("ready_by_user_id");
        builder.Property(x => x.SnapshotHash).HasColumnName("snapshot_hash").HasMaxLength(128);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.SiteSurveyId, x.RevisionNumber }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.SiteSurveyId, x.Status });

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SiteSurvey>()
            .WithMany(x => x.Revisions)
            .HasForeignKey(x => new { x.SiteSurveyId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ReadyByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
