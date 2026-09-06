# CRM and Site Survey Governance (การกำกับดูแลข้อมูลลูกค้าและสำรวจ)

**สถานะ:** Accepted Direction — Production Bootstrap

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Ownership, Lifecycle, Duplicate, Privacy, Readiness และ Audit รายการ Field อยู่ที่ [Field Catalog](crm-site-survey-field-catalog.md)

## Ownership

| Concern | Owner | ผู้ตรวจ/ผู้มีอำนาจ |
| --- | --- | --- |
| Customer/Contact/Address | Sales/CRM Owner | Data Steward/Privacy Owner |
| Opportunity/Stage/Owner | Sales Owner | Sales Manager ตาม Scope |
| Site/Appointment | Sales/Survey Coordinator | Branch Owner |
| Survey Draft/Evidence | Surveyor | Survey Reviewer/Estimator |
| Ready/Void Survey Revision | Surveyor/Reviewer | ผู้มี Permission + Scope |

Role เป็นชุด Permission ที่ปรับได้ Business Logic ห้าม Hard-code ชื่อตำแหน่ง

## Duplicate and Identity Rules

- Customer Code เป็น Business Identifier; ชื่อ/โทรศัพท์/อีเมลเป็นสัญญาณ Duplicate ไม่เป็น Unique Identity
- Tax Identifier เมื่อมีต้อง Normalize, จำกัดสิทธิ์ และตรวจ Duplicate ใน Organization โดยไม่เปิดเผยค่าทั้งหมด
- Duplicate Candidate แสดง Match Reason ให้ Data Steward ตัดสิน; Phase แรกไม่ Auto-merge
- Customer ที่ถูกอ้างเปลี่ยนเป็น Inactive ได้ แต่ Phase แรกยังไม่ Merge/ย้ายประวัติ; หากต้องเพิ่มภายหลังต้องออก Merge Contract โดยเฉพาะ
- Contact คนเดียวอาจอยู่หลาย Customer ในอนาคต แต่ Phase แรกเก็บ Contact ต่อ Customer เพื่อลด Boundary ที่ยังไม่จำเป็น

## Opportunity Controls

- Owner ต้องมี Active Membership ใน Branch Scope
- Stage Transition ใช้ Allowlist และบังคับ Field ตาม [Flow](crm-site-survey-flow.md)
- Owner/Branch/Expected Budget/Outcome เปลี่ยนต้อง Audit พร้อม Before/After
- Closed Opportunity แก้ Business Field ไม่ได้ ยกเว้น Reopen Use Case ที่มี Reason/Permission
- Quotation Accepted/Won และ Project Handover ต้องเป็น Cross-module Use Case แบบ Idempotent ไม่ให้ Client Patch Stage โดยตรง

## Survey Revision Controls

- Survey Identity ผูก Opportunity + Site; Revision เก็บข้อมูลการไปสำรวจแต่ละครั้ง
- Draft Mutation ใช้ ETag; Ready/Clone/Void ใช้ Idempotency Key
- Mark Ready ตรวจ Field, Checklist, Measurement, Evidence Upload และ Cross-resource Scope ใน Transaction เดียว
- Ready/Superseded/Void Revision ล็อก Business Content; Lifecycle Metadata เปลี่ยนได้ผ่าน Transition Use Case และการแก้เนื้อหาใช้ Clone New Revision
- Ready Revision ใหม่ Supersede รุ่นเก่าตาม Policy แต่ไม่เปลี่ยน Estimate ที่อ้างรุ่นเก่า
- หาก Void Revision ที่ Estimate อ้าง ระบบแจ้ง Risk/Audit แต่ไม่ลบหรือสลับ Source อัตโนมัติ
- Phase แรก Resolve System-owned Published Survey Template ตาม Work Type/Visit Date; ไม่มี Template ให้ Fail-closed `SURVEY_TEMPLATE_UNAVAILABLE`
- Published Survey Template Version เป็น Immutable และรุ่นใหม่ไม่เปลี่ยน Ready Revision เดิม

## Measurement and Evidence

