# Reporting & Executive Dashboard API Contract (`G-19`)

เอกสารนี้กำหนดข้อตกลง API สำหรับรายงานสรุปผู้บริหาร (Executive KPI Dashboard) และรายงานประจำโมดูลแบบอ่านอย่างเดียว (Read-Only Dapper Aggregations) ของ **Project ERP (tan-erp)**

อ้างอิงนโยบาย: [API Conventions](api-conventions.md), [RBAC](rbac.md), [Permission Catalog](permission-catalog.md), [Raw SQL Policy](../04-data/raw-sql-policy.md)

---

## 1. การยืนยันตัวตน ขอบเขตสาขา และสิทธิ์การเข้าถึง (Auth, Branch Isolation & RBAC)

ทุก Endpoint ภายใต้ `/api/v1/reports` บังคับส่ง:
- Header `Authorization: Bearer <Firebase ID Token>`
- Header `X-Organization-Membership-Id: <Membership UUID>`

### กฎการแยกข้อมูลระดับองค์กรและสาขา (Tenant & Branch Isolation)
1. ทุกคำสั่ง SQL กรอง `organization_id` จาก `RequestAccessContext` เสมอ
2. หาก Membership ของผู้เรียกผูกกับสาขาเฉพาะ (`BranchId != null`):
   - หากไม่ส่งพารามิเตอร์ `branchId` ระบบจะบังคับกรองเฉพาะสาขาของผู้เรียกโดยอัตโนมัติ (ไม่รั่วไหลข้ามสาขา)
   - หากส่งพารามิเตอร์ `branchId` ที่ไม่ตรงกับสาขาของผู้เรียก ระบบตอบกลับ `403 Forbidden` (`PERMISSION_DENIED`)
3. หาก Membership ของผู้เรียกเป็นระดับองค์กร (`BranchId == null`):
   - ไม่ส่ง `branchId` = รวมข้อมูลทุกสาขาภายใต้ `OrganizationId`
   - ส่ง `?branchId=<uuid>` = กรองเฉพาะสาขาที่เลือก

### ตารางสิทธิ์ต่อรายงาน (Per-Report Permission Matrix)

| Endpoint | Permission Key | หมายเหตุ |
| :--- | :--- | :--- |
| `GET /api/v1/reports/dashboard-summary` | Active Membership + Per-Section Check | คืนค่าเฉพาะการ์ด KPI ที่ผู้เรียกมีสิทธิ์อ่าน (`permitted: true`); ส่วนที่ไม่มีสิทธิ์จะคืน `permitted: false` และข้อมูลเป็น `null` |
| `GET /api/v1/reports/pipeline` | `opportunities.read` | Pipeline & Win Rate จาก `crm.opportunities` |
| `GET /api/v1/reports/quotations` | `quotations.read` | มูลค่าใบเสนอราคาและอัตราตอบรับจาก `commercial.quotations` |
| `GET /api/v1/reports/projects-budget` | `projects.read` | งบประมาณโครงการเทียบ Commitment และต้นทุนจริง (`projects.*`) |
| `GET /api/v1/reports/stock-value` | `inventory.read` | มูลค่าสินค้าคงคลังตามคลังสินค้าและสาขา (`inventory.stock_balances`) |
| `GET /api/v1/reports/ar-aging` | `billings.read` | อายุลูกหนี้การค้า 5 ช่วง (`Current`, `1-30`, `31-60`, `61-90`, `Over 90`) |
| `GET /api/v1/reports/service-sla` | `service-requests.read` | งานบริการคงค้าง (Backlog) และสถานะ SLA หักวันหยุดปฏิทินทำงาน |

---

## 2. รายละเอียด Endpoints

### 2.1 `GET /api/v1/reports/dashboard-summary`
- **Query Parameters:**
  - `branchId` (`uuid`, optional)
  - `asOfDate` (`YYYY-MM-DD`, optional — ใช้กับส่วน AR Aging; ค่าเริ่มต้นคือวันที่ปัจจุบัน UTC)
- **Response `200 OK` (`DashboardSummaryResponse`):**
  - `generatedAtUtc` (`string`, ISO-8601)
  - `effectiveBranchId` (`uuid | null`)
  - `pipeline`: `{ permitted: boolean, summary: PipelineSummaryMetrics | null }`
  - `quotations`: `{ permitted: boolean, summary: QuotationSummaryMetrics | null }`
  - `projectsBudget`: `{ permitted: boolean, summary: ProjectBudgetSummaryMetrics | null }`
  - `stockValue`: `{ permitted: boolean, summary: StockValueSummaryMetrics | null }`
  - `arAging`: `{ permitted: boolean, summary: ArAgingSummaryMetrics | null }`
  - `serviceSla`: `{ permitted: boolean, summary: ServiceSlaSummaryMetrics | null }`

