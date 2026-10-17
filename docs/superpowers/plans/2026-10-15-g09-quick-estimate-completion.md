# G-09 Quick Estimate Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax. Read `AGENTS.md`, `design.md`, `CONTEXT.md` และ `.agents/skills/building-erp-apis/SKILL.md` before Task 2 (`building-erp-forms` อ่านก่อน Task 6 และ Task 8). Reply to the user in Thai; code, identifiers and code comments stay in English. macOS: BSD `sed -i ''`. การแก้ไฟล์เดิมทุกจุดใช้ Edit tool ตาม old_string/new_string ที่ให้ไว้ (ต้องตรงทุกตัวอักษรและพบครั้งเดียว — ไม่ตรงให้หยุดและรายงาน ห้ามเดา).

**กฎของผู้ใช้ที่บังคับทุก Task (override รูปแบบ TDD ปกติ):** รอบนี้ **ห้ามเขียน test ใหม่ทุกชนิด** (ไม่มีไฟล์ test, ไม่มี RED/GREEN) และ implementer **ห้ามรันชุดทดสอบใด ๆ** (`dotnet test`, `vitest`, Playwright) และไม่เปิดเบราว์เซอร์. การตรวจของทุก Task คือ **compile-only**: backend `dotnet build backend/TanErp.slnx --nologo -v q` (ต้อง `0 Warning(s)` `0 Error(s)`; `TreatWarningsAsErrors` เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย — test เดิมต้องยัง compile ได้); frontend `cd frontend && npx tsc --noEmit`, `npm run lint`, key parity th/en และ `npm run build` ครั้งเดียวใน Task 8. `dotnet ef migrations add/has-pending-model-changes`, การ regenerate OpenAPI และ `npm run generate:api` เป็นเครื่องมือ build จึงทำได้; **ห้าม `dotnet ef database update`** (controller เป็นผู้ apply migration). ถ้า test เดิมไม่ compile/ไม่ valid เพราะ Task นี้ ให้แก้ **เฉพาะจุดที่ถูกบังคับ** แบบ minimal และรายงาน (รายการที่คาดไว้อยู่ในตาราง "test/fixture เดิมที่เสี่ยง"). Pitfalls: `Organization` เปล่าชนกับ namespace `TanErp.Application.Organization` / `TanErp.Infrastructure.Persistence.*` → ใช้ `TanErp.Domain.Organization.Organization`/`.Branch` เต็ม; repo เป็น `net10.0`; ถ้า `npm run build` พังที่ไฟล์ generated ให้ลบ `.next/dev` แล้วรันใหม่.

**Goal:** ปิดช่องว่างของ Quick Estimate (CP-16) 3 ข้อ — (1) conversion สร้างร่าง Official Estimate ที่มี **รายการขอบเขต** ตามเส้นวัดของใบประเมิน (ไม่ใช่ร่างเปล่า) โดย idempotent เหมือนเดิม; (2) **Branch Rate Override** ของแม่แบบราคา (อัตราเฉพาะสาขาตามช่วงเวลา) ผ่าน maker–checker และข้ามสาขา/องค์กรไม่ได้; (3) แนบรูป/หลักฐานกับ Quick Estimate ด้วย Shared Attachment (G-01).

**Architecture:** (1) `QuickEstimateConversionMapper` (Application, pure) แปลง calculation snapshot เป็น `EstimateSectionDraftDto`; `QuickEstimateStore.ConvertAsync` เรียก `IEstimateStore.UpdateDraftAsync` เดิมกับร่างที่เพิ่งสร้าง (ไม่แก้ `IEstimateStore`). (2) entity ใหม่ `BranchRateOverride` + ตาราง `quick_estimate.branch_rate_overrides` + store/handler/controller แยก; `QuickEstimateEngine.Calculate` รับ `RateOverride?` ตัวเลือก; Calculate ค้น override ที่ approved ในช่วงวันที่ของ **สาขาของใบประเมิน**. (3) ลงทะเบียน owner type `quick-estimate` ครบสามจุดของ ADR 0017 + ใช้ `AttachmentList` กลาง (เรียก `useDeferredFileUpload` อยู่แล้ว).

**Tech Stack:** .NET `net10.0`, EF Core + Npgsql, PostgreSQL, Next.js + TypeScript strict, next-intl, TanStack Query, react-hook-form + zod.

## Scope

**ทำ:** conversion พร้อมรายการขอบเขต; Branch Rate Override (request → approve/return → revoke, ใช้ตอนคำนวณ, UI); evidence ด้วย G-01; สัญญา API/OpenAPI/เอกสาร. **ไม่ทำโดยตั้งใจ:** การเขียน/รัน test (คำสั่งผู้ใช้); **ต้นทุนจาก template** และ **การใช้ `IUnitConversionResolver`/snapshot ของ G-08** — แม่แบบไม่มี mapping ไป Item/ต้นทุนเลย (F1) และหน่วยของแม่แบบเป็น label อิสระ (`m2`) ไม่ใช่หน่วย Item; backfill conversion เดิม; override ของ factor/add-on/ช่วงความเสี่ยง; promotion/manual price override/mobile; แก้ branch scope ของ endpoint Quick Estimate เดิม (F8); ยกเลิกคำขอโดยผู้ขอ (ผู้อนุมัติ "ส่งกลับ" ได้); เปลี่ยนช่องกรอก Site Survey Revision เป็น dropdown (F14).

## Findings จากการสำรวจโค้ด (ตรวจแล้ว)

| # | หัวข้อ | สิ่งที่พบ | ผลต่อแผน |
| --- | --- | --- | --- |
| F1 | **Template ไม่มี "item/line mapping"** | `PricingTemplate` มีแค่ rate/ช่วง/ภาษี + `ConfigJson` (grade/complexity/add-on/modifier) (`Domain/QuickEstimates/PricingTemplate.cs:54-82`, `TemplateConfig` `:13-21`); ไม่มี `BranchId`/Item/Cost. สิ่งที่ map ได้คือ **เส้นวัด** ที่ตรึงใน snapshot `lines[]` + `template.unitCode` (`QuickEstimateEngine.cs:138-141`) | "รายการ" = 1 Work Item ต่อเส้นวัด; **ไม่มีราคา/ต้นทุน** (ตรงกฎ `quick-estimate-api-contract.md:196`) |
| F2 | Conversion วันนี้สร้างร่างเปล่า | `ConvertAsync` เรียก `_estimates.CreateDraftAsync` (`QuickEstimateStore.cs:526`) แล้วเก็บ snapshot (`:545-546`); สัญญาระบุ "ร่างเปล่า" (`quick-estimate-api-contract.md:250`) | เติมรายการหลัง CreateDraft ก่อนบันทึก conversion |
| F3 | `CreateDraftAsync` มี tx ของตัวเองและคืน estimate เดิมถ้า Opportunity มีแล้ว | `EstimateStore.cs:75-77` (tx), `:154-165` (คืน existing ไม่ตรวจสถานะ/รายการ), `:79-111` (replay) | เติมเฉพาะร่างที่ว่างจริง (draft, revision 1, ไม่มี section); retry หลังเติมสำเร็จ → ไม่ว่างแล้ว → ข้าม → นับรายการจาก projection |
| F4 | มีทางเขียนร่างอยู่แล้ว | `IEstimateStore.UpdateDraftAsync` (`IEstimateStore.cs:41-48`) ตรวจ `RowVersion` (`EstimateStore.cs:345`) และสถานะ draft (`:348`); work item ไม่มี `ItemId` ผ่าน `SetCustomWorkItemReason` (`:408-410`) | ไม่แก้ `IEstimateStore` → `FakeEstimateStore` (`GetQuotationDocumentHandlerTests.cs:36`) ไม่พัง |
| F5 | readiness ของร่างที่เติม | ไม่มี cost → `ESTIMATE_COST_INCOMPLETE`; ไม่มี Item → ต้องมี reason code+reason (`EstimateRevision.cs:55-68`) | ใส่ `overrideReasonCode=QUICK_ESTIMATE_CONVERSION`, `overrideReason="{เลข QE} · v{n}"`; Estimator ต้องเติมต้นทุนก่อนคำนวณ (ตั้งใจ) |
| F6 | maker–checker เดิม | แม่แบบ: ผู้สร้าง/ผู้ส่งตัดสินไม่ได้ (`PricingTemplate.cs:173-179`); review: DB check `decided_by <> requested_by` (`QuickEstimateConfigurations.cs` `ck_quick_estimate_reviews_decider`) | ใช้แบบเดียวกัน (domain + check constraint) |
| F7 | `RoleChangeRequest` (G-03b) ใช้ตรง ๆ ไม่ได้ | ผูก `RoleId`/`BaseRoleRowVersion`/`ProposedGrantsJson` (`Domain/IdentityAccess/RoleChangeRequest.cs:14-26`) | ยืมเฉพาะ **รูปแบบ** (ผู้ขอ ≠ ผู้ตัดสิน, `RowVersion`); entity ใหม่ — ADR 0025 |
| F8 | Endpoint Quick Estimate เดิมไม่ตรวจ branch | กรองเฉพาะ `OrganizationId` (`QuickEstimateStore.cs:556-558`, `QuickEstimateHandler.cs:200`); มี `BranchId` ตอนสร้างเท่านั้น (`:282`) | ไม่แก้ (blast radius); ส่วนใหม่ตรวจ branch เอง — evidence ผ่าน `AttachmentHandler.cs:58-63` |
| F9 | Governance กำหนด override ไว้แล้ว | `pricing-template-governance.md:57,127-142`; permission `pricing-templates.override-rate` ใน catalog ส่วนทิศทาง (`permission-catalog.md:125`) แต่ **ยังไม่ seed** (`TestOnlyDataSeeder.cs:197-199`); ตาราง `branch_rate_overrides` ระบุใน `quick-estimate-data-contract.md:86` | ใช้ชื่อตาราง/permission ตามเอกสาร; implement ชุดย่อย |
| F10 | จุดเลือก template ตอนคำนวณ | `CalculateAsync` โหลด template จาก `e.TemplateId` (`QuickEstimateStore.cs:386-390`), `Calculate(template, input, Today)` (`:390`); `InputHash` (`QuickEstimateEngine.cs:152-166`) ไม่รู้จัก override; reuse ผลเมื่อ hash+template เท่ากัน (`:392`) | ต่อที่จุดเดียว; ใส่ override ใน hash/snapshot **เฉพาะเมื่อมี** → ผลเดิมเท่าเดิมทุกไบต์ |
| F11 | G-01 ใช้งานได้จริง | ลงทะเบียน owner 3 จุด: `AttachmentValues.cs:17-25`, `AttachmentOwnerRegistry.cs:20-29`, `AttachmentOwnerScopeReader.cs:13,22-33`; Files รับ parent ผ่าน registry อัตโนมัติ (`FileUploadSession.cs:25-26`, `FileParentAccessResolver.cs:312-351`); limit 10 MB JPEG/PNG/WebP (`CreateUploadSessionHandler.cs:15,103-114`), ≤ 50 active/owner (`AttachmentLink.cs:8`), ≤ 20 ไฟล์/คำขอ (`AttachmentHandler.cs:15`); FE `AttachmentList` ใช้ `useDeferredFileUpload` (`AttachmentList.tsx:46,61`), ใช้ที่ `installation-detail.tsx:247` | evidence ไม่ต้องมี permission/error/migration ใหม่ |
| F12 | Descriptor บังคับ `SignPermission`/`SignStates` | `AttachmentOwnerRegistry.cs:9-15` | `SignPermission=quick-estimates.update`, `SignStates` ว่าง → ลายเซ็นถูกปฏิเสธ `ATTACHMENT_OWNER_LOCKED` |
| F13 | `pricing-templates.approve` ไม่ใช่ approval-class | ADR 0020 การตัดสินใจ 5; `ApprovalPermissionCoverageTests.cs:13-18` | ผู้อนุมัติ override = `pricing-templates.approve` เดิม; key ใหม่ `override-rate` ไม่ลงท้าย `approve/publish/review` → ไม่เข้า test กวาด, ไม่แก้ `ApprovalPermissionKeys` |
| F14 | UI Conversion บังคับพิมพ์ Site Survey Revision ID | `quick-estimate-workspace.tsx` ช่อง `surveyRevisionId` | นอกขอบเขต |

### ข้อที่แผนหลัก (gap plan) คลาดเคลื่อน

1. "ตาม template mapping" — template **ไม่มี** mapping; ใช้เส้นวัดของใบประเมินแทน (F1). 2. หมายเหตุ G-08 "G-09 ต้องสร้างแถวต้นทุนจาก template ผ่าน `IUnitConversionResolver`" — ไม่เป็นจริงในรอบนี้ (ไม่มีแถวต้นทุน; ต้องมี decision N1). 3. "Branch override ของ template" — template เป็นระดับ Organization ล้วน (ไม่มี `BranchId`) → override เป็นเอนทิตีใหม่. 4. Test "override ข้าม branch ไม่ได้" ทำเป็น **กฎในโค้ด** (request ไม่มี `branchId`; นอกสาขา = 404) แต่ไม่เขียน test ตามคำสั่งผู้ใช้.

## Decisions ที่ผู้ใช้อาจ veto

| # | ตัดสินใจ | เหตุผล / ทางเลือกที่ไม่เลือก |
| --- | --- | --- |
| D1 | **"รายการ" = Work Item ต่อเส้นวัด**: 1 section `QE` (ชื่อ = `roomOrArea`) + work item `QE-01..` (คำอธิบาย `{templateCode} · {workSubtype หรือ roomOrArea}`, ปริมาณ = `billableQuantity`, หน่วย = `template.unitCode`, margin 0.30 = ค่าเริ่มต้นของ `EstimateWorkItem`) ไม่มี cost และไม่คัดลอกจำนวนเงิน | ตรงกฎ "ไม่ใช้ช่วงราคาเป็นราคาขาย"; ทางเลือก: คัดลอก `adjustedAmount` เป็นราคาคงที่ (ขัดกฎ + ต้องมี override-price) |
| D2 | เติมเฉพาะ **ร่างที่ว่างจริง**; ถ้า Opportunity มี Estimate ที่มีงานอยู่แล้ว → ผูก conversion กับของเดิมเหมือนเดิมโดยไม่แตะรายการ | ไม่ทำลายงาน Estimator; ทางเลือก: ปฏิเสธ conversion (เปลี่ยนพฤติกรรมเดิม) |
| D3 | **ไม่ backfill** ใบที่ converted แล้ว (ร่างเปล่าคงอยู่; แถวเก่าไม่มี `mappedLineCount` → `null` = ไม่ระบุ) | ร่างเดิมอาจถูกแก้แล้ว; ไม่มี data migration |
| D4 | Branch Override = **เอนทิตีใหม่** `BranchRateOverride` ผูก **template version เดียว**; template ออก version ใหม่/superseded/disabled → override เลิกมีผล (ขอใหม่) | ADR 0025; ทางเลือก: template ที่มี `BranchId` (กระทบ unique index/effective list ทั้งโมดูล) |
| D5 | override แทนเฉพาะ `referenceRate`/`minimumCharge`; ช่วงเวลา ≤ 366 วัน, `effectiveTo ≥ วันนี้`, ห้ามซ้อนกับ `requested`/`approved` ของสาขา+template เดียวกัน; ไม่มีเพดาน % | ชุดย่อยของ governance (EX-PT-002); ดู N3 |
| D6 | สิทธิ์: ขอ = **`pricing-templates.override-rate` (ใหม่ ตามชื่อใน governance/catalog)**; อนุมัติ/ส่งกลับ/เพิกถอน = `pricing-templates.approve`; อ่าน = `pricing-templates.read`. ไม่แก้ `ApprovalPermissionKeys` (F13) | ผลคือ **ช่วงราคาเบื้องต้น** ไม่ใช่ราคาในใบเสนอ; ทางเลือก: ใช้ `pricing-templates.manage` ขอแทน (ไม่เพิ่ม key แต่ผู้ดูแลสาขาต้องแก้แม่แบบองค์กรได้) |
| D7 | ข้ามสาขาไม่ได้โดยโครงสร้าง: request ไม่มี `branchId` (ใช้ `access.BranchId`; ไม่มี → `ACTIVE_BRANCH_REQUIRED`); ตัดสิน/เพิกถอน/อ่านนอกสาขา → `404`; Membership ระดับ Organization เห็นทุกสาขาใน org เดียวกัน; คำนวณค้นด้วย `BranchId` ของ **ใบประเมิน**; ทุก query กรอง `OrganizationId` | ตรง `TC-QE-019` |
| D8 | Evidence: owner `quick-estimate`; อ่าน `quick-estimates.read`; แนบ/ถอน `quick-estimates.update` เฉพาะสถานะ `draft/calculated/returned`; purposes UI = `evidence`,`general`; ไม่มีลายเซ็น | หลักฐานต้องตรงกับที่ผู้ตรวจเห็น |
| D9 | `mappedLineCount` เก็บใน `source_snapshot` ของ conversion และส่งใน `ConversionResponse` | ไม่ต้องมี migration เพิ่ม |
| D10 | Error ใหม่ 4 ตัว: `BRANCH_OVERRIDE_INVALID` (422), `_OVERLAP` (409), `_INVALID_STATE` (409), `_SELF_APPROVAL` (403); ใช้ซ้ำ `PRICING_TEMPLATE_NOT_USABLE`, `PRICING_TEMPLATE_VERSION_CONFLICT`, `QUICK_ESTIMATE_REASON_REQUIRED`, `RESOURCE_NOT_FOUND`, `ACTIVE_BRANCH_REQUIRED`, `IDEMPOTENCY_KEY_REUSED` | ลด code ใหม่ |
| D11 | ไม่มี test ใหม่ (คำสั่งผู้ใช้) | ตารางด้านล่างระบุว่าแต่ละ test เดิมยังถูกต้องเพราะอะไร |

