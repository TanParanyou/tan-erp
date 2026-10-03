using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "activated_at_utc",
                schema: "projects",
                table: "projects",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "baseline_budget_hash",
                schema: "projects",
                table: "projects",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "baseline_budget_total",
                schema: "projects",
                table: "projects",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "budget_frozen_at_utc",
                schema: "projects",
                table: "projects",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "completed_at_utc",
                schema: "projects",
                table: "projects",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "planned_end_date",
                schema: "projects",
                table: "projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_reason",
                schema: "projects",
                table: "projects",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "project_budget_lines",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_budget_lines", x => x.id);
                    table.CheckConstraint("ck_project_budget_lines_amount", "amount >= 0");
                    table.CheckConstraint("ck_project_budget_lines_category", "category IN ('material', 'labor', 'subcontract', 'service', 'other')");
                    table.ForeignKey(
                        name: "FK_project_budget_lines_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_budget_lines_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_change_orders",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    budget_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    contract_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    submitted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_change_orders", x => x.id);
                    table.CheckConstraint("ck_project_change_orders_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
                    table.CheckConstraint("ck_project_change_orders_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_project_change_orders_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_change_orders_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_change_orders_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_change_orders_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_milestones",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    planned_date = table.Column<DateOnly>(type: "date", nullable: true),
                    weight = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    completed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    completed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_milestones", x => x.id);
                    table.CheckConstraint("ck_project_milestones_weight", "weight BETWEEN 1 AND 1000");
                    table.ForeignKey(
                        name: "FK_project_milestones_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_milestones_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_milestones_users_completed_by_user_id",
                        column: x => x.completed_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_status_history",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    to_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_status_history_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_status_history_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_status_history_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_projects_baseline_budget_total",
                schema: "projects",
                table: "projects",
                sql: "baseline_budget_total IS NULL OR baseline_budget_total >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_projects_planned_dates",
                schema: "projects",
                table: "projects",
                sql: "planned_end_date IS NULL OR planned_start_date IS NULL OR planned_end_date >= planned_start_date");

            migrationBuilder.CreateIndex(
                name: "IX_project_budget_lines_organization_id_project_id_sort_order",
                schema: "projects",
                table: "project_budget_lines",
                columns: new[] { "organization_id", "project_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_project_budget_lines_project_id",
                schema: "projects",
                table: "project_budget_lines",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_change_orders_created_by_user_id",
                schema: "projects",
                table: "project_change_orders",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_change_orders_decided_by_user_id",
                schema: "projects",
                table: "project_change_orders",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_change_orders_organization_id_number",
                schema: "projects",
                table: "project_change_orders",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_change_orders_organization_id_project_id_status",
                schema: "projects",
                table: "project_change_orders",
                columns: new[] { "organization_id", "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_project_change_orders_project_id",
                schema: "projects",
                table: "project_change_orders",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_milestones_completed_by_user_id",
                schema: "projects",
                table: "project_milestones",
                column: "completed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_milestones_organization_id_project_id_sort_order",
                schema: "projects",
                table: "project_milestones",
                columns: new[] { "organization_id", "project_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_project_milestones_project_id",
                schema: "projects",
                table: "project_milestones",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_status_history_actor_user_id",
                schema: "projects",
                table: "project_status_history",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_status_history_organization_id_project_id_occurred_~",
                schema: "projects",
                table: "project_status_history",
                columns: new[] { "organization_id", "project_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_project_status_history_project_id",
                schema: "projects",
                table: "project_status_history",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "project_budget_lines",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "project_change_orders",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "project_milestones",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "project_status_history",
                schema: "projects");

            migrationBuilder.DropCheckConstraint(
                name: "ck_projects_baseline_budget_total",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropCheckConstraint(
                name: "ck_projects_planned_dates",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "activated_at_utc",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "baseline_budget_hash",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "baseline_budget_total",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "budget_frozen_at_utc",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "completed_at_utc",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "planned_end_date",
                schema: "projects",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "status_reason",
                schema: "projects",
                table: "projects");
        }
    }
}
