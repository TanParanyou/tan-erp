# Official Estimate Completion Implementation Plan

> **For agentic workers:** Implement one task at a time and review its evidence before continuing. Use `executing-plans` for execution; the checkboxes below track work, not current implementation status.

**Status:** Core implementation gates pass; full backend suite passed 289 Integration, 287 Unit, and 3 Architecture tests on 2026-09-30. Release readiness and UAT baseline reconciliation remain open (updated 2026-09-30). UAT-EST-003 has an explicit base-unit rule, stable `ESTIMATE_UNIT_INVALID` API/UI handling, and passing API/component tests; manual UAT remains open. Estimate unit-conversion snapshots remain deferred because the component model does not persist conversion evidence. UAT-EST-006 has sequential review, a test-only multi-trigger evaluator, frozen route evidence, reviewer projection, and custom-work-item readiness/API persistence, Item Master snapshots, bilingual editor fields, and migration. Estimate API passed 40/40 and the Item Catalog Estimate scenario passed 1/1 on PostgreSQL; focused component tests passed 19/19, frontend lint/typecheck passed, and backend build/unit/architecture checks passed. Authorized-user UAT and production-policy sign-off remain open. UAT-EST-012 cross-organization/branch isolation has a passing read/update/calculate/approve negative matrix. UAT-EST-013 API projection is verified against an allowlist; customer-facing output remains deferred to CP-04. Same-key quotation replay evidence covers UAT-EST-011/018, but business UAT remains open. Unchecked items also require authorized sanitized data, pilot master data, UAT participants, or named policy approvers; synthetic fixtures cannot close those gates.

