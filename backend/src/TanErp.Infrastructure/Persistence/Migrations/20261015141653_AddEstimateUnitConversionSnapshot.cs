using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateUnitConversionSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "conversion_factor",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "conversion_from_unit",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "conversion_id",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "conversion_scope",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "conversion_to_unit",
                schema: "estimates",
                table: "estimate_cost_components",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit_conversion_mode",
                schema: "estimates",
                table: "calculation_policy_versions",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "none");

            migrationBuilder.AddCheckConstraint(
                name: "ck_estimate_cost_components_conversion_snapshot",
                schema: "estimates",
                table: "estimate_cost_components",
                sql: "(conversion_id IS NULL AND conversion_scope IS NULL AND conversion_from_unit IS NULL AND conversion_to_unit IS NULL AND conversion_factor IS NULL) OR (conversion_id IS NOT NULL AND conversion_scope IN ('item', 'shared') AND conversion_from_unit IS NOT NULL AND conversion_to_unit IS NOT NULL AND conversion_factor > 0)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_estimate_calculation_policy_unit_conversion_mode",
                schema: "estimates",
                table: "calculation_policy_versions",
                sql: "unit_conversion_mode IN ('none', 'snapshot')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_estimate_cost_components_conversion_snapshot",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropCheckConstraint(
                name: "ck_estimate_calculation_policy_unit_conversion_mode",
                schema: "estimates",
                table: "calculation_policy_versions");

            migrationBuilder.DropColumn(
                name: "conversion_factor",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "conversion_from_unit",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "conversion_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "conversion_scope",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "conversion_to_unit",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropColumn(
                name: "unit_conversion_mode",
                schema: "estimates",
                table: "calculation_policy_versions");
        }
    }
}
