using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "approval_snapshot_json",
                schema: "estimates",
                table: "estimate_revisions",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "approved_at_utc",
                schema: "estimates",
                table: "estimate_revisions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "approved_by_user_id",
                schema: "estimates",
                table: "estimate_revisions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "last_financial_editor_user_id",
                schema: "estimates",
                table: "estimate_revisions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "submitted_at_utc",
                schema: "estimates",
                table: "estimate_revisions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "submitted_by_user_id",
                schema: "estimates",
                table: "estimate_revisions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "estimate_approval_requests",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision_no = table.Column<int>(type: "integer", nullable: false),
                    calculation_version = table.Column<int>(type: "integer", nullable: false),
                    calculation_input_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    calculation_snapshot_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    policy_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    policy_version = table.Column<int>(type: "integer", nullable: false),
                    route_snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    route_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    submission_note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    closed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_approval_requests", x => x.id);
                    table.UniqueConstraint("AK_estimate_approval_requests_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_approval_requests_status", "status IN ('open', 'approved', 'returned', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_estimate_approval_requests_estimate_revisions_estimate_revi~",
                        columns: x => new { x.estimate_revision_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimate_approval_requests_estimates_estimate_id_organizati~",
                        columns: x => new { x.estimate_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimates",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estimate_approval_snapshots",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_approval_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calculation_version = table.Column<int>(type: "integer", nullable: false),
                    calculation_input_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    calculation_snapshot_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    route_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    approved_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_approval_snapshots", x => x.id);
                    table.UniqueConstraint("AK_estimate_approval_snapshots_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_approval_snapshots_calculation_version", "calculation_version > 0");
                    table.ForeignKey(
                        name: "FK_estimate_approval_snapshots_estimate_approval_requests_esti~",
                        columns: x => new { x.estimate_approval_request_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_approval_requests",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimate_approval_snapshots_estimate_revisions_estimate_rev~",
                        columns: x => new { x.estimate_revision_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimate_approval_snapshots_estimates_estimate_id_organizat~",
                        columns: x => new { x.estimate_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimates",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estimate_approval_steps",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_approval_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    reviewer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    scope_type = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    scope_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_approval_steps", x => x.id);
                    table.UniqueConstraint("AK_estimate_approval_steps_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_approval_steps_scope", "scope_type IN ('organization', 'branch')");
                    table.CheckConstraint("ck_estimate_approval_steps_status", "status IN ('pending', 'approved', 'returned')");
                    table.ForeignKey(
                        name: "FK_estimate_approval_steps_estimate_approval_requests_estimate~",
                        columns: x => new { x.estimate_approval_request_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_approval_requests",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimate_approval_steps_memberships_reviewer_membership_id_~",
                        columns: x => new { x.reviewer_membership_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "memberships",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estimate_approval_decisions",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_approval_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_approval_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    reason_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    calculation_snapshot_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    route_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_approval_decisions", x => x.id);
                    table.UniqueConstraint("AK_estimate_approval_decisions_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_approval_decisions_decision", "decision IN ('approved', 'returned')");
                    table.ForeignKey(
                        name: "FK_estimate_approval_decisions_estimate_approval_requests_esti~",
                        columns: x => new { x.estimate_approval_request_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_approval_requests",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimate_approval_decisions_estimate_approval_steps_estimat~",
                        columns: x => new { x.estimate_approval_step_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_approval_steps",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_decisions_estimate_approval_request_id_or~",
                schema: "estimates",
                table: "estimate_approval_decisions",
                columns: new[] { "estimate_approval_request_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_decisions_estimate_approval_step_id_organ~",
                schema: "estimates",
                table: "estimate_approval_decisions",
                columns: new[] { "estimate_approval_step_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_decisions_organization_id_estimate_approv~",
                schema: "estimates",
                table: "estimate_approval_decisions",
                columns: new[] { "organization_id", "estimate_approval_step_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_requests_estimate_id_organization_id",
                schema: "estimates",
                table: "estimate_approval_requests",
                columns: new[] { "estimate_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_requests_estimate_revision_id_organizatio~",
                schema: "estimates",
                table: "estimate_approval_requests",
                columns: new[] { "estimate_revision_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_requests_organization_id_estimate_revisio~",
                schema: "estimates",
                table: "estimate_approval_requests",
                columns: new[] { "organization_id", "estimate_revision_id" },
                unique: true,
                filter: "status = 'open'");

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_snapshots_estimate_approval_request_id_or~",
                schema: "estimates",
                table: "estimate_approval_snapshots",
                columns: new[] { "estimate_approval_request_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_snapshots_estimate_id_organization_id",
                schema: "estimates",
                table: "estimate_approval_snapshots",
                columns: new[] { "estimate_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_snapshots_estimate_revision_id_organizati~",
                schema: "estimates",
                table: "estimate_approval_snapshots",
                columns: new[] { "estimate_revision_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_snapshots_organization_id_estimate_revisi~",
                schema: "estimates",
                table: "estimate_approval_snapshots",
                columns: new[] { "organization_id", "estimate_revision_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_steps_estimate_approval_request_id_organi~",
                schema: "estimates",
                table: "estimate_approval_steps",
                columns: new[] { "estimate_approval_request_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_steps_organization_id_estimate_approval_r~",
                schema: "estimates",
                table: "estimate_approval_steps",
                columns: new[] { "organization_id", "estimate_approval_request_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_approval_steps_reviewer_membership_id_organization~",
                schema: "estimates",
                table: "estimate_approval_steps",
                columns: new[] { "reviewer_membership_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estimate_approval_decisions",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimate_approval_snapshots",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimate_approval_steps",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimate_approval_requests",
                schema: "estimates");

            migrationBuilder.DropColumn(
                name: "approval_snapshot_json",
                schema: "estimates",
                table: "estimate_revisions");

            migrationBuilder.DropColumn(
                name: "approved_at_utc",
                schema: "estimates",
                table: "estimate_revisions");

            migrationBuilder.DropColumn(
                name: "approved_by_user_id",
                schema: "estimates",
                table: "estimate_revisions");

            migrationBuilder.DropColumn(
                name: "last_financial_editor_user_id",
                schema: "estimates",
                table: "estimate_revisions");

            migrationBuilder.DropColumn(
                name: "submitted_at_utc",
                schema: "estimates",
                table: "estimate_revisions");

            migrationBuilder.DropColumn(
                name: "submitted_by_user_id",
                schema: "estimates",
                table: "estimate_revisions");

        }
    }
}
