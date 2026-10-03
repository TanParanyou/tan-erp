# Estimate Master Data Maintenance Plan

> **For implementation:** Execute the numbered tasks in order. Each task has a reviewable output and focused verification; do not treat this document as evidence that implementation has passed.

**Code status:** Complete — Item, taxonomy, branches, aliases, images, capabilities, attributes, tax category, SKU/Barcode, unit conversions, Cost Source/Record and Review Queue code paths are implemented with regression coverage. User acceptance remains for native browser review at tablet/200% zoom; a rehearsal against a sanitized copy of legacy files/cost data is a release-data check. An isolated migration test with 11 synthetic legacy file rows passes; the development database has not been accessed. This plan's unchecked implementation boxes are historical requirements, not a reliable progress checklist; use the dated verification record for verified coverage.

**Goal:** ให้ทีมดูแลข้อมูลที่จำเป็นต่อ Official Estimate ได้เอง ตั้งแต่สร้าง Item และต้นทุนที่มีหลักฐาน ผ่านผู้ตรวจอิสระ เผยแพร่ แล้วเลือก Item เข้า BOQ และเปิด Estimate กลับมาได้โดย Snapshot เดิมไม่เปลี่ยน

**Architecture:** ขยาย Item/Cost use cases และ EF stores ที่มีอยู่ใน Clean Architecture สี่ Project; Cost Source เป็นข้อมูลเลือกซ้ำระดับ Organization ส่วนหลักฐานราคาตรึงกับ Cost Record แต่ละฉบับ. Frontend แยก `item-master` feature และใช้ OpenAPI types, Central API Client, TanStack Query และคอมโพเนนต์กลาง

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core/EF Core/PostgreSQL, Next.js/React/TypeScript strict, TanStack Query, Playwright

**Reader:** ทีมพัฒนาและผู้ตรวจรับงาน Item Master/Official Estimate

## Global Constraints

- ทำงานบนสี่ Backend Projects และ `frontend/src/features/item-master/` ตามโครงเดิม; ห้ามเขียนทับการแก้ Item Catalog ที่ยังไม่ commit
- ทุก UI copy มีคู่ `th/en`; ห้ามใช้ `any`, `as any`, `@ts-ignore`, FE relation `array.find` หรือ fallback เดาข้อมูล
- ใช้ Tailwind semantic tokens, zero radius, SVG stroke icons, TanStack Query และ Central ApiClient
- Backend เป็นเจ้าของ RBAC, tenant scope, lifecycle, Cost Resolver, audit และราคา authoritative; FE guard เป็น UX เท่านั้น
- Published Cost และ Estimate Snapshot ห้ามแก้ย้อนหลัง; File private และต้องผ่าน Parent Invariant

## Decisions and boundary

1. รอบแรกครอบคลุม Item, Item Category, Item Brand, Unit of Measure, Cost Source และ Cost Record. Cost Source เริ่มที่ชนิด Manual พร้อมเหตุผลและหลักฐานที่ตรวจสอบได้; ไม่สร้าง Supplier Master เพื่อรองรับชนิดอื่นในรอบนี้
2. ตั้งต้นข้อมูลด้วยฟอร์มทีละรายการ. Import เป็นงานภายหลัง
3. หน้าดูแลต้นทุนรอบแรกสร้างราคา Default ระดับ Organization. ตัวแก้ราคาเฉพาะ Branch เป็นงานภายหลัง แต่การเลือก Item ใน Estimate ยังต้องใช้ Branch Context และ Cost Resolver เดิม
4. คำว่า “CRUD” ในแผนนี้หมายถึงการดูแลข้อมูลตาม Lifecycle ของแต่ละชนิด: Item ใช้ Draft/Active/Inactive; Cost Record ใช้ Draft/Submitted/Returned/Approved/Published/Disabled และสร้างรุ่นใหม่แทนการแก้ Published Record. ไม่มี Hard Delete สำหรับข้อมูลที่เคยถูกอ้างอิง
5. เกณฑ์จบคือเส้นทางจริง: จัดเตรียม Taxonomy/Unit → สร้างและ Activate Item → บันทึก Cost Source/Cost Record → ส่งตรวจและให้คนละคน Approve → Publish → เลือกเข้า BOQ → Save/Reload → ตรวจ Cost Snapshot

ยึดคำศัพท์และกฎ Lifecycle จาก [CONTEXT.md](../../../CONTEXT.md), [Item Master Governance](../../01-business/item-master-governance.md), [Item Master Field Catalog](../../01-business/item-master-field-catalog.md) และ [Item Master API Contract](../../03-contracts/item-master-api-contract.md). Contract ในเอกสารระบุ route ปัจจุบันและ endpoint ใหม่ที่ต้องสร้างแยกชัดเจน

