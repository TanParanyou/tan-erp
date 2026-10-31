using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCompletionG18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_service_requests_status",
                schema: "service",
                table: "service_requests");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "service",
                table: "service_requests",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<Guid>(
                name: "billing_document_id",
                schema: "service",
                table: "service_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "billing_document_number",
                schema: "service",
                table: "service_requests",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "paused_at_utc",
                schema: "service",
                table: "service_requests",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "paused_seconds",
                schema: "service",
                table: "service_requests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "resolved_at_utc",
                schema: "service",
                table: "service_requests",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "service_charge_amount",
                schema: "service",
                table: "service_requests",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "service_charge_note",
                schema: "service",
                table: "service_requests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "to_status",
                schema: "service",
                table: "service_request_events",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "from_status",
                schema: "service",
                table: "service_request_events",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "technician_assignments",
                schema: "service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_date = table.Column<DateOnly>(type: "date", nullable: false),
                    assignment_kind = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    installation_job_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technician_assignments", x => x.id);
                    table.CheckConstraint("ck_technician_assignments_kind", "assignment_kind IN ('installation', 'service_request')");
                    table.CheckConstraint("ck_technician_assignments_target", "(assignment_kind = 'installation' AND installation_job_id IS NOT NULL AND service_request_id IS NULL) OR (assignment_kind = 'service_request' AND service_request_id IS NOT NULL AND installation_job_id IS NULL)");
                    table.ForeignKey(
                        name: "FK_technician_assignments_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_technician_assignments_installation_jobs_installation_job_id",
                        column: x => x.installation_job_id,
                        principalSchema: "service",
                        principalTable: "installation_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_technician_assignments_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_technician_assignments_service_requests_service_request_id",
                        column: x => x.service_request_id,
                        principalSchema: "service",
                        principalTable: "service_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_technician_assignments_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_technician_assignments_users_technician_user_id",
                        column: x => x.technician_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_service_requests_billing_document_id",
                schema: "service",
                table: "service_requests",
                column: "billing_document_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_service_requests_status",
                schema: "service",
                table: "service_requests",
                sql: "status IN ('open', 'scheduled', 'in_progress', 'waiting_customer', 'resolved', 'closed')");

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_branch_id",
                schema: "service",
                table: "technician_assignments",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_created_by_user_id",
                schema: "service",
                table: "technician_assignments",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_installation_job_id",
                schema: "service",
                table: "technician_assignments",
                column: "installation_job_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_organization_id_technician_user_id_w~",
                schema: "service",
                table: "technician_assignments",
                columns: new[] { "organization_id", "technician_user_id", "work_date" });

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_organization_id_work_date",
                schema: "service",
                table: "technician_assignments",
                columns: new[] { "organization_id", "work_date" });

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_service_request_id",
                schema: "service",
                table: "technician_assignments",
                column: "service_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_technician_assignments_technician_user_id",
                schema: "service",
                table: "technician_assignments",
                column: "technician_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_requests_billing_documents_billing_document_id",
                schema: "service",
                table: "service_requests",
                column: "billing_document_id",
                principalSchema: "finance",
                principalTable: "billing_documents",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_service_requests_billing_documents_billing_document_id",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropTable(
                name: "technician_assignments",
                schema: "service");

            migrationBuilder.DropIndex(
                name: "IX_service_requests_billing_document_id",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropCheckConstraint(
                name: "ck_service_requests_status",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "billing_document_id",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "billing_document_number",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "paused_at_utc",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "paused_seconds",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "resolved_at_utc",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "service_charge_amount",
                schema: "service",
                table: "service_requests");

            migrationBuilder.DropColumn(
                name: "service_charge_note",
                schema: "service",
                table: "service_requests");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "service",
                table: "service_requests",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<string>(
                name: "to_status",
                schema: "service",
                table: "service_request_events",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<string>(
                name: "from_status",
                schema: "service",
                table: "service_request_events",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_service_requests_status",
                schema: "service",
                table: "service_requests",
                sql: "status IN ('open', 'scheduled', 'in_progress', 'resolved', 'closed')");
        }
    }
}
