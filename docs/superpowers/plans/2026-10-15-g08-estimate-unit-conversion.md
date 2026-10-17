# G-08 Estimate Unit Conversion Snapshot Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax. Read `AGENTS.md`, `design.md`, `CONTEXT.md` และ `.agents/skills/building-erp-apis/SKILL.md` before Task 2 (`building-erp-forms` อ่านก่อน Task 6). Reply to the user in Thai; code, identifiers and code comments stay in English. macOS: use BSD `sed -i ''`. การแก้ไฟล์เดิมทุกจุดใช้ Edit tool ตาม old_string/new_string ที่ให้ไว้ (ต้องตรงทุกตัวอักษรและมีครั้งเดียว) — ถ้า old_string ไม่ตรงให้หยุดและรายงาน ห้ามเดา.

**กฎของผู้ใช้ที่บังคับทุก Task (สำคัญ — override รูปแบบ TDD ปกติ):** รอบนี้ **ห้ามเขียน test ใหม่ทุกชนิด** (ไม่มีไฟล์ test, ไม่มี RED/GREEN) และ implementer **ห้ามรันชุดทดสอบใด ๆ** (`dotnet test`, `vitest`, Playwright) และไม่เปิดเบราว์เซอร์. การตรวจของทุก Task คือ **compile-only**: backend `dotnet build backend/TanErp.slnx --nologo -v q` (ต้อง `0 Warning(s)` `0 Error(s)`; `TreatWarningsAsErrors` เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย — test เดิมต้องยัง compile ได้); frontend `cd frontend && npx tsc --noEmit`, `npm run lint`, ตรวจ key parity th/en, และ `npm run build` ครั้งเดียวใน Task 6. `dotnet ef migrations add/has-pending-model-changes`, การ regenerate OpenAPI และ `npm run generate:api` เป็นเครื่องมือ build จึงทำได้; **ห้าม `dotnet ef database update`** (controller เป็นผู้ apply migration). ถ้า test เดิมไม่ compile เพราะ Task นี้ ให้แก้ **เฉพาะจุดที่ถูกบังคับ** แบบ minimal และรายงาน (แผนนี้คาดว่าไม่มี — ทุก parameter ใหม่ของ ctor/record เป็น optional ท้ายสุด).

**Goal:** ตอนนี้ Estimate รับต้นทุนจาก Catalog ได้เฉพาะ "หน่วยฐาน" ของสินค้า (หน่วยอื่น → 422 `ESTIMATE_UNIT_INVALID`) ทั้งที่ Item Master มีตาราง conversion พร้อมแล้ว. slice นี้ให้ Estimator เลือกหน่วยอื่นที่มี conversion ใช้งานอยู่ได้ โดยระบบ **ตรึง snapshot** `{ conversionId, scope, fromUnit, toUnit, factor }` บน cost component ตอนบันทึก draft, คำนวณต้นทุนจาก snapshot ผ่าน **Calculation Policy Version ใหม่**, ปฏิเสธหน่วยต่างมิติ, และคง Estimate เดิมทั้งหมดไว้เหมือนเดิม.

**Architecture:** (1) Domain: `EstimateConversionSnapshot` + 5 คอลัมน์แบนบน `EstimateCostComponent` + `CalculationPolicyVersion.UnitConversionMode` (`none|snapshot`); `EstimateRevision.Calculate` ใช้ snapshot เฉพาะเมื่อ policy เป็น `snapshot`. (2) Persistence: 1 migration (คอลัมน์ nullable + default `'none'` ไม่ backfill). (3) Application: port `IUnitConversionResolver` (ใช้ตาราง conversion เดิม — ไม่สร้างตารางใหม่). (4) `EstimateStore.UpdateDraftAsync` ตรึง/คงค่า snapshot. (5) Api: field ใหม่แบบ optional บน request/response, endpoint อ่านตัวเลือกหน่วย. (6) Frontend: dropdown หน่วยบนแถวต้นทุนที่ผูก catalog.

**Tech Stack:** .NET `net10.0` (`AGENTS.md` ที่เขียน ".NET 9" ล้าสมัย), EF Core + Npgsql, PostgreSQL, Next.js + TypeScript strict, next-intl, TanStack Query.

## Scope

**ทำในรอบนี้ (ทุก bullet ของ G-08 ยกเว้น test):** ตรึง conversion snapshot บน cost component ตอนเลือกหน่วย; Calculation Policy Version ใหม่ที่ใช้ snapshot โดย Estimate เดิมคงผลเดิม; ปฏิเสธหน่วยต่างมิติ; revision/copy คง snapshot; แสดงหลักฐาน conversion ให้ผู้ตรวจอนุมัติเห็น; API/OpenAPI/UI ที่เกี่ยวข้อง.

**ไม่ทำโดยตั้งใจ:** การเขียน/รัน test (คำสั่งผู้ใช้); admin API/UI สำหรับสร้างและ publish Calculation Policy (ไม่มีอยู่เลยในระบบ — ดู D6); conversion หลายขั้น (multi-hop) ระหว่างหน่วย; conversion ที่ระดับ Work Item (หน่วยที่ลูกค้าเห็น); เปลี่ยน Quotation/PDF; backfill Estimate เดิม; G-09 (ใช้ port ที่สร้างที่นี่ภายหลัง).

## Findings จากการสำรวจโค้ด (ตรวจแล้ว)

| หัวข้อ | สิ่งที่พบ | ผลต่อแผน |
| --- | --- | --- |
| **มีตาราง conversion แล้ว 2 ชนิด** | `ItemUnitConversion` (เฉพาะ Item; `Domain/Items/ItemUnitConversion.cs:5-18`, ตาราง `item_master.item_unit_conversions`) และ `UnitConversion` (ใช้ร่วม; `Domain/Items/UnitConversion.cs`). Factor บวก ≤ 6 ทศนิยม (`ItemUnitConversion.cs:27-28`); ความหมาย **1 FromUnit = Factor ToUnit** (barcode ตรวจ `quantityInBaseUnit == factor`: `ItemStore.cs:1558-1559`); Item-specific ต้องชี้ไปที่ **หน่วยฐาน** (`ItemStore.cs:1344-1345`); immutable version ไม่ซ้อนช่วงวันที่ (`ItemStore.cs:1370`); `docs/04-data/item-master-data-contract.md:100` | **ไม่สร้างตาราง conversion ใหม่**; resolver อ่านตารางเดิม: Item-specific ก่อน แล้ว Shared ที่ชี้หน่วยฐานโดยตรง (ลำดับเดียวกับ barcode `ItemStore.cs:1547-1557`) |
| มิติหน่วยถูกตรวจตอนสร้าง conversion แต่ **เปลี่ยนภายหลังได้** | สร้าง: `ItemStore.cs:1351-1352` (item) และ `:1430-1431` (shared). แต่ `UnitOfMeasure.Update` แก้ `Dimension` ได้ (`UnitOfMeasure.cs:88`) | resolver ต้องตรวจมิติ **ซ้ำ ณ วันตรึง** ไม่เชื่อว่ามีแถว conversion = มิติตรงกัน |
| Conversion แก้ไม่ได้ผ่าน API | `ItemUnitConversionsController.cs:26,40` และ `UnitConversionsController.cs:26,38` มีแค่ GET/POST; ไม่มี end-date/deactivate | ข้อ test ใน gap plan "แก้ conversion ใน Item Master" ทำได้จริงเพียงเพิ่มรุ่นใหม่ที่ช่วงไม่ซ้อน; snapshot ต้องกันกรณีนี้ (D5) |
| **Estimate รับเฉพาะหน่วยฐาน** | `EstimateStore.cs:438-443` โยน `ESTIMATE_UNIT_INVALID` ถ้า `cDto.UnitCode` ≠ `item.BaseUnit.NormalizedCode`; cost resolve ที่ `item.BaseUnitId` (`:445-453`); client ต้องส่ง `unitCost` = ต้นทุนที่ resolve (`:474`); `docs/03-contracts/official-estimate-api-contract.md:107` ระบุว่ายังไม่ใช้ conversion | ต้องแก้จุดนี้ที่เดียว; ข้อความในสัญญาต้องแก้ (Task 1) |
| จุดที่ snapshot ต้นทุนถูกตรึง | ทุก `UpdateDraftAsync` ลบแล้วสร้าง section/work item/cost component ใหม่ทั้งหมด (`EstimateStore.cs:348-352`, `:365-`) แล้ว resolve ต้นทุนใหม่ + `SetCatalogCostSnapshot` (`:482-499`; `EstimateCostComponent.cs:117-178`) | snapshot conversion ตรึงที่จังหวะเดียวกัน (บันทึก draft) และ **ต้องคงค่าเดิม** ข้ามการลบ-สร้างใหม่ (D5) |
| ต้นทุนรวมของแถวคำนวณตอน save ไม่ใช่ตอน Calculate | `EstimateCostComponent.cs:85,113,177` (`TotalCost = round(Quantity × UnitCost, 2)`); `EstimateRevision.Calculate` เรียก `section.Recalculate()` (`EstimateRevision.cs:295-298`) ซึ่งรวม `TotalCost` ของแถว (`EstimateSection.cs:75-84`) | จึงให้ `Calculate` สั่งแถวที่มี snapshot คำนวณ total จาก snapshot ก่อนรวม (Task 2) |
| **"Calculation version" คลุมเครือ** | `EstimateRevision.CalculationVersion` เป็นตัวนับ int ที่เพิ่มทุกครั้งที่คำนวณ (`EstimateRevision.cs:379`); ตัว "รุ่นกฎ" คือแถว `CalculationPolicyVersion` (`CalculationPolicyVersion.cs:12-86`; ตาราง `estimates.calculation_policy_versions`) ที่ตรึงชื่อ `CODE-vN` ลง revision/snapshot (`EstimateRevision.cs:376`, `EstimateCalculationSnapshot.cs:46`) | "Calculation version ใหม่" ของ gap plan = **แถว policy ใหม่** (`version+1`) ไม่ใช่ตัวนับ |
| Policy ทำหน้าที่แค่ overhead/rounding | policy มี `OverheadMethod/Value`, `RoundingMode` เท่านั้น (`CalculationPolicyVersion.cs:20-24`); resolve ตามสาขา ต้องเหลือแถวเดียว (`EstimateStore.cs:1206-1257`) ไม่งั้น `ESTIMATE_POLICY_UNAVAILABLE` | เพิ่มคอลัมน์ `unit_conversion_mode`; ห้าม seed แถว effective ซ้อนใน test seeder (จะทำให้ resolve ได้ `null` ทุก test: `TestOnlyDataSeeder.cs:441-448`) |
| **ไม่มีทางสร้าง/publish policy นอกโค้ด** | `CalculationPolicyVersion` ถูกสร้างเฉพาะใน seeder (`TestOnlyDataSeeder.cs:443`) และ test; `Publish` ต้องมี checker อิสระ (`CalculationPolicyVersion.cs:58-68`); ไม่มี controller/handler | ความสามารถนี้ถูก "เปิด" ด้วยแถว policy ที่ ops สร้าง — ระบุเป็นข้อจำกัดและ follow-up (D6) |
| Hash ของ policy | `ContentHash` คำนวณตอน publish จากสตริงคงที่ (`CalculationPolicyVersion.cs:78-82`) และเก็บลง snapshot | เพิ่ม mode ต่อท้ายสตริง **เฉพาะ** เมื่อไม่ใช่ `none` เพื่อให้ policy เดิมคำนวณ hash ได้เท่าเดิม |
| Quotation/PDF อ่านระดับ Work Item | `EstimateStore.cs:1992` (`item.Quantity, item.UnitCode, item.UnitSellingPrice, item.TotalSellingPrice`) และ `Infrastructure/Documents/QuotationPdfModelBuilder.cs:59` | ไม่ต้องแก้ Quotation/PDF; ตัวเลขที่แปลงแล้วไหลผ่าน `TotalCost` → ราคาขาย |
| หลักฐานต้นทุนของผู้อนุมัติ | `EstimateReviewQueueReader.cs:125-133` ส่ง `Quantity/UnitCode/UnitCost/TotalCost` ต่อ component | เพิ่ม conversion ต่อแถว (Task 4) มิฉะนั้นผู้อนุมัติเห็น `2 BAG × 1,200` โดยไม่รู้ว่า 1 BAG ≠ หน่วยฐาน |
| Snapshot/hash ของการคำนวณ | `CalculateInputHash` (`EstimateStore.cs:2156-2210`) และ `snapshotObj` (`EstimateRevision.cs:384-455`) ไม่รู้จัก conversion; `InputHash` ไม่ถูกคำนวณซ้ำเพื่อตรวจที่ใดเลย | เพิ่ม field ลง input hash และ snapshot JSON (มีผลเฉพาะการคำนวณใหม่) |
| Revision / copy | `EstimateRevision.CloneAsDraft` (`:132-182`, เรียกจาก `Estimate.cs:97`) คัดลอกต้นทุน catalog ผ่าน `SetCatalogCostSnapshot` แล้ว | ต้องคัดลอก conversion ต่อท้ายจุดเดียวกัน |
| UI: ไม่มีตัวเลือกหน่วยเลย | `estimate-cost-component-table.tsx` ไม่มีช่อง `unitCode` (แสดงเฉพาะ type/description/quantity/unitCost); `catalogCostComponents` ตั้ง `quantity: 1, unitCode: cost.unitCode` (`estimate-workspace-mapper.ts:84-93`) | เพิ่ม dropdown หน่วยบนแถวที่มี `itemId` เท่านั้น (Task 6) |
| Catalog search หนัก | `EstimateCatalogReader.cs` 301 บรรทัด (cursor + facets + unique-winner query) | ไม่ยัดตัวเลือกหน่วยลงทุกแถวของ search; ใช้ endpoint แยก `GET …/items/{itemId}/units` (D12) |

### ข้อที่แผนหลัก (gap plan) คลาดเคลื่อน/ต้องปรับ

1. "Calculation version ใหม่" — ที่ถูกคือ **Calculation Policy Version** (แถว `calculation_policy_versions`); `CalculationVersion` ในโค้ดเป็นตัวนับต่อ revision.
2. "ตรึงตอนเลือกหน่วย" — UI ไม่มีการเลือกหน่วยอยู่ก่อน และทุก snapshot ในโมดูลนี้ถูกตรึงตอน **บันทึก draft**; จึงตรึงที่จังหวะนั้น.
3. "แตะ `Domain/Estimates/`, calculation policy ที่ version แล้ว" — ครบเฉพาะ domain; ต้องแตะ `Infrastructure/Persistence/Estimates/EstimateStore.cs` (จุดตรึงจริง), Items resolver และ API contract ด้วย.
4. "แก้ conversion ใน Item Master" — ไม่มี API แก้/ปิด conversion (ดู Findings).
5. ไม่มีใครสามารถสร้าง/publish policy ใหม่ได้นอกโค้ด — gap plan ไม่ได้พูดถึง.

