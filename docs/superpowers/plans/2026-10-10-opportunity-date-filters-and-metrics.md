# [CRM Opportunities] Date Range Filters & Pipeline Metrics Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** เพิ่มฟังก์ชันตัวกรองช่วงเวลา (Next Action Date & Target Decision Date) และแถบการ์ดสถิติ Pipeline Metrics (Total Pipeline Value, Active Deals, Closing This Month, Win Rate / Won Value) ในหน้า Opportunities List เพื่อให้ทีมขายและผู้บริหารสามารถติดตามสถานะและวิเคราะห์ยอดขายได้อย่างแม่นยำ

**Architecture:** 
- Backend .NET 9 Clean Architecture: 
  - เพิ่มตัวกรอง `NextActionPreset` ("today", "this_week", "overdue") และ `TargetDecisionDateFrom` / `TargetDecisionDateTo` ใน `OpportunityListFilter` และ Query LINQ
  - เพิ่ม Endpoint ใหม่ `GET /api/v1/opportunities/metrics` เพื่อ Aggregate ตัวเลขภาพรวม Pipeline (Total Pipeline Value, Active Deals, Closing This Month, Win Rate % และ Won Value) จาก PostgreSQL โดยตรง ไม่คำนวณ Client-Side
- Frontend Next.js & Atelier Architectural Navy Sharp:
  - เพิ่มคอมโพเนนต์ `PipelineMetricCards` แสดง 4 Stat Cards ใต้ `PageHeader` (Reuse `StatCard`) พร้อม Loading State
  - เพิ่มตัวกรอง `NextActionDate` Preset Select และ `TargetDecisionDate` Filter ใน `ListToolbar` ซิงค์ผ่าน `useListState` และแสดงใน `ActiveFilterChips`
  - รองรับ i18n ครบถ้วนทั้ง `th.json` และ `en.json` พร้อม Strict TypeScript

