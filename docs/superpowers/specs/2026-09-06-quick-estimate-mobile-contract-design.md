# Quick Estimate Mobile Wireframe and Contract Design

**สถานะ:** Approved Design Direction  
**ขอบเขต:** Documentation Foundation เท่านั้น — ยังไม่สร้าง Application Code

## เป้าหมาย

กำหนดรากฐาน Mobile UX, API Contract และ Data Contract ของ Quick Estimate เพื่อให้เจ้าหน้าที่ประเมินช่วงราคาหน้างานได้เร็ว และรองรับการเปลี่ยน Field/Template ในอนาคตโดยไม่ผูกหน้าจอกับประเภทงานแบบตายตัว

## ขอบเขต

รวม:

- Mobile-first Wireframe สำหรับ Quick Estimate
- Task-oriented API ที่รองรับ Autosave, Calculate, Review, Share และ Convert
- Typed Relational Data สำหรับข้อมูลหลัก
- Versioned JSONB สำหรับ Template Configuration และ Immutable Calculation Snapshot
- Error Localization, RBAC, Scope, Maker–Checker, Concurrency และ Idempotency

ไม่รวม:

- Application Code ของ Frontend/Backend
- ราคาจริง, Authority Limit, SLA และ Pilot Threshold
- Official Estimate, Quotation, Procurement, Inventory, Production และ MRP Screen

## แนวทางที่เลือก

ใช้ **Config-driven Mobile Wizard + Task-oriented API + Hybrid Relational/JSONB Data**

เหตุผล:

- ผู้ใช้หน้างานเห็นเฉพาะ Field ที่จำเป็นในแต่ละขั้น ลดเวลาคีย์และความผิดพลาด
- Template สามารถเพิ่ม Field/Rule/Option รุ่นใหม่ได้โดยไม่สร้างหน้าจอเฉพาะทุก Work Type
- API สื่อ Intent ชัดเจนและบังคับ State/RBAC ที่ Backend ได้
- Relational Column รองรับ Integrity, Scope และ Query สำคัญ ส่วน JSONB รองรับ Config/Snapshot ที่เปลี่ยนรูปได้

ไม่เลือก Single Long Form เพราะใช้งานบนมือถือยากและ Error กระจายหลายจุด ไม่เลือก Generic JSON Form ทั้งระบบเพราะ Validation, Reporting และ Migration จะคลุมเครือเกินไป

## Mobile Information Architecture

Wizard หลักมี 5 ขั้นและมี Sticky Summary แสดงสถานะ Save, ช่วงราคา และปุ่มหลักที่ทำได้ในขณะนั้น:

```text
1. งานและลูกค้า
   Customer → Opportunity → Property/Room → Work Type → Template

2. ขนาดหน้างาน
   Measurement Lines → Unit → Confidence → Validation

3. วัสดุและเงื่อนไข
   Grade/Options → Complexity Checklist → Assumptions/Exclusions

4. รูปและช่วงราคา
   Evidence Checklist → Calculate → Price Range → Risk/Validity/Tax Notice

5. ตรวจและส่งต่อ
   Blocked / PendingReview / Shareable → Review → Share → Follow-up/Convert
```

UX Rules:

- Autosave หลัง Field สำคัญเปลี่ยน โดยแสดง Saved/Offline/Conflict อย่างชัดเจน
- Numeric Input ใช้หน่วยติดกับ Field และเปิดแป้นตัวเลขที่เหมาะสม
- Measurement Line เพิ่ม/คัดลอก/ลบได้ และ Undo การลบได้ก่อน Save
- รูปถ่ายแสดง Checklist ว่าขาดรูปประเภทใด ไม่บังคับจำชื่อไฟล์
- Calculate ทำที่ Server เท่านั้น Client แสดง Preview Input แต่ไม่ตัดสินราคาเอง
- ปุ่ม Share ไม่แสดงเป็น Action หลักเมื่อ Policy เป็น Blocked/PendingReview
- ทุก Error ผูกกับ Field หรือ Step และมี Summary สำหรับกลับไปแก้
- Touch Target ขั้นต่ำ 44px, รองรับ Keyboard, Screen Reader และ Reduced Motion

## Wireframe Deliverables

สร้าง:

- `docs/01-business/quick-estimate-mobile-wireframe.md` เป็นคำอธิบาย Screen/State/Interaction หลัก
- `docs/portal/assets/quick-estimate-mobile-wireframe.svg` เป็นภาพ Low-fidelity แบบหลายหน้าจอพร้อม Accessible Description ใน Markdown

Wireframe ต้องแสดงอย่างน้อย:

- Happy Path ทั้ง 5 ขั้น
- Autosave/Offline State
- Field Validation
- Blocked และ PendingReview
- Share Confirmation
- Empty/Loading/Error State ที่มีผลต่อการตัดสินใจ

## API Boundary

ใช้ Resource URL สำหรับอ่านข้อมูล และ Command Endpoint สำหรับ Action ที่เปลี่ยน State:

