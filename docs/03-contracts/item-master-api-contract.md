# Item Master API Contract (ข้อตกลง API ข้อมูลสินค้าและต้นทุน)

**สถานะ:** Accepted Direction — Baseline สำหรับ OpenAPI

## Common Contract

```http
Authorization: Bearer <firebase-id-token>
Accept-Language: th
If-Match: "<row-version>"       # update/transition
Idempotency-Key: <uuid>         # create และ transition สำคัญตาม OpenAPI
```

Backend สร้าง Organization/Branch Scope จาก PostgreSQL Membership ไม่เชื่อ Scope จาก Client, เงินใช้ Decimal String + Currency และ Error ใช้ RFC 9457 ตาม [Error Contract](error-contract.md)

## Endpoint Matrix

| Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| ค้นหา/อ่าน Item | `GET /api/v1/items`, `GET /api/v1/items/{id}` | `items.read` | 200 |
| อ่าน/จัดการ Category | `GET/POST /api/v1/item-categories`, `GET/PUT /api/v1/item-categories/{id}` | `items.read`, `items.manage-taxonomy` | 200/201 |
| อ่าน/จัดการ Brand | `GET/POST /api/v1/item-brands`, `GET/PUT /api/v1/item-brands/{id}` | `items.read`, `items.manage-taxonomy` | 200/201 |
| อ่าน/จัดการ Tax Category | `GET/POST /api/v1/item-tax-categories`, `GET/PUT /api/v1/item-tax-categories/{id}` | `items.read`, `items.manage-taxonomy` | 200/201 |
| จัดการ Alias | `POST /api/v1/items/{id}/aliases`, `DELETE /api/v1/items/{id}/aliases/{aliasId}` | `items.update` | 201/204 |

### Master Data Code Creation

`code` ในคำขอ Create ของ Item, Category, Brand, Unit of Measure, Tax Category และ Cost Source เป็น optional/nullable: ไม่ส่งหรือส่ง `null` เพื่อให้ระบบออกเลขเมื่อบันทึกสำเร็จ; ส่งรหัสที่ไม่ว่างเพื่อกำหนดเอง รหัสที่กำหนดเองซ้ำในองค์กรตอบ `409` ด้วย structured error (`ITEM_CODE_CONFLICT` หรือ `COST_SOURCE_CODE_DUPLICATE`) ส่วนรหัส GEN ตรวจรหัสเดิมและข้ามเลขที่ถูกใช้แล้วก่อนบันทึก

| Master data | Numbering type | Default example | Scope / reset |
| --- | --- | --- | --- |
| Item (ทุกประเภทใช้ชุดเดียว) | `items` | `ITM-00001` | ทั้งองค์กร / ไม่ reset |
| Category | `item-categories` | `CAT-00001` | ทั้งองค์กร / ไม่ reset |
| Brand | `item-brands` | `BRD-00001` | ทั้งองค์กร / ไม่ reset |
| Unit of Measure | `units-of-measure` | `UOM-00001` | ทั้งองค์กร / ไม่ reset |
| Tax Category | `item-tax-categories` | `TAX-00001` | ทั้งองค์กร / ไม่ reset |
| Cost Source | `cost-sources` | `SRC-00001` | ทั้งองค์กร / ไม่ reset |

