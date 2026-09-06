# Quick Estimate Mobile Wireframe and Contracts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [x]`) syntax for tracking.

**Goal:** สร้างเอกสาร Mobile Wireframe, API Contract และ Data Contract ของ Quick Estimate ที่เชื่อมกันและพร้อมใช้เป็นรากฐานพัฒนา Production

**Architecture:** ใช้ Config-driven Mobile Wizard 5 ขั้น, Task-oriented API และ Hybrid Relational/JSONB Data โดย Backend เป็นเจ้าของการคำนวณ RBAC และ State Transition เอกสารหลักแต่ละเรื่องแยกไฟล์และเชื่อมกลับ Documentation Map

**Tech Stack:** Markdown, local SVG, JSON examples, RFC 9457 Problem Details, PostgreSQL concepts, Firebase Identity boundary

## Global Constraints

- อยู่ใน Documentation Foundation เท่านั้น ห้ามสร้าง Application Code
- ภาษาไทยเป็นหลัก และอธิบาย English Technical Term แบบสั้น
- Portal ไม่มี Dependency ภายนอก
- Touch Target ขั้นต่ำ 44px และ Wireframe ต้องมี Text Alternative
- PostgreSQL เก็บ Typed Relational Core; JSONB ใช้เฉพาะ Versioned Config/Snapshot
- Firebase ให้ Identity เท่านั้น; Backend/PostgreSQL เป็นเจ้าของ RBAC และ Scope
- ห้ามใส่ราคาจริงหรือ Threshold ที่ยังไม่ผ่าน Pilot

---

### Task 1: Mobile Wireframe

**Files:**
- Create: `docs/01-business/quick-estimate-mobile-wireframe.md`
- Create: `docs/portal/assets/quick-estimate-mobile-wireframe.svg`
- Modify: `docs/01-business/quick-estimate-flow.md`

**Interfaces:**
- Consumes: Field/Required At จาก `quick-estimate-template-catalog.md` และ Share Decision จาก `quick-estimate-pricing-rules.md`
- Produces: Screen/Action/State names ที่ API Contract ต้องรองรับ

- [x] **Step 1: สร้าง Wireframe 5 ขั้น**

ระบุ Screen: `job`, `measurements`, `options`, `evidence-price`, `review-share` พร้อม Sticky Save/Price/Primary Action

- [x] **Step 2: กำหนด State และ Interaction**

ครอบคลุม loading, empty, validation, autosaving, saved, offline, conflict, calculating, blocked, pending-review, shareable และ shared

- [x] **Step 3: สร้าง SVG Low-fidelity**

วาด Mobile Frame หลักอย่างน้อย 5 จอด้วย local SVG และใส่คำอธิบายลำดับอ่านใน Markdown

- [x] **Step 4: ตรวจเอกสาร**

Run: `git diff --check`
Expected: exit 0 และไม่มี whitespace error

- [x] **Step 5: Commit**

```bash
git add docs/01-business/quick-estimate-mobile-wireframe.md docs/portal/assets/quick-estimate-mobile-wireframe.svg docs/01-business/quick-estimate-flow.md
git commit -m "docs: define quick estimate mobile wireframe"
```

### Task 2: API Contract

**Files:**
- Create: `docs/03-contracts/quick-estimate-api-contract.md`
- Modify: `docs/03-contracts/api-conventions.md`
- Modify: `docs/03-contracts/error-contract.md`

**Interfaces:**
- Consumes: Screen actions from Task 1, Permission keys from `permission-catalog.md`, errors from `error-contract.md`
- Produces: Endpoint, request/response, status and idempotency contracts used by Data Contract

- [x] **Step 1: กำหนด Resource และ Command Endpoints**

กำหนด create/read/patch/calculate/submit-review/review/share/convert/effective-template/upload-session พร้อม HTTP status

- [x] **Step 2: เพิ่ม JSON Examples**

มีตัวอย่าง Create Draft, Patch with `If-Match`, Calculate Result, Review Decision, Share และ Conversion

- [x] **Step 3: กำหนด Cross-cutting Rules**

ระบุ Authentication, RBAC/Scope, localization, validation, concurrency, idempotency และ retry behavior ต่อ Endpoint

- [x] **Step 4: เพิ่ม Contract Test Matrix**

กำหนดอย่างน้อย Happy Path, invalid field, out-of-scope, maker-checker, stale token และ repeated idempotency key

- [x] **Step 5: Commit**

```bash
git add docs/03-contracts/quick-estimate-api-contract.md docs/03-contracts/api-conventions.md docs/03-contracts/error-contract.md
git commit -m "docs: define quick estimate api contract"
```

### Task 3: Data Contract and Cross-links

**Files:**
- Create: `docs/04-data/quick-estimate-data-contract.md`
- Modify: `docs/04-data/conceptual-data-model.md`
- Modify: `docs/04-data/database-standards.md`
- Modify: `docs/04-data/audit-and-revision.md`
- Modify: `docs/README.md`
- Modify: `docs/portal/README.md`

**Interfaces:**
- Consumes: Request/response fields and state transitions from Tasks 1–2
- Produces: Entity ownership, typed fields, JSONB schemas, keys, constraints and indexes for future persistence work

- [x] **Step 1: กำหนด Aggregate และ Entity**

กำหนด QuickEstimate, Version, MeasurementLine, Evidence, Review, ShareAttempt และ ConversionLink พร้อม key/scope/lifecycle ownership

- [x] **Step 2: กำหนด Relational Field Contract**

ระบุ type, nullability, constraint, immutable rule และ index ของ Identity, Scope, Money, Status, Effective Date และ Concurrency Token

- [x] **Step 3: กำหนด JSONB Contract**

ระบุ `schemaVersion`, envelope, validation, immutability และตัวอย่าง template/input/calculation/customer-summary snapshot

- [x] **Step 4: เพิ่ม Data Integrity Test Cases**

ครอบคลุม FK/scope, duplicate version, money precision, cross-organization reference, immutable shared version และ schema mismatch

- [x] **Step 5: เชื่อม Documentation Map**

เพิ่มลิงก์จาก Flow, API/Data Standards, Portal README และ `docs/README.md` โดยไม่ทำซ้ำกฎ

- [x] **Step 6: ตรวจ Acceptance ทั้งชุด**

Run: `git diff --check`
Expected: exit 0

Run: `node -e "JSON.parse(require('fs').readFileSync('docs/portal/data/quick-estimate-flow.json'))"`
Expected: exit 0

- [x] **Step 7: Commit**

```bash
git add docs/04-data/quick-estimate-data-contract.md docs/04-data/conceptual-data-model.md docs/04-data/database-standards.md docs/04-data/audit-and-revision.md docs/README.md docs/portal/README.md
git commit -m "docs: define quick estimate data contract"
```
