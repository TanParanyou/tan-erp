# Project API Contract (ข้อตกลง API โครงการ)

**สถานะ:** Implemented 2026-10-04 สำหรับ Handover (CP-08) และ Project Control (CP-09: แผนเวลา, งบ Baseline, Milestone/Progress, สถานะตาม [Project Lifecycle](../01-business/project-lifecycle.md), Change Order). ค่าเริ่มต้นเป็นการตัดสินใจของทีมพัฒนา (ผู้ใช้มอบหมาย) ยังไม่ผ่าน Sales/Project Manager/Finance.

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


## Project Control (CP-09, implemented 2026-10-04)

ทุก endpoint ใต้ `/api/v1/projects/{projectId}` คืน `ProjectControlResponse` (+ `ETag` = row version ของ Project) ยกเว้นที่ระบุ. `PUT/POST` ที่แก้ Project ใช้ `If-Match`; Milestone/Change Order ใช้ `expectedVersion` ใน body.

| Action | Method/Path | Permission |
| --- | --- | --- |
| อ่านแผน/งบ/Progress/Milestone/Change Order/ประวัติ | `GET /control` | `projects.read` |
| แผนเวลา | `PUT /plan` `{plannedStartDate, plannedEndDate}` | `projects.update` |
| งบ Baseline (แทนที่ทั้งชุด) | `PUT /budget` `{lines:[{category,description,amount}]}` | `projects.update` |
| Milestone | `POST /milestones`, `PUT /milestones/{id}`, `POST /milestones/{id}/complete`, `POST /milestones/{id}/delete` | `projects.update` |
| เปลี่ยนสถานะ | `POST /transitions` `{targetStatus, reason}` | `projects.transition` |
| Change Order | `POST /change-orders` (+`Idempotency-Key`), `POST /change-orders/{id}/submit`, `/cancel` | `projects.change-orders.manage` |
| ตัดสิน Change Order | `POST /change-orders/{id}/approve`, `/reject` | `projects.change-orders.approve` |

กฎที่ตัดสินใจ:

- **Lifecycle:** `planned → active | cancelled`; `active → on_hold | ready_for_handover | cancelled`; `on_hold → active | cancelled`; `ready_for_handover → active | completed`; `completed`/`cancelled` เป็นปลายทาง. ต้องมีเหตุผลเมื่อไป `on_hold`, `cancelled` หรือเปิดกลับจาก `ready_for_handover` (`PROJECT_REASON_REQUIRED`). ประวัติเก็บใน `project_status_history` (append-only).
- **เริ่มงาน (`planned → active`):** ต้องมีวันเริ่ม+สิ้นสุดและงบ Baseline อย่างน้อย 1 รายการ (`PROJECT_NOT_READY`); ระบบตรึงงบ (`budget_frozen_at_utc`) ณ ตอนนั้น. `ready_for_handover`/`completed` ต้องไม่มี Change Order สถานะ `submitted` และ `ready_for_handover` ต้องทำ Milestone ครบ.
- **งบ Baseline:** แก้ได้เฉพาะ `planned` ก่อนตรึง (`PROJECT_BUDGET_FROZEN` หลังจากนั้น); หมวด `material|labor|subcontract|service|other`, 1–100 รายการ, จำนวนเงิน ≥ 0; เก็บ `baseline_budget_total` และ `baseline_budget_hash`. ไม่ผูกกับ master price.
- **Milestone/Progress:** ความคืบหน้า = Σน้ำหนักที่เสร็จ ÷ Σน้ำหนักทั้งหมด (น้ำหนักเป็นจำนวนเต็ม 1–1000), ปัดทศนิยม 2 ตำแหน่ง. เพิ่ม/แก้/ลบได้ใน `planned|active|on_hold`; "ทำเสร็จ" ได้เฉพาะ `active`; ที่เสร็จแล้วแก้/ลบไม่ได้. Milestone มี row version ของตัวเอง ไม่หมุน version ของ Project.
- **Change Order:** สร้างได้เมื่อ `active|on_hold`; `draft → submitted → approved | rejected`, `draft|submitted → cancelled`; เลขที่จาก Document Numbering (`project-change-orders`, `PCO`). **Maker–checker:** ผู้ตัดสินต้องไม่ใช่ผู้สร้าง (`PROJECT_CHANGE_ORDER_SELF_APPROVAL`, DB check constraint ด้วย); Reject ต้องมี note. อนุมัติแล้ว `currentBudget = baseline + Σ budgetDelta`, `currentContract = baseline + Σ contractDelta`; **Baseline ไม่เปลี่ยน**. ควบคุมเกินงบ: อนุมัติที่ทำให้งบปัจจุบัน < 0 → `422 PROJECT_BUDGET_NEGATIVE`.
- ภาระผูกพัน (`budget.committedAmount`/`availableBudget`) มาจากใบสั่งซื้อที่อนุมัติ ดู [Procurement API Contract](procurement-api-contract.md); ยังไม่มี actual cost (รอ CP-11/15), ไม่มี WBS/dependency/calendar และไม่มีการแก้/ยกเลิก Change Order หลังตัดสินใจ.

Errors เพิ่มเติม: `409` `PROJECT_VERSION_CONFLICT`, `PROJECT_INVALID_STATE`, `PROJECT_BUDGET_FROZEN`, `PROJECT_INVALID_TRANSITION`, `PROJECT_NOT_READY`, `PROJECT_MILESTONE_COMPLETED`, `PROJECT_MILESTONE_VERSION_CONFLICT`, `PROJECT_CHANGE_ORDER_INVALID_STATE`, `PROJECT_CHANGE_ORDER_VERSION_CONFLICT`; `422` `PROJECT_PLAN_INVALID`, `PROJECT_BUDGET_INVALID`, `PROJECT_BUDGET_NEGATIVE`, `PROJECT_REASON_REQUIRED`, `PROJECT_MILESTONE_INVALID`, `PROJECT_CHANGE_ORDER_INVALID`; `403` `PROJECT_CHANGE_ORDER_SELF_APPROVAL`.

Data (migration `AddProjectControl`): คอลัมน์ใหม่ใน `projects.projects` (`planned_end_date`, `baseline_budget_total/hash`, `budget_frozen_at_utc`, `activated_at_utc`, `completed_at_utc`, `status_reason`) และตาราง `project_budget_lines`, `project_milestones`, `project_change_orders`, `project_status_history`.
