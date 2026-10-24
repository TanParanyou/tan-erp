using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExtendedFieldsAndMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "avatar_file_id",
                schema: "identity_access",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "department_id",
                schema: "identity_access",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "employee_code",
                schema: "identity_access",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "job_title_id",
                schema: "identity_access",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "identity_access",
                table: "users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                schema: "identity_access",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.id);
                    table.ForeignKey(
                        name: "FK_departments_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_titles",
                schema: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_titles", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_titles_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "organization",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_job_titles_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_avatar_file_id",
                schema: "identity_access",
                table: "users",
                column: "avatar_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_department_id",
                schema: "identity_access",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_employee_code",
                schema: "identity_access",
                table: "users",
                column: "employee_code",
                filter: "employee_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_job_title_id",
                schema: "identity_access",
                table: "users",
                column: "job_title_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_organization_id_code",
                schema: "organization",
                table: "departments",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_job_titles_department_id",
                schema: "organization",
                table: "job_titles",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_titles_organization_id_code",
                schema: "organization",
                table: "job_titles",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_departments_department_id",
                schema: "identity_access",
                table: "users",
                column: "department_id",
                principalSchema: "organization",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_files_avatar_file_id",
                schema: "identity_access",
                table: "users",
                column: "avatar_file_id",
                principalSchema: "files",
                principalTable: "uploaded_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_job_titles_job_title_id",
                schema: "identity_access",
                table: "users",
                column: "job_title_id",
                principalSchema: "organization",
                principalTable: "job_titles",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_departments_department_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_files_avatar_file_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_job_titles_job_title_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropTable(
                name: "job_titles",
                schema: "organization");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "organization");

            migrationBuilder.DropIndex(
                name: "IX_users_avatar_file_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_department_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_employee_code",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_job_title_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "avatar_file_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "department_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "employee_code",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "job_title_id",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "identity_access",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone_number",
                schema: "identity_access",
                table: "users");
        }
    }
}
