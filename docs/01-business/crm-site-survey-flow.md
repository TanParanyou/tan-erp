# CRM and Site Survey Flow (กระบวนการลูกค้า งานขาย และสำรวจหน้างาน)

**สถานะ:** Accepted Direction — Development Baseline

เอกสารนี้เป็น Flow หลักตั้งแต่รับลูกค้าจนข้อมูลหน้างานพร้อมสร้าง Official Estimate รายการ Field อยู่ที่ [CRM and Site Survey Field Catalog](crm-site-survey-field-catalog.md) และกฎควบคุมอยู่ที่ [CRM and Site Survey Governance](crm-site-survey-governance.md)

## เป้าหมาย

ทำให้ทีมขายและทีมสำรวจใช้ Customer, Opportunity, Site และ Site Survey ที่สัมพันธ์กันอย่างชัดเจน พร้อมตรึง Revision ที่ส่งต่อให้ Estimation โดยไม่บังคับผ่าน Quick Estimate

## End-to-End Sequence

```text
Customer + Contact
  → Opportunity + Owner + Branch
  → Site + Survey Appointment
  → Survey Draft Revision
  → Areas + Measurements + Checklist + Evidence
  → Validate Readiness
  → Mark Revision Ready (immutable)
  → Create Official Estimate Draft
```

## Customer Lifecycle

```text
Draft → Active → Inactive
          ▲          │
          └──────────┘ Reactivate after validation
```

- Draft เก็บข้อมูลขั้นต่ำระหว่างรับสาย/ติดต่อได้ แต่สร้าง Opportunity ไม่ได้จน Active
- Active ใช้สร้าง Opportunity/Site ใหม่ได้
- Inactive ห้ามสร้างธุรกรรมใหม่ แต่ประวัติเดิมยังอ่านได้
- Customer ที่ถูกอ้างห้าม Hard Delete; Duplicate ใช้ Review ไม่ Merge อัตโนมัติ

## Opportunity Lifecycle

```text
Draft → Qualified → Surveying → Estimating → Proposed → Won
  └──────────────→ Cancelled       └──────────────→ Lost
```

- `Qualified` ต้องมี Active Customer, Branch, Owner, Scope Summary และ Next Action
- `Surveying` ต้องมี Site และ Survey Assignment/Appointment
- `Estimating` ต้องมี Ready Survey Revision หรือ Reason ที่ Policy อนุญาตให้เริ่มโดยไม่มี Survey
- `Proposed` เป็นผลจาก Commercial/Quotation Use Case ไม่ตั้งเองจากหน้าจอ CRM
- `Won/Lost/Cancelled` บังคับ Outcome Reason; Closed Stage เปิดใหม่ได้เฉพาะผู้มีสิทธิ์และมี Audit

Stage แสดงความคืบหน้างานขาย ไม่เป็น Approval State และไม่ควบคุม Estimate Lifecycle แทน Estimation

## Site Survey Revision Lifecycle

```text
Survey Identity
  └─ Revision 1: Draft → Ready → Superseded
                       └──────→ Void
           clone ─────────────→ Revision 2: Draft → Ready
```

- Draft แก้ Area, Measurement, Checklist, Note และ Evidence Manifest ได้ด้วย ETag
- Ready ต้องผ่าน Readiness Gate และเป็น Immutable
- วัดเพิ่มหรือแก้สาระสำคัญให้ Clone Ready Revision เป็น Draft ใหม่
- Ready Revision ล็อก Business Content; Lifecycle Metadata เปลี่ยนเป็น Superseded/Void ได้ผ่าน Use Case เท่านั้น
- Ready Revision ใหม่ทำรุ่นเก่าเป็น Superseded สำหรับการเลือกใหม่ แต่ Estimate เดิมยังอ้างรุ่นเดิม
- Void บังคับ Reason; Revision ที่ Estimate อ้างยังเก็บเป็นประวัติและแสดงสถานะ Void ชัดเจน

## Readiness Gate

ขั้นต่ำก่อน Ready:

1. Customer, Opportunity, Branch, Site และ Surveyor อยู่ Scope เดียวกัน
2. Visit date/time และ Survey Template Version ครบ
3. มีอย่างน้อยหนึ่ง Area และ Scope Summary
4. Required Measurement/Checklist ตาม Work Type ผ่าน
5. Evidence ที่ Template บังคับ Upload สำเร็จและมี File Reference
6. Assumption, Constraint และ Missing Detail ถูกระบุ
7. ไม่มี Validation Error หรือ Upload ที่ยังไม่สำเร็จ

Readiness หมายถึง “พร้อมเป็นข้อมูลต้นทางของ Estimate” ไม่ได้หมายถึงราคาได้รับอนุมัติ

## Action Matrix

| Action | State | Permission | Guard | Result |
| --- | --- | --- | --- | --- |
| Create/Activate Customer | —/Draft | `customers.create`, `customers.activate` | Organization Scope + Field Gate | Active Customer |
| Create Opportunity | Active Customer | `opportunities.create` | Branch/Owner Scope | Draft Opportunity |
| Transition Opportunity | Current Stage | `opportunities.transition` | Allowed Transition + Required Field | New Stage + History |
| Create Survey | Opportunity | `surveys.create` | Site/Assignment Scope | Survey + Draft Revision 1 |
| Save Survey Draft | Draft Revision | `surveys.update` | Own/Branch Scope + ETag | New ETag |
| Mark Ready | Draft Revision | `surveys.mark-ready` | Readiness Gate | Immutable Ready Revision |
| Clone Revision | Ready/Superseded | `surveys.create-revision` | Reason + Idempotency | New Draft Revision |
| Void Revision | Draft/Ready | `surveys.void` | Authority + Reason | Void + Audit |
| Create Estimate | Ready Revision | `estimates.create` | Cross-resource Scope | Official Estimate Draft |

## ตัวอย่างสั้น

`TEST_ONLY`: Customer “บริษัท ตัวอย่าง จำกัด” มี Opportunity “Built-in ห้องนอนใหญ่” และ Site “คอนโดตัวอย่าง” Survey Revision 1 มี Area ห้องนอน, Measurement หน่วยเมตร, Checklist และรูปครบ เมื่อ Mark Ready แล้วแก้ไม่ได้ หากกลับไปวัดใหม่ให้สร้าง Revision 2 ส่วน Estimate เดิมยังอ้าง Revision 1

## เอกสารที่เกี่ยวข้อง

- [Field Catalog](crm-site-survey-field-catalog.md)
- [Governance](crm-site-survey-governance.md)
- [Responsive Text Wireframe](crm-site-survey-responsive-wireframe.md)
- [API Contract](../03-contracts/crm-site-survey-api-contract.md)
- [Data Contract](../04-data/crm-site-survey-data-contract.md)
