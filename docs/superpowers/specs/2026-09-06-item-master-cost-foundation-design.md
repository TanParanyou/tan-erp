# Item Master and Cost Foundation Design

**สถานะ:** Approved Design Direction
**ขอบเขต:** Approved Design สำหรับ Estimate Catalog Foundation; การลงมือพัฒนาต้องอ้าง Implementation Plan ที่อนุมัติแยกต่างหาก

## เป้าหมาย

สร้างรากฐานข้อมูลสินค้า วัสดุ แรงงาน บริการ ผู้รับเหมา หน่วยนับ และต้นทุนอ้างอิงที่ Official Estimate ใช้ได้อย่างตรวจสอบย้อนหลัง พร้อมรองรับการต่อยอด Procurement, Inventory, Production และ MRP โดยไม่สร้างโมดูลเหล่านั้นก่อนเวลา

## แนวทางที่เลือก

ใช้ **Typed Item + Capability Flags + Versioned Cost Records**:

- `itemType` อธิบายธรรมชาติของรายการ: material, labor, service, subcontract หรือ other
- Capability ระบุการใช้งาน: `canSell`, `canCost`, `canPurchase`, `canStock`, `canProduce`
- Item Master เก็บ Identity/Classification/Default Unit; ไม่เก็บ Current Cost ก้อนเดียวบน Item
- Cost Record แยกเป็น Versioned Record ที่มี Source, Unit, Currency, Quantity Break, Branch Scope และ Effective Period
- Estimate เก็บ Cost Snapshot ที่ใช้จริง จึงไม่เปลี่ยนตาม Master Data ภายหลัง
- `name` และ `description` ใช้ Localized JSONB รูปทรงคงที่ `{ th, en? }`; Core Business Fields ยังคงเป็น Typed Columns
- Item เป็นข้อมูลระดับ Organization และเปิดใช้ทุกสาขาหรือสาขาที่เลือกผ่าน Item Branch Availability
- Item Image อ้าง Verified File ของ File Service เดิม ไม่เก็บ Binary หรือ Public URL บน Item
- Category ใช้ Parent Category เพื่อรองรับ Subcategory โดยไม่สร้าง Entity ซ้ำ, Brand เป็น Master แยก และ Alias เป็นคำค้นของ Item
- Phase แรกไม่สร้าง BOM, Stock Balance, Supplier Contract หรือ General-purpose Workflow Builder

แนวทางนี้ยืดหยุ่นกว่าการใช้ Item Type เป็นตัวกำหนดทุกพฤติกรรม และปลอดภัยกว่าการเก็บราคาเดียวบน Item ส่วนการสร้าง Generic Product Model เต็มรูปแบบถูกเลื่อนไปจนมีข้อมูล Procurement/Inventory/Production จริง

## Domain Boundaries

- Item Master เป็นข้อมูลมาตรฐานที่นำกลับมาใช้ ไม่ใช่ Work Item ใน Estimate
- Work Item เป็นขอบเขตงาน/ผลส่งมอบ และอาจอ้าง Item ที่ `canSell=true`
- Cost Component อ้าง Item ที่ `canCost=true` พร้อม Cost Record/Snapshot
- Unit Conversion เป็นของ Item เมื่อการแปลงขึ้นกับขนาด/บรรจุภัณฑ์; Conversion กลางใช้เฉพาะมิติเดียวกันและ Exact เท่านั้น
- Cost Source บอกที่มา/หลักฐาน ส่วน Cost Record บอกค่าต้นทุนที่ใช้ได้ใน Scope/ช่วงเวลา
- Reference Rate ของ Quick Estimate ไม่ใช่ Cost Record ของ Official Estimate
- Item Branch Availability บอกว่าเลือก Item ในสาขาได้หรือไม่; Branch-scoped Cost Record บอกต้นทุนของสาขาและไม่ใช่สิ่งเดียวกัน
- Item Image เป็น Relation ไปยัง Verified File; File Service เป็นเจ้าของ Binary, Media Validation และสิทธิ์ดาวน์โหลด

## Estimate Catalog Foundation Boundary

Slice แรกที่เชื่อม `estimate-item-catalog-modal.tsx` รองรับเฉพาะ Active Item ที่ `canCost=true`, THB, Base Unit, Organization Default Cost และ Branch Override แบบ Deterministic พร้อม Primary Image และ Estimate Snapshot

ภายใน Slice นี้รวม Item/Hierarchical Category/Brand/Alias/Unit CRUD ขั้นต่ำ, Activation/Deactivation, Branch Availability, Item Image Attach/Detach/Reorder, Cost Maker–Checker/Publish, Catalog Search และการเก็บ Item/Cost Snapshot ใน Estimate ส่วน Unit Conversion Chain, Quantity Break UI ขั้นสูง, Import, Supplier Integration, Inventory และ Production ยังคง Deferred แม้ Schema/Contract เดิมรองรับการต่อยอด

