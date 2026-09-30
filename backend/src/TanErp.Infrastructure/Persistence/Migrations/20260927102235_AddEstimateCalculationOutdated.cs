using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateCalculationOutdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "calculation_outdated",
                schema: "estimates",
                table: "estimate_revisions",
                type: "boolean",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE estimates.estimate_revisions SET calculation_outdated = TRUE;");

            migrationBuilder.AlterColumn<bool>(
                name: "calculation_outdated",
                schema: "estimates",
                table: "estimate_revisions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "calculation_outdated",
                schema: "estimates",
                table: "estimate_revisions");
        }
    }
}
