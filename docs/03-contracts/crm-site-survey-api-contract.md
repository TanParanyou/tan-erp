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
  "displayNameEn": null,
  "preferredLocale": "th",
  "primaryContact": {
    "name": "คุณตัวอย่าง TEST_ONLY",
    "roleTitle": null,
    "phone": "+66812345678",
    "email": "sample@example.test",
    "preferredChannel": "phone"
  }
}
```

Response 201 คืน Customer Draft, generated code และ ETag พร้อม `duplicateCandidates` ที่ Mask แล้วเมื่อผู้ใช้มี Permission ระบบไม่ Auto-merge

## Customer + Contact Slice 1 Specification

### Headers and Scopes
```text
Required business header: X-Membership-Id: <membership UUID>
Required create header: Idempotency-Key: <opaque 16-128 characters>
Supported permission scope in Slice 1: organization only
List sort: normalizedDisplayName ASC, id ASC
List defaults: limit=25; allowed range 1..100
Create result: Customer status=draft, one active primaryContact, ETag="<rowVersion>"
Duplicate signal: exact normalized name/phone/email inside the selected Organization only
```

### Request Body Rules
Request body มีเฉพาะ `customerType`, `displayNameTh`, `displayNameEn`, `preferredLocale` และ `primaryContact.{name,roleTitle,phone,email,preferredChannel}`; ห้ามมี `organizationId`, `branchId`, `status`, `code`, `rowVersion` หรือ actor ID

### Exact List Response Shape
```json
{
  "items": [
    {
      "id": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
      "code": "CUS-019A3CF896F0",
      "customerType": "organization",
      "displayNameTh": "บริษัท ตัวอย่าง จำกัด TEST_ONLY",
      "displayNameEn": null,
      "preferredLocale": "th",
      "status": "draft",
      "primaryContact": {
        "name": "คุณตัวอย่าง TEST_ONLY",
        "phone": "+66******123",
        "email": "t***@example.test",
        "isMasked": true
      }
    }
  ],
  "nextCursor": null
}
```

### Exact Detail & Create Response Shape
Detail และ Create response ส่งคืน Customer object พร้อม `ETag: "<rowVersion>"` header
สำหรับ Create response เพิ่ม `duplicateCandidates: [...]`
เมื่อผู้ใช้มี `customer-contacts.manage` ให้ `primaryContact.isMasked = false` และคืนค่าเบอร์โทรศัพท์และอีเมลเต็ม; เมื่อมีเฉพาะ `customers.read` จะคืนเฉพาะค่าที่ Mask แล้ว (`isMasked = true`)


## Customer Activation + Opportunity + Site Slice 2 Specification

### 1. Activate Customer
```http
POST /api/v1/customers/{customerId}/activate
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
If-Match: "<customer rowVersion UUID>"
```
Request ไม่มี body. Response `200` ใช้ `CustomerResponse` เดิมและส่ง ETag ใหม่

### 2. Create and List Site
```http
POST /api/v1/customers/{customerId}/sites
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
Content-Type: application/json

{
  "label": "คอนโดสุขุมวิท TEST_ONLY",
  "addressLine1": "99/9 ถนนสุขุมวิท TEST_ONLY",
  "subdistrict": "คลองตันเหนือ",
  "district": "วัฒนา",
  "province": "กรุงเทพมหานคร",
  "postalCode": "10110",
  "countryCode": "TH",
  "latitude": null,
  "longitude": null,
  "accessNote": "ติดต่อเจ้าหน้าที่ก่อนขึ้นอาคาร TEST_ONLY"
}
```
Response `201`:
```json
{
  "id": "019a3cf8-96f0-7c9f-b207-93aa818f4c10",
  "customerId": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
  "code": "SITE-019A3CF896F0",
  "label": "คอนโดสุขุมวิท TEST_ONLY",
  "addressLine1": "99/9 ถนนสุขุมวิท TEST_ONLY",
  "subdistrict": "คลองตันเหนือ",
  "district": "วัฒนา",
  "province": "กรุงเทพมหานคร",
  "postalCode": "10110",
  "countryCode": "TH",
  "latitude": null,
  "longitude": null,
  "accessNote": "ติดต่อเจ้าหน้าที่ก่อนขึ้นอาคาร TEST_ONLY",
  "status": "active",
  "rowVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4c11",
  "createdAtUtc": "2026-09-08T12:00:00Z"
}
```
`GET /api/v1/customers/{customerId}/sites` คืน `{ "items": SiteResponse[] }` เรียง `normalizedLabel ASC, id ASC`; ไม่มี pagination ใน slice นี้

### 3. Create, List and Read Opportunity
```http
POST /api/v1/opportunities
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
Content-Type: application/json

