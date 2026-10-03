using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "service");

            migrationBuilder.CreateTable(
                name: "installation_jobs",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    scheduled_start = table.Column<DateOnly>(type: "date", nullable: false),
                    scheduled_end = table.Column<DateOnly>(type: "date", nullable: false),
                    crew_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cancel_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ready_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    handover_date = table.Column<DateOnly>(type: "date", nullable: true),
                    handover_signer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    handover_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    dispute_count = table.Column<int>(type: "integer", nullable: false),
                    last_dispute_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    warranty_months = table.Column<int>(type: "integer", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_installation_jobs", x => x.id);
                    table.CheckConstraint("ck_installation_jobs_dates", "scheduled_end >= scheduled_start");
                    table.CheckConstraint("ck_installation_jobs_handover", "(status = 'handed_over') = (handover_date IS NOT NULL)");
                    table.CheckConstraint("ck_installation_jobs_status", "status IN ('planned', 'in_progress', 'ready_for_handover', 'handed_over', 'cancelled')");
                    table.CheckConstraint("ck_installation_jobs_warranty_months", "warranty_months IS NULL OR warranty_months BETWEEN 0 AND 120");
                    table.ForeignKey(
                        name: "FK_installation_jobs_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_installation_jobs_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_installation_jobs_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_installation_jobs_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "installation_checklist_items",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    installation_job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    required = table.Column<bool>(type: "boolean", nullable: false),
                    done = table.Column<bool>(type: "boolean", nullable: false),
                    done_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    done_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_installation_checklist_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_installation_checklist_items_installation_jobs_installation~",
                        column: x => x.installation_job_id,
                        principalSchema: "service",
                        principalTable: "installation_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_installation_checklist_items_users_done_by_user_id",
                        column: x => x.done_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "installation_defects",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    installation_job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    no = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    severity = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reported_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reported_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    resolved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    resolution_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    verified_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    reopen_count = table.Column<int>(type: "integer", nullable: false),
                    last_reopen_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_installation_defects", x => x.id);
                    table.CheckConstraint("ck_installation_defects_severity", "severity IN ('minor', 'major', 'critical')");
                    table.CheckConstraint("ck_installation_defects_status", "status IN ('open', 'resolved', 'verified', 'reopened')");
                    table.CheckConstraint("ck_installation_defects_verifier", "verified_by_user_id IS NULL OR verified_by_user_id <> resolved_by_user_id");
                    table.ForeignKey(
                        name: "FK_installation_defects_installation_jobs_installation_job_id",
                        column: x => x.installation_job_id,
                        principalSchema: "service",
                        principalTable: "installation_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_installation_defects_users_reported_by_user_id",
                        column: x => x.reported_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_installation_defects_users_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_installation_defects_users_verified_by_user_id",
                        column: x => x.verified_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warranties",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    installation_job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    months = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warranties", x => x.id);
                    table.CheckConstraint("ck_warranties_term", "months BETWEEN 1 AND 120 AND end_date >= start_date");
                    table.ForeignKey(
                        name: "FK_warranties_installation_jobs_installation_job_id",
                        column: x => x.installation_job_id,
                        principalSchema: "service",
                        principalTable: "installation_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warranties_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_requests",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warranty_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    priority = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    in_warranty = table.Column<bool>(type: "boolean", nullable: false),
                    scheduled_date = table.Column<DateOnly>(type: "date", nullable: true),
                    resolution_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reopen_count = table.Column<int>(type: "integer", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_requests", x => x.id);
                    table.CheckConstraint("ck_service_requests_priority", "priority IN ('low', 'normal', 'high', 'urgent')");
                    table.CheckConstraint("ck_service_requests_status", "status IN ('open', 'scheduled', 'in_progress', 'resolved', 'closed')");
                    table.CheckConstraint("ck_service_requests_warranty", "in_warranty = (warranty_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_service_requests_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_requests_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_requests_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_requests_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_requests_warranties_warranty_id",
                        column: x => x.warranty_id,
                        principalSchema: "service",
                        principalTable: "warranties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_request_events",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    to_status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_request_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_request_events_service_requests_service_request_id",
                        column: x => x.service_request_id,
                        principalSchema: "service",
                        principalTable: "service_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_request_events_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_installation_checklist_items_done_by_user_id",
                schema: "service",
                table: "installation_checklist_items",
                column: "done_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_installation_checklist_items_installation_job_id_sort_order",
                schema: "service",
                table: "installation_checklist_items",
                columns: new[] { "installation_job_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_installation_defects_installation_job_id_no",
                schema: "service",
                table: "installation_defects",
                columns: new[] { "installation_job_id", "no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_installation_defects_reported_by_user_id",
                schema: "service",
                table: "installation_defects",
                column: "reported_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_installation_defects_resolved_by_user_id",
                schema: "service",
                table: "installation_defects",
                column: "resolved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_installation_defects_verified_by_user_id",
                schema: "service",
                table: "installation_defects",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_installation_jobs_branch_id",
                schema: "service",
                table: "installation_jobs",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_installation_jobs_created_by_user_id",
                schema: "service",
                table: "installation_jobs",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_installation_jobs_organization_id_number",
                schema: "service",
                table: "installation_jobs",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_installation_jobs_organization_id_project_id_status",
                schema: "service",
                table: "installation_jobs",
                columns: new[] { "organization_id", "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_installation_jobs_project_id",
                schema: "service",
                table: "installation_jobs",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_events_actor_user_id",
                schema: "service",
                table: "service_request_events",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_events_service_request_id_occurred_at_utc",
                schema: "service",
                table: "service_request_events",
                columns: new[] { "service_request_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_branch_id",
                schema: "service",
                table: "service_requests",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_created_by_user_id",
                schema: "service",
                table: "service_requests",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_organization_id_number",
                schema: "service",
                table: "service_requests",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_organization_id_project_id",
                schema: "service",
                table: "service_requests",
                columns: new[] { "organization_id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_organization_id_status_created_at_utc",
                schema: "service",
                table: "service_requests",
                columns: new[] { "organization_id", "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_project_id",
                schema: "service",
                table: "service_requests",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_warranty_id",
                schema: "service",
                table: "service_requests",
                column: "warranty_id");

            migrationBuilder.CreateIndex(
                name: "IX_warranties_installation_job_id",
                schema: "service",
                table: "warranties",
                column: "installation_job_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warranties_organization_id_number",
                schema: "service",
                table: "warranties",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warranties_organization_id_project_id_end_date",
                schema: "service",
                table: "warranties",
                columns: new[] { "organization_id", "project_id", "end_date" });

            migrationBuilder.CreateIndex(
                name: "IX_warranties_project_id",
                schema: "service",
                table: "warranties",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "installation_checklist_items",
                schema: "service");

            migrationBuilder.DropTable(
                name: "installation_defects",
                schema: "service");

            migrationBuilder.DropTable(
                name: "service_request_events",
                schema: "service");

            migrationBuilder.DropTable(
                name: "service_requests",
                schema: "service");

            migrationBuilder.DropTable(
                name: "warranties",
                schema: "service");

            migrationBuilder.DropTable(
                name: "installation_jobs",
                schema: "service");
        }
    }
}
