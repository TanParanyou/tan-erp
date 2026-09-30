using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TanErp.Infrastructure.Persistence;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260926202000_AddItemTaxCategories")]
public partial class AddItemTaxCategories : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "item_tax_categories",
            schema: "item_master",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                normalized_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                name = table.Column<string>(type: "jsonb", nullable: false),
                status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                sort_order = table.Column<int>(type: "integer", nullable: false),
                row_version = table.Column<Guid>(type: "uuid", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_item_tax_categories", x => x.id);
                table.UniqueConstraint("AK_item_tax_categories_id_organization_id", x => new { x.id, x.organization_id });
                table.ForeignKey("FK_item_tax_categories_organizations_organization_id", x => x.organization_id, "organizations", "id", principalSchema: "organization", onDelete: ReferentialAction.Restrict);
                table.CheckConstraint("CK_item_tax_categories_name_shape", "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");
                table.CheckConstraint("CK_item_tax_categories_sort_order", "sort_order >= 0");
                table.CheckConstraint("CK_item_tax_categories_status", "status IN ('active', 'inactive')");
            });

        migrationBuilder.CreateIndex("IX_item_tax_categories_organization_id_normalized_code", "item_tax_categories", new[] { "organization_id", "normalized_code" }, schema: "item_master", unique: true);
        migrationBuilder.CreateIndex("IX_item_tax_categories_organization_id_status_sort_order_id", "item_tax_categories", new[] { "organization_id", "status", "sort_order", "id" }, schema: "item_master");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "item_tax_categories", schema: "item_master");
    }
}
