# Official Estimate Business Controls Design

**สถานะ:** Approved Design Direction  
**ขอบเขต:** Documentation Foundation เท่านั้น  
**ผู้อ่าน:** เจ้าของกระบวนการประเมินราคา, ฝ่ายบัญชี, ผู้อนุมัติ, UX/UI และทีมพัฒนา

## เป้าหมาย

ทำให้ Official Estimate มี Field, สูตร, สถานะ, การอนุมัติ และสถานการณ์ UAT ที่ครบพอสำหรับเริ่มพัฒนาแบบ Production โดยใช้ค่าเริ่มต้นที่ปลอดภัยเมื่อบริษัทยังไม่มีข้อมูลจริง และยังเปลี่ยนนโยบายภายหลังได้โดยไม่แก้ประวัติเอกสารเดิม

## แนวทางที่เลือก

ใช้ **Configurable Policy + Versioned Snapshot**:

- Field หลักและ Financial Invariant เป็น Typed Contract ที่เปลี่ยนโดย Migration/Version เท่านั้น
- Calculation Policy และ Approval Policy เป็น Configuration ที่มี Version, Effective Period และ Organization/Branch Scope
- การ Calculate และ Approve ต้องบันทึก Snapshot/Hash ของ Policy Version และ Input ที่ใช้
- Published Policy และ Approved/Quoted Estimate Revision แก้ย้อนหลังไม่ได้
- ไม่สร้าง General-purpose Rules Engine ในระยะแรก กฎต้องอยู่ในชนิดที่ระบบรองรับและ Validate ได้

แนวทางนี้สมดุลกว่าการ Hard-code ซึ่งเปลี่ยนยาก และ Generic Rules Engine ซึ่งเพิ่มความซับซ้อนและความเสี่ยงเกินความจำเป็น

## หลักความปลอดภัยเมื่อยังไม่มีข้อมูลจริง

Production Bootstrap ใช้ Fail-closed:

- Estimate ทุกฉบับต้องผ่าน Independent Checker อย่างน้อยหนึ่งคน
- Maker อนุมัติงานของตนเองไม่ได้
- ไม่มี Auto-approval จนกว่า Organization จะ Publish Approval Policy ที่ผ่านการยืนยัน
- Cost ที่ไม่มีแหล่งอ้างอิงใช้ได้เฉพาะ Provisional Cost พร้อมเหตุผลและ Approval Trigger
- Missing Cost, Calculation Outdated หรือ Invalid Unit เป็น Blocking Error
- Threshold ตัวเลขที่ใช้ในเอกสารตัวอย่างต้องติด `TEST_ONLY` และไม่สามารถ Activate เป็น Production Policy อัตโนมัติ

## เอกสารอ้างอิงหลักที่จะสร้างหรือปรับ

### 1. Official Estimate Field Catalog

สร้าง `docs/01-business/official-estimate-field-catalog.md` เป็น Reference เดียวของ Field ระดับ Business โดยแต่ละ Field ต้องระบุ:

- Canonical Name และคำอธิบายไทย
- Scope: Estimate, Revision, Section, Work Item, Cost Component, Adjustment หรือ Tax Line
- Data Type, Precision, Unit/Currency และรูปแบบที่แสดง
- Requiredness ตามสถานะ: Save Draft, Calculate, Submit, Approve และ Quote
- Default Source และสิทธิ์ Override
- Validation/Blocking Code
- Internal/Customer Visibility
- Audit และ Revision Behavior
- ตัวอย่างสั้นที่ติด `TEST_ONLY` เมื่อเป็นตัวเลขสมมติ

Field Groups ขั้นต่ำ:

1. Document Context: Organization, Branch, Customer, Opportunity, Site Survey, Owner, Currency
2. Scope Structure: Section Code/Name/Order และ Work Item Code/Description/Quantity/Unit
3. Cost: Material, Labor, Subcontract, Service, Other Direct Cost, Cost Source และ Effective Date
4. Pricing: Pricing Method, Margin/Markup Target, Overhead, Adjustment และ Discount
5. Tax/Commercial: Tax Policy, Validity, Payment/Delivery Note และ Customer-facing Description
6. Control: Status, Revision, Calculation Version, Policy Version, ETag และ Audit Reason

ข้อมูล Cost, Margin, Markup, Internal Note และ Approval Threshold เป็น Internal-only โดยค่าเริ่มต้น ห้ามหลุดไป Customer Contract

### 2. Calculation Policy

