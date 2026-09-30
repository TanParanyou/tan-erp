# Item Master Foundation, SKU and Barcode Implementation Plan

> **For agentic workers:** Continue this implementation task by task with review after each independently testable change. The unfinished Estimate Master Data plan and its verification gates remain open.

**Code status:** Complete — Item/SKU, Barcode, shared and Item-specific unit conversions, taxonomy maintenance, API/schema, and Item field maintenance are implemented and covered by focused tests. Item Master is newly introduced, so no legacy Item rows exist to backfill; additive migration and activation without Barcode are verified. Browser coverage includes the priced Item → BOQ snapshot journey, cost-version conflict recovery, private-image access, and image error/empty state. Native tablet/200% zoom remains for user acceptance; migration rehearsal against a sanitized copy of legacy file/cost data remains a release-data check.

**Goal:** ให้ Item เป็นตัวตนหลักที่ใช้ต่อจาก Estimate ไปยังจัดซื้อ คลัง และผลิตได้ โดยมีรหัสภายใน/SKU ชัดเจน, Barcode/GTIN หลายหน่วยที่ตรวจสอบได้ และฟอร์ม Item ที่แก้ทุกฟิลด์ของ baseline ปัจจุบันได้จริง

**Architecture:** คง `Item.id` เป็น identity ถาวรและ `Item.code` เป็นรหัสภายใน/SKU สำหรับสินค้าที่ซื้อหรือเก็บได้; เก็บ Barcode เป็น child relation แยกตามหน่วยและระดับบรรจุ. เพิ่ม API/EF migration แบบ additive, ใช้ Central API Client และ OpenAPI types, ไม่สร้าง stock/BOM/supplier transaction ในงานนี้

**Tech Stack:** SDK ตาม `backend/global.json`, .NET/C# Clean Architecture, EF Core/PostgreSQL, Next.js/React/TypeScript strict, TanStack Query, Playwright

## Global Constraints

- อ่าน `AGENTS.md`, `design.md`, `CONTEXT.md`, `docs/README.md` และ `backend/AGENTS.md`/`frontend/AGENTS.md` ก่อนแก้แต่ละส่วน
- ไม่ใช้ `any`, `as any`, `@ts-ignore`; ข้อความ UI ต้องมี `th/en` คู่กัน; ใช้ Tailwind semantic tokens และคอมโพเนนต์กลางก่อนสร้างใหม่
- Backend ถือ tenant scope, uniqueness, lifecycle, ETag และ audit; FE guard เป็น UX เท่านั้น. EF Core ถือ write/transaction; error เป็น Problem Details แบบ stable code
- รักษางาน Item Catalog/Cost ที่ยังไม่ commit; ไม่แก้ migration เดิมย้อนหลัง. New schema ใช้ additive migration และทดสอบ legacy-data upgrade
- การเปิดใช้ Item สำหรับ Estimate ไม่บังคับ Barcode; โมดูลคลังที่ต้องสแกนค่อยกำหนด Gate ของตนเอง

## Decision and ownership map

| Concern | Owner | กฎรากฐาน |
| --- | --- | --- |
| Item identity | `Item` | `id` ถาวร, `code` ไม่ซ้ำใน Organization และไม่เปลี่ยนหลัง Activate; `code` คือ SKU ภายในของ physical Item |
| Barcode/GTIN | `ItemBarcode` | หลายค่าต่อ Item, หนึ่งค่าชี้ Item/Unit/Quantity เดียว; GTIN ผ่าน length/check digit, ค่าไม่ถูกนำกลับไปใช้กับ Item อื่น |
| Product vs Material | `Item.itemType` | เพิ่ม `product` สำหรับสินค้าสำเร็จรูปที่ผลิตซ้ำได้; custom built-in ต่อโครงการยังเป็น Work Item |
| Supplier SKU | Procurement ในอนาคต | เป็นรหัสผู้ขายที่สัมพันธ์ Item–Supplier, ไม่ใช่ Item Code หรือ GTIN |
| Technical specification / Variant family | Product/Production contract ในอนาคต | Typed และ versioned เมื่อมี calculation/BOM rule; ไม่ใช้ `attributes` เป็น authority |
| Lot/Serial/Stock/BOM | Inventory/Production ในอนาคต | Transaction และ revision ของโมดูลเจ้าของ; ไม่เพิ่มคอลัมน์ stock quantity/current BOM บน Item |

