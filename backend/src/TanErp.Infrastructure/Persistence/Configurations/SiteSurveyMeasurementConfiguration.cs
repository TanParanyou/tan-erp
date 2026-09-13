using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Organization;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SiteSurveyMeasurementConfiguration : IEntityTypeConfiguration<SiteSurveyMeasurement>
{
    public void Configure(EntityTypeBuilder<SiteSurveyMeasurement> builder)
    {
        builder.ToTable("site_survey_measurements", "crm", t =>
        {
            t.HasCheckConstraint("CK_site_survey_measurements_value", "value > 0");
            t.HasCheckConstraint("CK_site_survey_measurements_type", "measurement_type IN ('width', 'depth', 'height', 'length', 'area', 'opening', 'count', 'custom')");
            t.HasCheckConstraint("CK_site_survey_measurements_unit", "unit_code IN ('mm', 'cm', 'm', 'sqm', 'unit')");
            t.HasCheckConstraint("CK_site_survey_measurements_capture_method", "capture_method IN ('measured', 'customer_provided', 'derived')");
        });

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SiteSurveyAreaId).HasColumnName("site_survey_area_id").IsRequired();
        builder.Property(x => x.MeasurementType).HasColumnName("measurement_type").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Value).HasColumnName("value").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(16).IsRequired();
        builder.Property(x => x.CaptureMethod).HasColumnName("capture_method").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.SiteSurveyAreaId, x.SortOrder });

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SiteSurveyArea>()
            .WithMany(x => x.Measurements)
            .HasForeignKey(x => new { x.SiteSurveyAreaId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