## Decisions needed (ต้องให้ธุรกิจ/ผู้ใช้ตัดสิน — แผนเดินหน้าด้วยคำแนะนำ)

| # | คำถาม | คำแนะนำ |
| --- | --- | --- |
| N1 | ให้แม่แบบเก็บ "สูตรต้นทุนจาก Item" เพื่อให้ร่างที่แปลงมี cost component ด้วยหรือไม่? | **เลื่อน** — ต้องออกแบบ schema/ฟอร์ม/governance ของ template และ Production ยังไม่มีค่าราคา; ถ้าทำ ต้องผ่าน `IUnitConversionResolver` + กติกาตรึงของ G-08 D5 |
| N2 | Margin เริ่มต้น 0.30 ของรายการที่แปลง ยอมรับได้หรือไม่ | ใช้ค่าเริ่มต้นของโดเมนไปก่อน; Estimator แก้ได้ก่อนคำนวณ |
| N3 | override ต้องมีเพดาน % / จำกัด 366 วัน / แนบหลักฐานไฟล์ก่อนอนุมัติหรือไม่ | เริ่มด้วย 366 วัน + `sourceReference` ข้อความ; เพดาน % และหลักฐานไฟล์เป็น follow-up (Cost Owner ยืนยัน) |
| N4 | ผู้อนุมัติ override ใช้ `pricing-templates.approve` ร่วมกับการอนุมัติแม่แบบ (ไม่เป็น approval-class) ได้หรือไม่ | ได้ (F13); ถ้าต้องแยก → เพิ่ม key + แก้ ADR 0020/`ReviewedNotApproval` |

## test/fixture เดิมที่เสี่ยง (ตรวจแล้ว — ต้องไม่ถูกทำให้พัง)

| ไฟล์ | จุดเสี่ยง | คงความถูกต้องอย่างไร |
| --- | --- | --- |
| `backend/tests/TanErp.UnitTests/Estimates/GetQuotationDocumentHandlerTests.cs:36` | `FakeEstimateStore : IEstimateStore` | ไม่แก้ `IEstimateStore` |
| `backend/tests/TanErp.UnitTests/QuickEstimates/QuickEstimateEngineTests.cs` (:23 ctor, :44-140 `Calculate`) | signature engine / ctor `PricingTemplate` | `PricingTemplate` ไม่เปลี่ยน; พารามิเตอร์ใหม่ของ `Calculate` เป็นตัวเลือกท้ายสุด; ไม่มี override → ผลเท่าเดิม |
| `backend/tests/TanErp.IntegrationTests/Api/QuickEstimateEndpointsTests.cs` (:234-290: `Contains("86120")`, hash 64 ตัว, reuse v1, 67,400) | ผลคำนวณ/แชร์ | ไม่มี override ในข้อมูลทดสอบ; field ใหม่ของ response เป็น optional ท้ายสุด |
| `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs:3386-3457` | conversion ตอนนี้สร้างรายการ; assert แค่ `Contains("draft")`, replay ได้ `OfficialEstimateId` เดิม, `QUICK_ESTIMATE_ALREADY_CONVERTED`, `QUICK_ESTIMATE_FIELD_REQUIRED` | ยังจริงทั้งหมด (ลำดับตรวจเดิมคงเดิม; เติมรายการก่อนบันทึก conversion) |
| `…/UnitTests/Attachments/AttachmentHandlerTests.cs:42-51`, `…/IntegrationTests/Persistence/AttachmentOwnerScopeReaderParityTests.cs:13-18`, `…/UnitTests/Files/FileGatePermissionsTests.cs` | registry/whitelist/scope reader ต้องตรงกัน | Task 6 แก้ทั้งสามชุดใน commit เดียว; assertion เป็น `SetEquals`/`Contains` |
| `…/UnitTests/IdentityAccess/ApprovalPermissionCoverageTests.cs` | กวาดคีย์ `*.approve/publish/review` ใน seeder | `override-rate` ไม่ตรงรูปแบบ |
| `…/IntegrationTests/Api/OpenApiContractTests.cs` | swagger สด vs `contracts/openapi/tan-erp.v1.json` | Task 7 regenerate (ระหว่าง Task 4–6 snapshot ค้าง ไม่มี test รัน) |
| `frontend/src/lib/attachments/attachment-owner-types.test.ts:15` | `toEqual(["installation-job"])` | **ถูกบังคับ**: แก้เป็น `["installation-job", "quick-estimate"]` (Task 6) |
| `frontend/src/features/quick-estimates/components/quick-estimate-components.test.tsx` | workspace จะ render `AttachmentList` (react-query) โดยไม่มี `QueryClientProvider` | **ถูกบังคับ**: เพิ่ม `vi.mock("@/components/forms/AttachmentList", …)` เหมือน `service-components.test.tsx:42` (Task 6); `PricingTemplateDetail` เพิ่มแค่ `Link` + key ใน messages |

## Task Map

| Task | เรื่อง | commit |
| --- | --- | --- |
| 1 | ADR 0025 + สัญญา/permission/attachment/error/ศัพท์ (docs) | `docs(quick-estimates): add ADR 0025 and contracts for G-09` |
| 2 | Domain `BranchRateOverride` + engine `RateOverride?` + EF + migration | `feat(quick-estimates): add the branch rate override entity and an engine override input` |
| 3 | Store/handler ของ override + ต่อเข้า Calculate + `branchOverrideId` | `feat(quick-estimates): request, decide and apply branch rate overrides` |
| 4 | API: controller/contracts/error codes/permission/seeder/DI | `feat(quick-estimates): expose the branch rate override api` |
| 5 | Conversion สร้างรายการขอบเขต | `feat(quick-estimates): create scope lines when converting a quick estimate` |
| 6 | Evidence: owner `quick-estimate` (backend 3 จุด) + `AttachmentList` ใน workspace | `feat(quick-estimates): attach photo evidence to a quick estimate` |
| 7 | OpenAPI snapshot + generated types + api-client | `feat(quick-estimates): publish the override and conversion contract` |
| 8 | Frontend: หน้า override, permission, i18n, ข้อความ conversion | `feat(quick-estimates): add the branch rate override screen` |
| 9 | Verification + สถานะ (ไม่ติ๊ก) | `docs(quick-estimates): record G-09 verification and update status` |

ลำดับคอมไพล์ได้ทีละ commit: Task 2 เพิ่ม type/พารามิเตอร์ตัวเลือก (ไม่มีผู้เรียก); Task 3 ใช้ type จาก Task 2, field ท้าย record มี default; Task 4 ใช้ handler จาก Task 3 (DI ใน Task 4); Task 5 ใช้ `IEstimateStore` เดิม; Task 6 แก้ registry สามชุดพร้อมกัน; Task 7 หลัง Task 3–5; Task 8 ใช้ type จาก Task 7.

> **รูปแบบ patch (`text`):** `FILE <path>` ตามด้วยขั้นตอน — `>>> หลัง: X` = แทรกบรรทัดที่ตามมา (จนถึง `>>>` ถัดไปหรือ `FILE` ถัดไป) ต่อท้ายบรรทัดเดียวที่มี `X`; `>>> แทนที่:` … `>>> ด้วย:` … = Edit (old_string → new_string). `X`/old_string ต้องพบครั้งเดียวในไฟล์ — ไม่ตรงให้หยุดและรายงาน.

## Task 1: ADR 0025, สัญญา และศัพท์ (docs เท่านั้น)

**Files:** Create: `docs/adr/0025-quick-estimate-branch-override-and-scope-line-conversion.md`; Modify: `docs/adr/README.md`, `CONTEXT.md`, `docs/03-contracts/quick-estimate-api-contract.md`, `docs/03-contracts/permission-catalog.md`, `docs/03-contracts/attachment-api-contract.md`, `docs/03-contracts/error-contract.md`

- [ ] **Step 1: สร้าง ADR**
```markdown
---
status: accepted
---

# Branch Rate Override ของแม่แบบราคา และ Conversion ที่สร้างรายการขอบเขต

Quick Estimate (CP-16) แปลงเป็นร่าง Official Estimate เปล่า ไม่มีราคาเฉพาะสาขา และแนบหลักฐานไม่ได้. ADR นี้บันทึก 3 การตัดสินใจของ G-09.

**Conversion:** แม่แบบไม่มี mapping ไป Item/ต้นทุน จึงแปลง **เส้นวัด** ใน calculation snapshot เป็น Work Item ขอบเขต (ปริมาณคิดราคา + หน่วยของแม่แบบ) ไม่มี cost และไม่คัดลอกจำนวนเงิน. เติมเฉพาะร่างที่ว่างจริง (ไม่ทับงานเดิม, retry ปลอดภัย). ไม่ backfill conversion เดิม.

**Branch Rate Override:** เอนทิตีใหม่ `BranchRateOverride` (`quick_estimate.branch_rate_overrides`) ผูก template version เดียวและสาขาเดียว แทนเฉพาะ `referenceRate`/`minimumCharge` ในช่วงวันที่ ≤ 366 วัน โดยไม่แก้ template (ชุดย่อยของ `pricing-template-governance.md` หัวข้อ Branch Rate Override). ผู้ขอ (`pricing-templates.override-rate`) ≠ ผู้ตัดสิน (`pricing-templates.approve`) บังคับทั้งใน domain และ DB check. ไม่ใช้ `RoleChangeRequest` (ผูกกับ Role/สิทธิ์) และไม่ทำ template ที่มี `BranchId` (กระทบ unique index/effective list ทั้งโมดูล). ข้ามสาขาไม่ได้: request ไม่รับ `branchId`, นอกสาขา = 404, คำนวณค้นด้วยสาขาของใบประเมิน. Override อยู่ใน snapshot และ `InputHash` เฉพาะเมื่อมี.

**Evidence:** ลงทะเบียน owner type `quick-estimate` ใน Shared Attachment (ADR 0017) ด้วย permission เดิมของโมดูล.

ข้อเสียที่ยอมรับ: `pricing-templates.approve` อนุมัติราคาเฉพาะสาขาโดยไม่เป็น approval-class (ADR 0020) เพราะผลคือช่วงราคาเบื้องต้น; override ไม่ข้าม template version; endpoint Quick Estimate เดิมยังไม่ตรวจ branch.
```
- [ ] **Step 2: patch เอกสารอื่น**
```text
FILE docs/adr/README.md
>>> หลัง: - [0024 — Estimate Unit Conversion Snapshot](0024-estimate-unit-conversion-snapshot.md)
- [0025 — Branch Rate Override ของแม่แบบราคา และ Conversion ที่สร้างรายการขอบเขต](0025-quick-estimate-branch-override-and-scope-line-conversion.md)

FILE CONTEXT.md
>>> หลัง: _Avoid_: Draft Template, Current Settings
**Branch Rate Override (ราคาเฉพาะสาขา)**:
อัตราอ้างอิง/ราคาขั้นต่ำเฉพาะสาขาของ Pricing Template Version หนึ่งในช่วงวันที่ที่กำหนด ต้องผ่านผู้อนุมัติอิสระ ไม่แก้อัตรามาตรฐาน และหมดผลเมื่อพ้นช่วงเวลาหรือแม่แบบเปลี่ยนรุ่น
_Avoid_: Branch Template, Discount

FILE docs/03-contracts/quick-estimate-api-contract.md
>>> แทนที่:
Conversion คัดลอก Scope/Measurement/Assumption เป็น Source Snapshot แต่ต้องคำนวณ Official Estimate ใหม่ ไม่ใช้ค่ากลางของ Price Range เป็นราคาขาย
>>> ด้วย:
Conversion คัดลอก Scope/Measurement/Assumption เป็น Source Snapshot และสร้างร่าง Official Estimate ที่มี **รายการขอบเขต** (1 section ชื่อ = `roomOrArea`; 1 work item ต่อเส้นวัด: ปริมาณ = `billableQuantity`, หน่วย = `unitCode` ของแม่แบบ, ไม่มี cost/ราคา) แต่ต้องเติมต้นทุนและคำนวณ Official Estimate ใหม่ ไม่ใช้ค่ากลางของ Price Range เป็นราคาขาย. เติมเฉพาะร่างที่ว่างจริง (Opportunity ที่มี Estimate ที่มีงานแล้ว → ผูกกับของเดิมโดยไม่แตะรายการ); response มี `mappedLineCount`; ใบที่แปลงก่อน G-09 ไม่ถูก backfill.

## Branch Rate Override (G-09)

Base `/api/v1/pricing-templates/{templateId}/branch-rate-overrides` (ต้อง `Authorization` + `X-Membership-Id`): `POST` + `Idempotency-Key` (`pricing-templates.override-rate`; body `{referenceRate, minimumCharge, effectiveFrom, effectiveTo, reasonCode, reasonDetail, sourceReference?}`; สาขา = สาขาของ Membership ไม่รับ `branchId`; template ต้อง `calibration`/`active`) → 201; `GET` (`pricing-templates.read`; Membership ผูกสาขาเห็นเฉพาะสาขาตน); `POST {id}/decision` + `If-Match` (`pricing-templates.approve`; `{decision: approved|returned, note}`; ผู้ขอตัดสินเองไม่ได้); `POST {id}/revoke` + `If-Match` (`pricing-templates.approve`; `{reason}` บังคับ; เฉพาะ `approved`). สถานะ `requested → approved | returned`, `approved → revoked`. ช่วง ≤ 366 วัน, `effectiveTo` ≥ วันนี้, ห้ามซ้อนกับ `requested`/`approved` ของสาขา+template เดียวกัน. Calculate ใช้ override ที่ `approved` ในช่วงวันที่ของ **สาขาของใบประเมิน**; ผลบันทึก `branchOverride` ใน snapshot และ `CalculationResponse.branchOverrideId`. นอกสาขา/องค์กร = `404 RESOURCE_NOT_FOUND`. Error: `403 BRANCH_OVERRIDE_SELF_APPROVAL`; `409 BRANCH_OVERRIDE_OVERLAP`, `BRANCH_OVERRIDE_INVALID_STATE`, `PRICING_TEMPLATE_NOT_USABLE`, `PRICING_TEMPLATE_VERSION_CONFLICT`; `422 BRANCH_OVERRIDE_INVALID`, `QUICK_ESTIMATE_REASON_REQUIRED`, `ACTIVE_BRANCH_REQUIRED`.
>>> แทนที่:
- ไม่มี Branch override, promotion, manual override ของราคา และไม่มีการแนบหลักฐาน/อัปโหลดรูป
>>> ด้วย:
- ไม่มี promotion และ manual override ของราคา; Branch override ทำเฉพาะ `referenceRate`/`minimumCharge`; หลักฐานรูปแนบผ่าน Shared Attachment (owner `quick-estimate`, ดู [Attachment Contract](attachment-api-contract.md))
>>> แทนที่:
- Conversion สร้าง **ร่าง Official Estimate เปล่า** และเก็บ snapshot ของ Quick Estimate ใน `quick_estimate_conversions` เป็นอ้างอิงต้นทาง ไม่คัดลอกรายการเข้า Estimate; ต้องระบุ Site Survey Revision
>>> ด้วย:
- Conversion สร้างร่าง Official Estimate ที่มีรายการขอบเขตต่อเส้นวัด (ไม่มีต้นทุน/ราคา — แม่แบบไม่มี mapping ไป Item) และเก็บ snapshot ใน `quick_estimate_conversions`; ต้องระบุ Site Survey Revision

FILE docs/03-contracts/permission-catalog.md
>>> หลัง: | Quick Estimate | `pricing-templates.approve` | อนุมัติ/ส่งกลับ/ปรับเทียบ/เปิดใช้/ปิดใช้แม่แบบราคา (ห้ามอนุมัติของตัวเอง) | Organization |
| Quick Estimate | `pricing-templates.override-rate` | ขอ Branch Rate Override ของแม่แบบ (สาขาของ Membership); ผู้อนุมัติ/เพิกถอนใช้ `pricing-templates.approve` | Organization |

FILE docs/03-contracts/attachment-api-contract.md
>>> แทนที่: (รอบแรก: `installation-job`)
>>> ด้วย: (`installation-job`, `quick-estimate`)
>>> หลัง: | `installation-job` | `installations.read` | `installations.operate` | `installations.handover` | `planned`, `in_progress`, `ready_for_handover` | `ready_for_handover` |
| `quick-estimate` | `quick-estimates.read` | `quick-estimates.update` | — (ไม่มีลายเซ็น) | `draft`, `calculated`, `returned` | — |

FILE docs/03-contracts/error-contract.md
>>> หลัง: | `PRICING_TEMPLATE_INVALID_STATE` | 409 |
| `BRANCH_OVERRIDE_INVALID` | 422 | Branch Rate Override ไม่ผ่าน: อัตรา/ขั้นต่ำ, ช่วงวันที่ (> 366 วันหรือสิ้นสุดก่อนวันนี้), เหตุผลขาด |
| `BRANCH_OVERRIDE_OVERLAP` | 409 | ช่วงเวลาซ้อนกับคำขอ/ราคาเฉพาะสาขาของ template เดียวกันในสาขาเดียวกัน |
| `BRANCH_OVERRIDE_INVALID_STATE` | 409 | Action ใช้กับสถานะปัจจุบันของ override ไม่ได้ |
| `BRANCH_OVERRIDE_SELF_APPROVAL` | 403 | ผู้ขอ override ตัดสินคำขอของตนเองไม่ได้ |
```
- [ ] **Step 3: ตรวจ** — `grep -c "0025" docs/adr/README.md` ≥ 1; `grep -n "override-rate" docs/03-contracts/permission-catalog.md` เห็น 2 แถว
- [ ] **Step 4: Commit**
```bash
git add docs/adr docs/03-contracts CONTEXT.md
git commit -m "docs(quick-estimates): add ADR 0025 and contracts for G-09

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 2: Domain `BranchRateOverride` + engine `RateOverride?` + EF + migration

**Files:** Create: `backend/src/TanErp.Domain/QuickEstimates/BranchRateOverride.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/BranchRateOverrideConfiguration.cs`; Modify: `backend/src/TanErp.Domain/QuickEstimates/QuickEstimateEngine.cs`, `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`; Generate: migration `AddBranchRateOverrides`

- [ ] **Step 1: สร้าง `BranchRateOverride.cs`**
```csharp
using TanErp.Domain.Common;