ตัวอย่างเป็นเลข preview เท่านั้น ไม่ได้จองหรือเพิ่ม counter; เลขจริงถูกจัดสรรใน transaction เดียวกับการบันทึกข้อมูล, idempotency record และ audit event. Customer numbering และเลขเอกสารธุรกรรมยังคงพฤติกรรมเดิม.
| สร้าง/แก้ Item | `POST /api/v1/items`, `PUT /api/v1/items/{id}` | `items.create`, `items.update` | 201/200 |
| เปิด/ปิดใช้ Item | `POST /api/v1/items/{id}/activate`, `/deactivate` | `items.activate`, `items.deactivate` | 200 |
| กำหนดสาขาที่ใช้ Item | `PUT /api/v1/items/{id}/branch-availability` | `items.manage-branches` | 200 |
| อ่านสาขาองค์กรที่เปิดใช้งาน | `GET /api/v1/branches` | `organizations.read` | 200 |
| จัดการภาพ Item | `GET/POST /api/v1/items/{id}/images`, `POST /api/v1/items/{id}/images/{imageId}/primary`, `PUT /api/v1/items/{id}/images/reorder`, `DELETE /api/v1/items/{id}/images/{imageId}` | `items.read`, `items.manage-images` | 200/201/204 |
| จัดการ Barcode | `GET/POST /api/v1/items/{id}/barcodes`, `POST /api/v1/items/{id}/barcodes/{barcodeId}/primary`, `POST /api/v1/items/{id}/barcodes/{barcodeId}/deactivate` | `items.read`, `items.manage-barcodes` | 200/201 |
| ค้นด้วย Barcode ที่สแกน | `GET /api/v1/items/by-barcode?value=...` | `items.read` | 200 |
| ค้นหา Estimate Catalog | `GET /api/v1/estimate-catalog/items` | `items.read`, `cost-records.read` | 200 |
| อ่าน/จัดการ Manual Cost Source | `GET/POST /api/v1/cost-sources`, `GET/PUT /api/v1/cost-sources/{id}`, `POST /api/v1/cost-sources/{id}/deactivate` | `cost-sources.read`, `cost-sources.manage` | 200/201 |
| อ่าน/สร้าง/แก้ Cost | `GET/POST /api/v1/items/{id}/costs`, `GET/PUT /api/v1/items/{id}/costs/{costId}` | `cost-records.read`, `cost-records.create` | 200/201 |
| ส่งตรวจ Cost | `POST /api/v1/items/{id}/costs/{costId}/submit` | `cost-records.submit` | 200 |
| Approve/Return Cost | `POST /api/v1/items/{id}/costs/{costId}/approve`, `/return` | `cost-records.approve` | 200 |
| Publish/Disable Cost | `POST /api/v1/items/{id}/costs/{costId}/publish`, `/disable` | `cost-records.publish`, `cost-records.disable` | 200 |
| Resolve Cost | `GET /api/v1/items/{id}/resolved-cost` | `cost-records.read` | 200 |
| อ่าน/จัดการ Unit | `GET/POST /api/v1/units-of-measure`, `GET/PUT /api/v1/units-of-measure/{id}` | `units.read`, `units.manage` | 200/201 |

ทุก Path ตรวจ Resource Scope; Resource นอก Scope คืน 404

Create/Update ของ Category ใช้ `code` ไม่เกิน 30 ตัว, `name`, `description?`, `parentCategoryId?`, `allowedItemTypes` และ `sortOrder`; Update เป็นการบันทึกค่าฟิลด์ทั้งหมดและเพิ่ม `imageFileId?` ดังนั้น Client ต้องส่งค่าเดิมของ Parent, Allowlist, Sort Order และ Image File ID มาด้วยเมื่อแก้เพียงบางฟิลด์. Brand ใช้ `code` ไม่เกิน 30 ตัว, `name`, `description?` และ `sortOrder`; Update เพิ่ม `imageFileId?`. ภาพแนบได้หนึ่งภาพต่อรายการ: สร้างรายการก่อน จากนั้นสร้าง Upload Session ด้วย parent type `item-category` หรือ `item-brand`, อัปโหลด/ตรวจสอบไฟล์ แล้วบันทึก ID ด้วย `PUT` และ ETag ล่าสุด; การถอดภาพให้ส่ง `imageFileId: null`. Upload จริงทำหลังผู้ใช้กดบันทึกเท่านั้น. Unit ใช้ `code` ไม่เกิน 20 ตัว, `name`, `symbol` ไม่เกิน 16 ตัว, `dimension`, `decimalScale` 0–6 และ `roundingMode` จาก enum `half_up`, `half_even`, `up`, `down`, `ceiling`, `floor`; API ปฏิเสธค่าอื่น และ Update ต้องรักษา precision/rounding เดิมไว้เมื่อแก้ฟิลด์อื่น. UI แสดงคำอธิบายที่แปลแล้วแทน enum code.

