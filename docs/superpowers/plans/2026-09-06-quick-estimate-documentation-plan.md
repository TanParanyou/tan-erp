# Quick Estimate Documentation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** เพิ่ม Quick Estimate เป็น Flow ทางการของ Documentation Foundation ให้เจ้าหน้าที่ประเมินช่วงราคาหน้างานได้เร็ว ส่งสรุปเบื้องต้นอย่างปลอดภัย และ Convert เป็น Official Estimate Draft ได้โดยไม่คีย์ข้อมูลซ้ำ

**Architecture:** Markdown เป็นแหล่งอ้างอิงหลักของ Business Flow, Requirements, Permission และ Data Ownership ส่วน Static Portal อ่าน Quick Estimate diagram จาก JSON แยกไฟล์ Viewer เดิมไม่ต้องเปลี่ยน เพราะ `portal.json` ทำหน้าที่เป็น Catalog และ `flow.schema.json` รองรับโครงสร้าง Node/Edge อยู่แล้ว

**Tech Stack:** Markdown, JSON, JSON Schema และ Static Documentation Portal เดิม

> เอกสารนี้เป็น Execution Record ของ Baseline เดิม ชื่อ Permission ปัจจุบันให้ยึด [Permission Catalog](../../03-contracts/permission-catalog.md) เป็นแหล่งอ้างอิงหลัก

## Global Constraints

- Repository อยู่ใน Documentation Foundation; ไม่มี Frontend/Backend ERP, Database หรือ Cloud Resource ในแผนนี้
- ระบบคือ Project ERP; MRP เป็น Future Module
- ใช้คำ `Quick Estimate (ราคาประเมินเบื้องต้น)` และ `Official Estimate (ประมาณการทางการ)` ตาม `CONTEXT.md`
- Quick Estimate เป็น Business Record แยกจาก Official Estimate และ Convert แบบ Snapshot
- Customer-facing Summary แสดง Price Range, Assumptions, Exclusions และ Validity พร้อมคำเตือนว่าไม่ใช่ Quotation
- การเปลี่ยนข้อมูลสำคัญหลังแชร์ต้องสร้าง Version ใหม่
- Backend ในอนาคตเป็นผู้ตรวจ Permission, Scope, Share Policy และ Maker–Checker
- Release แรกเป็น Online-first พร้อม Autosave, Retry, Idempotency และ Safe Local Draft; ยังไม่ทำ Offline-first synchronization
- JSON ทุก Node ต้องมีข้อความ `th` และ `en`; HTML ห้าม Hard-code Quick Estimate Flow
- ไฟล์ที่ไม่เกี่ยวข้องและยังไม่ถูกติดตาม เช่น `design.md`, `preview.html`, `.agents/` และ `.codex/` ไม่อยู่ในขอบเขตและห้าม Stage

---

## Planned File Map

```text
docs/
├── README.md                                      # เพิ่มทางลัดไป Quick Estimate
├── 00-overview/
│   ├── requirements-catalog.md                   # เพิ่ม Functional/NFR ของงานหน้างาน
│   └── scope-and-non-goals.md                    # เพิ่ม Quick Estimate ใน Phase Estimation
├── 01-business/
│   ├── quick-estimate-flow.md                    # Source of truth ใหม่
│   ├── end-to-end-business-flow.md               # แทรก Quick Estimate เป็น optional fast path
│   ├── estimation-flow.md                        # เชื่อม Convert เข้าสู่ Official Estimate
│   ├── roles-and-responsibilities.md             # เพิ่มหน้าที่ Field Estimator/Reviewer
│   └── approval-matrix.md                        # เพิ่ม Share Policy triggers
├── 03-contracts/
│   ├── permission-catalog.md                     # เพิ่ม Quick Estimate/Template permissions
│   └── error-contract.md                         # เพิ่ม Stable Error Codes ของ Flow
├── 04-data/
│   ├── conceptual-data-model.md                  # เพิ่ม Record, Version, Template และ link
│   └── audit-and-revision.md                     # แยก Quick Estimate Version จาก Estimate Revision
├── portal/
│   ├── README.md                                 # เพิ่มตัวอย่างและคำอธิบาย Flow
│   └── data/
│       ├── portal.json                           # ลงทะเบียน Flow ใหม่
│       └── quick-estimate-flow.json              # Diagram content ใหม่
└── superpowers/specs/
    └── 2026-09-06-quick-estimate-workflow-design.md
```

