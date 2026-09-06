# Quick Estimate Data Contract (ข้อตกลงข้อมูลราคาหน้างาน)

**สถานะ:** Accepted Direction — Logical Data Baseline; ชื่อ Physical Table/Precision จริงยืนยันตอนออก Migration

## เป้าหมาย

กำหนด Ownership, Entity, Field, Constraint, JSONB Boundary และ Index สำหรับ [Quick Estimate API](../03-contracts/quick-estimate-api-contract.md) เพื่อให้ปรับ Template ได้โดยยังรักษา Data Integrity, RBAC Scope และ Reporting

## Aggregate Boundary

```text
QuickEstimate
 ├─ QuickEstimateVersion ─ CalculationSnapshot
 │   ├─ MeasurementLine
 │   ├─ EvidenceLink
 │   └─ CustomerSummarySnapshot
 ├─ ReviewRequest ─ ReviewDecision
 ├─ ShareAttempt
 └─ ConversionLink ─► OfficialEstimate

PricingTemplate ─ PricingTemplateVersion ─ RateSetVersion
                                      └─ BranchRateOverride
```

`QuickEstimate` เป็น Transaction Boundary ของ Draft/Lifecycle ส่วน Pricing Template/Rate เป็น Aggregate อื่นที่ถูกอ้างด้วย Version และ Snapshot ห้ามแก้พร้อมกันใน Transaction เดียวกับ Quick Estimate

## Relational Core

### `quick_estimates`

| Column | Logical Type | Null | Constraint/Meaning |
| --- | --- | ---: | --- |
| `id` | UUID | No | Primary Key |
| `organization_id` | UUID | No | Trusted Scope; Index ตัวแรกของ Business Query |
| `branch_id` | UUID | No | ต้องเป็น Branch ของ Organization |
| `number` | String | No | Unique `(organization_id, number)` |
| `customer_id` | UUID | No | Customer ต้องอยู่ใน Organization/Scope |
| `opportunity_id` | UUID | No | ต้องเป็น Opportunity ของ Customer/Branch เดียวกัน |
| `owner_user_id` | UUID | No | ผู้รับผิดชอบรายการ |
| `status` | Enum | No | `draft`, `calculated`, `pendingReview`, `shareable`, `shared`, `converted`, `closed`, `expired` |
| `current_version_no` | Integer | No | ตั้งแต่ 1 และเพิ่มใน Transaction |
| `latest_share_decision` | Enum/null | Yes | Cache สำหรับ Work Queue; Source จริงอยู่ Version |
| `valid_until` | Date/null | Yes | จาก Version ล่าสุดที่คำนวณ |
| `row_version` | Concurrency Token | No | เปลี่ยนทุก Write |
| `created_at_utc` / `updated_at_utc` | Timestamp UTC | No | Server-owned |

### `quick_estimate_versions`

| Column | Logical Type | Null | Constraint/Meaning |
| --- | --- | ---: | --- |
| `id` | UUID | No | Primary Key |
| `organization_id` | UUID | No | ซ้ำเพื่อบังคับ Scope/Index |
| `quick_estimate_id` | UUID | No | FK + Unique กับ `version_no` |
| `version_no` | Integer | No | Unique `(quick_estimate_id, version_no)` |
| `template_version_id` | UUID | No | Version ที่ใช้จริง |
| `rate_resolution_id` | UUID | No | ชุด Standard/Branch Rate ที่ Resolve แล้ว |
| `schema_version` | Integer | No | Schema ของ JSONB Snapshot |
| `input_snapshot` | JSONB | No | Validated Input/Option/Assumption/Exclusion |
| `calculation_snapshot` | JSONB | No | Immutable Formula Input/Result |
| `net_lower` / `net_upper` | Decimal Money | No | Query/Report โดยไม่ Parse JSONB |
| `display_lower` / `display_upper` | Decimal Money | No | ช่วงที่ลูกค้าเห็น |
| `currency` | Code | No | ค่าเดียวกับ Money ทุก Field |
| `tax_display_policy` | Enum | No | `inclusive`/`exclusive` |
| `share_decision` | Enum | No | `blocked`, `pendingReview`, `shareable` |
| `calculated_at_utc` / `calculated_by` | UTC/UUID | No | Server-owned |
| `published_at_utc` | UTC/null | Yes | เมื่อ Share แล้ว Version เป็น Immutable |

### Child Entities

