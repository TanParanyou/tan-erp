# Official Estimate Data Contract (ข้อตกลงข้อมูลประมาณการทางการ)

**สถานะ:** Accepted Direction — Logical Schema Baseline

## Aggregate

```text
Estimate
 └─ EstimateRevision
     ├─ EstimateSection
     │   └─ WorkItem
     │       └─ CostComponent
     ├─ CalculationSnapshot
     ├─ ApprovalRequest ─ ApprovalDecision
     └─ QuotationLink ─► Quotation
```

Estimate เป็น Identity/Lifecycle ส่วน Revision เป็น Business Snapshot ที่เป็น Draft ได้หนึ่งฉบับตาม Policy และ Immutable เมื่อ Approved/Quoted

## Relational Core

### `estimates`

| Column | Type | Rule |
| --- | --- | --- |
| `id` | UUID | PK |
| `organization_id`, `branch_id` | UUID | Trusted Scope; FK ต้องสัมพันธ์กัน |
| `number` | String | Unique `(organization_id, number)` |
| `customer_id`, `opportunity_id` | UUID | Customer/Opportunity/Branch เดียวกัน |
| `site_survey_id` | UUID/null | Optional Source ใน Scope |
| `source_quick_estimate_version_id` | UUID/null | Optional เท่านั้น |
| `owner_user_id` | UUID | ผู้รับผิดชอบ |
| `status` | Enum | draft/submitted/returned/approved/quoted/accepted/expired/cancelled |
| `current_revision_no` | Integer | ตั้งแต่ 1 |
| `row_version` | Token | เปลี่ยนทุก Write |
| `created_at_utc`, `updated_at_utc` | UTC | Server-owned |

### `estimate_revisions`

| Column | Type | Rule |
| --- | --- | --- |
| `id` | UUID | PK |
| `organization_id`, `estimate_id` | UUID | Scope + FK |
| `revision_no` | Integer | Unique `(estimate_id, revision_no)` |
| `parent_revision_id` | UUID/null | Revision 1 เป็น null |
| `change_reason_code`, `change_reason` | String/null | บังคับตั้งแต่ Revision 2 |
| `status` | Enum | draft/submitted/returned/approved/quoted |
| `currency` | ISO Code | Currency เดียวต่อ Revision ระยะแรก |
| `calculation_version` | Integer | เพิ่มเมื่อ Calculate สำเร็จ |
| `calculation_outdated` | Boolean | true เมื่อ Financial Input เปลี่ยน |
| `net_cost`, `selling_before_discount` | Decimal Money | Server-derived |
| `discount_amount`, `net_before_tax` | Decimal Money | Server-derived |
| `tax_amount`, `grand_total` | Decimal Money | Server-derived |
| `margin_amount`, `margin_rate`, `markup_rate` | Decimal | เก็บทั้งผลและฐานสูตร |
| `calculation_snapshot` | JSONB | Immutable ต่อ Calculation Version |
| `approved_at_utc`, `published_at_utc` | UTC/null | Lifecycle marker |
| `row_version` | Token | Draft concurrency |

### BOQ Entities

| Entity | Typed Field สำคัญ | Constraint |
| --- | --- | --- |
| `estimate_sections` | revision, code, nameTh/nameEn, sortOrder, subtotal | Unique code/sort ต่อ Revision |
| `estimate_work_items` | section, code, description, quantity, unit, sellingRuleType/value, cost/selling/margin, sortOrder | Quantity > 0; Unit Active; totals Server-derived |
| `estimate_cost_components` | workItem, type, item/service ID, description, quantity, unit, unitCost, currency, costSource, effectiveAt | Type material/labor/service/other; Amount ≥0 |
| `estimate_adjustments` | revision/workItem scope, type, basis, value, reason, permission context | Published rule หรือ authorized override |
| `estimate_tax_lines` | revision, taxCode, rate, taxableBase, amount, effectiveAt | Server-derived จาก Tax Policy |

Core BOQ ต้องเป็น Relational Rows ห้ามเก็บ Sections/Work Items/Cost Components เป็น JSONB ก้อนเดียว เพราะต้อง Constraint, Query, Diff และ Report

## Approval and Quotation

| Entity | Field สำคัญ | Rule |
| --- | --- | --- |
| `estimate_approval_requests` | revision, routeVersion, requestedBy/At, status | หนึ่ง Open Route ต่อ Revision |
| `estimate_approval_steps` | request, sequence, permission/scope, thresholdSnapshot, status | ลำดับคงที่หลัง Submit |
| `estimate_approval_decisions` | step, reviewer, approved/returned, reason, note, decidedAt | Append-only; Maker–Checker |
| `estimate_approval_snapshots` | revision, calculation hash, policy version, JSONB | Freeze ตอน Approved |
| `estimate_quotation_links` | revision, quotation ID, issuedAt, snapshot hash | Quotation อ้าง Approved Revision |
| `customer_acceptances` | quotation, decision, channel, occurredAt, evidenceFileId | Append-only ตาม Retention Policy |

## JSONB Boundary

ใช้ Envelope:

```json
{
  "schemaVersion": 1,
  "kind": "official-estimate-calculation",
  "capturedAtUtc": "2026-09-06T04:20:00Z",
  "data": {}
}
```

JSONB ที่อนุญาต:

- Calculation Rule Configuration ที่มี Version
- Calculation Snapshot: input/rate/rule/intermediate/result ก่อน–หลังปัด
- Approval Policy Snapshot และ Customer-facing Quotation Snapshot
- Metadata เสริมที่ไม่ใช้เป็น Scope, Permission, FK หรือ Financial Total หลัก

