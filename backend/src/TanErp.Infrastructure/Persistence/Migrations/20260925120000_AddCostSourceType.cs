using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TanErp.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260925120000_AddCostSourceType")]
public sealed class AddCostSourceType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "source_type", schema: "item_master", table: "cost_sources", type: "character varying(16)", maxLength: 16, nullable: true);
        migrationBuilder.Sql("UPDATE item_master.cost_sources SET source_type = 'legacy' WHERE source_type IS NULL;");
        migrationBuilder.AlterColumn<string>(name: "source_type", schema: "item_master", table: "cost_sources", type: "character varying(16)", maxLength: 16, nullable: false, oldClrType: typeof(string), oldType: "character varying(16)", oldMaxLength: 16, oldNullable: true);
        migrationBuilder.AddCheckConstraint(name: "CK_cost_sources_source_type", schema: "item_master", table: "cost_sources", sql: "source_type IN ('manual', 'legacy')");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(name: "CK_cost_sources_source_type", schema: "item_master", table: "cost_sources");
        migrationBuilder.DropColumn(name: "source_type", schema: "item_master", table: "cost_sources");
    }
}