## Development architecture and non-negotiable rules

### Ownership and dependency direction

ใช้ Modular Monolith และ Clean Architecture สี่ Project ตาม [Backend Architecture](../../02-architecture/backend-architecture.md). Runtime ที่ checkout นี้ตรึงไว้คือ .NET SDK `10.0.400` ใน `backend/global.json`; ให้ใช้ค่าใน repository จริงเมื่อรันงาน ไม่เปลี่ยน Target Framework เพื่อทำ Slice นี้

```text
Browser / item-master feature
  → Central ApiClient → TanErp.Api (HTTP, auth, request context, DTO)
  → TanErp.Application/Items (use case, port, permission/validation orchestration)
  → TanErp.Domain/Items (entity, lifecycle, invariants)
  → TanErp.Infrastructure/Persistence/Items (EF Core, transaction, projection)
  → PostgreSQL
```

- `Domain` เป็นเจ้าของ Item/Cost Source/Cost Record lifecycle และ invariant ที่ไม่ต้องพึ่งฐานข้อมูล. `Application` เป็นเจ้าของ use case และ interface ที่มี type ชัดเจน; ไม่ส่ง `DbContext` หรือ `IQueryable` ข้ามชั้น. `Infrastructure` เป็นเจ้าของ EF mapping, migration, organization-scoped query, transaction และ audit. Controller รับ/คืน HTTP เท่านั้น ห้ามใส่ business rule
- ขยาย store/handler ที่มีอยู่ (`IItemStore`, `ICostRecordStore`, `ItemStore`, `CostRecordStore`) เท่าที่จำเป็น. สร้าง Cost Source port/use case/store เฉพาะงาน ไม่สร้าง generic repository หรือ aggregate ใหม่สำหรับทุก lookup
- การเขียนทุกครั้งใช้ EF Core และบันทึก AuditEvent ใน transaction เดียวกับ business change. การ Publish Cost ต้องตรวจสถานะ, Maker–Checker, effective/quantity overlap และ supersession ภายใต้ transaction/lock เดิม; ห้ามเขียนทางลัดที่ข้าม `CostRecordStore`. Read ปกติใช้ EF projection; ใช้ Dapper/Raw SQL เฉพาะเมื่อมี query-plan evidence ตาม [Raw SQL Policy](../../04-data/raw-sql-policy.md)

### Data model and cost provenance

- Item อยู่ระดับ Organization; Category, Brand, Unit, Cost Source และ Cost Record ต้องพิสูจน์ same-organization ในทุกความสัมพันธ์. ใช้ composite FK หรือ transactional guard ที่มี integration test ตาม [Item Master Data Contract](../../04-data/item-master-data-contract.md). Item ไม่มี `currentCost`; ราคาใช้ published Cost ที่ Resolver เลือก ณ Branch/Unit/Quantity/Effective Time
- Cost Source มี `code/name/sourceType=manual/priority/isActive/rowVersion` ระดับ Organization. เหตุผล/reference/หลักฐานของ **ราคาหนึ่งฉบับ** อยู่ที่ Cost Record; ไม่เก็บซ้ำบน Source. [Field Catalog](../../01-business/item-master-field-catalog.md) และ [Data Contract](../../04-data/item-master-data-contract.md) ระบุเจ้าของข้อมูลแล้ว
- File evidence ใช้ File Upload Session และ Verified File พร้อม `parentType=costRecord`, `parentId=costId`; File private, อ่านตาม permission, ไม่เก็บ public URL. UI เลือกไฟล์ไว้ก่อนและ upload เมื่อ Submit เท่านั้น. Manual Cost ต้องมี Reason และ `sourceReference` หรือ Verified Evidence File
- Published Cost/Estimate Snapshot เป็น immutable history. Cost รุ่นใหม่แทนรุ่นเก่าตาม effective period; การปิด Item หรือ Source ต้องไม่ทำให้ Estimate เดิมอ่านไม่ได้. Migration ต้อง additive และทดสอบทั้งฐานใหม่กับฐานที่มีข้อมูลเก่า

### HTTP contract, permission and concurrency

