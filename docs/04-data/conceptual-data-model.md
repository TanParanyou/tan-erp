# Conceptual Data Model (แบบจำลองข้อมูลเชิงแนวคิด)

**สถานะ:** Draft — แสดง Ownership และความสัมพันธ์ ยังไม่ใช่ Database Schema

```text
Organization ─┬─ Branch
              ├─ Membership ─ Role ─ Permission
              ├─ Customer ─┬─ Contact
              │            ├─ Address
              │            ├─ Site
              │            └─ Opportunity ─ Stage History
              ├─ Item Category
              ├─ Unit ─ Unit Conversion
              ├─ Cost Source
              ├─ Item ─┬─ Item Unit Conversion
              │        └─ Cost Record ─ Cost Review
              ├─ Calculation Policy Version
              ├─ Tax Policy Version
              ├─ Approval Policy Version ─ Approval Authority
              ├─ Pricing Template ─ Pricing Template Version
              │                         ├─ Measurement Rule
              │                         └─ Reference Rate Source
              └─ Site Survey ─ Site Survey Revision
                                  ├─ Area ─ Measurement
                                  ├─ Checklist Result
                                  └─ Evidence Reference

Pricing Template Version ─► Quick Estimate Version
Quick Estimate ─► Quick Estimate Version ─┬─ Calculation Snapshot
                                         ├─ Preliminary Summary
                                         └─ snapshot convert ─► Estimate (Official Estimate Draft)
                                                         ├─ Estimate Revision
                                                         │    ├─ Section
                                                         │    │    └─ Work Item ─ Cost Components
                                                         │    ├─ Calculation Snapshot
                                                         │    └─ Approval Route ─ Approval Decision
                                                         └─ Quotation ─ Customer Acceptance
                                                                              │
                                                                              ▼
                                                                           Project

Ready Site Survey Revision ─► Quick Estimate หรือ Estimate (Official Estimate Draft)
```

Logical Field, Constraint, JSONB Boundary และ Index ของ Quick Estimate อยู่ที่ [Quick Estimate Data Contract](quick-estimate-data-contract.md)

Logical Schema ของ Official Estimate/BOQ, Approval และ Quotation Link อยู่ที่ [Official Estimate Data Contract](official-estimate-data-contract.md)

Logical Schema ของ Item, Unit, Cost Source และ Versioned Cost Record อยู่ที่ [Item Master Data Contract](item-master-data-contract.md)

Logical Schema ของ Customer, Opportunity, Site และ Site Survey Revision อยู่ที่ [CRM and Site Survey Data Contract](crm-site-survey-data-contract.md)

## Aggregate Candidates

| Aggregate | Boundary/Invariants สำคัญ |
| --- | --- |
| Organization | Membership และ Business Setting อยู่ใน Organization เดียวกัน |
| Item | Identity, Type, Category, Capability, Base Unit และ Lifecycle; ไม่มี Current Cost Field |
| Unit/Conversion | Dimension, Precision และ Exact/Item-specific Conversion ที่ไม่มี Cycle |
| Cost Source | หลักฐาน Supplier Quote/Price List/Contract/Historical/Manual |
| Cost Record | Source, Scope, Unit, Currency, Quantity Break, Effective Period และ Approval Lifecycle |
| Customer | Identity/Lifecycle พร้อม Contact, Address และ Site โดยไม่ผูกกับงานขายเดียว |
| Opportunity | Customer/Branch/Owner, Controlled Stage และ Append-only Stage History |
| Site Survey | Identity/Assignment และลำดับ Revision ของ Site/Opportunity |
| Site Survey Revision | Area, Measurement, Checklist, Evidence Manifest และ Immutable Ready Snapshot |
| Pricing Template | Work Type และชุด Version ที่ใช้คำนวณ Price Range |
| Pricing Template Version | Measurement Rule, Rate, Factor, Range, Defaults และ Effective Period ที่เผยแพร่แล้ว |
| Quick Estimate | Identity, Ownership และ Lifecycle ของการประเมินหน้างาน |
| Quick Estimate Version | Input, Template snapshot, Price Range, Assumptions/Exclusions และ Customer-safe content ของแต่ละฉบับ |
| Calculation Snapshot | Measurement Input/Unit, Billable Quantity, Rate/Factor/Risk, Tax/Rounding และผลก่อน/หลังปัดที่ใช้คำนวณซ้ำ |
| Preliminary Summary | สิ่งที่แสดงหรือส่งให้ลูกค้าจาก Quick Estimate Version หนึ่งฉบับ |
| Estimate | Revision sequence และ Lifecycle |
| Estimate Revision | Work Items, calculation snapshot และ immutable after approval |
| Calculation Policy | รุ่นกฎ Pricing, Overhead, Discount และ Rounding ของ Official Estimate |
| Approval Policy | รุ่น Trigger, Authority และ Route ของ Official Estimate |
| Quotation | อ้างอิง Approved Revision และ document lifecycle |
| Project | Baseline, scope change และ project lifecycle |

## Ownership Rules

- CRM เป็นเจ้าของ Customer/Contact/Address/Site/Opportunity; Module อื่นอ้างด้วย ID และ Snapshot เท่าที่จำเป็น
- Site Survey เป็นเจ้าของ Survey/Revision/Area/Measurement/Checklist/Evidence Reference; Estimate อ้าง Ready Revision ID/Hash และไม่แก้ Source
- Item Master เป็นเจ้าของ Item/Unit/Cost Source/Cost Record แต่ Estimate Revision เก็บ Cost Record, Conversion และ Policy ที่ใช้จริงเป็น Snapshot
- Published Cost Record แก้ย้อนหลังไม่ได้; Branch Override ชนะ Organization Default ตาม Versioned Cost Resolution Policy
- Pricing Template Version เป็นต้นทางกฎคำนวณ แต่ Quick Estimate Version ต้องเก็บ Template/Input/Factor/Result Snapshot เพื่อคำนวณย้อนหลังได้
- Published Pricing Template Version เป็น Immutable; Lifecycle คือ Draft, Submitted, Approved, Calibration, Active, Superseded และ Disabled
- Standard Rate เป็นของ Organization; Branch Rate Override ต้องมี Effective Period, Reason และ Approval โดยไม่แก้ Standard Rate เดิม
- Quick Estimate เป็นเจ้าของ Version และ Preliminary Summary; หนึ่ง Version Convert ไป Official Estimate Draft ได้หนึ่งรายการต่อเจตนาที่ระบุ
- Estimation เป็นเจ้าของ Estimate/Revision/Approval; Quotation ไม่ย้อนมาแก้ Revision
- Published Calculation/Tax/Approval Policy Version เป็น Immutable และ Estimate Snapshot ต้องอ้าง Version ที่ใช้จริง
- Project รับ Baseline จาก Commercial แล้วเปลี่ยนผ่าน Change Order
- Audit Record อ้าง Resource แต่ไม่เป็นเจ้าของ Business State

## สิ่งที่ยังไม่สรุป

ความสัมพันธ์ Contact ข้าม Customer, Customer Merge, BOQ/BOM และ Document Numbering ขั้นสูงต้องผ่าน Workshop ก่อนขยาย Schema ค่า Survey Template/Readiness, Retention/Redaction, Cost Source Priority/Staleness/Exception และ Tax/Approval Threshold จริงต้องผ่าน Owner ที่เกี่ยวข้องก่อน Production
