using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "estimates");

            migrationBuilder.CreateTable(
                name: "estimates",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_revision_id = table.Column<Guid>(type: "uuid", nullable: true),
                    site_survey_snapshot_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    current_revision_no = table.Column<int>(type: "integer", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimates", x => x.id);
                    table.UniqueConstraint("AK_estimates_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimates_current_revision_no", "current_revision_no >= 1");
                    table.CheckConstraint("ck_estimates_status", "status IN ('draft', 'submitted', 'returned', 'approved', 'quoted', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_estimates_branches_branch_id_organization_id",
                        columns: x => new { x.branch_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimates_customers_customer_id_organization_id",
                        columns: x => new { x.customer_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimates_opportunities_opportunity_id_organization_id",
                        columns: x => new { x.opportunity_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimates_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estimates_site_survey_revisions_site_survey_revision_id_org~",
                        columns: x => new { x.site_survey_revision_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "site_survey_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "estimate_revisions",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision_no = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    calculation_version = table.Column<int>(type: "integer", nullable: false),
                    calculation_policy_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    tax_policy_version = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    net_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    selling_before_discount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    net_before_tax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    margin_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    margin_rate = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    markup_rate = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    calculation_snapshot_json = table.Column<string>(type: "text", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_revisions", x => x.id);
                    table.UniqueConstraint("AK_estimate_revisions_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_revisions_grand_total", "grand_total >= 0");
                    table.CheckConstraint("ck_estimate_revisions_net_cost", "net_cost >= 0");
                    table.CheckConstraint("ck_estimate_revisions_revision_no", "revision_no >= 1");
                    table.CheckConstraint("ck_estimate_revisions_status", "status IN ('draft', 'submitted', 'returned', 'approved', 'quoted', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_estimate_revisions_estimates_estimate_id_organization_id",
                        columns: x => new { x.estimate_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimates",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_estimate_revisions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estimate_sections",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name_th = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    subtotal_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    subtotal_selling_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_sections", x => x.id);
                    table.UniqueConstraint("AK_estimate_sections_id_organization_id", x => new { x.id, x.organization_id });
                    table.ForeignKey(
                        name: "FK_estimate_sections_estimate_revisions_estimate_revision_id_o~",
                        columns: x => new { x.estimate_revision_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_revisions",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_estimate_sections_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estimate_work_items",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_section_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description_th = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_en = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    selling_rule_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    selling_rule_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_selling_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_selling_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_work_items", x => x.id);
                    table.UniqueConstraint("AK_estimate_work_items_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_work_items_quantity", "quantity > 0");
                    table.CheckConstraint("ck_estimate_work_items_selling_rule_type", "selling_rule_type IN ('margin', 'markup', 'fixed_price')");
                    table.ForeignKey(
                        name: "FK_estimate_work_items_estimate_sections_estimate_section_id_o~",
                        columns: x => new { x.estimate_section_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_sections",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_estimate_work_items_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estimate_cost_components",
                schema: "estimates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estimate_cost_components", x => x.id);
                    table.UniqueConstraint("AK_estimate_cost_components_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_estimate_cost_components_quantity", "quantity > 0");
                    table.CheckConstraint("ck_estimate_cost_components_type", "type IN ('material', 'labor', 'subcontract', 'service', 'other_direct')");
                    table.CheckConstraint("ck_estimate_cost_components_unit_cost", "unit_cost >= 0");
                    table.ForeignKey(
                        name: "FK_estimate_cost_components_estimate_work_items_estimate_work_~",
                        columns: x => new { x.estimate_work_item_id, x.organization_id },
                        principalSchema: "estimates",
                        principalTable: "estimate_work_items",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_estimate_cost_components_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_estimate_work_item_id_organization~",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "estimate_work_item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_organization_id_estimate_work_item~",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "organization_id", "estimate_work_item_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_revisions_estimate_id_organization_id",
                schema: "estimates",
                table: "estimate_revisions",
                columns: new[] { "estimate_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_revisions_organization_id_estimate_id_revision_no",
                schema: "estimates",
                table: "estimate_revisions",
                columns: new[] { "organization_id", "estimate_id", "revision_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_sections_estimate_revision_id_organization_id",
                schema: "estimates",
                table: "estimate_sections",
                columns: new[] { "estimate_revision_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_sections_organization_id_estimate_revision_id_code",
                schema: "estimates",
                table: "estimate_sections",
                columns: new[] { "organization_id", "estimate_revision_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_work_items_estimate_section_id_organization_id",
                schema: "estimates",
                table: "estimate_work_items",
                columns: new[] { "estimate_section_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_work_items_organization_id_estimate_section_id_code",
                schema: "estimates",
                table: "estimate_work_items",
                columns: new[] { "organization_id", "estimate_section_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimates_branch_id_organization_id",
                schema: "estimates",
                table: "estimates",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimates_customer_id_organization_id",
                schema: "estimates",
                table: "estimates",
                columns: new[] { "customer_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimates_opportunity_id_organization_id",
                schema: "estimates",
                table: "estimates",
                columns: new[] { "opportunity_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimates_organization_id_number",
                schema: "estimates",
                table: "estimates",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimates_organization_id_opportunity_id",
                schema: "estimates",
                table: "estimates",
                columns: new[] { "organization_id", "opportunity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimates_site_survey_revision_id_organization_id",
                schema: "estimates",
                table: "estimates",
                columns: new[] { "site_survey_revision_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estimate_cost_components",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimate_work_items",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimate_sections",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimate_revisions",
                schema: "estimates");

            migrationBuilder.DropTable(
                name: "estimates",
                schema: "estimates");
        }
    }
}
