# Item Master Field Catalog (รายการช่องข้อมูลสินค้า บริการ และต้นทุน)

**สถานะ:** Accepted Direction — Production Baseline; ค่า Cost ทั้งหมดในตัวอย่างเป็น `TEST_ONLY`

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Field, Type, Required Gate และ Visibility สำหรับ Item, Unit, Conversion, Cost Source และ Cost Record

## Gate Legend

- `D` = บังคับเมื่อ Save Draft ครั้งแรก
- `A` = บังคับก่อน Activate Item/Unit
- `S` = บังคับก่อน Submit Cost
- `P` = บังคับก่อน Publish Cost
- Master Data และ Cost เป็น `Internal` โดยค่าเริ่มต้น Quotation ใช้ Description/Selling Snapshot จาก Estimate เท่านั้น

## Item

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `organizationId` | UUID | D | จาก Active Membership; Client เปลี่ยนไม่ได้ |
| `code` | String ≤50 | D | Normalize Uppercase; Unique ใน Organization; immutable หลัง Active ครั้งแรก |
| `itemType` | Enum | D | `material`, `labor`, `service`, `subcontract`, `other` |
| `categoryId` | UUID | A | Active Category ใน Organization เดียวกัน |
| `nameTh` | String ≤250 | A | บังคับ; Trim/Normalize |
| `nameEn` | String ≤250/null | Optional | ใช้สำหรับเอกสาร/ค้นหาภาษาอังกฤษ |
| `descriptionTh` | String ≤2,000/null | Optional | ไม่ส่งลูกค้าอัตโนมัติ |
| `descriptionEn` | String ≤2,000/null | Optional | ไม่ส่งลูกค้าอัตโนมัติ |
| `baseUnitCode` | String ≤20 | A | Active Unit และเข้ากับ Item |
| `taxCategoryCode` | String ≤30/null | Optional | เป็น Classification ไม่ใช่ Tax Rate |
| `status` | Enum | System | `draft`, `active`, `inactive` |
| `rowVersion` | Token | System | เปลี่ยนทุก Write |

## Item Capabilities

| Capability | ความหมาย | Baseline |
| --- | --- | --- |
| `canSell` | ใช้เป็นมาตรฐานของ Work Item/รายการขาย | false |
| `canCost` | ใช้เป็น Cost Component/มี Cost Record | true สำหรับทุก Type ยกเว้นเหตุผลเฉพาะ |
| `canPurchase` | เตรียมใช้กับ Procurement | true สำหรับ Material/Subcontract ตาม Policy |
| `canStock` | เตรียมใช้กับ Inventory | false จนออกแบบ Stock Module |
| `canProduce` | เตรียมใช้กับ BOM/Production | false จนออกแบบ Production Module |

Capability ไม่ให้สิทธิ์ User และไม่สร้าง Transaction ของโมดูลอนาคต

## Category

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤30 | A | Unique ใน Organization |
| `nameTh`, `nameEn` | String | A/Optional | ภาษาไทยบังคับ |
| `parentCategoryId` | UUID/null | Optional | ห้าม Cycle; Parent อยู่ Organization เดียวกัน |
| `allowedItemTypes` | Enum Set | A | Item ต้องอยู่ใน Allowlist |
| `status` | Enum | System | Active/Inactive; Category ที่ถูกใช้ห้ามลบ |

## Unit of Measure

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤20 | D | Unique ใน Organization เช่น `m`, `m2`, `sheet`, `day` |
| `nameTh`, `nameEn` | String ≤100 | A/Optional | ภาษาไทยบังคับ |
| `symbol` | String ≤20 | A | ใช้แสดงผล ไม่ใช้เป็น Identifier |
| `dimension` | Enum | A | `length`, `area`, `volume`, `mass`, `time`, `count`, `custom` |
| `decimalScale` | Integer 0–4 | A | ควบคุมการรับ/แสดง Quantity |
| `status` | Enum | System | Active/Inactive |

## Unit Conversion

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `itemId` | UUID/null | A | null เฉพาะ Exact Conversion กลาง |
| `fromUnitCode`, `toUnitCode` | Code | A | ต้องต่างกันและ Active |
| `factor` | Decimal(24,10) | A | >0; `toQuantity = fromQuantity × factor` |
| `roundingMode` | Enum | A | `none`, `up`, `down`, `nearest` |
| `roundingIncrement` | Decimal(18,4)/null | Conditional | บังคับเมื่อ Mode ไม่ใช่ none |
| `effectiveFrom`, `effectiveTo` | UTC/null | A | Period ไม่กลับด้าน/ไม่ซ้อนใน Natural Key |
| `reason` | String ≤500 | Conditional | บังคับสำหรับ Item-specific Packaging Conversion |

Conversion กลางต้องเป็น Dimension เดียวกัน Item-specific ใช้กับ Packaging/ขนาดเฉพาะและต้องบันทึก Snapshot เมื่อ Estimate ใช้

