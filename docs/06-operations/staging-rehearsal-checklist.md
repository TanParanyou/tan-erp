# Staging Rehearsal Checklist (เช็กลิสต์ซ้อมก่อนปล่อยบน Staging)

**สถานะ:** Ready for Rehearsal — มี Production Deployment Artifacts ([`deploy/backend.Dockerfile`](file:///Users/syaco/Documents/development/tan-erp/deploy/backend.Dockerfile), [`deploy/frontend.Dockerfile`](file:///Users/syaco/Documents/development/tan-erp/deploy/frontend.Dockerfile), [`deploy/compose.production.yml`](file:///Users/syaco/Documents/development/tan-erp/deploy/compose.production.yml), [`deploy/.env.production.example`](file:///Users/syaco/Documents/development/tan-erp/deploy/.env.production.example)) และ Health Probes สมบูรณ์แล้ว. ใช้ร่วมกับ [Go/No-go Record Template](go-no-go-record-template.md); ผลที่ได้ต้องบันทึกลงสำเนาของ record นั้น

อ้างอิง: [Environments](environments.md), [Deployment](deployment.md), [Observability](observability.md), [Backup/Restore](backup-and-restore.md), [Incident Response](incident-response.md), [Administrator Recovery](administrator-recovery.md)

## สถานะความพร้อมและสิ่งที่ปิดช่องว่างแล้ว (อัปเดต 2026-11-06)

- **Health endpoints (เสร็จสมบูรณ์):** `/health/live` และ `/health/ready` อยู่บน `main` และทำงานสมบูรณ์แล้วใน `backend/src/TanErp.Api/Program.cs` ควบคู่กับ `TanErp.Api.Health.DatabaseHealthCheck` (ตรวจสอบความพร้อมของ PostgreSQL)
- **Production Artifacts (เสร็จสมบูรณ์):** มี Dockerfile มาตรฐานสำหรับการสร้าง Immutable Container Images ครบถ้วน:
  - `deploy/backend.Dockerfile`: Multi-stage .NET 10 Web API พร้อม QuestPDF Skia dependencies (`libfontconfig1`), persistent storage path `/var/lib/tan-erp/storage`, non-root user `app` บนพอร์ต `5179`
  - `deploy/frontend.Dockerfile`: Multi-stage Next.js 16 (React 19) พร้อม unprivileged user `nextjs` บนพอร์ต `3000`
  - `deploy/compose.production.yml`: Orchestration สำหรับ PostgreSQL 16, Backend API และ Frontend พร้อม Service Healthchecks
  - `deploy/.env.production.example`: ตัวอย่างคอนฟิกความปลอดภัยสูงแบบไม่มี hardcoded secrets
- **สิ่งที่ต้องระบุในการซ้อมแต่ละรอบ:**
  - กำหนด Hosting Target หรือ Staging Server (VM/Kubernetes/Docker Host)
  - ตั้งค่า Secret จริง (PostgreSQL password, JWT Signing Key ≥ 32 chars, AES Encryption Key ≥ 32 chars) ใน `.env.production`

---

## 0. ก่อนเริ่ม

- [ ] Staging แยกจาก Production: database, storage volume/bucket และ secret ([Environments](environments.md))
- [ ] ข้อมูลเป็น synthetic หรือ masked; ไม่คัดลอกข้อมูลส่วนบุคคลโดยไม่ Mask
- [ ] ไม่มีตัวแปรของ Emulator (`FIREBASE_AUTH_EMULATOR_HOST`) ใน staging/production (backend ปฏิเสธใน Production)
- [ ] ไม่เปิด `SeedTestData` และ demo seed; ไม่ใช้บัญชี/รหัสผ่าน `TEST_ONLY`
- [ ] ระบุ commit SHA, version และ container image digest/tag ที่จะซ้อม (ต้องเป็นตัวเดียวกับที่จะ promote สู่ production)

## 1. Build และ deploy

- [ ] สร้าง immutable container artifact ครั้งเดียวจาก commit ที่ระบุ (`deploy/backend.Dockerfile`, `deploy/frontend.Dockerfile`), บันทึก Image ID/digest
- [ ] Security / dependency scan ผ่านหรือมีรายการยกเว้นที่อนุมัติ
- [ ] Deploy artifact เดิมขึ้น staging ผ่าน `deploy/compose.production.yml` (ไม่ build ใหม่)
- [ ] Version ของ frontend, backend และ schema ตรวจได้จาก diagnostics และ health endpoints

## 2. Migration

- [ ] Backup ก่อน migrate และยืนยันกู้ได้
- [ ] `dotnet ef database update` (หรือ migration runner container) บนสำเนา sanitized legacy data — บันทึกเวลา, เวอร์ชันก่อน/หลัง, รายการที่ backfill (เช่น `AddIdentityAdministration` เติม `normalized_email` และ `row_version`)
- [ ] ตรวจจำนวนแถวและเอกสารสำคัญ (quotations, estimates, document counters) ก่อน/หลัง
- [ ] ซ้อม rollback หรือ forward-fix ของ migration นี้ และบันทึกว่าเลือกแบบใด

## 3. Bootstrap สิทธิ์ (production-like)

- [ ] สร้าง Role จริงและมอบ Permission รวมของใหม่: `users.read`, `users.manage`, `memberships.manage`, `roles.assign`, `roles.assign-approval`, `quotations.read`
- [ ] มีผู้ดูแลคนแรกที่ล็อกอินได้ และมีผู้ตรวจอิสระอย่างน้อยหนึ่งคนสำหรับ Approval
- [ ] ซ้อมเพิ่มผู้ใช้ → ล็อกอินครั้งแรกผูกบัญชีด้วยอีเมลที่ยืนยันแล้ว → ถอนสิทธิ์ (ผลทันที)
- [ ] ซ้อม [Administrator Recovery](administrator-recovery.md) บน staging ที่กู้จาก backup

## 4. Smoke test

- [ ] Liveness (`/health/live`) และ Readiness (`/health/ready`) ตอบถูกต้อง `200 OK` (Healthy) และ readiness ล้มเหลว `503 Service Unavailable` เมื่อ database ไม่พร้อม (พัฒนาเสร็จสมบูรณ์แล้วใน `Program.cs`)
- [ ] `GET /api/v1/me` ตอบ 401 เมื่อไม่มี token และ 200 พร้อม memberships เมื่อมี token ที่ถูกต้อง
- [ ] Journey หลัก: ลูกค้า → โอกาสขาย → Survey → Estimate → อนุมัติโดยผู้ตรวจอิสระ → ออกใบเสนอราคา → เปิด/พิมพ์เอกสาร PDF (QuestPDF) → ลูกค้ายอมรับ (OTP Email/SMS)
- [ ] เอกสารใบเสนอราคา: payload ไม่มี cost/margin, ข้อมูลลูกค้า ณ วันออก, โลโก้และสีแบรนด์องค์กรแสดงผลถูกต้อง, พิมพ์เป็น PDF จาก backend ได้ไบต์และ hash คงที่
- [ ] ข้ามองค์กร/สาขา ได้ 404 และไม่มีสิทธิ์ได้ 403
- [ ] Error ทุกตัวมี `traceId` ที่ผู้ใช้เห็นและค้นหาใน log ได้ โดยไม่มี token/PII ใน log

## 5. Observability และ Operations

- [ ] Logs มี trace id, error code และไม่เปิด PII; metrics และ traces ของ critical flow เห็นได้
- [ ] Alert อย่างน้อย: error rate, latency, readiness fail, database/connection, backup fail — มี owner, severity และ runbook
- [ ] Rate limit และ dependency failure (database, storage, SMS gateway) ถูกทดสอบและผู้ใช้เห็นข้อความกลาง + trace id
- [ ] Capacity sampling ตาม workload ที่ตกลง
- [ ] On-call, contact และ incident channel พร้อม

## 6. Backup / Restore drill

- [ ] ทำตาม [Backup/Restore](backup-and-restore.md): กู้ลงพื้นที่แยก, ตรวจ schema, จำนวนแถว/hash, migration history และ document counters
- [ ] กู้ file bytes ใน persistent storage (`/var/lib/tan-erp/storage`), identity/config/secrets ได้
- [ ] วัดเวลากู้เทียบ RTO และระยะข้อมูลที่เสียเทียบ RPO ที่ Business อนุมัติ

## 7. ปิดการซ้อม

- [ ] บันทึกผลดิบ เวลา และผู้ปฏิบัติของทุกข้อลง Go/No-go Record
- [ ] สรุปความเสี่ยงที่เหลือ เงื่อนไขหยุด rollout และการตัดสินใจ rollback / forward-fix
- [ ] ผู้มีอำนาจตัดสินใจ `Go` หรือ `No-go` (ไม่ใช่ผู้พัฒนา)
