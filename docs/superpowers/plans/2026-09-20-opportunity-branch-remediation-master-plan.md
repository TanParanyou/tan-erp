# Opportunity Branch Remediation Master Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ทำให้ branch `feat/opportunity-qualification` ปลอดภัยและพร้อม merge โดยปิดช่องโหว่ไฟล์, ทำ Official Estimate จาก trusted evidence และ harden Commercial Slice 5B/Document Numbering ที่ผู้ใช้ยืนยันให้คงไว้.

**Architecture:** แยก remediation เป็นสอง vertical slices ที่ส่งมอบและ rollback ได้อิสระ. Slice R1 ทำ File Service เป็น parent-bound capability และแก้ Work Images/Customer/Site ให้ใช้ authorization เดียวกัน; Slice R2 ทำ Estimate creation เป็น server-derived trusted handoff พร้อมเพิ่ม idempotency, atomic numbering, permission และ UI contract ให้ Commercial Slice 5B. Master นี้เป็น sequencing/index เท่านั้น; exact contracts, file maps และ tests อยู่ในแผนลูก.

**Tech Stack:** .NET 10 SDK ตาม `backend/global.json`, ASP.NET Core, EF Core/PostgreSQL, Next.js 16, React 19, TypeScript, TanStack Query, Vitest, Playwright, OpenAPI.

## Global Constraints

- ผู้ใช้ยืนยันให้คง Commercial Slice 5B และ Document Numbering เมื่อ 2026-09-20; ต้องเพิ่ม authoritative plan/contract และแก้ `AGENTS.md` boundary ก่อนแก้ implementation.
- ไม่ใช้ `any`, `as any`, `@ts-ignore`, string-message error branching หรือ frontend relational `array.find`.
- Backend เป็น security authority; ทุก file read/bind ตรวจ organization, parent resource, permission และ status.
- Mutations ใช้ deterministic idempotency; retries ต้องไม่สร้าง file, attachment, audit หรือ business record ซ้ำ.
- UI copy ใช้ i18n ครบ `th`/`en`; upload จริงเริ่มใน `onSubmit`; ปุ่ม submit ถูก lock ขณะ pending.
- Reuse proposal: สร้าง `IFileParentAccessResolver` และ parent-bound upload session เป็นของกลางสำหรับ Opportunity, Customer และ Site แทน permission/file validation ซ้ำในแต่ละโมดูล.
- แตะเฉพาะไฟล์ในแผนลูก; ไม่มี refactor ทั่ว repository.

---

## Delivery Order

| Gate | Plan | Deliverable | Targeted tests |
| --- | --- | --- | ---: |
| R0 | แผนนี้ | Freeze boundary, baseline SHA, SDK และ rollback strategy | 0 |
| R1 | [File Security & Work Images](2026-09-20-file-security-work-images-remediation.md) | parent-bound upload/read/bind, retry-safe multi-image, cursor gallery | 6 |
| R2 | [Official Estimate & Commercial Hardening](2026-09-20-official-estimate-commercial-hardening.md) | trusted Ready Survey handoff, server snapshot export, idempotent quotation/acceptance, atomic numbering | 8 |
| R3 | แผนนี้ | Full gates, evidence update, merge/no-merge decision | existing suites |

ห้ามเริ่ม R2 ก่อน R1 migration/contract ผ่าน targeted integration tests เพราะ Estimate และ Survey evidence อาจใช้ File Service เดียวกันใน slices ถัดไป. ห้ามอ้าง Production readiness จาก targeted tests; full gates อยู่ R3.

## Task 0: Freeze baseline and scope

**Files:**
- Modify: `AGENTS.md`
- Modify: `docs/README.md`
- Modify: `docs/05-engineering/opportunity-work-images-verification.md`
- Modify: `docs/05-engineering/official-estimate-verification.md`
- Reference: `docs/superpowers/plans/2026-09-13-opportunity-work-images-vertical-slice.md`

**Interfaces:**
- Consumes: current branch SHA and review findings dated 2026-09-20.
- Produces: one recorded remediation baseline, explicit Pilot-not-Production status and links to both child plans.

