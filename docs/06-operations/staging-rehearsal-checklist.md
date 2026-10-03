# Staging Rehearsal Checklist (เช็กลิสต์ซ้อมก่อนปล่อยบน Staging)

**สถานะ:** Draft — ยังไม่ได้ใช้ซ้อมจริง. เทคโนโลยี Hosting ยังไม่ถูกล็อก ([Deployment](deployment.md)) จึงเขียนแบบไม่ผูกผู้ให้บริการ. ใช้ร่วมกับ [Go/No-go Record Template](go-no-go-record-template.md); ผลที่ได้ต้องบันทึกลงสำเนาของ record นั้น

อ้างอิง: [Environments](environments.md), [Deployment](deployment.md), [Observability](observability.md), [Backup/Restore](backup-and-restore.md), [Incident Response](incident-response.md), [Administrator Recovery](administrator-recovery.md)

## ช่องว่างที่ต้องปิดก่อนซ้อมได้จริง (พบจากการตรวจ repo และ CI, 2026-10-03)

- **Health endpoint:** ยังไม่อยู่ใน `main` — เสนอไว้ใน PR #13 (`/health/live`, `/health/ready`); ข้อ 4 ของเช็กลิสต์พึ่ง PR นั้น
- **CI verify:** `.github/workflows/verify.yml` ไม่เคยรันสำเร็จ (ล้มที่ 0 วินาทีเพราะ workflow ไม่ถูกต้อง) — PR #12 แก้ให้เริ่มรันได้และแก้เทสต์ที่ล้มบน Linux; ขั้นหลังจากนั้น (frontend verify, migration, Playwright) ต้องดูผลรันจริง
- **ยังไม่มี build/publish ของ immutable artifact:** `verify.yml` ตรวจอย่างเดียว ไม่สร้างหรือเก็บ artifact และ `deploy/` มีเฉพาะ `compose.yml` สำหรับ local (PostgreSQL + Firebase Emulator) ยังไม่มีขั้นตอน promote ข้าม environment
- **ยังไม่ได้กำหนด Hosting, Production Firebase project, storage bucket, secrets และ RPO/RTO** — ต้องมีเจ้าของตัดสินใจก่อน

## 0. ก่อนเริ่ม

- [ ] Staging แยกจาก Production: database, Firebase project, storage bucket และ secret ([Environments](environments.md))
- [ ] ข้อมูลเป็น synthetic หรือ masked; ไม่คัดลอกข้อมูลส่วนบุคคลโดยไม่ Mask
- [ ] ไม่มีตัวแปรของ Emulator (`FIREBASE_AUTH_EMULATOR_HOST`) ใน staging/production (backend ปฏิเสธใน Production)
- [ ] ไม่เปิด `SeedTestData` และ demo seed; ไม่ใช้บัญชี/รหัสผ่าน `TEST_ONLY`
- [ ] ระบุ commit SHA, version และ artifact ID ที่จะซ้อม (ต้องเป็นตัวเดียวกับที่จะ promote)

## 1. Build และ deploy

- [ ] สร้าง artifact ครั้งเดียวจาก commit ที่ระบุ, บันทึก ID/checksum
- [ ] Security / dependency scan ผ่านหรือมีรายการยกเว้นที่อนุมัติ
- [ ] Deploy artifact เดิมขึ้น staging (ไม่ build ใหม่)
- [ ] Version ของ frontend, backend และ schema ตรวจได้จาก diagnostics

## 2. Migration

- [ ] Backup ก่อน migrate และยืนยันกู้ได้
- [ ] `dotnet ef database update` (หรือขั้นตอนที่ตกลงกัน) บนสำเนา sanitized legacy data — บันทึกเวลา, เวอร์ชันก่อน/หลัง, รายการที่ backfill (เช่น `AddIdentityAdministration` เติม `normalized_email` และ `row_version`)
- [ ] ตรวจจำนวนแถวและเอกสารสำคัญ (quotations, estimates, document counters) ก่อน/หลัง
- [ ] ซ้อม rollback หรือ forward-fix ของ migration นี้ และบันทึกว่าเลือกแบบใด

## 3. Bootstrap สิทธิ์ (production-like)

- [ ] สร้าง Role จริงและมอบ Permission รวมของใหม่: `users.read`, `users.manage`, `memberships.manage`, `roles.assign`, `roles.assign-approval`, `quotations.read`
- [ ] มีผู้ดูแลคนแรกที่ล็อกอินได้ และมีผู้ตรวจอิสระอย่างน้อยหนึ่งคนสำหรับ Approval
- [ ] ซ้อมเพิ่มผู้ใช้ → ล็อกอินครั้งแรกผูกบัญชีด้วยอีเมลที่ยืนยันแล้ว → ถอนสิทธิ์ (ผลทันที)
- [ ] ซ้อม [Administrator Recovery](administrator-recovery.md) บน staging ที่กู้จาก backup

## 4. Smoke test

- [ ] Liveness/readiness ตอบถูกต้อง (หลังสร้าง endpoint) และ readiness ล้มเมื่อ database ไม่พร้อม
- [ ] `GET /api/v1/me` ตอบ 401 เมื่อไม่มี token และ 200 พร้อม memberships เมื่อมี token ที่ถูกต้อง
- [ ] Journey หลัก: ลูกค้า → โอกาสขาย → Survey → Estimate → อนุมัติโดยผู้ตรวจอิสระ → ออกใบเสนอราคา → เปิด/พิมพ์เอกสาร → ลูกค้ายอมรับ
- [ ] เอกสารใบเสนอราคา: payload ไม่มี cost/margin, ข้อมูลลูกค้า ณ วันออก, th/en, พิมพ์เป็น PDF ในเบราว์เซอร์ที่ใช้งานจริง
- [ ] ข้ามองค์กร/สาขา ได้ 404 และไม่มีสิทธิ์ได้ 403
- [ ] Error ทุกตัวมี `traceId` ที่ผู้ใช้เห็นและค้นหาใน log ได้ โดยไม่มี token/PII ใน log

## 5. Observability และ Operations

- [ ] Logs มี trace id, error code และไม่เปิด PII; metrics และ traces ของ critical flow เห็นได้
- [ ] Alert อย่างน้อย: error rate, latency, readiness fail, database/connection, backup fail — มี owner, severity และ runbook
- [ ] Rate limit และ dependency failure (database, Firebase, storage) ถูกทดสอบและผู้ใช้เห็นข้อความกลาง + trace id
- [ ] Capacity sampling ตาม workload ที่ตกลง
- [ ] On-call, contact และ incident channel พร้อม

## 6. Backup / Restore drill

- [ ] ทำตาม [Backup/Restore](backup-and-restore.md): กู้ลงพื้นที่แยก, ตรวจ schema, จำนวนแถว/hash, migration history และ document counters
- [ ] กู้ file bytes, identity/config/secrets ได้ (ส่วนที่การซ้อม local วันที่ 2026-09-30 ยังไม่ครอบคลุม)
- [ ] วัดเวลากู้เทียบ RTO และระยะข้อมูลที่เสียเทียบ RPO ที่ Business อนุมัติ

## 7. ปิดการซ้อม

- [ ] บันทึกผลดิบ เวลา และผู้ปฏิบัติของทุกข้อลง Go/No-go Record
- [ ] สรุปความเสี่ยงที่เหลือ เงื่อนไขหยุด rollout และการตัดสินใจ rollback / forward-fix
- [ ] ผู้มีอำนาจตัดสินใจ `Go` หรือ `No-go` (ไม่ใช่ผู้พัฒนา)