namespace TanErp.Domain.QuickEstimates;

public static class OverrideStatus
{
    public const string Requested = "requested", Approved = "approved", Returned = "returned", Revoked = "revoked";
}

/// <summary>The rate the engine prices with instead of a template's standard rate (one approved branch override).</summary>
public sealed record RateOverride(Guid Id, decimal ReferenceRate, decimal MinimumCharge);

/// <summary>
/// A branch-specific reference rate and minimum charge for one template version over a limited period; it never edits the template.
/// Maker–checker: the requester cannot decide it. It applies only to quick estimates of its own branch while approved and in period.
/// </summary>
public class BranchRateOverride : Entity
{
    public const int MaxPeriodDays = 366;
    public Guid OrganizationId { get; private set; } public Guid BranchId { get; private set; }
    public Guid TemplateId { get; private set; } public string Status { get; private set; } = OverrideStatus.Requested;
    public decimal ReferenceRate { get; private set; } public decimal MinimumCharge { get; private set; }
    public DateOnly EffectiveFrom { get; private set; } public DateOnly EffectiveTo { get; private set; }
    public string ReasonCode { get; private set; } = string.Empty; public string ReasonDetail { get; private set; } = string.Empty;
    public string? SourceReference { get; private set; } public Guid RequestedByUserId { get; private set; }
    public DateTimeOffset RequestedAtUtc { get; private set; } public Guid? DecidedByUserId { get; private set; }
    public DateTimeOffset? DecidedAtUtc { get; private set; } public string? DecisionNote { get; private set; }
    public Guid? ClosedByUserId { get; private set; } public DateTimeOffset? ClosedAtUtc { get; private set; }
    public string? CloseNote { get; private set; } public Guid RowVersion { get; private set; }

    protected BranchRateOverride() { }

    public BranchRateOverride(
        Guid id, Guid organizationId, Guid branchId, Guid templateId, decimal referenceRate, decimal minimumCharge, DateOnly effectiveFrom, DateOnly effectiveTo,
        string? reasonCode, string? reasonDetail, string? sourceReference, Guid requestedByUserId, DateOnly today, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || branchId == Guid.Empty || templateId == Guid.Empty || requestedByUserId == Guid.Empty) throw new ArgumentException("Organization, branch, template and actor are required.");
        if (referenceRate <= 0 || referenceRate > 99_999_999m) throw Invalid("The reference rate must be above zero.");
        if (minimumCharge < 0 || minimumCharge > 99_999_999m) throw Invalid("The minimum charge cannot be negative.");
        if (effectiveTo < effectiveFrom || effectiveTo < today) throw Invalid("The override period is invalid or has already ended.");
        if (effectiveTo.DayNumber - effectiveFrom.DayNumber + 1 > MaxPeriodDays) throw Invalid($"An override can last at most {MaxPeriodDays} days.");
        var code = Clean(reasonCode);
        var detail = Clean(reasonDetail);
        var source = Clean(sourceReference);
        if (code is null or { Length: > 64 } || detail is null or { Length: > 500 } || source is { Length: > 200 }) throw Invalid("A reason code (max 64) and a reason detail (max 500) are required.");
        OrganizationId = organizationId; BranchId = branchId; TemplateId = templateId; ReferenceRate = referenceRate; MinimumCharge = minimumCharge;
        EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo; ReasonCode = code; ReasonDetail = detail; SourceReference = source;
        RequestedByUserId = requestedByUserId; RequestedAtUtc = now.ToUniversalTime(); RowVersion = Guid.NewGuid();
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static QuickEstimateException Invalid(string message) => new("BRANCH_OVERRIDE_INVALID", message);

    private void Require(string status)
    {
        if (Status != status) throw new QuickEstimateException("BRANCH_OVERRIDE_INVALID_STATE", $"Cannot do this to an override that is '{Status}'.");
    }

    /// <summary>Maker–checker: the requester cannot decide. A returned override is final; the branch files a new request.</summary>
    public void Decide(bool approve, string? note, Guid deciderUserId, DateOnly today, DateTimeOffset now)
    {
        Require(OverrideStatus.Requested);
        if (deciderUserId == RequestedByUserId) throw new QuickEstimateException("BRANCH_OVERRIDE_SELF_APPROVAL", "The requester of an override cannot decide it.");
        var text = Clean(note);
        if (text is { Length: > 500 }) throw Invalid("The note cannot exceed 500 characters.");
        if (!approve && text is null) throw new QuickEstimateException("QUICK_ESTIMATE_REASON_REQUIRED", "A reason is required to return an override.");
        if (approve && EffectiveTo < today) throw Invalid("The override period has already ended.");
        Status = approve ? OverrideStatus.Approved : OverrideStatus.Returned;
        DecidedByUserId = deciderUserId; DecidedAtUtc = now.ToUniversalTime(); DecisionNote = text; RowVersion = Guid.NewGuid();
    }

    public void Revoke(string? reason, Guid actorUserId, DateTimeOffset now)
    {
        Require(OverrideStatus.Approved);
        var text = Clean(reason) ?? throw new QuickEstimateException("QUICK_ESTIMATE_REASON_REQUIRED", "A reason is required to revoke an override.");
        if (text.Length > 500) throw Invalid("The reason cannot exceed 500 characters.");
        Status = OverrideStatus.Revoked;
        ClosedByUserId = actorUserId; ClosedAtUtc = now.ToUniversalTime(); CloseNote = text; RowVersion = Guid.NewGuid();
    }
}
```
- [ ] **Step 2: engine — พารามิเตอร์ตัวเลือก (ไม่มี override → ผลเท่าเดิมทุกไบต์)**
```text
FILE backend/src/TanErp.Domain/QuickEstimates/QuickEstimateEngine.cs
>>> แทนที่:
    public static CalculationResult Calculate(PricingTemplate template, QuickEstimateInput input, DateOnly today)
    {
        var config = template.Config;
>>> ด้วย:
    public static CalculationResult Calculate(PricingTemplate template, QuickEstimateInput input, DateOnly today, RateOverride? rateOverride = null)
    {
        var config = template.Config;
        var referenceRate = rateOverride?.ReferenceRate ?? template.ReferenceRate;
        var minimumCharge = rateOverride?.MinimumCharge ?? template.MinimumCharge;
>>> แทนที่:
            var baseAmount = billable * template.ReferenceRate;
>>> ด้วย:
            var baseAmount = billable * referenceRate;
>>> แทนที่:
        var net = Round2(Math.Max(adjustedTotal, template.MinimumCharge));
>>> ด้วย:
        var net = Round2(Math.Max(adjustedTotal, minimumCharge));
>>> แทนที่:
        var snapshotJson = JsonSerializer.Serialize(snapshot, Json);
>>> ด้วย:
        var snapshotJson = JsonSerializer.Serialize(snapshot, Json);
        if (rateOverride is not null)
        {
            // Only an applied override adds this node, so snapshots without one are unchanged.
            var node = System.Text.Json.Nodes.JsonNode.Parse(snapshotJson)!.AsObject();
            node["branchOverride"] = new System.Text.Json.Nodes.JsonObject
            {
                ["id"] = rateOverride.Id, ["referenceRate"] = rateOverride.ReferenceRate, ["minimumCharge"] = rateOverride.MinimumCharge,
                ["standardReferenceRate"] = template.ReferenceRate, ["standardMinimumCharge"] = template.MinimumCharge
            };
            snapshotJson = node.ToJsonString(Json);
        }
>>> แทนที่:
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalInput))).ToLowerInvariant();
>>> ด้วย:
        if (rateOverride is not null) canonicalInput += FormattableString.Invariant($"|override:{rateOverride.Id:N}:{rateOverride.ReferenceRate}:{rateOverride.MinimumCharge}");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalInput))).ToLowerInvariant();
