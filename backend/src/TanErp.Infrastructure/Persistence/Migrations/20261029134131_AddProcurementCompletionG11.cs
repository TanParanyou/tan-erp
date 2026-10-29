using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurementCompletionG11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox");

            migrationBuilder.AddColumn<decimal>(
                name: "invoiced_quantity",
                schema: "procurement",
                table: "purchase_order_lines",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "returned_quantity",
                schema: "procurement",
                table: "goods_receipt_lines",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "purchase_requests",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    total_estimated_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    required_by_date = table.Column<DateOnly>(type: "date", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    submitted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cancel_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    converted_purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    converted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_requests", x => x.id);
                    table.CheckConstraint("ck_purchase_requests_converted", "(status = 'converted') = (converted_purchase_order_id IS NOT NULL)");
                    table.CheckConstraint("ck_purchase_requests_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
                    table.CheckConstraint("ck_purchase_requests_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'cancelled', 'converted')");
                    table.CheckConstraint("ck_purchase_requests_total_amount", "total_estimated_amount >= 0");
                    table.ForeignKey(
                        name: "FK_purchase_requests_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_requests_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_requests_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_requests_purchase_orders_converted_purchase_order_~",
                        column: x => x.converted_purchase_order_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_requests_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_requests_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_invoices",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    supplier_invoice_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    normalized_supplier_invoice_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    invoice_date = table.Column<DateOnly>(type: "date", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    subtotal_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_po_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_variance_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    match_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    submitted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    posted_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    decision_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cancel_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    void_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    voided_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    voided_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    row_version = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_invoices", x => x.id);
                    table.CheckConstraint("ck_supplier_invoices_amounts", "subtotal_amount >= 0 AND tax_amount >= 0 AND total_amount >= 0 AND total_po_amount >= 0");
                    table.CheckConstraint("ck_supplier_invoices_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
                    table.CheckConstraint("ck_supplier_invoices_match_status", "match_status IN ('pending', 'matched', 'price_variance')");
                    table.CheckConstraint("ck_supplier_invoices_status", "status IN ('draft', 'on_hold', 'matched', 'approved', 'rejected', 'cancelled', 'voided')");
                    table.CheckConstraint("ck_supplier_invoices_void", "(status = 'voided') = (void_reason IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_supplier_invoices_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalSchema: "procurement",
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_users_decided_by_user_id",
                        column: x => x.decided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoices_users_voided_by_user_id",
                        column: x => x.voided_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_returns",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    goods_receipt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    returned_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    total_return_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    stock_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    stock_document_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    returned_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_returns", x => x.id);
                    table.CheckConstraint("ck_supplier_returns_total_amount", "total_return_amount >= 0");
                    table.ForeignKey(
                        name: "FK_supplier_returns_branches_branch_id",
                        column: x => x.branch_id,
                        principalSchema: "organization",
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_goods_receipts_goods_receipt_id",
                        column: x => x.goods_receipt_id,
                        principalSchema: "procurement",
                        principalTable: "goods_receipts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_organizations_organization_id",
                        column: x => x.organization_id,
                        principalSchema: "organization",
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "projects",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_stock_documents_stock_document_id",
                        column: x => x.stock_document_id,
                        principalSchema: "inventory",
                        principalTable: "stock_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalSchema: "procurement",
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_returns_users_returned_by_user_id",
                        column: x => x.returned_by_user_id,
                        principalSchema: "identity_access",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchase_request_lines",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_no = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    item_name_th = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    estimated_unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    estimated_line_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_request_lines", x => x.id);
                    table.CheckConstraint("ck_purchase_request_lines_quantity", "quantity > 0");
                    table.CheckConstraint("ck_purchase_request_lines_unit_price", "estimated_unit_price >= 0");
                    table.ForeignKey(
                        name: "FK_purchase_request_lines_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_request_lines_purchase_requests_purchase_request_id",
                        column: x => x.purchase_request_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_request_lines_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_invoice_lines",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_no = table.Column<int>(type: "integer", nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    item_name_th = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    po_unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    price_variance_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    price_variance_percent = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_invoice_lines", x => x.id);
                    table.CheckConstraint("ck_supplier_invoice_lines_prices", "unit_price >= 0 AND po_unit_price >= 0");
                    table.CheckConstraint("ck_supplier_invoice_lines_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "FK_supplier_invoice_lines_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoice_lines_purchase_order_lines_purchase_order_~",
                        column: x => x.purchase_order_line_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_order_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_invoice_lines_supplier_invoices_supplier_invoice_id",
                        column: x => x.supplier_invoice_id,
                        principalSchema: "procurement",
                        principalTable: "supplier_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supplier_invoice_lines_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_return_lines",
                schema: "procurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_return_id = table.Column<Guid>(type: "uuid", nullable: false),
                    goods_receipt_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    line_return_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier_return_lines", x => x.id);
                    table.CheckConstraint("ck_supplier_return_lines_quantity", "quantity > 0");
                    table.CheckConstraint("ck_supplier_return_lines_unit_price", "unit_price >= 0");
                    table.ForeignKey(
                        name: "FK_supplier_return_lines_goods_receipt_lines_goods_receipt_lin~",
                        column: x => x.goods_receipt_line_id,
                        principalSchema: "procurement",
                        principalTable: "goods_receipt_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_return_lines_items_item_id",
                        column: x => x.item_id,
                        principalSchema: "item_master",
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_return_lines_purchase_order_lines_purchase_order_l~",
                        column: x => x.purchase_order_line_id,
                        principalSchema: "procurement",
                        principalTable: "purchase_order_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplier_return_lines_supplier_returns_supplier_return_id",
                        column: x => x.supplier_return_id,
                        principalSchema: "procurement",
                        principalTable: "supplier_returns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supplier_return_lines_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "item_master",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements",
                sql: "kind IN ('receipt', 'issue', 'transfer_out', 'transfer_in', 'adjustment_in', 'adjustment_out', 'return_in', 'return_out')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_purchase_order_lines_invoiced",
                schema: "procurement",
                table: "purchase_order_lines",
                sql: "invoiced_quantity >= 0 AND invoiced_quantity <= received_quantity");

            migrationBuilder.AddCheckConstraint(
                name: "ck_goods_receipt_lines_returned",
                schema: "procurement",
                table: "goods_receipt_lines",
                sql: "returned_quantity >= 0 AND returned_quantity <= quantity");

            migrationBuilder.AddCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox",
                sql: "kind IN ('billing.issued', 'billing.voided', 'payment.recorded', 'payment.reversed', 'supplier-invoice.posted', 'supplier-invoice.voided')");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_request_lines_item_id",
                schema: "procurement",
                table: "purchase_request_lines",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_request_lines_purchase_request_id_item_id",
                schema: "procurement",
                table: "purchase_request_lines",
                columns: new[] { "purchase_request_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_request_lines_purchase_request_id_line_no",
                schema: "procurement",
                table: "purchase_request_lines",
                columns: new[] { "purchase_request_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "IX_purchase_request_lines_unit_id",
                schema: "procurement",
                table: "purchase_request_lines",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_branch_id",
                schema: "procurement",
                table: "purchase_requests",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_converted_purchase_order_id",
                schema: "procurement",
                table: "purchase_requests",
                column: "converted_purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_created_by_user_id",
                schema: "procurement",
                table: "purchase_requests",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_decided_by_user_id",
                schema: "procurement",
                table: "purchase_requests",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_organization_id_branch_id_status",
                schema: "procurement",
                table: "purchase_requests",
                columns: new[] { "organization_id", "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_organization_id_converted_purchase_order_~",
                schema: "procurement",
                table: "purchase_requests",
                columns: new[] { "organization_id", "converted_purchase_order_id" });

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_organization_id_number",
                schema: "procurement",
                table: "purchase_requests",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_organization_id_project_id_status",
                schema: "procurement",
                table: "purchase_requests",
                columns: new[] { "organization_id", "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_organization_id_status_created_at_utc",
                schema: "procurement",
                table: "purchase_requests",
                columns: new[] { "organization_id", "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_purchase_requests_project_id",
                schema: "procurement",
                table: "purchase_requests",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoice_lines_item_id",
                schema: "procurement",
                table: "supplier_invoice_lines",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoice_lines_purchase_order_line_id",
                schema: "procurement",
                table: "supplier_invoice_lines",
                column: "purchase_order_line_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoice_lines_supplier_invoice_id_line_no",
                schema: "procurement",
                table: "supplier_invoice_lines",
                columns: new[] { "supplier_invoice_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoice_lines_supplier_invoice_id_purchase_order_l~",
                schema: "procurement",
                table: "supplier_invoice_lines",
                columns: new[] { "supplier_invoice_id", "purchase_order_line_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoice_lines_unit_id",
                schema: "procurement",
                table: "supplier_invoice_lines",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_branch_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_created_by_user_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_decided_by_user_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "decided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_organization_id_number",
                schema: "procurement",
                table: "supplier_invoices",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_organization_id_purchase_order_id",
                schema: "procurement",
                table: "supplier_invoices",
                columns: new[] { "organization_id", "purchase_order_id" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_organization_id_status_created_at_utc",
                schema: "procurement",
                table: "supplier_invoices",
                columns: new[] { "organization_id", "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_organization_id_supplier_id_invoice_date",
                schema: "procurement",
                table: "supplier_invoices",
                columns: new[] { "organization_id", "supplier_id", "invoice_date" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_organization_id_supplier_id_normalized_su~",
                schema: "procurement",
                table: "supplier_invoices",
                columns: new[] { "organization_id", "supplier_id", "normalized_supplier_invoice_number" },
                unique: true,
                filter: "status NOT IN ('cancelled', 'voided', 'rejected')");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_project_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_purchase_order_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_supplier_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_invoices_voided_by_user_id",
                schema: "procurement",
                table: "supplier_invoices",
                column: "voided_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_return_lines_goods_receipt_line_id",
                schema: "procurement",
                table: "supplier_return_lines",
                column: "goods_receipt_line_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_return_lines_item_id",
                schema: "procurement",
                table: "supplier_return_lines",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_return_lines_purchase_order_line_id",
                schema: "procurement",
                table: "supplier_return_lines",
                column: "purchase_order_line_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_return_lines_supplier_return_id",
                schema: "procurement",
                table: "supplier_return_lines",
                column: "supplier_return_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_return_lines_unit_id",
                schema: "procurement",
                table: "supplier_return_lines",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_branch_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_goods_receipt_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "goods_receipt_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_organization_id_goods_receipt_id",
                schema: "procurement",
                table: "supplier_returns",
                columns: new[] { "organization_id", "goods_receipt_id" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_organization_id_number",
                schema: "procurement",
                table: "supplier_returns",
                columns: new[] { "organization_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_organization_id_purchase_order_id_returned~",
                schema: "procurement",
                table: "supplier_returns",
                columns: new[] { "organization_id", "purchase_order_id", "returned_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_project_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_purchase_order_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_returned_by_user_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "returned_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_stock_document_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "stock_document_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_returns_supplier_id",
                schema: "procurement",
                table: "supplier_returns",
                column: "supplier_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchase_request_lines",
                schema: "procurement");

            migrationBuilder.DropTable(
                name: "supplier_invoice_lines",
                schema: "procurement");

            migrationBuilder.DropTable(
                name: "supplier_return_lines",
                schema: "procurement");

            migrationBuilder.DropTable(
                name: "purchase_requests",
                schema: "procurement");

            migrationBuilder.DropTable(
                name: "supplier_invoices",
                schema: "procurement");

            migrationBuilder.DropTable(
                name: "supplier_returns",
                schema: "procurement");

            migrationBuilder.DropCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements");

            migrationBuilder.DropCheckConstraint(
                name: "ck_purchase_order_lines_invoiced",
                schema: "procurement",
                table: "purchase_order_lines");

            migrationBuilder.DropCheckConstraint(
                name: "ck_goods_receipt_lines_returned",
                schema: "procurement",
                table: "goods_receipt_lines");

            migrationBuilder.DropCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox");

            migrationBuilder.DropColumn(
                name: "invoiced_quantity",
                schema: "procurement",
                table: "purchase_order_lines");

            migrationBuilder.DropColumn(
                name: "returned_quantity",
                schema: "procurement",
                table: "goods_receipt_lines");

            migrationBuilder.AddCheckConstraint(
                name: "ck_stock_movements_kind",
                schema: "inventory",
                table: "stock_movements",
                sql: "kind IN ('receipt', 'issue', 'transfer_out', 'transfer_in', 'adjustment_in', 'adjustment_out', 'return_in')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_accounting_outbox_kind",
                schema: "finance",
                table: "accounting_outbox",
                sql: "kind IN ('billing.issued', 'billing.voided', 'payment.recorded', 'payment.reversed')");
        }
    }
}
