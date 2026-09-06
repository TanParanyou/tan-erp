# Item Master Data Contract (ข้อตกลงข้อมูลสินค้า หน่วย และต้นทุน)

**สถานะ:** Accepted Direction — Logical Schema Baseline

## Aggregate and Ownership

```text
Organization
 ├─ ItemCategory
 ├─ Unit
 ├─ CostSource
 └─ Item
     ├─ Capability Flags
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
| `category_id`, `base_unit_id` | UUID | ต้องอยู่ Organization/Shared Scope ที่อนุญาต |
| `name_th`, `name_en`, `description_th`, `description_en` | String/Nullable | `name_th` บังคับก่อน Active |
| `can_sell`, `can_cost`, `can_purchase`, `can_stock`, `can_produce` | Boolean | Typed flags; อย่างน้อยหนึ่งค่า True ก่อน Active |
| `status` | String | `draft|active|inactive` |
| `activated_once`, `inactive_reason` | Boolean/String? | Code immutable เมื่อ `activated_once=true`; Inactive บังคับเหตุผล |
| `row_version` | Concurrency token | Compare-and-swap |
| Audit columns | UUID/Timestamp | UTC, actor และ request trace |

Capability ใช้ Typed Columns ไม่ใช้ EAV/JSONB เพื่อให้ Constraint, Query และสิทธิ์ตรวจสอบได้ตรงไปตรงมา

### Unit and Conversion

`units` มี `code`, ชื่อไทย/อังกฤษ, `dimension`, `decimal_scale`, `rounding_mode`, `status` และ Unique Code ตาม Scope

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

## Index and Constraints Baseline

- Unique `(organization_id, normalized_code)` บน Item และ `(scope, normalized_code)` บน Unit
- Search index บน normalized code/name/type/category/status; Full-text/Trigram เพิ่มเมื่อวัดแล้วจำเป็น
- Cost resolve composite index เริ่มจาก `(organization_id, item_id, status, currency, unit_id, effective_from_utc)` พร้อม Branch/Quantity columns ตาม Query Plan
- Import unique `(organization_id, file_hash, template_version, mode)` ตาม Retry Policy และ `(batch_id, row_number)`
- FK ที่มี Organization ต้องพิสูจน์ Same-organization ด้วย Composite FK หรือ Transactional Guard ที่มี Integration Test
- Retention/Audit/File relationship อ้าง Policy กลาง; Business Record ที่เคยใช้งานห้าม Hard Delete

## Transaction Boundaries

- Activate/Deactivate Item เขียน State + Audit ใน Transaction เดียว
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

Field และ Gate ฉบับเต็มอยู่ที่ [Item Master Field Catalog](../01-business/item-master-field-catalog.md)