>>> แทนที่:
Round2(adjustedTotal), template.MinimumCharge, net,
>>> ด้วย:
Round2(adjustedTotal), minimumCharge, net,
```
- [ ] **Step 3: EF configuration + DbSet**
```csharp
// BranchRateOverrideConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class BranchRateOverrideConfiguration : IEntityTypeConfiguration<BranchRateOverride>
{
    public void Configure(EntityTypeBuilder<BranchRateOverride> builder)
    {
        builder.ToTable("branch_rate_overrides", "quick_estimate", t =>
        {
            t.HasCheckConstraint("ck_branch_rate_overrides_status", "status IN ('requested', 'approved', 'returned', 'revoked')");
            t.HasCheckConstraint("ck_branch_rate_overrides_rates", "reference_rate > 0 AND minimum_charge >= 0");
            t.HasCheckConstraint("ck_branch_rate_overrides_period", "effective_to >= effective_from");
            t.HasCheckConstraint("ck_branch_rate_overrides_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> requested_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id"); builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired(); builder.Property(x => x.TemplateId).HasColumnName("template_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired(); builder.Property(x => x.ReferenceRate).HasColumnName("reference_rate").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.MinimumCharge).HasColumnName("minimum_charge").HasPrecision(18, 2).IsRequired(); builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").IsRequired(); builder.Property(x => x.ReasonCode).HasColumnName("reason_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReasonDetail).HasColumnName("reason_detail").HasMaxLength(500).IsRequired(); builder.Property(x => x.SourceReference).HasColumnName("source_reference").HasMaxLength(200);
        builder.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id").IsRequired(); builder.Property(x => x.RequestedAtUtc).HasColumnName("requested_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id"); builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500); builder.Property(x => x.ClosedByUserId).HasColumnName("closed_by_user_id");
        builder.Property(x => x.ClosedAtUtc).HasColumnName("closed_at_utc").HasColumnType("timestamptz"); builder.Property(x => x.CloseNote).HasColumnName("close_note").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.TemplateId, x.BranchId, x.Status });
        builder.HasOne<TanErp.Domain.Organization.Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TanErp.Domain.Organization.Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PricingTemplate>().WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RequestedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ClosedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
```
```text
FILE backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs
>>> หลัง: public DbSet<TanErp.Domain.QuickEstimates.QuickEstimateConversion> QuickEstimateConversions
    public DbSet<TanErp.Domain.QuickEstimates.BranchRateOverride> BranchRateOverrides => Set<TanErp.Domain.QuickEstimates.BranchRateOverride>();
```
- [ ] **Step 4: Generate migration (ครั้งเดียว — ถ้าต้องแก้ให้ `dotnet ef migrations remove` แล้ว generate ใหม่; ห้าม `database update`)**
```bash
cd /Users/syaco/Documents/development/tan-erp && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" \
dotnet ef migrations add AddBranchRateOverrides \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
grep -c "CreateTable" backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddBranchRateOverrides.cs
```
Expected: `CreateTable` 1 ตัว (`branch_rate_overrides` ใน schema `quick_estimate`) + check constraint 4 + index และไม่มีการแก้/ลบอย่างอื่น; ถ้ามีอย่างอื่นให้หยุดและรายงาน.
- [ ] **Step 5: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```
Expected: `0 Warning(s)` `0 Error(s)`; ไม่มีการเปลี่ยนโมเดลค้าง
- [ ] **Step 6: Commit**
```bash
git add backend/src/TanErp.Domain/QuickEstimates backend/src/TanErp.Infrastructure/Persistence
git commit -m "feat(quick-estimates): add the branch rate override entity and an engine override input

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 3: Store/handler ของ override + ต่อเข้า Calculate

**Files:** Create: `backend/src/TanErp.Application/QuickEstimates/BranchRateOverrideModels.cs`, `…/IBranchRateOverrideStore.cs`, `…/BranchRateOverrideHandler.cs`, `backend/src/TanErp.Infrastructure/Persistence/QuickEstimates/BranchRateOverrideStore.cs`; Modify: `backend/src/TanErp.Application/QuickEstimates/QuickEstimateModels.cs`, `backend/src/TanErp.Infrastructure/Persistence/QuickEstimates/QuickEstimateStore.cs`

- [ ] **Step 1: models + interface** (ไฟล์ละ type group; namespace `TanErp.Application.QuickEstimates`)
```csharp
// BranchRateOverrideModels.cs
namespace TanErp.Application.QuickEstimates;

public sealed record BranchOverrideInput(decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo, string? ReasonCode, string? ReasonDetail, string? SourceReference);

public sealed record BranchOverrideProjection(
    Guid Id, Guid TemplateId, Guid BranchId, string BranchCode, string Status, decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo,
    string ReasonCode, string ReasonDetail, string? SourceReference, QePerson RequestedBy, DateTimeOffset RequestedAtUtc, QePerson? DecidedBy, DateTimeOffset? DecidedAtUtc, string? DecisionNote, Guid RowVersion);
```
```csharp
// IBranchRateOverrideStore.cs
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.QuickEstimates;

public interface IBranchRateOverrideStore
{
    Task<Result<BranchOverrideProjection>> RequestAsync(RequestAccessContext access, Guid templateId, BranchOverrideInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default);
    Task<IReadOnlyList<BranchOverrideProjection>> ListAsync(RequestAccessContext access, Guid templateId, CancellationToken ct = default);
    Task<Result<BranchOverrideProjection>> DecideAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, bool approve, string? note, string traceId, CancellationToken ct = default);
    Task<Result<BranchOverrideProjection>> RevokeAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, string? reason, string traceId, CancellationToken ct = default);
}
```
- [ ] **Step 2: `BranchRateOverrideHandler.cs`**
```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;

namespace TanErp.Application.QuickEstimates;

/// <summary>Branch rate override use cases. Permission is resolved from PostgreSQL per call; the store enforces branch scope and maker–checker.</summary>
public class BranchRateOverrideHandler
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IBranchRateOverrideStore _store;

    public BranchRateOverrideHandler(IRequestAccessResolver accessResolver, IBranchRateOverrideStore store)
    {
        _accessResolver = accessResolver;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> AccessAsync(QeCaller caller, string permission, CancellationToken ct) => _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, permission, ct);

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    public async Task<Result<BranchOverrideProjection>> RequestAsync(QeCaller caller, Guid templateId, string idempotencyKey, BranchOverrideInput? input, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.override-rate", ct);
        if (access.IsFailure) return Result<BranchOverrideProjection>.Failure(access.Error);
        if (input is null) return Fail<BranchOverrideProjection>("BRANCH_OVERRIDE_INVALID", "A request body is required.");
        var payload = FormattableString.Invariant($"{templateId:N}|{input.ReferenceRate}|{input.MinimumCharge}|{input.EffectiveFrom:yyyy-MM-dd}|{input.EffectiveTo:yyyy-MM-dd}|{input.ReasonCode?.Trim()}|{input.ReasonDetail?.Trim()}|{input.SourceReference?.Trim()}");
        return await _store.RequestAsync(access.Value!, templateId, input, Sha256Hex.Compute(idempotencyKey), Sha256Hex.Compute(payload), caller.TraceId, ct);
    }

    public async Task<Result<IReadOnlyList<BranchOverrideProjection>>> ListAsync(QeCaller caller, Guid templateId, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.read", ct);
        if (access.IsFailure) return Result<IReadOnlyList<BranchOverrideProjection>>.Failure(access.Error);
        return Result<IReadOnlyList<BranchOverrideProjection>>.Success(await _store.ListAsync(access.Value!, templateId, ct));
    }

    public async Task<Result<BranchOverrideProjection>> DecideAsync(QeCaller caller, Guid templateId, Guid overrideId, Guid expectedVersion, string? decision, string? note, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.approve", ct);
        if (access.IsFailure) return Result<BranchOverrideProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BranchOverrideProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        if (decision is not ("approved" or "returned")) return Fail<BranchOverrideProjection>("QUICK_ESTIMATE_FIELD_INVALID", "The decision must be approved or returned.");
        return await _store.DecideAsync(access.Value!, templateId, overrideId, expectedVersion, decision == "approved", note, caller.TraceId, ct);
    }

    public async Task<Result<BranchOverrideProjection>> RevokeAsync(QeCaller caller, Guid templateId, Guid overrideId, Guid expectedVersion, string? reason, CancellationToken ct = default)
    {
        var access = await AccessAsync(caller, "pricing-templates.approve", ct);
        if (access.IsFailure) return Result<BranchOverrideProjection>.Failure(access.Error);
        if (expectedVersion == Guid.Empty) return Fail<BranchOverrideProjection>("QUICK_ESTIMATE_FIELD_REQUIRED", "Expected version is required.");
        return await _store.RevokeAsync(access.Value!, templateId, overrideId, expectedVersion, reason, caller.TraceId, ct);
    }
}
```
- [ ] **Step 3: `BranchRateOverrideStore.cs`** (Infrastructure)
```csharp
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.QuickEstimates;
using TanErp.Domain.Common;
using TanErp.Domain.QuickEstimates;

namespace TanErp.Infrastructure.Persistence.QuickEstimates;

public class BranchRateOverrideStore : IBranchRateOverrideStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public BranchRateOverrideStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    private DateOnly Today => DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime);

    private void Audit(RequestAccessContext access, string action, Guid id, string traceId, object changes, DateTimeOffset now) =>
        _db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), access.OrganizationId, access.ActorUserId, action, "BranchRateOverride", id.ToString(), now, traceId,
            JsonSerializer.Serialize(changes), branchId: access.BranchId, actorMembershipId: access.MembershipId));

    private async Task<List<BranchOverrideProjection>> ProjectAsync(IReadOnlyList<BranchRateOverride> rows, CancellationToken ct)
    {
        var userIds = rows.SelectMany(o => new Guid?[] { o.RequestedByUserId, o.DecidedByUserId }).Where(i => i.HasValue).Select(i => i!.Value).Distinct().ToList();
        var people = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => new QePerson(u.Id, u.DisplayName), ct);
        var branchIds = rows.Select(o => o.BranchId).Distinct().ToList();
        var codes = await _db.Branches.AsNoTracking().Where(b => branchIds.Contains(b.Id)).ToDictionaryAsync(b => b.Id, b => b.Code, ct);
        return rows.Select(o => new BranchOverrideProjection(o.Id, o.TemplateId, o.BranchId, codes[o.BranchId], o.Status, o.ReferenceRate, o.MinimumCharge, o.EffectiveFrom, o.EffectiveTo,
            o.ReasonCode, o.ReasonDetail, o.SourceReference, people[o.RequestedByUserId], o.RequestedAtUtc, o.DecidedByUserId.HasValue ? people[o.DecidedByUserId.Value] : null, o.DecidedAtUtc, o.DecisionNote, o.RowVersion)).ToList();
    }

    public async Task<IReadOnlyList<BranchOverrideProjection>> ListAsync(RequestAccessContext access, Guid templateId, CancellationToken ct = default)
    {
        var orgId = access.OrganizationId;
        var branch = access.BranchId;
        // A branch-bound membership sees only its own branch; an organization-wide membership sees every branch of its organization.
        var rows = await _db.BranchRateOverrides.AsNoTracking().Where(o => o.OrganizationId == orgId && o.TemplateId == templateId && (branch == null || o.BranchId == branch))
            .OrderByDescending(o => o.RequestedAtUtc).ToListAsync(ct);
        return await ProjectAsync(rows, ct);
    }

    public async Task<Result<BranchOverrideProjection>> RequestAsync(
        RequestAccessContext access, Guid templateId, BranchOverrideInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
    {
        const string operation = "quick-estimate.branch-override.request";
        var orgId = access.OrganizationId;
        if (!access.BranchId.HasValue) return Fail<BranchOverrideProjection>("ACTIVE_BRANCH_REQUIRED", "An active branch is required.");
        var branchId = access.BranchId.Value;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var keyLock = $"{orgId:N}:{operation}:{keyHash}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({keyLock}, 0))", ct);
            var replay = await _db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(r => r.OrganizationId == orgId && r.Operation == operation && r.KeyHash == keyHash, ct);
            if (replay is not null)
            {
                if (replay.PayloadHash != payloadHash) return Fail<BranchOverrideProjection>("IDEMPOTENCY_KEY_REUSED", "The idempotency key has already been used with a different payload.");
                var existing = Guid.TryParse(replay.ResourceId, out var existingId) ? await _db.BranchRateOverrides.AsNoTracking().FirstOrDefaultAsync(o => o.Id == existingId && o.OrganizationId == orgId, ct) : null;
                if (existing is not null) return Result<BranchOverrideProjection>.Success((await ProjectAsync(new[] { existing }, ct))[0]);
            }

            var template = await _db.PricingTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == templateId && t.OrganizationId == orgId, ct);
            if (template is null) return Fail<BranchOverrideProjection>("RESOURCE_NOT_FOUND", "Pricing template not found.");
            if (!TemplateStatus.IsUsable(template.Status)) return Fail<BranchOverrideProjection>("PRICING_TEMPLATE_NOT_USABLE", "The pricing template is not usable.");

            // Serializes concurrent requests for the same branch and template so the overlap check below cannot race.
            var scopeLock = $"{orgId:N}:branch-override:{templateId:N}:{branchId:N}";
            await _db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({scopeLock}, 0))", ct);

            var now = _clock.UtcNow;
            BranchRateOverride entity;
            try
            {
                entity = new BranchRateOverride(Guid.NewGuid(), orgId, branchId, templateId, input.ReferenceRate, input.MinimumCharge, input.EffectiveFrom, input.EffectiveTo,
                    input.ReasonCode, input.ReasonDetail, input.SourceReference, access.ActorUserId, Today, now);
            }
            catch (QuickEstimateException ex)
            {
                return Fail<BranchOverrideProjection>(ex.Code, ex.Message);
            }

            var from = entity.EffectiveFrom;
            var to = entity.EffectiveTo;
            var overlaps = await _db.BranchRateOverrides.AsNoTracking().AnyAsync(o => o.OrganizationId == orgId && o.TemplateId == templateId && o.BranchId == branchId
                && (o.Status == OverrideStatus.Requested || o.Status == OverrideStatus.Approved) && o.EffectiveFrom <= to && from <= o.EffectiveTo, ct);
            if (overlaps) return Fail<BranchOverrideProjection>("BRANCH_OVERRIDE_OVERLAP", "An override request for this branch and template already covers part of this period.");

            _db.BranchRateOverrides.Add(entity);
            Audit(access, "branch-override.requested", entity.Id, traceId, new { templateId, branchId, entity.EffectiveFrom, entity.EffectiveTo }, now);
            _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, entity.Id.ToString(), now));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return Result<BranchOverrideProjection>.Success((await ProjectAsync(new[] { entity }, ct))[0]);
        });
    }

    private async Task<Result<BranchOverrideProjection>> MutateAsync(
        RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, Func<BranchRateOverride, DateTimeOffset, Task<string>> mutate, string traceId, CancellationToken ct)
    {
        var orgId = access.OrganizationId;
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            var row = await _db.BranchRateOverrides.FirstOrDefaultAsync(o => o.Id == overrideId && o.TemplateId == templateId && o.OrganizationId == orgId, ct);
            // Another branch's override is indistinguishable from a missing one.
            if (row is null || !access.HasBranchAccess(row.BranchId)) return Fail<BranchOverrideProjection>("RESOURCE_NOT_FOUND", "Branch override not found.");
            if (row.RowVersion != expectedVersion) return Fail<BranchOverrideProjection>("PRICING_TEMPLATE_VERSION_CONFLICT", "Branch override version conflict.");
            var now = _clock.UtcNow;
            string audit;
            try
            {
                audit = await mutate(row, now);
            }
            catch (QuickEstimateException ex)
            {
                return Fail<BranchOverrideProjection>(ex.Code, ex.Message);
            }

            Audit(access, audit, row.Id, traceId, new { templateId, row.BranchId, status = row.Status }, now);
            try
            {
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Fail<BranchOverrideProjection>("PRICING_TEMPLATE_VERSION_CONFLICT", "Branch override version conflict.");
            }

            return Result<BranchOverrideProjection>.Success((await ProjectAsync(new[] { row }, ct))[0]);
        });
    }

    public Task<Result<BranchOverrideProjection>> DecideAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, bool approve, string? note, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, templateId, overrideId, expectedVersion, async (o, now) =>
        {
            if (approve && !TemplateStatus.IsUsable((await _db.PricingTemplates.AsNoTracking().FirstAsync(t => t.Id == o.TemplateId, ct)).Status)) throw new QuickEstimateException("PRICING_TEMPLATE_NOT_USABLE", "The pricing template is not usable.");
            o.Decide(approve, note, access.ActorUserId, Today, now);
            return approve ? "branch-override.approved" : "branch-override.returned";
        }, traceId, ct);

    public Task<Result<BranchOverrideProjection>> RevokeAsync(RequestAccessContext access, Guid templateId, Guid overrideId, Guid expectedVersion, string? reason, string traceId, CancellationToken ct = default) =>
        MutateAsync(access, templateId, overrideId, expectedVersion, (o, now) =>
        {
            o.Revoke(reason, access.ActorUserId, now);
            return Task.FromResult("branch-override.revoked");
        }, traceId, ct);
}
```
- [ ] **Step 4: ต่อ override เข้า Calculate และ projection**
```text
FILE backend/src/TanErp.Application/QuickEstimates/QuickEstimateModels.cs
>>> แทนที่:
    IReadOnlyList<CalculationLineProjection> Lines,
    QePerson CreatedBy,
    DateTimeOffset CreatedAtUtc);
>>> ด้วย:
    IReadOnlyList<CalculationLineProjection> Lines,
    QePerson CreatedBy,
    DateTimeOffset CreatedAtUtc,
    Guid? BranchOverrideId = null);

FILE backend/src/TanErp.Infrastructure/Persistence/QuickEstimates/QuickEstimateStore.cs
>>> แทนที่:
            var result = QuickEstimateEngine.Calculate(template, e.ToInput(), Today);
>>> ด้วย:
            // Only an approved override of this quick estimate's own branch, for this exact template version and today's date, can apply.
            var today = Today;
            var branchId = e.BranchId;
            var rateOverride = await _db.BranchRateOverrides.AsNoTracking()
                .Where(o => o.OrganizationId == access.OrganizationId && o.TemplateId == template.Id && o.BranchId == branchId && o.Status == OverrideStatus.Approved && o.EffectiveFrom <= today && today <= o.EffectiveTo)
                .Select(o => new RateOverride(o.Id, o.ReferenceRate, o.MinimumCharge)).FirstOrDefaultAsync(ct);
            var result = QuickEstimateEngine.Calculate(template, e.ToInput(), today, rateOverride);
>>> แทนที่:
JsonSerializer.Deserialize<List<string>>(calc.ReasonCodesJson) ?? new List<string>(), calc.InputHash, lines, Person(calc.CreatedByUserId)!, calc.CreatedAtUtc);
>>> ด้วย:
JsonSerializer.Deserialize<List<string>>(calc.ReasonCodesJson) ?? new List<string>(), calc.InputHash, lines, Person(calc.CreatedByUserId)!, calc.CreatedAtUtc,
                snapshot.TryGetProperty("branchOverride", out var branchOverride) ? branchOverride.GetProperty("id").GetGuid() : (Guid?)null);
```
- [ ] **Step 5: Compile-only gate (ห้ามรันชุดทดสอบ)** — `dotnet build backend/TanErp.slnx --nologo -v q` → `0 Warning(s)` `0 Error(s)`
- [ ] **Step 6: Commit**
```bash
git add backend/src
git commit -m "feat(quick-estimates): request, decide and apply branch rate overrides

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 4: API ของ override — controller, contracts, error codes, permission, DI

