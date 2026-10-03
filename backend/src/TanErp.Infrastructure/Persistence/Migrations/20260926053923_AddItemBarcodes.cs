using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemBarcodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_barcodes",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    value = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    normalized_value = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity_in_base_unit = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    packaging_level = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_barcodes", x => x.id);
                    table.UniqueConstraint("AK_item_barcodes_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_item_barcodes_identifier_type", "identifier_type IN ('gtin', 'internal')");
                    table.CheckConstraint("CK_item_barcodes_packaging_level", "packaging_level IN ('each', 'inner', 'case', 'pallet')");
                    table.CheckConstraint("CK_item_barcodes_quantity", "quantity_in_base_unit > 0");
                    table.CheckConstraint("CK_item_barcodes_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_item_barcodes_items_item_id_organization_id",
                        columns: x => new { x.item_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_barcodes_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_barcodes_units_unit_id_organization_id",
                        columns: x => new { x.unit_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_barcodes_item_id_organization_id",
                schema: "item_master",
                table: "item_barcodes",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_barcodes_organization_id_normalized_value",
                schema: "item_master",
                table: "item_barcodes",
                columns: new[] { "organization_id", "normalized_value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_item_barcodes_single_active_primary",
                schema: "item_master",
                table: "item_barcodes",
                columns: new[] { "organization_id", "item_id", "packaging_level" },
                unique: true,
                filter: "is_primary = true AND status = 'active'");

            migrationBuilder.CreateIndex(
                name: "IX_item_barcodes_unit_id_organization_id",
                schema: "item_master",
                table: "item_barcodes",
                columns: new[] { "unit_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_barcodes",
                schema: "item_master");
        }
    }
}
