using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectWbsCostLedgerAndTimesheets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "wbs_node_id",
                schema: "projects",
                table: "project_budget_lines",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "labor_rates",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worker_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_labor_rates", x => x.id);
                    table.CheckConstraint("ck_labor_rates_hourly_rate", "hourly_rate > 0");
                    table.ForeignKey(
                        name: "FK_labor_rates_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_rates_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_rates_users_worker_user_id",
                        column: x => x.worker_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_wbs_nodes",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_wbs_nodes", x => x.id);
                    table.CheckConstraint("ck_project_wbs_nodes_level", "level BETWEEN 1 AND 3");
                    table.ForeignKey(
                        name: "FK_project_wbs_nodes_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_wbs_nodes_project_wbs_nodes_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "projects",
                        principalTable: "project_wbs_nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_wbs_nodes_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labor_timesheet_entries",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wbs_node_id = table.Column<Guid>(type: "uuid", nullable: true),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    worker_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_date = table.Column<DateOnly>(type: "date", nullable: false),
                    hours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    cost_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    task_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_overtime_warning = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_labor_timesheet_entries", x => x.id);
                    table.CheckConstraint("ck_labor_timesheet_entries_decider", "decided_by_user_id IS NULL OR (decided_by_user_id <> created_by_user_id AND decided_by_user_id <> worker_user_id)");
                    table.CheckConstraint("ck_labor_timesheet_entries_hours", "hours >= 0.25 AND hours <= 24");
                    table.CheckConstraint("ck_labor_timesheet_entries_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_project_wbs_nodes_wbs_node_id",
                        column: x => x.wbs_node_id,
                        principalSchema: "projects",
                        principalTable: "project_wbs_nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_users_worker_user_id",
                        column: x => x.worker_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_labor_timesheet_entries_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalSchema: "production",
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_cost_ledger_entries",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wbs_node_id = table.Column<Guid>(type: "uuid", nullable: true),
                    category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    entry_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    source_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    is_reversal = table.Column<bool>(type: "boolean", nullable: false),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_cost_ledger_entries", x => x.id);
                    table.CheckConstraint("ck_project_cost_ledger_entries_amount", "amount <> 0");
                    table.CheckConstraint("ck_project_cost_ledger_entries_category", "category IN ('material', 'labor', 'subcontract', 'service', 'other')");
                    table.CheckConstraint("ck_project_cost_ledger_entries_entry_type", "entry_type IN ('commitment', 'actual')");
                    table.ForeignKey(
                        name: "FK_project_cost_ledger_entries_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_cost_ledger_entries_project_wbs_nodes_wbs_node_id",
                        column: x => x.wbs_node_id,
                        principalSchema: "projects",
                        principalTable: "project_wbs_nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_cost_ledger_entries_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_cost_ledger_entries_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_project_budget_lines_wbs_node_id",
                schema: "projects",
                table: "project_budget_lines",
                column: "wbs_node_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_rates_organization_id_worker_user_id",
                schema: "projects",
                table: "labor_rates",
                columns: new[] { "organization_id", "worker_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_labor_rates_updated_by_user_id",
                schema: "projects",
                table: "labor_rates",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_rates_worker_user_id",
                schema: "projects",
                table: "labor_rates",
                column: "worker_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_branch_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_created_by_user_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_decided_by_user_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_organization_id_number",
                schema: "projects",
                table: "labor_timesheet_entries",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_organization_id_project_id_status",
                schema: "projects",
                table: "labor_timesheet_entries",
                columns: new[] { "organization_id", "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_organization_id_worker_user_id_work~",
                schema: "projects",
                table: "labor_timesheet_entries",
                columns: new[] { "organization_id", "worker_user_id", "work_date" });

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_project_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_wbs_node_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "wbs_node_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_work_order_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_labor_timesheet_entries_worker_user_id",
                schema: "projects",
                table: "labor_timesheet_entries",
                column: "worker_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_cost_ledger_entries_actor_user_id",
                schema: "projects",
                table: "project_cost_ledger_entries",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_cost_ledger_entries_organization_id_project_id_occu~",
                schema: "projects",
                table: "project_cost_ledger_entries",
                columns: new[] { "organization_id", "project_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_project_cost_ledger_entries_organization_id_source_type_sou~",
                schema: "projects",
                table: "project_cost_ledger_entries",
                columns: new[] { "organization_id", "source_type", "source_id" });

            migrationBuilder.CreateIndex(
                name: "IX_project_cost_ledger_entries_project_id",
                schema: "projects",
                table: "project_cost_ledger_entries",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_cost_ledger_entries_wbs_node_id",
                schema: "projects",
                table: "project_cost_ledger_entries",
                column: "wbs_node_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_wbs_nodes_organization_id_project_id_code",
                schema: "projects",
                table: "project_wbs_nodes",
                columns: new[] { "organization_id", "project_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_wbs_nodes_organization_id_project_id_sort_order",
                schema: "projects",
                table: "project_wbs_nodes",
                columns: new[] { "organization_id", "project_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_project_wbs_nodes_parent_id",
                schema: "projects",
                table: "project_wbs_nodes",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_wbs_nodes_project_id",
                schema: "projects",
                table: "project_wbs_nodes",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "FK_project_budget_lines_project_wbs_nodes_wbs_node_id",
                schema: "projects",
                table: "project_budget_lines",
                column: "wbs_node_id",
                principalSchema: "projects",
                principalTable: "project_wbs_nodes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_budget_lines_project_wbs_nodes_wbs_node_id",
                schema: "projects",
                table: "project_budget_lines");

            migrationBuilder.DropTable(
                name: "labor_rates",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "labor_timesheet_entries",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "project_cost_ledger_entries",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "project_wbs_nodes",
                schema: "projects");

            migrationBuilder.DropIndex(
                name: "IX_project_budget_lines_wbs_node_id",
                schema: "projects",
                table: "project_budget_lines");

            migrationBuilder.DropColumn(
                name: "wbs_node_id",
                schema: "projects",
                table: "project_budget_lines");
        }
    }
}
