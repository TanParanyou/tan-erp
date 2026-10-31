# Reporting & Executive Dashboard Verification (`G-19`)

**สถานะ:** Verified (Automated Backend Unit + Architecture + PostgreSQL 17 Integration + OpenAPI Contract + Frontend Typecheck/Lint/Vitest/Build) — 2026-10-31  
**แผนอ้างอิง:** [2026-10-31-g19-dashboard-reporting.md](../superpowers/plans/2026-10-31-g19-dashboard-reporting.md), [2026-10-05-erp-gap-closure-plan.md](../superpowers/plans/2026-10-05-erp-gap-closure-plan.md)  
**สัญญา API:** [Reporting API Contract](../03-contracts/reporting-api-contract.md)

---

## 1. สรุปสิ่งที่ส่งมอบ (Implemented Scope)

1. **Read-Only Dapper Reporting Store (`TanErp.Infrastructure/Persistence/Reporting/ReportingStore.cs`):**
   - ปฏิบัติตาม [Raw SQL Policy](../04-data/raw-sql-policy.md) ครบทุกข้อ: Parameterized SQL 100%, ระบุชื่อคอลัมน์และ Alias ชัดเจน (ไม่ใช้ `SELECT *` และไม่ใช้ `dynamic`), กำหนด `CommandDefinition` พร้อม `commandTimeout: 15` และ `CancellationToken`, และมี deterministic `ORDER BY ... , id`
   - ครอบคลุม 6 รายงานปฏิบัติการและ Executive Summary:
     1. **Pipeline & Win Rate (`crm.opportunities`):** นับจำนวนและงบประมาณคาดการณ์แยกตาม Stage พร้อมคำนวณ `WinRatePercent`
     2. **Quotation Value (`commercial.quotations`):** มูลค่าใบเสนอราคาที่ออกใช้งาน มูลค่าที่ลูกค้าตอบรับ และอัตราตอบรับ (`AcceptanceRatePercent`)
     3. **Project Budget vs Actual (`projects.*`):** เทียบงบ Baseline + Change Order ที่อนุมัติแล้ว (`CurrentBudget`) กับ `CommittedAmount` และ `ActualAmount` ในสมุดบัญชีต้นทุนโครงการ พร้อมระบุโครงการที่เกินงบ (`IsOverBudget`)
     4. **Stock Value (`inventory.stock_balances`):** ยอดคงเหลือ ยอดจอง ยอดพร้อมใช้ และมูลค่าสต็อกแยกตามคลังสินค้าและสาขา
     5. **AR Aging (`finance.billing_documents`):** คำนวณยอดค้างชำระสุทธิผ่าน `ArAgingCalculator` แบ่ง 5 ช่วงอายุหนี้ (`current`, `1_30`, `31_60`, `61_90`, `over_90`)
     6. **Service Backlog & SLA (`service.service_requests`):** ประเมินสถานะ SLA ผ่าน `ServiceSlaCalculator` โดยหักวันหยุดปฏิทินทำงาน (`mrp.working_calendar_holidays` ทั้งระดับองค์กรและสาขา) และช่วงรอลูกค้า (`waiting_customer`) พร้อมสรุปยอดค่าบริการงานนอกประกัน

2. **การแยกข้อมูลระดับองค์กร/สาขา และสิทธิ์รายรายงาน (`ReportingHandler.cs` & `ReportsController.cs`):**
   - ทุกคำสั่ง SQL บังคับกรอง `organization_id = @OrgId` และ `(@BranchId::uuid IS NULL OR branch_id = @BranchId)`
   - **Branch Clamping:** ผู้ใช้ที่สังกัดสาขาเฉพาะ (`access.BranchId != null`) จะถูกจำกัดข้อมูลเฉพาะสาขาตนเองโดยอัตโนมัติแม้ไม่ส่ง `?branchId` และถูกปฏิเสธด้วย `403 PERMISSION_DENIED` ทันทีหากส่ง `branchId` ของสาขาอื่น
   - **Per-Report Permission & Graceful Dashboard Redaction:** รายงานแต่ละตัวบังคับสิทธิ์อ่านของโมดูลเดิม (`opportunities.read`, `quotations.read`, `projects.read`, `inventory.read`, `billings.read`, `service-requests.read`) ส่วน `/api/v1/reports/dashboard-summary` จะคืน `permitted: false, summary: null` เฉพาะส่วนที่ผู้ใช้ไม่มีสิทธิ์โดยไม่ทำให้หน้าภาพรวมล้มเหลวทั้งหน้า

