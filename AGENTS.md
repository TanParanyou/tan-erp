# tan-erp Agent Guide

## Start here

Read `CONTEXT.md` for canonical business terms and `docs/README.md` for the documentation map before changing this repository.

## Current phase

This repository is in **Application Implementation**. The authorized implementation boundary is the Foundation Login and Current User vertical slice documented in `docs/superpowers/plans/2026-09-06-foundation-login-current-user.md`. Do not add CRM, Survey, Estimation, Item, Commercial, Project, Procurement, Inventory, Production, or MRP behavior without a separately approved implementation task.

## Product boundaries

- Call the product **Project ERP** or **tan-erp**. MRP is a future module inside the ERP.
- Thai is the default language. Pair important English technical terms with a short Thai explanation.
- Keep one authoritative document per concern. Link to it instead of duplicating rules.
- Record durable trade-off decisions in `docs/adr/` and business vocabulary only in `CONTEXT.md`.

---

## 1. Language & Communication (ภาษาและการสื่อสาร)

- **ตอบกลับผู้ใช้เป็นภาษาไทยเสมอ** ทุกครั้งโดยไม่มีข้อยกเว้น
- Comment ในโค้ดใช้ **ภาษาอังกฤษ** (ยกเว้น Business Logic เฉพาะทางที่ซับซ้อน สามารถใช้ภาษาไทยกำกับได้)
- ชื่อตัวแปร, Function, Method, Class, DTO, Type และ Interface ใช้ **ภาษาอังกฤษ** เสมอตามแบบแผนสากล

---

## 2. Reuse First Policy (ใช้ของเดิมก่อนเสมอ)

- **ค้นหาก่อนเขียนใหม่:** ก่อนสร้าง Component, Hook, Helper, Utility function, DTO หรือ Service ใหม่ ให้ตรวจสอบในโปรเจกต์ก่อนเสมอว่ามีอยู่แล้วหรือไม่
- **ห้าม Duplicate Logic:** หากมีโค้ดหรือคอมโพเนนต์ที่ทำงานใกล้เคียงกัน ให้พิจารณา Reuse หรือ Extend จากของเดิม
- **ต้องเสนอขอสร้างใหม่:** หากจำเป็นต้องสร้าง Component / Hook / Utility ใหม่ที่ยังไม่มีในระบบ ให้แจ้งเหตุผลและโครงสร้างให้ผู้ใช้ทราบก่อนลงมือสร้าง

---

## 3. Scope Guardrails & Minimal Blast Radius

- **Minimal Blast Radius:** แตะเฉพาะไฟล์ที่เกี่ยวกับ Task โดยตรง ห้าม refactor, จัดระเบียบ หรือแก้ format ไฟล์ที่ไม่เกี่ยวข้อง
- **Commit message format:** `type(scope): description`
  - type: `feat`, `fix`, `refactor`, `docs`, `style`, `chore`, `test`
  - ตัวอย่าง: `feat(auth): add current user session refresh`
- **Do Not Commit:**
  - ห้าม commit `.env`, `.env.local`, tokens, credentials, connection strings หรือ secrets
  - ห้าม commit โฟลเดอร์ build/cache: `bin/`, `obj/`, `.next/`, `node_modules/`, `dist/`

---

## 4. Documentation workflow

1. Identify the reader and the decision or task the document supports.
2. Update the authoritative document listed in `docs/README.md`.
3. Update related JSON flow data when the business or architecture flow changes.
4. Keep `flow.schema.json` compatible or increment the JSON version when making a breaking change.
5. Verify internal links, JSON syntax, responsive portal behavior, keyboard use, Thai/English text, and print layout.
6. Summarize the decision and list every changed document.

---

## 5. Portal rules

- Keep the portal dependency-free: semantic HTML, CSS, JavaScript, JSON, and local SVG only.
- JSON is the source for diagrams; HTML must not hard-code business nodes or connections.
- Every visual diagram needs a readable text sequence and accessible labels.
- Preserve visible keyboard focus, 44px touch targets, adequate contrast, and reduced-motion support.
- Do not add external fonts, trackers, analytics, CDNs, or network dependencies.

---

## 6. ERP Form & UX Standards (Atelier Architectural Navy Sharp)

