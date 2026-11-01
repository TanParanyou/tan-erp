using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemPhysicalSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "height_mm",
                schema: "item_master",
                table: "items",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "length_mm",
                schema: "item_master",
                table: "items",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "weight_kg",
                schema: "item_master",
                table: "items",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "width_mm",
                schema: "item_master",
                table: "items",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "height_mm",
                schema: "item_master",
                table: "items");

            migrationBuilder.DropColumn(
                name: "length_mm",
                schema: "item_master",
                table: "items");

            migrationBuilder.DropColumn(
                name: "weight_kg",
                schema: "item_master",
                table: "items");

            migrationBuilder.DropColumn(
                name: "width_mm",
                schema: "item_master",
                table: "items");
        }
    }
}