## Cost Source

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤50 | S | Unique ใน Organization |
| `sourceType` | Enum | S | `supplier-quote`, `price-list`, `contract`, `historical`, `manual` |
| `supplierId` | UUID/null | Conditional | บังคับเมื่อ Source Type อ้าง Supplier |
| `referenceNumber` | String ≤100/null | Conditional | เลขเอกสาร/รายการราคา |
| `evidenceFileId` | UUID/null | Conditional | ตาม Cost Policy; File อยู่ Scope เดียวกัน |
| `capturedAtUtc` | UTC | S | Server-owned หรือข้อมูลนำเข้าที่ Validate |
| `note` | String ≤1,000/null | Optional | Internal เท่านั้น |

## Cost Record

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `itemId` | UUID | S | Active และ `canCost=true` |
| `branchId` | UUID/null | S | null = Organization Default; ค่าอื่นต้องอยู่ Scope |
| `costSourceId` | UUID | S | Source ใน Organization เดียวกัน |
| `unitCode` | String ≤20 | S | Base Unit หรือมี Valid Conversion |
| `currency` | ISO 4217 | S | Phase แรก Resolve เป็น THB สำหรับ Estimate THB |
| `amount` | Decimal(19,4) | S | ≥0; Zero Cost ต้องมี Reason |
| `minimumQuantity` | Decimal(18,4) | S | ≥0; ค่าเริ่มต้น 0 |
| `maximumQuantity` | Decimal(18,4)/null | Optional | ต้องมากกว่า Minimum |
| `effectiveFrom` | UTC | S | ใช้ Business Time Zone แปลงก่อนเก็บ UTC |
| `effectiveTo` | UTC/null | Optional | Exclusive End; ต้องมากกว่า From |
| `sourcePriority` | Integer/null | P | มาจาก Cost Policy; User ไม่กำหนดเองโดยทั่วไป |
| `status` | Enum | System | draft/submitted/returned/approved/published/superseded/disabled |
| `changeReasonCode`, `changeReason` | String/null | Conditional | บังคับเมื่อ Version 2+ หรือ Disable |
| `rowVersion` | Token | System | Draft concurrency |

Natural Key สำหรับ Period Overlap คือ Organization + Item + Branch Scope + Unit + Currency + Quantity Range + Published Status

## Import Batch and Row

| Field | Type | Rule |
| --- | --- | --- |
| `fileId`, `fileHash` | UUID/Hash | File Service + Duplicate detection |
| `templateVersion` | String | ต้องเป็น Version ที่ระบบรองรับ |
| `mode` | Enum | `create-only`, `update-existing`, `upsert` ตาม Permission |
| `status` | Enum | uploaded/parsing/validated/invalid/committing/committed/failed |
| `rowNumber` | Integer | เริ่มตามตำแหน่งจริงในไฟล์ |
| `rawData` | JSONB | Internal, จำกัดขนาด, Retention สั้น |
| `action` | Enum | create/update/skip/error |
| `errors` | JSONB | Stable Code + Field + localized message |

## Field Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-FIELD-ITEM-001` | Code ซ้ำต่างตัวพิมพ์/ช่องว่าง | Reject `ITEM_CODE_CONFLICT` |
| `TC-FIELD-ITEM-002` | Activate ไม่มี Base Unit | `ITEM_FIELD_REQUIRED` |
| `TC-FIELD-ITEM-003` | Item ไม่มี `canCost` แต่สร้าง Cost | Reject |
| `TC-FIELD-ITEM-004` | Conversion Factor ≤0 | `ITEM_CONVERSION_INVALID` |
| `TC-FIELD-ITEM-005` | Conversion กลางข้าม Dimension | Reject |
| `TC-FIELD-ITEM-006` | Cost Period กลับด้าน | Reject Field Error |
| `TC-FIELD-ITEM-007` | Cost Amount =0 ไม่มี Reason | Reject Field Error |
| `TC-FIELD-ITEM-008` | Branch นอก Organization | 404 + Security Audit |
| `TC-FIELD-ITEM-009` | Patch ด้วย ETag เก่า | `ITEM_VERSION_CONFLICT` |
| `TC-FIELD-ITEM-010` | Inactive Item ใน Snapshot เก่า | อ่านประวัติได้แต่เลือกใหม่ไม่ได้ |

## ตัวอย่างสั้น

`TEST_ONLY`: `MAT-PLY-18`, Material, Base Unit `sheet`, `canCost=true`, `canPurchase=true`; Cost `1,250.0000 THB/sheet` เป็นเพียงรูปแบบข้อมูล ไม่ใช่ราคาจริง

## เอกสารที่เกี่ยวข้อง

- [Item Master Flow](item-master-flow.md)
- [Item Master Governance](item-master-governance.md)
- [Item Master Data Contract](../04-data/item-master-data-contract.md)