**Latest recheck (2026-09-30):** Current backend build passed with 0 warnings/errors; UnitTests passed 288/288 and ArchitectureTests 3/3. IntegrationTests passed 288/289 because one PostgreSQL fixture failed during SSL startup; that test passed on two isolated reruns. The Quotation response allowlist regression passed 1/1. The startup root cause remains unconfirmed; see [verification record](../../05-engineering/official-estimate-verification.md#quotation-api-response-allowlist-2026-09-30).

**Goal:** ปิดช่องว่างจาก BOQ/Calculation ที่มีแล้วไปสู่ Official Estimate ที่ตรวจต้นทุน ส่งอนุมัติ สร้าง Revision และออก Quotation จากฉบับที่อนุมัติได้อย่างตรวจสอบย้อนหลัง

**Architecture:** ขยาย Estimate aggregate และ use cases เดิม ไม่สร้างระบบประเมินราคาอีกชุด. Backend เป็นเจ้าของ Cost/Policy Resolution, Readiness, Approval Route, สถานะและ Snapshot; EF Core เขียนธุรกรรมและ Audit แบบ atomic. Frontend ใช้ OpenAPI types, Central API Client, TanStack Query และคอมโพเนนต์กลาง.

**Tech Stack:** SDK ตาม `backend/global.json`, ASP.NET Core, EF Core/PostgreSQL, Next.js/React/TypeScript strict, OpenAPI, Vitest และ Playwright.

## Global Constraints

- อ่าน `AGENTS.md`, `design.md`, `CONTEXT.md`, `docs/README.md` และ `backend/AGENTS.md`/`frontend/AGENTS.md` ก่อนแก้โค้ด. รักษางาน Item Master ที่ยังอยู่ใน working tree; ใช้ additive migrations เท่านั้น.
- ทุกข้อความ UI ใช้ `frontend/src/messages/th.json` และ `en.json`; ห้าม `any`, `as any`, `@ts-ignore`, FE join ความสัมพันธ์ด้วย `array.find` หรือเดาค่าทดแทนเมื่อ contract ไม่มีข้อมูล.
- Reuse `IEstimateStore`, `EstimateStore`, Estimate workspace, Cost Resolver, File Service, Audit/Idempotency, Permission Guard, Form/Modal/DataTable primitives ก่อนสร้างของใหม่. หากต้องทำ primitive กลางใหม่ ให้เสนอ Global Reuse ก่อนลงมือ.
- ใช้ Decimal กับเงิน/ปริมาณ, scope จาก PostgreSQL Membership, stable localized Problem Details, `If-Match` สำหรับ mutation ที่มีรุ่น และ Idempotency-Key สำหรับคำสั่งสร้าง/เปลี่ยนสถานะ.
- Published Policy, Approved/Quoted Revision และ Calculation/Cost/Approval Snapshot ห้ามแก้ย้อนหลัง; ห้ามสร้าง Tax Rate, Margin หรือ Approval Threshold จริงจากตัวอย่าง `TEST_ONLY`.

## Scope and evidence

แหล่งกฎหลักคือ [Estimation Flow](../../01-business/estimation-flow.md), [Calculation Rules](../../01-business/estimation-calculation-rules.md), [Approval Matrix](../../01-business/approval-matrix.md), [Field Catalog](../../01-business/official-estimate-field-catalog.md), [API Contract](../../03-contracts/official-estimate-api-contract.md), [Data Contract](../../04-data/official-estimate-data-contract.md) และ [UAT](../../05-engineering/official-estimate-uat-scenarios.md). แผนนี้ไม่เปลี่ยนกฎในเอกสารเหล่านั้นโดยปริยาย; Task 1 ต้องแก้ความไม่ตรงกันระหว่าง contract กับ route จริงก่อนเขียน endpoint ใหม่.

| ส่วน | หลักฐานใน checkout | ช่องว่าง |
| --- | --- | --- |
| Draft, BOQ, Calculate | `EstimatesController` มี Create/Get/UpdateDraft/Calculate; [verification Slice 5A](../../05-engineering/official-estimate-verification.md) บันทึกการทดสอบ ณ 2026-09-21 | เป็นผลทดสอบเดิม ไม่ใช่ผลกับ working tree ปัจจุบัน |
| Item/Cost | `EstimateStore.UpdateDraftAsync` ใช้ Cost Resolver และตรึง catalog cost ที่เลือก; [Item Master verification](../../05-engineering/item-master-estimate-catalog-verification.md) | ต้องตรวจความพร้อมของข้อมูลจริงและกฎ Provisional/Override ก่อน Submit |
| Pricing policy | `EstimateRevision.Calculate` ใช้ `EstimateDefaults.DefaultTaxRate`, policy version คงที่ และ Snapshot สรุปยอด | ยังไม่มี Published Calculation/Tax Policy ที่ resolve ตาม scope/time, overhead, tax mode และ readiness ตาม contract |
| Approval | `EstimatesController` ไม่มี Submit/Review/Cancel/New Revision; `AppDbContext` ไม่มี Approval Request/Step/Decision/Policy | ยังไม่มี Route, Authority, Maker–Checker, immutable Approval Snapshot และ Return flow |
| Quotation gate | `IssueQuotationAsync` ตรวจ `GrandTotal > 0` แล้วเรียก `MarkQuoted`; `MarkQuoted` รับ `draft` | ออก Quotation จาก Draft ได้ ขัดกับ [Estimation Flow](../../01-business/estimation-flow.md) และ [Permission Catalog](../../03-contracts/permission-catalog.md) |
| Quick Estimate | มีเอกสาร/contract แยก แต่ไม่อยู่ในเส้นทาง Official Estimate ปัจจุบัน | แยกเป็นแผนเฉพาะเมื่อเลือกทำ; ไม่เป็นเงื่อนไขก่อนใช้ Official Estimate |

**ขอบเขตแผนนี้:** Official Estimate ตั้งแต่ข้อมูลที่ต้องใช้จริง → คำนวณ → ส่งตรวจ/อนุมัติ → Revision/Cancel → ออก Quotation จาก Approved Revision. ไม่รวม Quick Estimate, Supplier/Procurement, Inventory, Production, Quotation PDF/Amendment/Void หรือการสร้าง Project.

## File ownership map

| ความรับผิดชอบ | ไฟล์หลักที่จะใช้/ขยาย |
| --- | --- |
| Estimate invariant / calculation | `backend/src/TanErp.Domain/Estimates/Estimate.cs`, `EstimateRevision.cs`, `EstimateValues.cs`; เพิ่ม policy/approval types ใน folder เดียวกันเฉพาะที่จำเป็น |
| Use cases / ports | `backend/src/TanErp.Application/Estimates/IEstimateStore.cs`; เพิ่ม feature folders `SubmitEstimate/`, `ReviewEstimate/`, `CreateEstimateRevision/`, `CancelEstimate/` |
| EF persistence | `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`, `AppDbContext.cs`, configurations และ additive migrations ใหม่ |
| HTTP / contracts | `backend/src/TanErp.Api/Controllers/EstimatesController.cs`, `Contracts/Estimates/`, error resources และ OpenAPI |
| Frontend | `frontend/src/features/estimates/api/estimate-queries.ts`, `components/estimate-card.tsx`, `components/estimate-workspace-drawer.tsx`, generated types และสองไฟล์ภาษา |
| Tests / verification | `backend/tests/TanErp.UnitTests/Estimates/`, `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`, `frontend/src/features/estimates/`, `frontend/e2e/official-estimate.spec.ts`, `docs/05-engineering/official-estimate-verification.md` |

## Task 1 — Freeze current contract and close the Draft quotation path

**Files:** `docs/03-contracts/official-estimate-api-contract.md`, `docs/04-data/official-estimate-data-contract.md`, `backend/src/TanErp.Domain/Estimates/EstimateRevision.cs`, `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`, `frontend/src/features/estimates/components/estimate-card.tsx`, `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`, `frontend/src/features/estimates/components/estimate-card.test.tsx`.

**Produces:** Quotation issue ต้องการ Current Revision ที่ `approved` และ Calculation Snapshot ที่ตรง Revision; `draft`, `returned`, `submitted`, `cancelled` ถูกปฏิเสธที่ Backend. เนื่องจากยังไม่มีทางสร้าง Approved Revision ก่อน Task 4 เส้นทางออก Quotation ใหม่จะถูกปิดชั่วคราวโดยตั้งใจ. Task 4 เติมการตรวจ Frozen Approval Snapshot ก่อนเปิดเส้นทางนี้อีกครั้ง. Same-key replay ของ Quotation เดิมยังคืนผลเดิมก่อนตรวจสถานะ.

- [x] เขียน integration test `IssueQuotation_DraftCalculated_ReturnsEstimateInvalidStateWithoutNumberOrStageChange` และ unit test `MarkQuoted_RequiresApprovedRevision`; รันให้เห็นว่า fail กับโค้ดปัจจุบัน.
- [x] ปรับ domain/store ให้บังคับ `approved` + Calculation Snapshot; เก็บ idempotency replay precedence เดิม แล้วรัน test ให้ผ่าน. การตรวจ Approval Snapshot/Hash ยังเป็นงาน Task 4. ปรับ test เดิมที่เคยคาดหวัง Quote จาก Draft ให้คาดหวังการปฏิเสธ โดยยังไม่สร้าง Approved Revision ปลอมใน test.
- [x] ปรับ `EstimateCard` และ component test ให้ไม่เสนอ Issue ใน Draft; ไม่ใช้ FE เป็น security authority.
- [ ] สำรวจ Quotation เดิมที่ออกจาก Draft บนสำเนาฐานข้อมูล: บันทึกจำนวน/ID ที่ได้รับผลโดยไม่เติม Approval ปลอมหรือเปลี่ยน Snapshot ย้อนหลัง; วาง migration/operational disposition แยกหากพบข้อมูลจริง. ยังไม่มี sanitized copy/ข้อมูลจริงให้ตรวจ.
- [x] ปรับ API Contract ให้ตรง routes จริง `PUT /{id}/revisions/{revisionId}/draft` และ `POST /{id}/revisions/{revisionId}/calculate`; ตรวจ contract 5A/5B ว่าไม่สร้างเส้นทางซ้ำ.

## Task 2 — Prove master data and cost readiness for a pilot Estimate

**Files:** `docs/05-engineering/official-estimate-verification.md` (เพิ่มหัวข้อ data readiness evidence), `backend/tests/TanErp.IntegrationTests/Scenarios/ItemCatalogEstimateFlowTests.cs`, เอกสาร data readiness แบบไม่เก็บข้อมูลลูกค้าจริงภายใต้ `docs/05-engineering/` เฉพาะเมื่อมีชุด pilot.

**Produces:** รายงานบน sanitized copy หรือชุดข้อมูลทดสอบที่ระบุ Organization/Branch, Category, Unit/Conversion, Active Item, Published Cost, Source/Evidence, effective period และ reviewer แยกจาก maker; สามารถสร้าง BOQ อย่างน้อยหนึ่งกรณี Material + Labor + Subcontract แล้ว Save/Reload ได้โดยไม่เดาราคา.

- [ ] กำหนดชุดงาน pilot กับผู้ประเมินและ Cost Owner: Work Item, Quantity/Unit, รายการต้นทุน, Branch, วันที่ใช้ราคา และ Cost Evidence. บันทึกค่าเงินจริงเฉพาะในระบบที่ได้รับอนุญาต ไม่ใส่ใน repo.
- [ ] ตรวจรายการ Active ที่ `canCost`, Unit/Conversion และ Published Cost ที่ Cost Resolver เลือกได้ตาม Branch/Quantity/Effective Time; แยก `missing`, `stale`, `ambiguous`, `provisional` เป็นรายงานนับจำนวน.
- [x] เพิ่ม scenario test `PilotEstimate_CatalogCostsResolveAndSnapshotSurvivesNewCostVersion`; รันให้ผ่านด้วยข้อมูล synthetic ที่มี Source/Evidence และ Cost Versions หลายชุด. รันซ้ำหลังเพิ่มไฟล์ evidence, versioned policies และ migration แล้ว; scenario ตรวจ Branch-scoped policy precedence ด้วย.
- [ ] หากข้อมูลจริงยังขาด ให้เติมผ่านหน้าดูแล Item/Cost ที่มีอยู่และ workflow Maker–Checker เดิม; ไม่สร้าง seed ราคา production ใน migration.

## Task 3 — Versioned calculation/tax policy and complete readiness

**Files:** เพิ่ม `backend/src/TanErp.Domain/Estimates/CalculationPolicyVersion.cs`, `TaxPolicyVersion.cs`, `EstimateCalculationSnapshot.cs`; เพิ่ม port สำหรับ resolve policy ใน `backend/src/TanErp.Application/Estimates/`; เพิ่ม configurations/migration หลัง migration ล่าสุดและ store ใน `backend/src/TanErp.Infrastructure/Persistence/Estimates/`; ปรับ `EstimateRevision.cs`, `EstimateStore.cs`, `EstimateProjections.cs`, `backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`.

**Produces:** Published policy ที่ version/effective period/scope ชัด, คำนวณ Overhead, Discount `none|percent|fixed-amount`, Tax `exclusive|inclusive|exempt`, rounding และ `blocked|requiresAttention|ready` ตามเอกสาร. Snapshot เก็บ policy ID/version/hash, input/cost hash, intermediate/rounded totals และ reason pointer. ไม่มี published policy ที่จำเป็นต้องใช้ → `ESTIMATE_POLICY_UNAVAILABLE` แบบ fail-closed.

- [x] เพิ่ม deterministic unit tests สำหรับ margin/markup, overhead remainder และ percentage allocation, discount เกินยอด, tax ทั้ง 3 mode, zero denominator และ calculation invalidation/readiness หลังแก้ Financial Input; Estimate domain tests ผ่าน 24/24.
- [x] ปิด automated test matrix `TC-CALC-EST-001` ถึง `012` และเพิ่ม mapping ไปยัง UAT; ทุกแถวระบุ executable test และ UAT scenario. การทำ UAT จริงยังเป็น release gate ใน Task 7.
- [x] เพิ่ม typed immutable policy version และ migration แบบ additive; PostgreSQL ยืนยัน same-organization FK และ API ปฏิเสธ effective policy ที่ overlap แบบ fail-closed. การป้องกัน ambiguity อยู่ที่ resolver ไม่ใช่ exclusion constraint; ต้องคง test นี้ไว้เมื่อเปลี่ยน resolver. ห้าม publish rate/threshold จากข้อมูล `TEST_ONLY`.
- [x] แยก `Calculate` ออกจากการ Save Draft: Save เปลี่ยนข้อมูลทำ snapshot ล่าสุด outdated; Calculate เพิ่ม version ใน append-only `estimate_calculation_snapshots`; history endpoint อ่านเวอร์ชันเก่าได้. Published policy resolution และการตรึง hash เข้ากับ Approval ยัง pending ในข้อถัดไป.
- [x] ให้ Backend Resolve Cost/Policy ณ context ที่กำหนดและคำนวณด้วย Decimal; ปฏิเสธ client-derived totals. Manual และ catalog cost ที่ไม่มี source/evidence ถูกจัดเป็น provisional; ต้องมี reason ก่อน Submit และส่งเป็น `requiresAttention` ให้ independent reviewer. Cost source/reference/evidence/reason ถูกเก็บใน estimate และ calculation snapshots.
- [x] แยก stale published cost จาก missing cost ด้วย `ITEM_COST_STALE` (409); มี PostgreSQL resolver test สำหรับต้นทุนที่หมด effective period.
- [x] เพิ่มและรัน API tests สำหรับ stale/ambiguous cost, financial edit invalidation และ calculation history ผ่าน Estimate endpoint บน PostgreSQL เฉพาะเครื่อง; ชุดใหม่ผ่านเป็นส่วนหนึ่งของ Estimate API 33/33.

**Progress 2026-09-27–28 (follow-up):** เพิ่ม typed Calculation/Tax Policy แบบ versioned พร้อม Maker–Checker publish, effective period, branch-over-organization resolver แบบ fail-closed, overhead allocation แบบ Decimal/ปันเศษให้ยอดรวมตรง, tax `exclusive|inclusive|exempt`, policy ID/version/hash ใน calculation snapshot และ history response. ปิด TC-011 ด้วย branch-scoped `estimates.override-price`, reason code ที่บังคับสำหรับ Fixed Price, readiness trigger, reason ใน calculation snapshot และ review queue; verification เดิมผ่าน focused domain 29/29, Estimate API 30/30, Fixed Price/review PostgreSQL 4/4, frontend focused 11/11. เพิ่ม API tests สำหรับ stale/ambiguous cost แบบตรวจรหัสและไม่มีการเขียน, financial-edit invalidation และ history หลัง publish future policy; เมื่อรันบน PostgreSQL เฉพาะเครื่อง Estimate API ผ่าน 33/33. การรันพบและแก้ fixture ที่ส่ง `CalculationVersion` เก่าหลัง recalculate. เพิ่ม domain reproduction test ที่คำนวณจาก BOQ ที่ clone จาก revision อนุมัติด้วย policy/cost เดิมหลัง policy ใหม่มีผล; focused test ผ่าน 1/1. เพิ่ม workspace tests สำหรับ duplicate-save lock, conflict input retention และ cost error localization; lifecycle tests สำหรับ confirmation/pending lock; frontend typecheck/lint, Vitest 572/572 และ webpack production build ผ่าน. เพิ่ม UAT-EST-021 และ UAT-EST-022 แล้วแต่ยังไม่ได้รันกับผู้ใช้จริง. ยังเหลือ browser E2E, sanitized legacy migration rehearsal, pilot data, UAT และ Business/Finance/Security sign-off.

### Remaining estimate gaps — execution-ready breakdown

ทำรายการต่อไปนี้ตามลำดับ dependency ด้านล่าง รายการที่ทำได้ใน checkout ให้ปิดด้วย automated test ก่อน; pilot/UAT/sign-off ต้องใช้ environment หรือผู้รับผิดชอบที่มีอำนาจจริง จึงห้ามทำเครื่องหมายผ่านจาก synthetic fixtures.

#### Gap A — Enforce Fixed Price authority (TC-CALC-EST-011)

**Files:** `backend/src/TanErp.Application/Estimates/UpdateEstimateDraft/UpdateEstimateDraftHandler.cs`, `backend/src/TanErp.Application/Estimates/IEstimateStore.cs`, `backend/src/TanErp.Domain/Estimates/EstimateWorkItem.cs`, `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/EstimateWorkItemConfiguration.cs`, `backend/src/TanErp.Api/Contracts/Estimates/UpdateEstimateDraftRequest.cs`, `backend/src/TanErp.Api/Contracts/Estimates/EstimateDetailResponse.cs`, permission/error catalogs, OpenAPI/generated API types, estimate workspace form/schema/messages, `EstimateEndpointsTests.cs`, `EstimateTests.cs`, and estimate component tests.

1. Add a typed `SellingRuleReasonCode` (or equivalent bounded reason field) to the work-item draft contract and persisted estimate work item. Require a non-empty reason whenever `SellingRuleType == fixed_price`; reject a reason on non-fixed methods only if the contract explicitly disallows it.
2. Before saving any draft containing a fixed-price work item, resolve `estimates.override-price` for the same membership and branch scope in addition to `estimates.update`. The backend remains the security authority; hiding the field in React is UX only.
3. Include the fixed-price reason and permission-triggered risk in calculation/readiness and immutable calculation snapshot so Submit resolves an independent approval route. Do not permit a fixed-price draft to report `ready` without that approval trigger.
4. Add a PostgreSQL integration test proving missing permission and missing reason are rejected without changing the draft or row version; add the authorized case proving the fixed price, reason, readiness trigger and snapshot survive save/reload/recalculate. Add bilingual UI labels and field validation, then regenerate OpenAPI/types.
5. Add `UAT-EST-021 — Fixed Price Override Authority` with unauthorized, missing-reason, and authorized maker/checker paths. Pass only when the unauthorized writes are rejected, the reason is visible to the independent reviewer, and the quoted customer projection contains no internal reason or cost data.

**Verification commands:** run `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter FullyQualifiedName~EstimateTests` and `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~FixedPrice`; run `npm --prefix frontend run test -- estimate-work-item-card.test.tsx` after adding the focused component test.

**Status:** Implemented in the current working tree. Domain/API tests prove required reason, branch-scoped authority, non-mutating rejection, persisted snapshot, `requiresAttention`, and reviewer projection; component coverage verifies the editor/review presentation. UAT-EST-021 is specified but still needs execution with authorized pilot roles and customer-safe projection evidence.

**Latest recheck (2026-09-30):** Current backend build passed with 0 warnings/errors; UnitTests passed 288/288 and ArchitectureTests 3/3. IntegrationTests passed 288/289 because one PostgreSQL fixture failed during SSL startup; that test passed on two isolated reruns. The Quotation response allowlist regression passed 1/1. The startup root cause remains unconfirmed; see [verification record](../../05-engineering/official-estimate-verification.md#quotation-api-response-allowlist-2026-09-30).

#### Gap B — Complete calculation evidence matrix (TC-CALC-EST-001–012)

**Files:** `backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`, `backend/tests/TanErp.IntegrationTests/Persistence/CostResolverTests.cs`, `backend/tests/TanErp.IntegrationTests/Scenarios/ItemCatalogEstimateFlowTests.cs`, `docs/01-business/estimation-calculation-rules.md`, `docs/05-engineering/official-estimate-uat-scenarios.md`.

1. Keep the existing direct tests for component rounding, margin, markup, overhead remainder, discount rejection, all tax modes, zero denominator, policy absence/expiry/overlap, replay/key reuse, financial invalidation and ignored client-derived totals. Map each case in the calculation rules table to its test method and UAT case ID.
2. Add Estimate API coverage for stale and ambiguous selected catalog costs. Assert stable Problem Details codes, no Calculation Snapshot append, and unchanged financial revision state.
3. Add a historical reproduction regression: calculate and approve under policy/cost N, publish successor policy N+1 with a later effective period, then clone the immutable approved revision and recalculate its unchanged BOQ using N at the original calculation time. `HistoricalCalculationCanBeReproducedFromRevisionInputsAndOriginalPolicies` passed 1/1; `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` passed as part of `EstimateEndpointsTests` 33/33 against PostgreSQL and checks future-effective policy selection and history retention.
4. Add `UAT-EST-022 — Calculation Replay and Historical Policy` covering same-key retry, same-key/different-payload rejection, and auditor comparison of frozen snapshots across policy versions. UAT-022 is now documented; map TC-009/010 to it plus the existing `UAT-EST-004` for stale readiness.

**Verification commands:** run `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter FullyQualifiedName~EstimateTests` and `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~CalculateEstimate`; run the full IntegrationTests project after cost resolver/API and migration cases are added.

**Acceptance:** every TC row names one executable test and one UAT case; no case is marked passed solely because a lower-level resolver test exists.

#### Gap C — Finish Estimate UI and browser evidence

**Files:** `frontend/src/features/estimates/components/estimate-workspace-drawer.tsx`, `estimate-work-item-card.tsx`, `estimate-lifecycle-actions.tsx`, corresponding component tests, `frontend/e2e/official-estimate.spec.ts`.

1. Add a component test that holds Save pending and verifies duplicate submit is disabled and sends exactly one request.
2. Add a conflict test for HTTP 409 that retains the user's unsaved financial inputs, presents the localized conflict state, and offers the existing reload/compare action without silently overwriting either copy.
3. Verify confirmation modal behavior for high-risk estimate actions and pending-state lock; reuse the shared Modal/ConfirmationModal pattern rather than adding another primitive.
4. Run the authored two-user Playwright journey with frontend, backend, PostgreSQL and Firebase emulator running. Capture the actual result for Submit → Return → edit/recalculate/resubmit → independent Approve → Quote → customer acceptance → New Revision. Fix failures before recording the journey as passed.

**Verification commands:** run `npm --prefix frontend run test -- estimate-lifecycle-actions.test.tsx estimate-workspace-drawer.test.tsx` (use the actual workspace test filename if it is introduced separately), then run `npx playwright test e2e/official-estimate.spec.ts` from `frontend/` while the documented local dependencies are available.

**Acceptance:** focused component tests pass and the browser journey completes against a running local environment; Playwright `--list` alone is not execution evidence.

#### Gap D — Pilot data, migration, UAT and release decision

**Owners/inputs:** Business Owner, Finance, Cost Owner, Estimate pilot user, Security Owner, and an authorized sanitized PostgreSQL environment. Never place customer prices or evidence files in Git.

1. Select one pilot Branch and a representative BOQ containing material, labor and subcontract work; provide quantity/unit, cost owner, effective date and approved evidence in the authorized environment.
2. Report counts for active costable Items and resolvable costs classified as `missing`, `stale`, `ambiguous`, or `provisional`; correct gaps through the existing Item/Cost Maker–Checker screens.
3. Rehearse the additive migrations on an empty PostgreSQL database and a sanitized pre-change copy. Inspect historical Quotations created from Draft before migration; preserve historical approval/audit data and record any compatibility decision.
4. Run applicable `UAT-EST-001` through `UAT-EST-022`, including Thai/English, keyboard, 320px, tablet/200% zoom and reduced motion; record operator, environment, date, result and evidence location in the verification record.
5. Obtain written Business/Finance confirmation of margin/markup defaults, overhead, tax mode/rate, cost staleness, discount authority and reviewer scope. If no signed values exist, keep Production Bootstrap/fail-closed and record “not approved for real-price evaluation.”

**Acceptance:** attach evidence metadata (not sensitive evidence contents), sanitized migration report, UAT results and named sign-offs to `docs/05-engineering/official-estimate-verification.md`; only then reconsider the plan exit criteria.

#### Test-to-UAT mapping baseline

| Calculation case | Existing automated evidence | UAT mapping | Remaining status |
| --- | --- | --- | --- |
| TC-001 | `CostComponent_CalculatesQuantityTimesUnitCostWithPolicyRounding` | UAT-EST-002 | Covered |
| TC-002–003 | `Calculate_WithMarginSellingRule_CalculatesCorrectTotalsAndMarginRate`; `Calculate_UsesDistinctMarginAndMarkupFormulas` | UAT-EST-002 | Covered |
| TC-004 | `Calculate_WithFixedOverhead_DistributesRoundingRemainderAndPricesOnLoadedCost` | UAT-EST-002 | Covered |
| TC-005–006 | `Calculate_RejectsDiscountAboveSellingPriceWithoutChangingSnapshotTotals`; `Calculate_AppliesTaxModeToDiscountedSellingTotal` | UAT-EST-002 | Covered |
| TC-007 | `EvaluateReadiness_RequiresAttentionWhenMarginDenominatorIsZero` | UAT-EST-002 | Covered |
| TC-008 | `CalculateEstimate_WithoutSingleEffectivePolicy_FailsClosed` | UAT-EST-002, UAT-EST-014 | Covered |
| TC-009 | `CalculateEstimate_PercentDiscountPersistsTypedInputAndRequiresReason` (same-key replay and changed-payload rejection); `HistoricalCalculationCanBeReproducedFromRevisionInputsAndOriginalPolicies` (recompute cloned frozen BOQ under original policy after successor becomes effective); `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` (future-effective policy selection and history retention) | UAT-EST-022 | Domain reproduction 1/1; Estimate API history journey passed in PostgreSQL EstimateEndpointsTests 33/33 |
| TC-010 | `FinancialEdit_BlocksReadinessAndPreservesSnapshotUntilRecalculated`; API `FinancialEdit_MarksCalculationOutdatedAndPreservesPriorSnapshot` | UAT-EST-004, UAT-EST-022 | Domain and API automated coverage passed; PostgreSQL EstimateEndpointsTests 33/33 |
| TC-011 | `UpdatingFixedPriceWithoutReason_IsRejected`; `FixedPriceWithReason_RequiresAttentionAndPersistsReasonInSnapshot`; API `UpdateEstimateDraft_FixedPriceWithoutReason_IsRejectedWithoutWriting`, `...WithoutOverridePermission_IsRejectedWithoutWriting`, `...WithFixedPricePermission_PersistsReasonAndRequiresApproval`; review queue projection assertions | UAT-EST-021 | Implementation and automated tests covered; UAT execution pending |
| TC-012 | `CalculateEstimate_PercentDiscountPersistsTypedInputAndRequiresReason` sends extra client-derived totals and asserts server projection | UAT-EST-002 | Covered |

## Task 4 — Bootstrap approval route, Submit, Approve and Return

**Files:** เพิ่ม `backend/src/TanErp.Domain/Estimates/ApprovalPolicyVersion.cs`, `EstimateApprovalRequest.cs`, `EstimateApprovalStep.cs`, `EstimateApprovalDecision.cs`; เพิ่ม use cases/ports ใน `backend/src/TanErp.Application/Estimates/SubmitEstimate/`, `ReviewEstimate/`; เพิ่ม configurations/migration และ `AppDbContext` DbSets; ปรับ `EstimateStore.cs`, `EstimatesController.cs`, request/response contracts, permission seed, error resources และ `EstimateEndpointsTests.cs`.

**Produces:** `POST /api/v1/estimates/{id}/submit` และ `POST /api/v1/estimates/{id}/review-decisions` ตาม API Contract. Submit ตรึง Calculation Hash + Policy Version + Route/Step; Bootstrap บังคับ Independent Checker อย่างน้อยหนึ่งคน, Maker/Last Financial Editor ห้าม Final Approve, ไม่มี auto-approve. Return มี reason และเปิดให้แก้/คำนวณ/ส่งใหม่; Approved Revision immutable.

- [x] เพิ่ม typed approval route/authority และ append-only decision schema พร้อม unique open request ต่อ Revision, concurrency token, tenant FK และ audit ใน transaction เดียว.
- [x] Resolve ผู้ตรวจด้วย permission + organization/branch scope; ตัด maker/last financial editor. ใช้ Bootstrap independent checker โดยไม่สร้าง threshold ตัวเลขจริง.
- [x] เพิ่ม Submit/Review handlers ผ่าน `IEstimateStore`; Idempotency-Key replay มาก่อน state/version check; Problem Details มีรหัสคงที่และคำแปลไทย/อังกฤษ.
- [x] ทดสอบ Return → คำนวณใหม่ → Resubmit → Independent Approve → Issue Quotation บน PostgreSQL; Quote gate ตรวจ immutable approval/calculation snapshot version, input hash, snapshot hash และความตรงกันเชิง JSON ของ snapshot.
- [x] เพิ่ม PostgreSQL tests สำหรับ concurrent decision (ผู้ชนะ 1 ราย อีกคำขอได้ 409), ถอน approval authority ก่อนตัดสิน และ ApprovedRevision ปฏิเสธ draft write; Estimate API tests ผ่าน 23/23.

## Task 5 — New Revision and Cancel lifecycle

**Files:** เพิ่ม use cases `CreateEstimateRevision/`, `CancelEstimate/`; ปรับ `Estimate.cs`, `EstimateRevision.cs`, `EstimateStore.cs`, `EstimatesController.cs`, request/response contracts, migrations เฉพาะเมื่อ schema ต้องเพิ่ม และ `EstimateEndpointsTests.cs`.

**Produces:** `POST /api/v1/estimates/{id}/revisions` จาก Approved/Quoted พร้อม reason และ Draft Revision ใหม่ที่ clone business snapshot ด้วย ID/version ใหม่; `POST /api/v1/estimates/{id}/cancel` จาก Draft/Returned หรือ Submitted เมื่อมี Cancel Authority. Revision เดิม/Quotation เดิมยังอ่านได้; Cancel Submitted ปิด route ที่เปิดอยู่ใน transaction เดียว.

- [x] เพิ่ม domain clone สำหรับ Section/Work Item/Cost Component ด้วย ID ใหม่; Draft revision ใหม่เริ่ม `calculationOutdated=true` และไม่มี Calculation Snapshot.
- [x] เพิ่ม `POST /api/v1/estimates/{id}/revisions` พร้อม reason, permission, If-Match, Idempotency-Key และ localized errors; integration test ผ่านการออก Quotation, สร้าง Revision และ replay โดยยังใช้ revision/quotation เดิม.
- [x] เพิ่ม Cancel lifecycle รวมการปิด approval route ของ Submitted revision ใน transaction เดียว.
- [x] เพิ่ม cross-organization 404 และ cancellation concurrency test. New Revision ได้เพิ่ม cross-organization 404 ใน approval→quotation journey และ PostgreSQL same-version concurrency test (หนึ่ง Created, หนึ่ง 409; มี Revision เดียวเพิ่ม).

## Task 6 — Workspace, review queue and contract parity

**Files:** `frontend/src/features/estimates/api/estimate-queries.ts`, `components/estimate-card.tsx`, `components/estimate-workspace-drawer.tsx`, เพิ่ม review UI ภายใต้ `frontend/src/features/estimates/components/` และ route เฉพาะเมื่อจำเป็น, `frontend/src/messages/th.json`, `en.json`, `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts`, component tests และ `frontend/e2e/official-estimate.spec.ts`.

**Produces:** Estimator เห็น `blocked/requiresAttention/ready` และ reason ที่ focus ถึง field/work item; Submit/Return/Approve/New Revision/Cancel แสดงตาม permission + state; Reviewer เห็น totals, cost evidence, exception, diff และ frozen route; ทุก mutation มี loading lock และ stale-version recovery โดยไม่ทิ้งค่าฟอร์ม.

- [x] Regenerate OpenAPI/types หลัง backend endpoints เสร็จ. OpenAPI contract integration test ผ่านและ frontend generated types สร้างสำเร็จด้วย TypeScript 5.9.3 ที่ `openapi-typescript` ต้องการ; cost provenance และ provisional reason อยู่ใน contract แล้ว.
- [x] เพิ่ม readiness reason focus และ lifecycle component tests สำหรับ permission/state visibility, review decision, Return reason/note, revision reason และ loading states; focused Estimate component tests ผ่าน 17 tests ในรอบก่อนหน้า. เพิ่มข้อความ provisional-cost readiness และ reason input แบบ th/en.
- [x] เพิ่ม component tests สำหรับ cancel confirmation/reason/pending lock, duplicate save lock, 409 conflict input retention และ stale/ambiguous/missing cost error localization; component tests ที่เกี่ยวข้องผ่าน 9/9. ใช้ shared `Modal`, `Button`, `Input` และ semantic tokens.
- [x] Lifecycle controls ใช้ shared `Modal`, `Input`, `Textarea`, `Button`, semantic tokens และ th/en messages.
- [x] เพิ่ม review queue และ frozen route projection ให้เห็นผู้ตรวจ/ขอบเขต/นโยบายที่ถูกตรึง hash ของ route และ calculation, readiness exception, cost source/evidence/provisional reason และ diff ยอดรวม/รายการ BOQ เทียบรุ่นก่อน; API จำกัดผลลัพธ์ตาม reviewer membership ที่มอบหมาย. PostgreSQL journey, OpenAPI parity, component และ sidebar permission tests ผ่าน.
- [x] เพิ่มและรัน Playwright journey สองผู้ใช้: Ready Survey → BOQ → Calculate → Submit → Return → Edit/Recalculate/Resubmit → Independent Approve → Issue Quotation → Customer Acceptance → New Revision. รุ่นวันที่ 2026-09-28 ผ่าน Chromium 1/1.
- [x] รัน browser journey รุ่นปัจจุบันซ้ำหลังเพิ่ม organization billing profile และ primary Billing Address; 2026-09-30 ผ่าน Chromium 1/1 ใน 57 วินาที บน Test API, Firebase Auth Emulator และ disposable PostgreSQL. ตรวจ API URL ผ่าน read-only browser smoke ก่อนเริ่ม workflow. ครอบคลุม Return/Recalculate/Resubmit, Independent Approve, Quotation issue/acceptance, New Revision, English workspace, keyboard focus, reduced motion และ 320px overflow. ดู [Estimate Verification](../../05-engineering/official-estimate-verification.md#browser-e2e-follow-up-2026-09-30).

## Task 7 — Release verification and business sign-off

**Files:** `docs/05-engineering/official-estimate-verification.md`, `docs/05-engineering/official-estimate-uat-scenarios.md` เฉพาะเมื่อพบ scenario ที่ขาด, `docs/README.md` หากมีเอกสาร authoritative ใหม่.

- [x] Code gates: backend build, Unit 287/287, Architecture 3/3, IntegrationTests 289/289 (full solution rerun 2026-09-30), Estimate API 40/40 และ Item Catalog Estimate 1/1 บน PostgreSQL; regression ยืนยันว่า approval snapshot hash ที่ไม่ตรงปฏิเสธการออกใบเสนอราคาโดยไม่เปลี่ยนสถานะหรือเลขเอกสาร. Frontend typecheck/lint, Vitest 597/597, webpack build และ OpenAPI contract parity 1/1 ผ่าน. Latest recheck details are above; the most recent full integration run had one fixture-startup failure, so the suite remains intermittent. [ ] UAT, sanitized data review, pilot data และการลงนามนโยบายยังเป็น release gates.
- [x] รัน PostgreSQL migration rehearsal บนฐานใหม่: isolated PostgreSQL 18 ใช้ migrate ฐานว่าง `tan_erp_rehearsal` จนครบ 39 migrations ถึง `20260927153247_AddEstimateFixedPriceReason`; ยืนยันตาราง snapshot/approval และคอลัมน์ fixed-price reason. [ ] sanitized copy ของข้อมูลเดิมและ audit Quotation ที่เคยออกจาก Draft ยังรอฐานข้อมูลที่ได้รับอนุญาต.
- [ ] รัน UAT-EST-001 ถึง UAT-EST-022 ที่เกี่ยวข้อง โดยเฉพาะ Maker–Checker, Fixed Price Override, Return, Revision, Historical Reproduction, Concurrent Edit และ Customer-safe Quotation Projection. Automated browser coverage now checks the Thai journey, English estimate workspace, keyboard focus within the drawer, 320px overflow, and reduced-motion emulation; authorized-role UAT, tablet/200% zoom, and business acceptance remain pending.
- [ ] ปิดช่องว่างใน UAT baseline ก่อนจบแผน: [x] UAT-EST-003 base-unit API gate/error mapping/component regression (Estimate conversion snapshot ยังเป็นงานต่อเนื่อง); [x] runtime test profile `TEST_ONLY-TH-EST-V1` พร้อม PostgreSQL integration; [x] cross-Organization/Branch action matrix ของ UAT-EST-012 สำหรับ read/update/calculate/approve; [x] UAT-EST-013 บันทึก API projection และมี integration allowlist regression; customer preview/export/document UAT ส่งต่อ CP-04. [x] automated replay ใบเสนอราคาสำเร็จด้วย key เดิมสำหรับ UAT-EST-011/018 ผ่าน PostgreSQL integration journey; การทดสอบ UAT จริงยังค้าง.

> UAT-EST-006 progress: sequential review is covered by `ReviewEstimate_SequentialApprovalRequiresEachAssignedStep`; the test-only profile evaluates combined amount/margin/discount/fixed-price/custom-work-item/provisional-cost triggers, chooses the strongest route, freezes threshold/trigger/reviewer evidence, and projects triggers in the review queue. The Estimate API class passed 40/40 and Item Catalog Estimate flow passed 1/1 on PostgreSQL after the custom-work-item change. Custom work items persist bounded reasons and Item Master snapshots and feed readiness/approval routing. Production keeps the bootstrap checker until policy sign-off. Authorized-user UAT remains open.
- [ ] ให้ Business Owner + Finance ยืนยัน Default Margin/Markup, Overhead, Tax Mode/Rate, Cost Staleness, Discount Authority และรายชื่อ/ขอบเขตผู้ตรวจ. หากยังไม่ยืนยันค่า Policy จริง ให้คง Bootstrap/Fail-closed และระบุชัดว่าไม่เปิดใช้ประเมินราคาจริง.
- [x] บันทึกวันที่/สภาพแวดล้อม/ผลจริง/ข้อจำกัดของ code และ browser gates ใน verification record. [ ] ยังไม่ปิด release: ต้องผ่าน UAT โดยบทบาทที่ได้รับอนุญาต, sanitized legacy audit, pilot data readiness และ Business/Finance/Security sign-off ก่อน.

## Dependency order

```text
Task 1 (close Draft → Quote) ─┬─→ Task 3 (policy/calculation) ─→ Task 4 (approval) ─→ Task 5 (revision/cancel) ─→ Task 6 (UI) ─→ Task 7 (release)
                              └─→ Task 2 (pilot master data) ────────────────────────────────────────────────────────┘
```

Task 2 ทำคู่ขนานกับการพัฒนาเชิงโค้ดได้ แต่ข้อมูลจริงและ Business/Finance sign-off เป็น gate ก่อนใช้งานประเมินราคาจริง. Quick Estimate เป็นแผนแยกและไม่ต้องรอก่อนปิด Official Estimate.