อ่านกฎฟิลด์จาก [Field Catalog](../../01-business/item-master-field-catalog.md), schema จาก [Data Contract](../../04-data/item-master-data-contract.md), HTTP จาก [API Contract](../../03-contracts/item-master-api-contract.md), และ lifecycle จาก [Governance](../../01-business/item-master-governance.md). เอกสารเหล่านี้เป็น authority; แผนนี้ระบุลำดับทำงานและหลักฐานรับงาน

## Task 1 — Align existing Item contract and identity

**Files:** `backend/src/TanErp.Api/Contracts/Items/ItemContracts.cs`, `backend/src/TanErp.Domain/Items/Item.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemConfiguration.cs`, `backend/src/TanErp.Api/Controllers/ItemsController.cs`, `backend/tests/TanErp.IntegrationTests/Api/ItemEndpointsTests.cs`, `frontend/src/features/item-master/schemas/item-form-schema.ts`, `frontend/src/features/item-master/components/item-editor.tsx`, OpenAPI/generated API types

**Produces:** Code สูงสุด 50 ตัวอักษรตรงกับ DB/Field Catalog ทั้ง API และ UI; request ที่แก้ Item ได้ครบ `taxCategoryCode`, Capability, Attributes และ Branch mode โดยไม่เขียน default ทับข้อมูลเดิม; `product` เป็น Item Type ที่ validation/DB/UI รองรับเมื่อ migration ผ่าน

Create/Update ต้องส่ง Capability ชัดเจนตามชนิด Item; UI เริ่ม `canCost=true`, `canSell=false`, `canStock=false`, `canProduce=false` และเสนอ `canPurchase=true` เฉพาะ Material/Subcontract โดยผู้ใช้ตรวจแก้ได้. Backend ไม่เดาค่า `true` ให้ทุก Capability เมื่อ request ขาด field

- [x] เพิ่ม test ว่า Code ความยาว 51 ตัวถูกปฏิเสธ, ตรวจ Product และ `taxCategoryCode` round trip; มี test เดิมครอบคลุม Code immutability และการไม่ reuse Code ของ Inactive Item
- [x] เพิ่ม test ว่า Edit Item ที่มี `selected_branches`, Capability, Attributes และ Tax Category แล้ว Save โดยรักษาค่าเดิม; สาขาและรายละเอียดบันทึกใน transaction เดียวกัน
- [x] ปรับ DTO/Domain/DB additive migration สำหรับ `product`; API request/response จำกัด Code 50 และ Tax Category 30; regenerate OpenAPI/types
- [x] รัน focused Item API tests, `dotnet build backend/TanErp.slnx --no-restore -m:1`, `cd frontend && npm run typecheck && npm run lint`

ตัวอย่าง round trip ที่ต้องคงค่าเมื่อแก้เฉพาะชื่อ:

```json
{
  "code": "MAT-PLY-18",
  "itemType": "material",
  "availabilityMode": "selected_branches",
  "capabilities": { "canSell": false, "canCost": true, "canPurchase": true, "canStock": false, "canProduce": false },
  "taxCategoryCode": "MATERIAL",
  "attributes": { "thickness": "18mm" },
  "attributesSchemaVersion": 1
}
```

## Task 2 — Barcode aggregate, migration and validation

**Files:** เพิ่ม `backend/src/TanErp.Domain/Items/ItemBarcode.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemBarcodeConfiguration.cs`, additive migration ใน `backend/src/TanErp.Infrastructure/Persistence/Migrations/`, ปรับ `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`, เพิ่ม `backend/tests/TanErp.UnitTests/Items/ItemBarcodeTests.cs` และ `backend/tests/TanErp.IntegrationTests/Persistence/ItemBarcodeSchemaTests.cs`

