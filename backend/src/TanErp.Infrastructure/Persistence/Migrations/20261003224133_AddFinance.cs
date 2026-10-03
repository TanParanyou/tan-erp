using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFinance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.CreateTable(
                name: "accounting_outbox",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dedupe_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    next_attempt_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    last_error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    external_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    external_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    confirmed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    sent_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_outbox", x => x.id);
                    table.CheckConstraint("ck_accounting_outbox_attempts", "attempts >= 0");
                    table.CheckConstraint("ck_accounting_outbox_kind", "kind IN ('billing.issued', 'billing.voided', 'payment.recorded', 'payment.reversed')");
                    table.CheckConstraint("ck_accounting_outbox_status", "status IN ('pending', 'sent', 'failed', 'dead')");
                    table.ForeignKey(
                        name: "FK_accounting_outbox_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "billing_documents",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    paid_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reference_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    void_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    voided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    voided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issued_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_documents", x => x.id);
                    table.CheckConstraint("ck_billing_documents_amounts", "amount > 0 AND paid_amount >= 0 AND paid_amount <= amount");
                    table.CheckConstraint("ck_billing_documents_kind", "kind IN ('deposit', 'milestone', 'final', 'other')");
                    table.CheckConstraint("ck_billing_documents_status", "status IN ('issued', 'partially_paid', 'paid', 'voided')");
                    table.CheckConstraint("ck_billing_documents_void", "(status = 'voided') = (void_reason IS NOT NULL) AND (status <> 'voided' OR paid_amount = 0)");
                    table.ForeignKey(
                        name: "FK_billing_documents_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_documents_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_documents_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_documents_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_billing_documents_users_voided_by_user_id",
                        column: x => x.voided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    received_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reversal_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reversed_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    reversed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    recorded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.CheckConstraint("ck_payments_amount", "amount > 0");
                    table.CheckConstraint("ck_payments_method", "method IN ('transfer', 'cheque', 'cash', 'card')");
                    table.CheckConstraint("ck_payments_reversal", "(status = 'reversed') = (reversal_reason IS NOT NULL)");
                    table.CheckConstraint("ck_payments_status", "status IN ('recorded', 'reversed')");
                    table.ForeignKey(
                        name: "FK_payments_billing_documents_billing_document_id",
                        column: x => x.billing_document_id,
                        principalSchema: "finance",
                        principalTable: "billing_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payments_users_recorded_by_user_id",
                        column: x => x.recorded_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payments_users_reversed_by_user_id",
                        column: x => x.reversed_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_outbox_organization_id_dedupe_key",
                schema: "finance",
                table: "accounting_outbox",
                columns: new[] { "organization_id", "dedupe_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounting_outbox_organization_id_status_next_attempt_at_utc",
                schema: "finance",
                table: "accounting_outbox",
                columns: new[] { "organization_id", "status", "next_attempt_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_branch_id",
                schema: "finance",
                table: "billing_documents",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_created_by_user_id",
                schema: "finance",
                table: "billing_documents",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_organization_id_number",
                schema: "finance",
                table: "billing_documents",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_organization_id_project_id_status",
                schema: "finance",
                table: "billing_documents",
                columns: new[] { "organization_id", "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_project_id",
                schema: "finance",
                table: "billing_documents",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_billing_documents_voided_by_user_id",
                schema: "finance",
                table: "billing_documents",
                column: "voided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_billing_document_id",
                schema: "finance",
                table: "payments",
                column: "billing_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_organization_id_number",
                schema: "finance",
                table: "payments",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_organization_id_reference",
                schema: "finance",
                table: "payments",
                columns: new[] { "organization_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_recorded_by_user_id",
                schema: "finance",
                table: "payments",
                column: "recorded_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_reversed_by_user_id",
                schema: "finance",
                table: "payments",
                column: "reversed_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_outbox",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "billing_documents",
                schema: "finance");
        }
    }
}
