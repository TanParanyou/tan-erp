# Wave E (`G-19`) — Executive Dashboard & Module Reporting Implementation Plan

**Goal:** Deliver read-only Dapper KPI aggregations, 6 module-specific operational reports with strict Organization & Branch scope isolation, per-report permission enforcement, and CSV export (`useCsvExport` / `useDataExport` / `useAnalyticsData`) under Atelier Architectural Navy Sharp (`0px` sharp geometry).

**Related Gap:** `G-19` in [2026-10-05-erp-gap-closure-plan.md](2026-10-05-erp-gap-closure-plan.md)
**Related Validation Questions:** `VQ-RPT-01`, `VQ-RPT-02` in [open-validation-questions.md](../../01-business/open-validation-questions.md)
**Related Policies:** [Raw SQL Policy](../../04-data/raw-sql-policy.md), [API Conventions](../../03-contracts/api-conventions.md), [Design System](../../../design.md)

---

## 1. Architectural Guardrails

1. **Clean Architecture & Raw SQL Policy (`docs/04-data/raw-sql-policy.md`):**
   - `TanErp.Application` remains pure (zero dependency on `Dapper` or `TanErp.Infrastructure`).
   - `Dapper` (`2.1.66`) is referenced ONLY in `TanErp.Infrastructure`.
   - `ReportingStore` in `TanErp.Infrastructure/Persistence/Reporting/ReportingStore.cs` executes parameterized read-only SQL via `CommandDefinition(sql, parameters, commandTimeout: 15, cancellationToken: ct)` with explicit column aliases (`AS "PropertyName"`), zero `SELECT *`, zero `dynamic`, and deterministic `ORDER BY ... , id`.
   - `ReportsController` stays thin (`LayerDependencyTests` rule: `ReportsController_ShouldStayThin`), depending on `ReportingHandler` rather than `IReportingStore` or `TanErp.Infrastructure`.

2. **Strict Organization & Branch Isolation (`RequestAccessContext`):**
   - Every SQL query filters `organization_id = @OrgId` and `(@BranchId::uuid IS NULL OR branch_id = @BranchId)`.
   - When the caller's membership is branch-scoped (`access.BranchId.HasValue`):
     - Passing a different `branchId` query parameter returns `403 PERMISSION_DENIED`.
     - Omitting `branchId` automatically clamps `@BranchId = access.BranchId.Value` so cross-branch rows never leak.
   - When the caller's membership is organization-wide (`access.BranchId is null`), `@BranchId` equals the optional `branchId` query filter (`null` = all branches in the organization).

3. **Per-Report Permission Enforcement (Reuse Existing Permission Keys):**
   - `GET /api/v1/reports/pipeline` → `opportunities.read`
   - `GET /api/v1/reports/quotations` → `quotations.read`
   - `GET /api/v1/reports/projects-budget` → `projects.read`
   - `GET /api/v1/reports/stock-value` → `inventory.read`
   - `GET /api/v1/reports/ar-aging` → `billings.read`
   - `GET /api/v1/reports/service-sla` → `service-requests.read`
   - `GET /api/v1/reports/dashboard-summary` → authenticates active membership and resolves each of the 6 KPI sections independently based on the caller's held permissions (`permitted: true/false`), preventing whole-page `403` errors for role-specific users while strictly redacting unauthorized module metrics (`null`).

4. **Preserve `/[locale]` Home Shell Compatibility:**
   - Keep `frontend/src/app/[locale]/(erp)/page.tsx` and `ErpShell` default overview intact so `auth.spec.ts` and `erp-shell.test.tsx` remain green.
   - Mount the Executive Dashboard & Reporting workspace at `/[locale]/reports` (`frontend/src/app/[locale]/(erp)/reports/page.tsx`) with components under `frontend/src/features/dashboard/` and navigation entry in `SidebarNav.tsx`.

---

## 2. Six Operational Reports & KPI Definitions

1. **Pipeline & Win Rate (`crm.opportunities`):**
   - Aggregates opportunity counts and `expected_budget` sums across stages (`draft`, `qualified`, `surveying`, `estimating`, `proposed`, `won`, `lost`, `cancelled`).
   - `winRatePercent = (wonCount + lostCount) == 0 ? 0 : round(wonCount * 100.0 / (wonCount + lostCount), 2)`.
   - Detailed rows per stage and branch for table view & CSV export.