- API ใช้ `/api/v1`, UUID, UTC, typed request/response และ RFC Problem Details พร้อม stable `code` และ error resources ไทย/อังกฤษ. ระบุ response status ใน OpenAPI แล้ว generate FE types; FE ห้ามเขียน API shape เอง. `Idempotency-Key` ใช้กับ Create/Publish, `If-Match` กับ Update/Transition ตาม route ปัจจุบัน; retry key เดิมต้องไม่สร้างข้อมูลซ้ำ
- ใช้ `IRequestAccessResolver` จาก Firebase identity + PostgreSQL membership; scope จาก server เท่านั้น. Resource นอก Organization ตอบ 404. แยกสิทธิ์ `items.*`, `items.manage-taxonomy`, `units.*`, `cost-records.*`; ก่อนเพิ่ม Cost Source routes ให้ระบุ permission key ใน [Permission Catalog](../../03-contracts/permission-catalog.md), seed role mapping และทดสอบ read/manage แยกกัน. FE guard ใช้เพื่อ UX เท่านั้น
- Backend response ที่หน้าจอต้องแสดง relation ต้องเป็น structured projection เช่น Category/Brand/Unit/Source `{ id, code, name }`; อย่าส่ง UUID อย่างเดียวแล้วให้ FE `array.find`. รายการ Item ที่โตได้ต้องค้นหา/กรอง/แบ่งหน้าฝั่ง server ด้วย stable sort + `id` tie-breaker. ตรวจ list contract ของ Category/Brand/Unit และ Cost ก่อนใช้เป็นหน้าดูแล; ห้ามโหลดทั้งหมดมา filter ที่ browser
- ใช้ route/verb ใน [Item Master API Contract](../../03-contracts/item-master-api-contract.md) ที่ปรับตาม Controller จริงแล้ว: `PUT /items/{id}`, `PUT /item-categories/{id}`, `PUT /item-brands/{id}`, `PUT /units-of-measure/{id}`. Cost Source และ Review Queue เป็น endpoint ใหม่ที่ต้องเพิ่ม; ห้ามสร้าง route คู่ขนานเพียงเพื่อให้ UI ผ่าน

### Frontend boundaries and reusable UI

- เพิ่ม `frontend/src/features/item-master/` สำหรับ API adapter, query keys/hooks, schema และ components; App Router page ทำหน้าที่ประกอบหน้าเท่านั้น. HTTP ทุกคำสั่งผ่าน `src/lib/api/api-client.ts`; server state อยู่ใน TanStack Query และ query key ต้องมี Membership/Locale/Filter. หลัง mutation invalidate เฉพาะ Item/Taxonomy/Cost/Catalog ที่ได้รับผล; เมื่อเปลี่ยน Membership หรือ logout ห้ามแสดง cache เดิม
- ใช้ generated OpenAPI types, TypeScript strict และ typed `ApiError.code`; ห้าม `any`, `as any`, `@ts-ignore`, string-message matching, arbitrary fallback หรือ FE join relation จากหลายรายการ. ฟอร์มใช้ React Hook Form + Zod, `[id]` route สำหรับ Item create/edit, `FormTabs` เมื่อมีหลายกลุ่มข้อมูล, loading แบบ Minimal Mono และปุ่ม `isLoading` ป้องกันส่งซ้ำ
- หน้ารายการยึด [Building ERP Lists](../../../.agents/skills/building-erp-lists/SKILL.md): `DataTable` คงอยู่เมื่อ loading/error/empty, filter state ผูก URL, pagination ฝั่ง server. ฟอร์มยึด [Building ERP Forms](../../../.agents/skills/building-erp-forms/SKILL.md); ใช้ shared component ก่อนสร้างใหม่. การปิดใช้/Disable ต้องมี Confirmation Modal พร้อม loading lock
- Styling ใช้ Tailwind semantic tokens, zero radius และ SVG stroke icons ตาม [design.md](../../../design.md). UI copy ทุกข้อความอยู่ใน `frontend/src/messages/th.json` และ `en.json`; ภาษาไทยเป็นค่าเริ่มต้น. กฎ business เช่น Maker–Checker, source validity และราคา authoritative อยู่ Backend

### UX/UI acceptance contract

ยึด [Item Master Responsive Wireframe](../../01-business/item-master-responsive-wireframe.md) เป็นเอกสารหลักของข้อมูลบนหน้าจอ, เส้นทางผู้ดูแล/ผู้ตรวจ, Screen States และ Responsive Behavior; แผนนี้กำหนดเพียงเกณฑ์ปิดงานที่ต้องพิสูจน์:

1. ผู้ใช้สร้างข้อมูลอ้างอิง → Item → Cost Draft ได้โดยไม่หลงทางหรือเสียค่าฟอร์ม, เห็น Required Gate ของ Activate และแยก Draft Cost จากราคาที่ Published ชัดเจน
2. ผู้ตรวจเห็น Maker/Last Editor, Source/หลักฐาน, ราคา, หน่วย, ช่วงมีผล และรุ่นก่อน Approve/Return; ผู้เผยแพร่เห็นผลกระทบก่อน Publish. Action ถูกซ่อนหรือปิดตามสิทธิ์/สถานะพร้อมเหตุผลที่อ่านเข้าใจ
3. Item List และ Cost Review มี Search/Filter/Pagination ฝั่ง server, Loading/Empty/Error/Retry ที่คงบริบท; conflict จาก ETag หรือราคาที่เปลี่ยนไม่ทำให้บันทึกทับหรือทิ้งข้อมูลผู้ใช้เงียบ ๆ
4. Desktop/Tablet/Mobile, Keyboard, Screen Reader, 320px, Zoom 200%, Reduced Motion และไทย/อังกฤษผ่านการตรวจบนเส้นทางสำคัญจริง; ปุ่ม Import/Conversion/Branch Cost ที่ยังไม่พร้อมไม่แสดงเป็น Action ใช้งานได้