ปรับ `docs/01-business/estimation-calculation-rules.md` ให้เป็น Authoritative Calculation Reference

Baseline Phase 1:

- Currency เดียวต่อ Revision และเริ่มด้วย THB
- Quantity ใช้ Decimal 4 ตำแหน่ง
- Rate/Unit Cost ใช้ Decimal 4 ตำแหน่ง
- Money จัดเก็บ/แสดง 2 ตำแหน่ง
- Margin/Markup/Tax Rate ใช้ Decimal 6 ตำแหน่งภายในและแสดงตาม UI Policy
- Pricing Method หลักเป็น Margin โดยกำหนด Default แยกตาม Work Type; ผู้มีสิทธิ์อาจเลือก Markup ตาม Policy
- Overhead รองรับ Percent of Direct Cost หรือ Fixed Amount แต่หนึ่ง Revision ใช้ Policy Version ที่ชัดเจน
- Discount Phase 1 ใช้ระดับ Document; Line/Section Discount เป็น Future Capability
- Tax คำนวณหลัง Discount ตาม Versioned Tax Policy
- คำนวณ Intermediate ด้วย Precision เต็ม, ปัด Money ระดับ Work Item ด้วย Midpoint Away From Zero, รวมเป็น Section/Document และคำนวณ Tax ที่ระดับ Document
- Customer Total ต้องเท่ากับผลรวมค่าที่แสดง ห้ามมี Hidden Rounding Difference

ทุก Calculate บันทึก Input, Cost Source Version, Policy Version, Intermediate, Rounded Result และ Hash เพื่อคำนวณซ้ำและตรวจสอบได้

### 3. Approval Policy

ปรับ `docs/01-business/approval-matrix.md` ให้แยก Permission ออกจาก Business Authority:

- Permission บอกว่า User ทำ Action ประเภทใดได้
- Approval Policy บอกว่า User อนุมัติ Estimate ฉบับใดได้ภายใต้ Amount/Margin/Discount/Exception/Scope
- Backend ต้องตรวจทั้งสองส่วนทุกครั้ง

Trigger ขั้นต่ำ:

- Grand Total เกิน Authority Limit
- Margin ต่ำกว่า Minimum Margin
- Discount สูงกว่า Maximum Discount
- Manual Cost/Price/Tax Override
- Provisional หรือ Stale Cost
- Custom Work Item ไม่มี Item Master
- Sensitive Project หรือ Missing Required Evidence

ผล Policy มี `blocked`, `requiresApproval` หรือ `eligibleForApproval` พร้อม Stable Reason Code เสมอ ระบบเลือก Route ที่เข้มที่สุดเมื่อมีหลาย Trigger และ Freeze Route/Threshold Snapshot ตอน Submit

Production Bootstrap ไม่มี Threshold เชิงตัวเลขและบังคับ Independent Checker ทุกฉบับ ส่วน Demo/UAT อาจใช้ Profile `TEST_ONLY-TH-EST-V1` ที่ไม่สามารถ Publish โดยไม่ผ่าน Business Owner + Finance Approval

### 4. Status and Action Matrix

ปรับ `docs/01-business/estimation-flow.md` โดยใช้ Revision Status เป็น Canonical State:

```text
Draft ──submit──> Submitted ──approve──> Approved ──issue quotation──> Quoted
  ▲                    │
  └──────return────────┘

Approved/Quoted ──new revision──> New Draft Revision
Draft/Submitted/Returned ──cancel──> Cancelled
```

- `Returned` เป็น Editable Work Queue State ของ Revision
- `In Review` เป็น Derived UI State จาก Approval Request/Step ไม่ใช่ Revision Status เพิ่มอีกค่า
- `Ready for Review` เป็น Readiness Result ไม่ใช่ Persisted Status
- `Accepted` และ `Expired` เป็น Commercial/Quotation Outcome ไม่ใช้ควบคุมการแก้ Revision
- Approved/Quoted Revision เป็น Immutable และการเปลี่ยน Financial/Scope Field ต้องสร้าง Revision ใหม่

Action Matrix ต้องระบุ Permission, Policy Gate, Allowed State, Resulting State, Idempotency, Required Reason และ Audit Event ของ Create, Patch, Calculate, Submit, Return, Approve, Cancel, New Revision และ Issue Quotation

### 5. UAT Scenarios

สร้าง `docs/05-engineering/official-estimate-uat-scenarios.md` เป็น How-to สำหรับ Workshop/UAT โดยใช้ตัวอย่างงาน Built-in ที่ติด `TEST_ONLY`