## Decisions ที่ผู้ใช้อาจ veto

| # | ตัดสินใจ | เหตุผล / ทางเลือกที่ไม่เลือก |
| --- | --- | --- |
| D1 | **ใช้ตาราง conversion เดิม ไม่สร้างใหม่:** Item-specific (ชี้หน่วยฐาน) ก่อน แล้ว Shared ที่ชี้หน่วยฐานโดยตรง; **ไม่รองรับหลายขั้น** (A→B→base) → ไม่พบ = `ESTIMATE_UNIT_CONVERSION_NOT_FOUND` | ตรงกับกติกา barcode เดิม; multi-hop ต้องมี path/cycle logic และ snapshot ที่เป็นลำดับ (เลื่อนเป็น follow-up) |
| D2 | **เก็บเป็น 5 คอลัมน์แบน nullable** บน `estimate_cost_components` (`conversion_id/scope/from_unit/to_unit/factor`) + check "ครบทั้งชุดหรือ null ทั้งชุด" + factor > 0; **ไม่มี FK** ไปตาราง conversion (อ้างได้สองตารางและ snapshot ต้องอยู่รอดแม้ต้นทางถูกลบ). API แสดงเป็น object ซ้อน `conversionSnapshot` และ `factor` เป็น **string** (6 ทศนิยม ตามสัญญา conversion เดิม) ; JSON ใช้เฉพาะใน calculation snapshot | ไม่ใช้ JSONB เพราะต้อง query/constraint และสอดคล้องคอลัมน์ snapshot เดิมที่แบนทั้งหมด (`EstimateCostComponentConfiguration.cs:44-65`) |
| D3 | **ระดับ cost component เท่านั้น** (แถวต้นทุนที่ผูก catalog item); หน่วยของ Work Item (ที่ลูกค้าเห็น) ไม่เปลี่ยน | Quotation/PDF อ่านระดับ Work Item จึงไม่ต้องแก้ และไม่เสี่ยงเปลี่ยนเอกสารลูกค้า |
| D4 | **สูตร:** `ต้นทุนแถว = round(Quantity × Factor × UnitCostSnapshot, 2, AwayFromZero)` คิดที่ความแม่นยำ decimal เต็ม **ปัดครั้งเดียวตอนท้าย**; `UnitCost` (ต่อหน่วยที่เลือก, ใช้แสดง) = `round(UnitCostSnapshot × Factor, 4)`; `UnitCostSnapshot`/`UnitSnapshot` ยังเป็นต้นทุน/หน่วย **ฐาน** ที่ resolve จริง. ปริมาณที่ใช้เลือกขั้นราคา (cost tier) = `Quantity × Factor` (หน่วยฐาน) ไม่ปัด. **ไม่ใช้** `DecimalScale/RoundingMode` ของหน่วย (โค้ด component เดิมไม่ใช้) | ปัดครั้งเดียวไม่สะสมความคลาดเคลื่อนจากการปัด unit cost 4 ตำแหน่งก่อนคูณ; `Quantity×UnitCost` ของแถวแปลงอาจต่างจาก total ไม่เกินครึ่งสตางค์ต่อหน่วยจำนวน — ยอมรับ |
| D5 | **ตรึงที่ "บันทึก draft" ไม่ใช่ตอนคลิก:** (a) หน่วย = หน่วยฐาน → ไม่มี snapshot; (b) หน่วยอื่น + `conversionId` ที่ client ส่งมา **เท่ากับ** snapshot เดิมของแถวเดียวกัน (id เดียวกัน, item เดียวกัน, หน่วยเดียวกัน) → **คงของเดิมโดยไม่ค้นใหม่**; (c) นอกนั้นค้น conversion ที่ effective วันนี้ (UTC date ตามที่ barcode ใช้) แล้วตรึงใหม่ — ถ้า client ส่ง `conversionId` ที่ไม่ตรงกับที่ค้นได้ → `409 ITEM_COST_VERSION_CONFLICT` (stale). เปลี่ยนจำนวน/ราคาไม่เปลี่ยน snapshot. การ "รีเฟรช" = ไม่ส่ง `conversionId` (เลือกหน่วยใหม่) | ถ้าไม่คงค่า การลบ-สร้างใหม่ทุก save (`EstimateStore.cs:348-352`) จะทำให้ factor เปลี่ยนเงียบ ๆ เมื่อมีรุ่น conversion ใหม่ — ขัดกับเป้า "ตรึง" |
| D6 | **Policy ใหม่ = คอลัมน์ `unit_conversion_mode` (`none`\|`snapshot`) บน `calculation_policy_versions`**, แถวเดิมทั้งหมด = `none`. ใช้สองจุด: (1) ตอน save ถ้า policy ที่ effective ของสาขาไม่ใช่ `snapshot` → หน่วยที่ไม่ใช่หน่วยฐานยังได้ `ESTIMATE_UNIT_INVALID` เหมือนเดิมทุกประการ; (2) ตอน `Calculate` ถ้ามีแถวที่มี snapshot แต่ policy ไม่ใช่ `snapshot` → `EstimatePolicyUnavailableException` (→ `409 ESTIMATE_POLICY_UNAVAILABLE`) ไม่คำนวณเงียบ ๆ. **ไม่ seed policy v2 ที่ effective** (ดู Findings). แถว v2 (`version = v1+1`, ค่า overhead/rounding เท่าเดิม, `unit_conversion_mode='snapshot'`, ช่วงเวลาต่อจาก v1) ต้องสร้าง+publish โดย ops ด้วยกลไกเดียวกับ v1 (ไม่มี API) — **admin API ของ policy เป็น follow-up ที่ต้องให้ผู้ใช้อนุมัติ** | แยก "ความสามารถ" ออกจาก "การเปิดใช้" ได้โดยไม่แตะ Estimate ที่อนุมัติแล้ว |
| D7 | **Estimate ที่กำลังทำ/ที่อนุมัติแล้ว: ไม่ backfill และไม่ lazy-snapshot.** ทุกแถวเดิมถูกจำกัดที่หน่วยฐานมาตลอด จึงไม่มีแถวใดต้อง snapshot; ผลคำนวณเดิม/revision ที่ calculated แล้วคง `CalculationPolicyVersion` string และ `CalculationSnapshotJson` เดิม. Draft เดิมที่คำนวณซ้ำหลัง v2 มีผลจะใช้ v2 (พฤติกรรม policy เดิม — `EstimateStore.cs:608`) แต่ผลเท่าเดิมเพราะไม่มีแถวที่มี snapshot และ overhead/rounding เท่ากัน | การ backfill ไม่มีความหมาย (ไม่มีข้อมูลหน่วยอื่นให้ตรึง) |
| D8 | **Revision/copy คง snapshot** (`CloneAsDraft` คัดลอก 5 ฟิลด์และคำนวณ total จาก snapshot เดิม). ข้อควรทราบ: revision ใหม่ต้องคำนวณภายใต้ policy `snapshot` — ถ้า v2 หมดอายุแล้ว คำนวณไม่ผ่าน (`ESTIMATE_POLICY_UNAVAILABLE`) | ตรงกับ "revised estimate keeps its snapshots" |
| D9 | **Error code ใหม่ 2 ตัว (422):** `ESTIMATE_UNIT_CONVERSION_NOT_FOUND`, `ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH`. หน่วยที่ไม่มี/ไม่ active หรือ policy ยังไม่เปิด = `ESTIMATE_UNIT_INVALID` เดิม; conversion เปลี่ยน/stale = `ITEM_COST_VERSION_CONFLICT` เดิม | ลดจำนวน code ใหม่ และ UI เดิมจัดการ 409 อยู่แล้ว |
| D10 | **`unitCost` ที่ client ส่งสำหรับแถวที่แปลงหน่วย ถูกละเว้น** (server คำนวณจาก snapshot) แทนที่จะบังคับให้เท่ากัน; แถวหน่วยฐานยังบังคับเท่ากับต้นทุนที่ resolve เหมือนเดิม | กัน 409 หลอกจากความต่างของการปัดทศนิยมฝั่ง JS กับ `decimal`; ไม่ลดความปลอดภัย (client ไม่ใช่ authority อยู่แล้ว) |
| D11 | **ผู้อนุมัติเห็น conversion** (`reviewQueue.costEvidence[].conversion`); Quotation/PDF ไม่เปลี่ยน (D3) | หลักฐานที่ผู้อนุมัติใช้ต้องเห็นว่าหน่วยถูกแปลงอย่างไร |
| D12 | **ตัวเลือกหน่วยผ่าน endpoint แยก** `GET /api/v1/estimate-catalog/items/{itemId}/units?branchId=` (สิทธิ์ `items.read` ระดับสาขา เหมือน catalog search) คืน `conversionsEnabled` + หน่วยฐาน + หน่วยแปลงที่ effective; ถ้า policy ยังไม่เปิดคืนเฉพาะหน่วยฐาน → UI ซ่อน dropdown | ไม่เพิ่มภาระให้ catalog search |
| D13 | **ไม่มี test ใหม่ในรอบนี้** (คำสั่งผู้ใช้) — ตาราง "test/fixture เดิมที่เสี่ยง" ด้านล่างบอกว่าแต่ละตัวยังถูกต้องเพราะอะไร | ผู้ใช้รันเอง |

## test/fixture เดิมที่เสี่ยง (ตรวจแล้ว — ต้องไม่ถูกทำให้พัง)

| ไฟล์ | จุดเสี่ยง | คงความถูกต้องอย่างไร |
| --- | --- | --- |
| `backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs` (:16-88,:483-523,:587,:651-683) | สร้าง `CalculationPolicyVersion` แบบ positional 11 อาร์กิวเมนต์; `Calculate`; assert `CalculationSnapshotJson` ด้วย `Contains` | parameter `unitConversionMode` เป็น optional ท้ายสุด (default `none`); เพิ่ม property ใน JSON ไม่กระทบ `Contains`; แถวที่ไม่มี snapshot ไม่เข้าเงื่อนไขใหม่ |
| `…/EstimateCostComponentSnapshotTests.cs` | `SetCatalogCostSnapshot`/`ClearCatalogCostSnapshot` | ไม่แก้ `SetCatalogCostSnapshot`; `Clear…` ล้างชุด conversion เพิ่ม (เดิมเป็น null อยู่แล้ว) |
| `backend/tests/TanErp.IntegrationTests/Persistence/ItemSchemaConstraintTests.cs` (:282-300,:432-445) | insert component ผ่าน EF | คอลัมน์ใหม่ nullable + check ผ่านเมื่อเป็น null ทั้งชุด |
| `…/Api/EstimateEndpointsTests.cs` (:916-959,:1470-1533,:2216-2330) | policy ctor; theory `unit-mismatch` ใช้หน่วย `BOX` ที่ไม่มีในองค์กร และ policy seeded เป็น `none` | ตอน policy ไม่ใช่ `snapshot` โค้ดโยน `ESTIMATE_UNIT_INVALID` **ก่อนแตะ resolver** → 422 เท่าเดิม |
| `…/Scenarios/ItemCatalogEstimateFlowTests.cs` (:117-121,:497) | policy ctor, ชื่อ `TEST_ONLY_BRANCH_CALC-v1` | ไม่เปลี่ยนสตริง/พฤติกรรม |
| `…/Api/OpenApiContractTests.cs` | เทียบ swagger สดกับ `contracts/openapi/tan-erp.v1.json` | Task 5 regenerate snapshot ใน commit เดียวกับที่ contract เปลี่ยน (หลัง Task 4/5 ไม่มี commit ใดที่ snapshot ค้าง ยกเว้น Task 4 → แก้ใน Task 5; ผู้ใช้รัน test หลังครบทุก Task) |
| `backend/tests/TanErp.ArchitectureTests/LayerDependencyTests.cs` | ทิศทาง dependency | port อยู่ Application, implementation อยู่ Infrastructure, controller เรียกเฉพาะ port |
| `frontend/src/features/estimates/utils/estimate-workspace-mapper.test.ts`, `…/components/estimate-workspace-layout.test.tsx` (:100-102), `estimate-workspace-drawer.test.tsx` (:270), `estimate-review-queue.test.tsx` (:50), `estimate-card.test.tsx`, `estimate-work-item-card.test.tsx` (:30 mock ตาราง) | fixture พิมพ์เป็น response type; assert ด้วย `toMatchObject` | field ใหม่ใน type ทั้งหมด optional; dropdown render เฉพาะแถวที่มี `itemId` และคืน `null` ถ้า policy ไม่เปิด; ไม่มี `toEqual` ทั้งก้อน |
| `frontend/e2e/estimate-item-catalog.spec.ts`, `official-estimate.spec.ts` | DOM ของตารางต้นทุน | dropdown ซ่อนเมื่อ `conversionsEnabled=false` (ค่าเริ่มต้นของสภาพแวดล้อม test) |

## Task Map

| Task | เรื่อง | commit |
| --- | --- | --- |
| 1 | ADR 0024, สัญญา API/error, ศัพท์ (docs) | `docs(estimates): add ADR 0024 for the estimate unit conversion snapshot` |
| 2 | Domain + persistence + migration: snapshot บนแถวต้นทุน, `UnitConversionMode` ของ policy, `Calculate`, `CloneAsDraft` | `feat(estimates): add the unit conversion snapshot and a snapshot-mode calculation policy` |
| 3 | Port `IUnitConversionResolver` + implementation (มิติ/ลำดับ/ช่วงวันที่) + error codes + DI | `feat(estimates): resolve item and shared unit conversions with a dimension guard` |
| 4 | `EstimateStore` ตรึง/คงค่า snapshot + projection/DTO/response/review evidence | `feat(estimates): freeze the unit conversion on catalog cost components when saving a draft` |
| 5 | Endpoint ตัวเลือกหน่วย + OpenAPI snapshot + generated types + api client | `feat(estimates): expose item unit options and the conversion snapshot in the api contract` |
| 6 | Frontend: dropdown หน่วย, schema/mapper, หลักฐานใน review queue, i18n | `feat(estimates): select a converted unit on catalog cost components` |
| 7 | Verification + สถานะ (ไม่ติ๊ก) | `docs(estimates): record G-08 verification and update status` |