| Entity | Field สำคัญ | Invariant |
| --- | --- | --- |
| `measurement_lines` | version ID, line ID, sort order, subtype, quantity, unit, typed dimensions, `flexible_input` JSONB | Unique line/sort ต่อ Version; Dimension > 0 ตาม Template |
| `evidence_links` | version ID, file ID, slot code, status, capturedAt, uploadedBy | File ต้อง Verified และอยู่ Scope เดียวกันก่อน Share |
| `review_requests` | source version, requestedBy/At, status | มี Open Request ได้หนึ่งรายการต่อ Source Version |
| `review_decisions` | request ID, reviewer, decision, reason, decidedAt | Reviewer ไม่ใช่ Maker เมื่อ Policy บังคับ |
| `share_attempts` | source version, idempotency fingerprint, channel, locale, status | Unique Idempotency Key ภายใน Organization/Operation |
| `customer_summary_snapshots` | share ID, schema version, JSONB, hash | Immutable; ไม่มี Internal Rate/Factor/Note |
| `conversion_links` | source version, official estimate ID, idempotency fingerprint | Unique Source Version ต่อ Conversion Intent |

## Pricing Reference Entities

| Entity | Relational Field | JSONB Field |
| --- | --- | --- |
| `pricing_templates` | organization, code, work type, owner, status | ไม่มี Core Identity ใน JSONB |
| `pricing_template_versions` | template ID, version, lifecycle, effective period, maker/checker | `template_config` |
| `rate_set_versions` | code, version, item/option, unit, amount, currency, source, effective period, status | Source metadata เฉพาะที่ไม่ใช้ Join |
| `branch_rate_overrides` | branch, standard rate version, amount/factor, period, maker/checker, status | Evidence metadata เฉพาะที่ไม่ใช้ Constraint |

Effective Period ของ Rate/Override สำหรับ Scope+Item+Unit เดียวกันห้ามซ้อน ใช้ Exclusion Constraint หรือ Transactional Validation ที่ป้องกัน Race Condition

## JSONB Envelope

ทุก JSONB Document ใช้ Envelope เดียวกัน:

```json
{
  "schemaVersion": 1,
  "kind": "quick-estimate-input",
  "capturedAtUtc": "2026-09-06T03:15:00Z",
  "data": {}
}
```

กฎ:

- `schemaVersion` และ `kind` บังคับและห้ามเปลี่ยนหลัง Publish
- Application Validate ด้วย Versioned Schema ก่อนเขียน
- Migration ต้องรองรับการอ่าน Version เดิมก่อนเริ่มเขียน Version ใหม่
- ห้ามเก็บ Permission, Role, Organization Scope หรือ Error Translation ใน JSONB
- ห้ามใช้ JSONB เป็นหลักในการ Filter ข้าม Organization หรือ Join Business Entity
- เก็บ Hash ของ Customer/Calculation Snapshot เพื่อช่วยตรวจ Tampering โดย Hash ไม่แทน Audit/Signature

## Snapshot Examples

### Input Snapshot

```json
{
  "schemaVersion": 1,
  "kind": "quick-estimate-input",
  "capturedAtUtc": "2026-09-06T03:15:00Z",
  "data": {
    "propertyType": "house",
    "roomOrArea": "ห้องนอนใหญ่",
    "materialGradeId": "premium",
    "measurementConfidence": "medium",
    "complexityAnswers": [{ "code": "HIDDEN_SYSTEM", "answer": "unknown" }],
    "assumptions": ["ผนังและพื้นพร้อมติดตั้ง"],
    "exclusions": ["ไม่รวมการย้ายระบบไฟ"]
  }
}
```

### Calculation Snapshot

```json
{
  "schemaVersion": 1,
  "kind": "quick-estimate-calculation",
  "capturedAtUtc": "2026-09-06T03:15:01Z",
  "data": {
    "formulaVersion": "qe-hybrid-v1",
    "template": { "code": "QE-BI-WARDROBE-LM", "version": 2 },
    "rateSetVersions": [{ "code": "RATE-BI-WARDROBE", "version": 4 }],
    "billableQuantity": { "value": "3.00", "unit": "m" },
    "riskModifiers": ["HIDDEN_SYSTEM"],
    "rawBounds": { "lower": "43350.00", "upper": "58650.00" },
    "displayedBounds": { "lower": "43000.00", "upper": "59000.00" },
    "currency": "THB",
    "roundingStep": "1000.00"
  }
}
```

ตัวเลขราคาเป็น `TEST_ONLY` Money ใน JSON ใช้ Decimal String

## Immutability Rules

