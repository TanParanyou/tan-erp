# Estimate Master Data Maintenance Plan

**Status:** Draft implementation plan — scope and priorities agreed on 2026-09-23

**Goal:** ให้ทีมดูแลข้อมูลที่จำเป็นต่อ Official Estimate ได้เอง ตั้งแต่สร้าง Item และต้นทุนที่มีหลักฐาน ผ่านผู้ตรวจอิสระ เผยแพร่ แล้วเลือก Item เข้า BOQ และเปิด Estimate กลับมาได้โดย Snapshot เดิมไม่เปลี่ยน

**Reader:** ทีมพัฒนาและผู้ตรวจรับงาน Item Master/Official Estimate

## Decisions and boundary

1. รอบแรกครอบคลุม Item, Item Category, Item Brand, Unit of Measure, Cost Source และ Cost Record. Cost Source เริ่มที่ชนิด Manual พร้อมเหตุผลและหลักฐานที่ตรวจสอบได้; ไม่สร้าง Supplier Master เพื่อรองรับชนิดอื่นในรอบนี้
2. ตั้งต้นข้อมูลด้วยฟอร์มทีละรายการ. Import เป็นงานภายหลัง
3. หน้าดูแลต้นทุนรอบแรกสร้างราคา Default ระดับ Organization. ตัวแก้ราคาเฉพาะ Branch เป็นงานภายหลัง แต่การเลือก Item ใน Estimate ยังต้องใช้ Branch Context และ Cost Resolver เดิม
4. คำว่า “CRUD” ในแผนนี้หมายถึงการดูแลข้อมูลตาม Lifecycle ของแต่ละชนิด: Item ใช้ Draft/Active/Inactive; Cost Record ใช้ Draft/Submitted/Returned/Approved/Published/Disabled และสร้างรุ่นใหม่แทนการแก้ Published Record. ไม่มี Hard Delete สำหรับข้อมูลที่เคยถูกอ้างอิง
5. เกณฑ์จบคือเส้นทางจริง: จัดเตรียม Taxonomy/Unit → สร้างและ Activate Item → บันทึก Cost Source/Cost Record → ส่งตรวจและให้คนละคน Approve → Publish → เลือกเข้า BOQ → Save/Reload → ตรวจ Cost Snapshot

ยึดคำศัพท์และกฎ Lifecycle จาก [CONTEXT.md](../../../CONTEXT.md), [Item Master Governance](../../01-business/item-master-governance.md), [Item Master Field Catalog](../../01-business/item-master-field-catalog.md) และ [Item Master API Contract](../../03-contracts/item-master-api-contract.md). จุดที่เอกสารกับโค้ดขัดกันต้องปรับให้ตรงกันก่อนเริ่ม implementation

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
- ก่อนเพิ่ม endpoint Cost Source ให้แก้ความไม่ตรงกันของโมเดล: `CostSource` ในโค้ดปัจจุบันมี `code/name/priority/isActive` แต่ Field Catalog ระบุ `sourceType/supplierId/referenceNumber/evidenceFileId/capturedAtUtc/note`. แผนรอบแรกให้ Source ชนิด Manual เป็นข้อมูลระดับองค์กรที่เลือกซ้ำได้ ส่วนเหตุผล/reference/หลักฐานของ **ราคาหนึ่งฉบับ** เก็บบน Cost Record เพื่อไม่ให้แก้ Source แล้วหลักฐานราคาฉบับเก่าเปลี่ยน. ปรับ Field Catalog, API/Data Contract และ Domain ให้เป็นข้อตกลงเดียวกันก่อน migration; ห้ามเก็บข้อมูลเดียวกันสองตำแหน่งเป็น authority คู่กัน
- เมื่อเพิ่ม File evidence ต้องใช้ File Upload Session และ Verified File พร้อม Parent Invariant ที่ชัดเจน; File private, อ่านตาม permission, ไม่เก็บ public URL. UI เลือกไฟล์ไว้ก่อนและ upload ใน Submit เท่านั้น. หากหลักฐานไฟล์ยังไม่พร้อม ให้ policy ระบุอย่างชัดเจนว่ากรณี Manual ใช้ reference/reason แบบใดได้; ห้ามทำ field บังคับในเอกสารแต่ปล่อย `null` ใน API โดยไม่มี policy
- Published Cost/Estimate Snapshot เป็น immutable history. Cost รุ่นใหม่แทนรุ่นเก่าตาม effective period; การปิด Item หรือ Source ต้องไม่ทำให้ Estimate เดิมอ่านไม่ได้. Migration ต้อง additive และทดสอบทั้งฐานใหม่กับฐานที่มีข้อมูลเก่า

