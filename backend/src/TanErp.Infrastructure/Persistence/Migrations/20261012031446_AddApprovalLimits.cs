using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approval_limits",
                schema: "identity_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    is_unlimited = table.Column<bool>(type: "boolean", nullable: false),
                    max_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_limits", x => x.id);
                    table.CheckConstraint("ck_approval_limits_amount", "(is_unlimited AND max_amount IS NULL) OR (NOT is_unlimited AND max_amount IS NOT NULL AND max_amount >= 0)");
                    table.CheckConstraint("ck_approval_limits_currency", "char_length(currency) = 3");
                    table.CheckConstraint("ck_approval_limits_document_type", "document_type IN ('estimate', 'purchase_order', 'change_order', 'credit_note')");
                    table.ForeignKey(
                        name: "FK_approval_limits_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_approval_limits_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_approval_limits_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "identity_access",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_approval_limits_branch_id",
                schema: "identity_access",
                table: "approval_limits",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_limits_organization_id_document_type_is_active",
                schema: "identity_access",
                table: "approval_limits",
                columns: new[] { "organization_id", "document_type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_approval_limits_role_id",
                schema: "identity_access",
                table: "approval_limits",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ux_approval_limits_branch",
                schema: "identity_access",
                table: "approval_limits",
                columns: new[] { "organization_id", "role_id", "document_type", "branch_id", "currency" },
                unique: true,
                filter: "branch_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_approval_limits_org_wide",
                schema: "identity_access",
                table: "approval_limits",
                columns: new[] { "organization_id", "role_id", "document_type", "currency" },
                unique: true,
                filter: "branch_id IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_limits",
                schema: "identity_access");
        }
    }
}
