using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Files;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteSurveyEvidenceConfiguration : IEntityTypeConfiguration<SiteSurveyEvidence>
{
    public void Configure(EntityTypeBuilder<SiteSurveyEvidence> builder)
    {
        builder.ToTable("site_survey_evidence", "crm", t =>
        {
            t.HasCheckConstraint("CK_site_survey_evidence_kind", "kind IN ('site_photo', 'measurement_sketch', 'other')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SiteSurveyRevisionId).HasColumnName("site_survey_revision_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Caption).HasColumnName("caption").HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.SiteSurveyRevisionId, x.FileId }).IsUnique();
        builder.HasIndex(x => x.FileId);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SiteSurveyRevision>()
            .WithMany(x => x.Evidence)
            .HasForeignKey(x => new { x.SiteSurveyRevisionId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