**Files:** Create: `backend/src/TanErp.Api/Contracts/QuickEstimates/BranchRateOverrideContracts.cs`, `backend/src/TanErp.Api/Controllers/BranchRateOverridesController.cs`; Modify: `QuickEstimateContracts.cs`, `QuickEstimateMapper.cs` (ทั้งสองใต้ `Contracts/QuickEstimates/`), `ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`, `Program.cs` (ใต้ `backend/src/TanErp.Api/`), `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`

**อ่าน `.agents/skills/building-erp-apis/SKILL.md` ก่อน.** `pricing-templates.override-rate` ไม่ใช่ approval-class → **ไม่แก้** `AdministrationPolicy.ApprovalPermissionKeys` (F13/D6).

- [ ] **Step 1: contracts + controller**
```csharp
// BranchRateOverrideContracts.cs
using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Contracts.QuickEstimates;

public sealed record BranchRateOverrideRequest(decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo, string? ReasonCode, string? ReasonDetail, string? SourceReference);
public sealed record BranchRateOverrideDecisionRequest(string? Decision, string? Note);
public sealed record BranchRateOverrideRevokeRequest(string? Reason);
public sealed record BranchRateOverrideResponse(
    Guid Id, Guid TemplateId, Guid BranchId, string BranchCode, string Status, decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo,
    string ReasonCode, string ReasonDetail, string? SourceReference, QePersonResponse RequestedBy, DateTimeOffset RequestedAtUtc, QePersonResponse? DecidedBy, DateTimeOffset? DecidedAtUtc, string? DecisionNote, Guid RowVersion);
public sealed record BranchRateOverrideListResponse(IReadOnlyList<BranchRateOverrideResponse> Items);

internal static class BranchRateOverrideMapper
{
    public static BranchRateOverrideResponse To(BranchOverrideProjection x) => new(x.Id, x.TemplateId, x.BranchId, x.BranchCode, x.Status, x.ReferenceRate, x.MinimumCharge, x.EffectiveFrom, x.EffectiveTo,
        x.ReasonCode, x.ReasonDetail, x.SourceReference, QuickEstimateMapper.To(x.RequestedBy), x.RequestedAtUtc, x.DecidedBy is null ? null : QuickEstimateMapper.To(x.DecidedBy), x.DecidedAtUtc, x.DecisionNote, x.RowVersion);
}
```
```csharp
// BranchRateOverridesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.QuickEstimates;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Controllers;

/// <summary>Branch-specific rate overrides of a pricing template. No business logic lives here.</summary>
[ApiController]
[Route("api/v1/pricing-templates/{templateId:guid}/branch-rate-overrides")]
[Authorize]
public class BranchRateOverridesController : QuickEstimateControllerBase
{
    private readonly BranchRateOverrideHandler _handler;

    public BranchRateOverridesController(BranchRateOverrideHandler handler)
    {
        _handler = handler;
    }

    private IActionResult Respond(Result<BranchOverrideProjection> result, int status = StatusCodes.Status200OK)
    {
        if (result.IsFailure) return Problem(result.Error.Code);
        Response.Headers.ETag = $"\"{result.Value!.RowVersion}\"";
        return StatusCode(status, BranchRateOverrideMapper.To(result.Value));
    }

    [HttpPost]
    [ProducesResponseType<BranchRateOverrideResponse>(StatusCodes.Status201Created), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RequestOverride([FromRoute] Guid templateId, [FromBody] BranchRateOverrideRequest request, CancellationToken ct)
    {
        var caller = ReadIdempotent(out var key, out var failure);
        if (caller is null) return failure!;
        var input = new BranchOverrideInput(request.ReferenceRate, request.MinimumCharge, request.EffectiveFrom, request.EffectiveTo, request.ReasonCode, request.ReasonDetail, request.SourceReference);
        return Respond(await _handler.RequestAsync(caller, templateId, key, input, ct), StatusCodes.Status201Created);
    }

    [HttpGet]
    [ProducesResponseType<BranchRateOverrideListResponse>(StatusCodes.Status200OK), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromRoute] Guid templateId, CancellationToken ct)
    {
        var caller = ReadAuthenticated(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, templateId, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new BranchRateOverrideListResponse(result.Value!.Select(BranchRateOverrideMapper.To).ToList()));
    }

    [HttpPost("{overrideId:guid}/decision")]
    [ProducesResponseType<BranchRateOverrideResponse>(StatusCodes.Status200OK), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Decide([FromRoute] Guid templateId, [FromRoute] Guid overrideId, [FromBody] BranchRateOverrideDecisionRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.DecideAsync(caller, templateId, overrideId, version, request.Decision, request.Note, ct));
    }

    [HttpPost("{overrideId:guid}/revoke")]
    [ProducesResponseType<BranchRateOverrideResponse>(StatusCodes.Status200OK), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict), ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Revoke([FromRoute] Guid templateId, [FromRoute] Guid overrideId, [FromBody] BranchRateOverrideRevokeRequest request, CancellationToken ct)
    {
        var caller = ReadConditional(out var version, out var failure);
        if (caller is null) return failure!;
        return Respond(await _handler.RevokeAsync(caller, templateId, overrideId, version, request.Reason, ct));
    }
}
```
- [ ] **Step 2: `branchOverrideId`, DI, error codes, permission** (resx: แทรกต่อท้ายบรรทัด `PRICING_TEMPLATE_SELF_APPROVAL_DETAIL` ทั้งบรรทัด — `Errors.resx:1139`, `Errors.en.resx:977`)
```text
FILE backend/src/TanErp.Api/Contracts/QuickEstimates/QuickEstimateContracts.cs
>>> แทนที่:
IReadOnlyList<CalculationLineResponse> Lines, QePersonResponse CreatedBy, DateTimeOffset CreatedAtUtc);
>>> ด้วย:
IReadOnlyList<CalculationLineResponse> Lines, QePersonResponse CreatedBy, DateTimeOffset CreatedAtUtc, Guid? BranchOverrideId = null);

FILE backend/src/TanErp.Api/Contracts/QuickEstimates/QuickEstimateMapper.cs
>>> แทนที่:
x.Lines.Select(To).ToList(), To(x.CreatedBy), x.CreatedAtUtc);
>>> ด้วย:
x.Lines.Select(To).ToList(), To(x.CreatedBy), x.CreatedAtUtc, x.BranchOverrideId);

FILE backend/src/TanErp.Api/Program.cs
>>> หลัง: builder.Services.AddScoped<TanErp.Application.QuickEstimates.QuickEstimateHandler>();
builder.Services.AddScoped<TanErp.Application.QuickEstimates.IBranchRateOverrideStore, TanErp.Infrastructure.Persistence.QuickEstimates.BranchRateOverrideStore>();
builder.Services.AddScoped<TanErp.Application.QuickEstimates.BranchRateOverrideHandler>();

FILE backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs
>>> หลัง: "PRICING_TEMPLATE_SELF_APPROVAL" => StatusCodes.Status403Forbidden,
        "BRANCH_OVERRIDE_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "BRANCH_OVERRIDE_OVERLAP" => StatusCodes.Status409Conflict,
        "BRANCH_OVERRIDE_INVALID_STATE" => StatusCodes.Status409Conflict,
        "BRANCH_OVERRIDE_SELF_APPROVAL" => StatusCodes.Status403Forbidden,

FILE backend/src/TanErp.Api/Resources/Errors.resx
>>> หลัง: <data name="PRICING_TEMPLATE_SELF_APPROVAL_DETAIL"
  <data name="BRANCH_OVERRIDE_INVALID_TITLE" xml:space="preserve"><value>ข้อมูลราคาเฉพาะสาขาไม่ถูกต้อง</value></data>
  <data name="BRANCH_OVERRIDE_INVALID_DETAIL" xml:space="preserve"><value>ตรวจอัตรา ช่วงวันที่ (ไม่เกิน 366 วัน และต้องไม่สิ้นสุดก่อนวันนี้) และเหตุผลอีกครั้ง</value></data>
  <data name="BRANCH_OVERRIDE_OVERLAP_TITLE" xml:space="preserve"><value>ช่วงเวลาซ้อนกับราคาเฉพาะสาขาเดิม</value></data>
  <data name="BRANCH_OVERRIDE_OVERLAP_DETAIL" xml:space="preserve"><value>สาขานี้มีคำขอหรือราคาเฉพาะสาขาของแม่แบบนี้ในช่วงเวลาดังกล่าวอยู่แล้ว</value></data>
  <data name="BRANCH_OVERRIDE_INVALID_STATE_TITLE" xml:space="preserve"><value>สถานะราคาเฉพาะสาขาไม่อนุญาตให้ทำรายการนี้</value></data>
  <data name="BRANCH_OVERRIDE_INVALID_STATE_DETAIL" xml:space="preserve"><value>สถานะปัจจุบันของราคาเฉพาะสาขาไม่อนุญาตให้ทำรายการนี้</value></data>
  <data name="BRANCH_OVERRIDE_SELF_APPROVAL_TITLE" xml:space="preserve"><value>ผู้ขอตัดสินคำขอของตนเองไม่ได้</value></data>
  <data name="BRANCH_OVERRIDE_SELF_APPROVAL_DETAIL" xml:space="preserve"><value>ผู้ขอราคาเฉพาะสาขาตัดสินคำขอของตนเองไม่ได้</value></data>

FILE backend/src/TanErp.Api/Resources/Errors.en.resx
>>> หลัง: <data name="PRICING_TEMPLATE_SELF_APPROVAL_DETAIL"
  <data name="BRANCH_OVERRIDE_INVALID_TITLE" xml:space="preserve"><value>Invalid Branch Rate Override</value></data>
  <data name="BRANCH_OVERRIDE_INVALID_DETAIL" xml:space="preserve"><value>Check the rate, the period (at most 366 days, not ending before today) and the reason.</value></data>
  <data name="BRANCH_OVERRIDE_OVERLAP_TITLE" xml:space="preserve"><value>Overlapping Branch Rate Override</value></data>
  <data name="BRANCH_OVERRIDE_OVERLAP_DETAIL" xml:space="preserve"><value>This branch already has an override request or override for this template in that period.</value></data>
  <data name="BRANCH_OVERRIDE_INVALID_STATE_TITLE" xml:space="preserve"><value>Invalid Branch Rate Override State</value></data>
  <data name="BRANCH_OVERRIDE_INVALID_STATE_DETAIL" xml:space="preserve"><value>The current status of the override does not allow this action.</value></data>
  <data name="BRANCH_OVERRIDE_SELF_APPROVAL_TITLE" xml:space="preserve"><value>Branch Override Self Approval</value></data>
  <data name="BRANCH_OVERRIDE_SELF_APPROVAL_DETAIL" xml:space="preserve"><value>The requester of an override cannot decide it.</value></data>

FILE backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs
>>> หลัง: ("pricing-templates.approve", "Approve Pricing Templates"),
            ("pricing-templates.override-rate", "Request Branch Rate Overrides For Pricing Templates"),
```
- [ ] **Step 3: Compile-only gate (ห้ามรันชุดทดสอบ)** — `dotnet build backend/TanErp.slnx --nologo -v q` → `0 Warning(s)` `0 Error(s)`; `grep -c "BRANCH_OVERRIDE" backend/src/TanErp.Api/Resources/Errors.resx backend/src/TanErp.Api/Resources/Errors.en.resx` ได้ `8` ทั้งสองไฟล์; `grep -c "BRANCH_OVERRIDE" backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs` ได้ `4`
- [ ] **Step 4: Commit**
```bash
git add backend/src
git commit -m "feat(quick-estimates): expose the branch rate override api

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 5: Conversion สร้างรายการขอบเขต

**Files:** Create: `backend/src/TanErp.Application/QuickEstimates/QuickEstimateConversionMapper.cs`; Modify: `backend/src/TanErp.Application/QuickEstimates/QuickEstimateModels.cs`, `backend/src/TanErp.Infrastructure/Persistence/QuickEstimates/QuickEstimateStore.cs`, `backend/src/TanErp.Api/Contracts/QuickEstimates/QuickEstimateContracts.cs`, `…/QuickEstimateMapper.cs`

- [ ] **Step 1: `QuickEstimateConversionMapper.cs`** (pure; ไม่คัดลอกจำนวนเงิน)
```csharp
using System.Text.Json;
using TanErp.Application.Estimates;
using TanErp.Domain.Estimates;

namespace TanErp.Application.QuickEstimates;

/// <summary>
/// Builds the scope lines of an official estimate draft from one frozen quick-estimate calculation: one work item per measurement line
/// (billable quantity and the template unit). No price or cost is copied; the estimator adds costs and calculates the estimate anew.
/// </summary>
public static class QuickEstimateConversionMapper
{
    public const string SectionCode = "QE";
    public const string ReasonCode = "QUICK_ESTIMATE_CONVERSION";

    /// <summary>The EstimateWorkItem default margin; the estimator can change it before calculating.</summary>
    public const decimal DefaultMargin = 0.30m;

    private static string Cut(string value, int max) => value.Length <= max ? value : value[..max];

    public static IReadOnlyList<EstimateSectionDraftDto> BuildSections(string quickEstimateNumber, string roomOrArea, int calculationVersion, string templateCode, string snapshotJson)
    {
        using var document = JsonDocument.Parse(snapshotJson);
        var unitCode = document.RootElement.GetProperty("template").GetProperty("unitCode").GetString() ?? string.Empty;
        var reason = Cut($"{quickEstimateNumber} · v{calculationVersion}", 500);
        var items = new List<EstimateWorkItemDraftDto>();
        foreach (var line in document.RootElement.GetProperty("lines").EnumerateArray())
        {
            var order = items.Count + 1;
            var subtype = line.TryGetProperty("workSubtype", out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
            var label = string.IsNullOrWhiteSpace(subtype) ? roomOrArea : subtype.Trim();
            items.Add(new EstimateWorkItemDraftDto(
                Id: null, Code: $"QE-{order:00}", DescriptionTh: Cut($"{templateCode} · {label}", 500), DescriptionEn: null,
                Quantity: line.GetProperty("billableQuantity").GetDecimal(), UnitCode: unitCode, SellingRuleType: SellingRuleType.Margin, SellingRuleValue: DefaultMargin,
                SortOrder: order, CostComponents: Array.Empty<EstimateCostComponentDraftDto>(), OverrideReasonCode: ReasonCode, OverrideReason: reason));
        }

        return items.Count == 0 ? Array.Empty<EstimateSectionDraftDto>() : new[] { new EstimateSectionDraftDto(null, SectionCode, Cut(roomOrArea, 200), null, 1, items) };
    }
}
```
- [ ] **Step 2: `ConvertAsync` — เติมรายการก่อนบันทึก conversion + `mappedLineCount`**
```text
FILE backend/src/TanErp.Application/QuickEstimates/QuickEstimateModels.cs
>>> แทนที่:
public sealed record ConversionProjection(Guid Id, int SourceVersion, Guid OfficialEstimateId, DateTimeOffset CreatedAtUtc);
>>> ด้วย:
public sealed record ConversionProjection(Guid Id, int SourceVersion, Guid OfficialEstimateId, DateTimeOffset CreatedAtUtc, int? MappedLineCount = null);

FILE backend/src/TanErp.Infrastructure/Persistence/QuickEstimates/QuickEstimateStore.cs
>>> แทนที่:
        if (created.IsFailure) return Result<QuickEstimateProjection>.Failure(created.Error);
>>> ด้วย:
        if (created.IsFailure) return Result<QuickEstimateProjection>.Failure(created.Error);