ลำดับคอมไพล์ได้ทีละ commit: Task 2 เพิ่ม domain/config ที่ยังไม่มีผู้เรียก (ctor/method ใหม่เป็น optional/additive); Task 3 เพิ่ม port+implementation+DI โดยยังไม่มีใครเรียก; Task 4 แก้ ctor ของ `EstimateStore` พร้อม DI ที่ลงทะเบียนไว้แล้วใน Task 3 (ไม่มี test ที่ `new EstimateStore` โดยตรง — ตรวจแล้ว) และเพิ่ม field optional ท้าย record; Task 5 ต้องมาหลัง Task 4 (contract เสร็จ); Task 6 ต้องมาหลัง Task 5 (generated types).

---

> **รูปแบบ patch (`text`):** ทำตามลำดับจากบนลงล่าง — `>>> หลัง: X` = แทรกบรรทัดที่ตามมา (จนถึง `>>>` ถัดไป) ต่อท้ายบรรทัดเดียวที่มี `X`; `>>> แทนที่:` … `>>> ด้วย:` … = Edit (old_string → new_string). `X` และ old_string ต้องตรงทุกตัวอักษรและพบครั้งเดียวในไฟล์ — ไม่ตรงให้หยุดและรายงาน ห้ามเดา.

## Task 1: ADR 0024, สัญญา API/error, ศัพท์ธุรกิจ (docs เท่านั้น)

**Files:** Create: `docs/adr/0024-estimate-unit-conversion-snapshot.md`; Modify: `docs/adr/README.md`, `CONTEXT.md`, `docs/03-contracts/official-estimate-api-contract.md`, `docs/03-contracts/error-contract.md`

- [ ] **Step 1: สร้าง `docs/adr/0024-estimate-unit-conversion-snapshot.md`**
```markdown
---
status: accepted
---

# ตรึง Unit Conversion Snapshot บน Cost Component ของ Estimate

Estimate รับต้นทุนจาก Catalog ได้เฉพาะหน่วยฐานของ Item ทั้งที่ Item Master มี Item/Shared Unit Conversion แล้ว (FR-ITEM-005). ADR นี้กำหนดวิธีใช้ conversion ใน Estimate โดย Estimate ที่อนุมัติแล้วต้องไม่เปลี่ยนเมื่อ Item Master เพิ่มรุ่น conversion ใหม่.

## ตัดสินใจ

1. ใช้ตาราง `item_unit_conversions` (เฉพาะ Item, ชี้หน่วยฐาน) ก่อน แล้ว `unit_conversions` (ใช้ร่วม, ชี้หน่วยฐานโดยตรง) ไม่สร้างตารางใหม่ ไม่รองรับหลายขั้น; ความหมาย 1 FromUnit = Factor ToUnit.
2. ตรึง `{ conversionId, scope, fromUnit, toUnit, factor }` เป็น 5 คอลัมน์แบนบน `estimate_cost_components` ตอนบันทึก draft; ไม่มี FK ไปตาราง conversion. snapshot ที่ client echo กลับด้วย `conversionId` เดิมถูกคงไว้ ไม่ค้นใหม่.
3. ต้นทุนแถว = `round(Quantity × Factor × UnitCostSnapshot, 2)` ปัดครั้งเดียว; UnitCostSnapshot/UnitSnapshot ยังเป็นต้นทุนและหน่วยฐานที่ resolve จริง; ขั้นราคาเลือกด้วย Quantity × Factor.
4. หน่วยต่างมิติถูกปฏิเสธ (`ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH`) โดยตรวจมิติ ณ วันตรึงซ้ำ เพราะมิติของหน่วยแก้ได้ภายหลัง.
5. Calculation Policy Version มี `unit_conversion_mode` (`none|snapshot`); แถวเดิมเป็น `none`. Policy ที่ไม่ใช่ `snapshot` ไม่รับหน่วยอื่นนอกจากหน่วยฐาน (พฤติกรรมเดิม) และไม่คำนวณแถวที่มี snapshot. Hash ของ policy เดิมไม่เปลี่ยน.
6. ไม่ backfill Estimate เดิม: ทุกแถวเดิมใช้หน่วยฐานอยู่แล้ว.
7. Quotation/PDF ไม่เปลี่ยนเพราะอ่านระดับ Work Item; Review Queue แสดง conversion ต่อแถวต้นทุน.

## ข้อเสียที่ยอมรับ

ไม่มี API/UI สร้าง Calculation Policy จึงต้องให้ ops สร้างแถว `snapshot` เอง (follow-up); ไม่รองรับ conversion หลายขั้น; Cost tier ใช้ปริมาณหน่วยฐานที่ไม่ปัด; Item Master ยังไม่มี API ปิด/แก้ conversion.
```
- [ ] **Step 2: แก้ `docs/adr/README.md`, `CONTEXT.md`, สัญญา**
```text
FILE docs/adr/README.md
>>> หลัง: - [0023 — Server-side Quotation PDF
- [0024 — Estimate Unit Conversion Snapshot](0024-estimate-unit-conversion-snapshot.md)

FILE CONTEXT.md
>>> หลัง: _Avoid_: Estimate Formula, Assumed Factor

**Conversion Snapshot (ภาพตรึงการแปลงหน่วย)**:
ค่า conversion (รหัสที่อ้าง, ชนิด, หน่วยต้นทาง/ปลายทาง, factor) ที่ตรึงบน Cost Component ของ Estimate ตอนบันทึก draft; Estimate คำนวณจากค่านี้ ไม่ใช่จาก Unit Conversion ปัจจุบัน
_Avoid_: Live Conversion, Current Factor

FILE docs/03-contracts/official-estimate-api-contract.md
>>> แทนที่:
Estimate ปัจจุบันรับ Catalog Cost เฉพาะหน่วยฐานของ Item และยังไม่ใช้ Item/Shared Unit Conversion. หากส่ง Cost Component Unit ที่ไม่ตรงหน่วยฐาน API ตอบ 422 `ESTIMATE_UNIT_INVALID` โดยไม่เขียน Draft; conversion snapshot ใน Estimate เป็นงานต่อเนื่องและห้ามตีความว่ามีการแปลงหน่วยแล้ว.
>>> ด้วย:
Catalog Cost Component รับหน่วยฐานของ Item เสมอ และรับหน่วยอื่นที่มี Item/Shared Unit Conversion ใช้งานอยู่ **เฉพาะเมื่อ Calculation Policy ที่ effective ของสาขามี `unitConversionMode = snapshot`** ([ADR 0024](../adr/0024-estimate-unit-conversion-snapshot.md)); มิฉะนั้นหน่วยที่ไม่ตรงหน่วยฐานได้ 422 `ESTIMATE_UNIT_INVALID` โดยไม่เขียน Draft. ตอนบันทึก Draft Server ตรึง `conversionSnapshot { conversionId, conversionScope (item|shared), fromUnit, toUnit, factor (string, 1 fromUnit = factor toUnit) }` บน component และละเว้น `unitCost` ที่ Client ส่งสำหรับแถวที่แปลงหน่วย (คำนวณ `round(quantity × factor × unitCostSnapshot, 2)`; `unitCost` ที่คืนคือต่อหน่วยที่เลือก). Client ส่ง `conversionId` เดิมกลับเพื่อคง snapshot; ไม่ส่ง = ค้น conversion ที่ effective วันนี้และตรึงใหม่; `conversionId` ที่ไม่ตรงกับที่ค้นได้ → 409 `ITEM_COST_VERSION_CONFLICT`. หน่วยต่างมิติ → 422 `ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH`; ไม่มี conversion → 422 `ESTIMATE_UNIT_CONVERSION_NOT_FOUND`. `GET /api/v1/estimate-catalog/items/{itemId}/units?branchId=` คืน `conversionsEnabled`, `baseUnitCode` และหน่วยที่เลือกได้. Calculate ที่เจอแถวมี snapshot แต่ policy ไม่ใช่ `snapshot` → 409 `ESTIMATE_POLICY_UNAVAILABLE`.

FILE docs/03-contracts/error-contract.md
>>> หลัง: | `ESTIMATE_UNIT_INVALID` | 422 |
| `ESTIMATE_UNIT_CONVERSION_NOT_FOUND` | 422 | ไม่มี Item/Shared Unit Conversion ที่ active และ effective วันนี้จากหน่วยที่เลือกไปหน่วยฐานของ Item; เลือกหน่วยฐานหรือให้ผู้ดูแล Item Master เพิ่ม conversion |
| `ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH` | 422 | หน่วยที่เลือกอยู่คนละมิติกับหน่วยฐานของ Item (เช่น น้ำหนัก → ความยาว) จึงแปลงไม่ได้ |
```
- [ ] **Step 3: ตรวจลิงก์ภายใน (ต้องไม่มีบรรทัด MISSING)**
```bash
cd /Users/syaco/Documents/development/tan-erp && python3 - <<'PY'
import re, pathlib
for f in ["docs/adr/0024-estimate-unit-conversion-snapshot.md", "docs/adr/README.md", "docs/03-contracts/official-estimate-api-contract.md"]:
    base = pathlib.Path(f).parent
    for target in re.findall(r"\]\(([^)#]+\.md)(?:#[^)]*)?\)", pathlib.Path(f).read_text(encoding="utf-8")):
        if not target.startswith("http") and not (base / target).resolve().exists():
            print("MISSING", f, target)
print("LINK_CHECK_DONE")
PY
```
- [ ] **Step 4: Commit**
```bash
git add docs/adr CONTEXT.md docs/03-contracts
git commit -m "docs(estimates): add ADR 0024 for the estimate unit conversion snapshot

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 2: Domain + persistence + migration

**Files:** Create: `backend/src/TanErp.Domain/Estimates/EstimateConversionSnapshot.cs`, migration `*_AddEstimateUnitConversionSnapshot.cs` + `.Designer.cs` (generate); Modify: `backend/src/TanErp.Domain/Estimates/CalculationPolicyVersion.cs`, `EstimateCostComponent.cs`, `EstimateRevision.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/CalculationPolicyVersionConfiguration.cs`, `EstimateCostComponentConfiguration.cs`, `AppDbContextModelSnapshot.cs` (generate)

**ห้าม apply migration** — controller เป็นผู้ apply.

- [ ] **Step 1: สร้าง `backend/src/TanErp.Domain/Estimates/EstimateConversionSnapshot.cs`**
```csharp
namespace TanErp.Domain.Estimates;

public static class UnitConversionModes
{
    public const string None = "none";
    public const string Snapshot = "snapshot";

    public static bool IsValid(string? value) => value is None or Snapshot;
}

public static class UnitConversionScopes
{
    public const string Item = "item";
    public const string Shared = "shared";

    public static bool IsValid(string? value) => value is Item or Shared;
}

/// <summary>
/// The unit conversion frozen on one catalog cost component when its draft was saved: 1 FromUnit = Factor ToUnit, where ToUnit is the
/// item's base unit (the unit the cost record is published in). It has no mutator: a later Item Master change never alters a saved estimate.
/// </summary>
public sealed class EstimateConversionSnapshot
{
    public Guid ConversionId { get; }
    public string Scope { get; }
    public string FromUnit { get; }
    public string ToUnit { get; }
    public decimal Factor { get; }

    private EstimateConversionSnapshot(Guid conversionId, string scope, string fromUnit, string toUnit, decimal factor)
    {
        ConversionId = conversionId;
        Scope = scope;
        FromUnit = fromUnit;
        ToUnit = toUnit;
        Factor = factor;
    }

    public static EstimateConversionSnapshot Create(Guid conversionId, string scope, string fromUnit, string toUnit, decimal factor)
    {
        if (conversionId == Guid.Empty)
            throw new ArgumentException("Conversion ID cannot be empty.", nameof(conversionId));
        if (!UnitConversionScopes.IsValid(scope))
            throw new ArgumentException($"Invalid conversion scope: '{scope}'.", nameof(scope));
        if (string.IsNullOrWhiteSpace(fromUnit) || string.IsNullOrWhiteSpace(toUnit) || fromUnit.Trim().Length > 32 || toUnit.Trim().Length > 32)
            throw new ArgumentException("Conversion unit codes are required and limited to 32 characters.");
        if (string.Equals(fromUnit.Trim(), toUnit.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("A conversion cannot map a unit to itself.", nameof(toUnit));
        if (factor <= 0m || factor >= 1_000_000_000_000m || decimal.Round(factor, 6) != factor)
            throw new ArgumentOutOfRangeException(nameof(factor), "Conversion factor must be positive with at most six decimal places.");

        return new EstimateConversionSnapshot(conversionId, scope, fromUnit.Trim(), toUnit.Trim(), factor);
    }
}
```
- [ ] **Step 2: แก้ `CalculationPolicyVersion.cs`**
```text
FILE backend/src/TanErp.Domain/Estimates/CalculationPolicyVersion.cs
>>> หลัง: public string? ContentHash { get; private set; }
    public string UnitConversionMode { get; private set; } = UnitConversionModes.None;
>>> แทนที่:
        Guid createdByUserId)
    {
        if (id == Guid.Empty
>>> ด้วย:
        Guid createdByUserId,
        string unitConversionMode = UnitConversionModes.None)
    {
        if (id == Guid.Empty
>>> หลัง: throw new ArgumentException("Rounding mode is unsupported.", nameof(roundingMode));
        if (!UnitConversionModes.IsValid(unitConversionMode))
            throw new ArgumentException("Unit conversion mode is unsupported.", nameof(unitConversionMode));
>>> หลัง: RoundingMode = roundingMode;
        UnitConversionMode = unitConversionMode;
>>> แทนที่:
        var value = FormattableString.Invariant($"{OrganizationId:N}|{BranchId:N}|{PolicyCode}|{Version}|{OverheadMethod}|{OverheadValue}|{RoundingMode}|{EffectiveFromUtc:O}|{EffectiveToUtc:O}");
>>> ด้วย:
        // Appended only when it is not the legacy default so hashes of policies published before G-08 stay reproducible.
        var modeSuffix = UnitConversionMode == UnitConversionModes.None ? string.Empty : $"|{UnitConversionMode}";
        var value = FormattableString.Invariant($"{OrganizationId:N}|{BranchId:N}|{PolicyCode}|{Version}|{OverheadMethod}|{OverheadValue}|{RoundingMode}|{EffectiveFromUtc:O}|{EffectiveToUtc:O}{modeSuffix}");
```
- [ ] **Step 3: แก้ `EstimateCostComponent.cs`**
```text
FILE backend/src/TanErp.Domain/Estimates/EstimateCostComponent.cs
>>> หลัง: public string? ProvisionalNote { get; private set; }

    // Unit conversion snapshot (G-08). All null unless the line is costed in a unit other than the item's base unit.
    public Guid? ConversionId { get; private set; }
    public string? ConversionScope { get; private set; }
    public string? ConversionFromUnit { get; private set; }
    public string? ConversionToUnit { get; private set; }
    public decimal? ConversionFactor { get; private set; }

    public EstimateConversionSnapshot? ConversionSnapshot => ConversionId.HasValue
        ? EstimateConversionSnapshot.Create(ConversionId.Value, ConversionScope!, ConversionFromUnit!, ConversionToUnit!, ConversionFactor!.Value)
        : null;
>>> แทนที่:
        string unitCode,
        decimal unitCost,
        int sortOrder)
    {
        if (!CostComponentType.IsValid(type))
>>> ด้วย:
        string unitCode,
        decimal unitCost,
        int sortOrder)
    {
        if (ConversionId.HasValue)
            throw new EstimateInvalidStateException("A cost component with a unit conversion snapshot is replaced, not updated in place.");
        if (!CostComponentType.IsValid(type))
>>> แทนที่:
        CostRecordReasonSnapshot = null;
        ProvisionalReasonCode = null;
        ProvisionalNote = null;
    }
}
>>> ด้วย:
        CostRecordReasonSnapshot = null;
        ProvisionalReasonCode = null;
        ProvisionalNote = null;
        ConversionId = null;
        ConversionScope = null;
        ConversionFromUnit = null;
        ConversionToUnit = null;
        ConversionFactor = null;
    }