### Architecture checks for each work package

1. **Contract first:** อัปเดต Field/API/Data Contract และ Permission Catalog เฉพาะจุดที่เปลี่ยน; ปรับ OpenAPI และ generated types ให้ตรง endpoint จริง
2. **Backend:** unit test invariant, PostgreSQL integration test สำหรับ tenant boundary/transaction/ETag/idempotency/audit/publish และ Architecture Tests สำหรับ dependency direction
3. **Frontend:** component/adapter tests สำหรับ typed error, permission, loading/empty/conflict และ E2E สองผู้ใช้ตาม acceptance journey
4. **Release:** build/test/lint ที่ `AGENTS.md` กำหนด และบันทึก evidence จริงใน verification document ก่อนเปลี่ยนสถานะจาก `Partial`

## Baseline and implementation gaps

| Concern | มีแล้ว | งานที่ต้องทำ |
| --- | --- | --- |
| Item | API สร้าง/อ่าน/แก้/Activate/Deactivate, Alias, Branch Availability, Image | หน้ารายการและฟอร์มดูแล Item; ต่อ API เดิมและตรวจ Contract ที่เปลี่ยนระหว่างงานค้าง |
| Category, Brand, Unit | API สร้าง/อ่าน/แก้ | หน้าดูแลและตัวเลือกใน Item Form; ตรวจผลกระทบของการปิดใช้ก่อนเพิ่มคำสั่ง Deactivate ใด ๆ |
| Cost Record | API Draft/Edit/Submit/Return/Approve/Publish/Disable, ETag และ Idempotency | หน้าบันทึกต้นทุนและคิวตรวจอนุมัติ; แสดงประวัติรุ่นและหลักฐาน |
| Cost Source | มี Entity และความสัมพันธ์กับ Cost Record | นิยาม Manual Source ให้ตรง Field Catalog, เพิ่ม API/permission/validation/audit และ UI ดูแล; ปิดช่องว่าง `costSourceId` ที่ API ปัจจุบันยังรับเป็น `null` กับเอกสารที่กำหนดให้มี Source |
| Estimate Catalog | API ค้นหาพร้อมราคาและ Modal สำหรับเลือกเข้า Estimate | ตรวจเส้นทาง priced Item → BOQ save/reload และ conflict เมื่อราคาเปลี่ยน; ไม่สร้าง Catalog อีกชุด |

Baseline นี้ตรวจซ้ำจากโค้ดวันที่ 2026-09-25; repository มีงาน Item Catalog ที่ยังไม่ commit. ก่อนเริ่มแต่ละงานให้เทียบกับ `HEAD` และ working tree ล่าสุดเพื่อไม่เขียนทับงานที่กำลังทำ. [Item Master Estimate Catalog Verification](../../05-engineering/item-master-estimate-catalog-verification.md) เป็น `Partial`; priced Item browser journey และ full PostgreSQL suite ผ่านแล้ว ส่วน native zoom/negative browser coverage และ migration rehearsal กับสำเนาข้อมูลจริงยังเปิดอยู่

## Work packages

### 1. Contract and data readiness

- ใช้ข้อตกลง Manual Source ใน Field/API/Data Contract ที่ปรับแล้วเป็นฐาน; เพิ่ม OpenAPI/DTO ให้ตรงกันสำหรับ `costSourceId`, `sourceType`, reason/evidence, ETag, status และ Error Code. ห้ามใช้ Supplier ID จำลอง
- เพิ่ม Cost Source API ระดับ Organization สำหรับ list/detail/create/update/deactivate เท่าที่ Lifecycle ต้องใช้ พร้อม scope, permission, ETag, AuditEvent และ Problem Details. บังคับ Source ที่ใช้งานได้เมื่อสร้าง/ส่งตรวจ Cost Record และตรวจความสัมพันธ์ Organization เดียวกัน
- หากต้องเพิ่มหลักฐานเป็นไฟล์ ให้ใช้ File Service/Verified File และกฎ Parent Invariant เดิม; ต้องพิสูจน์ว่าผูกไฟล์กับ Source/Record ได้อย่างถูกต้องก่อนเปิด UI แนบไฟล์. ระหว่างนั้น reason และ reference ต้องมี Contract ที่ชัดเจนและห้ามอ้างว่าไฟล์ถูกตรวจแล้ว
- ทดสอบการแก้ไขย้อนหลัง: Cost Published และ Estimate Snapshot เดิมคงเดิม, Source ที่ถูกปิดใช้ยังอ่านประวัติได้, ผู้ใช้ข้าม Organization อ่านหรือแก้ไม่ได้

