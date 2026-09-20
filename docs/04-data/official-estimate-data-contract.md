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

Estimate เป็น Identity/Lifecycle ส่วน Revision เป็น Business Snapshot ที่เป็น Draft ได้หนึ่งฉบับตาม Policy และ Immutable เมื่อ Approved/Quoted/Cancelled

## Relational Core

### `estimates`

| Column | Type | Rule |
| --- | --- | --- |
| `id` | UUID | PK |
| `organization_id`, `branch_id` | UUID | Trusted Scope; FK ต้องสัมพันธ์กัน |
| `number` | String | Unique `(organization_id, number)` |
| `customer_id`, `opportunity_id` | UUID | Customer/Opportunity/Branch เดียวกัน |
| `site_survey_revision_id`, `site_survey_snapshot_hash` | UUID/String/null | Optional Ready/Superseded Source ใน Scope; Freeze Revision ที่ใช้ |
| `source_quick_estimate_version_id` | UUID/null | Optional เท่านั้น |
| `owner_user_id` | UUID | ผู้รับผิดชอบ |
| `status` | Enum | draft/submitted/returned/approved/quoted/cancelled; mirror Current Revision |
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
| `status` | Enum | draft/submitted/returned/approved/quoted/cancelled |
| `currency` | ISO Code | Currency เดียวต่อ Revision ระยะแรก |
| `calculation_policy_version_id`, `tax_policy_version_id` | UUID/null | Resolve ตอน Calculate และ Freeze ใน Snapshot |
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
| `estimate_sections` | revision, code, nameTh/nameEn, description, sortOrder, subtotal | Unique code/sort ต่อ Revision |
| `estimate_work_items` | section, code, itemId, descriptionTh/En, scopeNote, quantity, unit, sellingRuleType/value, cost/selling/margin, sortOrder | Quantity > 0; Unit Active; totals Server-derived |
| `estimate_cost_components` | workItem, type, item/service ID, description, quantity, unit, unitCost, currency, costRecord ID/version, costSource snapshot, conversion snapshot, cost policy version, effectiveAt, provisional flag/reason | Type material/labor/subcontract/service/other-direct; Amount ≥0; Master Data เปลี่ยนแล้ว Snapshot เดิมไม่เปลี่ยน |
| `estimate_adjustments` | revision/workItem scope, type, basis, value, reason, permission context | Published rule หรือ authorized override |
| `estimate_tax_lines` | revision, taxCode, rate, taxableBase, amount, effectiveAt | Server-derived จาก Tax Policy |

Core BOQ ต้องเป็น Relational Rows ห้ามเก็บ Sections/Work Items/Cost Components เป็น JSONB ก้อนเดียว เพราะต้อง Constraint, Query, Diff และ Report

Type/Precision/Required Gate/Visibility ของ Field อ้าง [Official Estimate Field Catalog](../01-business/official-estimate-field-catalog.md) เพื่อไม่กำหนด Business Rule ซ้ำใน Physical Model

Cost Record/Conversion/Resolver Source of Truth อยู่ที่ [Item Master Data Contract](item-master-data-contract.md) Estimate เก็บเฉพาะ Reference + Frozen Snapshot ที่ใช้คำนวณ ไม่เป็นเจ้าของ Current Cost

## Versioned Policies

| Entity | Field สำคัญ | Rule |
| --- | --- | --- |
| `calculation_policy_versions` | organization/branch scope, code, version, effective period, pricing/overhead/rounding config, status, hash | Published Version immutable; Effective Period ไม่ซ้อนใน Scope เดียวกัน |
| `tax_policy_versions` | organization/branch scope, code, version, effective period, tax/display config, status, hash | Published Version immutable; Rate ไม่ Hard-code ใน Estimate Logic |
| `approval_policy_versions` | organization/branch scope, code, version, effective period, trigger/authority/route config, status, hash | Published Version immutable; Production Bootstrap แยกเป็น System Policy |
| `approval_authorities` | policy version, subject/permission/scope, amount/margin/discount limits, effective period | Permission ไม่แทน Authority; Query ต้องใช้ Decimal/Currency เดียวกับ Policy |

Policy Configuration ใช้ Typed Header/Scope/Status/Effective Period และ JSONB เฉพาะ Versioned Rule Payload ที่ Validate Schema แล้ว ระบบต้องอ่าน Version เก่าได้ตลอด Retention ของ Estimate ที่อ้าง

## Approval and Quotation

