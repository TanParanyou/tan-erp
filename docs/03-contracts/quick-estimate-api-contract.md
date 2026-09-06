# Quick Estimate API Contract (ข้อตกลง API ราคาหน้างาน)

**สถานะ:** Accepted Direction — Baseline สำหรับ OpenAPI ก่อนเริ่ม Backend/Frontend

## ขอบเขต

เอกสารนี้กำหนด Endpoint, Payload, State Transition, Concurrency และ Idempotency ของ [Mobile Wireframe](../01-business/quick-estimate-mobile-wireframe.md) Backend เป็นผู้คำนวณราคาและตรวจ Permission/Scope ทุกครั้ง

## Common Headers

```http
Authorization: Bearer <firebase-id-token>
Accept-Language: th
X-Correlation-ID: <uuid>          # optional from client
If-Match: "<row-version>"         # required for draft mutation
Idempotency-Key: <uuid>           # required for share/conversion
```

Response ส่ง `ETag` เมื่อ Resource มี Concurrency Token และใช้ RFC 9457 Problem Details ตาม [Error Contract](error-contract.md)

## Endpoint Matrix

| Wireframe Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| เริ่ม Draft | `POST /api/v1/quick-estimates` | `quick-estimates.create` | 201 |
| เปิดรายการ | `GET /api/v1/quick-estimates/{id}` | `quick-estimates.read` | 200 |
| Autosave | `PATCH /api/v1/quick-estimates/{id}/draft` | `quick-estimates.update` | 200 |
| คำนวณ | `POST /api/v1/quick-estimates/{id}/calculate` | `quick-estimates.update` | 200 |
| ส่งตรวจ | `POST /api/v1/quick-estimates/{id}/submit-review` | `quick-estimates.update` | 202 |
| ตัดสิน Review | `POST /api/v1/quick-estimates/{id}/review-decisions` | `quick-estimates.review` | 201 |
| แชร์สรุป | `POST /api/v1/quick-estimates/{id}/shares` | `quick-estimates.share` | 201/202 |
| สร้างราคาทางการ | `POST /api/v1/quick-estimates/{id}/conversion` | `quick-estimates.convert` | 201 |
| หา Template | `GET /api/v1/pricing-templates/effective` | `pricing-templates.read` | 200 |
| เริ่มอัปโหลด | `POST /api/v1/files/upload-sessions` | ตาม Parent Resource | 201 |

ID ที่อยู่นอก Organization/Branch/Opportunity Scope คืน 404 แม้ผู้ใช้มี Permission ชื่อเดียวกัน

## Create Draft

```http
POST /api/v1/quick-estimates
Content-Type: application/json
```

```json
{
  "customerId": "5d70e0d5-f894-4da7-a591-90c81419855a",
  "opportunityId": "9c5ae5f9-f02d-40ef-bd62-678a4631cd10",
  "branchId": "6493ddaf-284b-4a98-b1c2-f715fe5c971a"
}
```

```json
{
  "id": "25db70f4-c586-4f3f-a789-4023b6022fca",
  "number": "QE-2026-000042",
  "status": "draft",
  "currentVersion": 1,
  "saveState": "saved",
  "updatedAtUtc": "2026-09-06T03:15:00Z"
}
```

Server เลือก Organization จาก Trusted Authorization Context ไม่เชื่อ `organizationId` จาก Client

## Autosave Draft

ใช้ JSON Merge Patch เฉพาะ Field ที่อนุญาต ห้ามส่ง Derived Price, Share Decision หรือ Audit Field

```http
PATCH /api/v1/quick-estimates/25db70f4-c586-4f3f-a789-4023b6022fca/draft
Content-Type: application/merge-patch+json
If-Match: "qe-rv-7"
```

```json
{
  "propertyType": "house",
  "roomOrArea": "ห้องนอนใหญ่",
  "templateVersionId": "e0b9762b-042f-4fb6-9478-e303312aeed4",
  "materialGradeId": "premium",
  "measurementConfidence": "medium",
  "measurements": [
    {
      "lineId": "4cfc5c85-9dc5-42f0-a801-50fd97e82ea4",
      "workSubtype": "wardrobe",
      "widthM": "3.00",
      "heightM": "2.60",
      "depthM": "0.60",
      "quantity": 1
    }
  ]
}
```

Response 200 คืน Draft Summary และ `ETag` ใหม่ หาก `If-Match` เก่าให้คืน 409 `QUICK_ESTIMATE_VERSION_CONFLICT` พร้อม `currentETag` ใน Extension ที่ไม่เปิดเผย Payload อื่น

## Calculate

```http
POST /api/v1/quick-estimates/{id}/calculate
If-Match: "qe-rv-8"
```

```json
{
  "expectedDraftVersion": 8
}
```

```json
{
  "quickEstimateId": "25db70f4-c586-4f3f-a789-4023b6022fca",
  "version": 2,
  "calculationId": "08c1e046-d059-4979-a309-387d70691350",
  "template": {
    "code": "QE-BI-WARDROBE-LM",
    "version": 2
  },
  "displayedRange": {
    "lower": { "amount": "43000.00", "currency": "THB" },
    "upper": { "amount": "59000.00", "currency": "THB" }
  },
  "taxDisplayPolicy": "exclusive",
  "validUntil": "2026-09-13",
  "shareDecision": "pendingReview",
  "reasonCodes": ["HIDDEN_SYSTEM", "EVIDENCE_INCOMPLETE"],
  "nextActions": ["uploadEvidence", "submitReview"]
}
```

จำนวนเงินในตัวอย่างเป็น `TEST_ONLY` JSON Money ใช้ Decimal String เพื่อไม่สูญเสีย Precision

## Review

