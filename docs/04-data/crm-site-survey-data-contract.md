# CRM and Site Survey Data Contract (ข้อตกลงข้อมูลลูกค้า งานขาย และสำรวจ)

**สถานะ:** Accepted Direction — Logical Schema Baseline

## Aggregate and Ownership

```text
Customer ─┬─ Contact
          ├─ Address
          ├─ Site
          └─ Opportunity ─ OpportunityStageHistory
                          └─ SiteSurvey
                              └─ SiteSurveyRevision
                                  ├─ Area ─ Measurement
                                  ├─ ChecklistResult
                                  └─ EvidenceReference ─► File Service

Official Estimate ─► Customer/Opportunity/Ready Survey Revision Snapshot
```

Customer และ Opportunity เป็น CRM Aggregate แยกกัน Site Survey เป็น Aggregate ของ Survey Module; Cross-aggregate mutation ทำผ่าน Application Use Case และ Transaction/Outbox ตาม Boundary ไม่ใช้ Shared Table Ownership

## Relational Core

### Customer, Contact, Address and Site

| Entity | Typed Field สำคัญ | Constraint |
| --- | --- | --- |
| `customers` | organization, code, type, display/legal name, preferred locale, status, inactive reason, row version, audit | Unique `(organization_id, code)`; no hard delete after reference |
| `customer_private_identifiers` | customer, identifier type, normalized hash, protected value, mask | Permission/Encryption policy; Unique เมื่อ Business Policy ยืนยัน |
| `customer_contacts` | customer, name, role, normalized/display phone/email, channel, primary, status, row version | อย่างน้อย Phone/Email; partial unique primary per Customer |
| `customer_addresses` | customer, address type, structured address, status, row version | Billing/Contact address; version/audit เมื่อถูก Snapshot |
| `sites` | customer, label, structured address, geo, access note, status, row version | Organization/Customer เดียวกัน; historical reference preserved |

Tax/Private Identifier แยก Table เพื่อจำกัด Projection/Permission/Encryption โดยไม่ทำให้ Customer Core เป็น JSONB

### Opportunity

`opportunities` มี organization, branch, customer, primary site, owner, code, title, scope summary, work types, source, expected budget/currency, target decision date, next action, stage, outcome reason และ row version

`opportunity_stage_history` เป็น Append-only มี from/to stage, reason, actor, occurred at, policy version และ request trace Closed/Reopen/Quotation-driven transitions ต้องเกิดผ่าน Use Case ที่ Validate State + Permission + Scope

### Site Survey

| Entity | Typed Field สำคัญ | Constraint |
| --- | --- | --- |
| `site_surveys` | organization, branch, opportunity, site, survey number, assignment/schedule, latest revision, status | Cross-resource scope เดียวกัน; Unique number |
| `survey_template_versions` | scope, code/version, work types, required field/measurement/checklist/evidence schema, effective period, status, hash | Published immutable; Effective Period ไม่ซ้อน; Phase แรก System-owned |
| `site_survey_revisions` | survey, revision number, template version, visit/scope, assumptions/constraints/missing, readiness, status, snapshot hash, ready actor/time, row version | Unique `(survey_id, revision_number)`; Business Content ล็อกหลัง Ready และ Lifecycle เปลี่ยนผ่าน Use Case |
| `site_survey_areas` | revision, code, name, description, sort order | Unique code/sort per Revision |
| `site_survey_measurements` | area, type, value, unit, capture method, derived source/formula version, note | Value >0; Unit/derivation valid; no cycle |
| `site_survey_checklist_results` | revision/area, template item code/version, result, note | Unique item per scope; required completed before Ready |
| `site_survey_evidence` | revision/area, file id, kind, caption, captured at, manifest metadata | File uploaded/validated/in scope before Ready |

## JSONB Boundary

Relational Field ใช้กับ Identity, Scope, State, Measurement, Evidence Reference และสิ่งที่ต้อง Query/Constraint ส่วน JSONB ใช้ได้เฉพาะ:

