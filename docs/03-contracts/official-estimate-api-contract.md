# Official Estimate API Contract (ข้อตกลง API ประมาณการทางการ)

**สถานะ:** Accepted Direction — Baseline สำหรับ OpenAPI

## Common Contract

```http
Authorization: Bearer <firebase-id-token>
Accept-Language: th
If-Match: "<row-version>"       # Draft mutation
Idempotency-Key: <uuid>         # calculate/submit/cancel/revision/quotation
```

Backend สร้าง Organization/Branch Scope จาก PostgreSQL Membership ไม่เชื่อ Role/Scope จาก Client Money ใช้ Decimal String + Currency และ Error ใช้ RFC 9457

## Endpoint Matrix

| Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| สร้าง Draft | `POST /api/v1/estimates` | `estimates.create` | 201 |
| อ่าน Workspace | `GET /api/v1/estimates/{id}` | `estimates.read` | 200 |
| Autosave Draft | `PUT /api/v1/estimates/{id}/revisions/{revisionId}/draft` | `estimates.update` | 200 |
| คำนวณ | `POST /api/v1/estimates/{id}/revisions/{revisionId}/calculate` | `estimates.update` | 200 |
| อ่าน Calculation Snapshot History | `GET /api/v1/estimates/{id}/revisions/{revisionId}/calculations` | `estimates.read` | 200 |
| ส่งตรวจ | `POST /api/v1/estimates/{id}/submit` | `estimates.submit` | 200 |
| Approve/Return | `POST /api/v1/estimates/{id}/review-decisions` | `estimates.approve` | 200 |
| ยกเลิก | `POST /api/v1/estimates/{id}/cancel` | `estimates.cancel` | 200 |
| สร้าง Revision | `POST /api/v1/estimates/{id}/revisions` | `estimates.revise` | 201 |
| ออก Quotation | `POST /api/v1/estimates/{id}/quotation` | `quotations.issue` | 201 |
| ตอบรับ Quotation | `POST /api/v1/estimates/{id}/quotation/accept` | `quotations.accept` | 200 |
| อ่านเลขที่เอกสาร | `GET /api/v1/settings/document-sequences` | `document-sequences.read` | 200 |
| ทดสอบเลขที่เอกสาร | `POST /api/v1/settings/document-sequences/preview` | `document-sequences.manage` | 200 |
| ตั้งค่าเลขที่เอกสาร | `PUT /api/v1/settings/document-sequences/{documentType}` | `document-sequences.manage` | 200 |

ทุก Path ตรวจ Resource Scope; Resource นอก Scope คืน 404

## Create Draft

```http
POST /api/v1/estimates
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{
  "opportunityId": "9c5ae5f9-f02d-40ef-bd62-678a4631cd10",
  "siteSurveyRevisionId": "e4a8d0bd-c464-44c4-b0fa-eac3de2869b1",
  "currency": "THB"
}
```

```json
{
  "id": "b090c7b1-ce66-43ad-99a8-011fedfc80df",
  "number": "EST-2026-0042",
  "status": "draft",
  "currentRevision": 1,
  "etag": "est-rv-1"
}
```

Backend derives `customerId`, `branchId`, และ `siteSurveySnapshotHash` จาก Opportunity ที่อยู่ใน stage `estimating` และ Ready/Superseded Site Survey Revision ที่อยู่ใน Opportunity และ Organization เดียวกันโดยตรง Request ห้ามส่งค่าเหล่านี้มาเองเพื่อป้องกันการปลอมแปลง

## Autosave Draft

```http
PUT /api/v1/estimates/{id}/revisions/{revisionId}/draft
Content-Type: application/json
If-Match: "<expectedRevisionVersion>"
```

