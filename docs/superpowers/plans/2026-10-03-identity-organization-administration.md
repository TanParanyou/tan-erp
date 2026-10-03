# CP-02 Identity/Organization Administration — Scope and Implementation Plan

**สถานะ:** Draft — ขอบเขตรอบแรกและกฎด้านล่างผ่านการตัดสินใจของเจ้าของงานเมื่อ 2026-10-03; ยังไม่เริ่ม implementation. ชื่อ Role จริงยังเป็น Draft ใน [Roles and Responsibilities](../../01-business/roles-and-responsibilities.md)
**แหล่งอ้างอิงหลัก:** [ERP Completion Plan CP-02](2026-09-29-erp-completion-master-plan.md), [RBAC](../../03-contracts/rbac.md), [Permission Catalog](../../03-contracts/permission-catalog.md), [Authentication](../../03-contracts/authentication.md), [Approval Matrix](../../01-business/approval-matrix.md)

## เป้าหมาย

ผู้ดูแลที่ได้รับอนุญาตเพิ่มผู้ใช้ จัดการ Membership และมอบ/ถอน Role ได้ผ่าน UI/API โดยมี Audit; การถอนสิทธิ์มีผลกับ request ถัดไป และผู้ไม่มีสิทธิ์ยกระดับสิทธิ์ตนเองหรือผู้อื่นเกินที่ตนมีไม่ได้.

## ขอบเขตรอบแรก (ตัดสินใจแล้ว)

**อยู่ในขอบเขต:** รายการ/ฟอร์มผู้ใช้, เพิ่มผู้ใช้ด้วยอีเมล, สร้าง/เปิด-ปิดใช้ Membership พร้อมสาขาและช่วงเวลา, มอบและถอน Role ให้ Membership, Audit, ทดสอบ cross-organization / privilege escalation / การถอนสิทธิ์.

**นอกขอบเขต:** สร้าง/แก้ Role และ Permission set, Organization/Branch CRUD, การสร้างบัญชี Firebase หรือส่งรหัสผ่านโดยระบบ, SSO/Social login, Project-scope และ Own-scope assignment.

## กฎที่ตัดสินใจแล้ว

1. **เพิ่มผู้ใช้:** ผู้ดูแลกรอกอีเมล ชื่อ สาขา และ Role. ระบบสร้าง User ใน PostgreSQL สถานะ *pending* (ยังไม่มี Firebase UID). ผู้ใช้ล็อกอินด้วย Firebase ด้วยอีเมลเดียวกัน ระบบผูก UID ครั้งแรกเมื่อ token มี `email_verified = true` และอีเมลตรงเท่านั้น (Firebase เป็น Identity เท่านั้น; PostgreSQL เป็นเจ้าของสิทธิ์). ไม่เรียก Firebase Admin API.
2. **ผู้ดูแลคนสุดท้าย:** Backend ปฏิเสธการถอน Role/ปิด Membership/ปิดผู้ใช้ที่ทำให้ Organization ไม่เหลือผู้มีสิทธิ์จัดการผู้ใช้ที่ active อย่างน้อยหนึ่งคน (ตรวจใน transaction เดียวกับการเปลี่ยน). การกู้สิทธิ์ทำผ่าน runbook โดยผู้มีอำนาจแก้ DB พร้อมบันทึกเหตุผลและ Audit.
3. **ป้องกัน privilege escalation:** ผู้ดูแลมอบได้เฉพาะ Role ที่ permission ทั้งชุดอยู่ใน permission ที่ตนถืออยู่ (ที่ Scope เดียวกันหรือกว้างกว่า) และมอบ/ถอนสิทธิ์ของตนเองไม่ได้. Role ที่มี Approval Permission (เช่น `estimates.approve`, `cost-records.approve`) ต้องผ่านผู้ตรวจที่เป็นคนละคนกับผู้เสนอก่อนมีผล (maker–checker). Production Bootstrap ต้องมี Independent Checker อย่างน้อยหนึ่งคนตาม [Roles](../../01-business/roles-and-responsibilities.md).

## คำถามที่ยังเปิด (ต้องตอบก่อนออก contract)

- Permission ใหม่สำหรับงานนี้: เสนอ `users.read`, `users.manage`, `memberships.manage`, `roles.assign`, `roles.assign-approval` — ยืนยันชื่อและ Scope
- Maker–checker ของ Approval Role: เก็บเป็น *pending assignment* ที่ผู้ตรวจยืนยัน หรือให้ผู้ตรวจสร้าง assignment เอง? (เสนอแบบ pending)
- Membership ที่หมดอายุ (`ExpiresAtUtc`) ต้องแจ้งเตือนหรือไม่
- อีเมลซ้ำข้าม Organization: User เป็นของ Organization เดียวหรือหนึ่ง User หลาย Membership ข้ามองค์กร (โมเดลปัจจุบัน: User ไม่มี Organization, Membership มี) 
- ข้อมูลส่วนบุคคล (อีเมล/ชื่อ) ที่แสดงในรายการและ Audit — นโยบายการมองเห็นและ retention