        // Scope lines are added only to a draft that is still empty, so an estimator's work is never overwritten and a retry after a partial
        // success finds the lines already there. The count comes from the stored draft, not from this attempt.
        var draft = created.Value!;
        EstimateRevisionProjection? stored = draft.CurrentRevision;
        if (stored is not null && draft.Status == TanErp.Domain.Estimates.EstimateStatus.Draft && stored.RevisionNo == 1 && stored.Sections.Count == 0)
        {
            var scope = QuickEstimateConversionMapper.BuildSections(estimate.Number, estimate.RoomOrArea, releasable.Value!.Version, releasable.Value.TemplateCode, releasable.Value.SnapshotJson);
            if (scope.Count > 0)
            {
                try
                {
                    _db.ChangeTracker.Clear();
                    stored = await _estimates.UpdateDraftAsync(orgId, draft.Id, stored.Id, stored.RowVersion, scope, access.ActorUserId, ct);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Fail<QuickEstimateProjection>("QUICK_ESTIMATE_VERSION_CONFLICT", "The official estimate draft changed while converting; retry.");
                }
            }
        }

        var lineCount = stored?.Sections.Where(s => s.Code == QuickEstimateConversionMapper.SectionCode).Sum(s => s.WorkItems.Count) ?? 0;
>>> แทนที่:
locked.AddOnCodes, measurements = locked.Measurements }, Json);
>>> ด้วย:
locked.AddOnCodes, measurements = locked.Measurements, mappedLineCount = lineCount }, Json);
>>> แทนที่:
            conversions.Select(c => new ConversionProjection(c.Id, c.SourceVersion, c.OfficialEstimateId, c.CreatedAtUtc)).ToList(),
>>> ด้วย:
            conversions.Select(c => new ConversionProjection(c.Id, c.SourceVersion, c.OfficialEstimateId, c.CreatedAtUtc, MappedLineCount(c.SourceSnapshotJson))).ToList(),
>>> แทนที่:
    public async Task<QuickEstimateProjection?> GetQuickEstimateAsync(Guid organizationId, Guid id, CancellationToken ct = default)
>>> ด้วย:
    /// <summary>Conversions made before G-09 have no count in their stored snapshot; null means "not recorded".</summary>
    private static int? MappedLineCount(string sourceSnapshotJson)
    {
        using var document = JsonDocument.Parse(sourceSnapshotJson);
        return document.RootElement.TryGetProperty("mappedLineCount", out var count) && count.ValueKind == JsonValueKind.Number ? count.GetInt32() : null;
    }

    public async Task<QuickEstimateProjection?> GetQuickEstimateAsync(Guid organizationId, Guid id, CancellationToken ct = default)

FILE backend/src/TanErp.Api/Contracts/QuickEstimates/QuickEstimateContracts.cs
>>> แทนที่:
public sealed record ConversionResponse(Guid Id, int SourceVersion, Guid OfficialEstimateId, DateTimeOffset CreatedAtUtc);
>>> ด้วย:
public sealed record ConversionResponse(Guid Id, int SourceVersion, Guid OfficialEstimateId, DateTimeOffset CreatedAtUtc, int? MappedLineCount = null);

FILE backend/src/TanErp.Api/Contracts/QuickEstimates/QuickEstimateMapper.cs
>>> แทนที่:
new(x.Id, x.SourceVersion, x.OfficialEstimateId, x.CreatedAtUtc);
>>> ด้วย:
new(x.Id, x.SourceVersion, x.OfficialEstimateId, x.CreatedAtUtc, x.MappedLineCount);
```
ลำดับเดิมคงอยู่: replay → not found → already converted → opportunity required → releasable → CreateDraft → **เติมรายการ** → tx ของ conversion (idempotency record และ `QUICK_ESTIMATE_ALREADY_CONVERTED` ไม่เปลี่ยน).
- [ ] **Step 3: Compile-only gate (ห้ามรันชุดทดสอบ)** — `dotnet build backend/TanErp.slnx --nologo -v q` → `0 Warning(s)` `0 Error(s)`
- [ ] **Step 4: Commit**
```bash
git add backend/src
git commit -m "feat(quick-estimates): create scope lines when converting a quick estimate

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 6: Evidence — owner type `quick-estimate` (backend 3 จุด) + `AttachmentList` ใน workspace

**Files:** Modify: `backend/src/TanErp.Domain/Attachments/AttachmentValues.cs`, `backend/src/TanErp.Application/Attachments/AttachmentOwnerRegistry.cs`, `backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentOwnerScopeReader.cs`, `frontend/src/lib/attachments/attachment-owner-types.ts`, `…/attachment-owner-types.test.ts`, `frontend/src/features/quick-estimates/components/quick-estimate-workspace.tsx`, `…/quick-estimate-components.test.tsx`

**อ่าน `.agents/skills/building-erp-forms/SKILL.md` ก่อน** (Deferred Upload: ใช้ `AttachmentList` กลางซึ่งเรียก `useDeferredFileUpload` — ห้ามเรียก `fileClient` ตรง). ไม่มี permission/error/i18n/migration ใหม่ (F11). สามจุด backend ต้องแก้ใน commit เดียว (ADR 0017).

- [ ] **Step 1: backend + frontend**
```text
FILE backend/src/TanErp.Domain/Attachments/AttachmentValues.cs
>>> แทนที่:
    public const string InstallationJob = "installation-job";

    private static readonly HashSet<string> Registered = new(StringComparer.Ordinal) { InstallationJob };
>>> ด้วย:
    public const string InstallationJob = "installation-job";
    public const string QuickEstimate = "quick-estimate";

    private static readonly HashSet<string> Registered = new(StringComparer.Ordinal) { InstallationJob, QuickEstimate };

FILE backend/src/TanErp.Application/Attachments/AttachmentOwnerRegistry.cs
>>> แทนที่:
using TanErp.Domain.Attachments;
using TanErp.Domain.Service;
>>> ด้วย:
using TanErp.Domain.Attachments;
using TanErp.Domain.QuickEstimates;
using TanErp.Domain.Service;
>>> แทนที่:
            SignStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.ReadyForHandover })
    }.ToDictionary
>>> ด้วย:
            SignStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.ReadyForHandover }),
        // Quick estimates have no signatures: SignPermission mirrors the manage permission and SignStates is empty, so a capture is refused with ATTACHMENT_OWNER_LOCKED.
        new AttachmentOwnerDescriptor(
            AttachmentOwnerTypes.QuickEstimate,
            ReadPermission: "quick-estimates.read",
            ManagePermission: "quick-estimates.update",
            SignPermission: "quick-estimates.update",
            ManageStates: new HashSet<string>(StringComparer.Ordinal) { QuickEstimateStatus.Draft, QuickEstimateStatus.Calculated, QuickEstimateStatus.Returned },
            SignStates: new HashSet<string>(StringComparer.Ordinal))
    }.ToDictionary

FILE backend/src/TanErp.Infrastructure/Persistence/Attachments/AttachmentOwnerScopeReader.cs
>>> แทนที่:
new HashSet<string>(StringComparer.Ordinal) { AttachmentOwnerTypes.InstallationJob };
>>> ด้วย:
new HashSet<string>(StringComparer.Ordinal) { AttachmentOwnerTypes.InstallationJob, AttachmentOwnerTypes.QuickEstimate };
>>> แทนที่:
            default:
                return null;
>>> ด้วย:
            case AttachmentOwnerTypes.QuickEstimate:
                return await _db.QuickEstimates.AsNoTracking()
                    .Where(q => q.Id == ownerId && q.OrganizationId == organizationId)
                    .Select(q => new AttachmentOwnerScope(q.OrganizationId, q.BranchId, q.Status))
                    .FirstOrDefaultAsync(ct);
            default:
                return null;

FILE frontend/src/lib/attachments/attachment-owner-types.ts
>>> แทนที่:
export const ATTACHMENT_OWNER_TYPES = ["installation-job"] as const;
>>> ด้วย:
export const ATTACHMENT_OWNER_TYPES = ["installation-job", "quick-estimate"] as const;

FILE frontend/src/lib/attachments/attachment-owner-types.test.ts
>>> แทนที่:
    expect(ATTACHMENT_OWNER_TYPES).toEqual(["installation-job"]);
>>> ด้วย:
    expect(ATTACHMENT_OWNER_TYPES).toEqual(["installation-job", "quick-estimate"]);

FILE frontend/src/features/quick-estimates/components/quick-estimate-workspace.tsx
>>> แทนที่:
import { PageHeader } from "@/components/layout/PageHeader";
>>> ด้วย:
import { PageHeader } from "@/components/layout/PageHeader";
import { AttachmentList } from "@/components/forms/AttachmentList";
>>> แทนที่:
      <Modal
        isOpen={dialog !== null}
>>> ด้วย:
      <AttachmentList ownerType="quick-estimate" ownerId={id} canManage={editable} purposes={["evidence", "general"]} />

      <Modal
        isOpen={dialog !== null}

FILE frontend/src/features/quick-estimates/components/quick-estimate-components.test.tsx
>>> แทนที่:
vi.mock("@/lib/permissions/can", () => ({
>>> ด้วย:
vi.mock("@/components/forms/AttachmentList", () => ({ AttachmentList: () => null }));
vi.mock("@/lib/permissions/can", () => ({
```
(`editable` = `canUpdate` + สถานะ `draft/calculated/returned` ตรงกับ `ManageStates` ฝั่ง server; server ตรวจสิทธิ์/สถานะซ้ำ — frontend guard เป็นเพียง UX.)
- [ ] **Step 2: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
cd frontend && npx tsc --noEmit && npx eslint src/features/quick-estimates src/lib/attachments
```
Expected: `0 Warning(s)` `0 Error(s)`; tsc/eslint exit 0
- [ ] **Step 3: Commit**
```bash
git add backend/src frontend/src
git commit -m "feat(quick-estimates): attach photo evidence to a quick estimate

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 7: OpenAPI snapshot + generated types + api-client

**Files:** Modify: `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts` (generated), `frontend/src/lib/api/api-client.ts`

- [ ] **Step 1: formatter — ดึงสคริปต์ที่พิสูจน์แล้วจากแผน G-08 (ต้องผ่าน `FORMATTER_OK` ก่อนใช้; ไม่ผ่านให้หยุดและรายงาน)**
```bash
cd /Users/syaco/Documents/development/tan-erp && S=${TMPDIR:-/tmp}/openapi-g09 && mkdir -p "$S"
sed -n '/^import json, sys/,/^PY$/p' docs/superpowers/plans/2026-10-15-g08-estimate-unit-conversion.md | sed '$d' > "$S/fmt.py"
python3 "$S/fmt.py" contracts/openapi/tan-erp.v1.json "$S/roundtrip.json" && cmp contracts/openapi/tan-erp.v1.json "$S/roundtrip.json" && echo FORMATTER_OK
```
- [ ] **Step 2: สร้าง snapshot จาก swagger สด (ไม่ใช้ `UPDATE_OPENAPI=1`)**
```bash
cd /Users/syaco/Documents/development/tan-erp && S=${TMPDIR:-/tmp}/openapi-g09
dotnet build backend/TanErp.slnx --nologo -v q
ASPNETCORE_ENVIRONMENT=Test ASPNETCORE_URLS=http://127.0.0.1:5099 \
ConnectionStrings__Database="Host=127.0.0.1;Port=1;Database=openapi;Username=u;Password=p" \
  dotnet run --project backend/src/TanErp.Api --no-build --no-launch-profile > "$S/api.log" 2>&1 &
API_PID=$!
for i in $(seq 1 60); do curl -sf http://127.0.0.1:5099/swagger/v1/swagger.json -o "$S/swagger.json" && break; sleep 1; done
kill $API_PID 2>/dev/null; wait $API_PID 2>/dev/null; lsof -ti :5099 | xargs kill 2>/dev/null
test -s "$S/swagger.json" && python3 "$S/fmt.py" "$S/swagger.json" contracts/openapi/tan-erp.v1.json
git diff --numstat contracts/openapi/tan-erp.v1.json
git diff -U0 contracts/openapi/tan-erp.v1.json | python3 -c '
import sys
minus=[];plus=[]
for l in sys.stdin:
    if l.startswith("---") or l.startswith("+++"): continue
    if l.startswith("-"): minus.append(l[1:].rstrip("\n"))
    elif l.startswith("+"): plus.append(l[1:].rstrip("\n"))
bad=[m for m in minus if (m+",") not in plus]
print("ONLY_COMMA_CHANGES" if not bad else "UNEXPECTED_DELETIONS: "+repr(bad[:5]))'
cd frontend && npm run generate:api && cd .. && git diff --numstat frontend/src/generated/api/tan-erp.v1.ts
```
Expected: `ONLY_COMMA_CHANGES`; เพิ่ม path `/api/v1/pricing-templates/{templateId}/branch-rate-overrides[/{overrideId}/decision|/revoke]`, schema `BranchRateOverrideRequest/Response/ListResponse/DecisionRequest/RevokeRequest`, field `branchOverrideId` (CalculationResponse) และ `mappedLineCount` (ConversionResponse). ถ้ามี `UNEXPECTED_DELETIONS` ให้ตรวจว่าเป็น drift อื่นก่อนไปต่อ.
- [ ] **Step 3: api-client**
```text
FILE frontend/src/lib/api/api-client.ts
>>> หลัง: export type TemplateStepName = "submit" | "calibration" | "activate" | "disable";
export type BranchRateOverrideRequest = components["schemas"]["BranchRateOverrideRequest"];
export type BranchRateOverrideResponse = components["schemas"]["BranchRateOverrideResponse"];
export type BranchRateOverrideListResponse = components["schemas"]["BranchRateOverrideListResponse"];
>>> แทนที่:
  async listQuickEstimates(options: RequestOptions, query: ListQuickEstimatesParams): Promise<QuickEstimateListResponse> {
>>> ด้วย:
  async listBranchRateOverrides(templateId: string, options: RequestOptions): Promise<BranchRateOverrideListResponse> {
    return this.request<BranchRateOverrideListResponse>(`/api/v1/pricing-templates/${encodeURIComponent(templateId)}/branch-rate-overrides`, "GET", options);
  }

  async requestBranchRateOverride(templateId: string, payload: BranchRateOverrideRequest, options: RequestOptions): Promise<BranchRateOverrideResponse> {
    return this.request<BranchRateOverrideResponse>(`/api/v1/pricing-templates/${encodeURIComponent(templateId)}/branch-rate-overrides`, "POST", options, payload);
  }

  async decideBranchRateOverride(templateId: string, overrideId: string, rowVersion: string, decision: "approved" | "returned", note: string | null, options: RequestOptions): Promise<BranchRateOverrideResponse> {
    return this.request<BranchRateOverrideResponse>(`/api/v1/pricing-templates/${encodeURIComponent(templateId)}/branch-rate-overrides/${encodeURIComponent(overrideId)}/decision`, "POST", { ...options, ifMatch: rowVersion }, { decision, note });
  }

  async revokeBranchRateOverride(templateId: string, overrideId: string, rowVersion: string, reason: string, options: RequestOptions): Promise<BranchRateOverrideResponse> {
    return this.request<BranchRateOverrideResponse>(`/api/v1/pricing-templates/${encodeURIComponent(templateId)}/branch-rate-overrides/${encodeURIComponent(overrideId)}/revoke`, "POST", { ...options, ifMatch: rowVersion }, { reason });
  }

  async listQuickEstimates(options: RequestOptions, query: ListQuickEstimatesParams): Promise<QuickEstimateListResponse> {
```
- [ ] **Step 4: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts   # check:api เทียบกับ index
cd frontend && npx tsc --noEmit && npm run lint && npm run check:api
```
Expected: `0 Warning(s)` `0 Error(s)`; tsc/lint/`check:api` exit 0
- [ ] **Step 5: Commit**
```bash
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api
git commit -m "feat(quick-estimates): publish the override and conversion contract

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 8: Frontend — หน้า Branch Rate Override, permission, i18n, ข้อความ conversion