```json
{
  "expectedRevisionVersion": "8b584988-cb94-4363-8a3a-2325c8ceb7e6",
  "sections": [{
    "id": "a1db5bb5-9533-4af4-91c8-345289f582ee",
    "nameTh": "งาน Built-in ห้องนอนใหญ่",
    "nameEn": null,
    "sortOrder": 10,
    "workItems": [{
      "id": "c3411125-da11-4f7f-a717-aa635ea52d3f",
      "code": "WI-001",
      "descriptionTh": "ตู้เสื้อผ้า Built-in",
      "descriptionEn": null,
      "quantity": "3.00",
      "unitCode": "m",
      "sellingRule": { "type": "margin", "value": "0.30" },
      "costComponents": [{
        "id": "8cfd0cb1-a506-491e-8357-465a3fcae23c",
        "type": "material",
        "itemId": "bd0653a7-40c6-49bb-a28e-c25d90a604d0",
        "quantity": "6.00",
        "unitCode": "sheet",
        "unitCost": { "amount": "1250.00", "currency": "THB" },
        "costRecordId": "fae18682-d1d2-4700-902e-d46717c2b04a",
        "costRecordVersion": 3
      }]
    }]
  }]
}
```

Work Item ใช้ `sellingRuleType` เป็น `margin|markup|fixed_price`, `sellingRuleValue` เป็นค่า Rule และ `sellingRuleReasonCode` เป็น nullable string สูงสุด 64 ตัวอักษร. เมื่อเลือก `fixed_price`, Backend ตรวจ Permission `estimates.override-price` กับ Branch ของ Estimate และบังคับ reason code; Unauthorized คืน 403 `PERMISSION_DENIED`, reason ว่างคืน 400 `ESTIMATE_FIXED_PRICE_REASON_REQUIRED`. Readiness เพิ่ม `ESTIMATE_FIXED_PRICE_OVERRIDE` และ calculation snapshot ตรึง reason ที่ใช้ ผู้ตรวจที่ได้รับมอบหมายเห็น reason/ราคาคงที่ใน review queue; Customer Quotation projection ไม่ส่งฟิลด์นี้

ตัวอย่างตัวเลขเป็น `TEST_ONLY` `unitCost` ที่ Client ส่งเป็น Draft Input เท่านั้น Server ต้อง Resolve/Validate Cost Record และตรึง Cost/Source ใน Draft; Calculation Snapshot จะตรึง Policy และผลคำนวณ. Estimate ปัจจุบันรับ Catalog Cost เฉพาะหน่วยฐานของ Item และยังไม่ใช้ Item/Shared Unit Conversion. หากส่ง Cost Component Unit ที่ไม่ตรงหน่วยฐาน API ตอบ 422 `ESTIMATE_UNIT_INVALID` โดยไม่เขียน Draft; conversion snapshot ใน Estimate เป็นงานต่อเนื่องและห้ามตีความว่ามีการแปลงหน่วยแล้ว. Provisional Cost ใช้ Workflow/Reason แยกตาม Policy Client ห้ามส่ง Total/GP เป็นค่าที่เชื่อถือได้ Server คำนวณและคืน ETag ใหม่ `If-Match` เก่าคืน 409 `ESTIMATE_VERSION_CONFLICT`

Field, Required Gate, Precision และ Customer Visibility อ้าง [Official Estimate Field Catalog](../01-business/official-estimate-field-catalog.md) Request ที่ส่ง Derived Total, Margin, Tax หรือ Approval State ให้ Reject/Ignore ตาม Contract โดยห้ามใช้เป็นค่าจริง

Autosave เปลี่ยน `calculationOutdated` เป็น `true` และคง Calculation Snapshot ล่าสุดไว้เพื่อดูประวัติ แต่ Submit และ Issue Quotation ใช้ผลนั้นไม่ได้จน Calculate สำเร็จใหม่

## Calculate

```http
POST /api/v1/estimates/{id}/revisions/{revisionId}/calculate
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{
  "expectedRevisionVersion": "8b584988-cb94-4363-8a3a-2325c8ceb7e6",
  "discountType": "percent",
  "discountValue": 0.10,
  "discountReasonCode": "TEST_ONLY_DISCOUNT"
}
```

`discountType` รับ `none|percent|fixed-amount`; `discountValue` เป็น Rate 0–1 เมื่อเป็น percent และเป็นจำนวนเงินเมื่อเป็น fixed-amount. `discountReasonCode` ต้องระบุเมื่อมูลค่าส่วนลดมากกว่าศูนย์. `discountAmount` เป็น input compatibility เดิมสำหรับ no-discount; การสร้างส่วนลดใหม่ต้องใช้ฟิลด์ชนิด/ค่า/เหตุผลแบบ typed.