2. **Quotation Value (`commercial.quotations` + `crm.customers` + `organization.branches`):**
   - Aggregates `issuedCount`, `issuedAmount`, `acceptedCount`, `acceptedAmount`, `activePipelineAmount` (`status = 'issued'`), and `acceptanceRatePercent`.
   - Detailed rows (`number`, `customerCode`, `customerName`, `branchCode`, `status`, `totalAmount`, `issuedAtUtc`, `acceptedAtUtc`) ordered by `issued_at_utc DESC, id ASC`.

3. **Project Budget vs Actual (`projects.projects` + `projects.project_change_orders` + `projects.project_cost_ledger_entries`):**
   - Per non-cancelled project (and status summary): `baselineBudget` (`COALESCE(baseline_budget_total, 0)`), `approvedChangeOrderBudget` (`SUM(budget_delta)` where `status = 'approved'`), `currentBudget = baselineBudget + approvedChangeOrderBudget`, `committedAmount` (`entry_type = 'commitment'`), `actualAmount` (`entry_type = 'actual'`), `totalConsumed = committedAmount + actualAmount`, `varianceAmount = currentBudget - totalConsumed`, `isOverBudget = currentBudget > 0 AND totalConsumed > currentBudget` (or `totalConsumed > currentBudget`).

4. **Stock Value (`inventory.stock_balances` + `inventory.warehouses` + `item_master.items`):**
   - Filters `sb.organization_id = @OrgId` and `(@BranchId::uuid IS NULL OR w.branch_id = @BranchId)`.
   - Aggregates `totalStockValue`, `warehouseCount`, `skuCount`, and detailed rows (`warehouseCode`, `warehouseName`, `branchCode`, `itemCode`, `itemNameTh`, `itemNameEn`, `onHand`, `reserved`, `available`, `totalValue`).

5. **AR Aging (`finance.billing_documents` + `projects.projects` + `crm.customers`):**
   - Filters `b.organization_id = @OrgId`, `b.status <> 'voided'`, and `(@BranchId::uuid IS NULL OR b.branch_id = @BranchId)`.
   - Uses domain `ArAgingCalculator` (`Current`, `1-30`, `31-60`, `61-90`, `Over 90`) on invoice `Outstanding` (`GrossTotal - heldRetention - PaidAmount > 0`) as of `asOfDate` (defaults to today UTC).
   - Returns 5-bucket totals + per-invoice/customer aging rows for CSV export.

6. **Service Backlog & SLA Overdue (`service.service_requests` + `projects.projects` + `mrp.working_calendar_holidays`):**
   - Filters `sr.organization_id = @OrgId` and `(@BranchId::uuid IS NULL OR sr.branch_id = @BranchId)`.
   - Evaluates each request via `ServiceSlaCalculator.Evaluate` (deducting working-calendar holidays and `PausedSeconds`).
   - Returns `openBacklogCount` (`status NOT IN ('resolved', 'closed')`), `slaWarningCount` (active requests in `warning`), `slaBreachedCount` (active requests in `breached`), `waitingCustomerCount`, `outOfWarrantyChargedTotal`, and detailed service request SLA rows for CSV export.

---

## 3. Execution Phases (`/tiered-orchestration`)

- **Phase 1 (Backend — `146b0ca5-4ac7-4cf9-a125-a5c854374eeb`):** Dapper `ReportingStore`, `ReportingHandler`, `ReportsController`, thin-controller architecture test, unit tests, and PostgreSQL integration tests (hand-calculated fixture assertions + branch-scope isolation + permission denial).
- **Phase 2 (Contract Bridge — Main Orchestrator):** Regenerate `openapi.v1.json` and TypeScript schema (`npm run generate:api`).
- **Phase 3 (Frontend — `5bb9344d-a2cd-4457-8886-cacd9337b06a`):** Typed `apiClient` methods, TanStack Query hooks + `useAnalyticsData` / `useCsvExport` / `useDataExport` integration, `<DashboardPage />` at `/[locale]/reports`, `SidebarNav` link, `th.json`/`en.json` parity, and Vitest component tests.
- **Phase 4 (Verification & Docs — Main Orchestrator):** Full backend + frontend verification gates, `docs/05-engineering/reporting-verification.md`, roadmap/gap-closure/open-questions updates, and atomic commit.