- `survey_answer_payload` ที่มี `schemaVersion`, `templateVersion`, `kind`, typed payload และ hash
- File/exif metadata ที่ผ่าน Allowlist และไม่มี Signed URL/Secret
- Readiness validation snapshot และ immutable source snapshot

ห้ามเก็บ Customer/Contact/Opportunity ทั้งก้อน, Permission, Error Translation หรือ Binary File ใน JSONB Published Template Schema ทุก Version ต้องยังอ่านได้ตลอด Retention ของ Revision ที่อ้าง

## Snapshot Contract

เมื่อ Mark Ready ให้ Canonicalize แล้ว Hash:

- Survey/Revision/Template Version
- Customer/Opportunity/Site identifiers และ Customer-safe labels ที่จำเป็น
- Area/Measurement/Unit/Capture Method/Derivation
- Checklist results, assumptions, constraints, missing details
- Evidence file ID/checksum/manifest; ไม่ฝัง Binary
- Readiness result, warnings, actor/time

Official Estimate เก็บ `site_survey_revision_id` + snapshot hash + Source Summary ที่ต้องใช้ หาก Source ถูก Void ภายหลังให้แจ้ง Risk แต่ห้ามสลับ Revision อัตโนมัติ

## Index and Constraint Baseline

- Customer search: `(organization_id, status, normalized_display_name, id)`; Contact token/hash index จำกัด Projection
- Opportunity: `(organization_id, branch_id, stage, owner_user_id, next_action_at_utc, id)`
- Survey: `(organization_id, branch_id, opportunity_id, status, scheduled_start_utc, id)`
- Revision unique `(site_survey_id, revision_number)` และ unique Ready selection ตาม Current Policy
- Template unique `(scope, code, version)` และ Published Effective Period ไม่ซ้อนต่อ Work Type/Scope
- Composite FK/Transactional Guard พิสูจน์ Organization/Customer/Branch/Site relationship
- Personal data index ต้องมีเหตุผล, จำกัดผู้เข้าถึง และไม่เขียน Plain Value ซ้ำโดยไม่จำเป็น

## Transaction Boundaries

- Create Customer + Primary Contact เป็น Atomic
- Stage Transition เขียน Opportunity + Append History + Audit แบบ Atomic
- Create Survey เขียน Identity + Revision 1 แบบ Atomic
- Mark Ready ล็อก Revision, Validate dependencies, เขียน snapshot/hash/status/audit แบบ Atomic
- Clone Revision จองเลขรุ่นและคัดลอก Source Snapshot เป็น Draft ใน Transaction เดียว
- File upload ไม่อยู่ DB Transaction; Ready ตรวจสถานะ File Service จาก durable reference ก่อน Commit

## Data Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-DATA-CRM-001` | Customer Code ซ้ำใน Organization | Unique reject |
| `TC-DATA-CRM-002` | Code เดียวกันคนละ Organization | อนุญาต |
| `TC-DATA-CRM-003` | Primary Contact สองรายการ | Constraint/transaction reject |
| `TC-DATA-CRM-004` | Opportunity Customer/Branch ข้าม Scope | Reject/rollback |
| `TC-DATA-CRM-005` | Illegal Closed Stage Update | Reject; history ไม่ขาด |
| `TC-DATA-SRV-001` | Revision Number พร้อมกัน | ได้เลขไม่ซ้ำแบบ Atomic |
| `TC-DATA-SRV-002` | Measurement Value ≤0/Unit ผิด | Reject |
| `TC-DATA-SRV-003` | Derived Measurement Cycle | Reject/rollback |
| `TC-DATA-SRV-004` | Update Ready Revision | Reject immutable row |
| `TC-DATA-SRV-005` | Evidence File นอก Scope/Upload ไม่ครบ | Ready rollback |
| `TC-DATA-SRV-006` | New Ready Revision | Source/Snapshot ที่ Estimate เดิมอ้างไม่เปลี่ยน |
| `TC-DATA-SEC-001` | Query ไม่มี Organization Scope | Architecture/integration gate fail |

Field และ Lifecycle ฉบับเต็มอยู่ที่ [Field Catalog](../01-business/crm-site-survey-field-catalog.md) และ [Governance](../01-business/crm-site-survey-governance.md)
