using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserThemePreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "density",
                schema: "identity_access",
                table: "users",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "comfortable");

            migrationBuilder.AddColumn<int>(
                name: "text_scale",
                schema: "identity_access",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<string>(
                name: "theme_mode",
                schema: "identity_access",
                table: "users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "system");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_density",
                schema: "identity_access",
                table: "users",
                sql: "density IN ('comfortable', 'compact')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_text_scale",
                schema: "identity_access",
                table: "users",
                sql: "text_scale BETWEEN 90 AND 130");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_theme_mode",
                schema: "identity_access",
                table: "users",
                sql: "theme_mode IN ('light', 'dark', 'system')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_users_density",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_text_scale",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_theme_mode",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "density",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "text_scale",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "theme_mode",
                schema: "identity_access",
                table: "users");
        }
    }
}
