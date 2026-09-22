# Antigravity Prompt — Complete Item Master Estimate Catalog

คัดลอกข้อความระหว่างเส้นคั่นไปให้ Antigravity โดยเริ่มจาก repository root ของ `tan-erp`

---

คุณกำลังทำงานต่อจาก implementation ที่เริ่มไว้แล้วใน Project ERP (`tan-erp`) เป้าหมายคือปิด Item Master Estimate Catalog vertical slice ให้ครบตามแผน completion โดยรักษาการแก้ไขที่มีอยู่และทำงานต่อเนื่องผ่านทุก Gate จนถึง verification จริง

## 1. โหลดบริบทก่อนแก้ไฟล์

อ่านไฟล์ต่อไปนี้ทั้งหมดตามลำดับ:

1. `AGENTS.md`
2. `design.md`
3. `CONTEXT.md`
4. `docs/README.md`
5. `docs/superpowers/plans/2026-09-22-item-master-estimate-catalog-completion.md` — source of truth สำหรับงานคงเหลือ
6. `docs/superpowers/plans/2026-09-21-item-master-estimate-catalog-foundation.md` — ใช้อ่าน architecture เดิมเท่านั้น; ถ้าลำดับงานต่างกันให้ใช้ completion plan
7. เอกสาร authoritative:
   - `docs/superpowers/specs/2026-09-06-item-master-cost-foundation-design.md`
   - `docs/01-business/item-master-flow.md`
   - `docs/01-business/item-master-field-catalog.md`
   - `docs/01-business/item-master-governance.md`
   - `docs/03-contracts/item-master-api-contract.md`
   - `docs/04-data/item-master-data-contract.md`
   - `docs/05-engineering/item-master-uat-scenarios.md`
   - `docs/adr/0008-typed-items-versioned-cost-records.md`
   - `docs/adr/0012-localized-jsonb-for-item-text.md`
   - `docs/adr/0013-organization-items-with-branch-availability.md`
8. `.agents/skills/building-erp-apis/SKILL.md`; ก่อนแก้ React/TypeScript ให้อ่าน skills ที่ repository กำหนดสำหรับ modern web และ React

จากนั้นรัน:

```bash
git status --short
git diff --stat
git diff --check
dotnet --version
```

SDK ต้องเป็น `10.0.400` ตาม `backend/global.json` หากไม่ตรงให้รายงาน environment blocker แทนการแก้ target framework หรือ package version

Repository มีงาน Item Master ที่ยังไม่ commit อยู่แล้ว ให้ถือว่าเป็น implementation ที่ต้องตรวจและทำต่อ ห้าม reset, checkout ทับ, ลบ migration หรือย้อนการแก้ไขเดิมเพียงเพื่อให้เริ่มจาก working tree ว่าง ตรวจทุกไฟล์ก่อนแก้และรักษาการแก้ไขที่ไม่เกี่ยวข้องของผู้ใช้

## 2. วิธีทำงาน

ทำ Tasks 1–8 ใน completion plan ตามลำดับ และใช้ TDD ในทุก behavior ที่เพิ่ม:

1. เขียน test ที่เจาะ invariant/contract หนึ่งเรื่อง
2. รัน test ให้เห็น failure ที่คาดไว้
3. implement ขั้นต่ำผ่าน Domain/Application/Infrastructure/API ตาม Clean Architecture
4. รัน focused test ให้ผ่าน
5. ตรวจ diff, migration SQL, OpenAPI และ security boundary ที่เกี่ยวข้อง
6. ทำเครื่องหมาย checkbox เฉพาะ step ที่มีหลักฐานผ่านจริง
7. commit ตาม boundary/message ในแผนเมื่อแยกจากการแก้ไขของผู้ใช้ได้อย่างปลอดภัย; ห้าม push

ทำงานต่อเนื่องผ่าน Gate A–D โดยไม่ต้องรออนุมัติระหว่าง Gate เว้นแต่พบ blocker ที่ต้องใช้อำนาจ/ข้อมูลจากผู้ใช้, migration ที่เสี่ยงทำลายข้อมูล, contract conflict ที่เปลี่ยน business behavior หรือการแก้ไขเดิมของผู้ใช้ชนกับงานโดยหลีกเลี่ยงไม่ได้

