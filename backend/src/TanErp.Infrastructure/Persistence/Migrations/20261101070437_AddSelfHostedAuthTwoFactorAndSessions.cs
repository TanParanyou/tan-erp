using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSelfHostedAuthTwoFactorAndSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "failed_login_count",
                schema: "identity_access",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "google_subject_id",
                schema: "identity_access",
                table: "users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_totp_enabled",
                schema: "identity_access",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lockout_end_utc",
                schema: "identity_access",
                table: "users",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "password_changed_at_utc",
                schema: "identity_access",
                table: "users",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                schema: "identity_access",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pending_totp_secret_encrypted",
                schema: "identity_access",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "require_two_factor",
                schema: "identity_access",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "totp_enabled_at_utc",
                schema: "identity_access",
                table: "users",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "totp_secret_encrypted",
                schema: "identity_access",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_backup_codes",
                schema: "identity_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    used_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_backup_codes", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_backup_codes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                schema: "identity_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_refresh_token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    previous_refresh_token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    last_rotated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    grace_period_until_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    client_platform = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    device_label = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    last_used_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    expires_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    revoked_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    revoke_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sessions", x => x.id);
                    table.CheckConstraint("ck_user_sessions_client_platform", "client_platform IN ('web', 'mobile')");
                    table.ForeignKey(
                        name: "FK_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_google_subject_id",
                schema: "identity_access",
                table: "users",
                column: "google_subject_id",
                unique: true,
                filter: "google_subject_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_backup_codes_user_id",
                schema: "identity_access",
                table: "user_backup_codes",
                columns: new[] { "user_id", "used_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_active",
                schema: "identity_access",
                table: "user_sessions",
                columns: new[] { "user_id", "revoked_at_utc", "expires_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_backup_codes",
                schema: "identity_access");

            migrationBuilder.DropTable(
                name: "user_sessions",
                schema: "identity_access");

            migrationBuilder.DropIndex(
                name: "ix_users_google_subject_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "failed_login_count",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "google_subject_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_totp_enabled",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "lockout_end_utc",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_changed_at_utc",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "pending_totp_secret_encrypted",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "require_two_factor",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "totp_enabled_at_utc",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "totp_secret_encrypted",
                schema: "identity_access",
                table: "users");
        }
    }
}