    /// <summary>
    /// Freezes the conversion. The catalog cost snapshot must be set first because the factor applies to that base-unit cost.
    /// The line's unit becomes the conversion's from-unit (canonical code).
    /// </summary>
    public void SetConversionSnapshot(EstimateConversionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (CostOrigin != "catalog" || !UnitCostSnapshot.HasValue || string.IsNullOrWhiteSpace(UnitSnapshot))
            throw new ArgumentException("A unit conversion snapshot requires a catalog cost snapshot in the item's base unit.", nameof(snapshot));
        if (!string.Equals(UnitSnapshot, snapshot.ToUnit, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("The conversion must end in the unit of the catalog cost snapshot.", nameof(snapshot));

        ConversionId = snapshot.ConversionId;
        ConversionScope = snapshot.Scope;
        ConversionFromUnit = snapshot.FromUnit;
        ConversionToUnit = snapshot.ToUnit;
        ConversionFactor = snapshot.Factor;
        UnitCode = snapshot.FromUnit;
        ApplyConversionSnapshotTotals();
    }

    /// <summary>Re-derives the cost from the frozen factor: Quantity x Factor x base-unit cost, rounded once to two decimals.</summary>
    public void ApplyConversionSnapshotTotals()
    {
        if (!ConversionFactor.HasValue || !UnitCostSnapshot.HasValue)
            return;

        UnitCost = decimal.Round(UnitCostSnapshot.Value * ConversionFactor.Value, 4, MidpointRounding.AwayFromZero);
        TotalCost = decimal.Round(Quantity * ConversionFactor.Value * UnitCostSnapshot.Value, 2, MidpointRounding.AwayFromZero);
    }
}
```
- [ ] **Step 4: แก้ `EstimateRevision.cs`** (CloneAsDraft, Calculate, snapshot JSON)
```text
FILE backend/src/TanErp.Domain/Estimates/EstimateRevision.cs
>>> แทนที่:
                            sourceCost.CostRecordReasonSnapshot);
                    }
                    cost.SetProvisionalReason(
>>> ด้วย:
                            sourceCost.CostRecordReasonSnapshot);
                        if (sourceCost.ConversionSnapshot is { } sourceConversion)
                            cost.SetConversionSnapshot(sourceConversion);
                    }
                    cost.SetProvisionalReason(
>>> แทนที่:
        foreach (var section in _sections)
        {
            section.Recalculate();
        }

        var rounding =
>>> ด้วย:
        // Lines costed through a frozen unit conversion are only valid under a policy that knows how to apply it.
        var conversionComponents = _sections.SelectMany(section => section.WorkItems)
            .SelectMany(workItem => workItem.CostComponents)
            .Where(component => component.ConversionId.HasValue)
            .ToArray();
        if (conversionComponents.Length > 0)
        {
            if (calculationPolicy.UnitConversionMode != UnitConversionModes.Snapshot)
                throw new EstimatePolicyUnavailableException("The effective calculation policy does not support unit conversion snapshots.");
            foreach (var component in conversionComponents)
                component.ApplyConversionSnapshotTotals();
        }

        foreach (var section in _sections)
        {
            section.Recalculate();
        }

        var rounding =
>>> แทนที่:
            overheadAmount,
            calculationPolicyVersionId = calculationPolicy.Id,
>>> ด้วย:
            overheadAmount,
            unitConversionMode = calculationPolicy.UnitConversionMode,
            unitConversions = conversionComponents.Select(component => new
            {
                componentId = component.Id,
                conversionId = component.ConversionId,
                conversionScope = component.ConversionScope,
                fromUnit = component.ConversionFromUnit,
                toUnit = component.ConversionToUnit,
                factor = component.ConversionFactor,
                quantity = component.Quantity,
                unitCostSnapshot = component.UnitCostSnapshot,
                totalCost = component.TotalCost
            }),
            calculationPolicyVersionId = calculationPolicy.Id,
```
(ถ้า anchor `cost.SetProvisionalReason(` ไม่ตรงเพราะต่อบรรทัด ให้ใช้ `cost.SetProvisionalReason(sourceCost.ProvisionalReasonCode, sourceCost.ProvisionalNote);` ตามไฟล์จริงและคง old_string ให้ครบบรรทัด — ตรวจด้วย `grep -n "cost.SetProvisionalReason" backend/src/TanErp.Domain/Estimates/EstimateRevision.cs` ก่อน.)
- [ ] **Step 5: แก้ EF configurations**
```text
FILE backend/src/TanErp.Infrastructure/Persistence/Configurations/CalculationPolicyVersionConfiguration.cs
>>> หลัง: table.HasCheckConstraint("ck_estimate_calculation_policy_overhead",
            table.HasCheckConstraint("ck_estimate_calculation_policy_unit_conversion_mode", "unit_conversion_mode IN ('none', 'snapshot')");
>>> หลัง: builder.Property(row => row.ContentHash).HasColumnName("content_hash").HasMaxLength(64);
        builder.Property(row => row.UnitConversionMode).HasColumnName("unit_conversion_mode").HasMaxLength(16).HasDefaultValue("none").IsRequired();

FILE backend/src/TanErp.Infrastructure/Persistence/Configurations/EstimateCostComponentConfiguration.cs
>>> หลัง: t.HasCheckConstraint("ck_estimate_cost_components_cost_origin", "cost_origin IN ('manual', 'catalog')");
            t.HasCheckConstraint("ck_estimate_cost_components_conversion_snapshot",
                "(conversion_id IS NULL AND conversion_scope IS NULL AND conversion_from_unit IS NULL AND conversion_to_unit IS NULL AND conversion_factor IS NULL) OR " +
                "(conversion_id IS NOT NULL AND conversion_scope IN ('item', 'shared') AND conversion_from_unit IS NOT NULL AND conversion_to_unit IS NOT NULL AND conversion_factor > 0)");
>>> หลัง: builder.Property(x => x.ProvisionalNote).HasColumnName("provisional_note").HasMaxLength(1000);
        builder.Property(x => x.ConversionId).HasColumnName("conversion_id");
        builder.Property(x => x.ConversionScope).HasColumnName("conversion_scope").HasMaxLength(16);
        builder.Property(x => x.ConversionFromUnit).HasColumnName("conversion_from_unit").HasMaxLength(32);
        builder.Property(x => x.ConversionToUnit).HasColumnName("conversion_to_unit").HasMaxLength(32);
        builder.Property(x => x.ConversionFactor).HasColumnName("conversion_factor").HasPrecision(18, 6);
        builder.Ignore(x => x.ConversionSnapshot);
```
- [ ] **Step 6: Generate migration (ครั้งเดียว — ถ้าต้องแก้ให้ `dotnet ef migrations remove` แล้ว generate ใหม่; ห้าม `database update`)**
```bash
cd /Users/syaco/Documents/development/tan-erp && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" \
dotnet ef migrations add AddEstimateUnitConversionSnapshot \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
grep -c "AddColumn" backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddEstimateUnitConversionSnapshot.cs
```
Expected: migration มี `AddColumn` 6 ตัว (`unit_conversion_mode` พร้อม `defaultValue: "none"` บน `estimates.calculation_policy_versions`; `conversion_id/scope/from_unit/to_unit/factor` บน `estimates.estimate_cost_components`) + `AddCheckConstraint` 2 ตัว และไม่มีการแก้/ลบอย่างอื่น. ถ้ามีอย่างอื่นให้หยุดและรายงาน.
- [ ] **Step 7: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```
Expected: `0 Warning(s)` `0 Error(s)`; `has-pending-model-changes` รายงานว่าไม่มีการเปลี่ยนโมเดลค้าง
- [ ] **Step 8: Commit**
```bash
git add backend/src/TanErp.Domain/Estimates backend/src/TanErp.Infrastructure/Persistence
git commit -m "feat(estimates): add the unit conversion snapshot and a snapshot-mode calculation policy

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 3: Port `IUnitConversionResolver`, implementation, error codes, DI

**Files:** Create: `backend/src/TanErp.Application/Items/IUnitConversionResolver.cs`, `backend/src/TanErp.Infrastructure/Persistence/Items/UnitConversionResolver.cs`; Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `backend/src/TanErp.Api/Resources/Errors.resx`, `Errors.en.resx`, `backend/src/TanErp.Api/Program.cs`

**Global Reuse Proposal (AGENTS.md §2):** `ItemStore.cs:1547-1557` (barcode) มีตรรกะ "Item-specific ก่อน Shared" แบบ inline; resolver นี้เป็นของกลางสำหรับ Estimate และ G-09 (template mapping) — **ไม่ refactor barcode** (minimal blast radius). ถ้าผู้ใช้ต้องการ ให้ migrate barcode มาใช้ port นี้เป็น follow-up.

- [ ] **Step 1: สร้าง `backend/src/TanErp.Application/Items/IUnitConversionResolver.cs`**
```csharp
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items;

/// <summary>The conversion that applies today: 1 FromUnit = Factor ToUnit, ToUnit being the item's base unit.</summary>
public sealed record ResolvedUnitConversion(
    Guid ConversionId, string Scope, Guid FromUnitId, string FromUnitCode, Guid ToUnitId, string ToUnitCode, decimal Factor);

/// <summary>A unit an estimate line of this item may use. The base unit has no conversion.</summary>
public sealed record EstimateUnitOption(
    Guid UnitId, string UnitCode, bool IsBase, Guid? ConversionId, string? Scope, decimal? Factor);

public interface IUnitConversionResolver
{
    /// <summary>
    /// Resolves the active conversion from the unit with <paramref name="fromUnitCode"/> to the item's base unit on <paramref name="onDate"/>.
    /// Item-specific conversions win over shared ones. Fails with ITEM_NOT_FOUND, ESTIMATE_UNIT_INVALID (unknown, inactive or the base unit itself),
    /// ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH or ESTIMATE_UNIT_CONVERSION_NOT_FOUND.
    /// </summary>
    Task<Result<ResolvedUnitConversion>> ResolveToBaseUnitAsync(
        Guid organizationId, Guid itemId, string fromUnitCode, DateOnly onDate, CancellationToken ct);

    /// <summary>The base unit first, then every same-dimension unit with an active conversion to it on <paramref name="onDate"/>.</summary>
    Task<Result<IReadOnlyList<EstimateUnitOption>>> ListEstimateUnitsAsync(
        Guid organizationId, Guid itemId, DateOnly onDate, CancellationToken ct);
}
```
- [ ] **Step 2: สร้าง `backend/src/TanErp.Infrastructure/Persistence/Items/UnitConversionResolver.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Results;
using TanErp.Application.Items;
using TanErp.Domain.Estimates;
using TanErp.Domain.Items;

namespace TanErp.Infrastructure.Persistence.Items;

public sealed class UnitConversionResolver : IUnitConversionResolver
{
    private readonly AppDbContext _db;

    public UnitConversionResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ResolvedUnitConversion>> ResolveToBaseUnitAsync(
        Guid organizationId, Guid itemId, string fromUnitCode, DateOnly onDate, CancellationToken ct)
    {
        var item = await _db.Items.AsNoTracking().Include(row => row.BaseUnit)
            .FirstOrDefaultAsync(row => row.Id == itemId && row.OrganizationId == organizationId, ct);
        if (item?.BaseUnit is null)
            return Result<ResolvedUnitConversion>.Failure(new Error("ITEM_NOT_FOUND", "Item not found."));

        var normalizedCode = fromUnitCode.Trim().ToUpperInvariant();
        var fromUnit = await _db.Units.AsNoTracking().FirstOrDefaultAsync(unit =>
            unit.OrganizationId == organizationId && unit.NormalizedCode == normalizedCode && unit.Status == ItemStatus.Active, ct);
        if (fromUnit is null || fromUnit.Id == item.BaseUnitId)
            return Result<ResolvedUnitConversion>.Failure(new Error("ESTIMATE_UNIT_INVALID", $"Unit '{fromUnitCode}' is not an active convertible unit for this item."));

        // The dimension is editable after a conversion was created, so it is checked again here.
        if (!string.Equals(fromUnit.Dimension, item.BaseUnit.Dimension, StringComparison.Ordinal))
            return Result<ResolvedUnitConversion>.Failure(new Error("ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH", "The unit has a different dimension than the item's base unit."));

        var itemConversion = await _db.ItemUnitConversions.AsNoTracking().SingleOrDefaultAsync(row =>
            row.OrganizationId == organizationId && row.ItemId == itemId && row.FromUnitId == fromUnit.Id && row.ToUnitId == item.BaseUnitId
            && row.Status == ItemStatus.Active && row.EffectiveFrom <= onDate && (row.EffectiveTo == null || row.EffectiveTo >= onDate), ct);
        if (itemConversion is not null)
        {
            return Result<ResolvedUnitConversion>.Success(new ResolvedUnitConversion(
                itemConversion.Id, UnitConversionScopes.Item, fromUnit.Id, fromUnit.Code, item.BaseUnit.Id, item.BaseUnit.Code, itemConversion.Factor));
        }

        var sharedConversion = await _db.UnitConversions.AsNoTracking().SingleOrDefaultAsync(row =>
            row.OrganizationId == organizationId && row.FromUnitId == fromUnit.Id && row.ToUnitId == item.BaseUnitId
            && row.Status == ItemStatus.Active && row.EffectiveFrom <= onDate && (row.EffectiveTo == null || row.EffectiveTo >= onDate), ct);
        if (sharedConversion is not null)
        {
            return Result<ResolvedUnitConversion>.Success(new ResolvedUnitConversion(
                sharedConversion.Id, UnitConversionScopes.Shared, fromUnit.Id, fromUnit.Code, item.BaseUnit.Id, item.BaseUnit.Code, sharedConversion.Factor));
        }

        return Result<ResolvedUnitConversion>.Failure(new Error("ESTIMATE_UNIT_CONVERSION_NOT_FOUND", "No active unit conversion to the item's base unit exists."));
    }

    public async Task<Result<IReadOnlyList<EstimateUnitOption>>> ListEstimateUnitsAsync(
        Guid organizationId, Guid itemId, DateOnly onDate, CancellationToken ct)
    {
        var item = await _db.Items.AsNoTracking().Include(row => row.BaseUnit)
            .FirstOrDefaultAsync(row => row.Id == itemId && row.OrganizationId == organizationId, ct);
        if (item?.BaseUnit is null)
            return Result<IReadOnlyList<EstimateUnitOption>>.Failure(new Error("ITEM_NOT_FOUND", "Item not found."));

        var baseUnitId = item.BaseUnitId;
        var dimension = item.BaseUnit.Dimension;
        var options = new List<EstimateUnitOption> { new(item.BaseUnit.Id, item.BaseUnit.Code, true, null, null, null) };

        var itemRows = await (
            from conversion in _db.ItemUnitConversions.AsNoTracking()
            join unit in _db.Units.AsNoTracking() on new { UnitId = conversion.FromUnitId, conversion.OrganizationId } equals new { UnitId = unit.Id, unit.OrganizationId }
            where conversion.OrganizationId == organizationId && conversion.ItemId == itemId && conversion.ToUnitId == baseUnitId
                && conversion.Status == ItemStatus.Active && conversion.EffectiveFrom <= onDate
                && (conversion.EffectiveTo == null || conversion.EffectiveTo >= onDate)
                && unit.Status == ItemStatus.Active && unit.Dimension == dimension
            orderby unit.Code
            select new { conversion.Id, UnitId = unit.Id, unit.Code, conversion.Factor }).ToListAsync(ct);
        options.AddRange(itemRows.Select(row => new EstimateUnitOption(row.UnitId, row.Code, false, row.Id, UnitConversionScopes.Item, row.Factor)));

        var sharedRows = await (
            from conversion in _db.UnitConversions.AsNoTracking()
            join unit in _db.Units.AsNoTracking() on new { UnitId = conversion.FromUnitId, conversion.OrganizationId } equals new { UnitId = unit.Id, unit.OrganizationId }
            where conversion.OrganizationId == organizationId && conversion.ToUnitId == baseUnitId
                && conversion.Status == ItemStatus.Active && conversion.EffectiveFrom <= onDate
                && (conversion.EffectiveTo == null || conversion.EffectiveTo >= onDate)
                && unit.Status == ItemStatus.Active && unit.Dimension == dimension
            orderby unit.Code
            select new { conversion.Id, UnitId = unit.Id, unit.Code, conversion.Factor }).ToListAsync(ct);
        var coveredUnitIds = itemRows.Select(row => row.UnitId).ToHashSet();
        options.AddRange(sharedRows.Where(row => !coveredUnitIds.Contains(row.UnitId))
            .Select(row => new EstimateUnitOption(row.UnitId, row.Code, false, row.Id, UnitConversionScopes.Shared, row.Factor)));

        return Result<IReadOnlyList<EstimateUnitOption>>.Success(options);
    }
}
```
- [ ] **Step 3: error codes + DI** (ต้องเพิ่ม `GetStatus` **และ** resx ทั้งสองภาษาใน Task เดียวกัน)
```text
FILE backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs
>>> หลัง: "ESTIMATE_UNIT_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "ESTIMATE_UNIT_CONVERSION_NOT_FOUND" => StatusCodes.Status422UnprocessableEntity,
        "ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH" => StatusCodes.Status422UnprocessableEntity,

FILE backend/src/TanErp.Api/Resources/Errors.resx
>>> หลัง: <data name="ESTIMATE_UNIT_INVALID_DETAIL"
  <data name="ESTIMATE_UNIT_CONVERSION_NOT_FOUND_TITLE" xml:space="preserve"><value>ไม่พบอัตราแปลงหน่วย</value></data>
  <data name="ESTIMATE_UNIT_CONVERSION_NOT_FOUND_DETAIL" xml:space="preserve"><value>ไม่มีอัตราแปลงหน่วยที่ใช้งานได้ ณ วันนี้จากหน่วยที่เลือกไปยังหน่วยฐานของสินค้า ให้ผู้ดูแลข้อมูลสินค้าเพิ่มอัตราแปลงหน่วยก่อน หรือเลือกหน่วยฐาน</value></data>
  <data name="ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH_TITLE" xml:space="preserve"><value>หน่วยต่างมิติแปลงกันไม่ได้</value></data>
  <data name="ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH_DETAIL" xml:space="preserve"><value>หน่วยที่เลือกอยู่คนละมิติกับหน่วยฐานของสินค้า เช่น น้ำหนักกับความยาว เลือกหน่วยในมิติเดียวกับหน่วยฐาน</value></data>

FILE backend/src/TanErp.Api/Resources/Errors.en.resx
>>> หลัง: <data name="ESTIMATE_UNIT_INVALID_DETAIL"
  <data name="ESTIMATE_UNIT_CONVERSION_NOT_FOUND_TITLE" xml:space="preserve"><value>Unit conversion not found</value></data>
  <data name="ESTIMATE_UNIT_CONVERSION_NOT_FOUND_DETAIL" xml:space="preserve"><value>No active conversion exists today from the selected unit to the item's base unit. Ask the item data owner to add one, or choose the base unit.</value></data>
  <data name="ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH_TITLE" xml:space="preserve"><value>Units of different dimensions cannot be converted</value></data>
  <data name="ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH_DETAIL" xml:space="preserve"><value>The selected unit has a different dimension than the item's base unit, for example weight versus length. Choose a unit of the same dimension as the base unit.</value></data>

FILE backend/src/TanErp.Api/Program.cs
>>> หลัง: builder.Services.AddScoped<TanErp.Application.Items.ICostResolver, TanErp.Infrastructure.Persistence.Items.CostResolver>();
builder.Services.AddScoped<TanErp.Application.Items.IUnitConversionResolver, TanErp.Infrastructure.Persistence.Items.UnitConversionResolver>();
```
(บรรทัด `ESTIMATE_UNIT_INVALID_DETAIL` ใน resx ทั้งสองไฟล์เป็นบรรทัดเดียวที่ลงท้าย `</data>` — แทรกบรรทัดใหม่ต่อท้ายบรรทัดนั้น)
- [ ] **Step 4: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)`
- [ ] **Step 5: Commit**
```bash
git add backend/src
git commit -m "feat(estimates): resolve item and shared unit conversions with a dimension guard

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 4: `EstimateStore` ตรึง/คงค่า snapshot และ contract ของ estimate

**Files:** Modify: `backend/src/TanErp.Application/Estimates/IEstimateStore.cs`, `EstimateProjections.cs`, `IEstimateReviewQueueReader.cs`, `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`, `EstimateReviewQueueReader.cs`, `backend/src/TanErp.Api/Contracts/Estimates/UpdateEstimateDraftRequest.cs`, `EstimateDetailResponse.cs`, `EstimateReviewQueueContracts.cs`, `backend/src/TanErp.Api/Controllers/EstimatesController.cs`; Create: `backend/src/TanErp.Api/Contracts/Estimates/EstimateUnitConversionContracts.cs`

- [ ] **Step 1: Application: DTO, projection, review evidence** (field ใหม่ทั้งหมดเป็น optional ท้ายสุด)
```text
FILE backend/src/TanErp.Application/Estimates/IEstimateStore.cs
>>> แทนที่:
    string? ProvisionalReasonCode = null,
    string? ProvisionalNote = null);

public sealed record EstimateWorkItemDraftDto(
>>> ด้วย:
    string? ProvisionalReasonCode = null,
    string? ProvisionalNote = null,
    Guid? ConversionId = null);

public sealed record EstimateWorkItemDraftDto(

FILE backend/src/TanErp.Application/Estimates/EstimateProjections.cs
>>> แทนที่:
    string? ProvisionalNote = null,
    bool IsProvisional = false);
>>> ด้วย:
    string? ProvisionalNote = null,
    bool IsProvisional = false,
    EstimateConversionSnapshotProjection? ConversionSnapshot = null);

public sealed record EstimateConversionSnapshotProjection(Guid ConversionId, string Scope, string FromUnit, string ToUnit, decimal Factor)
{
    public static EstimateConversionSnapshotProjection? From(EstimateConversionSnapshot? snapshot) => snapshot is null
        ? null
        : new EstimateConversionSnapshotProjection(snapshot.ConversionId, snapshot.Scope, snapshot.FromUnit, snapshot.ToUnit, snapshot.Factor);
}

FILE backend/src/TanErp.Application/Estimates/IEstimateReviewQueueReader.cs
>>> แทนที่:
    string? ProvisionalReasonCode, string? ProvisionalNote, bool IsProvisional);
