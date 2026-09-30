using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionedEstimatePolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "calculation_policy_hash",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "calculation_policy_version_id",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tax_policy_hash",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "tax_policy_version_id",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "calculation_policy_versions",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    policy_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    overhead_method = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    overhead_value = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: false),
                    rounding_mode = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    effective_from_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    effective_to_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calculation_policy_versions", x => x.id);
                    table.UniqueConstraint("AK_calculation_policy_versions_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_calculation_policy_overhead", "overhead_value >= 0 AND (overhead_method <> 'percent-direct-cost' OR overhead_value <= 1)");
                    table.CheckConstraint("ck_estimate_calculation_policy_period", "effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
                    table.CheckConstraint("ck_estimate_calculation_policy_status", "status IN ('draft', 'published', 'superseded', 'disabled')");
                    table.ForeignKey(
                        name: "FK_calculation_policy_versions_branches_branch_id_organization~",
                        columns: x => new { x.branch_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_calculation_policy_versions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tax_policy_versions",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    policy_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    tax_mode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: false),
                    tax_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    effective_from_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    effective_to_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_policy_versions", x => x.id);
                    table.UniqueConstraint("AK_tax_policy_versions_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_tax_policy_mode", "tax_mode IN ('exclusive', 'inclusive', 'exempt')");
                    table.CheckConstraint("ck_estimate_tax_policy_period", "effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
                    table.CheckConstraint("ck_estimate_tax_policy_rate", "tax_rate >= 0 AND tax_rate <= 1 AND (tax_mode <> 'exempt' OR tax_rate = 0)");
                    table.CheckConstraint("ck_estimate_tax_policy_status", "status IN ('draft', 'published', 'superseded', 'disabled')");
                    table.ForeignKey(
                        name: "FK_tax_policy_versions_branches_branch_id_organization_id",
                        columns: x => new { x.branch_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tax_policy_versions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_calculation_snapshots_calculation_policy_version_i~",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                columns: new[] { "calculation_policy_version_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_calculation_snapshots_tax_policy_version_id_organi~",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                columns: new[] { "tax_policy_version_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_calculation_policy_versions_branch_id_organization_id",
                schema: "estimates",
                table: "calculation_policy_versions",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_calculation_policy_versions_organization_id_branch_id_polic~",
                schema: "estimates",
                table: "calculation_policy_versions",
                columns: new[] { "organization_id", "branch_id", "policy_code", "version" },
                unique: true,
                filter: "branch_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_calculation_policy_versions_organization_id_policy_code_ver~",
                schema: "estimates",
                table: "calculation_policy_versions",
                columns: new[] { "organization_id", "policy_code", "version" },
                unique: true,
                filter: "branch_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tax_policy_versions_branch_id_organization_id",
                schema: "estimates",
                table: "tax_policy_versions",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_tax_policy_versions_organization_id_branch_id_policy_code_v~",
                schema: "estimates",
                table: "tax_policy_versions",
                columns: new[] { "organization_id", "branch_id", "policy_code", "version" },
                unique: true,
                filter: "branch_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tax_policy_versions_organization_id_policy_code_version",
                schema: "estimates",
                table: "tax_policy_versions",
                columns: new[] { "organization_id", "policy_code", "version" },
                unique: true,
                filter: "branch_id IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_calculation_snapshots_calculation_policy_versions_~",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                columns: new[] { "calculation_policy_version_id", "organization_id" },
                principalSchema: "estimates",
                principalTable: "calculation_policy_versions",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_calculation_snapshots_tax_policy_versions_tax_poli~",
                schema: "estimates",
                table: "estimate_calculation_snapshots",
                columns: new[] { "tax_policy_version_id", "organization_id" },
                principalSchema: "estimates",
                principalTable: "tax_policy_versions",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimate_calculation_snapshots_calculation_policy_versions_~",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_estimate_calculation_snapshots_tax_policy_versions_tax_poli~",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropTable(
                name: "calculation_policy_versions",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "tax_policy_versions",
                schema: "estimates");

            migrationBuilder.DropIndex(
                name: "IX_estimate_calculation_snapshots_calculation_policy_version_i~",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropIndex(
                name: "IX_estimate_calculation_snapshots_tax_policy_version_id_organi~",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropColumn(
                name: "calculation_policy_hash",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropColumn(
                name: "calculation_policy_version_id",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropColumn(
                name: "tax_policy_hash",
                schema: "estimates",
                table: "estimate_calculation_snapshots");

            migrationBuilder.DropColumn(
                name: "tax_policy_version_id",
                schema: "estimates",
                table: "estimate_calculation_snapshots");
        }
    }
}