### HTTP contract, permission and concurrency

- API ใช้ `/api/v1`, UUID, UTC, typed request/response และ RFC Problem Details พร้อม stable `code` และ error resources ไทย/อังกฤษ. ระบุ response status ใน OpenAPI แล้ว generate FE types; FE ห้ามเขียน API shape เอง. `Idempotency-Key` ใช้กับ Create/Publish, `If-Match` กับ Update/Transition ตาม route ปัจจุบัน; retry key เดิมต้องไม่สร้างข้อมูลซ้ำ
- ใช้ `IRequestAccessResolver` จาก Firebase identity + PostgreSQL membership; scope จาก server เท่านั้น. Resource นอก Organization ตอบ 404. แยกสิทธิ์ `items.*`, `items.manage-taxonomy`, `units.*`, `cost-records.*`; ก่อนเพิ่ม Cost Source routes ให้ระบุ permission key ใน [Permission Catalog](../../03-contracts/permission-catalog.md), seed role mapping และทดสอบ read/manage แยกกัน. FE guard ใช้เพื่อ UX เท่านั้น
- Backend response ที่หน้าจอต้องแสดง relation ต้องเป็น structured projection เช่น Category/Brand/Unit/Source `{ id, code, name }`; อย่าส่ง UUID อย่างเดียวแล้วให้ FE `array.find`. รายการ Item ที่โตได้ต้องค้นหา/กรอง/แบ่งหน้าฝั่ง server ด้วย stable sort + `id` tie-breaker. ตรวจ list contract ของ Category/Brand/Unit และ Cost ก่อนใช้เป็นหน้าดูแล; ห้ามโหลดทั้งหมดมา filter ที่ browser
- ก่อน FE implementation ให้ reconcile route/verb จริงกับเอกสาร: Controller ปัจจุบันใช้ `PUT /items/{id}`, `PUT /item-categories/{id}` และ `/units-of-measure` ขณะที่ API Contract เขียน `PATCH` และ `/units`. ให้ OpenAPI, เอกสาร และ generated types ตรงกันก่อน; ห้ามสร้าง route คู่ขนานเพียงเพื่อให้ UI ผ่าน

### Frontend boundaries and reusable UI

