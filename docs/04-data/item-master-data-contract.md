# Item Master Data Contract (ข้อตกลงข้อมูลสินค้า หน่วย และต้นทุน)

**สถานะ:** Accepted Direction — Logical Schema Baseline

## Aggregate and Ownership

```text
Organization
 ├─ ItemCategory
 ├─ ItemBrand
 ├─ Unit
 ├─ CostSource
 └─ Item
     ├─ Capability Flags
     ├─ ItemAlias
     ├─ ItemBranchAvailability ─ Branch
     ├─ ItemImage ─ Verified File
     ├─ ItemUnitConversion
     └─ CostRecord ─ CostReview

ItemImportBatch ─ ItemImportRow
Estimate Cost Component ─► Item/CostRecord/Conversion Snapshot
```

Item เป็นเจ้าของตัวตน/ประเภท/Capability/หน่วยฐาน ส่วน Cost Record เป็น Versioned Financial Record แยก Lifecycle; Item ไม่มี Column `current_cost`

## Relational Core

### `items`

| Column | Type | Rule |
| --- | --- | --- |
| `id`, `organization_id` | UUID | PK และ Trusted Scope |
| `code`, `normalized_code` | String | Unique `(organization_id, normalized_code)` |
| `item_type` | Enum/String | `material|labor|service|subcontract|other` |
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

`item_categories` มี `id`, `organization_id`, `code`, `normalized_code`, `name` JSONB, `description` JSONB, `parent_category_id?`, `allowed_item_types`, `sort_order`, `status`, `row_version` และ Audit Columns; Parent ต้องอยู่ Organization เดียวกันและห้าม Cycle ตารางเดียวรองรับทั้ง Category/Subcategory โดยไม่สร้าง `item_subcategories`

`item_brands` มี `id`, `organization_id`, `code`, `normalized_code`, `name` JSONB, `description` JSONB, `sort_order`, `status`, `row_version` และ Audit Columns พร้อม Unique `(organization_id, normalized_code)` Item อ้าง Brand แบบ Nullable เพื่อรองรับ Labor/Service ที่ไม่มี Brand

`item_aliases` มี `id`, `organization_id`, `item_id`, `alias` JSONB, `normalized_th`, `normalized_en?`, `status`, `row_version` และ Audit Columns พร้อม Unique ต่อ Item/ภาษา/ค่าที่ Normalize แล้ว Alias ใช้เพื่อค้นหาเท่านั้น ไม่แทนชื่อ Item และไม่ถูก Snapshot เป็น Description อัตโนมัติ

### Item Images and File Metadata

`item_images` เป็น Relation ไปยัง `files.uploaded_files` และมี `id`, `organization_id`, `item_id`, `file_id`, `role=primary|gallery|technical`, `is_primary`, `display_order`, `alt_text` JSONB, `caption` JSONB, `status=active|inactive`, `row_version` และ Audit Columns

- Unique `(organization_id, item_id, file_id)` และ Partial Unique `(organization_id, item_id) WHERE is_primary=true AND status='active'`
- File ต้อง `verified`, อยู่ Organization เดียวกัน และถูกสร้างจาก Upload Session ที่ `parent_type=item`, `parent_id=item_id`
- Binary, Base64 และ Public URL ห้ามอยู่ใน `items`/`item_images`; API คืน `fileId` และ Authorized Content URL เท่านั้น
- Production File Metadata เพิ่ม `content_sha256`, `width`, `height`, `scan_status`, `verified_at_utc`; Variant/Thumbnail อ้าง File แยกหรือ Derived Asset Relation โดยไม่เขียนทับ Original
- Allowlist เฉพาะ JPEG/PNG/WebP, ตรวจ Magic Number, จำกัด 10 MB ต่อ Original, ลบ EXIF/GPS, Scan ก่อน Verified และใช้ Private Object Storage ใน Production

### Unit and Conversion

`units` มี `code`, `name` JSONB ตาม Localized Text Contract, `symbol`, `dimension`, `decimal_scale`, `rounding_mode`, `status` และ Unique Code ตาม Scope

`unit_conversions` ใช้กับ Exact Conversion กลาง; `item_unit_conversions` ใช้ Packaging/ขนาดเฉพาะ Item โดยมี `from_unit_id`, `to_unit_id`, `factor`, `effective_from/to`, `reason`, `status`, `row_version` ห้าม Factor ≤ 0, Self-loop, Cycle และ Period ซ้อนของคู่เดียวกัน

### `cost_sources`

มี `source_type`, `name`, `supplier_id?`, `reference_number?`, `evidence_file_id?`, `captured_at_utc`, `expires_at_utc?`, `status` และ Audit Cost Source เป็นหลักฐาน ไม่เป็นราคาหรือ Permission

### `cost_records`

| Column | Type | Rule |
| --- | --- | --- |
| `id`, `organization_id`, `item_id` | UUID | PK/Scope/FK |
| `version_number`, `source_id` | Integer/UUID | Unique ต่อ Item/Scope/Natural Key |
| `scope_type`, `branch_id` | String/UUID? | `organization` ต้องไม่มี Branch; `branch` ต้องมี Branch ใน Organization |
| `unit_id`, `currency` | UUID/CHAR(3) | Unit ใช้กับ Item ได้; ISO Currency |
| `amount` | Numeric | `>= 0`; Zero บังคับ Reason/Policy |
| `minimum_quantity`, `maximum_quantity` | Numeric | Min ≥ 0; Max null หรือ > Min |
| `effective_from_utc`, `effective_to_utc` | Timestamp | To null หรือ > From |
| `status` | String | `draft|submitted|returned|approved|published|superseded|disabled` |
| `reason`, `evidence_file_id` | String/UUID? | บังคับตาม Source/Exception Policy |
| `created_by`, `last_financial_editor_id`, `approved_by` | UUID | Maker–Checker Constraint ที่ Use Case + DB transaction |
| `row_version`, Audit columns | Token/Timestamp | Optimistic concurrency + trace |

Published/Superseded/Disabled ห้าม Update Financial Fields; การเปลี่ยนราคา/ช่วงเวลาสร้าง Version ใหม่ PostgreSQL Exclusion Constraint หรือ Transactional Guard ป้องกัน Published Period/Quantity Range ที่ซ้อนใน Natural Key เดียวกัน

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
