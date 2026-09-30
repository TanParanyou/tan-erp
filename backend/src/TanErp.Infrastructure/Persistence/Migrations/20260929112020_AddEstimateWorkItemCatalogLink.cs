using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateWorkItemCatalogLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "item_code_snapshot",
                schema: "estimates",
                table: "estimate_work_items",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "item_id",
                schema: "estimates",
                table: "estimate_work_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item_name_en_snapshot",
                schema: "estimates",
                table: "estimate_work_items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item_name_th_snapshot",
                schema: "estimates",
                table: "estimate_work_items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "override_reason",
                schema: "estimates",
                table: "estimate_work_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "override_reason_code",
                schema: "estimates",
                table: "estimate_work_items",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_estimate_work_items_item_id_organization_id",
                schema: "estimates",
                table: "estimate_work_items",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_work_items_items_item_id_organization_id",
                schema: "estimates",
                table: "estimate_work_items",
                columns: new[] { "item_id", "organization_id" },
                principalSchema: "item_master",
                principalTable: "items",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimate_work_items_items_item_id_organization_id",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropIndex(
                name: "IX_estimate_work_items_item_id_organization_id",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropColumn(
                name: "item_code_snapshot",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropColumn(
                name: "item_id",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropColumn(
                name: "item_name_en_snapshot",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropColumn(
                name: "item_name_th_snapshot",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropColumn(
                name: "override_reason",
                schema: "estimates",
                table: "estimate_work_items");

            migrationBuilder.DropColumn(
                name: "override_reason_code",
                schema: "estimates",
                table: "estimate_work_items");
        }
    }
}