## Item Lifecycle

```text
Draft → Active → Inactive
```

- Draft เก็บข้อมูลไม่ครบได้แต่ใช้ใน Estimate ใหม่ไม่ได้
- Active ต้องผ่าน Required Field/Unit/Capability Validation
- Inactive ห้ามเลือกใหม่แต่ Historical Snapshot ยังอ่านได้
- Item ที่เคยถูกใช้งานห้าม Hard Delete
- Code เปลี่ยนไม่ได้หลัง Active ครั้งแรก; Name/Category/Capability เปลี่ยนได้พร้อม Audit และ ETag

## Cost Lifecycle

```text
Draft → Submitted → Approved → Published → Superseded
   └──── Returned ─────┘          └──────→ Disabled
```

- Published Cost Record แก้ย้อนหลังไม่ได้
- Cost Version ใหม่อาจกำหนด Effective From ในอนาคต
- Period ที่ชนกันใน Item/Unit/Currency/Quantity Range/Branch Scope เดียวกันต้อง Reject
- Branch Override เป็น Cost Record ที่ Scope แคบกว่า ไม่แก้ Organization Default
- Disable หยุดการเลือกสำหรับ Estimate ใหม่ แต่ไม่กระทบ Snapshot เดิม

## Deterministic Cost Resolution

เลือก Cost ตามลำดับ:

1. Organization, Item, Unit, Currency และ Estimate Date ต้องตรง
2. Branch-specific Record ชนะ Organization Default
3. Quantity ต้องอยู่ใน Quantity Break
4. Source Priority มาจาก Versioned Cost Policy
5. Effective From ใหม่ที่สุดชนะ
6. หากยังเหลือหลายรายการระดับเดียวกัน ให้ Block `ITEM_COST_AMBIGUOUS` แทนการสุ่มเลือก

หากไม่พบ Cost ให้ผู้มีสิทธิ์ใช้ Provisional Cost พร้อมเหตุผลและ Approval Trigger หรือหยุดไว้เป็น Draft

## Unit Model

- Unit มี Code, Name TH/EN, Symbol, Dimension และ Decimal Scale
- Base Unit อยู่บน Item และทุก Cost Record ระบุ Unit ชัดเจน
- Conversion Factor ใช้ Decimal และมี Direction/Effective Period
- Exact Conversion กลางใช้ได้เฉพาะ Dimension เดียวกัน เช่น cm ↔ m
- Packaging/Size Conversion เช่น sheet ↔ m² ต้องผูก Item/Variant และมี Rounding Rule
- ห้ามแปลงข้าม Dimension โดยเดา Factor

## Import Model

Import ใช้ Two-phase Batch:

```text
Upload → Parse → Preview/Validate → Commit หรือ Reject
```

- รองรับ CSV/XLSX แบบ Data-only; ไม่รัน Macro/Formula/External Link
- Item และ Cost ใช้ Sheet/Template แยกกัน
- Preview แสดง Create/Update/Skip/Error ก่อน Commit
- แถว Error ไม่ถูก Commit แบบ Partial โดยค่าเริ่มต้น
- Commit เป็น Atomic ต่อ Batch และใช้ Idempotency Key
- Duplicate ตรวจด้วย Organization + Item Code และ Cost Natural Key
- Export Error Report มี Row, Field, Stable Code และข้อความไทย/อังกฤษ

## Responsive UX

- Desktop: Master table + filter + Inspector; Cost History/Branch Override อยู่ใน Detail
- Tablet: table/card hybrid + Side Sheet
- Mobile: search/list → item detail; แก้ Field สำคัญและดู Current Cost ได้ แต่ Bulk Import เป็น Desktop/Tablet workflow
- ทุกขนาดแสดง Active/Inactive, Missing Cost, Stale Cost, Future Cost และ Branch Override โดยไม่ใช้สีอย่างเดียว
- Touch Target ≥44px, Input Mobile ≥48px, Keyboard/Screen Reader, Zoom 200%, ไทย/อังกฤษ และ Reduced Motion

## API Direction

Resource API:

```text
GET/POST        /api/v1/items
GET/PATCH       /api/v1/items/{id}
POST            /api/v1/items/{id}/activate
POST            /api/v1/items/{id}/deactivate
GET/POST        /api/v1/items/{id}/cost-records
POST            /api/v1/cost-records/{id}/submit
POST            /api/v1/cost-records/{id}/review-decisions
POST            /api/v1/cost-records/{id}/publish
GET/POST        /api/v1/units
GET/POST        /api/v1/item-unit-conversions
POST            /api/v1/item-import-batches
POST            /api/v1/item-import-batches/{id}/commit
```

