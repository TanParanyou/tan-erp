# CRM and Site Survey Foundation Design

**สถานะ:** Approved Design Direction
**ขอบเขต:** Documentation Foundation เท่านั้น

## เป้าหมาย

กำหนดข้อมูลต้นทางขั้นต่ำที่ทำให้เริ่มพัฒนา Vertical Slice `Customer → Opportunity → Site Survey → Official Estimate` ได้ โดยรักษาประวัติ ป้องกันข้อมูลข้ามองค์กร และรองรับการเปลี่ยนกระบวนการขาย/แบบสำรวจในอนาคต

## แนวทางที่เลือก

ใช้ **Stable CRM Identities + Versioned Survey Snapshot**:

- Customer เป็นข้อมูลหลักของบุคคลหรือนิติบุคคล ไม่ผูก Lifecycle กับงานขายหนึ่งงาน
- Contact และ Address แยกจาก Customer เพื่อรองรับหลายผู้ติดต่อ/หลายสถานที่
- Opportunity เป็นงานขายหนึ่งเรื่องของ Customer และ Branch มี Owner/Stage/Outcome ของตนเอง
- Site เป็นสถานที่จริงที่ Opportunity ใช้งาน และ Site Survey เป็นกิจกรรมสำรวจที่ Site นั้น
- Site Survey แยก Identity ออกจาก Revision; Draft แก้ได้ แต่ Ready Revision เป็น Immutable
- Official Estimate อ้าง `siteSurveyRevisionId` ที่แน่นอนและเก็บ Source Snapshot ที่จำเป็น
- Firebase ให้ Identity เท่านั้น; PostgreSQL เป็นเจ้าของ Membership, RBAC และข้อมูลธุรกิจ

## Boundaries

- CRM เป็นเจ้าของ Customer, Contact, Address, Opportunity และ Site
- Survey เป็นเจ้าของ Site Survey, Revision, Area/Measurement, Checklist และ Evidence Reference
- File Service เป็นเจ้าของไฟล์จริง; Survey เก็บ File ID, metadata และ hash/reference
- Estimation อ้าง Customer/Opportunity/Survey Revision ด้วย ID และ Snapshot ไม่แก้ข้อมูลต้นทาง
- Quotation/Project เปลี่ยนสถานะ Opportunity ผ่าน Use Case ที่กำหนด ไม่เขียนตาราง CRM โดยตรง

## Lifecycle

```text
Customer: Draft → Active → Inactive

Opportunity: Draft → Qualified → Surveying → Estimating → Proposed
                                                └──────→ Won
                                                └──────→ Lost/Cancelled

Site Survey Revision: Draft → Ready → Superseded
                         └──→ Void
```

- Stage ของ Opportunity เป็น Controlled Transition แต่ชื่อ/Mapping ปรับได้ในอนาคตผ่าน Versioned Sales Policy; Phase แรกไม่สร้าง Workflow Builder
- Ready Survey Revision ล็อก Field, Measurement, Checklist และ Evidence Manifest
- หากกลับไปวัดใหม่ ให้ Clone เป็น Draft Revision ใหม่; Estimate เดิมยังอ้าง Revision เดิม
- Inactive Customer/Site ใช้สร้างธุรกรรมใหม่ไม่ได้ แต่ Historical Reference ยังอ่านได้

## Data Direction

Relational Core:

- customers, customer_contacts, customer_addresses
- opportunities, opportunity_stage_history
- sites
- site_surveys, site_survey_revisions
- site_survey_areas, site_survey_measurements, site_survey_evidence
- site_survey_checklist_results

JSONB ใช้เฉพาะ Versioned Survey Answer Payload ที่ Validate ด้วย Published Schema และ Metadata ที่ยืดหยุ่น ไม่เก็บ Customer/Contact/Opportunity Core เป็น JSONB

## UX Direction

- Desktop: Customer/Opportunity list + inspector; Survey workspace แบ่ง Context, Area/Measurement, Checklist, Evidence และ Readiness
- Tablet: list + side sheet; Survey ใช้ Section เดียวกันแบบหนึ่งคอลัมน์
- Mobile: Quick Customer Capture, Opportunity context และเก็บ Survey ทีละ Area พร้อม Save State ชัดเจน
- Phase แรกเป็น Online-first; Draft ที่ยังส่งไม่สำเร็จต้องไม่แสดงว่า Saved และไม่ทำ Offline multi-device merge
- ทุกหน้ารองรับไทย/อังกฤษ, Keyboard, Screen Reader, 320px, Zoom 200% และ Touch Target ≥44px

## API Direction

Resource API แยก Customer, Contact, Opportunity, Site, Survey และ Survey Revision Business transitions ใช้ Action endpoint, Mutation ใช้ ETag และ Create/Ready/Clone สำคัญใช้ Idempotency Key

ทุก Request ตรวจ Permission + Organization + Branch + Opportunity/Own Scope Backend คืน 404 เมื่อ Resource อยู่นอก Scope และ Error ใช้ RFC 9457 + Stable Code + ภาษาไทยเป็นค่าเริ่มต้น

## Privacy and Audit

- Personal/Contact Data ใช้ Field Allowlist, Permission แยก และ Mask ใน Log/Export
- Sensitive Change เช่น Contact, Address, Owner, Stage Outcome, Ready/Void Survey และ Evidence Manifest มี Audit
- ไม่มี Hard Delete สำหรับข้อมูลที่ถูกอ้างโดย Estimate/Quotation/Project
- Retention, Export และ Redaction ต้องผ่าน Privacy/Security Owner ก่อน Production; Phase นี้ไม่เดากฎกฎหมายแทนองค์กร

## Documents to Deliver

1. Business Flow, Field Catalog, Governance และ Responsive Text Wireframe
2. API Contract และ Data Contract
3. Permission/Error/Requirement/Conceptual Model Cross-links
4. UAT Scenarios และ Development Ready Gate
5. ADR สำหรับ Versioned Site Survey Revision

Static HTML Portal, SVG, JSON Flow และ Application Code อยู่นอกงานรอบนี้ตามคำสั่งผู้ใช้

## Acceptance Criteria

- Customer, Contact, Opportunity, Site, Survey และ Estimate ไม่ใช้ความหมายปะปนกัน
- Official Estimate อ้าง Ready Survey Revision ที่ Immutable ได้
- Duplicate/Scope/Concurrency/Stage/Readiness/Error มี Contract ชัดเจน
- Survey รองรับหลาย Area, Measurement, Checklist และ Evidence โดยไม่สร้าง Generic Form Builder
- Personal Data ไม่หลุดผ่าน Log, Search, Export หรือ Cross-organization Response
- ทุกตัวอย่างติด `TEST_ONLY`; ไม่มีข้อมูลลูกค้าจริง
- Development Ready Gate บอกชัดว่าเริ่ม Scaffold/Vertical Slice ได้เมื่อใด
