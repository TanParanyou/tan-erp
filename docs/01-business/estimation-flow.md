# Estimation Flow (กระบวนการประเมินราคา)

**สถานะ:** Accepted Direction — Production Baseline ที่ต้อง UAT ด้วยเคสจริงก่อนเริ่มพัฒนา

เอกสารนี้เป็น Flow หลักของ Official Estimate หน้าจออยู่ที่ [Responsive Wireframe](official-estimate-responsive-wireframe.md), Field อยู่ที่ [Field Catalog](official-estimate-field-catalog.md), สูตรอยู่ที่ [Calculation Rules](estimation-calculation-rules.md) และกฎอนุมัติอยู่ที่ [Approval Matrix](approval-matrix.md)

## เป้าหมาย

เปลี่ยน Customer, Opportunity และ Site Survey ให้เป็นประมาณการที่ตรวจสอบต้นทุน คำนวณซ้ำ อนุมัติ และออก Quotation ได้ โดยไม่ลบประวัติ Revision และไม่บังคับผ่าน Quick Estimate

## End-to-End Flow

```text
Customer + Opportunity + Site Survey (optional at Draft)
  → Create Official Estimate Draft
  → Structure Sections and Work Items
  → Add Cost Components and Sources
  → Apply Calculation/Tax Policy
  → Calculate and review Readiness
  → Resolve and freeze Approval Route
  → Submit → Approve or Return
  → Issue Quotation from Approved Revision
```

Quick Estimate เป็น Optional Source ในอนาคต หาก Convert ต้องใช้ Snapshot/Idempotency และคำนวณ Official Estimate ใหม่เสมอ

## Canonical Revision State

```text
Draft ──submit──> Submitted ──approve──> Approved ──issue quotation──> Quoted
  ▲                    │
  └──────return────────┘

Draft/Returned/Submitted ──cancel──> Cancelled
Approved/Quoted ──new revision──> Draft (Revision ถัดไป)
```

| State | ความหมาย | Editable |
| --- | --- | --- |
| `draft` | กำลังจัด Scope/Cost/Price | ได้ตาม Permission/Scope |
| `submitted` | Route ถูก Freeze และรอตรวจ | ไม่ได้สำหรับ Maker |
| `returned` | ถูกส่งกลับให้แก้พร้อมเหตุผล | ได้ตาม Permission/Scope |
| `approved` | ผ่าน Route และ Snapshot ถูกตรึง | ไม่ได้ |
| `quoted` | มี Quotation อ้าง Revision นี้แล้ว | ไม่ได้ |
| `cancelled` | ยุติ Revision พร้อมเหตุผล | ไม่ได้ |

`Ready for Review` เป็น Readiness Result และ `In Review` เป็น Derived UI State จาก Approval Step ไม่ใช่ Revision Status ส่วน `Accepted/Expired` เป็นผลของ Quotation ไม่ใช้ควบคุมการแก้ Revision

## Action Matrix

| Action | Allowed State | Permission | Policy/Guard | Result |
| --- | --- | --- | --- | --- |
| Create Draft | — | `estimates.create` | Customer/Opportunity/Branch Scope | Draft Rev 1 |
| Patch Draft | Draft/Returned | `estimates.update` | ETag; Financial Change ทำ Calculation Outdated | State เดิม + ETag ใหม่ |
| Calculate | Draft/Returned | `estimates.update` | Field/Unit/Policy ครบ; Idempotency | Calculation Snapshot ใหม่ |
| Submit | Draft/Returned | `estimates.submit` | Latest Calculation + Route Resolve | Submitted |
| Return | Submitted | `estimates.approve` | Active Step + Authority + Reason | Returned |
| Approve | Submitted | `estimates.approve` | Active Step + Authority + Maker–Checker | Approved |
| Cancel | Draft/Returned/Submitted | `estimates.cancel` | Reason; Submitted ต้องมี Cancel Authority | Cancelled |
| New Revision | Approved/Quoted | `estimates.update` | Change Reason + Idempotency | Draft Revision ถัดไป |
| Issue Quotation | Approved | `quotations.issue` | Customer Snapshot ครบ + Idempotency | Quoted/Quotation Draft |

ทุก Action ตรวจ Organization/Branch/Resource Scope ที่ Backend และเขียน Audit Event ตาม [Audit and Revision](../04-data/audit-and-revision.md)

## Standard Working Sequence

1. สร้าง Draft จาก Customer + Opportunity และเลือก Site Survey เมื่อมี
2. แบ่ง Section ตามพื้นที่หรือหมวด เช่น Built-in, Electrical, Curtain
3. เพิ่ม Work Item จาก Item Master หรือ Custom Work Item พร้อมเหตุผล
4. ระบุ Quantity/Unit และ Material/Labor/Subcontract/Service/Other Cost
5. ระบบ Resolve Calculation/Tax Policy แล้วคำนวณ Total/Margin/Markup
6. แก้ Blocking Error และตรวจ Provisional/Stale/Override/Risk Trigger
7. ระบบ Resolve Approval Route แล้ว Submit ด้วย Calculation Version ล่าสุด
8. Reviewer ตรวจ Revision Diff, Financial Snapshot และ Exception แล้ว Approve หรือ Return
9. ออก Quotation จาก Approved Revision เท่านั้น
10. การเปลี่ยน Scope/Quantity/Cost/Price/Discount/Tax หลังอนุมัติต้องสร้าง Revision ใหม่

## Invariants

- Money/Rate/Quantity ใช้ Decimal ตาม [Calculation Rules](estimation-calculation-rules.md)
- Client เขียน Derived Total, Margin, Tax หรือ Approval State ไม่ได้
- Missing Cost/Policy/Unit เป็น Blocking Error
- Provisional Cost และ Override ต้องมีเหตุผลและ Approval Trigger
- Submit ใช้ Calculation ล่าสุดและ Freeze Approval Route
- Maker/Last Financial Editor ห้ามเป็น Final Approver
- Approved/Quoted Revision เป็น Immutable Business Record
- Quotation หนึ่งฉบับอ้าง Estimate Revision เดียว
- Retry Command ใช้ Idempotency Key เดิมและต้องไม่สร้าง Record ซ้ำ

## ตัวอย่างสั้น

`TEST_ONLY`: ตู้ Built-in มี Material, Labor และ Subcontract Cost ผู้ประเมิน Calculate แล้วพบ Margin ต่ำกว่า Demo Policy ระบบยังคำนวณได้แต่คืน `requiresAttention` และเพิ่ม Financial Approver หากผู้ตรวจ Return เพราะ Labor Cost ไม่ครบ ผู้ประเมินแก้รายการ Calculate ใหม่ และ Submit Route ใหม่โดย Revision Number เดิมจนกว่าจะ Approved

## Validation Workshop ก่อน Production

ต้องนำ UAT Scenario ไปยืนยัน Default Margin/Markup ต่อประเภทงาน, Overhead, Tax Display, Cost Staleness, Discount Authority, Approval Candidate และ Customer-facing Field หากยังไม่ยืนยันให้ใช้ Production Bootstrap ที่บังคับ Independent Checker ทุก Estimate
