using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierItemPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "supplier_item_prices",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    min_order_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    lead_time_days = table.Column<int>(type: "integer", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    is_preferred = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_item_prices", x => x.id);
                    table.CheckConstraint("ck_supplier_item_prices_amounts", "unit_price >= 0 AND min_order_quantity >= 0");
                    table.CheckConstraint("ck_supplier_item_prices_lead_time", "lead_time_days BETWEEN 0 AND 365");
                    table.CheckConstraint("ck_supplier_item_prices_period", "effective_to IS NULL OR effective_to >= effective_from");
                    table.ForeignKey(
                        name: "FK_supplier_item_prices_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_item_prices_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_item_prices_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalSchema: "procurement",
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_item_prices_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_item_prices_item_id",
                schema: "procurement",
                table: "supplier_item_prices",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_item_prices_organization_id_item_id_effective_from",
                schema: "procurement",
                table: "supplier_item_prices",
                columns: new[] { "organization_id", "item_id", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_item_prices_organization_id_supplier_id_item_id_ef~",
                schema: "procurement",
                table: "supplier_item_prices",
                columns: new[] { "organization_id", "supplier_id", "item_id", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_item_prices_supplier_id",
                schema: "procurement",
                table: "supplier_item_prices",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_item_prices_updated_by_user_id",
                schema: "procurement",
                table: "supplier_item_prices",
                column: "updated_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "supplier_item_prices",
                schema: "procurement");
        }
    }
}