```json
{
  "revision": 2,
  "calculationVersion": 4,
  "calculationPolicyVersion": "EST-CALC-TH-v1",
  "taxPolicyVersion": "TAX-TH-v1",
  "totals": {
    "cost": { "amount": "128000.00", "currency": "THB" },
    "sellingBeforeDiscount": { "amount": "185000.00", "currency": "THB" },
    "discount": { "amount": "0.00", "currency": "THB" },
    "tax": { "amount": "12950.00", "currency": "THB" },
    "grandTotal": { "amount": "197950.00", "currency": "THB" },
    "marginRate": "0.3081"
  },
  "readiness": "requiresAttention",
  "readinessReasons": [
    { "code": "ESTIMATE_ZERO_DENOMINATOR", "targetType": "revision", "targetId": "00000000-0000-0000-0000-000000000001", "targetField": "marginRate" }
  ],
  "calculatedAtUtc": "2026-09-06T04:20:00Z"
}
```

ตัวเลขเป็น `TEST_ONLY` Calculation Snapshot ต้องทำซ้ำได้จาก Input/Cost/Rule Version เดิม

Response ของ Revision คืน `calculationOutdated: false` เมื่อ snapshot ตรงกับ Financial Input ปัจจุบัน; การแก้ Draft ภายหลังทำให้เป็น `true` จนกว่าจะ Calculate ใหม่ ส่วนลดที่มากกว่า Selling Before Discount และ Margin Rate ที่อยู่นอกช่วง `[0, 1)` ถูก Reject ด้วย `ESTIMATE_INPUT_INVALID`

`GET /api/v1/estimates/{id}/revisions/{revisionId}/calculations` คืน Snapshot ที่บันทึกแบบ append-only เรียงตาม Calculation Version โดยแต่ละรายการมี Input Hash, Policy Version, Actor และ Captured Time

## Submit and Review

```json
{ "revisionNo": 2, "calculationVersion": 4, "note": "TEST_ONLY review note" }
```

Request ใช้ `If-Match: "<estimate-row-version>"` และ `Idempotency-Key`. Submit ต้องใช้ผลคำนวณล่าสุดและ readiness ต้องไม่เป็น `blocked`; Backend ตรึง Calculation Hash/Policy Version และ route ที่มี Independent Checker หนึ่งคนตาม Bootstrap system policy; ยังไม่มี threshold วงเงินจริง. Calculate/Revision response ส่ง `readiness` (`blocked|requiresAttention|ready`) และ `readinessReasons[]` ที่มี `code`, `targetType`, `targetId`, `targetField`. Readiness ตรวจ snapshot freshness, section/work item presence, cost component presence, fixed price reason/override trigger, custom work item reason/trigger, provisional cost และ zero denominator; stale/ambiguous resolved cost ถูกปฏิเสธระหว่างการคำนวณ.

`UpdateEstimateWorkItemDto` รับ `itemId` เพื่อผูกกับ Item Master หรือ `overrideReasonCode` และ `overrideReason` สำหรับรายการงานกำหนดเอง หากไม่มี `itemId` ต้องระบุเหตุผลทั้งสองช่อง มิฉะนั้น readiness เป็น `blocked`; รายการที่มีเหตุผลครบจะเพิ่ม trigger `CUSTOM_WORK_ITEM` เพื่อเลือก approval route และผู้ตรวจเห็นเหตุผลภายใน รายละเอียด Revision ส่ง Item Master เป็น object `{ id, code, nameTh, nameEn }` พร้อม snapshot ณ เวลาบันทึก.

หากไม่มี Published Approval Policy หรือไม่มี Independent Checker ที่เข้า Permission/Scope/Authority ให้คืน 409 `ESTIMATE_POLICY_UNAVAILABLE` และไม่เปลี่ยนสถานะ

```http
POST /api/v1/estimates/{id}/review-decisions
If-Match: "<estimate-row-version>"
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{
  "revisionNo": 2,
  "decision": "returned",
  "reasonCode": "MISSING_LABOR_COST",
  "note": "เพิ่มค่าแรงติดตั้งใน WI-002"
}
```

Decision เป็น `approved` หรือ `returned`; API สำเร็จตอบ 200. Approve บันทึก Approval Snapshot และทำ Revision เป็น Immutable. Maker/Last Financial Editor ห้าม approve หรือ return. Reviewer Membership ต้องตรงกับ step ที่ assign และ permission/scope ต้องยัง active ตอนตัดสิน.

