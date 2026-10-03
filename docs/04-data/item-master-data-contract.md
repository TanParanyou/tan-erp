# Item Master Data Contract (ข้อตกลงข้อมูลสินค้า หน่วย และต้นทุน)

**สถานะ:** Accepted Direction — Logical Schema Baseline

## Aggregate and Ownership

```text
Organization
 ├─ ItemCategory
 ├─ ItemBrand
 ├─ ItemTaxCategory
 ├─ Unit
 ├─ CostSource
 └─ Item
     ├─ Capability Flags
     ├─ ItemAlias
     ├─ ItemBarcode ─ Unit
     ├─ ItemBranchAvailability ─ Branch
     ├─ ItemImage ─ Verified File
     ├─ ItemUnitConversion
     └─ CostRecord ─ CostReview

ItemImportBatch ─ ItemImportRow
Estimate Cost Component ─► Item/CostRecord/Conversion Snapshot
```

Item เป็นเจ้าของตัวตน/ประเภท/Capability/หน่วยฐาน ส่วน Cost Record เป็น Versioned Financial Record แยก Lifecycle; Item ไม่มี Column `current_cost`

`items.code` คือ Item Code ภายในและเป็น SKU สำหรับ Item ที่ซื้อ/เก็บได้ จึงไม่มี `items.sku` อีกคอลัมน์. External GTIN/Internal Barcode อยู่ในตารางลูก `item_barcodes` เพื่อรองรับหลายหน่วยและระดับบรรจุ; Supplier Part Number ต้องอยู่กับความสัมพันธ์ Item–Supplier เมื่อโมดูลจัดซื้อพร้อม. กฎฟิลด์และ Gate อยู่ที่ [Item Master Field Catalog](../01-business/item-master-field-catalog.md)

`item_type=product` ใช้กับสินค้าสำเร็จรูปที่ทำซ้ำได้และถูกเพิ่มด้วย additive migration; ค่าที่รองรับคือ `material|labor|service|subcontract|other|product`. Product Family/Variant grouping และ Technical Specification Revision จะเป็น aggregate แยกเมื่อมีกฎจัดซื้อ/คลัง/ผลิตชัดเจน; ไม่ใช้ `attributes` JSONB เป็น BOM หรือสูตรคำนวณ

## Relational Core

### `items`

| Column | Type | Rule |
| --- | --- | --- |
| `id`, `organization_id` | UUID | PK และ Trusted Scope |
| `code`, `normalized_code` | String | Unique `(organization_id, normalized_code)` |
| `item_type` | Enum/String | `material|labor|service|subcontract|other|product` |
| `tax_category_code` | String/Nullable | Optional code from the organization-scoped `item_master.item_tax_categories` master; never stores a tax rate |
| `category_id`, `brand_id`, `base_unit_id` | UUID/Nullable | ต้องอยู่ Organization/Shared Scope ที่อนุญาต |
| `name` | JSONB | Object ที่อนุญาตเฉพาะ `th`, `en`; `th` บังคับก่อน Active; ค่าแต่ละภาษาไม่เกิน 250 ตัวอักษร |
| `description` | JSONB | Object ที่อนุญาตเฉพาะ `th`, `en`; ค่าแต่ละภาษาไม่เกิน 2,000 ตัวอักษร; ห้าม HTML |
| `can_sell`, `can_cost`, `can_purchase`, `can_stock`, `can_produce` | Boolean | Typed flags; อย่างน้อยหนึ่งค่า True ก่อน Active |
| `availability_mode` | String | `all_branches|selected_branches`; ไม่อนุมานความหมายจากจำนวน Relation |
| `attributes`, `attributes_schema_version` | JSONB/Integer | ใช้เฉพาะข้อมูลแสดงผล/กรองที่ไม่ควบคุมราคา สิทธิ์ Lifecycle หรือ Calculation |
| `status` | String | `draft|active|inactive` |
| `activated_once`, `activated_at_utc`, `activated_by_user_id` | Boolean/Timestamp/UUID? | Code immutable เมื่อ `activated_once=true` |
| `inactive_at_utc`, `inactive_by_user_id`, `inactive_reason_code`, `inactive_reason` | Timestamp/UUID/String? | Inactive บังคับ Reason Code และข้อความประกอบตาม Policy |
| `row_version` | Concurrency token | Compare-and-swap |
| `created_at_utc`, `created_by_user_id`, `updated_at_utc`, `updated_by_user_id` | Timestamp/UUID | UTC และ Actor |