{
  "customerId": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
  "primarySiteId": "019a3cf8-96f0-7c9f-b207-93aa818f4c10",
  "title": "Built-in ห้องนอนใหญ่ TEST_ONLY",
  "scopeSummary": "สำรวจและประเมินตู้เสื้อผ้า TEST_ONLY",
  "workTypes": ["built-in"],
  "sourceCode": null,
  "expectedBudget": 250000.00,
  "currencyCode": "THB",
  "targetDecisionDate": "2026-10-15",
  "nextActionAtUtc": "2026-09-10T03:00:00Z",
  "nextActionNote": "นัดยืนยันเวลา TEST_ONLY"
}
```
Request ห้ามมี `organizationId`, `branchId`, `ownerUserId`, `code`, `stage`, `rowVersion` หรือ actor ID
- ใน Slice 2 นี้ `branchId` ถูก Derive จาก Active Branch ของ selected Membership
- `ownerUserId` ถูก Derive จาก authenticated User
(แม้ baseline example ด้านล่างจะแสดง branchId/ownerUserId ใน payload สำหรับ slice ถัดไป แต่ใน Slice 2 ฝั่ง Client ห้ามส่งมาเด็ดขาด)

Response `201`:
```json
{
  "id": "019a3cf8-96f0-7c9f-b207-93aa818f4d10",
  "code": "OPP-019A3CF896F0",
  "customerId": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
  "primarySiteId": "019a3cf8-96f0-7c9f-b207-93aa818f4c10",
  "branchId": "019a3cf8-96f0-7c9f-b207-93aa818f4a13",
  "ownerUserId": "019a3cf8-96f0-7c9f-b207-93aa818f4a10",
  "title": "Built-in ห้องนอนใหญ่ TEST_ONLY",
  "scopeSummary": "สำรวจและประเมินตู้เสื้อผ้า TEST_ONLY",
  "workTypes": ["built-in"],
  "sourceCode": null,
  "expectedBudget": 250000.00,
  "currencyCode": "THB",
  "targetDecisionDate": "2026-10-15",
  "nextActionAtUtc": "2026-09-10T03:00:00Z",
  "nextActionNote": "นัดยืนยันเวลา TEST_ONLY",
  "stage": "draft",
  "rowVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4d11",
  "createdAtUtc": "2026-09-08T12:00:00Z"
}
```
`GET /api/v1/opportunities/{id}` คืน shape เดียวกันพร้อม ETag
`GET /api/v1/opportunities?search=&customerId=&stage=draft&limit=25&cursor=` คืน `{ items, nextCursor }`; allowed limit `1..100`, stable sort `nextActionAtUtc ASC NULLS LAST, id ASC`

## Opportunity Qualification Slice 3 Specification

### Request

```http
POST /api/v1/opportunities/{opportunityId}/stage-transitions
Authorization: Bearer <Firebase ID token>
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
Accept-Language: th | en
Content-Type: application/json
```

```json
{
  "targetStage": "qualified",
  "expectedVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4d11"
}
```

- Slice 3 รับ `targetStage` เพียง `qualified`; ไม่รับ `organizationId`, `branchId`, `ownerUserId`, `fromStage`, actor, `reasonCode` หรือ free-text `note`
- `expectedVersion` คือ UUID จาก `OpportunityResponse.rowVersion`; UUID ว่างคืน `422 OPPORTUNITY_FIELD_REQUIRED`
- Missing/invalid JSON หรือ header context คืน `400 REQUEST_VALIDATION_FAILED`, `400 MEMBERSHIP_CONTEXT_REQUIRED`, `400 IDEMPOTENCY_KEY_REQUIRED` หรือ `400 IDEMPOTENCY_KEY_INVALID` ตาม baseline

### Success

```http
HTTP/1.1 200 OK
ETag: "019a3cf8-96f0-7c9f-b207-93aa818f4d12"
Content-Type: application/json
```

Response ใช้ `OpportunityResponse` เดิมทุก field โดยเปลี่ยนเฉพาะ:

```json
{
  "stage": "qualified",
  "rowVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4d12"
}
```

ค่าฟิลด์อื่นต้องเท่ากับ resource ก่อน transition. Retry key/payload เดิมคืน `200`, body และ ETag เดียวกับ transition แรก และไม่เพิ่ม history/audit.

### Error Matrix

| HTTP | Stable code | Condition |
| ---: | --- | --- |
| 400 | `REQUEST_VALIDATION_FAILED` | malformed body/route binding |
| 400 | `MEMBERSHIP_CONTEXT_REQUIRED` | ไม่มี/ผิดรูปแบบ `X-Membership-Id` |
| 400 | `IDEMPOTENCY_KEY_REQUIRED` / `IDEMPOTENCY_KEY_INVALID` | ไม่มี key หรือความยาวนอก 16–128 |
| 401 | `AUTHENTICATION_REQUIRED` / `AUTHENTICATION_INVALID` | ไม่มี/ใช้ token ไม่ได้ |
| 403 | `ACTIVE_MEMBERSHIP_REQUIRED` / `PERMISSION_DENIED` | membership หมดอายุ/ไม่มี `opportunities.transition` |
| 404 | `RESOURCE_NOT_FOUND` | Opportunity, Customer, Branch หรือ Owner Membership ไม่อยู่ Organization/scope ที่อนุญาต |
| 409 | `IDEMPOTENCY_KEY_REUSED` | key เดิมแต่ canonical payload ต่าง |
| 409 | `OPPORTUNITY_VERSION_CONFLICT` | `expectedVersion` ไม่ตรง current row version |
| 409 | `OPPORTUNITY_INVALID_TRANSITION` | current stage ไม่ใช่ `draft` หรือ target ไม่ใช่ `qualified` |
| 409 | `CUSTOMER_INVALID_STATE` | Customer ปัจจุบันไม่ Active |
| 422 | `OPPORTUNITY_FIELD_REQUIRED` | Q gate ขาด `scopeSummary`, work type, `nextActionAtUtc` หรือ `nextActionNote` |
| 422 | `ACTIVE_BRANCH_REQUIRED` | selected membership ไม่มี Active Branch |

Precedence หลัง authentication/context/permission: idempotency replay/conflict → resource scope → expected version → current state/target → Q gate.

### Canonical Stage Vocabulary

Stage ทั้งหมดในระบบ: `draft`, `qualified`, `surveying`, `estimating`, `proposed`, `won`, `lost`, `cancelled`.

### Persistence Contract

ตาราง `crm.opportunity_stage_history`:
- Columns: `id` (uuid, PK), `organization_id` (uuid, FK), `opportunity_id` (uuid, composite FK `(id, organization_id)`), `from_stage` / `to_stage` (varchar 32), `reason_code` / `note` (nullable text), `actor_user_id` (uuid), `occurred_at_utc` (timestamptz), `policy_version` (varchar 64), `trace_id` (varchar 128).
- Composite Index: `(organization_id, opportunity_id, occurred_at_utc, id)`
- Delete behavior: `Restrict`
- Audit Event: `opportunity.stage-changed` payload `{"changedFields":["stage"],"fromStage":"draft","toStage":"qualified"}` (ไม่มี PII/business text)

## Baseline Examples (Broader / Future Slices)

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
