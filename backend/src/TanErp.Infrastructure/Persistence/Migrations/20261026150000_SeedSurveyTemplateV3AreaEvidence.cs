using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Publishes SURVEY-BASELINE-v3 (same checklist as v2, snapshot hash v4) as the newest effective template so new survey
    /// revisions support survey-area attachments. Data-only: the model is unchanged, so there is no Designer snapshot.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261026150000_SeedSurveyTemplateV3AreaEvidence")]
    public partial class SeedSurveyTemplateV3AreaEvidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO crm.survey_template_versions
    (id, code, required_checklist_items, minimum_evidence_count, minimum_area_evidence_count, snapshot_hash_version, effective_from_utc, effective_to_utc, created_at_utc, created_by_user_id)
VALUES
    ('019a7c00-0000-7000-8000-000000000003', 'SURVEY-BASELINE-v3',
     ARRAY['site_access_confirmed','utilities_checked','existing_conditions_inspected','customer_requirements_confirmed']::text[],
     1, 0, 'v4', '2026-10-26T00:00:00Z', NULL, '2026-10-26T00:00:00Z', NULL);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The guard trigger forbids deleting published templates, so rollback is intentionally a no-op.
        }
    }
}
