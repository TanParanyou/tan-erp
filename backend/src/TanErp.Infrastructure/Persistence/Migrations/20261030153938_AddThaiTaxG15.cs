using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThaiTaxG15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_billing_documents_amounts",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox");

            migrationBuilder.AddColumn<decimal>(
                name: "net_cash_received",
                schema: "finance",
                table: "payments",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "receipt_number",
                schema: "finance",
                table: "payments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "wht_amount",
                schema: "finance",
                table: "payments",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "wht_certificate_number",
                schema: "finance",
                table: "payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "wht_rate",
                schema: "finance",
                table: "payments",
                type: "numeric(6,4)",
                precision: 6,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "wht_type",
                schema: "finance",
                table: "payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "credit_note_base_amount",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "credit_note_vat_amount",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "debit_note_base_amount",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "debit_note_vat_amount",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "released_retention_at_utc",
                schema: "finance",
                table: "billing_documents",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "released_retention_by_user_id",
                schema: "finance",
                table: "billing_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "retention_amount",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "retention_rate",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(6,4)",
                precision: 6,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "retention_status",
                schema: "finance",
                table: "billing_documents",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "none");

            migrationBuilder.AddColumn<string>(
                name: "tax_invoice_number",
                schema: "finance",
                table: "billing_documents",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_amount",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_rate",
                schema: "finance",
                table: "billing_documents",
                type: "numeric(6,4)",
                precision: 6,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "billing_adjustment_notes",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    note_type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tax_base_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    vat_rate = table.Column<decimal>(type: "numeric(6,4)", precision: 6, scale: 4, nullable: false),
                    vat_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_adjustment_notes", x => x.id);
                    table.CheckConstraint("ck_billing_adjustment_notes_amounts", "tax_base_delta > 0 AND vat_rate >= 0 AND vat_delta >= 0 AND total_delta >= tax_base_delta");
                    table.CheckConstraint("ck_billing_adjustment_notes_status", "status IN ('pending_approval', 'approved', 'rejected', 'voided')");
                    table.CheckConstraint("ck_billing_adjustment_notes_type", "note_type IN ('credit', 'debit')");
                    table.ForeignKey(
                        name: "FK_billing_adjustment_notes_billing_documents_billing_document~",
                        column: x => x.billing_document_id,
                        principalSchema: "finance",
                        principalTable: "billing_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_adjustment_notes_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_adjustment_notes_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_adjustment_notes_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_adjustment_notes_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "billing_lines",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tax_base_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    vat_rate = table.Column<decimal>(type: "numeric(6,4)", precision: 6, scale: 4, nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_lines", x => x.id);
                    table.CheckConstraint("ck_billing_lines_amounts", "tax_base_amount > 0 AND vat_rate >= 0 AND vat_amount >= 0 AND total_amount >= tax_base_amount");
                    table.CheckConstraint("ck_billing_lines_line_number", "line_number >= 1");
                    table.ForeignKey(
                        name: "FK_billing_lines_billing_documents_billing_document_id",
                        column: x => x.billing_document_id,
                        principalSchema: "finance",
                        principalTable: "billing_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_lines_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_released_retention_by_user_id",
                schema: "finance",
                table: "billing_documents",
                column: "released_retention_by_user_id");

            migrationBuilder.AddCheckConstraint(
                name: "ck_billing_documents_amounts",
                schema: "finance",
                table: "billing_documents",
                sql: "amount > 0 AND vat_amount >= 0 AND retention_amount >= 0 AND credit_note_base_amount >= 0 AND debit_note_base_amount >= 0 AND paid_amount >= 0 AND paid_amount <= (amount + vat_amount + debit_note_base_amount + debit_note_vat_amount - credit_note_base_amount - credit_note_vat_amount)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_billing_documents_retention_status",
                schema: "finance",
                table: "billing_documents",
                sql: "retention_status IN ('none', 'held', 'released')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox",
                sql: "kind IN ('billing.issued', 'billing.voided', 'billing-note.approved', 'payment.recorded', 'payment.reversed', 'supplier-invoice.posted', 'supplier-invoice.voided')");

            migrationBuilder.CreateIndex(
                name: "IX_billing_adjustment_notes_billing_document_id",
                schema: "finance",
                table: "billing_adjustment_notes",
                column: "billing_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_adjustment_notes_branch_id",
                schema: "finance",
                table: "billing_adjustment_notes",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_adjustment_notes_created_by_user_id",
                schema: "finance",
                table: "billing_adjustment_notes",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_adjustment_notes_decided_by_user_id",
                schema: "finance",
                table: "billing_adjustment_notes",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_adjustment_notes_organization_id_number",
                schema: "finance",
                table: "billing_adjustment_notes",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_billing_lines_billing_document_id_line_number",
                schema: "finance",
                table: "billing_lines",
                columns: new[] { "billing_document_id", "line_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_billing_lines_organization_id",
                schema: "finance",
                table: "billing_lines",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "FK_billing_documents_users_released_retention_by_user_id",
                schema: "finance",
                table: "billing_documents",
                column: "released_retention_by_user_id",
                principalSchema: "identity_access",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_billing_documents_users_released_retention_by_user_id",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropTable(
                name: "billing_adjustment_notes",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "billing_lines",
                schema: "finance");

            migrationBuilder.DropIndex(
                name: "IX_billing_documents_released_retention_by_user_id",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropCheckConstraint(
                name: "ck_billing_documents_amounts",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropCheckConstraint(
                name: "ck_billing_documents_retention_status",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox");

            migrationBuilder.DropColumn(
                name: "net_cash_received",
                schema: "finance",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "receipt_number",
                schema: "finance",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "wht_amount",
                schema: "finance",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "wht_certificate_number",
                schema: "finance",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "wht_rate",
                schema: "finance",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "wht_type",
                schema: "finance",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "credit_note_base_amount",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "credit_note_vat_amount",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "debit_note_base_amount",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "debit_note_vat_amount",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "released_retention_at_utc",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "released_retention_by_user_id",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "retention_amount",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "retention_rate",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "retention_status",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "tax_invoice_number",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "vat_amount",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.DropColumn(
                name: "vat_rate",
                schema: "finance",
                table: "billing_documents");

            migrationBuilder.AddCheckConstraint(
                name: "ck_billing_documents_amounts",
                schema: "finance",
                table: "billing_documents",
                sql: "amount > 0 AND paid_amount >= 0 AND paid_amount <= amount");

            migrationBuilder.AddCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox",
                sql: "kind IN ('billing.issued', 'billing.voided', 'payment.recorded', 'payment.reversed', 'supplier-invoice.posted', 'supplier-invoice.voided')");
        }
    }
}
