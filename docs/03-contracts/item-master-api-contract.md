# Item Master API Contract (ข้อตกลง API ข้อมูลสินค้าและต้นทุน)

**สถานะ:** Accepted Direction — Baseline สำหรับ OpenAPI

## Common Contract

```http
Authorization: Bearer <firebase-id-token>
Accept-Language: th
If-Match: "<row-version>"       # update/transition
Idempotency-Key: <uuid>         # import commit และ transition สำคัญ
```

Backend สร้าง Organization/Branch Scope จาก PostgreSQL Membership ไม่เชื่อ Scope จาก Client, เงินใช้ Decimal String + Currency และ Error ใช้ RFC 9457 ตาม [Error Contract](error-contract.md)

## Endpoint Matrix

| Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| ค้นหา/อ่าน Item | `GET /api/v1/items`, `GET /api/v1/items/{id}` | `items.read` | 200 |
| อ่าน/จัดการ Category | `GET/POST /api/v1/item-categories`, `PATCH /api/v1/item-categories/{id}` | `items.read`, `items.manage-taxonomy` | 200/201 |
| อ่าน/จัดการ Brand | `GET/POST /api/v1/item-brands`, `PATCH /api/v1/item-brands/{id}` | `items.read`, `items.manage-taxonomy` | 200/201 |
| จัดการ Alias | `GET/POST /api/v1/items/{id}/aliases`, `PATCH/DELETE /api/v1/items/{id}/aliases/{aliasId}` | `items.read`, `items.update` | 200/201/204 |
| สร้าง/แก้ Item | `POST /api/v1/items`, `PATCH /api/v1/items/{id}` | `items.create`, `items.update` | 201/200 |
| เปิด/ปิดใช้ Item | `POST /api/v1/items/{id}/activate`, `/deactivate` | `items.activate`, `items.deactivate` | 200 |
| กำหนดสาขาที่ใช้ Item | `PUT /api/v1/items/{id}/branch-availability` | `items.manage-branches` | 200 |
| จัดการภาพ Item | `GET/POST /api/v1/items/{id}/images`, `PATCH/DELETE /api/v1/items/{id}/images/{imageId}` | `items.read`, `items.manage-images` | 200/201/204 |
| ค้นหา Estimate Catalog | `GET /api/v1/estimate-catalog/items` | `items.read`, `cost-records.read` | 200 |
| อ่าน/สร้าง Cost | `GET/POST /api/v1/items/{id}/cost-records` | `cost-records.read`, `cost-records.create` | 200/201 |
| ส่งตรวจ Cost | `POST /api/v1/cost-records/{id}/submit` | `cost-records.submit` | 200 |
| Approve/Return Cost | `POST /api/v1/cost-records/{id}/review-decisions` | `cost-records.approve` | 200 |
| Publish/Disable Cost | `POST /api/v1/cost-records/{id}/publish`, `/disable` | `cost-records.publish`, `cost-records.disable` | 200 |
| Resolve Cost | `GET /api/v1/items/{id}/resolved-cost` | `cost-records.read` | 200 |
| อ่าน/จัดการ Unit | `GET/POST /api/v1/units`, `PATCH /api/v1/units/{id}` | `units.read`, `units.manage` | 200/201 |
| เพิ่ม Conversion | `POST /api/v1/item-unit-conversions` | `units.manage` | 201 |
| สร้าง/อ่าน Import | `POST/GET /api/v1/item-import-batches` | `item-imports.create` | 202/200 |
| Commit Import | `POST /api/v1/item-import-batches/{id}/commit` | `item-imports.commit` | 200 |

ทุก Path ตรวจ Resource Scope; Resource นอก Scope คืน 404

## Item Example

```json
{
  "code": "MAT-PLY-18",
  "type": "material",
  "categoryId": "8a3f4e5d-2714-4cd8-a0fd-1e91fb1bb205",
  "brandId": "3b957fad-f79e-44af-82e6-d3dd12b27467",
  "name": {
    "th": "ไม้อัด 18 มม. TEST_ONLY",
    "en": "18 mm plywood TEST_ONLY"
  },
  "description": {
    "th": "วัสดุตัวอย่างสำหรับการทดสอบ",
    "en": "Test-only material"
  },
  "aliases": [
    { "th": "ไม้เขียว", "en": "Green board" }
  ],
  "baseUnitCode": "sheet",
  "availability": {
    "mode": "allBranches",
    "branchIds": []
  },
  "capabilities": {
    "canSell": true,
    "canCost": true,
    "canPurchase": true,
    "canStock": true,
    "canProduce": false
  }
}
```

