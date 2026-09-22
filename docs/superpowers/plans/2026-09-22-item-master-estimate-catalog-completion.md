# Item Master Estimate Catalog Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ปิด Item Master Estimate Catalog vertical slice ให้ครบตาม completion plan และ gates A–D: สร้าง Master Data ที่กำกับดูแลอย่างเข้มงวด (Item, Taxonomy, Branch Availability, Private Images, Versioned Cost with Maker-Checker) พร้อมเชื่อมข้อมูลจริงเข้ากับ Estimate Workspace Drawer / Catalog Modal และบันทึก Immutable Calculation Snapshot ใน Official Estimate

**Architecture:** Item เป็น Aggregate ระดับ Organization; Branch Availability, Item Image และ Cost Record เป็น Relation แยกที่บังคับ Same-organization Scope ฝั่ง Backend Catalog Read Model รับ Branch Context แล้วคืน Structured Projection พร้อม Published Cost และ Verified Primary Image ส่วน Estimate Command จะ Resolve/Validate ราคาใหม่ก่อนบันทึก Snapshot จึงไม่เชื่อราคาใน Browser

**Tech Stack:** .NET 10 (SDK 10.0.400 via `/Users/syaco/.dotnet`), ASP.NET Core, EF Core 10, PostgreSQL/Npgsql, xUnit/Testcontainers, Next.js 16, React 19, TypeScript strict, TanStack Query, Zod, Vitest/Testing Library, Playwright, OpenAPI

---

## Global Constraints & Guardrails

- Scope จบที่ Item/Cost/Image Catalog → Estimate Cost Component → Calculation Snapshot; Supplier CRUD, Procurement, Inventory, Warehouse, Production, BOM/MRP, Import และ Unit Conversion Chain อยู่นอก scope
- ภาษาไทยเป็นค่าเริ่มต้น; UI copy ใหม่ทุกข้อความต้องมีทั้ง `frontend/src/messages/th.json` และ `frontend/src/messages/en.json`
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, arbitrary fallback หรือ Frontend `array.find` เพื่อประกอบความสัมพันธ์จาก Foreign Key
- Frontend ใช้ TanStack Query สำหรับ Server State และใช้ Tailwind semantic tokens; รักษา Atelier Architectural Navy Sharp และ `border-radius: 0px`
- Reuse `AuditEvent`, `IFileStorageProvider`, File Upload Session, `FileClient`, Problem Details, Request Context และ Permission infrastructure เดิม
- EF Core เป็นเจ้าของ Write/Transaction; Catalog/Cost Resolver ใช้ EF projection ก่อน และใช้ Parameterized Dapper Read Model เฉพาะเมื่อ Query Plan พิสูจน์ว่าจำเป็น
- File Selection ใช้ Deferred Upload; Binary อยู่ใน Private Storage และ Item เก็บเฉพาะ Verified File relation
- ทุก Resource lookup บังคับ Organization/Branch Scope ฝั่ง Backend; Resource นอก Scope คืน 404
- ทุก Write ที่แก้ State ต้องเขียน Audit Event ใน Transaction เดียวกันและใช้ optimistic concurrency
- ห้ามแก้ Published Cost หรือ Historical Estimate Snapshot ย้อนหลัง

---

## Gate A — Schema Safety

### Task 1: Domain Models, Localized JSONB Persistence, File Parent Extension & Migration
- [x] Step 1.1: Write failing unit tests for `LocalizedText`, `Item`, `ItemCategory`, `ItemBrand`, `ItemAlias`, `UnitOfMeasure`, `ItemBranchAvailability`
- [x] Step 1.2: Implement domain entities and invariants
- [x] Step 1.3: Configure EF Core mappings with same-organization composite FKs, JSONB converters and constraints
- [x] Step 1.4: Extend File Service with `FileParentTypes.Item` and safe file metadata fields (`content_sha256`, `scan_status`, `verified_at_utc`, `width`, `height`)
- [x] Step 1.5: Add schema migration and persistence integration tests
- [x] Step 1.6: Run tests and verify zero destructive changes

---

## Gate B — Governed Master Data

