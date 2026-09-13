using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSurveyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_surveys",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    survey_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    assigned_surveyor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_start_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    scheduled_end_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_surveys", x => x.id);
                    table.UniqueConstraint("AK_site_surveys_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_site_surveys_schedule_range", "scheduled_start_utc IS NULL OR scheduled_end_utc IS NULL OR scheduled_end_utc > scheduled_start_utc");
                    table.CheckConstraint("CK_site_surveys_status", "status IN ('scheduled', 'in_progress', 'completed', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_site_surveys_branches_branch_id_organization_id",
                        columns: x => new { x.branch_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_surveys_opportunities_opportunity_id_organization_id",
                        columns: x => new { x.opportunity_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_surveys_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_surveys_sites_site_id_organization_id",
                        columns: x => new { x.site_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "sites",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_surveys_users_assigned_surveyor_id",
                        column: x => x.assigned_surveyor_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "site_survey_revisions",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision_number = table.Column<int>(type: "integer", nullable: false),
                    survey_template_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    visited_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    scope_summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    assumptions = table.Column<string[]>(type: "text[]", nullable: false),
                    constraints = table.Column<string[]>(type: "text[]", nullable: false),
                    missing_details = table.Column<string[]>(type: "text[]", nullable: false),
                    readiness = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ready_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ready_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    snapshot_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_survey_revisions", x => x.id);
                    table.UniqueConstraint("AK_site_survey_revisions_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_site_survey_revisions_readiness", "readiness IN ('incomplete', 'requiresAttention', 'ready')");
                    table.CheckConstraint("CK_site_survey_revisions_revision_number", "revision_number > 0");
                    table.CheckConstraint("CK_site_survey_revisions_status", "status IN ('draft', 'ready', 'superseded', 'void')");
                    table.ForeignKey(
                        name: "FK_site_survey_revisions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_survey_revisions_site_surveys_site_survey_id_organizat~",
                        columns: x => new { x.site_survey_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "site_surveys",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_site_survey_revisions_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_survey_revisions_users_ready_by_user_id",
                        column: x => x.ready_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_created_by_user_id",
                schema: "crm",
                table: "site_survey_revisions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_organization_id_site_survey_id_revisi~",
                schema: "crm",
                table: "site_survey_revisions",
                columns: new[] { "organization_id", "site_survey_id", "revision_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_organization_id_site_survey_id_status",
                schema: "crm",
                table: "site_survey_revisions",
                columns: new[] { "organization_id", "site_survey_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_ready_by_user_id",
                schema: "crm",
                table: "site_survey_revisions",
                column: "ready_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_site_survey_id_organization_id",
                schema: "crm",
                table: "site_survey_revisions",
                columns: new[] { "site_survey_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_assigned_surveyor_id",
                schema: "crm",
                table: "site_surveys",
                column: "assigned_surveyor_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_branch_id_organization_id",
                schema: "crm",
                table: "site_surveys",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_opportunity_id_organization_id",
                schema: "crm",
                table: "site_surveys",
                columns: new[] { "opportunity_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_organization_id_branch_id_status_scheduled_sta~",
                schema: "crm",
                table: "site_surveys",
                columns: new[] { "organization_id", "branch_id", "status", "scheduled_start_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_organization_id_opportunity_id_id",
                schema: "crm",
                table: "site_surveys",
                columns: new[] { "organization_id", "opportunity_id", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_organization_id_survey_number",
                schema: "crm",
                table: "site_surveys",
                columns: new[] { "organization_id", "survey_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_surveys_site_id_organization_id",
                schema: "crm",
                table: "site_surveys",
                columns: new[] { "site_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_survey_revisions",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "site_surveys",
                schema: "crm");
        }
    }
}
