# Project API Contract (ข้อตกลง API โครงการ)

**สถานะ:** Implemented 2026-10-04 สำหรับ Handover (CP-08). ค่าเริ่มต้นด้านล่างเป็นการตัดสินใจของทีมพัฒนา (ผู้ใช้มอบหมาย) ยังไม่ผ่าน Sales/Project Owner; สถานะอื่นของ [Project Lifecycle](../01-business/project-lifecycle.md) ยังเป็น Future (CP-09).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| จำนวน Project ต่อ Quotation | **หนึ่งต่อหนึ่ง** (unique `(organization_id, quotation_id)`; การแบ่งงานเป็นหลาย Project ยังไม่รองรับ) |
| เงื่อนไข Handover | Quotation `accepted` และ Opportunity อยู่ stage `won` |
| Project Owner | ผู้ใช้ที่มี Membership Active ใน Branch ของ Quotation; ผู้สร้างระบุเอง (ไม่เดา) |
| Branch | ใช้ Branch ของ Quotation |
| ชื่อ/วันเริ่ม | ชื่อว่าง = ชื่อ Opportunity (≤ 200); วันเริ่มงานเป็น optional (`planned_start_date`) |
| สถานะเริ่มต้น | `planned` เท่านั้น |
| Baseline | สำเนา ณ วัน Handover: เลข Quotation, ยอด, Quotation snapshot hash, Estimate/Revision ID, Survey Revision ID + hash และ `baseline_hash` (SHA-256 ของค่าเหล่านี้) ไม่อ่านกลับจาก master ที่เปลี่ยนได้ |
| ผลข้างเคียง | ไม่เรียก Procurement/Production/Billing อัตโนมัติ |

## Endpoints

Headers: `Authorization`, `X-Membership-Id`; `POST` ต้องมี `Idempotency-Key`.

| Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| Handover | `POST /api/v1/projects` | `projects.create` | 201 + `ETag` |
| รายการ | `GET /api/v1/projects?search=&status=&page=&pageSize=` | `projects.read` | 200 |
| รายละเอียด | `GET /api/v1/projects/{id}` | `projects.read` | 200 + `ETag` |
| ต้นทาง Handover ของ Opportunity | `GET /api/v1/projects/handover-source?opportunityId=` | `projects.read` | 200/404 |

```json
// POST /api/v1/projects
{ "quotationId": "<uuid>", "expectedQuotationVersion": "<quotation rowVersion>",
  "ownerUserId": "<uuid>", "plannedStartDate": "2026-11-01", "name": null }
```

- `handover-source` คืน `{ quotationId, quotationNumber, quotationStatus, quotationRowVersion, contractAmount, opportunityStage, existingProjectId, existingProjectCode }` ของ Quotation `accepted` ล่าสุด; 404 เมื่อไม่มี. ใช้ให้ UI รู้ rowVersion ที่ต้องส่งและลิงก์ไป Project ที่มีอยู่.
- Response ใช้ Structured projection ไม่ส่งแค่ FK: `owner {id,displayName,email}`, `customer {id,code,displayNameTh,displayNameEn}`, `site {id,label}`, `opportunity {id,code,title}`, `baseline {...}`.
- List ใช้ `page`/`pageSize` (≤ 100, ค่าเริ่มต้น 25) เรียง `createdAt desc, id desc`; `search` ตรง code/ชื่อ/เลข Quotation (ไม่แยกตัวพิมพ์); `status` ไม่ถูกต้อง → 422 `PROJECT_FIELD_INVALID`.

## Errors

| HTTP | Code | เงื่อนไข |
| ---: | --- | --- |
| 404 | `RESOURCE_NOT_FOUND` | Quotation/Opportunity/Estimate นอก Organization หรือ Owner ไม่มี Membership Active ใน Branch |
| 409 | `QUOTATION_VERSION_CONFLICT` | `expectedQuotationVersion` ไม่ตรง |
| 409 | `PROJECT_HANDOVER_NOT_ALLOWED` | Quotation ไม่ใช่ `accepted` หรือ Opportunity ไม่ใช่ `won` |
| 409 | `PROJECT_ALREADY_EXISTS` | มี Project ของ Quotation นี้แล้ว (รวมกรณีชนกันพร้อมกัน) |
| 409 | `IDEMPOTENCY_KEY_REUSED` | Key เดิม payload ต่าง |
| 422 | `PROJECT_FIELD_REQUIRED` / `PROJECT_FIELD_INVALID` | ข้อมูลไม่ครบ/ไม่ถูกต้อง |
| 403 | `PERMISSION_DENIED` | ไม่มีสิทธิ์ |

Precedence: Idempotency replay → scope/version → state → duplicate → owner → numbering. Key + payload เดิมคืน Project เดิม.

## Data

`projects.projects` (migration `AddProjectsFromHandover`): `code` unique ต่อ Organization (เลขจาก Document Numbering ประเภท `projects`, prefix `PRJ`, รูปแบบ `PRJ-{YYYY}-{SEQ:4}`), `status` check, `baseline_contract_amount >= 0`, FK Restrict ไป Organization/Branch/Customer/Opportunity/Quotation/Users. Audit: `project.created-from-handover` (code, quotationId/number, ownerUserId, baselineHash).
