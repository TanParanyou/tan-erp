using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionCompletionG13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_work_orders_cost_allocated",
                schema: "production",
                table: "work_orders");

            migrationBuilder.AddColumn<decimal>(
                name: "labor_cost",
                schema: "production",
                table: "work_orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "labor_hours",
                schema: "production",
                table: "work_orders",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "planned_end_date",
                schema: "production",
                table: "work_orders",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "planned_start_date",
                schema: "production",
                table: "work_orders",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql(
                "UPDATE production.work_orders SET planned_start_date = (created_at_utc AT TIME ZONE 'UTC')::date, planned_end_date = (created_at_utc AT TIME ZONE 'UTC')::date WHERE planned_start_date = DATE '0001-01-01';");

            migrationBuilder.AddColumn<decimal>(
                name: "subcontract_cost",
                schema: "production",
                table: "work_orders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "work_centers",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    daily_capacity_hours = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_centers", x => x.id);
                    table.CheckConstraint("ck_work_centers_capacity", "daily_capacity_hours > 0 AND daily_capacity_hours <= 24");
                    table.CheckConstraint("ck_work_centers_hourly_rate", "hourly_rate >= 0");
                    table.CheckConstraint("ck_work_centers_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_work_centers_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_centers_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_centers_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bom_routing_steps",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bom_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    operation_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    operation_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    work_center_id = table.Column<Guid>(type: "uuid", nullable: true),
                    setup_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    run_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    is_subcontracted = table.Column<bool>(type: "boolean", nullable: false),
                    std_subcontract_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bom_routing_steps", x => x.id);
                    table.CheckConstraint("ck_bom_routing_steps_values", "sequence > 0 AND setup_hours >= 0 AND run_hours >= 0 AND std_subcontract_cost >= 0");
                    table.ForeignKey(
                        name: "FK_bom_routing_steps_bom_revisions_bom_revision_id",
                        column: x => x.bom_revision_id,
                        principalSchema: "production",
                        principalTable: "bom_revisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bom_routing_steps_work_centers_work_center_id",
                        column: x => x.work_center_id,
                        principalSchema: "production",
                        principalTable: "work_centers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_operations",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    operation_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    operation_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    work_center_id = table.Column<Guid>(type: "uuid", nullable: true),
                    planned_setup_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    planned_run_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    actual_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    is_subcontracted = table.Column<bool>(type: "boolean", nullable: false),
                    subcontract_purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subcontract_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    completed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_operations", x => x.id);
                    table.CheckConstraint("ck_work_order_operations_values", "sequence > 0 AND planned_setup_hours >= 0 AND planned_run_hours >= 0 AND actual_hours >= 0 AND subcontract_cost >= 0 AND status IN ('pending', 'completed')");
                    table.ForeignKey(
                        name: "FK_work_order_operations_purchase_orders_subcontract_purchase_~",
                        column: x => x.subcontract_purchase_order_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_order_operations_work_centers_work_center_id",
                        column: x => x.work_center_id,
                        principalSchema: "production",
                        principalTable: "work_centers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_order_operations_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalSchema: "production",
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_work_orders_cost_allocated",
                schema: "production",
                table: "work_orders",
                sql: "cost_allocated >= 0 AND labor_hours >= 0 AND labor_cost >= 0 AND subcontract_cost >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_work_orders_dates",
                schema: "production",
                table: "work_orders",
                sql: "planned_end_date >= planned_start_date");

            migrationBuilder.CreateIndex(
                name: "IX_bom_routing_steps_bom_revision_id_sequence",
                schema: "production",
                table: "bom_routing_steps",
                columns: new[] { "bom_revision_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bom_routing_steps_work_center_id",
                schema: "production",
                table: "bom_routing_steps",
                column: "work_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_centers_branch_id",
                schema: "production",
                table: "work_centers",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_centers_created_by_user_id",
                schema: "production",
                table: "work_centers",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_centers_organization_id_branch_id_status",
                schema: "production",
                table: "work_centers",
                columns: new[] { "organization_id", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_work_centers_organization_id_normalized_code",
                schema: "production",
                table: "work_centers",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_order_operations_subcontract_purchase_order_id",
                schema: "production",
                table: "work_order_operations",
                column: "subcontract_purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_operations_work_center_id",
                schema: "production",
                table: "work_order_operations",
                column: "work_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_operations_work_order_id_sequence",
                schema: "production",
                table: "work_order_operations",
                columns: new[] { "work_order_id", "sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bom_routing_steps",
                schema: "production");

            migrationBuilder.DropTable(
                name: "work_order_operations",
                schema: "production");

            migrationBuilder.DropTable(
                name: "work_centers",
                schema: "production");

            migrationBuilder.DropCheckConstraint(
                name: "ck_work_orders_cost_allocated",
                schema: "production",
                table: "work_orders");

            migrationBuilder.DropCheckConstraint(
                name: "ck_work_orders_dates",
                schema: "production",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "labor_cost",
                schema: "production",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "labor_hours",
                schema: "production",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "planned_end_date",
                schema: "production",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "planned_start_date",
                schema: "production",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "subcontract_cost",
                schema: "production",
                table: "work_orders");

            migrationBuilder.AddCheckConstraint(
                name: "ck_work_orders_cost_allocated",
                schema: "production",
                table: "work_orders",
                sql: "cost_allocated >= 0");
        }
    }
}
