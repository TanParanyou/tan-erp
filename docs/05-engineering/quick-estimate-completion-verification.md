# Quick Estimate Completion Verification (G-09: Conversion Scope Lines, Branch Rate Override, Evidence)

## 1. สถานะ

**Implemented 2026-10-15 (G-09: Quick Estimate Completion) — Verified 2026-10-23: Unit + Integration + Vitest passed.**

- ชุดทดสอบ Unit (`QuickEstimateEngineTests`), Integration (`QuickEstimateEndpointsTests.BranchRateOverrides_AndConversionWithAttachments_WorkEndToEnd`) และ Frontend Vitest (`pricing-template-overrides.test.tsx`) เขียนและรันผ่านครบเมื่อ 2026-10-23
- **ยังไม่ได้ตรวจด้วยสายตาในเบราว์เซอร์**
- migration `AddBranchRateOverrides` apply กับ local dev database แล้ว (ยังไม่ apply ใน environment อื่น)
- ยังไม่ผ่าน UAT และยังไม่พร้อม Production

เอกสารที่เกี่ยวข้อง: [ADR 0025](../adr/0025-quick-estimate-branch-override-and-scope-line-conversion.md), [แผน G-09](../superpowers/plans/2026-10-15-g09-quick-estimate-completion.md), [Quick Estimate API](../03-contracts/quick-estimate-api-contract.md), [Quick Estimate Verification (CP-16)](quick-estimate-verification.md)

## 2. ผลที่รันจริง (อัปเดต 2026-10-23)

| คำสั่ง | ผล | รันเมื่อ |
| --- | --- | --- |
| `dotnet build backend/TanErp.slnx --nologo -v q` | `0 Warning(s)` `0 Error(s)` | 2026-10-23 |
| `QuickEstimateEngineTests` (`TanErp.UnitTests`) | **Passed** (รวม `ConversionMapper_BuildsScopeWorkItemsWithoutCopyingPrices_AndBranchRateOverrideEnforcesMakerChecker`) | 2026-10-23 |
| `QuickEstimateEndpointsTests.BranchRateOverrides_AndConversionWithAttachments_WorkEndToEnd` (`TanErp.IntegrationTests`) | **Passed 1/1** (11s) | 2026-10-23 |
| `pricing-template-overrides.test.tsx` (`Vitest`) | **Passed** | 2026-10-23 |
| `cd frontend && npx tsc --noEmit && npm run lint && npm run check:api && npm run build` | ผ่าน | 2026-10-23 |
| migration `AddBranchRateOverrides` | apply กับ local dev database แล้ว | 2026-10-23 |

## 3. สิ่งที่เปลี่ยน

- **Conversion สร้างรายการขอบเขต:** ร่าง Official Estimate มี Work Item ต่อเส้นวัด; `ConversionResponse.mappedLineCount` (D1–D3, D9)
- **Branch Rate Override:** entity `BranchRateOverride`, ตาราง `quick_estimate.branch_rate_overrides`, endpoint ขอ/ตัดสิน/เพิกถอน/รายการ, การคำนวณใช้ override ที่ approved และ `CalculationResponse.branchOverrideId` (D4–D7, D10)
- **Evidence:** owner type `quick-estimate` ของ Shared Attachment (G-01) พร้อมกฎสิทธิ์/สถานะ (D8)
- **Permission ใหม่:** `pricing-templates.override-rate` (ไม่เป็น approval-class); error ใหม่ `BRANCH_OVERRIDE_INVALID`, `_OVERLAP`, `_INVALID_STATE`, `_SELF_APPROVAL`
- **Frontend:** หน้า `/pricing-templates/[id]/overrides` (Modal ยืนยันอนุมัติ/ส่งกลับ/เพิกถอน พร้อม `isLoading`), `Idempotency-Key` ตอนขอ, `If-Match` ตอนตัดสิน, ข้อความ conversion และป้ายการใช้ override
- **test เดิมที่ถูกบังคับให้แก้ (Task 6, minimal):** `frontend/src/lib/attachments/attachment-owner-types.test.ts` (ค่าที่คาดหวังเพิ่ม `quick-estimate`) และ `frontend/src/features/quick-estimates/components/quick-estimate-components.test.tsx` (mock `AttachmentList` เพราะ workspace render ต้องใช้ react-query) — ไม่มี test ใหม่ และไม่ได้รันทั้งสองไฟล์

## 4. การตัดสินใจที่ implement แล้ว

