using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteSurveyChecklistResultConfiguration : IEntityTypeConfiguration<SiteSurveyChecklistResult>
{
    public void Configure(EntityTypeBuilder<SiteSurveyChecklistResult> builder)
    {
        builder.ToTable("site_survey_checklist_results", "crm", t =>
        {
            t.HasCheckConstraint("CK_site_survey_checklist_results_result", "result IN ('pass', 'fail', 'not_applicable')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SiteSurveyRevisionId).HasColumnName("site_survey_revision_id").IsRequired();
        builder.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Result).HasColumnName("result").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);

        builder.HasIndex(x => new { x.OrganizationId, x.SiteSurveyRevisionId, x.ItemCode }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SiteSurveyRevision>()
            .WithMany(x => x.ChecklistResults)
            .HasForeignKey(x => new { x.SiteSurveyRevisionId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
