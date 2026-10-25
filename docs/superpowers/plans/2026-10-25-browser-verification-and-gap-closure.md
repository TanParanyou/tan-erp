# Browser Verification & Gap Closure Plan (2026-10-25)

**สถานะ:** Draft — รอเจ้าของงานอนุมัติขอบเขตก่อนเริ่ม

**เป้าหมาย:** ปิดข้อค้าง "ยังไม่ตรวจในเบราว์เซอร์" ของ slice G-02 ถึง G-21 ใน [Implementation Roadmap](../../00-overview/implementation-roadmap.md) แล้วจึงปิดช่องว่าง Finance. ไม่เพิ่มโมดูลใหม่ และไม่เปลี่ยน Expected Result เพื่อให้ผ่าน.

## กติการ่วม

- **ไม่เขียนและไม่รัน test ใหม่** (เจ้าของงานรัน test เอง); ตรวจเฉพาะ `dotnet build`, `npx tsc --noEmit`, `npx eslint <ไฟล์>`, `npm run build`.
- ทุกการแก้ต้องเคารพ `AGENTS.md`: i18n th/en ครบ, ไม่ใช้ `any`, Reuse First, Minimal Blast Radius.
- พบ defect ให้บันทึกก่อนแก้: slice, ขั้นตอน reproduce, ผลที่เห็น, ผลที่ควรเป็น. ห้ามเดากฎธุรกิจ; ถ้าไม่ชัดให้หยุดถามเจ้าของงาน.
- รายงานผลลง `docs/05-engineering/<slice>-verification.md` เดิม ในหัวข้อ "Browser check 2026-10-25" ระบุว่าตรวจอะไรและผลเป็นอย่างไร.

## การแบ่งโมเดล

| โมเดล | งาน |
| --- | --- |
| **Haiku** | เดินตรวจในเบราว์เซอร์ตาม checklist, บันทึก defect, แก้ defect ระดับ UI/i18n/placeholder/layout, อัปเดตเอกสาร verification, รัน build/lint |
| **Sonnet** | วินิจฉัยและแก้ defect ที่เกี่ยวกับ logic: สิทธิ์/ขอบเขต Organization, maker–checker, Approval Authority, OTP, hash/PDF, snapshot conversion, concurrency; รีวิว diff ของ Haiku ที่แตะ backend; Credit Note |

**เกณฑ์ส่งต่อ:** Haiku แก้เองได้เฉพาะไฟล์ใน `frontend/` ที่เป็นข้อความ, i18n, class/layout, placeholder. หากต้นเหตุอยู่ที่ backend, สิทธิ์, สถานะเอกสาร, การคำนวณ หรือ contract ให้ Haiku หยุดแล้วส่ง defect ให้ Sonnet.

## Task A — Browser check (Haiku ทั้งหมด)

เตรียม: เริ่ม backend + frontend ด้วย `preview_start`, ใช้ข้อมูล seed/demo, ล็อกอินด้วยบัญชีทดสอบจาก seed.

| Task | Slice | สิ่งที่เดินตรวจ | หมายเหตุ |
| --- | --- | --- | --- |
| A1 | G-02 / G-21 | กระดิ่งแจ้งเตือน: เอกสารรออนุมัติ, Membership ใกล้หมดอายุ | ไม่มี email/push |
| A2 | G-03a | Organization profile, Branch CRUD | |
| A3 | G-03b | สร้าง/แก้/ปิดใช้ Role, ตัวเลือก Permission แบบ feature-first | maker–checker: ส่ง defect ให้ Sonnet |
| A4 | G-03c | หน้า Approval Limits | กฎบังคับตอนอนุมัติ: ส่ง defect ให้ Sonnet |
| A5 | G-04 | Quotation Preview th/en, ดาวน์โหลดและเปิด PDF, ยืนยัน hash หน้าลูกค้า | ต้องเปิดไฟล์ PDF ดูจริง |
| A6 | G-05 | External Acceptance + OTP (`AcceptanceOtp:Required=true` เฉพาะ local) | ไม่ส่งอีเมลจริง |
| A7 | G-06 | Item import `upsert`, ต้นทุน Draft เข้า maker–checker | |
| A8 | G-07 | Survey template v2/v4 และ area evidence | |
| A9 | G-08 / G-09 | Estimate unit conversion (ปิดอยู่), Quick Estimate → ร่าง Official Estimate, Branch Rate Override | G-08 แค่ยืนยันว่าปิดจริง |
| A10 | ทุกหน้าข้างต้น | แท็บเล็ต, zoom 200%, ปุ่ม icon มี Tooltip + aria-label, ช่องกรอกมี placeholder | |

ผลลัพธ์ของ Task A: ตาราง defect หนึ่งตารางต่อ slice พร้อมป้ายผู้รับ (Haiku / Sonnet).

## Task B — แก้ defect