**Files:** Create: `frontend/src/features/quick-estimates/api/branch-override-queries.ts`, `…/components/pricing-template-overrides.tsx`, `frontend/src/app/[locale]/(erp)/pricing-templates/[id]/overrides/page.tsx`; Modify: `frontend/src/lib/permissions/permissions.ts`, `frontend/src/features/quick-estimates/quick-estimate-status.ts`, `…/components/pricing-template-detail.tsx`, `…/components/quick-estimate-workspace.tsx`, `frontend/src/messages/th.json`, `frontend/src/messages/en.json`

**อ่าน `.agents/skills/building-erp-forms/SKILL.md` ก่อน.** กฎ: ไม่ใช้ `any`; ข้อความผ่าน i18n th+en; ฟอร์มใช้ react-hook-form + zod + `Controller`; ปุ่มบันทึก/ยืนยันมี `isLoading` และ disable; เพิกถอน/อนุมัติ/ส่งกลับผ่าน Modal ยืนยัน; loading = `MonoSpinner`; ไม่มี fallback ลูกโซ่ (ไม่มีข้อมูล = `-`); server เป็น authority. **ห้ามแก้ `th.json`/`en.json` ด้วย `json.dumps`** — ใช้ Edit ตาม anchor เท่านั้น.

- [ ] **Step 1: permission + status helpers + error codes**
```text
FILE frontend/src/lib/permissions/permissions.ts
>>> หลัง:   PRICING_TEMPLATES_APPROVE: "pricing-templates.approve",
  PRICING_TEMPLATES_OVERRIDE_RATE: "pricing-templates.override-rate",

FILE frontend/src/features/quick-estimates/quick-estimate-status.ts
>>> แทนที่:
  "PRICING_TEMPLATE_SELF_APPROVAL",
>>> ด้วย:
  "PRICING_TEMPLATE_SELF_APPROVAL",
  "BRANCH_OVERRIDE_INVALID",
  "BRANCH_OVERRIDE_OVERLAP",
  "BRANCH_OVERRIDE_INVALID_STATE",
  "BRANCH_OVERRIDE_SELF_APPROVAL",
>>> แทนที่:
export type QuickEstimateBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";
>>> ด้วย:
export const OVERRIDE_STATUSES = ["requested", "approved", "returned", "revoked"] as const;
export type OverrideStatusValue = (typeof OVERRIDE_STATUSES)[number];
export function isOverrideStatus(value: string | null | undefined): value is OverrideStatusValue {
  return OVERRIDE_STATUSES.some((status) => status === value);
}

export type QuickEstimateBadgeVariant = "success" | "warning" | "danger" | "info" | "neutral";

export function overrideStatusVariant(status: string | null | undefined): QuickEstimateBadgeVariant {
  switch (status) {
    case "approved": return "success";
    case "requested": return "warning";
    case "returned":
    case "revoked": return "danger";
    default: return "neutral";
  }
}
```
- [ ] **Step 2: `api/branch-override-queries.ts`**
```ts
import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type BranchRateOverrideListResponse, type BranchRateOverrideRequest } from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";

export function branchOverridesKey(membershipId: string | undefined, locale: string, templateId: string) {
  return ["business", membershipId, locale, "quick-estimates", "templates", "branch-overrides", templateId] as const;
}

export function useBranchOverrides(templateId: string): UseQueryResult<BranchRateOverrideListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: branchOverridesKey(membershipId, locale, templateId),
    enabled: Boolean(membershipId && templateId),
    queryFn: async ({ signal }) => apiClient.listBranchRateOverrides(templateId, await buildOptions({ signal })),
  });
}

/** Every write refreshes the template's override list. */
export function useBranchOverrideMutations(templateId: string) {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => { await queryClient.invalidateQueries({ queryKey: branchOverridesKey(membershipId, locale, templateId) }); };
  const request = useMutation({
    mutationFn: async (input: { payload: BranchRateOverrideRequest; idempotencyKey: string }) =>
      apiClient.requestBranchRateOverride(templateId, input.payload, await buildOptions({ idempotencyKey: input.idempotencyKey })),
    onSuccess: refresh,
  });
  const decide = useMutation({
    mutationFn: async (input: { overrideId: string; rowVersion: string; decision: "approved" | "returned"; note: string | null }) =>
      apiClient.decideBranchRateOverride(templateId, input.overrideId, input.rowVersion, input.decision, input.note, await buildOptions()),
    onSuccess: refresh,
  });
  const revoke = useMutation({
    mutationFn: async (input: { overrideId: string; rowVersion: string; reason: string }) =>
      apiClient.revokeBranchRateOverride(templateId, input.overrideId, input.rowVersion, input.reason, await buildOptions()),
    onSuccess: refresh,
  });
  return { request, decide, revoke };
}
```
- [ ] **Step 3: `components/pricing-template-overrides.tsx`** (props ของ `Modal`/`PageHeader` ใช้ตามที่ `pricing-template-detail.tsx` ใช้อยู่ — ห้ามเดา prop ใหม่)
```tsx
"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { BranchRateOverrideResponse } from "@/lib/api/api-client";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useBranchOverrideMutations, useBranchOverrides } from "../api/branch-override-queries";
import { isOverrideStatus, overrideStatusVariant, quickEstimateErrorCode } from "../quick-estimate-status";

const RATE = /^\d{1,8}(\.\d{1,4})?$/;
const formSchema = z.object({
  referenceRate: z.string().regex(RATE), minimumCharge: z.string().regex(RATE),
  effectiveFrom: z.string().min(1), effectiveTo: z.string().min(1),
  reasonCode: z.string().trim().min(1).max(64), reasonDetail: z.string().trim().min(1).max(500), sourceReference: z.string().trim().max(200),
}).refine((values) => values.effectiveTo >= values.effectiveFrom, { path: ["effectiveTo"] });
type FormValues = z.infer<typeof formSchema>;
const FIELDS = ["referenceRate", "minimumCharge", "effectiveFrom", "effectiveTo", "reasonCode", "reasonDetail", "sourceReference"] as const;
const EMPTY: FormValues = { referenceRate: "", minimumCharge: "0", effectiveFrom: "", effectiveTo: "", reasonCode: "", reasonDetail: "", sourceReference: "" };

type DialogMode = "approved" | "returned" | "revoke";
const DIALOG_TITLES = { approved: "approveTitle", returned: "returnTitle", revoke: "revokeTitle" } as const;

export function PricingTemplateOverrides({ templateId }: { templateId: string }) {
  const t = useTranslations("quickEstimates.overrides");
  const tErrors = useTranslations("quickEstimates.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canRequest = can(selectedMembership, PERMISSIONS.PRICING_TEMPLATES_OVERRIDE_RATE);
  const canDecide = can(selectedMembership, PERMISSIONS.PRICING_TEMPLATES_APPROVE);
  const query = useBranchOverrides(templateId);
  const mutations = useBranchOverrideMutations(templateId);
  const busy = mutations.request.isPending || mutations.decide.isPending || mutations.revoke.isPending;
  const [formOpen, setFormOpen] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [target, setTarget] = useState<{ mode: DialogMode; item: BranchRateOverrideResponse } | null>(null);
  const [note, setNote] = useState("");
  const [noteError, setNoteError] = useState(false);
  const keyRef = useRef(crypto.randomUUID()); // a retry replays with the same key; a new request gets a new key after success
  const { control, handleSubmit, reset } = useForm<FormValues>({ resolver: zodResolver(formSchema), defaultValues: EMPTY });

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? quickEstimateErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function submitRequest(values: FormValues): Promise<void> {
    setMessage(null);
    try {
      await mutations.request.mutateAsync({
        idempotencyKey: keyRef.current,
        payload: { referenceRate: Number(values.referenceRate), minimumCharge: Number(values.minimumCharge), effectiveFrom: values.effectiveFrom, effectiveTo: values.effectiveTo,
          reasonCode: values.reasonCode, reasonDetail: values.reasonDetail, sourceReference: values.sourceReference || null },
      });
      keyRef.current = crypto.randomUUID();
      toast.success(t("done"));
      reset(EMPTY);
      setFormOpen(false);
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function confirm(): Promise<void> {
    if (!target) return;
    const { mode, item } = target;
    const overrideId = item.id ?? "";
    const rowVersion = item.rowVersion ?? "";
    const text = note.trim();
    if (mode !== "approved" && !text) { setNoteError(true); return; }
    setMessage(null);
    try {
      if (mode === "revoke") await mutations.revoke.mutateAsync({ overrideId, rowVersion, reason: text });
      else await mutations.decide.mutateAsync({ overrideId, rowVersion, decision: mode, note: text || null });
      toast.success(t("done"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
    setTarget(null);
  }

  function open(mode: DialogMode, item: BranchRateOverrideResponse): void {
    setTarget({ mode, item });
    setNote("");
    setNoteError(false);
  }

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;
  const items = query.data.items ?? [];

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/pricing-templates/${templateId}` }]}
        actions={canRequest ? <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => setFormOpen(true)}>{t("request")}</Button> : undefined}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      {items.length === 0 ? <p className="text-sm text-erp-text-muted">{t("empty")}</p> : (
        <ul className="space-y-3">
          {items.map((item) => (
            <li key={item.id} className="erp-card space-y-2 p-4">
              <div className="flex flex-wrap items-center gap-3">
                <span className="font-mono text-sm font-bold text-erp-navy">{item.branchCode ?? "-"}</span>
                <StatusBadge label={isOverrideStatus(item.status) ? t(`statuses.${item.status}`) : "-"} variant={overrideStatusVariant(item.status)} />
                <span className="text-xs text-erp-text-muted">{t("period", { from: item.effectiveFrom ? formatDate(item.effectiveFrom, locale) : "-", to: item.effectiveTo ? formatDate(item.effectiveTo, locale) : "-" })}</span>
              </div>
              <dl className="grid grid-cols-2 gap-3 text-sm md:grid-cols-4">
                <div><dt className="text-xs text-erp-text-muted">{t("referenceRate")}</dt><dd className="font-mono font-semibold">{formatCurrency(item.referenceRate, "THB", locale)}</dd></div>
                <div><dt className="text-xs text-erp-text-muted">{t("minimumCharge")}</dt><dd className="font-mono font-semibold">{formatCurrency(item.minimumCharge, "THB", locale)}</dd></div>
                <div><dt className="text-xs text-erp-text-muted">{t("reasonCode")}</dt><dd className="font-mono">{item.reasonCode ?? "-"}</dd></div>
                <div><dt className="text-xs text-erp-text-muted">{t("sourceReference")}</dt><dd>{item.sourceReference ?? "-"}</dd></div>
              </dl>
              <p className="text-xs text-erp-text-muted">{item.reasonDetail ?? "-"} · {t("requestedBy", { name: item.requestedBy?.displayName ?? "-" })}</p>
              {item.decisionNote && <p className="text-xs text-erp-text-muted">{item.decisionNote}</p>}
              <div className="flex flex-wrap gap-3">
                {canDecide && item.status === "requested" && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => open("approved", item)}>{t("approve")}</Button>}
                {canDecide && item.status === "requested" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("returned", item)}>{t("return")}</Button>}
                {canDecide && item.status === "approved" && <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => open("revoke", item)}>{t("revoke")}</Button>}
              </div>
            </li>
          ))}
        </ul>
      )}

      <Modal isOpen={formOpen} onClose={() => { if (!busy) setFormOpen(false); }} title={t("requestTitle")} size="md" closeDisabled={busy} closeOnOverlayClick={!busy} closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setFormOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.request.isPending} disabled={busy} onClick={() => void handleSubmit(submitRequest)()}>{tCommon("actions.confirm")}</Button>
          </div>
        )}
      >
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
          {FIELDS.map((name) => (
            <Controller key={name} name={name} control={control} render={({ field, fieldState }) => (
              <Input {...field} label={t(name)} type={name === "effectiveFrom" || name === "effectiveTo" ? "date" : "text"} required={name !== "sourceReference"} disabled={busy} error={fieldState.error ? t("fieldInvalid") : undefined} />
            )} />
          ))}
        </div>
      </Modal>

      <Modal isOpen={target !== null} onClose={() => { if (!busy) setTarget(null); }} title={target ? t(DIALOG_TITLES[target.mode]) : undefined} size="sm" closeDisabled={busy} closeOnOverlayClick={!busy} closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setTarget(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirm()}>{tCommon("actions.confirm")}</Button>
          </div>
        )}
      >
        {target && <Input label={t("note")} value={note} maxLength={500} disabled={busy} required={target.mode !== "approved"} error={noteError ? t("reasonRequired") : undefined} onChange={(event) => setNote(event.target.value)} />}
      </Modal>
    </section>
  );
}
```
- [ ] **Step 4: route + ลิงก์จากหน้ารายละเอียด + ข้อความ conversion**
```text
FILE frontend/src/app/[locale]/(erp)/pricing-templates/[id]/overrides/page.tsx   (Create)
"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { useTranslations } from "next-intl";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { PricingTemplateOverrides } from "@/features/quick-estimates/components/pricing-template-overrides";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";

interface PageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function Page({ params }: PageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("quickEstimates");

  return (
    <PermissionGuard permission={PERMISSIONS.PRICING_TEMPLATES_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <PricingTemplateOverrides templateId={id} />
    </PermissionGuard>
  );
}

FILE frontend/src/features/quick-estimates/components/pricing-template-detail.tsx
>>> แทนที่:
import { useRouter } from "next/navigation";
>>> ด้วย:
import Link from "next/link";
import { useRouter } from "next/navigation";
>>> แทนที่:
  const tCommon = useTranslations("common");
>>> ด้วย:
  const tCommon = useTranslations("common");
  const tOverrides = useTranslations("quickEstimates.overrides");
>>> หลัง:           {canManage && status === "draft" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setEditing(true)}>{tCommon("actions.edit")}</Button>}
          {(status === "active" || status === "calibration") && <Link href={`/${locale}/pricing-templates/${id}/overrides`} className="inline-flex min-h-11 items-center border border-erp-border px-4 text-sm font-semibold text-erp-navy">{tOverrides("open")}</Link>}

FILE frontend/src/features/quick-estimates/components/quick-estimate-workspace.tsx
>>> แทนที่:
            {t("conversionLine", { version: conversion.sourceVersion ?? 0 })}{" "}
>>> ด้วย:
            {t("conversionLine", { version: conversion.sourceVersion ?? 0 })}{" "}
            {typeof conversion.mappedLineCount === "number" && <>{t("conversionLines", { count: conversion.mappedLineCount })}{" "}</>}
>>> แทนที่:
          <p className="font-mono text-xs text-erp-text-muted" title={calc.inputHash ?? undefined}>{t("inputHash")}: {(calc.inputHash ?? "").slice(0, 16)}</p>
>>> ด้วย:
          {calc.branchOverrideId && <p className="text-xs font-semibold text-erp-navy">{t("branchOverrideApplied")}</p>}
          <p className="font-mono text-xs text-erp-text-muted" title={calc.inputHash ?? undefined}>{t("inputHash")}: {(calc.inputHash ?? "").slice(0, 16)}</p>
```
- [ ] **Step 5: i18n (th + en ครบทุกคู่ — Edit ตาม anchor)**
```text
FILE frontend/src/messages/en.json
>>> แทนที่:
      "QUICK_ESTIMATE_FIELD_INVALID": "A field value is invalid."
>>> ด้วย:
      "QUICK_ESTIMATE_FIELD_INVALID": "A field value is invalid.",
      "BRANCH_OVERRIDE_INVALID": "Check the rate, the period (at most 366 days, not ending before today) and the reason.",
      "BRANCH_OVERRIDE_OVERLAP": "This branch already has an override request or override for this template in that period.",
      "BRANCH_OVERRIDE_INVALID_STATE": "The current status of the override does not allow this action.",
      "BRANCH_OVERRIDE_SELF_APPROVAL": "You cannot decide an override request you made."
>>> แทนที่:
      "convertDescription": "Creates an empty official estimate draft. The quick estimate snapshot is kept as the source reference.",
