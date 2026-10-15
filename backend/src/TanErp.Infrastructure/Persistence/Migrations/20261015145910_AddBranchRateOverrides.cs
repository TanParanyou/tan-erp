using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchRateOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_rate_overrides",
                schema: "quick_estimate",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reference_rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    minimum_charge = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: false),
                    reason_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    reason_detail = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    source_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    closed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    closed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    close_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branch_rate_overrides", x => x.id);
                    table.CheckConstraint("ck_branch_rate_overrides_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> requested_by_user_id");
                    table.CheckConstraint("ck_branch_rate_overrides_period", "effective_to >= effective_from");
                    table.CheckConstraint("ck_branch_rate_overrides_rates", "reference_rate > 0 AND minimum_charge >= 0");
                    table.CheckConstraint("ck_branch_rate_overrides_status", "status IN ('requested', 'approved', 'returned', 'revoked')");
                    table.ForeignKey(
                        name: "FK_branch_rate_overrides_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branch_rate_overrides_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branch_rate_overrides_pricing_templates_template_id",
                        column: x => x.template_id,
                        principalSchema: "quick_estimate",
                        principalTable: "pricing_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branch_rate_overrides_users_closed_by_user_id",
                        column: x => x.closed_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branch_rate_overrides_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branch_rate_overrides_users_requested_by_user_id",
                        column: x => x.requested_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_branch_rate_overrides_branch_id",
                schema: "quick_estimate",
                table: "branch_rate_overrides",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_branch_rate_overrides_closed_by_user_id",
                schema: "quick_estimate",
                table: "branch_rate_overrides",
                column: "closed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_branch_rate_overrides_decided_by_user_id",
                schema: "quick_estimate",
                table: "branch_rate_overrides",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_branch_rate_overrides_organization_id_template_id_branch_id~",
                schema: "quick_estimate",
                table: "branch_rate_overrides",
                columns: new[] { "organization_id", "template_id", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_branch_rate_overrides_requested_by_user_id",
                schema: "quick_estimate",
                table: "branch_rate_overrides",
                column: "requested_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_branch_rate_overrides_template_id",
                schema: "quick_estimate",
                table: "branch_rate_overrides",
                column: "template_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_rate_overrides",
                schema: "quick_estimate");
        }
    }
}