> ศึกษาคู่มือละเอียดเพิ่มเติมได้ที่ `.agents/skills/building-erp-forms/SKILL.md`

- **Visual Theme:** ยึดมั่นตาม **Atelier Architectural Navy Sharp**
  - `border-radius: 0px !important` (คม เหลี่ยม ปราศจากความโค้งมน)
  - Solid Navy `#0B3056` เป็นสีหลักของระบบ
  - Pure SVG Stroke Icons (ห้ามพึ่งพา CDN หรือ Icon Pack ภายนอก)
- **Single Dynamic Route `[id]`:** ใช้หน้าเดียวรองรับทั้ง Create (`id="create"` หรือ `"add"`) และ Edit (`id` จริง)
- **Deferred File Upload:** เมื่อเลือกไฟล์แนบ/รูปภาพ ให้อัปโหลดจริงในฟังก์ชัน `onSubmit` เท่านั้น ห้ามยิง API อัปโหลดทันทีที่เลือก เพื่อป้องกันไฟล์ขยะค้างในระบบ
- **Double Submit Protection:** ปุ่มบันทึก/ส่งข้อมูลต้องมีสถานะ `isLoading` และถูก Disable ทันทีขณะยิง Request
- **Safety Confirmation Modal:** การลบ (Delete), การยกเลิก (Void/Cancel) หรือการกระทำที่มีความเสี่ยงสูง ต้องมี Confirmation Modal ยืนยันเสมอ พร้อมทั้งปุ่มยืนยันใน Modal ต้องมี `isLoading` ล็อกปุ่มไว้ขณะยิง API
- **Minimal Mono Loading:** ขณะดึงข้อมูล Edit ให้ใช้ Minimal Loading Component ในคอนเทนเนอร์ (ห้ามใช้ Skeleton Loader)

---

## 7. Application & Clean Architecture Guardrails

> อ่านเอกสารเหล่านี้เมื่อได้รับอนุญาตให้ทำงานในส่วน Application:
> - Backend: `docs/02-architecture/backend-architecture.md`
> - Frontend: `docs/02-architecture/frontend-architecture.md`
> - API and errors: `docs/03-contracts/`
> - Data and Raw SQL: `docs/04-data/`
> - Testing and completion gates: `docs/05-engineering/`

### Backend (.NET 9 C# Clean Architecture)
- แบ่ง 4 โครงการ: `Domain`, `Application`, `Infrastructure`, `WebApi`
- ห้ามใส่ Business Logic ใน Controllers หรือ Endpoints (ต้องอยู่ใน Application Use Cases/Services หรือ Domain Entities)
- EF Core เป็นเจ้าของ Writes และ Transactions; Dapper / Parameterized Raw SQL ใช้เฉพาะ Complex Reads ใน Infrastructure
- PostgreSQL เป็นเจ้าของข้อมูลองค์กรและ RBAC; Firebase ให้บริการเฉพาะ Identity (Authentication)
- ทุก Error Response ต้องเป็นไปตาม RFC 7807 Problem Details

### Frontend (Next.js App Router + TypeScript)
- จัดโครงสร้างตาม Business Feature (`src/features/<feature>/`)
- TanStack Query เป็นเจ้าของ Server State; ห้าม fetch API ใน `useEffect` โดยตรง
- ห้ามเรียก Database ตรงจาก Frontend Code; ทุกอย่างต้องผ่าน API Client
- Frontend Permission Guard มีไว้เพื่อประสบการณ์ผู้ใช้ (UX) แต่ Backend คือผู้ถือกฎความปลอดภัยสูงสุด (Security Authority)

---

## 8. Definition of Done & Verification Before Completion

- การเปลี่ยนแปลงอยู่ในขอบเขตงานที่ได้รับมอบหมาย และไม่กระทบต่อโค้ดส่วนอื่น
- ผ่าน Verification Gates ก่อนส่งมอบงานเสมอ:
  ```bash
  # Backend
  dotnet build
  dotnet test

  # Frontend
  npm run build
  npm run lint
  ```
- **ห้ามกล่าวอ้างว่างาน "เสร็จแล้ว" หรือ "ผ่านแล้ว" หากยังไม่ได้รันคำสั่ง Verify และยืนยันผลลัพธ์จริง**
