# CRM and Site Survey API Contract (ข้อตกลง API ลูกค้า งานขาย และสำรวจ)

**สถานะ:** Accepted Direction — Baseline สำหรับ OpenAPI

## Common Contract

```http
Authorization: Bearer <firebase-id-token>
Accept-Language: th
If-Match: "<row-version>"
Idempotency-Key: <uuid>
```

Backend Resolve Membership/Scope จาก PostgreSQL ทุก Request, Error ใช้ RFC 9457 และ Personal Data ใช้ Response Allowlist ตาม Permission

## Endpoint Matrix

| Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| ค้นหา/อ่าน Customer | `GET /api/v1/customers`, `GET /api/v1/customers/{id}` | `customers.read` | 200 |
| สร้าง/แก้ Customer | `POST /api/v1/customers`, `PATCH /api/v1/customers/{id}` | `customers.create`, `customers.update` | 201/200 |
| Activate/Deactivate | `POST /api/v1/customers/{id}/activate`, `/deactivate` | `customers.activate`, `customers.deactivate` | 200 |
| เพิ่ม/แก้ Contact | `POST /api/v1/customers/{id}/contacts`, `PATCH /api/v1/contacts/{id}` | `customer-contacts.manage` | 201/200 |
| เพิ่ม/แก้ Address | `POST /api/v1/customers/{id}/addresses`, `PATCH /api/v1/customer-addresses/{id}` | `customer-contacts.manage` | 201/200 |
| สร้าง/แก้ Site | `POST /api/v1/customers/{id}/sites`, `PATCH /api/v1/sites/{id}` | `sites.manage` | 201/200 |
| ค้นหา/อ่าน Opportunity | `GET /api/v1/opportunities`, `GET /api/v1/opportunities/{id}` | `opportunities.read` | 200 |
| สร้าง/แก้ Opportunity | `POST /api/v1/opportunities`, `PATCH /api/v1/opportunities/{id}` | `opportunities.create`, `opportunities.update` | 201/200 |
| เปลี่ยน Stage | `POST /api/v1/opportunities/{id}/stage-transitions` | `opportunities.transition` | 200 |
| สร้าง/อ่าน Survey | `POST /api/v1/site-surveys`, `GET /api/v1/site-surveys/{id}` | `surveys.create`, `surveys.read` | 201/200 |
| Save Draft Revision | `PATCH /api/v1/site-survey-revisions/{id}` | `surveys.update` | 200 |
| Mark Ready | `POST /api/v1/site-survey-revisions/{id}/mark-ready` | `surveys.mark-ready` | 200 |
| Clone Revision | `POST /api/v1/site-surveys/{id}/revisions` | `surveys.create-revision` | 201 |
| Void Revision | `POST /api/v1/site-survey-revisions/{id}/void` | `surveys.void` | 200 |
| อ่าน Survey Template | `GET /api/v1/survey-template-versions` | `surveys.read` | 200 |

Resource นอก Scope คืน 404 รายการค้นหาใช้ Cursor Pagination + Stable Sort และไม่คืน Contact/Location Field ที่ Permission ไม่อนุญาต

## Customer Example

```json
{
  "customerType": "organization",
  "displayNameTh": "บริษัท ตัวอย่าง จำกัด TEST_ONLY",
  "preferredLocale": "th",
  "primaryContact": {
    "name": "คุณตัวอย่าง TEST_ONLY",
    "phone": "+66XXXXXXXXX",
    "email": null,
    "preferredChannel": "phone"
  }
}
```

Response 201 คืน Customer Draft, generated code และ ETag พร้อม `duplicateCandidates` ที่ Mask แล้วเมื่อผู้ใช้มี Permission ระบบไม่ Auto-merge

## Opportunity Example

```json
{
  "customerId": "5d70e0d5-f894-4da7-a591-90c81419855a",
  "branchId": "6493ddaf-284b-4a98-b1c2-f715fe5c971a",
  "ownerUserId": "6fd379db-10f1-40f8-8d8b-0418470c6753",
  "title": "Built-in ห้องนอนใหญ่ TEST_ONLY",
  "scopeSummary": "สำรวจและประเมินตู้เสื้อผ้า TEST_ONLY",
  "workTypes": ["built-in"],
  "nextActionAtUtc": "2026-09-07T03:00:00Z",
  "nextActionNote": "นัดยืนยันเวลา TEST_ONLY"
}
```

Stage Transition Request ระบุ `targetStage`, `reasonCode?`, `note?`, `expectedVersion` Stage Closed/Reopen บังคับ Reason และไม่รับ `won` จาก Generic Patch