`In Review` เป็น Derived UI State จาก Approval Request/Step; Revision ยังคง `submitted` จนได้ผล `approved` หรือ `returned`

## Cancel

```http
POST /api/v1/estimates/{id}/cancel
If-Match: "<estimate-row-version>"
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{ "reason": "TEST_ONLY customer withdrew" }
```

Draft/Returned ยกเลิกได้เมื่อมี `estimates.cancel`; Submitted ต้องเป็นผู้ตรวจที่ถูก assign ใน Open Approval Route และยังมี `estimates.cancel` ภายใน scope ที่ active. Backend ปิด Approval Request และเปลี่ยน Estimate/Revision เป็น `cancelled` ใน Transaction เดียว. Approved/Quoted ยกเลิกไม่ได้และยังไม่มี Commercial cancellation flow. Same-key replay ตรวจมาก่อน ETag/state.

## New Revision

```http
POST /api/v1/estimates/{id}/revisions
If-Match: "<estimate-row-version>"
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{ "reason": "TEST_ONLY scope change" }
```

Request ปัจจุบันรับเหตุผลเท่านั้น; Source คือ Current Revision ที่ Approved หรือ Quoted. Response 201 คืน Estimate Detail ที่มี Draft Revision ใหม่และ BOQ ที่ Clone ด้วย ID/Concurrency Token ใหม่. Calculate Snapshot ถูกทิ้งใน revision ใหม่และต้อง Calculate/Submit/Approve ใหม่; Revision เดิมและ Quotation เดิมไม่เปลี่ยน. Idempotency replay ตรวจมาก่อน ETag ปัจจุบัน.

## Issue Quotation

```http
POST /api/v1/estimates/{estimateId}/quotation
Authorization: Bearer <firebase-id-token>
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{
  "expectedEstimateVersion": "8b584988-cb94-4363-8a3a-2325c8ceb7e6",
  "expectedOpportunityVersion": "7efd2427-4632-4467-85ef-96860bf53a48"
}
```

- Permission: `quotations.issue`
- Preconditions: Current Revision เป็น `approved` และมี Calculation Snapshot ที่ยังตรงกับ Revision, Opportunity อยู่ใน stage `estimating`, replay check มาก่อน version/state check
- Atomic Effects: ออกเลขที่เอกสารด้วย Atomic Sequence Engine, บันทึก Snapshot, ปรับ Estimate/Revision เป็น `quoted`, ปรับ Opportunity เป็น `proposed`, บันทึก 1 Stage History, 2 Audits, 1 Idempotency Record
- Response `201` ใช้ `QuotationResponse` และมีเฉพาะ `quotationId`, `estimateId`, `opportunityId`, `number`, `status`, `grandTotal`, `issuedAtUtc`, `estimateRevisionId`, `revisionNo`, `opportunityStage`, `opportunityRowVersion` และ `estimateRowVersion`. Integration test `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` ตรวจชื่อ property ทั้งชุดตรงกับ allowlist นี้. Response นี้เป็นผลการออก Quotation สำหรับแอปภายใน; ยังไม่มี endpoint สำหรับ customer preview/export หรือ rendered document และไม่ใช่ Customer-facing Output ตาม FR-QUO-002.

```json
{
  "quotationId": "<guid>",
  "estimateId": "<guid>",
  "opportunityId": "<guid>",
  "number": "<quotation-number>",
  "status": "issued",
  "grandTotal": 37450.00,
  "issuedAtUtc": "<timestamp>",
  "estimateRevisionId": "<guid>",
  "revisionNo": 1,
  "opportunityStage": "proposed",
  "opportunityRowVersion": "<guid>",
  "estimateRowVersion": "<guid>"
}
```

## Accept Quotation

```http
POST /api/v1/estimates/{estimateId}/quotation/accept
Authorization: Bearer <firebase-id-token>
Idempotency-Key: <16-128 chars>
Content-Type: application/json
```

```json
{
  "expectedOpportunityVersion": "7efd2427-4632-4467-85ef-96860bf53a48",
  "decisionNote": null
}
```

