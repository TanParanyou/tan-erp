using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                schema: "identity_access",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "row_version",
                schema: "identity_access",
                table: "roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "role_change_requests",
                schema: "identity_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    base_role_row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    proposed_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    proposed_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    proposed_grants = table.Column<string>(type: "jsonb", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_change_requests", x => x.id);
                    table.CheckConstraint("ck_role_change_requests_independent_checker", "decided_by_user_id IS NULL OR status = 'cancelled' OR decided_by_user_id <> requested_by_user_id");
                    table.CheckConstraint("ck_role_change_requests_status", "status IN ('pending', 'approved', 'rejected', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_role_change_requests_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_change_requests_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "identity_access",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_change_requests_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_change_requests_users_requested_by_user_id",
                        column: x => x.requested_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_change_requests_decided_by_user_id",
                schema: "identity_access",
                table: "role_change_requests",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_change_requests_organization_id_status",
                schema: "identity_access",
                table: "role_change_requests",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_role_change_requests_requested_by_user_id",
                schema: "identity_access",
                table: "role_change_requests",
                column: "requested_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_role_change_requests_pending",
                schema: "identity_access",
                table: "role_change_requests",
                column: "role_id",
                unique: true,
                filter: "status = 'pending'");

            migrationBuilder.Sql("UPDATE identity_access.roles SET row_version = gen_random_uuid();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_change_requests",
                schema: "identity_access");

            migrationBuilder.DropColumn(
                name: "is_system",
                schema: "identity_access",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "identity_access",
                table: "roles");
        }
    }
}
