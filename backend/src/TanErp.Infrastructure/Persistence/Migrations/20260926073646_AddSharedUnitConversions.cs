using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedUnitConversions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unit_conversions",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    factor = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit_conversions", x => x.id);
                    table.UniqueConstraint("AK_unit_conversions_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_unit_conversions_distinct_units", "from_unit_id <> to_unit_id");
                    table.CheckConstraint("CK_unit_conversions_effective_period", "effective_to IS NULL OR effective_to >= effective_from");
                    table.CheckConstraint("CK_unit_conversions_factor", "factor > 0");
                    table.CheckConstraint("CK_unit_conversions_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_unit_conversions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unit_conversions_units_from_unit_id_organization_id",
                        columns: x => new { x.from_unit_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unit_conversions_units_to_unit_id_organization_id",
                        columns: x => new { x.to_unit_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_unit_conversions_from_unit_id_organization_id",
                schema: "item_master",
                table: "unit_conversions",
                columns: new[] { "from_unit_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_unit_conversions_organization_id_from_unit_id_to_unit_id_ef~",
                schema: "item_master",
                table: "unit_conversions",
                columns: new[] { "organization_id", "from_unit_id", "to_unit_id", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_unit_conversions_to_unit_id_organization_id",
                schema: "item_master",
                table: "unit_conversions",
                columns: new[] { "to_unit_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unit_conversions",
                schema: "item_master");
        }
    }
}
