using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TanErp.Infrastructure.Persistence;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260927141500_AddEstimateCostProvenance")]
public sealed class AddEstimateCostProvenance : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "cost_origin", schema: "estimates", table: "estimate_cost_components", type: "character varying(16)", maxLength: 16, nullable: false, defaultValue: "manual");
        migrationBuilder.AddColumn<Guid>(name: "cost_source_id_snapshot", schema: "estimates", table: "estimate_cost_components", type: "uuid", nullable: true);
        migrationBuilder.AddColumn<string>(name: "cost_source_code_snapshot", schema: "estimates", table: "estimate_cost_components", type: "character varying(50)", maxLength: 50, nullable: true);
        migrationBuilder.AddColumn<string>(name: "cost_source_reference_snapshot", schema: "estimates", table: "estimate_cost_components", type: "character varying(500)", maxLength: 500, nullable: true);
        migrationBuilder.AddColumn<Guid>(name: "cost_evidence_file_id_snapshot", schema: "estimates", table: "estimate_cost_components", type: "uuid", nullable: true);
        migrationBuilder.AddColumn<string>(name: "cost_record_reason_snapshot", schema: "estimates", table: "estimate_cost_components", type: "character varying(1000)", maxLength: 1000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "provisional_reason_code", schema: "estimates", table: "estimate_cost_components", type: "character varying(64)", maxLength: 64, nullable: true);
        migrationBuilder.AddColumn<string>(name: "provisional_note", schema: "estimates", table: "estimate_cost_components", type: "character varying(1000)", maxLength: 1000, nullable: true);

        migrationBuilder.Sql("UPDATE estimates.estimate_cost_components SET cost_origin = CASE WHEN item_id IS NULL AND cost_record_id IS NULL AND item_code_snapshot IS NULL THEN 'manual' ELSE 'catalog' END;");
        migrationBuilder.AddCheckConstraint("ck_estimate_cost_components_cost_origin", "estimate_cost_components", "cost_origin IN ('manual', 'catalog')", schema: "estimates");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint("ck_estimate_cost_components_cost_origin", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("cost_origin", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("cost_source_id_snapshot", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("cost_source_code_snapshot", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("cost_source_reference_snapshot", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("cost_evidence_file_id_snapshot", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("cost_record_reason_snapshot", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("provisional_reason_code", "estimate_cost_components", schema: "estimates");
        migrationBuilder.DropColumn("provisional_note", "estimate_cost_components", schema: "estimates");
    }
}