### Task 1: Publish the authoritative Quick Estimate business flow

**Files:**
- Create: `docs/01-business/quick-estimate-flow.md`
- Modify: `docs/README.md`
- Modify: `docs/01-business/end-to-end-business-flow.md`
- Modify: `docs/01-business/estimation-flow.md`
- Modify: `docs/01-business/roles-and-responsibilities.md`
- Modify: `docs/01-business/approval-matrix.md`

**Interfaces:**
- Consumes: `CONTEXT.md` and `docs/superpowers/specs/2026-09-06-quick-estimate-workflow-design.md`
- Produces: One authoritative business guide that later requirements, data model and Portal nodes link to

- [x] **Step 1: Prove the authoritative guide is not published yet**

Run:

```bash
test -f docs/01-business/quick-estimate-flow.md
```

Expected: exit code `1`, proving the planned guide is absent before this task.

- [x] **Step 2: Create the Quick Estimate source-of-truth guide**

Create `docs/01-business/quick-estimate-flow.md` with these exact sections and decisions:

```markdown
# Quick Estimate Flow (ราคาประเมินเบื้องต้นหน้างาน)

## Purpose
ช่วยเจ้าหน้าที่สร้าง Price Range อย่างรวดเร็วจาก Pricing Template และข้อมูลขั้นต่ำ โดยไม่ทำให้ราคานี้กลายเป็น Quotation หรือ Approved Price

## Flow
Customer/Opportunity → Work Type → Pricing Template → Measurement → Price Range → Evidence/Assumptions → Share Policy → Preliminary Summary → Follow-up/Convert

## Lifecycle
Draft → Calculated → Shareable/PendingReview → Shared → Converted/Closed/Expired

## Mobile UX
เลือกงาน → กรอกข้อมูลเร็ว → สรุปและดำเนินการ

## Rules
แยก Record, Version หลังแชร์, Price Range, Validity, Customer-safe data และ Snapshot conversion

## Configurable Policy
Template version, rate, factor, range, required fields, checklist, threshold, SLA, notification และ summary template

## Reliability and Recovery
Online-first, Autosave, Retry, Safe Local Draft, Idempotency, upload retry และ version conflict

## Errors
Stable Error Codes พร้อม HTTP status และการตอบสนองของผู้ใช้

## Audit and Metrics
เหตุการณ์ที่ต้อง Audit และตัวชี้วัดเวลาใช้งาน/ความแม่นยำโดยไม่เก็บ PII เกินจำเป็น

## Acceptance Criteria
Mobile completion, customer-safe summary, policy enforcement, versioning, conversion, bilingual accessibility และ traceability

## Out of Scope
On-site approved Quotation, free-form workflow builder, offline-first multi-device sync, image-only AI pricing และ customer editing

## Validation Questions
คำถามเรื่อง Work Type, สูตร, Range, อายุราคา, Share Threshold และผู้ตรวจที่ต้องยืนยันกับลูกค้า
```

Expand each section using the accepted spec; mark unresolved business values as `Validation Questions` and never invent company rates, thresholds or validity periods.

- [x] **Step 3: Connect the master business flow**

Update `docs/01-business/end-to-end-business-flow.md` so the pre-sales path reads:

```text
Customer → Opportunity → Site Survey
    ├─ Quick Estimate → Preliminary Summary → Follow-up
    │                                      └─ Convert
    └───────────────────────────────────────────► Official Estimate
                                                   ↓
                                      Approval → Quotation → Project
```

State that Quick Estimate is an optional fast path and Official Estimate remains required before Quotation.

- [x] **Step 4: Connect Official Estimate and operating roles**

Update `docs/01-business/estimation-flow.md` with `Quick Estimate Convert` as an accepted source of a new Official Estimate Draft. Add a `Field Estimator` row to `roles-and-responsibilities.md` and state that the role may create/calculate/share only within its permission and Share Policy. Add Quick Estimate triggers to `approval-matrix.md`: wide price range, high value, custom material, manual override, stale template and missing evidence.

