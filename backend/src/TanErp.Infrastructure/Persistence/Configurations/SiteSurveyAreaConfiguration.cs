using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteSurveyAreaConfiguration : IEntityTypeConfiguration<SiteSurveyArea>
{
    public void Configure(EntityTypeBuilder<SiteSurveyArea> builder)
    {
        builder.ToTable("site_survey_areas", "crm");

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SiteSurveyRevisionId).HasColumnName("site_survey_revision_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.SiteSurveyRevisionId, x.Code }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SiteSurveyRevision>()
            .WithMany(x => x.Areas)
            .HasForeignKey(x => new { x.SiteSurveyRevisionId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