- Permission: `quotations.accept`
- Preconditions: Quotation อยู่ในสถานะ `issued`, Opportunity อยู่ใน stage `proposed`, replay check มาก่อน version check
- Atomic Effects: ปรับ Quotation เป็น `accepted`, ปรับ Opportunity เป็น `won`, บันทึก 1 Stage History, 2 Audits (ละเว้น `decisionNote`), 1 Idempotency Record

## Document Sequence Settings

```http
GET /api/v1/settings/document-sequences
Permission: document-sequences.read

POST /api/v1/settings/document-sequences/preview
Permission: document-sequences.manage
{ "documentType": "Quotation", "pattern": "QT-{YYYY}-{SEQ:4}", "resetPeriod": "Yearly" }

PUT /api/v1/settings/document-sequences/{documentType}
Permission: document-sequences.manage
If-Match: "<rowVersion>"
{ "pattern": "QT-{YYYY}-{SEQ:4}", "resetPeriod": "Yearly" }
```

## State/Retry Rules

- Draft/Returned แก้ได้; Submitted อ่านอย่างเดียวสำหรับ Maker; Approved/Quoted/Cancelled immutable
- Calculate/Submit/Cancel/Revision/Quotation/Accept ใช้ Idempotency Key
- Key เดิม + Payload เดิมคืนผลเดิม; Payload ต่างคืน `IDEMPOTENCY_KEY_REUSED`
- Draft เปลี่ยนหลัง Calculate ทำผลเป็น Outdated และ Submit ไม่ได้จนคำนวณใหม่
- Timeout Retry ใช้ Key เดิม ห้ามสร้าง Revision/Quotation/Audit ซ้ำ

## Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-API-EST-001` | Create จาก Customer/Opportunity โดยไม่มี Quick Estimate | 201 |
| `TC-API-EST-002` | Create อ้าง Site Survey Revision นอก Scope หรือยังไม่ Ready | 404/422 ตาม Scope/State |
| `TC-API-EST-003` | Patch ด้วย ETag ล่าสุด | 200 + ETag ใหม่ |
| `TC-API-EST-004` | Patch ด้วย ETag เก่า | 409; Draft ไม่เปลี่ยน |
| `TC-API-EST-005` | Client ส่ง Total ปลอม | ไม่ใช้ค่า Client |
| `TC-API-EST-006` | Calculate ขาด Cost Component | `blocked` + `ESTIMATE_COST_INCOMPLETE` |
| `TC-API-EST-007` | Submit ผลคำนวณ Outdated | 409 `ESTIMATE_CALCULATION_OUTDATED` |
| `TC-API-EST-008` | Maker Approve Revision ตนเอง | 403 Maker–Checker |
| `TC-API-EST-009` | Approve Revision ที่ผ่าน Policy | Immutable Approval Snapshot |
| `TC-API-EST-010` | Patch Approved Revision | 409 Invalid State |
| `TC-API-EST-011` | สร้าง Revision จาก Approved | 201 Draft ใหม่; ต้นฉบับไม่เปลี่ยน |
| `TC-API-EST-012` | ออก Quotation จาก Draft | 409 Invalid State |
| `TC-API-EST-013` | Retry Quotation ด้วย Key เดิม | คืน Quotation เดิม |
| `TC-API-EST-014` | Request ภาษาไม่รองรับ | ใช้ไทยและ Stable Code เดิม |
| `TC-API-EST-015` | ไม่มี Published Approval Policy/Checker | 409 `ESTIMATE_POLICY_UNAVAILABLE`; ยังไม่ Submit |
| `TC-API-EST-016` | Provisional Cost ไม่มีเหตุผล | 422 Field Error; ยังไม่ Submit |
| `TC-API-EST-017` | Cancel Submitted โดยไม่มี Authority | 403; Route/Revision ไม่เปลี่ยน |
| `TC-API-EST-018` | Cancel Submitted โดยมี Authority | 200 Cancelled; Open Route ปิดแบบ Atomic |
| `TC-API-EST-019` | Customer-facing Projection | ไม่มี Internal Cost/Margin/Threshold/Note |

## Data Mapping

อ่าน [Official Estimate Data Contract](../04-data/official-estimate-data-contract.md) และ [Item Master API Contract](item-master-api-contract.md)
