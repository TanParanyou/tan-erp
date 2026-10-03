using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CustomerManagementCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "customer_billing_snapshot_hash", schema: "commercial", table: "quotations", type: "character varying(128)", maxLength: 128, nullable: true);
            migrationBuilder.AddColumn<string>(name: "customer_billing_snapshot_json", schema: "commercial", table: "quotations", type: "jsonb", nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "billing_cycle",
                schema: "crm",
                table: "customers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "billing_day",
                schema: "crm",
                table: "customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "branch_code",
                schema: "crm",
                table: "customers",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "credit_limit",
                schema: "crm",
                table: "customers",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "credit_term_days",
                schema: "crm",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "currency_code",
                schema: "crm",
                table: "customers",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "THB");

            migrationBuilder.AddColumn<string>(
                name: "inactive_reason",
                schema: "crm",
                table: "customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "legal_name",
                schema: "crm",
                table: "customers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_tax_identifier",
                schema: "crm",
                table: "customers",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_condition_note",
                schema: "crm",
                table: "customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tax_identifier",
                schema: "crm",
                table: "customers",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "row_version",
                schema: "crm",
                table: "customer_contacts",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddCheckConstraint(name: "ck_customers_credit_term_days", schema: "crm", table: "customers", sql: "credit_term_days >= 0");
            migrationBuilder.AddCheckConstraint(name: "ck_customers_credit_limit", schema: "crm", table: "customers", sql: "credit_limit IS NULL OR credit_limit >= 0");
            migrationBuilder.AddCheckConstraint(name: "ck_customers_billing_day", schema: "crm", table: "customers", sql: "billing_day IS NULL OR billing_day BETWEEN 1 AND 31");

            migrationBuilder.CreateTable(
                name: "customer_addresses",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_line1 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    subdistrict = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_addresses", x => x.id);
                    table.UniqueConstraint("AK_customer_addresses_id_organization_id", x => new { x.id, x.organization_id });
                    table.CheckConstraint("ck_customer_addresses_postal_code", "postal_code ~ '^[0-9]{5}$'");
                    table.CheckConstraint("ck_customer_addresses_status", "status IN ('active', 'inactive')");
                    table.CheckConstraint("ck_customer_addresses_type", "address_type IN ('billing', 'contact')");
                    table.ForeignKey(
                        name: "FK_customer_addresses_customers_customer_id_organization_id",
                        columns: x => new { x.customer_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_addresses_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_organization_id_normalized_tax_identifier_branch_~",
                schema: "crm",
                table: "customers",
                columns: new[] { "organization_id", "normalized_tax_identifier", "branch_code" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_addresses_customer_id",
                schema: "crm",
                table: "customer_addresses",
                column: "customer_id",
                unique: true,
                filter: "status = 'active' AND is_primary = true AND address_type = 'billing'");

            migrationBuilder.CreateIndex(
                name: "IX_customer_addresses_customer_id_organization_id",
                schema: "crm",
                table: "customer_addresses",
                columns: new[] { "customer_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_addresses_organization_id",
                schema: "crm",
                table: "customer_addresses",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "customer_billing_snapshot_hash", schema: "commercial", table: "quotations");
            migrationBuilder.DropColumn(name: "customer_billing_snapshot_json", schema: "commercial", table: "quotations");
            migrationBuilder.DropTable(
                name: "customer_addresses",
                schema: "crm");

            migrationBuilder.DropIndex(
                name: "IX_customers_organization_id_normalized_tax_identifier_branch_~",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropCheckConstraint(name: "ck_customers_credit_term_days", schema: "crm", table: "customers");
            migrationBuilder.DropCheckConstraint(name: "ck_customers_credit_limit", schema: "crm", table: "customers");
            migrationBuilder.DropCheckConstraint(name: "ck_customers_billing_day", schema: "crm", table: "customers");

            migrationBuilder.DropColumn(
                name: "billing_cycle",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "billing_day",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "branch_code",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "credit_limit",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "credit_term_days",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "currency_code",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "inactive_reason",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "legal_name",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "normalized_tax_identifier",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "payment_condition_note",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "tax_identifier",
                schema: "crm",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "crm",
                table: "customer_contacts");
        }
    }
}