`GET /api/v1/items` รองรับ `search`, `itemType`, `categoryId`, `brandId`, `status`, `sortBy`, `sortOrder`, `pageNumber` และ `pageSize`. `sortBy` รับ `code`, `itemType` หรือ `status`; `sortOrder` รับ `asc`/`desc`; ค่าเริ่มต้นคือ `code asc`. การเรียงใช้ Item ID เป็น stable tie-breaker และ `pageSize` ถูกจำกัดไม่เกิน 100 (ค่าเริ่มต้น 25). Item Import มี Phase 1 `createOnly` แบบ CSV ตาม [Import Contract](#import-contract).

## Planned Item Identity and Barcode Contract

`Item.code` เป็น Item Code ภายในและ SKU สำหรับสินค้าที่ซื้อหรือเก็บได้; ไม่มี request/response field `sku` อีกชุด. `GET /items` และ Estimate Catalog ต้องค้น Code/SKU ด้วย field เดิม. Barcode เป็น optional child resource:

| Action | Planned path | Permission |
| --- | --- | --- |
| อ่าน Barcode ของ Item | `GET /api/v1/items/{id}/barcodes` | `items.read` |
| เพิ่ม Barcode | `POST /api/v1/items/{id}/barcodes` | `items.manage-barcodes` |
| ตั้ง Primary ของระดับบรรจุ | `POST /api/v1/items/{id}/barcodes/{barcodeId}/primary` | `items.manage-barcodes` |
| ปิดใช้ Barcode | `POST /api/v1/items/{id}/barcodes/{barcodeId}/deactivate` | `items.manage-barcodes` |
| ค้นด้วยค่าที่สแกน | `GET /api/v1/items/by-barcode?value=...` | `items.read` |
| อ่าน/เพิ่ม Item Unit Conversion | `GET/POST /api/v1/items/{itemId}/unit-conversions` | `items.read`, `items.manage-barcodes` |
| อ่าน/เพิ่ม Shared Unit Conversion | `GET/POST /api/v1/unit-conversions` | `units.read`, `units.manage` |

Create รับ `{ identifierType, value, unitId, quantityInBaseUnit, packagingLevel, isPrimary }`; Response คืน field เดียวกันพร้อม `id`, `itemId`, `status`, `rowVersion`. Create ต้องมี `Idempotency-Key`; เปลี่ยน Primary/ปิดใช้ต้องมี `If-Match`. Value/Item/Unit/Quantity ที่เคยถูกใช้ไม่แก้ทับ; ปิดรายการเดิมแล้วเพิ่มรายการใหม่. Backend ตรวจ Organization/Branch Scope, GTIN check digit และ Unique Value แล้วคืน RFC Problem Details ด้วย stable code เช่น `ITEM_BARCODE_CONFLICT`, `ITEM_BARCODE_INVALID`, `ITEM_BARCODE_UNIT_INVALID`. ค้นด้วย Barcode คืน structured Item + Unit + Quantity ที่ตรงเพียงรายการเดียว; ค่าไม่พบหรือ Inactive คืน 404 โดยไม่เดา Item จาก SKU หรือ Alias

Barcode ที่ `unitId` เท่ากับ Base Unit ใช้ `quantityInBaseUnit` เป็นตัวคูณของบรรจุได้ทันที เช่น ลัง 12 แผ่นคืน `12 sheet`. หาก `unitId` ต่างจาก Base Unit ต้องมี Item Unit Conversion หรือ Shared Unit Conversion ที่ active/effective และจำนวนใน Barcode ต้องตรงกับ Factor; Barcode บันทึกจำนวนหน่วยฐานเป็น snapshot จึงไม่เปลี่ยนตาม conversion รุ่นใหม่. Item Unit Conversion แปลงตรงไปยัง Base Unit เท่านั้นจึงไม่มีวงจร; Shared Unit Conversion ใช้ได้เมื่อ From/To อยู่ Dimension เดียวกันและ Backend ปฏิเสธวงจรในกราฟ. ทั้งสองชนิดต้องใช้หน่วยที่ active ในองค์กรเดียวกัน, ห้ามช่วงวันที่ซ้อนของคู่เดียวกัน และแก้ด้วยการเพิ่มรุ่นใหม่โดยไม่ overwrite ข้อมูลเดิม. Create ใช้ `Idempotency-Key`; Unit Conversion ทั้งสองชนิดเป็น immutable version.

## Manual Cost Source Contract

`POST /api/v1/cost-sources` รับ `code`, `name: { th, en }`, `sourceType: "manual"`; `priority` เป็น system metadata ไม่รับจาก UI. Response คืน `id`, `code`, `name`, `sourceType`, `isActive`, `rowVersion`. `PUT` ใช้ `If-Match` และแก้ชื่อได้; Code/Type เปลี่ยนไม่ได้หลังถูก Cost Record อ้างอิง. Deactivate ใช้ `If-Match` และเก็บ Audit; Source เดิมยังอ่านได้ใน Cost history แต่เลือกกับ Cost ใหม่ไม่ได้. Source ที่ migration ระบุเป็น `legacy` อ่านย้อนหลังได้แต่ห้ามเลือกกับ Cost ใหม่. ทุกคำสั่งใช้ Organization Scope และ RFC Problem Details; Cross-organization ID ตอบ 404

Cost Record ใหม่ต้องมี `costSourceId` เป็น Active Manual Source ของ Organization เดียวกัน. `reason` ต้องไม่ว่าง และต้องมี `sourceReference` หรือ `evidenceFileId` ที่ Verified และผูกกับ Cost Record นี้. เนื่องจาก Draft ต้องมี ID ก่อนอัปโหลดไฟล์ กระบวนการแนบไฟล์คือสร้าง Draft ด้วย `sourceReference` ชั่วคราวที่ตรวจสอบได้ → อัปโหลดด้วย `parentType=costRecord`, `parentId=costId` → `PUT` Draft ด้วย `evidenceFileId` ก่อน Submit; ไม่รับ File ID ข้าม Parent. Draft เก่าที่ไม่มี Source ยังอ่านได้ แต่ Submit/Publish ไม่ผ่าน Gate จนแก้ให้ครบ หรือสร้างรุ่นใหม่หาก immutable แล้ว

`GET /api/v1/cost-records/review-queue?status=submitted&pageNumber=1&pageSize=20` เป็น endpoint ใหม่สำหรับ Reviewer; คืนรายการแบบ page ที่มี Item, Cost Source, Unit, Maker, Last Financial Editor, Amount, Effective Period, Status และ ETag เป็น structured objects. จำกัดข้อมูลตาม `cost-records.approve` และ Organization Scope; Frontend ห้ามโหลด Cost ทุก Item แล้วประกอบคิวเอง

## Item Example

```json
{
  "code": "MAT-PLY-18",
  "itemType": "material",
  "categoryId": "8a3f4e5d-2714-4cd8-a0fd-1e91fb1bb205",
  "brandId": "3b957fad-f79e-44af-82e6-d3dd12b27467",
  "name": {
    "thai": "ไม้อัด 18 มม. TEST_ONLY",
    "english": "18 mm plywood TEST_ONLY"
  },
  "description": {
    "thai": "วัสดุตัวอย่างสำหรับการทดสอบ",
    "english": "Test-only material"
  },
  "aliases": [
    { "thai": "ไม้เขียว", "english": "Green board" }
  ],
  "baseUnitId": "11111111-1111-4111-8111-111111111111",
  "availabilityMode": "all_branches",
  "selectedBranchIds": [],
  "capabilities": {
    "canSell": true,
    "canCost": true,
    "canPurchase": true,
    "canStock": false,
    "canProduce": false
  }
}
```

Response คืน `id`, `status=draft`, `etag` และ Audit Summary Code ถูก Normalize ตาม Contract และ Unique ภายใน Organization; หลัง Activate ครั้งแรกแก้ Code ไม่ได้

API Localized Text ใช้ `{ thai, english }` ตาม DTO ปัจจุบัน; persistence เก็บ JSONB `{ th, en }` ตาม Data Contract โดย mapper เป็นเจ้าของการแปลง. `name.thai` บังคับก่อน Activate และ API ไม่ทำ Arbitrary Fallback ระหว่างภาษา ถ้าค่าที่ร้องขอไม่มีให้คืน `null`/สถานะว่างตาม Contract

## Estimate Catalog

```http
GET /api/v1/estimate-catalog/items?branchId=6493ddaf-284b-4a98-b1c2-f715fe5c971a&search=HMR&cursor=...&pageSize=25
```

Input บังคับ `branchId`; Filter ที่รองรับคือ `search`, `itemType`, `categoryId`, `brandId`, `attributeKey`, `attributeValue`, `hasCost`, `cursor`, `pageSize` และ Stable Sort `normalized_code,id` ส่ง `attributeKey` เพื่อกรองรายการที่มี key นั้น และส่ง `attributeValue` เพิ่มเพื่อจับคู่ค่าแบบตรงกัน Facets ของ Attributes คืนคู่ `key`, `value`, `count` จากรายการที่ผ่าน filter อื่นทั้งหมดก่อน cursor และก่อนใช้ attribute filter ค่าเริ่มต้นแสดงเฉพาะ Item ที่มี Published Cost สำหรับหน่วยฐานและจำนวน 1; ส่ง `hasCost=false` เพื่อดูรายการที่ยังไม่มีราคา Search ครอบคลุม Code, Localized Name และ Active Alias ฝั่ง Server Backend บังคับ Active + `canCost=true` + Branch Availability และ Resolve Cost ตาม Branch โดยไม่ส่ง Cost Candidate ที่ผู้ใช้ไม่มีสิทธิ์อ่าน Facets นับจากชุดที่ผ่าน filter ทั้งหมดก่อน cursor

```json
{
  "items": [
    {
      "id": "bd0653a7-40c6-49bb-a28e-c25d90a604d0",
      "code": "MAT-PLY-18",
      "name": { "th": "ไม้อัด 18 มม. TEST_ONLY", "en": "18 mm plywood TEST_ONLY" },
      "description": { "th": "วัสดุตัวอย่างสำหรับการทดสอบ", "en": null },
      "itemType": "material",
      "costComponentType": "material",
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
    "brands": [{ "id": "3b957fad-f79e-44af-82e6-d3dd12b27467", "name": { "th": "วนชัย", "en": "Vanachai" }, "count": 1 }],
    "attributes": [{ "key": "thickness_mm", "value": "18", "count": 1 }]
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
  "costSourceId": "44b0e87b-e7b4-4dc3-849c-8c449dcb6263",
  "scope": "organization",
  "branchId": null,
  "unitId": "230e6ca4-09c1-4b8c-8c23-8af359981ee8",
  "currency": "THB",
  "amount": "1250.00",
  "minimumQuantity": "0.0000",
  "maximumQuantity": null,
  "effectiveFromUtc": "2026-09-06T00:00:00Z",
  "effectiveToUtc": null,
  "sourceReference": "MANUAL-TEST-001",
  "evidenceFileId": null,
  "reason": "บันทึกต้นทุนด้วยมือเพื่อทดสอบ TEST_ONLY"
}
```

ตัวเลขทั้งหมดเป็น `TEST_ONLY` Draft/Returned แก้ได้; Published/Superseded/Disabled เป็น Immutable และ Maker/Last Financial Editor อนุมัติ Version เดียวกันไม่ได้

Cost ทุกคำสั่งแก้ไข/เปลี่ยนสถานะต้องมี `If-Match` เป็น UUID ที่ใส่เครื่องหมายคำพูด ขาดหรือผิดรูปแบบคืน 428; ETag เก่าคืน `ITEM_COST_VERSION_CONFLICT` โดยไม่เขียนข้อมูล Create และ Publish ต้องมี `Idempotency-Key` ยาว 16–128 ตัวอักษร Publish อนุญาตเฉพาะ `Approved` เท่านั้น Publish ปฏิเสธช่วงเวลาและช่วงจำนวนที่ซ้อนกับ Published Cost ใน scope/unit/currency เดียวกัน ยกเว้นต้นทุนใหม่ที่มีช่วงจำนวนเท่ากันและวันที่เริ่มใหม่กว่าเพื่อ supersede ต้นทุนเดิม โดยต้นทุนใหม่ต้องครอบคลุมจนถึงวันสิ้นสุดเดิมเพื่อไม่ให้เกิดช่องว่าง เมื่อ supersede จะปิด `effectiveToUtc` ของต้นทุนเดิมก่อนวันเริ่มต้นทุนใหม่หนึ่งไมโครวินาที; resolver ยังอ่านสถานะ `Superseded` ได้เฉพาะช่วงประวัติศาสตร์ที่มีผลจริง

## Resolve Cost

```http
GET /api/v1/items/{id}/resolved-cost?branchId=...&unitCode=sheet&currency=THB&quantity=6.0000&effectiveAt=2026-09-06T04:20:00Z
```

```json
{
  "itemId": "bd0653a7-40c6-49bb-a28e-c25d90a604d0",
  "costRecordId": "fae18682-d1d2-4700-902e-d46717c2b04a",
  "costRecordVersion": 3,
  "sourceType": "manual",
  "original": { "amount": "1250.00", "currency": "THB", "unitCode": "sheet" },
  "resolved": { "amount": "1250.00", "currency": "THB", "unitCode": "sheet" },
  "scope": "branch",
  "effectiveFromUtc": "2026-09-06T00:00:00Z",
  "stale": false,
  "policyVersion": "COST-RESOLVE-v1"
}
```

Resolver เรียง Branch scope ก่อน Organization scope จากนั้นเลือก `EffectiveFromUtc` ล่าสุดและ `MinimumQuantity` สูงสุดที่ครอบคลุมจำนวนที่ร้องขอ ถ้ายังมีผู้ชนะมากกว่าหนึ่งรายการคืน `ITEM_COST_AMBIGUOUS` โดยไม่ใช้ Source Priority, Version หรือ ID ตัดสิน หากไม่พบคืน `ITEM_COST_NOT_FOUND` Catalog นับเฉพาะ Item ที่มีผู้ชนะราคาเพียงหนึ่งรายการก่อนแบ่งหน้าและคำนวณ Facets

## Import Contract

### Phase 1 — implemented 2026-10-04 (`createOnly`, CSV, synchronous)

ทีมพัฒนาเลือกขอบเขตเริ่มต้นนี้แทน Batch/File Service/`upsert` ด้านล่าง (ผู้ใช้มอบหมายให้ตัดสินใจ) ยังไม่ผ่าน Item Master Owner/Data Steward; ส่วนที่ไม่ทำคือ Future.

| Action | Method/Path | Permission | Success |
| --- | --- | --- | --- |
| Preview/Validate | `POST /api/v1/items/imports/preview` body `{ content }` | `items.create` | 200 |
| Commit | `POST /api/v1/items/imports/commit` body `{ content, expectedContentSha256? }` + `Idempotency-Key` | `items.create` | 201 |

- ไฟล์เป็น CSV UTF-8 (BOM ได้) RFC 4180, ส่งเนื้อหาเป็น text ใน JSON (ไม่เก็บไฟล์), ≤ 1,000,000 ตัวอักษร, ≤ 500 แถวข้อมูล. Header ต้องตรงตามลำดับ: `code,itemType,categoryCode,brandCode,baseUnitCode,nameTh,nameEn,descriptionTh,descriptionEn,taxCategoryCode,canSell,canCost,canPurchase,canStock,canProduce`. ผิดทั้งไฟล์ → `422 ITEM_IMPORT_FILE_INVALID`.
- Preview ไม่เขียนข้อมูล; คืน `{ contentSha256, totalRows, validRows, invalidRows, rows: [{ rowNumber, code, nameTh, isValid, errors: [{ field, code }] }] }`. error code: `REQUIRED`, `INVALID`, `TOO_LONG`, `NOT_FOUND`, `INACTIVE`, `DUPLICATE_IN_FILE`, `ALREADY_EXISTS`; `field` คือชื่อคอลัมน์หรือ `capabilities` (ต้องมี capability อย่างน้อย 1 ค่า).
- `categoryCode`/`brandCode`/`baseUnitCode`/`taxCategoryCode` อ้างรหัสที่มีและ `active` ใน Organization (ไม่สร้างให้); `code` ว่าง = ออกรหัสผ่าน Document Numbering; รหัสซ้ำในไฟล์หรือซ้ำกับของเดิมถูกปฏิเสธ; `canX` รับ `true/false/1/0` (ว่าง = false).
- Commit ตรวจซ้ำทั้งไฟล์: ถ้ามีแถวไม่ถูกต้องคืน `422 ITEM_IMPORT_VALIDATION_FAILED` และไม่สร้างอะไร; ถ้า `expectedContentSha256` ไม่ตรงคืน `409 ITEM_IMPORT_CONTENT_CHANGED`; สำเร็จสร้าง Item ทั้งหมดเป็น `draft` (AvailabilityMode = all branches) ใน Transaction เดียว ไม่สร้าง/เผยแพร่ Cost และไม่ Activate. ตอบ `{ batchId, createdCount, contentSha256, replayed }`.
- Idempotency: Key + content hash เดิมคืนผลเดิม (`replayed: true`); Key เดิม content ต่าง → `409 IDEMPOTENCY_KEY_REUSED`. ชนรหัสพร้อมกันขณะ Commit → `409 ITEM_CODE_CONFLICT`.
- Audit: `items.create` ต่อ Item (มี `importBatchId`) และ `items.import` หนึ่งรายการต่อ Batch (`createdCount`, `contentSha256`).

### Planned full contract (Future: Batch, File Service, upsert, Cost)

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
| `TC-API-ITEM-024` | แนบภาพ Category/Brand จาก Upload Session ของ Parent หรือ Organization อื่น | ปฏิเสธไฟล์ ไม่บันทึก `imageFileId` |

## Data Mapping

อ่าน [Item Master Data Contract](../04-data/item-master-data-contract.md)