- [x] **Step 5: Add the documentation entry point**

Add `Quick Estimate Flow` to the business-reader path and authoritative-document table in `docs/README.md`. Link to `01-business/quick-estimate-flow.md` instead of duplicating its rules.

- [x] **Step 6: Verify Task 1 and commit**

Run:

```bash
test -f docs/01-business/quick-estimate-flow.md
rg -n "Quick Estimate|ราคาประเมินเบื้องต้น|Official Estimate|Quotation|Validation Questions" docs/README.md docs/01-business
git diff --check
```

Expected: the guide exists; the master and detailed flows distinguish Quick Estimate, Official Estimate and Quotation; no whitespace errors.

Commit only the Task 1 files:

```bash
git add docs/README.md docs/01-business/quick-estimate-flow.md docs/01-business/end-to-end-business-flow.md docs/01-business/estimation-flow.md docs/01-business/roles-and-responsibilities.md docs/01-business/approval-matrix.md
git commit -m "docs: publish quick estimate business flow"
```

### Task 2: Trace Quick Estimate through requirements, permissions and data

**Files:**
- Modify: `docs/00-overview/requirements-catalog.md`
- Modify: `docs/00-overview/scope-and-non-goals.md`
- Modify: `docs/03-contracts/permission-catalog.md`
- Modify: `docs/03-contracts/error-contract.md`
- Modify: `docs/04-data/conceptual-data-model.md`
- Modify: `docs/04-data/audit-and-revision.md`

**Interfaces:**
- Consumes: `docs/01-business/quick-estimate-flow.md`
- Produces: Traceable requirement IDs, permission keys, aggregate ownership and version rules for future implementation planning

- [x] **Step 1: Add functional requirements**

Add these identifiers to `requirements-catalog.md` without changing existing IDs:

```text
FR-QEST-001  Create Quick Estimate from a versioned Pricing Template
FR-QEST-002  Calculate and present a Price Range from minimum inputs
FR-QEST-003  Enforce Share Policy before customer presentation
FR-QEST-004  Share a customer-safe Preliminary Summary
FR-QEST-005  Create a new Version after material changes to shared content
FR-QEST-006  Convert one Quick Estimate Version idempotently into an Official Estimate Draft
FR-QEST-007  Autosave and safely recover an unsynced field draft
NFR-QEST-001 Complete the minimum mobile flow with clear save state and 44px touch targets
```

Mark business-specific thresholds and formulas as Draft while keeping separation, versioning, security and conversion principles Accepted.

- [x] **Step 2: Update scope and non-goals**

Add Quick Estimate, Pricing Template, Preliminary Summary and Conversion to Phase Estimation. Add Offline-first multi-device sync, on-site approved Quotation and image-only AI pricing to Non-goals.

- [x] **Step 3: Publish the permission keys**

Add these keys to `permission-catalog.md`:

```text
quick-estimates.read
quick-estimates.create
quick-estimates.update
quick-estimates.review
quick-estimates.share
quick-estimates.convert
quick-estimates.close
pricing-templates.read
pricing-templates.manage
```

Document expected Organization/Branch/Opportunity/Own scopes and state that `quick-estimates.share` is still constrained by Share Policy and Maker–Checker.

- [x] **Step 4: Update conceptual ownership and history**

Add `Quick Estimate`, `Quick Estimate Version`, `Pricing Template Version` and `Preliminary Summary` to `conceptual-data-model.md`. Show the relationship:

```text
Pricing Template Version ─► Quick Estimate Version
Quick Estimate ─► Quick Estimate Version ─► Preliminary Summary
                              │
                              └─ snapshot convert ─► Official Estimate Draft
```

Update `audit-and-revision.md` to distinguish a Quick Estimate Version from an Official Estimate Revision and enumerate shared/versioned/converted audit events.

- [x] **Step 5: Define stable Quick Estimate errors**

Add this table to `error-contract.md`:

```text
QUICK_ESTIMATE_INCOMPLETE         422  Required input/assumption missing
QUICK_ESTIMATE_REVIEW_REQUIRED    422  Share Policy requires internal review
QUICK_ESTIMATE_TEMPLATE_EXPIRED   409  Template version is no longer shareable
QUICK_ESTIMATE_VERSION_CONFLICT   409  Another update changed the version
QUICK_ESTIMATE_ALREADY_CONVERTED  409  Same source version points to an existing Official Estimate
```

