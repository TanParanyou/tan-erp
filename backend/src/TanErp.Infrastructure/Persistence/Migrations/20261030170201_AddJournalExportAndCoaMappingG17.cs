using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalExportAndCoaMappingG17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_accounting_outbox_status",
                schema: "finance",
                table: "accounting_outbox");

            migrationBuilder.CreateTable(
                name: "account_mappings",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    debit_account_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    debit_account_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    credit_account_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    credit_account_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_mappings", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_mappings_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_mappings_users_updated_by_user_id",
                        column: x => x.updated_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_accounting_outbox_status",
                schema: "finance",
                table: "accounting_outbox",
                sql: "status IN ('pending', 'sent', 'exported', 'failed', 'dead')");

            migrationBuilder.CreateIndex(
                name: "IX_account_mappings_organization_id_kind",
                schema: "finance",
                table: "account_mappings",
                columns: new[] { "organization_id", "kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_mappings_updated_by_user_id",
                schema: "finance",
                table: "account_mappings",
                column: "updated_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_mappings",
                schema: "finance");

            migrationBuilder.DropCheckConstraint(
                name: "ck_accounting_outbox_status",
                schema: "finance",
                table: "accounting_outbox");

            migrationBuilder.AddCheckConstraint(
                name: "ck_accounting_outbox_status",
                schema: "finance",
                table: "accounting_outbox",
                sql: "status IN ('pending', 'sent', 'failed', 'dead')");
        }
    }
}