## 3. ลำดับบังคับ

### Gate A — Schema Safety

ทำ Task 1 ให้จบก่อน:

- ตรวจ migration chain บนฐานข้อมูลว่างและ upgrade path
- ปิด same-organization composite FK ของ Item/File/Cost relations
- ทำ `FileParentTypes.Item`
- เก็บ SHA-256, scan status, verified timestamp และ optional image dimensions อย่างซื่อสัตย์ต่อสิ่งที่ตรวจจริง
- `content_verified` หมายถึง magic-number/content validation เท่านั้น; ใช้ `clean` เมื่อมี malware scanner จริง
- migration ต้อง additive ไม่มี table drop, public URL, `current_cost` หรือการ rewrite historical Estimate snapshot

ห้ามเริ่ม Gate B ขณะที่ schema test หรือ migration review ยังไม่ผ่าน

### Gate B — Governed Master Data

ทำ Tasks 2–4 ตามลำดับ:

- Item/Category/Brand/Unit/Alias/Branch Availability management API
- ETag/optimistic concurrency, stable Problem Details และ structured projection
- private Item Image attach/reorder/set-primary/detach ผ่าน File Upload Session เดิม
- Versioned Cost พร้อม append-only review, maker–checker, overlap protection และ Draft → Submitted → Approved/Returned → Published/Superseded/Disabled
- ทุก mutation เขียน Business State และ `AuditEvent` ใน transaction เดียวกัน
- ทุก lookup scope ด้วย Organization/Branch จาก membership; cross-organization resource ตอบ 404
- Published Cost เป็น immutable; resolver เลือก Branch ก่อน Organization และคืน `ITEM_COST_AMBIGUOUS` เมื่อ precedence เท่ากัน

ห้ามใส่ business logic ใน Controller และห้าม expose `DbSet` ใหม่ผ่าน Application abstraction เพื่อทำ shortcut

### Gate C — Authoritative Estimate Flow

ทำ Tasks 5–7 ตามลำดับ:

- Catalog search/filter/cursor ทำบน server และค้น code, Thai/English JSONB name และ Alias
- Invalid cursor คืน `ITEM_CATALOG_CURSOR_INVALID`; ห้ามคืน empty success
- Catalog projection ใช้จำนวน SQL query คงที่ ไม่ทำ N+1 cost/image query
- คืนเฉพาะ Active, `canCost=true`, branch-available Item ที่มี Published deterministic cost
- Estimate รับ `itemId`/`costRecordId` แต่ re-resolve ราคาบน server และไม่เชื่อ `unitCost` จาก browser
- บันทึก Item/Cost identity, version, localized name, scope, effective time, policy และ resolved time เป็น immutable snapshot
- ราคาที่เปลี่ยนก่อน save คืน `ITEM_COST_VERSION_CONFLICT`; UI invalidate query และให้ผู้ใช้เลือกใหม่โดยไม่เปลี่ยนราคาเงียบ ๆ
- `estimate-item-catalog-modal.tsx` ใช้ TanStack Query และ generated OpenAPI types
- private image ใช้ authorized file-content endpoint และ revocable object URL; ห้ามเก็บ token, storage path หรือ signed URL อายุยาวใน state/API
- ลบ production dependency ต่อ `ESTIMATE_CATALOG_ITEMS` และ `useCatalogFilter` เมื่อไม่มี import เหลือ
- UI copy ใหม่ต้องมีทั้ง `th.json` และ `en.json`; รักษา keyboard/focus/44px controls/zero-radius/Tailwind semantic tokens

TypeScript ต้อง strict: ใช้ DTO/interface หรือ `unknown` พร้อม narrow; ห้าม `any`, `as any`, `@ts-ignore`, arbitrary fallback และ frontend `array.find` เพื่อประกอบ relation

### Gate D — Production Evidence

ทำ Task 8:

