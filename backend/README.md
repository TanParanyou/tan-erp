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
# 1. เตรียม frontend local environment และเปิด PostgreSQL + Auth Emulator
make dev-env
docker compose -f deploy/compose.yml up -d postgres firebase-emulator

# 2. เตรียมบัญชี TEST_ONLY รวม independent Estimate reviewer ใน Auth Emulator
make db-seed

# 3. เปิด Test API (:5005); migration และ demo seed ทำตอนเริ่ม API
SeedDedicatedEstimateReviewer=true make dev-backend-demo

# 4. เปิด terminal ใหม่: dev frontend (:3005)
npm --prefix frontend run dev -- -p 3005

# 5. เปิด terminal ใหม่: ทดสอบ Estimate → Approval → Quotation → Revision
npm --prefix frontend run test:e2e -- official-estimate.spec.ts --project=chromium

# 6. หยุด API/frontend ด้วย Ctrl+C; หยุด containers โดยไม่ลบ Volume
docker compose -f deploy/compose.yml stop
```

Compose เปิด Auth Emulator อยู่แล้ว จึงไม่ต้องเปิด Firebase CLI ซ้ำที่พอร์ต 9099. Browser journey ที่ใช้ Emulator ต้องรันกับ dev frontend: `firebase-client.ts` ไม่เชื่อม Emulator ใน production build ตาม safety guard. การผ่าน production build เป็น code gate แยกจากการล็อกอินด้วยบัญชี TEST_ONLY.

## ตัวแปรสภาพแวดล้อมที่จำเป็น (Environment Variables)

*หมายเหตุ: ระบุเฉพาะชื่อตัวแปร ห้ามใส่ค่าจริงหรือข้อมูลลับลงในที่นี้*

- `ConnectionStrings__Database` — Connection string ไปยัง PostgreSQL 17
- `Firebase__ProjectId` — Firebase Project ID (เช่น `tan-erp-test-only`)
- `FIREBASE_AUTH_EMULATOR_HOST` — Host และ Port สำหรับ Firebase Auth Emulator (เฉพาะ Non-Production เช่น `127.0.0.1:9099`)
- `GOOGLE_APPLICATION_CREDENTIALS` — พาธไปยัง Firebase service account JSON (เมื่ออยู่นอกโหมด Emulator)
- `SeedTestData` — แฟล็กเปิดใช้ข้อมูลสังเคราะห์ `TEST_ONLY` (เฉพาะ Environment `Test` เท่านั้น หากเปิดใน Production จะถูกปฏิเสธทันที)
- `SeedEstimateDemoData` และ `SeedItemCatalogDemoData` — เปิดชุดข้อมูล Estimate และ Item Catalog สำหรับ Demo เพิ่มเติม; ใช้ `make dev-backend-demo` ซึ่งจำกัดการทำงานไว้ที่ Test environment และข้อมูลทุกชุดเป็น `TEST_ONLY` ไม่ใช่ราคา/นโยบาย Production
