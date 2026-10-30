using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditExposureAndArAgingG16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credit_limit_overrides",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_ceiling_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expires_on = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_limit_overrides", x => x.id);
                    table.CheckConstraint("ck_credit_limit_overrides_requested_ceiling", "requested_ceiling_amount > 0");
                    table.CheckConstraint("ck_credit_limit_overrides_status", "status IN ('pending_approval', 'approved', 'rejected')");
                    table.ForeignKey(
                        name: "FK_credit_limit_overrides_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credit_limit_overrides_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credit_limit_overrides_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credit_limit_overrides_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credit_limit_overrides_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_credit_limit_overrides_branch_id",
                schema: "finance",
                table: "credit_limit_overrides",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_credit_limit_overrides_created_by_user_id",
                schema: "finance",
                table: "credit_limit_overrides",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_credit_limit_overrides_customer_id",
                schema: "finance",
                table: "credit_limit_overrides",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_credit_limit_overrides_decided_by_user_id",
                schema: "finance",
                table: "credit_limit_overrides",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_credit_limit_overrides_organization_id_customer_id_status",
                schema: "finance",
                table: "credit_limit_overrides",
                columns: new[] { "organization_id", "customer_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credit_limit_overrides",
                schema: "finance");
        }
    }
}