>>> ด้วย:
    string? ProvisionalReasonCode, string? ProvisionalNote, bool IsProvisional,
    EstimateConversionSnapshotProjection? Conversion = null);
```
- [ ] **Step 2: Api contracts** (สร้างไฟล์ใหม่ก่อน แล้วแก้ไฟล์เดิม)

`backend/src/TanErp.Api/Contracts/Estimates/EstimateUnitConversionContracts.cs`
```csharp
using System.Globalization;
using TanErp.Application.Estimates;

namespace TanErp.Api.Contracts.Estimates;

/// <summary>The unit conversion frozen on a cost component. <c>Factor</c> is a string (six decimals), like the Item Master conversion contracts.</summary>
public sealed record EstimateConversionSnapshotResponse(Guid ConversionId, string ConversionScope, string FromUnit, string ToUnit, string Factor)
{
    public static EstimateConversionSnapshotResponse? From(EstimateConversionSnapshotProjection? projection) => projection is null
        ? null
        : new EstimateConversionSnapshotResponse(projection.ConversionId, projection.Scope, projection.FromUnit, projection.ToUnit, FormatFactor(projection.Factor));

    internal static string FormatFactor(decimal factor) => factor.ToString("0.######", CultureInfo.InvariantCulture);
}
```
```text
FILE backend/src/TanErp.Api/Contracts/Estimates/UpdateEstimateDraftRequest.cs
>>> แทนที่:
    [MaxLength(1000)] string? ProvisionalNote = null);

public sealed record UpdateEstimateWorkItemDto(
>>> ด้วย:
    [MaxLength(1000)] string? ProvisionalNote = null,
    Guid? ConversionId = null);

public sealed record UpdateEstimateWorkItemDto(

FILE backend/src/TanErp.Api/Contracts/Estimates/EstimateDetailResponse.cs
>>> แทนที่:
    bool IsProvisional = false);
>>> ด้วย:
    bool IsProvisional = false,
    EstimateConversionSnapshotResponse? ConversionSnapshot = null);
>>> แทนที่:
                    c.IsProvisional)).ToList(),
>>> ด้วย:
                    c.IsProvisional,
                    EstimateConversionSnapshotResponse.From(c.ConversionSnapshot))).ToList(),

FILE backend/src/TanErp.Api/Contracts/Estimates/EstimateReviewQueueContracts.cs
>>> แทนที่:
    string? ProvisionalReasonCode, string? ProvisionalNote, bool IsProvisional);
>>> ด้วย:
    string? ProvisionalReasonCode, string? ProvisionalNote, bool IsProvisional,
    EstimateConversionSnapshotResponse? Conversion = null);
>>> แทนที่:
                evidence.ProvisionalReasonCode, evidence.ProvisionalNote, evidence.IsProvisional)).ToList(),
>>> ด้วย:
                evidence.ProvisionalReasonCode, evidence.ProvisionalNote, evidence.IsProvisional,
                EstimateConversionSnapshotResponse.From(evidence.Conversion))).ToList(),

FILE backend/src/TanErp.Api/Controllers/EstimatesController.cs
>>> แทนที่:
                    c.ProvisionalReasonCode,
                    c.ProvisionalNote)).ToList(),
>>> ด้วย:
                    c.ProvisionalReasonCode,
                    c.ProvisionalNote,
                    c.ConversionId)).ToList(),
```
- [ ] **Step 3: `EstimateReviewQueueReader.cs`** — ส่ง conversion ให้ผู้อนุมัติ
```text
FILE backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateReviewQueueReader.cs
>>> แทนที่:
                    component.ProvisionalNote, component.IsProvisional)))
>>> ด้วย:
                    component.ProvisionalNote, component.IsProvisional,
                    EstimateConversionSnapshotProjection.From(component.ConversionSnapshot))))
```
- [ ] **Step 4: `EstimateStore.cs`** — DI, จุดตรึง, การคงค่า, projection, input hash
```text
FILE backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs
>>> หลัง: private readonly IQuotationDocumentWriter _documents;
    private readonly IUnitConversionResolver _unitConversions;
