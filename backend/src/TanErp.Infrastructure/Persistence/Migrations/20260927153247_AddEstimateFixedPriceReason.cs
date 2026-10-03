using Microsoft.EntityFrameworkCore.Migrations;

namespace TanErp.Infrastructure.Persistence.Migrations;

public partial class AddEstimateFixedPriceReason : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "selling_rule_reason_code",
            schema: "estimates",
            table: "estimate_work_items",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "selling_rule_reason_code", schema: "estimates", table: "estimate_work_items");
    }
}
