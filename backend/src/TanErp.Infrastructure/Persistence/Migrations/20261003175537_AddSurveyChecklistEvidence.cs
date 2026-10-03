using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyChecklistEvidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_survey_checklist_results",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    result = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_survey_checklist_results", x => x.id);
                    table.CheckConstraint("CK_site_survey_checklist_results_result", "result IN ('pass', 'fail', 'not_applicable')");
                    table.ForeignKey(
                        name: "FK_site_survey_checklist_results_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_survey_checklist_results_site_survey_revisions_site_su~",
                        columns: x => new { x.site_survey_revision_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "site_survey_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_survey_evidence",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_survey_evidence", x => x.id);
                    table.CheckConstraint("CK_site_survey_evidence_kind", "kind IN ('site_photo', 'measurement_sketch', 'other')");
                    table.ForeignKey(
                        name: "FK_site_survey_evidence_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_survey_evidence_site_survey_revisions_site_survey_revi~",
                        columns: x => new { x.site_survey_revision_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "site_survey_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_site_survey_evidence_uploaded_files_file_id",
                        column: x => x.file_id,
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_checklist_results_organization_id_site_survey_r~",
                schema: "crm",
                table: "site_survey_checklist_results",
                columns: new[] { "organization_id", "site_survey_revision_id", "item_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_checklist_results_site_survey_revision_id_organ~",
                schema: "crm",
                table: "site_survey_checklist_results",
                columns: new[] { "site_survey_revision_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_evidence_file_id",
                schema: "crm",
                table: "site_survey_evidence",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_evidence_organization_id_site_survey_revision_i~",
                schema: "crm",
                table: "site_survey_evidence",
                columns: new[] { "organization_id", "site_survey_revision_id", "file_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_evidence_site_survey_revision_id_organization_id",
                schema: "crm",
                table: "site_survey_evidence",
                columns: new[] { "site_survey_revision_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_survey_checklist_results",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "site_survey_evidence",
                schema: "crm");
        }
    }
}
