using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CustomerContactSlice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "crm");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    customer_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    display_name_th = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    display_name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    normalized_display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    preferred_locale = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                    table.UniqueConstraint("AK_customers_id_organization_id", x => new { x.id, x.organization_id });
                    table.ForeignKey(
                        name: "FK_customers_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "idempotency_records",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    key_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    payload_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    resource_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotency_records", x => x.id);
                    table.CheckConstraint("CK_idempotency_records_hashes_not_empty", "payload_hash <> '' AND resource_id <> ''");
                    table.ForeignKey(
                        name: "FK_idempotency_records_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_contacts",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    normalized_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    preferred_channel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_contacts", x => x.id);
                    table.CheckConstraint("CK_customer_contacts_phone_or_email", "normalized_phone IS NOT NULL OR normalized_email IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_customer_contacts_customers_customer_id_organization_id",
                        columns: x => new { x.customer_id, x.organization_id },
                        principalSchema: "crm",
                        principalTable: "customers",
                        principalColumns: new[] { "id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_contacts_customer_id",
                schema: "crm",
                table: "customer_contacts",
                column: "customer_id",
                unique: true,
                filter: "is_primary = true AND status = 'active'");

            migrationBuilder.CreateIndex(
                name: "IX_customer_contacts_customer_id_organization_id",
                schema: "crm",
                table: "customer_contacts",
                columns: new[] { "customer_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_contacts_organization_id_normalized_email",
                schema: "crm",
                table: "customer_contacts",
                columns: new[] { "organization_id", "normalized_email" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_contacts_organization_id_normalized_phone",
                schema: "crm",
                table: "customer_contacts",
                columns: new[] { "organization_id", "normalized_phone" });

            migrationBuilder.CreateIndex(
                name: "IX_customers_organization_id_code",
                schema: "crm",
                table: "customers",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_organization_id_status_normalized_display_name_id",
                schema: "crm",
                table: "customers",
                columns: new[] { "organization_id", "status", "normalized_display_name", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_idempotency_records_organization_id_operation_key_hash",
                schema: "audit",
                table: "idempotency_records",
                columns: new[] { "organization_id", "operation", "key_hash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_contacts",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "idempotency_records",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "crm");
        }
    }
}
