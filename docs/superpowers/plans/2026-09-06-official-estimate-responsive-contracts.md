# Official Estimate Responsive Wireframe and Contracts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** สร้าง Responsive Wireframe, API Contract และ Data Contract ของ Official Estimate ที่รองรับ Desktop, Tablet และ Mobile

**Architecture:** Desktop ใช้ Split BOQ/Cost Inspector, Tablet ใช้ BOQ/Side Sheet และ Mobile ใช้ Task-focused Work Item Flow API เป็น Task-oriented และข้อมูลใช้ Relational Core กับ JSONB เฉพาะ Versioned Snapshot

**Tech Stack:** Markdown, local SVG, JSON examples, RFC 9457, PostgreSQL logical schema, Firebase Identity boundary

## Global Constraints

- Documentation Foundation เท่านั้น ไม่มี Application Code
- Official Estimate สร้างตรงจาก Customer/Opportunity/Site Survey ได้; Quick Estimate เป็น Optional Source
- Approved/Quoted Revision เป็น Immutable
- Backend เป็นเจ้าของ Calculation, RBAC, Scope และ State Transition
- ไทยเป็นหลักและอธิบาย English Term สั้น ๆ
- Touch Target ขั้นต่ำ 44px และ Wireframe มี Text Alternative

---

### Task 1: Responsive Wireframe

**Files:**
- Create: `docs/01-business/official-estimate-responsive-wireframe.md`
- Create: `docs/portal/assets/official-estimate-responsive-wireframe.svg`
- Modify: `docs/01-business/estimation-flow.md`

**Interfaces:**
- Consumes: Official Estimate flow/calculation/approval documents
- Produces: Screen, action and state names for API Contract

- [ ] **Step 1:** กำหนด Desktop Split Workspace, Tablet Side Sheet และ Mobile Task Flow
- [ ] **Step 2:** กำหนด loading, empty, validation, conflict, recalculation, approval และ immutable states
- [ ] **Step 3:** สร้าง SVG เปรียบเทียบสามขนาดพร้อม text alternative
- [ ] **Step 4:** ตรวจ SVG/XML และ Markdown links
- [ ] **Step 5:** Commit `docs: define official estimate responsive wireframe`

### Task 2: API Contract

**Files:**
- Create: `docs/03-contracts/official-estimate-api-contract.md`
- Modify: `docs/03-contracts/api-conventions.md`
- Modify: `docs/03-contracts/error-contract.md`

**Interfaces:**
- Consumes: Wireframe actions, Permission Catalog และ Approval Matrix
- Produces: Endpoint, request/response, concurrency and idempotency contracts

- [ ] **Step 1:** กำหนด create/read/draft/calculate/submit/review/revision/quotation endpoints
- [ ] **Step 2:** เพิ่ม JSON examples สำหรับ Draft, BOQ, Calculation, Approval และ Revision
- [ ] **Step 3:** กำหนด ETag, idempotency, RBAC/scope, localized errors และ retry
- [ ] **Step 4:** เพิ่ม Contract Test Cases
- [ ] **Step 5:** Commit `docs: define official estimate api contract`

### Task 3: Data Contract and Documentation Map

**Files:**
- Create: `docs/04-data/official-estimate-data-contract.md`
- Modify: `docs/04-data/conceptual-data-model.md`
- Modify: `docs/04-data/database-standards.md`
- Modify: `docs/04-data/audit-and-revision.md`
- Modify: `docs/README.md`
- Modify: `docs/portal/README.md`

**Interfaces:**
- Consumes: API fields/state transitions
- Produces: Aggregate, entity, constraint, index and JSONB snapshot boundaries

- [ ] **Step 1:** กำหนด Estimate/Revision/Section/Work Item/Cost Component/Approval/Quotation entities
- [ ] **Step 2:** กำหนด typed field, money precision direction, keys, constraints and indexes
- [ ] **Step 3:** กำหนด immutable calculation/quotation JSONB envelope
- [ ] **Step 4:** เพิ่ม Data Integrity Test Cases
- [ ] **Step 5:** เชื่อม Documentation Map และ authoritative documents
- [ ] **Step 6:** ตรวจ whitespace, links, XML และ JSON syntax
- [ ] **Step 7:** Commit `docs: define official estimate data contract`
