using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.CreateTable(
                name: "warehouses",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.id);
                    table.CheckConstraint("ck_warehouses_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_warehouses_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warehouses_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warehouses_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_balances",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    on_hand = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    reserved = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_balances", x => x.id);
                    table.CheckConstraint("ck_stock_balances_on_hand", "on_hand >= 0");
                    table.CheckConstraint("ck_stock_balances_reserved", "reserved >= 0 AND reserved <= on_hand");
                    table.CheckConstraint("ck_stock_balances_total_value", "total_value >= 0");
                    table.ForeignKey(
                        name: "FK_stock_balances_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_balances_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_balances_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_documents",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    posted_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    posted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_documents", x => x.id);
                    table.CheckConstraint("ck_stock_documents_type", "document_type IN ('receipt', 'issue', 'transfer', 'adjustment')");
                    table.ForeignKey(
                        name: "FK_stock_documents_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_documents_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_documents_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_documents_users_posted_by_user_id",
                        column: x => x.posted_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_documents_warehouses_to_warehouse_id",
                        column: x => x.to_warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_documents_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_reservations",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_reservations", x => x.id);
                    table.CheckConstraint("ck_stock_reservations_quantity", "quantity >= 0");
                    table.CheckConstraint("ck_stock_reservations_status", "status IN ('active', 'released', 'consumed')");
                    table.ForeignKey(
                        name: "FK_stock_reservations_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_reservations_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_reservations_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_reservations_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_movements",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    quantity_delta = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    value_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    on_hand_after = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    posted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movements", x => x.id);
                    table.CheckConstraint("ck_stock_movements_kind", "kind IN ('receipt', 'issue', 'transfer_out', 'transfer_in', 'adjustment_in', 'adjustment_out')");
                    table.CheckConstraint("ck_stock_movements_on_hand_after", "on_hand_after >= 0");
                    table.CheckConstraint("ck_stock_movements_quantity", "quantity_delta <> 0");
                    table.ForeignKey(
                        name: "FK_stock_movements_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_movements_stock_documents_stock_document_id",
                        column: x => x.stock_document_id,
                        principalSchema: "inventory",
                        principalTable: "stock_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_movements_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_movements_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_balances_item_id",
                schema: "inventory",
                table: "stock_balances",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_balances_organization_id_item_id",
                schema: "inventory",
                table: "stock_balances",
                columns: new[] { "organization_id", "item_id" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_balances_organization_id_warehouse_id_item_id",
                schema: "inventory",
                table: "stock_balances",
                columns: new[] { "organization_id", "warehouse_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_balances_unit_id",
                schema: "inventory",
                table: "stock_balances",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_balances_warehouse_id",
                schema: "inventory",
                table: "stock_balances",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_branch_id",
                schema: "inventory",
                table: "stock_documents",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_organization_id_number",
                schema: "inventory",
                table: "stock_documents",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_organization_id_source_type_source_id",
                schema: "inventory",
                table: "stock_documents",
                columns: new[] { "organization_id", "source_type", "source_id" },
                unique: true,
                filter: "source_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_organization_id_warehouse_id_posted_at_utc",
                schema: "inventory",
                table: "stock_documents",
                columns: new[] { "organization_id", "warehouse_id", "posted_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_posted_by_user_id",
                schema: "inventory",
                table: "stock_documents",
                column: "posted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_project_id",
                schema: "inventory",
                table: "stock_documents",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_to_warehouse_id",
                schema: "inventory",
                table: "stock_documents",
                column: "to_warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_documents_warehouse_id",
                schema: "inventory",
                table: "stock_documents",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_item_id",
                schema: "inventory",
                table: "stock_movements",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_organization_id_warehouse_id_item_id_posted~",
                schema: "inventory",
                table: "stock_movements",
                columns: new[] { "organization_id", "warehouse_id", "item_id", "posted_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_stock_document_id",
                schema: "inventory",
                table: "stock_movements",
                column: "stock_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_unit_id",
                schema: "inventory",
                table: "stock_movements",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_warehouse_id",
                schema: "inventory",
                table: "stock_movements",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_reservations_created_by_user_id",
                schema: "inventory",
                table: "stock_reservations",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_reservations_item_id",
                schema: "inventory",
                table: "stock_reservations",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_reservations_organization_id_project_id_status",
                schema: "inventory",
                table: "stock_reservations",
                columns: new[] { "organization_id", "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_reservations_organization_id_warehouse_id_item_id_sta~",
                schema: "inventory",
                table: "stock_reservations",
                columns: new[] { "organization_id", "warehouse_id", "item_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_reservations_project_id",
                schema: "inventory",
                table: "stock_reservations",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_reservations_warehouse_id",
                schema: "inventory",
                table: "stock_reservations",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_branch_id",
                schema: "inventory",
                table: "warehouses",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_created_by_user_id",
                schema: "inventory",
                table: "warehouses",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_organization_id_branch_id_status",
                schema: "inventory",
                table: "warehouses",
                columns: new[] { "organization_id", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_organization_id_normalized_code",
                schema: "inventory",
                table: "warehouses",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_balances",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "stock_movements",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "stock_reservations",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "stock_documents",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "warehouses",
                schema: "inventory");
        }
    }
}