3. **Frontend Executive Dashboard & CSV Export (`frontend/src/features/dashboard/` & `/[locale]/reports`):**
   - สร้างหน้า Executive KPI Dashboard & Operational Reports ตามมาตรฐาน **Atelier Architectural Navy Sharp** (`0px` sharp corners, Solid Navy, Pure SVG Stroke Icons, `h-11 min-h-[44px]` input height harmony)
   - เชื่อมต่อและนำ Hooks กลางกลับมาใช้ซ้ำครบทั้ง `useCsvExport`, `useDataExport`, และ `useAnalyticsData`
   - รองรับสองภาษาครบคู่ 100% (`th.json` / `en.json` จำนวน 112 keys ใต้ `reporting.*` + `shell.reports`) โดยไม่แตะหน้า Home เดิม (`/[locale]`) เพื่อรักษาความเข้ากันได้ของ `erp-shell` และ `auth.spec.ts`

---

## 2. หลักฐานผลการตรวจสอบ (Verification Evidence — 2026-10-31)

| ชุดตรวจสอบ | คำสั่ง | ผลลัพธ์ |
| :--- | :--- | :--- |
| Backend Build | `dotnet build backend/TanErp.slnx` | **Passed** (`0 Warning(s), 0 Error(s)`) |
| Architecture Tests | `dotnet test backend/tests/TanErp.ArchitectureTests --no-build` | **5/5 Passed** (รวม `ReportsController_ShouldStayThin`) |
| Unit Tests | `dotnet test backend/tests/TanErp.UnitTests --no-build` | **716/716 Passed** (รวม `ReportingHandlerTests` `8/8`) |
| Integration + OpenAPI Contract Tests | `dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~OpenApiContractTests\|FullyQualifiedName~ReportingEndpointsTests" --no-build` | **4/4 Passed** บน PostgreSQL 17 Testcontainers (Hand-calculated fixtures ครบ 6 รายงาน, Cross-branch clamping, Per-report permission & redacted dashboard sections, OpenAPI parity) |
| Frontend TypeScript | `cd frontend && npx tsc --noEmit` | **Passed** (`0 errors`) |
| Frontend Lint | `cd frontend && npm run lint` | **Passed** (`0 warnings, 0 errors`) |
| Frontend Vitest | `cd frontend && npx vitest run src/features/dashboard/components/dashboard-components.test.tsx src/components/layout/SidebarNav.test.tsx src/components/layout/erp-shell.test.tsx` | **27/27 Passed** (`dashboard-components.test.tsx` `4/4`, `SidebarNav.test.tsx` `14/14`, `erp-shell.test.tsx` `9/9`) |
| Frontend Production Build | `cd frontend && npm run build` | **Passed** (`91/91` routes generated รวม `/th/reports` และ `/en/reports`) |

---

## 3. ข้อจำกัดและรายการรอตรวจรับทางธุรกิจ (Known Limitations & Open Gates)

- การคำนวณ KPI รอบแรกอ่านข้อมูลสดจาก PostgreSQL ผ่าน Dapper (`commandTimeout: 15s`) โดยยังไม่มี Materialized View หรือ Scheduled Snapshot สำหรับข้อมูลระดับหลายแสนแถว (`VQ-RPT-01`)
- สูตรนับอัตราชนะงาน (`Won / (Won + Lost)`) และการไม่รวมใบเสนอราคาสถานะ `draft`/`voided`/`superseded` ในยอด `IssuedAmount` เป็นสมมติฐานเริ่มต้นที่รอผู้บริหาร/ฝ่ายขายยืนยันใน UAT (`VQ-RPT-02`)
- ผลการทดสอบอัตโนมัติข้างต้นยังไม่แทนการตรวจรับ UAT โดยผู้ใช้งานจริงตามบทบาทใน Production