State that an idempotent retry with the same idempotency key returns the original conversion result instead of `QUICK_ESTIMATE_ALREADY_CONVERTED`.

- [x] **Step 6: Verify Task 2 and commit**

Run:

```bash
rg -n "FR-QEST-00[1-7]|NFR-QEST-001" docs/00-overview/requirements-catalog.md
rg -n "quick-estimates\.(read|create|update|review|share|convert|close)|pricing-templates\.(read|manage)" docs/03-contracts/permission-catalog.md
rg -n "QUICK_ESTIMATE_(INCOMPLETE|REVIEW_REQUIRED|TEMPLATE_EXPIRED|VERSION_CONFLICT|ALREADY_CONVERTED)" docs/03-contracts/error-contract.md
rg -n "Quick Estimate Version|Pricing Template Version|Preliminary Summary|Official Estimate Draft" docs/04-data
git diff --check
```

Expected: all eight requirement IDs, all nine permission keys and all four conceptual records appear; no whitespace errors.

Commit only the Task 2 files:

```bash
git add docs/00-overview/requirements-catalog.md docs/00-overview/scope-and-non-goals.md docs/03-contracts/permission-catalog.md docs/03-contracts/error-contract.md docs/04-data/conceptual-data-model.md docs/04-data/audit-and-revision.md
git commit -m "docs: trace quick estimate controls"
```

### Task 3: Add the JSON-driven Quick Estimate portal flow

**Files:**
- Create: `docs/portal/data/quick-estimate-flow.json`
- Modify: `docs/portal/data/portal.json`
- Modify: `docs/portal/README.md`

**Interfaces:**
- Consumes: `docs/portal/schema/flow.schema.json`, `docs/01-business/quick-estimate-flow.md` and the accepted Quick Estimate spec
- Produces: Flow catalog entry `quick-estimate` and a schema-compatible bilingual diagram rendered by the existing Portal

- [x] **Step 1: Prove the catalog and data file do not exist**

Run:

```bash
test -f docs/portal/data/quick-estimate-flow.json
rg -n '"id": "quick-estimate"' docs/portal/data/portal.json
```

Expected: both commands exit `1` before the diagram is added.

- [x] **Step 2: Create the bilingual diagram data**

Create `quick-estimate-flow.json` with `version: 1`, document link `../01-business/quick-estimate-flow.md`, illustration `assets/estimate.svg`, and these groups:

```text
capture       เก็บข้อมูล / Capture
calculate     คำนวณ / Calculate
governance    ตรวจและแชร์ / Govern & Share
conversion    ต่อยอด / Convert
```

Create ordered nodes with these stable IDs:

```text
customer-context
work-template
measurement
price-range
evidence
share-policy
internal-review
preliminary-summary
customer-interest
official-conversion
```

Every node must contain `title.th`, `title.en`, `summary.th`, `summary.en`, `phase: phase-2`, an allowed `status`, an existing icon key and a valid Markdown document link.

- [x] **Step 3: Encode success and branch edges**

Add the primary sequence from `customer-context` through `official-conversion`. Add branch edges:

```text
share-policy → internal-review       type approval, label ต้องตรวจภายใน / Review required
internal-review → preliminary-summary type approval, label อนุมัติให้แชร์ / Approved to share
customer-interest → measurement       type return, label ขอปรับข้อมูล / Revision requested
```

The visible primary order remains understandable without interpreting color; labels carry the decision meaning.

- [x] **Step 4: Register and document the flow**

Insert a `quick-estimate` entry in `portal.json` immediately before the existing `estimation` entry:

```json
{
  "id": "quick-estimate",
  "file": "quick-estimate-flow.json",
  "title": { "th": "ประเมินราคาด่วนหน้างาน", "en": "On-site Quick Estimate" },
  "shortTitle": { "th": "ราคาด่วน", "en": "Quick Estimate" },
  "icon": "estimate"
}
```

Update `docs/portal/README.md` so the listed Flow set and editing example include Quick Estimate.

- [x] **Step 5: Validate JSON structure and references**