### Task 2: Governed Item & Taxonomy Management API
- [x] Step 2.1: Write failing API tests for Item, Category, Brand, Unit, Alias, and Branch Availability
- [x] Step 2.2: Implement `IItemStore` and CQRS handlers (Create, Update, Activate, Deactivate, SetBranchAvailability, Manage Taxonomy)
- [x] Step 2.3: Implement API controllers with RFC 9457 Problem Details and ETag/If-Match concurrency
- [x] Step 2.4: Ensure atomic `AuditEvent` logging and scope security (cross-organization returns 404)
- [x] Step 2.5: Run tests and verify OpenAPI contracts

### Task 3: Verified Private Item Images over File Service
- [x] Step 3.1: Write failing unit and integration tests for Item Image invariants
- [x] Step 3.2: Implement `ItemImage` entity, configurations, and partial unique constraint for primary image
- [x] Step 3.3: Implement `IItemImageStore` and image endpoints (attach, set primary, reorder, detach)
- [x] Step 3.4: Integrate with `FileParentAccessResolver` for permission and authorization
- [x] Step 3.5: Run tests and verify image privacy (no public URLs or token leaks)

### Task 4: Versioned Cost Workflow, Maker-Checker & Deterministic Resolver
- [x] Step 4.1: Write tests for Cost lifecycle (Draft → Submitted → Approved/Returned → Published/Superseded/Disabled) and maker-checker invariant
- [x] Step 4.2: Implement `CostRecord`, `CostRecordReview`, `CostSource` entities and persistence
- [x] Step 4.3: Implement `ICostResolver` with deterministic precedence (Branch > Organization, latest EffectiveFrom, ambiguity detection)
- [x] Step 4.4: Implement Cost endpoints with atomic audit and concurrency
- [x] Step 4.5: Run tests and verify cost immutability and resolver invariants

---

## Gate C — Authoritative Estimate Flow

### Task 5: Branch-Aware Estimate Catalog Read Model API
- [x] Step 5.1: Write integration tests for `GET /api/v1/estimate-catalog/items`
- [x] Step 5.2: Implement `IEstimateCatalogReader` with server-side search, facets, cursor pagination and zero N+1 queries
- [x] Step 5.3: Filter for Active, `canCost=true`, branch-available items with deterministic published cost
- [x] Step 5.4: Validate cursor format (`ITEM_CATALOG_CURSOR_INVALID` on malformed cursor)
- [x] Step 5.5: Register controller endpoint, verify OpenAPI parity, and run tests

### Task 6: Estimate Cost Component Reference, Price Revalidation & Snapshot
- [x] Step 6.1: Write tests for Estimate Cost Component catalog reference and snapshot invariants
- [x] Step 6.2: Update `EstimateCostComponent` entity and EF mapping to store immutable snapshot fields
- [x] Step 6.3: Update `UpdateEstimateDraftHandler` to ignore client unitCost, resolve server-side, and return `ITEM_COST_VERSION_CONFLICT` on price change
- [x] Step 6.4: Add migration for Estimate cost component snapshot columns
- [x] Step 6.5: Run regression tests for Estimate BOQ & Calculation

### Task 7: Frontend Catalog Adapter, TanStack Query Hook & Modal Connection
- [ ] Step 7.1: Regenerate TypeScript OpenAPI client (`npm run generate:api`)
- [ ] Step 7.2: Implement `estimate-catalog-client.ts` and `use-estimate-catalog.ts` hook
- [ ] Step 7.3: Implement authenticated private thumbnail loading via `FileClient` and revocable object URLs
- [ ] Step 7.4: Connect `estimate-item-catalog-modal.tsx` to query hook, remove static `ESTIMATE_CATALOG_ITEMS`
- [ ] Step 7.5: Update translations in `th.json` and `en.json`, run frontend tests and verification pipeline

---

## Gate D — Production Evidence

### Task 8: End-to-End Verification, Security Tests, Playwright & Verification Record
- [ ] Step 8.1: Write backend end-to-end integration scenario (`ItemCatalogEstimateFlowTests.cs`)
- [ ] Step 8.2: Write security tests for tenant/branch/file-parent/maker-checker/concurrency
- [ ] Step 8.3: Write Playwright E2E test for catalog browsing, selection, BOQ insertion, and snapshot verification at desktop and 320px
- [ ] Step 8.4: Create verification record `docs/05-engineering/item-master-estimate-catalog-verification.md`
- [ ] Step 8.5: Run full verification suite (`dotnet test`, `npm run verify`, `npx playwright test`) and sign off
