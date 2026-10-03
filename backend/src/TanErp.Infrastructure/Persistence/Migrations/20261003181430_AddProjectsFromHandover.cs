using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectsFromHandover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "projects");

            migrationBuilder.CreateTable(
                name: "projects",
                schema: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estimate_revision_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_survey_revision_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    planned_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    baseline_quotation_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    baseline_contract_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    baseline_quotation_snapshot_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    baseline_survey_snapshot_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    baseline_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                    table.CheckConstraint("ck_projects_baseline_contract_amount", "baseline_contract_amount >= 0");
                    table.CheckConstraint("ck_projects_status", "status IN ('planned', 'active', 'on_hold', 'ready_for_handover', 'completed', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_projects_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_opportunities_opportunity_id",
                        column: x => x.opportunity_id,
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalSchema: "commercial",
                        principalTable: "quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_users_owner_user_id",
                        column: x => x.owner_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_projects_branch_id",
                schema: "projects",
                table: "projects",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_created_by_user_id",
                schema: "projects",
                table: "projects",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_customer_id",
                schema: "projects",
                table: "projects",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_opportunity_id",
                schema: "projects",
                table: "projects",
                column: "opportunity_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_organization_id_branch_id_status_created_at_utc",
                schema: "projects",
                table: "projects",
                columns: new[] { "organization_id", "branch_id", "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_projects_organization_id_code",
                schema: "projects",
                table: "projects",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_organization_id_owner_user_id",
                schema: "projects",
                table: "projects",
                columns: new[] { "organization_id", "owner_user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_projects_organization_id_quotation_id",
                schema: "projects",
                table: "projects",
                columns: new[] { "organization_id", "quotation_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_owner_user_id",
                schema: "projects",
                table: "projects",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_quotation_id",
                schema: "projects",
                table: "projects",
                column: "quotation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "projects",
                schema: "projects");
        }
    }
}
