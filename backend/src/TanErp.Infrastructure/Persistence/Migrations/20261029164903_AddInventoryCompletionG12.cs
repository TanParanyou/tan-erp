using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryCompletionG12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_stock_documents_organization_id_source_type_source_id",
                schema: "inventory",
                table: "stock_documents");

            migrationBuilder.AddColumn<Guid>(
                name: "location_id",
                schema: "inventory",
                table: "stock_movements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lot_number",
                schema: "inventory",
                table: "stock_movements",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "serial_numbers_json",
                schema: "inventory",
                table: "stock_movements",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "track_lot",
                schema: "item_master",
                table: "items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "track_serial",
                schema: "item_master",
                table: "items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "inventory_period_closes",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_year = table.Column<int>(type: "integer", nullable: false),
                    period_month = table.Column<int>(type: "integer", nullable: false),
                    period_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    line_count = table.Column<int>(type: "integer", nullable: false),
                    total_on_hand_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_valuation_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    closed_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    closed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_period_closes", x => x.id);
                    table.CheckConstraint("ck_inventory_period_closes_month", "period_month >= 1 AND period_month <= 12");
                    table.ForeignKey(
                        name: "FK_inventory_period_closes_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventory_period_closes_users_closed_by_user_id",
                        column: x => x.closed_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_locations",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse_locations", x => x.id);
                    table.CheckConstraint("ck_warehouse_locations_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_warehouse_locations_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warehouse_locations_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inventory_period_close_lines",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_close_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    on_hand_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    average_cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_period_close_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_period_close_lines_inventory_period_closes_period~",
                        column: x => x.period_close_id,
                        principalSchema: "inventory",
                        principalTable: "inventory_period_closes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_period_close_lines_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventory_period_close_lines_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventory_period_close_lines_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_location_balances",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: ""),
                    on_hand = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_location_balances", x => x.id);
                    table.CheckConstraint("ck_stock_location_balances_on_hand", "on_hand >= 0");
                    table.ForeignKey(
                        name: "FK_stock_location_balances_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_location_balances_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_location_balances_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_location_balances_warehouse_locations_location_id",
                        column: x => x.location_id,
                        principalSchema: "inventory",
                        principalTable: "warehouse_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_location_balances_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_serials",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    normalized_serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lot_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: ""),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    last_stock_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_serials", x => x.id);
                    table.CheckConstraint("ck_stock_serials_status", "status IN ('in_stock', 'issued', 'returned_out')");
                    table.ForeignKey(
                        name: "FK_stock_serials_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_serials_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_serials_stock_documents_last_stock_document_id",
                        column: x => x.last_stock_document_id,
                        principalSchema: "inventory",
                        principalTable: "stock_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_serials_warehouse_locations_location_id",
                        column: x => x.location_id,
                        principalSchema: "inventory",
                        principalTable: "warehouse_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_serials_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_location_id",
                schema: "inventory",
                table: "stock_movements",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_organization_id_source_type_source_id",
                schema: "inventory",
                table: "stock_documents",
                columns: new[] { "organization_id", "source_type", "source_id" },
                unique: true,
                filter: "source_id IS NOT NULL AND source_type <> 'project_return'");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_close_lines_item_id",
                schema: "inventory",
                table: "inventory_period_close_lines",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_close_lines_period_close_id",
                schema: "inventory",
                table: "inventory_period_close_lines",
                column: "period_close_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_close_lines_unit_id",
                schema: "inventory",
                table: "inventory_period_close_lines",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_close_lines_warehouse_id",
                schema: "inventory",
                table: "inventory_period_close_lines",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_closes_closed_by_user_id",
                schema: "inventory",
                table: "inventory_period_closes",
                column: "closed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_closes_organization_id_period_end_date",
                schema: "inventory",
                table: "inventory_period_closes",
                columns: new[] { "organization_id", "period_end_date" });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_period_closes_organization_id_period_year_period_~",
                schema: "inventory",
                table: "inventory_period_closes",
                columns: new[] { "organization_id", "period_year", "period_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_location_balances_item_id",
                schema: "inventory",
                table: "stock_location_balances",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_location_balances_location_id",
                schema: "inventory",
                table: "stock_location_balances",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_location_balances_organization_id_warehouse_id_item_id",
                schema: "inventory",
                table: "stock_location_balances",
                columns: new[] { "organization_id", "warehouse_id", "item_id" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_location_balances_organization_id_warehouse_id_locati~",
                schema: "inventory",
                table: "stock_location_balances",
                columns: new[] { "organization_id", "warehouse_id", "location_id", "item_id", "lot_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_location_balances_unit_id",
                schema: "inventory",
                table: "stock_location_balances",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_location_balances_warehouse_id",
                schema: "inventory",
                table: "stock_location_balances",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_serials_item_id",
                schema: "inventory",
                table: "stock_serials",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_serials_last_stock_document_id",
                schema: "inventory",
                table: "stock_serials",
                column: "last_stock_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_serials_location_id",
                schema: "inventory",
                table: "stock_serials",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_serials_organization_id_item_id_normalized_serial_num~",
                schema: "inventory",
                table: "stock_serials",
                columns: new[] { "organization_id", "item_id", "normalized_serial_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_serials_organization_id_warehouse_id_item_id_status",
                schema: "inventory",
                table: "stock_serials",
                columns: new[] { "organization_id", "warehouse_id", "item_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_serials_warehouse_id",
                schema: "inventory",
                table: "stock_serials",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_locations_organization_id_warehouse_id_normalized~",
                schema: "inventory",
                table: "warehouse_locations",
                columns: new[] { "organization_id", "warehouse_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_locations_organization_id_warehouse_id_status",
                schema: "inventory",
                table: "warehouse_locations",
                columns: new[] { "organization_id", "warehouse_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_locations_warehouse_id",
                schema: "inventory",
                table: "warehouse_locations",
                column: "warehouse_id");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_movements_warehouse_locations_location_id",
                schema: "inventory",
                table: "stock_movements",
                column: "location_id",
                principalSchema: "inventory",
                principalTable: "warehouse_locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                INSERT INTO inventory.warehouse_locations (
                    id, organization_id, warehouse_id, code, normalized_code, name, is_default, status, row_version, created_at_utc, updated_at_utc
                )
                SELECT
                    gen_random_uuid(), w.organization_id, w.id, 'DEFAULT', 'DEFAULT', 'Default Location', TRUE, 'active', gen_random_uuid(), w.created_at_utc, w.updated_at_utc
                FROM inventory.warehouses w
                ON CONFLICT (organization_id, warehouse_id, normalized_code) DO NOTHING;

                INSERT INTO inventory.stock_location_balances (
                    id, organization_id, warehouse_id, location_id, item_id, unit_id, lot_number, on_hand, row_version, updated_at_utc
                )
                SELECT
                    gen_random_uuid(), b.organization_id, b.warehouse_id, wl.id, b.item_id, b.unit_id, '', b.on_hand, gen_random_uuid(), b.updated_at_utc
                FROM inventory.stock_balances b
                INNER JOIN inventory.warehouse_locations wl
                    ON wl.organization_id = b.organization_id AND wl.warehouse_id = b.warehouse_id AND wl.is_default = TRUE
                WHERE b.on_hand > 0
                ON CONFLICT (organization_id, warehouse_id, location_id, item_id, lot_number) DO NOTHING;

                UPDATE inventory.stock_movements m
                SET location_id = wl.id
                FROM inventory.warehouse_locations wl
                WHERE wl.organization_id = m.organization_id
                  AND wl.warehouse_id = m.warehouse_id
                  AND wl.is_default = TRUE
                  AND m.location_id IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_stock_movements_warehouse_locations_location_id",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropTable(
                name: "inventory_period_close_lines",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "stock_location_balances",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "stock_serials",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "inventory_period_closes",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "warehouse_locations",
                schema: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_stock_movements_location_id",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "IX_stock_documents_organization_id_source_type_source_id",
                schema: "inventory",
                table: "stock_documents");

            migrationBuilder.DropColumn(
                name: "location_id",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "lot_number",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "serial_numbers_json",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "track_lot",
                schema: "item_master",
                table: "items");

            migrationBuilder.DropColumn(
                name: "track_serial",
                schema: "item_master",
                table: "items");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_organization_id_source_type_source_id",
                schema: "inventory",
                table: "stock_documents",
                columns: new[] { "organization_id", "source_type", "source_id" },
                unique: true,
                filter: "source_id IS NOT NULL");
        }
    }
}