Capability ใช้ Typed Columns ไม่ใช้ EAV/JSONB เพื่อให้ Constraint, Query และสิทธิ์ตรวจสอบได้ตรงไปตรงมา

Localized JSONB ต้องเป็น JSON Object รูปทรงคงที่ `{ "th": string, "en"?: string }` ตาม [ADR 0012](../adr/0012-localized-jsonb-for-item-text.md) Database มี Check Constraint เรื่อง Type/Allowed Keys/Length และมี Expression Index `(organization_id, lower(name->>'th'))` กับภาษาอังกฤษเมื่อ Query Plan พิสูจน์ว่าจำเป็น ห้ามสร้าง GIN ทั้ง Document โดยไม่มี Query ที่รองรับ

### Branch Availability

`item_branch_availabilities` มี `id`, `organization_id`, `item_id`, `branch_id`, `status=active|inactive`, `effective_from_utc?`, `effective_to_utc?`, `inactive_reason?`, `row_version` และ Audit Columns พร้อม Unique `(organization_id, item_id, branch_id)` และ Same-organization Composite FK

- `availability_mode=all_branches` ทำให้สาขาปัจจุบันและสาขาใหม่เลือก Item ได้โดยไม่ต้องมี Relation
- `availability_mode=selected_branches` เลือกได้เฉพาะ Active Relation ที่มีผล ณ เวลาที่ Query
- Availability ไม่เก็บราคาและไม่แทน Branch-scoped Cost Record
- การเปลี่ยน Mode/Relation ไม่แก้ Historical Estimate Snapshot

### Category, Brand and Alias

`item_categories` มี `id`, `organization_id`, `code` (ไม่เกิน 30 ตัว), `normalized_code`, `name` JSONB, `description` JSONB, `image_file_id?`, `parent_category_id?`, `allowed_item_types`, `sort_order`, `status`, `row_version` และ Audit Columns; Parent ต้องอยู่ Organization เดียวกันและห้าม Cycle ตารางเดียวรองรับทั้ง Category/Subcategory โดยไม่สร้าง `item_subcategories`

`item_brands` มี `id`, `organization_id`, `code` (ไม่เกิน 30 ตัว), `normalized_code`, `name` JSONB, `description` JSONB, `image_file_id?`, `sort_order`, `status`, `row_version` และ Audit Columns พร้อม Unique `(organization_id, normalized_code)` Item อ้าง Brand แบบ Nullable เพื่อรองรับ Labor/Service ที่ไม่มี Brand

Brand และ Category รองรับภาพหลักได้อย่างละหนึ่งไฟล์ โดยอ้าง `files.uploaded_files` ผ่าน `image_file_id`; ใช้ Upload Session ที่ผูก `parent_type=item-brand|item-category` และ `parent_id` ของรายการ, ไฟล์ต้องผ่านการตรวจสอบก่อนแนบ และ API อ่านภาพต้องผ่านสิทธิ์ของ Master Data ภายใน Organization เดิม การเปลี่ยน/ถอดภาพบันทึก Audit พร้อมการแก้ไขรายการ; ไม่เก็บ Binary หรือ Public URL ในตาราง Master Data

`item_tax_categories` มี `id`, `organization_id`, `code` (ไม่เกิน 30 ตัว), `normalized_code`, `name` JSONB, `sort_order`, `status`, `row_version` และ Audit Columns พร้อม Unique `(organization_id, normalized_code)`. Item เก็บ code เป็น classification snapshot; Tax Category ไม่เป็นเจ้าของอัตราภาษีและไม่มีผลกับ VAT Calculation ใน Slice นี้

`item_aliases` มี `id`, `organization_id`, `item_id`, `alias` JSONB, `normalized_th`, `normalized_en?`, `status`, `row_version` และ Audit Columns พร้อม Unique ต่อ Item/ภาษา/ค่าที่ Normalize แล้ว Alias ใช้เพื่อค้นหาเท่านั้น ไม่แทนชื่อ Item และไม่ถูก Snapshot เป็น Description อัตโนมัติ

### Item Barcode