**Produces:** ตาราง `item_master.item_barcodes` ตาม Data Contract; uniqueness ระดับ Organization, same-organization FK, one active primary per Item/packaging level และ GTIN check digit validation

Barcode ของชิ้นและลังเป็นคนละค่าได้ โดยทั้งสองคืนหน่วยฐาน `sheet` และจำนวน `1` หรือ `12` ตามลำดับ. การใช้หน่วยอื่นที่ไม่ใช่ Base Unit ต้องมี Unit Conversion ที่ยืนยันแล้ว; Barcode ไม่สร้าง conversion อัตโนมัติ

- [x] เพิ่ม unit tests สำหรับ GTIN-8/12/13/14 ที่ถูกต้อง, check digit ผิด, leading zero, Internal Barcode, quantity ≤0 และ packaging level ไม่รู้จัก; ใช้ GTIN ที่ตรวจ check digit แล้ว
- [x] เพิ่ม PostgreSQL coverage สำหรับ Barcode ซ้ำใน Organization เดียวกัน, ค่าเดียวกันข้าม Organization, FK Item/Unit ข้าม Organization, primary ซ้ำต่อ packaging level และ quantity constraint; API tests ยืนยัน duplicate 409 และ inactive value reuse 409
- [x] สร้าง Domain entity และ EF mapping/migration แบบ additive; เก็บ `value` เป็น string และ normalized string สำหรับ unique/search; เขียน audit ใน transaction เดียวกับ mutation
- [x] ซ้อม additive migration ใน Testcontainer จาก revision ก่อนมี Item Master; ยืนยันว่าไม่มีตาราง Item เดิมให้ backfill, migration สร้าง `items`/`item_barcodes`, ข้อมูลไฟล์ legacy ที่มีอยู่ยังอยู่ครบ และ API เปิดใช้ Item โดยไม่มี Barcode ได้

```text
GTIN value = digits of length 8, 12, 13 or 14 with valid GS1 check digit
Unique(organization_id, normalized_value), including inactive rows
Unique(organization_id, item_id, packaging_level) WHERE is_primary AND status = 'active'
ForeignKey(item_id, organization_id) → items(id, organization_id)
ForeignKey(unit_id, organization_id) → units(id, organization_id)
```

## Task 3 — Barcode API, search and permissions

**Files:** เพิ่ม `backend/src/TanErp.Api/Contracts/Items/ItemBarcodeContracts.cs`, `backend/src/TanErp.Api/Controllers/ItemBarcodesController.cs`; reuse `backend/src/TanErp.Application/Items/IItemStore.cs` and `backend/src/TanErp.Infrastructure/Persistence/Items/ItemStore.cs`; ปรับ permission seed/error resources/OpenAPI และ `backend/tests/TanErp.IntegrationTests/Api/ItemEndpointsTests.cs`

**Produces:** Barcode list/create/primary/deactivate และ exact scan lookup ตาม Planned API Contract; create ใช้ Idempotency-Key, mutation ใช้ If-Match, search คืน structured Item/Unit/Quantity เดียว

```json
{
  "identifierType": "gtin",
  "value": "6291041500213",
  "unitId": "11111111-1111-4111-8111-111111111111",
  "quantityInBaseUnit": "12.0000",
  "packagingLevel": "case",
  "isPrimary": true
}
```

ตัวอย่างในเอกสารเป็นรูปแบบ payload เท่านั้น; test ต้องใช้ GTIN ที่คำนวณ check digit ถูกต้องและ UUID ของ Unit ที่ seed จริงใน Testcontainer

