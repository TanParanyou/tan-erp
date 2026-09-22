using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HardenItemCatalogTenantAndJsonConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_estimate_cost_components_cost_records_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropForeignKey(
                name: "FK_estimate_cost_components_items_item_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_item_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_description_shape",
                schema: "item_master",
                table: "items",
                sql: "description IS NULL OR (jsonb_typeof(description) = 'object' AND (description - 'th' - 'en') = '{}'::jsonb)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_items_name_shape",
                schema: "item_master",
                table: "items",
                sql: "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_images_alt_text_shape",
                schema: "item_master",
                table: "item_images",
                sql: "jsonb_typeof(alt_text) = 'object' AND (alt_text - 'th' - 'en') = '{}'::jsonb");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_images_caption_shape",
                schema: "item_master",
                table: "item_images",
                sql: "caption IS NULL OR (jsonb_typeof(caption) = 'object' AND (caption - 'th' - 'en') = '{}'::jsonb)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_categories_description_shape",
                schema: "item_master",
                table: "item_categories",
                sql: "description IS NULL OR (jsonb_typeof(description) = 'object' AND (description - 'th' - 'en') = '{}'::jsonb)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_categories_name_shape",
                schema: "item_master",
                table: "item_categories",
                sql: "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_brands_description_shape",
                schema: "item_master",
                table: "item_brands",
                sql: "description IS NULL OR (jsonb_typeof(description) = 'object' AND (description - 'th' - 'en') = '{}'::jsonb)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_brands_name_shape",
                schema: "item_master",
                table: "item_brands",
                sql: "jsonb_typeof(name) = 'object' AND (name - 'th' - 'en') = '{}'::jsonb");

            migrationBuilder.CreateIndex(
                name: "ix_item_aliases_org_item_normalized_en",
                schema: "item_master",
                table: "item_aliases",
                columns: new[] { "organization_id", "item_id", "normalized_en" },
                unique: true,
                filter: "normalized_en IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_item_aliases_alias_shape",
                schema: "item_master",
                table: "item_aliases",
                sql: "jsonb_typeof(alias) = 'object' AND (alias - 'th' - 'en') = '{}'::jsonb");

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_cost_record_id_organization_id",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "cost_record_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_item_id_organization_id",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_evidence_file_id_organization_id",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "evidence_file_id", "organization_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_cost_records_uploaded_files_evidence_file_id_organization_id",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "evidence_file_id", "organization_id" },
                principalSchema: "files",
                principalTable: "uploaded_files",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_cost_components_cost_records_cost_record_id_organi~",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "cost_record_id", "organization_id" },
                principalSchema: "item_master",
                principalTable: "cost_records",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_cost_components_items_item_id_organization_id",
                schema: "estimates",
                table: "estimate_cost_components",
                columns: new[] { "item_id", "organization_id" },
                principalSchema: "item_master",
                principalTable: "items",
                principalColumns: new[] { "id", "organization_id" },
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cost_records_uploaded_files_evidence_file_id_organization_id",
                schema: "item_master",
                table: "cost_records");

            migrationBuilder.DropForeignKey(
                name: "FK_estimate_cost_components_cost_records_cost_record_id_organi~",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropForeignKey(
                name: "FK_estimate_cost_components_items_item_id_organization_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropCheckConstraint(
                name: "CK_items_description_shape",
                schema: "item_master",
                table: "items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_items_name_shape",
                schema: "item_master",
                table: "items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_images_alt_text_shape",
                schema: "item_master",
                table: "item_images");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_images_caption_shape",
                schema: "item_master",
                table: "item_images");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_categories_description_shape",
                schema: "item_master",
                table: "item_categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_categories_name_shape",
                schema: "item_master",
                table: "item_categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_brands_description_shape",
                schema: "item_master",
                table: "item_brands");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_brands_name_shape",
                schema: "item_master",
                table: "item_brands");

            migrationBuilder.DropIndex(
                name: "ix_item_aliases_org_item_normalized_en",
                schema: "item_master",
                table: "item_aliases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_item_aliases_alias_shape",
                schema: "item_master",
                table: "item_aliases");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_cost_record_id_organization_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_estimate_cost_components_item_id_organization_id",
                schema: "estimates",
                table: "estimate_cost_components");

            migrationBuilder.DropIndex(
                name: "IX_cost_records_evidence_file_id_organization_id",
                schema: "item_master",
                table: "cost_records");

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "cost_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_estimate_cost_components_item_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "item_id");

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_cost_components_cost_records_cost_record_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "cost_record_id",
                principalSchema: "item_master",
                principalTable: "cost_records",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_estimate_cost_components_items_item_id",
                schema: "estimates",
                table: "estimate_cost_components",
                column: "item_id",
                principalSchema: "item_master",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