`item_barcodes` มี `id`, `organization_id`, `item_id`, `identifier_type=gtin|internal`, `value`, `unit_id`, `quantity_in_base_unit`, `packaging_level=each|inner|case|pallet`, `is_primary`, `status=active|inactive`, `row_version` และ Audit Columns. `value` เป็นข้อความเพื่อรักษาเลขศูนย์นำหน้า; `normalized_value` เติมเลขศูนย์ซ้ายให้ครบ 14 หลักสำหรับรหัสตัวเลขความยาว 8/12/13/14 หรือ uppercase สำหรับรหัสอื่น เพื่อให้รูปแบบ GTIN ที่สมมูลกันไม่ชี้คนละ Item. Normalized Value มี Unique `(organization_id, normalized_value)` ตลอดอายุ ไม่เปิดให้ Item อื่นนำ Barcode เดิมกลับไปใช้หลัง Deactivate. `(item_id, organization_id)` และ `(unit_id, organization_id)` ใช้ Composite FK เพื่อพิสูจน์ Scope เดียวกัน และมี Partial Unique สำหรับ Active Primary ต่อ `(organization_id, item_id, packaging_level)`

Domain ตรวจ GTIN ตามความยาว 8/12/13/14 และ check digit; Database ตรวจความยาว, status, positive quantity และ uniqueness. การแก้ Value หรือย้าย Barcode ไปอีก Item หลังใช้งานให้ปิดรายการเดิมแล้วสร้างรายการใหม่พร้อม Audit; Snapshot ธุรกรรมเดิมเก็บรหัสและหน่วยที่สแกนไว้. การค้นหาด้วย Barcode ทำที่ Backend ภายใต้ Organization/Branch Scope และคืน Item/Unit/Quantity เดียวหรือ stable error; ไม่ให้ Frontend ดึงทุก Item ไปเทียบเอง. ไม่ถือ Barcode เป็นตัวตนของ Batch/Serial ของสินค้าจริง ซึ่งโมดูลคลังจะเป็นเจ้าของ

### Item Images and File Metadata

`item_images` เป็น Relation ไปยัง `files.uploaded_files` และมี `id`, `organization_id`, `item_id`, `file_id`, `role=primary|gallery|technical`, `is_primary`, `display_order`, `alt_text` JSONB, `caption` JSONB, `status=active|inactive`, `row_version` และ Audit Columns

- Unique `(organization_id, item_id, file_id)` และ Partial Unique `(organization_id, item_id) WHERE is_primary=true AND status='active'`
- File ต้อง `verified`, อยู่ Organization เดียวกัน และถูกสร้างจาก Upload Session ที่ `parent_type=item`, `parent_id=item_id`
- Binary, Base64 และ Public URL ห้ามอยู่ใน `items`/`item_images`; API คืน `fileId` และ Authorized Content URL เท่านั้น
- Production File Metadata เพิ่ม `content_sha256`, `width`, `height`, `scan_status`, `verified_at_utc`; Variant/Thumbnail อ้าง File แยกหรือ Derived Asset Relation โดยไม่เขียนทับ Original
- Allowlist เฉพาะ JPEG/PNG/WebP, ตรวจ Magic Number, จำกัด 10 MB ต่อ Original, ลบ EXIF/GPS, Scan ก่อน Verified และใช้ Private Object Storage ใน Production

### Unit and Conversion

`units` มี `code` (ไม่เกิน 20 ตัว), `name` JSONB ตาม Localized Text Contract, `symbol` (ไม่เกิน 16 ตัว), `dimension`, `decimal_scale` (0–6), `rounding_mode` (`half_up`, `half_even`, `up`, `down`, `ceiling`, `floor`), `status` และ Unique Code ตาม Scope. API Update ต้องรับและบันทึก precision/rounding ทุกครั้ง จึงห้าม Client แก้เฉพาะชื่อโดยละค่าทั้งสองฟิลด์

`unit_conversions` ใช้ Exact Conversion กลาง; `item_unit_conversions` ใช้ Packaging/ขนาดเฉพาะ Item โดยมี `organization_id`, `item_id` (เฉพาะ Item-specific), `from_unit_id`, `to_unit_id`, `factor`, `effective_from/to`, `reason`, `status`, `row_version` และ audit creator. Factor ต้องมากกว่า 0 และมีทศนิยมไม่เกิน 6 ตำแหน่ง; ห้าม Self-loop และ Period ซ้อนของคู่เดียวกัน. Item-specific conversion ต้องชี้ตรงไปยัง Base Unit ของ Item จึงไม่สามารถสร้าง Cycle ได้; Shared conversion ต้องใช้ Dimension เดียวกันและปฏิเสธเส้นทางที่สร้าง Cycle. Units ต้อง active และอยู่ใน Organization เดียวกัน. Conversion เป็น immutable version; Barcode ที่อาศัย conversion เก็บ `quantity_in_base_unit` เป็น snapshot แยกจาก conversion ในอนาคต