| Entity | Field สำคัญ | Rule |
| --- | --- | --- |
| `estimate_approval_requests` | revision, routeVersion, requestedBy/At, status | หนึ่ง Open Route ต่อ Revision |
| `estimate_approval_steps` | request, sequence, permission/scope, thresholdSnapshot, status | ลำดับคงที่หลัง Submit |
| `estimate_approval_decisions` | step, reviewer, approved/returned, reason, note, decidedAt | Append-only; Maker–Checker |
| `estimate_approval_snapshots` | revision, calculation hash, policy version, JSONB | Freeze ตอน Approved |
| `commercial.quotations` | id, organization_id, branch_id, customer_id, opportunity_id, estimate_id, estimate_revision_id, number, status, total_amount, snapshot_hash, issued_at_utc, accepted_at_utc, row_version | Commercial Quotation ออกจาก Estimate Revision ที่คำนวณแล้ว |
| `common.document_sequence_definitions` | id, organization_id, document_type, prefix, format_pattern, reset_period, padding, is_branch_specific, is_active | กำหนดรูปแบบเลขที่เอกสาร |
| `common.document_sequence_counters` | id, organization_id, document_type, period_key, current_value | ตัวนับเลขที่เอกสารระดับ atomic sequence |

`estimate_approval_requests` ต้องอ้าง `approval_policy_version_id`, Calculation Snapshot Hash และ Frozen Route Hash ส่วน `In Review` derive จาก Open Request/Active Step ไม่เก็บเป็น Revision Status

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
    "calculationPolicyVersionId": "EST-CALC-TH-v1",
    "taxPolicyVersionId": "TAX-TH-v1",
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
    "approvalReasons": ["PROVISIONAL_COST"]
  }
}
```

ตัวเลขเป็น `TEST_ONLY` JSON Money ใช้ Decimal String

## Invariants

- Work Item/Cost Component/Adjustment ต้องอยู่ Revision และ Organization เดียวกัน
- Quantity ใช้ Decimal(18,4), Unit Cost Decimal(19,4), Rate Decimal(12,6) และ Money Decimal(19,2)
- Client เขียน Total, Margin, Tax หรือ Approval State โดยตรงไม่ได้
- Draft เปลี่ยน Financial Input ต้องตั้ง `calculation_outdated=true`
- Submit ต้องอ้าง Calculation Version ล่าสุดและผ่าน Blocking Validation
- Approved/Quoted Revision, Calculation/Approval/Quotation Snapshot แก้หรือลบไม่ได้
- Revision ใหม่ Clone เนื้อหาผ่าน Application Use Case และอ้าง Parent; ไม่ Share Row ระหว่าง Revision
- Quotation ต้องอ้าง Approved Revision และ Snapshot Hash ตรงกัน
- Delete Item ที่ถูกใช้ใน Published Revision ให้ Inactive ที่ Master Data ไม่ Cascade ลบประวัติ
- Customer-facing Projection ต้องใช้ Allowlist; ห้ามรวม Cost, Margin/Markup, Internal Note, Trigger, Threshold หรือ Approval Detail
- Cancelled Revision แก้ไม่ได้; Cancel Submitted ต้องปิด Open Route และเปลี่ยน State แบบ Atomic

## Index Baseline

- `estimates (organization_id, branch_id, status, updated_at_utc desc, id)`
- `estimates (organization_id, owner_user_id, status, updated_at_utc desc, id)`
- `estimates (organization_id, customer_id, opportunity_id, id)`
- Unique `estimate_revisions (estimate_id, revision_no)`
- `estimate_work_items (revision_id, section_id, sort_order, id)`
- `estimate_cost_components (work_item_id, type, id)`
- `estimate_approval_requests (organization_id, status, requested_at_utc, id)`
- `calculation_policy_versions (organization_id, branch_id, status, effective_from, effective_to, id)`
- `approval_policy_versions (organization_id, branch_id, status, effective_from, effective_to, id)`
- Unique Idempotency Fingerprint ต่อ Organization + Operation

ทุก Business Access Path ต้องรวม `organization_id` ที่ Boundary และทดสอบ Query Plan กับข้อมูลใกล้ Production ก่อนเพิ่ม Index ซ้ำ

## Transaction Boundaries

- Autosave: lock Estimate/Revision → validate scope/ETag → mutate Draft → mark calculation outdated
- Calculate: resolve Effective Cost/Rule → insert Snapshot → update derived totals/version ใน Transaction เดียว
- Submit: validate latest calculation → create frozen Approval Route → change state
- Approve: lock active step → enforce Maker–Checker → append decision/snapshot → change revision state
- Cancel: lock Revision/Open Route → validate Authority/Reason → close Route → change Revision/Header state
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
| `TC-DATA-EST-015` | Effective Policy Version ซ้อน Scope/Period | Reject |
| `TC-DATA-EST-016` | แก้ Published Policy Version | Reject Immutable |
| `TC-DATA-EST-017` | Submit แล้ว Publish Policy ใหม่ | Approval Request เดิมอ้าง Frozen Version/Route เดิม |
| `TC-DATA-EST-018` | Customer Projection จาก Approved Revision | ไม่มี Internal Cost/Margin/Threshold/Note |
| `TC-DATA-EST-019` | Cancel Submitted ระหว่าง Review | Route ปิดและ Revision Cancelled ใน Transaction เดียว |

## Open Physical Decisions

- PostgreSQL mechanism ของ `row_version` และ Effective-period exclusion
- Approval Route storage เมื่อมีหลายขั้น/หลายผู้อนุมัติ
- Snapshot Schema Registry, Retention และ Migration Window