**Tech Stack:** .NET 9 (C#, EF Core, PostgreSQL), Next.js App Router (TypeScript, TanStack Query, Tailwind CSS), Vitest & xUnit

## Global Constraints
- **Strict TypeScript:** ห้ามใช้ `any`, `as any` หรือ `@ts-ignore` เด็ดขาด
- **No Hardcoded Strings:** ข้อความและป้ายกำกับทั้งหมดต้องดึงผ่านระบบแปลภาษา (`th.json` และ `en.json`)
- **Visual Design:** Atelier Architectural Navy Sharp (`border-radius: 0px`, `#0B3056`, Pure SVG Icons)
- **Table-Preserved Architecture:** คงตาราง `DataTable` ไว้เสมอขณะกรองหรือเกิด Error
- **Verification Gates:** ต้องผ่าน `dotnet build`, `dotnet test`, `npm run lint`, `vitest` และ `npm run build` ก่อนส่งมอบ

---

### Task 1: Backend Domain & Query Layer Extension for Metrics & Date Filters

**Files:**
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs`
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/ListOpportunities/ListOpportunitiesQuery.cs`
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/ListOpportunities/ListOpportunitiesHandler.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/GetOpportunityMetrics/OpportunityMetricsProjection.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/GetOpportunityMetrics/GetOpportunityMetricsQuery.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/GetOpportunityMetrics/GetOpportunityMetricsHandler.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityMetricsHandlerTests.cs`

**Interfaces:**
- `OpportunityListFilter`:
  ```csharp
  public sealed record OpportunityListFilter(
      string? Search,
      Guid? CustomerId,
      string? Stage,
      string? SortBy = null,
      string? SortOrder = null,
      int? Page = null,
      int Limit = 25,
      string? Cursor = null,
      Guid? OwnerId = null,
      string? NextActionPreset = null, // "today", "this_week", "overdue"
      DateOnly? TargetDecisionDateFrom = null,
      DateOnly? TargetDecisionDateTo = null);
  ```
- `OpportunityMetricsProjection`:
  ```csharp
  public sealed record OpportunityMetricsProjection(
      decimal TotalPipelineValue,
      int ActiveDealsCount,
      int ClosingThisMonthCount,
      decimal WonValue,
      int WonDealsCount,
      decimal WinRatePercentage);
  ```
- `IOpportunityStore`:
  ```csharp
  Task<OpportunityMetricsProjection> GetMetricsAsync(
      Guid organizationId,
      Guid? branchId = null,
      CancellationToken cancellationToken = default);
  ```

- [ ] **Step 1: Write unit tests for Opportunity Metrics and Date Filtering**
  - สร้างไฟล์ `OpportunityMetricsHandlerTests.cs` ตรวจสอบการคำนวณ `TotalPipelineValue` (เฉพาะ Stage ที่ยังไม่ปิด), `ClosingThisMonthCount` และ `WinRatePercentage`
- [ ] **Step 2: Extend `OpportunityListFilter` and create `OpportunityMetricsProjection`**
  - ปรับปรุง `IOpportunityStore.cs` และเพิ่ม `GetOpportunityMetricsQuery` กับ `GetOpportunityMetricsHandler`
- [ ] **Step 3: Update `ListOpportunitiesQuery` and `ListOpportunitiesHandler`**
  - เพิ่ม parameters `NextActionPreset`, `TargetDecisionDateFrom`, `TargetDecisionDateTo`
- [ ] **Step 4: Run unit tests to verify failure/coverage**

---

### Task 2: Backend Persistence & API Endpoint Implementation

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Modify: `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `contracts/openapi/tan-erp.v1.json`
- Test: `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityStoreTests.cs` (or Handler tests)

**Interfaces:**
- `GET /api/v1/opportunities/metrics`:
  - Returns `OpportunityMetricsResponse`
- `GET /api/v1/opportunities`:
  - Query parameters: `nextActionPreset`, `targetDecisionDateFrom`, `targetDecisionDateTo`

- [ ] **Step 1: Implement LINQ queries in `OpportunityStore.ListAsync`**
  - กรอง `NextActionPreset`:
    - "today": `o.NextActionAtUtc >= todayStart && o.NextActionAtUtc < tomorrowStart`
    - "this_week": `o.NextActionAtUtc >= weekStart && o.NextActionAtUtc < weekEnd`
    - "overdue": `o.NextActionAtUtc < now && !closedStages.Contains(o.Stage)`
  - กรอง `TargetDecisionDateFrom` และ `TargetDecisionDateTo`
- [ ] **Step 2: Implement `OpportunityStore.GetMetricsAsync`**
  - รวมยอดตาม Organization/Branch ด้วย EF Core Aggregations
- [ ] **Step 3: Add `GetMetrics` endpoint in `OpportunitiesController`**
  - เพิ่ม `[HttpGet("metrics")]` พร้อม Permission Guard
- [ ] **Step 4: Register handler in `Program.cs` and update OpenAPI contract**
- [ ] **Step 5: Run `dotnet test` to verify backend logic**

---

### Task 3: Frontend API Client & TanStack Query Hooks

**Files:**
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/features/opportunities/api/opportunity-queries.ts`
- Modify: `frontend/src/generated/api/tan-erp.v1.ts`

**Interfaces:**
- `apiClient.getOpportunityMetrics()`
- `useOpportunityMetrics()` hook
- `ListOpportunitiesParams` ขยาย `nextActionPreset`, `targetDecisionDateFrom`, `targetDecisionDateTo`

- [ ] **Step 1: Update API Client with metrics method and new list params**
- [ ] **Step 2: Add `useOpportunityMetrics` React Query hook with cache invalidation keys**
- [ ] **Step 3: Update `opportunityListQueryKey` to include new filter parameters**

---

### Task 4: Frontend Pipeline Metrics Cards Component

**Files:**
- Create: `frontend/src/features/opportunities/components/pipeline-metric-cards.tsx`
- Create: `frontend/src/features/opportunities/components/pipeline-metric-cards.test.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- `<PipelineMetricCards metrics={metrics} isLoading={isLoading} />`
- ใช้คอมโพเนนต์กลาง `StatCard` 4 ใบ:
  1. Total Pipeline Value (Icon: Currency / TrendingUp)
  2. Active Deals (Icon: Briefcase)
  3. Closing This Month (Icon: Calendar)
  4. Win Rate / Won Value (Icon: Trophy / CheckCircle)

- [ ] **Step 1: Write test for `PipelineMetricCards` in `pipeline-metric-cards.test.tsx`**
- [ ] **Step 2: Add translation keys in `th.json` and `en.json` for all 4 metric titles & tooltips**
- [ ] **Step 3: Implement `PipelineMetricCards` using `StatCard`**
- [ ] **Step 4: Verify test with `npx vitest run pipeline-metric-cards.test.tsx`**

---

### Task 5: Frontend Date Range Filters in OpportunityList

**Files:**
- Modify: `frontend/src/features/opportunities/components/opportunity-list.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-list.test.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Details:**
- เพิ่ม `nextActionPreset` Select ใน `ListToolbar` ("ทั้งหมด", "วันนี้", "สัปดาห์นี้", "เลยกำหนด")
- เพิ่ม `targetDecisionMonth` หรือ Date Range Input
- แสดงผลใน `ActiveFilterChips` พร้อมฟังก์ชัน Dismiss
- เรนเดอร์ `<PipelineMetricCards />` ใต้ `PageHeader`

- [ ] **Step 1: Update `OpportunityFilters` interface and `useListState` schema**
- [ ] **Step 2: Add Date Filter controls in `ListToolbar`**
- [ ] **Step 3: Add `ActiveFilterChips` items for new date filters**
- [ ] **Step 4: Integrate `<PipelineMetricCards />` above `ListToolbar`**
- [ ] **Step 5: Add tests for Date Filter selection and Metrics rendering**
- [ ] **Step 6: Run `npm run test`, `npm run lint`, and `npm run build`**

---

### Task 6: Full System Verification & Review Gate

- [ ] **Step 1: Run Backend Tests**
  - `dotnet build`
  - `dotnet test`
- [ ] **Step 2: Run Frontend Verification**
  - `npm run lint`
  - `npm test`
  - `npm run build`
- [ ] **Step 3: Manual Browser Smoke Test**
  - ตรวจสอบหน้า `http://localhost:3005/th/opportunities`
  - ทดสอบสลับตัวกรองวันนัดหมายถัดไป (วันนี้, สัปดาห์นี้, เลยกำหนด)
  - ทดสอบตัวเลข Stat Cards อัปเดตถูกต้อง
