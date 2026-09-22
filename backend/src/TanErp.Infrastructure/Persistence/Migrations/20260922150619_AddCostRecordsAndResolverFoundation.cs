using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCostRecordsAndResolverFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cost_sources",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "jsonb", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_sources", x => x.id);
                    table.UniqueConstraint("AK_cost_sources_id_organization_id", x => new { x.id, x.organization_id });
                    table.ForeignKey(
                        name: "FK_cost_sources_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cost_records",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    minimum_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    maximum_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    effective_from_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    effective_to_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    cost_source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    evidence_file_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_financial_editor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_records", x => x.id);
                    table.UniqueConstraint("AK_cost_records_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_cost_records_amount", "amount > 0");
                    table.CheckConstraint("CK_cost_records_effective_period", "effective_to_utc IS NULL OR effective_to_utc >= effective_from_utc");
                    table.CheckConstraint("CK_cost_records_maximum_quantity", "maximum_quantity IS NULL OR maximum_quantity > minimum_quantity");
                    table.CheckConstraint("CK_cost_records_minimum_quantity", "minimum_quantity >= 0");
                    table.CheckConstraint("CK_cost_records_scope", "scope IN ('organization', 'branch')");
                    table.CheckConstraint("CK_cost_records_status", "status IN ('draft', 'submitted', 'returned', 'approved', 'published', 'superseded', 'disabled')");
                    table.ForeignKey(
                        name: "FK_cost_records_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cost_records_cost_sources_cost_source_id_organization_id",
                        columns: x => new { x.cost_source_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "cost_sources",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cost_records_items_item_id_organization_id",
                        columns: x => new { x.item_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cost_records_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cost_records_units_unit_id_organization_id",
                        columns: x => new { x.unit_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cost_record_reviews",
                schema: "item_master",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cost_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reviewer_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    authority_snapshot = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_record_reviews", x => x.id);
                    table.UniqueConstraint("AK_cost_record_reviews_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("CK_cost_record_reviews_decision", "decision IN ('approved', 'returned')");
                    table.ForeignKey(
                        name: "FK_cost_record_reviews_cost_records_cost_record_id_organizatio~",
                        columns: x => new { x.cost_record_id, x.organization_id },
                        principalSchema: "item_master",
                        principalTable: "cost_records",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cost_record_reviews_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cost_record_reviews_cost_record_id_organization_id",
                schema: "item_master",
                table: "cost_record_reviews",
                columns: new[] { "cost_record_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_record_reviews_organization_id_cost_record_id_decided_~",
                schema: "item_master",
                table: "cost_record_reviews",
                columns: new[] { "organization_id", "cost_record_id", "decided_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_branch_id",
                schema: "item_master",
                table: "cost_records",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_cost_source_id_organization_id",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "cost_source_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_item_id_organization_id",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "item_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_organization_id_item_id_scope_branch_id_status",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "organization_id", "item_id", "scope", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_organization_id_item_id_status_currency_unit_i~",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "organization_id", "item_id", "status", "currency", "unit_id", "effective_from_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_records_unit_id_organization_id",
                schema: "item_master",
                table: "cost_records",
                columns: new[] { "unit_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_sources_organization_id_code",
                schema: "item_master",
                table: "cost_sources",
                columns: new[] { "organization_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cost_record_reviews",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "cost_records",
                schema: "item_master");

            migrationBuilder.DropTable(
                name: "cost_sources",
                schema: "item_master");
        }
    }
}