```json
POST /api/v1/quick-estimates/{id}/submit-review
{
  "sourceVersion": 2,
  "note": "ขอตรวจงานไฟซ่อนและหลักฐานจุดติดตั้ง"
}
```

```json
POST /api/v1/quick-estimates/{id}/review-decisions
{
  "sourceVersion": 2,
  "decision": "approved",
  "reasonCode": "EVIDENCE_VERIFIED",
  "note": "ตรวจรูปและสมมติฐานแล้ว"
}
```

`decision` เป็น `approved` หรือ `returned` เท่านั้น Maker–Checker ฝ่าฝืนคืน 403 `MAKER_CHECKER_VIOLATION` การ Approve ไม่เปลี่ยนผลคำนวณย้อนหลัง

## Share

```http
POST /api/v1/quick-estimates/{id}/shares
Idempotency-Key: 29023849-9972-4c0a-b328-520229267a8e
```

```json
{
  "sourceVersion": 2,
  "channel": "onscreen",
  "recipient": null,
  "locale": "th"
}
```

Response 201 เมื่อสร้าง Customer Summary สำเร็จ หรือ 202 เมื่อช่องทางส่งทำงานภายหลัง Response ต้องคืน `shareId`, `sourceVersion`, `deliveryStatus` และ `customerSummaryUrl` ที่มีอายุสั้นเมื่อ Policy อนุญาต

ห้ามส่ง Internal Factor, Cost, Reviewer Note, Permission หรือ Threshold ใน Customer Summary

## Conversion

```http
POST /api/v1/quick-estimates/{id}/conversion
Idempotency-Key: c4986ca7-c535-40d7-87ba-6c22694f6ca6
```

```json
{ "sourceVersion": 2 }
```

```json
{
  "officialEstimateId": "b090c7b1-ce66-43ad-99a8-011fedfc80df",
  "status": "draft",
  "sourceQuickEstimateVersion": 2,
  "links": { "self": "/api/v1/estimates/b090c7b1-ce66-43ad-99a8-011fedfc80df" }
}
```

Conversion คัดลอก Scope/Measurement/Assumption เป็น Source Snapshot แต่ต้องคำนวณ Official Estimate ใหม่ ไม่ใช้ค่ากลางของ Price Range เป็นราคาขาย

## Effective Template Query

```http
GET /api/v1/pricing-templates/effective?workType=built-in&branchId=<uuid>&at=2026-09-06T03:15:00Z
```

คืนเฉพาะ Calibration/Active Version ใน Scope พร้อม Field Definition, Option, Evidence Rule และ `schemaVersion` ไม่คืน Rate/Factor ภายในที่ผู้ใช้ไม่มีสิทธิ์อ่าน

## Upload Session

Client ขอ Upload Session พร้อม `parentType=quick-estimate`, `parentId`, `evidenceSlotCode`, MIME type และ size Backend ตรวจ Scope/Policy แล้วคืนข้อมูลอัปโหลดอายุสั้น เมื่ออัปโหลดเสร็จต้องเรียก Complete/Verify ตาม File Contract ก่อน Evidence เป็น `ready`

## State and Retry Rules

- Draft mutation ต้องมี `If-Match`; ห้าม Last-write-wins
- Calculate สร้าง Version เมื่อ Input ที่มีผลต่อผลลัพธ์เปลี่ยน
- Share/Conversion บังคับ `Idempotency-Key`; Key เดิม + Payload เดิมคืนผลเดิม
- Key เดิม + Payload ต่างคืน 409 `IDEMPOTENCY_KEY_REUSED`
- Network Timeout อนุญาตให้ Retry ด้วย Key/ETag เดิม
- Action ที่ไม่ตรง State คืน 409 Stable Error Code และไม่เปลี่ยนข้อมูล

## Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-API-QE-001` | Create ด้วย Membership/Scope ถูกต้อง | 201 + ID/ETag |
| `TC-API-QE-002` | Create โดยเชื่อ Organization จาก Body | Server เพิกเฉย/ปฏิเสธและใช้ Trusted Context |
| `TC-API-QE-003` | Patch ด้วย ETag ล่าสุด | 200 + ETag ใหม่ |
| `TC-API-QE-004` | Patch ด้วย ETag เก่า | 409 Version Conflict; Draft ไม่เปลี่ยน |
| `TC-API-QE-005` | Calculate ข้อมูลไม่ครบ | 422 พร้อม Field `errors` |
| `TC-API-QE-006` | Calculate Resource นอก Scope | 404 |
| `TC-API-QE-007` | Submit Review ซ้ำจาก Network Retry | ไม่สร้าง Review Request ซ้ำ |
| `TC-API-QE-008` | Maker Approve งานตนเอง | 403 Maker–Checker |
| `TC-API-QE-009` | Share เมื่อ Blocked | 422 Review/Incomplete Code ตามเหตุผล |
| `TC-API-QE-010` | Share Key เดิมและ Payload เดิม | คืน Share เดิม |
| `TC-API-QE-011` | Share Key เดิมแต่ Payload ต่าง | 409 `IDEMPOTENCY_KEY_REUSED` |
| `TC-API-QE-012` | Convert Source Version เดิมซ้ำด้วย Key เดิม | คืน Official Estimate เดิม |
| `TC-API-QE-013` | ขอภาษาไม่รองรับ | ใช้ไทยและ Code เดิม |
| `TC-API-QE-014` | Template ถูก Disable ก่อน Calculate | 409 `PRICING_TEMPLATE_NOT_USABLE` |

## Data Mapping

Entity, Column, JSONB Snapshot และ Index ที่รองรับ Contract นี้อยู่ที่ [Quick Estimate Data Contract](../04-data/quick-estimate-data-contract.md)
