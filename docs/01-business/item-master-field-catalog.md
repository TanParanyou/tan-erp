# Item Master Field Catalog (รายการช่องข้อมูลสินค้า บริการ และต้นทุน)

**สถานะ:** Accepted Direction — Production Baseline; ค่า Cost ทั้งหมดในตัวอย่างเป็น `TEST_ONLY`

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Field, Type, Required Gate และ Visibility สำหรับ Item, Category, Brand, Alias, Branch Availability, Item Image, Item Barcode, Unit, Conversion, Cost Source และ Cost Record

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
| `itemType` | Enum | D | `material`, `labor`, `service`, `subcontract`, `other`, `product` |
| `categoryId` | UUID | A | Active Category ใน Organization เดียวกัน |
| `brandId` | UUID/null | Optional | Active Brand ใน Organization เดียวกัน; Material/Product ใช้ได้แต่ Labor/Service ว่างได้ |
| `name` | Localized JSON `{th,en?}` | A | `th` บังคับ ≤250; อนุญาตเฉพาะ `th`,`en`; Trim/Normalize |
| `description` | Localized JSON `{th?,en?}` | Optional | แต่ละภาษา ≤2,000; Plain text; ไม่ส่งลูกค้าอัตโนมัติ |
| `baseUnitCode` | String ≤20 | A | Active Unit และเข้ากับ Item |
| `taxCategoryCode` | String ≤30/null | Optional | เลือกรหัสจาก Tax Category Master ของ Organization; เป็น Classification ไม่ใช่ Tax Rate |
| `availabilityMode` | Enum | D | `allBranches` หรือ `selectedBranches`; ไม่อนุมานจาก Branch List |
| `attributes` | JSON Object | Optional | String values สำหรับแสดงผล/กรอง; ห้ามควบคุมราคา สิทธิ์ Lifecycle หรือ Calculation |
| `attributesSchemaVersion` | Integer | Conditional | บังคับเมื่อมี Attributes |
| `status` | Enum | System | `draft`, `active`, `inactive` |
| `rowVersion` | Token | System | เปลี่ยนทุก Write |

### ตัวตนสินค้า: Item Code, SKU และ Barcode

