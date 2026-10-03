using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropCheckConstraint(
                name: "ck_stock_documents_type",
                schema: "inventory",
                table: "stock_documents");

            migrationBuilder.EnsureSchema(
                name: "production");

            migrationBuilder.CreateTable(
                name: "boms",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boms", x => x.id);
                    table.ForeignKey(
                        name: "FK_boms_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_boms_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_boms_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bom_revisions",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bom_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision_no = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    output_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    approved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bom_revisions", x => x.id);
                    table.CheckConstraint("ck_bom_revisions_approver", "approved_by_user_id IS NULL OR approved_by_user_id <> created_by_user_id");
                    table.CheckConstraint("ck_bom_revisions_output_quantity", "output_quantity > 0");
                    table.CheckConstraint("ck_bom_revisions_status", "status IN ('draft', 'approved', 'obsolete')");
                    table.ForeignKey(
                        name: "FK_bom_revisions_boms_bom_id",
                        column: x => x.bom_id,
                        principalSchema: "production",
                        principalTable: "boms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bom_revisions_users_approved_by_user_id",
                        column: x => x.approved_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bom_revisions_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bom_lines",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bom_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    component_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    scrap_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bom_lines", x => x.id);
                    table.CheckConstraint("ck_bom_lines_quantity", "quantity > 0");
                    table.CheckConstraint("ck_bom_lines_scrap_percent", "scrap_percent >= 0 AND scrap_percent <= 50");
                    table.ForeignKey(
                        name: "FK_bom_lines_bom_revisions_bom_revision_id",
                        column: x => x.bom_revision_id,
                        principalSchema: "production",
                        principalTable: "bom_revisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bom_lines_items_component_item_id",
                        column: x => x.component_item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_orders",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bom_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    planned_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    completed_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    cost_allocated = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cancel_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_orders", x => x.id);
                    table.CheckConstraint("ck_work_orders_cost_allocated", "cost_allocated >= 0");
                    table.CheckConstraint("ck_work_orders_quantities", "planned_quantity > 0 AND completed_quantity >= 0 AND completed_quantity <= planned_quantity");
                    table.CheckConstraint("ck_work_orders_status", "status IN ('draft', 'released', 'in_progress', 'completed', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_work_orders_bom_revisions_bom_revision_id",
                        column: x => x.bom_revision_id,
                        principalSchema: "production",
                        principalTable: "bom_revisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inventory",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_materials",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    required_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    issued_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    returned_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    issued_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    returned_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_materials", x => x.id);
                    table.CheckConstraint("ck_work_order_materials_quantities", "required_quantity > 0 AND issued_quantity >= 0 AND returned_quantity >= 0 AND returned_quantity <= issued_quantity");
                    table.CheckConstraint("ck_work_order_materials_values", "issued_value >= 0 AND returned_value >= 0");
                    table.ForeignKey(
                        name: "FK_work_order_materials_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_order_materials_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalSchema: "production",
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_order_transactions",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    stock_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_document_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    actor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_transactions", x => x.id);
                    table.CheckConstraint("ck_work_order_transactions_kind", "kind IN ('issue', 'return', 'completion')");
                    table.ForeignKey(
                        name: "FK_work_order_transactions_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_order_transactions_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalSchema: "production",
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements",
                sql: "kind IN ('receipt', 'issue', 'transfer_out', 'transfer_in', 'adjustment_in', 'adjustment_out', 'return_in')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_stock_documents_type",
                schema: "inventory",
                table: "stock_documents",
                sql: "document_type IN ('receipt', 'issue', 'transfer', 'adjustment', 'return')");

            migrationBuilder.CreateIndex(
                name: "IX_bom_lines_bom_revision_id_component_item_id",
                schema: "production",
                table: "bom_lines",
                columns: new[] { "bom_revision_id", "component_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bom_lines_component_item_id",
                schema: "production",
                table: "bom_lines",
                column: "component_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_bom_revisions_approved_by_user_id",
                schema: "production",
                table: "bom_revisions",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_bom_revisions_bom_id_revision_no",
                schema: "production",
                table: "bom_revisions",
                columns: new[] { "bom_id", "revision_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bom_revisions_created_by_user_id",
                schema: "production",
                table: "bom_revisions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_bom_revisions_one_approved",
                schema: "production",
                table: "bom_revisions",
                column: "bom_id",
                unique: true,
                filter: "status = 'approved'");

            migrationBuilder.CreateIndex(
                name: "IX_boms_created_by_user_id",
                schema: "production",
                table: "boms",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_boms_item_id",
                schema: "production",
                table: "boms",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_boms_organization_id_item_id",
                schema: "production",
                table: "boms",
                columns: new[] { "organization_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_boms_organization_id_normalized_code",
                schema: "production",
                table: "boms",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_order_materials_item_id",
                schema: "production",
                table: "work_order_materials",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_materials_work_order_id_item_id",
                schema: "production",
                table: "work_order_materials",
                columns: new[] { "work_order_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_order_transactions_actor_user_id",
                schema: "production",
                table: "work_order_transactions",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_transactions_work_order_id_created_at_utc",
                schema: "production",
                table: "work_order_transactions",
                columns: new[] { "work_order_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_bom_revision_id",
                schema: "production",
                table: "work_orders",
                column: "bom_revision_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_branch_id",
                schema: "production",
                table: "work_orders",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_created_by_user_id",
                schema: "production",
                table: "work_orders",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_item_id",
                schema: "production",
                table: "work_orders",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_organization_id_number",
                schema: "production",
                table: "work_orders",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_organization_id_project_id",
                schema: "production",
                table: "work_orders",
                columns: new[] { "organization_id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_organization_id_status_created_at_utc",
                schema: "production",
                table: "work_orders",
                columns: new[] { "organization_id", "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_project_id",
                schema: "production",
                table: "work_orders",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_warehouse_id",
                schema: "production",
                table: "work_orders",
                column: "warehouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bom_lines",
                schema: "production");

            migrationBuilder.DropTable(
                name: "work_order_materials",
                schema: "production");

            migrationBuilder.DropTable(
                name: "work_order_transactions",
                schema: "production");

            migrationBuilder.DropTable(
                name: "work_orders",
                schema: "production");

            migrationBuilder.DropTable(
                name: "bom_revisions",
                schema: "production");

            migrationBuilder.DropTable(
                name: "boms",
                schema: "production");

            migrationBuilder.DropCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropCheckConstraint(
                name: "ck_stock_documents_type",
                schema: "inventory",
                table: "stock_documents");

            migrationBuilder.AddCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements",
                sql: "kind IN ('receipt', 'issue', 'transfer_out', 'transfer_in', 'adjustment_in', 'adjustment_out')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_stock_documents_type",
                schema: "inventory",
                table: "stock_documents",
                sql: "document_type IN ('receipt', 'issue', 'transfer', 'adjustment')");
        }
    }
}
