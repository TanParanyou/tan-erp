using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TanErp.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260925121500_AllowZeroCostWithReason")]
public sealed class AllowZeroCostWithReason : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(name: "CK_cost_records_amount", schema: "item_master", table: "cost_records");
        migrationBuilder.AddCheckConstraint(name: "CK_cost_records_amount", schema: "item_master", table: "cost_records", sql: "amount >= 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DO $$ BEGIN IF EXISTS (SELECT 1 FROM item_master.cost_records WHERE amount = 0) THEN RAISE EXCEPTION 'Cannot downgrade while zero amount cost records exist.'; END IF; END $$;");
        migrationBuilder.DropCheckConstraint(name: "CK_cost_records_amount", schema: "item_master", table: "cost_records");
        migrationBuilder.AddCheckConstraint(name: "CK_cost_records_amount", schema: "item_master", table: "cost_records", sql: "amount > 0");
    }
}
