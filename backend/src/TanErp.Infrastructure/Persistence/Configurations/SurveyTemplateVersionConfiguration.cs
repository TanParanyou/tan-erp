using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SurveyTemplateVersionConfiguration : IEntityTypeConfiguration<SurveyTemplateVersionRow>
{
    public void Configure(EntityTypeBuilder<SurveyTemplateVersionRow> builder)
    {
        builder.ToTable("survey_template_versions", "crm", t =>
        {
            t.HasCheckConstraint("CK_survey_template_versions_hash_version", "snapshot_hash_version IN ('v2', 'v3', 'v4')");
            t.HasCheckConstraint("CK_survey_template_versions_effective_range", "effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
            t.HasCheckConstraint("CK_survey_template_versions_evidence_counts", "minimum_evidence_count >= 0 AND minimum_area_evidence_count >= 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.RequiredChecklistItems).HasColumnName("required_checklist_items").HasColumnType("text[]").IsRequired();
        builder.Property(x => x.MinimumEvidenceCount).HasColumnName("minimum_evidence_count").IsRequired();
        builder.Property(x => x.MinimumAreaEvidenceCount).HasColumnName("minimum_area_evidence_count").IsRequired();
        builder.Property(x => x.SnapshotHashVersion).HasColumnName("snapshot_hash_version").HasMaxLength(8).IsRequired();
        builder.Property(x => x.EffectiveFromUtc).HasColumnName("effective_from_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.EffectiveToUtc).HasColumnName("effective_to_utc").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