## Pre-flight (ก่อนแก้โค้ด)

1. อ่าน `AGENTS.md`, `design.md`, `.agents/skills/building-erp-apis/SKILL.md`, `.agents/skills/building-erp-forms/SKILL.md`, `.agents/skills/building-erp-lists/SKILL.md`
2. อ่านโค้ดจริง: `UsersController`, `ListUsersHandler`, `IUserReadStore`, `RequestAccessResolver`, `CurrentUserReader`, `Membership`, `MembershipRole`, `User`, `AuditEvent` และ mapping ของ login (`firebase_uid` ปัจจุบันเป็น required)
3. ตรวจว่า token claim `email_verified` เข้าถึงได้ใน pipeline ของ Authentication หรือไม่ และ Firebase Emulator ให้ค่านี้อย่างไร

## Tasks

**Task 1 — Contract และ data model:** เขียน API contract (`docs/03-contracts/identity-administration-api-contract.md`) และปรับ data/ADR: `users.firebase_uid` เป็น nullable + unique เมื่อไม่ null, สถานะ pending, อีเมล normalized unique ตามขอบเขตที่ตกลง; migration พร้อม rollback และผลต่อข้อมูลเดิม (ไม่มีแถวที่ UID เป็น null มาก่อน). เพิ่ม Permission ใหม่ใน catalog/RBAC และ seed สำหรับ Test เท่านั้น.

**Task 2 — Login linking:** ผูก UID ครั้งแรกด้วย `email_verified` + อีเมลตรงเท่านั้น; ปฏิเสธเมื่อไม่ verified หรืออีเมลซ้ำกับผู้ใช้ที่ผูกแล้ว; ทดสอบ account-takeover (อีเมลคล้าย/ตัวพิมพ์ต่าง, token ไม่ verified).

**Task 3 — Backend use cases:** Create/Update User (pending), Create/Deactivate Membership, Assign/Revoke Role, List/Detail พร้อม Search/Pagination ตามมาตรฐาน; handler ใน Application ตามรูปแบบเดิม, EF Core เป็นเจ้าของ write และ transaction; ETag/RowVersion; Idempotency-Key สำหรับ create; RFC 7807 + error code ใหม่ (th/en); ตรวจ last-administrator guard, anti-escalation, maker–checker ใน transaction เดียว; Audit ทุกการเปลี่ยนแปลงโดยไม่เก็บข้อมูลอ่อนไหวเกินจำเป็น.

**Task 4 — Revocation semantics:** ยืนยันว่า RequestAccessResolver ตรวจสิทธิ์สดจาก PostgreSQL ทุก request (ไม่ cache นานกว่าที่ contract กำหนด) และทดสอบ: ถอน Role/ปิด Membership แล้ว request ถัดไป 403/401; ปิดผู้ใช้แล้วใช้ token เดิมไม่ได้.

**Task 5 — Frontend:** หน้ารายการ/ฟอร์มผู้ใช้ตาม `building-erp-lists`/`building-erp-forms` (single dynamic route `[id]`, FormTabs, confirmation modal พร้อม `isLoading` สำหรับถอนสิทธิ์/ปิดใช้, คำอธิบายผลกระทบ), TanStack Query, permission guard แบบ UX, i18n th/en ครบ, Tailwind-first, a11y (keyboard, 320px).

**Task 6 — Tests และเอกสาร:** integration (PostgreSQL): cross-organization 404, escalation ถูกปฏิเสธ, last-admin guard, maker–checker, revocation, linking; unit ของ handler; Vitest ของ UI; Playwright journey (เพิ่มผู้ใช้ → มอบ Role → ผู้ใช้ล็อกอินได้ → ถอน → ใช้ไม่ได้). อัปเดต contract, verification doc, runbook กู้สิทธิ์ผู้ดูแล, master plan CP-02, roadmap.

## Gates

`dotnet build backend/TanErp.slnx` + `dotnet test`; `npm run check:api`, `lint`, `typecheck`, `test`, `build`; Playwright journey. ไม่กล่าวอ้างว่า "เสร็จ" โดยไม่มีผลรันจริง. Security review ของ Task 2 และ 3 ก่อน merge.

## ความเสี่ยงที่ต้องระวัง

- การผูกบัญชีด้วยอีเมลเป็นจุดเสี่ยง account takeover: ต้องใช้ `email_verified` และห้ามผูกทับ UID เดิม
- การแก้ `firebase_uid` เป็น nullable กระทบ index/ข้อสมมติเดิมของ CurrentUserReader และ seeder
- Maker–checker ของ Approval Role เพิ่มความซับซ้อนของ state (pending assignment) — ควรตกลงให้ชัดก่อน Task 1