>>> ด้วย:
      "convertDescription": "Creates an official estimate draft with one scope line per measurement line. No price or cost is copied; the estimator adds costs and calculates the estimate anew.",
>>> แทนที่:
      "depthMPlaceholder": "0.00"
    }
  },
  "richText": {
>>> ด้วย:
      "depthMPlaceholder": "0.00",
      "conversionLines": "{count} scope line(s) created.",
      "branchOverrideApplied": "A branch rate override was applied to this calculation."
    },
    "overrides": {
      "title": "Branch rate overrides", "subtitle": "A branch-specific rate for a limited period. It needs an independent approver and never changes the standard rate.", "open": "Branch overrides",
      "loadError": "Could not load branch overrides.", "empty": "No branch overrides yet", "request": "Request override",
      "requestTitle": "Request a branch rate override", "referenceRate": "Reference rate", "minimumCharge": "Minimum charge",
      "effectiveFrom": "Effective from", "effectiveTo": "Effective to", "reasonCode": "Reason code",
      "reasonDetail": "Reason detail", "sourceReference": "Source reference", "period": "{from} to {to}",
      "requestedBy": "Requested by {name}", "fieldInvalid": "Check this value.",
      "statuses": { "requested": "Awaiting approval", "approved": "Approved", "returned": "Returned", "revoked": "Revoked" },
      "approve": "Approve", "return": "Return", "revoke": "Revoke",
      "approveTitle": "Approve this override?", "returnTitle": "Return this override?", "revokeTitle": "Revoke this override?",
      "note": "Note", "reasonRequired": "Enter a reason.", "done": "Saved."
    }
  },
  "richText": {

FILE frontend/src/messages/th.json
>>> แทนที่:
      "QUICK_ESTIMATE_FIELD_INVALID": "ค่าของข้อมูลไม่ถูกต้อง"
>>> ด้วย:
      "QUICK_ESTIMATE_FIELD_INVALID": "ค่าของข้อมูลไม่ถูกต้อง",
      "BRANCH_OVERRIDE_INVALID": "ตรวจอัตรา ช่วงวันที่ (ไม่เกิน 366 วัน และต้องไม่สิ้นสุดก่อนวันนี้) และเหตุผลอีกครั้ง",
      "BRANCH_OVERRIDE_OVERLAP": "สาขานี้มีคำขอหรือราคาเฉพาะสาขาของแม่แบบนี้ในช่วงเวลาดังกล่าวอยู่แล้ว",
      "BRANCH_OVERRIDE_INVALID_STATE": "สถานะปัจจุบันของราคาเฉพาะสาขาไม่อนุญาตให้ทำรายการนี้",
      "BRANCH_OVERRIDE_SELF_APPROVAL": "ผู้ขอตัดสินคำขอของตนเองไม่ได้"
>>> แทนที่:
      "convertDescription": "สร้างร่างใบประเมินทางการเปล่า สแนปช็อตของใบประเมินเบื้องต้นถูกเก็บเป็นข้อมูลอ้างอิง",
>>> ด้วย:
      "convertDescription": "สร้างร่างใบประเมินทางการที่มีรายการขอบเขตตามเส้นวัดแต่ละเส้น ไม่คัดลอกราคาหรือต้นทุน ผู้ประเมินต้องเติมต้นทุนและคำนวณใหม่",
>>> แทนที่:
      "depthMPlaceholder": "0.00"
    }
  },
  "richText": {
>>> ด้วย:
      "depthMPlaceholder": "0.00",
      "conversionLines": "สร้างรายการขอบเขต {count} รายการ",
      "branchOverrideApplied": "การคำนวณนี้ใช้ราคาเฉพาะสาขา"
    },
    "overrides": {
      "title": "ราคาเฉพาะสาขา (Branch Rate Override)", "subtitle": "อัตราเฉพาะสาขาตามช่วงเวลา ต้องมีผู้อนุมัติอิสระ และไม่แก้อัตรามาตรฐาน", "open": "ราคาเฉพาะสาขา",
      "loadError": "โหลดรายการราคาเฉพาะสาขาไม่สำเร็จ", "empty": "ยังไม่มีราคาเฉพาะสาขา", "request": "ขอราคาเฉพาะสาขา",
      "requestTitle": "ขอราคาเฉพาะสาขา", "referenceRate": "ราคาอ้างอิง", "minimumCharge": "ราคาขั้นต่ำ",
      "effectiveFrom": "มีผลตั้งแต่", "effectiveTo": "มีผลถึง", "reasonCode": "รหัสเหตุผล",
      "reasonDetail": "รายละเอียดเหตุผล", "sourceReference": "เอกสารอ้างอิง", "period": "{from} ถึง {to}",
      "requestedBy": "ขอโดย {name}", "fieldInvalid": "กรุณาตรวจสอบค่านี้",
      "statuses": { "requested": "รออนุมัติ", "approved": "อนุมัติแล้ว", "returned": "ส่งกลับ", "revoked": "เพิกถอน" },
      "approve": "อนุมัติ", "return": "ส่งกลับ", "revoke": "เพิกถอน",
      "approveTitle": "อนุมัติราคาเฉพาะสาขานี้หรือไม่", "returnTitle": "ส่งราคาเฉพาะสาขานี้กลับหรือไม่", "revokeTitle": "เพิกถอนราคาเฉพาะสาขานี้หรือไม่",
      "note": "หมายเหตุ", "reasonRequired": "กรอกเหตุผล", "done": "บันทึกแล้ว"
    }
  },
  "richText": {
```
- [ ] **Step 6: Compile-only gate (ห้ามรันชุดทดสอบ; `npm run build` ครั้งเดียวของ plan นี้)**
```bash
cd /Users/syaco/Documents/development/tan-erp/frontend && npx tsc --noEmit && npm run lint && node -e '
const th=require("./src/messages/th.json"),en=require("./src/messages/en.json");
const keys=(o,p="")=>Object.entries(o).flatMap(([k,v])=>v&&typeof v==="object"?keys(v,p+k+"."):[p+k]);
const a=new Set(keys(th)),b=new Set(keys(en));
const miss=[...a].filter(k=>!b.has(k)).concat([...b].filter(k=>!a.has(k)));
console.log(miss.length?"KEY_PARITY_FAIL "+miss.slice(0,10).join(","):"KEY_PARITY_OK")' && npm run build
```
Expected: tsc/lint exit 0; `KEY_PARITY_OK`; `next build` ผ่าน. ตรวจ `grep -rnE ": any|as any|@ts-ignore" src/features/quick-estimates` ต้องไม่พบ
- [ ] **Step 7: Commit**
```bash
git add frontend/src
git commit -m "feat(quick-estimates): add the branch rate override screen

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 9: Verification + สถานะ (ไม่ติ๊ก)

**Files:** Create: `docs/05-engineering/quick-estimate-completion-verification.md`; Modify: `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/00-overview/implementation-roadmap.md`, `docs/03-contracts/quick-estimate-api-contract.md`, `docs/05-engineering/quick-estimate-verification.md`

- [ ] **Step 1: รัน compile-gate เต็มครั้งเดียวและจดผลจริง**
```bash
cd /Users/syaco/Documents/development/tan-erp && dotnet build backend/TanErp.slnx --nologo -v q
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
cd frontend && npx tsc --noEmit && npm run lint && npm run check:api
```
ห้ามรัน `dotnet test`/`vitest`/Playwright และห้ามเปิดเบราว์เซอร์.
- [ ] **Step 2: สร้าง `docs/05-engineering/quick-estimate-completion-verification.md`** ตามหัวข้อ (กรอกผลจาก Step 1 ตามจริง): (1) **สถานะ:** Implemented 2026-10-15 — **no tests written, none run**; ไม่ได้ตรวจเบราว์เซอร์; migration `AddBranchRateOverrides` ยังไม่ apply; ไม่ใช่ UAT/Production-ready. (2) **ผลที่รันจริง (compile/lint เท่านั้น):** ตารางคำสั่ง → ผลจริง (`dotnet build`, `has-pending-model-changes`, `FORMATTER_OK`, `ONLY_COMMA_CHANGES`, `check:api`, `tsc`, `lint`, `KEY_PARITY_OK`, `npm run build`). (3) **สิ่งที่เปลี่ยน:** conversion รายการขอบเขต (D1–D3, D9); Branch Rate Override (D4–D7, D10); evidence `quick-estimate` (D8); permission ใหม่ `pricing-templates.override-rate` (seeder + FE + catalog; ไม่เป็น approval-class); error ใหม่ 4 ตัว; test เดิม 2 ไฟล์ที่ถูกบังคับให้แก้ (`attachment-owner-types.test.ts`, `quick-estimate-components.test.tsx`). (4) **ข้อจำกัด/ยังเปิด:** แม่แบบไม่มี cost mapping (N1) → ร่างที่แปลงยัง `ESTIMATE_COST_INCOMPLETE` จนกว่าจะเติมต้นทุน; margin 0.30 (N2); override เฉพาะ rate/ขั้นต่ำ ไม่มีเพดาน % ไม่มีหลักฐานไฟล์ (N3); ผู้ขอยกเลิกคำขอเองไม่ได้; endpoint Quick Estimate เดิมไม่ตรวจ branch (F8); ช่องกรอก Site Survey Revision ยังเป็นข้อความ (F14); ใบที่แปลงก่อน G-09 ไม่ backfill; ต้องมอบ `pricing-templates.override-rate` ให้ Role จริงเอง; `data-contract`/governance ยังไม่ได้ปรับถ้อยคำให้ตรงชุดย่อยนี้. (5) **คำสั่งสำหรับผู้ใช้ (ยังไม่เคยรันในรอบนี้):**
```bash
dotnet test backend/TanErp.slnx --nologo --filter "FullyQualifiedName~QuickEstimate|FullyQualifiedName~Attachment|FullyQualifiedName~FileGate|FullyQualifiedName~ApprovalPermissionCoverage|FullyQualifiedName~OpenApiContract|FullyQualifiedName~EstimateEndpointsTests"
cd frontend && npx vitest run src/features/quick-estimates src/lib/attachments src/features/service
```
- [ ] **Step 3: อัปเดตสถานะ/ถ้อยคำ (ไม่ติ๊กข้อใดว่า verified)**
```text
FILE docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md
>>> แทนที่:
| G-09 | Quick Estimate: conversion พร้อมรายการ, Branch override, หลักฐาน | B | CP-16, G-01, G-08 | Sales + Cost Owner | — |
>>> ด้วย:
| G-09 | Quick Estimate: conversion พร้อมรายการ, Branch override, หลักฐาน | B | CP-16, G-01, G-08 | Sales + Cost Owner | G-09 Implemented 2026-10-15 (no tests written, none run; browser not checked; migration ยังไม่ apply; ยังไม่ผ่าน UAT; ร่างที่แปลงมีรายการขอบเขตแต่ยังไม่มีต้นทุน) — [แผน G-09](2026-10-15-g09-quick-estimate-completion.md); [ADR 0025](../../adr/0025-quick-estimate-branch-override-and-scope-line-conversion.md); [Verification](../../05-engineering/quick-estimate-completion-verification.md) |
>>> แทนที่:
- [ ] Tests: conversion ซ้ำได้ draft เดิม, override ข้าม branch ไม่ได้
>>> ด้วย:
- [ ] Tests: conversion ซ้ำได้ draft เดิม, override ข้าม branch ไม่ได้ — **ยังไม่เขียน** (คำสั่งผู้ใช้ให้ไม่เขียน test ในรอบนี้)

> **สถานะ G-09 (2026-10-15):** Implemented — no tests written, none run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: conversion เติม Work Item ขอบเขตต่อเส้นวัด (ปริมาณ+หน่วยของแม่แบบ ไม่คัดลอกราคา/ต้นทุน; เติมเฉพาะร่างที่ว่าง; ไม่ backfill); Branch Rate Override (`pricing-templates.override-rate` ขอ, `pricing-templates.approve` ตัดสิน/เพิกถอน, เฉพาะ rate/ขั้นต่ำ ≤ 366 วัน, ข้ามสาขาไม่ได้); evidence owner `quick-estimate`. **ข้อเท็จจริงที่คลาดจากแผนหลัก:** แม่แบบไม่มี item/cost mapping จึงไม่ใช้ `IUnitConversionResolver` ของ G-08. **ยังไม่ทำ/ยังเปิด:** cost recipe ในแม่แบบ (decision N1), เพดาน % ของ override, branch scope ของ endpoint Quick Estimate เดิม, test ทั้งหมด. [Verification](../../05-engineering/quick-estimate-completion-verification.md)

FILE docs/00-overview/implementation-roadmap.md
>>> แทนที่:
ไม่มี mobile/offline/Branch override/promotion/อัปโหลดหลักฐาน, conversion สร้างร่างเปล่า;
>>> ด้วย:
ไม่มี mobile/offline/promotion; G-09 (2026-10-15, no tests written/none run, ยังไม่ตรวจเบราว์เซอร์/UAT): conversion สร้างร่างพร้อมรายการขอบเขต (ยังไม่มีต้นทุน), Branch Rate Override ผ่าน maker–checker, แนบรูปหลักฐานด้วย Shared Attachment;

FILE docs/03-contracts/quick-estimate-api-contract.md
>>> หลัง: ## สถานะการ implement (CP-16)
> **G-09 (2026-10-15):** Branch Rate Override, conversion พร้อมรายการขอบเขต และ evidence (owner `quick-estimate`) implement แล้วแบบ compile-only — ไม่มี test, ยังไม่ผ่าน UAT; ดู [Verification](../05-engineering/quick-estimate-completion-verification.md)

FILE docs/05-engineering/quick-estimate-verification.md
>>> หลัง: ## ข้อจำกัด
> **อัปเดต G-09 (2026-10-15):** ข้อจำกัดเรื่อง Branch override / อัปโหลดหลักฐาน / "conversion สร้างร่างเปล่า" ด้านล่างถูกแทนที่ด้วย [Quick Estimate Completion Verification](quick-estimate-completion-verification.md) (ยังไม่มี test และไม่ผ่าน UAT)
```
- [ ] **Step 4: ตรวจลิงก์/ไม่ติ๊ก** — `grep -n "G-09" docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` ต้องไม่มี `[x]` ใน G-09; ลิงก์ใน 3 ไฟล์ชี้ไฟล์ที่มีอยู่
- [ ] **Step 5: Commit**
```bash
git add docs
git commit -m "docs(quick-estimates): record G-09 verification and update status

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Self-review

- ทุก path ที่ cite มีอยู่จริงหรือระบุ Create; Edit ทุกจุดตรวจ old_string กับไฟล์จริงแล้ว (ข้อยกเว้นที่ให้ implementer ยืนยัน: props ของ `Modal`/`PageHeader` ใน Task 8 ให้เทียบ `pricing-template-detail.tsx`).
- ลำดับ Task คอมไพล์ได้ทีละ commit: พารามิเตอร์/field ใหม่เป็น optional ท้ายสุด (`Calculate(..., RateOverride? = null)`, `CalculationProjection.BranchOverrideId`, `ConversionProjection.MappedLineCount`, `CalculationResponse.BranchOverrideId`, `ConversionResponse.MappedLineCount`); `IEstimateStore` ไม่เปลี่ยน; DI ของ handler ใหม่อยู่ Task 4 หลัง Task 3.
- ไม่มี test ใหม่/ไม่มีขั้นรัน test; test เดิมที่ถูกบังคับให้แก้มี 2 ไฟล์ (Task 6) + regenerate OpenAPI snapshot (Task 7).
- Migration: generate เท่านั้น (Task 2) ห้าม apply; ไม่มี migration เพิ่มสำหรับ conversion/evidence.
- ค่าที่ธุรกิจยังไม่ยืนยัน (margin 0.30, 366 วัน, ไม่มีเพดาน %) อยู่ใน Decisions needed N2–N3.
