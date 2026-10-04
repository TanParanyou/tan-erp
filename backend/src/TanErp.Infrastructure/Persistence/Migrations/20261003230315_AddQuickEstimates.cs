using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuickEstimates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "quick_estimate");

            migrationBuilder.CreateTable(
                name: "pricing_templates",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    work_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    measurement_rule = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    unit_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reference_rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    minimum_charge = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    base_range_rate = table.Column<decimal>(type: "numeric(6,4)", precision: 6, scale: 4, nullable: false),
                    max_range_rate = table.Column<decimal>(type: "numeric(6,4)", precision: 6, scale: 4, nullable: false),
                    rounding_step = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    validity_days = table.Column<int>(type: "integer", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(6,4)", precision: 6, scale: 4, nullable: false),
                    tax_display = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    direct_share_limit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: true),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    config = table.Column<string>(type: "jsonb", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    submitted_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricing_templates", x => x.id);
                    table.CheckConstraint("ck_pricing_templates_rates", "reference_rate > 0 AND minimum_charge >= 0 AND base_range_rate >= 0 AND max_range_rate >= base_range_rate AND max_range_rate <= 0.9 AND rounding_step > 0");
                    table.CheckConstraint("ck_pricing_templates_rule", "measurement_rule IN ('area', 'length', 'volume', 'count')");
                    table.CheckConstraint("ck_pricing_templates_status", "status IN ('draft', 'submitted', 'approved', 'calibration', 'active', 'superseded', 'disabled')");
                    table.CheckConstraint("ck_pricing_templates_tax", "tax_rate BETWEEN 0 AND 0.3 AND tax_display IN ('exclusive', 'inclusive')");
                    table.CheckConstraint("ck_pricing_templates_work_type", "work_type IN ('built-in', 'curtain', 'wallpaper')");
                    table.ForeignKey(
                        name: "FK_pricing_templates_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pricing_templates_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pricing_templates_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quick_estimates",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: true),
                    property_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    room_or_area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    grade_code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    complexity_codes = table.Column<string>(type: "jsonb", nullable: false),
                    add_on_codes = table.Column<string>(type: "jsonb", nullable: false),
                    measurement_confidence = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    custom_material = table.Column<bool>(type: "boolean", nullable: false),
                    measurements = table.Column<string>(type: "jsonb", nullable: false),
                    current_calculation_version = table.Column<int>(type: "integer", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quick_estimates", x => x.id);
                    table.CheckConstraint("ck_quick_estimates_confidence", "measurement_confidence IN ('low', 'medium', 'high')");
                    table.CheckConstraint("ck_quick_estimates_status", "status IN ('draft', 'calculated', 'pending_review', 'approved', 'returned', 'converted')");
                    table.ForeignKey(
                        name: "FK_quick_estimates_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimates_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimates_pricing_templates_template_id",
                        column: x => x.template_id,
                        principalSchema: "quick_estimate",
                        principalTable: "pricing_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimates_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quick_estimate_calculations",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quick_estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    template_version = table.Column<int>(type: "integer", nullable: false),
                    input_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    net_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    displayed_lower = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    displayed_upper = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: false),
                    share_decision = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reason_codes = table.Column<string>(type: "jsonb", nullable: false),
                    snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quick_estimate_calculations", x => x.id);
                    table.CheckConstraint("ck_quick_estimate_calculations_decision", "share_decision IN ('blocked', 'pending_review', 'shareable')");
                    table.CheckConstraint("ck_quick_estimate_calculations_range", "displayed_lower <= displayed_upper AND displayed_lower >= 0");
                    table.ForeignKey(
                        name: "FK_quick_estimate_calculations_pricing_templates_template_id",
                        column: x => x.template_id,
                        principalSchema: "quick_estimate",
                        principalTable: "pricing_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimate_calculations_quick_estimates_quick_estimate_~",
                        column: x => x.quick_estimate_id,
                        principalSchema: "quick_estimate",
                        principalTable: "quick_estimates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimate_calculations_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quick_estimate_conversions",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quick_estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version = table.Column<int>(type: "integer", nullable: false),
                    official_estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quick_estimate_conversions", x => x.id);
                    table.ForeignKey(
                        name: "FK_quick_estimate_conversions_quick_estimates_quick_estimate_id",
                        column: x => x.quick_estimate_id,
                        principalSchema: "quick_estimate",
                        principalTable: "quick_estimates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimate_conversions_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quick_estimate_reviews",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quick_estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    request_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    reason_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quick_estimate_reviews", x => x.id);
                    table.CheckConstraint("ck_quick_estimate_reviews_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> requested_by_user_id");
                    table.CheckConstraint("ck_quick_estimate_reviews_status", "status IN ('requested', 'approved', 'returned')");
                    table.ForeignKey(
                        name: "FK_quick_estimate_reviews_quick_estimates_quick_estimate_id",
                        column: x => x.quick_estimate_id,
                        principalSchema: "quick_estimate",
                        principalTable: "quick_estimates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimate_reviews_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimate_reviews_users_requested_by_user_id",
                        column: x => x.requested_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quick_estimate_shares",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quick_estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version = table.Column<int>(type: "integer", nullable: false),
                    channel = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    recipient = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    locale = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    summary = table.Column<string>(type: "jsonb", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quick_estimate_shares", x => x.id);
                    table.CheckConstraint("ck_quick_estimate_shares_channel", "channel IN ('onscreen', 'pdf', 'line', 'email')");
                    table.ForeignKey(
                        name: "FK_quick_estimate_shares_quick_estimates_quick_estimate_id",
                        column: x => x.quick_estimate_id,
                        principalSchema: "quick_estimate",
                        principalTable: "quick_estimates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quick_estimate_shares_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pricing_templates_created_by_user_id",
                schema: "quick_estimate",
                table: "pricing_templates",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_templates_decided_by_user_id",
                schema: "quick_estimate",
                table: "pricing_templates",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_templates_organization_id_code_version",
                schema: "quick_estimate",
                table: "pricing_templates",
                columns: new[] { "organization_id", "code", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pricing_templates_organization_id_status_work_type",
                schema: "quick_estimate",
                table: "pricing_templates",
                columns: new[] { "organization_id", "status", "work_type" });

            migrationBuilder.CreateIndex(
                name: "ux_pricing_templates_one_active",
                schema: "quick_estimate",
                table: "pricing_templates",
                columns: new[] { "organization_id", "code" },
                unique: true,
                filter: "status = 'active'");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_calculations_created_by_user_id",
                schema: "quick_estimate",
                table: "quick_estimate_calculations",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_calculations_quick_estimate_id_version",
                schema: "quick_estimate",
                table: "quick_estimate_calculations",
                columns: new[] { "quick_estimate_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_calculations_template_id",
                schema: "quick_estimate",
                table: "quick_estimate_calculations",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_conversions_created_by_user_id",
                schema: "quick_estimate",
                table: "quick_estimate_conversions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_conversions_official_estimate_id",
                schema: "quick_estimate",
                table: "quick_estimate_conversions",
                column: "official_estimate_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_conversions_quick_estimate_id_source_version",
                schema: "quick_estimate",
                table: "quick_estimate_conversions",
                columns: new[] { "quick_estimate_id", "source_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_reviews_decided_by_user_id",
                schema: "quick_estimate",
                table: "quick_estimate_reviews",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_reviews_quick_estimate_id_source_version",
                schema: "quick_estimate",
                table: "quick_estimate_reviews",
                columns: new[] { "quick_estimate_id", "source_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_reviews_requested_by_user_id",
                schema: "quick_estimate",
                table: "quick_estimate_reviews",
                column: "requested_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_shares_created_by_user_id",
                schema: "quick_estimate",
                table: "quick_estimate_shares",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimate_shares_quick_estimate_id_source_version",
                schema: "quick_estimate",
                table: "quick_estimate_shares",
                columns: new[] { "quick_estimate_id", "source_version" });

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimates_branch_id",
                schema: "quick_estimate",
                table: "quick_estimates",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimates_created_by_user_id",
                schema: "quick_estimate",
                table: "quick_estimates",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimates_organization_id_number",
                schema: "quick_estimate",
                table: "quick_estimates",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimates_organization_id_status_updated_at_utc",
                schema: "quick_estimate",
                table: "quick_estimates",
                columns: new[] { "organization_id", "status", "updated_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_quick_estimates_template_id",
                schema: "quick_estimate",
                table: "quick_estimates",
                column: "template_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quick_estimate_calculations",
                schema: "quick_estimate");

            migrationBuilder.DropTable(
                name: "quick_estimate_conversions",
                schema: "quick_estimate");

            migrationBuilder.DropTable(
                name: "quick_estimate_reviews",
                schema: "quick_estimate");

            migrationBuilder.DropTable(
                name: "quick_estimate_shares",
                schema: "quick_estimate");

            migrationBuilder.DropTable(
                name: "quick_estimates",
                schema: "quick_estimate");

            migrationBuilder.DropTable(
                name: "pricing_templates",
                schema: "quick_estimate");
        }
    }
}