### 2. Maintenance workspace

- เพิ่มทางเข้าหน้าดูแล Item Master ในเมนูตาม Permission. ใช้ฟอร์มและตารางกลางที่มีอยู่ก่อน (`DataTable`, `FormTabs`, `MultiLangInput`, `Button`, `ConfirmationModal`, `PageLoading`) และใช้ route `[id]` เดียวสำหรับสร้าง/แก้ Item ตามมาตรฐานฟอร์ม
- ทำ Category/Brand/Unit maintenance ก่อน Item Form เพื่อให้ผู้ใช้สร้างข้อมูลอ้างอิงได้ในลำดับการทำงานจริง. รองรับสถานะว่าง, ค้นหา/กรอง, ETag conflict และการเลือกข้อมูลที่ Active เท่านั้น; ถ้าต้องปิดใช้ข้อมูลที่ถูกอ้างอิง ให้กำหนดกฎ Backend และทดสอบก่อนแสดงคำสั่ง
- ทำ Item list/detail/form พร้อม Draft/Activate/Deactivate, Alias, Branch Availability และภาพตาม API เดิม. บังคับไทย/อังกฤษใน UI copy ผ่าน `messages/th.json` และ `messages/en.json`; ค่าชื่ออังกฤษของ Item อาจว่างตาม Contract. Upload ภาพเมื่อ Submit เท่านั้น
- ทำ Cost Source form และ Cost Record form/ประวัติภายใต้ Item. Default scope เป็น Organization; แสดง `Draft → Submitted → Approved → Published` และ Returned/Disabled อย่างถูกต้อง. คำสั่งเสี่ยงใช้ Confirmation Modal, loading lock, ETag และ Idempotency Key ตาม Contract
- แยก action ตาม Permission จริงและคิวผู้ตรวจ; Backend ต้องปฏิเสธ Maker หรือ Last Financial Editor ที่พยายามอนุมัติเอง. Frontend แสดงเหตุผลที่ทำต่อไม่ได้จาก Stable Error Code

### 3. End-to-end acceptance and release evidence

- ทดสอบ API ของ Cost Source/Cost Record สำหรับ same-organization, inactive source, missing evidence/reason, permission, Maker–Checker, stale ETag, replay และ publish overlap
- ทดสอบผู้ใช้สองคนผ่าน browser: ผู้ดูแลสร้าง Category/Brand/Unit/Item/Manual Source/Cost Draft; ผู้ตรวจ Approve; ผู้มีสิทธิ์ Publish; ผู้ประเมินราคาเลือก Item เข้า BOQ, Save และ Reload แล้วเห็น Cost Record ID/version/amount ใน Snapshot เดิม
- ทดสอบเปลี่ยนต้นทุนหลัง Snapshot แล้ว Estimate เดิมไม่เปลี่ยน, ราคาไม่พร้อม/ราคาเปลี่ยนขณะเลือก, Empty/Error state, keyboard, 320px viewport และภาษาไทย/อังกฤษ
- ก่อนประกาศพร้อมใช้งานให้ปิดหลักฐานที่ยังเปิดอยู่ใน [verification record](../../05-engineering/item-master-estimate-catalog-verification.md): native tablet/200% browser review, private-image และ negative/conflict browser paths, migration rehearsal กับสำเนาข้อมูลจริง พร้อมเก็บผล build/test/lint ล่าสุด

## Implementation tasks and review gates

งานต่อไปนี้ใช้ Contract ที่ปรับแล้วใน [Item Master API Contract](../../03-contracts/item-master-api-contract.md). แต่ละ Task ต้อง review ได้แยกกันและทดสอบเฉพาะส่วนก่อนเดินต่อ. ห้ามแก้ migration เก่าที่อยู่ใน working tree ของงาน Item Catalog; งานนี้ใช้ additive migration ใหม่หลังสำรวจข้อมูลจริง

### Task 1 — Manual Cost Source API และ migration

**Files:** `backend/src/TanErp.Domain/Items/CostSource.cs`, `backend/src/TanErp.Application/Items/ICostSourceStore.cs` (new), `backend/src/TanErp.Infrastructure/Persistence/Items/CostSourceStore.cs` (new), `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostSourceConfiguration.cs`, migration ใหม่ใน `backend/src/TanErp.Infrastructure/Persistence/Migrations/`, `backend/src/TanErp.Api/Controllers/CostSourcesController.cs` (new), `backend/src/TanErp.Api/Contracts/Items/CostSourceContracts.cs` (new), DI registration, permission seed, error resources, `backend/tests/TanErp.IntegrationTests/Api/CostSourceEndpointsTests.cs` (new)