- Firebase ให้ Identity; PostgreSQL ตรวจ Membership/Permission/Scope
- Draft mutation ใช้ ETag/If-Match
- Activate/Publish/Import Commit ใช้ Idempotency Key
- List ใช้ Cursor Pagination และ Stable Sort
- Import Upload ส่งผ่าน File Service Contract; API ไม่รับ Base64 file ใน JSON
- Error ใช้ RFC 9457 + Stable Code + Field/Row Pointer

## Data Direction

Relational Core:

- items (รวม Localized JSONB และ Typed Capability Columns), item_categories
- item_brands, item_aliases; Category ใช้ `parent_category_id` แทน Subcategory table
- item_branch_availabilities, item_images ซึ่งอ้าง Verified File
- units, unit_conversions, item_unit_conversions
- cost_sources, cost_records, cost_record_reviews
- item_import_batches, item_import_rows
- audit events และ estimate cost snapshots

JSONB ใช้กับ Localized Text ที่มีรูปทรงคงที่, Attributes ที่ไม่ใช่กฎธุรกิจ, Import Raw Row/Validation Metadata และ Versioned Snapshot เท่านั้น ห้ามเก็บ Item/Unit/Cost Core เป็น JSONB ก้อนเดียว

## Permission Direction

- `items.read`, `items.create`, `items.update`, `items.activate`, `items.deactivate`
- `items.manage-branches`, `items.manage-images`
- `cost-records.read`, `cost-records.create`, `cost-records.submit`, `cost-records.approve`, `cost-records.publish`, `cost-records.disable`
- `units.read`, `units.manage`
- `item-imports.create`, `item-imports.commit`

การมี Permission ไม่ข้าม Scope, State, Maker–Checker หรือ Cost Authority

## Documents to Deliver

1. `docs/01-business/item-master-flow.md`
2. `docs/01-business/item-master-field-catalog.md`
3. `docs/01-business/item-master-governance.md`
4. `docs/01-business/item-master-responsive-wireframe.md`
5. `docs/03-contracts/item-master-api-contract.md`
6. `docs/04-data/item-master-data-contract.md`
7. `docs/05-engineering/item-master-uat-scenarios.md`
8. ADR สำหรับ Typed Item/Capability และ Versioned Cost Record
9. Cross-links ใน Requirement, Permission, Error, Master Data, Conceptual Model และ Documentation Map

Static HTML Portal, SVG และ JSON Flow ถูกพักตามคำสั่งผู้ใช้วันที่ 2026-09-06; Text Wireframe ใน Markdown เป็นเอกสารภาพหน้าจอหลักของรอบนี้

## Test Coverage

- Item lifecycle, duplicate code และ inactive historical reference
- Capability/Unit validation และ invalid conversion
- Cost effective period overlap, branch precedence, quantity break และ ambiguous cost
- Provisional cost, stale cost, maker–checker และ published immutability
- ETag conflict, idempotent publish/import และ cross-organization denial
- Import preview/atomic commit/error report
- Estimate snapshot ไม่เปลี่ยนเมื่อ Item/Cost รุ่นใหม่ถูก Publish
- Responsive/keyboard/Thai-English/customer-safe display

## Out of Scope

- Supplier onboarding, Purchase Order และ Contract Management
- Stock on Hand, Warehouse, Lot/Serial และ Valuation
- BOM/Recipe, Routing, Work Center และ Production Planning
- Multi-level Variant/Configurator และ Product Rules Engine
- Automatic currency conversion
- การนำ Demo Cost ไปใช้ Production โดยไม่อนุมัติ

## Acceptance Criteria

- Item/Work Item/Cost Component/Cost Source/Cost Record ใช้ความหมายไม่ปะปนกัน
- Cost Resolution ให้ผลเดิมจาก Item/Unit/Currency/Date/Quantity/Branch/Policy Version เดิม
- Published Cost และ Historical Estimate Snapshot แก้ย้อนหลังไม่ได้
- Import ไม่ Commit ข้อมูลบางส่วนโดยไม่ตั้งใจและ Error ชี้ Row/Field ได้
- Desktop/Tablet/Mobile Wireframe แสดง State/Error/Permission ครบ
- API/Data/RBAC/Error/Markdown Flow ใช้ชื่อและ Lifecycle เดียวกัน
- ไม่มี Threshold/Cost จริงที่ไม่ได้ติด `TEST_ONLY`
- ไม่มี Application Code หรือ Dependency ใหม่
