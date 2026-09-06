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
| Autosave Draft | `PATCH /api/v1/estimates/{id}/draft` | `estimates.update` | 200 |
| คำนวณ | `POST /api/v1/estimates/{id}/calculate` | `estimates.update` | 200 |
| ส่งตรวจ | `POST /api/v1/estimates/{id}/submit` | `estimates.submit` | 202 |
| Approve/Return | `POST /api/v1/estimates/{id}/review-decisions` | `estimates.approve` | 201 |
| ยกเลิก | `POST /api/v1/estimates/{id}/cancel` | `estimates.cancel` | 200 |
| สร้าง Revision | `POST /api/v1/estimates/{id}/revisions` | `estimates.update` | 201 |
| ออก Quotation | `POST /api/v1/estimates/{id}/quotation` | `quotations.issue` | 201 |

ทุก Path ตรวจ Resource Scope; Resource นอก Scope คืน 404

## Create Draft

```json
{
  "customerId": "5d70e0d5-f894-4da7-a591-90c81419855a",
  "opportunityId": "9c5ae5f9-f02d-40ef-bd62-678a4631cd10",
  "siteSurveyId": "e4a8d0bd-c464-44c4-b0fa-eac3de2869b1",
  "branchId": "6493ddaf-284b-4a98-b1c2-f715fe5c971a",
  "currency": "THB",
  "sourceQuickEstimateVersionId": null
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

`sourceQuickEstimateVersionId` เป็น Optional Official Estimate เริ่มจาก Customer/Opportunity/Site Survey โดยตรงได้

## Autosave Draft

```http
PATCH /api/v1/estimates/{id}/draft
Content-Type: application/merge-patch+json
If-Match: "est-rv-7"
```

```json
{
  "revision": 2,
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

ตัวอย่างตัวเลขเป็น `TEST_ONLY` `unitCost` ที่ Client ส่งเป็น Draft Input เท่านั้น Server ต้อง Resolve/Validate Cost Record และสร้าง Cost/Source/Conversion/Policy Snapshot ตอน Calculate; Provisional Cost ใช้ Workflow/Reason แยกตาม Policy Client ห้ามส่ง Total/GP เป็นค่าที่เชื่อถือได้ Server คำนวณและคืน ETag ใหม่ `If-Match` เก่าคืน 409 `ESTIMATE_VERSION_CONFLICT`

Field, Required Gate, Precision และ Customer Visibility อ้าง [Official Estimate Field Catalog](../01-business/official-estimate-field-catalog.md) Request ที่ส่ง Derived Total, Margin, Tax หรือ Approval State ให้ Reject/Ignore ตาม Contract โดยห้ามใช้เป็นค่าจริง

## Calculate

```json
POST /api/v1/estimates/{id}/calculate
{ "revision": 2, "expectedDraftVersion": 8 }
```

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
  "reasonCodes": ["PROVISIONAL_COST"],
  "calculatedAtUtc": "2026-09-06T04:20:00Z"
}
```

ตัวเลขเป็น `TEST_ONLY` Calculation Snapshot ต้องทำซ้ำได้จาก Input/Cost/Rule Version เดิม

## Submit and Review

```json
POST /api/v1/estimates/{id}/submit
{ "revision": 2, "calculationVersion": 4, "note": "ตรวจ BOQ และกำไร" }
```

Submit ต้องใช้ผลคำนวณล่าสุดและไม่มี Blocking Error Backend Resolve Approval Route จากยอด, Margin, Discount, Exception และ Scope

หากไม่มี Published Approval Policy หรือไม่มี Independent Checker ที่เข้า Permission/Scope/Authority ให้คืน 409 `ESTIMATE_POLICY_UNAVAILABLE` และไม่เปลี่ยนสถานะ

```json
POST /api/v1/estimates/{id}/review-decisions
{
  "revision": 2,
  "decision": "returned",
  "reasonCode": "MISSING_LABOR_COST",
  "note": "เพิ่มค่าแรงติดตั้งใน WI-002"
}
```

Decision เป็น `approved` หรือ `returned` Approve บันทึก Approval Snapshot และทำ Revision เป็น Immutable Maker ห้าม Approve งานตนเองเมื่อ Maker–Checker มีผล

`In Review` เป็น Derived UI State จาก Approval Request/Step; Revision ยังคง `submitted` จนได้ผล `approved` หรือ `returned`

## Cancel

```json
POST /api/v1/estimates/{id}/cancel
{
  "revision": 2,
  "reasonCode": "CUSTOMER_WITHDREW",
  "reason": "ลูกค้าชะลอโครงการโดยไม่มีกำหนด"
}
```

Draft/Returned ยกเลิกได้ด้วย `estimates.cancel`; Submitted ต้องมี Cancel Authority ตาม Approval Policy การยกเลิกต้องปิด Open Approval Route และเปลี่ยน Revision เป็น `cancelled` ใน Transaction เดียว Approved/Quoted ต้องสร้างกระบวนการ Commercial ที่เหมาะสม ไม่ใช้ Endpoint นี้

## New Revision

```json
POST /api/v1/estimates/{id}/revisions
{
  "sourceRevision": 2,
  "reasonCode": "CUSTOMER_SCOPE_CHANGED",
  "reason": "เพิ่มตู้เก็บของบริเวณโถง"
}
```

Response 201 คืน Draft Revision ใหม่ที่ Clone Business Snapshot แต่มี ID/Concurrency Token ใหม่ Revision เดิมไม่เปลี่ยน

## Issue Quotation

```json
POST /api/v1/estimates/{id}/quotation
{ "approvedRevision": 2, "locale": "th", "validityDays": 15 }
```

ใช้ได้เฉพาะ Approved Revision และ `quotations.issue` Response คืน `quotationId`, `number`, `estimateRevision`, `status=draft` และ Customer Snapshot Hash ห้ามออกซ้ำเมื่อ Retry ด้วย Idempotency Key เดิม

## State/Retry Rules

- Draft/Returned แก้ได้; Submitted อ่านอย่างเดียวสำหรับ Maker; Approved/Quoted/Cancelled immutable
- Calculate/Submit/Cancel/Revision/Quotation ใช้ Idempotency Key
- Key เดิม + Payload เดิมคืนผลเดิม; Payload ต่างคืน `IDEMPOTENCY_KEY_REUSED`
- Draft เปลี่ยนหลัง Calculate ทำผลเป็น Outdated และ Submit ไม่ได้จนคำนวณใหม่
- Timeout Retry ใช้ Key/ETag เดิม ห้ามสร้าง Revision/Quotation ซ้ำ

## Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-API-EST-001` | Create จาก Customer/Opportunity โดยไม่มี Quick Estimate | 201 |
| `TC-API-EST-002` | Create อ้าง Site Survey นอก Scope | 404 |
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