**Produces:** `GET/POST /api/v1/cost-sources`, `GET/PUT /api/v1/cost-sources/{id}`, `POST /api/v1/cost-sources/{id}/deactivate`; response `{ id, code, name, sourceType, isActive, rowVersion }`. `sourceType` รับ `manual` เท่านั้น; server เป็นเจ้าของ `priority`. `PUT`/Deactivate ใช้ `If-Match`; mutation เขียน Audit ใน transaction เดียว

- [ ] เขียน integration tests ให้ fail สำหรับ code ซ้ำใน Organization, Cross-organization 404, read/manage permission, stale ETag, deactivated Source และ Audit หนึ่งครั้งต่อ mutation
- [ ] เพิ่ม `source_type` ด้วย additive migration; backfill Cost Source เดิมเป็น `legacy` แบบอ่านได้แต่เลือกใหม่ไม่ได้ และบันทึกจำนวนแถวก่อน/หลัง. สร้างใหม่ได้เฉพาะ `manual`; ห้ามตีความ Source เดิมเป็น Manual โดยพลการ
- [ ] ใช้ Domain + Application port + EF store + thin Controller ตาม dependency direction; update OpenAPI และ error resources `th/en`
- [ ] รัน `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~CostSourceEndpointsTests` และ `dotnet build backend/TanErp.slnx`; review migration SQL ก่อนผ่าน Gate

### Task 2 — Cost Record provenance และไฟล์หลักฐาน

**Files:** `backend/src/TanErp.Domain/Items/CostRecord.cs`, `backend/src/TanErp.Application/Items/ICostRecordStore.cs`, `backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs`, `backend/src/TanErp.Api/Contracts/Items/CostRecordContracts.cs`, `backend/src/TanErp.Domain/Files/FileUploadSession.cs`, `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs`, `backend/src/TanErp.Infrastructure/Files/FileStore.cs`, `backend/tests/TanErp.IntegrationTests/Api/CostRecordEndpointsTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/FileUploadSessionTests.cs`

**Consumes:** Active Manual Cost Source จาก Task 1. **Produces:** Cost Draft ใหม่ต้องมี `costSourceId` + Reason + Source Reference หรือ Verified File; `parentType=costRecord`, `parentId=costId` สำหรับหลักฐาน. Legacy Cost ที่ Source ว่างยังอ่านได้ แต่ Submit/Publish ถูกปฏิเสธ; Published เก่าไม่ถูกแก้ย้อนหลัง

- [ ] เขียน tests ให้ fail สำหรับ Source ว่าง/Inactive/ข้าม Organization, Manual ไม่มี Reason/Reference/Evidence, File ข้าม Parent/Organization/ยังไม่ Verified, และ Legacy Draft ที่พยายาม Submit
- [ ] เพิ่ม Cost Record parent ใน File Parent Access Resolver โดยตรวจ `cost-records.create` สำหรับเขียนและ `cost-records.read` สำหรับอ่าน; File ที่แนบต้องมี Parent Type/ID ตรงกับ Cost Record ใน Organization เดียวกัน
- [ ] บังคับ rule ใน Application/Domain/Store ขณะ Create/Update/Submit/Publish; ส่ง stable Problem Code และไม่ใช้ string message ใน FE. ทำ Amount `0` ให้ผ่านเฉพาะเมื่อมี Reason และ Independent Checker ตาม Governance (DTO/Domain ปัจจุบันยังปฏิเสธค่า `0`); ทดสอบค่าเป็นลบถูกปฏิเสธเสมอ
- [ ] ลำดับ upload: Create Draft ด้วย Reference → เลือก File ค้างไว้ใน Browser → Upload หลัง Save ได้ `costId` → PUT Draft ด้วย Verified `fileId` + ETag → Submit. สำหรับ Cost Editor ใหม่ให้ใช้ decimal string ที่ parse เป็น `decimal` ฝั่ง server ตาม [Item Master API Contract](../../03-contracts/item-master-api-contract.md); ปรับ Cost Record DTO/OpenAPI/generated types และเพิ่ม precision round-trip test ของ `amount`/`quantity` โดยไม่ผ่าน JS floating-point
- [ ] ทดสอบ snapshot immutability และ transaction rollback เมื่อหลักฐานไม่ผ่าน; รัน focused `CostRecordEndpointsTests` + `FileUploadSessionTests` และ review migration upgrade ของ legacy Cost

### Task 3 — Read models สำหรับหน้าดูแลและคิวผู้ตรวจ

