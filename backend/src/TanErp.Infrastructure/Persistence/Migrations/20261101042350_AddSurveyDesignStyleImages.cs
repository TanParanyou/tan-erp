using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyDesignStyleImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "image_file_id",
                schema: "crm",
                table: "survey_design_styles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_file_ids",
                schema: "crm",
                table: "survey_design_styles",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_survey_design_styles_image_file_id",
                schema: "crm",
                table: "survey_design_styles",
                column: "image_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_survey_design_styles_uploaded_files_image_file_id",
                schema: "crm",
                table: "survey_design_styles",
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
                name: "FK_survey_design_styles_uploaded_files_image_file_id",
                schema: "crm",
                table: "survey_design_styles");

            migrationBuilder.DropIndex(
                name: "IX_survey_design_styles_image_file_id",
                schema: "crm",
                table: "survey_design_styles");

            migrationBuilder.DropColumn(
                name: "image_file_id",
                schema: "crm",
                table: "survey_design_styles");

            migrationBuilder.DropColumn(
                name: "image_file_ids",
                schema: "crm",
                table: "survey_design_styles");
        }
    }
}