- Measurement เก็บ Value + Unit + Capture Method; Derived Value ต้องอ้าง Source และ Formula Code/Version ที่อนุญาต
- ห้ามเดาหน่วยหรือ Conversion เมื่อไม่ชัด; ใช้ Item/Unit Contract กลาง
- Evidence File ต้อง Upload Complete, ผ่าน File Validation และอยู่ Organization Scope ก่อน Ready
- Binary อยู่ File Service; Database เก็บ Reference, checksum/metadata และ Evidence Manifest Snapshot
- การลบไฟล์ที่ถูกอ้างโดย Ready Revision ต้องใช้ Retention/Redaction Workflow ไม่ลบโดยตรง

## Privacy and Security Baseline

- เก็บข้อมูลส่วนบุคคลเท่าที่ Workflow ต้องใช้ พร้อม Data Classification ใน Field Catalog
- Search/Export/Log ใช้ Allowlist และ Mask ค่า Contact/Tax/Location ตาม Permission
- Resource นอก Scope คืน 404 และไม่เปิดเผย Duplicate Candidate ข้าม Organization
- Export Customer/Survey Evidence เป็น Sensitive Action ต้องมี Permission, Audit และ Expiry สำหรับ Download Link
- Retention, Consent/Legal Basis, Data Subject Request และ Redaction Rule ต้องให้ Privacy/Security Owner ขององค์กรยืนยันก่อน Production

## Audit Events

- `customer.created`, `updated`, `activated`, `deactivated`, `duplicate-reviewed`
- `contact.created`, `updated`, `deactivated`; `site.created`, `updated`, `deactivated`
- `opportunity.created`, `owner-changed`, `stage-changed`, `closed`, `reopened`
- `survey.created`, `revision-updated`, `marked-ready`, `revision-cloned`, `revision-superseded`, `revision-voided`
- `survey.evidence-added`, `evidence-removed`, `exported`

Audit เก็บ ID/Code/Before-After ที่จำเป็น ไม่คัดลอก Personal Data หรือไฟล์ทั้งก้อนลง Log

## Production Bootstrap

จนกว่าจะยืนยัน Sales/Survey Policy จริง:

- Survey ทุก Ready Revision ต้องมีผู้ใช้กด Mark Ready และเก็บ Readiness Result; ไม่มี Auto-ready
- Measurement ที่ `customer-provided` หรือมี Missing Detail แสดง Warning ใน Estimate
- Opportunity Won/Lost/Reopen และ Survey Void ต้องบังคับ Reason
- ไม่มี Customer Auto-merge, Auto-delete หรือ Cross-organization Search
- ไม่มี Runtime Survey Form Builder; Baseline Template เปลี่ยนผ่าน Reviewed Migration/Seed และ Version ใหม่
- กฎที่ Resolve ไม่ได้ให้ Fail-closed และคง Draft

## Governance Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-GOV-CRM-001` | ชื่อ/เบอร์คล้ายลูกค้าเดิม | แสดง Candidate; ไม่ Auto-merge |
| `TC-GOV-CRM-002` | Inactive Customer สร้าง Opportunity | Block |
| `TC-GOV-CRM-003` | Patch Closed Opportunity | Block ยกเว้น Reopen Use Case |
| `TC-GOV-SRV-001` | Ready Revision ขาด Required Evidence | Fail-closed; ยัง Draft |
| `TC-GOV-SRV-002` | ETag เก่าแก้ Survey Draft | Conflict; ไม่ Last-write-wins |
| `TC-GOV-SRV-003` | Ready Revision ใหม่ | รุ่นเก่า Superseded; Estimate เดิมไม่เปลี่ยน |
| `TC-GOV-SRV-004` | Void Revision ที่ Estimate อ้าง | เก็บประวัติ + Risk/Audit; ไม่ลบ |
| `TC-GOV-SEC-001` | Search/Export ข้าม Organization | 404/deny + Security Audit |
| `TC-GOV-SEC-002` | Log Error Contact/Tax ID | ไม่มีค่าจริงใน Log |
| `TC-GOV-SEC-003` | Retry Mark Ready Key เดิม | คืนผลเดิมครั้งเดียว |

## Production Sign-off

Business Owner ยืนยัน Stage/Required Field/Outcome, Survey Owner ยืนยัน Template/Measurement/Checklist/Evidence และ Privacy/Security Owner ยืนยัน Retention/Export/Mask/Redaction ก่อน Production ระหว่างรอใช้ Production Bootstrap
