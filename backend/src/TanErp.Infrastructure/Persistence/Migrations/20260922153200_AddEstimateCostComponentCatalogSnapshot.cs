using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateCostComponentCatalogSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cost_effective_from_utc",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cost_policy_version",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cost_record_version",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cost_scope_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item_code_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "item_id",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item_name_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "resolved_at_utc",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_cost_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "cost_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_item_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_organization_id_item_id",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "organization_id", "item_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_cost_components_cost_records_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "cost_record_id",
                principalSchema: "item_master",
                principalTable: "cost_records",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_cost_components_items_item_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "item_id",
                principalSchema: "item_master",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimate_cost_components_cost_records_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropForeignKey(
                name: "FK_estimate_cost_components_items_item_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_item_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_organization_id_item_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "cost_effective_from_utc",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "cost_policy_version",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "cost_record_version",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "cost_scope_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "currency_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "item_code_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "item_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "item_name_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "resolved_at_utc",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "unit_cost_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "unit_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");
        }
    }
}
