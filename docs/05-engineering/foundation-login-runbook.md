# Foundation Login & Current User Runbook (คู่มือปฏิบัติการระบบยืนยันตัวตนระดับรากฐาน)

คู่มือนี้สรุปแนวทางปฏิบัติสำหรับผู้ดูแลระบบและนักพัฒนาในการดูแลรักษา, ดีบัก และทดสอบการทำงานของ Foundation Login and Current User Vertical Slice

## 1. การเริ่มต้นระบบในเครื่องพัฒนา (Local Stack Startup)

ระบบจำลองทั้งหมด (PostgreSQL 17 และ Firebase Auth Emulator) สามารถเริ่มต้นได้ผ่าน Docker Compose:

```bash
# 1. เริ่มต้น Container สำหรับ PostgreSQL 17 และ Firebase Emulator
docker compose -f deploy/compose.yml up -d

# 2. ตรวจสอบสถานะการทำงานและความพร้อมใช้งาน (Health Checks)
docker compose -f deploy/compose.yml ps

# 3. เตรียมข้อมูลผู้ใช้ทดสอบใน Firebase Emulator (พอร์ต 9099)
node scripts/seed-emulator-users.mjs

# 4. อัปเดตโครงสร้างฐานข้อมูลด้วย EF Core Migration
dotnet ef database update \
  --project backend/src/TanErp.Infrastructure \
  --startup-project backend/src/TanErp.Api \
  --connection "Host=localhost;Database=tan_erp;Username=postgres;Password=postgres"
```

## 2. การทดสอบ Migration Apply และ Rollback (Rehearsal)

เมื่อมีการเปลี่ยนแปลง Migration หรือต้องการทดสอบการย้อนกลับของ Schema:

```bash
# ตรวจสอบประวัติ Migration
dotnet ef migrations list --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api

# ทดสอบ Rollback กลับสู่จุดเริ่มต้น (0)
dotnet ef database update 0 \
  --project backend/src/TanErp.Infrastructure \
  --startup-project backend/src/TanErp.Api \
  --connection "Host=localhost;Database=tan_erp;Username=postgres;Password=postgres"

# ทำการ Re-apply กลับมาเป็นรุ่นล่าสุด
dotnet ef database update \
  --project backend/src/TanErp.Infrastructure \
  --startup-project backend/src/TanErp.Api \
  --connection "Host=localhost;Database=tan_erp;Username=postgres;Password=postgres"
```

## 3. การรัน Backend API และ Frontend สำหรับการพัฒนา

> 💡 **ทางลัดด้วย Make:** สามารถสั่งรันทั้งระบบพร้อมกันได้ง่าย ๆ ด้วย `make dev` (หรือแยกเฉพาะส่วนด้วย `make dev-backend` และ `make dev-frontend`)

### Backend API (โหมด Test พร้อม SeedTestData - พอร์ต 5005)
```bash
ASPNETCORE_ENVIRONMENT=Test \
SeedTestData=true \
FIREBASE_AUTH_EMULATOR_HOST=127.0.0.1:9099 \
Firebase__ProjectId=tan-erp-test-only \
ConnectionStrings__Database="Host=localhost;Database=tan_erp;Username=postgres;Password=postgres" \
dotnet run --no-launch-profile --project backend/src/TanErp.Api --urls http://localhost:5005
```

### Frontend Next.js Server (พอร์ต 3005)
```bash
cp frontend/.env.example frontend/.env.local
npm --prefix frontend run dev -- -p 3005
```

## 4. การตรวจสอบความพร้อมใช้งาน (Health & Readiness Checks)

- **PostgreSQL 17**: `docker exec tan-erp-postgres pg_isready -U postgres -d tan_erp`
- **Firebase Auth Emulator**: `curl -s http://127.0.0.1:9099/` (ต้องได้ `{"authEmulator":{"ready":true}}`)
- **Backend API Unauthorized Challenge**: `curl -s -i http://localhost:5005/api/v1/me` (ต้องได้ `401 Unauthorized` ในรูปแบบ RFC 9457 Problem Details)
- **Frontend Server**: `curl -s -i http://localhost:3005/th/login` (ต้องได้ `200 OK` พร้อม Login Form)

## 5. การวิเคราะห์ปัญหาด้วย Trace ID (Trace-ID Troubleshooting)

ทุก Error Response จาก Backend API จะมีฟิลด์ `traceId` ตามมาตรฐาน W3C Trace Context:

```json
{
  "type": "https://tan-erp.local/problems/authentication-invalid",
  "title": "โทเค็นไม่ถูกต้อง",
  "status": 401,
  "detail": "Token ไม่ถูกต้อง หมดอายุ หรือมาจาก Firebase Project อื่น",
  "instance": "/api/v1/me",
  "code": "AUTHENTICATION_INVALID",
  "traceId": "00-ea8808b2b025bb44bccd5bd8fe53c689-f4f2c6fdb208a1fd-00"
}
```

- ผู้ดูแลระบบสามารถนำค่า `traceId` ไปค้นหาใน Log ของ Backend API เพื่อดู Stack Trace และ Exception Context ได้ทันที
- Error Details ในระดับผู้ใช้จะถูกแปลตาม `Accept-Language` (`th` หรือ `en`) โดยไม่เปิดเผย Internal Exception หรือ SQL Statements ออกสู่ภายนอก

## 6. นโยบายความปลอดภัยของ Token และ Log (Token-Safe Logging Policy)

- **ห้ามบันทึก Bearer Token หรือ Password ลงใน Log เด็ดขาด:** ตัว Log Handler ถูกออกแบบให้ตรวจสอบและซ่อนค่า Header `Authorization`
- **ห้ามบันทึกข้อมูลลับหรือ Service Account Credentials ลงใน Git:** ใช้ `FIREBASE_AUTH_EMULATOR_HOST` สำหรับ Non-Production

## 7. ขอบเขตที่ยังไม่ได้เปิดใช้งาน (Explicit Out-of-Scope Notice)

สไลซ์นี้เป็น **Foundation Application Runtime** เท่านั้น:
- **ยังไม่มีหน้าจอจัดการสิทธิ์และสมาชิกภาพ (Membership / Role Admin UI):** การกำหนดสิทธิ์ดำเนินการผ่านฐานข้อมูล PostgreSQL
- **ยังไม่มีการเปิดใช้ Third-Party Social Login หรือ Enterprise SSO ใน Production:** มีเฉพาะ Email/Password บน Emulator สำหรับการทดสอบ
- **โมดูลธุรกิจอื่น (CRM, Survey, Estimation, Procurement ฯลฯ) ยังไม่ได้รับอนุญาตให้เปิดใช้งาน**
