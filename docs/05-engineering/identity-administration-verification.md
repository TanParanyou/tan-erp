# Identity Administration Verification (CP-02, รอบแรก)

**สถานะ:** Verified ด้วย automated tests และ walkthrough ในเครื่อง 2026-10-03; ยังไม่ผ่าน UAT โดยผู้ดูแลจริง และยังไม่ผ่านการทบทวน Security

อ้างอิง: [Plan](../superpowers/plans/2026-10-03-identity-organization-administration.md), [API Contract](../03-contracts/identity-administration-api-contract.md), [Recovery Runbook](../06-operations/administrator-recovery.md)

## ขอบเขตที่ทำ

เพิ่มผู้ใช้ด้วยอีเมล (pending) พร้อม Membership และ Role; ผูกตัวตน Firebase ตอนล็อกอินครั้งแรกด้วยอีเมลที่ยืนยันแล้ว; แก้ชื่อ/เปิด-ปิดผู้ใช้; แก้สาขา/ช่วงเวลา/เปิด-ปิด Membership; มอบ/ถอน Role พร้อมกัน privilege escalation และ maker–checker สำหรับ Role ที่มี Approval Permission; กัน last administrator; Audit ทุกการเปลี่ยนแปลง; หน้า UI ไทย/อังกฤษ (รายการ, ฟอร์มเพิ่ม, รายละเอียด, คิวคำขอ).

## หลักฐานอัตโนมัติ (branch `feat/identity-administration`)

- Backend `dotnet build backend/TanErp.slnx`: 0 warnings/errors. Full `dotnet test`: Architecture 3/3, Integration 312/312, Unit 288/288 (Integration เพิ่ม 20 เคสของ `IdentityAdministrationEndpointsTests`). หลังเพิ่ม `rowVersion` ของคำขอและ role seed ที่หลังสุด รันซ้ำเฉพาะ OpenApi/IdentityAdministration/CurrentUser/Users: 44/44
- เคสที่ครอบคลุม: สร้างผู้ใช้ pending + Audit ไม่มีข้อมูลส่วนบุคคล, อีเมลซ้ำ (ไม่สนตัวพิมพ์/ช่องว่าง), input ไม่ถูกต้อง, role เกินสิทธิ์ผู้มอบ (403), Role อนุมัติ → คำขอ pending, ผู้ขออนุมัติเองไม่ได้ / ผู้ตรวจอื่นอนุมัติได้ / ยกเลิกได้เฉพาะผู้ขอ, ถอน Role ของตนเองไม่ได้, last administrator (ปิด Membership, ปิดผู้ใช้, ผู้ดูแล pending ไม่นับ), If-Match ขาด/ล้าสมัย, ข้ามองค์กร 404, ผู้ไม่มีสิทธิ์ 403/401, ค้นหา/กรอง/แบ่งหน้า, `GET /me` ผูกตัวตน (ต้อง verified; ไม่ผูกซ้ำ; ไม่ผูก pending ที่ถูกปิด), ถอน Role / ปิด Membership / ปิดผู้ใช้ แล้ว request ถัดไปถูกปฏิเสธ
- Migration `AddIdentityAdministration` ทดสอบ up/down/up บน DB ที่มีข้อมูลและ backfill `normalized_email`/`row_version`
- Frontend: `npm run check:api`, `npm run lint`, `tsc --noEmit`, `npm run build` ผ่าน; Vitest ทั้งชุด 630/630 (`--testTimeout=60000`)
- Playwright `e2e/user-administration.spec.ts` 1/1 กับ stack จริง (PostgreSQL, Firebase Emulator, backend, frontend): เชิญผู้ใช้ → ผู้ใช้ล็อกอินด้วยบัญชี emulator ที่ยืนยันอีเมล → สถานะเปลี่ยนเป็นใช้งานอยู่ → ผู้ดูแลถอน Role ผ่าน dialog → ผู้ใช้เสียเมนูทันทีหลังโหลดใหม่