## Survey Example

```json
{
  "opportunityId": "9c5ae5f9-f02d-40ef-bd62-678a4631cd10",
  "siteId": "75ea2635-0db9-46ae-889b-609ce88e4ef2",
  "assignedSurveyorId": "6fd379db-10f1-40f8-8d8b-0418470c6753",
  "scheduledStartUtc": "2026-09-08T03:00:00Z",
  "scheduledEndUtc": "2026-09-08T05:00:00Z"
}
```

Response สร้าง Survey Identity + Draft Revision 1 แบบ Atomic Draft Patch รับ `surveyTemplateVersion`, visit/scope, area/measurement/checklist/evidence manifest ตาม [Field Catalog](../01-business/crm-site-survey-field-catalog.md) แต่ Client ห้ามส่ง Readiness/Status/Snapshot Hash เป็นค่าที่เชื่อถือได้

## Mark Ready and Clone

```json
POST /api/v1/site-survey-revisions/{id}/mark-ready
{
  "expectedVersion": 8,
  "acknowledgedWarningCodes": ["CUSTOMER_PROVIDED_MEASUREMENT"]
}
```

Backend Validate Cross-resource Scope, Published Template, Field/Measurement/Checklist/Evidence และ Upload State แล้วสร้าง Snapshot Hash + Ready timestamp ใน Transaction เดียว หากยังไม่พร้อมคืน 422 `SURVEY_NOT_READY` พร้อม Field/Area/File pointers

Clone Request ระบุ `sourceRevisionId`, `reasonCode`, `reason` และใช้ Idempotency Key Response คืน Draft Revision ใหม่พร้อม ETag; Source ไม่เปลี่ยน

## Concurrency, Retry and Privacy

- Draft Patch ใช้ ETag; ค่าเก่าคืน `*_VERSION_CONFLICT` โดยไม่ Merge อัตโนมัติ
- Create/Transition/Mark Ready/Clone/Void ใช้ Idempotency Key ตามความเสี่ยง
- Key เดิม + Payload เดิมคืนผลเดิม; Payload ต่างคืน `IDEMPOTENCY_KEY_REUSED`
- Search query ถูก Normalize และ Rate-limit; Duplicate Match ไม่ค้นข้าม Organization
- Trace/Problem Detail ไม่คืน Phone, Email, Tax ID, Address หรือ Signed File URL
- File upload ใช้ File Service; API รับ `fileId` ไม่รับ Base64 ใน JSON
- Official Estimate ต้องส่ง `siteSurveyRevisionId`; Backend รับเฉพาะ Ready/Superseded Revision ใน Scope ตาม Policy

## Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-API-CRM-001` | Create Customer + Primary Contact | 201 + ETag; PII ไม่อยู่ Log |
| `TC-API-CRM-002` | Patch Customer ด้วย ETag เก่า | 409 `CUSTOMER_VERSION_CONFLICT` |
| `TC-API-CRM-003` | Create Opportunity จาก Inactive Customer | 409 `CUSTOMER_INVALID_STATE` |
| `TC-API-CRM-004` | Illegal Stage Transition | 409 `OPPORTUNITY_INVALID_TRANSITION` |
| `TC-API-CRM-005` | Close โดยไม่มี Reason | 422 Field Error |
| `TC-API-SRV-001` | Create Survey ข้าม Customer/Branch | 404; ไม่มีข้อมูลรั่ว |
| `TC-API-SRV-002` | Ready โดยขาด Measurement/Evidence | 422 `SURVEY_NOT_READY` |
| `TC-API-SRV-003` | Patch Ready Revision | 409 `SURVEY_INVALID_STATE` |
| `TC-API-SRV-004` | Retry Mark Ready Key เดิม | คืน Ready Result เดิม |
| `TC-API-SRV-005` | Clone Ready Revision | 201 Draft รุ่นใหม่; Source ไม่เปลี่ยน |
| `TC-API-SRV-006` | Official Estimate อ้าง Draft Survey | 422 `SURVEY_REVISION_NOT_USABLE` |
| `TC-API-SRV-007` | Resolve ไม่มี Published Template ตรง Work Type/Date | 409 `SURVEY_TEMPLATE_UNAVAILABLE` |
| `TC-API-SEC-001` | Search/Export ข้าม Organization | 404/deny + Security Audit |
| `TC-API-I18N-001` | ภาษาไม่รองรับ | Stable Code + fallback ไทย |

## Data Mapping

อ่าน [CRM and Site Survey Data Contract](../04-data/crm-site-survey-data-contract.md)
