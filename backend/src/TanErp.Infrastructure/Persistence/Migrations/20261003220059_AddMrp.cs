using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMrp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mrp");

            migrationBuilder.CreateTable(
                name: "runs",
                schema: "mrp",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    as_of_date = table.Column<DateOnly>(type: "date", nullable: false),
                    purchase_lead_time_days = table.Column<int>(type: "integer", nullable: false),
                    production_lead_time_days = table.Column<int>(type: "integer", nullable: false),
                    input_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runs", x => x.id);
                    table.CheckConstraint("ck_mrp_runs_lead_times", "purchase_lead_time_days BETWEEN 0 AND 365 AND production_lead_time_days BETWEEN 0 AND 365");
                    table.ForeignKey(
                        name: "FK_runs_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_runs_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_runs_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recommendations",
                schema: "mrp",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_no = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    need_by = table.Column<DateOnly>(type: "date", nullable: false),
                    order_by = table.Column<DateOnly>(type: "date", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    gross_requirement = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    stock_used = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    scheduled_receipts_used = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    reasons = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    converted_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    converted_id = table.Column<Guid>(type: "uuid", nullable: true),
                    converted_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendations", x => x.id);
                    table.CheckConstraint("ck_mrp_recommendations_action", "action IN ('buy', 'make', 'shortage')");
                    table.CheckConstraint("ck_mrp_recommendations_converted", "(status = 'converted') = (converted_id IS NOT NULL)");
                    table.CheckConstraint("ck_mrp_recommendations_quantity", "quantity > 0");
                    table.CheckConstraint("ck_mrp_recommendations_status", "status IN ('proposed', 'approved', 'rejected', 'converted')");
                    table.ForeignKey(
                        name: "FK_recommendations_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recommendations_runs_run_id",
                        column: x => x.run_id,
                        principalSchema: "mrp",
                        principalTable: "runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recommendations_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_decided_by_user_id",
                schema: "mrp",
                table: "recommendations",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_item_id",
                schema: "mrp",
                table: "recommendations",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_organization_id_status",
                schema: "mrp",
                table: "recommendations",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_run_id_line_no",
                schema: "mrp",
                table: "recommendations",
                columns: new[] { "run_id", "line_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_runs_branch_id",
                schema: "mrp",
                table: "runs",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_runs_created_by_user_id",
                schema: "mrp",
                table: "runs",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_runs_organization_id_created_at_utc",
                schema: "mrp",
                table: "runs",
                columns: new[] { "organization_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_runs_organization_id_number",
                schema: "mrp",
                table: "runs",
                columns: new[] { "organization_id", "number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recommendations",
                schema: "mrp");

            migrationBuilder.DropTable(
                name: "runs",
                schema: "mrp");
        }
    }
}