```text
POST   /api/v1/quick-estimates
GET    /api/v1/quick-estimates/{id}
PATCH  /api/v1/quick-estimates/{id}/draft
POST   /api/v1/quick-estimates/{id}/calculate
POST   /api/v1/quick-estimates/{id}/submit-review
POST   /api/v1/quick-estimates/{id}/review-decisions
POST   /api/v1/quick-estimates/{id}/shares
POST   /api/v1/quick-estimates/{id}/conversion
GET    /api/v1/pricing-templates/effective
POST   /api/v1/files/upload-sessions
```

Contract Rules:

- ทุก Write ใช้ Firebase ID Token และ Backend RBAC/Resource Scope
- `PATCH draft` ใช้ `If-Match`/Concurrency Token ป้องกัน Lost Update
- Calculate คืน Calculation Snapshot และ Share Decision จาก Server
- Share/Conversion ใช้ `Idempotency-Key`
- Error ใช้ RFC 9457 Problem Details, Stable `code`, `traceId` และ Field `errors`
- ภาษา Error ใช้ `Accept-Language`; Business Logic ห้าม Parse ข้อความ
- Resource นอก Scope คืน 404; ไม่มี Permission ใน Resource ที่มองเห็นได้คืน 403

## Data Boundary

### Relational Core

เก็บเป็น Typed Table/Column:

- Quick Estimate Identity, Organization, Branch, Customer, Opportunity และ Owner
- Lifecycle Status, Current Version, Share Decision และ Validity
- Template/Rate Version Reference
- Measurement Line Identity, Work Type, Quantity, Unit และลำดับ
- Evidence Metadata และ File Reference
- Review Decision, Share Attempt, Conversion Link และ Audit Event
- Money/Currency, Effective Period, Concurrency Token และ UTC Timestamp

ข้อมูลเหล่านี้ต้องใช้ Constraint, Foreign Key, Index, Scope Filter หรือ Reporting จึงไม่เก็บเป็น JSONB อย่างเดียว

### Versioned JSONB

ใช้เฉพาะ:

- `template_config`: Field Definition, Option, Checklist และ Rule Graph ของ Template Version
- `input_snapshot`: Input/Option/Assumption/Exclusion ตาม Schema Version ณ ตอนคำนวณ
- `calculation_snapshot`: Rate/Factor/Modifier/Formula Result ก่อน–หลังปัด
- `customer_summary_snapshot`: ข้อมูลที่แสดง/ส่งให้ลูกค้าใน Version นั้น

ทุก JSONB Payload ต้องมี `schemaVersion`, ผ่าน Application Validation ก่อนเขียน และ Published Snapshot เป็น Immutable ห้าม Query ข้ามองค์กรโดยพึ่ง JSONB Field

## Data Flow

```text
Mobile Wizard
  → Autosave Typed Draft + Flexible Inputs
  → Server loads Effective Template/Rate Versions
  → Server validates Unit, Scope, Evidence and Rules
  → Server calculates immutable Snapshot
  → Share Policy returns Blocked/PendingReview/Shareable
  → Approved Share creates Customer Summary Version
  → Convert creates Official Estimate Draft with Source Version reference
```

## Security and Failure Design

- Firebase ยืนยัน Identity เท่านั้น; PostgreSQL เป็นเจ้าของ Membership, Permission และ Scope
- Backend ตรวจ Permission + Organization/Branch/Opportunity/Own Scope ทุก Endpoint
- Reviewer ห้ามตัดสินรายการตนเองเมื่อ Maker–Checker มีผล
- Upload สำเร็จเมื่อ Backend ยืนยัน File Metadata; Local Preview ไม่ถือเป็น Evidence ที่บันทึกแล้ว
- Offline เก็บ Draft ฝั่งอุปกรณ์ได้ชั่วคราว แต่ห้าม Calculate/Review/Share แบบ Offline
- Version Conflict ไม่ Merge ราคาอัตโนมัติ ให้ผู้ใช้ Reload/Compare และเลือกเก็บค่าที่ถูกต้อง
- Retry Share/Conversion ด้วย Idempotency Key เดิมต้องคืนผลเดิม

## Acceptance Criteria

- Wireframe ครบ 5 ขั้นและ State สำคัญ อ่านได้ทั้งภาพและข้อความ
- ทุก Field สำคัญจาก Template Catalog มีตำแหน่งใน Wizard หรือระบุว่า Derived
- API Request/Response Example เชื่อมกับ Wireframe Action ได้ทุกขั้น
- Data Entity/Field Mapping เชื่อมกับ API และแยก Relational/JSONB ชัดเจน
- RBAC, Scope, Maker–Checker, Error Localization, Concurrency และ Idempotency มี Test Case
- เอกสารไม่ใส่ราคาจริงหรือ Threshold ที่ยังไม่ผ่าน Pilot
- Portal/README เชื่อมไปยังเอกสารใหม่โดยไม่ทำลาย JSON Flow Schema

## เอกสารหลักที่จะสร้างหลังอนุมัติ Spec

1. `docs/01-business/quick-estimate-mobile-wireframe.md`
2. `docs/portal/assets/quick-estimate-mobile-wireframe.svg`
3. `docs/03-contracts/quick-estimate-api-contract.md`
4. `docs/04-data/quick-estimate-data-contract.md`
5. Cross-links ใน Documentation Map, Quick Estimate Flow, API/Data Standards และ Portal