## การปรับตาม `AGENTS.md`, `design.md` และสกิล API (หลังตรวจย้อนกลับ)

หลังตรวจงานเทียบกฎของ repo พบและแก้: `Idempotency-Key` บังคับสำหรับ `POST` ที่สร้างข้อมูล (สร้างผู้ใช้, มอบ Role) พร้อม replay/ปฏิเสธ payload ต่าง; พารามิเตอร์รายการเป็น `limit`, `sortBy`, `sortOrder` (whitelist, id tie-breaker); ย้ายกฎล้วน (anti-escalation, last-administrator) จาก Infrastructure ไปไว้ใน `AdministrationPolicy` ของ Application พร้อม unit test 14 เคส (Store ยังเป็นผู้จัด transaction/การโหลดข้อมูล เหมือน `EstimateStore`); query key มี locale; ชนิดพารามิเตอร์ FE ดึงจาก generated paths; ฟอร์มใช้ `noValidate` เหมือน editor อื่น (ไม่เช่นนั้นข้อความตรวจสอบเป็นของเบราว์เซอร์ ไม่ใช่ i18n); FE ส่ง Idempotency-Key ต่อ intent และใช้ key เดิมเมื่อ retry.

ข้อที่ยังเป็น deviation ที่ยอมรับชั่วคราว: use case รวมอยู่ใน `IdentityAdministrationService` เดียว (ไม่แยก Command/Handler ต่อ use case); ตรรกะกำกับ transaction และการโหลดข้อมูลอยู่ใน Store.

## ที่พบระหว่างทดสอบและแก้แล้ว

- LINQ ของคิวคำขอกรองหลัง projection แปลเป็น SQL ไม่ได้ (HTTP 500) → ย้ายตัวกรองก่อน projection
- Role ที่ผู้อนุมัติถือสิทธิ์ไม่ครบถูกปฏิเสธด้วย `ROLE_ESCALATION_DENIED` ตามที่ออกแบบ (แก้ที่ fixture ของเทสต์)
- `common.table.actions` ไม่มีในไฟล์ข้อความ (ถูกจับโดยเทสต์) → ใช้ `common.actions.manage`

## ข้อจำกัด / ยังเปิด

- ชื่อ Permission และขอบเขตเป็นค่าเริ่มต้นที่เสนอ รอเจ้าของงานยืนยัน; ชื่อ Role จริงยังไม่กำหนด
- Production ยังไม่มี seed Role/Permission: ต้องกำหนด Role จริง ให้ Permission ใหม่ทั้ง 5 รายการ และมีผู้ดูแลคนแรก (bootstrap) ผ่านขั้นตอนของเจ้าของระบบ — ไม่มีหน้าสร้าง/แก้ Role และไม่มี Organization/Branch CRUD
- ไม่รองรับเชิญอีเมลที่มีอยู่แล้วในอีกองค์กร (คืน `USER_EMAIL_ALREADY_EXISTS`); User เป็นระดับระบบ จึงปิด/เปิดผู้ใช้ที่มี Membership ข้ามองค์กรจากหน้านี้ไม่ได้
- ไม่ส่งอีเมลเชิญและไม่สร้างบัญชี Firebase ให้: ผู้ใช้ต้องมีบัญชีที่ยืนยันอีเมลแล้วด้วยวิธีที่องค์กรกำหนดเอง
- Mutating request ไม่ใช้ Idempotency-Key (กันซ้ำด้วย unique constraint และ If-Match)
- การผูกตัวตนเป็นจุดเสี่ยง account takeover: ต้องได้รับ Security review ก่อน Production (เช่น นโยบายการยืนยันอีเมลของ Firebase ที่ใช้จริง)
- ยังไม่ตรวจ 200% zoom, keyboard/screen-reader ครบ, และ logout/cache isolation ของ FE; ยังไม่ได้ UAT โดยผู้ดูแลจริง
- Audit เก็บ identifier ไม่เก็บอีเมล/ชื่อ; นโยบาย retention ของ Audit ยังไม่ได้กำหนดเฉพาะเรื่องนี้