- Draft Version แก้ผ่าน Aggregate และ Concurrency Token เท่านั้น
- Calculate สร้าง/Freeze Calculation Snapshot ที่อ้าง Template/Rate Version ชัดเจน
- Shared Version, Customer Summary และ Review Decision ห้าม Update/Delete โดยผู้ใช้ทั่วไป
- แก้ Measurement, Grade, Assumption หรือ Exclusion หลัง Share ต้องสร้าง Version ใหม่
- Disable Template/Rate ไม่เปลี่ยน Snapshot เดิม
- Retention/Purge ต้องรักษา Legal/Audit Link และไม่ทำให้ Quotation/Official Estimate ขาด Source

## Index Baseline

- `quick_estimates (organization_id, branch_id, status, updated_at_utc desc, id)`
- `quick_estimates (organization_id, owner_user_id, status, updated_at_utc desc, id)`
- `quick_estimates (organization_id, opportunity_id, id)`
- Unique `quick_estimate_versions (quick_estimate_id, version_no)`
- `quick_estimate_versions (organization_id, valid_until, share_decision)`
- Unique `share_attempts (organization_id, operation, idempotency_fingerprint)`
- Unique `conversion_links (source_quick_estimate_version_id, intent_key)`
- Effective lookup บน Template/Rate ใช้ `(organization_id, scope, code, status, effective_from_utc, effective_to_utc)`

ทุก Index ที่มีข้อมูลธุรกิจหลายองค์กรต้องนำ `organization_id` เป็นส่วนของ Access Path ที่ใช้จริง และต้องยืนยันด้วย Query Plan ก่อน Production

## Transaction Boundaries

- Create/Autosave: Quick Estimate Aggregate หนึ่งรายการ
- Calculate: Lock/ตรวจ Draft Version → Resolve Effective Template/Rate → Insert Version/Snapshot → Update Header ใน Transaction เดียว
- Review Decision: Lock Open Request → ตรวจ Maker–Checker → Insert Decision → Update Review State
- Activate Template: Version ใหม่ Active และ Version เดิม Superseded ใน Transaction เดียว
- Conversion: Insert Official Estimate Draft + Conversion Link แบบ Idempotent ใน Transaction เดียว
- File Binary อยู่นอก Database; Database เก็บ Metadata/State และ Outbox/Event สำหรับการยืนยันผลเมื่อจำเป็น

## Data Integrity Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-DATA-QE-001` | Duplicate Number ใน Organization เดียวกัน | Unique violation |
| `TC-DATA-QE-002` | Number เดียวกันต่าง Organization | อนุญาต |
| `TC-DATA-QE-003` | Opportunity ไม่ตรง Customer/Branch | Reject ก่อน Commit |
| `TC-DATA-QE-004` | Version No ซ้ำใน Quick Estimate | Unique violation |
| `TC-DATA-QE-005` | Money มี Precision เกิน Contract | Reject/Round ตาม Domain Rule ก่อน Persist |
| `TC-DATA-QE-006` | JSONB ไม่มี `schemaVersion`/`kind` | Reject |
| `TC-DATA-QE-007` | JSONB Schema Version ไม่รองรับ | Reject โดยไม่เขียน Partial Data |
| `TC-DATA-QE-008` | Template/Rate Reference ข้าม Organization | Reject และ Audit Security Event |
| `TC-DATA-QE-009` | Update Shared Version/Snapshot | Reject immutable operation |
| `TC-DATA-QE-010` | Rate Effective Period ซ้อนกันจาก Concurrent Requests | มีได้เพียงหนึ่ง Request สำเร็จ |
| `TC-DATA-QE-011` | Share Idempotency Fingerprint ซ้ำ | คืน/อ้างผลเดิม ไม่สร้าง Attempt ซ้ำ |
| `TC-DATA-QE-012` | Activate Version ใหม่ | Active เดิม Superseded ใน Transaction เดียว |
| `TC-DATA-QE-013` | Disable Template ที่มี Snapshot เดิม | Snapshot ยังอ่าน/ทำซ้ำได้ |
| `TC-DATA-QE-014` | Query Branch A | ไม่คืน Record ของ Branch B/Organization อื่น |

## Open Physical Decisions

ค่าต่อไปนี้ตัดสินตอน Logical-to-Physical Schema Review ไม่ควรเดาก่อนมี Volume/Pilot Data:

- Precision/Scale ของ Measurement, Rate และ Money แต่ละชนิด
- PostgreSQL mechanism สำหรับ `row_version`
- Partition/Archival ของ Audit/Share Attempt
- JSON Schema registry/storage และระยะรองรับ Schema Version เก่า
- Encryption/Retention ของ Offline Draft และ File Metadata