**Files:** `backend/src/TanErp.Application/Items/IItemStore.cs`, `backend/src/TanErp.Infrastructure/Persistence/Items/ItemStore.cs`, `backend/src/TanErp.Api/Controllers/ItemsController.cs`, `backend/src/TanErp.Api/Controllers/ItemTaxonomyController.cs`, `backend/src/TanErp.Application/Items/ICostRecordStore.cs`, `backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs`, `backend/src/TanErp.Api/Controllers/CostReviewQueueController.cs` (new), response contracts, `backend/tests/TanErp.IntegrationTests/Api/ItemEndpointsTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/CostReviewQueueEndpointsTests.cs` (new)

**Produces:** Item List ที่ใช้ `pageNumber/pageSize` ตาม Contract ปัจจุบัน; Category/Brand/Unit/Source lookup ที่จำกัดผลและค้นหา Server-side; Review Queue แบบ page ที่มี Item/Source/Unit/Maker/Last Editor เป็น structured objects. ห้าม FE ดึงทุก Cost Record แล้วประกอบคิวด้วย `find`

- [ ] เขียน integration tests ให้ fail สำหรับตัวกรอง, stable sort + `id` tie-breaker, หน้าว่าง, scope และสิทธิ์ Review Queue; ทดสอบว่า relation แสดงผลครบจาก response เดียว
- [ ] ขยาย query/projection เฉพาะที่ UI ใช้; กำหนด limit สูงสุดและ paging ทั้งหน้าบริหารและ lookup. เพิ่ม `Idempotency-Key` ให้ Create Item/Category/Brand/Unit ที่ปัจจุบันยังไม่มี โดยใช้ Request Context และ Idempotency Store กลาง; test retry key เดิม/payload เดิมและ key เดิม/payload ต่าง. หากเปลี่ยน shape ของ list เดิม ให้ regenerate OpenAPI และแก้ consumer ที่ใช้ list เดิมใน change เดียวกัน
- [ ] รัน focused `ItemEndpointsTests` + `CostReviewQueueEndpointsTests` และ `OpenApiContract` tests; ตรวจ PostgreSQL query plan ก่อนเลือก Raw SQL

### Task 4 — Reference Data และ Item maintenance UI

**Files:** เพิ่ม routes ภายใต้ `frontend/src/app/[locale]/(erp)/item-master/` (`page.tsx`, `[id]/page.tsx`, `reference-data/page.tsx`), เพิ่ม `frontend/src/features/item-master/api/`, `components/`, `schemas/`, ปรับ ERP navigation, `frontend/src/messages/th.json`, `frontend/src/messages/en.json`, generated API types และ component/adapter tests ภายใต้ feature ใหม่

**Consumes:** API/lookup จาก Tasks 1–3. **Produces:** หน้า Item List/Detail/Create/Edit และ Reference Data ที่รักษา state เมื่อกลับจากการเพิ่ม Category/Brand/Unit/Source; Item Draft/Activate/Deactivate, Alias, Branch Availability และรูปผ่าน API เดิม

- [ ] เขียน component tests ให้ fail สำหรับ loading/empty/error, form validation, tab error indicator, ETag conflict, no double submit, i18n และ permission-specific actions
- [ ] ใช้ `DataTable`, `ListToolbar`, `useListState`, `FormTabs`, `MultiLangInput`, `ConfirmationModal`, `ImageUpload`/File flow ที่มีอยู่; Item ใช้ `[id]` route เดียวสำหรับ create/edit. รายการข้อมูลอ้างอิงใช้ UI สั้นตาม wireframe; ไม่มี Import/Conversion/Branch Cost action ที่ใช้งานได้
- [ ] HTTP ผ่าน Central ApiClient และ generated types; Query Key มี Membership/Locale/Filter; invalidation รวม Item, lookup และ Estimate Catalog เมื่อ Item ที่ใช้ได้เปลี่ยน. ตรวจ 320px/keyboard/200% zoom ด้วย component หรือ browser test ที่เหมาะสม
- [ ] รัน `cd frontend && npm run lint && npm run typecheck && npm run test`; review หน้าจอ Desktop/Tablet/Mobile ตาม [wireframe](../../01-business/item-master-responsive-wireframe.md)

### Task 5 — Cost editor, Review Queue และ Publish UX

**Files:** `frontend/src/features/item-master/components/cost-record-editor.tsx`, `cost-timeline.tsx`, `cost-review-queue.tsx` (new), related API/hooks/schemas/tests, route `frontend/src/app/[locale]/(erp)/item-master/cost-reviews/page.tsx` (new), `frontend/src/messages/th.json`, `frontend/src/messages/en.json`