Scenario ขั้นต่ำ:

1. สร้าง Draft จาก Customer/Opportunity/Site Survey โดยไม่ผ่าน Quick Estimate
2. เพิ่ม Section, Work Item และ Material/Labor Cost ครบแล้ว Calculate สำเร็จ
3. Missing Cost หรือ Unit ผิดต้อง Calculate/Submit ไม่ได้ตามระดับ Error
4. แก้ Financial Input หลัง Calculate ทำ Snapshot Outdated
5. Provisional Cost ต้องมีเหตุผลและ Trigger Approval
6. Margin ต่ำ/Discount สูงเลือก Approval Route ที่เข้มที่สุด
7. Maker พยายาม Approve งานตนเองแล้วถูกปฏิเสธ
8. Checker Return พร้อม Reason แล้ว Maker แก้และ Submit ใหม่
9. Approved Revision แก้ไม่ได้
10. สร้าง Revision ใหม่จาก Approved พร้อม Change Reason
11. ออก Quotation จาก Approved Revision แบบ Idempotent
12. ผู้ใช้นอก Organization/Branch Scope อ่านหรือแก้ไม่ได้
13. Customer-facing Output ไม่มี Internal Cost/Margin/Approval Data
14. เปลี่ยน Policy Version แล้ว Revision เก่ายังคำนวณ/ตรวจสอบจาก Snapshot เดิมได้

แต่ละ Scenario ต้องมี Role, Preconditions, Test Data, Steps, Expected Result, Audit Evidence และช่อง Business Sign-off

### 6. Domain Language and ADR

เพิ่มศัพท์ต่อไปนี้ใน `CONTEXT.md` โดยไม่ใส่รายละเอียด Implementation:

- Calculation Policy (นโยบายคำนวณ)
- Approval Policy (นโยบายอนุมัติ)
- Provisional Cost (ต้นทุนชั่วคราว)
- Calculation Snapshot (ภาพบันทึกการคำนวณ)
- Approval Route (เส้นทางอนุมัติ)

สร้าง ADR เรื่อง Versioned Business Policies and Frozen Snapshots เพราะเป็นการตัดสินใจที่เปลี่ยนภายหลังมีต้นทุนสูงและมีทางเลือกจริงระหว่าง Hard-code, Typed Config และ Generic Rules Engine

## Contract Alignment

หลังปรับเอกสารหลัก ต้องทำให้เอกสารต่อไปนี้สอดคล้องกันโดยไม่คัดลอกกฎซ้ำ:

- Requirements Catalog อ้าง Field/Calculation/Approval/UAT Requirements
- Official Estimate API Contract ใช้ Status, Reason Code และ Required Field Gates เดียวกัน
- Error Contract เพิ่มเฉพาะ Stable Code ที่ Field/Policy ต้องใช้
- Permission Catalog แยก Action Permission กับ Approval Authority ชัดเจน
- Official Estimate Data Contract อ้าง Policy Version/Snapshot และ Precision Baseline
- Documentation Map และ Portal Map ชี้ไป Authoritative Document
- JSON Flow ใช้ Status/Transition เดียวกับ Estimation Flow และมี Text Alternative

## Out of Scope

- Application Code, Database Migration และ OpenAPI Generation
- General-purpose Expression Language หรือ User-written Script
- Multi-currency ใน Revision เดียว
- Line/Section Discount และ Compound Tax หลายชั้น
- Accounting Posting, Invoice, Payment และ Withholding Tax Workflow
- การ Activate Threshold จริงโดยไม่มี Business Owner/Finance Sign-off

## Acceptance Criteria

- ทุก Field สำคัญรู้ว่าใครกรอก เมื่อใด บังคับเมื่อใด และลูกค้าเห็นหรือไม่
- Formula มีลำดับ, Precision, Rounding และ Snapshot ที่คำนวณซ้ำได้
- Approval ใช้ Fail-closed, Maker–Checker และ Versioned Route
- Status ใน Business Flow, API, Data และ JSON Diagram ใช้ความหมายเดียวกัน
- UAT ครอบคลุม Happy Path, Validation, Security, Concurrency, Revision และ Customer Data Leakage
- ตัวเลขสมมติทุกค่าติด `TEST_ONLY` และไม่ถูกระบุว่าเป็นค่าจริงของบริษัท
- ไม่มี Placeholder เช่น TBD/TODO หรือกฎซ้ำหลายเอกสาร
- Internal Link, Markdown, JSON และ SVG Validation ผ่าน