Response คืน `id`, `status=draft`, `etag` และ Audit Summary Code ถูก Normalize ตาม Contract และ Unique ภายใน Organization; หลัง Activate ครั้งแรกแก้ Code ไม่ได้

Localized Object อนุญาตเฉพาะ `th`, `en`; `name.th` บังคับก่อน Activate และ API ไม่ทำ Arbitrary Fallback ระหว่างภาษา ถ้าค่าที่ร้องขอไม่มีให้คืน `null`/สถานะว่างตาม Contract

## Estimate Catalog

```http
GET /api/v1/estimate-catalog/items?branchId=6493ddaf-284b-4a98-b1c2-f715fe5c971a&search=HMR&cursor=...&pageSize=25
```

Input บังคับ `branchId`; Filter ที่รองรับใน Slice แรกคือ `search`, `itemType`, `categoryId`, `brandId`, `status=active`, `hasCost`, constrained `attributes`, `cursor`, `pageSize` และ Stable Sort `normalized_code,id` Search ครอบคลุม Code, Localized Name และ Active Alias ฝั่ง Server Backend บังคับ Active + `canCost=true` + Branch Availability และ Resolve Cost ตาม Branch โดยไม่ส่ง Cost Candidate ที่ผู้ใช้ไม่มีสิทธิ์อ่าน

```json
{
  "items": [
    {
      "id": "bd0653a7-40c6-49bb-a28e-c25d90a604d0",
      "code": "MAT-PLY-18",
      "name": { "th": "ไม้อัด 18 มม. TEST_ONLY", "en": "18 mm plywood TEST_ONLY" },
      "description": { "th": "วัสดุตัวอย่างสำหรับการทดสอบ", "en": null },
      "itemType": "material",
      "category": { "id": "8a3f4e5d-2714-4cd8-a0fd-1e91fb1bb205", "name": { "th": "ไม้", "en": "Wood" } },
      "brand": { "id": "3b957fad-f79e-44af-82e6-d3dd12b27467", "name": { "th": "วนชัย", "en": "Vanachai" } },
      "baseUnit": { "id": "230e6ca4-09c1-4b8c-8c23-8af359981ee8", "code": "sheet", "symbol": "แผ่น" },
      "attributes": { "thickness": "18mm" },
      "primaryImage": { "fileId": "7a27e878-987b-4f46-8146-b28f08f55dc5", "altText": { "th": "แผ่นไม้อัด", "en": "Plywood sheet" } },
      "resolvedCost": {
        "costRecordId": "fae18682-d1d2-4700-902e-d46717c2b04a",
        "version": 3,
        "amount": "1250.0000",
        "currency": "THB",
        "unitCode": "sheet",
        "scope": "branch",
        "effectiveFromUtc": "2026-09-06T00:00:00Z",
        "policyVersion": "COST-RESOLVE-v1"
      }
    }
  ],
  "facets": {
    "itemTypes": [{ "value": "material", "count": 1 }],
    "categories": [{ "id": "8a3f4e5d-2714-4cd8-a0fd-1e91fb1bb205", "name": { "th": "ไม้", "en": "Wood" }, "count": 1 }],
    "brands": [{ "id": "3b957fad-f79e-44af-82e6-d3dd12b27467", "name": { "th": "วนชัย", "en": "Vanachai" }, "count": 1 }]
  },
  "pageInfo": { "nextCursor": null, "hasNextPage": false }
}
```

Catalog Response เป็น Structured Projection จาก Backend; Frontend ห้ามโหลด Master ทั้งหมดแล้ว `find` ความสัมพันธ์เอง `primaryImage.fileId` อ่านผ่าน Authorized File Content API และ URL ที่ Client สร้างไม่ถือเป็น Persistent Data

