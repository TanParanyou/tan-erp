# Conceptual Data Model (แบบจำลองข้อมูลเชิงแนวคิด)

**สถานะ:** Draft — แสดง Ownership และความสัมพันธ์ ยังไม่ใช่ Database Schema

```text
Organization ─┬─ Branch
              ├─ Membership ─ Role ─ Permission
              ├─ Customer ─ Contact
              ├─ Item Master ─ Reference Cost
              └─ Opportunity ─ Site Survey
                               │
                               ▼
                          Estimate
                               ├─ Estimate Revision
                               │    ├─ Section
                               │    │    └─ Work Item ─ Cost Components
                               │    └─ Approval Decision
                               └─ Quotation ─ Customer Acceptance
                                                   │
                                                   ▼
                                                Project
```

## Aggregate Candidates

| Aggregate | Boundary/Invariants สำคัญ |
| --- | --- |
| Organization | Membership และ Business Setting อยู่ใน Organization เดียวกัน |
| Item | Code/Unit/Status และประวัติต้นทุนอ้างอิง |
| Site Survey | Evidence และ Readiness ก่อน Estimate |
| Estimate | Revision sequence และ Lifecycle |
| Estimate Revision | Work Items, calculation snapshot และ immutable after approval |
| Quotation | อ้างอิง Approved Revision และ document lifecycle |
| Project | Baseline, scope change และ project lifecycle |

## Ownership Rules

- CRM เป็นเจ้าของ Customer/Opportunity; Module อื่นอ้างด้วย ID และ Snapshot เท่าที่จำเป็น
- Item Master เป็นเจ้าของรายการมาตรฐาน แต่ Estimate Revision เก็บค่าที่ใช้คำนวณเป็น Snapshot
- Estimation เป็นเจ้าของ Estimate/Revision/Approval; Quotation ไม่ย้อนมาแก้ Revision
- Project รับ Baseline จาก Commercial แล้วเปลี่ยนผ่าน Change Order
- Audit Record อ้าง Resource แต่ไม่เป็นเจ้าของ Business State

## สิ่งที่ยังไม่สรุป

ความสัมพันธ์ Address/Contact หลายประเภท, Unit Conversion, Cost Source, Tax Model, BOQ/BOM และ Document Numbering ต้องผ่าน Workshop ก่อนออก Logical/Physical Schema