ทุก Payload ต้องมี `schemaVersion`/`kind`, Validate ก่อนเขียน, อ่าน Version เก่าได้ และ Immutable หลัง Publish ห้ามเก็บ Error Translation, Permission หรือ Core State ใน JSONB

## Calculation Snapshot Example

```json
{
  "schemaVersion": 1,
  "kind": "official-estimate-calculation",
  "capturedAtUtc": "2026-09-06T04:20:00Z",
  "data": {
    "ruleVersion": "estimate-pricing-v1",
    "revision": 2,
    "costSourceVersions": ["COST-HMR-18-v3", "LABOR-CARPENTER-v2"],
    "totals": {
      "cost": "128000.00",
      "sellingBeforeDiscount": "185000.00",
      "discount": "0.00",
      "netBeforeTax": "185000.00",
      "tax": "12950.00",
      "grandTotal": "197950.00",
      "currency": "THB"
    },
    "marginRate": "0.3081",
    "roundingRule": "currency-2dp",
    "approvalReasons": ["COST_COMPONENT_MISSING"]
  }
}
```

ตัวเลขเป็น `TEST_ONLY` JSON Money ใช้ Decimal String

## Invariants

- Work Item/Cost Component/Adjustment ต้องอยู่ Revision และ Organization เดียวกัน
- Client เขียน Total, Margin, Tax หรือ Approval State โดยตรงไม่ได้
- Draft เปลี่ยน Financial Input ต้องตั้ง `calculation_outdated=true`
- Submit ต้องอ้าง Calculation Version ล่าสุดและผ่าน Blocking Validation
- Approved/Quoted Revision, Calculation/Approval/Quotation Snapshot แก้หรือลบไม่ได้
- Revision ใหม่ Clone เนื้อหาผ่าน Application Use Case และอ้าง Parent; ไม่ Share Row ระหว่าง Revision
- Quotation ต้องอ้าง Approved Revision และ Snapshot Hash ตรงกัน
- Delete Item ที่ถูกใช้ใน Published Revision ให้ Inactive ที่ Master Data ไม่ Cascade ลบประวัติ

## Index Baseline

- `estimates (organization_id, branch_id, status, updated_at_utc desc, id)`
- `estimates (organization_id, owner_user_id, status, updated_at_utc desc, id)`
- `estimates (organization_id, customer_id, opportunity_id, id)`
- Unique `estimate_revisions (estimate_id, revision_no)`
- `estimate_work_items (revision_id, section_id, sort_order, id)`
- `estimate_cost_components (work_item_id, type, id)`
- `estimate_approval_requests (organization_id, status, requested_at_utc, id)`
- Unique Idempotency Fingerprint ต่อ Organization + Operation

ทุก Business Access Path ต้องรวม `organization_id` ที่ Boundary และทดสอบ Query Plan กับข้อมูลใกล้ Production ก่อนเพิ่ม Index ซ้ำ

## Transaction Boundaries

- Autosave: lock Estimate/Revision → validate scope/ETag → mutate Draft → mark calculation outdated
- Calculate: resolve Effective Cost/Rule → insert Snapshot → update derived totals/version ใน Transaction เดียว
- Submit: validate latest calculation → create frozen Approval Route → change state
- Approve: lock active step → enforce Maker–Checker → append decision/snapshot → change revision state
- New Revision: clone approved/quoted source + parent link + change reason ใน Transaction เดียว
- Issue Quotation: validate Approved Revision → create Quotation/Snapshot/Link แบบ Idempotent

## Data Integrity Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-DATA-EST-001` | Duplicate Estimate Number ใน Organization | Unique violation |
| `TC-DATA-EST-002` | Opportunity/Customer/Branch คนละ Scope | Reject |
| `TC-DATA-EST-003` | Duplicate Revision Number | Unique violation |
| `TC-DATA-EST-004` | Quantity ≤0 หรือ Money <0 โดยไม่ใช่ Adjustment | Reject |
| `TC-DATA-EST-005` | Client เขียน Derived Total | Ignore/Reject ที่ API Boundary |
| `TC-DATA-EST-006` | Financial Input เปลี่ยน | `calculation_outdated=true` |
| `TC-DATA-EST-007` | Update Approved Revision | Reject immutable operation |
| `TC-DATA-EST-008` | JSONB ไม่มี/ไม่รองรับ Schema Version | Reject แบบ Atomic |
| `TC-DATA-EST-009` | Cost Source ข้าม Organization | Reject + Security Audit |
| `TC-DATA-EST-010` | Concurrent Approval Decision | มี Decision สำเร็จหนึ่งรายการต่อ Step |
| `TC-DATA-EST-011` | Revision ใหม่จาก Approved | Clone ครบ อ้าง Parent และต้นฉบับไม่เปลี่ยน |
| `TC-DATA-EST-012` | Quotation อ้าง Draft Revision | Reject |
| `TC-DATA-EST-013` | Retry Quotation Key เดิม | ไม่สร้าง Quotation/Link ซ้ำ |
| `TC-DATA-EST-014` | Query Branch A | ไม่คืน Branch B/Organization อื่น |

## Open Physical Decisions

- Precision/Scale จริงของ Quantity, Rate, Money, Margin และ Tax
- PostgreSQL mechanism ของ `row_version` และ Effective-period exclusion
- Approval Route storage เมื่อมีหลายขั้น/หลายผู้อนุมัติ
- Snapshot Schema Registry, Retention และ Migration Window
