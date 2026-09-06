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

## ตัวแปรสภาพแวดล้อมที่จำเป็น (Environment Variables)

*หมายเหตุ: ระบุเฉพาะชื่อตัวแปร ห้ามใส่ค่าจริงหรือข้อมูลลับลงในที่นี้*

- `ConnectionStrings__Database` — Connection string ไปยัง PostgreSQL 17
- `Firebase__ProjectId` — Firebase Project ID
- `FIREBASE_AUTH_EMULATOR_HOST` — Host และ Port สำหรับ Firebase Auth Emulator (เฉพาะ Non-Production เช่น `127.0.0.1:9099`)
- `GOOGLE_APPLICATION_CREDENTIALS` — พาธไปยัง Firebase service account JSON (เมื่ออยู่นอกโหมด Emulator)
- `SeedTestData` — แฟล็กเปิดใช้ข้อมูลสังเคราะห์ `TEST_ONLY` (เฉพาะ Environment `Test`)
