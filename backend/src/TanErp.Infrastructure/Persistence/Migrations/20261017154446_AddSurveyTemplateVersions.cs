using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyTemplateVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "survey_template_versions",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    required_checklist_items = table.Column<string[]>(type: "text[]", nullable: false),
                    minimum_evidence_count = table.Column<int>(type: "integer", nullable: false),
                    minimum_area_evidence_count = table.Column<int>(type: "integer", nullable: false),
                    snapshot_hash_version = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    effective_from_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    effective_to_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_template_versions", x => x.id);
                    table.CheckConstraint("CK_survey_template_versions_effective_range", "effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
                    table.CheckConstraint("CK_survey_template_versions_evidence_counts", "minimum_evidence_count >= 0 AND minimum_area_evidence_count >= 0");
                    table.CheckConstraint("CK_survey_template_versions_hash_version", "snapshot_hash_version IN ('v2', 'v3', 'v4')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_survey_template_versions_code",
                schema: "crm",
                table: "survey_template_versions",
                column: "code",
                unique: true);

            // Seed the two built-in templates byte-for-byte as in SurveyTemplates.BaselineV1/BaselineV2 so the snapshot hash
            // of every existing survey is unchanged (the hash depends only on snapshot_hash_version).
            migrationBuilder.Sql(@"
INSERT INTO crm.survey_template_versions
    (id, code, required_checklist_items, minimum_evidence_count, minimum_area_evidence_count, snapshot_hash_version, effective_from_utc, effective_to_utc, created_at_utc, created_by_user_id)
VALUES
    ('019a7c00-0000-7000-8000-000000000001', 'SURVEY-BASELINE-v1', ARRAY[]::text[], 0, 0, 'v2', '2026-01-01T00:00:00Z', NULL, '2026-01-01T00:00:00Z', NULL),
    ('019a7c00-0000-7000-8000-000000000002', 'SURVEY-BASELINE-v2',
     ARRAY['site_access_confirmed','utilities_checked','existing_conditions_inspected','customer_requirements_confirmed']::text[],
     1, 0, 'v3', '2026-01-01T00:00:00Z', NULL, '2026-01-01T00:00:00Z', NULL);");

            // Published content is immutable: only effective_to_utc may change, once (NULL -> value); rows are never deleted.
            migrationBuilder.Sql(@"
CREATE FUNCTION crm.survey_template_versions_guard() RETURNS trigger AS $$
BEGIN
    IF TG_OP = 'DELETE' THEN
        RAISE EXCEPTION 'survey_template_versions rows cannot be deleted';
    END IF;
    IF NEW.id IS DISTINCT FROM OLD.id OR NEW.code IS DISTINCT FROM OLD.code
       OR NEW.required_checklist_items IS DISTINCT FROM OLD.required_checklist_items
       OR NEW.minimum_evidence_count IS DISTINCT FROM OLD.minimum_evidence_count
       OR NEW.minimum_area_evidence_count IS DISTINCT FROM OLD.minimum_area_evidence_count
       OR NEW.snapshot_hash_version IS DISTINCT FROM OLD.snapshot_hash_version
       OR NEW.effective_from_utc IS DISTINCT FROM OLD.effective_from_utc
       OR NEW.created_at_utc IS DISTINCT FROM OLD.created_at_utc
       OR NEW.created_by_user_id IS DISTINCT FROM OLD.created_by_user_id
       OR (OLD.effective_to_utc IS NOT NULL AND NEW.effective_to_utc IS DISTINCT FROM OLD.effective_to_utc) THEN
        RAISE EXCEPTION 'published survey template versions are immutable';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_survey_template_versions_guard
BEFORE UPDATE OR DELETE ON crm.survey_template_versions
FOR EACH ROW EXECUTE FUNCTION crm.survey_template_versions_guard();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_survey_template_versions_guard ON crm.survey_template_versions; DROP FUNCTION IF EXISTS crm.survey_template_versions_guard();");

            migrationBuilder.DropTable(
                name: "survey_template_versions",
                schema: "crm");
        }
    }
}