>>> แทนที่:
        IQuotationDocumentWriter documents)
    {
>>> ด้วย:
        IQuotationDocumentWriter documents,
        IUnitConversionResolver unitConversions)
    {
        _unitConversions = unitConversions;
>>> แทนที่:
            // Remove existing relational structure
>>> ด้วย:
            // Frozen conversions of the components about to be replaced. A posted ConversionId equal to a stored one keeps that snapshot
            // unchanged, so a newer Item Master conversion version never silently re-prices a saved line.
            var storedConversions = revision.Sections
                .SelectMany(section => section.WorkItems)
                .SelectMany(workItem => workItem.CostComponents)
                .Where(component => component.ItemId.HasValue && component.ConversionSnapshot is not null)
                .ToDictionary(component => component.Id, component => (ItemId: component.ItemId!.Value, Snapshot: component.ConversionSnapshot!));
            bool? unitConversionsEnabled = null;

            // Remove existing relational structure
>>> แทนที่:
                            if (item.BaseUnit is null ||
                                !string.Equals(cDto.UnitCode.Trim(), item.BaseUnit.NormalizedCode, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new ItemCostConflictException("ESTIMATE_UNIT_INVALID",
                                    $"Cost component unit '{cDto.UnitCode}' must match the catalog item's base unit '{item.BaseUnit?.Code ?? string.Empty}'.");
                            }
>>> ด้วย:
                            if (item.BaseUnit is null)
                            {
                                throw new ItemCostConflictException("ESTIMATE_UNIT_INVALID", $"Item '{item.Code}' has no base unit.");
                            }

                            EstimateConversionSnapshot? conversion = null;
                            if (!string.Equals(cDto.UnitCode.Trim(), item.BaseUnit.NormalizedCode, StringComparison.OrdinalIgnoreCase))
                            {
                                unitConversionsEnabled ??= await IsUnitConversionEnabledAsync(_db, organizationId, estimate.BranchId, _clock.UtcNow, cancellationToken);
                                var storedConversion = cDto.Id.HasValue && storedConversions.TryGetValue(cDto.Id.Value, out var previous) && previous.ItemId == item.Id
                                    ? previous.Snapshot
                                    : null;
                                conversion = await ResolveConversionSnapshotAsync(
                                    organizationId, item.Id, item.BaseUnit.Code, cDto.UnitCode, cDto.ConversionId, storedConversion,
                                    unitConversionsEnabled.Value, cancellationToken);
                            }

                            // Cost tiers are published per base unit, so the tier is chosen with the quantity expressed in the base unit.
                            var costQuantity = conversion is null ? cDto.Quantity : cDto.Quantity * conversion.Factor;
>>> แทนที่:
                                cDto.Currency ?? EstimateDefaults.DefaultCurrency,
                                cDto.Quantity,
                                _clock.UtcNow), cancellationToken);
>>> ด้วย:
                                cDto.Currency ?? EstimateDefaults.DefaultCurrency,
                                costQuantity,
                                _clock.UtcNow), cancellationToken);
>>> แทนที่:
                            if (cDto.UnitCost != resolved.Amount)
>>> ด้วย:
                            // A converted line is priced by the server from the frozen factor; the posted unit cost is not an authority there.
                            if (conversion is null && cDto.UnitCost != resolved.Amount)
>>> แทนที่:
                                resolved.EvidenceFileId,
                                resolved.Reason);
>>> ด้วย:
                                resolved.EvidenceFileId,
                                resolved.Reason);

                            if (conversion is not null)
                            {
                                comp.SetConversionSnapshot(conversion);
                            }
>>> แทนที่:
                                c.IsProvisional))
>>> ด้วย:
                                c.IsProvisional,
                                EstimateConversionSnapshotProjection.From(c.ConversionSnapshot)))
>>> แทนที่:
                        component.ProvisionalReasonCode,
                        component.ProvisionalNote
                    })
>>> ด้วย:
                        component.ProvisionalReasonCode,
                        component.ProvisionalNote,
                        component.ConversionId,
                        component.ConversionFactor
                    })
>>> แทนที่:
    private static async Task<TPolicy?> ResolvePolicyAsync<TPolicy>(
>>> ด้วย:
    /// <summary>True when the policy effective for the branch applies frozen unit conversions (unit_conversion_mode = snapshot).</summary>
    internal static async Task<bool> IsUnitConversionEnabledAsync(
        AppDbContext db, Guid organizationId, Guid branchId, DateTimeOffset atUtc, CancellationToken cancellationToken)
    {
        var policy = await ResolvePolicyAsync(db.CalculationPolicyVersions, organizationId, branchId, atUtc, cancellationToken);
        return policy is not null && policy.UnitConversionMode == UnitConversionModes.Snapshot;
    }

    /// <summary>
    /// Decides the conversion snapshot of a converted line: keeps the stored one when the client echoes its id (unit unchanged), otherwise
    /// freezes the conversion effective today. A policy without unit conversions keeps the legacy base-unit-only rule.
    /// </summary>
    private async Task<EstimateConversionSnapshot> ResolveConversionSnapshotAsync(
        Guid organizationId, Guid itemId, string baseUnitCode, string unitCode, Guid? postedConversionId,
        EstimateConversionSnapshot? stored, bool conversionsEnabled, CancellationToken cancellationToken)
    {
        if (!conversionsEnabled)
        {
            throw new ItemCostConflictException("ESTIMATE_UNIT_INVALID",
                $"Cost component unit '{unitCode}' must match the catalog item's base unit '{baseUnitCode}'.");
        }

        if (stored is not null && postedConversionId == stored.ConversionId &&
            string.Equals(stored.FromUnit, unitCode.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return stored;
        }

        var resolved = await _unitConversions.ResolveToBaseUnitAsync(
            organizationId, itemId, unitCode, DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime), cancellationToken);
        if (resolved.IsFailure)
            throw new ItemCostConflictException(resolved.Error.Code, resolved.Error.Message);

        var conversion = resolved.Value!;
        if (postedConversionId.HasValue && postedConversionId.Value != conversion.ConversionId)
        {
            throw new ItemCostConflictException("ITEM_COST_VERSION_CONFLICT",
                $"The unit conversion for '{unitCode}' changed. Re-select the unit.");
        }

        return EstimateConversionSnapshot.Create(
            conversion.ConversionId, conversion.Scope, conversion.FromUnitCode, conversion.ToUnitCode, conversion.Factor);
    }

    private static async Task<TPolicy?> ResolvePolicyAsync<TPolicy>(
```
(ก่อน Edit ที่ใช้ anchor `c.IsProvisional))` ให้ตรวจ `grep -n "c.IsProvisional))" EstimateStore.cs` ว่าพบครั้งเดียว — ตรวจแล้วที่ `:2115`.)
- [ ] **Step 5: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)`. (Swagger snapshot จะยังไม่ตรงจนถึง Task 5 — เป็นที่คาดไว้ และไม่ถูกรันใน Task นี้.)
- [ ] **Step 6: Commit**
```bash
git add backend/src
git commit -m "feat(estimates): freeze the unit conversion on catalog cost components when saving a draft

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 5: Endpoint ตัวเลือกหน่วย, OpenAPI snapshot, generated types, API client

**Files:** Create: `backend/src/TanErp.Application/Estimates/IEstimateUnitOptionsReader.cs`, `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateUnitOptionsReader.cs`; Modify: `backend/src/TanErp.Api/Contracts/Estimates/EstimateUnitConversionContracts.cs`, `backend/src/TanErp.Api/Controllers/EstimateCatalogController.cs`, `backend/src/TanErp.Api/Program.cs`, `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts`, `frontend/src/lib/api/api-client.ts`

- [ ] **Step 1: สร้าง port `backend/src/TanErp.Application/Estimates/IEstimateUnitOptionsReader.cs`**
```csharp
using TanErp.Application.Common.Results;
using TanErp.Application.Items;

namespace TanErp.Application.Estimates;

/// <param name="ConversionsEnabled">False when the branch's effective calculation policy is not in snapshot mode; then only the base unit is offered.</param>
public sealed record EstimateUnitOptionsProjection(bool ConversionsEnabled, string BaseUnitCode, IReadOnlyList<EstimateUnitOption> Units);

public interface IEstimateUnitOptionsReader
{
    Task<Result<EstimateUnitOptionsProjection>> ListAsync(Guid organizationId, Guid branchId, Guid itemId, CancellationToken ct);
}
```
- [ ] **Step 2: สร้าง `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateUnitOptionsReader.cs`**
```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates;
using TanErp.Application.Items;

namespace TanErp.Infrastructure.Persistence.Estimates;

public sealed class EstimateUnitOptionsReader : IEstimateUnitOptionsReader
{
    private readonly AppDbContext _db;
    private readonly IUnitConversionResolver _conversions;
    private readonly IClock _clock;

    public EstimateUnitOptionsReader(AppDbContext db, IUnitConversionResolver conversions, IClock clock)
    {
        _db = db;
        _conversions = conversions;
        _clock = clock;
    }

    public async Task<Result<EstimateUnitOptionsProjection>> ListAsync(Guid organizationId, Guid branchId, Guid itemId, CancellationToken ct)
    {
        var units = await _conversions.ListEstimateUnitsAsync(organizationId, itemId, DateOnly.FromDateTime(_clock.UtcNow.UtcDateTime), ct);
        if (units.IsFailure)
            return Result<EstimateUnitOptionsProjection>.Failure(units.Error);

        var enabled = await EstimateStore.IsUnitConversionEnabledAsync(_db, organizationId, branchId, _clock.UtcNow, ct);
        var all = units.Value!;
        var baseUnit = all.First(unit => unit.IsBase);
        var offered = enabled ? all : all.Where(unit => unit.IsBase).ToList();
        return Result<EstimateUnitOptionsProjection>.Success(new EstimateUnitOptionsProjection(enabled, baseUnit.UnitCode, offered));
    }
}
```
- [ ] **Step 3: contract + controller + DI**
```text
FILE backend/src/TanErp.Api/Contracts/Estimates/EstimateUnitConversionContracts.cs
>>> แทนที่:
    internal static string FormatFactor(decimal factor) => factor.ToString("0.######", CultureInfo.InvariantCulture);
}
>>> ด้วย:
    internal static string FormatFactor(decimal factor) => factor.ToString("0.######", CultureInfo.InvariantCulture);
}

public sealed record EstimateUnitOptionResponse(Guid UnitId, string UnitCode, bool IsBase, Guid? ConversionId, string? ConversionScope, string? Factor);

public sealed record EstimateUnitOptionsResponse(bool ConversionsEnabled, string BaseUnitCode, IReadOnlyList<EstimateUnitOptionResponse> Units)
{
    public static EstimateUnitOptionsResponse From(EstimateUnitOptionsProjection projection) => new(
        projection.ConversionsEnabled,
        projection.BaseUnitCode,
        projection.Units.Select(unit => new EstimateUnitOptionResponse(
            unit.UnitId, unit.UnitCode, unit.IsBase, unit.ConversionId, unit.Scope,
            unit.Factor is null ? null : EstimateConversionSnapshotResponse.FormatFactor(unit.Factor.Value))).ToList());
}

FILE backend/src/TanErp.Api/Controllers/EstimateCatalogController.cs
>>> แทนที่:
using TanErp.Api.Contracts.Items;
>>> ด้วย:
using TanErp.Api.Contracts.Estimates;
using TanErp.Api.Contracts.Items;
>>> แทนที่:
using TanErp.Application.Items.Catalog;
>>> ด้วย:
using TanErp.Application.Estimates;
using TanErp.Application.Items.Catalog;
>>> แทนที่:
    private readonly IEstimateCatalogReader _catalogReader;

    public EstimateCatalogController(
        IRequestAccessResolver accessResolver,
        IEstimateCatalogReader catalogReader)
    {
        _accessResolver = accessResolver;
        _catalogReader = catalogReader;
    }
>>> ด้วย:
    private readonly IEstimateCatalogReader _catalogReader;
    private readonly IEstimateUnitOptionsReader _unitOptionsReader;

    public EstimateCatalogController(
        IRequestAccessResolver accessResolver,
        IEstimateCatalogReader catalogReader,
        IEstimateUnitOptionsReader unitOptionsReader)
    {
        _accessResolver = accessResolver;
        _catalogReader = catalogReader;
        _unitOptionsReader = unitOptionsReader;
    }
>>> แทนที่:
        var response = ItemResponseMapper.ToResponse(result.Value!);
        return Ok(response);
    }
}
>>> ด้วย:
        var response = ItemResponseMapper.ToResponse(result.Value!);
        return Ok(response);
    }

    [HttpGet("{itemId:guid}/units")]
    [ProducesResponseType<EstimateUnitOptionsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Units(Guid itemId, [FromQuery, Required] Guid branchId, CancellationToken ct = default)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveBranchAccessAsync(auth.FirebaseUid, auth.MembershipId, "items.read", branchId, ct);
        if (accessResult.IsFailure) return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);

        var result = await _unitOptionsReader.ListAsync(accessResult.Value!.OrganizationId, branchId, itemId, ct);
        return result.IsFailure
            ? ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext)
            : Ok(EstimateUnitOptionsResponse.From(result.Value!));
    }
}

