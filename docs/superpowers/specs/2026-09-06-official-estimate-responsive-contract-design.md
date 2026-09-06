# Official Estimate Responsive Wireframe and Contract Design

**สถานะ:** Approved Design Direction  
**ขอบเขต:** Documentation Foundation เท่านั้น

## เป้าหมาย

กำหนด Responsive Wireframe, API Contract และ Data Contract ของ Official Estimate/BOQ ให้ผู้ประเมินจัดรายการต้นทุน ตรวจราคาขาย กำไร Revision และ Approval ได้บน Desktop, Tablet และ Mobile โดยไม่ใช้ Quick Estimate เป็นขั้นบังคับ

## Responsive Design

### Desktop — Split Workspace

- Header แสดง Estimate Number, Revision, Customer, Project/Opportunity และ Status
- Financial HUD แสดง Cost, Selling Price, Margin/Markup, Discount, Tax และ Grand Total
- ซ้ายเป็น Section/Work Item BOQ Table สำหรับค้นหา จัดลำดับ และเลือกหลายรายการ
- ขวาเป็น Cost Inspector แสดง Material/Labor/Service Component, Calculation Detail และ Audit Context ของ Work Item ที่เลือก
- Approval/Revision Action อยู่ใน Sticky Command Bar และเปลี่ยนตาม Permission/State

### Tablet — BOQ + Side Sheet

- BOQ ใช้ความกว้างหลักเต็มพื้นที่และลด Column เหลือรายการ, จำนวน, ต้นทุน, ราคาขาย และสถานะ
- Cost Inspector เปิดเป็น Side Sheet ที่ปิดได้และคืน Focus ไป Work Item เดิม
- Financial HUD เป็น 2 แถว ไม่ใช้ Horizontal Scroll สำหรับยอดสำคัญ

### Mobile — Task-focused Flow

- หน้าแรกเป็น Section/Work Item List พร้อม Search/Filter และ Summary ย่อ
- แตะ Work Item เปิดหน้า Detail แยกเป็น Scope → Quantity/Unit → Cost Components → Selling Rule → Result
- Financial Summary และ Primary Action ติดด้านล่าง แต่ไม่บัง Error/Keyboard
- Bulk Edit, Column Resize และ Drag/Reorder ไม่เป็น Primary Mobile Workflow
- Approval ใช้ Review Summary ก่อน Decision และไม่ซ่อน Margin/Exception

ทุกขนาดใช้ Touch Target ขั้นต่ำ 44px, รองรับ Keyboard/Screen Reader, Text Zoom 200%, ไทย/อังกฤษ และ Reduced Motion

## Business Flow

```text
Customer/Opportunity + Site Survey
  → Create Official Estimate Draft
  → Add Sections and Work Items
  → Add Material/Labor/Service/Other Cost Components
  → Calculate Cost, Selling Price, Margin/Markup, Discount and Tax
  → Validate Completeness and Approval Policy
  → Submit → Review/Return/Approve
  → Issue Quotation from Approved Revision
  → Customer Acceptance → Project
```

## State Model

```text
Draft → Submitted → Returned → Draft
          └──────→ Approved → Quoted → Accepted
                              └──────→ Expired/Cancelled

Material change after Approved/Quoted → New Revision → Draft
```

Approved/Quoted Revision เป็น Immutable การเปลี่ยน Scope, Quantity, Cost, Selling Rule, Discount หรือ Tax ต้องสร้าง Revision ใหม่

## API Direction

ใช้ Resource Endpoint สำหรับอ่าน/แก้ Draft และ Command Endpoint สำหรับ Business Transition:

```text
POST   /api/v1/estimates
GET    /api/v1/estimates/{id}
PATCH  /api/v1/estimates/{id}/draft
POST   /api/v1/estimates/{id}/calculate
POST   /api/v1/estimates/{id}/submit
POST   /api/v1/estimates/{id}/review-decisions
POST   /api/v1/estimates/{id}/revisions
POST   /api/v1/estimates/{id}/quotation
```

- Draft Write ใช้ ETag/`If-Match`
- Calculate, Submit, Revision และ Quotation ใช้ Idempotency ตามความเสี่ยง
- Backend เป็นเจ้าของ Calculation, Approval Policy, RBAC และ Resource Scope
- Error ใช้ RFC 9457, Stable Code, Field Errors, Trace ID และ `Accept-Language`

## Data Direction

Relational Core:

- Estimate, Estimate Revision, Section, Work Item และ Cost Component
- Quantity/Unit, Cost/Selling Money, Discount, Tax, Margin/Markup และ Totals
- Approval Request/Decision, Quotation Link, Customer Acceptance และ Audit
- Organization/Branch/Opportunity/Project Scope, Status, Version และ Concurrency Token

JSONB เฉพาะ:

- Versioned Calculation Rule Configuration
- Immutable Calculation Snapshot
- Customer-facing Quotation Snapshot ที่มี Schema Version

ห้ามเก็บ Core BOQ, Permission, Scope, Money Total หรือ Approval State เป็น JSONB อย่างเดียว

## Error and Security

- Firebase ให้ Identity; PostgreSQL เป็นเจ้าของ Membership/Permission/Scope
- Resource นอก Scope คืน 404; ขาด Permission กับ Resource ที่เปิดเผยได้คืน 403
- Maker ห้าม Approve Revision ตนเองเมื่อ Policy บังคับ
- Stale Cost, Margin ต่ำ, Discount สูง, Missing Cost และ Custom Item เป็น Approval Trigger
- Request เก่าห้ามเขียนทับ Draft ใหม่และไม่ Merge Financial Result อัตโนมัติ
- Quotation ต้องอ้าง Approved Revision เท่านั้นและ Retry ด้วย Idempotency Key เดิมต้องคืนผลเดิม

## Deliverables

1. `docs/01-business/official-estimate-responsive-wireframe.md`
2. `docs/portal/assets/official-estimate-responsive-wireframe.svg`
3. `docs/03-contracts/official-estimate-api-contract.md`
4. `docs/04-data/official-estimate-data-contract.md`
5. Cross-links ใน Estimation Flow, API/Data Standards และ Documentation Map

## Acceptance Criteria

- Wireframe แสดง Desktop, Tablet และ Mobile พร้อม Text Alternative
- ทุก Action สำคัญเชื่อมกับ Endpoint และ Permission
- API มี Request/Response, Concurrency, Idempotency, Error และ Contract Cases
- Data Contract ระบุ Entity, Field, Constraint, Index, Revision และ JSONB Boundary
- Approved/Quoted Revision ทำซ้ำได้จาก Snapshot เดิมและแก้ย้อนหลังไม่ได้
- Quick Estimate เป็น Optional Source เท่านั้น Official Estimate สร้างจาก Customer/Opportunity/Site Survey ได้โดยตรง
- ไม่มีราคาจริงหรือ Threshold ที่ยังไม่ผ่าน Business Workshop