## Item Image Upload and Attachment

1. สร้าง Item Draft ก่อนเพื่อให้มี `itemId`
2. สร้าง File Upload Session ด้วย `parentType=item`, `parentId=itemId`, `creationIntent=false`
3. Complete Upload ให้ได้ Verified `fileId`
4. `POST /api/v1/items/{id}/images` ด้วย `fileId`, `role`, `isPrimary`, `displayOrder`, `altText`, `caption`
5. Backend ตรวจ Organization, Parent Invariant, MIME/Status และเขียน Item Image + Audit แบบ Atomic

การเลือกไฟล์ใน Form ต้องยังไม่ Upload จนผู้ใช้ Submit ตาม Deferred File Upload Rule

## Cost Record Example

```json
{
  "sourceId": "44b0e87b-e7b4-4dc3-849c-8c449dcb6263",
  "scope": { "type": "branch", "branchId": "6493ddaf-284b-4a98-b1c2-f715fe5c971a" },
  "unitCode": "sheet",
  "currency": "THB",
  "amount": "1250.00",
  "minimumQuantity": "1.0000",
  "maximumQuantity": null,
  "effectiveFromUtc": "2026-09-06T00:00:00Z",
  "effectiveToUtc": null,
  "evidenceFileId": "7a27e878-987b-4f46-8146-b28f08f55dc5",
  "reason": "Supplier quote TEST_ONLY"
}
```

ตัวเลขทั้งหมดเป็น `TEST_ONLY` Draft/Returned แก้ได้; Published/Superseded/Disabled เป็น Immutable และ Maker/Last Financial Editor อนุมัติ Version เดียวกันไม่ได้

## Resolve Cost

```http
GET /api/v1/items/{id}/resolved-cost?branchId=...&unitCode=sheet&currency=THB&quantity=6.0000&effectiveAt=2026-09-06T04:20:00Z
```

```json
{
  "itemId": "bd0653a7-40c6-49bb-a28e-c25d90a604d0",
  "costRecordId": "fae18682-d1d2-4700-902e-d46717c2b04a",
  "costRecordVersion": 3,
  "sourceType": "supplierQuote",
  "original": { "amount": "1250.00", "currency": "THB", "unitCode": "sheet" },
  "resolved": { "amount": "1250.00", "currency": "THB", "unitCode": "sheet" },
  "scope": "branch",
  "effectiveFromUtc": "2026-09-06T00:00:00Z",
  "stale": false,
  "policyVersion": "COST-RESOLVE-v1"
}
```

Resolver ใช้ลำดับใน [Item Master Governance](../01-business/item-master-governance.md) หากไม่พบคืน `ITEM_COST_NOT_FOUND`; ถ้ายังเสมอกันหลังทุก Rule คืน `ITEM_COST_AMBIGUOUS` พร้อม Candidate ID ที่ผู้มีสิทธิ์เท่านั้นอ่านได้

## Import Contract

1. Client Upload ไฟล์ผ่าน File Service แล้วส่ง `fileId`, `templateVersion`, `mode=createOnly|upsert` เพื่อสร้าง Batch
2. Backend Parse แบบ Data-only และคืนสถานะ `parsing|invalid|readyToCommit`
3. `GET /item-import-batches/{id}` คืน Summary และ Error แบบ Page; ไม่คืนข้อมูล Cost ที่ผู้ใช้ไม่มีสิทธิ์อ่าน
4. Commit รับ `expectedBatchVersion` และ Idempotency Key; Batch ต้อง `readyToCommit` และไม่มี Error
5. Commit เป็น Atomic; Cost ที่ Import เข้ามายังเป็น Draft ไม่ Publish อัตโนมัติ

## State, Concurrency and Retry

