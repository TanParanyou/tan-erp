using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMrpCompletionG14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_planning_parameters",
                schema: "mrp",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    safety_stock = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    min_order_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    lot_multiple = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    lead_time_days = table.Column<int>(type: "integer", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_planning_parameters", x => x.id);
                    table.CheckConstraint("ck_item_planning_parameters_values", "safety_stock >= 0 AND min_order_quantity >= 0 AND lot_multiple >= 0 AND (lead_time_days IS NULL OR (lead_time_days >= 0 AND lead_time_days <= 365))");
                    table.ForeignKey(
                        name: "FK_item_planning_parameters_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_planning_parameters_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_planning_parameters_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_planning_parameters_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "working_calendar_holidays",
                schema: "mrp",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    holiday_date = table.Column<DateOnly>(type: "date", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_working_calendar_holidays", x => x.id);
                    table.ForeignKey(
                        name: "FK_working_calendar_holidays_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_working_calendar_holidays_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_working_calendar_holidays_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_planning_parameters_item_id",
                schema: "mrp",
                table: "item_planning_parameters",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_planning_parameters_updated_by_user_id",
                schema: "mrp",
                table: "item_planning_parameters",
                column: "updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_planning_parameters_warehouse_id",
                schema: "mrp",
                table: "item_planning_parameters",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ux_mrp_item_planning_parameters_org_item_default",
                schema: "mrp",
                table: "item_planning_parameters",
                columns: new[] { "organization_id", "item_id" },
                unique: true,
                filter: "warehouse_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_mrp_item_planning_parameters_org_item_warehouse",
                schema: "mrp",
                table: "item_planning_parameters",
                columns: new[] { "organization_id", "item_id", "warehouse_id" },
                unique: true,
                filter: "warehouse_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_working_calendar_holidays_branch_id",
                schema: "mrp",
                table: "working_calendar_holidays",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_working_calendar_holidays_created_by_user_id",
                schema: "mrp",
                table: "working_calendar_holidays",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_mrp_working_calendar_holidays_org_branch_date",
                schema: "mrp",
                table: "working_calendar_holidays",
                columns: new[] { "organization_id", "branch_id", "holiday_date" },
                unique: true,
                filter: "branch_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_mrp_working_calendar_holidays_org_date",
                schema: "mrp",
                table: "working_calendar_holidays",
                columns: new[] { "organization_id", "holiday_date" },
                unique: true,
                filter: "branch_id IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_planning_parameters",
                schema: "mrp");

            migrationBuilder.DropTable(
                name: "working_calendar_holidays",
                schema: "mrp");
        }
    }
}