- เพิ่ม `frontend/src/features/item-master/` สำหรับ API adapter, query keys/hooks, schema และ components; App Router page ทำหน้าที่ประกอบหน้าเท่านั้น. HTTP ทุกคำสั่งผ่าน `src/lib/api/api-client.ts`; server state อยู่ใน TanStack Query และ query key ต้องมี Membership/Locale/Filter. หลัง mutation invalidate เฉพาะ Item/Taxonomy/Cost/Catalog ที่ได้รับผล; เมื่อเปลี่ยน Membership หรือ logout ห้ามแสดง cache เดิม
- ใช้ generated OpenAPI types, TypeScript strict และ typed `ApiError.code`; ห้าม `any`, `as any`, `@ts-ignore`, string-message matching, arbitrary fallback หรือ FE join relation จากหลายรายการ. ฟอร์มใช้ React Hook Form + Zod, `[id]` route สำหรับ Item create/edit, `FormTabs` เมื่อมีหลายกลุ่มข้อมูล, loading แบบ Minimal Mono และปุ่ม `isLoading` ป้องกันส่งซ้ำ
- หน้ารายการยึด [Building ERP Lists](../../../.agents/skills/building-erp-lists/SKILL.md): `DataTable` คงอยู่เมื่อ loading/error/empty, filter state ผูก URL, pagination ฝั่ง server. ฟอร์มยึด [Building ERP Forms](../../../.agents/skills/building-erp-forms/SKILL.md); ใช้ shared component ก่อนสร้างใหม่. การปิดใช้/Disable ต้องมี Confirmation Modal พร้อม loading lock
- Styling ใช้ Tailwind semantic tokens, zero radius และ SVG stroke icons ตาม [design.md](../../../design.md). UI copy ทุกข้อความอยู่ใน `frontend/src/messages/th.json` และ `en.json`; ภาษาไทยเป็นค่าเริ่มต้น. กฎ business เช่น Maker–Checker, source validity และราคา authoritative อยู่ Backend

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

Baseline นี้อ้างอิงโค้ดวันที่ 2026-09-23; repository มีงาน Item Catalog ที่ยังไม่ commit. ก่อนเริ่มแต่ละงานให้เทียบกับ `HEAD` และ working tree ล่าสุดเพื่อไม่เขียนทับงานที่กำลังทำ. [Item Master Estimate Catalog Verification](../../05-engineering/item-master-estimate-catalog-verification.md) ยังเป็น `Partial`; การผ่าน browser journey ของ priced Item และการซ้อม migration ด้วยข้อมูลจริงยังเปิดอยู่

## Work packages

### 1. Contract and data readiness

- ตรวจ OpenAPI/DTO กับ Field Catalog ให้ตรงกันสำหรับ Cost Source และ Cost Record โดยเฉพาะ `costSourceId`, `sourceType`, reason/evidence, ETag, status และ Error Code. ตัดสินรูปแบบ Manual Source ที่เก็บหลักฐานได้ก่อนแก้ Schema หรือ API; ห้ามใช้ Supplier ID จำลอง
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
- ก่อนประกาศพร้อมใช้งานให้ปิดหลักฐานที่เปิดอยู่ใน [verification record](../../05-engineering/item-master-estimate-catalog-verification.md): priced Item browser journey และ migration rehearsal กับสำเนาข้อมูลจริง พร้อมรัน `dotnet build`, `dotnet test`, `npm run build`, `npm run lint` จากตำแหน่งโปรเจกต์ที่ถูกต้องและบันทึกผลจริง

## Order and exit criteria

1. ปิด Contract/Cost Source gap ก่อน เพื่อให้ราคาทุกตัวที่ทีมสร้างใหม่มีที่มาและตรวจสอบได้
2. เปิดหน้าข้อมูลอ้างอิง → Item → Cost workflow โดยทดสอบทีละช่วงและ reuse API/คอมโพเนนต์เดิม
3. ปิด E2E และ migration evidence ก่อนเปลี่ยนสถานะ verification จาก `Partial`

**Out of scope:** Customer/Site lifecycle เพิ่มเติม, Supplier CRUD, bulk Import, Unit Conversion Chain, Branch Cost management UI, Procurement, Inventory, Production และ MRP. กฎ Cost Resolver สำหรับ Branch และข้อมูลที่มีอยู่ยังคงใช้ตาม Contract เดิม

**Global reuse proposal:** หากฟอร์ม Category/Brand/Unit/Cost Source ใช้พฤติกรรมร่วมกันหลายจุด ให้สร้างเพียง primitive กลางสำหรับรายการอ้างอิงที่ใช้ API/validation/permission แบบกำหนดค่าได้ หลังตรวจว่าคอมโพเนนต์กลางปัจจุบันรองรับไม่ได้; ห้ามทำ abstraction ที่รวม Lifecycle ของ Item กับ Cost Record เพราะกฎต่างกัน
