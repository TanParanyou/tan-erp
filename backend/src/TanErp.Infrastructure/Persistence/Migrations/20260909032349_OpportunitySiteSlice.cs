using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OpportunitySiteSlice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sites",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    normalized_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_line1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    subdistrict = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    country_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(10,6)", nullable: true),
                    access_note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sites", x => x.id);
                    table.UniqueConstraint("AK_sites_id_customer_id_organization_id", x => new { x.id, x.customer_id, x.organization_id });
                    table.UniqueConstraint("AK_sites_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_sites_coordinates_paired", "(latitude IS NULL AND longitude IS NULL) OR (latitude IS NOT NULL AND longitude IS NOT NULL)");
                    table.CheckConstraint("CK_sites_latitude_range", "latitude IS NULL OR (latitude >= -90.000000 AND latitude <= 90.000000)");
                    table.CheckConstraint("CK_sites_longitude_range", "longitude IS NULL OR (longitude >= -180.000000 AND longitude <= 180.000000)");
                    table.CheckConstraint("CK_sites_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_sites_customers_customer_id_organization_id",
                        columns: x => new { x.customer_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sites_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "opportunities",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    primary_site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    normalized_title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    scope_summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    work_types = table.Column<string[]>(type: "text[]", nullable: false),
                    source_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    expected_budget = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    target_decision_date = table.Column<DateOnly>(type: "date", nullable: true),
                    next_action_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    next_action_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    stage = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunities", x => x.id);
                    table.UniqueConstraint("AK_opportunities_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_opportunities_expected_budget", "expected_budget IS NULL OR expected_budget > 0");
                    table.CheckConstraint("CK_opportunities_stage", "stage IN ('draft', 'qualified', 'surveying', 'estimating', 'proposed', 'won', 'lost', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_opportunities_branches_branch_id_organization_id",
                        columns: x => new { x.branch_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunities_customers_customer_id_organization_id",
                        columns: x => new { x.customer_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunities_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunities_sites_primary_site_id_customer_id_organizatio~",
                        columns: x => new { x.primary_site_id, x.customer_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "sites",
                        principalColumns: new[] { "id", "customer_id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_branch_id_organization_id",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_customer_id_organization_id",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "customer_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_organization_id_branch_id_stage_owner_user_id~",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "organization_id", "branch_id", "stage", "owner_user_id", "next_action_at_utc", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_organization_id_code",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_organization_id_customer_id_stage_id",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "organization_id", "customer_id", "stage", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_primary_site_id_customer_id_organization_id",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "primary_site_id", "customer_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_sites_customer_id_organization_id",
                schema: "crm",
                table: "sites",
                columns: new[] { "customer_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_sites_organization_id_code",
                schema: "crm",
                table: "sites",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sites_organization_id_customer_id_status_normalized_label_id",
                schema: "crm",
                table: "sites",
                columns: new[] { "organization_id", "customer_id", "status", "normalized_label", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opportunities",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "sites",
                schema: "crm");
        }
    }
}
