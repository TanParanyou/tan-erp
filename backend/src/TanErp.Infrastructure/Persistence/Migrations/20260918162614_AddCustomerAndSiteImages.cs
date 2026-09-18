using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerAndSiteImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                schema: "crm",
                table: "customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "site_images",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_images", x => x.id);
                    table.CheckConstraint("CK_site_images_display_order", "display_order >= 0");
                    table.ForeignKey(
                        name: "FK_site_images_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_images_sites_site_id_organization_id",
                        columns: x => new { x.site_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "sites",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_images_uploaded_files_file_id",
                        column: x => x.file_id,
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_site_images_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_image_file_id",
                schema: "crm",
                table: "customers",
                column: "image_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_images_created_by_user_id",
                schema: "crm",
                table: "site_images",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_images_file_id",
                schema: "crm",
                table: "site_images",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_images_org_site_deleted_created_order",
                schema: "crm",
                table: "site_images",
                columns: new[] { "organization_id", "site_id", "is_deleted", "created_at_utc", "display_order", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_site_images_site_id_organization_id",
                schema: "crm",
                table: "site_images",
                columns: new[] { "site_id", "organization_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_customers_uploaded_files_image_file_id",
                schema: "crm",
                table: "customers",
                column: "image_file_id",
                principalSchema: "files",
                principalTable: "uploaded_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customers_uploaded_files_image_file_id",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropTable(
                name: "site_images",
                schema: "crm");

            migrationBuilder.DropIndex(
                name: "IX_customers_image_file_id",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                schema: "crm",
                table: "customers");
        }
    }
}