### 2.2 `GET /api/v1/reports/pipeline`
- **Permission:** `opportunities.read`
- **Query Parameters:** `branchId` (`uuid`, optional)
- **Response `200 OK` (`PipelineReportResponse`):**
  - `summary`: `totalCount`, `totalExpectedBudget`, `openCount`, `openExpectedBudget`, `wonCount`, `wonBudget`, `lostCount`, `winRatePercent`
  - `stages`: รายการสรุปแยกตาม Stage (`stage`, `opportunityCount`, `expectedBudgetTotal`)

### 2.3 `GET /api/v1/reports/quotations`
- **Permission:** `quotations.read`
- **Query Parameters:** `branchId` (`uuid`, optional)
- **Response `200 OK` (`QuotationReportResponse`):**
  - `summary`: `totalCount`, `issuedCount`, `issuedAmount`, `acceptedCount`, `acceptedAmount`, `acceptanceRatePercent`
  - `rows`: รายการใบเสนอราคา (`quotationId`, `number`, `customerCode`, `customerName`, `branchCode`, `status`, `totalAmount`, `issuedAtUtc`, `acceptedAtUtc`)

### 2.4 `GET /api/v1/reports/projects-budget`
- **Permission:** `projects.read`
- **Query Parameters:** `branchId` (`uuid`, optional)
- **Response `200 OK` (`ProjectBudgetReportResponse`):**
  - `summary`: `projectCount`, `overBudgetCount`, `totalBaselineBudget`, `totalApprovedChangeOrderBudget`, `totalCurrentBudget`, `totalCommittedAmount`, `totalActualAmount`, `totalVarianceAmount`
  - `rows`: รายการโครงการ (`projectId`, `projectCode`, `projectName`, `branchCode`, `status`, `baselineContractAmount`, `baselineBudget`, `approvedChangeOrderBudget`, `currentBudget`, `committedAmount`, `actualAmount`, `varianceAmount`, `isOverBudget`)

### 2.5 `GET /api/v1/reports/stock-value`
- **Permission:** `inventory.read`
- **Query Parameters:** `branchId` (`uuid`, optional)
- **Response `200 OK` (`StockValueReportResponse`):**
  - `summary`: `warehouseCount`, `skuCount`, `totalOnHand`, `totalReserved`, `totalValue`
  - `rows`: รายการคงคลัง (`warehouseId`, `warehouseCode`, `warehouseName`, `branchCode`, `itemId`, `itemCode`, `itemNameTh`, `itemNameEn`, `onHand`, `reserved`, `available`, `totalValue`)

### 2.6 `GET /api/v1/reports/ar-aging`
- **Permission:** `billings.read`
- **Query Parameters:** `branchId` (`uuid`, optional), `asOfDate` (`YYYY-MM-DD`, optional)
- **Response `200 OK` (`ReportingArAgingResponse`):**
  - `summary`: `asOfDate`, `invoiceCount`, `current`, `bucket1To30`, `bucket31To60`, `bucket61To90`, `bucketOver90`, `totalOutstanding`
  - `rows`: รายการใบวางบิลค้างชำระ (`billingDocumentId`, `number`, `projectCode`, `customerCode`, `customerName`, `branchCode`, `dueDate`, `daysOverdue`, `agingBucket`, `grossTotal`, `paidAmount`, `outstandingAmount`)

### 2.7 `GET /api/v1/reports/service-sla`
- **Permission:** `service-requests.read`
- **Query Parameters:** `branchId` (`uuid`, optional)
- **Response `200 OK` (`ServiceSlaReportResponse`):**
  - `summary`: `totalCount`, `openBacklogCount`, `waitingCustomerCount`, `slaWarningCount`, `slaBreachedCount`, `outOfWarrantyChargedTotal`
  - `rows`: รายการใบแจ้งซ่อม/บริการ (`serviceRequestId`, `number`, `title`, `projectCode`, `branchCode`, `priority`, `status`, `inWarranty`, `slaStatus`, `elapsedHours`, `resolveTargetHours`, `serviceChargeAmount`, `createdAtUtc`)

---

## 3. รหัสข้อผิดพลาด (Error Codes — RFC 9457 Problem Details)

- `401 Unauthorized` (`AUTHENTICATION_REQUIRED`): ไม่ได้ส่งหรือส่ง Token ไม่ถูกต้อง
- `403 Forbidden` (`ACTIVE_MEMBERSHIP_REQUIRED`): ไม่พบ Membership ที่ใช้งานอยู่
- `403 Forbidden` (`PERMISSION_DENIED`): ไม่มีสิทธิ์อ่านรายงานนั้น หรือพยายามเข้าถึง `branchId` ข้ามสาขาที่ตนสังกัด
