using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationBranding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "brand_logo_file_id",
                schema: "organization",
                table: "organizations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "brand_primary_color",
                schema: "organization",
                table: "organizations",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_organizations_brand_logo_file_id",
                schema: "organization",
                table: "organizations",
                column: "brand_logo_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_organizations_uploaded_files_brand_logo_file_id",
                schema: "organization",
                table: "organizations",
                column: "brand_logo_file_id",
                principalSchema: "files",
                principalTable: "uploaded_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_organizations_uploaded_files_brand_logo_file_id",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organizations_brand_logo_file_id",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "brand_logo_file_id",
                schema: "organization",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "brand_primary_color",
                schema: "organization",
                table: "organizations");
        }
    }
}