FILE backend/src/TanErp.Api/Program.cs
>>> หลัง: builder.Services.AddScoped<TanErp.Application.Items.IUnitConversionResolver, TanErp.Infrastructure.Persistence.Items.UnitConversionResolver>();
builder.Services.AddScoped<TanErp.Application.Estimates.IEstimateUnitOptionsReader, TanErp.Infrastructure.Persistence.Estimates.EstimateUnitOptionsReader>();
```
(`using TanErp.Application.Estimates;` อาจทำให้ชื่อชนกับ `TanErp.Application.Items` ใน controller — ถ้า build รายงาน ambiguity ให้ระบุชื่อเต็มของ `IEstimateUnitOptionsReader` แทน using.)
- [ ] **Step 4: api client (ชนิดและเมธอด) — แก้ `frontend/src/lib/api/api-client.ts`**
```text
FILE frontend/src/lib/api/api-client.ts
>>> หลัง: export type EstimateRevisionResponse = components["schemas"]["EstimateRevisionResponse"];
export type EstimateUnitOptionsResponse = components["schemas"]["EstimateUnitOptionsResponse"];
>>> แทนที่:
  async getEstimateCatalog(
>>> ด้วย:
  async getEstimateUnitOptions(itemId: string, branchId: string, options: RequestOptions): Promise<EstimateUnitOptionsResponse> {
    return this.request<EstimateUnitOptionsResponse>(
      `/api/v1/estimate-catalog/items/${encodeURIComponent(itemId)}/units?branchId=${encodeURIComponent(branchId)}`,
      "GET",
      options
    );
  }

  async getEstimateCatalog(
```
- [ ] **Step 5: OpenAPI snapshot ด้วย live swagger + formatter (ไม่ใช้ `UPDATE_OPENAPI=1`)** — formatter นี้พิสูจน์แล้วว่า round-trip snapshot ที่ commit ได้ byte-ต่อ-byte (G-04 Task 8); ต้องผ่าน `FORMATTER_OK` ก่อนใช้ ถ้าไม่ผ่านให้หยุดและรายงาน
```bash
cd /Users/syaco/Documents/development/tan-erp && S=${TMPDIR:-/tmp}/openapi-g08 && mkdir -p "$S" && cat > "$S/fmt.py" <<'PY'
import json, sys
SHORT = {"\\": "\\\\", "\n": "\\n", "\r": "\\r", "\t": "\\t", "\b": "\\b", "\f": "\\f"}
def esc(s):
    o = []
    for ch in s:
        c = ord(ch)
        if ch in SHORT: o.append(SHORT[ch])
        elif c < 0x20 or c >= 0x7F or ch in "\"&'+<>`":
            for u in ([c] if c <= 0xFFFF else [0xD800 + ((c - 0x10000) >> 10), 0xDC00 + ((c - 0x10000) & 0x3FF)]): o.append("\\u%04X" % u)
        else: o.append(ch)
    return '"' + "".join(o) + '"'
def w(v, d):
    p, q = "  " * (d + 1), "  " * d
    if isinstance(v, dict): return "{}" if not v else "{\n" + ",\n".join(p + esc(k) + ": " + w(x, d + 1) for k, x in v.items()) + "\n" + q + "}"
    if isinstance(v, list): return "[]" if not v else "[\n" + ",\n".join(p + w(x, d + 1) for x in v) + "\n" + q + "]"
    if isinstance(v, bool): return "true" if v else "false"
    if v is None: return "null"
    if isinstance(v, str): return esc(v)
    return repr(v).replace("e", "E") if isinstance(v, float) else json.dumps(v)
open(sys.argv[2], "w", encoding="utf-8", newline="").write(w(json.load(open(sys.argv[1], encoding="utf-8")), 0))
PY
python3 "$S/fmt.py" contracts/openapi/tan-erp.v1.json "$S/roundtrip.json" && cmp contracts/openapi/tan-erp.v1.json "$S/roundtrip.json" && echo FORMATTER_OK
```
```bash
cd /Users/syaco/Documents/development/tan-erp && S=${TMPDIR:-/tmp}/openapi-g08
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
Expected: `ONLY_COMMA_CHANGES` (ลบได้เฉพาะบรรทัดที่ต่างจากบรรทัดที่เพิ่มแค่เครื่องหมาย `,` ท้ายบรรทัด); เพิ่ม path `/api/v1/estimate-catalog/items/{itemId}/units`, schema `EstimateConversionSnapshotResponse`, `EstimateUnitOptionResponse`, `EstimateUnitOptionsResponse` และ field `conversionSnapshot` (response), `conversionId` (request), `conversion` (review evidence). ถ้ามี `UNEXPECTED_DELETIONS` ให้ตรวจว่าเป็น drift อื่นก่อนไปต่อ.
- [ ] **Step 6: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts   # check:api เทียบกับ index
cd frontend && npx tsc --noEmit && npm run lint && npm run check:api
```
Expected: `0 Warning(s)` `0 Error(s)`; tsc/lint/`check:api` exit 0
- [ ] **Step 7: Commit**
```bash
git add backend/src contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api
git commit -m "feat(estimates): expose item unit options and the conversion snapshot in the api contract

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 6: Frontend: dropdown หน่วย, schema/mapper, หลักฐานใน review queue, i18n

**Files:** Create: `frontend/src/features/estimates/hooks/use-estimate-unit-options.ts`, `frontend/src/features/estimates/components/estimate-cost-unit-select.tsx`; Modify: `frontend/src/features/estimates/schemas/estimate-workspace-schema.ts`, `utils/estimate-workspace-mapper.ts`, `components/estimate-cost-component-table.tsx`, `components/estimate-workspace-drawer.tsx`, `components/estimate-review-queue.tsx`, `frontend/src/messages/th.json`, `en.json`

**อ่าน `.agents/skills/building-erp-forms/SKILL.md` ก่อน** (ไม่มีฟอร์มใหม่; ใช้ `Select` กลาง, ข้อความทั้งหมดผ่าน i18n, error ตาม code ที่รู้จัก, ไม่ใช้ `any`). **ห้ามแก้ `th.json`/`en.json` ด้วย `json.dumps`** — ใช้ Insert ตาม anchor เท่านั้น. ไม่มี fallback ลูกโซ่: แถวที่ไม่มี `baseUnitCost` จะ disable dropdown.

- [ ] **Step 1: schema และ mapper**
```text
FILE frontend/src/features/estimates/schemas/estimate-workspace-schema.ts
>>> แทนที่:
  provisionalNote: z.string().nullable().optional(),
  isProvisional: z.boolean().optional(),
>>> ด้วย:
  provisionalNote: z.string().nullable().optional(),
  isProvisional: z.boolean().optional(),
  /** Base-unit cost from the catalog/saved snapshot; the unit picker derives unitCost = baseUnitCost x factor from it. */
  baseUnitCost: z.number().nullable().optional(),
  conversion: z.object({
    conversionId: z.string(),
    scope: z.string(),
    fromUnit: z.string(),
    toUnit: z.string(),
    factor: z.string(),
  }).nullable().optional(),

FILE frontend/src/features/estimates/utils/estimate-workspace-mapper.ts
>>> แทนที่:
          isProvisional: cost.isProvisional,
        })),
>>> ด้วย:
          isProvisional: cost.isProvisional,
          baseUnitCost: cost.unitCostSnapshot ?? null,
          conversion: cost.conversionSnapshot
            ? {
                conversionId: cost.conversionSnapshot.conversionId,
                scope: cost.conversionSnapshot.conversionScope,
                fromUnit: cost.conversionSnapshot.fromUnit,
                toUnit: cost.conversionSnapshot.toUnit,
                factor: cost.conversionSnapshot.factor,
              }
            : null,
        })),
>>> แทนที่:
          provisionalReasonCode: cost.provisionalReasonCode ?? null, provisionalNote: cost.provisionalNote ?? null,
>>> ด้วย:
          provisionalReasonCode: cost.provisionalReasonCode ?? null, provisionalNote: cost.provisionalNote ?? null,
          conversionId: cost.conversion?.conversionId ?? null,
>>> แทนที่:
      itemId: item.id, costRecordId: cost.costRecordId, costRecordVersion: cost.version,
>>> ด้วย:
      itemId: item.id, costRecordId: cost.costRecordId, costRecordVersion: cost.version,
      baseUnitCost: cost.amount, conversion: null,
```
- [ ] **Step 2: hook `frontend/src/features/estimates/hooks/use-estimate-unit-options.ts`** (query key มี `"catalog"` ที่ตำแหน่ง [4] เพื่อให้ถูก invalidate พร้อม catalog เมื่อเกิด `ITEM_COST_*` conflict ใน `reportError` เดิม)
```ts
"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, type EstimateUnitOptionsResponse } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";

export interface UseEstimateUnitOptionsParams {
  branchId?: string;
  itemId: string;
}

export function useEstimateUnitOptions({ branchId, itemId }: UseEstimateUnitOptionsParams) {
  const membershipId = useOptionalSelectedMembership()?.selectedMembership?.id;
  const locale = useSafeLocale();

  return useQuery<EstimateUnitOptionsResponse>({
    queryKey: ["business", membershipId, locale, "estimates", "catalog", "unit-options", branchId, itemId],
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token || !membershipId || !branchId) {
        throw new Error("AUTHENTICATION_REQUIRED");
      }
      return apiClient.getEstimateUnitOptions(itemId, branchId, {
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
        signal,
      });
    },
    enabled: Boolean(membershipId && branchId),
    staleTime: 60 * 1000,
  });
}
```
- [ ] **Step 3: component `frontend/src/features/estimates/components/estimate-cost-unit-select.tsx`** (ภายนอกคืน `null` เมื่อไม่มี `itemId` โดยไม่เรียก hook ใด ๆ ของ query — test ที่ render ตารางโดยไม่มี `QueryClientProvider` และไม่มี `itemId` จึงไม่กระทบ)
```tsx
"use client";

import { useFormContext, useWatch } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Select } from "@/components/ui/Select";
import { useEstimateUnitOptions } from "../hooks/use-estimate-unit-options";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";

interface EstimateCostUnitSelectProps {
  sectionIndex: number;
  itemIndex: number;
  costIndex: number;
  branchId?: string;
}

/** Unit picker for a catalog-linked cost line. Renders nothing unless the item has converted units the effective policy allows. */
export function EstimateCostUnitSelect(props: EstimateCostUnitSelectProps) {
  const { control } = useFormContext<EstimateWorkspaceFormData>();
  const itemId = useWatch({
    control,
    name: `sections.${props.sectionIndex}.workItems.${props.itemIndex}.costComponents.${props.costIndex}.itemId`,
  });
  if (!itemId) return null;
  return <CatalogCostUnitSelect {...props} itemId={itemId} />;
}

function CatalogCostUnitSelect({ sectionIndex, itemIndex, costIndex, branchId, itemId }: EstimateCostUnitSelectProps & { itemId: string }) {
  const t = useTranslations("estimates");
  const path = `sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${costIndex}` as const;
  const { control, setValue } = useFormContext<EstimateWorkspaceFormData>();
  const unitCode = useWatch({ control, name: `${path}.unitCode` });
  const baseUnitCost = useWatch({ control, name: `${path}.baseUnitCost` });
  const { data } = useEstimateUnitOptions({ branchId, itemId });

  if (!data?.conversionsEnabled || data.units.length < 2) return null;

  const unitsByCode = new Map(data.units.map((unit) => [unit.unitCode, unit]));
  const options = data.units.map((unit) => ({
    value: unit.unitCode,
    label: unit.isBase
      ? t("unitOptionBase", { unit: unit.unitCode })
      : t("unitOptionConverted", { unit: unit.unitCode, factor: unit.factor ?? "-", base: data.baseUnitCode }),
  }));

  function handleChange(code: string) {
    const option = unitsByCode.get(code);
    if (!option || baseUnitCost === null || baseUnitCost === undefined) return;
    const converted = !option.isBase && option.conversionId && option.conversionScope && option.factor;
    const factor = converted ? Number(option.factor) : 1;
    setValue(`${path}.unitCode`, option.unitCode, { shouldDirty: true });
    setValue(
      `${path}.conversion`,
      converted
        ? { conversionId: option.conversionId!, scope: option.conversionScope!, fromUnit: option.unitCode, toUnit: data!.baseUnitCode, factor: option.factor! }
        : null,
      { shouldDirty: true },
    );
    // Preview only: the server recomputes the line from the frozen factor when the draft is saved.
    setValue(`${path}.unitCost`, Math.round(baseUnitCost * factor * 10000) / 10000, { shouldDirty: true });
  }

  return (
    <Select
      id={`cost-unit-${sectionIndex}-${itemIndex}-${costIndex}`}
      label={t("unitSelectLabel")}
      options={options}
      value={unitCode}
      disabled={baseUnitCost === null || baseUnitCost === undefined}
      onChange={(event) => handleChange(event.target.value)}
      wrapperClassName="col-span-2 mb-0"
    />
  );
}
```
- [ ] **Step 4: ต่อเข้าตาราง ตัวจัดการ error และ review queue**
```text
FILE frontend/src/features/estimates/components/estimate-cost-component-table.tsx
>>> แทนที่:
import { EstimateItemCatalogModal } from "./estimate-item-catalog-modal";
>>> ด้วย:
import { EstimateItemCatalogModal } from "./estimate-item-catalog-modal";
import { EstimateCostUnitSelect } from "./estimate-cost-unit-select";
>>> แทนที่:
                  {/* Quantity Input */}
>>> ด้วย:
                  <EstimateCostUnitSelect sectionIndex={sectionIndex} itemIndex={itemIndex} costIndex={cIdx} branchId={branchId} />

                  {/* Quantity Input */}

FILE frontend/src/features/estimates/components/estimate-workspace-drawer.tsx
>>> หลัง: case "ESTIMATE_UNIT_INVALID": toast.error(t("unitInvalid")); break;
      case "ESTIMATE_UNIT_CONVERSION_NOT_FOUND": toast.error(t("unitConversionNotFound")); break;
      case "ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH": toast.error(t("unitConversionDimensionMismatch")); break;

FILE frontend/src/features/estimates/components/estimate-review-queue.tsx
>>> แทนที่:
              <div className="break-words text-xs text-muted-foreground">{evidence.itemCode ?
>>> ด้วย:
              {evidence.conversion && <div className="text-xs text-muted-foreground">{t("reviewUnitConversion", { from: evidence.conversion.fromUnit, factor: evidence.conversion.factor, to: evidence.conversion.toUnit })}</div>}
              <div className="break-words text-xs text-muted-foreground">{evidence.itemCode ?
```
- [ ] **Step 5: i18n th/en (ครบทั้งสองภาษา)**
```text
FILE frontend/src/messages/th.json
>>> หลัง: "unitInvalid": "หน่วยต้นทุนไม่ตรงกับหน่วยฐานของสินค้า
    "unitSelectLabel": "หน่วย",
    "unitOptionBase": "{unit} (หน่วยฐาน)",
    "unitOptionConverted": "{unit} (1 {unit} = {factor} {base})",
    "unitConversionNotFound": "ยังไม่มีอัตราแปลงหน่วยที่ใช้งานได้ของสินค้านี้ ให้ผู้ดูแลข้อมูลสินค้าเพิ่มอัตราแปลงหน่วยก่อน หรือเลือกหน่วยฐาน",
    "unitConversionDimensionMismatch": "หน่วยที่เลือกอยู่คนละมิติกับหน่วยฐาน (เช่น น้ำหนักกับความยาว) จึงแปลงไม่ได้",
>>> หลัง: "reviewNoCostEvidence": "รุ่นนี้ไม่มีรายการส่วนประกอบต้นทุน",
    "reviewUnitConversion": "แปลงหน่วย: 1 {from} = {factor} {to}",

FILE frontend/src/messages/en.json
>>> หลัง: "unitInvalid": "The cost unit does not match the item's base unit.
    "unitSelectLabel": "Unit",
    "unitOptionBase": "{unit} (base unit)",
    "unitOptionConverted": "{unit} (1 {unit} = {factor} {base})",
    "unitConversionNotFound": "No active unit conversion exists for this item. Ask the item data owner to add one, or choose the base unit.",
    "unitConversionDimensionMismatch": "The selected unit has a different dimension than the base unit (for example weight versus length), so it cannot be converted.",
>>> หลัง: "reviewNoCostEvidence": "No cost components in this revision",
    "reviewUnitConversion": "Unit conversion: 1 {from} = {factor} {to}",
```
- [ ] **Step 6: Compile-only gate (ห้ามรันชุดทดสอบ; `npm run build` รันครั้งเดียวที่นี่)**
```bash
cd frontend && npx tsc --noEmit && npm run lint
node -e 'const t=require("./src/messages/th.json"),e=require("./src/messages/en.json");const f=(o,p="")=>Object.entries(o).flatMap(([k,v])=>typeof v==="object"&&v!==null?f(v,p+k+"."):[p+k]);const a=f(t).sort().join("\n"),b=f(e).sort().join("\n");console.log(a===b?"KEY_PARITY_OK":"KEY_PARITY_MISMATCH")'
npm run build
```
Expected: tsc/lint exit 0; `KEY_PARITY_OK`; `npm run build` exit 0
- [ ] **Step 7: Commit**
```bash
git add frontend/src
git commit -m "feat(estimates): select a converted unit on catalog cost components

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 7: Verification และเอกสารสถานะ (ไม่ติ๊กว่าตรวจแล้ว)

**Files:** Create: `docs/05-engineering/estimate-unit-conversion-verification.md`; Modify: `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/00-overview/implementation-roadmap.md`

**กฎ:** ไม่รันชุดทดสอบใด ๆ และ **ไม่เขียน test ใด ๆ** — เอกสารต้องบอกตรง ๆ ว่า **no tests written, none run**. **ห้ามติ๊ก `[x]` ข้อใดของ G-08** — checkbox ทุกข้อคง `[ ]`. แทน `<วันที่จริง>` ด้วยผล `date +%F` (ทุกจุด).

- [ ] **Step 1: compile/lint gate สุดท้าย (เท่านั้นที่รันได้)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
cd frontend && npx tsc --noEmit && npm run lint && npm run check:api
cd .. && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
git status --short | grep -v "^??"
```
Expected: `0 Warning(s)` `0 Error(s)`; tsc/lint/`check:api` exit 0; ไม่มีการเปลี่ยนโมเดลค้าง. `npm run build` และ `KEY_PARITY_OK` ผ่านแล้วใน Task 6 — บันทึกเป็น "ผลจาก Task 6". จดผลจริงลงเอกสาร.
- [ ] **Step 2: สร้าง `docs/05-engineering/estimate-unit-conversion-verification.md`** (ภาษาไทย รูปแบบเดียวกับ `quotation-pdf-verification.md`) ประกอบด้วย:
  1. **สถานะ:** `Implemented <วันที่จริง> (G-08: Estimate Unit Conversion Snapshot) — no tests written, none run.` ระบุว่า compile/lint เท่านั้นที่รัน; ยังไม่ตรวจในเบราว์เซอร์; ยังไม่ผ่าน UAT/Production. ลิงก์ [ADR 0024](../adr/0024-estimate-unit-conversion-snapshot.md), แผน, [Official Estimate API](../03-contracts/official-estimate-api-contract.md).
  2. **ผลที่รันจริง (compile/lint เท่านั้น):** ตารางคำสั่ง → ผลจริงจาก Step 1 และจาก Task ก่อนหน้า (`dotnet build`, `has-pending-model-changes`, `FORMATTER_OK`, `ONLY_COMMA_CHANGES`, `check:api`, `tsc`, `lint`, `KEY_PARITY_OK`, `npm run build`).
  3. **คำสั่งที่ผู้ใช้ควรรันเอง** (คัดลอกจากหัวข้อ "คำสั่งสำหรับผู้ใช้" ด้านล่างตรง ๆ; ห้ามเขียนว่าผ่าน).
  4. **ข้อกำหนดก่อนเปิดใช้ (สำคัญ):** ความสามารถนี้ **ปิดอยู่ตามค่าเริ่มต้น** จนกว่าจะมี Calculation Policy ที่ effective ของสาขาเป็น `unit_conversion_mode='snapshot'` ซึ่ง **ไม่มี API/UI สร้างหรือ publish ได้** (ดู D6): ops ต้องสร้างแถว v2 (`version = v1+1`, overhead/rounding เท่า v1, `EffectiveFromUtc` = ปลายช่วงของ v1, ปิดช่วง v1 ด้วย `EffectiveTo` เดียวกัน) แล้ว publish ด้วย checker อิสระผ่านโค้ด `CalculationPolicyVersion.Publish` (ต้องคำนวณ `ContentHash`; ห้ามใส่ค่า hash ด้วยมือ). ต้องไม่มี policy effective ซ้อนกันสองแถวต่อสาขา (จะได้ `ESTIMATE_POLICY_UNAVAILABLE`). ต้องมี Item Master conversion ที่ active สำหรับ Item ที่จะลอง. Follow-up ที่เสนอ: Calculation Policy admin API (maker–checker) — รอผู้ใช้อนุมัติขอบเขต.
  5. **สิ่งที่ผู้ใช้ควรตรวจด้วยมือหลังรัน test:** (ก) Estimate เดิมที่อนุมัติ/คำนวณแล้วเปิดดูแล้วตัวเลขและ `CalculationPolicyVersion` ไม่เปลี่ยน; (ข) เพิ่ม conversion รุ่นใหม่ใน Item Master แล้วเปิด Estimate ที่บันทึกไว้ → factor ใน `conversionSnapshot` เดิม และบันทึก draft ซ้ำแล้วยังเดิม; (ค) เลือกหน่วยต่างมิติ → 422 `ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH`; ไม่มี conversion → 422 `ESTIMATE_UNIT_CONVERSION_NOT_FOUND`; policy ไม่ใช่ `snapshot` → 422 `ESTIMATE_UNIT_INVALID` เหมือนเดิม; (ฆ) `New revision` แล้ว snapshot ถูกคัดลอก; (ง) หน้า Review Queue แสดง "แปลงหน่วย: 1 … = … …"; (จ) ใบเสนอราคา/PDF ของ Estimate ที่มีแถวแปลงหน่วย แสดงเฉพาะหน่วยและราคาระดับ Work Item (ไม่เห็นต้นทุน/conversion).
  6. **สิ่งที่ยังไม่ได้ทำ/ข้อจำกัดที่ยอมรับ:** ไม่มี test ใหม่; conversion หลายขั้นไม่รองรับ; cost tier ใช้ปริมาณหน่วยฐานไม่ปัด; พรีวิวบรรทัดฝั่ง UI ใช้ `baseUnitCost × factor` (ปัด 4 ตำแหน่ง) ผลจริงมาจาก server; Item Master ยังไม่มี API ปิด/แก้ conversion; `ITEM_COST_VERSION_CONFLICT` ใช้ข้อความ UI เดิมสำหรับกรณี conversion เปลี่ยน; revision ใหม่ของ Estimate ที่มี snapshot คำนวณไม่ได้ถ้าไม่มี policy `snapshot` ที่ effective; G-09 ต้องใช้ `IUnitConversionResolver` และกติกา D5 เมื่อสร้างแถวจาก template.
- [ ] **Step 3: แก้ `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`** — ปรับถ้อยคำและเพิ่มบล็อกสถานะ (checkbox ทุกข้อคง `[ ]`)
```text
>>> แทนที่:
| G-08 | Estimate Unit Conversion Snapshot | B | FR-ITEM-005 | Cost Owner + Estimator | — |
>>> ด้วย:
| G-08 | Estimate Unit Conversion Snapshot | B | FR-ITEM-005 | Cost Owner + Estimator | Implemented <วันที่จริง> (no tests written, none run; browser not checked; ยังไม่ผ่าน UAT; **ปิดอยู่จนกว่าจะมี Calculation Policy แบบ `snapshot` ซึ่งยังไม่มี API สร้าง**) — [แผน G-08](2026-10-15-g08-estimate-unit-conversion.md); [ADR 0024](../../adr/0024-estimate-unit-conversion-snapshot.md); [Verification](../../05-engineering/estimate-unit-conversion-verification.md) |
>>> แทนที่:
- [ ] Estimate line ตรึง `conversionSnapshot { fromUnit, toUnit, factor, conversionId }` ตอนเลือกหน่วย
>>> ด้วย:
- [ ] Cost component ที่ผูก catalog item ตรึง `conversionSnapshot { fromUnit, toUnit, factor, conversionId }` (+ `conversionScope`) ตอน **บันทึก draft** ที่เลือกหน่วยที่ไม่ใช่หน่วยฐาน (ไม่ใช่ตอนคลิก; ระดับ Work Item/Quotation ไม่เปลี่ยน)
>>> แทนที่:
- [ ] Calculation version ใหม่; Estimate เก่าคำนวณด้วย version เดิม
>>> ด้วย:
- [ ] Calculation **Policy** Version ใหม่ที่ `unit_conversion_mode = snapshot` (ไม่ใช่ตัวนับ `CalculationVersion` ต่อ revision); Estimate เก่าไม่มี snapshot จึงคงผลและ policy string เดิม (ไม่ backfill)
>>> แทนที่:
- [ ] Tests: แก้ conversion ใน Item Master แล้ว Estimate ที่อนุมัติไม่เปลี่ยน, หน่วยต่างมิติถูกปฏิเสธ
>>> ด้วย:
- [ ] Tests: แก้ conversion ใน Item Master แล้ว Estimate ที่อนุมัติไม่เปลี่ยน, หน่วยต่างมิติถูกปฏิเสธ — **ยังไม่เขียน** (คำสั่งผู้ใช้ให้ไม่เขียน test ในรอบนี้)

> **สถานะ G-08 (<วันที่จริง>):** Implemented — no tests written, none run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านบนคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: ใช้ตาราง conversion เดิม (Item-specific ก่อน Shared, ตรงหน่วยฐานเท่านั้น) ผ่าน `IUnitConversionResolver` ที่ตรวจมิติซ้ำ; 5 คอลัมน์ snapshot บน `estimate_cost_components`; `calculation_policy_versions.unit_conversion_mode`; สูตร `round(qty × factor × base cost, 2)`; snapshot คงข้ามการบันทึกซ้ำและ `New revision`; ผู้อนุมัติเห็น conversion; endpoint `GET /api/v1/estimate-catalog/items/{itemId}/units`; error ใหม่ `ESTIMATE_UNIT_CONVERSION_NOT_FOUND` และ `…_DIMENSION_MISMATCH`. **ยังไม่ทำ/ยังเปิด:** admin API สำหรับสร้าง/publish Calculation Policy (ต้องมีแถว `snapshot` ก่อนเปิดใช้), conversion หลายขั้น, test ทั้งหมด. **G-09:** ต้องสร้างแถวต้นทุนจาก template ผ่าน `IUnitConversionResolver` และกติกาตรึง/คงค่า (D5) เดียวกัน. [Verification](../../05-engineering/estimate-unit-conversion-verification.md)
```
- [ ] **Step 4: แก้ `docs/00-overview/implementation-roadmap.md`** — แถว Official Estimate
```text
>>> แทนที่:
manual UAT and conversion snapshot deferred.
>>> ด้วย:
manual UAT deferred. Unit conversion snapshot on catalog cost components is implemented in G-08 (<วันที่จริง>; [ADR 0024](../adr/0024-estimate-unit-conversion-snapshot.md); no tests written or run, browser not checked, ยังไม่ผ่าน UAT) but stays off until a snapshot-mode Calculation Policy is published, which has no API yet ([Verification](../05-engineering/estimate-unit-conversion-verification.md)).
```
- [ ] **Step 5: ตรวจลิงก์ภายใน (ต้องไม่มี MISSING) และไม่เหลือ `<วันที่จริง>` (`grep -c` ทุกไฟล์ต้องลงท้าย `:0`)**
```bash
cd /Users/syaco/Documents/development/tan-erp && python3 - <<'PY'
import re, pathlib
files = ["docs/05-engineering/estimate-unit-conversion-verification.md", "docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md", "docs/00-overview/implementation-roadmap.md"]
for f in files:
    base = pathlib.Path(f).parent
    for target in re.findall(r"\]\(([^)#]+\.md)(?:#[^)]*)?\)", pathlib.Path(f).read_text(encoding="utf-8")):
        if not target.startswith("http") and not (base / target).resolve().exists():
            print("MISSING", f, target)
print("LINK_CHECK_DONE")
PY
grep -c "<วันที่จริง>" docs/05-engineering/estimate-unit-conversion-verification.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/00-overview/implementation-roadmap.md
```
- [ ] **Step 6: Commit**
```bash
git add docs
git commit -m "docs(estimates): record G-08 verification and update status

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## คำสั่งสำหรับผู้ใช้ (รันเอง — implementer ห้ามรัน)

แผนนี้ไม่เขียน test ใหม่ คำสั่งด้านล่างคือชุดเดิมที่เกี่ยวข้องและควรยังผ่าน:
```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Estimates"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests|FullyQualifiedName~ItemCatalogEstimateFlowTests|FullyQualifiedName~ItemSchemaConstraintTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
cd frontend && npx vitest run src/features/estimates
```
ก่อนใช้ความสามารถนี้จริง: apply migration `AddEstimateUnitConversionSnapshot`, สร้างและ publish Calculation Policy แบบ `snapshot` ตามหัวข้อ 4 ของเอกสาร verification, และตรวจรายการด้วยมือในหัวข้อ 5.

## Self-review (ตรวจแล้ว)

- ทุก path ที่อ้างมีอยู่จริงหรือทำเครื่องหมาย Create; ทุก cite `file:line` ตรวจจากโค้ดบน `main` ณ วันที่เขียนแผน (บรรทัดอาจขยับหลังแก้ใน Task ก่อนหน้า — ให้ยึด anchor ของ Edit ไม่ใช่เลขบรรทัด).
- ลำดับ Task คอมไพล์ได้ทีละ commit: ทุก parameter/field ใหม่ของ ctor/record เป็น optional ท้ายสุด; `EstimateStore` ctor เปลี่ยนใน Task 4 หลัง DI ใน Task 3; ไม่มี test ที่ `new EstimateStore`/`new EstimateCatalogController`; swagger snapshot ค้างระหว่าง Task 4 → Task 5 (ไม่มี test รันใน Task เหล่านี้).
- Error code ใหม่เพียงสองตัว เพิ่มพร้อม `ProblemDetailsMapper.GetStatus` + `Errors.resx` + `Errors.en.resx` ใน Task 3 ที่เดียวกัน; ไม่มี permission ใหม่; migration เดียว (Task 2) และไม่ apply.
- ไม่มีขั้นใดสั่งรัน test หรือให้เขียน test; ไม่มีขั้นที่บอกว่า "เหมือน…" โดยไม่มีโค้ด; bare `Organization` ไม่ถูกใช้ (ไม่มีโค้ดอ้างชนิดนี้).