**Consumes:** Task 2 provenance/attachment และ Task 3 Review Queue. **Produces:** แยก Current Published/Future/Draft/History, ผู้จัดทำส่งตรวจ, ผู้ตรวจคนละคน Approve/Return, ผู้มีสิทธิ์ Publish พร้อมผลกระทบและ Confirmation

- [ ] เขียน tests ให้ fail สำหรับ Draft ไม่ถูกแสดงเป็น Current Price, Maker Self-Approve ถูกปฏิเสธ, ETag Conflict คงฟอร์มไว้, Return reason, Publish confirmation และ retry ที่ไม่สร้างคำสั่งซ้ำ
- [ ] ใช้ Form/Modal/StatusBadge กลาง, typed Error Code, `isLoading` lock, Deferred Upload และอัปเดต Query Cache หลังแต่ละ Transition. ผู้ตรวจเห็น Maker, Last Editor, Source, Reference/File, Amount, Unit, Effective Period และรุ่นก่อนตัดสินใจ
- [ ] รัน focused component/adapter tests แล้ว `npm run lint`, `npm run typecheck`; ตรวจ Thai/English และ responsive states ตาม wireframe

### Task 6 — End-to-end, migration rehearsal และ release evidence

**Files:** เพิ่ม `frontend/e2e/item-master-estimate-flow.spec.ts`, ขยาย integration tests ที่เกี่ยวข้อง, อัปเดต `docs/05-engineering/item-master-estimate-catalog-verification.md` ด้วยผลจริง

**Produces:** หลักฐานสองผู้ใช้: เตรียม Reference Data → Activate Item → สร้าง/ส่งตรวจ/อนุมัติ/Publish Cost → เลือกใน Catalog → Save/Reload BOQ → ราคาและ Cost Record version ใน Snapshot เดิมไม่เปลี่ยนหลัง Publish รุ่นใหม่

- [x] รัน Playwright บนฐานข้อมูลแยก: ผู้จัดทำ/ผู้ตรวจคนละตัวตน, priced Item → BOQ, Save/Reload, snapshot คงเดิมหลัง publish ราคาใหม่, ทดสอบ cost-version conflict โดยฟอร์มไม่ทิ้งข้อมูล, Thai/English, 320px และ keyboard ผ่าน
- [x] ทดสอบ browser สำหรับ private-image access, error/empty states, deferred upload และ native file authorization
- [ ] ตรวจ native tablet/200% zoom (มีเพียง viewport 600 CSS px ซึ่งเป็น reflow evidence เทียบเท่า ยังไม่ใช่ native zoom)
- [x] ซ้อม migration จาก pre-Item-Master schema ด้วยไฟล์ legacy สังเคราะห์ 11 รายการ; ตรวจ path/name ครบ, `scan_status=pending`, ไม่มี hash/verified timestamp และไม่มี default
- [ ] ซ้อม migration บนสำเนาฐานจริงที่มี legacy files/cost sources/cost records; บันทึกจำนวนแถวก่อน/หลัง, SQL และ same-organization constraints โดยไม่แก้ฐาน development จริง
- [x] รัน `dotnet build backend/TanErp.slnx`, full `dotnet test backend/TanErp.slnx`, `npm run build -- --webpack`, `npm run lint`, `npm run typecheck`, `npm run test`; OpenAPI/generated types ผ่าน Contract tests และอัปเดตใน change set
- [ ] เปลี่ยนสถานะจาก `Partial` หลัง native browser acceptance และ migration rehearsal บนสำเนาข้อมูลจริงผ่าน

## Order and exit criteria

1. ปิด Contract/Cost Source gap ก่อน เพื่อให้ราคาทุกตัวที่ทีมสร้างใหม่มีที่มาและตรวจสอบได้
2. เปิดหน้าข้อมูลอ้างอิง → Item → Cost workflow โดยทดสอบทีละช่วงและ reuse API/คอมโพเนนต์เดิม
3. ปิด E2E และ migration evidence ก่อนเปลี่ยนสถานะ verification จาก `Partial`

**Out of scope:** Customer/Site lifecycle เพิ่มเติม, Supplier CRUD, bulk Import, Unit Conversion Chain, Branch Cost management UI, Procurement, Inventory, Production และ MRP. กฎ Cost Resolver สำหรับ Branch และข้อมูลที่มีอยู่ยังคงใช้ตาม Contract เดิม

**Global reuse proposal:** หากฟอร์ม Category/Brand/Unit/Cost Source ใช้พฤติกรรมร่วมกันหลายจุด ให้สร้างเพียง primitive กลางสำหรับรายการอ้างอิงที่ใช้ API/validation/permission แบบกำหนดค่าได้ หลังตรวจว่าคอมโพเนนต์กลางปัจจุบันรองรับไม่ได้; ห้ามทำ abstraction ที่รวม Lifecycle ของ Item กับ Cost Record เพราะกฎต่างกัน
