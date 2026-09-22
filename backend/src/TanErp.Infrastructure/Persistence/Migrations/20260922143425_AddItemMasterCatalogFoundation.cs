using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemMasterCatalogFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "item_master");

            migrationBuilder.AddColumn<string>(
                name: "content_sha256",
                schema: "files",
                table: "uploaded_files",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "height",
                schema: "files",
                table: "uploaded_files",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "scan_status",
                schema: "files",
                table: "uploaded_files",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "verified_at_utc",
                schema: "files",
                table: "uploaded_files",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "width",
                schema: "files",
                table: "uploaded_files",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "actor_membership_id",
                schema: "audit",
                table: "audit_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                schema: "audit",
                table: "audit_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                schema: "audit",
                table: "audit_events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "request_id",
                schema: "audit",
                table: "audit_events",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "row_version_after",
                schema: "audit",
                table: "audit_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "row_version_before",
                schema: "audit",
                table: "audit_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_uploaded_files_id_organization_id",
                schema: "files",
                table: "uploaded_files",
                columns: new[] { "id", "organization_id" });

            migrationBuilder.CreateTable(
                name: "item_brands",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "jsonb", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_brands", x => x.id);
                    table.UniqueConstraint("AK_item_brands_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_item_brands_sort_order", "sort_order >= 0");
                    table.CheckConstraint("CK_item_brands_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_item_brands_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_categories",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "jsonb", nullable: true),
                    parent_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    allowed_item_types = table.Column<string>(type: "jsonb", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_categories", x => x.id);
                    table.UniqueConstraint("AK_item_categories_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_item_categories_not_self_parent", "parent_category_id IS NULL OR parent_category_id != id");
                    table.CheckConstraint("CK_item_categories_sort_order", "sort_order >= 0");
                    table.CheckConstraint("CK_item_categories_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_item_categories_item_categories_parent_category_id_organiza~",
                        columns: x => new { x.parent_category_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "item_categories",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_categories_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "units",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "jsonb", nullable: false),
                    symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    dimension = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    decimal_scale = table.Column<int>(type: "integer", nullable: false),
                    rounding_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.id);
                    table.UniqueConstraint("AK_units_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_units_decimal_scale", "decimal_scale >= 0 AND decimal_scale <= 6");
                    table.CheckConstraint("CK_units_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_units_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "items",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    normalized_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "jsonb", nullable: true),
                    base_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tax_category_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    availability_mode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    attributes = table.Column<string>(type: "jsonb", nullable: true),
                    attributes_schema_version = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    activated_once = table.Column<bool>(type: "boolean", nullable: false),
                    activated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    activated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inactive_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    inactive_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inactive_reason_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    inactive_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    can_cost = table.Column<bool>(type: "boolean", nullable: false),
                    can_produce = table.Column<bool>(type: "boolean", nullable: false),
                    can_purchase = table.Column<bool>(type: "boolean", nullable: false),
                    can_sell = table.Column<bool>(type: "boolean", nullable: false),
                    can_stock = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                    table.UniqueConstraint("AK_items_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_items_active_requires_thai_name", "status != 'active' OR (name->>'th' IS NOT NULL AND length(trim(name->>'th')) > 0)");
                    table.CheckConstraint("CK_items_at_least_one_capability", "can_sell OR can_cost OR can_purchase OR can_stock OR can_produce");
                    table.CheckConstraint("CK_items_availability_mode", "availability_mode IN ('all_branches', 'selected_branches')");
                    table.CheckConstraint("CK_items_item_type", "item_type IN ('material', 'labor', 'service', 'subcontract', 'other')");
                    table.CheckConstraint("CK_items_status", "status IN ('draft', 'active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_items_item_brands_brand_id_organization_id",
                        columns: x => new { x.brand_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "item_brands",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_item_categories_category_id_organization_id",
                        columns: x => new { x.category_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "item_categories",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_units_base_unit_id_organization_id",
                        columns: x => new { x.base_unit_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_aliases",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alias = table.Column<string>(type: "jsonb", nullable: false),
                    normalized_th = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    normalized_en = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_aliases", x => x.id);
                    table.UniqueConstraint("AK_item_aliases_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_item_aliases_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_item_aliases_items_item_id_organization_id",
                        columns: x => new { x.item_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_aliases_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_branch_availabilities",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    effective_from_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    effective_to_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    inactive_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_branch_availabilities", x => x.id);
                    table.UniqueConstraint("AK_item_branch_availabilities_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_item_branch_availabilities_period", "effective_from_utc IS NULL OR effective_to_utc IS NULL OR effective_to_utc > effective_from_utc");
                    table.CheckConstraint("CK_item_branch_availabilities_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_item_branch_availabilities_branches_branch_id_organization_~",
                        columns: x => new { x.branch_id, x.organization_id },
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_branch_availabilities_items_item_id_organization_id",
                        columns: x => new { x.item_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_branch_availabilities_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_images",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    alt_text = table.Column<string>(type: "jsonb", nullable: false),
                    caption = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_images", x => x.id);
                    table.UniqueConstraint("AK_item_images_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_item_images_display_order", "display_order >= 0");
                    table.CheckConstraint("CK_item_images_role", "role IN ('primary', 'gallery', 'technical')");
                    table.CheckConstraint("CK_item_images_status", "status IN ('active', 'inactive')");
                    table.ForeignKey(
                        name: "FK_item_images_items_item_id_organization_id",
                        columns: x => new { x.item_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_images_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_images_uploaded_files_file_id_organization_id",
                        columns: x => new { x.file_id, x.organization_id },
                        principalSchema: "files",
                        principalTable: "uploaded_files",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_uploaded_files_scan_status",
                schema: "files",
                table: "uploaded_files",
                sql: "scan_status IN ('content_verified', 'clean', 'pending', 'infected')");

            migrationBuilder.CreateIndex(
                name: "IX_audit_events_organization_id_occurred_at_utc",
                schema: "audit",
                table: "audit_events",
                columns: new[] { "organization_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_events_organization_id_resource_type_resource_id_occu~",
                schema: "audit",
                table: "audit_events",
                columns: new[] { "organization_id", "resource_type", "resource_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_item_aliases_item_id_organization_id",
                schema: "item_master",
                table: "item_aliases",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_aliases_organization_id_item_id_normalized_th",
                schema: "item_master",
                table: "item_aliases",
                columns: new[] { "organization_id", "item_id", "normalized_th" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_aliases_organization_id_normalized_en",
                schema: "item_master",
                table: "item_aliases",
                columns: new[] { "organization_id", "normalized_en" });

            migrationBuilder.CreateIndex(
                name: "IX_item_aliases_organization_id_normalized_th",
                schema: "item_master",
                table: "item_aliases",
                columns: new[] { "organization_id", "normalized_th" });

            migrationBuilder.CreateIndex(
                name: "IX_item_branch_availabilities_branch_id_organization_id",
                schema: "item_master",
                table: "item_branch_availabilities",
                columns: new[] { "branch_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_branch_availabilities_item_id_organization_id",
                schema: "item_master",
                table: "item_branch_availabilities",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_branch_availabilities_organization_id_branch_id_status",
                schema: "item_master",
                table: "item_branch_availabilities",
                columns: new[] { "organization_id", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_item_branch_availabilities_organization_id_item_id_branch_id",
                schema: "item_master",
                table: "item_branch_availabilities",
                columns: new[] { "organization_id", "item_id", "branch_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_brands_organization_id_normalized_code",
                schema: "item_master",
                table: "item_brands",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_brands_organization_id_status_sort_order_id",
                schema: "item_master",
                table: "item_brands",
                columns: new[] { "organization_id", "status", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_organization_id_normalized_code",
                schema: "item_master",
                table: "item_categories",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_organization_id_parent_category_id",
                schema: "item_master",
                table: "item_categories",
                columns: new[] { "organization_id", "parent_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_organization_id_status_sort_order_id",
                schema: "item_master",
                table: "item_categories",
                columns: new[] { "organization_id", "status", "sort_order", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_categories_parent_category_id_organization_id",
                schema: "item_master",
                table: "item_categories",
                columns: new[] { "parent_category_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_images_file_id_organization_id",
                schema: "item_master",
                table: "item_images",
                columns: new[] { "file_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_images_item_id_organization_id",
                schema: "item_master",
                table: "item_images",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_item_images_organization_id_item_id_file_id",
                schema: "item_master",
                table: "item_images",
                columns: new[] { "organization_id", "item_id", "file_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_images_organization_id_item_id_status_display_order_id",
                schema: "item_master",
                table: "item_images",
                columns: new[] { "organization_id", "item_id", "status", "display_order", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_item_images_single_active_primary",
                schema: "item_master",
                table: "item_images",
                columns: new[] { "organization_id", "item_id" },
                unique: true,
                filter: "is_primary = true AND status = 'active'");

            migrationBuilder.CreateIndex(
                name: "IX_items_base_unit_id_organization_id",
                schema: "item_master",
                table: "items",
                columns: new[] { "base_unit_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_brand_id_organization_id",
                schema: "item_master",
                table: "items",
                columns: new[] { "brand_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_category_id_organization_id",
                schema: "item_master",
                table: "items",
                columns: new[] { "category_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_organization_id_base_unit_id",
                schema: "item_master",
                table: "items",
                columns: new[] { "organization_id", "base_unit_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_organization_id_brand_id",
                schema: "item_master",
                table: "items",
                columns: new[] { "organization_id", "brand_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_organization_id_category_id",
                schema: "item_master",
                table: "items",
                columns: new[] { "organization_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_items_organization_id_normalized_code",
                schema: "item_master",
                table: "items",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_organization_id_status_normalized_code",
                schema: "item_master",
                table: "items",
                columns: new[] { "organization_id", "status", "normalized_code" });

            migrationBuilder.CreateIndex(
                name: "IX_units_organization_id_normalized_code",
                schema: "item_master",
                table: "units",
                columns: new[] { "organization_id", "normalized_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_units_organization_id_status",
                schema: "item_master",
                table: "units",
                columns: new[] { "organization_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_aliases",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "item_branch_availabilities",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "item_images",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "items",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "item_brands",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "item_categories",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "units",
                schema: "item_master");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_uploaded_files_id_organization_id",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropCheckConstraint(
                name: "CK_uploaded_files_scan_status",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropIndex(
                name: "IX_audit_events_organization_id_occurred_at_utc",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropIndex(
                name: "IX_audit_events_organization_id_resource_type_resource_id_occu~",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropColumn(
                name: "content_sha256",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropColumn(
                name: "height",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropColumn(
                name: "scan_status",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropColumn(
                name: "verified_at_utc",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropColumn(
                name: "width",
                schema: "files",
                table: "uploaded_files");

            migrationBuilder.DropColumn(
                name: "actor_membership_id",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropColumn(
                name: "branch_id",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropColumn(
                name: "reason",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropColumn(
                name: "request_id",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropColumn(
                name: "row_version_after",
                schema: "audit",
                table: "audit_events");

            migrationBuilder.DropColumn(
                name: "row_version_before",
                schema: "audit",
                table: "audit_events");
        }
    }
}
