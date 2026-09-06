# Conceptual Data Model (แบบจำลองข้อมูลเชิงแนวคิด)

**สถานะ:** Draft — แสดง Ownership และความสัมพันธ์ ยังไม่ใช่ Database Schema

```text
Organization ─┬─ Branch
              ├─ Membership ─ Role ─ Permission
              ├─ Customer ─ Contact
              ├─ Item Master ─ Reference Cost
              ├─ Calculation Policy Version
              ├─ Tax Policy Version
              ├─ Approval Policy Version ─ Approval Authority
              ├─ Pricing Template ─ Pricing Template Version
              │                         ├─ Measurement Rule
              │                         └─ Reference Rate Source
              └─ Opportunity ─ Site Survey

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

Site Survey ─► Quick Estimate หรือ Estimate (Official Estimate Draft)
```

Logical Field, Constraint, JSONB Boundary และ Index ของ Quick Estimate อยู่ที่ [Quick Estimate Data Contract](quick-estimate-data-contract.md)

Logical Schema ของ Official Estimate/BOQ, Approval และ Quotation Link อยู่ที่ [Official Estimate Data Contract](official-estimate-data-contract.md)

## Aggregate Candidates

| Aggregate | Boundary/Invariants สำคัญ |
| --- | --- |
| Organization | Membership และ Business Setting อยู่ใน Organization เดียวกัน |
| Item | Code/Unit/Status และประวัติต้นทุนอ้างอิง |
| Site Survey | Evidence และ Readiness ก่อน Estimate |
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

- CRM เป็นเจ้าของ Customer/Opportunity; Module อื่นอ้างด้วย ID และ Snapshot เท่าที่จำเป็น
- Item Master เป็นเจ้าของรายการมาตรฐาน แต่ Estimate Revision เก็บค่าที่ใช้คำนวณเป็น Snapshot
- Pricing Template Version เป็นต้นทางกฎคำนวณ แต่ Quick Estimate Version ต้องเก็บ Template/Input/Factor/Result Snapshot เพื่อคำนวณย้อนหลังได้
- Published Pricing Template Version เป็น Immutable; Lifecycle คือ Draft, Submitted, Approved, Calibration, Active, Superseded และ Disabled
- Standard Rate เป็นของ Organization; Branch Rate Override ต้องมี Effective Period, Reason และ Approval โดยไม่แก้ Standard Rate เดิม
- Quick Estimate เป็นเจ้าของ Version และ Preliminary Summary; หนึ่ง Version Convert ไป Official Estimate Draft ได้หนึ่งรายการต่อเจตนาที่ระบุ
- Estimation เป็นเจ้าของ Estimate/Revision/Approval; Quotation ไม่ย้อนมาแก้ Revision
- Published Calculation/Tax/Approval Policy Version เป็น Immutable และ Estimate Snapshot ต้องอ้าง Version ที่ใช้จริง
- Project รับ Baseline จาก Commercial แล้วเปลี่ยนผ่าน Change Order
- Audit Record อ้าง Resource แต่ไม่เป็นเจ้าของ Business State

## สิ่งที่ยังไม่สรุป

ความสัมพันธ์ Address/Contact หลายประเภท, Unit Conversion, Cost Source, BOQ/BOM และ Document Numbering ต้องผ่าน Workshop ก่อนออก Logical/Physical Schema ค่า Tax/Approval Threshold จริงต้องผ่าน Business Owner และ Finance Sign-off ก่อน Publish