- `Item.id` เป็นตัวตนอ้างอิงถาวรของระบบ; `Item.code` เป็นรหัสภายในองค์กรที่ไม่ซ้ำ และใช้เป็น **SKU ภายใน** เมื่อ Item เป็นสินค้าที่ซื้อหรือเก็บคลังได้. หน้าจออาจแสดงป้าย “SKU / รหัสสินค้า” สำหรับรายการดังกล่าว แต่ API และฐานข้อมูลมี authoritative field เดียวคือ `code`. Labor/Service ยังคงมี Item Code โดยไม่เรียกว่า SKU
- SKU แยกใหม่จะสร้างข้อมูลซ้ำและต้องมีกฎตัดสินว่าเอกสาร/การค้นหาอ้างรหัสใด; จึงไม่เพิ่ม `sku` อีกคอลัมน์ในฐานนี้. Code เปลี่ยนไม่ได้หลัง Activate ครั้งแรก และห้ามนำ Code ของ Item ที่ปิดใช้แล้วไปใช้ซ้ำใน Organization เดียวกัน
- Barcode เป็นตัวระบุที่เครื่องสแกนอ่านได้ ไม่ใช่ SKU. `GTIN` คือรหัสสินค้าตาม GS1 ที่อาจอยู่ใต้สัญลักษณ์ EAN/UPC/ITF/DataMatrix; Internal Barcode เป็นรหัสที่องค์กรออกเองและห้ามแสดงว่าเป็น GTIN. รหัสผู้ขายหรือผู้ผลิตเป็นความสัมพันธ์กับ Supplier/Manufacturer ในโมดูลจัดซื้อ ไม่ใช่ SKU หรือ Barcode หลักของ Item
- Item หนึ่งตัวมีรหัสภายในสำรองและ Barcode ได้หลายรายการตามหน่วย/ระดับบรรจุ เช่น แผ่นกับลัง; รหัสเหล่านี้ช่วยค้นหรือสแกน Item เดิมและไม่สร้าง SKU หลักเพิ่มเติม. Barcode ที่แยกความต่างด้านขนาด สี หรือสเปกซึ่งมีผลต่อการสั่ง/เก็บ/คิดต้นทุน ต้องชี้ไปคนละ Item ตาม [GS1 variant guidance](https://support.gs1.org/support/solutions/articles/43000734083-how-many-gs1-gtins-do-i-need-when-i-have-a-product-with-many-sizes-and-colours-). ระดับบรรจุที่ต่างกันมี GTIN ของตัวเองตามมาตรฐาน GS1; การแปลงจำนวนไปหน่วยฐานต้องมี conversion ที่ยืนยันแล้วก่อนใช้ธุรกรรมคลัง
- ฟอร์ม Item มีปุ่มสร้างเร็วข้าง Category, Brand และ Unit สำหรับผู้มีสิทธิ์ `items.manage-taxonomy`; เมื่อสร้างสำเร็จ ระบบเลือกค่าที่สร้างให้ Item ปัจจุบันทันที. `itemType` ยังคงเป็น Enum ที่ระบบกำหนด เพราะใช้ควบคุมความสามารถของ Item และไม่ใช่ taxonomy ที่องค์กรสร้างเอง
- Tax Category มี Master แยกเฉพาะรหัส/ชื่อสองภาษา/ลำดับ/สถานะ และสร้างหรือแก้ไขจากหน้าข้อมูลอ้างอิงได้; ปุ่มสร้างเร็วใน Item เลือก Category ที่สร้างให้ทันที. รหัสนี้ไม่เก็บอัตราภาษีและไม่เปลี่ยนผลคำนวณ VAT

## Item Barcode

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `itemId` | UUID | D | Item ใน Organization เดียวกัน; Barcode ผูกกับ Item เดียวตลอดอายุ |
| `identifierType` | Enum | D | `gtin` หรือ `internal`; ระบุแหล่งของรหัส ไม่ใช่รูปแบบภาพ Barcode |
| `value` | String | D | เก็บเป็นข้อความเพื่อรักษาเลขศูนย์นำหน้า; ไม่ซ้ำใน Organization รวมรายการที่ปิดใช้แล้ว |
| `unitId` | UUID | D | หน่วยที่สแกนแล้วหมายถึง; ต้องอยู่ Organization เดียวกัน |
| `quantityInBaseUnit` | Decimal(18,4) | D | มากกว่า 0; ต้องสอดคล้องกับ Unit Conversion ที่ยืนยันแล้วก่อนนำไปใช้ในธุรกรรม |
| `packagingLevel` | Enum | D | `each`, `inner`, `case`, `pallet`; สำหรับระดับที่ซื้อ/เก็บ/ขายจริง |
| `isPrimary` | Boolean | D | Primary ได้หนึ่งรายการต่อ Item และระดับบรรจุ |
| `status`, `rowVersion` | Enum/Token | System | `active|inactive`; ปิดใช้แทน Hard Delete และใช้ ETag เมื่อเปลี่ยน |

`gtin` รับเลข 8, 12, 13 หรือ 14 หลักและตรวจ GS1 check digit; การผ่าน check digit ไม่พิสูจน์สิทธิ์เจ้าของหรือการจดทะเบียนกับ GS1. `internal` ใช้กับระบบภายในเท่านั้นและต้องไม่ชน Barcode ใดใน Organization. Item ที่ไม่มี Barcode ยังสร้างและ Activate ได้. การสแกนต้องคืน Item + หน่วย/จำนวนที่ตรงแน่นอนเพียงรายการเดียว; Barcode ที่ Inactive ไม่ถูกเลือกในธุรกรรมใหม่ แต่ประวัติเดิมยังอ่านได้. อ้างอิง [GS1 GTIN vs Barcode](https://support.gs1.org/support/solutions/articles/43000734124-what-is-the-difference-between-a-gs1-gtin-a-barcode-an-ean-and-a-upc-), [Check Digit](https://www.gs1.org/services/check-digit-calculator) และ [GTIN Packaging Rules](https://www.gs1.org/1/gtinrules/en/rule/270/packcase-quantity)

### ขอบเขตรากฐานก่อนเปิดโมดูลถัดไป

- Item Type `product` ใช้กับสินค้าสำเร็จรูปที่ทำซ้ำได้ ส่วน Product Family/Variant grouping และ Technical Specification Revision เป็น aggregate แยกในระยะถัดไปเมื่อมีกฎจัดซื้อ/คลัง/ผลิตชัดเจน. รูปแบบสินค้า/ขนาด/สีที่ต้องสั่งซื้อ เก็บคลัง หรือตั้งต้นทุนแยกกันต้องเป็น Item คนละตัวพร้อม Code/SKU ของตน; ไม่อัดความแตกต่างที่มีผลต่อธุรกรรมลง `attributes` อิสระ. งานบิวต์อินสั่งทำเฉพาะโครงการยังเป็น Work Item ไม่สร้าง SKU ใหม่ทุกครั้งโดยอัตโนมัติ
- สเปกเชิงเทคนิคที่มีผลต่อสูตรคำนวณ/การผลิต เช่น ขนาดจริง วัสดุ เกรด ผิว สี หรือ Revision ของแบบ ต้องมี Typed, Versioned Contract ตาม Category เมื่อโมดูลนั้นเป็นเจ้าของกฎ; `attributes` ปัจจุบันใช้แสดง/กรองเท่านั้น
- Supplier Part Number, ราคาซื้อ, MOQ และ Lead Time อยู่กับ Item–Supplier/Quotation; Lot/Serial, ที่เก็บ และจำนวนคงเหลืออยู่กับ Inventory; BOM/Router/Work Center อยู่กับ Production. Item เก็บตัวตนและ Capability เพื่อให้โมดูลเหล่านั้นอ้าง Item เดียวกัน

## Item Capabilities

| Capability | ความหมาย | Baseline |
| --- | --- | --- |
| `canSell` | ใช้เป็นมาตรฐานของ Work Item/รายการขาย | false |
| `canCost` | ใช้เป็น Cost Component/มี Cost Record | true สำหรับทุก Type ยกเว้นเหตุผลเฉพาะ |
| `canPurchase` | เตรียมใช้กับ Procurement | true สำหรับ Material/Subcontract ตาม Policy |
| `canStock` | เตรียมใช้กับ Inventory | false จนออกแบบ Stock Module |
| `canProduce` | เตรียมใช้กับ BOM/Production | false จนออกแบบ Production Module |

Capability ไม่ให้สิทธิ์ User และไม่สร้าง Transaction ของโมดูลอนาคต

## Item Branch Availability

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `itemId`, `branchId` | UUID | A | Item/Branch อยู่ Organization เดียวกัน |
| `status` | Enum | System | `active|inactive` |
| `effectiveFromUtc`, `effectiveToUtc` | UTC/null | Optional | Exclusive End; Period ไม่กลับด้าน |
| `inactiveReason` | String ≤500/null | Conditional | บังคับเมื่อปิด Relation |
| `rowVersion` | Token | System | Compare-and-swap |

`allBranches` ไม่ต้องมี Relation; `selectedBranches` ต้องมี Active Relation อย่างน้อยหนึ่งรายการก่อน Activate Item ราคาสาขาเก็บใน Cost Record ไม่เก็บใน Availability

## Item Image

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `fileId` | UUID | D | Verified JPEG/PNG/WebP จาก Upload Session ของ Item เดียวกัน |
| `role` | Enum | D | `primary|gallery|technical` |
| `isPrimary` | Boolean | D | Active Primary ได้หนึ่งภาพต่อ Item |
| `displayOrder` | Integer ≥0 | D | Stable Sort ด้วย `displayOrder,id` |
| `altText` | Localized JSON `{th,en?}` | A | `th` บังคับสำหรับภาพที่ Active; แต่ละภาษา ≤250 |
| `caption` | Localized JSON `{th?,en?}` | Optional | แต่ละภาษา ≤500; Plain text |
| `status` | Enum | System | `active|inactive` |
| `rowVersion` | Token | System | Compare-and-swap |

Original File จำกัด 10 MB; Backend ตรวจ MIME และ Magic Number, ลบ EXIF/GPS, Scan ก่อน Verified และสร้าง Thumbnail/Medium Variant โดยไม่เขียนทับ Original

## Category

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤30 | A | Unique ใน Organization |
| `name` | Localized JSON `{th,en?}` | A | ภาษาไทยบังคับ; แต่ละภาษา ≤250 |
| `description` | Localized JSON `{th?,en?}` | Optional | แต่ละภาษา ≤1,000; Plain text |
| `parentCategoryId` | UUID/null | Optional | ห้าม Cycle; Parent อยู่ Organization เดียวกัน |
| `allowedItemTypes` | Enum Set | A | Item ต้องอยู่ใน Allowlist |
| `sortOrder` | Integer ≥0 | D | Stable Sort ด้วย `sortOrder,id` |
| `imageFileId` | UUID/null | Optional | ภาพหลักที่ผ่านการตรวจสอบและ Upload Session ผูกกับ Category เดียวกัน; ไฟล์เป็น Private และอ่านได้ตามสิทธิ์ใน Organization |
| `status` | Enum | System | Active/Inactive; Category ที่ถูกใช้ห้ามลบ |

Category และ Subcategory ใช้ Entity เดียวกัน โดย Subcategory คือ Category ที่มี `parentCategoryId`

## Brand

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤30 | D | Normalize Uppercase; Unique ใน Organization |
| `name` | Localized JSON `{th,en?}` | A | ภาษาไทยบังคับ; แต่ละภาษา ≤250 |
| `description` | Localized JSON `{th?,en?}` | Optional | แต่ละภาษา ≤1,000; Plain text |
| `sortOrder` | Integer ≥0 | D | Stable Sort ด้วย `sortOrder,id` |
| `imageFileId` | UUID/null | Optional | โลโก้/ภาพหลักที่ผ่านการตรวจสอบและ Upload Session ผูกกับ Brand เดียวกัน; ไฟล์เป็น Private และอ่านได้ตามสิทธิ์ใน Organization |
| `status` | Enum | System | Active/Inactive; Brand ที่ถูกใช้ห้าม Hard Delete |

## Item Alias

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `alias` | Localized JSON `{th,en?}` | D | ต้องมีอย่างน้อยหนึ่งภาษา; แต่ละภาษา ≤250 |
| `status` | Enum | System | Active/Inactive |
| `rowVersion` | Token | System | Compare-and-swap |

Alias ใช้ค้นหาคำเรียกอื่น เช่น “ไม้เขียว” แต่ไม่แทน Item Name และไม่ถูกส่งเป็น Estimate Description โดยอัตโนมัติ

## Unit of Measure

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤20 | D | Unique ใน Organization เช่น `m`, `m2`, `sheet`, `day` |
| `name` | Localized JSON `{th,en?}` | A | ภาษาไทยบังคับ; แต่ละภาษา ≤100 |
| `symbol` | String ≤16 | A | ใช้แสดงผล ไม่ใช้เป็น Identifier |
| `dimension` | Enum | A | `length`, `area`, `volume`, `mass`, `time`, `count`, `custom` |
| `decimalScale` | Integer 0–6 | A | ควบคุมการรับ/แสดง Quantity |
| `roundingMode` | Enum | A | `half_up`, `half_even`, `up`, `down`, `ceiling`, `floor`; กำหนดนโยบายปัดเศษของหน่วยและต้องคงเดิมเมื่อแก้ฟิลด์อื่น |
| `status` | Enum | System | Active/Inactive |

`half_up` ปัดค่ากึ่งกลางออกจากศูนย์, `half_even` ปัดค่ากึ่งกลางไปเลขคู่, `up` ปัดออกจากศูนย์, `down` ปัดเข้าหาศูนย์, `ceiling` ปัดไปทางค่าบวก และ `floor` ปัดไปทางค่าลบ โดยใช้ `decimalScale` เป็นจำนวนหลักทศนิยม

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

Cost Source เป็นรายการประเภทแหล่งต้นทุนระดับ Organization ที่เลือกใช้ซ้ำได้ ไม่ใช่หลักฐานของราคาฉบับใดฉบับหนึ่ง รอบแรกสร้างและเลือกใช้เฉพาะชนิด `manual`; ข้อมูล Supplier และชนิดอื่นเป็นงานภายหลัง รายละเอียดอ้างอิงและหลักฐานของราคาหนึ่งฉบับอยู่ที่ Cost Record

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `code` | String ≤32 | S | Unique ใน Organization; คงเดิมหลังถูกอ้างอิง |
| `name` | Localized Text | S | `th` บังคับ; `en` ว่างได้ |
| `sourceType` | Enum | S | สร้างใหม่ได้เฉพาะ `manual`; `legacy` ใช้อ่านข้อมูลเดิมที่ระบุชนิดไม่ได้และเลือกใหม่ไม่ได้ |
| `priority` | Integer | System | Metadata ของ Source; Cost Resolver รอบแรกไม่ใช้ตัดสินราคา |
| `isActive` | Boolean | System | Source ที่ปิดใช้ยังอ่านประวัติได้ แต่เลือกกับ Cost ใหม่ไม่ได้ |
| `rowVersion` | UUID | System | ใช้กับ ETag เมื่อแก้/ปิดใช้ |

## Cost Record

| Field | Type | Gate | Rule |
| --- | --- | --- | --- |
| `itemId` | UUID | S | Active และ `canCost=true` |
| `branchId` | UUID/null | S | null = Organization Default; ค่าอื่นต้องอยู่ Scope |
| `costSourceId` | UUID | S | Source ชนิด Manual ที่ Active และอยู่ Organization เดียวกัน; บังคับสำหรับ Draft ใหม่ |
| `sourceReference` | String ≤128/null | Conditional | ระบุที่มาที่ตรวจสอบได้ของราคาฉบับนี้เมื่อไม่มีไฟล์หลักฐาน |
| `reason` | String ≤500/null | Conditional | บังคับสำหรับ Manual Source, Zero Cost, รุ่นใหม่หรือ Disable ตาม Governance |
| `evidenceFileId` | UUID/null | Conditional | File ที่ Verified และผูกกับ Cost Record นี้; ถ้าไม่มีไฟล์ต้องมี `sourceReference` ที่ตรวจสอบได้ |
| `unitId` | UUID | S | Base Unit หรือมี Valid Conversion; `unitCode` เป็นข้อมูลแสดงผลจาก Unit |
| `currency` | ISO 4217 | S | Phase แรก Resolve เป็น THB สำหรับ Estimate THB |
| `amount` | Decimal(19,4) | S | ≥0; Zero Cost ต้องมี Reason |
| `minimumQuantity` | Decimal(18,4) | S | ≥0; ค่าเริ่มต้น 0 |
| `maximumQuantity` | Decimal(18,4)/null | Optional | ต้องมากกว่า Minimum |
| `effectiveFrom` | UTC | S | ใช้ Business Time Zone แปลงก่อนเก็บ UTC |
| `effectiveTo` | UTC/null | Optional | Exclusive End; ต้องมากกว่า From |
| `status` | Enum | System | draft/submitted/returned/approved/published/superseded/disabled |
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
| `TC-FIELD-ITEM-011` | Localized Name มี Key อื่นหรือ `th` ว่างตอน Activate | Reject Field Error |
| `TC-FIELD-ITEM-012` | Selected Branches ไม่มีสาขาที่ Active | Activate ไม่ได้ |
| `TC-FIELD-ITEM-013` | ภาพไม่มี Alt Text ไทยตอน Activate | Reject Field Error |
| `TC-FIELD-ITEM-014` | Attributes พยายามกำหนดราคา/สถานะ | Reject ตาม Attributes Schema |
| `TC-FIELD-ITEM-015` | Category Parent เกิด Cycle | Reject และไม่เปลี่ยนข้อมูลบางส่วน |
| `TC-FIELD-ITEM-016` | Alias ซ้ำหลัง Normalize | Reject `ITEM_ALIAS_CONFLICT` |
| `TC-FIELD-ITEM-017` | Item อ้าง Brand ข้าม Organization | 404 + Security Audit |

## ตัวอย่างสั้น

`TEST_ONLY`: `MAT-PLY-18`, Material, Base Unit `sheet`, `canCost=true`, `canPurchase=true`; Cost `1,250.0000 THB/sheet` เป็นเพียงรูปแบบข้อมูล ไม่ใช่ราคาจริง

## เอกสารที่เกี่ยวข้อง

- [Item Master Flow](item-master-flow.md)
- [Item Master Governance](item-master-governance.md)
- [Item Master Data Contract](../04-data/item-master-data-contract.md)
