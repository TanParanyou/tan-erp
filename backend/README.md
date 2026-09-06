# TanErp Backend

Backend สำหรับระบบ Project ERP (`tan-erp`) พัฒนาด้วย .NET 10 LTS (`net10.0`), ASP.NET Core, EF Core 10, Npgsql และ PostgreSQL 17

## โครงสร้างโปรเจกต์ (Clean Architecture)

- `src/TanErp.Domain` — Domain Entities, Value Objects, Domain Exceptions
- `src/TanErp.Application` — Feature Folders, Use Case Handlers, Ports & Abstractions
- `src/TanErp.Infrastructure` — EF Core DbContext, Configurations, Migrations, Firebase Auth Adapter
- `src/TanErp.Api` — Controllers, Auth Handlers, OpenAPI, Error Localization
- `tests/TanErp.UnitTests` — Unit Tests สำหรับ Domain และ Application
- `tests/TanErp.IntegrationTests` — Integration Tests ด้วย Testcontainers.PostgreSql และ WebApplicationFactory
- `tests/TanErp.ArchitectureTests` — Architecture Fitness Tests ด้วย ArchUnitNET

## คำสั่งสำหรับพัฒนาและทดสอบ (Commands)

```bash
# กู้คืน Package
dotnet restore backend/TanErp.slnx

# บิลด์ Solution
dotnet build backend/TanErp.slnx --no-restore

# รันชุดทดสอบทั้งหมด
dotnet test backend/TanErp.slnx --no-build

# รัน Architecture Tests
dotnet test backend/tests/TanErp.ArchitectureTests

# รัน EF Core Migration (เมื่อต้องการ Apply สู่ Database ภายนอก)
dotnet ef database update --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```

## การรันสภาพแวดล้อมจำลองภายในเครื่อง (Local Stack & Acceptance)

```bash
# 1. สตาร์ต PostgreSQL 17 Container
docker compose -f deploy/compose.yml up -d

# 2. สตาร์ต Firebase Auth Emulator (พอร์ต 9099)
npx firebase emulators:start --only auth --project tan-erp-test-only

# 3. เตรียมข้อมูลผู้ใช้ทดสอบใน Firebase Emulator
node scripts/seed-emulator-users.mjs

# 4. ทำ Database Migration (หรือรัน 'make db-migrate')
dotnet ef database update --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api

# 5. สตาร์ต Backend API ด้วยโหมด Test (พอร์ต 5005 หรือรัน 'make dev-backend')
ASPNETCORE_ENVIRONMENT=Test \
SeedTestData=true \
FIREBASE_AUTH_EMULATOR_HOST=127.0.0.1:9099 \
Firebase__ProjectId=tan-erp-test-only \
ConnectionStrings__Database="Host=localhost;Database=tan_erp;Username=postgres;Password=postgres" \
dotnet run --project backend/src/TanErp.Api --urls http://localhost:5005

# 6. หยุดการทำงานของ Container โดยไม่ลบ Volume (หรือรัน 'make stop')
docker compose -f deploy/compose.yml stop
```

## ตัวแปรสภาพแวดล้อมที่จำเป็น (Environment Variables)

*หมายเหตุ: ระบุเฉพาะชื่อตัวแปร ห้ามใส่ค่าจริงหรือข้อมูลลับลงในที่นี้*

- `ConnectionStrings__Database` — Connection string ไปยัง PostgreSQL 17
- `Firebase__ProjectId` — Firebase Project ID (เช่น `tan-erp-test-only`)
- `FIREBASE_AUTH_EMULATOR_HOST` — Host และ Port สำหรับ Firebase Auth Emulator (เฉพาะ Non-Production เช่น `127.0.0.1:9099`)
- `GOOGLE_APPLICATION_CREDENTIALS` — พาธไปยัง Firebase service account JSON (เมื่ออยู่นอกโหมด Emulator)
- `SeedTestData` — แฟล็กเปิดใช้ข้อมูลสังเคราะห์ `TEST_ONLY` (เฉพาะ Environment `Test` เท่านั้น หากเปิดใน Production จะถูกปฏิเสธทันที)