### `cost_sources`

เป็นรายการแหล่งต้นทุนที่เลือกใช้ซ้ำใน Organization มี `code`, `name` JSONB, `source_type=manual|legacy`, `priority`, `is_active`, `row_version`, Audit Columns และ Unique `(organization_id, code)`. รอบแรกสร้างได้เฉพาะ `manual`; `legacy` ใช้กับข้อมูลเดิมที่ระบุชนิดไม่ได้เพื่อรักษาประวัติและห้ามเลือกกับ Cost ใหม่. `priority` เป็น metadata เท่านั้นใน Cost Resolver รอบแรก Source ที่ปิดใช้ยังอ่านย้อนหลังได้ แต่ห้ามอ้างใน Cost Record ใหม่หรือส่งตรวจใหม่. Reference/Reason/File Evidence เป็นข้อมูลของ Cost Record แต่ละฉบับ ไม่เก็บซ้ำบน Source. Supplier และ Source ชนิดอื่นเป็นงานระยะถัดไป

### `cost_records`

| Column | Type | Rule |
| --- | --- | --- |
| `id`, `organization_id`, `item_id` | UUID | PK/Scope/FK |
| `version_number`, `cost_source_id` | Integer/UUID | Unique ต่อ Item/Scope/Natural Key; Cost ใหม่ต้องอ้าง Active Source ใน Organization เดียวกัน |
| `scope_type`, `branch_id` | String/UUID? | `organization` ต้องไม่มี Branch; `branch` ต้องมี Branch ใน Organization |
| `unit_id`, `currency` | UUID/CHAR(3) | Unit ใช้กับ Item ได้; ISO Currency |
| `amount` | Numeric | `>= 0`; Zero บังคับ Reason/Policy |
| `minimum_quantity`, `maximum_quantity` | Numeric | Min ≥ 0; Max null หรือ > Min |
| `effective_from_utc`, `effective_to_utc` | Timestamp | To null หรือ > From |
| `status` | String | `draft|submitted|returned|approved|published|superseded|disabled` |
| `source_reference`, `reason`, `evidence_file_id` | String/String/UUID? | Manual ต้องมี Reason และ `source_reference` หรือ Verified Evidence File; Evidence File ต้องผูกกับ Cost Record เดียวกัน |
| `created_by`, `last_financial_editor_id`, `approved_by` | UUID | Maker–Checker Constraint ที่ Use Case + DB transaction |
| `row_version`, Audit columns | Token/Timestamp | Optimistic concurrency + trace |

Published/Superseded/Disabled ห้าม Update Financial Fields; การเปลี่ยนราคา/ช่วงเวลาสร้าง Version ใหม่ PostgreSQL Exclusion Constraint หรือ Transactional Guard ป้องกัน Published Period/Quantity Range ที่ซ้อนใน Natural Key เดียวกัน

Migration เพิ่ม `source_type` ให้ `cost_sources` แบบ additive และ backfill แถวเดิมเป็น `legacy` แบบอ่านได้แต่เลือกใหม่ไม่ได้; ไม่ตีความแถวเดิมว่าเป็น Manual โดยพลการ. ก่อนบังคับ `cost_source_id` ที่ฐานข้อมูลต้องสำรวจ Cost Record เดิมที่เป็น `null`; เก็บแถวเก่าให้อ่านย้อนหลังได้ และห้าม Submit/Publish แถวที่ยังไม่มี Source จนผู้มีสิทธิ์แก้ Draft/Returned หรือสร้างรุ่นใหม่ตาม Lifecycle. ห้ามเติม Source ปลอมหรือแก้ Published Snapshot ย้อนหลัง

### Review and Import

- `cost_record_reviews`: Cost Record, decision, reason code/note, actor, authority snapshot, decided at; Append-only
- `item_import_batches`: file id/hash, template version, mode, status, row counts, created/committed actor/time, idempotency fingerprint, row version, validation summary JSONB
- `item_import_rows`: batch, row number, normalized natural key, operation, status, field errors JSONB, raw values JSONB, resolved target id/version

Raw Import JSONB เป็น Staging/Diagnostics เท่านั้น; Commit map เข้า Relational Core หลัง Validate และไม่ใช้ JSONB เป็น Item/Unit/Cost หลัก

