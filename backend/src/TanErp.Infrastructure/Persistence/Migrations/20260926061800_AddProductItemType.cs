using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_items_item_type",
                schema: "item_master",
                table: "items");

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_item_type",
                schema: "item_master",
                table: "items",
                sql: "item_type IN ('material', 'labor', 'service', 'subcontract', 'other', 'product')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_items_item_type",
                schema: "item_master",
                table: "items");

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_item_type",
                schema: "item_master",
                table: "items",
                sql: "item_type IN ('material', 'labor', 'service', 'subcontract', 'other')");
        }
    }
}
