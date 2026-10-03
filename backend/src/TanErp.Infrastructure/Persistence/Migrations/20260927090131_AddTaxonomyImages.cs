using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxonomyImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                schema: "item_master",
                table: "item_categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                schema: "item_master",
                table: "item_brands",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_image_file_id",
                schema: "item_master",
                table: "item_categories",
                column: "image_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_brands_image_file_id",
                schema: "item_master",
                table: "item_brands",
                column: "image_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_item_brands_uploaded_files_image_file_id",
                schema: "item_master",
                table: "item_brands",
                column: "image_file_id",
                principalSchema: "files",
                principalTable: "uploaded_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_item_categories_uploaded_files_image_file_id",
                schema: "item_master",
                table: "item_categories",
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
                name: "FK_item_brands_uploaded_files_image_file_id",
                schema: "item_master",
                table: "item_brands");

            migrationBuilder.DropForeignKey(
                name: "FK_item_categories_uploaded_files_image_file_id",
                schema: "item_master",
                table: "item_categories");

            migrationBuilder.DropIndex(
                name: "IX_item_categories_image_file_id",
                schema: "item_master",
                table: "item_categories");

            migrationBuilder.DropIndex(
                name: "IX_item_brands_image_file_id",
                schema: "item_master",
                table: "item_brands");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                schema: "item_master",
                table: "item_categories");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                schema: "item_master",
                table: "item_brands");
        }
    }
}