- **B1 (Haiku):** defect ระดับ UI/i18n/layout ทั้งหมด; รัน `tsc`, `eslint` เฉพาะไฟล์ที่แก้.
- **B2 (Sonnet):** defect ระดับ logic; แก้ที่ต้นเหตุครั้งเดียวในโค้ดกลาง (grep ผู้เรียกทุกจุดก่อนแก้); รัน `dotnet build`.
- **B3 (Sonnet รีวิว):** ตรวจ diff ทุกส่วนที่แตะ backend, สิทธิ์ หรือ migration ก่อน commit.

## Task C — Operations gate (เตรียมเอกสารโดย Haiku, ตัดสินโดยเจ้าของงาน)

- C1 (Haiku): ร่าง checklist ซ้อม restore บน staging (database + file bytes + identity + config/secrets) จาก [Backup/Restore](../../06-operations/backup-and-restore.md).
- C2 (Haiku): ร่าง checklist ย้ายข้อมูลจาก sanitized legacy copy.
- ทั้งสองข้อต้องให้เจ้าของงานรันบน staging และอนุมัติ RPO/RTO เอง.

## Task D — Credit Note (Sonnet, **blocked**)

ต้องได้กฎจาก Finance ก่อน (วงเงินอนุมัติ, ผลต่อใบแจ้งหนี้และลูกหนี้, ภาษี) และเจ้าของงานอนุมัติ scope ใหม่. ยังไม่เริ่มจนกว่าจะปลด block. เมื่อเริ่ม: เขียน plan ย่อยของ slice ตามกระบวนการ Full process และใช้ hook G-15 ที่มีอยู่.

## งานที่เลื่อน (รอ business)

snapshot-mode Calculation Policy API, allowlist ฟิลด์ใบเสนอราคา, license QuestPDF, ราคา Quick Estimate จริง, lot/serial, routing/capacity, safety stock.

## Definition of Done

- ทุกแถวใน Task A มีผลตรวจบันทึกในเอกสาร verification ของ slice.
- defect ที่พบถูกแก้หรือบันทึกเป็นข้อเลื่อนพร้อมเหตุผล.
- `dotnet build`, `npm run build`, `npm run lint` ผ่านครั้งเดียวก่อนส่งมอบ (Gate เต็มตาม `AGENTS.md` หมวด 8); test ให้เจ้าของงานรัน.
- Roadmap ปรับจาก "ยังไม่ตรวจในเบราว์เซอร์" เป็นผลจริงเฉพาะ slice ที่ตรวจแล้ว; ไม่เปลี่ยนสถานะ UAT.

## ผลการดำเนินการ 2026-10-25

**Task A ที่ตรวจแล้ว (Haiku, บัญชี foundation-user, local emulator):** A1 (เฉพาะ empty state), A2, A3, A4 (list/filter), A7 (ถึง Preview) ผ่าน; ไม่พบ HTTP 4xx/5xx. A8 ผ่านบางส่วน (UI ไม่แสดงเลขเวอร์ชัน template, ใบสำรวจที่ตรวจไม่มีพื้นที่ จึงไม่เห็น evidence). A10 ตรวจ tablet 768px บางหน้า; ยังไม่ทดสอบ zoom 200% จริงและ tooltip.

**Defect และสถานะ**

| # | สถานะ |
| --- | --- |
| D1 PageHeader บีบ title | แก้แล้ว (`PageHeader.tsx`); ตรวจ 4 หน้าจาก ~63 ที่ใช้ร่วม |
| D2 ตาราง survey ดูเหมือนถูกตัด | ปิดเป็นไม่แก้ (DataTable มี horizontal scroll อยู่แล้ว) |
| D3 การ์ดลูกค้าแสดงรหัสแนวตั้ง | แก้แล้ว (`opportunity-detail.tsx`); คอลัมน์ค่ายังแคบที่ ~800px เพราะ `.erp-dl` dt 140px (ไม่แตะของกลาง) |
| D4 ป้ายสิทธิ์ภาษาไทยไม่ครบ | แก้แล้ว: เติม action 103 key, resource 4 key (th/en ตรงกัน) และ `getResourceLabel` แปลง `.` เป็น `_`; ตรวจในเบราว์เซอร์แล้วว่า `customers.credit` / `projects.change-orders` แสดงเป็น "เครดิตลูกค้า (Customer Credit)" / "ใบสั่งเปลี่ยนแปลงงาน (Change Orders)" ที่ /th/settings/roles/create |
| D5 Role ว่างบันทึกได้ | รอเจ้าของงาน |

**ค้าง:** A5, A6, A9 (backend :5005 ไม่เปิด demo flags); `npm run build` (ไม่รันขณะ dev server ใช้ `.next`); test (เจ้าของงานรัน); UAT ทุก slice ยังไม่ผ่าน.

**Verification ที่รัน:** `npx tsc --noEmit` ผ่าน, `npm run lint` ผ่าน, key parity th/en ใต้ `roleAdmin.editor.permissions` ตรงกัน.

