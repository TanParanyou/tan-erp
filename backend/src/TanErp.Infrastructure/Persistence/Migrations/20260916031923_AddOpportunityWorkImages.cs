using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOpportunityWorkImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "opportunity_work_images",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stage_at_attach = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunity_work_images", x => x.id);
                    table.CheckConstraint("CK_opportunity_work_images_display_order", "display_order >= 0");
                    table.ForeignKey(
                        name: "FK_opportunity_work_images_opportunities_opportunity_id_organi~",
                        columns: x => new { x.opportunity_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunity_work_images_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunity_work_images_uploaded_files_file_id",
                        column: x => x.file_id,
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_opportunity_work_images_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_work_images_created_by_user_id",
                schema: "crm",
                table: "opportunity_work_images",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_work_images_file_id",
                schema: "crm",
                table: "opportunity_work_images",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_work_images_opportunity_id_organization_id",
                schema: "crm",
                table: "opportunity_work_images",
                columns: new[] { "opportunity_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_work_images_org_opp_deleted_created_order",
                schema: "crm",
                table: "opportunity_work_images",
                columns: new[] { "organization_id", "opportunity_id", "is_deleted", "created_at_utc", "display_order", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_work_images_org_opp_stage_deleted",
                schema: "crm",
                table: "opportunity_work_images",
                columns: new[] { "organization_id", "opportunity_id", "stage_at_attach", "is_deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opportunity_work_images",
                schema: "crm");
        }
    }
}