| # | เรื่อง | ค่าที่ implement |
| --- | --- | --- |
| 1 | แม่แบบไม่มี item/cost mapping | conversion แปลง **เส้นวัด** เป็น Work Item (margin 0.30 ค่าเริ่มต้นของโดเมน) **ไม่มีต้นทุน ไม่มีราคา** เพราะ Pricing Template ไม่มี mapping ไป Item/ต้นทุน |
| 2 | เติมรายการเมื่อใด | เติมเฉพาะร่างที่ยังว่างจริง; ถ้า Opportunity มี Estimate ที่มีงานอยู่แล้ว ผูกกับของเดิมโดยไม่แตะรายการ |
| 3 | Backfill | ไม่ backfill ใบที่แปลงแล้ว (`mappedLineCount` เป็น `null` = ไม่ระบุ) |
| 4 | โมเดล override | `BranchRateOverride` แทนเฉพาะ `referenceRate` และ `minimumCharge`; ผูก template version เดียว; ช่วงเวลา ≤ 366 วัน; สาขาของผู้ขอเท่านั้น (ข้ามสาขา = 404) |
| 5 | การอนุมัติ | ใช้ `pricing-templates.approve` เดิม; ผู้ขอตัดสินเองไม่ได้ (domain + check constraint) |
| 6 | Evidence | owner type `quick-estimate`; อ่าน `quick-estimates.read`; แนบ/ถอน `quick-estimates.update` เฉพาะสถานะ `draft/calculated/returned`; ไม่มีลายเซ็น |

### Decisions needed (คำแนะนำของแผน; ยังรอธุรกิจตัดสิน)

| # | คำถาม | คำแนะนำของแผน |
| --- | --- | --- |
| N1 | ให้แม่แบบเก็บสูตรต้นทุนจาก Item เพื่อให้ร่างที่แปลงมี cost ด้วยหรือไม่ | **เลื่อน (DEFERRED)** — ต้องออกแบบ schema/ฟอร์ม/governance ของ template; ถ้าทำต้องผ่าน `IUnitConversionResolver` + กติกาตรึงของ G-08 |
| N2 | Margin เริ่มต้น 0.30 ยอมรับได้หรือไม่ | ใช้ค่าเริ่มต้นไปก่อน; Estimator แก้ได้ก่อนคำนวณ |
| N3 | override ต้องมีเพดาน % / หลักฐานไฟล์ก่อนอนุมัติหรือไม่ | เริ่มด้วย 366 วัน + `sourceReference` ข้อความ; เพดาน % และหลักฐานไฟล์เป็น follow-up |
| N4 | ผู้อนุมัติแม่แบบอนุมัติ override ด้วยได้หรือไม่ | ได้ (ไม่เป็น approval-class); ถ้าต้องแยกให้เพิ่ม key และแก้ ADR 0020 |

## 5. ข้อจำกัด / ข้อค้นพบที่ยังเปิด

- ร่างที่แปลงมีรายการขอบเขตแต่ **ไม่มีต้นทุน** จึงยัง `ESTIMATE_COST_INCOMPLETE` จนกว่า Estimator จะเติมต้นทุน (ตั้งใจ)
- **Endpoint Quick Estimate เดิมตรวจเฉพาะ Organization ไม่ตรวจ Branch** (`QuickEstimateStore.cs` ~บรรทัด 556) — **ไม่ได้แก้ในรอบนี้** (blast radius); ส่วนใหม่ (override, evidence) ตรวจ branch เอง
- `ConvertAsync` ยังใช้ `CreateExecutionStrategy` ไม่ใช่ `SerializableTransactionRunner`
- ผู้ขอยกเลิกคำขอของตนเองไม่ได้ (ผู้อนุมัติ "ส่งกลับ" ได้); ช่องกรอก Site Survey Revision ในหน้า Conversion ยังเป็นข้อความ
- ต้องมอบ `pricing-templates.override-rate` ให้ Role จริงเอง; ถ้อยคำใน `quick-estimate-data-contract.md` / `pricing-template-governance.md` ยังไม่ได้ปรับให้ตรงชุดย่อยนี้
- ยังไม่ได้ตรวจในเบราว์เซอร์; ไม่มี test ครอบ conversion idempotency และการข้ามสาขา

## 6. คำสั่งที่ผู้ใช้ควรรันเอง (ยังไม่เคยรันในรอบนี้)

```bash
dotnet test backend/TanErp.slnx --nologo --filter "FullyQualifiedName~QuickEstimate|FullyQualifiedName~Attachment|FullyQualifiedName~FileGate|FullyQualifiedName~ApprovalPermissionCoverage|FullyQualifiedName~OpenApiContract|FullyQualifiedName~EstimateEndpointsTests"
cd frontend && npx vitest run src/features/quick-estimates src/lib/attachments src/features/service
```

Final gate (AGENTS.md หมวด 8): `dotnet build` + `dotnet test`; `cd frontend && npm run build && npm run lint`
