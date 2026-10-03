using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityAdministration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_firebase_uid",
                schema: "identity_access",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "firebase_uid",
                schema: "identity_access",
                table: "users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                schema: "identity_access",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "row_version",
                schema: "identity_access",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "row_version",
                schema: "organization",
                table: "memberships",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Backfill existing rows: unique non-empty row versions and normalized emails before the unique index is built.
            migrationBuilder.Sql("UPDATE identity_access.users SET normalized_email = lower(btrim(email)), row_version = gen_random_uuid();");
            migrationBuilder.Sql("UPDATE organization.memberships SET row_version = gen_random_uuid();");

            migrationBuilder.CreateTable(
                name: "role_assignment_requests",
                schema: "identity_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_assignment_requests", x => x.id);
                    table.CheckConstraint("ck_role_assignment_requests_independent_checker", "decided_by_user_id IS NULL OR status = 'cancelled' OR decided_by_user_id <> requested_by_user_id");
                    table.CheckConstraint("ck_role_assignment_requests_status", "status IN ('pending', 'approved', 'rejected', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_role_assignment_requests_memberships_membership_id",
                        column: x => x.membership_id,
                        principalSchema: "organization",
                        principalTable: "memberships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_assignment_requests_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_assignment_requests_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "identity_access",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_assignment_requests_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_assignment_requests_users_requested_by_user_id",
                        column: x => x.requested_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_firebase_uid",
                schema: "identity_access",
                table: "users",
                column: "firebase_uid",
                unique: true,
                filter: "firebase_uid IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_normalized_email",
                schema: "identity_access",
                table: "users",
                column: "normalized_email",
                unique: true,
                filter: "normalized_email <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_role_assignment_requests_decided_by_user_id",
                schema: "identity_access",
                table: "role_assignment_requests",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_assignment_requests_organization_id_status",
                schema: "identity_access",
                table: "role_assignment_requests",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_role_assignment_requests_requested_by_user_id",
                schema: "identity_access",
                table: "role_assignment_requests",
                column: "requested_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_assignment_requests_role_id",
                schema: "identity_access",
                table: "role_assignment_requests",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ux_role_assignment_requests_pending",
                schema: "identity_access",
                table: "role_assignment_requests",
                columns: new[] { "membership_id", "role_id" },
                unique: true,
                filter: "status = 'pending'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_assignment_requests",
                schema: "identity_access");

            // Pending users have no Firebase UID; give them a unique placeholder before the column becomes required again.
            migrationBuilder.Sql("UPDATE identity_access.users SET firebase_uid = 'pending-' || id::text WHERE firebase_uid IS NULL;");

            migrationBuilder.DropIndex(
                name: "ix_users_firebase_uid",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_normalized_email",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "normalized_email",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "organization",
                table: "memberships");

            migrationBuilder.AlterColumn<string>(
                name: "firebase_uid",
                schema: "identity_access",
                table: "users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_firebase_uid",
                schema: "identity_access",
                table: "users",
                column: "firebase_uid",
                unique: true);
        }
    }
}