Run:

```bash
node -e 'const fs=require("fs"),path=require("path");const catalog=JSON.parse(fs.readFileSync("docs/portal/data/portal.json"));const descriptor=catalog.flows.find(x=>x.id==="quick-estimate");if(!descriptor)throw Error("catalog entry missing");const file=path.join("docs/portal/data",descriptor.file);const flow=JSON.parse(fs.readFileSync(file));const ids=new Set(flow.nodes.map(x=>x.id));if(ids.size!==flow.nodes.length)throw Error("duplicate node id");for(const n of flow.nodes){if(!n.title.th||!n.title.en||!n.summary.th||!n.summary.en)throw Error("missing locale: "+n.id);if(!fs.existsSync(path.resolve("docs/portal",n.document)))throw Error("broken document: "+n.id)}for(const e of flow.edges){if(!ids.has(e.from)||!ids.has(e.to))throw Error("broken edge: "+e.from+" -> "+e.to)}console.log("Quick Estimate JSON OK")'
```

Expected: `Quick Estimate JSON OK`.

- [x] **Step 6: Verify Portal behavior**

Serve locally using the documented command:

```bash
python3 -m http.server 8765 --bind 127.0.0.1 --directory docs/portal
```

Open `http://127.0.0.1:8765/#quick-estimate` and verify:

- Thai is the default and English switching translates Flow content
- 10 nodes and all labeled connections render
- Search and Phase 2 filtering preserve understandable results
- A node opens its detail dialog and Markdown link
- Keyboard arrows change tabs and visible focus is present
- At 390×844 cards stack vertically without horizontal overflow
- Print Preview includes the flow and text alternative
- Browser console has zero errors and warnings

- [x] **Step 7: Commit Task 3**

```bash
git add docs/portal/data/quick-estimate-flow.json docs/portal/data/portal.json docs/portal/README.md
git commit -m "docs: visualize quick estimate workflow"
```

### Task 4: Run final documentation governance checks

**Files:**
- Verify: `CONTEXT.md`
- Verify: `docs/README.md`
- Verify: `docs/00-overview/`
- Verify: `docs/01-business/`
- Verify: `docs/03-contracts/`
- Verify: `docs/04-data/`
- Verify: `docs/portal/`

**Interfaces:**
- Consumes: Deliverables from Tasks 1–3
- Produces: Evidence that terminology, links, JSON and repository scope are consistent

- [x] **Step 1: Check terminology and forbidden ambiguity**

Run:

```bash
rg -n "Quick Estimate|Official Estimate|Quotation" CONTEXT.md docs/00-overview docs/01-business docs/03-contracts docs/04-data
rg -n "Quick Estimate.*Quotation|Quotation.*Quick Estimate" docs/01-business docs/00-overview
```

Review every match and confirm no sentence treats Quick Estimate as Quotation or Approved Price.

- [x] **Step 2: Check every Markdown link and JSON document reference**

Run the repository link checker pattern already used by the Documentation Portal verification: recursively inspect relative Markdown links, JSON `document` fields and JSON `illustration.src` values, resolving Portal references from `docs/portal/`.

Expected: zero broken links or assets.

- [x] **Step 3: Check placeholders and formatting**

Run:

```bash
! rg -n 'T[B]D|T[O]DO|PLACEH[O]LDER' CONTEXT.md docs --glob '*.md'
! rg -n '[[:blank:]]+$' CONTEXT.md docs
git diff --check
```

Expected: all commands exit `0` and print no findings.

- [x] **Step 4: Confirm only intended files are staged or committed**

Run:

```bash
git status --short
git log --oneline -5
```

Expected: Quick Estimate work is represented by the Task commits. Pre-existing untracked `design.md`, `preview.html`, `.agents/` and `.codex/` remain unstaged and untouched.

- [x] **Step 5: Record verification outcome**

Update the execution checklist in this plan from `[ ]` to `[x]` only for completed steps, then commit the plan/checklist change separately:

```bash
git add docs/superpowers/plans/2026-09-06-quick-estimate-documentation-plan.md docs/superpowers/specs/2026-09-06-quick-estimate-workflow-design.md
git commit -m "docs: plan quick estimate documentation"
```
