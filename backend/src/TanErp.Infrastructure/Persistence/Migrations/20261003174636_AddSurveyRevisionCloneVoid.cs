using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyRevisionCloneVoid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clone_reason",
                schema: "crm",
                table: "site_survey_revisions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_revision_id",
                schema: "crm",
                table: "site_survey_revisions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "void_reason",
                schema: "crm",
                table: "site_survey_revisions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "voided_at_utc",
                schema: "crm",
                table: "site_survey_revisions",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "voided_by_user_id",
                schema: "crm",
                table: "site_survey_revisions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_source_revision_id",
                schema: "crm",
                table: "site_survey_revisions",
                column: "source_revision_id");

            migrationBuilder.CreateIndex(
                name: "IX_site_survey_revisions_voided_by_user_id",
                schema: "crm",
                table: "site_survey_revisions",
                column: "voided_by_user_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_site_survey_revisions_void_reason",
                schema: "crm",
                table: "site_survey_revisions",
                sql: "status <> 'void' OR (void_reason IS NOT NULL AND voided_at_utc IS NOT NULL AND voided_by_user_id IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_site_survey_revisions_site_survey_revisions_source_revision~",
                schema: "crm",
                table: "site_survey_revisions",
                column: "source_revision_id",
                principalSchema: "crm",
                principalTable: "site_survey_revisions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_site_survey_revisions_users_voided_by_user_id",
                schema: "crm",
                table: "site_survey_revisions",
                column: "voided_by_user_id",
                principalSchema: "identity_access",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_site_survey_revisions_site_survey_revisions_source_revision~",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropForeignKey(
                name: "FK_site_survey_revisions_users_voided_by_user_id",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropIndex(
                name: "IX_site_survey_revisions_source_revision_id",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropIndex(
                name: "IX_site_survey_revisions_voided_by_user_id",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_site_survey_revisions_void_reason",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropColumn(
                name: "clone_reason",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropColumn(
                name: "source_revision_id",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropColumn(
                name: "void_reason",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropColumn(
                name: "voided_at_utc",
                schema: "crm",
                table: "site_survey_revisions");

            migrationBuilder.DropColumn(
                name: "voided_by_user_id",
                schema: "crm",
                table: "site_survey_revisions");
        }
    }
}