## Cost Resolution Read Model

Input: Organization, Branch, Item, Unit, Currency, Quantity, Effective At, Cost Policy Version

Query ต้อง Filter Published + Effective + Quantity Range แล้วเรียง Branch scope, Source Priority, Effective From ล่าสุด และ Stable ID เพื่อค้น Tie; หากธุรกิจยังเสมอให้คืน `ITEM_COST_AMBIGUOUS` ไม่เลือกเงียบ ๆ Dapper/Parameterized Raw SQL ใช้ได้เฉพาะ Read Model ใน Infrastructure ตาม [Raw SQL Policy](raw-sql-policy.md)

Output ที่ Estimate Snapshot ต้องเก็บ `cost_record_id/version`, source reference, original/resolved amount+unit+currency, conversion factor/path/version, effective period, branch scope, policy version และ staleness/exception reason

### Estimate Cost Component Reference and Snapshot

Cost Component ที่มาจาก Catalog เพิ่ม `item_id`, `cost_record_id`, `cost_record_version`, `item_code_snapshot`, `item_name_snapshot` JSONB, `unit_snapshot`, `unit_cost_snapshot`, `currency_snapshot`, `cost_scope_snapshot`, `cost_effective_from_utc`, `cost_policy_version` และ `resolved_at_utc`

Frontend ส่ง Item/Cost identity ที่เลือกได้ แต่ Backend ต้อง Resolve/Validate ใหม่จาก Organization, Branch, Quantity, Unit, Currency และ Effective At ก่อนบันทึกหรือ Calculate ห้ามเชื่อราคา/ชื่อจาก Client เป็น Authority รายการ Manual ยังคงอนุญาตโดย `item_id=null`, บังคับเหตุผล และใช้กฎ Provisional Cost

### Audit Event Contract

ใช้ `audit.audit_events` และ `AppDbContext.AddAuditEvent` ที่มีอยู่เป็นระบบกลาง ไม่สร้าง Item Audit Table ซ้ำ โดยรองรับ `id`, `organization_id`, `branch_id?`, `actor_user_id`, `actor_membership_id?`, `action`, `resource_type`, `resource_id`, `occurred_at_utc`, `trace_id`, `request_id?`, `row_version_before?`, `row_version_after?`, `reason?` และ `changes` JSONB

`changes` เก็บเฉพาะ Field ที่เปลี่ยนในรูป `{ "field": { "old": value, "new": value } }` และจำกัดขนาด Payload; ห้ามเก็บ Binary, Raw File, Bearer Token, Signed URL, Credential หรือข้อมูลส่วนบุคคลที่ไม่จำเป็น Audit เป็น Append-only และ Business Write + Audit ต้อง Commit/Rollback พร้อมกัน

## Index and Constraints Baseline

- Unique `(organization_id, normalized_code)` บน Item และ `(scope, normalized_code)` บน Unit
- Search index บน normalized code/name/type/category/status; Full-text/Trigram เพิ่มเมื่อวัดแล้วจำเป็น
- Branch availability index `(organization_id, branch_id, status, item_id)` และ Item mode index `(organization_id, availability_mode, status)`
- Category/Brand unique code และ search expression indexes; Category parent index `(organization_id, parent_category_id, status, sort_order)`
- Alias search indexes `(organization_id, normalized_th, item_id)` และ `(organization_id, normalized_en, item_id)` เมื่อภาษาอังกฤษมีค่า
- Item image index `(organization_id, item_id, status, display_order, id)` และ Partial Unique Primary Image
- Cost resolve composite index เริ่มจาก `(organization_id, item_id, status, currency, unit_id, effective_from_utc)` พร้อม Branch/Quantity columns ตาม Query Plan
- Audit index `(organization_id, resource_type, resource_id, occurred_at_utc DESC)` และ `(organization_id, occurred_at_utc DESC)`; Audit Event เป็น Append-only
- Import unique `(organization_id, file_hash, template_version, mode)` ตาม Retry Policy และ `(batch_id, row_number)`
- FK ที่มี Organization ต้องพิสูจน์ Same-organization ด้วย Composite FK หรือ Transactional Guard ที่มี Integration Test
- Retention/Audit/File relationship อ้าง Policy กลาง; Business Record ที่เคยใช้งานห้าม Hard Delete

## Transaction Boundaries

