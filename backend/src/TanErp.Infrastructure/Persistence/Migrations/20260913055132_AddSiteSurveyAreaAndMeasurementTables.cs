using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSurveyAreaAndMeasurementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_survey_areas",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_survey_areas", x => x.id);
                    table.UniqueConstraint("AK_site_survey_areas_id_organization_id", x => new { x.id, x.organization_id });
                    table.ForeignKey(
                        name: "FK_site_survey_areas_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_survey_areas_site_survey_revisions_site_survey_revisio~",
                        columns: x => new { x.site_survey_revision_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "site_survey_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_survey_measurements",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    measurement_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    capture_method = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_survey_measurements", x => x.id);
                    table.UniqueConstraint("AK_site_survey_measurements_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_site_survey_measurements_capture_method", "capture_method IN ('measured', 'customer_provided', 'derived')");
                    table.CheckConstraint("CK_site_survey_measurements_type", "measurement_type IN ('width', 'depth', 'height', 'length', 'area', 'opening', 'count', 'custom')");
                    table.CheckConstraint("CK_site_survey_measurements_unit", "unit_code IN ('mm', 'cm', 'm', 'sqm', 'unit')");
                    table.CheckConstraint("CK_site_survey_measurements_value", "value > 0");
                    table.ForeignKey(
                        name: "FK_site_survey_measurements_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_survey_measurements_site_survey_areas_site_survey_area~",
                        columns: x => new { x.site_survey_area_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "site_survey_areas",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_areas_organization_id_site_survey_revision_id_c~",
                schema: "crm",
                table: "site_survey_areas",
                columns: new[] { "organization_id", "site_survey_revision_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_areas_site_survey_revision_id_organization_id",
                schema: "crm",
                table: "site_survey_areas",
                columns: new[] { "site_survey_revision_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_measurements_organization_id_site_survey_area_i~",
                schema: "crm",
                table: "site_survey_measurements",
                columns: new[] { "organization_id", "site_survey_area_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_measurements_site_survey_area_id_organization_id",
                schema: "crm",
                table: "site_survey_measurements",
                columns: new[] { "site_survey_area_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_survey_measurements",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "site_survey_areas",
                schema: "crm");
        }
    }
}
