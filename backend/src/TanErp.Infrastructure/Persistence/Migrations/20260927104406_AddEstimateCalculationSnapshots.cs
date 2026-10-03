using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateCalculationSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "estimate_calculation_snapshots",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calculation_version = table.Column<int>(type: "integer", nullable: false),
                    input_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    calculation_policy_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    tax_policy_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    captured_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    captured_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_calculation_snapshots", x => x.id);
                    table.UniqueConstraint("AK_estimate_calculation_snapshots_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_calculation_snapshots_version", "calculation_version > 0");
                    table.ForeignKey(
                        name: "FK_estimate_calculation_snapshots_estimate_revisions_estimate_~",
                        columns: x => new { x.estimate_revision_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_calculation_snapshots_estimate_revision_id_organiz~",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                columns: new[] { "estimate_revision_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_calculation_snapshots_organization_id_estimate_rev~",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                columns: new[] { "organization_id", "estimate_revision_id", "calculation_version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estimate_calculation_snapshots",
                schema: "estimates");
        }
    }
}