- Activate/Deactivate Item เขียน State + Audit ใน Transaction เดียว
- เปลี่ยน Branch Availability และ Attach/Detach/Reorder/Primary Image เขียน State + Audit ใน Transaction เดียว
- Approve/Return เขียน Decision + State + Authority Snapshot แบบ Atomic
- Publish เขียน Published Version, Supersede รุ่นเดิมตาม Policy, ตรวจ Overlap และ Audit แบบ Atomic
- Import Commit ล็อก Batch/ตรวจ Version แล้ว Apply ทุก Row หรือ Rollback ทั้ง Batch
- Resolver เป็น Read-only; Estimate เป็นผู้รับผิดชอบการเขียน Snapshot ใน Transaction ของ Estimation

## Data Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-DATA-ITEM-001` | Code ต่าง Case/Space ใน Organization เดียวกัน | Unique reject |
| `TC-DATA-ITEM-002` | Code เดียวกันคนละ Organization | อนุญาต |
| `TC-DATA-ITEM-003` | Item ไม่มี Capability แล้ว Activate | Reject |
| `TC-DATA-ITEM-004` | Cost Branch อยู่อีก Organization | FK/transaction reject |
| `TC-DATA-ITEM-005` | Amount ติดลบ | Check reject |
| `TC-DATA-ITEM-006` | Quantity Max ≤ Min | Check reject |
| `TC-DATA-ITEM-007` | Published Period ซ้อน Natural Key | Atomic reject |
| `TC-DATA-ITEM-008` | Update Published Amount | Reject immutable record |
| `TC-DATA-ITEM-009` | Maker เป็น Approver | Reject/rollback |
| `TC-DATA-ITEM-010` | Conversion Cycle | Reject/rollback |
| `TC-DATA-ITEM-011` | Import แถวหนึ่งผิด | ทั้ง Batch ไม่ Commit |
| `TC-DATA-ITEM-012` | Published Cost เปลี่ยนภายหลัง | Estimate Snapshot เดิมไม่เปลี่ยน |
| `TC-DATA-ITEM-013` | `name` ไม่ใช่ Object/มี Key ที่ไม่อนุญาต/ไม่มี `th` ตอน Activate | Check/Application reject |
| `TC-DATA-ITEM-014` | Item แบบ Selected Branch ไม่มี Active Relation ของสาขา | Catalog ไม่คืน Item |
| `TC-DATA-ITEM-015` | File ข้าม Organization/Parent หรือยังไม่ Verified | Relation reject และ Security Audit ตาม Policy |
| `TC-DATA-ITEM-016` | ตั้ง Primary Image สองรายการพร้อมกัน | Partial Unique/Transaction reject |
| `TC-DATA-ITEM-017` | Client ส่ง Unit Cost ที่ไม่ตรง Published Cost | Backend Resolve ใหม่และ reject conflict |
| `TC-DATA-ITEM-018` | Category Parent เป็นลูกหลานของตนเอง | Reject cycle/rollback |
| `TC-DATA-ITEM-019` | Brand หรือ Category ข้าม Organization | Composite FK reject |
| `TC-DATA-ITEM-020` | Alias ซ้ำหลัง Normalize ใน Item เดียวกัน | Unique reject |

Field และ Gate ฉบับเต็มอยู่ที่ [Item Master Field Catalog](../01-business/item-master-field-catalog.md)
# Master Data Codes and Audit

รหัส Item, Category, Brand, Unit, Tax Category และ Cost Source เป็นรหัสระดับองค์กร ไม่ผูกกับสาขาและไม่ reset; Item ทุกประเภทใช้ sequence `items` ร่วมกัน ส่วนรหัสอื่นแยกชุดตามชนิดข้อมูล ค่าเริ่มต้นคือ `ITM-`, `CAT-`, `BRD-`, `UOM-`, `TAX-`, `SRC-` ตามด้วย sequence 5 หลัก

การ Create รองรับรหัสที่ผู้ใช้กำหนดเองหรือ `null` เพื่อ GEN ระบบตรวจสิทธิ์และ idempotency ก่อนจัดสรรเลข แล้วเขียน counter, record, idempotency และ audit ภายใน transaction เดียวกัน หาก generated code ชนกับข้อมูลเดิมจะข้ามเลขนั้น; preview ไม่จองเลข ดูรายละเอียด contract และตัวอย่างได้ที่ [Item Master API Contract](../03-contracts/item-master-api-contract.md).