- Mutation ใช้ ETag/`If-Match`; ค่าเก่าคืน 409 โดยไม่เขียนบางส่วน
- Transition/Commit ใช้ Idempotency Key; Key เดิม + Payload เดิมคืนผลเดิม, Payload ต่างคืน `IDEMPOTENCY_KEY_REUSED`
- List ใช้ Cursor + Stable Tie-breaker `id`, filter ตาม code/name/type/category/status/capability/costState
- API ไม่รับ Current Cost เป็น Field ของ Item; Current Cost เป็นผลจาก Resolve
- Estimate ต้องเก็บ Cost Record/Conversion/Policy Snapshot ที่ใช้จริง ไม่เรียก Current Cost เพื่อเปลี่ยนอดีต
- เมื่อเพิ่ม Catalog Item เข้า Estimate Client ส่ง `itemId`, `costRecordId`, `quantity`, `unitCode`; Backend Resolve/Validate ราคาใหม่ก่อนบันทึก Snapshot และไม่เชื่อ `unitCost` จาก Client

## Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-API-ITEM-001` | สร้าง Draft ที่ Code ไม่ซ้ำ | 201 + ETag |
| `TC-API-ITEM-002` | Activate โดย Base Unit ไม่ครบ | 422 `ITEM_FIELD_REQUIRED` |
| `TC-API-ITEM-003` | แก้ Code หลังเคย Active | 409 `ITEM_INVALID_STATE` |
| `TC-API-ITEM-004` | Patch ด้วย ETag เก่า | 409; ข้อมูลไม่เปลี่ยน |
| `TC-API-ITEM-005` | Maker อนุมัติ Cost ตนเอง | 403 `MAKER_CHECKER_VIOLATION` |
| `TC-API-ITEM-006` | Publish Period ซ้อนใน Scope เดียวกัน | 409 `ITEM_COST_PERIOD_OVERLAP` |
| `TC-API-ITEM-007` | Resolve มี Branch Override | คืน Branch Record |
| `TC-API-ITEM-008` | Resolve ไม่มี Candidate | 404 `ITEM_COST_NOT_FOUND` |
| `TC-API-ITEM-009` | Resolve Candidate เสมอกัน | 409 `ITEM_COST_AMBIGUOUS` |
| `TC-API-ITEM-010` | Conversion ข้าม Dimension | 422 `ITEM_CONVERSION_INVALID` |
| `TC-API-ITEM-011` | Import มี Error หนึ่งแถว | Batch invalid; Commit ไม่ได้ |
| `TC-API-ITEM-012` | Retry Commit ด้วย Key เดิม | คืนผลเดิม ไม่สร้างซ้ำ |
| `TC-API-ITEM-013` | ผู้ใช้ข้าม Organization | 404 + Security Audit |
| `TC-API-ITEM-014` | ภาษาไม่รองรับ | ข้อความไทย + Stable Code |
| `TC-API-ITEM-015` | Client ส่ง `currentCost` ใน Item | Reject/Ignore; ไม่ใช้เป็นค่าจริง |
| `TC-API-ITEM-016` | Catalog ขอ Item ที่ไม่เปิดใช้ใน Branch | ไม่คืนรายการและไม่เปิดเผยว่ามี Item |
| `TC-API-ITEM-017` | Catalog Resolve ได้ Branch Override | คืน Branch Cost และ Version ที่แน่นอน |
| `TC-API-ITEM-018` | Attach File ข้าม Organization/Parent/ยังไม่ Verified | 404/409 ตาม Contract; ไม่สร้าง Relation |
| `TC-API-ITEM-019` | เปลี่ยน Primary Image พร้อมกัน | หนึ่ง Request สำเร็จ อีก Request ได้ Conflict |
| `TC-API-ITEM-020` | Update Estimate ด้วยราคาจาก Client ที่ล้าสมัย | 409 `ITEM_COST_VERSION_CONFLICT`; ไม่บันทึกบางส่วน |
| `TC-API-ITEM-021` | ค้นด้วย Alias ไทย/อังกฤษ | คืน Item เดียวโดยไม่เปลี่ยน Display Name |
| `TC-API-ITEM-022` | Filter Category ลูกหรือ Brand | คืนเฉพาะ Structured Projection ที่ตรง Scope |
| `TC-API-ITEM-023` | Supplier filter ก่อน Supplier Master พร้อม | Contract ไม่รับ Parameter และคืน Validation Error |

## Data Mapping

อ่าน [Item Master Data Contract](../04-data/item-master-data-contract.md)