- [x] เพิ่ม API tests สำหรับ malformed GTIN 422, stale ETag 409, key เดิม/เนื้อหาต่าง, create replay, duplicate 409, scan, inactive 404, inactive value reuse 409 และ cross-organization 404
- [x] เพิ่ม API test ว่า leading zero ไม่ถูกตัด; scan normalizes supported GTIN forms
- [x] ใช้ Item store ผ่าน EF transaction; controllers รับ request/response เท่านั้น; เพิ่ม permission `items.manage-barcodes`, localized Problem Details และ regenerate OpenAPI
- [x] รัน focused Barcode/Unit Conversion integration tests, `OpenApiContractTests` และ `dotnet build`; ใช้ generated frontend contract

## Task 4 — Unit conversion foundation for non-base barcodes

**Files:** เพิ่ม `backend/src/TanErp.Domain/Items/ItemUnitConversion.cs` และ `UnitConversion.cs`, EF mapping, additive migrations, store/API contracts, integration/schema tests; ปรับ OpenAPI/generated types และ conversion maintenance UI

**Produces:** Versioned conversion เฉพาะ Item สำหรับหน่วยบรรจุ และ Shared Exact Conversion สำหรับหน่วย Dimension เดียวกัน; Barcode ที่ `unitId` ต่างจาก Base Unit ผ่านได้เมื่อมี conversion ที่ active/effective และ Factor ตรงกับ quantity snapshot เท่านั้น

- [x] เพิ่ม tests สำหรับ factor ≤0, self-loop, cycle, period ซ้อน, cross-organization FK และ barcode scan ที่คืน quantity snapshot
- [x] เพิ่ม EF mapping, migrations, store และ thin endpoints สำหรับทั้ง Shared และ Item Conversion; create ใช้ idempotency, advisory transaction lock, audit และ stable localized Problem Details
- [x] ทดสอบ Barcode ที่ Base Unit ผ่านได้; Barcode หน่วยอื่นที่ไม่มี Conversion หรือ quantity ไม่ตรงถูกปฏิเสธ; หลังเพิ่ม conversion แล้ว scan คืน Item/Unit/quantity snapshot
- [x] รัน focused Item/Shared Unit Conversion, Barcode และ OpenAPI tests รวมถึง `dotnet build`
- [x] เพิ่ม API coverage สำหรับ conversion ที่หมดอายุและ regression ว่า barcode เดิมยังอ่าน factor snapshot เดิมหลังเพิ่ม conversion รุ่นถัดไป

## Task 5 — Complete Item maintenance UI

**Files:** `frontend/src/features/item-master/components/item-editor.tsx`, `frontend/src/features/item-master/components/item-barcode-maintenance.tsx`, `frontend/src/features/item-master/schemas/item-form-schema.ts`, `frontend/src/features/item-master/api/item-master-queries.ts`, `frontend/src/lib/api/api-client.ts`, `frontend/src/messages/th.json`, `frontend/src/messages/en.json`; เพิ่ม Item editor/Barcode component tests

**Produces:** หน้า `[id]` เดียวที่ Create/Edit Item ได้ครบ Field Catalog ปัจจุบัน รวม Alias, Branch Availability, Capability, Attributes, Tax Category, Image และ Barcode; ไม่ hardcode Capability/default branch ทับข้อมูล; ใช้ `FormTabs`, `useFormTabErrors`, `MultiLangInput`, File Service และ Confirmation Modal กลาง