**ข้อมูลทดสอบที่ต้องจัดการ:** สาขา `ZZTB01`, Role "ZZ-TEST-ROLE EDIT" (inactive) และไฟล์ CSV เทมเพลตใน Downloads.

### รอบที่สอง (backend เปิด demo flags ด้วย `make dev`)

| รายการ | ผล |
| --- | --- |
| A5 Preview th/en | ผ่าน: ไม่แสดงต้นทุน/margin |
| A5 / G-04 Amend → PDF ฝั่งเซิร์ฟเวอร์ | ผ่านบางส่วน: Amend QT-2026-0008 → QT-2026-0009 มีปุ่มดาวน์โหลด, PDF 30,332 bytes, SHA-256 `92c4cb5d…bc46`; ตรวจ PDF ด้วยสายตาแล้ว (ดึงจาก `GET /api/v1/quotations/{id}/document`, SHA-256 ตรงกับที่แสดงบนหน้า): สระ/วรรณยุกต์ไทยเรียงถูก, 1 หน้า A4, สองภาษา, ไม่มีต้นทุน/margin, VAT 5,000.00 และยอดสุทธิ 76,428.57 ตรงกับหน้า preview |
| A6 ลิงก์ acceptance + หน้าลูกค้า | ผ่าน: หน้าลูกค้าแสดง SHA-256 ตรงกับ PDF, placeholder th/en ถูกต้อง, เพิกถอนลิงก์แล้วใช้ไม่ได้; ลิงก์เก่าของฉบับที่ถูกแทนที่ตรวจ runtime ไม่ได้จาก UI (สัญญา [External Acceptance API](../../03-contracts/external-acceptance-api-contract.md) ระบุว่าใช้ไม่ได้อัตโนมัติเมื่อใบไม่ใช่ `issued` และตอบ 404 แบบเดียวกัน; ยังไม่ได้ยืนยันด้วย token จริง); ไม่ได้ทดสอบ OTP (ปิดอยู่) |
| A9 Quick Estimate: error 422 | ผ่าน: alert แสดงแล้ว |
| A9 แปลงเป็นร่าง Official Estimate | BLOCKED: dialog ต้องการ UUID ของ survey revision, UI ไม่แสดง UUID และ survey demo ทั้ง 4 ยังไม่พร้อมแปลง (backend ตอบ 400) |

ข้อสังเกต UI: dialog แปลงแสดงเพียง "ดำเนินการไม่สำเร็จ" เมื่อ 400 และ placeholder `SRV-2026-0001` ทำให้เข้าใจว่าใส่รหัสใบสำรวจได้ ทั้งที่ต้องเป็น UUID.

ข้อมูล demo ที่สร้าง: QT-2026-0009 (id `31012215-5f87-4654-bd2c-0799430ab020`, แทน QT-2026-0008), ลิงก์ acceptance ของ QT-2026-0009 (เพิกถอนแล้ว), QE-2026-0002 (id `c08e2863-a001-4252-8089-d51d8f911675`).

### รอบที่สาม: build และ OTP (API-level)

- **`npm run build` (frontend):** ผ่าน (exit 0) ที่ commit `c4f517b` รันใน git worktree แยกเพื่อไม่ทับ `.next` ของ dev server (ต้อง clone `node_modules` ด้วย `cp -Rc` เพราะ Turbopack ไม่รับ symlink)
- **OTP (G-05):** ทดสอบผ่าน API บน backend ชั่วคราว (`AcceptanceOtp:Required=true`, SMTP → Mailpit) ไม่ได้ทดสอบหน้าลูกค้าใน UI. ผล: สร้างลิงก์ไม่ใส่อีเมล → 422 `ACCEPTANCE_RECIPIENT_EMAIL_REQUIRED`; ใส่อีเมล → หน้า public มี `otpRequired=true`, `otpChannels=["email"]`; ขอรหัส (`POST …/otp {"channel":"email"}`) → 202, อีเมลสองภาษาถึงผู้รับ; รหัสผิด → 422 `ACCEPTANCE_OTP_INVALID`; รหัสถูก → 200 + `verificationToken`; ใช้รหัสซ้ำ → 410 `ACCEPTANCE_OTP_EXPIRED`; `accept` โดยไม่มี OTP token → 422 `ACCEPTANCE_OTP_REQUIRED`; ไม่ได้กดยอมรับจริง; ลิงก์ทดสอบถูกเพิกถอนแล้ว (404) ยังไม่ได้ทดสอบ lockout/rate limit และ UI ขั้นกรอก OTP
- **การแปล Quick Estimate:** ยังไม่ทำ ต้องตัดสินใจ: API ใบสำรวจมีเฉพาะต่อ opportunity (`/opportunities/{id}/surveys`) ไม่มี endpoint รายการ revision ที่พร้อมใช้ทั้งองค์กร ช่องเลือกจึงต้องเพิ่ม endpoint ที่ backend (หรือผูก Quick Estimate กับ Opportunity)