- [ ] Record `git rev-parse HEAD`, `git status --short`, installed SDKs and current verification results before edits; do not overwrite prior tested SHA.
- [ ] Mark Work Images and Official Estimate verification as `Remediation Required` until R1/R2 pass; preserve historical results as dated evidence.
- [ ] Update `AGENTS.md` only after adding approved Slice 5B plan/contract; record quotation issuance, acceptance, Proposed/Won and document-numbering settings as authorized boundary.
- [ ] Validate links with:

```bash
test -f docs/superpowers/plans/2026-09-20-file-security-work-images-remediation.md
test -f docs/superpowers/plans/2026-09-20-official-estimate-commercial-hardening.md
git diff --check
```

- [ ] Proposed commit: `docs(remediation): freeze opportunity branch repair scope`

## Task 1: Execute R1 independently

**Files:** all files enumerated by [R1](2026-09-20-file-security-work-images-remediation.md).

**Interfaces:**
- Consumes: parent type/id contract and reusable access resolver defined by R1.
- Produces: authorized file preview, verified parent ownership, safe retry and complete cursor gallery.

- [ ] Execute R1 Tasks 1–5 in order with each named red/green test.
- [ ] Stop if migration cannot be applied to a disposable PostgreSQL database; do not bypass session ownership with in-memory state.
- [ ] Review R1 diff independently before starting R2.

## Task 2: Execute R2 independently

**Files:** all files enumerated by [R2](2026-09-20-official-estimate-commercial-hardening.md).

**Interfaces:**
- Consumes: Ready Survey Revision persisted state and the current authorized boundary.
- Produces: server-derived Estimate linkage, idempotent quotation issuance/acceptance, atomic unique numbering and exact permission-aware UI.

- [ ] Execute R2 Tasks 1–4 in order with each named red/green test.
- [ ] Review migrations and OpenAPI diff separately; require every quotation/document-sequence endpoint to match the newly frozen Slice 5B contract.
- [ ] Review R2 diff independently before final verification.

## Task 3: Full verification and evidence

**Files:**
- Modify: `docs/05-engineering/opportunity-work-images-verification.md`
- Modify: `docs/05-engineering/official-estimate-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: completed R1/R2 commits.
- Produces: reproducible verification evidence and merge/no-merge decision.

- [ ] Install/use the exact SDK from `backend/global.json`; do not change `global.json` merely to make local verification pass.
- [ ] Run full gates once after both slices:

```bash
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
npm --prefix frontend run check:api
npm --prefix frontend run verify
npm --prefix frontend run test:e2e -- opportunity-work-images.spec.ts official-estimate.spec.ts
git diff --check
```

- [ ] Record command, exit code, test count, tested SHA and unresolved risks. Browser evidence must cover authorized image preview, multi-image retry, page 2 gallery and Estimate creation from a Ready Survey Revision.
- [ ] Proposed commit: `test(remediation): verify opportunity branch stabilization`

## Definition of Success

- Anonymous or wrong-scope callers cannot read/bind a file; authorized parent readers can preview it.
- Upload completion is impossible without a persisted, unexpired, same-organization/same-actor session whose slots match the multipart files.
- Opportunity multi-image retry uploads only failed items and gallery can traverse beyond 25 images without duplicates or omissions.
- Customer/Site/Opportunity accept only verified files bound to their exact parent intent.
- Official Estimate derives customer, branch, Ready Survey Revision and snapshot hash from persisted server state; request cannot forge them.
- Official Estimate export uses the latest server calculation snapshot.
- Quotation issue/accept replay safely, allocate unique atomic numbers, transition Estimating → Proposed → Won once, and expose actions only for exact permissions.
- Document-numbering list/preview/update uses validated formats, permission checks, optimistic concurrency and safe audit JSON.
- Full backend/frontend/contract/E2E gates pass at one recorded SHA and `git diff --check` is clean.

## Deferred Scope

- Public sharing, CDN/object-storage signed URLs, malware scanning, EXIF redaction pipeline and retention worker; R1 leaves extension points but does not invent policy.
- Maker–Checker approval, new Estimate revisions, customer-facing quotation PDF, quotation amendment/void and Project handoff after Won.
