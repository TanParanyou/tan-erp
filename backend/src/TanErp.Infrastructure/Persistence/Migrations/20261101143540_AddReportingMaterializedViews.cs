using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TanErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportingMaterializedViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS reporting;");

            migrationBuilder.Sql(@"
CREATE MATERIALIZED VIEW reporting.mv_pipeline_stages AS
SELECT organization_id, branch_id, stage,
       COUNT(*)::int AS opportunity_count,
       COALESCE(SUM(expected_budget), 0) AS expected_budget_total
FROM crm.opportunities
GROUP BY organization_id, branch_id, stage;");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ux_mv_pipeline_stages ON reporting.mv_pipeline_stages (organization_id, branch_id, stage);");

            migrationBuilder.Sql(@"
CREATE MATERIALIZED VIEW reporting.mv_project_budget AS
SELECT
    p.organization_id,
    p.branch_id,
    p.id AS project_id,
    p.code AS project_code,
    p.name AS project_name,
    COALESCE(b.branch_code, '') AS branch_code,
    p.status,
    COALESCE(p.baseline_contract_amount, 0) AS baseline_contract_amount,
    COALESCE(p.baseline_budget_total, (
        SELECT COALESCE(SUM(bl.amount), 0)
        FROM projects.project_budget_lines bl
        WHERE bl.organization_id = p.organization_id AND bl.project_id = p.id
    ), 0) AS baseline_budget,
    COALESCE((
        SELECT SUM(co.budget_delta)
        FROM projects.project_change_orders co
        WHERE co.organization_id = p.organization_id AND co.project_id = p.id AND co.status = 'approved'
    ), 0) AS approved_change_order_budget,
    COALESCE((
        SELECT SUM(cle.amount)
        FROM projects.project_cost_ledger_entries cle
        WHERE cle.organization_id = p.organization_id AND cle.project_id = p.id AND cle.entry_type = 'commitment'
    ), 0) AS committed_amount,
    COALESCE((
        SELECT SUM(cle.amount)
        FROM projects.project_cost_ledger_entries cle
        WHERE cle.organization_id = p.organization_id AND cle.project_id = p.id AND cle.entry_type = 'actual'
    ), 0) AS actual_amount
FROM projects.projects p
LEFT JOIN organization.branches b ON b.organization_id = p.organization_id AND b.id = p.branch_id
WHERE p.status <> 'cancelled';");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ux_mv_project_budget ON reporting.mv_project_budget (project_id);");

            migrationBuilder.Sql(@"
CREATE MATERIALIZED VIEW reporting.mv_stock_value AS
SELECT
    sb.organization_id,
    w.branch_id,
    sb.id AS balance_id,
    sb.warehouse_id,
    COALESCE(w.code, '') AS warehouse_code,
    COALESCE(w.name, '') AS warehouse_name,
    COALESCE(b.branch_code, '') AS branch_code,
    sb.item_id,
    COALESCE(i.code, '') AS item_code,
    COALESCE(i.name->>'th', '') AS item_name_th,
    COALESCE(i.name->>'en', '') AS item_name_en,
    sb.on_hand,
    sb.reserved,
    (sb.on_hand - sb.reserved) AS available,
    sb.total_value
FROM inventory.stock_balances sb
INNER JOIN inventory.warehouses w ON w.organization_id = sb.organization_id AND w.id = sb.warehouse_id
INNER JOIN item_master.items i ON i.organization_id = sb.organization_id AND i.id = sb.item_id
LEFT JOIN organization.branches b ON b.organization_id = w.organization_id AND b.id = w.branch_id;");
            migrationBuilder.Sql("CREATE UNIQUE INDEX ux_mv_stock_value ON reporting.mv_stock_value (balance_id);");
            migrationBuilder.Sql("CREATE INDEX ix_mv_stock_value_org_branch ON reporting.mv_stock_value (organization_id, branch_id);");
            migrationBuilder.Sql("CREATE INDEX ix_mv_project_budget_org_branch ON reporting.mv_project_budget (organization_id, branch_id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS reporting.mv_stock_value;");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS reporting.mv_project_budget;");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS reporting.mv_pipeline_stages;");
            migrationBuilder.Sql("DROP SCHEMA IF EXISTS reporting;");
        }
    }
}