- [x] เพิ่ม component tests สำหรับ edit round trip, tab error indicator, no double submit, rejected update ที่คง input, permission, loading/error, deferred image upload และ required Barcode; stale ETag/GTIN check-digit มี API tests และ Thai/English มี Playwright coverage
- [x] ต่อ Barcode list/create/primary/deactivate ผ่าน central client/generated types และ TanStack Query key ที่รวม Membership/Locale; scan lookup ให้ Backend resolve
- [x] ขยาย invalidation ให้ครอบคลุม Item detail/list และ Estimate Catalog หลัง Barcode mutation
- [x] แนบภาพผ่าน verified File Session เฉพาะเมื่อกดส่งภาพหลัง Item มี ID; bind `parentType=item` และคง File ID ที่ verify แล้วระหว่าง retry
- [x] ฟอร์มใช้ FormTabs, auto-switch ไป tab ที่มี validation error, controlled capability/branch/attribute values, Alias maintenance และ confirmation สำหรับการลบ
- [x] เพิ่ม component test สำหรับการสร้าง Item packaging conversion และระบุ Base Unit เป็นปลายทาง
- [x] เพิ่ม Playwright acceptance สำหรับสร้าง Item ผ่านฟอร์ม, เพิ่ม GTIN เป็น primary, activate และสแกนได้ quantity snapshot, ยืนยันก่อน deactivate แล้วสแกนไม่พบ, ภาษาไทย/อังกฤษ, viewport 320px และ keyboard focus
- [x] Component acceptance ยืนยันว่าเลือกรูปแล้วไม่ upload จนกด submit และแนบเฉพาะ File ID ที่ verified
- [ ] ตรวจ tablet/zoom 200% ด้วย browser จริง
- [x] รัน schema tests 4 cases, `npm run typecheck`, `npm run lint`, และ production build แบบ Webpack

## Task 6 — Acceptance and release evidence

**Files:** เพิ่ม `frontend/e2e/item-master-identity.spec.ts`, ปรับ `docs/05-engineering/item-master-estimate-catalog-verification.md` และแผนนี้เฉพาะผลจริง

**Produces:** หลักฐานผู้ใช้สร้าง Item ที่เป็น SKU, เพิ่ม GTIN แผ่น/ลัง, ค้นหรือสแกนแล้วได้หน่วยถูกต้อง, เปิดใช้ Item, สร้าง/Publish Cost และเลือกเข้า Estimate BOQ โดย Snapshot เดิมไม่เปลี่ยน

- [x] ทดสอบ Barcode ค่าเดียวกันข้าม Organization, uniqueness ภายใน Organization, scan ที่ scope ตามองค์กร/สาขา, และ Barcode แยกตามระดับบรรจุภัณฑ์ผ่าน PostgreSQL/API tests
- [x] รัน Item Master browser journey: create Item ผ่าน UI, add/activate/scan/deactivate GTIN ผ่าน confirmation พร้อมตรวจ quantity snapshot และ inactive 404, Thai/English, viewport 320px/keyboard; ใช้ disposable database
- [x] รัน priced Item → BOQ journey: เลือก Item ที่มี published cost เข้า BOQ, บันทึก/โหลดซ้ำ, ยืนยัน snapshot เดิมหลัง publish ราคาใหม่ และตรวจ cost-version conflict โดยข้อมูลในฟอร์มยังอยู่ครบ
- [ ] ตรวจ 200% zoom ด้วย browser จริงและบันทึกผล
- [x] รัน `dotnet build backend/TanErp.slnx -m:1` (0 warning/error) และ frontend `npm run typecheck`, `npm run lint`, `npm run test` (512/512), `npm run build -- --webpack`
- [x] รัน backend Unit Tests (223/223), Architecture Tests (3/3), และ Integration Tests (254/254)

## Exit criteria

Foundation ผ่านเมื่อ Item Code/SKU ไม่กำกวม, Barcode/GTIN หลายระดับมี invariant และค้นหาได้, Item form แก้ข้อมูลที่มีอยู่ครบ, migration แบบเพิ่มส่วนใหม่รักษาข้อมูลเดิมโดยไม่สร้าง Item Code เทียม และ priced Item → BOQ พร้อม Snapshot ผ่าน browser journey. Item Master เป็น schema ใหม่และไม่มี legacy Item rows ให้ backfill; migration test ยืนยัน baseline ก่อน/หลังและรักษาไฟล์ legacy ที่มีอยู่. Procurement/Inventory/Production เพิ่ม data contracts ของตนได้โดยอ้าง `Item.id` เดิมโดยไม่เปลี่ยนความหมายของ Item Code