- เพิ่ม backend end-to-end scenario ตั้งแต่ taxonomy/item/private image/versioned branch cost/catalog ถึง Estimate immutable snapshot
- เพิ่ม security tests สำหรับ tenant/branch/file-parent/permission/maker-checker/stale ETag/idempotency/cursor/tampered price/private content
- เพิ่ม Playwright flow ที่ desktop และ viewport 320px พร้อม keyboard, focus return และ 200% zoom
- สร้าง `docs/05-engineering/item-master-estimate-catalog-verification.md`
- อัปเดต UAT/doc map และ authoritative contract เฉพาะเมื่อ implementation ที่ตรวจแล้วแตกต่างจริง

## 4. Guardrails

- Scope จบที่ Item/Cost/Image Catalog → Estimate Cost Component → Calculation Snapshot
- Supplier CRUD, Procurement, Inventory, Warehouse, Production, BOM/MRP, Import และ Unit Conversion Chain อยู่นอก scope
- Item เป็น Organization-owned aggregate; หลายสาขาใช้ `all_branches|selected_branches` และ relation แยก ไม่ duplicate Item
- Branch Availability และ Branch Cost เป็นคนละ concept
- Category ใช้ self-reference และกัน cycle; ไม่มี Subcategory entity แยก
- Brand nullable ได้; Alias ใช้ค้นหาแต่ไม่แทน canonical Item name
- ใช้ `LocalizedText` กลางและ JSONB เฉพาะ `th`/`en`
- Reuse File Service เดิม; Deferred Upload เกิดตอน submit
- API/Frontend ห้ามคืนหรือเก็บ storage path, binary, token, secret หรือ persistent bearer URL
- รักษา minimal blast radius; ไม่ refactor/format ไฟล์ที่ไม่เกี่ยวข้อง
- ไม่ติดตั้ง dependency ใหม่ถ้าของเดิมรองรับงาน และไม่แก้ test/runtime/configuration เพื่อซ่อน failure
- Regenerate OpenAPI และ TypeScript types ทุกครั้งที่ public contract เปลี่ยน

## 5. Completion criteria ต่อ Task

Task หนึ่งเสร็จเมื่อครบทุกข้อ:

- focused test ที่ระบุในแผนผ่านด้วย output ล่าสุด
- `git diff --check` ผ่าน
- migration SQL ไม่มี destructive/unscoped change
- OpenAPI และ generated types ตรงกันเมื่อ contract เปลี่ยน
- ไม่มี violation จาก `any`, hardcoded UI copy, public file URL, client-authoritative price, N+1 Catalog query หรือ loose cross-organization FK
- checkbox ใน completion plan ตรงกับหลักฐานจริง
- commit boundary ไม่รวมไฟล์ unrelated ของผู้ใช้

## 6. Final verification

หลัง Task 8 ให้รันจาก dependency state ที่ถูกต้อง:

```bash
dotnet --version
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
cd frontend
npm ci
npm run verify
npm run test:e2e -- estimate-item-catalog.spec.ts
cd ..
git diff --check
git status --short
```

บันทึก command, timestamp, commit SHA, pass/fail และ test counts ลง verification document ห้ามกล่าวว่า production-ready หากมีคำสั่งใด fail หรือถูกข้าม

## 7. รูปแบบรายงานสุดท้าย

รายงานภาษาไทยโดยแยก:

1. ผลลัพธ์ที่ใช้งานได้จริง
2. Tasks/Gates ที่ผ่าน พร้อม checkbox
3. migrations, constraints และ indexes ที่เพิ่ม
4. API/OpenAPI/frontend contracts ที่เปลี่ยน
5. security invariants และ audit behavior ที่พิสูจน์แล้ว
6. test/build/lint/typecheck/E2E commands พร้อมจำนวน pass/fail
7. commit SHAs และรายการไฟล์สำคัญ
8. blocker หรือความเสี่ยงที่ยังเหลือ; ถ้าไม่มีให้ระบุว่าไม่มีพร้อมหลักฐาน

ทำต่อจน Definition of Done ใน completion plan ครบจริง หยุดเฉพาะเมื่อสำเร็จหรือพบ blocker ที่ต้องให้ผู้ใช้ตัดสินใจ ห้ามสรุปว่าเสร็จจากจำนวนไฟล์หรือการ build ผ่านเพียงบางส่วน

---

